using System;

namespace Engine.Statement
{
    public static class OperationString
    {
        public static OperationGeneric<string> Transform(OperationNode opNode)
        {
            switch (opNode.Op)
            {
                case OperationNode.Assign:
                    return new OperationAssignGeneric<string>(opNode.Value);
                case "+":
                    return new AdditionString(opNode.Value);
                default:
                    throw new Exception($"Not supported operation {opNode.Op}");
            }
        }
    }
    
    public class AdditionString : OperationGeneric<string>
    {
        private readonly string value;

        public AdditionString(string value)
        {
            this.value = value;
        }

        public override string Perform(string source)
        {
            return source + value;
        }
    }
}
