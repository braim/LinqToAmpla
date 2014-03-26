using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using SE.MESCC.DAL.WebReferences;
using SE.MESCC.DAL.WebReferences.DataWS;

namespace SE.MESCC.DAL.DataWrapper.BaseRecords
{
    public interface IFieldDefinitionProvider
    {
        FieldDefinition[] GetFieldDefinitions();
        string GetLocation();

        void GetDeleteRecords(List<SE.MESCC.DAL.WebReferences.DataWS.DeleteRecord> deletelist);
         void MakeCurrentInitial();

    }
    public class BaseRecordList<T> : List<T>,IFieldDefinitionProvider where T : BaseRecord
    {
        public void MakeCurrentInitial()
        {
            InitialRecords.Clear();
            for(int i=0;i<this.Count;i++)
                InitialRecords.Add(new DeleteInfo() { ID = this[i].Id, Location = this[i].Location, Module = this[i].Module });
        }
        public class DeleteInfo
        {
            public string Location;
            public long ID;
            public SE.MESCC.DAL.WebReferences.DataWS.AmplaModules Module; 
            public DeleteInfo()
            {
            }
        }
        public SE.MESCC.DAL.WebReferences.DataWS.FieldDefinition[] FieldDefinitions = null;
        public string Location = null;

        #region IFieldDefinitionProvider Members

        public SE.MESCC.DAL.WebReferences.DataWS.FieldDefinition[] GetFieldDefinitions()
        {
            return FieldDefinitions;
        }
        public void AddBaseRecord(T b)
        {
            b.Schema = this;
            InitialRecords.Add(new DeleteInfo() { ID = b.Id, Location = b.Location, Module = b.Module });
            Add(b);
        }
        public string GetLocation()
        {
            return Location;
        }

        #endregion
        List<DeleteInfo> InitialRecords = new List<DeleteInfo>();
        public void GetDeleteRecords(List<SE.MESCC.DAL.WebReferences.DataWS.DeleteRecord> deletelist)
        {
            InitialRecords.ForEach(a =>
            {
                if (!this.Exists(b =>
                {
                     return (b.Module == a.Module) && (b.Location == b.Location) && (a.ID == b.Id);
                }))
                {
                   deletelist.Add(new DeleteRecord(){ Module = a.Module, Location = a.Location, MergeCriteria = new DeleteRecordsMergeCriteria(){ SetId = a.ID}});

                }
            });
        
        }
        //int IndexOfMin()
        //{
        //    DateTime mintime = DateTime.MaxValue;
        //    int minindex = -1;
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        if (this[i] is DownTimeRecord)
        //        {
        //            object o = this[i];
        //            if (((DownTimeRecord)o).StartDateTime < mintime)
        //                minindex = i;
        //        }
        //    }
        //    return minindex;
        //}
        //void MySort<T>()
        //{
        //    List<T> temp = new List<T>();
        //    int j = IndexOfMin();
        //    while (j != 0)
        //    {
        //        temp.Add(this[j]);
        //        j = IndexOfMin();
        //    }
        //    InitialRecords.Clear();
        //    for (int i = 0; i < temp.Count; i++)
        //    {
        //        Add(temp[i]);
        //    }
        //}


    }
    public class BaseRecord
    {
        #region Standard Ampla Fields
        public long Id { get; set; }
        public Boolean IsManual
        {
            get;
            set;
        }
        public Boolean HasAudit
        {
            get;
            set;
        }
        public string CreatedBy
        {
            get;
            set;
        }
        public DateTime CreatedDateTime
        {
            get;
            set;
        }
        public Boolean Deleted
        {
            get;
            set;
        }
        public string LastModified
        {
            get;
            set;
        }
        public string ObjectId
        {
            get;
            set;
        }
        #endregion


