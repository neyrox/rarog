using System;
using System.Collections.Generic;

namespace Engine
{
    public class Result
    {
        public static readonly Result OK = new Result(null, null);

        public readonly string Error;
        public readonly List<List<string>> Rows;

        public bool IsOK { get { return Error == null; } }

        public Result(List<List<string>> rows, string error = null)
        {
            Error = error;
            Rows = rows;
        }

        public static Result TableNotFound(string tableName)
        {
            return new Result(null, String.Format("Table \'{}\' not found", tableName));
        }
    }
}
