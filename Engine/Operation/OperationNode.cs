using System;
using System.Globalization;

namespace Engine.Statement
{
    public class OperationNode
    {
        public const string Assign = "assign";

        public string Op;
        public string Value;
    }

    public interface IOperationTransformer<T>
    {
        OperationGeneric<T> Transform(OperationNode opNode);
    }


    public abstract class OperationGeneric<T>
    {
        public abstract T Perform(T source);
    }

    public class OperationAssignGeneric<T> : OperationGeneric<T>
    {
        private readonly T value;

        public OperationAssignGeneric(string value)
        {
            this.value = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        public override T Perform(T source)
        {
            return value;
        }
    }
}
