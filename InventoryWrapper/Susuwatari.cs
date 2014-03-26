using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SE.MESCC.DAL.WebReferences.InventoryWS;

namespace SE.MESCC.DAL.InventoryWrapper
{
    /// <summary>
    /// Susuwtari is a simple Inventory solution based on few SQL Server tables 
    /// </summary>
    class Susuwatari:IDAL
    {
        
        const string __CNNStr = "Data Source=localhost;Initial Catalog=Inventory;Integrated Security=true;";
        #region IDAL Members
        public bool UndoSupported
        {
            get
            {
                return true;
            }
        }
        public bool DynamicSPSupported { get { return true; } }
        public string PerformMovement_old(SE.MESCC.DAL.WebReferences.InventoryWS.MaterialMovementSubmission submission,string ProdID,string SourceSP,string DestnSP)
        {
            System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(__CNNStr);
            try
            {
                cnn.Open();
                string command = string.Format(@"
                INSERT INTO [Inventory].[dbo].[Movements]
           ([SourceSZ]
           ,[SourceLot]
           ,[DestnSZ]
           ,[DestnLot]
           ,[SourceQuantity]
           ,[DestnQuantity]
           ,[ProdID]
            ,[Timestamp]
           ,[SourceSP]
           ,[DestnSP]
           ,[SourceM]
           ,[DestnM])
     VALUES
           ('{0}','{1}','{2}','{3}',{4},{5},'{6}','{7}','{8}','{9}','{10}','{11}')",
              submission.SourceDetail.WorkCenter,//0
              submission.SourceDetail.Lot,//1
              submission.DestinationDetail.WorkCenter,//2
              submission.DestinationDetail.Lot,//3
              submission.SourceDetail.Quantity,//4
              submission.DestinationDetail.Quantity,//5
              ProdID, submission.SampleDateTime,
              SourceSP, DestnSP,
              submission.SourceDetail.Material,
              submission.DestinationDetail.Material
              );
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(command, cnn);
                return cmd.ExecuteNonQuery().ToString();
            }
            finally
            {
                cnn.Close();
            }
        }
        public string PerformMovement(SE.MESCC.DAL.WebReferences.InventoryWS.MaterialMovementSubmission submission, string ProdID, string SourceSP, string DestnSP,string sourcelot)
        {
            System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(__CNNStr);
            try
            {
                cnn.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("PerformMovement", cnn);

                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceSZ",submission.SourceDetail.WorkCenter));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceLot",""+ (submission.SourceDetail.LotIdSpecified?sourcelot : submission.SourceDetail.Lot)));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DestnSZ",submission.DestinationDetail.WorkCenter));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DestnLot",submission.DestinationDetail.Lot));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceQuantity",submission.SourceDetail.Quantity));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DestnQuantity",submission.DestinationDetail.Quantity));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ProdID",ProdID));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@TimeStamp",submission.SampleDateTime));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceSP",SourceSP));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DestnSP",DestnSP));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceM",submission.SourceDetail.Material));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@DestnM",submission.DestinationDetail.Material));

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                var v = cmd.ExecuteScalar();

                return "" + v;
            }
            finally
            {
                cnn.Close();
            }
        }
        public bool LotExists_InWC(string location, string lot, string material)
        {
            System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(__CNNStr);
            try
            {
                cnn.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("LotExistsInWC", cnn);
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SZ", location));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Lot", lot));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Material", material));
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                var v = cmd.ExecuteScalar();
                int i = (int)v;
                return i > 0;
            }
            finally
            {
                cnn.Close();
            }
        }
        public string Adjust(Dictionary<string, double> lots, bool zeroout, string commnet, string wc,DateTime time,bool isPercentage)
        {
            return null;
        }
        public List<SE.MESCC.DAL.WebReferences.InventoryWS.LotDetail> GetLotBalance(string locations, string material, string spno)
        {
            System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(__CNNStr);
            try
            {
                cnn.Open();
                List<SE.MESCC.DAL.WebReferences.InventoryWS.LotDetail> result = new List<SE.MESCC.DAL.WebReferences.InventoryWS.LotDetail>();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("GetLotBalance", cnn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@SZ", System.Data.SqlDbType.NVarChar, 100); cmd.Parameters["@SZ"].Value = locations;
                if (material != null) { cmd.Parameters.Add("@Material", System.Data.SqlDbType.NVarChar, 50); cmd.Parameters["@Material"].Value = material; }
                cmd.Parameters.Add("@SP", System.Data.SqlDbType.NVarChar, 50); cmd.Parameters["@SP"].Value = spno;

                System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new LotDetail()
                    {
                        Lot = (string)dr["Lot"],
                        Material = (string)dr["M"],
                        Quantity = (double)(decimal)dr["Balance"],
                        //SampleDateTime = (DateTime)dr["Timestamp"],
                        Units = "Tonnes",
                        LotId = -1
                    });
                }
                dr.Close();
                return result;
            }
            finally
            {
                cnn.Close();
            }
        }

        public string UndoMovementsFrom(DateTime t)
        {
            System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(__CNNStr);
            try
            {
                cnn.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("UndoMovementsFrom", cnn);
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@FromDate", t));

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                var v = cmd.ExecuteScalar();
                return "" + v;
            }
            finally
            {
                cnn.Close();
            }
        }

        #endregion
    }
}
