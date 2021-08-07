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
    public class FS0710_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region  供应商
        public DataTable SearchSupplier()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("     select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where isnull(vcSupplierCode,'')<>''  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 纳入统计,返回dt
        public DataTable Search_NR(List<Object> PackSpot, List<Object> strSupplierCode, string dFrom, string dTo)
        {
            try
            {

                if (string.IsNullOrEmpty(dFrom))
                {
                    dFrom = "1900-01-01 00:00:00";

                }

                if (string.IsNullOrEmpty(dTo))
                {
                    dTo = "9999-12-31 00:00:00";

                }

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select a.vcSupplieCode,b.vcSupplierName,a.vcPackGPSNo,b.vcParstName,b.vcFormat     ");
                strSql.AppendLine("  ,a.vcUnit,sum(a.iSJNum) as isjNum,a.vcCostID ,a.vcPackSpot        ");
                strSql.AppendLine("   from      ");
                strSql.AppendLine("  (     ");
                strSql.AppendLine("  select * from TPackRuKuInFo where vcSJTime is not null      ");
                strSql.AppendLine("  and  CONVERT(datetime,vcSJTime,120)  between '" + dFrom + "' and '" + dTo + "'     ");
                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int i = 0; i < PackSpot.Count; i++)
                    {
                        if (PackSpot.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + PackSpot[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + PackSpot[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplieCode in( ");
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

                strSql.AppendLine("   )a left join    ");
                strSql.AppendLine("   (    ");
                strSql.AppendLine("   select vcSupplierCode,vcSupplierName,vcParstName,vcFormat,vcPackNo,vcPackGPSNo from TPackBase    ");
               // strSql.AppendLine("      where getdate() between dPackFrom and dPackTo      ");
                strSql.AppendLine("   )b on a.vcPackNo=b.vcPackNo and a.vcSupplieCode=b.vcSupplierCode     ");
                strSql.AppendLine("   group by  a.vcSupplieCode,b.vcSupplierName,a.vcPackGPSNo,b.vcParstName,b.vcFormat    ");
                strSql.AppendLine("   ,a.vcUnit,a.vcCostID ,a.vcPackSpot      ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion



        #region 水晶报表
        public void Save_Caystal(DataTable dt, string packSpot, string strSupplierCode, string dFrom, string dTo)
        {
            try
            {
                DataTable dt1 = this.SearchSupplier();
                DataRow[] dr = dt1.Select("vcValue='"+ strSupplierCode + "'");
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("   delete from TPackNRTJ_Caystal ;  ");
                //sql.AppendLine("   delete from TpackNRTJ_List ;  ");
                //sql.AppendLine(" INSERT INTO [dbo].[TpackNRTJ_List]     ");
                //sql.AppendLine("  ([vcPackSpot]   ");
                //sql.AppendLine("  ,[vcSupplieCode],vcSupplieName   ");
                //sql.AppendLine("  ,[dfrom]   ");
                //sql.AppendLine("  ,[dto]   ");
                //sql.AppendLine("  ,[dMyTime])   ");
                //sql.AppendLine("   values ");
                //sql.AppendLine("   ( ");
                //sql.AppendLine("  '"+ packSpot + "',  ");
                //sql.AppendLine("  '"+ strSupplierCode + "',  ");
                //sql.AppendLine("  '"+ dr[0]["vcName"].ToString() + "',  ");
                //sql.AppendLine("  '"+ dFrom + "',  ");
                //sql.AppendLine("  '"+ dTo + "',  ");
                //sql.AppendLine("  getdate()  ");
                //sql.AppendLine("    )");
              
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine(" INSERT INTO [dbo].[TPackNRTJ_Caystal]     ");
                    sql.AppendLine("   ([iNum]   ");
                    sql.AppendLine("   ,[vcPackGPSNo],[vcPackSpot]   ");
                    sql.AppendLine("   ,[vcSupplieCode],vcSupplieName   ");
                    sql.AppendLine("   ,[vcParstName]   ");
                    sql.AppendLine("   ,[vcFormat]   ");
                    sql.AppendLine("   ,[vcUnit]   ");
                    sql.AppendLine("   ,[isjNum]   ");
                    sql.AppendLine("   ,[Memo]   ");
                    sql.AppendLine("   ,[vcCostID]   ");
                    sql.AppendLine("   ,[dfrom]   ");
                    sql.AppendLine("   ,[dto]   ");
                    sql.AppendLine("   ,[dMyTime])   ");
                    sql.AppendLine("   VALUES ");
                    sql.AppendLine("   ( ");
                    sql.AppendLine("   '"+(i+1)+"', ");
                    sql.AppendLine("   '"+ dt.Rows[i]["vcPackGPSNo"].ToString() + "', ");
                    sql.AppendLine("  '" + packSpot + "',  ");
                    sql.AppendLine("  '" + strSupplierCode + "',  ");
                    sql.AppendLine("  '" + dr[0]["vcName"].ToString() + "',  ");
                    sql.AppendLine("   '" + dt.Rows[i]["vcParstName"].ToString() + "', ");
                    sql.AppendLine("   '" + dt.Rows[i]["vcFormat"].ToString() + "' ,");
                    sql.AppendLine("   '" + dt.Rows[i]["vcUnit"].ToString() + "', ");
                    sql.AppendLine("   '" + dt.Rows[i]["isjNum"].ToString() + "',");
                    sql.AppendLine("   '" + dt.Rows[i]["Memo"].ToString() + "', ");
                    sql.AppendLine("   '" + dt.Rows[i]["vcCostID"].ToString() + "', ");
                    if (dFrom == "")
                    {
                        sql.AppendLine("  '" + dFrom.Split(' ')[0].ToString() + "',  ");
                    }
                    else {
                        sql.AppendLine(" NULL,  ");
                    }
                    if (dTo == "")
                    {
                        sql.AppendLine("  '" + dTo.Split(' ')[0].ToString() + "',  ");
                    }
                    else
                    {
                        sql.AppendLine(" NULL,  ");
                    }
                    sql.AppendLine("  getdate()  ");
                    sql.AppendLine("   ) ");



                }

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                //if (ex.Message.IndexOf("-->") != -1)
                //{//主动判断抛出的异常
                //    int startIndex = ex.Message.IndexOf("-->");
                //    int endIndex = ex.Message.LastIndexOf("<--");
                //    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                //}
                //else
                    throw ex;
            }
        }
        #endregion


        public DataTable SearchNRCaystal(string strSupplierCode, string packSpot)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select* from TPackNRTJ_Temp where vcPackSpot='"+ packSpot + "'and vcSupplieCode='"+ strSupplierCode + "'  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region 保存
        public void Save_NR(DataTable listInfoData, ref string strErrorPartId, string UserId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine("   delete from TPackNRTJ_Temp ;  ");
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.AppendLine(" INSERT INTO [dbo].[TPackNRTJ_Temp]     ");
                    sql.AppendLine("            (iNum,[vcSupplieCode]     ");
                    sql.AppendLine("            ,[vcSupplieName]     ");
                    sql.AppendLine("            ,[vcPackGPSNo]     ");
                    sql.AppendLine("            ,[vcParstName]     ");
                    sql.AppendLine("            ,[vcFormat]     ");
                    sql.AppendLine("            ,[vcUnit]     ");
                    sql.AppendLine("            ,[isjNum]     ");
                    sql.AppendLine("            ,[Memo]     ");
                    sql.AppendLine("            ,[vcCostID],vcPackSpot,vcOperatorID,dOperatorTime)     ");
                    sql.AppendLine("      VALUES     ");
                    sql.AppendLine("    ( '"+i.ToString()+"', ");
                    sql.AppendLine("    '"+ listInfoData.Rows[i]["vcSupplieCode"].ToString()+ "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcSupplierName"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcPackGPSNo"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcParstName"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcFormat"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcUnit"].ToString() + "',   ");
                    sql.AppendLine("    '" + Decimal.ToInt32(Convert.ToDecimal(listInfoData.Rows[i]["isjNum"].ToString())) + "',   ");
                    sql.AppendLine("    '',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcCostID"].ToString() + "' , ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcPackSpot"].ToString() + "',  ");
                    sql.AppendLine("    '" +UserId+ "',  ");
                    sql.AppendLine("    getdate() ");
                    sql.AppendLine("     ) ");



                }

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion


        #region 保存（订单）
        public void Save_DD(DataTable listInfoData, ref string strErrorPartId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine("   delete from TPackDDTJ_Temp ;  ");
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.AppendLine(" INSERT INTO [dbo].[TPackDDTJ_Temp]    ");
                    sql.AppendLine("            ([vcSupplieCode]    ");
                    sql.AppendLine("            ,[vcOrderNo]    ");
                    sql.AppendLine("            ,[vcPackGPSNo]    ");
                    sql.AppendLine("            ,[vcPackNo]    ");
                    sql.AppendLine("            ,[dNaRuYuDing]    ");
                    sql.AppendLine("            ,[iOrderNumber]    ");
                    sql.AppendLine("            ,[dNaRuShiJi]    ");
                    sql.AppendLine("            ,[iSJNumber])    ");
                    sql.AppendLine("      VALUES     ");
                    sql.AppendLine("    (  ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcSupplieCode"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcOrderNo"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcPackGPSNo"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["vcPackNo"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["dNaRuYuDing"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["iOrderNumber"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["dNaRuShiJi"].ToString() + "',   ");
                    sql.AppendLine("    '" + listInfoData.Rows[i]["iSJNumber"].ToString() + "'   ");
                    sql.AppendLine("     ) ");



                }

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion




        #region 订单统计,返回dt
        public DataTable Search_DD(List<Object> PackSpot, List<Object> strSupplierCode, string dFrom, string dTo)
        {
            try
            {

                if (string.IsNullOrEmpty(dFrom))
                {
                    dFrom = "1900-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dTo))
                {
                    dTo = "9999-12-31 0:00:00";

                }

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select a.vcSupplieCode,a.vcOrderNo,a.vcPackGPSNo,a.vcPackNo,b.dNaRuYuDing,b.vcNaRuBianCi,b.iSJNumber,b.dNaRuShiJi,b.iOrderNumber from    ");
                strSql.AppendLine("  (    ");
                strSql.AppendLine("    select c.vcSupplieCode,c.vcOrderNo,c.vcPackGPSNo,c.vcPackNo from(    ");
                strSql.AppendLine("     select vcSupplieCode,vcOrderNo,vcPackGPSNo,vcPackNo from TPackRuKuInFo    "); 
                strSql.AppendLine("     --已验收时段为判断条件    ");
                strSql.AppendLine("      where  isnull(vcSJTime,'')<>''     ");
                strSql.AppendLine("  and  CONVERT(datetime,dYanshouTime,120) between '" + dFrom + "' and '" + dTo + "'     ");
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND  vcSupplieCode in( ");
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
                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int i = 0; i < PackSpot.Count; i++)
                    {
                        if (PackSpot.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + PackSpot[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + PackSpot[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }

                strSql.AppendLine("     )c left join     ");
                strSql.AppendLine("     (     ");
                strSql.AppendLine("     select * from TPackBase     ");
                strSql.AppendLine("     where GETDATE() between dPackFrom and dPackTo   ");
                strSql.AppendLine("     )d on c.vcPackNo=d.vcPackNo and c.vcSupplieCode=d.vcSupplierCode      ");
                strSql.AppendLine("        ");
                strSql.AppendLine("  )a left join     ");
                strSql.AppendLine("  (     ");
                strSql.AppendLine("  select * from TPack_FaZhu_ShiJi     ");
                strSql.AppendLine("  )b on a.vcOrderNo=b.vcOrderNo     ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion




        #region 
        public DataTable getSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcPackSupplierCode,vcPackSupplierName from TPackSupplier;");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt----公式
        public DataTable Search_NaRu_export()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select * from TPackNRTJ_Temp         \n");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 按检索条件检索,返回dt----公式
        public DataTable Search_DingDan_export()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select * from TPackDDTJ_Temp   order by vcOrderNo asc      \n");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存-公式
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
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
                        sql.Append("  insert into TPrice_GS(vcName,vcGs,vcArea,dBegin,dEnd,vcReason,vcOperatorID,dOperatorTime   \r\n");
                        sql.Append("  )   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcName"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcGs"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcArea"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dBegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + ",  \r\n");
                        sql.Append("'" + strUserId + "',  \r\n");
                        sql.Append("getdate()  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice_GS set    \r\n");
                        sql.Append("  vcName=" + ComFunction.getSqlValue(listInfoData[i]["vcName"], false) + "   \r\n");
                        sql.Append("  ,vcGs=" + ComFunction.getSqlValue(listInfoData[i]["vcGs"], false) + "   \r\n");
                        sql.Append("  ,vcArea=" + ComFunction.getSqlValue(listInfoData[i]["vcArea"], false) + "   \r\n");
                        sql.Append("  ,dBegin=" + ComFunction.getSqlValue(listInfoData[i]["dBegin"], true) + "   \r\n");
                        sql.Append("  ,dEnd=" + ComFunction.getSqlValue(listInfoData[i]["dEnd"], true) + "   \r\n");
                        sql.Append("  ,vcReason=" + ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    }
                }
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorName varchar(50)   \r\n");
                    sql.Append("  set @errorName=''   \r\n");
                    sql.Append("  set @errorName=(   \r\n");
                    sql.Append("  	select a.vcName+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcName from TPrice_GS a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from TPrice_GS   \r\n");
                    sql.Append("  		)b on a.vcName=b.vcName and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		   and    \r\n");
                    sql.Append("  		   (   \r\n");
                    sql.Append("  			   (a.dBegin>=b.dBegin and a.dBegin<=b.dEnd)   \r\n");
                    sql.Append("  			   or   \r\n");
                    sql.Append("  			   (a.dEnd>=b.dBegin and a.dEnd<=b.dEnd)   \r\n");
                    sql.Append("  		   )   \r\n");
                    sql.Append("  		where b.iAutoId is not null   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorName<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorName+'<--')   \r\n");
                    sql.Append("  end    \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorName = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion

       


    }
}
