using System;
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
                strSql.AppendLine("select vcOrderNo, b.vcName vcOrderState, dUploadDate, a.iAutoId from TOrderUploadManage a ");
                strSql.AppendLine("left join (select * from TCode where vcCodeId='C044') b ");
                strSql.AppendLine("on a.vcOrderState=b.vcValue where vcOrderType='H' ");
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine(" and vcOrderNo like '%" + vcOrderNo + "%' ");
                }
                if (vcOrderState.Length > 0)
                {
                    strSql.AppendLine(" and vcOrderState='" + vcOrderState + "' ");
                }
                strSql.AppendLine(" order by a.dOperatorTime desc ");
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
        public void dateMake(List<Dictionary<string, object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append("  update TOrderUploadManage set \r\n");
                    sql.Append("  vcOrderState='4'  \r\n");
                    sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    sql.Append("  update TUrgentOrder set vcShowFlag='1' \r\n");
                    sql.Append("  where vcOrderNo=" + ComFunction.getSqlValue(listInfoData[i]["vcOrderNo"], true) + " \r\n");
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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

        #region 订单做成
        /// <summary>
        /// 订单做成
        /// </summary>
        /// <returns></returns>
        public void orderMake(List<Dictionary<string, object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append("  update TOrderUploadManage set \r\n");
                    sql.Append("  vcOrderShowFlag='1',vcOrderState='1'   \r\n");
                    sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
