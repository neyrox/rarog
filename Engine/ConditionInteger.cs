
namespace Engine
{
    public abstract class ConditionInteger
    {
        public abstract bool Satisfies(int value);

        public static ConditionInteger Transform(string operation, string value)
        {
            if (operation == "=")
                return new ConditionIntegerEqual(value);
            else if (operation == "<")
                return new ConditionIntegerLess(value);
            else if (operation == "<=")
                return new ConditionIntegerLessOrEqual(value);
            else if (operation == ">")
                return new ConditionIntegerGreater(value);
            else if (operation == ">=")
                return new ConditionIntegerGreaterOrEqual(value);
            else if (operation == "<>")
                return new ConditionIntegerNotEqual(value);
            else
                return null;
        }
    }

    public class ConditionIntegerEqual : ConditionInteger
    {
        private int origin;

        public ConditionIntegerEqual(string origin)
        {
            this.origin = int.Parse(origin);
        }

        public override bool Satisfies(int value)
        {
            return value == origin;
        }
    }

    public class ConditionIntegerNotEqual : ConditionInteger
    {
        private int origin;

        public ConditionIntegerNotEqual(string origin)
        {
            this.origin = int.Parse(origin);
        }

        public override bool Satisfies(int value)
        {
            return value != origin;
        }
    }

    public class ConditionIntegerLess : ConditionInteger
    {
        private int origin;

        public ConditionIntegerLess(string origin)
        {
            this.origin = int.Parse(origin);
        }

        public override bool Satisfies(int value)
        {
            return value < origin;
        }
    }

    public class ConditionIntegerLessOrEqual : ConditionInteger
    {
        private int origin;

        public ConditionIntegerLessOrEqual(string origin)
        {
            this.origin = int.Parse(origin);
        }

        public override bool Satisfies(int value)
        {
            return value <= origin;
        }
    }

    public class ConditionIntegerGreater : ConditionInteger
    {
        private int origin;

        public ConditionIntegerGreater(string origin)
        {
            this.origin = int.Parse(origin);
        }

        public override bool Satisfies(int value)
        {
            return value > origin;
        }
    }

    public class ConditionIntegerGreaterOrEqual : ConditionInteger
    {
        private int origin;

        public ConditionIntegerGreaterOrEqual(string origin)
        {
            this.origin = int.Parse(origin);
        }

        public override bool Satisfies(int value)
        {
            return value >= origin;
        }
    }

    public class ConditionIntegerAny : ConditionInteger
    {
        public override bool Satisfies(int value)
        {
            return true;
        }
    }
}
