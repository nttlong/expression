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
            var mm = "x=y or x=1 and n=2".ToTokens();
            var rx = mm.FixBinaryLogical("=");//.FixBinaryLogical("and").FixBinaryLogical("or");//.FixBinaryLogical("<>");
            Console.WriteLine(rx.RebuilFromTokens());
            
            Console.WriteLine("Hello World!");
        }
    }
}
