using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SE.MESCC.DAL.DataWrapper;
using SE.MESCC.DAL.WebReferences.DataWS;
using SE.MESCC.DAL.DataWrapper.BaseRecords;


namespace SE.MESCC.DAL.DataWrapper.BaseRecords
{
    public class DownTimeRecord : ConfirmableRecordForEquipment, IComparable<DownTimeRecord>
    {
        public override AmplaModules Module
        {
            get
            {
                return AmplaModules.Downtime;
            }
        }
        #region Properties
      
        [AmplaField("Start_x0020_Time","Start Time")]
        public DateTime StartDateTime { get; set; }
        [AmplaField("End_x0020_Time","End Time")]
        public DateTime EndDateTime { get; set; }
        
        [AmplaField("Cause_x0020_Location","Cause Location")]
        public string CauseLocation { get; set; }
        [AmplaField("Secondary_x0020_Time_x0020_Element","Secondary Time Element")]
        public string SecondaryTimeElement { get; set; }
        [AmplaField("Tertiary_x0020_Time_x0020_Element","Tertiary Time Element")]
        public string TertiaryTimeElement { get; set; }
        [AmplaField("Primary_x0020_Time_x0020_Element", "Primary Time Element")]
        public string PrimaryTimeElement { get; set; }
        [AmplaField("Operator")]
        public string Operator { get; set; }
        [AmplaField("Comment")]
        public string Comment { get; set; }
        #endregion
        //# region WebService Field IDs
        //const string __CID_PrimaryTimeElement = @"Primary_x0020_Time_x0020_Element";
        //const string __CID_TertiaryTimeElement  = @"Tertiary_x0020_Time_x0020_Element";
        //const string __CID_SecondaryTimeElement = @"Secondary_x0020_Time_x0020_Element";
        //const string __CID_StartTime = @"Start_x0020_Time";
        //const string __CID_EndTime = @"End_x0020_Time";
        //const string __CID_CauseLocation = @"Cause_x0020_Location";
        //#endregion
        #region Constructors
        public DownTimeRecord(Row r,FieldDefinition[] definitions)
        {
            base.SetFields(r, definitions);
            /*
            this.ID = int.Parse( r.id);
            var a = r.Any;
            for (int i = 0; i < a.Length; i++)
            {
                string value = a[i].InnerText;
                switch (a[i].Name)
                {
                    case __CID_StartTime:
                        StartDateTime = DateTime.Parse(value);
                        break;
                    case __CID_EndTime:
                        EndDateTime = DateTime.Parse(value);
                        break;
                    case __CID_CauseLocation:
                        CauseLocation = value;
                        break;
                    case __CID_PrimaryTimeElement:
                        PrimaryTimeElement = value;
                        break;
                    case __CID_SecondaryTimeElement:
                        SecondaryTimeElement = value;
                        break;
                    case __CID_TertiaryTimeElement:
                        TertiaryTimeElement = value;
                        break;
                }
            }
            return;
             */

        }
        public DownTimeRecord()
        {
        }
        #endregion

        #region UI Fields, Limited Length Fields
        public TimeSpan DurationSpan
        {
            get
            {
                if (EndDateTime.Year == 1) return TimeSpan.Zero;
                return EndDateTime - StartDateTime;
            }
        }
        public string DurationSpanStr
        {
            get
            {
                if(DurationSpan.Days==0)
                    return string.Format ("{0:00}:{1:00}:{2:00}", (int)DurationSpan.Hours, DurationSpan.Minutes, DurationSpan.Seconds);
                else
                    return string.Format("{0:0} {1:00}:{2:00}:{3:00}",(int)DurationSpan.Days, (int)DurationSpan.Hours, DurationSpan.Minutes, DurationSpan.Seconds);
            }
        }
        const int __MAXLEN = 14;
        const int __MAXLEN2 = 26;
        const int __MAXLEN3 = 26;
        const int __MAXLEN_LWLOC = 50;
        const string __TRIMTXT = "...";
        const string __CMLoc = "RTCA.Kestrel.Underground.Development.";

