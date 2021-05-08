using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicExpr
{
    
    class Program
    {
        static void Main(string[] args)
        {
            //not((Code={code}+' ') and (x=2))
            var mm = "nde={code}+' ' or x=1 and n=2".ToTokens();
            var rx = mm.FixBinaryLogical("=");//.FixBinaryLogical("and").FixBinaryLogical("or");//.FixBinaryLogical("<>");
            Console.WriteLine(rx.RebuilFromTokens());
            var tk = TokenParsers.GetTokens("X.FirstName & '--' & X.LastName=M.cODE");
            var fx="FirstName & '--' & LastName".ToFxExpression();

            foreach (var x in tk)
            {
                Console.WriteLine(x);
            }
            //Test lan cuoi
            var FX = ExprBuilders.CreateFilter<MyClass>("Code={code}+' '", new { code = "XX" });
            //FX = ExprBuilders.CreateFilter<MyClass>("Code={code} and Name={name}", new { code = "XX" });
            //FX = ExprBuilders.CreateFilter<MyClass>("Code={code}", new { code = "XX" });
            //LambdaExpression SX = ExprBuilders.CreateSelection(typeof(MyClass),
            //        new Dictionary<string, string> {
            //            { "MyCode","Concat(Name,' ',Code)"},
            //            { "MyName","Name" }
            //        },
            //        new { code = "XX" }
            //    );
            //Expression<Func<MyClass,dynamic>> SX1 = ExprBuilders.CreateSelection<MyClass>(
            //        new Dictionary<string, string> {
            //            { "MyCode","Concat(Name,' ',Code)"},
            //            { "MyName","Name" }
            //        },
            //        new { code = "XX" }
            //    );
            //var dx = "X,Y,Z".Split(",").Select(p => new MyClass { 

            //    Code=p,
            //    Name=p+"--"
            //});
            //var nl = dx.Select(SX1.Compile()).Cast<dynamic>()
            //    .Select(p=> new { 
            //    Fx=p.MyCode
            //}).ToArray();
            Console.WriteLine("Hello World!");
        }
    }
}
