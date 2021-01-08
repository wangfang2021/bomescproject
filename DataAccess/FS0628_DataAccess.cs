﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS0628_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcOrderNo, string vcPartNo, string vcInsideOutsideType, string vcNewOldFlag, string vcInjectionFactory, string vcWorkArea, string dExpectReceiveDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select [iAutoId], [dOrderHandleDate], [vcOrderNo], [vcPartNo], c.vcName as [vcInsideOutsideType], d.vcName as [vcNewOldFlag],    ");
                strSql.AppendLine("  b.vcName as [vcInjectionFactory], [vcDock], [vcSupplier_id], [vcWorkArea], [vcCHCCode], [vcCarType], [vcOrderNum],     ");
                strSql.AppendLine("  [dExpectReceiveDate], [vcOderTimes],[vcInjectionOrderNo], [vcMemo],      ");
                strSql.AppendLine("   [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag  from TEmergentOrderManage a   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C000') b on a.vcInjectionFactory = b.vcValue   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C003') c on a.vcInsideOutsideType = c.vcValue   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C004') d on a.vcNewOldFlag = d.vcValue where 1=1    ");

                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderNo like '%" + vcOrderNo + "%' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcPartNo like '%" + vcPartNo + "%' ");
                }
                if (vcInsideOutsideType.Length > 0)
                {
                    strSql.AppendLine("  and  vcInsideOutsideType = '" + vcInsideOutsideType + "' ");
                }
                if (vcNewOldFlag.Length > 0)
                {
                    strSql.AppendLine("  and  vcNewOldFlag = '" + vcNewOldFlag + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcInjectionFactory = '" + vcInjectionFactory + "' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea like '" + vcWorkArea + "' ");
                }
                if (dExpectReceiveDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dExpectReceiveDate,112) = '" + dExpectReceiveDate.Replace("-", "") + "' ");
                }

                strSql.AppendLine("  order by  dOperatorTime desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        /// <param name="strErrorPartId"></param>
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("   insert into TEmergentOrderManage ( [dOrderHandleDate], [vcOrderNo], [vcPartNo], [vcInsideOutsideType], [vcNewOldFlag],  \n");
                        sql.Append("   [vcInjectionFactory], [vcDock], [vcSupplier_id], [vcWorkArea], [vcCHCCode], [vcCarType], [vcOrderNum],  \n");
                        sql.Append("   [dExpectReceiveDate], [vcOderTimes], [vcInjectionOrderNo], [vcMemo], [vcOperatorID], [dOperatorTime])   \n");
                        sql.Append(" values (  \r\n");
                        sql.Append("   GETDATE(), \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOrderNo"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPartNo"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcInsideOutsideType"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcNewOldFlag"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcInjectionFactory"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcDock"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcWorkArea"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCHCCode"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCarType"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOrderNum"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dExpectReceiveDate"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOderTimes"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcInjectionOrderNo"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcMemo"], false) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TEmergentOrderManage set    \r\n");
                        sql.Append("  vcOrderNo=" + getSqlValue(listInfoData[i]["vcOrderNo"], false) + ",   \r\n");
                        sql.Append("  vcPartNo=" + getSqlValue(listInfoData[i]["vcPartNo"], false) + ",    \r\n");
                        sql.Append("  vcInsideOutsideType=" + getSqlValue(listInfoData[i]["vcInsideOutsideType"], false) + ",    \r\n");
                        sql.Append("  vcNewOldFlag=" + getSqlValue(listInfoData[i]["vcNewOldFlag"], false) + " ,   \r\n");
                        sql.Append("  vcInjectionFactory=" + getSqlValue(listInfoData[i]["vcInjectionFactory"], false) + " ,   \r\n");
                        sql.Append("  vcDock=" + getSqlValue(listInfoData[i]["vcDock"], false) + ",    \r\n");
                        sql.Append("  vcSupplier_id=" + getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",    \r\n");
                        sql.Append("  vcWorkArea=" + getSqlValue(listInfoData[i]["vcWorkArea"], false) + ",    \r\n");
                        sql.Append("  vcCHCCode=" + getSqlValue(listInfoData[i]["vcCHCCode"], false) + ",    \r\n");
                        sql.Append("  vcCarType=" + getSqlValue(listInfoData[i]["vcCarType"], false) + ",    \r\n");
                        sql.Append("  vcOrderNum=" + getSqlValue(listInfoData[i]["vcOrderNum"], false) + ",    \r\n");
                        sql.Append("  dExpectReceiveDate=" + getSqlValue(listInfoData[i]["dExpectReceiveDate"], true) + ",    \r\n");
                        sql.Append("  vcOderTimes=" + getSqlValue(listInfoData[i]["vcOderTimes"], false) + ",    \r\n");
                        sql.Append("  vcInjectionOrderNo=" + getSqlValue(listInfoData[i]["vcInjectionOrderNo"], false) + ",    \r\n");
                        sql.Append("  vcMemo=" + getSqlValue(listInfoData[i]["vcMemo"], false) + ",    \r\n");
                        sql.Append("  vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
                        sql.Append(" where iAutoId=" + iAutoId + " ;  \n");

                    }
                }

                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="dtdel"></param>
        /// <param name="userId"></param>
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete [TEmergentOrderManage] where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void importSave(DataTable dt, object strUserId)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcOrderNo = dt.Rows[i]["vcOrderNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOrderNo"].ToString();
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                    string vcInsideOutsideType = dt.Rows[i]["vcInsideOutsideType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInsideOutsideType"].ToString();
                    if (vcInsideOutsideType == "内制")
                    {
                        vcInsideOutsideType = "0";
                    }
                    else if (vcInsideOutsideType == "外注")
                    {
                        vcInsideOutsideType = "1";
                    }
                    else
                    { }
                    string vcNewOldFlag = dt.Rows[i]["vcNewOldFlag"] == System.DBNull.Value ? "" : dt.Rows[i]["vcNewOldFlag"].ToString();
                    if (vcNewOldFlag == "号口")
                    {
                        vcNewOldFlag = "H";
                    }
                    else if (vcNewOldFlag == "旧型")
                    {
                        vcNewOldFlag = "Q";
                    }
                    else
                    { }
                    string vcInjectionFactory = dt.Rows[i]["vcInjectionFactory"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInjectionFactory"].ToString();
                    string vcDock = dt.Rows[i]["vcDock"] == System.DBNull.Value ? "" : dt.Rows[i]["vcDock"].ToString();
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                   
                    string vcCHCCode = dt.Rows[i]["vcCHCCode"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCHCCode"].ToString();
                    string vcCarType = dt.Rows[i]["vcCarType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarType"].ToString();
                    string vcOrderNum = dt.Rows[i]["vcOrderNum"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOrderNum"].ToString();

                    string dExpectReceiveDate = dt.Rows[i]["dExpectReceiveDate"] == System.DBNull.Value ? "null" : Convert.ToDateTime(dt.Rows[i]["dExpectReceiveDate"].ToString()).ToString();
                    string vcOderTimes = dt.Rows[i]["vcOderTimes"] == System.DBNull.Value ? "null" : dt.Rows[i]["vcOderTimes"].ToString();
                    string vcInjectionOrderNo = dt.Rows[i]["vcInjectionOrderNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInjectionOrderNo"].ToString();
                    string vcMemo = dt.Rows[i]["vcMemo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMemo"].ToString();


                    strSql.AppendLine("  insert into TEmergentOrderManage ( [dOrderHandleDate], [vcOrderNo], [vcPartNo], [vcInsideOutsideType], [vcNewOldFlag],   ");
                    strSql.AppendLine("  [vcInjectionFactory], [vcDock], [vcSupplier_id], [vcWorkArea], [vcCHCCode], [vcCarType], [vcOrderNum],   ");
                    strSql.AppendLine("  [dExpectReceiveDate], [vcOderTimes], [vcInjectionOrderNo], [vcMemo], [vcOperatorID], [dOperatorTime]) values(   ");
                    strSql.AppendLine("   GETDATE(),'" + vcOrderNo + "','" + vcPartNo + "','" + vcInsideOutsideType + "','" + vcNewOldFlag + "'  ");
                    strSql.AppendLine("   ,'" + vcInjectionFactory + "','" + vcDock + "','" + vcSupplier_id + "','" + vcWorkArea + "','" + vcCHCCode + "','" + vcCarType + "','" + vcOrderNum + "'  ");
                    strSql.AppendLine("   ,'" + dExpectReceiveDate + "','" + vcOderTimes + "','" + vcInjectionOrderNo + "','" + vcMemo + "' ");
                    strSql.AppendLine("   ,'" + strUserId + "',GETDATE()) ;   ");

                }
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion
    }
}
