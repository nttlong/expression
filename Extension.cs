﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DynamicExpr
{
    public static class Extension
    {
        internal static string FixBracket(this string Expr)
        {
            var R = new Regex(@"/not|and|x?or|&&|[<>!=]=|[<>&!]|\|{1,2}|=|=",RegexOptions.IgnoreCase);
            var ps=R.Matches(Expr);
            if (ps.Count == 0) return Expr;
            var index = 0;
            if (ps.Count > 1)
            {
                index = ps.Count % 2;
            }
            var p = ps[index];
            //if (!p.Success) return Expr;
            var op = p.Value;
            var strLeft = Expr.Substring(0, p.Index);
            var strRight = Expr.Substring(p.Index + p.Value.Length, Expr.Length - p.Index - p.Value.Length);
            return "(" + strLeft.FixBracket() + " " + op + " " + strRight.FixBracket() + ")";
            
            
        }
        internal static string FixSpace(this string Expr)
        {
            var isTurnOn = false;
            var ret = "";
            foreach(var c in Expr)
            {
                var C = c.ToString();
                if (c == '[')
                {
                    isTurnOn = true;
                }
                else if (c == ']')
                {
                    isTurnOn = false;
                }
                else if(c==' ' && isTurnOn)
                {
                    C = "_";
                    ret += C;
                }
                else
                {
                    ret += C;
                }
                
            }
            return ret;
        }
        public static FXExpr ToFxExpression(this string Expr)
        {
            return (new StrParser()).Eval(Expr);
        }
    }
}
