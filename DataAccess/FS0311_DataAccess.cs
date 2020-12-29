﻿using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0311_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string strPartid, string strSHF, bool vcFlag)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT iAuto_Id,vcPart_Id,vcCPDCompany,vcCarTypeName,\r\n");
                sbr.Append(" vcPartNameCN,vcZXBZNo,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,'0' as vcModFlag,'0' as vcAddFlag\r\n");
                sbr.Append(" FROM TtagMaster \r\n");
                sbr.Append(" Where 1=1 \r\n");
                if (!string.IsNullOrWhiteSpace(strPartid))
                {
                    sbr.Append("  AND vcPart_Id LIKE '" + strPartid + "%' \r\n");
                }
                if (!string.IsNullOrWhiteSpace(strSHF))
                {
                    sbr.Append("  AND vcCPDCompany LIKE '" + strSHF + "%' \r\n");
                }
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    if (bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAuto_Id"]);

                        sbr.Append(" UPDATE TtagMaster SET \r\n");
                        sbr.Append(" vcCarTypeName= " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + ", ");
                        sbr.Append(" vcPartNameCN= " + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCN"], false) + ", ");
                        sbr.Append(" vcZXBZNo= " + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + ", ");
                        sbr.Append(" vcSCSName= " + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + ", ");
                        sbr.Append(" vcSCSAdress= " + ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + ", ");
                        sbr.Append(" dTimeFrom= " + ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + ", ");
                        sbr.Append(" dTimeTo= " + ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + ", ");
                        sbr.Append(" vcOperator = '" + strUserId + "', ");
                        sbr.Append(" dOperatorTime = GETDATE() ");
                        sbr.Append(" WHERE iAuto_Id = '" + iAutoId + "' \r\n");
                    }
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion
    }
}