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
    public class FS0615_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="vcOrderNo"></param>
        /// <param name="vcOrderState"></param>
        /// <returns></returns>
        public DataTable Search(string vcOrderNo, string vcOrderState)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select vcOrderNo, b.vcName vcOrderState, dUploadDate, ");
                strSql.AppendLine("case a.vcOrderState when '2' then '撤销时间：'+convert(varchar, a.dOperatorTime,20) ");
                strSql.AppendLine("                    when '3' then '更正时间：'+convert(varchar, a.dOperatorTime,20) ");
                strSql.AppendLine(" else a.vcMemo end as vcMemo, a.iAutoId from TOrderUploadManage a ");
                strSql.AppendLine("left join (select * from TCode where vcCodeId='C044') b ");
                strSql.AppendLine("on a.vcOrderState=b.vcValue where vcOrderType in ('H','C','F') ");
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine(" and vcOrderNo like '%" + vcOrderNo + "%' ");
                }
                if (vcOrderState.Length > 0)
                {
                    strSql.AppendLine(" and vcOrderState='" + vcOrderState + "' ");
                }
                strSql.AppendLine(" order by a.dOperatorTime desc");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 纳期确认
        /// <summary>
        /// 纳期确认
        /// </summary>
        /// <param name="vcOrderNo"></param>
        /// <param name="vcOrderState"></param>
        /// <returns></returns>
        public string dateMake(List<Dictionary<string, object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                string msg = "";
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcOrderState"] != null && listInfoData[i]["vcOrderState"].ToString() == "待处理")
                    {
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update TOrderUploadManage set \r\n");
                        sql.Append("  vcOrderState='4'  \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                        sql.Append("  update TUrgentOrder set vcShowFlag='1' \r\n");
                        sql.Append("  where vcOrderNo=" + ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], true) + "; \r\n");
                    }
                    else
                    {
                        msg = "订单" + listInfoData[i]["vcOrderNo"] + "不能做纳期确认！";
                        break;
                    }
                }
                if (msg.Length <= 0)
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 订单做成
        /// <summary>
        /// 订单做成
        /// </summary>
        /// <returns></returns>
        public string orderMake(List<Dictionary<string, object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                string msg = "";
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcOrderState"].ToString() != "待处理" && listInfoData[i]["vcOrderState"].ToString() != "处理中")
                    {
                        msg = "订单“" + listInfoData[i]["vcOrderNo"] + "”状态不能做成！";
                        break;
                    }
                    else
                    {
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update TOrderUploadManage set vcOrderShowFlag='1' \r\n");
                        sql.Append("  where vcOrderNo=" + ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], true) + "; \r\n");
                    }
                }
                if (msg.Length <= 0)
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 紧急订单导航页初始化
        public void getCounts(ref int counts1, ref int counts2)
        {
            try
            {
                counts1 = excute.ExecuteScalar("select count(1) from TOrderUploadManage where vcOrderType in ('H','C','F') and vcOrderState='0'");
                counts2 = excute.ExecuteScalar("select count(1) from TOrderUploadManage where vcOrderType in ('H','C','F') and vcOrderState='4'");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取订单路径
        public string getPath(string orderNo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcFilePath FROM TOrderUploadManage WHERE vcOrderNo='" + orderNo + "'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["vcFilePath"].ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 初始化订单下拉框
        public DataTable getOrders()
        {
            try
            {
                string sql = "select distinct vcOrderNo as vcValue from TOrderUploadManage where vcOrderType in ('H','F','C') order by vcOrderNo";
                return excute.ExcuteSqlWithSelectToDT(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getTCode()
        {
            try
            {
                return excute.ExcuteSqlWithSelectToDT("select vcName,vcValue from TCode where vcCodeId='C044' ORDER BY vcMeaning");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
