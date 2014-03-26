using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.MESCC.DAL.DataWrapper
{
    public class ShiftLogException : DataWrapperCustomException
    {
     public   ShiftLogException():base(new string[]{"Shift Log Not Found"}){
    }
    }
    public class DataWrapperCustomException:System.ApplicationException
    {
        public DataWrapperCustomException(string msg):base(msg)
        {
            _ErrorList = new List<string>();
            _ErrorList.Add(msg);
        }
        public DataWrapperCustomException(List<string> errors)
            : base(errors[0])
        {
            _ErrorList = errors;
        }
        public DataWrapperCustomException(string[] errors)
            : base(errors[0])
        {
            _ErrorList = new List<string>();
            _ErrorList.AddRange(errors);
        }
        public List<string> _ErrorList;

        #region ERROR CODES
        public const uint __EQCNFG_Load         = 0x80000001;
        public const uint __EQCNFG_Save         = 0x80000002;
        public const uint __EQCNFG_Insert       = 0x80000003;
        public const uint __EQCNFG_Delete       = 0x80000004;
        public const uint __EQCNFG_UpdateGrid   = 0x80000005;
        public const uint __EQCNFG_UpdateDetail = 0x80000006;
        public const uint __EQCNFG_OpenInsert   = 0x80000007;
        public const uint __PRODOWN_SaveShift   = 0x80000008;
        public const uint __PRODOWN_LoadShift   = 0x80000009;
        public const uint __PRODOWN_SaveDowntime = 0x8000000a;
        public const uint __CONSUM_SaveLong     = 0x8000000b;
        public const uint __CONSUM_LoadShift    = 0x8000000c;
        public const uint __CONSUM_SaveShift    = 0x8000000b;
        public const uint __CONSUM_SaveDev      = 0x8000000d;
        public const uint __EQCNFG_Filter       = 0x8000000e;
        public const uint __PRODOWN_Operators = 0x8000000f;
        public const uint __SURVERY_FINDLASTSURVEY = 0x80000010;
        public const uint __SURVERY_PREVIEW = 0x80000011;
        public const uint __SURVERY_SUBMIT = 0x80000012;
        #endregion



    }
}
