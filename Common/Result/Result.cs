using System;
using System.Collections.Generic;

namespace Engine
{
    public class Result
    {
        public static readonly Result OK = new Result(null, null);

        public readonly string Error;
        public readonly List<ResultColumn> Columns;

        public bool IsOK { get { return Error == null; } }

        public Result(List<ResultColumn> columns, string error = null)
        {
            Error = error;
            Columns = columns;
        }

        public static Result TableNotFound(string tableName)
        {
            return new Result(null, $"Table \'{tableName}\' was not found");
        }
    }
}
