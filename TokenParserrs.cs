using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpr
{
    public static class TokenParsers
    {
        internal const string logicOps = " and or not ";
        internal const string operators = " ( ) ^ * / + - = != <> and or >= <= > < == ' & : ";
        internal static string[] _operators = { "-", "+", "/", "*", "^", "=", "==", ">=", "<=", "!=", "<>", ">", "<", "and", "or", "&" };

        internal static List<string> ApplyQuote(List<string> Tokens)
        {
            var st = new Stack<string>();

            var hasFoundQuote = false;
            var IsInQuote = false;
            var currentToken = "";
            for (var i = Tokens.Count - 1; i >= 0; i--)
            {

                if (Tokens[i] == "'")
                {
                    if (currentToken.IndexOf("'") == -1)
                    {
                        currentToken = "'";
                    }
                    else
                    {
                        currentToken = "'" + currentToken;
                        st.Push(currentToken);
                        currentToken = "";
                    }

                }
                else
                {

                    if (currentToken.IndexOf("'") == -1)
                    {
                        st.Push(Tokens[i]);
                    }
                    else
                    {
                        currentToken = Tokens[i] + currentToken;
                    }
                }

            }
            var ret = st.ToList();

            return ret;
        }

        internal static List<string> GetTokens(string expression)
        {

            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            for (var i = 0; i < expression.Length; i++)//.Replace(" ", string.Empty))
            {
                var c = expression[i].ToString();
                if (i + 1 < expression.Length)
                {
                    c += expression[i + 1].ToString();
                }
                if (operators.IndexOf(" " + c + " ") >= 0)
                {
                    if ((sb.Length > 0))
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    tokens.Add(c.ToString());
                    i++;
                }
                else
                {
                    c = expression[i].ToString();
                    if (operators.IndexOf(" " + c + " ") >= 0)
                    {
                        if ((sb.Length > 0))
                        {
                            tokens.Add(sb.ToString());
                            sb.Length = 0;
                        }
                        tokens.Add(c.ToString());
                    }
                    else
                    {
                        if (c != " ")
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            tokens.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
                }

            }

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            tokens = tokens.Where(p => !string.IsNullOrEmpty(p)).ToList();
            return ApplyQuote(tokens);
        }
        internal static List<string> ToTokens(this string txt)
        {
            return GetTokens(txt).FixUnaryLogical("not");
        }
        internal static List<string> FixUnaryLogical(this List<string> Tokens, string op)
        {
            var ret = new Stack<string>();
            for (var i = 0; i < Tokens.Count; i++)
            {
                var ck = Tokens[i];
                if (ck == op)
                {
                    i++;
                    var n = Tokens[i];
                    if (n == "(")
                    {
                        ret.Push(ck);
                        ret.Push(n);
                        i++;
                        while (i < Tokens.Count && ((n = Tokens[i]) != ")"))
                        {
                            ret.Push(n);
                            i++;
                        }
                        ret.Push(")");
                        continue;
                    }
                    else
                    {
                        ret.Push(ck);
                        ret.Push("(");
                        ret.Push(n);
                        i++;
                        while (i < Tokens.Count && (logicOps.IndexOf(" " + Tokens[i] + " ") == -1))
                        {
                            ret.Push(Tokens[i]);
                            i++;
                        }
                        ret.Push(")");
                    }
                    continue;
                }
                ret.Push(ck);
            }
            var fx = ret.Reverse().ToList();
            return fx;
        }
        internal static List<string> FixBinaryLogical(this List<string> Tokens, string op)
        {
            var st = new Stack<string>();
            var bcount = 0;
            for (var i = 0; i < Tokens.Count; i++)
            {
                var tk = Tokens[i];
                if (tk != op)
                {
                    st.Push(tk);
                    continue;
                }
                var l = st.Pop();
                if (l != ")")
                {
                    var stl = new Stack<string>();

                    stl.Push(")");
                    stl.Push(l);
                   
                    while (st.Count > 0 && logicOps.IndexOf(" " + l + " ") == -1)
                    {
                        l = st.Pop();
                       
                        stl.Push(l);
                        
                    }


                    
                    stl.Push("(");
                    while (stl.Count > 0)
                    {
                        st.Push(stl.Pop());
                    }

                    st.Push(tk);
                }
                else
                {
                    //st.Push(l);
                    st.Push(tk);
                }
                i++;
                
                st.Push("(");
                while (i < Tokens.Count && logicOps.IndexOf(" " + Tokens[i] + " ") == -1)
                {
                    if (Tokens[i] == op)
                    {
                        break;
                    }
                    st.Push(Tokens[i++]);
                }

                st.Push(")");
                if (i < Tokens.Count)
                {
                    st.Push(Tokens[i]);
                }

            }
            if (bcount > 0)
                st.Push(")");
            var ret = st.Reverse().ToList();

            return ret;
        }
        internal static string RebuilFromTokens(this List<string> Tokens)
        {
            return string.Join(" ", Tokens.ToArray());
        }
    }
}
