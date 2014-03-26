using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.BaseRecords
{
    class ProductionRecord:BaseRecord, IComparable<ProductionRecord>
    {
        public int CompareTo(ProductionRecord other)
        {
            throw new NotImplementedException();
        }
    }
}
