using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpr
{
    public class BExpr:FXExpr
    {

        public string Operator { get; internal set; }
        public FXExpr Left { get; internal set; }
        public FXExpr Right { get; internal set; }
        public override string ToString()
        {
            return "("+Left.ToString()+")"+Operator+"("+Right.ToString()+")";
        }
    }
}
