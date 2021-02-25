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
    public class FS1310_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPackPlant, string strPinMu, string strPartId, string strOperImage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select c.LinId as LinId");
                strSql.AppendLine(",a.vcBZPlant as vcPackPlant");
                strSql.AppendLine(",d.vcName as vcPackPlant_name");
                strSql.AppendLine(",a.vcPart_id as vcPartId");
                strSql.AppendLine(",b.vcBigPM as vcPinMu");
                strSql.AppendLine(",isnull(c.vcPicUrl_small,'/暂无图像.jpg') as vcOperImage_samll");
                strSql.AppendLine(",isnull(c.vcPicUrl,'/暂无图像.jpg') as vcOperImage");
                strSql.AppendLine(",c.vcOperatorID as vcOperator");
                strSql.AppendLine(",c.dOperatorTime as vcOperatorTime from ");
                strSql.AppendLine("(select * from TPackageMaster)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMRelation)b");
                strSql.AppendLine("on a.vcSmallPM=b.vcSmallPM");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPackOperImage)c");
                strSql.AppendLine("on a.vcPart_id=c.vcPartId");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C023')d");
                strSql.AppendLine("on a.vcBZPlant=d.vcValue");
                strSql.AppendLine("where 1=1");
                if (strPackPlant != "")
                {
                    strSql.AppendLine("and a.vcBZPlant='" + strPackPlant + "' ");
                }
                if (strPinMu != "")
                {
                    strSql.AppendLine("and b.vcBigPM='" + strPinMu + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("and a.vcPart_id like '" + strPartId + "%'");
                }
                if (strOperImage != "")
                {
                    if (strOperImage == "0")
                    {
                        strSql.AppendLine("and c.vcPicUrl is null");
                    }
                    else
                    {
                        strSql.AppendLine("and c.vcPicUrl is not null");
                    }
                }
                strSql.AppendLine("order by d.vcName desc,a.vcPart_id desc");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable getSubInfo(string strPlant, string strPartNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.vcPlant,a.vcPartId,isnull(a.vcPicUrl,'暂无图像.jpg') as vcPicUrl,isnull(a.vcPicUrl_small,'暂无图像.jpg') as vcPicUrl_small,c.vcBigPM,'' as vcPartName from");
                strSql.AppendLine("(select * from TPackOperImage)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPackageMaster)b");
                strSql.AppendLine("on  a.vcPartId=b.vcPart_id");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMRelation)c");
                strSql.AppendLine("on b.vcSmallPM=b.vcSmallPM");
                strSql.AppendLine("where vcPlant='" + strPlant + "' and vcPartId='" + strPartNo + "'");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPackOperImage(string strPartId, string strPinMu, string strPackPlant, string strOperImage, string strOperId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("declare @flag int");
                strSql.AppendLine("set @flag=(select count(*) from TPackOperImage where vcPartId='" + strPartId + "')");
                strSql.AppendLine("if(@flag=0)");
                strSql.AppendLine("begin");
                strSql.AppendLine("INSERT INTO [dbo].[TPackOperImage]");
                strSql.AppendLine("           ([vcPlant]");
                strSql.AppendLine("           ,[vcPartId]");
                strSql.AppendLine("           ,[vcPicUrl_small]");
                strSql.AppendLine("           ,[vcPicUrl]");
                strSql.AppendLine("           ,[vcOperatorID]");
                strSql.AppendLine("           ,[dOperatorTime])");
                strSql.AppendLine("     VALUES");
                strSql.AppendLine("           ('" + strPackPlant + "'");
                strSql.AppendLine("           ,'" + strPartId + "'");
                strSql.AppendLine("           ,null");
                strSql.AppendLine("           ,'" + strOperImage + "'");
                strSql.AppendLine("           ,'" + strOperId + "'");
                strSql.AppendLine("           ,GETDATE())");
                strSql.AppendLine("end");
                strSql.AppendLine("else ");
                strSql.AppendLine("begin");
                strSql.AppendLine("UPDATE [TPackOperImage] SET [vcPicUrl]='" + strOperImage + "',[vcOperatorID]='" + strOperId + "',[dOperatorTime]=GETDATE() where vcPartId='" + strPartId + "' ");
                strSql.AppendLine("end");
                excute.ExcuteSqlWithStringOper(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                //启动事务
                SqlCommand sqlCommand = sqlConnection.CreateCommand();//主表
                sqlCommand.Transaction = sqlTransaction;
                sqlCommand.CommandType = CommandType.Text;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("delete from TPackOperImage where vcpartid =@vcpartid");
                sqlCommand.CommandText = stringBuilder.ToString();

                sqlCommand.Parameters.AddWithValue("@vcpartid", "");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlCommand.Parameters["@vcpartid"].Value = listInfoData[i]["vcPartId"].ToString();
                    sqlCommand.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
    }
}
