using System;
using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class TypeTraits<T> where T : IComparable<T>
    {
        public IConverter<T> Converter;
        public PageStorage<T> PageStorage;
        public IOperationTransformer<T> Operations;
        public IResultFactory<T> Results;
        public string DefaultValue;
        public string TypeName;
    }
}
