using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;

namespace demo.Unit01
{
    public class ParseExpression
    {
        public static void Run()
        {
            ExpressionSyntax expression = Syntax.ParseExpression("1 + 2");
            Console.WriteLine(expression.Kind);

            BinaryExpressionSyntax binary = (BinaryExpressionSyntax)expression;
            Console.WriteLine(binary.OperatorToken);

            Console.WriteLine(binary.Left.Kind);
            Console.WriteLine(binary.Left);
        }
    }
}
