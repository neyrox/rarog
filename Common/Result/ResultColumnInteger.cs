﻿namespace Engine
{
    public class ResultColumnInteger: ResultColumnBase<int>
    {
        public const string TypeTag = "Int";

        public ResultColumnInteger(string name, int[] vals)
            : base(name, vals)
        {
        }

        public override void Accept(IResultColumnVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
