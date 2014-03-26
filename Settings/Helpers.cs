using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SE.MESCC.Settings
{
    public class AmplaSettings : System.Configuration.ConfigurationSection
    {
        public AmplaSettings()
        {
        }
        #region Properties
        [ConfigurationProperty("Shifts",   IsDefaultCollection = false)]
        public ShiftsCollection Shifts
        {
            get
            {
                ShiftsCollection urlsCollection =
                (ShiftsCollection)base["Shifts"];
                return urlsCollection;
            }
        }

        #endregion
        #region Fields
        static AmplaSettings fcs = null;
        static Configuration c = null;
        #endregion
        #region Static Singelton 
        public static AmplaSettings Default
        {
            get
            {
                if (fcs != null) return fcs;

                fcs = (AmplaSettings)System.Web.Configuration.WebConfigurationManager.GetSection("AmplaSettingGroup/AmplaSettings");
                return fcs;
            }
        }
        public static void Save()
        {
            c.Save(ConfigurationSaveMode.Full);
        }
        #endregion
        #region Methods
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        public static string legacyDowntimeWS_DowntimeWebServiceService_LOCALHOST
        {
            get
            {
                return "http://localhost:8888/b1/downtime";
            }
        }
        public static string generalLegacyWS_WebServiceService_LOCALHOST
        {
            get
            {
                return "http://localhost:8888/b1/General";
            }
        }
        public static string AmplaDataWS_DataWebService_LOCALHOST
        {
            get
            {
                return "http://localhost:8889/Ampla/Data/2008/06/Soap11";
            }
        }
        public static string InventoryWebServiceURL_LOCALHOST
        {
            get
            {
                return "http://localhost:8889/Ampla/Inventory/2010/03/Soap11";
            }
        }

        public string legacyDowntimeWS_DowntimeWebServiceService
        {
            get
            {
                return (string)this["legacyDowntimeWSURL"];
            }
            set
            {
                this["legacyDowntimeWSURL"] = value;
            }
        }
        public string generalLegacyWS_WebServiceService
        {
            get
            {
                return (string)this["generalLegacyWSURL"];
            }
            set
            {
                this["generalLegacyWSURL"] = value;
            }
        }
         [ConfigurationProperty("AmplaDataWSURL")]
        public string AmplaDataWS_DataWebService
        {
            get
            {
                return (string)this["AmplaDataWSURL"];
            }
            set
            {
                this["AmplaDataWSURL"] = value;
            }
        }

        [ConfigurationProperty("WSUsername")]
        public string WSUsername{
            get{
                return (string)this["WSUsername"];
            }
            set{
                this["WSUsername"]=value;
            }
        }
        [ConfigurationProperty("WSPassword")]
        public string WSPassword{
            get{
                return (string)this["WSPassword"];
            }
            set{
                this["WSPassword"]=value;
            }
        }
        public static SE.MESCC.DAL.WebReferences.DataWS.Credentials GetWSCredentials()
        {
            SE.MESCC.DAL.WebReferences.DataWS.Credentials result = new SE.MESCC.DAL.WebReferences.DataWS.Credentials();
            result.Username = Default.WSUsername;
            result.Password = Default.WSPassword;
            return result;
        }
                         

    }
    public class ShiftsCollection : ConfigurationElementCollection
    {
        public ShiftsCollection()
        {
            // Add one url to the collection.  This is
            // not necessary; could leave the collection 
            // empty until items are added to it outside
            // the constructor.
            ShiftConfigElement url =
                (ShiftConfigElement)CreateNewElement();
            Add(url);
        }
        #region Base Class Overrides
        public override
            ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return

                    ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override
            ConfigurationElement CreateNewElement()
        {
            return new ShiftConfigElement();
        }


        protected override
            ConfigurationElement CreateNewElement(
            string elementName)
        {
            return new ShiftConfigElement(elementName);
        }


        protected override Object
            GetElementKey(ConfigurationElement element)
        {
            return ((ShiftConfigElement)element).Name;
        }

        #endregion
        #region Properties
        public new string AddElementName
        {
            get
            { return base.AddElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string ClearElementName
        {
            get
            { return base.ClearElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string RemoveElementName
        {
            get
            { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }
        
        public ShiftConfigElement this[int index]
        {
            get
            {
                return (ShiftConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public ShiftConfigElement this[string Name]
        {
            get
            {
                return (ShiftConfigElement)BaseGet(Name);
            }
        }
        #endregion
        #region Methods
        public int IndexOf(ShiftConfigElement url)
        {
            return BaseIndexOf(url);
        }

        public void Add(ShiftConfigElement url)
        {
            BaseAdd(url);
            // Add custom code here.
        }

        protected override void
            BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(ShiftConfigElement url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
            // Add custom code here.
        }
        #endregion
    }
    public class ShiftConfigElement : ConfigurationElement
    {

        #region Constructors
        public ShiftConfigElement(String newName,TimeSpan min, TimeSpan max)
        {
            Name = newName;
            Start = min;
            End = max;
        }

        public ShiftConfigElement()
        {
        }

     
        public ShiftConfigElement(string elementName)
        {
            Name = elementName;
        }
        #endregion
        #region Properties
        [ConfigurationProperty("Name",
            IsRequired = true,
            IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }
        /*
        [ConfigurationProperty("Start",
            DefaultValue = "00:00",
            IsRequired = true)]
        [RegexStringValidator("^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$")]
        public string Start
        {
            get
            {
                return (string)this["Start"];
            }
            set
            {
                this["Start"] = value;
            }
        }
        [ConfigurationProperty("End",
    DefaultValue = "24:00",
    IsRequired = true)]
        [RegexStringValidator("^(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?$")]
        public string End
        {
            get
            {
                return (string)this["End"];
            }
            set
            {
                this["End"] = value;
            }
        }
         */
        [ConfigurationProperty("Start", IsRequired = true)]
        public TimeSpan Start
        {
            get
            {
                return (TimeSpan)this["Start"];
            }
            set
            {
                this["Start"] = value;
            }
        }
        [ConfigurationProperty("End", IsRequired = true)]
        public TimeSpan End
        {
            get
            {
                return (TimeSpan)this["End"];
            }
            set
            {
                this["End"] = value;
            }
        }
        #endregion
        #region Methods
        protected override void DeserializeElement(
           System.Xml.XmlReader reader,
            bool serializeCollectionKey)
        {
            base.DeserializeElement(reader,
                serializeCollectionKey);
            // You can your custom processing code here.
        }


        protected override bool SerializeElement(
            System.Xml.XmlWriter writer,
            bool serializeCollectionKey)
        {
            bool ret = base.SerializeElement(writer,
                serializeCollectionKey);
            // You can enter your custom processing code here.
            return ret;

        }


        protected override bool IsModified()
        {
            bool ret = base.IsModified();
            // You can enter your custom processing code here.
            return ret;
        }

        public override string ToString()
        {
            return string.Format("{0} {1:00}:{2:00}-{3:00}:{4:00}", Name, Start.Hours, Start.Minutes, End.Hours, End.Minutes);//in .NET 4 you can use TimeSpan format string
        }
        #endregion
    }

}