        public IFieldDefinitionProvider Schema = null;
        public List<string> _SubmitMask = null;
        private string _Location = null;
        public string Location
        {
            get
            {
                if (_Location != null) return _Location;
                if (Schema == null || Schema.GetLocation() == null) throw new DataWrapperCustomException(new string[] { "Operation requires location which is not set" });
                return Schema.GetLocation();
            }
            set
            {
                _Location = value;
            }
        }
        public virtual AmplaModules Module
        {
            get
            {
                return AmplaModules.Production;
            }
        }
        protected List<Field> Fields = new List<Field>();
        public void AddAsItWasDefined(string name, string value)
        {
            List<FieldDefinition> temp = new List<FieldDefinition>(_FieldDefinitions);
            temp.Add(new FieldDefinition(){ name=name,displayName=name});
            _FieldDefinitions = temp.ToArray();
        }
        protected FieldDefinition[] _FieldDefinitions = null;
        protected FieldDefinition[] FieldDefinitions
        {
            get
            {
                if (_FieldDefinitions != null) return _FieldDefinitions;
                FieldDefinition[] result = null;
                if (Schema != null) result = Schema.GetFieldDefinitions();
                if (result == null) throw new DataWrapperCustomException(new string[] { "Operation requires data schema which does not exist. Record needs to be loaded from Ampla to have Field Definitions. " });
                return result;
            }
        }

        protected void Add(XmlElement xml)
        {
            Fields.Add(new Field(){ Name = xml.Name,Value =xml.InnerText});
        }

        protected bool Exists(string name)
        {
            return (Fields.Exists(a => a.Name == name));
        }

        public virtual void SetFields(Row row,FieldDefinition[] definitions)
        {
            _FieldDefinitions = definitions;
            Dictionary<string, System.Reflection.PropertyInfo> TranslationDic = GetTransDic();
            this.Id = int.Parse(row.id);

            var a = row.Any;
            for (int i = 0; i < a.Length; i++)
            {
                string value = a[i].InnerText;
                string name = a[i].Name;
                if (TranslationDic.ContainsKey(name))
                {
                    bool found = true;
                    try
                    {
                        if (TranslationDic[name].PropertyType == typeof(string))
                        {
                            TranslationDic[name].SetValue(this, value, null);
                        }
                        else if (TranslationDic[name].PropertyType == typeof(int))
                        {
                            TranslationDic[name].SetValue(this, int.Parse(value), null);
                        }
                        else if (TranslationDic[name].PropertyType == typeof(decimal))
                        {
                            TranslationDic[name].SetValue(this, decimal.Parse(value), null);
                        }
                        else if (TranslationDic[name].PropertyType == typeof(double))
                        {
                            TranslationDic[name].SetValue(this, double.Parse(value), null);
                        }
                        else if (TranslationDic[name].PropertyType == typeof(DateTime))
                        {
                            TranslationDic[name].SetValue(this, DateTimeParse(value), null);
                        }
                        else if (TranslationDic[name].PropertyType == typeof(Boolean))
                        {
                            TranslationDic[name].SetValue(this, bool.Parse(value), null);
                        }
                        else if (TranslationDic[name].PropertyType.IsEnum)
                        {
                            TranslationDic[name].SetValue(this, Enum.Parse(TranslationDic[name].PropertyType, value), null);
                        }else found = false;
                    }
                    catch (System.Exception ex)
                    {
                        throw new DataWrapperCustomException(new string[] { string.Format("Failed setting values from Ampla Record to Custom Interface object model.name={0},value={1},type={2}.Exception Message:{3}", name, value, this.GetType().Name,ex.Message) });
                    }
                        

                    if(!found)
                        throw new DataWrapperCustomException(new string[] { "Type not supported" });
                    Add(a[i]);
               
                }
                else
                {
                    // during development you might use this exception as a warning 
            
                }
            }
        }



