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
    public class FS0404_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

 

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcOrderState, string vcOrderNo, string dTargetDate, string vcOrderType,string userID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" select [iAutoId], [vcOrderNo],  ");
                strSql.AppendLine("    case when vcOrderType='0' then [vcTargetYear]+'/'+[vcTargetMonth]+'/'+[vcTargetDay]  ");
                strSql.AppendLine("    	when vcOrderType='1' then [vcTargetYear]+'/'+[vcTargetMonth]+''+c.vcName   ");
                strSql.AppendLine("    	when vcOrderType='2' then [vcTargetYear]+'/'+[vcTargetMonth]  ");
                strSql.AppendLine("    else '' end as [dTargetDate],  ");
                strSql.AppendLine("  b.vcName as [vcOrderType], case when [vcOrderState]=2 then '撤销' else '已上传' end as vcOrderState, [vcMemo],   ");
                strSql.AppendLine("  [dUploadDate], [vcFilePath], [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[TOrderUploadManage]  a  ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C045' )b on a.vcOrderType= b.vcValue     ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C046' )c on a.vcTargetWeek= c.vcValue      ");
                strSql.AppendLine("  left join ( select vcUnitCode,vcUserID from [dbo].[SUser] )d on a.vcOperatorID= d.vcUserID  where d.vcUnitCode=( select vcUnitCode from [dbo].[SUser] where vcUserID = '"+ userID + "')    ");
                if (vcOrderState.Length > 0)
                {
                    if (vcOrderState == "2")
                    {
                        strSql.AppendLine("  and vcOrderState = '2' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and vcOrderState <>'2' ");
                    }
                }
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderNo like  '%" + vcOrderNo + "%' ");
                }
                if (dTargetDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dUploadDate,112) = '" + dTargetDate.Replace("-", "") + "' ");
                }
                if (vcOrderType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcOrderType = '" + vcOrderType + "' ");
                }
                strSql.AppendLine("  order by  dOperatorTime desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
        /// 通过订单号查询原订单是否存在
        /// </summary>
        /// <param name="lastOrderNo"></param>
        /// <returns></returns>
        public DataTable isCheckByOrderNo(string lastOrderNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from TOrderUploadManage where vcOrderNo='"+ lastOrderNo + "'  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 修正订单操作
        /// </summary>
        /// <param name="vcOrderType"></param>
        /// <param name="dTargetDate"></param>
        /// <param name="dTargetWeek"></param>
        /// <param name="lastOrderNo"></param>
        /// <param name="newOrderNo"></param>
        /// <param name="vcMemo"></param>
        /// <param name="fileList"></param>
        public void updateBylastOrderNo(string vcOrderType, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList,string UserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = dTargetDate.Substring(0, 4);
                string vcTargetMonth = dTargetDate.Substring(4, 2);
                string vcTargetDay = dTargetDate.Substring(6, 2);
                string filePath = fileList[0]["filePath"].ToString();
                strSql.AppendLine("  update TOrderUploadManage   ");
                strSql.AppendLine("  set vcOrderNo='" + newOrderNo + "',  ");
                strSql.AppendLine("  vcTargetYear='" + vcTargetYear + "',  ");
                strSql.AppendLine("  vcTargetMonth='" + vcTargetMonth + "',  ");
                strSql.AppendLine("  vcTargetDay='" + vcTargetDay + "',  ");
                strSql.AppendLine("  vcTargetWeek='" + dTargetWeek + "',  ");
                strSql.AppendLine("  vcOrderType='" + vcOrderType + "',  ");
                strSql.AppendLine("  vcOrderState='0',  ");
                strSql.AppendLine("  vcMemo='" + vcMemo + "',  ");
                strSql.AppendLine("  dUploadDate=GETDATE(),  ");
                strSql.AppendLine("  vcFilePath='" + filePath + "',  ");
                strSql.AppendLine("  vcOperatorID='" + UserId + "',  ");
                strSql.AppendLine("  dOperatorTime=GETDATE()  ");
                strSql.AppendLine("  where vcOrderNo='" + lastOrderNo + "'  ");
                strSql.AppendLine("    ");
                strSql.AppendLine("    ");
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

        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="vcOrderType"></param>
        /// <param name="dTargetDate"></param>
        /// <param name="dTargetWeek"></param>
        /// <param name="lastOrderNo"></param>
        /// <param name="newOrderNo"></param>
        /// <param name="vcMemo"></param>
        /// <param name="fileList"></param>
        /// <param name="userId"></param>
        public void addOrderNo(string vcOrderType, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = string.Empty;
                string vcTargetMonth = string.Empty;
                string vcTargetDay = string.Empty;
                if (vcOrderType == "0")
                {
                    vcTargetYear = dTargetDate.Substring(0, 4);
                    vcTargetMonth = dTargetDate.Substring(4, 2);
                    vcTargetDay = dTargetDate.Substring(6, 2);
                }
                else if (vcOrderType == "1")
                {
                    vcTargetYear = dTargetDate.Substring(0, 4);
                }
                else if (vcOrderType == "2") {
                    vcTargetYear = dTargetDate.Substring(0, 4);
                    vcTargetMonth = dTargetDate.Substring(4, 2);
                }

                string fileName = string.Empty;
                string filePath = string.Empty;
                for (int i=0;i< fileList.Count;i++)
                {
                    fileName= fileList[i]["fileName"].ToString().Trim().Substring(0,fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    filePath = fileList[i]["filePath"].ToString();
                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate],[dCreateDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],[vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "',   ");
                    strSql.AppendLine("  		   '"+ userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");
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

    }
}
