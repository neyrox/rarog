using Engine.Statement;
using Engine.Storage;

namespace Engine
{
    public class Registry
    {
        private readonly CacheHost cacheHost = new CacheHost();

        public readonly TypeTraits<int> IntTraits;
        public readonly TypeTraits<long> BigIntTraits;
        public readonly TypeTraits<double> DoubleTraits;
        public readonly TypeTraits<string> StrTraits;

        public Registry(IStreamProvider streams)
        {
            IntTraits = new TypeTraits<int>
            {
                Converter = new IntConverter(),
                PageStorage = new IntPage(streams, cacheHost),
                Operations = new OperationInteger(),
                Results = new IntResultFactory(),
                DefaultValue = "0",
                TypeName = ResultColumnInteger.TypeTag,
            };
            BigIntTraits = new TypeTraits<long>
            {
                Converter = new BigIntConverter(),
                PageStorage = new BigIntPage(streams, cacheHost),
                Operations = new OperationBigInt(),
                Results = new BigIntResultFactory(),
                DefaultValue = "0",
                TypeName = ResultColumnBigInt.TypeTag,
            };
            DoubleTraits = new TypeTraits<double>
            {
                Converter = new DoubleConverter(),
                PageStorage = new DoublePage(streams, cacheHost),
                Operations = new OperationDouble(),
                Results = new DoubleResultFactory(),
                DefaultValue = "0",
                TypeName = ResultColumnDouble.TypeTag,
            };
            StrTraits = new TypeTraits<string>
            {
                Converter = new VarCharConverter(),
                PageStorage = new VarCharPage(streams, cacheHost),
                Operations = new OperationString(),
                Results = new StringResultFactory(),
                DefaultValue = "",
                TypeName = ResultColumnString.TypeTag,
            };
        }
    }
}
