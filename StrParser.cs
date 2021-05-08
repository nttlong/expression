using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamicExpr
{
    internal class StrParser
    {
        
       

        public FXExpr Eval(string expression)
        {
            List<string> tokens =TokenParsers.GetTokens(expression);
            Stack<FXExpr> operandStack = new Stack<FXExpr>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;
           
            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                FXExpr preOperand = null;
                if(token[0]=='{' && token[token.Length - 1] == '}')
                {
                    operandStack.Push(new PExpr(token));
                    tokenIndex += 1;
                    continue;
                }
                if (token == "(")
                {
                    
                    if (operandStack.Count > 0)
                    {
                        preOperand= operandStack.Pop();
                    }
                    
                    if (preOperand is NExpr)
                    {
                        
                        string subExpr = getSubExpression(tokens, ref tokenIndex);
                        var fx = new StrParser();
                        var indexOfCom = -1;
                        List<FXExpr> _args = null;
                        if ((indexOfCom=subExpr.IndexOf(",")) > -1)
                        {
                            var left = (new StrParser()).Eval(subExpr.Split(',')[0]);
                            var rStr = subExpr.Substring(indexOfCom + 1, subExpr.Length - indexOfCom - 1);
                            var right = (new StrParser()).Eval(subExpr.Substring(indexOfCom+1, subExpr.Length-indexOfCom-1));
                            _args=ParseArgs(right);
                            _args.Insert(0, left);
                        }
                        var args=fx.Eval(subExpr);
                        var ret = new FExpr()
                        {
                            Name= (preOperand as NExpr).Value,
                            Args = _args??(new FXExpr[] { args }).ToList()
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
                if (Array.IndexOf(TokenParsers._operators, token) >= 0)
                {
                    while (operatorStack.Count > 0 && Array.IndexOf(TokenParsers._operators, token) < Array.IndexOf(TokenParsers._operators, operatorStack.Peek()))
                    {
                        string op = operatorStack.Pop();
                        var arg2 = operandStack.Pop();
                        var arg1 = operandStack.Pop();
                        var BExpr = new BExpr
                        {
                            Operator= TokenParsers._operators[Array.IndexOf(TokenParsers._operators, op)],
                            Left=arg1,
                            Right=arg2
                        };
                        operandStack.Push(BExpr);
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(new NExpr(token));
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
                    Operator= TokenParsers._operators[Array.IndexOf(TokenParsers._operators, op)]
                };
                operandStack.Push(ret);
            }
            return operandStack.Pop();
        }

        private List<FXExpr> ParseArgs(FXExpr Expr)
        {
            if(Expr is NExpr)
            {
                var vExpr = Expr as NExpr;
                return 
                    vExpr.Value.Split(',')
                    .Select(p => (new StrParser()).Eval(p))
                    .ToList();
            }
            if(Expr is BExpr)
            {
                var bExpr = Expr as BExpr;
                var lItems = ParseArgs(bExpr.Left);
                var lX = lItems[lItems.Count - 1];
                var rItems = ParseArgs(bExpr.Right);
                var rX = rItems[0];
                var ret = new List<FXExpr>();
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
                return (new FXExpr[] { Expr }).ToList();
            }
            if (Expr is PExpr)
            {
                return (new FXExpr[] { Expr }).ToList();
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

       
    }
    
}
