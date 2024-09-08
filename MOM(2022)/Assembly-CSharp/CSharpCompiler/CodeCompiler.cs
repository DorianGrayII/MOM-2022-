using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using Mono.CSharp;

namespace CSharpCompiler
{
    public class CodeCompiler : ICodeCompiler
    {
        private static long assemblyCounter;

        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            return this.CompileAssemblyFromDomBatch(options, new CodeCompileUnit[1] { compilationUnit });
        }

        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            try
            {
                return this.CompileFromDomBatch(options, ea);
            }
            finally
            {
                options.TempFiles.Delete();
            }
        }

        private CompilerResults CompileFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            throw new NotImplementedException("sorry ICodeGenerator is not implemented, feel free to fix it and request merge");
        }

        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            return this.CompileAssemblyFromFileBatch(options, new string[1] { fileName });
        }

        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            CompilerSettings compilerSettings = this.ParamsToSettings(options);
            foreach (string obj in fileNames)
            {
                string fullPath = Path.GetFullPath(obj);
                SourceFile item = new SourceFile(obj, fullPath, compilerSettings.SourceFiles.Count + 1);
                compilerSettings.SourceFiles.Add(item);
            }
            return this.CompileFromCompilerSettings(compilerSettings, options.GenerateInMemory);
        }

        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            return this.CompileAssemblyFromSourceBatch(options, new string[1] { source });
        }

        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            CompilerSettings compilerSettings = this.ParamsToSettings(options);
            int num = 0;
            foreach (string text in sources)
            {
                string source = text;
                Func<Stream> streamIfDynamicFile = () => new MemoryStream(Encoding.UTF8.GetBytes(source ?? ""));
                string text2 = num.ToString();
                SourceFile item = new SourceFile(text2, text2, compilerSettings.SourceFiles.Count + 1, streamIfDynamicFile);
                compilerSettings.SourceFiles.Add(item);
                num++;
            }
            return this.CompileFromCompilerSettings(compilerSettings, options.GenerateInMemory);
        }

        private CompilerResults CompileFromCompilerSettings(CompilerSettings settings, bool generateInMemory)
        {
            CompilerResults compilerResults = new CompilerResults(new TempFileCollection(Path.GetTempPath()));
            CustomDynamicDriver customDynamicDriver = new CustomDynamicDriver(new CompilerContext(settings, new CustomReportPrinter(compilerResults)));
            AssemblyBuilder outAssembly = null;
            try
            {
                customDynamicDriver.Compile(out outAssembly, AppDomain.CurrentDomain, generateInMemory);
            }
            catch (Exception ex)
            {
                compilerResults.Errors.Add(new CompilerError
                {
                    IsWarning = false,
                    ErrorText = ex.Message
                });
            }
            compilerResults.CompiledAssembly = outAssembly;
            return compilerResults;
        }

        private CompilerSettings ParamsToSettings(CompilerParameters parameters)
        {
            CompilerSettings compilerSettings = new CompilerSettings();
            StringEnumerator enumerator = parameters.ReferencedAssemblies.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    compilerSettings.AssemblyReferences.Add(current);
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            compilerSettings.Encoding = Encoding.UTF8;
            compilerSettings.GenerateDebugInfo = parameters.IncludeDebugInformation;
            compilerSettings.MainClass = parameters.MainClass;
            compilerSettings.Platform = Platform.AnyCPU;
            compilerSettings.StdLibRuntimeVersion = RuntimeVersion.v4;
            if (parameters.GenerateExecutable)
            {
                compilerSettings.Target = Target.Exe;
                compilerSettings.TargetExt = ".exe";
            }
            else
            {
                compilerSettings.Target = Target.Library;
                compilerSettings.TargetExt = ".dll";
            }
            if (parameters.GenerateInMemory)
            {
                compilerSettings.Target = Target.Library;
            }
            if (string.IsNullOrEmpty(parameters.OutputAssembly))
            {
                parameters.OutputAssembly = (compilerSettings.OutputFile = "DynamicAssembly_" + CodeCompiler.assemblyCounter + compilerSettings.TargetExt);
                CodeCompiler.assemblyCounter++;
            }
            compilerSettings.OutputFile = parameters.OutputAssembly;
            compilerSettings.Version = LanguageVersion.V_6;
            compilerSettings.WarningLevel = parameters.WarningLevel;
            compilerSettings.WarningsAreErrors = parameters.TreatWarningsAsErrors;
            return compilerSettings;
        }
    }
}