        const string __LWLoc = "RTCA.Kestrel.Underground.Longwalls.";
        public string CauseLocation_CM
        {
            get{
                if (CauseLocation!=null && CauseLocation.StartsWith(__CMLoc))
                    return CauseLocation.Substring(__CMLoc.Length);
                else
                    return CauseLocation;
            }
        }
        public string CauseLocation_LW
        {
            get
            {
                if(CauseLocation ==null)return "";
                string s = CauseLocation ;
                string lwloc = __LWLoc;
                if(!string.IsNullOrEmpty(Location) && Location.EndsWith(".Downtime")) lwloc = Location.Substring(0,Location.Length-(("Downtime").Length));
                
                if (CauseLocation.StartsWith(lwloc))
                    s = CauseLocation.Substring(lwloc.Length);
                if (s.Length > __MAXLEN_LWLOC)
                    s = "..." + s.Substring(s.Length - __MAXLEN_LWLOC);
                return s;
            }
        }
        public string CauseLocation_ltd
        {
            get
            {
                if (CauseLocation != null && CauseLocation.Length > __MAXLEN)
                {
                    return __TRIMTXT + CauseLocation.Substring(CauseLocation.Length - __MAXLEN);
                }
                return CauseLocation;
            }
        }
        public string SecondaryTimeElement_ltd{
            get
            {
                if (SecondaryTimeElement_str != null && SecondaryTimeElement_str.Length > __MAXLEN2)
                {
                    return __TRIMTXT + SecondaryTimeElement_str.Substring(SecondaryTimeElement_str.Length - __MAXLEN2);
                }
                return SecondaryTimeElement_str;
            }
        }
        public string TertiaryTimeElement_ltd
        {
            get
            {
                if (TertiaryTimeElement_str != null && TertiaryTimeElement_str.Length > __MAXLEN3)
                {
                    return __TRIMTXT + TertiaryTimeElement_str.Substring(TertiaryTimeElement_str.Length - __MAXLEN3);
                }
                return TertiaryTimeElement_str;
            }
        }
        private string _SecondaryTimeElement_str = null;
        public string SecondaryTimeElement_str
        {
            get
            {
                if (string.IsNullOrEmpty(SecondaryTimeElement) || string.IsNullOrEmpty(CauseLocation)) return "";// dont remove this line. so avoiding unnecessary lookups. 
                if (_SecondaryTimeElement_str == null) ResolveIDs(null);
                //  return SecondaryTimeElement;
                return _SecondaryTimeElement_str;
            }
            set
            {
                _SecondaryTimeElement_str = value;
            }
        }

        private string _TertiaryTimeElement_str = null;
        public string TertiaryTimeElement_str
        {
            get
            {
                if (string.IsNullOrEmpty(TertiaryTimeElement) || string.IsNullOrEmpty(CauseLocation)) return "";// dont remove this
                if (_TertiaryTimeElement_str == null) ResolveIDs(null);
                //   return TertiaryTimeElement;
                return _TertiaryTimeElement_str;
            }
            set
            {
                _TertiaryTimeElement_str = value;
            }
        }
        public string StartDate_str 
        {
            get
            {
                return StartDateTime.ToShortDateString();
            }
        }
        public string StartTime_str
        {
            get
            {
                return StartDateTime.ToLongTimeString();
            }
        }
        public string EndDate_str
        {
            get
            {
                return EndDateTime.Year == 1 ? "" : EndDateTime.ToShortDateString();
            }
        }
        public string EndTime_str
        {
            get
            {
                return EndDateTime.Year == 1 ? "" : EndDateTime.ToLongTimeString();
            }
        }
        public string EndTimeOrDate_str
        {
            get
            {
                if (EndDateTime.Year == 1) return "";
                if (StartDateTime.Date.Equals(EndDateTime.Date))
                    return "<b>"+ EndDateTime.ToLongTimeString()+"</b>";
                else
                    return EndDateTime.ToShortDateString() + " <b>" + EndDateTime.ToLongTimeString() + "</b>";
            }
        }
        public string Duration_str
        {
            get
            {
                if (EndDateTime.Year == 1)
                    return "";
                return DurationSpanStr;
            }
        }
        public void ResolveIDs(WebServiceHelper webservice)
        {
            if (string.IsNullOrEmpty(CauseLocation))
            {
                _TertiaryTimeElement_str = ""; _SecondaryTimeElement_str = "";
                return;
            }
            if (webservice == null) webservice = new WebServiceHelper(Location, AmplaModules.Downtime);
            var relationshipmatrix = webservice.GetRelationshipMatrixValues_cached(CauseLocation);
            foreach (RelationshipMatrixValue matrixvalue in relationshipmatrix)
            {
                string Classification = matrixvalue.MatrixValues[0].Value;
               MatrixValue CauseCode = matrixvalue.MatrixValues[1];
                MatrixValue EffectID = matrixvalue.MatrixValues[2];
                if (SecondaryTimeElement == CauseCode.id) _SecondaryTimeElement_str = CauseCode.Value;
                if (TertiaryTimeElement == EffectID.id) _TertiaryTimeElement_str = EffectID.Value;

            }
            if (_TertiaryTimeElement_str == null) _TertiaryTimeElement_str = "could not find effect with id" + TertiaryTimeElement;
            if (_SecondaryTimeElement_str == null) _SecondaryTimeElement_str = "could not find cause with id" + SecondaryTimeElement;


        }
        #endregion

