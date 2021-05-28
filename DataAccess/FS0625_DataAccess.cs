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
               
                strSql.AppendLine("  select vcValue1 as vcValue, vcValue2  as vcName from TOutCode where vcCodeId='C012' and vcIsColum='0'  ");
                  
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
        public DataTable Search(string dExportDate, string vcCarType, string vcPartNo, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcIsNewRulesFlag, string vcPurposes,string vcOESP,string dOrderPurposesDate,string vcEmailFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select [iAutoId],convert(varchar(10), dExportDate,111) as [dExportDate], [vcCarType], [vcPartNo], [vcPartName],c.vcName as [vcInsideOutsideType],   ");
                strSql.AppendLine("    a.[vcSupplier_id], [vcWorkArea],case when vcIsNewRulesFlag='1' then '√' else ' ' end as [vcIsNewRulesFlag], d.vcName as [vcOEOrSP], [vcDock],cast(isnull(vcNumber,0) as int) as [vcNumber],e.vcName as [vcPurposes],   ");
                strSql.AppendLine("   convert(varchar(10), dOrderPurposesDate,111) as [dOrderPurposesDate], [dOrderReceiveDate], [vcReceiveTimes],[vcActualNum],convert(varchar(10), dActualReceiveDate,111) as [dActualReceiveDate], [vcAccountOrderNo],    ");
                strSql.AppendLine("   convert(varchar(10), dAccountOrderReceiveDate,111) as [dAccountOrderReceiveDate], convert(varchar(10), dOrderSendDate,111) as [dOrderSendDate], [vcMemo],'' as vcSupplier_name,case when isnull(vcEmailFlag,'0')='0' then '待发送' else '已发送' end as vcEmailFlag, [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag   ");
                strSql.AppendLine("   from TOralTestManage a    ");
                //strSql.AppendLine("   left join (select vcSupplier_id,vcSupplier_name from Tsupplier) b on a.vcSupplier_id = b.vcSupplier_id   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C003') c on a.vcInsideOutsideType = c.vcValue   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C012') d on a.vcOEOrSP = d.vcValue  ");
                strSql.AppendLine("   left join (select vcValue1 as vcValue, vcValue2  as vcName from TOutCode where vcCodeId='C012' and vcIsColum='0') e on a.vcPurposes = e.vcValue  where 1=1  ");

                if (dExportDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dExportDate,112) = '" + dExportDate.Replace("-","").Replace("/", "") + "' ");
                }
                if (vcEmailFlag.Length > 0)
                {
                    strSql.AppendLine("  and  isnull(vcEmailFlag,'0') = '" + vcEmailFlag + "' ");
                }
                if (vcCarType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcCarType like  '" + vcCarType + "%' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPartNo like  '" + vcPartNo + "%' ");
                }
                if (vcInsideOutsideType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcInsideOutsideType = '" + vcInsideOutsideType + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and   a.vcSupplier_id    like  '" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcWorkArea like '" + vcWorkArea + "%' ");
                }
                if (vcIsNewRulesFlag.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcIsNewRulesFlag = '" + vcIsNewRulesFlag + "' ");
                }
                if (vcPurposes.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPurposes = '" + vcPurposes + "' ");
                }
                if (vcOESP.Length > 0)
                {
                    strSql.AppendLine("  and   a.vcOEOrSP = '" + vcOESP + "' ");
                }
                if (dOrderPurposesDate.Length > 0)
                {
                    if (dOrderPurposesDate == "无")
                    {
                        strSql.AppendLine("  and  isnull(a.dOrderPurposesDate,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  CONVERT(varchar(10),  a.dOrderPurposesDate,112) = '" + dOrderPurposesDate.Replace("-", "").Replace("/", "") + "' ");
                    }
                }
                strSql.AppendLine("  order by  iAutoId desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select  vcWorkArea as vcValue,vcWorkArea as vcName  from (   ");
                strSql.AppendLine("  select distinct  isnull (vcWorkArea,'无') as vcWorkArea from TOralTestManage where vcSupplier_id='"+ supplierCode + "'   ");
                strSql.AppendLine("  ) a  order by a.vcWorkArea asc   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetOrderPurposesDate()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select isnull(dOrderPurposesDate,'无') as vcValue,isnull(dOrderPurposesDate,'无') as vcName from(       ");
                strSql.AppendLine("   select  distinct convert(varchar(10), dOrderPurposesDate,111) as dOrderPurposesDate from [TOralTestManage]    ");
                strSql.AppendLine("   ) t order by vcValue desc     ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updateOrderPurposesDate(DataTable dtNewSupplierandWorkArea,string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [TOralTestManage] set  \n");
                sql.Append(" dOrderSendDate=GETDATE(),vcEmailFlag='1', \n");
                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                for (int i = 0; i < dtNewSupplierandWorkArea.Rows.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(dtNewSupplierandWorkArea.Rows[i]["iAutoId"]);
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

        public DataTable GetCarType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select   vcCarType as vcValue,vcCarType as vcName  from (   ");
                strSql.AppendLine("  select distinct isnull (vcCarType,'无') as    vcCarType from [dbo].TOralTestManage   ");
                strSql.AppendLine("  ) a order by a.vcCarType asc   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select  vcSupplier_id as vcValue,vcSupplier_id as vcName  from (   ");
                strSql.AppendLine("   select distinct  isnull (vcSupplier_id,'无') as vcSupplier_id from TOralTestManage   ");
                strSql.AppendLine("   ) a  order by a.vcSupplier_id asc   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetWorkArea()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select  vcWorkArea as vcValue,vcWorkArea as vcName  from (   ");
                strSql.AppendLine("  select distinct  isnull (vcWorkArea,'无') as vcWorkArea from TOralTestManage   ");
                strSql.AppendLine("  ) a  order by a.vcWorkArea asc   ");

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
                        string vcInsideOutsideType = listInfoData[i]["vcInsideOutsideType"] == null ? "" : listInfoData[i]["vcInsideOutsideType"].ToString();
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
                        string vcIsNewRulesFlag = listInfoData[i]["vcIsNewRulesFlag"] == null ? "" : listInfoData[i]["vcIsNewRulesFlag"].ToString();

                        if (vcIsNewRulesFlag == "是"|| vcIsNewRulesFlag == "√")
                        {
                            vcIsNewRulesFlag = "1";
                        }
                        else if (vcIsNewRulesFlag == "否"|| vcIsNewRulesFlag == ""|| vcIsNewRulesFlag == " ")
                        {
                            vcIsNewRulesFlag = "0";
                        }
                        else
                        { }

                        string vcOEOrSP = listInfoData[i]["vcOEOrSP"] == null ? "" : listInfoData[i]["vcOEOrSP"].ToString();
                        if (vcOEOrSP == "×")
                        {
                            vcOEOrSP = "1";
                        }
                        else if (vcOEOrSP == "⭕"|| vcOEOrSP == "○")
                        {
                            vcOEOrSP = "0";
                        }
                        else
                        { }
                        string vcPurposes = listInfoData[i]["vcPurposes"] == null ? "" : listInfoData[i]["vcPurposes"].ToString();
                        if (vcPurposes == "包装设定")
                        {
                            vcPurposes = "1";
                        }
                        else if (vcPurposes == "品质确认")
                        {
                            vcPurposes = "2";
                        }
                        else if (vcPurposes == "内制引取器具检证")
                        {
                            vcPurposes = "3";
                        }
                        else if (vcPurposes == "物流测试")
                        {
                            vcPurposes = "4";
                        }
                        else if (vcPurposes == "作业训练")
                        {
                            vcPurposes = "5";
                        }
                        else if (vcPurposes == "新规供应商")
                        {
                            vcPurposes = "6";
                        }
                        else if (vcPurposes == "其他")
                        {
                            vcPurposes = "7";
                        }
                        else
                        { }

                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("   UPDATE [dbo].[TOralTestManage]   \r\n");
                        sql.Append("      SET [vcCarType] = " + getSqlValue(listInfoData[i]["vcCarType"], false) + "   \r\n");
                        sql.Append("         ,[vcPartNo] =  " + getSqlValue(listInfoData[i]["vcPartNo"], false) + "   \r\n");
                        sql.Append("         ,[vcPartName] =  " + getSqlValue(listInfoData[i]["vcPartName"], false) + "   \r\n");
                        sql.Append("         ,[vcInsideOutsideType] =   " + getSqlValue(vcInsideOutsideType, true) + "  \r\n");
                        sql.Append("         ,[vcSupplier_id] =  " + getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "   \r\n");
                        sql.Append("         ,[vcWorkArea] =  " + getSqlValue(listInfoData[i]["vcWorkArea"], false) + "   \r\n");
                        sql.Append("         ,[vcIsNewRulesFlag] =  " + getSqlValue(vcIsNewRulesFlag, true) + "   \r\n");
                        sql.Append("         ,[vcOEOrSP] =   " + getSqlValue(vcOEOrSP, true) + "  \r\n");
                        sql.Append("         ,[vcDock] =   " + getSqlValue(listInfoData[i]["vcDock"], false) + "  \r\n");
                        sql.Append("         ,[vcNumber] =   " + getSqlValue(listInfoData[i]["vcNumber"], false) + "  \r\n");
                        sql.Append("         ,[vcPurposes] = " + getSqlValue(vcPurposes, true) + "    \r\n");
                        sql.Append("         ,[dOrderPurposesDate] = " + getSqlValue(listInfoData[i]["dOrderPurposesDate"], true) + "   \r\n");
                        sql.Append("         ,[dOrderReceiveDate] =  " + getSqlValue(listInfoData[i]["dOrderReceiveDate"], true) + "  \r\n");
                        sql.Append("         ,[vcReceiveTimes] =  " + getSqlValue(listInfoData[i]["vcReceiveTimes"], false) + "   \r\n");
                        sql.Append("         ,[vcActualNum] =  " + getSqlValue(listInfoData[i]["vcActualNum"], true) + "   \r\n");
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
                    if (vcIsNewRulesFlag == "是" || vcIsNewRulesFlag == "√")
                    {
                        vcIsNewRulesFlag = "1";
                    }
                    else if (vcIsNewRulesFlag == "否" || vcIsNewRulesFlag == "" || vcIsNewRulesFlag == " ")
                    {
                        vcIsNewRulesFlag = "0";
                    }
                    else
                    { }
                    /*   if (vcIsNewRulesFlag == "是")
                    {
                        vcIsNewRulesFlag = "1";
                    }
                    else if (vcIsNewRulesFlag == "否")
                    {
                        vcIsNewRulesFlag = "0";
                    }
                    else
                    { }*/
                    string vcOEOrSP = dt.Rows[i]["vcOEOrSP"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOEOrSP"].ToString();
                    if (vcOEOrSP == "×")
                    {
                        vcOEOrSP = "1";
                    }
                    else if (vcOEOrSP == "⭕" || vcOEOrSP == "○")
                    {
                        vcOEOrSP = "0";
                    }
                    else
                    { }
                    string vcDock = dt.Rows[i]["vcDock"] == System.DBNull.Value ? "" : dt.Rows[i]["vcDock"].ToString();
                    string vcNumber = dt.Rows[i]["vcNumber"] == System.DBNull.Value ? "" : dt.Rows[i]["vcNumber"].ToString();
                    string vcPurposes = dt.Rows[i]["vcPurposes"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPurposes"].ToString();
                    if (vcPurposes == "包装设定")
                    {
                        vcPurposes = "1";
                    }
                    else if (vcPurposes == "品质确认")
                    {
                        vcPurposes = "2";
                    }
                    else if (vcPurposes == "内制引取器具检证")
                    {
                        vcPurposes = "3";
                    }
                    else if (vcPurposes == "物流测试")
                    {
                        vcPurposes = "4";
                    }
                    else if (vcPurposes == "作业训练")
                    {
                        vcPurposes = "5";
                    }
                    else if (vcPurposes == "新规供应商")
                    {
                        vcPurposes = "6";
                    }
                    else
                    {
                        vcPurposes = "7";
                    }

                    string dOrderPurposesDate = dt.Rows[i]["dOrderPurposesDate"].ToString() == "" ? null : Convert.ToDateTime(dt.Rows[i]["dOrderPurposesDate"].ToString()).ToString();
                    string dOrderReceiveDate = dt.Rows[i]["dOrderReceiveDate"].ToString() == "" ? null : dt.Rows[i]["dOrderReceiveDate"].ToString();
                    string vcReceiveTimes = dt.Rows[i]["vcReceiveTimes"].ToString() == "" ? "" : dt.Rows[i]["vcReceiveTimes"].ToString();
                    string vcActualNum = dt.Rows[i]["vcActualNum"].ToString() == "" ? "" : dt.Rows[i]["vcActualNum"].ToString();
                    string dActualReceiveDate = dt.Rows[i]["dActualReceiveDate"].ToString() == "" ? null : Convert.ToDateTime(dt.Rows[i]["dActualReceiveDate"].ToString()).ToString();
                    string vcAccountOrderNo = dt.Rows[i]["vcAccountOrderNo"].ToString() == "" ? "" : dt.Rows[i]["vcAccountOrderNo"].ToString();
                    string dAccountOrderReceiveDate = dt.Rows[i]["dAccountOrderReceiveDate"].ToString() == "" ? null : Convert.ToDateTime(dt.Rows[i]["dAccountOrderReceiveDate"].ToString()).ToString();
                    string vcMemo = dt.Rows[i]["vcMemo"].ToString() == "" ? "" : dt.Rows[i]["vcMemo"].ToString();


                    strSql.AppendLine("  INSERT INTO [dbo].[TOralTestManage]   ");
                    strSql.AppendLine("             ([dExportDate] ,[vcCarType] ,[vcPartNo],[vcPartName]  ,[vcInsideOutsideType] ,[vcSupplier_id]   ");
                    strSql.AppendLine("             ,[vcWorkArea] ,[vcIsNewRulesFlag] ,[vcOEOrSP] ,[vcDock] ,[vcNumber],[vcPurposes] ,[dOrderPurposesDate] ,[dOrderReceiveDate]   ");
                    strSql.AppendLine("             ,[vcReceiveTimes],vcActualNum  ,[dActualReceiveDate],[vcAccountOrderNo] ,[dAccountOrderReceiveDate]   ,[vcMemo] ,   ");
                    strSql.AppendLine("  		   [vcOperatorID] ,[dOperatorTime],vcEmailFlag)   ");
                    strSql.AppendLine("  values(   ");
                    strSql.AppendLine("   GETDATE()," + getSqlValue(vcCarType, true) + "," + getSqlValue(vcPartNo, true) + "," + getSqlValue(vcPartName, true) + "," + getSqlValue(vcInsideOutsideType, true) + "," + getSqlValue(vcSupplier_id, true) + "  ");
                    strSql.AppendLine("   ," + getSqlValue(vcWorkArea, true) + "," + getSqlValue(vcIsNewRulesFlag, true) + "," + getSqlValue(vcOEOrSP, true) + "," + getSqlValue(vcDock, true) + "," + getSqlValue(vcNumber, true) + "," + getSqlValue(vcPurposes, true) + "," + getSqlValue(dOrderPurposesDate, true) + "," + getSqlValue(dOrderReceiveDate, true) + "  ");
                    strSql.AppendLine("   ," + getSqlValue(vcReceiveTimes, true) + "," + getSqlValue(vcActualNum, true) + "," + getSqlValue(dActualReceiveDate, true) + "," + getSqlValue(vcAccountOrderNo, true) + "," + getSqlValue(dAccountOrderReceiveDate, true) + "," + getSqlValue(vcMemo, true) + "  ");
                    strSql.AppendLine("   ,'" + strUserId + "',GETDATE(),'0') ;   ");
                   
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
        public DataTable getEmail(string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select iAutoId, vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan1, vcPhone1, vcEmail1, vcLinkMan2, vcPhone2, vcEmail2, vcLinkMan3, vcPhone3, vcEmail3 from [dbo].[TSupplierInfo] where vcSupplier_id='" + vcSupplier_id + "'  ");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getHSHD(string vcCodeID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select vcValue,vcName from TCode where vcCodeId='"+ vcCodeID + "'  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
