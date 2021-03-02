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
    public class FS0703_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 
        public DataTable SearchSupplier()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcPackSupplierCode as vcValue,vcPackSupplierName as vcName from TPackSupplier; ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region  品番错误导出数据
        public DataTable SearchException()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("        select vcPart_id as vcValue,vcException as vcName from TPackJSException ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region  品番错误导出数据
        public DataTable SearchExceptionCK()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" declare @icount int;     ");
                strSql.AppendLine(" select @icount=count(*) from TPackJSException     ");
                strSql.AppendLine(" if @icount=0     ");
                strSql.AppendLine("   begin     ");
                strSql.AppendLine("   select '计算成功' as vcValue,''as vcName ;     ");
                strSql.AppendLine("   end     ");
                strSql.AppendLine(" else     ");
                strSql.AppendLine("   begin     ");
                strSql.AppendLine("    select vcPart_id as vcValue,vcException as vcName from TPackJSException     ");
                strSql.AppendLine("   end     ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 计算
        public DataTable Calculation(string PackSpot, string PackFrom, List<Object> strSupplierCode)
        {
            try
            {
                DateTime dtn1 = DateTime.ParseExact(PackFrom.Substring(0, 7), "yyyy-MM", System.Globalization.CultureInfo.CurrentCulture);
                string strN = dtn1.AddMonths(1).ToString("yyyyMM");
                string strN_1 = dtn1.AddMonths(2).ToString("yyyyMM");
                string strN_2 = dtn1.AddMonths(3).ToString("yyyyMM");


                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" delete from TPackJSException   ;     \n");
                strSql.AppendLine(" select        \n");
                strSql.AppendLine(" T_1.vcYearMonth       \n");
                strSql.AppendLine("  ,T_1.vcPart_id         \n");
                strSql.AppendLine(" ,T_2.vcPackSpot,T_2.vcSupplierCode,      \n");
                strSql.AppendLine("  T_2.vcSupplierPlant,T_2.vcSupplierName,      \n");
                strSql.AppendLine(" '' as vcSupplierPack ---供应商包装       \n");
                strSql.AppendLine(" ,T_2.vcPackNo      \n");
                strSql.AppendLine(" ,T_2.vcCycle,T_2.iRelease,T_1.iHySOQN,T_1.iHySOQN1,T_1.iHySOQN2,      \n");
                strSql.AppendLine("  T_1.iD1*T_1.iQuantityPercontainer as iD1,      \n");
                strSql.AppendLine("  T_1.iD2*T_1.iQuantityPercontainer as iD2,      \n");
                strSql.AppendLine("  T_1.iD3*T_1.iQuantityPercontainer as iD3,      \n");
                strSql.AppendLine("  T_1.iD4*T_1.iQuantityPercontainer as iD4,      \n");
                strSql.AppendLine("  T_1.iD5*T_1.iQuantityPercontainer as iD5,      \n");
                strSql.AppendLine("  T_1.iD6*T_1.iQuantityPercontainer as iD6,      \n");
                strSql.AppendLine("  T_1.iD7*T_1.iQuantityPercontainer as iD7,      \n");
                strSql.AppendLine("  T_1.iD8*T_1.iQuantityPercontainer as iD8,      \n");
                strSql.AppendLine("  T_1.iD9*T_1.iQuantityPercontainer as iD9,           \n");
                strSql.AppendLine("  T_1.iD10*T_1.iQuantityPercontainer as iD10,      \n");
                strSql.AppendLine("  T_1.iD11*T_1.iQuantityPercontainer as iD11,      \n");
                strSql.AppendLine("  T_1.iD12*T_1.iQuantityPercontainer as iD12,      \n");
                strSql.AppendLine("  T_1.iD13*T_1.iQuantityPercontainer as iD13,      \n");
                strSql.AppendLine("  T_1.iD14*T_1.iQuantityPercontainer as iD14,      \n");
                strSql.AppendLine("  T_1.iD15*T_1.iQuantityPercontainer as iD15,      \n");
                strSql.AppendLine("  T_1.iD16*T_1.iQuantityPercontainer as iD16,      \n");
                strSql.AppendLine("  T_1.iD17*T_1.iQuantityPercontainer as iD17,      \n");
                strSql.AppendLine("  T_1.iD18*T_1.iQuantityPercontainer as iD18,      \n");
                strSql.AppendLine("  T_1.iD19*T_1.iQuantityPercontainer as iD19,      \n");
                strSql.AppendLine("  T_1.iD20*T_1.iQuantityPercontainer as iD20,      \n");
                strSql.AppendLine("  T_1.iD21*T_1.iQuantityPercontainer as iD21,      \n");
                strSql.AppendLine("  T_1.iD22*T_1.iQuantityPercontainer as iD22,       \n");
                strSql.AppendLine("  T_1.iD23*T_1.iQuantityPercontainer as iD23,       \n");
                strSql.AppendLine("  T_1.iD24*T_1.iQuantityPercontainer as iD24,       \n");
                strSql.AppendLine("  T_1.iD25*T_1.iQuantityPercontainer as iD25,       \n");
                strSql.AppendLine("  T_1.iD26*T_1.iQuantityPercontainer as iD26,       \n");
                strSql.AppendLine("  T_1.iD27*T_1.iQuantityPercontainer as iD27,       \n");
                strSql.AppendLine("  T_1.iD28*T_1.iQuantityPercontainer as iD28,       \n");
                strSql.AppendLine("  T_1.iD29*T_1.iQuantityPercontainer as iD29,       \n");
                strSql.AppendLine("  T_1.iD30*T_1.iQuantityPercontainer as iD30,       \n");
                strSql.AppendLine("  T_1.iD31*T_1.iQuantityPercontainer as iD31,       \n");
                strSql.AppendLine("  GETDATE() as dZCTime --作成时间        \n");
                strSql.AppendLine(" from         \n");
                strSql.AppendLine("  (       \n");
                strSql.AppendLine("  select a.vcYearMonth,a.vcPart_id,a.iHySOQN,a.iHySOQN1,a.iHySOQN2      \n");
                strSql.AppendLine("  ,b.vcDXYM as vcDXYM,c.vcDXYM as vcDXYM1,d.vcDXYM as vcDXYM2,b.iQuantityPercontainer,     \n");
                strSql.AppendLine("  b.iD1 ,b.iD2,b.iD3,b.iD4,b.iD5,b.iD6,b.iD7,b.iD8,b.iD9,b.iD10,b.iD11,b.iD12,b.iD13,b.iD14,b.iD15,b.iD16,     \n");
                strSql.AppendLine("  b.iD17,b.iD18,b.iD19,b.iD20,b.iD21,b.iD22,b.iD23,b.iD24,b.iD25,b.iD26,b.iD27,b.iD28,b.iD29,b.iD30,b.iD31     \n");
                strSql.AppendLine("  from       \n");
                strSql.AppendLine("  (         \n");
                strSql.AppendLine("   select *from TSoq where vcYearMonth='" + strN + "'      \n");
                strSql.AppendLine("  )a        \n");
                strSql.AppendLine("  inner join      \n");
                strSql.AppendLine("  (        \n");
                strSql.AppendLine("  select * from TSoqReply where vcDXYM='" + strN + "'      \n");
                strSql.AppendLine("  )b on a.vcPart_id=b.vcPart_id      \n");
                strSql.AppendLine("  inner join      \n");
                strSql.AppendLine("  (        \n");
                strSql.AppendLine("  select * from TSoqReply where vcDXYM='" + strN_1 + "'     \n");
                strSql.AppendLine("  )c on a.vcPart_id=c.vcPart_id       \n");
                strSql.AppendLine("  inner join      \n");
                strSql.AppendLine("   (         \n");
                strSql.AppendLine("   select * from TSoqReply where vcDXYM='" + strN_2 + "'      \n");
                strSql.AppendLine("   )d on a.vcPart_id=d.vcPart_id     \n");
                strSql.AppendLine("   inner join     \n");
                strSql.AppendLine("   (     \n");
                strSql.AppendLine("     select * from TPackageMaster where vcBZPlant='" + PackSpot + "'     \n");
                strSql.AppendLine("   )e on a.vcPart_id=e.vcPart_id     \n");
                strSql.AppendLine("   )T_1      \n");
                strSql.AppendLine("   left join     \n");
                strSql.AppendLine("   (         \n");
                strSql.AppendLine("      select vcPartsNo,ss.vcPackSpot,ss.vcSupplierCode,ss.vcSupplierPlant,ss.vcSupplierName,     \n");
                strSql.AppendLine("  	 ss.vcPackNo,ss.vcCycle,ss.iRelease     \n");
                strSql.AppendLine("  	  from      \n");
                strSql.AppendLine("  	 (     \n");
                strSql.AppendLine("       select * from TPackItem       \n");
                strSql.AppendLine("  	  )s inner join     \n");
                strSql.AppendLine("  	  (     \n");
                strSql.AppendLine("  	    select * from TPackBase     \n");
                strSql.AppendLine("  	  )ss on s.vcPackNo=ss.vcPackNo     \n");
                strSql.AppendLine("       \n");
                strSql.AppendLine("   )T_2 on T_1.vcPart_id=T_2.vcPartsNo         \n");
                strSql.AppendLine("  where vcSupplierCode in (     \n");
                for (int i = 0; i < strSupplierCode.Count; i++)
                {
                    if (strSupplierCode.Count - i == 1)
                    {
                        strSql.AppendLine("   '" + strSupplierCode[i] + "'   \n");
                    }
                    else
                        strSql.AppendLine("  '" + strSupplierCode[i] + "' ,   \n");
                }
                strSql.AppendLine("  )    \n");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 插入品番错误信息
        public void InsertCheck(string vcpart_id, string strUserId, string eX)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("     INSERT INTO [dbo].[TPackJSException]           \n");
                sql.AppendLine("                ([vcPart_id]           \n");
                sql.AppendLine("                ,[vcException]           \n");
                sql.AppendLine("                ,[vcOperatorID]           \n");
                sql.AppendLine("                ,[dOperatorTime])           \n");
                sql.AppendLine("          VALUES           \n");
                sql.AppendLine("            (    \n");
                sql.AppendLine("             '"+ vcpart_id + "',   \n");
                sql.AppendLine("             '"+ eX + "',   \n");
                sql.AppendLine("             '"+ strUserId + "',   \n");
                sql.AppendLine("             getdate())   \n");
                excute.ExcuteSqlWithSelectToDT(sql.ToString());
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
        public DataTable Search(string PackSpot, string PackFrom, List<Object> strSupplierCode)
        {
            try
            {
                DateTime dtn1 = DateTime.ParseExact(PackFrom.Substring(0, 7), "yyyy-MM", System.Globalization.CultureInfo.CurrentCulture);
                string strN = dtn1.AddMonths(1).ToString("yyyyMM");
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select         \n");
                strSql.Append("    vcYearMonth, vcPackNo,vcpart_id,vcPackSpot,vcSupplierCode,vcSupplierWork,vcSupplierName,vcSupplierPack ,vcCycle,iRelease         \n");
                strSql.Append("    ,sum(iDayNNum)as iDayNNum,         \n");
                strSql.Append("    sum(iDayN1Num)as iDayNNum1,         \n"); 
                strSql.Append("    sum(iDayN2Num)as iDayN2Num,         \n");
                strSql.Append("    sum(iDay1)as iDay1,sum(iDay2)as iDay2,sum(iDay3)as iDay3,sum(iDay4)as iDay4,sum(iDay5)as iDay5,sum(iDay6)as iDay6,sum(iDay7)as iDay7         \n");
                strSql.Append("   ,sum(iDay8)as iDay8,sum(iDay9)as iDay9,sum(iDay10)as iDay10,         \n");
                strSql.Append("    sum(iDay11)as iDay11,sum(iDay12)as iDay12,sum(iDay13)as iDay13,sum(iDay14)as iDay14,sum(iDay15)as iDay15,sum(iDay16)as iDay16,sum(iDay17)as iDay17         \n");
                strSql.Append("   ,sum(iDay18)as iDay18,sum(iDay19)as iDay19,sum(iDay20)as iDay20,         \n");
                strSql.Append("    sum(iDay21)as iDay21,sum(iDay22)as iDay22,sum(iDay23)as iDay23,sum(iDay24)as iDay24,sum(iDay25)as iDay25,sum(iDay26)as iDay26,sum(iDay27)as iDay27         \n");
                strSql.Append("   ,sum(iDay28)as iDay28,sum(iDay29)as iDay29,sum(iDay30)as iDay30,sum(iDay31)as iDay31,dZYTime,vcOperatorID,dOperatorTime         \n");
                strSql.Append("      from TPackNSCalculation         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if (PackSpot != "")
                    strSql.Append("   and  vcPackSpot='" + PackSpot + "'       \n");
                if (strN != "")
                    strSql.Append("   and    vcYearMonth ='" + strN + "'         \n");
                if (strSupplierCode.Count != 0)
                {
                    strSql.Append("   and   vcSupplierCode in (          \n");
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

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 存入内示计算
        public void Save_GS(DataTable listInfoData, string strUserId, ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.AppendLine("  delete from TPackNSCalculation where vcpart_id='" + listInfoData.Rows[i]["vcpart_id"].ToString() + "' and vcYearMonth='" + listInfoData.Rows[i]["vcYearMonth"].ToString() + "' and vcPackNo='" + listInfoData.Rows[i]["vcPackNo"].ToString() + "'  \n");
                }
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.AppendLine("  INSERT INTO [dbo].[TPackNSCalculation]    \n");
                    sql.AppendLine("             (   \n");
                    sql.AppendLine("              [vcYearMonth]    \n");
                    sql.AppendLine("             ,[vcPackNo]    \n");
                    sql.AppendLine("             ,[vcpart_id]    \n");
                    sql.AppendLine("             ,[vcPackSpot]    \n");
                    sql.AppendLine("             ,[vcSupplierCode]    \n");
                    sql.AppendLine("             ,[vcSupplierWork]    \n");
                    sql.AppendLine("             ,[vcSupplierName]    \n");
                    sql.AppendLine("             ,[vcSupplierPack]    \n");
                    sql.AppendLine("             ,[vcCycle]    \n");
                    sql.AppendLine("             ,[iRelease]    \n");
                    sql.AppendLine("             ,[iDayNNum]    \n");
                    sql.AppendLine("             ,[iDayN1Num]    \n");
                    sql.AppendLine("             ,[iDayN2Num]    \n");
                    sql.AppendLine("             ,[iDay1]    \n");
                    sql.AppendLine("             ,[iDay2]    \n");
                    sql.AppendLine("             ,[iDay3]    \n");
                    sql.AppendLine("             ,[iDay4]    \n");
                    sql.AppendLine("             ,[iDay5]    \n");
                    sql.AppendLine("             ,[iDay6]    \n");
                    sql.AppendLine("             ,[iDay7]    \n");
                    sql.AppendLine("             ,[iDay8]    \n");
                    sql.AppendLine("             ,[iDay9]    \n");
                    sql.AppendLine("             ,[iDay10]    \n");
                    sql.AppendLine("             ,[iDay11]    \n");
                    sql.AppendLine("           ,[iDay12]    \n");
                    sql.AppendLine("           ,[iDay13]    \n");
                    sql.AppendLine("           ,[iDay14]    \n");
                    sql.AppendLine("           ,[iDay15]    \n");
                    sql.AppendLine("           ,[iDay16]    \n");
                    sql.AppendLine("           ,[iDay17]    \n");
                    sql.AppendLine("           ,[iDay18]    \n");
                    sql.AppendLine("           ,[iDay19]    \n");
                    sql.AppendLine("           ,[iDay20]    \n");
                    sql.AppendLine("           ,[iDay21]    \n");
                    sql.AppendLine("           ,[iDay22]    \n");
                    sql.AppendLine("           ,[iDay23]    \n");
                    sql.AppendLine("           ,[iDay24]    \n");
                    sql.AppendLine("           ,[iDay25]    \n");
                    sql.AppendLine("           ,[iDay26]    \n");
                    sql.AppendLine("           ,[iDay27]    \n");
                    sql.AppendLine("           ,[iDay28]    \n");
                    sql.AppendLine("           ,[iDay29]    \n");
                    sql.AppendLine("           ,[iDay30]    \n");
                    sql.AppendLine("           ,[iDay31]    \n");
                    sql.AppendLine("           ,[dZYTime]    \n");
                    sql.AppendLine("           ,[vcOperatorID]    \n");
                    sql.AppendLine("           ,[dOperatorTime])    \n");
                    sql.AppendLine("     VALUES    \n");
                    sql.AppendLine("    (  \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcYearMonth"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackNo"].ToString() + "',    \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPart_id"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackSpot"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierCode"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierPlant"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierName"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierPack"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcCycle"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iRelease"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iHySOQN"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iHySOQN1"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iHySOQN2"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD1"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD2"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD3"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD4"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD5"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD6"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD7"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD8"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD9"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD10"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD11"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD12"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD13"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD14"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD15"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD16"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD17"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD18"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD19"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD20"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD21"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD22"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD23"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD24"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD25"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD26"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD27"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD28"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD29"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD30"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iD31"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["dZCTime"].ToString() + "',   \n");
                    sql.AppendLine("   '" + strUserId + "',   \n");
                    sql.AppendLine("   getdate()  \n");
                    sql.AppendLine("    )  \n");


                }

                excute.ExcuteSqlWithStringOper(sql.ToString());

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

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       SELECT * FROM TCode where vcCodeId='C023'       \n");
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
