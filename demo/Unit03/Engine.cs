using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Scripting.CSharp;
using Roslyn.Scripting;

namespace demo.Unit03
{
    public class Engine
    {
        public static void Run()
        {
            ScriptEngine engine = new ScriptEngine();
            int result = engine.Execute<int>("1 + 2");

            Console.WriteLine(result);

            //ScriptEngine engine = new ScriptEngine();
            //Session session = Session.Create();

            //engine.Execute("int i = 21;", session);
            //int result = engine.Execute<int>("i * 2", session);

            //Console.WriteLine(result);
        }
    }
}
