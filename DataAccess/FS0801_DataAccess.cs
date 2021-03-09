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
    public class FS0801_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcBZPlant, string vcPart_id, string vcBigPM, string vcSmallPM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcPart_id,t1.dTimeFrom,t1.dTimeTo,t1.vcBZPlant,t6.vcName as vcBZPlantName,t1.vcSR,  \n");
                strSql.Append("t4.vcBigPM,t1.vcSmallPM,t4.vcStandardTime,t1.vcBZQF,t7.vcName as vcBZQFName,t1.vcBZUnit,t1.vcRHQF,t8.vcName as vcRHQFName,  \n");
                strSql.Append("t1.vcReceiver,t1.vcSupplierId,t1.vcPackingPlant,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("from TPackageMaster t1  \n");
                //strSql.Append("left join (select distinct vcPartsNo,vcPackNo from TPackItem where getdate() between dFrom and dTo) t2 on t1.vcPart_id=t2.vcPartsNo  \n");
                //strSql.Append("left join TPMSmall t3 on left(t1.vcPart_id,5)=t3.vcPartsNoBefore5 and t1.vcSR=t3.vcSR   \n");
                //strSql.Append("and t2.vcPackNo = t3.vcBCPartsNo and t1.vcSupplierId=t3.vcSupplier_id  \n");
                strSql.Append("left join TPMRelation t4 on t1.vcSmallPM=t4.vcSmallPM  \n");
                //strSql.Append("left join TPMStandardTime t5 on t4.vcBigPM=t5.vcBigPM  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C023') t6 on t1.vcBZPlant=t6.vcValue  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C062') t7 on t1.vcBZQF=t7.vcValue  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C063') t8 on t1.vcRHQF=t8.vcValue  \n");
                strSql.Append("where 1=1   \n");
                if (vcBZPlant != "" && vcBZPlant != null)
                    strSql.Append("and isnull(t1.vcBZPlant,'') like '" + vcBZPlant + "%'  \n");
                if (vcPart_id != "" && vcPart_id != null)
                    strSql.Append("and isnull(t1.vcPart_id,'') like '%" + vcPart_id + "%'  \n");
                if (vcBigPM != "" && vcBigPM != null)
                    strSql.Append("and isnull(t4.vcBigPM,'') like '%" + vcBigPM + "%'  \n");
                if (vcSmallPM != "" && vcSmallPM != null)
                    strSql.Append("and isnull(t1.vcSmallPM,'') like '%" + vcSmallPM + "%'  \n");
                strSql.Append("order by t1.vcPart_id     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                        //无新增情况
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        string strBZPlant = listInfoData[i]["vcBZPlant"] == null ? "" : listInfoData[i]["vcBZPlant"].ToString();
                        string strBZQF = listInfoData[i]["vcBZQF"] == null ? "" : listInfoData[i]["vcBZQF"].ToString();
                        string strBZUnit = listInfoData[i]["vcBZUnit"] == null ? "" : listInfoData[i]["vcBZUnit"].ToString();
                        string strRHQF = listInfoData[i]["vcRHQF"] == null ? "" : listInfoData[i]["vcRHQF"].ToString();
                        string strSmallPM = listInfoData[i]["vcSmallPM"] == null ? "" : listInfoData[i]["vcSmallPM"].ToString();
                        string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                        string strTimeFrom = listInfoData[i]["dTimeFrom"].ToString();
                        string strTimeTo = listInfoData[i]["dTimeTo"].ToString();

                        sql.Append("update TPackageMaster set vcBZPlant='" + strBZPlant + "',vcBZQF='" + strBZQF + "',   \n");
                        sql.Append("vcBZUnit='" + strBZUnit + "',vcRHQF='" + strRHQF + "',vcSmallPM='" + strSmallPM + "'  \n");
                        sql.Append("where iAutoId='" + iAutoId + "'    \n");
                        #endregion
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
        #endregion

        #region 导入
        public void import(DataTable dt, string strUserId)
        {
            SqlConnection conn = ComConnectionHelper.CreateSqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlTransaction st = null;
            ComConnectionHelper.OpenConection_SQL(ref conn);
            st = conn.BeginTransaction();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("DELETE FROM [TPackageMaster_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 变量
                    string strBZQF = dt.Rows[i]["vcBZQF"].ToString();
                    switch (strBZQF)
                    {
                        case "已包装": strBZQF = "1"; break;
                        case "未包装": strBZQF = "0"; break;
                        default: strBZQF = ""; break;
                    }
                    string strRHQF = dt.Rows[i]["vcRHQF"].ToString();
                    switch (strRHQF)
                    {
                        case "可入荷": strRHQF = "1"; break;
                        case "不可入荷": strRHQF = "0"; break;
                        default: strRHQF = ""; break;
                    }
                    #endregion

                    #region 插临时表
                    sql.Append("INSERT INTO [dbo].[TPackageMaster_Temp]    \n");
                    sql.Append("           ([vcPart_id]    \n");
                    sql.Append("           ,[vcReceiver]    \n");
                    sql.Append("           ,[vcSupplierId]    \n");
                    sql.Append("           ,[vcPackingPlant]    \n");
                    sql.Append("           ,[vcSmallPM]    \n");
                    sql.Append("           ,[vcBZPlant]    \n");
                    sql.Append("           ,[vcBZQF]    \n");
                    sql.Append("           ,[vcBZUnit]    \n");
                    sql.Append("           ,[vcRHQF]    \n");
                    sql.Append("           ,[vcOperatorID]    \n");
                    sql.Append("           ,[dOperatorTime])    \n");
                    sql.Append("     VALUES    \n");
                    sql.Append("           ('" + dt.Rows[i]["vcPart_id"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcReceiver"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSupplierId"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPackingPlant"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSmallPM"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcBZPlant"].ToString() + "'    \n");
                    sql.Append("           ,'" + strBZQF + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcBZUnit"].ToString() + "'   \n");
                    sql.Append("           ,'" + strRHQF + "'    \n");
                    sql.Append("           ,'" + strUserId + "'    \n");
                    sql.Append("           ,getdate())    \n");
                    #endregion
                    if (i % 1000 == 0)
                    {
                        cmd = new SqlCommand(sql.ToString(), conn, st);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        sql.Length = 0;
                    }
                }
                sql.Append("update t2 set t2.vcBZPlant=t1.vcBZPlant,t2.vcBZQF=t1.vcBZQF,t2.vcBZUnit=t1.vcBZUnit,t2.vcRHQF=t1.vcRHQF,t2.vcSmallPM=t1.vcSmallPM,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TPackageMaster_Temp) t1  \n");
                sql.Append("inner join TPackageMaster t2 on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPart_id=t2.vcPart_id and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId \n");
                sql.Append("where t1.vcOperatorID='" + strUserId + "'  \n");

                cmd = new SqlCommand(sql.ToString(), conn, st);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();

                st.Commit();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
            }
            catch (Exception ex)
            {
                st.Rollback();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion

    }
}
