using System;
using System.Globalization;

namespace Engine.Statement
{
    public class OperationBigInt : IOperationTransformer<long>
    {
        public OperationGeneric<long> Transform(OperationNode opNode)
        {
            switch (opNode.Op)
            {
                case OperationNode.Assign:
                    return new OperationAssignGeneric<long>(opNode.Value);
                case "+":
                    return new AdditionBigInt(opNode.Value);
                case "-":
                    return new SubtractionBigInt(opNode.Value);
                case "*":
                    return new MultiplicationBigInt(opNode.Value);
                case "/":
                    return new DivisionBigInt(opNode.Value);
                default:
                    throw new Exception($"Not supported operation {opNode.Op}");
            }
        }
    }
    
    public class AdditionBigInt : OperationGeneric<long>
    {
        private readonly long value;

        public AdditionBigInt(string value)
        {
            this.value = long.Parse(value);
        }

        public override long Perform(long source)
        {
            return source + value;
        }
    }

    public class SubtractionBigInt : OperationGeneric<long>
    {
        private readonly long value;

        public SubtractionBigInt(string value)
        {
            this.value = long.Parse(value);
        }

        public override long Perform(long source)
        {
            return source - value;
        }
    }
    
    public class MultiplicationBigInt : OperationGeneric<long>
    {
        private readonly double value;

        public MultiplicationBigInt(string value)
        {
            this.value = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        public override long Perform(long source)
        {
            return (long)Math.Round(source * value);
        }
    }

    public class DivisionBigInt : OperationGeneric<long>
    {
        private readonly double value;

        public DivisionBigInt(string value)
        {
            this.value = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        public override long Perform(long source)
        {
            return (long)Math.Round(source / value);
        }
    }
}