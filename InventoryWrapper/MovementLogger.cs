using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.MESCC.DAL.InventoryWrapper
{
    public class MovementLogger
    {

        private static System.IO.StreamWriter sw = null;
        private static System.IO.StreamWriter GetSW()
        {
            try
            {
                if (sw == null)
                {
                    String path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    path = System.IO.Path.GetDirectoryName(path);

                    string basefile = "MovementReplayLog_";
                    int i = 0;
                    while (System.IO.File.Exists(System.IO.Path.Combine(path, basefile + i + ".csv")))
                        i++;

                    sw = new System.IO.StreamWriter(System.IO.Path.Combine(path,basefile + i + ".csv"));
                    //System.Diagnostics.Trace.WriteLine(System.
                    sw.WriteLine("Type,Time,Src WC,Src,Des WC,Des,Balance T, Source T, Des T");
                }
                return sw;
            }
            catch 
            {
                return null;
            }
        }


        public static void LogMovement_WA(string SrcWC, string SrcSP, string DesWC, string DesSP, Double SourceBalance, Double SourceTonnes, Double DesTonnes)
        {
            try
            {
                if (GetSW() != null)
                    sw.WriteLine("WA,{0},{1},{2},{3},{4},{5},{6},{7}", DateTime.Now, SrcWC, SrcSP, DesWC, DesSP, SourceBalance, SourceTonnes, DesTonnes);
            }
            catch 
            {
            }
        }
        public static void LogMovement_WASM(string SrcWC, string SrcLot, string DesWC, string DesLot, Double SourceBlanace, Double SourceTonnes, Double DesTonnes)
        {
            try
            {
                if (GetSW() != null)
                    sw.WriteLine("WASM,{0},{1},{2},{3},{4},{5},{6},{7}", DateTime.Now, SrcWC, SrcLot, DesWC, DesLot, SourceBlanace, SourceTonnes, DesTonnes);
            }
            catch 
            {
            }
        }
        private void LogMovement_SM(string SrcWC, string SrcLot, string DesWC, string DesLot, Double SourceBlanace, Double SourceTonnes, Double DesTonnes)
        {
            try
            {
                if (GetSW() != null)
                    sw.WriteLine("SM,{0},{1},{2},{3},{4},{5}", SrcWC, SrcLot, DesWC, DesLot, SourceTonnes, DesTonnes);
            }
            catch 
            {
            }
        }
        public static void Flush()
        {
            try
            {
                if (sw != null)
                    sw.Flush();

            }
            catch
            {
            }
        }
    }
}
