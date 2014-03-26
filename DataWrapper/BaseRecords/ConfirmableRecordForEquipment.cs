using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.BaseRecords
{
    public class ConfirmableRecordForEquipment:BaseRecord
    {
        public string IsConfirmed
        {
            get;
            set;
        }
        public string EquipmentId { get; set; }
    }
}
