using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpr
{
    public class StringToFormula
    {
        const string operators = " ( ) ^ * / + - = != <> and or ";
        private  string[] _operators = { "-", "+", "/", "*", "^","=","!=","<>","and","or"};
       

        public FxExpr Eval(string expression)
        {
            List<string> tokens = getTokens(expression);
            Stack<FxExpr> operandStack = new Stack<FxExpr>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;
           
            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                FxExpr preOperand = null;
                
                if (token == "(")
                {
                    
                    if (operandStack.Count > 0)
                    {
                        preOperand= operandStack.Pop();
                    }
                    
                    if (preOperand is VExpr)
                    {
                        
                        string subExpr = getSubExpression(tokens, ref tokenIndex);
                        var fx = new StringToFormula();
                        var indexOfCom = -1;
                        List<FxExpr> _args = null;
                        if ((indexOfCom=subExpr.IndexOf(",")) > -1)
                        {
                            var left = (new StringToFormula()).Eval(subExpr.Split(',')[0]);
                            var rStr = subExpr.Substring(indexOfCom + 1, subExpr.Length - indexOfCom - 1);
                            var right = (new StringToFormula()).Eval(subExpr.Substring(indexOfCom+1, subExpr.Length-indexOfCom-1));
                            _args=ParseArgs(right);
                            _args.Insert(0, left);
                        }
                        var args=fx.Eval(subExpr);
                        var ret = new FExpr()
                        {
                            Name= (preOperand as VExpr).Value,
                            Args = _args??(new FxExpr[] { args }).ToList()
                        };
                        //var args = Eval(subExpr);
                        operandStack.Push(ret);
                    }
                    else
                    {
                        string subExpr = getSubExpression(tokens, ref tokenIndex);
                        if (preOperand != null)
                        {
                            operandStack.Push(preOperand);
                        }
                        operandStack.Push(Eval(subExpr));
                    }
                    continue;
                }
                if (token == ")")
                {
                    throw new ArgumentException("Mis-matched parentheses in expression");
                }
                //If this is an operator  
                if (Array.IndexOf(_operators, token) >= 0)
                {
                    while (operatorStack.Count > 0 && Array.IndexOf(_operators, token) < Array.IndexOf(_operators, operatorStack.Peek()))
                    {
                        string op = operatorStack.Pop();
                        var arg2 = operandStack.Pop();
                        var arg1 = operandStack.Pop();
                        var BExpr = new BExpr
                        {
                            Operator= _operators[Array.IndexOf(_operators, op)],
                            Left=arg1,
                            Right=arg2
                        };
                        operandStack.Push(BExpr);
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(new VExpr(token));
                }
                tokenIndex += 1;
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                var arg2 = operandStack.Pop();
                var arg1 = operandStack.Pop();
                var ret = new BExpr
                {
                    Left=arg1,
                    Right=arg2,
                    Operator= this._operators[Array.IndexOf(_operators, op)]
                };
                operandStack.Push(ret);
            }
            return operandStack.Pop();
        }

        private List<FxExpr> ParseArgs(FxExpr Expr)
        {
            if(Expr is VExpr)
            {
                var vExpr = Expr as VExpr;
                return 
                    vExpr.Value.Split(',')
                    .Select(p => (new StringToFormula()).Eval(p))
                    .ToList();
            }
            if(Expr is BExpr)
            {
                var bExpr = Expr as BExpr;
                var lItems = ParseArgs(bExpr.Left);
                var lX = lItems[lItems.Count - 1];
                var rItems = ParseArgs(bExpr.Right);
                var rX = rItems[0];
                var ret = new List<FxExpr>();
                ret.AddRange(lItems.Where(p => p != lX));
                ret.Add(new BExpr { 
                    Left=lX,
                    Right=rX,
                    Operator=bExpr.Operator
                });
                ret.AddRange(rItems.Where(p => p != rX));
                return ret;
            }
            if(Expr is FExpr)
            {
                return (new FxExpr[] { Expr }).ToList();
            }
            throw new NotImplementedException();
        }

        private static string getSubExpression(List<string> tokens, ref int index)
        {
            StringBuilder subExpr = new StringBuilder();
            int parenlevels = 1;
            index += 1;
            while (index < tokens.Count && parenlevels > 0)
            {
                string token = tokens[index];
                if (tokens[index] == "(")
                {
                    parenlevels += 1;
                }

                if (tokens[index] == ")")
                {
                    parenlevels -= 1;
                }

                if (parenlevels > 0)
                {
                    subExpr.Append(token);
                }

                index += 1;
            }

            if ((parenlevels > 0))
            {
                throw new ArgumentException("Mis-matched parentheses in expression");
            }
            return subExpr.ToString();
        }

        internal static List<string> getTokens(string expression)
        {
            
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (char c in expression)//.Replace(" ", string.Empty))
            {
                if (operators.IndexOf(" "+c+" ") >= 0)
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
                    if (c != ' ')
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

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            tokens= tokens.Where(p=>!string.IsNullOrEmpty(p)).ToList();
            return tokens;
        }
    }
    public static class ExprBuilders
    {
        public static List<FxExpr> ToFormal(this string Expr,Type type)
        {
            return "";
        }
        public static LambdaExpression MakeConditional(Type type, string StrExpr, var p)
        {
            throw new NotFiniteNumberException();
        }
    }
}
