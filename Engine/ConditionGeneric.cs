using System;

namespace Engine
{
    public abstract class Condition<T> where T : IComparable<T>
    {
        public abstract bool Satisfies(T value);

        public static Condition<T> Transform(string operation, string value)
        {
            if (operation == "=")
                return new ConditionEqual<T>(value);
            else if (operation == "<")
                return new ConditionLess<T>(value);
            else if (operation == "<=")
                return new ConditionLessOrEqual<T>(value);
            else if (operation == ">")
                return new ConditionGreater<T>(value);
            else if (operation == ">=")
                return new ConditionGreaterOrEqual<T>(value);
            else if (operation == "<>")
                return new ConditionNotEqual<T>(value);

            throw new ArgumentException($"Unknown operation \'{operation}\' with value \'{value}\'");
        }
    }

    public abstract class ConditionWithOrigin<T> : Condition<T> where T : IComparable<T>
    {
        protected readonly T origin;

        protected ConditionWithOrigin(string origin)
        {
            this.origin = (T)Convert.ChangeType(origin, typeof(T));
        }
    }

    public class ConditionEqual<T> : ConditionWithOrigin<T> where T : IComparable<T>
    {
        public ConditionEqual(string origin)
            : base(origin)
        {
        }

        public override bool Satisfies(T value)
        {
            return value.CompareTo(origin) == 0;
        }
    }

    public class ConditionNotEqual<T> : ConditionWithOrigin<T> where T : IComparable<T>
    {
        public ConditionNotEqual(string origin)
            : base(origin)
        {
        }

        public override bool Satisfies(T value)
        {
            return value.CompareTo(origin) != 0;
        }
    }

    public class ConditionLess<T> : ConditionWithOrigin<T> where T : IComparable<T>
    {
        public ConditionLess(string origin)
            : base(origin)
        {
        }

        public override bool Satisfies(T value)
        {
            return value.CompareTo(origin) < 0;
        }
    }

    public class ConditionLessOrEqual<T> : ConditionWithOrigin<T> where T : IComparable<T>
    {
        public ConditionLessOrEqual(string origin)
            : base(origin)
        {
        }

        public override bool Satisfies(T value)
        {
            return value.CompareTo(origin) <= 0;
        }
    }

    public class ConditionGreater<T> : ConditionWithOrigin<T> where T : IComparable<T>
    {
        public ConditionGreater(string origin)
            : base(origin)
        {
        }

        public override bool Satisfies(T value)
        {
            return value.CompareTo(origin) > 0;
        }
    }

    public class ConditionGreaterOrEqual<T> : ConditionWithOrigin<T> where T : IComparable<T>
    {
        public ConditionGreaterOrEqual(string origin)
            : base(origin)
        {
        }

        public override bool Satisfies(T value)
        {
            return value.CompareTo(origin) >= 0;
        }
    }

    public class ConditionAny<T> : Condition<T> where T : IComparable<T>
    {
        public static ConditionAny<T> Instance => new ConditionAny<T>();

        public override bool Satisfies(T value)
        {
            return true;
        }
    }
}
