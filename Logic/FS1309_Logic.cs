using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;

namespace Logic
{
    public class FS1309_Logic
    {
        FS1309_DataAccess fs1309_DataAccess;

        public FS1309_Logic()
        {
            fs1309_DataAccess = new FS1309_DataAccess();
        }
        public Dictionary<string, object> setLoadPage(Dictionary<string, object> res, string strPackingPlant, ref string type, ref int code)
        {
            try
            {
                DataSet dataSet = fs1309_DataAccess.getLoadData(strPackingPlant);
                if (dataSet == null)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e2";
                    return res;
                }
                //页面赋值
                string strPageClientNum = "30";
                string strGZTTongjiFre = "30";
                string strBZLTongjiFre = "30";
                string strGZTZhuangTaiFre = "30";
                string strGZTQieHuanFre = "60";
                string strGZTShowType = "1";
                string strObjective = "100.00";
                if (dataSet.Tables[0].Rows.Count != 0)
                {
                    strPageClientNum = dataSet.Tables[0].Rows[0]["vcPageClientNum"].ToString();
                    strGZTTongjiFre = dataSet.Tables[0].Rows[0]["iGZTTongjiFre"].ToString();
                    strBZLTongjiFre = dataSet.Tables[0].Rows[0]["iBZLTongjiFre"].ToString();
                    strGZTZhuangTaiFre = dataSet.Tables[0].Rows[0]["iGZTZhuangTaiFre"].ToString();
                    strGZTQieHuanFre = dataSet.Tables[0].Rows[0]["iGZTQieHuanFre"].ToString();
                    strGZTShowType = dataSet.Tables[0].Rows[0]["iGZTShowType"].ToString();
                    strObjective = dataSet.Tables[0].Rows[0]["decObjective"].ToString();
                }
                string strBFromTime = "08:30";
                string strBCross = "0";
                string strBToTime = "17:15";
                if (dataSet.Tables[1].Rows.Count != 0)
                {
                    strBFromTime = dataSet.Tables[1].Rows[0]["tFromTime"].ToString();
                    strBCross = dataSet.Tables[1].Rows[0]["vcCross"].ToString();
                    strBToTime = dataSet.Tables[1].Rows[0]["tToTime"].ToString();
                }
                string strYFromTime = "21:00";
                string strYCross = "1";
                string strYToTime = "05:45";
                if (dataSet.Tables[2].Rows.Count != 0)
                {
                    strYFromTime = dataSet.Tables[2].Rows[0]["tFromTime"].ToString();
                    strYCross = dataSet.Tables[2].Rows[0]["vcCross"].ToString();
                    strYToTime = dataSet.Tables[2].Rows[0]["tToTime"].ToString();
                }
                //uuid
                string uuid = "";
                res.Add("PageClientNumItem", strPageClientNum);
                res.Add("GZTTongjiFreItem", strGZTTongjiFre);
                res.Add("BZLTongjiFreItem", strBZLTongjiFre);
                res.Add("GZTZhuangTaiFreItem", strGZTZhuangTaiFre);
                res.Add("GZTQieHuanFreItem", strGZTQieHuanFre);
                res.Add("GZTShowTypeItem", strGZTShowType);
                res.Add("ObjectiveItem", strObjective);
                res.Add("BFromTimeItem", strBFromTime);
                res.Add("BCrossItem", strBCross);
                res.Add("BToTimeItem", strBToTime);
                res.Add("YFromTimeItem", strYFromTime);
                res.Add("YCrossItem", strYCross);
                res.Add("YToTimeItem", strYToTime);
                res.Add("uuidItem", uuid);
                code = ComConstant.SUCCESS_CODE;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setDisplayInfo(string strPackingPlant, string strPageClientNum, string strGZTTongjiFre, string strBZLTongjiFre, string strGZTZhuangTaiFre, string strGZTQieHuanFre, string strGZTShowType,string strObjective
                    , string strBFromTime, string strBCross, string strBToTime, string strYFromTime, string strYCross, string strYToTime, string strOperId)
        {
            fs1309_DataAccess.setDisplayInfo(strPackingPlant, strPageClientNum, strGZTTongjiFre, strBZLTongjiFre, strGZTZhuangTaiFre, strGZTQieHuanFre, strGZTShowType, strObjective
                   , strBFromTime, strBCross, strBToTime, strYFromTime, strYCross, strYToTime, strOperId);
        }
    }
}
