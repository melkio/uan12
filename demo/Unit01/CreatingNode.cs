using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo.Unit01
{
    public class CreatingNode
    {
        public static void Run()
        {
            SyntaxToken namespaceToken = Syntax.Token(SyntaxKind.NamespaceKeyword);
            //SyntaxToken namespaceToken = Syntax.Token(
            //    SyntaxKind.NamespaceKeyword,
            //    Syntax.Space);

            QualifiedNameSyntax namespaceName = Syntax.QualifiedName(
                           left: Syntax.IdentifierName("demo"),
                           right: Syntax.IdentifierName("Unit01"));
            //NameSyntax namespaceName = Syntax.ParseName("demo.Unit01");

            NamespaceDeclarationSyntax currentNamespace = Syntax.NamespaceDeclaration(
                namespaceKeyword: namespaceToken,
                name: namespaceName);

            //Console.WriteLine(currentNamespace.ToString());
            Console.WriteLine(currentNamespace.Format().ToString());

        }
    }
}
