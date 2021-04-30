using System;
using System.Linq.Expressions;

namespace DynamicExpr
{
    public class MyClass
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
            var ret=(new StringToFormula()).Eval("not(b)");
            var x = ret.ToString();
            LambdaExpression Where = ExprBuilders.MakeConditional(typeof(MyClass), "code=={0} and name=={1}");
            Console.WriteLine("Hello World!");
        }
    }
}
