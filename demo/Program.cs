using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo
{
    class Program
    {
        public static void Main(String[] args)
        {
            Unit01.ShowSyntaxTreeVisualizer.Run();
            Unit01.CreatingNode.Run();
            Unit01.ModifyExistingNode.Run();
            Unit01.ParseExpression.Run();

            Unit02.EndToEndCompileAndRun.Run();
            Unit02.ErrorsAndWarnings.Run();

            Unit03.Engine.Run();

            Unit04.DeclarativeLanguage.Run();

            Unit05.ExtendLanguage.Run();

            Console.ReadLine();
        }
    }
}
