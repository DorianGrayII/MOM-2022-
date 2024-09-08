using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Security.Cryptography;
using Mono.CSharp;

namespace CSharpCompiler
{
    public class CustomDynamicDriver
    {
        private readonly CompilerContext ctx;

        public Report Report => this.ctx.Report;

        public CustomDynamicDriver(CompilerContext ctx)
        {
            this.ctx = ctx;
        }

        private void tokenize_file(SourceFile sourceFile, ModuleContainer module, ParserSession session)
        {
            Stream dataStream;
            try
            {
                dataStream = sourceFile.GetDataStream();
            }
            catch
            {
                this.Report.Error(2001, "Source file `" + sourceFile.Name + "' could not be found");
                return;
            }
            using (dataStream)
            {
                SeekableStreamReader input = new SeekableStreamReader(dataStream, this.ctx.Settings.Encoding);
                CompilationSourceFile file = new CompilationSourceFile(module, sourceFile);
                Tokenizer tokenizer = new Tokenizer(input, file, session, this.ctx.Report);
                int num = 0;
                int num2 = 0;
                int num3;
                while ((num3 = tokenizer.token()) != 257)
                {
                    num++;
                    if (num3 == 259)
                    {
                        num2++;
                    }
                }
                Console.WriteLine("Tokenized: " + num + " found " + num2 + " errors");
            }
        }

        public void Parse(ModuleContainer module)
        {
            bool tokenizeOnly = module.Compiler.Settings.TokenizeOnly;
            List<SourceFile> sourceFiles = module.Compiler.SourceFiles;
            Location.Initialize(sourceFiles);
            ParserSession session = new ParserSession
            {
                UseJayGlobalArrays = true,
                LocatedTokens = new LocatedToken[15000]
            };
            for (int i = 0; i < sourceFiles.Count; i++)
            {
                if (tokenizeOnly)
                {
                    this.tokenize_file(sourceFiles[i], module, session);
                }
                else
                {
                    this.Parse(sourceFiles[i], module, session, this.Report);
                }
            }
        }

        public void Parse(SourceFile file, ModuleContainer module, ParserSession session, Report report)
        {
            Stream dataStream;
            try
            {
                dataStream = file.GetDataStream();
            }
            catch
            {
                report.Error(2001, "Source file `{0}' could not be found", file.Name);
                return;
            }
            if (dataStream.ReadByte() == 77 && dataStream.ReadByte() == 90)
            {
                report.Error(2015, "Source file `{0}' is a binary file and not a text file", file.Name);
                dataStream.Close();
                return;
            }
            dataStream.Position = 0L;
            SeekableStreamReader seekableStreamReader = new SeekableStreamReader(dataStream, this.ctx.Settings.Encoding, session.StreamReaderBuffer);
            CustomDynamicDriver.Parse(seekableStreamReader, file, module, session, report);
            if (this.ctx.Settings.GenerateDebugInfo && report.Errors == 0 && !file.HasChecksum)
            {
                dataStream.Position = 0L;
                MD5 checksumAlgorithm = session.GetChecksumAlgorithm();
                file.SetChecksum(checksumAlgorithm.ComputeHash(dataStream));
            }
            seekableStreamReader.Dispose();
            dataStream.Close();
        }

        public static void Parse(SeekableStreamReader reader, SourceFile sourceFile, ModuleContainer module, ParserSession session, Report report)
        {
            CompilationSourceFile compilationSourceFile = new CompilationSourceFile(module, sourceFile);
            module.AddTypeContainer(compilationSourceFile);
            new CSharpParser(reader, compilationSourceFile, report, session).parse();
        }

        public bool Compile(out AssemblyBuilder outAssembly, AppDomain domain, bool generateInMemory)
        {
            CompilerSettings settings = this.ctx.Settings;
            outAssembly = null;
            if (settings.FirstSourceFile == null && (settings.Target == Target.Exe || settings.Target == Target.WinExe || settings.Target == Target.Module || settings.Resources == null))
            {
                this.Report.Error(2008, "No files to compile were specified");
                return false;
            }
            if (settings.Platform == Platform.AnyCPU32Preferred && (settings.Target == Target.Library || settings.Target == Target.Module))
            {
                this.Report.Error(4023, "Platform option `anycpu32bitpreferred' is valid only for executables");
                return false;
            }
            TimeReporter timeReporter = new TimeReporter(settings.Timestamps);
            this.ctx.TimeReporter = timeReporter;
            timeReporter.StartTotal();
            ModuleContainer moduleContainer2 = (RootContext.ToplevelTypes = new ModuleContainer(this.ctx));
            timeReporter.Start(TimeReporter.TimerType.ParseTotal);
            this.Parse(moduleContainer2);
            timeReporter.Stop(TimeReporter.TimerType.ParseTotal);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            if (settings.TokenizeOnly || settings.ParseOnly)
            {
                timeReporter.StopTotal();
                timeReporter.ShowStats();
                return true;
            }
            string outputFile = settings.OutputFile;
            string fileName = Path.GetFileName(outputFile);
            AssemblyDefinitionDynamic assemblyDefinitionDynamic = new AssemblyDefinitionDynamic(moduleContainer2, fileName, outputFile);
            moduleContainer2.SetDeclaringAssembly(assemblyDefinitionDynamic);
            ReflectionImporter importer = (ReflectionImporter)(assemblyDefinitionDynamic.Importer = new ReflectionImporter(moduleContainer2, this.ctx.BuiltinTypes));
            DynamicLoader dynamicLoader = new DynamicLoader(importer, this.ctx);
            dynamicLoader.LoadReferences(moduleContainer2);
            if (!this.ctx.BuiltinTypes.CheckDefinitions(moduleContainer2))
            {
                return false;
            }
            if (!assemblyDefinitionDynamic.Create(domain, AssemblyBuilderAccess.RunAndSave))
            {
                return false;
            }
            moduleContainer2.CreateContainer();
            dynamicLoader.LoadModules(assemblyDefinitionDynamic, moduleContainer2.GlobalRootNamespace);
            moduleContainer2.InitializePredefinedTypes();
            if (settings.GetResourceStrings != null)
            {
                moduleContainer2.LoadGetResourceStrings(settings.GetResourceStrings);
            }
            timeReporter.Start(TimeReporter.TimerType.ModuleDefinitionTotal);
            moduleContainer2.Define();
            timeReporter.Stop(TimeReporter.TimerType.ModuleDefinitionTotal);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            if (settings.DocumentationFile != null)
            {
                new DocumentationBuilder(moduleContainer2).OutputDocComment(outputFile, settings.DocumentationFile);
            }
            assemblyDefinitionDynamic.Resolve();
            if (this.Report.Errors > 0)
            {
                return false;
            }
            timeReporter.Start(TimeReporter.TimerType.EmitTotal);
            assemblyDefinitionDynamic.Emit();
            timeReporter.Stop(TimeReporter.TimerType.EmitTotal);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            timeReporter.Start(TimeReporter.TimerType.CloseTypes);
            moduleContainer2.CloseContainer();
            timeReporter.Stop(TimeReporter.TimerType.CloseTypes);
            timeReporter.Start(TimeReporter.TimerType.Resouces);
            if (!settings.WriteMetadataOnly)
            {
                assemblyDefinitionDynamic.EmbedResources();
            }
            timeReporter.Stop(TimeReporter.TimerType.Resouces);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            if (!generateInMemory)
            {
                assemblyDefinitionDynamic.Save();
            }
            outAssembly = assemblyDefinitionDynamic.Builder;
            timeReporter.StopTotal();
            timeReporter.ShowStats();
            return this.Report.Errors == 0;
        }
    }
}
