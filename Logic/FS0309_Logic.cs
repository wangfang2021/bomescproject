﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0309_Logic
    {
        FS0309_DataAccess fs0309_DataAccess;

        public FS0309_Logic()
        {
            fs0309_DataAccess = new FS0309_DataAccess();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            return convert(fs0309_DataAccess.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
            , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState
            ));
        }
        #endregion

        #region 相同品番行挨着，设定同一个颜色
        public DataTable convert(DataTable dt)
        {
            string strColor_A = "partFS0309A";//这两个变量是行的背景颜色class名字，具体颜色在前台画面定义
            string strColor_B = "partFS0309B";

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

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0309_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0309_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion


    }
}
