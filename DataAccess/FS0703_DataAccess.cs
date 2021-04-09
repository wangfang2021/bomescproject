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
                strSql.AppendLine("      select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where vcSupplierCode is not null ");

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
        public DataTable Calculation(List<Object> PackSpot, string PackFrom, List<Object> strSupplierCode)
        {
            try
            {
                DateTime dtn1 = DateTime.Parse(PackFrom.Substring(0, 7));
                // DateTime.ParseExact(PackFrom.Substring(0, 7), "yyyy-MM", System.Globalization.CultureInfo.CurrentCulture);
                string strN = dtn1.ToString("yyyyMM");
                string strN_1 = dtn1.AddMonths(1).ToString("yyyyMM");
                string strN_2 = dtn1.AddMonths(2).ToString("yyyyMM");
                string strN_CL = dtn1.AddMonths(-1).ToString("yyyyMM");

                string NowDF = dtn1.AddDays(1 - dtn1.Day).Date.ToString("yyyy-MM-dd HH:mm:ss");

                string NowDE = dtn1.AddDays(1 - dtn1.Day).Date.AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");



                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" delete from TPackJSException   ;      ");
                strSql.AppendLine(" delete from TPackNSCalculation   ;      ");
                strSql.AppendLine(" select         ");
                strSql.AppendLine("  T_1.vcYearMonth,   ");
                strSql.AppendLine("  T_1.vcPart_id,   ");
                strSql.AppendLine("  T_2.vcPackSpot,   ");
                strSql.AppendLine("  T_2.vcSupplierCode,   ");
                strSql.AppendLine("  T_2.vcSupplierPlant,   ");
                strSql.AppendLine("  T_2.vcSupplierName,   ");
                strSql.AppendLine("  T_2.vcPackGPSNo,   ");
                strSql.AppendLine("  T_2.vcPackNo,   ");
                strSql.AppendLine("  T_2.dUsedFrom,   ");
                strSql.AppendLine("  T_2.dUsedTo,   ");
                strSql.AppendLine("  T_2.vcCycle,   ");
                strSql.AppendLine("  T_2.iRelease,   ");
                strSql.AppendLine("  T_1.iHySOQN,   ");
                strSql.AppendLine("  T_1.iHySOQN1,   ");
                strSql.AppendLine("  T_1.iHySOQN2,   ");
                strSql.AppendLine("  case when 1>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 1<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD1 else '0' end as iD1,   ");
                strSql.AppendLine("  case when 2>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 2<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD2 else '0' end as iD2,   ");
                strSql.AppendLine("  case when 3>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 3<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD3 else '0' end as iD3,   ");
                strSql.AppendLine("  case when 4>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 4<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD4 else '0' end as iD4,   ");
                strSql.AppendLine("  case when 5>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 5<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD5 else '0' end as iD5,   ");
                strSql.AppendLine("  case when 6>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 6<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD6 else '0' end as iD6,   ");
                strSql.AppendLine("  case when 7>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 7<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD7 else '0' end as iD7,   ");
                strSql.AppendLine("  case when 8>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 8<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD8 else '0' end as iD8,   ");
                strSql.AppendLine("  case when 9>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 9<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD9 else '0' end as iD9,   ");
                strSql.AppendLine("  case when 10>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 10<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD10 else '0' end as iD10,     ");
                strSql.AppendLine("  case when 11>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 11<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD11 else '0' end as iD11,     ");
                strSql.AppendLine("  case when 12>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 12<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD12 else '0' end as iD12,     ");
                strSql.AppendLine("  case when 13>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 13<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD13 else '0' end as iD13,     ");
                strSql.AppendLine("  case when 14>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 14<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD14 else '0' end as iD14,     ");
                strSql.AppendLine("  case when 15>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 15<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD15 else '0' end as iD15,     ");
                strSql.AppendLine("  case when 16>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 16<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD16 else '0' end as iD16,     ");
                strSql.AppendLine("  case when 17>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 17<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD17 else '0' end as iD17,     ");
                strSql.AppendLine("  case when 18>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 18<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD18 else '0' end as iD18,     ");
                strSql.AppendLine("  case when 19>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 19<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD19 else '0' end as iD19,     ");
                strSql.AppendLine("  case when 20>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 20<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD20 else '0' end as iD20,     ");
                strSql.AppendLine("  case when 21>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 21<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD21 else '0' end as iD21,     ");
                strSql.AppendLine("  case when 22>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 22<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD22 else '0' end as iD22,     ");
                strSql.AppendLine("  case when 23>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 23<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD23 else '0' end as iD23,      ");
                strSql.AppendLine("  case when 24>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 24<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD24 else '0' end as iD24,      ");
                strSql.AppendLine("  case when 25>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 25<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD25 else '0' end as iD25,      ");
                strSql.AppendLine("  case when 26>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 26<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD26 else '0' end as iD26,      ");
                strSql.AppendLine("  case when 27>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 27<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD27 else '0' end as iD27,      ");
                strSql.AppendLine("  case when 28>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 28<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD28 else '0' end as iD28,      ");
                strSql.AppendLine("  case when 29>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 29<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD29 else '0' end as iD29,      ");
                strSql.AppendLine("  case when 30>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 30<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD30 else '0' end as iD30,      ");
                strSql.AppendLine("  case when 31>= CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dFrom1,23),9,2))and 31<=CONVERT(int, SUBSTRING(CONVERT(varchar(100),T_2.dTo1,23),9,2)) then T_1.iD31 else '0' end as iD31,     ");
               
                strSql.AppendLine("  GETDATE() as dZCTime --作成时间         ");
                strSql.AppendLine(" from          ");
                strSql.AppendLine("  (        ");
                strSql.AppendLine("  select a.vcYearMonth,a.vcPart_id,       ");
                strSql.AppendLine("    case when b.vcDXYM is null then '0'else  a.iHySOQN end as iHySOQN,         ");
                strSql.AppendLine("    case when c.vcDXYM is null then '0'else  a.iHySOQN1 end as iHySOQN1,         ");
                strSql.AppendLine("    case when d.vcDXYM is null then '0'else  a.iHySOQN2 end as iHySOQN2         ");
                strSql.AppendLine("  ,b.vcDXYM as vcDXYM,c.vcDXYM as vcDXYM1,d.vcDXYM as vcDXYM2,b.iQuantityPercontainer,      ");
                strSql.AppendLine("  b.iD1 ,b.iD2,b.iD3,b.iD4,b.iD5,b.iD6,b.iD7,b.iD8,b.iD9,b.iD10,b.iD11,b.iD12,b.iD13,b.iD14,b.iD15,b.iD16,      ");
                strSql.AppendLine("  b.iD17,b.iD18,b.iD19,b.iD20,b.iD21,b.iD22,b.iD23,b.iD24,b.iD25,b.iD26,b.iD27,b.iD28,b.iD29,b.iD30,b.iD31      ");
                strSql.AppendLine("  from        ");
                strSql.AppendLine("  (          ");
                strSql.AppendLine("   select *from TSoq where vcYearMonth='" + strN + "'       ");
                strSql.AppendLine("  )a         ");
                strSql.AppendLine("  left join       ");
                strSql.AppendLine("  (         ");
                strSql.AppendLine("  select * from TSoqReply where vcDXYM='" + strN + "' and vcCLYM='"+ strN_CL + "'      ");
                strSql.AppendLine("  )b on a.vcPart_id=b.vcPart_id       ");
                strSql.AppendLine("  left join       ");
                strSql.AppendLine("  (         ");
                strSql.AppendLine("  select * from TSoqReply where vcDXYM='" + strN_1 + "'  and vcCLYM='" + strN_CL + "'      ");
                strSql.AppendLine("  )c on a.vcPart_id=c.vcPart_id        ");
                strSql.AppendLine("  left join       ");
                strSql.AppendLine("   (          ");
                strSql.AppendLine("   select * from TSoqReply where vcDXYM='" + strN_2 + "' and vcCLYM='" + strN_CL + "'        ");
                strSql.AppendLine("   )d on a.vcPart_id=d.vcPart_id      ");
                strSql.AppendLine("   left join      ");
                strSql.AppendLine("   (      ");
                strSql.AppendLine("     select * from TPackageMaster where  1=1   ");
                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"   and   vcBZPlant in (   ");
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
                strSql.AppendLine("   and GETDATE() between dTimeFrom and dTimeTo      ");
                strSql.AppendLine("   )e on a.vcPart_id=e.vcPart_id      ");
                strSql.AppendLine("   )T_1       ");
                strSql.AppendLine("   left join      ");
                strSql.AppendLine("   (          ");
                strSql.AppendLine("      select vcPartsNo,ss.vcPackSpot,ss.vcSupplierCode,ss.vcSupplierPlant,ss.vcSupplierName,      ");
                strSql.AppendLine("  	 ss.vcPackNo,ss.vcCycle,ss.iRelease,dUsedFrom,dUsedTo,s.vcPackGPSNo,s.dFrom1,s.dTo1       ");
                strSql.AppendLine("  	  from       ");
                strSql.AppendLine("  	 (      ");
                strSql.AppendLine("       select         ");
                strSql.AppendLine("      vcPartsNo,vcPackNo,        ");
                strSql.AppendLine("      case when dFrom>'"+ NowDF + "' then dFrom else '"+ NowDF + "'end as dFrom1,        ");
                strSql.AppendLine("      case when dTo>'"+ NowDE + "' then  '"+ NowDE + "' else dTo  end as dTo1,        ");
                strSql.AppendLine("      vcPackGPSNo,vcPackSpot,vcShouhuofangID,vcCar,dUsedFrom,dUsedTo,vcDistinguish,iBiYao,varChangedItem        ");
                strSql.AppendLine("      from TPackItem    where dFrom<='"+ NowDE + "'and dTo>='"+ NowDF + "'         ");
        
                strSql.AppendLine("  	  )s LEFT join      ");
                strSql.AppendLine("  	  (      ");
                strSql.AppendLine("  	    select * from TPackBase      ");
                strSql.AppendLine("  where vcSupplierCode in (      ");
                for (int i = 0; i < strSupplierCode.Count; i++)
                {
                    if (strSupplierCode.Count - i == 1)
                    {
                        strSql.AppendLine("   '" + strSupplierCode[i] + "'    ");
                    }
                    else
                        strSql.AppendLine("  '" + strSupplierCode[i] + "' ,    ");
                }
                strSql.AppendLine("  )   and GETDATE() between  dPackFrom and dPackTo   ");
                strSql.AppendLine("  	  )ss on s.vcPackNo=ss.vcPackNo      ");
                strSql.AppendLine("        ");
                strSql.AppendLine("   )T_2 on T_1.vcPart_id=T_2.vcPartsNo          ");



                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        #endregion


        #region 插入品番错误信息
        public void InsertCheck(DataTable drImport, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < drImport.Rows.Count; i++)
                {
                    sql.AppendLine("     INSERT INTO [TPackJSException]           \n");
                    sql.AppendLine("                ([vcPart_id]           \n");
                    sql.AppendLine("                ,[vcException]           \n");
                    sql.AppendLine("                ,[vcOperatorID]           \n");
                    sql.AppendLine("                ,[dOperatorTime])           \n");
                    sql.AppendLine("          VALUES           \n");
                    sql.AppendLine("            (    \n");
                    sql.AppendLine("             '" + drImport.Rows[i]["vcPart_id"].ToString() + "',   \n");
                    sql.AppendLine("             '" + drImport.Rows[i]["vcException"].ToString() + "',   \n");
                    sql.AppendLine("             '" + strUserId + "',   \n");
                    sql.AppendLine("             getdate())   \n");
                }

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region 供应商
        public DataTable getSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("       select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where vcSupplierCode is not null");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt----公式
        public DataTable SearchCheck()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select a.*,b.dUsedFrom,b.dUsedTo from (        \n");
                strSql.Append("     select * from TPackNSCalculation        \n");
                strSql.Append("     )a left join        \n");
                strSql.Append("     (        \n");
                strSql.Append("     select * from TPackItem        \n");
                strSql.Append("     )b on a.vcpart_id=b.vcPartsNo        \n");
                strSql.Append("             \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion








        #region 按检索条件检索,返回dt----公式
        public DataTable Search(List<Object> PackSpot, string PackFrom, List<Object> strSupplierCode)
        {
            try
            {
                DateTime dtn1 = DateTime.ParseExact(PackFrom.Substring(0, 7), "yyyy-MM", System.Globalization.CultureInfo.CurrentCulture);
                string strN = dtn1.AddMonths(0).ToString("yyyyMM");
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select         \n");
                strSql.Append("    vcYearMonth, vcPackNo,vcPackSpot,vcPackGPSNo,vcSupplierCode,vcSupplierWork,vcSupplierName,vcSupplierPack ,vcCycle,iRelease         \n");
                strSql.Append("    ,sum(iDayNNum)as iDayNNum,         \n");
                strSql.Append("    sum(iDayN1Num)as iDayN1Num,         \n");
                strSql.Append("    sum(iDayN2Num)as iDayN2Num,         \n");
                strSql.Append("    sum(iDay1)as iDay1,sum(iDay2)as iDay2,sum(iDay3)as iDay3,sum(iDay4)as iDay4,sum(iDay5)as iDay5,sum(iDay6)as iDay6,sum(iDay7)as iDay7         \n");
                strSql.Append("   ,sum(iDay8)as iDay8,sum(iDay9)as iDay9,sum(iDay10)as iDay10,         \n");
                strSql.Append("    sum(iDay11)as iDay11,sum(iDay12)as iDay12,sum(iDay13)as iDay13,sum(iDay14)as iDay14,sum(iDay15)as iDay15,sum(iDay16)as iDay16,sum(iDay17)as iDay17         \n");
                strSql.Append("   ,sum(iDay18)as iDay18,sum(iDay19)as iDay19,sum(iDay20)as iDay20,         \n");
                strSql.Append("    sum(iDay21)as iDay21,sum(iDay22)as iDay22,sum(iDay23)as iDay23,sum(iDay24)as iDay24,sum(iDay25)as iDay25,sum(iDay26)as iDay26,sum(iDay27)as iDay27         \n");
                strSql.Append("   ,sum(iDay28)as iDay28,sum(iDay29)as iDay29,sum(iDay30)as iDay30,sum(iDay31)as iDay31,dZYTime         \n");
                strSql.Append("      from TPackNSCalculation         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1        \n");
                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"   and   vcPackSpot in (   ");
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
                    strSql.Append("     group by vcPackNo, vcYearMonth, vcPackNo,vcPackGPSNo,vcPackSpot,vcSupplierCode,vcSupplierWork,vcSupplierName,vcSupplierPack ,vcCycle,iRelease,dZYTime        \n");
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
                    sql.AppendLine("  INSERT INTO [dbo].[TPackNSCalculation]    \n");
                    sql.AppendLine("             (   \n");
                    sql.AppendLine("              [vcYearMonth]    \n");
                    sql.AppendLine("             ,[vcPackNo] ,vcPackGPSNo   \n");
                    sql.AppendLine("             ,[vcpart_id]    \n");
                    sql.AppendLine("             ,[vcPackSpot]    \n");
                    sql.AppendLine("             ,[vcSupplierCode]    \n");
                    sql.AppendLine("             ,[vcSupplierWork]    \n");
                    sql.AppendLine("             ,[vcSupplierName]    \n");
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
                    sql.AppendLine("           ,[vcIsorNoSend]    \n");
                    sql.AppendLine("           ,[vcIsorNoPrint]    \n");
                    sql.AppendLine("           ,[vcOperatorID]    \n");
                    sql.AppendLine("           ,[dOperatorTime])    \n");
                    sql.AppendLine("     VALUES    \n");
                    sql.AppendLine("    (  \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcYearMonth"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackNo"].ToString() + "',    \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackGPSNo"].ToString() + "',    \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPart_id"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackSpot"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierCode"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierPlant"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierName"].ToString() + "',   \n");
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
                    sql.AppendLine("   '0', \n");
                    sql.AppendLine("   '0', \n");
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

        #region 发送
        public void Save(DataTable listInfoData, string userId, ref string strErrorPartId, string PackFrom, List<Object> SupplierCodeList, List<Object> PackSpot)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                string SupplierCode = "";

                string dt1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (SupplierCodeList.Count > 0)
                {
                    for (int j = 0; j < SupplierCodeList.Count; j++)
                    {
                        if (j != 0)
                        {
                            SupplierCode = SupplierCode+ "," + SupplierCodeList[j].ToString();
                        }
                        else
                        {
                            SupplierCode = SupplierCode+ SupplierCodeList[j].ToString();

                        }

                    }
                }

                sql.AppendLine("     delete  from  TPackNSCalculation         \n");



                sql.AppendLine("     INSERT INTO [dbo].[TPackSearch]       \n");
                sql.AppendLine("                ([vcSupplier]       \n");
                sql.AppendLine("                ,[vcYearMonth]       \n");
                sql.AppendLine("                ,[vcNSDiff]       \n");
                sql.AppendLine("                ,[vcNSQJ]       \n");
                sql.AppendLine("                ,[vcNSState]       \n");
                sql.AppendLine("                ,[dFaBuTime]       \n");
                sql.AppendLine("                ,[dFirstDownload]       \n");
                sql.AppendLine("                ,[vcOperatorID]       \n");
                sql.AppendLine("                ,[dOperatorTime])       \n");
                sql.AppendLine("          VALUES       \n");
                sql.AppendLine("         (   \n");
                sql.AppendLine("         '"+ SupplierCode + "',   \n");
                sql.AppendLine("         '"+ PackFrom + "',   \n");
                sql.AppendLine("         '月度内示',   \n");
                sql.AppendLine("         NULL,   \n");
                sql.AppendLine("         '0',   \n");
                sql.AppendLine("         '"+ dt1 + "',   \n");
                sql.AppendLine("         NULL,   \n");
                sql.AppendLine("   '" + userId + "',   \n");
                sql.AppendLine("   getdate()  \n");
                sql.AppendLine("         )   \n");

                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.AppendLine("  INSERT INTO [dbo].[TPackNSCalculationCV]    \n");
                    sql.AppendLine("             (   \n");
                    sql.AppendLine("              [vcYearMonth]    \n");
                    sql.AppendLine("             ,[vcPackNo] ,vcPackGPSNo   \n");
                    //sql.AppendLine("             ,[vcpart_id]    \n");
                    sql.AppendLine("             ,[vcPackSpot]    \n");
                    sql.AppendLine("             ,[vcSupplierCode]    \n");
                    sql.AppendLine("             ,[vcSupplierWork]    \n");
                    sql.AppendLine("             ,[vcSupplierName]    \n");
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
                    sql.AppendLine("           ,[vcIsorNoSend]    \n");
                    sql.AppendLine("           ,[vcIsorNoPrint]    \n");

                    sql.AppendLine("           ,[vcNSDiff]    \n");
                    sql.AppendLine("           ,[dSendTime]    \n");

                    sql.AppendLine("           ,[vcOperatorID]    \n");
                    sql.AppendLine("           ,[dOperatorTime])    \n");
                    sql.AppendLine("     VALUES    \n");
                    sql.AppendLine("    (  \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcYearMonth"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackNo"].ToString() + "',    \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackGPSNo"].ToString() + "',    \n");
                    //sql.AppendLine("   '" + listInfoData.Rows[i]["vcPart_id"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcPackSpot"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierCode"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierWork"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcSupplierName"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["vcCycle"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iRelease"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDayNNum"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDayN1Num"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDayN2Num"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay1"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay2"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay3"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay4"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay5"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay6"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay7"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay8"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay9"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay10"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay11"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay12"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay13"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay14"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay15"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay16"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay17"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay18"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay19"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay20"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay21"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay22"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay23"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay24"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay25"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay26"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay27"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay28"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay29"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay30"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["iDay31"].ToString() + "',   \n");
                    sql.AppendLine("   '" + listInfoData.Rows[i]["dZYTime"].ToString() + "',   \n");
                    sql.AppendLine("   '1', \n");
                    sql.AppendLine("   '0', \n");
                    sql.AppendLine("   '月度内示', \n");
                    sql.AppendLine("   '"+ dt1 + "', \n");
                    sql.AppendLine("   '" + userId + "',   \n");
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
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion

        public void Updateprint()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("      update TPackNSCalculation set vcIsorNoPrint='1'     ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
