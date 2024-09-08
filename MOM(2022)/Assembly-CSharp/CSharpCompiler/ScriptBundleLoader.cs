using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSharpCompiler
{
    public class ScriptBundleLoader
    {
        public class ScriptBundle
        {
            private Assembly assembly;

            private IEnumerable<string> filePaths;

            private List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

            private List<object> instances = new List<object>();

            private ScriptBundleLoader manager;

            private string[] assemblyReferences;

            public ScriptBundle(ScriptBundleLoader manager, IEnumerable<string> filePaths)
            {
                this.filePaths = filePaths.Select((string x) => Path.GetFullPath(x));
                this.manager = manager;
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<string> list = new List<string>(assemblies.Length);
                Assembly[] array = assemblies;
                foreach (Assembly assembly in array)
                {
                    try
                    {
                        string location = assembly.Location;
                        list.Add(location);
                    }
                    catch (Exception)
                    {
                    }
                }
                this.assemblyReferences = list.ToArray();
                manager.logWriter.WriteLine("loading " + Environment.NewLine + string.Join(Environment.NewLine, filePaths.ToArray()));
                this.CompileFiles();
                this.CreateFileWatchers();
                this.CreateNewInstances();
            }

            private void CompileFiles()
            {
                this.filePaths = this.filePaths.Where((string x) => File.Exists(x)).ToArray();
                CompilerParameters compilerParameters = new CompilerParameters();
                compilerParameters.GenerateExecutable = false;
                compilerParameters.GenerateInMemory = true;
                compilerParameters.ReferencedAssemblies.AddRange(this.assemblyReferences);
                CompilerResults compilerResults = new CodeCompiler().CompileAssemblyFromFileBatch(compilerParameters, this.filePaths.ToArray());
                foreach (object error in compilerResults.Errors)
                {
                    Debug.LogError(error);
                }
                this.assembly = compilerResults.CompiledAssembly;
            }

            private void CreateFileWatchers()
            {
                foreach (string filePath in this.filePaths)
                {
                    FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
                    this.fileSystemWatchers.Add(fileSystemWatcher);
                    fileSystemWatcher.Path = Path.GetDirectoryName(filePath);
                    fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
                    fileSystemWatcher.Filter = Path.GetFileName(filePath);
                    fileSystemWatcher.Changed += delegate
                    {
                        this.Reload();
                    };
                    fileSystemWatcher.Deleted += delegate
                    {
                        this.Reload();
                    };
                    fileSystemWatcher.Renamed += delegate(object o, RenamedEventArgs a)
                    {
                        this.filePaths = this.filePaths.Select((string x) => (x == a.OldFullPath) ? a.FullPath : x);
                        this.Reload(recreateWatchers: true);
                    };
                    fileSystemWatcher.SynchronizingObject = this.manager.synchronizedInvoke;
                    fileSystemWatcher.EnableRaisingEvents = true;
                }
            }

            private void StopFileWatchers()
            {
                foreach (FileSystemWatcher fileSystemWatcher in this.fileSystemWatchers)
                {
                    fileSystemWatcher.EnableRaisingEvents = false;
                    fileSystemWatcher.Dispose();
                }
                this.fileSystemWatchers.Clear();
            }

            private void Reload(bool recreateWatchers = false)
            {
                this.manager.logWriter.WriteLine("reloading " + string.Join(", ", this.filePaths.ToArray()));
                this.StopInstances();
                this.CompileFiles();
                this.CreateNewInstances();
                if (recreateWatchers)
                {
                    this.StopFileWatchers();
                    this.CreateFileWatchers();
                }
            }

            private void CreateNewInstances()
            {
                if (this.assembly == null)
                {
                    return;
                }
                Type[] types = this.assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public);
                    foreach (MethodInfo methodInfo in methods)
                    {
                        ScriptLibrary.SetScript(methodInfo.Name, methodInfo);
                    }
                }
            }

            private void StopInstances()
            {
                foreach (object instance in this.instances)
                {
                    this.manager.synchronizedInvoke.Invoke((Action)delegate
                    {
                        this.manager.destroyInstance(instance);
                    }, null);
                }
                this.instances.Clear();
            }
        }

        public Func<Type, object> createInstance = (Type type) => Activator.CreateInstance(type);

        public Action<object> destroyInstance = delegate
        {
        };

        public TextWriter logWriter = Console.Out;

        private ISynchronizeInvoke synchronizedInvoke;

        private List<ScriptBundle> allFilesBundle = new List<ScriptBundle>();

        public ScriptBundleLoader(ISynchronizeInvoke synchronizedInvoke)
        {
            this.synchronizedInvoke = synchronizedInvoke;
        }

        public ScriptBundle LoadAndWatchScriptsBundle(IEnumerable<string> fileSources)
        {
            ScriptBundle scriptBundle = new ScriptBundle(this, fileSources);
            this.allFilesBundle.Add(scriptBundle);
            return scriptBundle;
        }
    }
}
