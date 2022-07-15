
namespace Engine
{
    public abstract class ConditionDouble
    {
        public abstract bool Satisfies(double value);

        public static ConditionDouble Transform(string operation, string value)
        {
            if (operation == "=")
                return new ConditionDoubleEqual(value);
            else if (operation == "<")
                return new ConditionDoubleLess(value);
            else if (operation == "<=")
                return new ConditionDoubleLessOrEqual(value);
            else if (operation == ">")
                return new ConditionDoubleGreater(value);
            else if (operation == ">=")
                return new ConditionDoubleGreaterOrEqual(value);
            else if (operation == "<>")
                return new ConditionDoubleNotEqual(value);
            else
                return null;
        }
    }

    public class ConditionDoubleEqual : ConditionDouble
    {
        private double origin;

        public ConditionDoubleEqual(string origin)
        {
            this.origin = double.Parse(origin);
        }

        public override bool Satisfies(double value)
        {
            return value == origin;
        }
    }

    public class ConditionDoubleNotEqual : ConditionDouble
    {
        private double origin;

        public ConditionDoubleNotEqual(string origin)
        {
            this.origin = double.Parse(origin);
        }

        public override bool Satisfies(double value)
        {
            return value != origin;
        }
    }

    public class ConditionDoubleLess : ConditionDouble
    {
        private double origin;

        public ConditionDoubleLess(string origin)
        {
            this.origin = double.Parse(origin);
        }

        public override bool Satisfies(double value)
        {
            return value < origin;
        }
    }

    public class ConditionDoubleLessOrEqual : ConditionDouble
    {
        private double origin;

        public ConditionDoubleLessOrEqual(string origin)
        {
            this.origin = double.Parse(origin);
        }

        public override bool Satisfies(double value)
        {
            return value <= origin;
        }
    }

    public class ConditionDoubleGreater : ConditionDouble
    {
        private double origin;

        public ConditionDoubleGreater(string origin)
        {
            this.origin = double.Parse(origin);
        }

        public override bool Satisfies(double value)
        {
            return value > origin;
        }
    }

    public class ConditionDoubleGreaterOrEqual : ConditionDouble
    {
        private double origin;

        public ConditionDoubleGreaterOrEqual(string origin)
        {
            this.origin = double.Parse(origin);
        }

        public override bool Satisfies(double value)
        {
            return value >= origin;
        }
    }

    public class ConditionDoubleAny : ConditionDouble
    {
        public override bool Satisfies(double value)
        {
            return true;
        }
    }
}
