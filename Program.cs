using System;
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
            var vv =(decimal) Math.Pow((double)1.4, (double)1.22);
            var x = "Concat(Code,xx,name)".FixSpace().FixBracket().ToFxExpression();
            Expression<Func<MyClass, string>> Filter = p => p.Code+p.Name; 
            var Where = ExprBuilders.ToLambdaExpression(typeof(MyClass),
                "2+salary^2+2",
                new { 
                    name="X",
                    codse="CCC"
            
            });
            var fx=Where.Compile().DynamicInvoke(new MyClass { Name="CCC",Code="X" });
            Console.WriteLine("Hello World!");
        }
    }
}
