using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo.Unit04
{
    public class SingletonRewriter : SyntaxRewriter
    {
        protected override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            AttributeSyntax attribute = node.Attributes
                  .SelectMany(a => a.ChildNodes().OfType<AttributeSyntax>())
                  .FirstOrDefault(a => a.Name.PlainName == "EnableSingleton");

            if (attribute == null) 
                return base.VisitClassDeclaration(node);

            LiteralExpressionSyntax argument = (LiteralExpressionSyntax) attribute
                .ArgumentListOpt
                .Arguments
                .First()
                .Expression;

            VariableDeclaratorSyntax syncVariable = Syntax.VariableDeclarator(identifier: Syntax.Identifier("_sync"));
            FieldDeclarationSyntax syncField = Syntax.FieldDeclaration(
                modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.PrivateKeyword), Syntax.Token(SyntaxKind.StaticKeyword)),
                declaration: Syntax.VariableDeclaration(
                    type: Syntax.ParseTypeName("System.Object"),
                    variables: Syntax.SeparatedList(syncVariable)));

            VariableDeclaratorSyntax instanceVariable = Syntax.VariableDeclarator(identifier: Syntax.Identifier("_instance"));
            FieldDeclarationSyntax instanceField = Syntax.FieldDeclaration(
                modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.PrivateKeyword), Syntax.Token(SyntaxKind.StaticKeyword)),
                declaration: Syntax.VariableDeclaration(
                    type: Syntax.ParseTypeName(node.Identifier.ValueText),
                    variables: Syntax.SeparatedList(instanceVariable)));

            ConstructorDeclarationSyntax constructor = Syntax.ConstructorDeclaration(
                modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.StaticKeyword)),
                identifier: Syntax.Identifier(node.Identifier.ValueText),
                parameterList: Syntax.ParameterList(parameters: Syntax.SeparatedList<ParameterSyntax>()),
                bodyOpt: Syntax.Block(
                    statements: Syntax.List<StatementSyntax>(Syntax.ExpressionStatement(Syntax.ParseExpression("_sync = new Object()")))));

            IfStatementSyntax ifStatement = Syntax.IfStatement(
                ifKeyword: Syntax.Token(SyntaxKind.IfKeyword),
                condition: Syntax.BinaryExpression(SyntaxKind.EqualsExpression, Syntax.IdentifierName("_instance"),
                    right: Syntax.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                statement: Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, Syntax.IdentifierName("_instance"),
                    right: Syntax.ObjectCreationExpression(
                        newKeyword: Syntax.Token(SyntaxKind.NewKeyword),
                        type: Syntax.ParseTypeName(node.Identifier.ValueText),
                        argumentListOpt: Syntax.ArgumentList(arguments: Syntax.SeparatedList<ArgumentSyntax>())))));

            LockStatementSyntax lockStatement = Syntax.LockStatement(
                lockKeyword: Syntax.Token(SyntaxKind.LockKeyword),
                expression: Syntax.IdentifierName("_sync"),
                statement: Syntax.Block(
                    statements: Syntax.List<StatementSyntax>(ifStatement, Syntax.ParseStatement("return _instance"))));
            //Syntax.BinaryExpression(SyntaxKind.EqualsEqualsExpression, Syntax.IdentifierName("_instance"));
            //Syntax.LiteralExpression(SyntaxKind.NullKeyword);
            //Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, Syntax.IdentifierName("_instance")));
            //Syntax.ObjectCreationExpression(
            //            type: Syntax.ParseTypeName(node.Identifier.ValueText),
            //            argumentListOpt: Syntax.ArgumentList(arguments: Syntax.SeparatedList<ArgumentSyntax>()),
            //            initializerOpt: Syntax.InitializerExpression(expressions: Syntax.SeparatedList<ExpressionSyntax>(Syntax.Identifier(node.Identifier.ValueText))));

            
                        

            //StatementSyntax getter = Syntax.ReturnStatement(expressionOpt: cast);
            PropertyDeclarationSyntax instance = Syntax.PropertyDeclaration(
              modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword), Syntax.Token(SyntaxKind.StaticKeyword)),
              type: Syntax.ParseTypeName(node.Identifier.ValueText),
              identifier: Syntax.Identifier(argument.Token.ValueText),
              accessorList: Syntax.AccessorList(
                accessors: Syntax.List(
                  Syntax.AccessorDeclaration(
                    kind: SyntaxKind.GetAccessorDeclaration,
                    bodyOpt: Syntax.Block(
                      statements: Syntax.List<StatementSyntax>(lockStatement)
                      )
                    )
                  )
                )
             );

            var newTypeDeclaration = (TypeDeclarationSyntax)base.VisitClassDeclaration(node);
            var members = new List<MemberDeclarationSyntax>(newTypeDeclaration.Members);
            members.Add(syncField);
            members.Add(instanceField);
            members.Add(constructor);
            members.Add(instance);

            return ((ClassDeclarationSyntax)newTypeDeclaration).Update(
                newTypeDeclaration.Attributes,
                newTypeDeclaration.Modifiers,
                newTypeDeclaration.Keyword,
                newTypeDeclaration.Identifier,
                newTypeDeclaration.TypeParameterListOpt,
                newTypeDeclaration.BaseListOpt,
                newTypeDeclaration.ConstraintClauses,
                newTypeDeclaration.OpenBraceToken,
                Syntax.List(members.AsEnumerable()),
                newTypeDeclaration.CloseBraceToken,
                newTypeDeclaration.SemicolonTokenOpt);
        }
    }
}
