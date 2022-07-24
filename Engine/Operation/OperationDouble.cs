using System;

namespace Engine.Statement
{
    public class OperationDouble : IOperationTransformer<double>
    {
        public OperationGeneric<double> Transform(OperationNode opNode)
        {
            switch (opNode.Op)
            {
                case OperationNode.Assign:
                    return new OperationAssignGeneric<double>(opNode.Value);
                case "+":
                    return new AdditionDouble(opNode.Value);
                case "-":
                    return new SubtractionDouble(opNode.Value);
                case "*":
                    return new MultiplicationDouble(opNode.Value);
                case "/":
                    return new DivisionDouble(opNode.Value);
                default:
                    throw new Exception($"Not supported operation {opNode.Op}");
            }
        }
    }

    public class AdditionDouble : OperationGeneric<double>
    {
        private readonly double value;

        public AdditionDouble(string value)
        {
            this.value = double.Parse(value);
        }

        public override double Perform(double source)
        {
            return source + value;
        }
    }

    public class SubtractionDouble : OperationGeneric<double>
    {
        private readonly double value;

        public SubtractionDouble(string value)
        {
            this.value = double.Parse(value);
        }

        public override double Perform(double source)
        {
            return source - value;
        }
    }
    
    public class MultiplicationDouble : OperationGeneric<double>
    {
        private readonly double value;

        public MultiplicationDouble(string value)
        {
            this.value = double.Parse(value);
        }

        public override double Perform(double source)
        {
            return source * value;
        }
    }

    public class DivisionDouble : OperationGeneric<double>
    {
        private readonly double value;

        public DivisionDouble(string value)
        {
            this.value = double.Parse(value);
        }

        public override double Perform(double source)
        {
            return source / value;
        }
    }
}