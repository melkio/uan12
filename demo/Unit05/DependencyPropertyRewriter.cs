using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo.Unit05
{
    public class DependencyPropertyRewriter : SyntaxRewriter
    {
        private readonly List<FieldDeclarationSyntax> _fields = new List<FieldDeclarationSyntax>();
        private readonly List<MethodDeclarationSyntax> _methods = new List<MethodDeclarationSyntax>();

        protected override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            Boolean isDependencyProperty = node
                .Attributes
                .SelectMany(a => a.ChildNodes().OfType<AttributeSyntax>())
                .Any(a => a.Name.PlainName == "Dependency");

            if (!isDependencyProperty)
            {
                return base.VisitPropertyDeclaration(node);
            }

            AccessorDeclarationSyntax changeCallback = node
                .AccessorList
                .Accessors
                .FirstOrDefault(a => a.Kind == SyntaxKind.UnknownAccessorDeclaration && a.Keyword.ValueText == "change");
            Boolean hasChangeCallback = changeCallback != null;

            TypeSyntax dpType = Syntax.ParseTypeName("System.Windows.DependencyProperty");

            // add a DP field:
            // DependencyProperty XxxProperty = DependencyProperty.Register("Xxx", typeof(PropType), typeof(OwnerType),
            //   new FrameworkPropertyMetadata(OnXxxChanged);
            TypeSyntax ownerType = Syntax.ParseTypeName(node
                .FirstAncestorOrSelf<ClassDeclarationSyntax>()
                .Identifier
                .ValueText);  
            String propertyName = node.Identifier.ValueText;
            ExpressionSyntax propertyNameExpression = Syntax.LiteralExpression(SyntaxKind.StringLiteralExpression, Syntax.Literal(text: '"' + propertyName + '"', value: propertyName));
            ExpressionSyntax typeofPropertyType = Syntax.TypeOfExpression(argumentList: Syntax.ArgumentList(arguments: Syntax.SeparatedList(Syntax.Argument(expression: node.Type))));
            ExpressionSyntax typeofOwnerType = Syntax.TypeOfExpression(argumentList: Syntax.ArgumentList(arguments: Syntax.SeparatedList(Syntax.Argument(expression: ownerType))));

            var registerArgs = new List<ArgumentSyntax> {
                                                            Syntax.Argument(expression: propertyNameExpression),
                                                            Syntax.Argument(expression: typeofPropertyType),
                                                            Syntax.Argument(expression: typeofOwnerType)
                                                        };

            if (hasChangeCallback)
            {
                ExpressionSyntax changeMethod = Syntax.ParseName("On" + propertyName + "Changed");
                
                ExpressionSyntax frameworkPropertyMetadata = Syntax.ObjectCreationExpression(
                  type: Syntax.ParseTypeName("System.Windows.FrameworkPropertyMetadata"),
                  argumentListOpt: Syntax.ArgumentList(arguments: Syntax.SeparatedList(Syntax.Argument(expression: changeMethod))));
                
                registerArgs.Add(Syntax.Argument(expression: frameworkPropertyMetadata)
                );
            }

            IEnumerable<SyntaxToken> argSeparators = Enumerable.Repeat(
                Syntax.Token(SyntaxKind.CommaToken), 
                registerArgs.Count - 1)
                .ToList(); 

            ExpressionSyntax dpexpr = Syntax.InvocationExpression(
              expression: Syntax.ParseName("System.Windows.DependencyProperty.Register"),
              argumentList: Syntax.ArgumentList(arguments: Syntax.SeparatedList(registerArgs, argSeparators)));
            
            String fieldName = propertyName + "Property";
            VariableDeclaratorSyntax declarator = Syntax.VariableDeclarator(
              identifier: Syntax.Identifier(fieldName),
              initializerOpt: Syntax.EqualsValueClause(value: dpexpr));
            FieldDeclarationSyntax newField = Syntax.FieldDeclaration(
              modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword), Syntax.Token(SyntaxKind.StaticKeyword)),
              declaration: Syntax.VariableDeclaration(
                type: dpType,
                variables: Syntax.SeparatedList(declarator)));
            _fields.Add(newField);

            // add a DP CLR wrapper:
            // public PropType Xxx
            // {
            //   get { return (PropType)GetValue(XxxProperty); }
            //   set { SetValue(XxxPropety, value); }
            // }
            ExpressionSyntax getval = Syntax.ParseExpression("GetValue(" + fieldName + ")");
            ExpressionSyntax cast = Syntax.CastExpression(type: node.Type, expression: getval);
            StatementSyntax getter = Syntax.ReturnStatement(expressionOpt: cast);

            StatementSyntax setter = Syntax.ParseStatement("SetValue(" + fieldName + ");");

            PropertyDeclarationSyntax newProperty = Syntax.PropertyDeclaration(
              modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)),
              type: node.Type,
              identifier: node.Identifier,
              accessorList: Syntax.AccessorList(
                accessors: Syntax.List(
                  Syntax.AccessorDeclaration(
                    kind: SyntaxKind.GetAccessorDeclaration,
                    bodyOpt: Syntax.Block(
                      statements: Syntax.List(
                        getter
                      )
                    )
                  ),
                  Syntax.AccessorDeclaration(
                    kind: SyntaxKind.SetAccessorDeclaration,
                    bodyOpt: Syntax.Block(
                      statements: Syntax.List(
                        setter
                      )
                    )
                  )
                )
              ));

            // add change callback if required
            // private static void OnXxxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            // {
            //   /* body */
            // }
            if (hasChangeCallback)
            {
                List<ParameterSyntax> parameterList = new List<ParameterSyntax>
                            {
                                Syntax.Parameter(identifier: Syntax.Identifier("d"), typeOpt: Syntax.ParseTypeName("System.Windows.DependencyObject")),
                                Syntax.Parameter(identifier: Syntax.Identifier("e"), typeOpt: Syntax.ParseTypeName("System.Windows.DependencyPropertyChangedEventArgs")),
                            };
                var paramSeparators = Enumerable.Repeat(Syntax.Token(SyntaxKind.CommaToken), parameterList.Count - 1).ToList();  
                ParameterListSyntax parameters = Syntax.ParameterList(
                  parameters: Syntax.SeparatedList(parameterList, paramSeparators)
                );
                MethodDeclarationSyntax changeMethod = Syntax.MethodDeclaration(
                  modifiers: Syntax.TokenList(Syntax.Token(SyntaxKind.PrivateKeyword), Syntax.Token(SyntaxKind.StaticKeyword)),
                  identifier: Syntax.Identifier("On" + propertyName + "Changed"),
                  returnType: Syntax.PredefinedType(Syntax.Token(SyntaxKind.VoidKeyword)),
                  parameterList: parameters,
                  bodyOpt: changeCallback.BodyOpt
                );
                _methods.Add(changeMethod);
            }

            return newProperty;
        }

        protected override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var newTypeDeclaration = (TypeDeclarationSyntax)base.VisitClassDeclaration(node);

            if (_fields.Count > 0 || _methods.Count > 0)
            {
                var members = new List<MemberDeclarationSyntax>(newTypeDeclaration.Members);
                members.InsertRange(0, _methods);
                members.InsertRange(0, _fields);

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

            return newTypeDeclaration;
        }
    }
}
