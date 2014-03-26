using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.MESCC.DAL.DataWrapper.BaseRecords
{
    /// <summary>
    /// Represents a standard Ampla Planning record without any custom fields
    /// </summary>
    public class PlanningRecord:BaseRecord
    {
        public string StateChangedBy { get; set; }
        public string StateChangeDateTime { get; set; }
        public DateTime PlannedStartDateTime { get; set; }
        public DateTime PlannedEndDateTime { get; set; }
        public int PlannedDuration { get; set; }
        public DateTime ActualStartDateTime { get; set; }
        public DateTime ActualEndDateTime { get; set; }
        public int ActualDuration { get; set; }
        public int ActivityId { get; set; }
        public string Product { get; set; }
        public double RequiredQuantity { get; set; }
        public string ReauiredQuantityUnits { get; set; }
        public string State { get; set; }
    }
}
