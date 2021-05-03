using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpr
{
    public class MyClass
    {
        public string Code { get; internal set; }
        public string Name { get; internal set; }
        public decimal Salary { get; set; }
    }
    internal static class Test
    {
        public static void Test20210205()
        {
            Expression<Func<MyClass, object>> px = p => new {

                MyCode = p.Code
            };
            var dict = new Dictionary<string, string>();
            dict.Add("MyCode", "Code");
            dict.Add("MyName", "Name");
            var Pr = Expression.Parameter(typeof(MyClass), "p");
            Expression<Func<MyClass, dynamic>> LD = ExprBuilders.CreateSelector<MyClass>("Code MyCode,Name MyName", new { });

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
            foreach (var x in ret)
            {
                Console.WriteLine(x.MyCode);
            }

        }
    }
}
