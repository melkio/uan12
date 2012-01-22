using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo.Unit01
{
    public class ShowSyntaxTreeVisualizer
    {
        public static void Run()
        {
            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(
                @"using System;
 
                namespace demo.Unit01
                {
                    class Program
                    {
                        static void Main(string[] args)
                        {
                            Console.WriteLine(""Hello, World!"");
                        }
                    }
                }");

        }
    }
}
