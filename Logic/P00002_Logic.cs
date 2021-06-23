using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Net;


namespace Logic
{
    public class P00002_Logic
    {
        static P00002_DataAccess P00002_DataAccess = new P00002_DataAccess();



        public static DataTable GetCheckType(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime, string supplierId)
        {
            return P00002_DataAccess.GetCheckType(partId, kanbanOrderNo, kanbanSerial, dock, scanTime, supplierId);
        }

        public DataTable GetInnoData(string inno)
        {
            return P00002_DataAccess.GetInnoData(inno);
        }

        public DataTable GetSPIS(string partId, string scanTime, string supplierId)
        {
            return P00002_DataAccess.GetSPIS(partId, scanTime, supplierId);
        }

        public DataTable getCheckInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime)
        {
            return P00002_DataAccess.getCheckInfo(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
        }

        public DataSet getNgReasonInfo()
        {
            try
            {
                return P00002_DataAccess.getNgReasonInfo();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet getTableInfoFromDB()
        {
            try
            {
                return P00002_DataAccess.getTableInfoFromDB();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool setCheckInfo(DataTable dtInfo_SJ_Temp, DataTable dtInfo_NG_Temp)
        {
            try
            {
                return P00002_DataAccess.setCheckInfo(dtInfo_SJ_Temp, dtInfo_NG_Temp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