        private Dictionary<string, System.Reflection.PropertyInfo>  GetTransDic(){
            System.Reflection.PropertyInfo[] props = this.GetType().GetProperties();
            Dictionary<string, System.Reflection.PropertyInfo> TranslationDic = new Dictionary<string, System.Reflection.PropertyInfo>();
            for (int i = 0; i < props.Count(); i++)
            {
                object[] propattribs = props[i].GetCustomAttributes(false);
                AmplaFieldAttribute afa = (AmplaFieldAttribute)propattribs.FirstOrDefault(pa => { return (pa is AmplaFieldAttribute); });
                if (afa != null)
                {
                    if(TranslationDic.ContainsKey(afa.FieldName)) throw new DataWrapperCustomException("Wrong Record Definiton class. duplicate '"+afa.FieldName+"' one defined as'"+TranslationDic[afa.FieldName]+"' and second one as '"+props[i]+"'");

                    TranslationDic.Add(afa.FieldName, props[i]);
                }
                else
                {
                    if (TranslationDic.ContainsKey(props[i].Name)) throw new DataWrapperCustomException("Wrong Record Definiton class. duplicate '" + props[i].Name + "' one defined as'" + TranslationDic[props[i].Name] + "' and second one as '" + props[i] + "'");
                    TranslationDic.Add(props[i].Name, props[i]);
                }
            }
            return TranslationDic;
        }
        internal void ClearModifiedFields()
        {
                         List<Field> result = new List<Field>();
             Dictionary<string, System.Reflection.PropertyInfo> TranslationDic = GetTransDic();
             System.Reflection.PropertyInfo propertyinfo = null;
             string fieldname = null;
             if (_FieldDefinitions != null)
             {



                 for (int i = 0; i < FieldDefinitions.Length; i++)
                 {

                     FieldDefinition fielddefiniton = FieldDefinitions[i];
                     fieldname = fielddefiniton.name;

                     if (Fields.Exists(f2 => f2.Name == fielddefiniton.name))// loaded
                     {
                         Field f = Fields.Find(ff => ff.Name == fielddefiniton.name);
                         propertyinfo = TranslationDic[f.Name];
                         if (HasXmlIgnore(propertyinfo)) continue;

                         if ((FieldStr(propertyinfo.GetValue(this, null))) != f.Value)
                         {
                             f.Value = FieldStr(propertyinfo.GetValue(this, null))  ;
                            // result.Add(new Field() { Name = fielddefiniton.displayName, Value = FieldStr(TranslationDic[f.Name].GetValue(this, null)) });
                         }
                     }
                     else// new
                     {
                         if (TranslationDic.ContainsKey(fielddefiniton.name))
                         {
                             propertyinfo = TranslationDic[fielddefiniton.name];
                             if (HasXmlIgnore(propertyinfo)) continue;
                             object o = propertyinfo.GetValue(this, null);
                             // check if o is not default 
                             if (o != null)
                             {
                                 string valuetosend = CheckIfNotNullorEmpty(o);
                                 if (valuetosend != null)
                                 {
                                     
                                     Fields.Add(new Field() { Name = fielddefiniton.name, Value = valuetosend });
                                 }

                             }
                         }
                     }
                 }
             }
        }
        internal List< Field> GetModifiedFields()
        {
             List<Field> result = new List<Field>();
             Dictionary<string, System.Reflection.PropertyInfo> TranslationDic = GetTransDic();
             System.Reflection.PropertyInfo propertyinfo = null;
             string fieldname = null;
            if (_FieldDefinitions != null)
            {
               
               
                
                for (int i = 0; i < FieldDefinitions.Length; i++)
                {
                    
                    FieldDefinition fielddefiniton = FieldDefinitions[i];
                    fieldname = fielddefiniton.name;

                    if (Fields.Exists(f2 => f2.Name == fielddefiniton.name))// loaded
                    {
                        Field f = Fields.Find(ff => ff.Name == fielddefiniton.name);
                        propertyinfo = TranslationDic[f.Name];
                        if (HasXmlIgnore(propertyinfo)) continue;

                        if ((FieldStr(propertyinfo.GetValue(this, null))) != f.Value)
                        {
                            result.Add(new Field() { Name = fielddefiniton.displayName, Value = FieldStr(TranslationDic[f.Name].GetValue(this, null)) });
                        }
                    }
                    else// new
                    {
                        if (TranslationDic.ContainsKey(fielddefiniton.name))
                        {
                            propertyinfo = TranslationDic[fielddefiniton.name];
                            if (HasXmlIgnore(propertyinfo)) continue;
                            object o = propertyinfo.GetValue(this, null);
                            // check if o is not default 
                            if (o != null)
                            {
                                string valuetosend = CheckIfNotNullorEmpty(o);
                                if(valuetosend !=null){
                                      result.Add(new Field() { Name = fielddefiniton.displayName, Value = valuetosend });
                                }

                            }
                        }
                    }
                }
                return result;
            }
            else
            {
                if (_SubmitMask != null)
                {
                    foreach (string key in _SubmitMask)
                    {
                        if (TranslationDic.ContainsKey(key))
                        {
                            propertyinfo = TranslationDic[key];
                           
                            if (HasXmlIgnore(propertyinfo)) continue;
                            string displayname = GetDisplayName(propertyinfo);
                            object o = propertyinfo.GetValue(this, null);
                            // check if o is not default 
                            if (o != null)
                            {
                                // string valuetosend = CheckIfNotNullorEmpty(o);
                                string valuetosend = CheckIfNotNullorEmpty(o);
                                if (valuetosend != null)
                                {
                                    result.Add(new Field() { Name = displayname, Value = valuetosend });
                                }
                            }
                        }
                        else
                        {
                            throw new DataWrapperCustomException(new string[]{"Invalid operation. could not find a property binding to ampla field '" + key + "'"});
                        }
                    }
                    return result;
                }
                throw new DataWrapperCustomException(new string[]{"To find modified fields (to save records) either there should be definiton or Submit Mask should be defined"});

            }

        }

