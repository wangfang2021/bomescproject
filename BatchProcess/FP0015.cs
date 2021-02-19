using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 获取资材入库数据
/// </summary>
namespace BatchProcess
{
    public class FP0015
    {
        private MultiExcute excute = new MultiExcute();
        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0015";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理开始", null, strUserId);
                //取最后一次更新时间
                DataTable dtTime = this.GetEndData();
                //去资材取从更新时间需要入库数据
                DataTable dtData = this.GetParkWorkData_MAPS(dtTime.Rows[0]["dOperatorTime"].ToString());





                //bool isok = this.UpDate(strUserId, dtNewItem);

                //if (isok)
                //{
                //    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行成功", null, strUserId);
                //    return true;
                //}
                //else
                //{
                //    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 取最后一次更新时间
        public DataTable GetEndData()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select  top 1  dOperatorTime from TPackWork  order by dOperatorTime desc   \n");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 去资材取从更新时间需要入库数据

        public DataTable GetParkWorkData_MAPS(string strDate)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DataTable dt = new DataTable();
                sql.Append("   select PART_NO,ORDER_NO,QUANTITY,CREATE_TIME from [TB_B0040] where CREATE_TIME >='"+ strDate + "' \n");
              

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

        #region 更新数据准备
        public bool UpDate(string strUserId, DataTable dtNewItem)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sql = new StringBuilder();
                sql.Append("  truncate  table TPackItem   \n");
                for (int i = 0; i < dtNewItem.Rows.Count; i++)
                {
                    sql.Append("  insert into TPackItem  \n");
                    sql.Append("  ([vcPartsNo],  \n");
                    sql.Append("   [vcPackNo],  \n");
                    sql.Append("   [vcPackGPSNo],  \n");
                    sql.Append("   [vcShouhuofangID],  \n");
                    sql.Append("   [vcShouhuofang],  \n");
                    sql.Append("   [vcCar],  \n");
                    sql.Append("   [dUsedFrom],  \n");
                    sql.Append("   [dUsedTo],  \n");
                    sql.Append("   [dFrom],  \n");
                    sql.Append("   [dTo],  \n");
                    sql.Append("   [vcDistinguish],  \n");
                    sql.Append("   [iBiYao],  \n");
                    sql.Append("   [vcOperatorID],  \n");
                    sql.Append("   [dOperatorTime],  \n");
                    sql.Append("   [varChangedItem]  \n");
                    sql.Append("   ) values  \n");
                    sql.Append("   ( \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPartsNo"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackNo"].ToString() + "' ,  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcPackGPSNo"].ToString() + "',    \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcShouhuofangID"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcShouhuofang"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcCar"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dUsedFrom"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dUsedTo"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dFrom"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["dTo"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["vcDistinguish"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["iBiYao"].ToString() + "',  \n");
                    sql.Append("  '" + strUserId + "', \n");
                    sql.Append("   GETDATE(), \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["varChangedItem"].ToString() + "'  \n");

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

                //批处理异常结束
                //ComMessage.GetInstance().ProcessMessage("FP0018", ex.ToString(), null, strUserId);
                throw ex;

            }
        }

        #endregion
    }
}