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
        public DataTable getPinMuInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcBigPM as vcValue,vcBigPM as vcName from TPMRelation");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strPlant, string strPinMu, string strPartId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select c.LinId as LinId");
                strSql.AppendLine(",d.vcName as vcPlant");
                strSql.AppendLine(",a.vcPart_id as vcPartId");
                strSql.AppendLine(",b.vcBigPM as vcPinMu");
                strSql.AppendLine(",isnull(c.vcPicUrl_small,'暂无图像.jpg') as vcOperImage_samll");
                strSql.AppendLine(",isnull(c.vcPicUrl,'暂无图像.jpg') as vcOperImage");
                strSql.AppendLine(",c.vcOperatorID as vcOperator");
                strSql.AppendLine(",c.dOperatorTime as vcOperatorTime from ");
                strSql.AppendLine("(select * from TPackageMaster)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMRelation)b");
                strSql.AppendLine("on a.vcSmallPM=b.vcSmallPM");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPackOperImage)c");
                strSql.AppendLine("on a.vcPart_id=c.vcPartId and a.vcBZPlant=c.vcPlant");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C023')d");
                strSql.AppendLine("on a.vcBZPlant=d.vcValue");
                strSql.AppendLine("where 1=1");
                if (strPlant != "")
                {
                    strSql.AppendLine("and a.vcBZPlant='" + strPlant + "' ");
                }
                if (strPlant != "")
                {
                    strSql.AppendLine("and b.vcBigPM='" + strPinMu + "'");
                }
                if (strPlant != "")
                {
                    strSql.AppendLine("and a.vcPart_id like '" + strPartId + "%'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setDeleteInfo(List<Dictionary<string, Object>> listInfoData)
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
                stringBuilder.AppendLine("delete from TPackOperImage where LinId =@LinId");
                sqlCommand.CommandText = stringBuilder.ToString();

                sqlCommand.Parameters.AddWithValue("@LinId", "");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlCommand.Parameters["@LinId"].Value = listInfoData[i]["LinId"].ToString();
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
    }
}
