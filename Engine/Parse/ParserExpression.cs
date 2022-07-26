using System;
using System.Collections.Generic;

namespace Engine
{
    public static class ParserExpression
    {
        private static readonly HashSet<string> terminators = new HashSet<string> { ",", ";", "FROM" };
        private static readonly Dictionary<string, int> binaryOps = new Dictionary<string, int>
        {
            {"+", 1},
            {"-", 1},
            {"*", 2},
            {"/", 2},
        };
        private static HashSet<string> funcs = new HashSet<string> { "COUNT" };

        public static ExpressionNode Convert(string[] tokens, ref int pos)
        {
            if (pos >= tokens.Length)
                throw new Exception("Unexpected end of query");

            var post = ToPost(tokens, ref pos);
            var stack = new Stack<ExpressionNode>();
            foreach (var item in post)
            {
                if (binaryOps.ContainsKey(item))
                {
                    // TODO: simplify?
                    if (stack.Count > 1)
                    {
                        var r = stack.Pop();
                        var l = stack.Pop();
                        stack.Push(new BinaryOperationNode(l, r, item));
                    }
                    else
                    {
                        stack.Push(new ValueNode(item));
                    }
                }
                else if (funcs.Contains(item))
                {
                    var a = stack.Pop();
                    stack.Push(new FunctionNode(item, a));
                }
                else
                {
                    stack.Push(new ValueNode(item));
                }
            }

            return stack.Pop();
        }

        private static List<string> ToPost(string[] tokens, ref int pos)
        {
            var post = new List<string>();
            var stack = new Stack<string>();

            while (pos < tokens.Length)
            {
                var token = tokens[pos];
                var upperToken = token.ToUpperInvariant();
                if (terminators.Contains(upperToken))
                    break;

                pos++;

                if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        post.Add(stack.Pop());

                    stack.Pop(); // Pop "("
                }
                else if (binaryOps.ContainsKey(token))
                {
                    if (stack.Count == 0 || Preced(token) > Preced(stack.Peek()))
                    {
                        stack.Push(token);
                    }
                    else
                    {
                        while (stack.Count > 0 && Preced(token) <= Preced(stack.Peek()))
                            post.Add(stack.Pop());
                        stack.Push(token);
                    }
                }
                else if (funcs.Contains(token))
                {
                    stack.Push(token);
                }
                else
                {
                    post.Add(token);
                }
            }

            while (stack.Count > 0)
                post.Add(stack.Pop());

            return post;
        }

        private static int Preced(string op)
        {
            if (binaryOps.TryGetValue(op, out var preced))
                return preced;
            return 0;
        }
    }
}
