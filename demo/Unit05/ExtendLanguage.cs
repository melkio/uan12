using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using System.IO;

namespace demo.Unit05
{
    public class ExtendLanguage
    {
        public static void Run()
        {
            String code = File.ReadAllText(@"Unit05\advanced.txt");

            SyntaxTree tree = SyntaxTree.ParseCompilationUnit(code);
            
            Console.WriteLine("***** INPUT *****");
            Console.WriteLine(tree.Root.Format().ToString());

            DependencyPropertyRewriter rewriter = new DependencyPropertyRewriter();
            SyntaxNode newRoot = rewriter.Visit(tree.Root);
            Console.WriteLine("***** OUTPUT *****");
            Console.WriteLine(newRoot.Format().ToString());


        }
    }
}
