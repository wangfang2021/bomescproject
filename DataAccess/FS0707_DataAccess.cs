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
    public class FS0707_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 供应商
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

        #region 种类
        public DataTable SearchKind(List<Object> strPartPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select distinct vcPorType  as vcName,vcPorType  as vcValue from TPartInfoMaster  where GETDATE() between dTimeFrom and dTimeTo and vcPorType <>''   ");
                if (strPartPlant.Count != 0)
                {

                    strSql.AppendLine(" and vcPartPlant in (     ");
                    for (int i = 0; i < strPartPlant.Count; i++)
                    {
                        if (strPartPlant.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + strPartPlant[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + strPartPlant[i] + "' ,   \n");
                    }

                    strSql.AppendLine("   )   ");
                }


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 工厂
        public DataTable SearchPartPlant()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select distinct vcPartPlant as vcName,vcPartPlant as vcValue from TPartInfoMaster  where GETDATE() between dTimeFrom and dTimeTo   ");
                strSql.AppendLine(" and vcPartPlant is not null    ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion




        #region 计算开始生成结果
        public DataTable SearchCalculation(string dFromBegin, string dFromEnd, List<Object> Project, string Kind, List<Object> OrderState, List<Object> OrderPartPlant)
        {
            try
            {
                string dFtime = dFromBegin.Substring(0, 7);
                string dTtime = dFromEnd.Substring(0, 7);

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    delete from   TPackWeekInfo;    ");

                strSql.AppendLine("    select a.vcPartsno,d.vcPackNo,d.vcPackGPSNo,d.vcSupplierName,d.vcSupplierCode,d.iRelease,        ");
                strSql.AppendLine("  a.vcD1b*d.iBiYao as vcD1bF,'' as vcD1bFShow ,a.vcD1y*d.iBiYao as vcD1yF, '' as vcD1yFShow,              ");
                strSql.AppendLine("  a.vcD2b*d.iBiYao as vcD2bF,'' as vcD2bFShow ,a.vcD2y*d.iBiYao as vcD2yF, '' as vcD2yFShow,              ");
                strSql.AppendLine("  a.vcD3b*d.iBiYao as vcD3bF,'' as vcD3bFShow ,a.vcD3y*d.iBiYao as vcD3yF, '' as vcD3yFShow,              ");
                strSql.AppendLine("  a.vcD4b*d.iBiYao as vcD4bF,'' as vcD4bFShow ,a.vcD4y*d.iBiYao as vcD4yF, '' as vcD4yFShow,              ");
                strSql.AppendLine("  a.vcD5b*d.iBiYao as vcD5bF,'' as vcD5bFShow ,a.vcD5y*d.iBiYao as vcD5yF, '' as vcD5yFShow,             ");
                strSql.AppendLine("  a.vcD6b*d.iBiYao as vcD6bF,'' as vcD6bFShow ,a.vcD6y*d.iBiYao as vcD6yF, '' as vcD6yFShow,              ");
                strSql.AppendLine("  a.vcD7b*d.iBiYao as vcD7bF,'' as vcD7bFShow ,a.vcD7y*d.iBiYao as vcD7yF, '' as vcD7yFShow,              ");
                strSql.AppendLine("  a.vcD8b*d.iBiYao as vcD8bF,'' as vcD8bFShow ,a.vcD8y*d.iBiYao as vcD8yF, '' as vcD8yFShow,              ");
                strSql.AppendLine("  a.vcD9b*d.iBiYao as vcD9bF,'' as vcD9bFShow ,a.vcD9y*d.iBiYao as vcD9yF, '' as vcD9yFShow,              ");
                strSql.AppendLine("  a.vcD10b*d.iBiYao as vcD10bF,'' as vcD10bFShow ,a.vcD10y*d.iBiYao as vcD10yF,'' as vcD10yFShow ,             ");
                strSql.AppendLine("  a.vcD11b*d.iBiYao as vcD11bF,'' as vcD11bFShow ,a.vcD11y*d.iBiYao as vcD11yF,'' as vcD11yFShow ,             ");
                strSql.AppendLine("  a.vcD12b*d.iBiYao as vcD12bF,'' as vcD12bFShow ,a.vcD12y*d.iBiYao as vcD12yF,'' as vcD12yFShow ,             ");
                strSql.AppendLine("  a.vcD13b*d.iBiYao as vcD13bF,'' as vcD13bFShow ,a.vcD13y*d.iBiYao as vcD13yF,'' as vcD13yFShow ,             ");
                strSql.AppendLine("  a.vcD14b*d.iBiYao as vcD14bF,'' as vcD14bFShow ,a.vcD14y*d.iBiYao as vcD14yF,'' as vcD14yFShow ,             ");
                strSql.AppendLine("  a.vcD15b*d.iBiYao as vcD15bF,'' as vcD15bFShow ,a.vcD15y*d.iBiYao as vcD15yF,'' as vcD15yFShow ,             ");
                strSql.AppendLine("  a.vcD16b*d.iBiYao as vcD16bF,'' as vcD16bFShow ,a.vcD16y*d.iBiYao as vcD16yF,'' as vcD16yFShow ,             ");
                strSql.AppendLine("  a.vcD17b*d.iBiYao as vcD17bF,'' as vcD17bFShow ,a.vcD17y*d.iBiYao as vcD17yF,'' as vcD17yFShow ,             ");
                strSql.AppendLine("  a.vcD18b*d.iBiYao as vcD18bF,'' as vcD18bFShow ,a.vcD18y*d.iBiYao as vcD18yF,'' as vcD18yFShow ,             ");
                strSql.AppendLine("  a.vcD19b*d.iBiYao as vcD19bF,'' as vcD19bFShow ,a.vcD19y*d.iBiYao as vcD19yF,'' as vcD19yFShow ,             ");
                strSql.AppendLine("  a.vcD20b*d.iBiYao as vcD20bF,'' as vcD20bFShow ,a.vcD20y*d.iBiYao as vcD20yF,'' as vcD20yFShow ,             ");
                strSql.AppendLine("  a.vcD21b*d.iBiYao as vcD21bF,'' as vcD21bFShow ,a.vcD21y*d.iBiYao as vcD21yF,'' as vcD21yFShow ,             ");
                strSql.AppendLine("  a.vcD22b*d.iBiYao as vcD22bF,'' as vcD22bFShow ,a.vcD22y*d.iBiYao as vcD22yF,'' as vcD22yFShow ,             ");
                strSql.AppendLine("  a.vcD23b*d.iBiYao as vcD23bF,'' as vcD23bFShow ,a.vcD23y*d.iBiYao as vcD23yF,'' as vcD23yFShow ,             ");
                strSql.AppendLine("  a.vcD24b*d.iBiYao as vcD24bF,'' as vcD24bFShow ,a.vcD24y*d.iBiYao as vcD24yF,'' as vcD24yFShow ,             ");
                strSql.AppendLine("  a.vcD25b*d.iBiYao as vcD25bF,'' as vcD25bFShow ,a.vcD25y*d.iBiYao as vcD25yF,'' as vcD25yFShow ,             ");
                strSql.AppendLine("  a.vcD26b*d.iBiYao as vcD26bF,'' as vcD26bFShow ,a.vcD26y*d.iBiYao as vcD26yF,'' as vcD26yFShow ,             ");
                strSql.AppendLine("  a.vcD27b*d.iBiYao as vcD27bF,'' as vcD27bFShow ,a.vcD27y*d.iBiYao as vcD27yF,'' as vcD27yFShow ,             ");
                strSql.AppendLine("  a.vcD28b*d.iBiYao as vcD28bF,'' as vcD28bFShow ,a.vcD28y*d.iBiYao as vcD28yF,'' as vcD28yFShow ,             ");
                strSql.AppendLine("  a.vcD29b*d.iBiYao as vcD29bF,'' as vcD29bFShow ,a.vcD29y*d.iBiYao as vcD29yF,'' as vcD29yFShow ,              ");
                strSql.AppendLine("  a.vcD30b*d.iBiYao as vcD30bF,'' as vcD30bFShow ,a.vcD30y*d.iBiYao as vcD30yF,'' as vcD30yFShow ,              ");
                strSql.AppendLine("  a.vcD31b*d.iBiYao as vcD31bF,'' as vcD31bFShow ,a.vcD31y*d.iBiYao as vcD31yF,'' as vcD31yFShow               ");
                if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                {
                    strSql.AppendLine(" , c.vcD1b*d.iBiYao as vcD1bT,'' as vcD1bTShow ,c.vcD1y*d.iBiYao as vcD1yT, '' as vcD1yTShow,              ");
                    strSql.AppendLine("  c.vcD2b*d.iBiYao as vcD2bT,'' as vcD2bTShow ,c.vcD2y*d.iBiYao as vcD2yT, '' as vcD2yTShow,              ");
                    strSql.AppendLine("  c.vcD3b*d.iBiYao as vcD3bT,'' as vcD3bTShow ,c.vcD3y*d.iBiYao as vcD3yT, '' as vcD3yTShow,              ");
                    strSql.AppendLine("  c.vcD4b*d.iBiYao as vcD4bT,'' as vcD4bTShow ,c.vcD4y*d.iBiYao as vcD4yT, '' as vcD4yTShow,              ");
                    strSql.AppendLine("  c.vcD5b*d.iBiYao as vcD5bT,'' as vcD5bTShow ,c.vcD5y*d.iBiYao as vcD5yT, '' as vcD5yTShow,              ");
                    strSql.AppendLine("  c.vcD6b*d.iBiYao as vcD6bT,'' as vcD6bTShow ,c.vcD6y*d.iBiYao as vcD6yT, '' as vcD6yTShow,              ");
                    strSql.AppendLine("  c.vcD7b*d.iBiYao as vcD7bT,'' as vcD7bTShow ,c.vcD7y*d.iBiYao as vcD7yT, '' as vcD7yTShow,              ");
                    strSql.AppendLine("  c.vcD8b*d.iBiYao as vcD8bT,'' as vcD8bTShow ,c.vcD8y*d.iBiYao as vcD8yT, '' as vcD8yTShow,              ");
                    strSql.AppendLine("  c.vcD9b*d.iBiYao as vcD9bT,'' as vcD9bTShow ,c.vcD9y*d.iBiYao as vcD9yT, '' as vcD9yTShow,              ");
                    strSql.AppendLine("  c.vcD10b*d.iBiYao as vcD10bT,'' as vcD10bTShow ,c.vcD10y*d.iBiYao as vcD10yT,'' as vcD10yTShow ,              ");
                    strSql.AppendLine("  c.vcD11b*d.iBiYao as vcD11bT,'' as vcD11bTShow ,c.vcD11y*d.iBiYao as vcD11yT,'' as vcD11yTShow ,              ");
                    strSql.AppendLine("  c.vcD12b*d.iBiYao as vcD12bT,'' as vcD12bTShow ,c.vcD12y*d.iBiYao as vcD12yT,'' as vcD12yTShow ,              ");
                    strSql.AppendLine("  c.vcD13b*d.iBiYao as vcD13bT,'' as vcD13bTShow ,c.vcD13y*d.iBiYao as vcD13yT,'' as vcD13yTShow ,              ");
                    strSql.AppendLine("  c.vcD14b*d.iBiYao as vcD14bT,'' as vcD14bTShow ,c.vcD14y*d.iBiYao as vcD14yT,'' as vcD14yTShow ,              ");
                    strSql.AppendLine("  c.vcD15b*d.iBiYao as vcD15bT,'' as vcD15bTShow ,c.vcD15y*d.iBiYao as vcD15yT,'' as vcD15yTShow ,              ");
                    strSql.AppendLine("  c.vcD16b*d.iBiYao as vcD16bT,'' as vcD16bTShow ,c.vcD16y*d.iBiYao as vcD16yT,'' as vcD16yTShow ,              ");
                    strSql.AppendLine("  c.vcD17b*d.iBiYao as vcD17bT,'' as vcD17bTShow ,c.vcD17y*d.iBiYao as vcD17yT,'' as vcD17yTShow ,              ");
                    strSql.AppendLine("  c.vcD18b*d.iBiYao as vcD18bT,'' as vcD18bTShow ,c.vcD18y*d.iBiYao as vcD18yT,'' as vcD18yTShow ,              ");
                    strSql.AppendLine("  c.vcD19b*d.iBiYao as vcD19bT,'' as vcD19bTShow ,c.vcD19y*d.iBiYao as vcD19yT,'' as vcD19yTShow ,              ");
                    strSql.AppendLine("  c.vcD20b*d.iBiYao as vcD20bT,'' as vcD20bTShow ,c.vcD20y*d.iBiYao as vcD20yT,'' as vcD20yTShow ,              ");
                    strSql.AppendLine("  c.vcD21b*d.iBiYao as vcD21bT,'' as vcD21bTShow ,c.vcD21y*d.iBiYao as vcD21yT,'' as vcD21yTShow ,              ");
                    strSql.AppendLine("  c.vcD22b*d.iBiYao as vcD22bT,'' as vcD22bTShow ,c.vcD22y*d.iBiYao as vcD22yT,'' as vcD22yTShow ,              ");
                    strSql.AppendLine("  c.vcD23b*d.iBiYao as vcD23bT,'' as vcD23bTShow ,c.vcD23y*d.iBiYao as vcD23yT,'' as vcD23yTShow ,              ");
                    strSql.AppendLine("  c.vcD24b*d.iBiYao as vcD24bT,'' as vcD24bTShow ,c.vcD24y*d.iBiYao as vcD24yT,'' as vcD24yTShow ,              ");
                    strSql.AppendLine("  c.vcD25b*d.iBiYao as vcD25bT,'' as vcD25bTShow ,c.vcD25y*d.iBiYao as vcD25yT,'' as vcD25yTShow ,              ");
                    strSql.AppendLine("  c.vcD26b*d.iBiYao as vcD26bT,'' as vcD26bTShow ,c.vcD26y*d.iBiYao as vcD26yT,'' as vcD26yTShow ,              ");
                    strSql.AppendLine("  c.vcD27b*d.iBiYao as vcD27bT,'' as vcD27bTShow ,c.vcD27y*d.iBiYao as vcD27yT,'' as vcD27yTShow ,              ");
                    strSql.AppendLine("  c.vcD28b*d.iBiYao as vcD28bT,'' as vcD28bTShow ,c.vcD28y*d.iBiYao as vcD28yT,'' as vcD28yTShow ,              ");
                    strSql.AppendLine("  c.vcD29b*d.iBiYao as vcD29bT,'' as vcD29bTShow ,c.vcD29y*d.iBiYao as vcD29yT,'' as vcD29yTShow ,              ");
                    strSql.AppendLine("  c.vcD30b*d.iBiYao as vcD30bT,'' as vcD30bTShow ,c.vcD30y*d.iBiYao as vcD30yT,'' as vcD30yTShow ,              ");
                    strSql.AppendLine("  c.vcD31b*d.iBiYao as vcD31bT,'' as vcD31bTShow ,c.vcD31y*d.iBiYao as vcD31yT,'' as vcD31yTShow                ");
                }
                strSql.AppendLine("    from(                                                        ");
                switch (Kind)
                {
                    case "0":
                        //strSql.AppendLine("    select * from MonthProdPlanTbl where vcMonth='" + dFtime + "'       ");
                        //strSql.AppendLine("    union all                                                    ");
                        strSql.AppendLine("    select * from WeekProdPlanTbl	where vcMonth='" + dFtime + "'      ");
                        strSql.AppendLine("    )a                                                           ");
                        if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                        {
                            strSql.AppendLine("    inner join                                                   ");
                            strSql.AppendLine("    (                                                            ");
                            //strSql.AppendLine("    select * from MonthProdPlanTbl where vcMonth='" + dTtime + "'       ");
                            //strSql.AppendLine("    union all                                                    ");
                            strSql.AppendLine("    select * from WeekProdPlanTbl	where vcMonth='" + dTtime + "'     ");
                            strSql.AppendLine("    )c on a.vcPartsno=c.vcPartsno                                ");
                        }
                        break;
                    case "1":
                        //strSql.AppendLine("    select * from MonthPackPlanTbl where vcMonth='" + dFtime + "'       ");
                        //strSql.AppendLine("    union all                                                    ");
                        strSql.AppendLine("    select * from WeekPackPlanTbl	where vcMonth='" + dFtime + "'      ");
                        strSql.AppendLine("    )a                                                           ");
                        if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                        {
                            strSql.AppendLine("    inner join                                                   ");
                            strSql.AppendLine("    (                                                            ");
                            //strSql.AppendLine("    select * from MonthPackPlanTbl where vcMonth='" + dTtime + "'       ");
                            //strSql.AppendLine("    union all                                                    ");
                            strSql.AppendLine("    select * from WeekPackPlanTbl	where vcMonth='" + dTtime + "'     ");
                            strSql.AppendLine("    )c on a.vcPartsno=c.vcPartsno                                ");
                        }
                        break;
                    case "2":
                        //strSql.AppendLine("    select * from MonthKanBanPlanTbl where vcMonth='" + dFtime + "'       ");
                        //strSql.AppendLine("    union all                                                    ");
                        strSql.AppendLine("    select * from WeekKanBanPlanTbl	where vcMonth='" + dFtime + "'      ");
                        strSql.AppendLine("    )a                                                           ");
                        if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                        {
                            strSql.AppendLine("    inner join                                                   ");
                            strSql.AppendLine("    (                                                            ");
                            //strSql.AppendLine("    select * from MonthKanBanPlanTbl where vcMonth='" + dTtime + "'       ");
                            //strSql.AppendLine("    union all                                                    ");
                            strSql.AppendLine("    select * from WeekKanBanPlanTbl	where vcMonth='" + dTtime + "'     ");
                            strSql.AppendLine("    )c on a.vcPartsno=c.vcPartsno                                ");
                        }
                        break;
                }
                strSql.AppendLine("    left join                                                   ");
                strSql.AppendLine("    (                                                            ");
                strSql.AppendLine("    select  * from TPartInfoMaster  where GETDATE() between dTimeFrom and dTimeTo                    ");

                if (OrderPartPlant.Count != 0)
                {
                    strSql.AppendLine("    and  vcPartPlant in (                                        ");
                    for (int i = 0; i < OrderPartPlant.Count; i++)
                    {
                        if (OrderPartPlant.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + OrderPartPlant[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + OrderPartPlant[i] + "' ,   \n");
                    }
                    strSql.AppendLine("                          )                                     ");
                }
                if (Project.Count != 0)
                {
                    strSql.AppendLine("    and vcPorType in (                                           ");
                    for (int i = 0; i < Project.Count; i++)
                    {
                        if (Project.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + Project[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + Project[i] + "' ,   \n");
                    }
                    strSql.AppendLine("    )                                                            ");
                }
                strSql.AppendLine("    )b on a.vcPartsno=b.vcPartsNo                                                                    ");
                strSql.AppendLine("    left join                                                                                       ");
                strSql.AppendLine("    (                                                                                                ");
                strSql.AppendLine("        select t.vcPartsNo,t.vcPackNo,t.vcPackGPSNo,y.iRelease,y.vcSupplierName,y.vcSupplierCode,t.iBiYao from(                ");
                strSql.AppendLine("       select * from TPackItem                                                                       ");
                strSql.AppendLine("       )t left join                                                                                 ");
                strSql.AppendLine("       (                                                                                             ");
                strSql.AppendLine("       select * from TPackBase                                                                       ");
                if (OrderState.Count != 0)
                {

                    strSql.AppendLine("                where   vcSupplierCode in(                                                      ");
                    for (int i = 0; i < OrderState.Count; i++)
                    {
                        if (OrderState.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + OrderState[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + OrderState[i] + "' ,   \n");
                    }
                    strSql.AppendLine("                )                                                                               ");
                }
                strSql.AppendLine("       )y on t.vcPackNo=y.vcPackNo                                                                   ");
                strSql.AppendLine("      )d on a.vcPartsno=d.vcPartsNo                                                                  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Updateprint()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("      update TPackWeekInfo set vcIsorNoPrint='1'     ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 发送
        public void Save(DataTable listInfoData, string strUserId, ref string strErrorPartId, string strBegin, string strEnd, string strFromBeginBZ, string strFromEndBZ, string strKind, List<Object> OrderState)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                string SupplierCode = "";

                string dt1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (OrderState.Count > 0)
                {
                    for (int j = 0; j < OrderState.Count; j++)
                    {
                        if (j != 0)
                        {
                            SupplierCode = SupplierCode + "," + OrderState[j].ToString();
                        }
                        else
                        {
                            SupplierCode = SupplierCode + OrderState[j].ToString();

                        }

                    }
                }
                switch (strKind) {
                    case "0":
                        strKind = "生产计划";
                        break;
                    case "1":
                        strKind = "包装计划";
                        break;
                    case "2":
                        strKind = "看板2计划";
                        break;
                }

                string Name = strBegin + "/" + strFromBeginBZ + " " + strEnd + "/" + strFromEndBZ + " " + strKind;
                sql.AppendLine("     delete from  TPackWeekInfo     \n");
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
                sql.AppendLine("         '" + SupplierCode + "',   \n");
                sql.AppendLine("         NULL,   \n");
                sql.AppendLine("         '周度内示',   \n");
                sql.AppendLine("         '" + Name + "',   \n");
                sql.AppendLine("         '0',   \n");
                sql.AppendLine("         '" + dt1 + "',   \n");
                sql.AppendLine("         NULL,   \n");
                sql.AppendLine("   '" + strUserId + "',   \n");
                sql.AppendLine("   getdate()  \n");
                sql.AppendLine("         )   \n");

                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.Append("  INSERT INTO [dbo].[TPackWeekInfoCV]    \r\n");
                    sql.Append("             (    \r\n");
                    sql.Append("             [vcPackNo]    \r\n");
                    sql.Append("             ,[vcPackGPSNo]    \r\n");
                    sql.Append("             ,[vcSupplierCode]    \r\n");
                    sql.Append("             ,[vcSupplierName]    \r\n");
                    sql.Append("             ,[iRelease]        \r\n");
                    sql.Append("             ,[vcD1yF]        \r\n");
                    sql.Append("             ,[vcD1yFShow]        \r\n");
                    sql.Append("             ,[vcD1bF]        \r\n");
                    sql.Append("             ,[vcD1bFShow]        \r\n");
                    sql.Append("             ,[vcD2yF]        \r\n");
                    sql.Append("             ,[vcD2yFShow]        \r\n");
                    sql.Append("             ,[vcD2bF]        \r\n");
                    sql.Append("             ,[vcD2bFShow]        \r\n");
                    sql.Append("             ,[vcD3yF]        \r\n");
                    sql.Append("             ,[vcD3yFShow]        \r\n");
                    sql.Append("             ,[vcD3bF]        \r\n");
                    sql.Append("             ,[vcD3bFShow]        \r\n");
                    sql.Append("             ,[vcD4yF]        \r\n");
                    sql.Append("             ,[vcD4yFShow]        \r\n");
                    sql.Append("             ,[vcD4bF]        \r\n");
                    sql.Append("             ,[vcD4bFShow]        \r\n");
                    sql.Append("             ,[vcD5yF]        \r\n");
                    sql.Append("             ,[vcD5yFShow]        \r\n");
                    sql.Append("             ,[vcD5bF]        \r\n");
                    sql.Append("             ,[vcD5bFShow]       \r\n");
                    sql.Append("             ,[vcD6yF]         \r\n");
                    sql.Append("             ,[vcD6yFShow]       \r\n");
                    sql.Append("             ,[vcD6bF]         \r\n");
                    sql.Append("             ,[vcD6bFShow]     \r\n");
                    sql.Append("             ,[vcD7yF]         \r\n");
                    sql.Append("             ,[vcD7yFShow]   \r\n");
                    sql.Append("             ,[vcD7bF]        \r\n");
                    sql.Append("             ,[vcD7bFShow]   \r\n");
                    sql.Append("             ,[vcD8yF]       \r\n");
                    sql.Append("             ,[vcD8yFShow]   \r\n");
                    sql.Append("             ,[vcD8bF]       \r\n");
                    sql.Append("             ,[vcD8bFShow]   \r\n");
                    sql.Append("             ,[vcD9yF]      \r\n");
                    sql.Append("             ,[vcD9yFShow]   \r\n");
                    sql.Append("             ,[vcD9bF]       \r\n");
                    sql.Append("             ,[vcD9bFShow]   \r\n");
                    sql.Append("             ,[vcD10yF]      \r\n");
                    sql.Append("             ,[vcD10yFShow]   \r\n");
                    sql.Append("             ,[vcD10bF]       \r\n");
                    sql.Append("             ,[vcD10bFShow]   \r\n");
                    sql.Append("             ,[vcD11yF]       \r\n");
                    sql.Append("             ,[vcD11yFShow]   \r\n");
                    sql.Append("         ,[vcD11bF]      \r\n");
                    sql.Append("         ,[vcD11bFShow]      \r\n");
                    sql.Append("         ,[vcD12yF]      \r\n");
                    sql.Append("         ,[vcD12yFShow]      \r\n");
                    sql.Append("         ,[vcD12bF]      \r\n");
                    sql.Append("         ,[vcD12bFShow]      \r\n");
                    sql.Append("         ,[vcD13yF]      \r\n");
                    sql.Append("         ,[vcD13yFShow]      \r\n");
                    sql.Append("         ,[vcD13bF]      \r\n");
                    sql.Append("         ,[vcD13bFShow]      \r\n");
                    sql.Append("         ,[vcD14yF]      \r\n");
                    sql.Append("         ,[vcD14yFShow]      \r\n");
                    sql.Append("         ,[vcD14bF]      \r\n");
                    sql.Append("         ,[vcD14bFShow]      \r\n");
                    sql.Append("         ,[vcD15yF]      \r\n");
                    sql.Append("         ,[vcD15yFShow]      \r\n");
                    sql.Append("         ,[vcD15bF]      \r\n");
                    sql.Append("         ,[vcD15bFShow]      \r\n");
                    sql.Append("         ,[vcD16yF]      \r\n");
                    sql.Append("         ,[vcD16yFShow]      \r\n");
                    sql.Append("         ,[vcD16bF]      \r\n");
                    sql.Append("         ,[vcD16bFShow]      \r\n");
                    sql.Append("         ,[vcD17yF]      \r\n");
                    sql.Append("         ,[vcD17yFShow]      \r\n");
                    sql.Append("       ,[vcD17bF]                      \r\n");
                    sql.Append("       ,[vcD17bFShow]                      \r\n");
                    sql.Append("       ,[vcD18yF]                      \r\n");
                    sql.Append("       ,[vcD18yFShow]                      \r\n");
                    sql.Append("       ,[vcD18bF]                      \r\n");
                    sql.Append("       ,[vcD18bFShow]                      \r\n");
                    sql.Append("       ,[vcD19yF]                      \r\n");
                    sql.Append("       ,[vcD19yFShow]                      \r\n");
                    sql.Append("       ,[vcD19bF]                      \r\n");
                    sql.Append("       ,[vcD19bFShow]                      \r\n");
                    sql.Append("       ,[vcD20yF]                      \r\n");
                    sql.Append("       ,[vcD20yFShow]                      \r\n");
                    sql.Append("       ,[vcD20bF]                      \r\n");
                    sql.Append("       ,[vcD20bFShow]                      \r\n");
                    sql.Append("       ,[vcD21yF]                      \r\n");
                    sql.Append("       ,[vcD21yFShow]                      \r\n");
                    sql.Append("       ,[vcD21bF]                      \r\n");
                    sql.Append("       ,[vcD21bFShow]                      \r\n");
                    sql.Append("       ,[vcD22yF]                      \r\n");
                    sql.Append("       ,[vcD22yFShow]                      \r\n");
                    sql.Append("       ,[vcD22bF]                      \r\n");
                    sql.Append("       ,[vcD22bFShow]                      \r\n");
                    sql.Append("       ,[vcD23yF]                      \r\n");
                    sql.Append("       ,[vcD23yFShow]                      \r\n");
                    sql.Append("       ,[vcD23bF]                      \r\n");
                    sql.Append("       ,[vcD23bFShow]                      \r\n");
                    sql.Append("       ,[vcD24yF]                      \r\n");
                    sql.Append("       ,[vcD24yFShow]                      \r\n");
                    sql.Append("       ,[vcD24bF]                      \r\n");
                    sql.Append("       ,[vcD24bFShow]                      \r\n");
                    sql.Append("       ,[vcD25yF]                      \r\n");
                    sql.Append("       ,[vcD25yFShow]                      \r\n");
                    sql.Append("       ,[vcD25bF]                      \r\n");
                    sql.Append("       ,[vcD25bFShow]                      \r\n");
                    sql.Append("       ,[vcD26yF]                      \r\n");
                    sql.Append("       ,[vcD26yFShow]                       \r\n");
                    sql.Append("       ,[vcD26bF]                       \r\n");
                    sql.Append("       ,[vcD26bFShow]                       \r\n");
                    sql.Append("       ,[vcD27yF]                       \r\n");
                    sql.Append("       ,[vcD27yFShow]                       \r\n");
                    sql.Append("       ,[vcD27bF]                       \r\n");
                    sql.Append("       ,[vcD27bFShow]                       \r\n");
                    sql.Append("       ,[vcD28yF]                       \r\n");
                    sql.Append("       ,[vcD28yFShow]                       \r\n");
                    sql.Append("       ,[vcD28bF]                       \r\n");
                    sql.Append("       ,[vcD28bFShow]                       \r\n");
                    sql.Append("       ,[vcD29yF]                       \r\n");
                    sql.Append("       ,[vcD29yFShow]                       \r\n");
                    sql.Append("       ,[vcD29bF]                       \r\n");
                    sql.Append("       ,[vcD29bFShow]                       \r\n");
                    sql.Append("       ,[vcD30yF]                       \r\n");
                    sql.Append("       ,[vcD30yFShow]                       \r\n");
                    sql.Append("       ,[vcD30bF]                       \r\n");
                    sql.Append("       ,[vcD30bFShow]                       \r\n");
                    sql.Append("       ,[vcD31yF]                       \r\n");
                    sql.Append("       ,[vcD31yFShow]                       \r\n");
                    sql.Append("       ,[vcD31bF]                       \r\n");
                    sql.Append("       ,[vcD31bFShow]                       \r\n");
                    sql.Append("       ,[vcD1yT]                       \r\n");
                    sql.Append("       ,[vcD1yTShow]                       \r\n");
                    sql.Append("       ,[vcD1bT]                       \r\n");
                    sql.Append("       ,[vcD1bTShow]                       \r\n");
                    sql.Append("       ,[vcD2yT]                       \r\n");
                    sql.Append("       ,[vcD2yTShow]                       \r\n");
                    sql.Append("       ,[vcD2bT]                       \r\n");
                    sql.Append("       ,[vcD2bTShow]                       \r\n");
                    sql.Append("       ,[vcD3yT]                       \r\n");
                    sql.Append("       ,[vcD3yTShow]                       \r\n");
                    sql.Append("       ,[vcD3bT]                       \r\n");
                    sql.Append("       ,[vcD3bTShow]                       \r\n");
                    sql.Append("       ,[vcD4yT]                       \r\n");
                    sql.Append("       ,[vcD4yTShow]                       \r\n");
                    sql.Append("       ,[vcD4bT]                       \r\n");
                    sql.Append("       ,[vcD4bTShow]                       \r\n");
                    sql.Append("       ,[vcD5yT]                       \r\n");
                    sql.Append("       ,[vcD5yTShow]                       \r\n");
                    sql.Append("       ,[vcD5bT]                       \r\n");
                    sql.Append("       ,[vcD5bTShow]                       \r\n");
                    sql.Append("       ,[vcD6yT]                       \r\n");
                    sql.Append("       ,[vcD6yTShow]                       \r\n");
                    sql.Append("       ,[vcD6bT]                       \r\n");
                    sql.Append("       ,[vcD6bTShow]                       \r\n");
                    sql.Append("       ,[vcD7yT]                       \r\n");
                    sql.Append("       ,[vcD7yTShow]                       \r\n");
                    sql.Append("       ,[vcD7bT]                       \r\n");
                    sql.Append("       ,[vcD7bTShow]                       \r\n");
                    sql.Append("       ,[vcD8yT]                       \r\n");
                    sql.Append("       ,[vcD8yTShow]                       \r\n");
                    sql.Append("       ,[vcD8bT]                       \r\n");
                    sql.Append("       ,[vcD8bTShow]                       \r\n");
                    sql.Append("       ,[vcD9yT]                       \r\n");
                    sql.Append("       ,[vcD9yTShow]                       \r\n");
                    sql.Append("       ,[vcD9bT]                       \r\n");
                    sql.Append("       ,[vcD9bTShow]                       \r\n");
                    sql.Append("       ,[vcD10yT]                       \r\n");
                    sql.Append("       ,[vcD10yTShow]                       \r\n");
                    sql.Append("       ,[vcD10bT]                       \r\n");
                    sql.Append("       ,[vcD10bTShow]                       \r\n");
                    sql.Append("       ,[vcD11yT]                       \r\n");
                    sql.Append("       ,[vcD11yTShow]                       \r\n");
                    sql.Append("       ,[vcD11bT]                       \r\n");
                    sql.Append("       ,[vcD11bTShow]                       \r\n");
                    sql.Append("       ,[vcD12yT]                       \r\n");
                    sql.Append("       ,[vcD12yTShow]                       \r\n");
                    sql.Append("       ,[vcD12bT]                       \r\n");
                    sql.Append("       ,[vcD12bTShow]                       \r\n");
                    sql.Append("       ,[vcD13yT]                       \r\n");
                    sql.Append("       ,[vcD13yTShow]                       \r\n");
                    sql.Append("       ,[vcD13bT]                       \r\n");
                    sql.Append("       ,[vcD13bTShow]                       \r\n");
                    sql.Append("       ,[vcD14yT]                       \r\n");
                    sql.Append("       ,[vcD14yTShow]                       \r\n");
                    sql.Append("       ,[vcD14bT]                       \r\n");
                    sql.Append("       ,[vcD14bTShow]                       \r\n");
                    sql.Append("       ,[vcD15yT]                       \r\n");
                    sql.Append("       ,[vcD15yTShow]                       \r\n");
                    sql.Append("       ,[vcD15bT]                       \r\n");
                    sql.Append("       ,[vcD15bTShow]                       \r\n");
                    sql.Append("       ,[vcD16yT]                       \r\n");
                    sql.Append("       ,[vcD16yTShow]                       \r\n");
                    sql.Append("       ,[vcD16bT]                       \r\n");
                    sql.Append("       ,[vcD16bTShow]                       \r\n");
                    sql.Append("       ,[vcD17yT]                       \r\n");
                    sql.Append("       ,[vcD17yTShow]                       \r\n");
                    sql.Append("       ,[vcD17bT]                       \r\n");
                    sql.Append("       ,[vcD17bTShow]                       \r\n");
                    sql.Append("       ,[vcD18yT]                       \r\n");
                    sql.Append("       ,[vcD18yTShow]                       \r\n");
                    sql.Append("       ,[vcD18bT]                       \r\n");
                    sql.Append("       ,[vcD18bTShow]                       \r\n");
                    sql.Append("       ,[vcD19yT]                       \r\n");
                    sql.Append("       ,[vcD19yTShow]                       \r\n");
                    sql.Append("       ,[vcD19bT]                       \r\n");
                    sql.Append("       ,[vcD19bTShow]                       \r\n");
                    sql.Append("       ,[vcD20yT]                       \r\n");
                    sql.Append("       ,[vcD20yTShow]                       \r\n");
                    sql.Append("       ,[vcD20bT]                       \r\n");
                    sql.Append("       ,[vcD20bTShow]                       \r\n");
                    sql.Append("       ,[vcD21yT]                       \r\n");
                    sql.Append("       ,[vcD21yTShow]                       \r\n");
                    sql.Append("       ,[vcD21bT]                       \r\n");
                    sql.Append("       ,[vcD21bTShow]                       \r\n");
                    sql.Append("       ,[vcD22yT]                       \r\n");
                    sql.Append("       ,[vcD22yTShow]                       \r\n");
                    sql.Append("       ,[vcD22bT]                       \r\n");
                    sql.Append("       ,[vcD22bTShow]                       \r\n");
                    sql.Append("       ,[vcD23yT]                       \r\n");
                    sql.Append("       ,[vcD23yTShow]                       \r\n");
                    sql.Append("       ,[vcD23bT]                       \r\n");
                    sql.Append("       ,[vcD23bTShow]                       \r\n");
                    sql.Append("       ,[vcD24yT]                       \r\n");
                    sql.Append("       ,[vcD24yTShow]                       \r\n");
                    sql.Append("       ,[vcD24bT]                       \r\n");
                    sql.Append("       ,[vcD24bTShow]                       \r\n");
                    sql.Append("       ,[vcD25yT]                       \r\n");
                    sql.Append("       ,[vcD25yTShow]                       \r\n");
                    sql.Append("       ,[vcD25bT]                       \r\n");
                    sql.Append("       ,[vcD25bTShow]                       \r\n");
                    sql.Append("       ,[vcD26yT]                       \r\n");
                    sql.Append("       ,[vcD26yTShow]                       \r\n");
                    sql.Append("       ,[vcD26bT]                       \r\n");
                    sql.Append("       ,[vcD26bTShow]                       \r\n");
                    sql.Append("       ,[vcD27yT]                       \r\n");
                    sql.Append("       ,[vcD27yTShow]                       \r\n");
                    sql.Append("       ,[vcD27bT]                       \r\n");
                    sql.Append("       ,[vcD27bTShow]                       \r\n");
                    sql.Append("       ,[vcD28yT]                       \r\n");
                    sql.Append("       ,[vcD28yTShow]                       \r\n");
                    sql.Append("       ,[vcD28bT]                       \r\n");
                    sql.Append("       ,[vcD28bTShow]                       \r\n");
                    sql.Append("       ,[vcD29yT]                       \r\n");
                    sql.Append("       ,[vcD29yTShow]                       \r\n");
                    sql.Append("       ,[vcD29bT]                       \r\n");
                    sql.Append("       ,[vcD29bTShow]                       \r\n");
                    sql.Append("       ,[vcD30yT]                       \r\n");
                    sql.Append("       ,[vcD30yTShow]                       \r\n");
                    sql.Append("       ,[vcD30bT]                       \r\n");
                    sql.Append("       ,[vcD30bTShow]                       \r\n");
                    sql.Append("       ,[vcD31yT]                       \r\n");
                    sql.Append("       ,[vcD31yTShow]                       \r\n");
                    sql.Append("       ,[vcD31bT]                       \r\n");
                    sql.Append("       ,[vcD31bTShow]                       \r\n");
                    sql.AppendLine("           ,[vcIsorNoSend]    \n");
                    sql.AppendLine("           ,[vcIsorNoPrint]    \n");

                    sql.AppendLine("           ,[vcNSQJ]    \n");
                    sql.AppendLine("           ,[vcNSDiff]    \n");
                    sql.AppendLine("           ,[dSendTime]    \n");
                    sql.AppendLine("           ,[vcPlanDiff]    \n");



                    sql.Append("       ,[vcOperatorID]                       \r\n");
                    sql.Append("       ,[dOperatorTime])                       \r\n");
                    sql.Append("       VALUES                       \r\n");
                    sql.Append("       (         \r\n");
                   // sql.Append("       '" + listInfoData.Rows[i]["vcPartsNo"].ToString() + "',         \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcPackNo"].ToString() + "',            \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcPackGPSNo"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcSupplierCode"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcSupplierName"].ToString() + "',              \r\n");

                    sql.Append("       '" + listInfoData.Rows[i]["iRelease"].ToString() + "',          \r\n");

                    #region
                    sql.Append("       '" + listInfoData.Rows[i]["vcD1yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD1yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD1bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD1bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD2yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD2yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD2bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD2bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD3yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD3yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD3bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD3bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD4yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD4yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD4bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD4bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD5yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD5yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD5bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD5bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD6yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD6yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD6bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD6bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD7yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD7yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD7bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD7bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD8yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD8yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD8bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD8bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD9yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD9yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD9bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD9bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD10yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD10yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD10bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD10bFShow"].ToString() + "',          \r\n");

                    sql.Append("       '" + listInfoData.Rows[i]["vcD11yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD11yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD11bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD11bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD12yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD12yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD12bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD12bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD13yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD13yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD13bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD13bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD14yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD14yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD14bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD14bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD15yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD15yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD15bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD15bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD16yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD16yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD16bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD16bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD17yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD17yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD17bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD17bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD18yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD18yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD18bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD18bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD19yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD19yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD19bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD19bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD20yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD20yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD20bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD20bFShow"].ToString() + "',          \r\n");

                    sql.Append("       '" + listInfoData.Rows[i]["vcD21yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD21yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD21bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD21bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD22yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD22yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD22bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD22bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD23yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD23yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD23bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD23bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD24yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD24yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD24bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD24bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD25yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD25yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD25bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD25bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD26yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD26yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD26bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD26bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD27yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD27yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD27bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD27bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD28yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD28yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD28bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD28bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD29yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD29yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD29bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD29bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD30yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD30yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD30bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD30bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD31yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD31yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD31bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD31bFShow"].ToString() + "',          \r\n");
                    #endregion

                    if (strBegin.Split("-")[1] != strEnd.Split("-")[1])
                    {
                        #region
                        sql.Append("       '" + listInfoData.Rows[i]["vcD1yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD1yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD1bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD1bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD2yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD2yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD2bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD2bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD3yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD3yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD3bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD3bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD4yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD4yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD4bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD4bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD5yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD5yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD5bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD5bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD6yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD6yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD6bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD6bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD7yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD7yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD7bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD7bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD8yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD8yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD8bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD8bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD9yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD9yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD9bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD9bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD10yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD10yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD10bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD10bTShow"].ToString() + "',          \r\n");

                        sql.Append("       '" + listInfoData.Rows[i]["vcD11yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD11yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD11bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD11bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD12yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD12yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD12bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD12bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD13yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD13yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD13bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD13bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD14yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD14yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD14bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD14bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD15yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD15yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD15bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD15bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD16yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD16yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD16bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD16bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD17yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD17yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD17bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD17bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD18yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD18yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD18bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD18bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD19yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD19yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD19bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD19bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD20yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD20yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD20bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD20bTShow"].ToString() + "',          \r\n");

                        sql.Append("       '" + listInfoData.Rows[i]["vcD21yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD21yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD21bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD21bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD22yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD22yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD22bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD22bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD23yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD23yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD23bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD23bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD24yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD24yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD24bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD24bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD25yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD25yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD25bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD25bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD26yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD26yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD26bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD26bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD27yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD27yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD27bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD27bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD28yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD28yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD28bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD28bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD29yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD29yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD29bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD29bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD30yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD30yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD30bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD30bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD31yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD31yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD31bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD31bTShow"].ToString() + "',          \r\n");
                        #endregion
                    }
                    else
                    {
                        #region
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");

                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");

                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        #endregion

                    }

                    sql.Append("       '1' ,                 \r\n");
                    sql.Append("      '0' ,                 \r\n");
                    sql.Append("      '"+ Name + "' ,                 \r\n");
                    sql.Append("       '周度内饰' ,                 \r\n");
                    sql.Append("       '"+dt1+"' ,                 \r\n");
                    sql.Append("      '"+strKind+"' ,                 \r\n");
                    sql.Append("    '" + strUserId + "'   ,                 \r\n");
                    sql.Append("      getdate()                   \r\n");
                    sql.Append("   )                       \r\n");


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

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPack_FaZhu_ShiJi where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                //  "vcPackSpot","vcPackNo","vcPackGPSNo","dPackFrom","dPackTo","vcParstName","vcSupplierName",
                //"vcSupplierCode","iRelease","iZCRelease","vcCycle","vcDistinguish","vcPackLocation","vcFormat","vcReleaseName"
                //先删除重复待更新的
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("  delete from  TPackBase where vcPackSpot=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + " and vcPackNo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + " and vcPackGPSNo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], false) + "   \r\n");
                }
                //插入
                sql.Append("  INSERT INTO [dbo].[TPackBase]   \r\n");
                sql.Append("             ([vcPackNo]   \r\n");
                sql.Append("             ,[vcPackSpot]   \r\n");
                sql.Append("             ,[dPackFrom]   \r\n");
                sql.Append("             ,[dPackTo]   \r\n");
                sql.Append("             ,[vcPackGPSNo]   \r\n");
                sql.Append("             ,[vcSupplierCode]   \r\n");
                sql.Append("             ,[vcSupplierPlant]   \r\n");
                sql.Append("             ,[vcCycle]   \r\n");
                sql.Append("             ,[vcSupplierName]   \r\n");
                sql.Append("             ,[vcParstName]   \r\n");
                sql.Append("             ,[vcPackLocation]   \r\n");
                sql.Append("             ,[vcDistinguish]   \r\n");
                sql.Append("             ,[vcFormat]   \r\n");
                //sql.Append("             ,[vcReleaseID]   \r\n");
                sql.Append("             ,[vcReleaseName]   \r\n");
                sql.Append("             ,[iRelease]   \r\n");
                sql.Append("             ,[iZCRelease]   \r\n");
                sql.Append("             ,[isYZC]   \r\n");
                sql.Append("             ,[vcOperatorID]   \r\n");
                sql.Append("             ,[dOperatorTime])   \r\n");
                sql.Append("       VALUES   \r\n");
                sql.Append("             (   \r\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dPackFrom"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dPackTo"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], true) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierCode"], true) + ", \r\n");
                    sql.Append("   null,  \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcCycle"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierName"], true) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcParstName"], true) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackLocation"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcDistinguish"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcFormat"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcReleaseName"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["iRelease"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["iZCRelease"], true) + ", \r\n");
                    sql.Append("   null,  \r\n");
                    sql.Append("   '" + strUserId + "',  \r\n");
                    sql.Append("   getdate())  \r\n");

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
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select *,'0' as vcModFlag,'0' as vcAddFlag from TPrice_GS         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if (strBegin != "")
                    strSql.Append("   and    dBegin>='" + strBegin + "'         \n");
                if (strEnd != "")
                    strSql.Append("   and    dEnd<='" + strEnd + "'         \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save_GS(DataTable listInfoData, string strUserId, ref string strErrorName, string strBegin, string strEnd)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    sql.Append("  INSERT INTO [dbo].[TPackWeekInfo]    \r\n");
                    sql.Append("             ([vcPartsNo]    \r\n");
                    sql.Append("             ,[vcPackNo]    \r\n");
                    sql.Append("             ,[vcPackGPSNo]    \r\n");
                    sql.Append("             ,[vcSupplierCode]    \r\n");
                    sql.Append("             ,[vcSupplierName]    \r\n");
                    sql.Append("             ,[iRelease]        \r\n");
                    sql.Append("             ,[vcD1yF]        \r\n");
                    sql.Append("             ,[vcD1yFShow]        \r\n");
                    sql.Append("             ,[vcD1bF]        \r\n");
                    sql.Append("             ,[vcD1bFShow]        \r\n");
                    sql.Append("             ,[vcD2yF]        \r\n");
                    sql.Append("             ,[vcD2yFShow]        \r\n");
                    sql.Append("             ,[vcD2bF]        \r\n");
                    sql.Append("             ,[vcD2bFShow]        \r\n");
                    sql.Append("             ,[vcD3yF]        \r\n");
                    sql.Append("             ,[vcD3yFShow]        \r\n");
                    sql.Append("             ,[vcD3bF]        \r\n");
                    sql.Append("             ,[vcD3bFShow]        \r\n");
                    sql.Append("             ,[vcD4yF]        \r\n");
                    sql.Append("             ,[vcD4yFShow]        \r\n");
                    sql.Append("             ,[vcD4bF]        \r\n");
                    sql.Append("             ,[vcD4bFShow]        \r\n");
                    sql.Append("             ,[vcD5yF]        \r\n");
                    sql.Append("             ,[vcD5yFShow]        \r\n");
                    sql.Append("             ,[vcD5bF]        \r\n");
                    sql.Append("             ,[vcD5bFShow]       \r\n");
                    sql.Append("             ,[vcD6yF]         \r\n");
                    sql.Append("             ,[vcD6yFShow]       \r\n");
                    sql.Append("             ,[vcD6bF]         \r\n");
                    sql.Append("             ,[vcD6bFShow]     \r\n");
                    sql.Append("             ,[vcD7yF]         \r\n");
                    sql.Append("             ,[vcD7yFShow]   \r\n");
                    sql.Append("             ,[vcD7bF]        \r\n");
                    sql.Append("             ,[vcD7bFShow]   \r\n");
                    sql.Append("             ,[vcD8yF]       \r\n");
                    sql.Append("             ,[vcD8yFShow]   \r\n");
                    sql.Append("             ,[vcD8bF]       \r\n");
                    sql.Append("             ,[vcD8bFShow]   \r\n");
                    sql.Append("             ,[vcD9yF]      \r\n");
                    sql.Append("             ,[vcD9yFShow]   \r\n");
                    sql.Append("             ,[vcD9bF]       \r\n");
                    sql.Append("             ,[vcD9bFShow]   \r\n");
                    sql.Append("             ,[vcD10yF]      \r\n");
                    sql.Append("             ,[vcD10yFShow]   \r\n");
                    sql.Append("             ,[vcD10bF]       \r\n");
                    sql.Append("             ,[vcD10bFShow]   \r\n");
                    sql.Append("             ,[vcD11yF]       \r\n");
                    sql.Append("             ,[vcD11yFShow]   \r\n");
                    sql.Append("         ,[vcD11bF]      \r\n");
                    sql.Append("         ,[vcD11bFShow]      \r\n");
                    sql.Append("         ,[vcD12yF]      \r\n");
                    sql.Append("         ,[vcD12yFShow]      \r\n");
                    sql.Append("         ,[vcD12bF]      \r\n");
                    sql.Append("         ,[vcD12bFShow]      \r\n");
                    sql.Append("         ,[vcD13yF]      \r\n");
                    sql.Append("         ,[vcD13yFShow]      \r\n");
                    sql.Append("         ,[vcD13bF]      \r\n");
                    sql.Append("         ,[vcD13bFShow]      \r\n");
                    sql.Append("         ,[vcD14yF]      \r\n");
                    sql.Append("         ,[vcD14yFShow]      \r\n");
                    sql.Append("         ,[vcD14bF]      \r\n");
                    sql.Append("         ,[vcD14bFShow]      \r\n");
                    sql.Append("         ,[vcD15yF]      \r\n");
                    sql.Append("         ,[vcD15yFShow]      \r\n");
                    sql.Append("         ,[vcD15bF]      \r\n");
                    sql.Append("         ,[vcD15bFShow]      \r\n");
                    sql.Append("         ,[vcD16yF]      \r\n");
                    sql.Append("         ,[vcD16yFShow]      \r\n");
                    sql.Append("         ,[vcD16bF]      \r\n");
                    sql.Append("         ,[vcD16bFShow]      \r\n");
                    sql.Append("         ,[vcD17yF]      \r\n");
                    sql.Append("         ,[vcD17yFShow]      \r\n");
                    sql.Append("       ,[vcD17bF]                      \r\n");
                    sql.Append("       ,[vcD17bFShow]                      \r\n");
                    sql.Append("       ,[vcD18yF]                      \r\n");
                    sql.Append("       ,[vcD18yFShow]                      \r\n");
                    sql.Append("       ,[vcD18bF]                      \r\n");
                    sql.Append("       ,[vcD18bFShow]                      \r\n");
                    sql.Append("       ,[vcD19yF]                      \r\n");
                    sql.Append("       ,[vcD19yFShow]                      \r\n");
                    sql.Append("       ,[vcD19bF]                      \r\n");
                    sql.Append("       ,[vcD19bFShow]                      \r\n");
                    sql.Append("       ,[vcD20yF]                      \r\n");
                    sql.Append("       ,[vcD20yFShow]                      \r\n");
                    sql.Append("       ,[vcD20bF]                      \r\n");
                    sql.Append("       ,[vcD20bFShow]                      \r\n");
                    sql.Append("       ,[vcD21yF]                      \r\n");
                    sql.Append("       ,[vcD21yFShow]                      \r\n");
                    sql.Append("       ,[vcD21bF]                      \r\n");
                    sql.Append("       ,[vcD21bFShow]                      \r\n");
                    sql.Append("       ,[vcD22yF]                      \r\n");
                    sql.Append("       ,[vcD22yFShow]                      \r\n");
                    sql.Append("       ,[vcD22bF]                      \r\n");
                    sql.Append("       ,[vcD22bFShow]                      \r\n");
                    sql.Append("       ,[vcD23yF]                      \r\n");
                    sql.Append("       ,[vcD23yFShow]                      \r\n");
                    sql.Append("       ,[vcD23bF]                      \r\n");
                    sql.Append("       ,[vcD23bFShow]                      \r\n");
                    sql.Append("       ,[vcD24yF]                      \r\n");
                    sql.Append("       ,[vcD24yFShow]                      \r\n");
                    sql.Append("       ,[vcD24bF]                      \r\n");
                    sql.Append("       ,[vcD24bFShow]                      \r\n");
                    sql.Append("       ,[vcD25yF]                      \r\n");
                    sql.Append("       ,[vcD25yFShow]                      \r\n");
                    sql.Append("       ,[vcD25bF]                      \r\n");
                    sql.Append("       ,[vcD25bFShow]                      \r\n");
                    sql.Append("       ,[vcD26yF]                      \r\n");
                    sql.Append("       ,[vcD26yFShow]                       \r\n");
                    sql.Append("       ,[vcD26bF]                       \r\n");
                    sql.Append("       ,[vcD26bFShow]                       \r\n");
                    sql.Append("       ,[vcD27yF]                       \r\n");
                    sql.Append("       ,[vcD27yFShow]                       \r\n");
                    sql.Append("       ,[vcD27bF]                       \r\n");
                    sql.Append("       ,[vcD27bFShow]                       \r\n");
                    sql.Append("       ,[vcD28yF]                       \r\n");
                    sql.Append("       ,[vcD28yFShow]                       \r\n");
                    sql.Append("       ,[vcD28bF]                       \r\n");
                    sql.Append("       ,[vcD28bFShow]                       \r\n");
                    sql.Append("       ,[vcD29yF]                       \r\n");
                    sql.Append("       ,[vcD29yFShow]                       \r\n");
                    sql.Append("       ,[vcD29bF]                       \r\n");
                    sql.Append("       ,[vcD29bFShow]                       \r\n");
                    sql.Append("       ,[vcD30yF]                       \r\n");
                    sql.Append("       ,[vcD30yFShow]                       \r\n");
                    sql.Append("       ,[vcD30bF]                       \r\n");
                    sql.Append("       ,[vcD30bFShow]                       \r\n");
                    sql.Append("       ,[vcD31yF]                       \r\n");
                    sql.Append("       ,[vcD31yFShow]                       \r\n");
                    sql.Append("       ,[vcD31bF]                       \r\n");
                    sql.Append("       ,[vcD31bFShow]                       \r\n");
                    sql.Append("       ,[vcD1yT]                       \r\n");
                    sql.Append("       ,[vcD1yTShow]                       \r\n");
                    sql.Append("       ,[vcD1bT]                       \r\n");
                    sql.Append("       ,[vcD1bTShow]                       \r\n");
                    sql.Append("       ,[vcD2yT]                       \r\n");
                    sql.Append("       ,[vcD2yTShow]                       \r\n");
                    sql.Append("       ,[vcD2bT]                       \r\n");
                    sql.Append("       ,[vcD2bTShow]                       \r\n");
                    sql.Append("       ,[vcD3yT]                       \r\n");
                    sql.Append("       ,[vcD3yTShow]                       \r\n");
                    sql.Append("       ,[vcD3bT]                       \r\n");
                    sql.Append("       ,[vcD3bTShow]                       \r\n");
                    sql.Append("       ,[vcD4yT]                       \r\n");
                    sql.Append("       ,[vcD4yTShow]                       \r\n");
                    sql.Append("       ,[vcD4bT]                       \r\n");
                    sql.Append("       ,[vcD4bTShow]                       \r\n");
                    sql.Append("       ,[vcD5yT]                       \r\n");
                    sql.Append("       ,[vcD5yTShow]                       \r\n");
                    sql.Append("       ,[vcD5bT]                       \r\n");
                    sql.Append("       ,[vcD5bTShow]                       \r\n");
                    sql.Append("       ,[vcD6yT]                       \r\n");
                    sql.Append("       ,[vcD6yTShow]                       \r\n");
                    sql.Append("       ,[vcD6bT]                       \r\n");
                    sql.Append("       ,[vcD6bTShow]                       \r\n");
                    sql.Append("       ,[vcD7yT]                       \r\n");
                    sql.Append("       ,[vcD7yTShow]                       \r\n");
                    sql.Append("       ,[vcD7bT]                       \r\n");
                    sql.Append("       ,[vcD7bTShow]                       \r\n");
                    sql.Append("       ,[vcD8yT]                       \r\n");
                    sql.Append("       ,[vcD8yTShow]                       \r\n");
                    sql.Append("       ,[vcD8bT]                       \r\n");
                    sql.Append("       ,[vcD8bTShow]                       \r\n");
                    sql.Append("       ,[vcD9yT]                       \r\n");
                    sql.Append("       ,[vcD9yTShow]                       \r\n");
                    sql.Append("       ,[vcD9bT]                       \r\n");
                    sql.Append("       ,[vcD9bTShow]                       \r\n");
                    sql.Append("       ,[vcD10yT]                       \r\n");
                    sql.Append("       ,[vcD10yTShow]                       \r\n");
                    sql.Append("       ,[vcD10bT]                       \r\n");
                    sql.Append("       ,[vcD10bTShow]                       \r\n");
                    sql.Append("       ,[vcD11yT]                       \r\n");
                    sql.Append("       ,[vcD11yTShow]                       \r\n");
                    sql.Append("       ,[vcD11bT]                       \r\n");
                    sql.Append("       ,[vcD11bTShow]                       \r\n");
                    sql.Append("       ,[vcD12yT]                       \r\n");
                    sql.Append("       ,[vcD12yTShow]                       \r\n");
                    sql.Append("       ,[vcD12bT]                       \r\n");
                    sql.Append("       ,[vcD12bTShow]                       \r\n");
                    sql.Append("       ,[vcD13yT]                       \r\n");
                    sql.Append("       ,[vcD13yTShow]                       \r\n");
                    sql.Append("       ,[vcD13bT]                       \r\n");
                    sql.Append("       ,[vcD13bTShow]                       \r\n");
                    sql.Append("       ,[vcD14yT]                       \r\n");
                    sql.Append("       ,[vcD14yTShow]                       \r\n");
                    sql.Append("       ,[vcD14bT]                       \r\n");
                    sql.Append("       ,[vcD14bTShow]                       \r\n");
                    sql.Append("       ,[vcD15yT]                       \r\n");
                    sql.Append("       ,[vcD15yTShow]                       \r\n");
                    sql.Append("       ,[vcD15bT]                       \r\n");
                    sql.Append("       ,[vcD15bTShow]                       \r\n");
                    sql.Append("       ,[vcD16yT]                       \r\n");
                    sql.Append("       ,[vcD16yTShow]                       \r\n");
                    sql.Append("       ,[vcD16bT]                       \r\n");
                    sql.Append("       ,[vcD16bTShow]                       \r\n");
                    sql.Append("       ,[vcD17yT]                       \r\n");
                    sql.Append("       ,[vcD17yTShow]                       \r\n");
                    sql.Append("       ,[vcD17bT]                       \r\n");
                    sql.Append("       ,[vcD17bTShow]                       \r\n");
                    sql.Append("       ,[vcD18yT]                       \r\n");
                    sql.Append("       ,[vcD18yTShow]                       \r\n");
                    sql.Append("       ,[vcD18bT]                       \r\n");
                    sql.Append("       ,[vcD18bTShow]                       \r\n");
                    sql.Append("       ,[vcD19yT]                       \r\n");
                    sql.Append("       ,[vcD19yTShow]                       \r\n");
                    sql.Append("       ,[vcD19bT]                       \r\n");
                    sql.Append("       ,[vcD19bTShow]                       \r\n");
                    sql.Append("       ,[vcD20yT]                       \r\n");
                    sql.Append("       ,[vcD20yTShow]                       \r\n");
                    sql.Append("       ,[vcD20bT]                       \r\n");
                    sql.Append("       ,[vcD20bTShow]                       \r\n");
                    sql.Append("       ,[vcD21yT]                       \r\n");
                    sql.Append("       ,[vcD21yTShow]                       \r\n");
                    sql.Append("       ,[vcD21bT]                       \r\n");
                    sql.Append("       ,[vcD21bTShow]                       \r\n");
                    sql.Append("       ,[vcD22yT]                       \r\n");
                    sql.Append("       ,[vcD22yTShow]                       \r\n");
                    sql.Append("       ,[vcD22bT]                       \r\n");
                    sql.Append("       ,[vcD22bTShow]                       \r\n");
                    sql.Append("       ,[vcD23yT]                       \r\n");
                    sql.Append("       ,[vcD23yTShow]                       \r\n");
                    sql.Append("       ,[vcD23bT]                       \r\n");
                    sql.Append("       ,[vcD23bTShow]                       \r\n");
                    sql.Append("       ,[vcD24yT]                       \r\n");
                    sql.Append("       ,[vcD24yTShow]                       \r\n");
                    sql.Append("       ,[vcD24bT]                       \r\n");
                    sql.Append("       ,[vcD24bTShow]                       \r\n");
                    sql.Append("       ,[vcD25yT]                       \r\n");
                    sql.Append("       ,[vcD25yTShow]                       \r\n");
                    sql.Append("       ,[vcD25bT]                       \r\n");
                    sql.Append("       ,[vcD25bTShow]                       \r\n");
                    sql.Append("       ,[vcD26yT]                       \r\n");
                    sql.Append("       ,[vcD26yTShow]                       \r\n");
                    sql.Append("       ,[vcD26bT]                       \r\n");
                    sql.Append("       ,[vcD26bTShow]                       \r\n");
                    sql.Append("       ,[vcD27yT]                       \r\n");
                    sql.Append("       ,[vcD27yTShow]                       \r\n");
                    sql.Append("       ,[vcD27bT]                       \r\n");
                    sql.Append("       ,[vcD27bTShow]                       \r\n");
                    sql.Append("       ,[vcD28yT]                       \r\n");
                    sql.Append("       ,[vcD28yTShow]                       \r\n");
                    sql.Append("       ,[vcD28bT]                       \r\n");
                    sql.Append("       ,[vcD28bTShow]                       \r\n");
                    sql.Append("       ,[vcD29yT]                       \r\n");
                    sql.Append("       ,[vcD29yTShow]                       \r\n");
                    sql.Append("       ,[vcD29bT]                       \r\n");
                    sql.Append("       ,[vcD29bTShow]                       \r\n");
                    sql.Append("       ,[vcD30yT]                       \r\n");
                    sql.Append("       ,[vcD30yTShow]                       \r\n");
                    sql.Append("       ,[vcD30bT]                       \r\n");
                    sql.Append("       ,[vcD30bTShow]                       \r\n");
                    sql.Append("       ,[vcD31yT]                       \r\n");
                    sql.Append("       ,[vcD31yTShow]                       \r\n");
                    sql.Append("       ,[vcD31bT]                       \r\n");
                    sql.Append("       ,[vcD31bTShow]                       \r\n");
                    sql.AppendLine("           ,[vcIsorNoSend]    \n");
                    sql.AppendLine("           ,[vcIsorNoPrint]    \n");
                    sql.Append("       ,[vcOperatorID]                       \r\n");
                    sql.Append("       ,[dOperatorTime])                       \r\n");
                    sql.Append("       VALUES                       \r\n");
                    sql.Append("       (         \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcPartsNo"].ToString() + "',         \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcPackNo"].ToString() + "',            \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcPackGPSNo"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcSupplierCode"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcSupplierName"].ToString() + "',              \r\n");

                    sql.Append("       '" + listInfoData.Rows[i]["iRelease"].ToString() + "',          \r\n");

                    #region
                    sql.Append("       '" + listInfoData.Rows[i]["vcD1yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD1yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD1bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD1bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD2yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD2yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD2bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD2bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD3yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD3yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD3bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD3bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD4yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD4yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD4bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD4bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD5yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD5yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD5bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD5bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD6yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD6yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD6bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD6bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD7yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD7yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD7bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD7bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD8yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD8yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD8bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD8bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD9yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD9yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD9bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD9bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD10yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD10yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD10bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD10bFShow"].ToString() + "',          \r\n");

                    sql.Append("       '" + listInfoData.Rows[i]["vcD11yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD11yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD11bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD11bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD12yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD12yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD12bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD12bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD13yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD13yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD13bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD13bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD14yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD14yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD14bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD14bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD15yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD15yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD15bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD15bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD16yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD16yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD16bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD16bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD17yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD17yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD17bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD17bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD18yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD18yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD18bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD18bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD19yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD19yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD19bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD19bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD20yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD20yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD20bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD20bFShow"].ToString() + "',          \r\n");

                    sql.Append("       '" + listInfoData.Rows[i]["vcD21yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD21yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD21bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD21bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD22yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD22yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD22bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD22bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD23yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD23yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD23bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD23bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD24yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD24yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD24bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD24bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD25yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD25yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD25bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD25bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD26yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD26yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD26bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD26bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD27yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD27yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD27bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD27bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD28yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD28yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD28bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD28bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD29yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD29yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD29bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD29bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD30yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD30yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD30bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD30bFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD31yF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD31yFShow"].ToString() + "',          \r\n");
                    sql.Append("       '" + listInfoData.Rows[i]["vcD31bF"].ToString() + "',              \r\n");
                    sql.Append("       '" + listInfoData.Rows[0]["vcD31bFShow"].ToString() + "',          \r\n");
                    #endregion

                    if (strBegin.Split("-")[1] != strEnd.Split("-")[1])
                    {
                        #region
                        sql.Append("       '" + listInfoData.Rows[i]["vcD1yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD1yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD1bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD1bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD2yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD2yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD2bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD2bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD3yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD3yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD3bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD3bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD4yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD4yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD4bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD4bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD5yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD5yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD5bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD5bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD6yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD6yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD6bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD6bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD7yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD7yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD7bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD7bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD8yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD8yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD8bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD8bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD9yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD9yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD9bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD9bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD10yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD10yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD10bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD10bTShow"].ToString() + "',          \r\n");

                        sql.Append("       '" + listInfoData.Rows[i]["vcD11yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD11yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD11bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD11bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD12yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD12yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD12bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD12bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD13yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD13yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD13bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD13bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD14yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD14yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD14bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD14bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD15yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD15yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD15bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD15bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD16yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD16yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD16bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD16bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD17yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD17yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD17bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD17bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD18yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD18yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD18bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD18bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD19yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD19yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD19bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD19bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD20yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD20yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD20bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD20bTShow"].ToString() + "',          \r\n");

                        sql.Append("       '" + listInfoData.Rows[i]["vcD21yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD21yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD21bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD21bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD22yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD22yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD22bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD22bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD23yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD23yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD23bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD23bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD24yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD24yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD24bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD24bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD25yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD25yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD25bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD25bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD26yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD26yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD26bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD26bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD27yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD27yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD27bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD27bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD28yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD28yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD28bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD28bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD29yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD29yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD29bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD29bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD30yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD30yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD30bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD30bTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD31yT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD31yTShow"].ToString() + "',          \r\n");
                        sql.Append("       '" + listInfoData.Rows[i]["vcD31bT"].ToString() + "',              \r\n");
                        sql.Append("       '" + listInfoData.Rows[0]["vcD31bTShow"].ToString() + "',          \r\n");
                        #endregion
                    }
                    else
                    {
                        #region
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',         \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");

                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");

                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        sql.Append("       '',          \r\n");
                        #endregion

                    }

                    sql.Append("       '0' ,                 \r\n");
                    sql.Append("      '0' ,                 \r\n");
                    sql.Append("    '" + strUserId + "'   ,                 \r\n");
                    sql.Append("      getdate()                   \r\n");
                    sql.Append("   )                       \r\n");


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

        public DataTable Search()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select ''as vcNum, vcPackNo,vcPackGPSNo,vcSupplierName,vcSupplierCode,         \n");
                strSql.Append("                         \n");
                strSql.Append("    vcD1bFShow,vcD1yFShow,vcD2bFShow,vcD2yFShow,vcD3bFShow,vcD3yFShow,vcD4bFShow, vcD4yFShow,vcD5bFShow, vcD5yFShow,vcD6bFShow ,vcD6yFShow  \n");
                strSql.Append("    ,vcD7bFShow,vcD7yFShow,vcD8bFShow,vcD8yFShow,vcD9bFShow, vcD9yFShow,vcD10bFShow,  vcD10yFShow,                       \n");
                strSql.Append("    vcD11bFShow, vcD11yFShow,vcD12bFShow,vcD12yFShow,vcD13bFShow, vcD13yFShow,vcD14bFShow,vcD14yFShow,vcD15bFShow,vcD15yFShow,vcD16bFShow,vcD16yFShow                \n");
                strSql.Append("    ,vcD17bFShow,vcD17yFShow,vcD18bFShow,vcD18yFShow,vcD19bFShow,vcD19yFShow,vcD20bFShow,vcD20yFShow,                         \n");
                strSql.Append("    vcD21bFShow, vcD21yFShow,vcD22bFShow,vcD22yFShow,vcD23bFShow,  vcD23yFShow,vcD24bFShow,vcD24yFShow,vcD25bFShow,vcD25yFShow,vcD26bFShow,vcD26yFShow               \n");
                strSql.Append("    ,vcD27bFShow,vcD27yFShow,vcD28bFShow,vcD28yFShow,vcD29bFShow,vcD29yFShow,vcD30bFShow,vcD30yFShow,vcD31bFShow, vcD31yFShow,            \n");
                strSql.Append("                     \n");
                strSql.Append("   vcD1bTShow,vcD1yTShow,vcD2bTShow,vcD2yTShow,vcD3bTShow, vcD3yTShow,vcD4bTShow,vcD4yTShow,vcD5bTShow, vcD5yTShow,vcD6bTShow ,vcD6yTShow                       \n");

                strSql.Append("   ,vcD7bTShow,vcD7yTShow,vcD8bTShow,vcD8yTShow,vcD9bTShow,vcD9yTShow,vcD10bTShow,vcD10yTShow,                      \n");

                strSql.Append("   vcD11bTShow,vcD11yTShow,vcD12bTShow,vcD12yTShow,vcD13bTShow,vcD13yTShow,vcD14bTShow,vcD14yTShow,vcD15bTShow, vcD15yTShow,vcD16bTShow, vcD16yTShow               \n");
                strSql.Append("   ,vcD17bTShow,vcD17yTShow,vcD18bTShow,vcD18yTShow,vcD19bTShow,vcD19yTShow,vcD20bTShow,vcD20yTShow,                       \n");

                strSql.Append("   vcD21bTShow,vcD21yTShow,vcD22bTShow, vcD22yTShow,vcD23bTShow,vcD23yTShow,vcD24bTShow, vcD24yTShow,vcD25bTShow,vcD25yTShow,vcD26bTShow,vcD26yTShow                      \n");
                strSql.Append("   ,vcD27bTShow,vcD27yTShow,vcD28bTShow,vcD28yTShow,vcD29bTShow,vcD29yTShow,vcD30bTShow,vcD30yTShow,vcD31bTShow,  vcD31yTShow,          \n");
                strSql.Append("             \n");

                strSql.Append("  sum(iRelease) as iRelease ,            \n");
                strSql.Append("  sum(vcD1yF) as vcD1yF,sum(vcD2yF) as vcD2yF,            \n");
                strSql.Append("  sum(vcD3yF) as vcD3yF,sum(vcD4yF) as vcD4yF,            \n");
                strSql.Append("  sum(vcD5yF) as vcD5yF,sum(vcD6yF) as vcD6yF,            \n");
                strSql.Append("  sum(vcD7yF) as vcD7yF,sum(vcD8yF) as vcD8yF,            \n");
                strSql.Append("  sum(vcD9yF) as vcD9yF,sum(vcD10yF) as vcD10yF,            \n");
                strSql.Append("  sum(vcD11yF) as vcD11yF,sum(vcD12yF) as vcD12yF,            \n");
                strSql.Append("  sum(vcD13yF) as vcD13yF,sum(vcD14yF) as vcD14yF,            \n");
                strSql.Append("  sum(vcD15yF) as vcD15yF,sum(vcD16yF) as vcD16yF,            \n");
                strSql.Append("  sum(vcD17yF) as vcD17yF,sum(vcD18yF) as vcD18yF,            \n");
                strSql.Append("  sum(vcD19yF) as vcD19yF,sum(vcD20yF) as vcD20yF,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD21yF) as vcD21yF,sum(vcD22yF) as vcD22yF,            \n");
                strSql.Append("  sum(vcD23yF) as vcD23yF,sum(vcD24yF) as vcD24yF,            \n");
                strSql.Append("  sum(vcD25yF) as vcD25yF,sum(vcD26yF) as vcD26yF,            \n");
                strSql.Append("  sum(vcD27yF) as vcD27yF,sum(vcD28yF) as vcD28yF,            \n");
                strSql.Append("  sum(vcD29yF) as vcD29yF,sum(vcD30yF) as vcD30yF,sum(vcD31yF) as vcD31yF,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD1bF) as vcD1bF,sum(vcD2bF) as vcD2bF,            \n");
                strSql.Append("  sum(vcD3bF) as vcD3bF,sum(vcD4bF) as vcD4bF,            \n");
                strSql.Append("  sum(vcD5bF) as vcD5bF,sum(vcD6bF) as vcD6bF,            \n");
                strSql.Append("  sum(vcD7bF) as vcD7bF,sum(vcD8bF) as vcD8bF,            \n");
                strSql.Append("  sum(vcD9bF) as vcD9bF,sum(vcD10bF) as vcD10bF,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD11bF) as vcD11bF,sum(vcD12bF) as vcD12bF,            \n");
                strSql.Append("  sum(vcD13bF) as vcD13bF,sum(vcD14bF) as vcD14bF,            \n");
                strSql.Append("  sum(vcD15bF) as vcD15bF,sum(vcD16bF) as vcD16bF,            \n");
                strSql.Append("  sum(vcD17bF) as vcD17bF,sum(vcD18bF) as vcD18bF,            \n");
                strSql.Append("  sum(vcD19bF) as vcD19bF,sum(vcD20bF) as vcD20bF,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD21bF) as vcD21bF,sum(vcD22bF) as vcD22bF,            \n");
                strSql.Append("  sum(vcD23bF) as vcD23bF,sum(vcD24bF) as vcD24bF,            \n");
                strSql.Append("  sum(vcD25bF) as vcD25bF,sum(vcD26bF) as vcD26bF,            \n");
                strSql.Append("  sum(vcD27bF) as vcD27bF,sum(vcD28bF) as vcD28bF,            \n");
                strSql.Append("  sum(vcD29bF) as vcD29bF,sum(vcD30bF) as vcD30bF,sum(vcD31bF) as vcD31bF,            \n");
                strSql.Append("  sum(vcD1yT) as vcD1yT,sum(vcD2yT) as vcD2yT,            \n");
                strSql.Append("  sum(vcD3yT) as vcD3yT,sum(vcD4yT) as vcD4yT,            \n");
                strSql.Append("  sum(vcD5yT) as vcD5yT,sum(vcD6yT) as vcD6yT,            \n");
                strSql.Append("  sum(vcD7yT) as vcD7yT,sum(vcD8yT) as vcD8yT,            \n");
                strSql.Append("  sum(vcD9yT) as vcD9yT,sum(vcD10yT) as vcD10yT,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD11yT) as vcD11yT,sum(vcD12yT) as vcD12yT,            \n");
                strSql.Append("  sum(vcD13yT) as vcD13yT,sum(vcD14yT) as vcD14yT,            \n");
                strSql.Append("  sum(vcD15yT) as vcD15yT,sum(vcD16yT) as vcD16yT,            \n");
                strSql.Append("  sum(vcD17yT) as vcD17yT,sum(vcD18yT) as vcD18yT,            \n");
                strSql.Append("  sum(vcD19yT) as vcD19yT,sum(vcD20yT) as vcD20yT,            \n");
                strSql.Append("  sum(vcD21yT) as vcD21yT,sum(vcD22yT) as vcD22yT,            \n");
                strSql.Append("  sum(vcD23yT) as vcD23yT,sum(vcD24yT) as vcD24yT,            \n");
                strSql.Append("  sum(vcD25yT) as vcD25yT,sum(vcD26yT) as vcD26yT,            \n");
                strSql.Append("  sum(vcD27yT) as vcD27yT,sum(vcD28yT) as vcD28yT,            \n");
                strSql.Append("  sum(vcD29yT) as vcD29yT,sum(vcD30yT) as vcD30yT,sum(vcD31yT) as vcD31yT,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD1bT) as vcD1bT,sum(vcD2bT) as vcD2bT,            \n");
                strSql.Append("  sum(vcD3bT) as vcD3bT,sum(vcD4bT) as vcD4bT,            \n");
                strSql.Append("  sum(vcD5bT) as vcD5bT,sum(vcD6bT) as vcD6bT,            \n");
                strSql.Append("  sum(vcD7bT) as vcD7bT,sum(vcD8bT) as vcD8bT,            \n");
                strSql.Append("  sum(vcD9bT) as vcD9bT,sum(vcD10bT) as vcD10bT,            \n");
                strSql.Append("  sum(vcD11bT) as vcD11bT,sum(vcD12bT) as vcD12bT,            \n");
                strSql.Append("  sum(vcD13bT) as vcD13bT,sum(vcD14bT) as vcD14bT,            \n");
                strSql.Append("  sum(vcD15bT) as vcD15bT,sum(vcD16bT) as vcD16bT,            \n");
                strSql.Append("  sum(vcD17bT) as vcD17bT,sum(vcD18bT) as vcD18bT,            \n");
                strSql.Append("  sum(vcD19bT) as vcD19bT,sum(vcD20bT) as vcD20bT,            \n");
                strSql.Append("              \n");
                strSql.Append("  sum(vcD21bT) as vcD21bT,sum(vcD22bT) as vcD22bT,            \n");
                strSql.Append("  sum(vcD23bT) as vcD23bT,sum(vcD24bT) as vcD24bT,            \n");
                strSql.Append("  sum(vcD25bT) as vcD25bT,sum(vcD26bT) as vcD26bT,            \n");
                strSql.Append("  sum(vcD27bT) as vcD27bT,sum(vcD28bT) as vcD28bT,            \n");
                strSql.Append("  sum(vcD29bT) as vcD29bT,sum(vcD30bT) as vcD30bT,sum(vcD31bT) as vcD31bT ,vcIsorNoPrint           \n");
                strSql.Append("              \n");
                strSql.Append("   from TPackWeekInfo         \n");
                strSql.Append("   group by vcPackNo,vcPackGPSNo,vcSupplierName, vcSupplierCode,           \n");

                strSql.Append("   vcD1yFShow,vcD2yFShow,vcD3yFShow,vcD4yFShow,vcD5yFShow,vcD6yFShow                  \n");
                strSql.Append("    ,vcD7yFShow,vcD8yFShow,vcD9yFShow,vcD10yFShow,                    \n");
                strSql.Append("    vcD11yFShow,vcD12yFShow,vcD13yFShow,vcD14yFShow,vcD15yFShow,vcD16yFShow           \n");
                strSql.Append("    ,vcD17yFShow,vcD18yFShow,vcD19yFShow,vcD20yFShow,                    \n");
                strSql.Append("    vcD21yFShow,vcD22yFShow,vcD23yFShow,vcD24yFShow,vcD25yFShow,vcD26yFShow           \n");
                strSql.Append("    ,vcD27yFShow,vcD28yFShow,vcD29yFShow,vcD30yFShow,vcD31yFShow,                    \n");
                strSql.Append("                        \n");
                strSql.Append("    vcD1bFShow,vcD2bFShow,vcD3bFShow,vcD4bFShow,vcD5bFShow,vcD6bFShow		         \n");
                strSql.Append("    ,vcD7bFShow,vcD8bFShow,vcD9bFShow,vcD10bFShow,                    \n");
                strSql.Append("    vcD11bFShow,vcD12bFShow,vcD13bFShow,vcD14bFShow,vcD15bFShow,vcD16bFShow           \n");
                strSql.Append("    ,vcD17bFShow,vcD18bFShow,vcD19bFShow,vcD20bFShow,                    \n");
                strSql.Append("    vcD21bFShow,vcD22bFShow,vcD23bFShow,vcD24bFShow,vcD25bFShow,vcD26bFShow           \n");
                strSql.Append("    ,vcD27bFShow,vcD28bFShow,vcD29bFShow,vcD30bFShow,vcD31bFShow,          \n");
                strSql.Append("                    \n");
                strSql.Append("   vcD1bTShow,vcD2bTShow,vcD3bTShow,vcD4bTShow,vcD5bTShow,vcD6bTShow                  \n");
                strSql.Append("   ,vcD7bTShow,vcD8bTShow,vcD9bTShow,vcD10bTShow,                     \n");
                strSql.Append("   vcD11bTShow,vcD12bTShow,vcD13bTShow,vcD14bTShow,vcD15bTShow,vcD16bTShow            \n");
                strSql.Append("   ,vcD17bTShow,vcD18bTShow,vcD19bTShow,vcD20bTShow,                     \n");
                strSql.Append("   vcD21bTShow,vcD22bTShow,vcD23bTShow,vcD24bTShow,vcD25bTShow,vcD26bTShow            \n");
                strSql.Append("   ,vcD27bTShow,vcD28bTShow,vcD29bTShow,vcD30bTShow,vcD31bTShow,          \n");
                strSql.Append("            \n");
                strSql.Append("   vcD1yTShow,vcD2yTShow,vcD3yTShow,vcD4yTShow,vcD5yTShow,vcD6yTShow                  \n");
                strSql.Append("   ,vcD7yTShow,vcD8yTShow,vcD9yTShow,vcD10yTShow,                     \n");
                strSql.Append("   vcD11yTShow,vcD12yTShow,vcD13yTShow,vcD14yTShow,vcD15yTShow,vcD16yTShow         \n");
                strSql.Append("   ,vcD17yTShow,vcD18yTShow,vcD19yTShow,vcD20yTShow,                     \n");
                strSql.Append("   vcD21yTShow,vcD22yTShow,vcD23yTShow,vcD24yTShow,vcD25yTShow,vcD26yTShow         \n");
                strSql.Append("   ,vcD27yTShow,vcD28yTShow,vcD29yTShow,vcD30yTShow,vcD31yTShow,vcIsorNoPrint                   \n");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
