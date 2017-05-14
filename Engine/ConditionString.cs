
namespace Engine
{
    public abstract class ConditionString
    {
        public abstract bool Satisfies(string value);

        public static ConditionString Transform(ConditionNode conditionNode)
        {
            if (conditionNode.Operation == "=")
                return new ConditionStringEqual(conditionNode.Value);
            else if (conditionNode.Operation == "<")
                return new ConditionStringLess(conditionNode.Value);
            else if (conditionNode.Operation == "<=")
                return new ConditionStringLessOrEqual(conditionNode.Value);
            else if (conditionNode.Operation == ">")
                return new ConditionStringGreater(conditionNode.Value);
            else if (conditionNode.Operation == ">=")
                return new ConditionStringGreaterOrEqual(conditionNode.Value);
            else if (conditionNode.Operation == "<>")
                return new ConditionStringNotEqual(conditionNode.Value);
            else
                return null;
        }
    }

    public class ConditionStringEqual: ConditionString
    {
        private string origin;

        public ConditionStringEqual(string origin)
        {
            this.origin = origin;
        }

        public override bool Satisfies(string value)
        {
            return value == origin;
        }
    }

    public class ConditionStringNotEqual: ConditionString
    {
        private string origin;

        public ConditionStringNotEqual(string origin)
        {
            this.origin = origin;
        }

        public override bool Satisfies(string value)
        {
            return value != origin;
        }
    }

    public class ConditionStringLess: ConditionString
    {
        private string origin;

        public ConditionStringLess(string origin)
        {
            this.origin = origin;
        }

        public override bool Satisfies(string value)
        {
            return value.CompareTo(origin) < 0;
        }
    }

    public class ConditionStringLessOrEqual: ConditionString
    {
        private string origin;

        public ConditionStringLessOrEqual(string origin)
        {
            this.origin = origin;
        }

        public override bool Satisfies(string value)
        {
            return value.CompareTo(origin) <= 0;
        }
    }

    public class ConditionStringGreater : ConditionString
    {
        private string origin;

        public ConditionStringGreater(string origin)
        {
            this.origin = origin;
        }

        public override bool Satisfies(string value)
        {
            return value.CompareTo(origin) > 0;
        }
    }

    public class ConditionStringGreaterOrEqual : ConditionString
    {
        private string origin;

        public ConditionStringGreaterOrEqual(string origin)
        {
            this.origin = origin;
        }

        public override bool Satisfies(string value)
        {
            return value.CompareTo(origin) >= 0;
        }
    }
}
