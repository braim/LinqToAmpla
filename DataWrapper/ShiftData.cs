using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SE.MESCC.Settings;
using SE.MESCC.DAL.DataWrapper;
using SE.MESCC.DAL.DataWrapper.BaseRecords;


namespace SE.MESCC.DAL.DataWrapper
{
    public class ShiftData
    {

        public BaseRecordList<DownTimeRecord> DownTimeRecords { get; set; }


        public string ConsumablesComment { get; set; }
       // public List<ConsumablesRecord> LongwallConsumables;
       // public List<ConsumablesRecord> DevelopmentConsumables;
       // public string LongwallconsumablesComment { get; set; }
       // public string DevelopmentConsumablesComment { get; set; }
        //public int AttendeesOpMaint { get; set; }
        //public int AttendeesMaintOp { get; set; }
        //public int PlannedAbsence { get; set; }
        //public int AlternateDuties { get; set; }
        //public string ShiftLogComments { get; set; }
        public DateTime Date { get; set; }
        public string ShiftName { get; set; }
        #region statics
        public static DateTime GetLastShiftDate()
        {
            for (int i = 0; i < AmplaSettings.Default.Shifts.Count; i++)
            {
                ShiftConfigElement s = AmplaSettings.Default.Shifts[i];
                if (IsInRange(s.Start, s.End, DateTime.Now.TimeOfDay))// find current
                    
                {
                   // return one back
                    if (i > 0) return DateTime.Now.Date;
                    else return DateTime.Now.Date.AddDays(-1);
                }
            }
            throw new DataWrapperCustomException(new string[] { "Exception finding name of shift for current time. time=" + DateTime.Now.ToShortTimeString() });
        }
        public static string GetLastShiftName()
        {
            for (int i = 0; i < AmplaSettings.Default.Shifts.Count; i++)
            {
                ShiftConfigElement s = AmplaSettings.Default.Shifts[i];
                if (IsInRange(s.Start, s.End, DateTime.Now.TimeOfDay))// find current
                    
                {
                   // return one back
                    if (i > 0) return AmplaSettings.Default.Shifts[i - 1].Name;
                    else return AmplaSettings.Default.Shifts[AmplaSettings.Default.Shifts.Count - 1].Name;
                }
            }
            throw new DataWrapperCustomException(new string[] { "Exception finding name of shift for current time. time=" + DateTime.Now.ToShortTimeString() });
        }
        public static string GetCurrentShiftName()
        {
            foreach (ShiftConfigElement s in AmplaSettings.Default.Shifts)
            {
                if (IsInRange(s.Start, s.End, DateTime.Now.TimeOfDay)) return s.Name;
                
            }
            throw new DataWrapperCustomException(new string[] { "Exception finding name of shift for current time. time=" + DateTime.Now.ToShortTimeString() });
        }
        public static bool IsInRange(TimeSpan t1,TimeSpan t2,TimeSpan v){
            if (t2 < t1)// passed over 24
            {
                if (v > t1 || v <= t2) return true;
                return false;
            }
            else
            {
                if (v > t1 && v <= t2) return true;
                return false;
            }
                
        }
        #endregion


        public static string GetShiftFullname(DateTime shiftdate, string shiftname)
        {
            for(int i=0;i<AmplaSettings.Default.Shifts.Count;i++)
            {
                if(AmplaSettings.Default.Shifts[i].Name == shiftname)
                {
                    if (AmplaSettings.Default.Shifts[i].End > AmplaSettings.Default.Shifts[i].Start)
                    {
                        return string.Format("{0} {1,2:00}:{2,2:00} - {0} {3,2:00}:{4,2:00}", shiftdate.ToShortDateString(),AmplaSettings.Default.Shifts[i].Start.Hours,AmplaSettings.Default.Shifts[i].Start.Minutes,
                            AmplaSettings.Default.Shifts[i].End.Hours,AmplaSettings.Default.Shifts[i].End.Minutes);
                    }
                    else
                    {
                        return string.Format("{0} {1,2:00}:{2,2:00} - {3} {4,2:00}:{5,2:00}",shiftdate.ToShortDateString(),AmplaSettings.Default.Shifts[i].Start.Hours,AmplaSettings.Default.Shifts[i].Start.Minutes,
                            shiftdate.AddDays(1).ToShortDateString(), AmplaSettings.Default.Shifts[i].End.Hours, AmplaSettings.Default.Shifts[i].End.Minutes);
                    }
                }
            }
            throw new DataWrapperCustomException(new string[]{string.Format("could not find shift '{0}'",shiftname)});
        }
        internal bool IsDirty = false;


        public DateTime ShiftStart
        {
            get
            {
              foreach(ShiftConfigElement  s in AmplaSettings.Default.Shifts){
                  if(ShiftName==s.Name){
                      return Date.Date + s.Start;
                  }
              }
              throw new System.Exception("Cound not find shift start for " + ShiftName);
            }
        }
        public DateTime ShiftEnd
        {
            get
            {
                foreach (ShiftConfigElement s in AmplaSettings.Default.Shifts)
                {
                    if (ShiftName == s.Name)
                    {
                        if (s.Start > s.End)
                        {
                            return Date.Date.AddDays(1) + s.End;
                        }
                        else
                            return Date.Date + s.End;
                    }

                }
                throw new System.Exception("Cound not find shift end for " + ShiftName);
            }
        }




    }
}
