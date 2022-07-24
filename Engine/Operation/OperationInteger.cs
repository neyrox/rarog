using System;

namespace Engine.Statement
{
    public class OperationInteger : IOperationTransformer<int>
    {
        public OperationGeneric<int> Transform(OperationNode opNode)
        {
            switch (opNode.Op)
            {
                case OperationNode.Assign:
                    return new OperationAssignGeneric<int>(opNode.Value);
                case "+":
                    return new AdditionInteger(opNode.Value);
                case "-":
                    return new SubtractionInteger(opNode.Value);
                case "*":
                    return new MultiplicationInteger(opNode.Value);
                case "/":
                    return new DivisionInteger(opNode.Value);
                default:
                    throw new Exception($"Operation {opNode.Op} is not supported");
            }
        }
    }

    public class AdditionInteger : OperationGeneric<int>
    {
        private readonly int value;

        public AdditionInteger(string value)
        {
            this.value = int.Parse(value);
        }

        public override int Perform(int source)
        {
            return source + value;
        }
    }

    public class SubtractionInteger : OperationGeneric<int>
    {
        private readonly int value;

        public SubtractionInteger(string value)
        {
            this.value = int.Parse(value);
        }

        public override int Perform(int source)
        {
            return source - value;
        }
    }
    
    public class MultiplicationInteger : OperationGeneric<int>
    {
        private readonly double value;

        public MultiplicationInteger(string value)
        {
            this.value = double.Parse(value);
        }

        public override int Perform(int source)
        {
            return (int)Math.Round(source * value);
        }
    }

    public class DivisionInteger : OperationGeneric<int>
    {
        private readonly double value;

        public DivisionInteger(string value)
        {
            this.value = double.Parse(value);
        }

        public override int Perform(int source)
        {
            return (int)Math.Round(source / value);
        }
    }
}