using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE.MESCC.DAL.WebReferences.InventoryWS;
using SE.MESCC.Settings;
using SE.MESCC.DAL.DataWrapper;

namespace SE.MESCC.DAL.InventoryWrapper
{
    public class IDataAccessLayer:IDAL
    {
        string userName;
        string userPassword;
        InventoryWebService inventoryws = new InventoryWebService();


        public IDataAccessLayer()
        {
            if (AmplaSettings.Default != null)
            {
                userName = AmplaSettings.Default.WSUsername;
                userPassword = AmplaSettings.Default.WSPassword;
            }
        }
        public string PerformMovement( SE.MESCC.DAL.WebReferences.InventoryWS.MaterialMovementSubmission submission,string ProdID,string SourceSP,string DestnSP,string sourcelot)
        {
            SE.MESCC.DAL.WebReferences.InventoryWS.MoveMaterialResponse response = null;
            try
            {
                //MaterialMovementResponse
                response =
                inventoryws.MoveMaterial(new MoveMaterialRequest()
                {
                    Credentials = new Credentials() { Username = userName, Password = userPassword },
                    Submission = submission,

                });
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                
                throw new DataWrapperCustomException(ex.Message+"  "+ex.Detail.InnerText);

            }
            return ".";
        }
        public bool LotExists_InWC(string location, string lot, string material)
        {
            try
            {
                GetCurrentLotsResponse response =
                inventoryws.GetCurrentLots(new GetCurrentLotsRequest()
                {
                    Credentials = GetCredentials(),
                    Filter = new LotFilter() { WorkCenter = location, Lot = lot, Material = material }
                });
                return (response.LotDetails != null && response.LotDetails.Length > 0);
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                throw new DataWrapperCustomException(ex.Message + "  " + ex.Detail.InnerText);
            }
        }

        public SE.MESCC.DAL.WebReferences.InventoryWS.Credentials GetCredentials(){
            return new SE.MESCC.DAL.WebReferences.InventoryWS.Credentials() { Username = userName, Password = userPassword };
        }


        public List<LotDetail> GetLotBalance(string locations,string material, string spno)
        {
            if (spno != null) throw new ArgumentException("Ampla does not support dynamic stockpiles, spno should be null");
            GetCurrentLotsResponse response = 
            inventoryws.GetCurrentLots( new GetCurrentLotsRequest()
            {
                 Credentials = GetCredentials(),
                  Filter = new LotFilter(){
                       Material = material, 
                       WorkCenter = locations
                  }
            });
            List<LotDetail> result = new List<LotDetail>();
            for(int i=0;i<response.LotDetails.Length;i++)
              //  if(response.LotDetails[i].Lot.StartsWith(spno) || string.IsNullOrEmpty(spno))
                    result.Add(response.LotDetails[i]);

            return result;           
        }
        //public List<LotDetail> GetLotBalance(string locations,string spno)
        //{

        //    GetCurrentLotsResponse response =
        //    inventoryws.GetCurrentLots(new GetCurrentLotsRequest()
        //    {
        //        Credentials = GetCredentials(),
        //        Filter = new LotFilter()
        //        {
                  
        //            WorkCenter = locations
        //        }
        //    });
        //    List<LotDetail> result = new List<LotDetail>();
        //    for (int i = 0; i < response.LotDetails.Length; i++)
        //        result.Add(response.LotDetails[i]);
        //    return result;
        //}
        public bool UndoSupported
        {
            get
            {
                return false;
            }
        }
        public bool DynamicSPSupported { get { return false; } }
        public string UndoMovementsFrom(DateTime t)
        {
            throw new InvalidOperationException();
        }
        //public bool IsThereOtherMaterialInSP(string location, string spno, string material)
        //{
        //    List<LotDetail> lots = GetLotBalance(location, spno);
        //    return !(lots.TrueForAll(a => a.Quantity==0 || a.Material == material));
        //}
        public string Adjust(Dictionary<string,double> lots,bool zeroout ,string commnet,string wc, DateTime timestamp,bool isPercentage)
        {
            try
            {
              
                List<LotQuantityAdjustment> adjustments = new List<LotQuantityAdjustment>();

                foreach (string lot in lots.Keys)
                {
                    adjustments.Add(
                              new LotQuantityAdjustment()
                              {
                                  Quantity = zeroout ? -100 : lots[lot],
                                  WorkCenter = wc,
                                  Lot = lot,
                                  Comment = commnet,
                                  SampleDateTime = timestamp.ToUniversalTime(),
                                  SampleDateTimeSpecified=true,
                                  Method = (zeroout || isPercentage) ? AdjustmentMethod.Percentage : AdjustmentMethod.Absolute
                              });

                }
                AdjustLotQuantityRequest request = new AdjustLotQuantityRequest()
                {
                   
                    Adjustments = adjustments.ToArray(),
                    Credentials = userName==null?null: new Credentials() { Username = userName, Password = userPassword }
                };
                inventoryws.AdjustLotQuantity(request);
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                throw new DataWrapperCustomException(ex.Message + "  " + ex.Detail.InnerText);
            }
            return ".";
        }
    }
}
