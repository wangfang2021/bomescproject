using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                //取待更新订单号
                DataTable dtOrder = this.GetOrder();
                string date = "";
                if (dtTime.Rows.Count > 0)
                {
                    date = dtTime.Rows[0]["dUpdateTime"].ToString();
                }
                else
                {
                    DateTime dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    date = dt2.ToString();
                }

                //去资材取从更新时间需要入库数据
                DataTable dtData = this.GetParkWorkData_MAPS(date, dtOrder);
                List<Object> strSupplierCode = new List<object>();
                DataTable dtBase = this.Search("", "", "", strSupplierCode, "", "", "", "");
                DataTable dtisrkf = this.SearchRKFrist();
                DataTable dtSave = this.SearchSaveDT();
                //更新入库表
                bool isokRK = this.InsertDate(strUserId, dtData, dtBase, dtisrkf, dtSave);
                if (isokRK)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行成功", null, strUserId);
                    return true;
                }
                else
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, ex.ToString(), null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 取包材入库验收时间（最后时间）
        public DataTable GetEndData()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select top 1 dUpdateTime from TPackRuKuInFo order by dUpdateTime desc   \n");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取待更新订单号
        public DataTable GetOrder()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select vcOrderNo from TPackRuKuInFo  where vcSJTime is null or dYanshouTime is null or dEmailTime is null or vcPackNo is null  \n");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 去资材取需要入库数据

        public DataTable GetParkWorkData_MAPS(string strDate, DataTable dtOrder)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DataTable dt = new DataTable();
                sql.Append(" --订单号，订单数，订单时间，验收数，验收时间,费用负担，验收时间，电送化时间，更新时间     \n");
                sql.Append("   select c.ORDER_NO,c.ORDER_QUANTITY,c.ORDER_DATE,cast(c.RECEIVED_QUANTITY as int)as RECEIVED_QUANTITY,c.RECEIVED_DATE,c.COST_GROUP,c.YanShouDate,   \n");
                sql.Append("   c.Email_TIME,c.CREATE_TIME,c.PART_ID,d.SUPPLIER_CODE,c.UNIT,d.PART_NO,c.FI_STORE_CODE,d.CREATE_USER,e.CREATE_TIME as SJNRTime   \n");
                sql.Append("   from(   \n");
                sql.Append("    select ORDER_NO,ORDER_QUANTITY,ORDER_DATE,RECEIVED_QUANTITY,RECEIVED_DATE,COST_GROUP,YanShouDate, \n");
                sql.Append("    Email_TIME,CREATE_TIME,PART_ID,UNIT ,FI_STORE_CODE \n");
                sql.Append("    from TB_B0030 where CONVERT(datetime,CREATE_TIME)>='" + strDate + "' \n");
                sql.Append("    and (ASCII(SUBSTRING(ORDER_NO,1,1))between 65 and 90) and (ASCII(SUBSTRING(ORDER_NO,2,1))between 65 and 90) \n");
                if (dtOrder.Rows.Count > 0)
                {
                    sql.Append("  union all  \n");
                    sql.Append("  select ORDER_NO,ORDER_QUANTITY,ORDER_DATE,RECEIVED_QUANTITY,RECEIVED_DATE,COST_GROUP,YanShouDate,  \n");
                    sql.Append("  Email_TIME,CREATE_TIME,PART_ID,UNIT,FI_STORE_CODE   \n");
                    sql.Append("  from TB_B0030 where ORDER_NO in (  \n");
                    for (int i = 0; i < dtOrder.Rows.Count; i++)
                    {
                        if (dtOrder.Rows.Count - i == 1)
                        {
                            sql.AppendLine("   '" + dtOrder.Rows[i]["vcOrderNo"].ToString() + "'   \n");
                        }
                        else
                            sql.AppendLine("  '" + dtOrder.Rows[i]["vcOrderNo"].ToString() + "' ,   \n");
                    }
                    sql.Append("     ) \n");
                }
                sql.Append("   )c left join     \n");
                sql.Append("   (     \n");
                sql.Append("   select PART_ID,SUPPLIER_CODE,CREATE_USER,PART_NO from TB_M0050     \n");
                sql.Append("   )d on c.PART_ID=d.PART_ID     \n");
                sql.Append(" left join      \n");
                sql.Append(" (      \n");
                sql.Append("   select ORDER_NO,PART_NO,CREATE_USER,STORE_CODE,CREATE_TIME from  TB_B0040 where CONVERT(datetime,CREATE_TIME)>='" + strDate + "'     \n");
                sql.Append("   and (ASCII(SUBSTRING(ORDER_NO,1,1))between 65 and 90) and (ASCII(SUBSTRING(ORDER_NO,2,1))between 65 and 90)     \n");
                sql.Append(" )e on c.ORDER_NO=e.ORDER_NO     \n");
                sql.Append(" order by  PART_NO      \n");

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
        public bool InsertDate(string strUserId, DataTable dtNewItem, DataTable dtBase, DataTable dtZk, DataTable dtSave)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sql = new StringBuilder();

                #region 更新入库表
                for (int i = 0; i < dtNewItem.Rows.Count; i++)
                {
                    sql.Append("  delete from TPackRuKuInFo where vcOrderNo='" + dtNewItem.Rows[i]["ORDER_NO"].ToString() + "' \n");
                }

                for (int i = 0; i < dtNewItem.Rows.Count; i++)
                {
                    string PackNo = "";
                    string PackSpot = "";
                    if (string.IsNullOrEmpty(dtNewItem.Rows[i]["PART_NO"].ToString()))
                    {
                        PackNo = "";
                        PackSpot = "";
                    }
                    else
                    {
                        DataRow[] dr1 = dtBase.Select("vcPackGPSNo='" + dtNewItem.Rows[i]["PART_NO"] + "'");
                        PackNo = dr1[0]["vcPackNo"].ToString();
                        PackSpot = dr1[0]["vcPackSpot"].ToString();
                    }
                    sql.Append("  INSERT INTO [TPackRuKuInFo] \n");
                    sql.Append("   ([vcOrderNo]            \n");
                    sql.Append("   ,[vcPackNo]           \n");
                    sql.Append("   ,[vcPackGPSNo]           \n");
                    sql.Append("   ,[vcPackSpot]         \n");
                    sql.Append("   ,[vcSupplieCode]            \n");
                    sql.Append("   ,[iDGNum]            \n");
                    sql.Append("   ,[vcDGTime]            \n");
                    sql.Append("   ,[iSJNum]            \n");
                    sql.Append("   ,[vcSJTime]            \n");
                    sql.Append("   ,[vcUnit]             \n");
                    sql.Append("   ,[vcCostID]             \n");
                    sql.Append("   ,[vcCodeID]          \n");
                    sql.Append("   ,[dYanshouTime]      \n");
                    sql.Append("   ,[dEmailTime]       \n");
                    sql.Append("   ,[vcYanShouID]      \n");
                    sql.Append("   ,[dUpdateTime]      \n");
                    sql.Append("   ,[vcOperatorID]     \n");
                    sql.Append("   ,[dOperatorTime])   \n");
                    sql.Append("    values  \n");
                    sql.Append("   ( \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["ORDER_NO"].ToString() + "',  \n");
                    sql.Append("  '" + PackNo + "',    \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["PART_NO"].ToString() + "' ,  \n");
                    sql.Append("  '" + PackSpot + "',    \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["SUPPLIER_CODE"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["ORDER_QUANTITY"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["ORDER_DATE"].ToString() + "',  \n");

                    if (string.IsNullOrEmpty(dtNewItem.Rows[i]["RECEIVED_QUANTITY"].ToString()))
                    {
                        sql.Append("  '0.00',  \n");
                    }
                    else
                    {
                        sql.Append("  '" + dtNewItem.Rows[i]["RECEIVED_QUANTITY"].ToString() + "',  \n");
                    }
                    sql.Append("  '" + dtNewItem.Rows[i]["SJNRTime"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["UNIT"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["COST_GROUP"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["FI_STORE_CODE"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["YanShouDate"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["Email_TIME"].ToString() + "',  \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["CREATE_USER"].ToString() + "',   \n");
                    sql.Append("  '" + dtNewItem.Rows[i]["CREATE_TIME"].ToString() + "',  \n");
                    sql.Append("  '" + strUserId + "', \n");
                    sql.Append("   GETDATE()) \n");




                }
                if (dtNewItem.Rows.Count > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }

                #endregion

                DataTable dtINFO = this.SearchINFO();

                DateTime dtNow = DateTime.Now;

                for (int r = 0; r < dtINFO.Rows.Count; r++)
                {
                    string a = dtINFO.Rows[r]["vcSJTime"].ToString() == "" ? "NULL" : dtINFO.Rows[r]["vcSJTime"].ToString();
                    sql.Append(" UPDATE [dbo].[TPack_FaZhu_ShiJi] set   \n");
                    sql.Append(" iSJNumber='" + dtINFO.Rows[r]["iSJNum"].ToString() + "',  \n");
                    if (a == "NULL")
                    {

                        sql.Append($" dNaRuShiJi={a}, \n");
                    }
                    else
                    {

                        sql.Append($" dNaRuShiJi='{a}', \n");
                    }
                    if (dtINFO.Rows[r]["vcIsorNoFaZhu"].ToString() == "0")
                    {

                        sql.Append($" vcState='0' \n");
                    }
                    else if (string.IsNullOrEmpty(dtINFO.Rows[r]["dNaRuYuDing"].ToString()))
                    {
                        //订单被资材系统删除
                        sql.Append($" vcState='6' \n");
                    }
                    else
                    {
                        DateTime dtYJ = Convert.ToDateTime(dtINFO.Rows[r]["dNaRuYuDing"].ToString());
                        if (string.IsNullOrEmpty(dtINFO.Rows[r]["iSJNum"].ToString()))
                        {
                            //未纳
                            if (dtNow >= dtYJ)
                            {
                                sql.Append($" vcState='4' \n");
                            }
                            else
                            {

                                sql.Append($" vcState='3' \n");

                            }
                        }
                        else
                        {
                            //纳入

                            DateTime dtSJ = Convert.ToDateTime(dtINFO.Rows[r]["vcSJTime"].ToString());
                            if (dtSJ <= dtYJ)
                            {
                                //纳期纳入
                                sql.Append($" vcState='2' \n");
                            }
                            else
                            {
                                //超期纳入
                                sql.Append($" vcState='5' \n");

                            }
                        }

                    }
                    sql.Append("       ,[vcOperatorID] = '" + strUserId + "'   \n");
                    sql.Append("       ,[dOperatorTime] = getdate()   \n");
                    sql.Append("  WHERE [vcOrderNo]='" + dtINFO.Rows[r]["vcOrderNo"].ToString() + "'   \n");

                }

                //根据品番分组
                DataTable dtcope = dtNewItem.Copy();
                dtcope.Clear();
                DataRow[] drnew = dtNewItem.Select("SJNRTime is not null or YanShouDate is not null or Email_TIME is not null or PART_NO is not null ");
                for (int m = 0; m < drnew.Length; m++)
                {
                    //dtcope.Rows.Add(drnew[m]);
                    DataRow drImport = dtcope.NewRow();
                    drImport["ORDER_NO"] = drnew[m]["ORDER_NO"].ToString();
                    drImport["ORDER_QUANTITY"] = drnew[m]["ORDER_QUANTITY"].ToString();
                    drImport["ORDER_DATE"] = drnew[m]["ORDER_DATE"].ToString();
                    drImport["RECEIVED_QUANTITY"] = drnew[m]["RECEIVED_QUANTITY"].ToString() == "" ? "0" : drnew[m]["RECEIVED_QUANTITY"].ToString();
                    drImport["SJNRTime"] = drnew[m]["SJNRTime"].ToString();
                    drImport["COST_GROUP"] = drnew[m]["COST_GROUP"].ToString();
                    drImport["YanShouDate"] = drnew[m]["YanShouDate"].ToString();
                    drImport["Email_TIME"] = drnew[m]["Email_TIME"].ToString();
                    drImport["CREATE_TIME"] = drnew[m]["CREATE_TIME"].ToString();
                    drImport["PART_ID"] = drnew[m]["PART_ID"].ToString();
                    drImport["SUPPLIER_CODE"] = drnew[m]["SUPPLIER_CODE"].ToString();
                    drImport["UNIT"] = drnew[m]["UNIT"].ToString();
                    drImport["PART_NO"] = drnew[m]["PART_NO"].ToString();
                    drImport["FI_STORE_CODE"] = drnew[m]["FI_STORE_CODE"].ToString();
                    drImport["CREATE_USER"] = drnew[m]["CREATE_USER"].ToString();
                    dtcope.Rows.Add(drImport);
                }

                DataTable dtb = new DataTable("dtb");
                DataColumn dc1 = new DataColumn("vcPackGPSNo", Type.GetType("System.String"));
                DataColumn dc2 = new DataColumn("iSJNum", Type.GetType("System.Int32"));
                dtb.Columns.Add(dc1);
                dtb.Columns.Add(dc2);
                var query = from t in dtcope.AsEnumerable()
                            group t by new { t1 = t.Field<string>("PART_NO") }
                            into m
                            select new
                            {
                                vcPackGPSNo = m.Key.t1,
                                iSJNum = m.Sum(t => t.Field<int>("RECEIVED_QUANTITY")).ToString()
                            };
                if (query.ToList().Count > 0)
                {
                    query.ToList().ForEach(q =>
                    {
                        DataRow dr = dtb.NewRow();
                        dr["vcPackGPSNo"] = q.vcPackGPSNo;
                        dr["iSJNum"] = q.iSJNum;
                        dtb.Rows.Add(dr);
                    });
                }
                #region 更新在库表
                for (int j = 0; j < dtb.Rows.Count; j++)
                {
                    DataRow[] dr = dtZk.Select("vcPackGPSNo='" + dtb.Rows[j]["vcPackGPSNo"].ToString() + "'");
                    DataRow[] dsave = dtSave.Select("vcPackGPSNo='" + dtb.Rows[j]["vcPackGPSNo"].ToString() + "'");
                    DataRow[] dr1 = dtBase.Select("vcPackGPSNo='" + dtb.Rows[j]["vcPackGPSNo"] + "'");
                    string SaveZK = dsave.Length == 0 ? "0" : dsave[0]["vcSaveZK"].ToString();
                    if (dr.Length > 0)
                    {//更新品番在库数据
                        sql.Append(" UPDATE [dbo].[TPackZaiKu] set   \n");
                        sql.Append("        [vcPackSpot] = '" + dr1[0]["vcPackSpot"].ToString() + "'   \n");
                        sql.Append("       ,[vcPackNo] = '" + dr1[0]["vcPackNo"].ToString() + "'   \n");
                        sql.Append("       ,[vcPackGPSNo] = '" + dtb.Rows[j]["vcPackGPSNo"].ToString() + "'   \n");
                        sql.Append("       ,[vcSupplierID] = '" + dr1[0]["vcSupplierCode"].ToString() + "'   \n");
                        sql.Append("       ,[iLiLun] =iLiLun+'" + dtb.Rows[j]["iSJNum"].ToString() + "'   \n");
                        sql.Append("       ,[iAnQuan] = '" + SaveZK + "'   \n");
                        sql.Append("       ,[vcOperatorID] = '" + strUserId + "'   \n");
                        sql.Append("       ,[dOperatorTime] = getdate()   \n");
                        sql.Append("  WHERE [vcPackGPSNo]='" + dtb.Rows[j]["vcPackGPSNo"].ToString() + "'   \n");
                    }
                    else
                    {
                        //插入新的品番在库
                        sql.Append(" INSERT INTO [dbo].[TPackZaiKu]   \n");
                        sql.Append("            ([vcPackSpot]   \n");
                        sql.Append("            ,[vcPackNo]   \n");
                        sql.Append("            ,[vcPackGPSNo]   \n");
                        sql.Append("            ,[vcSupplierID]   \n");
                        sql.Append("            ,[iLiLun]   \n");
                        sql.Append("            ,[iAnQuan]   \n");
                        sql.Append("            ,[vcOperatorID]   \n");
                        sql.Append("            ,[dOperatorTime])   \n");
                        sql.Append("  VALUES  \n");
                        sql.Append("   ( \n");
                        sql.Append("   '" + dr1[0]["vcPackSpot"].ToString() + "', \n");
                        sql.Append("   '" + dr1[0]["vcPackNo"].ToString() + "', \n");
                        sql.Append("   '" + dtb.Rows[j]["vcPackGPSNo"].ToString() + "', \n");
                        sql.Append("   '" + dr1[0]["vcSupplierCode"].ToString() + "', \n");
                        sql.Append("   '" + dtb.Rows[j]["iSJNum"].ToString() + "', \n");
                        sql.Append("   '" + SaveZK + "', \n");
                        sql.Append("   '" + strUserId + "', \n");
                        sql.Append("   getdate() \n");
                        sql.Append("   ) \n");

                    }

                }
                #endregion

                #region 更新实绩作业表（入库）
                for (int z = 0; z < dtcope.Rows.Count; z++)
                {
                    string PackNo1 = "";
                    string PackSpot1 = "";
                    if (dtcope.Rows[z]["RECEIVED_QUANTITY"].ToString() != "0")
                    {
                        DataRow[] dr1 = dtBase.Select("vcPackGPSNo='" + dtcope.Rows[z]["PART_NO"] + "'");
                        if (dr1.Length == 0)
                        {
                            PackNo1 = "";
                            PackSpot1 = "";
                        }
                        else
                        {
                            PackNo1 = dr1[0]["vcPackNo"].ToString();
                            PackSpot1 = dr1[0]["vcPackSpot"].ToString();
                        }
                        sql.Append("  delete from TPackWork where  vcOrderNo='" + dtcope.Rows[z]["ORDER_NO"].ToString() + "' and  vcZuoYeQuFen='0' \n");
                        sql.Append("  INSERT INTO [dbo].[TPackWork]    \n");
                        sql.Append("             ([vcZuoYeQuFen]    \n");
                        sql.Append("             ,[vcOrderNo]    \n");
                        sql.Append("             ,[vcPackNo]    \n");
                        sql.Append("             ,[vcPackGPSNo]    \n");
                        sql.Append("             ,[vcSupplierID]    \n");
                        sql.Append("             ,[vcPackSpot]    \n");
                        sql.Append("             ,[iNumber]    \n");
                        sql.Append("             ,[dBuJiTime]    \n");
                        sql.Append("             ,[dZiCaiTime]    \n");
                        sql.Append("             ,[vcYanShouID]    \n");
                        sql.Append("             ,[vcOperatorID]    \n");
                        sql.Append("             ,[dOperatorTime])    \n");
                        sql.Append("       VALUES    \n");
                        sql.Append("    (  \n");
                        sql.Append("    '0',  \n");
                        sql.Append("    '" + dtcope.Rows[z]["ORDER_NO"].ToString() + "',  \n");
                        sql.Append("    '" + PackNo1 + "',  \n");
                        sql.Append("    '" + dtcope.Rows[z]["PART_NO"].ToString() + "',  \n");
                        sql.Append("    '" + dtcope.Rows[z]["SUPPLIER_CODE"].ToString() + "',  \n");
                        sql.Append("  '" + PackSpot1 + "',    \n");
                        sql.Append("    '" + dtcope.Rows[z]["RECEIVED_QUANTITY"].ToString() + "',  \n");
                        sql.Append("     GETDATE() ,  \n");
                        sql.Append("    '" + dtcope.Rows[z]["CREATE_TIME"].ToString() + "',  \n");
                        sql.Append("    '" + dtcope.Rows[z]["CREATE_USER"].ToString() + "',  \n");
                        sql.Append("  '" + strUserId + "', \n");
                        sql.Append("   GETDATE()  \n");
                        sql.Append("     ) \n");

                    }

                }
                #endregion

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

        #region 更新GPS品番
        public DataTable Search(string PackSpot, string PackNo, string PackGPSNo, List<Object> strSupplierCode, string dFromB, string dFromE, string dToB, string dToE)
        {
            try
            {

                if (string.IsNullOrEmpty(dFromB))
                {
                    dFromB = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dFromE))
                {
                    dFromE = "9999-12-31 0:00:00";

                }
                if (string.IsNullOrEmpty(dToB))
                {
                    dToB = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dToE))
                {
                    dToE = "9999-12-31 0:00:00";

                }
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackBase");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplierCode in( ");
                    for (int i = 0; i < strSupplierCode.Count; i++)
                    {
                        if (strSupplierCode.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + strSupplierCode[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + strSupplierCode[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                strSql.AppendLine($"      AND dPackFrom BETWEEN '{dFromB}' and '{dFromE}'");
                strSql.AppendLine($"      AND dPackTo BETWEEN '{dToB}' and '{dToE}'");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取是否是第一次入库

        public DataTable SearchRKFrist()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select  * from TPackZaiKu ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 更新订单状态准备

        public DataTable SearchINFO()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select a.vcOrderNo,b.vcOrderNo as vcIsorNoOrder,b.vcPackNo,b.vcPackGPSNo,  ");
                strSql.AppendLine(" b.vcPackSpot,vcSupplieCode,iDGNum,vcDGTime,iSJNum,vcSJTime,vcUnit,vcCostID,vcCodeID,  ");
                strSql.AppendLine(" dYanshouTime,dEmailTime,vcYanShouID,dUpdateTime,a.dNaRuYuDing,c.vcIsorNoFaZhu  ");
                strSql.AppendLine(" from(  ");
                strSql.AppendLine(" select * from TPack_FaZhu_ShiJi  ");
                strSql.AppendLine(" )a left join  ");
                strSql.AppendLine(" (  ");
                strSql.AppendLine("  select * from TPackRuKuInFo   ");
                strSql.AppendLine(" )b on a.vcOrderNo=b.vcOrderNo  ");
                strSql.AppendLine(" left join  ");
                strSql.AppendLine(" (  ");
                strSql.AppendLine("  select * from TPackOrderFaZhu  ");
                strSql.AppendLine(" )c on a.vcOrderNo=c.vcOrderNo  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取安全在库数据

        public DataTable SearchSaveDT()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from TPackSaveZK ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}