using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpr
{
    public class BExpr: FxExpr
    {
        public string Operator { get; internal set; }
        public FxExpr Left { get; internal set; }
        public FxExpr Right { get; internal set; }
        public override string ToString()
        {
            return "("+Left.ToString()+")"+Operator+"("+Right.ToString()+")";
        }
    }
}
