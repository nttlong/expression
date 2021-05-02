using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicExpr
{
    public class MyClass
    {
        public string Code { get; internal set; }
        public string Name { get; internal set; }
        public decimal Salary { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<MyClass, object>> px = p => new { 
            
                MyCode=p.Code
            };
            var dict = new Dictionary<string, string>();
            dict.Add("MyCode", "Code");
            dict.Add("MyName", "Name");
            var Pr = Expression.Parameter(typeof(MyClass), "p");
            Expression<Func<MyClass,dynamic>> LD = ExprBuilders.CreateSelector<MyClass>("Code MyCode,Name MyName", new { });
            
            var lst = new List<MyClass>();
            lst.Add(new MyClass
            {
                Code = "123"
            });

            lst.Add(new MyClass
            {
                Code = "1234"
            });
            var ret = lst.Select(LD.Compile()).ToList();
            foreach(var x in ret)
            {
                Console.WriteLine(x.MyCode);
            }

            //var vv =(decimal) Math.Pow((d ouble)1.4, (double)1.22);
            //var x = "Concat(Code,xx,name)".FixSpace().FixBracket().ToFxExpression();
            //Expression<Func<MyClass, string>> Filter = p => p.Code+p.Name; 
            //var Where = ExprBuilders.ToLambdaExpression(typeof(MyClass),
            //    "2+salary^2+2",
            //    new { 
            //        name="X",
            //        codse="CCC"

            //});
            //var fx=Where.Compile().DynamicInvoke(new MyClass { Name="CCC",Code="X" });
            Console.WriteLine("Hello World!");
        }
    }
}