        #region Not in use RP fields - they may get filled but not used in Kestrel Custom Intefaces
        public string IsSplit { get; set; }

        public string Start_x0020_Time_x0020__x0028_Clipped_x0029_{get;set;}
        public string End_x0020_Time_x0020__x0028_Clipped_x0029_{get;set;}
        public string Duration_x0020__x0028_Clipped_x0029_{get;set;}
        [AmplaField("Eff._x0020_Duration")]
        public string Eff_x0020_Duration{get;set;}
        [AmplaField("Eff._x0020_Duration_x0020__x0028_Clipped_x0029_")]
        public string Eff_x0020_Duration_x0020__x0028_Clipped_x0029_{get;set;}
        [AmplaField("Eff._x0020__x0025_")]
        public string Eff_x0020__x0025_{get;set;}
        public string Capture_x0020_Location{get;set;}
        public string Is_x0020_Clipped{get;set;}
        public string Auto_x0020_Completed{get;set;}
        public string ClippedPercentage{get;set;}
        public string Equipment_x0020_Id_x0020__x0028_Location_x0029_{get;set;}
        public string Masked{get;set;}
        public string Virtual{get;set;}
        public string Cont_x0020_Miner_x0020_Equip_x0020_ID{get;set;}
        public string Info_x0020_Required { get; set; }
        public string Shuttle_x0020_Car_x0020_1 { get; set; }
        
        public string Fan_x0020_1 { get; set; }
        public string Fan_x0020_2 { get; set; }
        public string DCB { get; set; }
        public string Transformer { get; set; }
        public string Operation_x0020_Mode { get; set; }
        public string Equipment_x0020_Type_x0020__x0028_Location_x0029_ { get; set; }
        public string Automation_x0020_in_x0020_Use { get; set; }
        public string Required_x0020_Speed { get; set; }
        public string Shearer_x0020_Position { get; set; }
        public string Event { get; set; }
        public string Equipment_x0020_Type_x0020__x0028_Cause_x0020_Location_x0029_ { get; set; }
        public string Equipment_x0020_Id_x0020__x0028_Cause_x0020_Location_x0029_ { get; set; }

        public string Comments { get; set; }

        #endregion

        public void CreateDefaultSubmitMask()
        {
            _SubmitMask = new List<string>();
           _SubmitMask.AddRange(new string[]{"Cause_x0020_Location",
                                                "Secondary_x0020_Time_x0020_Element",
                                                "Tertiary_x0020_Time_x0020_Element",
                                                "Primary_x0020_Time_x0020_Element",
                                                "Operator",
                                                "Comment" });
        }

        #region IComparable<DownTimeRecord> Members

        public int CompareTo(DownTimeRecord other)
        {
            if (other == null) return 1;
            return (StartDateTime.CompareTo(other.StartDateTime));
        }

        #endregion
    }
}
