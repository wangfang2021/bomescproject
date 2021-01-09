using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0811_Logic
    {
        FS0811_DataAccess fs0811_DataAccess;

        public FS0811_Logic()
        {
            fs0811_DataAccess = new FS0811_DataAccess();
        }
        public DataTable getNowBanZhiInfo()
        {
            return fs0811_DataAccess.getNowBanZhiInfo();
        }
        public DataSet getLoadPageData(string strHosDate, string strBanZhi, string strFromTime, string strToTime)
        {
            DataSet dataSet = new DataSet();
            dataSet= fs0811_DataAccess.getSearchInfo(strHosDate, strBanZhi);
            if (dataSet != null && dataSet.Tables[1].Rows.Count != 0)
                return dataSet;
            else
            {
                dataSet = fs0811_DataAccess.getPackingPlanInfo(strHosDate, strBanZhi);
                if (dataSet != null && dataSet.Tables[1].Rows.Count != 0)
                    return dataSet;
                else
                    return null;
            }
        }
        public DataSet getSearchInfo(string strHosDate, string strBanZhi)
        {
            return fs0811_DataAccess.getSearchInfo(strHosDate, strBanZhi);
        }
        public string QueryInfo(string strSaleno)
        {
            return "";
        }
        public string SaveInfo(string strSaleno)
        {
            return "";
        }
    }
}