        private string GetDisplayName(System.Reflection.PropertyInfo propertyinfo)
        {
            object[] attribs = propertyinfo.GetCustomAttributes(false);
            if (attribs != null)
                for (int i = 0; i < attribs.Length; i++)
                    if (attribs[i] is AmplaFieldAttribute)
                    {
                        if (!string.IsNullOrEmpty(((AmplaFieldAttribute)attribs[i]).DisplayName))
                            return ((AmplaFieldAttribute)attribs[i]).DisplayName;
                        if (!string.IsNullOrEmpty(((AmplaFieldAttribute)attribs[i]).FieldName))
                            return ((AmplaFieldAttribute)attribs[i]).FieldName;
                    }

            return propertyinfo.Name;
        }

        private string CheckIfNotNullorEmpty(object o)
        {
            if (o is string && ((string)o) != "")
                return (string)o;
            if (o is int && ((int)o) != 0)
               return o.ToString();
            if (o is decimal && ((decimal)o) != (decimal)0)
                return o.ToString();
            if (o is double && ((double)o) != (double)0)
                return o.ToString();
            if (o is DateTime && ((DateTime)o).Year > 1000)
                return DateTimeStr((DateTime)o);
            if (o != null && o is Enum)
                return Enum.GetName(o.GetType(), o);
                
            return null;
        }
     
        private bool HasXmlIgnore(System.Reflection.PropertyInfo propertyinfo)
        {
            object[] attribs = propertyinfo.GetCustomAttributes(false);
            if (attribs != null)
                for (int i = 0; i < attribs.Length; i++)
                    if (attribs[i] is System.Xml.Serialization.XmlIgnoreAttribute)
                        return true;
            return false;
            
               
        }
        private string FieldStr(object o)
        {
            if (o == null) return "";
            if (o is DateTime) return DateTimeStr((DateTime)o);
            return o.ToString();
        }
        public static string DateTimeStr(DateTime o)
        {
            return (((DateTime)o).ToUniversalTime().ToString("s",    System.Globalization.DateTimeFormatInfo.InvariantInfo) + "Z");
        }
        protected DateTime DateTimeParse(string value)
        {
            return DateTime.Parse(value);
        }
        internal void AddSubmits(List<SubmitDataRecord> submitDataRecords, 
            List<BaseRecord> addlookuplist,
            AmplaModules module)
        {
            {
                List<Field> modifid = this.GetModifiedFields();
                if (modifid.Count > 0)
                {
                    submitDataRecords.Add(new SubmitDataRecord()
                    {
                        Fields = this.GetModifiedFields().ToArray(),
                        Location = this.Location,
                        MergeCriteria = new MergeCriteria()
                        {
                            KeyFieldNames = new string[] { },
                            SetId = this.Id

                        },
                        Module = this.Module
                    }

                    );
                    addlookuplist.Add(this);
                }
            }
        }






        public string Record_x0020_Type
        {
            get;
            set;
        }
        public string Process_x0020_Location{
            get;set;
        }
        public string 
        Duration{
            get;set;
        }

        [AmplaField("Sample_x0020_Period","Sample Period")]
        public string Sample_x0020_Period { get; set; }
        public DateTime SamplePeriod
        {
            get
            {
                return DateTimeParse(Sample_x0020_Period);
            }
            set
            {
                Sample_x0020_Period = DateTimeStr(value);
            }
        }
    }
}
