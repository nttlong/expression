using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpr
{
    public class BExpr
    {

        public string Operator { get; internal set; }
        public object Left { get; internal set; }
        public object Right { get; internal set; }
        public override string ToString()
        {
            return "("+Left.ToString()+")"+Operator+"("+Right.ToString()+")";
        }
    }
}
