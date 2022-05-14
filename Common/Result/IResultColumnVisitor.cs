using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IResultColumnVisitor
    {
        void Visit(ResultColumnDouble column);
        void Visit(ResultColumnInteger column);
        void Visit(ResultColumnString column);
    }
}
