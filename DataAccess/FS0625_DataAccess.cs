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
    public class FS0625_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

      /// <summary>
      /// 获取号试目的
      /// </summary>
      /// <returns></returns>
        public DataTable GetPurposes()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
               
                strSql.AppendLine("  select vcPurposes as vcCodeId,vcPurposes as vcCodeName from(select distinct vcPurposes from TOralTestManage) a  ");
                  
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string dExportDate, string vcCarType, string vcPartNo, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcIsNewRulesFlag, string vcPurposes)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select [iAutoId], [dExportDate], [vcCarType], [vcPartNo], [vcPartName], [vcInsideOutsideType],   ");
                strSql.AppendLine("   [vcSupplier_id], [vcWorkArea], [vcIsNewRulesFlag], [vcOEOrSP], [vcDock], [vcNumber], [vcPurposes],   ");
                strSql.AppendLine("   [dOrderPurposesDate], [dOrderReceiveDate], [vcReceiveTimes], [dActualReceiveDate], [vcAccountOrderNo],    ");
                strSql.AppendLine("   [dAccountOrderReceiveDate], [dOrderSendDate], [vcMemo], [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag   ");
                strSql.AppendLine("   from TOralTestManage where 1=1   ");

                if (dExportDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dExportDate,112) = '" + dExportDate.Replace("-","") + "' ");
                }
                if (vcCarType.Length > 0)
                {
                    strSql.AppendLine("  and  vcCarType = '" + vcCarType + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcPartNo = '" + vcPartNo + "' ");
                }
                if (vcInsideOutsideType.Length > 0)
                {
                    strSql.AppendLine("  and  vcInsideOutsideType = '" + vcInsideOutsideType + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id  like  '%" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea = '" + vcWorkArea + "' ");
                }
                if (vcIsNewRulesFlag.Length > 0)
                {
                    strSql.AppendLine("  and  vcIsNewRulesFlag = '" + vcIsNewRulesFlag + "' ");
                }
                if (vcPurposes.Length > 0)
                {
                    strSql.AppendLine("  and  vcPurposes = '" + vcPurposes + "' ");
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
                        
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("   UPDATE [dbo].[TOralTestManage]   \r\n");
                        sql.Append("      SET [vcCarType] = " + getSqlValue(listInfoData[i]["vcCarType"], false) + "   \r\n");
                        sql.Append("         ,[vcPartNo] =  " + getSqlValue(listInfoData[i]["vcPartNo"], false) + "   \r\n");
                        sql.Append("         ,[vcPartName] =  " + getSqlValue(listInfoData[i]["vcPartName"], false) + "   \r\n");
                        sql.Append("         ,[vcInsideOutsideType] =   " + getSqlValue(listInfoData[i]["vcInsideOutsideType"], false) + "  \r\n");
                        sql.Append("         ,[vcSupplier_id] =  " + getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "   \r\n");
                        sql.Append("         ,[vcWorkArea] =  " + getSqlValue(listInfoData[i]["vcWorkArea"], false) + "   \r\n");
                        sql.Append("         ,[vcIsNewRulesFlag] =  " + getSqlValue(listInfoData[i]["vcIsNewRulesFlag"], false) + "   \r\n");
                        sql.Append("         ,[vcOEOrSP] =   " + getSqlValue(listInfoData[i]["vcOEOrSP"], false) + "  \r\n");
                        sql.Append("         ,[vcDock] =   " + getSqlValue(listInfoData[i]["vcDock"], false) + "  \r\n");
                        sql.Append("         ,[vcNumber] =   " + getSqlValue(listInfoData[i]["vcNumber"], false) + "  \r\n");
                        sql.Append("         ,[vcPurposes] = " + getSqlValue(listInfoData[i]["vcPurposes"], false) + "    \r\n");
                        sql.Append("         ,[dOrderPurposesDate] = " + getSqlValue(listInfoData[i]["dOrderPurposesDate"], true) + "   \r\n");
                        sql.Append("         ,[dOrderReceiveDate] =  " + getSqlValue(listInfoData[i]["dOrderReceiveDate"], true) + "  \r\n");
                        sql.Append("         ,[vcReceiveTimes] =  " + getSqlValue(listInfoData[i]["vcReceiveTimes"], false) + "   \r\n");
                        sql.Append("         ,[dActualReceiveDate] =  " + getSqlValue(listInfoData[i]["dActualReceiveDate"], true) + "  \r\n");
                        sql.Append("         ,[vcAccountOrderNo] =  " + getSqlValue(listInfoData[i]["vcAccountOrderNo"], false) + "   \r\n");
                        sql.Append("         ,[dAccountOrderReceiveDate] =   " + getSqlValue(listInfoData[i]["dAccountOrderReceiveDate"], false) + "  \r\n");
                        sql.Append("         ,[dOrderSendDate] =  " + getSqlValue(listInfoData[i]["dOrderSendDate"], true) + "  \r\n");
                        sql.Append("         ,[vcMemo] =  " + getSqlValue(listInfoData[i]["vcMemo"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
                        sql.Append(" where iAutoId=" + iAutoId + " ;  \n");

                    }
                }

                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交

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
                sql.Append("  delete [TOralTestManage] where iAutoId in(   \r\n ");
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
        /// <summary>
        /// 一括赋予
        /// </summary>
        /// <returns></returns>
        public void allInstall(List<Dictionary<string, object>> listInfoData, string dAccountOrderReceiveDate, string vcAccountOrderNo, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [TOralTestManage] set  \n");
                if (dAccountOrderReceiveDate.Length>0)
                {
                    sql.Append(" dAccountOrderReceiveDate='" + Convert.ToDateTime(dAccountOrderReceiveDate) + "', \n");
                }
                if (vcAccountOrderNo.Length > 0)
                {
                    sql.Append(" vcAccountOrderNo='" + vcAccountOrderNo + "', \n");
                }
                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
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
        /// <summary>
        /// 导入 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strUserId"></param>
        public void importSave(DataTable dt, object strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //[vcSupplier_id], [vcWorkArea], [dBeginDate], [dEndDate],, 
                    string vcCarType = dt.Rows[i]["vcCarType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarType"].ToString();
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                    string vcPartName = dt.Rows[i]["vcPartName"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartName"].ToString();
                    string vcInsideOutsideType = dt.Rows[i]["vcInsideOutsideType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInsideOutsideType"].ToString();
                    if (vcInsideOutsideType == "内制")
                    {
                        vcInsideOutsideType = "0";
                    } else if (vcInsideOutsideType == "外注") {
                        vcInsideOutsideType = "1";
                    }
                    else
                    { }
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    string vcIsNewRulesFlag = dt.Rows[i]["vcIsNewRulesFlag"] == System.DBNull.Value ? "" : dt.Rows[i]["vcIsNewRulesFlag"].ToString();
                    if (vcIsNewRulesFlag=="是")
                    {
                        vcIsNewRulesFlag = "1";
                    } else if (vcIsNewRulesFlag == "否")
                    {
                        vcIsNewRulesFlag = "0";
                    } else
                    { }
                    string vcOEOrSP = dt.Rows[i]["vcOEOrSP"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOEOrSP"].ToString();
                    string vcDock = dt.Rows[i]["vcDock"] == System.DBNull.Value ? "" : dt.Rows[i]["vcDock"].ToString();
                    string vcNumber = dt.Rows[i]["vcNumber"] == System.DBNull.Value ? "" : dt.Rows[i]["vcNumber"].ToString();
                    string vcPurposes = dt.Rows[i]["vcPurposes"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPurposes"].ToString();
                    
                    string dOrderPurposesDate = dt.Rows[i]["dOrderPurposesDate"] == System.DBNull.Value ? "null" : Convert.ToDateTime(dt.Rows[i]["dOrderPurposesDate"].ToString()).ToString();
                    string dOrderReceiveDate = dt.Rows[i]["dOrderReceiveDate"] == System.DBNull.Value ? "null" : Convert.ToDateTime(dt.Rows[i]["dOrderReceiveDate"].ToString()).ToString();
                    string vcReceiveTimes = dt.Rows[i]["vcReceiveTimes"] == System.DBNull.Value ? "" : dt.Rows[i]["vcReceiveTimes"].ToString();
                    string dActualReceiveDate = dt.Rows[i]["dActualReceiveDate"] == System.DBNull.Value ? "null" : Convert.ToDateTime(dt.Rows[i]["dActualReceiveDate"].ToString()).ToString();
                    string vcAccountOrderNo = dt.Rows[i]["vcAccountOrderNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcAccountOrderNo"].ToString();
                    string dAccountOrderReceiveDate = dt.Rows[i]["dAccountOrderReceiveDate"] == System.DBNull.Value ? "null" : Convert.ToDateTime(dt.Rows[i]["dAccountOrderReceiveDate"].ToString()).ToString();
                    string vcMemo = dt.Rows[i]["vcMemo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMemo"].ToString();


                    strSql.AppendLine("  INSERT INTO [dbo].[TOralTestManage]   ");
                    strSql.AppendLine("             ([dExportDate] ,[vcCarType] ,[vcPartNo],[vcPartName]  ,[vcInsideOutsideType] ,[vcSupplier_id]   ");
                    strSql.AppendLine("             ,[vcWorkArea] ,[vcIsNewRulesFlag] ,[vcOEOrSP] ,[vcDock] ,[vcNumber],[vcPurposes] ,[dOrderPurposesDate] ,[dOrderReceiveDate]   ");
                    strSql.AppendLine("             ,[vcReceiveTimes]  ,[dActualReceiveDate],[vcAccountOrderNo] ,[dAccountOrderReceiveDate]   ,[vcMemo] ,   ");
                    strSql.AppendLine("  		   [vcOperatorID] ,[dOperatorTime])   ");
                    strSql.AppendLine("  values(   ");
                    strSql.AppendLine("   GETDATE(),'" + vcCarType + "','" + vcPartNo + "','" + vcPartName + "','" + vcInsideOutsideType + "','" + vcSupplier_id + "'  ");
                    strSql.AppendLine("   ,'" + vcWorkArea + "','" + vcIsNewRulesFlag + "','" + vcOEOrSP + "','" + vcDock + "','" + vcNumber + "','" + vcPurposes + "','" + dOrderPurposesDate + "','" + dOrderReceiveDate + "'  ");
                    strSql.AppendLine("   ,'" + vcReceiveTimes + "','" + dActualReceiveDate + "','" + vcAccountOrderNo + "','" + dAccountOrderReceiveDate + "','" + vcMemo + "'  ");
                    strSql.AppendLine("   ,'" + strUserId + "',GETDATE()) ;   ");
                   
                }
                if (strSql.Length>0)
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

        /// <summary>
        ///  通过code 获取抄送者邮箱  vcMeaning='1' 启用中 默认为0 不启用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable getCCEmail(string code)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcValue as displayName ,vcName as address from TCode where vcCodeId='C053' and vcMeaning='1' ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过供应商 工区 获取指定邮箱
        /// </summary>
        /// <param name="vcSupplier_id"></param>
        /// <param name="vcWorkArea"></param>
        /// <returns></returns>
        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcLinkMan,vcEmail from [dbo].[TSupplierInfo] where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "'   ");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
