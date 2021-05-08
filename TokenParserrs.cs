using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpr
{
    public static class TokenParsers
    {
        const string Quote= "$quote";
        internal const string operators = " ( ) ^ * / + - = != <> and or >= <= > < == ' & : ";
        internal static string[] _operators = { "-", "+", "/", "*", "^", "=", "==", ">=", "<=", "!=", "<>", ">", "<", "and", "or","&" };

        internal static List<string> ApplyQuote(List<string> Tokens)
        {
            var st = new Stack<string>();
           
            var hasFoundQuote = false;
            var IsInQuote = false;
            var currentToken = "";
            for(var i=Tokens.Count-1;i>=0;i--)
            {
                
                if (Tokens[i] == "'")
                {
                    if (currentToken.IndexOf("'") == -1)
                    {
                        currentToken = "'";
                    }
                    else
                    {
                        currentToken = "'"+ currentToken;
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
                        currentToken = Tokens[i]+ currentToken;
                    }
                }
               
            }
            var ret= st.ToList(); 
            
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
    }
}
