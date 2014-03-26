using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.BaseRecords
{
    class QualityRecord:ConfirmableRecordForEquipment
    {
        public string Material
        {
            get;
            set;
        }
        public string Lot
        {
            get;
            set;
        }
        public string QualityType
        {
            get;
            set;
        }
    }
}
