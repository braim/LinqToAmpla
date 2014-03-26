using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.MESCC.DAL.DataWrapper
{
    public class AmplaFieldAttribute:Attribute 
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public AmplaFieldAttribute(string fieldname)
            : base()
        {
            FieldName = fieldname;
        }
        public AmplaFieldAttribute(string fieldname, string displayname)
            : base()
        {
            FieldName = fieldname;
            DisplayName = displayname;
        }
        public AmplaFieldAttribute()
        {
            FieldName = null;
            DisplayName = null;
        }
    }
    public class AmplaLookupListAttribute : Attribute
    {
        public Type lookuplistype { get; set; }
        public AmplaLookupListAttribute(Type t)
            : base()
        {
            lookuplistype = t;
        }
    }
}
