// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CSharpCompiler.CustomReportPrinter
using System.CodeDom.Compiler;
using Mono.CSharp;

public class CustomReportPrinter : ReportPrinter
{
    private readonly CompilerResults compilerResults;

    public new int ErrorsCount { get; protected set; }

    public new int WarningsCount { get; private set; }

    public CustomReportPrinter(CompilerResults compilerResults)
    {
        this.compilerResults = compilerResults;
    }

    public override void Print(AbstractMessage msg, bool showFullPath)
    {
        if (msg.IsWarning)
        {
            int warningsCount = this.WarningsCount + 1;
            this.WarningsCount = warningsCount;
        }
        else
        {
            int warningsCount = this.ErrorsCount + 1;
            this.ErrorsCount = warningsCount;
        }
        this.compilerResults.Errors.Add(new CompilerError
        {
            IsWarning = msg.IsWarning,
            Column = msg.Location.Column,
            Line = msg.Location.Row,
            ErrorNumber = msg.Code.ToString(),
            ErrorText = msg.Text,
            FileName = (showFullPath ? msg.Location.SourceFile.FullPathName : msg.Location.SourceFile.Name)
        });
    }
}
