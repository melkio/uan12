using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Roslyn.Compilers.CSharp;

namespace demo.Unit04
{
    public class DeclarativeLanguage
    {
        public static void Run()
        {
            String code = File.ReadAllText(@"Unit04\singleton.txt");
            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(code);

            Console.WriteLine("***** INPUT *****");
            Console.WriteLine(tree.Root.Format().ToString());

            SingletonRewriter rewriter = new SingletonRewriter();
            SyntaxNode newRoot = rewriter.Visit(tree.Root);
            Console.WriteLine("***** OUTPUT *****");
            Console.WriteLine(newRoot.Format().ToString());
        }
    }
}
