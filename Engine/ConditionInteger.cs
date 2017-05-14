
namespace Engine
{
    public abstract class ConditionInteger
    {
        public abstract bool Satisfies(int value);

        public static ConditionInteger Transform(ConditionNode conditionNode)
        {
            if (conditionNode.Operation == "=")
                return new ConditionIntegerEqual(conditionNode.Value);
            else if (conditionNode.Operation == "<")
                return new ConditionIntegerLess(conditionNode.Value);
            else if (conditionNode.Operation == "<=")
                return new ConditionIntegerLessOrEqual(conditionNode.Value);
            else if (conditionNode.Operation == ">")
                return new ConditionIntegerGreater(conditionNode.Value);
            else if (conditionNode.Operation == ">=")
                return new ConditionIntegerGreaterOrEqual(conditionNode.Value);
            else if (conditionNode.Operation == "<>")
                return new ConditionIntegerNotEqual(conditionNode.Value);
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
            return value > origin;
        }
    }
}
