using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.MESCC.DAL.InventoryWrapper
{
    public static class DALFactory
    {
        private static IDAL ampladal = null;
        public static IDAL DAL
        {
            get
            {
                if (ampladal == null)
                    ampladal = new IDataAccessLayer();
                    //ampladal = new Susuwatari();
                return ampladal;
            }
        }
    }
}
