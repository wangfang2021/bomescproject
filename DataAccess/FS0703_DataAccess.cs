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
                strSql.AppendLine("      select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where isnull(vcSupplierCode,'')<>''  ");

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
                string strN = dtn1.ToString("yyyyMM");
                string strN_1 = dtn1.AddMonths(1).ToString("yyyyMM");
                string strN_2 = dtn1.AddMonths(2).ToString("yyyyMM");
                string strN_CL = dtn1.AddMonths(-1).ToString("yyyyMM");

                string NowDF = dtn1.AddDays(1 - dtn1.Day).Date.ToString("yyyy-MM-dd HH:mm:ss");

                int MonthDayNum = int.Parse(dtn1.AddDays(1 - dtn1.Day).Date.AddMonths(1).AddSeconds(-1).ToString("dd"));



                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  delete from TPackJSException          ");
                strSql.AppendLine("  delete from TPackNSCalculation        ");
                strSql.AppendLine("  select a.vcDXYM as vcYearMonth,a.vcPart_id,d.vcPackSpot,d.vcSupplierCode,d.vcSupplierPlant,       ");
                strSql.AppendLine("  d.vcSupplierName,d.vcPackGPSNo,d.vcPackNo,d.dUsedFrom,d.dUsedTo,d.vcCycle,d.iRelease,       ");
                strSql.AppendLine("  (a.iD1+a.iD2+a.iD3+a.iD4+a.iD5+a.iD6+a.iD7+a.iD8+a.iD9+a.iD10+a.iD11+a.iD12+a.iD13+a.iD14+       ");
                strSql.AppendLine("  a.iD15+a.iD16+a.iD17+a.iD18+a.iD19+a.iD20+a.iD21+a.iD22+a.iD23+a.iD24+a.iD25+a.iD26+a.iD27+       ");
                strSql.AppendLine("  a.iD28+a.iD29+a.iD30+a.iD31       ");
                strSql.AppendLine("  )*d.iBiYao as iHySOQN,       ");
                strSql.AppendLine("  (b.iD1+b.iD2+b.iD3+b.iD4+b.iD5+b.iD6+b.iD7+b.iD8+b.iD9+b.iD10+b.iD11+b.iD12+b.iD13+b.iD14+       ");
                strSql.AppendLine("  b.iD15+b.iD16+b.iD17+b.iD18+b.iD19+b.iD20+b.iD21+b.iD22+b.iD23+b.iD24+b.iD25+b.iD26+b.iD27+       ");
                strSql.AppendLine("  b.iD28+b.iD29+b.iD30+b.iD31       ");
                strSql.AppendLine("  )*d.iBiYao as iHySOQN1,       ");
                strSql.AppendLine("  (c.iD1+c.iD2+c.iD3+c.iD4+c.iD5+c.iD6+c.iD7+c.iD8+c.iD9+c.iD10+c.iD11+c.iD12+c.iD13+c.iD14+       ");
                strSql.AppendLine("  c.iD15+c.iD16+c.iD17+c.iD18+c.iD19+c.iD20+c.iD21+c.iD22+c.iD23+c.iD24+c.iD25+c.iD26+c.iD27+       ");
                strSql.AppendLine("  c.iD28+c.iD29+c.iD30+c.iD31       ");
                strSql.AppendLine("  )*d.iBiYao as iHySOQN2,       ");
                for (int i = 1; i <= MonthDayNum; i++)
                {
                    string time = dtn1.AddDays(i - dtn1.Day).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    strSql.AppendLine("   case when '"+ time + "' >=d.dFrom and '"+ time + "' <=d.dTo then a.iD"+i+"*d.iBiYao else '0' end as iD"+i+",      ");

                }
                for (int j=1;j<=31- MonthDayNum;j++) {
                    strSql.AppendLine("  '0' as iD'"+(j+ MonthDayNum).ToString() +"'  ,     ");
                }
                strSql.AppendLine("  GETDATE() as dZCTime       ");
                strSql.AppendLine("  from(       ");
                strSql.AppendLine("    select             ");
                strSql.AppendLine("       vcDXYM,vcPart_id,iPartNums,iQuantityPercontainer,            ");
                strSql.AppendLine("       isnull(iD1,'') as iD1,isnull(iD2,'') as iD2,isnull(iD3,'') as iD3,isnull(iD4,'') as iD4,isnull(iD5,'') as iD5,            ");
                strSql.AppendLine("       isnull(iD6,'') as iD6,isnull(iD7,'') as iD7,isnull(iD8,'') as iD8,isnull(iD9,'') as iD9,isnull(iD10,'') as iD10,            ");
                strSql.AppendLine("       isnull(iD11,'') as iD11,isnull(iD12,'') as iD12,isnull(iD13,'') as iD13,isnull(iD14,'') as iD14,isnull(iD15,'') as iD15,       ");
                strSql.AppendLine("       isnull(iD16,'') as iD16,isnull(iD17,'') as iD17,isnull(iD18,'') as iD18,isnull(iD19,'') as iD19,isnull(iD20,'') as iD20,       ");
                strSql.AppendLine("       isnull(iD21,'') as iD21,isnull(iD22,'') as iD22,isnull(iD23,'') as iD23,isnull(iD24,'') as iD24,isnull(iD25,'') as iD25,       ");
                strSql.AppendLine("       isnull(iD26,'') as iD26,isnull(iD27,'') as iD27,isnull(iD28,'') as iD28,isnull(iD29,'') as iD29,isnull(iD30,'') as iD30,       ");
                strSql.AppendLine("       isnull(iD31,'') as iD31            ");
                strSql.AppendLine("      from TSoqReply where vcDXYM='"+ strN + "' and vcCLYM='"+ strN_CL + "'       ");
                strSql.AppendLine("     )a left join       ");
                strSql.AppendLine("  (       ");
                strSql.AppendLine("  select             ");
                strSql.AppendLine("    vcDXYM,vcPart_id,iPartNums,iQuantityPercontainer,            ");
                strSql.AppendLine("    isnull(iD1,'') as iD1,isnull(iD2,'') as iD2,isnull(iD3,'') as iD3,isnull(iD4,'') as iD4,isnull(iD5,'') as iD5,            ");
                strSql.AppendLine("    isnull(iD6,'') as iD6,isnull(iD7,'') as iD7,isnull(iD8,'') as iD8,isnull(iD9,'') as iD9,isnull(iD10,'') as iD10,            ");
                strSql.AppendLine("    isnull(iD11,'') as iD11,isnull(iD12,'') as iD12,isnull(iD13,'') as iD13,isnull(iD14,'') as iD14,isnull(iD15,'') as iD15,       ");
                strSql.AppendLine("    isnull(iD16,'') as iD16,isnull(iD17,'') as iD17,isnull(iD18,'') as iD18,isnull(iD19,'') as iD19,isnull(iD20,'') as iD20,       ");
                strSql.AppendLine("    isnull(iD21,'') as iD21,isnull(iD22,'') as iD22,isnull(iD23,'') as iD23,isnull(iD24,'') as iD24,isnull(iD25,'') as iD25,       ");
                strSql.AppendLine("    isnull(iD26,'') as iD26,isnull(iD27,'') as iD27,isnull(iD28,'') as iD28,isnull(iD29,'') as iD29,isnull(iD30,'') as iD30,       ");
                strSql.AppendLine("    isnull(iD31,'') as iD31            ");
                strSql.AppendLine("   from TSoqReply where vcDXYM='"+ strN_1 + "' and vcCLYM='"+ strN_CL + "'       ");
                strSql.AppendLine("  )b on a.vcPart_id=b.vcPart_id       ");
                strSql.AppendLine("  left join       ");
                strSql.AppendLine("  (       ");
                strSql.AppendLine("    select            ");
                strSql.AppendLine("     vcDXYM,vcPart_id,iPartNums,iQuantityPercontainer,           ");
                strSql.AppendLine("     isnull(iD1,'') as iD1,isnull(iD2,'') as iD2,isnull(iD3,'') as iD3,isnull(iD4,'') as iD4,isnull(iD5,'') as iD5,           ");
                strSql.AppendLine("     isnull(iD6,'') as iD6,isnull(iD7,'') as iD7,isnull(iD8,'') as iD8,isnull(iD9,'') as iD9,isnull(iD10,'') as iD10,           ");
                strSql.AppendLine("     isnull(iD11,'') as iD11,isnull(iD12,'') as iD12,isnull(iD13,'') as iD13,isnull(iD14,'') as iD14,isnull(iD15,'') as iD15,      ");
                strSql.AppendLine("     isnull(iD16,'') as iD16,isnull(iD17,'') as iD17,isnull(iD18,'') as iD18,isnull(iD19,'') as iD19,isnull(iD20,'') as iD20,      ");
                strSql.AppendLine("     isnull(iD21,'') as iD21,isnull(iD22,'') as iD22,isnull(iD23,'') as iD23,isnull(iD24,'') as iD24,isnull(iD25,'') as iD25,      ");
                strSql.AppendLine("     isnull(iD26,'') as iD26,isnull(iD27,'') as iD27,isnull(iD28,'') as iD28,isnull(iD29,'') as iD29,isnull(iD30,'') as iD30,      ");
                strSql.AppendLine("     isnull(iD31,'') as iD31           ");
                strSql.AppendLine("    from TSoqReply where vcDXYM='"+ strN_2 + "' and vcCLYM='"+ strN_CL + "'      ");
                strSql.AppendLine("   )c on a.vcPart_id=c.vcPart_id      ");
                strSql.AppendLine("   left join      ");
                strSql.AppendLine("   (      ");
                strSql.AppendLine("       select vcPartsNo,b2.vcPackSpot,b2.vcSupplierCode,b2.vcSupplierPlant,b2.vcSupplierName,           ");
                strSql.AppendLine("       b2.vcPackNo,b2.vcCycle,b2.iRelease,dUsedFrom,dUsedTo,a2.vcPackGPSNo,a2.dFrom,a2.dTo,a2.iBiYao       ");
                strSql.AppendLine("       from (     ");
                strSql.AppendLine("        select vcPartsNo,vcPackNo,dFrom,dTo,             ");
                strSql.AppendLine("        vcPackGPSNo,vcPackSpot,vcShouhuofangID,vcCar,dUsedFrom,dUsedTo,vcDistinguish,iBiYao,varChangedItem     ");
                strSql.AppendLine("        from TPackItem where  dFrom<='9999-12-31 23:59:59'and dTo>='"+ NowDF + "'      ");
                strSql.AppendLine("        )a2 left join     ");
                strSql.AppendLine("        (     ");
                strSql.AppendLine("         select * from TPackBase where  dPackFrom<='9999-12-31 23:59:59'and dPackTo>='"+ NowDF + "'      ");
                strSql.AppendLine("        )b2 on a2.vcPackNo=b2.vcPackNo     ");
                strSql.AppendLine("        left join     ");
                strSql.AppendLine("        (     ");
                strSql.AppendLine("    select * from TPackageMaster where 1=1      ");
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
                strSql.AppendLine("        )c2 on a2.vcPartsNo=c2.vcPart_id      ");
                strSql.AppendLine("   )d on a.vcPart_id=d.vcPartsNo      ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
                strSql.AppendLine("         ");
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
                strSql.AppendLine("       select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where isnull(vcSupplierCode,'')<>'' ");
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
                strSql.Append("   ,sum(iDay28)as iDay28,sum(iDay29)as iDay29,sum(iDay30)as iDay30,sum(iDay31)as iDay31,dZYTime,vcIsorNoPrint     \n");
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
                    strSql.Append("     group by vcPackNo, vcYearMonth, vcPackNo,vcPackGPSNo,vcPackSpot,vcSupplierCode,vcSupplierWork,vcSupplierName,vcSupplierPack ,vcCycle,iRelease,dZYTime,vcIsorNoPrint        \n");
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
        public void Save_GS(DataTable listInfoData, string strUserId, ref string strErrorName, List<Object> strSupplierCode)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    if (strSupplierCode.IndexOf(listInfoData.Rows[i]["vcSupplierCode"].ToString()) != -1)
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
                        if (string.IsNullOrEmpty(listInfoData.Rows[i]["iHySOQN"].ToString()))
                        {
                            sql.AppendLine("  0.00000,   \n");
                        }
                        else
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iHySOQN"].ToString() + "',   \n");

                        if (string.IsNullOrEmpty(listInfoData.Rows[i]["iHySOQN1"].ToString()))
                            sql.AppendLine("  0.00000,   \n");
                        else
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iHySOQN1"].ToString() + "',   \n");

                        if (string.IsNullOrEmpty(listInfoData.Rows[i]["iHySOQN2"].ToString()))
                            sql.AppendLine("  0.00000,   \n");
                        else
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iHySOQN2"].ToString() + "',   \n");


                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD1"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD1"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD2"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD2"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD3"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD3"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD4"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD4"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD5"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD5"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD6"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD6"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD7"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD7"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD8"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD8"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD9"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD9"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD10"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD10"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");


                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD11"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD11"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD12"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD12"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD13"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD13"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD14"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD14"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD15"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD15"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD16"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD16"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD17"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD17"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD18"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD18"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD19"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD19"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD20"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD20"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");


                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD21"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD21"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD22"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD22"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD23"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD23"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD24"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD24"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD25"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD25"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD26"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD26"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD27"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD27"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD28"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD28"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD29"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD29"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD30"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD30"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");
                        if (!string.IsNullOrEmpty(listInfoData.Rows[i]["iD31"].ToString()))
                            sql.AppendLine("   '" + listInfoData.Rows[i]["iD31"].ToString() + "',   \n");
                        else
                            sql.AppendLine("  0.00000,   \n");

                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD2"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD3"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD4"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD5"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD6"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD7"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD8"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD9"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD10"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD11"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD12"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD13"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD14"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD15"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD16"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD17"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD18"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD19"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD20"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD21"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD22"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD23"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD24"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD25"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD26"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD27"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD28"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD29"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD30"].ToString() + "',   \n");
                        //sql.AppendLine("   '" + listInfoData.Rows[i]["iD31"].ToString() + "',   \n");
                        sql.AppendLine("   '" + listInfoData.Rows[i]["dZCTime"].ToString() + "',   \n");
                        sql.AppendLine("   '0', \n");
                        sql.AppendLine("   '0', \n");
                        sql.AppendLine("   '" + strUserId + "',   \n");
                        sql.AppendLine("   getdate()  \n");
                        sql.AppendLine("    )  \n");
                    }



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


                string dt1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                DataView dv = listInfoData.DefaultView;
                DataTable dtt = dv.ToTable(true, "vcSupplierCode");




                sql.AppendLine("     delete  from  TPackNSCalculation         \n");

                for (int a = 0; a < dtt.Rows.Count; a++)
                {
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
                    sql.AppendLine("         '" + dtt.Rows[a]["vcSupplierCode"].ToString() + "',   \n");
                    sql.AppendLine("         '" + PackFrom + "',   \n");
                    sql.AppendLine("         '月度内示',   \n");
                    sql.AppendLine("         NULL,   \n");
                    sql.AppendLine("         '0',   \n");
                    sql.AppendLine("         '" + dt1 + "',   \n");
                    sql.AppendLine("         NULL,   \n");
                    sql.AppendLine("   '" + userId + "',   \n");
                    sql.AppendLine("   getdate()  \n");
                    sql.AppendLine("         )   \n");

                }



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
                    sql.AppendLine("   '" + dt1 + "', \n");
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
