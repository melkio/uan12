using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo.Unit01
{
    public class ModifyExistingNode
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

            NamespaceDeclarationSyntax oldNamespace = tree
                .Root
                .DescendentNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .First();

            NameSyntax namespaceName = Syntax.ParseName("uan12");
            NamespaceDeclarationSyntax newNamespace = oldNamespace.Update(oldNamespace.NamespaceKeyword,
                namespaceName,
                oldNamespace.OpenBraceToken,
                oldNamespace.Externs,
                oldNamespace.Usings,
                oldNamespace.Members,
                oldNamespace.CloseBraceToken,
                oldNamespace.SemicolonTokenOpt);
            //SyntaxNode newRoot = tree.Root.ReplaceNode(oldNamespace, newNamespace);

            Console.WriteLine(newNamespace.Format().ToString());
            //Console.WriteLine(newRoot.Format().ToString());
        }
    }
}
