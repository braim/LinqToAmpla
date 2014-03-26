//----------------------------------------------------------------------------
//  Copyright (c) 2010 Schneider Electric (Australia) Pty Ltd.
//  All rights reserved. 
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using System.Linq;
using System.Configuration;
using System.IO;
using SE.MESCC.Settings;
using SE.MESCC.DAL.DataWrapper.BaseRecords;
using SE.MESCC.DAL.WebReferences.DataWS;
using DataWebService = SE.MESCC.DAL.WebReferences.DataWS.DataWebServiceClient;
namespace SE.MESCC.DAL.DataWrapper
{
	public class WebServiceHelper
    {

        internal const string __CauseLocationFieldName = "Cause Location";
        internal const string __PasswordFieldName = "password";
        internal const string __UsernameFieldName = "userName";

        #region Fields
        private readonly string _UserPassword;
        private readonly string _UserName;
        private readonly string _HierarchyLocation;
        private readonly AmplaModules _Module;
        protected readonly DataWebServiceClient _DataWebService = new DataWebServiceClient();
        private GetAllowedValuesResponse _GetAllowedValuesResponse;
        #endregion

        #region Methods 
        #region Internal and Private Methods 
        
		internal RelationshipMatrixValue[] GetRelationshipMatrixValues(string causeLocation)
		{
			GetRelationshipMatrixValuesRequest getRelationshipMatrixValuesRequest = new GetRelationshipMatrixValuesRequest
			{
				Credentials = credentials,
				Location = _HierarchyLocation,
				Module = AmplaModules.Downtime
			};

			List<DependentFieldValue> fields = new List<DependentFieldValue>
			                                   	{
			                                   		new DependentFieldValue
			                                   			{
			                                   				name =__CauseLocationFieldName,
			                                   				Value = causeLocation
			                                   			}
			                                   	};
			getRelationshipMatrixValuesRequest.DependentFieldValues = fields.ToArray();

			GetRelationshipMatrixValuesResponse getRelationshipMatrixValuesResponse = _DataWebService.GetRelationshipMatrixValues(getRelationshipMatrixValuesRequest);

			return getRelationshipMatrixValuesResponse.RelationshipMatrixValues;
		}

		public AllowedValueField GetAllowedValueField(string fieldName)
		{
			if (_GetAllowedValuesResponse == null)
			{
				GetAllowedValuesRequest getAllowedValuesRequest = new GetAllowedValuesRequest
				{
					Credentials = credentials,
					Location = _HierarchyLocation,
					Module = _Module
				};

				_GetAllowedValuesResponse = _DataWebService.GetAllowedValues(getAllowedValuesRequest);
			}

			if (_GetAllowedValuesResponse != null)
			{
				AllowedValueField[] allowedValueFields = _GetAllowedValuesResponse.AllowedValueFields;

				AllowedValueField allowedValueField = allowedValueFields
					.OfType<AllowedValueField>()
					.Where(allowedValuefield => allowedValuefield.Field == fieldName)
					.FirstOrDefault();

				return allowedValueField;
			}

			return null;
		}
        

        public static List<string> GetAllowedCauseLocations(string location)
        {
            AllowedValueField f = new WebServiceHelper(location,null). GetAllowedValueField("Cause Location");
            if (f != null && f.AllowedValues != null)
                return new List<string>(f.AllowedValues);
            else throw new DataWrapperCustomException(new string[] { "could not get list of allowed cause locations. Ampla Returned no allowed cause location" });
        }
        #endregion
        #region Get Records Methods
        private GetDataRequest ConstructRequest(DateTime date, string shiftname)
        {
            return ConstructRequest(date, shiftname, null);
        }
        private GetDataRequest ConstructRequest(DateTime date, string shiftname,  FilterEntry[] filterEntries)
        {
            string period = "\"" + shiftname + "\" Beginning \"" + date.ToString("dddd, dd MMMM yyyy") + "\"";
            return ConstructRequest(period,  filterEntries);

        }
        public GetDataRequest ConstructRequest(string period, FilterEntry[] filterEntries)
        {
            GetDataOutputOptions getDataOutputOptions = new GetDataOutputOptions { ResolveIdentifiers = false };
           

            DataFilter dataFilter = new DataFilter
            {

                Location = _HierarchyLocation,
                SamplePeriod = period,
                Criteria = filterEntries,
                Deleted = "False"
            };
            
            GetDataView getDataView = new GetDataView
            {
                Context = NavigationContext.Plant,
                Module = _Module,
                Mode = NavigationMode.Location
            };

            GetDataRequest getDataRequest = new GetDataRequest
            {
                Credentials = credentials,
                OutputOptions = getDataOutputOptions,
                Filter = dataFilter,
                View = getDataView
            };
            return getDataRequest;
        }
        #endregion

