using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SE.MESCC.DAL.WebReferences.InventoryWS;

namespace SE.MESCC.DAL.InventoryWrapper
{
    public interface IDAL
    {
        string PerformMovement(MaterialMovementSubmission submission,string ProdID, string SourceSP, string DestnSP,string sourcelot);
        
        string Adjust(Dictionary<string, double> adjustments, bool zeroout,string comment, string wc,DateTime timestamp,bool percentage);
         bool LotExists_InWC(string location, string lot, string material);
        List<LotDetail> GetLotBalance(string locations, string material, string spno);
        string UndoMovementsFrom(DateTime t);
        bool UndoSupported { get; }
        bool DynamicSPSupported { get; }
        /// <summary>
        /// in the specified location and stockpile, will check if there is positive balance of any material other than expected material
        /// </summary>
        /// <param name="location">location to look for</param>
        /// <param name="spno">the SP to check</param>
        /// <param name="material">the expected material type</param>
        /// <returns></returns>
        //bool IsThereOtherMaterialInSP(string location, string spno, string material);
        
    }
}
