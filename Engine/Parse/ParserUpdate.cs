using System;
using System.Collections.Generic;
using Engine.Statement;

namespace Engine
{
    public static class ParserUpdate
    {
        public static HashSet<string> Ops = new HashSet<string> { "+", "-", "*", "/"};

        public static Node Convert(string[] tokens, ref int pos)
        {
            ++pos;  // skip "UPDATE"

            var tableName = string.Empty;
            if (pos < tokens.Length)
            {
                tableName = tokens[pos];
                ++pos;
            }
            else
                return null;

            if (ParserCommon.AssertToken("SET", tokens, pos))
                ++pos;
            else
                return null;

            var columnNames = new List<string>();
            var ops = new List<OperationNode>();
            ConditionNode condition = null;
            ConvertColumns(tokens, columnNames, ops, ref pos);

            if (ParserCommon.AssertToken("WHERE", tokens, pos))
            {
                ++pos;

                condition = ParserCondition.Convert(tokens, ref pos);
            }

            if (ParserCommon.AssertToken(";", tokens, pos))
                ++pos;
            else
                return null;

            return new UpdateNode(tableName, columnNames, ops, condition);
        }

        private static void ConvertColumns(string[] tokens, List<string> columnNames, List<OperationNode> ops, ref int pos)
        {
            while (pos < tokens.Length)
            {
                var column = tokens[pos++];
                columnNames.Add(column);  // TODO: check column names
                if (pos >= tokens.Length)
                    break;

                if (ParserCommon.AssertToken("=", tokens, pos))
                    ++pos;
                else
                    break;

                OperationNode opNode = null;
                ConvertOp(tokens, column, ref opNode, ref pos);
                ops.Add(opNode);

                if (ParserCommon.AssertToken(";", tokens, pos) || ParserCommon.AssertToken("WHERE", tokens, pos))
                    break;

                if (ParserCommon.AssertToken(",", tokens, pos))
                    ++pos;
                else
                    break;
            }
        }
        
        private static void ConvertOp(string[] tokens, string column,ref OperationNode opNode, ref int pos)
        {
            var columnOrValue = tokens[pos++];
            if (pos >= tokens.Length)
                throw new Exception($"Unexpected end of query after {columnOrValue}");

            if (ParserCommon.AssertToken(Ops, tokens, pos))
            {
                var op = tokens[pos++];
                var value = tokens[pos++];
                if (column != columnOrValue)
                    throw new Exception("Only same-column operations are supported");
                opNode = new OperationNode {Op = op, Value = value};
            }
            else
            {
                opNode = new OperationNode {Op = OperationNode.Assign, Value = columnOrValue};
            }
        }
    }
}
