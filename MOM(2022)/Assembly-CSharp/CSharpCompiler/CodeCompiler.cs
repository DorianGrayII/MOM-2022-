namespace CSharpCompiler
{
    using Mono.CSharp;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection.Emit;
    using System.Text;

    public class CodeCompiler : ICodeCompiler
    {
        private static long assemblyCounter;

        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            CodeCompileUnit[] ea = new CodeCompileUnit[] { compilationUnit };
            return this.CompileAssemblyFromDomBatch(options, ea);
        }

        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            CompilerResults results;
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            try
            {
                results = this.CompileFromDomBatch(options, ea);
            }
            finally
            {
                options.TempFiles.Delete();
            }
            return results;
        }

        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            string[] fileNames = new string[] { fileName };
            return this.CompileAssemblyFromFileBatch(options, fileNames);
        }

        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            CompilerSettings settings = this.ParamsToSettings(options);
            string[] strArray = fileNames;
            for (int i = 0; i < strArray.Length; i++)
            {
                string path = strArray[i];
                string fullPath = Path.GetFullPath(path);
                SourceFile item = new SourceFile(path, fullPath, settings.SourceFiles.Count + 1, null);
                settings.SourceFiles.Add(item);
            }
            return this.CompileFromCompilerSettings(settings, options.GenerateInMemory);
        }

        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            string[] sources = new string[] { source };
            return this.CompileAssemblyFromSourceBatch(options, sources);
        }

        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            CompilerSettings settings = this.ParamsToSettings(options);
            int num = 0;
            foreach (string str in sources)
            {
                Func<Stream> func = delegate {
                    string s = str;
                    if (str == null)
                    {
                        string local1 = str;
                        s = "";
                    }
                    return new MemoryStream(Encoding.UTF8.GetBytes(s));
                };
                SourceFile item = new SourceFile(num.ToString(), num.ToString(), settings.SourceFiles.Count + 1, func);
                settings.SourceFiles.Add(item);
                num++;
            }
            return this.CompileFromCompilerSettings(settings, options.GenerateInMemory);
        }

        private CompilerResults CompileFromCompilerSettings(CompilerSettings settings, bool generateInMemory)
        {
            CompilerResults compilerResults = new CompilerResults(new TempFileCollection(Path.GetTempPath()));
            CustomDynamicDriver driver = new CustomDynamicDriver(new CompilerContext(settings, new CustomReportPrinter(compilerResults)));
            AssemblyBuilder outAssembly = null;
            try
            {
                driver.Compile(out outAssembly, AppDomain.CurrentDomain, generateInMemory);
            }
            catch (Exception exception)
            {
                CompilerError error1 = new CompilerError();
                error1.IsWarning = false;
                error1.ErrorText = exception.Message;
                compilerResults.Errors.Add(error1);
            }
            compilerResults.CompiledAssembly = outAssembly;
            return compilerResults;
        }

        private CompilerResults CompileFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            throw new NotImplementedException("sorry ICodeGenerator is not implemented, feel free to fix it and request merge");
        }

        private CompilerSettings ParamsToSettings(CompilerParameters parameters)
        {
            CompilerSettings settings = new CompilerSettings();
            foreach (string str in parameters.ReferencedAssemblies)
            {
                settings.AssemblyReferences.Add(str);
            }
            settings.Encoding = Encoding.UTF8;
            settings.GenerateDebugInfo = parameters.IncludeDebugInformation;
            settings.MainClass = parameters.MainClass;
            settings.Platform = Platform.AnyCPU;
            settings.StdLibRuntimeVersion = RuntimeVersion.v4;
            if (parameters.GenerateExecutable)
            {
                settings.Target = Target.Exe;
                settings.TargetExt = ".exe";
            }
            else
            {
                settings.Target = Target.Library;
                settings.TargetExt = ".dll";
            }
            if (parameters.GenerateInMemory)
            {
                settings.Target = Target.Library;
            }
            if (string.IsNullOrEmpty(parameters.OutputAssembly))
            {
                parameters.OutputAssembly = settings.OutputFile = "DynamicAssembly_" + assemblyCounter.ToString() + settings.TargetExt;
                assemblyCounter += 1L;
            }
            settings.OutputFile = parameters.OutputAssembly;
            settings.Version = LanguageVersion.V_6;
            settings.WarningLevel = parameters.WarningLevel;
            settings.WarningsAreErrors = parameters.TreatWarningsAsErrors;
            return settings;
        }
    }
}

