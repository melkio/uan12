using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using System.Globalization;

namespace demo.Unit02
{
    public class ErrorsAndWarnings
    {
        public static void Run()
        {
            string text = @"class Program
{
    static int Main(string[] args)
    {
    }
}";

            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(text);
            Compilation compilation = Compilation
                .Create("program.exe")
                .AddSyntaxTrees(tree)
                .AddReferences(new AssemblyFileReference(typeof(object).Assembly.Location));

            IEnumerable<Diagnostic> errorsAndWarnings = compilation.GetDiagnostics();

            Diagnostic error = errorsAndWarnings.First();
            Console.WriteLine(error.Info.GetMessage(CultureInfo.InvariantCulture));

            //Location errorLocation = error.Location;
            //Assert.AreEqual(4, error.Location.SourceSpan.Length);

            //IText programText = errorLocation.SourceTree.Text;
            //Assert.AreEqual("Main", programText.GetText(errorLocation.SourceSpan));

            //FileLinePositionSpan span = error.Location.GetLineSpan(usePreprocessorDirectives: true);
            //Assert.AreEqual(15, span.StartLinePosition.Character);
            //Assert.AreEqual(2, span.StartLinePosition.Line);
        }
    }
}
