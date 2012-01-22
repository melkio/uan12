using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using System.IO;
using Roslyn.Compilers;
using System.Reflection;

namespace demo.Unit02
{
    public class EndToEndCompileAndRun
    {
        public static void Run()
        {
            String source = File.ReadAllText(@"Unit02\source.txt");
            String operation = File.ReadAllText(@"Unit02\operation.txt");

            String code = source.Replace("$", operation);
            var tree = SyntaxTree.ParseCompilationUnit(code);
            var compilation = Compilation.Create(
                "uan12.dll",
                options: new CompilationOptions(assemblyKind: AssemblyKind.DynamicallyLinkedLibrary),
                syntaxTrees: new[] { tree },
                references: new[] { new AssemblyFileReference(typeof(object).Assembly.Location) });

            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                EmitResult compileResult = compilation.Emit(stream);
                compiledAssembly = Assembly.Load(stream.GetBuffer());
            }

            Type calculator = compiledAssembly.GetType("Calculator");
            MethodInfo evaluate = calculator.GetMethod("Evaluate");
            string answer = evaluate.Invoke(null, null).ToString();

            Console.WriteLine(answer);
        }
    }
}
