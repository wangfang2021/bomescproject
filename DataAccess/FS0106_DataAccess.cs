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
    public class FS0106_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable BindConst()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select distinct vcCodeId,vcCodeName From [dbo].[TOutCode]    ");
               
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
        public DataSet Search(string vcCodeId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select  iAutoId, vcCodeId, vcCodeName, vcIsColum, vcValue1, vcValue2, vcValue3, vcValue4, vcValue5, vcValue6, vcValue7, vcValue8, vcValue9, vcValue10, vcOperatorID, dOperatorTime,'0' as vcModFlag,'0' as vcAddFlag From [dbo].[TOutCode] where vcCodeId='" + vcCodeId + "'   ");
                strSql.AppendLine("   select top(1) '常量代码' as vcCodeId, '常量名称' as vcCodeName, vcValue1, vcValue2, vcValue3, vcValue4, vcValue5, vcValue6, vcValue7, vcValue8, vcValue9, vcValue10 From [dbo].[TOutCode] where vcIsColum=1 and vcCodeId='" + vcCodeId + "' order by  iAutoId desc  ");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool isExist(string vcCodeIdStr, string vcCodeNameStr)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select  [vcCodeId],[vcCodeName] From [dbo].[TOutCode]  where vcCodeId='" + vcCodeIdStr + "' and vcCodeName='"+ vcCodeNameStr + "'   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString()).Rows.Count>0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT vcCodeId ,vcCodeName  ,vcValue ,vcName ,vcMeaning FROM [dbo].[TCode] where vcCodeId='" + dtadd.Rows[i]["vcCodeId"] + "' and vcValue='" + dtadd.Rows[i]["vcValue"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT vcCodeId ,vcCodeName  ,vcValue ,vcName ,vcMeaning FROM [dbo].[TCode] where vcCodeId='" + dtadd.Rows[i]["vcCodeId"] + "' and vcValue='" + dtadd.Rows[i]["vcValue"] + "'  ");
                    }
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                sql.Append("  delete [TOutCode] where iAutoId in(   \r\n ");
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
        /// 保存
        /// </summary>
        /// <param name="dtadd"></param>
        /// <param name="userId"></param>

        public void Save(DataTable dt, string vcCodeId, string vcCodeName, string userId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool bModFlag = dt.Rows[i]["vcModFlag"].ToString().ToUpper()=="TRUE"?true:false;//true可编辑,false不可编辑
                    bool bAddFlag = dt.Rows[i]["vcAddFlag"].ToString().ToUpper() == "TRUE" ? true : false;//true可编辑,false不可编辑
                    //   INSERT INTO[dbo].[TOutCode]
                    //   ([vcCodeId],[vcCodeName],[vcIsColum],[vcValue1]
                    //     ,[vcValue2],[vcValue3],[vcValue4],[vcValue5]
                    //     ,[vcValue6],[vcValue7],[vcValue8]
                    //     ,[vcValue9],[vcValue10],[vcOperatorID],[dOperatorTime])
                    //VALUES()
                    string vcIsColum = "0";
                    string vcValue1 = dt.Rows[i]["vcValue1"].ToString() == "" ? null: dt.Rows[i]["vcValue1"].ToString();
                    string vcValue2 = dt.Rows[i]["vcValue2"].ToString() == "" ? null : dt.Rows[i]["vcValue2"].ToString();
                    string vcValue3 = dt.Rows[i]["vcValue3"].ToString() == "" ? null : dt.Rows[i]["vcValue3"].ToString();
                    string vcValue4 = dt.Rows[i]["vcValue4"].ToString() == "" ? null : dt.Rows[i]["vcValue4"].ToString();
                    string vcValue5 = dt.Rows[i]["vcValue5"].ToString() == "" ? null : dt.Rows[i]["vcValue5"].ToString();
                    string vcValue6 = dt.Rows[i]["vcValue6"].ToString() == "" ? null : dt.Rows[i]["vcValue6"].ToString();
                    string vcValue7 = dt.Rows[i]["vcValue7"].ToString() == "" ? null : dt.Rows[i]["vcValue7"].ToString();
                    string vcValue8 = dt.Rows[i]["vcValue8"].ToString() == "" ? null : dt.Rows[i]["vcValue8"].ToString();
                    string vcValue9 = dt.Rows[i]["vcValue9"].ToString() == "" ? null : dt.Rows[i]["vcValue9"].ToString();
                    string vcValue10 = dt.Rows[i]["vcValue10"].ToString() == "" ? null : dt.Rows[i]["vcValue10"].ToString();

                    if (bAddFlag == true)
                    {//新增
                        sql.Append("  INSERT INTO [dbo].[TOutCode]   \n");
                        sql.Append("             ([vcCodeId],[vcCodeName] ,[vcIsColum],[vcValue1]   \n");
                        sql.Append("             ,[vcValue2] ,[vcValue3] ,[vcValue4] ,[vcValue5]   \n");
                        sql.Append("             ,[vcValue6] ,[vcValue7] ,[vcValue8]   \n");
                        sql.Append("             ,[vcValue9],[vcValue10],[vcOperatorID],[dOperatorTime])   \n");
                        sql.Append("       VALUES(  \n");
                        sql.Append("   '" + vcCodeId + "','" + vcCodeName + "','" + vcIsColum + "'," + ComFunction.getSqlValue(vcValue1, true) + ",  \n");
                        sql.Append("   " + ComFunction.getSqlValue(vcValue2, true) + "," + ComFunction.getSqlValue(vcValue3, true) + "," + ComFunction.getSqlValue(vcValue4, true) + "," + ComFunction.getSqlValue(vcValue5, true) + ",  \n");
                        sql.Append("   " + ComFunction.getSqlValue(vcValue6, true) + "," + ComFunction.getSqlValue(vcValue7, true) + "," + ComFunction.getSqlValue(vcValue8, true) + "," + ComFunction.getSqlValue(vcValue9, true) + ",  \n");
                        sql.Append("   " + ComFunction.getSqlValue(vcValue10, true) + ",'" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(dt.Rows[i]["iAutoId"].ToString());

                        sql.Append("  update TOutCode set    \r\n");
                        sql.Append("   vcValue1="+ComFunction.getSqlValue(vcValue1, true)+"  \r\n");
                        sql.Append("   ,vcValue2=" + ComFunction.getSqlValue(vcValue2, true) + " \r\n");
                        sql.Append("   ,vcValue3=" + ComFunction.getSqlValue(vcValue3, true) + "  \r\n");
                        sql.Append("   ,vcValue4=" + ComFunction.getSqlValue(vcValue4, true) + "  \r\n");
                        sql.Append("   ,vcValue5=" + ComFunction.getSqlValue(vcValue5, true) + "  \r\n");
                        sql.Append("   ,vcValue6=" + ComFunction.getSqlValue(vcValue6, true) + "  \r\n");
                        sql.Append("   ,vcValue7=" + ComFunction.getSqlValue(vcValue7, true) + "  \r\n");
                        sql.Append("   ,vcValue8=" + ComFunction.getSqlValue(vcValue8, true) + "  \r\n");
                        sql.Append("   ,vcValue9=" + ComFunction.getSqlValue(vcValue9, true) + "  \r\n");
                        sql.Append("   ,vcValue10=" + ComFunction.getSqlValue(vcValue10, true) + "  \r\n");
                        sql.Append("  ,vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
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
                strErrorPartId= ex.Message;
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
