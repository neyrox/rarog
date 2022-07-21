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
    }

    public static class Exceptions
    {
        public static Exception FailedToLockDb()
        {
            return new Exception($"Failed to acquire database lock");
        }

        public static Exception TableNotFound(string tableName)
        {
            return new Exception($"Table \'{tableName}\' was not found");
        }

        public static Exception FailedToLockTable(string tableName)
        {
            return new Exception($"Failed to acquire table \'{tableName}\' lock");
        }
    }
}
