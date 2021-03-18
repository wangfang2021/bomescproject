using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using Common;

namespace Logic
{
    public class FS0313_Logic
    {
        FS0313_DataAccess fs0313_DataAccess;

        public FS0313_Logic()
        {
            fs0313_DataAccess = new FS0313_DataAccess();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string strMaxNum,string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            return convert(fs0313_DataAccess.Search(strMaxNum,strChange, strPart_id, strOriginCompany, strHaoJiu
            ,  strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState
            ));
        }
        #endregion

        #region 相同品番行挨着，设定同一个颜色
        public DataTable convert(DataTable dt)
        {
            string strColor_A = "partFS0313A";//这两个变量是行的背景颜色class名字，具体颜色在前台画面定义
            string strColor_B = "partFS0313B";

            dt.Columns.Add("vcBgColor");
            string strTempPartId = "";
            string strTempColor = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strPart_id = dt.Rows[i]["vcPart_id"] == DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                if (strTempPartId == "")
                {
                    dt.Rows[i]["vcBgColor"] = strColor_A;
                    strTempPartId = strPart_id;
                    strTempColor = strColor_A;
                }
                else
                {
                    if (strTempPartId == strPart_id)
                    {
                        dt.Rows[i]["vcBgColor"] = strTempColor;
                    }
                    else 
                    {
                        strTempPartId = strPart_id;
                        strTempColor = strTempColor==strColor_A? strColor_B: strColor_A;
                        dt.Rows[i]["vcBgColor"] = strTempColor;
                    }
                }
            }
            return dt;
        }
        #endregion


        #region 检索所有待处理的数据
        public int getAllTask()
        {
            DataTable dt = fs0313_DataAccess.getAllTask();
            if (dt == null || dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(dt.Rows[0]["iNum"]);
        }
        #endregion


        #region 财务保存
        public void SaveCaiWu(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0313_DataAccess.SaveCaiWu(listInfoData, strUserId);
        }
        #endregion


        #region 财务回复
        public void OKCaiWu(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0313_DataAccess.OKCaiWu(listInfoData, strUserId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0313_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 验证品番是否都是已经送信
        public string checkState(string strAutoIds)
        {
            DataTable dt = fs0313_DataAccess.checkState(strAutoIds);
            if (dt == null || dt.Rows.Count == 0)
                return "";
            else
            {
                StringBuilder res = new StringBuilder();
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    res.Append(dt.Rows[i]["vcPart_id"].ToString()).Append(";");
                }
                return res.ToString();
            }
        }
        #endregion

    }
}
