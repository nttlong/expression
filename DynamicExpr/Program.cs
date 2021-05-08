using System;
using System.Linq.Expressions;

namespace DynamicExpr
{
    public class MyClass
    {
        public MyClass()
        {
            this.Name = "BBB";
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            
            var ret=(new StringToFormula()).Eval("(b)");
            var x = ret.ToString();
            LambdaExpression Where = ExprBuilders.MakeConditional(typeof(MyClass), "code=={0} and name=={1}",new { 
                code="XXXX",
                name="BBB"
            
            
            });
            Console.WriteLine("Hello World!");
        }
    }
}