        public void Split(long setid, DateTime time)
        {
            DataWebService dataWebService =
            new DataWebService();
            dataWebService.SplitRecords(new SplitRecordsRequest()
            {
                Credentials = this.credentials,
                OriginalRecord = new OriginalRecord() { Location = this._HierarchyLocation, Module = AmplaModules.Downtime, SetId = setid }
                ,
                SplitRecords = new SplitRecord[] { new SplitRecord() { SplitDateTimeUtc = time.ToUniversalTime() } }
            });


        }

        #region Save Shift Data Methods

        public void SaveData_BaseRecords(List<BaseRecord> recordlists)
        {
            List<SubmitDataRecord> submitDataRecords = new List<SubmitDataRecord>();
            List<BaseRecord> addlookuplist = new List<BaseRecord>();
  
            recordlists.ForEach (a=> a.AddSubmits(submitDataRecords,addlookuplist, AmplaModules.Production));
            if (submitDataRecords.Count == 0) return ;//nothing to do

            SubmitDataRequest sbumitDataRequest = new SubmitDataRequest(){Credentials = credentials,SubmitDataRecords =  submitDataRecords.ToArray()};
            
            SubmitDataResponse submitDataResponse = null;
            try
            {
                submitDataResponse = _DataWebService.SubmitData(sbumitDataRequest);

            }
            catch (System.Web.Services.Protocols.SoapException soapex)
            {
                throw new DataWrapperCustomException("SOAP EXCEPTION:" + soapex.Message + "  \n " + soapex.Detail.InnerText);
            }
            catch (System.Exception ex)
            {
                throw new DataWrapperCustomException(new string[] { "Error submitting data to ampla:" + ex.Message });
            }
            if (submitDataResponse != null)
            {
                if (submitDataResponse.DataSubmissionResults.Count() != sbumitDataRequest.SubmitDataRecords.Count())
                    throw new DataWrapperCustomException(new string[] { "Data submission to Ampla returned unexpected results (number of results)" });
                List<string> errors = new List<string>();
                for (int i = 0; i < submitDataResponse.DataSubmissionResults.Count(); i++)
                {
                    if (submitDataResponse.DataSubmissionResults[i].RecordAction == RecordAction.Update && submitDataResponse.DataSubmissionResults[i].SetId == Convert.ToDouble(sbumitDataRequest.SubmitDataRecords[i].MergeCriteria.SetId))
                    {
                      
                        continue;
                    }
                    else if (submitDataResponse.DataSubmissionResults[i].RecordAction == RecordAction.Insert)
                    {
                       
                        addlookuplist[i].Id =(int) submitDataResponse.DataSubmissionResults[i].SetId;
                        continue;
                    }
                    else
                    {
                        errors.Add("Unexpected result in reply to record number " + i + " Action=" + submitDataResponse.DataSubmissionResults[i].RecordAction +
                            " Response SetId=" + submitDataResponse.DataSubmissionResults[i].SetId + " Request SetId=" + sbumitDataRequest.SubmitDataRecords[i].MergeCriteria.SetId);
                    }
                }
                if(errors.Count>0)throw new DataWrapperCustomException(errors.ToArray());
                recordlists.ForEach(a => a.ClearModifiedFields());
            }
        }
        public void DeleteRecords(DeleteRecord[] recordids)
        {
            DeleteRecordsRequest deleterequest = new DeleteRecordsRequest(){Credentials = credentials};

            deleterequest.DeleteRecords = recordids;
            DeleteRecordsResponse response = null;
            try
            {
                response  = _DataWebService.DeleteRecords(deleterequest);
            }
            catch (System.Exception ex)
            {
                throw new DataWrapperCustomException(new string[] { "Error deleting records data to ampla. Exception:" + ex.Message });
            }
            if (response == null || response.DeleteRecordsResults == null || recordids.Length != response.DeleteRecordsResults.Length)
            {
                throw new DataWrapperCustomException(new string[]{"Unexpected delete record response."});
            }
            List<string> errors = new List<string>();
            for(int i=0;i<response.DeleteRecordsResults.Length;i++){
                if(response.DeleteRecordsResults[i].RecordAction != DeleteRecordsAction.Delete){
                    errors.Add(string.Format("Failed deleting {0} in location:{1} with SetID={2}",response.DeleteRecordsResults[i].Module,response.DeleteRecordsResults[i].Location,response.DeleteRecordsResults[i].SetId));

                }
            }
            if(errors.Count>0)throw new DataWrapperCustomException(errors.ToArray());

            
        }
        private void SaveData_EnumerateGenericList(object o, Type genericlistobjectype, List<BaseRecord> temp, List<IFieldDefinitionProvider> UpdateList, List<DeleteRecord> deletelist)
        {

            Type generictype = genericlistobjectype.GetGenericTypeDefinition();
            Type[] argtypes = genericlistobjectype.GetGenericArguments();
            if ((generictype.FullName == "System.Collections.Generic.List`1" || generictype.FullName == "SE.MESCC.Helpers.BaseRecordList`1")
                && argtypes.Length == 1 && (argtypes[0].BaseType == typeof(BaseRecord)|| argtypes[0].BaseType.BaseType == typeof(BaseRecord)))
            {
                //object o = props[i].GetValue(generalobject, null);
                if (o != null)
                {

                    System.Reflection.PropertyInfo propinof_i = o.GetType().GetProperty("Item");
                    System.Reflection.PropertyInfo propinof_c = o.GetType().GetProperty("Count");
                    if (propinof_i != null)
                    {
                        int count = (int)propinof_c.GetValue(o, null);
                        for (int j = 0; j < count; j++)
                        {
                            object o2 = propinof_i.GetValue(o, new object[] { j });
                            if (o2 is BaseRecord)
                            {
                                BaseRecord b2 = (BaseRecord)o2;
                                if (generictype.FullName == "SE.MESCC.Helpers.BaseRecordList`1" && o is IFieldDefinitionProvider && b2.Schema == null)
                                {
                                    // HACK. this is a temparary hack. fix it so every BaseRecord have proper schema
                                    // this is a new record. this is a HACK :( . this should be done in another way
                                    b2.Schema = (IFieldDefinitionProvider)o;
                                }
                                temp.Add(b2);
                            }
                        }
                    }

                    if (generictype.FullName == "SE.MESCC.Helpers.BaseRecordList`1")
                    {
                        if (o is IFieldDefinitionProvider)
                        {
                            UpdateList.Add((IFieldDefinitionProvider)o);
                            ((IFieldDefinitionProvider)o).GetDeleteRecords(deletelist);
                        }
                    }
                }
            }

            // Create Delete List
            if ((generictype.FullName == "SE.MESCC.Helpers.BaseRecordList`1") && argtypes.Length == 1 && argtypes[0].BaseType == typeof(BaseRecord))
            {

            }
        }
        public void SaveData(object generalobject)
        {
            List<IFieldDefinitionProvider> UpdateList = new List<IFieldDefinitionProvider>();
            List<BaseRecord> temp = new List<BaseRecord>();
            List<DeleteRecord> deletelist = new List<DeleteRecord>();

            if (generalobject is BaseRecord)temp.Add(generalobject as BaseRecord);
            if ((generalobject.GetType().IsGenericType) &&
                 (generalobject.GetType().GetGenericTypeDefinition().FullName == "SE.MESCC.Helpers.BaseRecordList`1"))
            {
                SaveData_EnumerateGenericList(generalobject, generalobject.GetType(), temp, UpdateList, deletelist);
 
            }
            else
            {
                // process properties
                System.Reflection.PropertyInfo[] props = generalobject.GetType().GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i].PropertyType.IsGenericType)
                    {
                        SaveData_EnumerateGenericList(props[i].GetValue(generalobject, null), props[i].PropertyType, temp, UpdateList, deletelist);
                        /*
                        Type generictype = props[i].PropertyType.GetGenericTypeDefinition();
                        Type[] argtypes = props[i].PropertyType.GetGenericArguments();
                        if ( (generictype.FullName == "System.Collections.Generic.List`1" || generictype.FullName == "Helpers.BaseRecordList`1")
                            && argtypes.Length == 1 && argtypes[0].BaseType == typeof(BaseRecord))
                        {
                            object o = props[i].GetValue(generalobject, null);
                            if (o != null)
                            {
                                System.Reflection.PropertyInfo propinof_i = o.GetType().GetProperty("Item");
                                System.Reflection.PropertyInfo propinof_c = o.GetType().GetProperty("Count");
                                if (propinof_i != null)
                                {
                                    int count = (int)propinof_c.GetValue(o, null);
                                    for (int j = 0; j < count; j++)
                                    {
                                        object o2 = propinof_i.GetValue(o, new object[] { j });
                                        if (o2 is BaseRecord)
                                        {
                                            BaseRecord b2 = (BaseRecord)o2;
                                            if (generictype.FullName == "Helpers.BaseRecordList`1" && o is IFieldDefinitionProvider&& b2.Schema== null)
                                            {
                                                // HACK. this is a temparary hack. fix it so every BaseRecord have proper schema
                                                // this is a new record. this is a HACK :( . this should be done in another way
                                                b2.Schema = (IFieldDefinitionProvider)o;
                                            }
                                            temp.Add(b2);
                                        }
                                    }
                                }
                                if (generictype.FullName == "Helpers.BaseRecordList`1")
                                {
                                    if (o is IFieldDefinitionProvider)
                                    {
                                        UpdateList.Add((IFieldDefinitionProvider)o);
                                        ((IFieldDefinitionProvider)o).GetDeleteRecords(deletelist);
                                    }
                                }
                            }
                        }
                        if ((generictype.FullName == "Helpers.BaseRecordList`1") && argtypes.Length == 1 && argtypes[0].BaseType == typeof(BaseRecord))
                        {
                        }
                        */
                    }//if the property is generic
                    else if (props[i].PropertyType.BaseType == typeof(BaseRecord))
                    {
                        BaseRecord b = (BaseRecord)props[i].GetValue(generalobject, null);
                        if (b != null) temp.Add(b);
                    }
                }// property loop end
            }// if it's a generic list
            SaveData_BaseRecords(temp);
            if (deletelist.Count > 0) DeleteRecords(deletelist.ToArray());
                UpdateList.ForEach(a=>a.MakeCurrentInitial());
        }
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="dtrpName">RP name. if null, default from WebConfig will be used. use Empty if RP name is already in location,this value will be concatenated to location in requests</param>
        /// <param name="module">Ampla module. if null method tries to guess module based on location+dtrpName. by default Downtime will be used</param>
        public WebServiceHelper(string location,AmplaModules? module)
        {

            if (AmplaSettings.Default == null) throw new ConfigurationErrorsException("Configuration for Ampla Connectivity Missing in Application's Configuration");

            _UserName = AmplaSettings.Default.WSUsername;
            _UserPassword = AmplaSettings.Default.WSPassword;
            _DataWebService = new DataWebService();
          //  if(AmplaSettings.Default.AmplaDataWS_DataWebService!=null)
          //      _DataWebService.Url = AmplaSettings.Default.AmplaDataWS_DataWebService;
            if(string.IsNullOrEmpty(location))
                _HierarchyLocation = ConfigurationManager.AppSettings["hierachyLocation"];
            else
                _HierarchyLocation = location;
            
            if (module != null) _Module = module.Value;
            else
            {
                if (_HierarchyLocation.EndsWith("Downtime")) _Module = AmplaModules.Downtime;
                else if (_HierarchyLocation.EndsWith("Shift Log")) _Module = AmplaModules.Knowledge;
                else if (_HierarchyLocation.EndsWith("Production")) _Module = AmplaModules.Production;
                else _Module = AmplaModules.Downtime;
            }
            if (string.IsNullOrEmpty(_UserName) || string.IsNullOrEmpty(_UserPassword))
            {
                throw new DataWrapperCustomException(new string[] { "Username or password not provided in application settings" });
            }
        }


        #endregion

        #region Properties
        protected Credentials credentials
        {
            get
            {
                return new Credentials
                {
                    Password = _UserPassword,
                    Username = _UserName
                };
            }
        }
        #endregion


        #region Generic
        public BaseRecordList<T> GetRecords<T>(DateTime periodBegining, string periodName, FilterEntry[] filters ) where T : BaseRecord, new()
        {
            GetDataRequest getDataRequest = ConstructRequest(periodBegining, periodName, filters);
            return GetRecords<T>( getDataRequest);
            
        }
        public BaseRecordList<T> GetRecords<T>(string periodname,  FilterEntry[] filters) where T : BaseRecord, new()
        {
            GetDataRequest getDataRequest = ConstructRequest(periodname,  filters);
            return GetRecords<T>(getDataRequest);
        }
        public BaseRecordList<T> GetRecords<T>(DateTime fromdate,DateTime todate, FilterEntry[] filters) where T : BaseRecord, new()
        {
            //>= "Wednesday, August 03, 2011 1:25:07 PM" And < "Wednesday, July 27, 2011 1:25:07 PM"
            string fromdate_str = BaseRecord.DateTimeStr( fromdate);//.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");
            string todate_str = BaseRecord.DateTimeStr( todate);//.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");


            // string period = "\"" + shiftname + "\" Beginning \"" + date.ToString("dddd, dd MMMM yyyy") + "\"";

            string period = ">= \"" + fromdate_str + "\" AND < \"" + todate_str + "\"";
            return GetRecords<T>(period, filters);

        }

        public BaseRecordList<T> GetRecords<T>(GetDataRequest getDataRequest) where T : BaseRecord, new()
        {
            BaseRecordList<T> result = new BaseRecordList<T>();
            GetDataResponse dataResponse = null;
            //GetDataRequest getDataRequest = ConstructRequest(periodname, module, location, filters);
            try
            {
                dataResponse = _DataWebService.GetData(getDataRequest);
            }
            catch (System.Web.Services.Protocols.SoapException sx)
            {
                string msg = "AMPLA SOAP Exception:" + sx.Message + (sx.Detail!=null? sx.Detail.InnerXml:"");

                throw new DataWrapperCustomException(msg);
            }
            if (dataResponse.RowSets.Length == 1)
            {
                RowSet rowSet = dataResponse.RowSets[0];

                result.FieldDefinitions = rowSet.Columns;
                result.Location = getDataRequest.Filter.Location;

                if (rowSet.Rows.Length == 0)
                {
                    // caller should worry about number of records
                    // throw new Exception(new string[] { "No data found for shift " + shiftname + " on " + date+" for module "+module });
                }
                for (int i = 0; i < rowSet.Rows.Length; i++)
                {
                    T p = new T();
                    p.SetFields(rowSet.Rows[i], rowSet.Columns);

                    result.AddBaseRecord(p);
                }
            }

            return result;
        }
        public BaseRecordList<T> GetRecords<T>(DateTime periodBegining, string periodName) where T : BaseRecord, new()
        {
            return GetRecords<T>(periodBegining, periodName,null);

        }


        #endregion

        #region Cached System Configuration Methods
        static Dictionary<string, RelationshipMatrixValue[]> _RelationShipCache = new Dictionary<string, RelationshipMatrixValue[]>();
        public RelationshipMatrixValue[] GetRelationshipMatrixValues_cached(string causeLocation)
        {
            try
            {
                if (!_RelationShipCache.ContainsKey(causeLocation))
                {
                    // may throw exception
                    _RelationShipCache.Add(causeLocation, GetRelationshipMatrixValues(causeLocation));
                }
                return _RelationShipCache[causeLocation];
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    throw new DataWrapperCustomException(new string[]{"Time elements not configured for this location"});
                else
                throw new DataWrapperCustomException(new string[]{"Erorr getting relationship matrix:"+ex.Message});

            }

        }
        #endregion



       
    }
}
