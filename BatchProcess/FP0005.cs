using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 接口资材数据更新包材基础数据
/// </summary>
namespace BatchProcess
{
    public class FP0005
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0005";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI0501", null, strUserId);
                string strAllGPSNO = "";
                //获取所有待更新GPS品番
                DataTable dtPack = this.GetGPSNO();
                if (dtPack.Rows.Count == 0)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE0501", null, strUserId);
                    return true;
                }
                //从资材数据库获取对应品番的属性
                string strMapsGPSNo = "";
                DataTable dtMAPS = this.GetGPSNOData_MAPS(strAllGPSNO, dtPack);
                if (dtMAPS.Rows.Count == 0)
                {//没有要结果的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE0502", null, strUserId);
                    return true;
                }
                //更新
                bool isSuccess = UPDate(dtPack, dtMAPS, strUserId);
                if (isSuccess)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PI0502", null, strUserId);
                    return true;
                }
                else
                {
                    //更新失败
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE0503", null, strUserId);
                    return false;
                }

            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE0503", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 获取所有待更新GPS品番
        public DataTable GetGPSNO()
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                DataTable dtBC = new DataTable();
                sql.Append("  SELECT [iAutoId]  \n");
                sql.Append("        ,[vcPackNo]  \n");
                sql.Append("        ,[vcPackSpot]  \n");
                sql.Append("        ,[dPackFrom]  \n");
                sql.Append("        ,[dPackTo]  \n");
                sql.Append("        ,[vcPackGPSNo]  \n");
                sql.Append("        ,[vcSupplierCode]  \n");
                sql.Append("        ,[vcSupplierPlant]  \n");
                sql.Append("        ,[vcCycle]  \n");
                sql.Append("        ,[vcSupplierName]  \n");
                sql.Append("        ,[vcParstName]  \n");
                sql.Append("        ,[vcPackLocation]  \n");
                sql.Append("        ,[vcDistinguish]  \n");
                sql.Append("        ,[vcFormat]  \n");
                sql.Append("        ,[vcReleaseID]  \n");
                sql.Append("        ,[vcReleaseName]  \n");
                sql.Append("        ,[iRelease]  \n");
                sql.Append("        ,[iZCRelease]  \n");
                sql.Append("        ,[isYZC]  \n");
                sql.Append("        ,[vcOperatorID]  \n");
                sql.Append("        ,[dOperatorTime]  \n");
                sql.Append("    FROM [TPackBase] where vcPackGPSNo is not null  \n");
                dtBC = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                return dtBC;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 从资材数据库获取对应品番的属性
        public DataTable GetGPSNOData_MAPS(string strTempGpsNo, DataTable dtPack)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DataTable dt = new DataTable();
                sql.Append("  select a.SUPPLIER_CODE,a.PART_NAME,a.SPECIFICATION,a.ORDER_LOT,b.SUPPLIER_NAME,a.PART_NO from (  \n");
                sql.Append("  select SUPPLIER_CODE,PART_NAME,SPECIFICATION,ORDER_LOT,PART_NO   \n");
                sql.Append("  from TB_M0050 where PART_NO in (  \n");
                for (int i = 0; i < dtPack.Rows.Count; i++)
                {
                    if (dtPack.Rows.Count - i == 1)
                        sql.Append("  '" + dtPack.Rows[i]["vcPackGPSNo"].ToString() + "' \n");
                    else
                        sql.Append("  '" + dtPack.Rows[i]["vcPackGPSNo"].ToString() + "', \n");
                }

                sql.Append("  ) and DEL_FLAG='0' \n");
                sql.Append("  )a left join  \n");
                sql.Append("  (  \n");
                sql.Append("  SELECT * from TB_M0100  \n");
                sql.Append("  )b on a.SUPPLIER_CODE=b.SUPPLIER_CODE  \n");

                dt = this.MAPSSearch(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 资材系统取数据连接
        public DataTable MAPSSearch(string sql)
        {
            SqlConnection conn = Common.ComConnectionHelper.CreateConnection_MAPS();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = sql;
                Common.ComConnectionHelper.OpenConection_SQL(ref conn);
                da.Fill(dt);
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                return dt;
            }
            catch (Exception ex)
            {
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion

        #region 更新

        public bool UPDate(DataTable dtPack, DataTable dtMAPS, string strUserId)
        {
            try
            {
                //a.SUPPLIER_CODE,a.PART_NAME,a.SPECIFICATION,a.ORDER_LOT,b.SUPPLIER_NAME,PART_NO
                StringBuilder sql = new StringBuilder();
                //删除从资材找到所有GPSNO数据
                sql.Append("delete from [TPackBase] where vcPackGPSNo in(   \n");
                for (int j = 0; j < dtMAPS.Rows.Count; j++)
                {
                    if (dtMAPS.Rows.Count - j == 1)
                        sql.Append(" '"+ dtMAPS.Rows[j]["PART_NO"].ToString() + "' \n");
                    else
                        sql.Append(" '" + dtMAPS.Rows[j]["PART_NO"].ToString() + "', \n");

                }
                sql.Append(")  \n");
                for (int i = 0; i < dtPack.Rows.Count; i++)
                {
                    DataRow[] dr = dtMAPS.Select("PART_NO='" + dtPack.Rows[i]["vcPackGPSNo"].ToString() + "'");
                    if (dr.Length > 0)
                    {
                        sql.Append("   insert into [TPackBase]  \n");
                        sql.Append("       ([vcPackNo]  \n");
                        sql.Append("       ,[vcPackSpot]  \n");
                        sql.Append("       ,[dPackFrom]  \n");
                        sql.Append("       ,[dPackTo]  \n");
                        sql.Append("       ,[vcPackGPSNo]  \n");
                        sql.Append("       ,[vcSupplierCode]  \n");
                        sql.Append("       ,[vcSupplierPlant]  \n");
                        sql.Append("       ,[vcCycle]  \n");
                        sql.Append("       ,[vcSupplierName]  \n");
                        sql.Append("       ,[vcParstName]  \n");
                        sql.Append("       ,[vcPackLocation]  \n");
                        sql.Append("       ,[vcDistinguish]  \n");
                        sql.Append("       ,[vcFormat]  \n");
                        sql.Append("       ,[vcReleaseID]  \n");
                        sql.Append("       ,[vcReleaseName]  \n");
                        sql.Append("       ,[iRelease]  \n");
                        sql.Append("       ,[iZCRelease]  \n");
                        sql.Append("       ,[isYZC]  \n");
                        sql.Append("       ,[vcOperatorID]  \n");
                        sql.Append("        ,[dOperatorTime] )\n");
                        sql.Append("    values \n");
                        sql.Append("    ( \n");
                        sql.Append("         '" + dtPack.Rows[i]["vcPackNo"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["vcPackSpot"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["dPackFrom"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["dPackTo"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["vcPackGPSNo"].ToString() + "', \n");
                        sql.Append("         '" + dr[0]["SUPPLIER_CODE"].ToString() + "', \n");//供应商代码
                        sql.Append("         '" + dtPack.Rows[i]["vcSupplierPlant"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["vcCycle"].ToString() + "', \n");
                        sql.Append("         '" + dr[0]["SUPPLIER_NAME"].ToString() + "', \n");//供应商名称
                        sql.Append("         '" + dr[0]["PART_NAME"].ToString() + "', \n");//品名
                        sql.Append("         '" + dtPack.Rows[i]["vcPackLocation"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["vcDistinguish"].ToString() + "', \n");
                        sql.Append("         '" + dr[0]["SPECIFICATION"].ToString() + "', \n");//规格
                        sql.Append("         '" + dtPack.Rows[i]["vcReleaseID"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["vcReleaseName"].ToString() + "', \n");
                        sql.Append("         '" + dtPack.Rows[i]["iRelease"].ToString() + "', \n");
                        sql.Append("         '" + dr[0]["ORDER_LOT"].ToString() + "', \n");//订购批量
                        sql.Append("         '1', \n");//来自资材系统页面不可编辑
                        sql.Append("         '" + strUserId + "', \n");
                        sql.Append("         getdate() \n");
                        sql.Append("     ) \n");

                    }
                }
                int isok = 1;
                if (sql.Length > 0)
                {
                    isok = excute.ExcuteSqlWithStringOper(sql.ToString());
                }
                if (isok > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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
