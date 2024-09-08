namespace CSharpCompiler
{
    using Mono.CSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public class CustomDynamicDriver
    {
        private readonly CompilerContext ctx;

        public CustomDynamicDriver(CompilerContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Compile(out AssemblyBuilder outAssembly, AppDomain domain, bool generateInMemory)
        {
            CompilerSettings settings = this.ctx.Settings;
            outAssembly = null;
            if ((settings.FirstSourceFile == null) && ((settings.Target == Target.Exe) || ((settings.Target == Target.WinExe) || ((settings.Target == Target.Module) || (settings.Resources == null)))))
            {
                this.Report.Error(0x7d8, "No files to compile were specified");
                return false;
            }
            if ((settings.Platform == Platform.AnyCPU32Preferred) && ((settings.Target == Target.Library) || (settings.Target == Target.Module)))
            {
                this.Report.Error(0xfb7, "Platform option `anycpu32bitpreferred' is valid only for executables");
                return false;
            }
            TimeReporter reporter = new TimeReporter(settings.Timestamps);
            this.ctx.TimeReporter = reporter;
            reporter.StartTotal();
            ModuleContainer module = new ModuleContainer(this.ctx);
            RootContext.ToplevelTypes = module;
            reporter.Start(TimeReporter.TimerType.ParseTotal);
            this.Parse(module);
            reporter.Stop(TimeReporter.TimerType.ParseTotal);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            if (settings.TokenizeOnly || settings.ParseOnly)
            {
                reporter.StopTotal();
                reporter.ShowStats();
                return true;
            }
            string outputFile = settings.OutputFile;
            AssemblyDefinitionDynamic assembly = new AssemblyDefinitionDynamic(module, Path.GetFileName(outputFile), outputFile);
            module.SetDeclaringAssembly(assembly);
            ReflectionImporter importer = new ReflectionImporter(module, this.ctx.BuiltinTypes);
            assembly.Importer = importer;
            DynamicLoader loader = new DynamicLoader(importer, this.ctx);
            loader.LoadReferences(module);
            if (!this.ctx.BuiltinTypes.CheckDefinitions(module))
            {
                return false;
            }
            if (!assembly.Create(domain, AssemblyBuilderAccess.RunAndSave))
            {
                return false;
            }
            module.CreateContainer();
            loader.LoadModules(assembly, module.GlobalRootNamespace);
            module.InitializePredefinedTypes();
            if (settings.GetResourceStrings != null)
            {
                module.LoadGetResourceStrings(settings.GetResourceStrings);
            }
            reporter.Start(TimeReporter.TimerType.ModuleDefinitionTotal);
            module.Define();
            reporter.Stop(TimeReporter.TimerType.ModuleDefinitionTotal);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            if (settings.DocumentationFile != null)
            {
                new DocumentationBuilder(module).OutputDocComment(outputFile, settings.DocumentationFile);
            }
            assembly.Resolve();
            if (this.Report.Errors > 0)
            {
                return false;
            }
            reporter.Start(TimeReporter.TimerType.EmitTotal);
            assembly.Emit();
            reporter.Stop(TimeReporter.TimerType.EmitTotal);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            reporter.Start(TimeReporter.TimerType.CloseTypes);
            module.CloseContainer();
            reporter.Stop(TimeReporter.TimerType.CloseTypes);
            reporter.Start(TimeReporter.TimerType.Resouces);
            if (!settings.WriteMetadataOnly)
            {
                assembly.EmbedResources();
            }
            reporter.Stop(TimeReporter.TimerType.Resouces);
            if (this.Report.Errors > 0)
            {
                return false;
            }
            if (!generateInMemory)
            {
                assembly.Save();
            }
            outAssembly = assembly.Builder;
            reporter.StopTotal();
            reporter.ShowStats();
            return (this.Report.Errors == 0);
        }

        public void Parse(ModuleContainer module)
        {
            bool tokenizeOnly = module.Compiler.Settings.TokenizeOnly;
            List<SourceFile> sourceFiles = module.Compiler.SourceFiles;
            Location.Initialize(sourceFiles);
            ParserSession session1 = new ParserSession();
            session1.UseJayGlobalArrays = true;
            session1.LocatedTokens = new LocatedToken[0x3a98];
            ParserSession session = session1;
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

        public void Parse(SourceFile file, ModuleContainer module, ParserSession session, Mono.CSharp.Report report)
        {
            Stream dataStream;
            try
            {
                dataStream = file.GetDataStream();
            }
            catch
            {
                report.Error(0x7d1, "Source file `{0}' could not be found", file.Name);
                return;
            }
            if ((dataStream.ReadByte() == 0x4d) && (dataStream.ReadByte() == 90))
            {
                report.Error(0x7df, "Source file `{0}' is a binary file and not a text file", file.Name);
                dataStream.Close();
            }
            else
            {
                dataStream.Position = 0L;
                SeekableStreamReader reader = new SeekableStreamReader(dataStream, this.ctx.Settings.Encoding, session.StreamReaderBuffer);
                Parse(reader, file, module, session, report);
                if (this.ctx.Settings.GenerateDebugInfo && ((report.Errors == 0) && !file.HasChecksum))
                {
                    dataStream.Position = 0L;
                    MD5 checksumAlgorithm = session.GetChecksumAlgorithm();
                    file.SetChecksum(checksumAlgorithm.ComputeHash(dataStream));
                }
                reader.Dispose();
                dataStream.Close();
            }
        }

        public static void Parse(SeekableStreamReader reader, SourceFile sourceFile, ModuleContainer module, ParserSession session, Mono.CSharp.Report report)
        {
            CompilationSourceFile tc = new CompilationSourceFile(module, sourceFile);
            module.AddTypeContainer(tc);
            new CSharpParser(reader, tc, report, session).parse();
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
                this.Report.Error(0x7d1, "Source file `" + sourceFile.Name + "' could not be found");
                return;
            }
            using (dataStream)
            {
                Tokenizer tokenizer = new Tokenizer(new SeekableStreamReader(dataStream, this.ctx.Settings.Encoding, null), new CompilationSourceFile(module, sourceFile), session, this.ctx.Report);
                int num2 = 0;
                int num3 = 0;
                while (true)
                {
                    int num = tokenizer.token();
                    if (num == 0x101)
                    {
                        string[] textArray1 = new string[] { "Tokenized: ", num2.ToString(), " found ", num3.ToString(), " errors" };
                        Console.WriteLine(string.Concat(textArray1));
                        break;
                    }
                    num2++;
                    if (num == 0x103)
                    {
                        num3++;
                    }
                }
            }
        }

        public Mono.CSharp.Report Report
        {
            get
            {
                return this.ctx.Report;
            }
        }
    }
}

