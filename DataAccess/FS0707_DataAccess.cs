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
                DateTime dtimet;
                DateTime dtimef;
                int timeup = 0;
                int timenow = 0;
                if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                {
                    //上个月分天数
                    dtimet = Convert.ToDateTime(dFromBegin);
                    timeup = dtimet.AddDays(1 - dtimet.Day).AddMonths(1).AddDays(-1).Day;
                    //这个月
                    dtimef = Convert.ToDateTime(dFromEnd);
                    timenow = dtimef.AddDays(1 - dtimef.Day).AddMonths(1).AddDays(-1).Day;

                }
                else
                {

                    //这个月
                    dtimet = Convert.ToDateTime(dFromBegin);
                    timeup = dtimet.AddDays(1 - dtimet.Day).AddMonths(1).AddDays(-1).Day;
                }



                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    delete from   TPackWeekInfo;    ");

                strSql.AppendLine("    select a.vcPartsno,d.vcPackNo,d.vcPackGPSNo,d.vcSupplierName,d.vcSupplierCode,d.iRelease,        ");
                strSql.AppendLine(" case when '" + (dFtime + "-01") + "'>=dFrom1 and '" + (dFtime + "-01") + "'<=dTo1 and isnull(a.vcD1b,'')<>''  then a.vcD1b*d.iBiYao else 0 end as vcD1bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-02") + "'>=dFrom1 and '" + (dFtime + "-02") + "'<=dTo1 and isnull(a.vcD2b,'')<>''  then  a.vcD2b*d.iBiYao else 0 end as vcD2bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-03") + "'>=dFrom1 and '" + (dFtime + "-03") + "'<=dTo1 and isnull(a.vcD3b,'')<>''  then a.vcD3b*d.iBiYao else 0 end as vcD3bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-04") + "'>=dFrom1 and '" + (dFtime + "-04") + "'<=dTo1 and isnull(a.vcD4b,'')<>''  then a.vcD4b*d.iBiYao else 0 end as vcD4bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-05") + "'>=dFrom1 and '" + (dFtime + "-05") + "'<=dTo1 and isnull(a.vcD5b,'')<>''  then  a.vcD5b*d.iBiYao else 0 end as vcD5bF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-06") + "'>=dFrom1 and '" + (dFtime + "-06") + "'<=dTo1 and isnull(a.vcD6b,'')<>''  then a.vcD6b*d.iBiYao else 0 end as vcD6bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-07") + "'>=dFrom1 and '" + (dFtime + "-07") + "'<=dTo1 and isnull(a.vcD7b,'')<>''  then a.vcD7b*d.iBiYao else 0 end as vcD7bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-08") + "'>=dFrom1 and '" + (dFtime + "-08") + "'<=dTo1 and isnull(a.vcD8b,'')<>''  then a.vcD8b*d.iBiYao else 0 end as vcD8bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-09") + "'>=dFrom1 and '" + (dFtime + "-09") + "'<=dTo1 and isnull(a.vcD9b,'')<>''  then a.vcD9b*d.iBiYao else 0 end as vcD9bF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-10") + "'>=dFrom1 and '" + (dFtime + "-10") + "'<=dTo1 and isnull(a.vcD10b,'')<>'' then a.vcD10b*d.iBiYao else 0 end as vcD10bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-11") + "'>=dFrom1 and '" + (dFtime + "-11") + "'<=dTo1 and isnull(a.vcD11b,'')<>'' then a.vcD11b*d.iBiYao else 0 end as vcD11bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-12") + "'>=dFrom1 and '" + (dFtime + "-12") + "'<=dTo1 and isnull(a.vcD12b,'')<>'' then a.vcD12b*d.iBiYao else 0 end as vcD12bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-13") + "'>=dFrom1 and '" + (dFtime + "-13") + "'<=dTo1 and isnull(a.vcD13b,'')<>'' then a.vcD13b*d.iBiYao else 0 end as vcD13bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-14") + "'>=dFrom1 and '" + (dFtime + "-14") + "'<=dTo1 and isnull(a.vcD14b,'')<>'' then a.vcD14b*d.iBiYao else 0 end as vcD14bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-15") + "'>=dFrom1 and '" + (dFtime + "-15") + "'<=dTo1 and isnull(a.vcD15b,'')<>'' then a.vcD15b*d.iBiYao else 0 end as vcD15bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-16") + "'>=dFrom1 and '" + (dFtime + "-16") + "'<=dTo1 and isnull(a.vcD16b,'')<>'' then a.vcD16b*d.iBiYao else 0 end as vcD16bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-17") + "'>=dFrom1 and '" + (dFtime + "-17") + "'<=dTo1 and isnull(a.vcD17b,'')<>'' then a.vcD17b*d.iBiYao else 0 end as vcD17bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-18") + "'>=dFrom1 and '" + (dFtime + "-18") + "'<=dTo1 and isnull(a.vcD18b,'')<>'' then a.vcD18b*d.iBiYao else 0 end as vcD18bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-19") + "'>=dFrom1 and '" + (dFtime + "-19") + "'<=dTo1 and isnull(a.vcD19b,'')<>'' then a.vcD19b*d.iBiYao else 0 end as vcD19bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-20") + "'>=dFrom1 and '" + (dFtime + "-20") + "'<=dTo1 and isnull(a.vcD20b,'')<>'' then a.vcD20b*d.iBiYao else 0 end as vcD20bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-21") + "'>=dFrom1 and '" + (dFtime + "-21") + "'<=dTo1 and isnull(a.vcD21b,'')<>'' then a.vcD21b*d.iBiYao else 0 end as vcD21bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-22") + "'>=dFrom1 and '" + (dFtime + "-22") + "'<=dTo1 and isnull(a.vcD22b,'')<>'' then a.vcD22b*d.iBiYao else 0 end as vcD22bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-23") + "'>=dFrom1 and '" + (dFtime + "-23") + "'<=dTo1 and isnull(a.vcD23b,'')<>'' then a.vcD23b*d.iBiYao else 0 end as vcD23bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-24") + "'>=dFrom1 and '" + (dFtime + "-24") + "'<=dTo1 and isnull(a.vcD24b,'')<>'' then a.vcD24b*d.iBiYao else 0 end as vcD24bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-25") + "'>=dFrom1 and '" + (dFtime + "-25") + "'<=dTo1 and isnull(a.vcD25b,'')<>'' then a.vcD25b*d.iBiYao else 0 end as vcD25bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-26") + "'>=dFrom1 and '" + (dFtime + "-26") + "'<=dTo1 and isnull(a.vcD26b,'')<>'' then a.vcD26b*d.iBiYao else 0 end as vcD26bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-27") + "'>=dFrom1 and '" + (dFtime + "-27") + "'<=dTo1 and isnull(a.vcD27b,'')<>'' then a.vcD27b*d.iBiYao else 0 end as vcD27bF,         ");
                strSql.AppendLine(" case when '" + (dFtime + "-28") + "'>=dFrom1 and '" + (dFtime + "-28") + "'<=dTo1 and isnull(a.vcD28b,'')<>'' then a.vcD28b*d.iBiYao else 0 end as vcD28bF,         ");

                if (timeup >= 29)
                {
                    strSql.AppendLine(" case when '" + (dFtime + "-29") + "'>=dFrom1 and '" + (dFtime + "-29") + "'<=dTo1 and isnull(a.vcD29b,'')<>'' then a.vcD29b*d.iBiYao else 0 end as vcD29bF,          ");
                }
                else
                {
                    strSql.AppendLine(" 0 as vcD29bF,          ");

                }
                if (timeup >= 30)
                    strSql.AppendLine(" case when '" + (dFtime + "-30") + "'>=dFrom1 and '" + (dFtime + "-30") + "'<=dTo1 and isnull(a.vcD30b,'')<>'' then a.vcD30b*d.iBiYao else 0 end as vcD30bF,          ");
                else
                    strSql.AppendLine(" 0 as vcD30bF,          ");
                if (timeup >= 31)
                    strSql.AppendLine(" case when '" + (dFtime + "-31") + "'>=dFrom1 and '" + (dFtime + "-31") + "'<=dTo1 and isnull(a.vcD31b,'')<>'' then a.vcD31b*d.iBiYao else 0 end as vcD31bF,         ");
                else
                    strSql.AppendLine(" 0 as vcD31bF,          ");

                strSql.AppendLine(" case when '" + (dFtime + "-01") + "'>=dFrom1 and '" + (dFtime + "-01") + "'<=dTo1 and isnull(a.vcD1y,'')<>''  then   a.vcD1y*d.iBiYao else 0 end as vcD1yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-02") + "'>=dFrom1 and '" + (dFtime + "-02") + "'<=dTo1 and isnull(a.vcD2y,'')<>''  then   a.vcD2y*d.iBiYao else 0 end as vcD2yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-03") + "'>=dFrom1 and '" + (dFtime + "-03") + "'<=dTo1 and isnull(a.vcD3y,'')<>''  then   a.vcD3y*d.iBiYao else 0 end as vcD3yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-04") + "'>=dFrom1 and '" + (dFtime + "-04") + "'<=dTo1 and isnull(a.vcD4y,'')<>''  then   a.vcD4y*d.iBiYao else 0 end as vcD4yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-05") + "'>=dFrom1 and '" + (dFtime + "-05") + "'<=dTo1 and isnull(a.vcD5y,'')<>''  then   a.vcD5y*d.iBiYao else 0 end as vcD5yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-06") + "'>=dFrom1 and '" + (dFtime + "-06") + "'<=dTo1 and isnull(a.vcD6y,'')<>''  then   a.vcD6y*d.iBiYao else 0 end as vcD6yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-07") + "'>=dFrom1 and '" + (dFtime + "-07") + "'<=dTo1 and isnull(a.vcD7y,'')<>''  then   a.vcD7y*d.iBiYao else 0 end as vcD7yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-08") + "'>=dFrom1 and '" + (dFtime + "-08") + "'<=dTo1 and isnull(a.vcD8y,'')<>''  then   a.vcD8y*d.iBiYao else 0 end as vcD8yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-09") + "'>=dFrom1 and '" + (dFtime + "-09") + "'<=dTo1 and isnull(a.vcD9y,'')<>''  then   a.vcD9y*d.iBiYao else 0 end as vcD9yF,          ");
                strSql.AppendLine(" case when '" + (dFtime + "-10") + "'>=dFrom1 and '" + (dFtime + "-10") + "'<=dTo1 and isnull(a.vcD10y,'')<>'' then   a.vcD10y*d.iBiYao else 0 end as vcD10yF,             ");
                strSql.AppendLine(" case when '" + (dFtime + "-11") + "'>=dFrom1 and '" + (dFtime + "-11") + "'<=dTo1 and isnull(a.vcD11y,'')<>'' then   a.vcD11y*d.iBiYao else 0 end as vcD11yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-12") + "'>=dFrom1 and '" + (dFtime + "-12") + "'<=dTo1 and isnull(a.vcD12y,'')<>'' then   a.vcD12y*d.iBiYao else 0 end as vcD12yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-13") + "'>=dFrom1 and '" + (dFtime + "-13") + "'<=dTo1 and isnull(a.vcD13y,'')<>'' then   a.vcD13y*d.iBiYao else 0 end as vcD13yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-14") + "'>=dFrom1 and '" + (dFtime + "-14") + "'<=dTo1 and isnull(a.vcD14y,'')<>'' then   a.vcD14y*d.iBiYao else 0 end as vcD14yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-15") + "'>=dFrom1 and '" + (dFtime + "-15") + "'<=dTo1 and isnull(a.vcD15y,'')<>'' then   a.vcD15y*d.iBiYao else 0 end as vcD15yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-16") + "'>=dFrom1 and '" + (dFtime + "-16") + "'<=dTo1 and isnull(a.vcD16y,'')<>'' then   a.vcD16y*d.iBiYao else 0 end as vcD16yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-17") + "'>=dFrom1 and '" + (dFtime + "-17") + "'<=dTo1 and isnull(a.vcD17y,'')<>'' then   a.vcD17y*d.iBiYao else 0 end as vcD17yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-18") + "'>=dFrom1 and '" + (dFtime + "-18") + "'<=dTo1 and isnull(a.vcD18y,'')<>'' then   a.vcD18y*d.iBiYao else 0 end as vcD18yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-19") + "'>=dFrom1 and '" + (dFtime + "-19") + "'<=dTo1 and isnull(a.vcD19y,'')<>'' then   a.vcD19y*d.iBiYao else 0 end as vcD19yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-20") + "'>=dFrom1 and '" + (dFtime + "-20") + "'<=dTo1 and isnull(a.vcD20y,'')<>'' then   a.vcD20y*d.iBiYao else 0 end as vcD20yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-21") + "'>=dFrom1 and '" + (dFtime + "-21") + "'<=dTo1 and isnull(a.vcD21y,'')<>'' then   a.vcD21y*d.iBiYao else 0 end as vcD21yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-22") + "'>=dFrom1 and '" + (dFtime + "-22") + "'<=dTo1 and isnull(a.vcD22y,'')<>'' then   a.vcD22y*d.iBiYao else 0 end as vcD22yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-23") + "'>=dFrom1 and '" + (dFtime + "-23") + "'<=dTo1 and isnull(a.vcD23y,'')<>'' then   a.vcD23y*d.iBiYao else 0 end as vcD23yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-24") + "'>=dFrom1 and '" + (dFtime + "-24") + "'<=dTo1 and isnull(a.vcD24y,'')<>'' then   a.vcD24y*d.iBiYao else 0 end as vcD24yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-25") + "'>=dFrom1 and '" + (dFtime + "-25") + "'<=dTo1 and isnull(a.vcD25y,'')<>'' then   a.vcD25y*d.iBiYao else 0 end as vcD25yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-26") + "'>=dFrom1 and '" + (dFtime + "-26") + "'<=dTo1 and isnull(a.vcD26y,'')<>'' then   a.vcD26y*d.iBiYao else 0 end as vcD26yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-27") + "'>=dFrom1 and '" + (dFtime + "-27") + "'<=dTo1 and isnull(a.vcD27y,'')<>'' then   a.vcD27y*d.iBiYao else 0 end as vcD27yF,            ");
                strSql.AppendLine(" case when '" + (dFtime + "-28") + "'>=dFrom1 and '" + (dFtime + "-28") + "'<=dTo1 and isnull(a.vcD28y,'')<>'' then   a.vcD28y*d.iBiYao else 0 end as vcD28yF,            ");

                if (timeup >= 29)
                {
                    strSql.AppendLine(" case when '" + (dFtime + "-29") + "'>=dFrom1 and '" + (dFtime + "-29") + "'<=dTo1 and isnull(a.vcD29y,'')<>'' then a.vcD29y*d.iBiYao else 0 end as vcD29yF,          ");
                }
                else
                {
                    strSql.AppendLine(" 0 as vcD29yF,          ");

                }
                if (timeup >= 30)
                    strSql.AppendLine(" case when '" + (dFtime + "-30") + "'>=dFrom1 and '" + (dFtime + "-30") + "'<=dTo1 and isnull(a.vcD30y,'')<>'' then a.vcD30y*d.iBiYao else 0 end as vcD30yF,          ");
                else
                    strSql.AppendLine(" 0 as vcD30yF,          ");
                if (timeup >= 31)
                    strSql.AppendLine(" case when '" + (dFtime + "-31") + "'>=dFrom1 and '" + (dFtime + "-31") + "'<=dTo1 and isnull(a.vcD31y,'')<>'' then a.vcD31y*d.iBiYao else 0 end as vcD31yF,         ");
                else
                    strSql.AppendLine(" 0 as vcD31yF,          ");




                strSql.AppendLine("             ");
                strSql.AppendLine("   '' as vcD1bFShow ,  '' as vcD1yFShow,            ");
                strSql.AppendLine("   '' as vcD2bFShow ,  '' as vcD2yFShow,            ");
                strSql.AppendLine("   '' as vcD3bFShow ,  '' as vcD3yFShow,            ");
                strSql.AppendLine("   '' as vcD4bFShow ,  '' as vcD4yFShow,            ");
                strSql.AppendLine("   '' as vcD5bFShow ,  '' as vcD5yFShow,            ");
                strSql.AppendLine("   '' as vcD6bFShow ,  '' as vcD6yFShow,            ");
                strSql.AppendLine("   '' as vcD7bFShow ,  '' as vcD7yFShow,            ");
                strSql.AppendLine("   '' as vcD8bFShow ,  '' as vcD8yFShow,            ");
                strSql.AppendLine("   '' as vcD9bFShow ,  '' as vcD9yFShow,            ");
                strSql.AppendLine("  '' as vcD10bFShow , '' as vcD10yFShow ,            ");
                strSql.AppendLine("  '' as vcD11bFShow , '' as vcD11yFShow ,            ");
                strSql.AppendLine("  '' as vcD12bFShow , '' as vcD12yFShow ,            ");
                strSql.AppendLine("  '' as vcD13bFShow , '' as vcD13yFShow ,            ");
                strSql.AppendLine("  '' as vcD14bFShow , '' as vcD14yFShow ,            ");
                strSql.AppendLine("  '' as vcD15bFShow , '' as vcD15yFShow ,            ");
                strSql.AppendLine("  '' as vcD16bFShow , '' as vcD16yFShow ,            ");
                strSql.AppendLine("  '' as vcD17bFShow , '' as vcD17yFShow ,            ");
                strSql.AppendLine("  '' as vcD18bFShow , '' as vcD18yFShow ,            ");
                strSql.AppendLine("  '' as vcD19bFShow , '' as vcD19yFShow ,            ");
                strSql.AppendLine("  '' as vcD20bFShow , '' as vcD20yFShow ,            ");
                strSql.AppendLine("  '' as vcD21bFShow , '' as vcD21yFShow ,            ");
                strSql.AppendLine("  '' as vcD22bFShow , '' as vcD22yFShow ,            ");
                strSql.AppendLine("  '' as vcD23bFShow , '' as vcD23yFShow ,            ");
                strSql.AppendLine("  '' as vcD24bFShow , '' as vcD24yFShow ,            ");
                strSql.AppendLine("  '' as vcD25bFShow , '' as vcD25yFShow ,            ");
                strSql.AppendLine("  '' as vcD26bFShow , '' as vcD26yFShow ,            ");
                strSql.AppendLine("  '' as vcD27bFShow , '' as vcD27yFShow ,            ");
                strSql.AppendLine("  '' as vcD28bFShow , '' as vcD28yFShow ,            ");
                strSql.AppendLine("  '' as vcD29bFShow , '' as vcD29yFShow ,            ");
                strSql.AppendLine("  '' as vcD30bFShow , '' as vcD30yFShow ,            ");
                strSql.AppendLine("  '' as vcD31bFShow , '' as vcD31yFShow              ");






                if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                {
                    strSql.AppendLine(",case when '" + (dTtime + "-01") + "'>=dFrom1 and '" + (dTtime + "-01") + "'<=dTo1 and isnull(c.vcD1b,'')<>''  then c.vcD1b*d.iBiYao  else 0 end as vcD1bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-02") + "'>=dFrom1 and '" + (dTtime + "-02") + "'<=dTo1 and isnull(c.vcD2b,'')<>''  then c.vcD2b*d.iBiYao  else 0 end as vcD2bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-03") + "'>=dFrom1 and '" + (dTtime + "-03") + "'<=dTo1 and isnull(c.vcD3b,'')<>''  then c.vcD3b*d.iBiYao  else 0 end as vcD3bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-04") + "'>=dFrom1 and '" + (dTtime + "-04") + "'<=dTo1 and isnull(c.vcD4b,'')<>''  then c.vcD4b*d.iBiYao  else 0 end as vcD4bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-05") + "'>=dFrom1 and '" + (dTtime + "-05") + "'<=dTo1 and isnull(c.vcD5b,'')<>''  then c.vcD5b*d.iBiYao  else 0 end as vcD5bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-06") + "'>=dFrom1 and '" + (dTtime + "-06") + "'<=dTo1 and isnull(c.vcD6b,'')<>''  then c.vcD6b*d.iBiYao  else 0 end as vcD6bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-07") + "'>=dFrom1 and '" + (dTtime + "-07") + "'<=dTo1 and isnull(c.vcD7b,'')<>''  then c.vcD7b*d.iBiYao  else 0 end as vcD7bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-08") + "'>=dFrom1 and '" + (dTtime + "-08") + "'<=dTo1 and isnull(c.vcD8b,'')<>''  then c.vcD8b*d.iBiYao  else 0 end as vcD8bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-09") + "'>=dFrom1 and '" + (dTtime + "-09") + "'<=dTo1 and isnull(c.vcD9b,'')<>''  then c.vcD9b*d.iBiYao  else 0 end as vcD9bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-10") + "'>=dFrom1 and '" + (dTtime + "-10") + "'<=dTo1 and isnull(c.vcD10b,'')<>'' then c.vcD10b*d.iBiYao  else 0 end as vcD10bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-11") + "'>=dFrom1 and '" + (dTtime + "-11") + "'<=dTo1 and isnull(c.vcD11b,'')<>'' then c.vcD11b*d.iBiYao  else 0 end as vcD11bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-12") + "'>=dFrom1 and '" + (dTtime + "-12") + "'<=dTo1 and isnull(c.vcD12b,'')<>'' then c.vcD12b*d.iBiYao  else 0 end as vcD12bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-13") + "'>=dFrom1 and '" + (dTtime + "-13") + "'<=dTo1 and isnull(c.vcD13b,'')<>'' then c.vcD13b*d.iBiYao  else 0 end as vcD13bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-14") + "'>=dFrom1 and '" + (dTtime + "-14") + "'<=dTo1 and isnull(c.vcD14b,'')<>'' then c.vcD14b*d.iBiYao  else 0 end as vcD14bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-15") + "'>=dFrom1 and '" + (dTtime + "-15") + "'<=dTo1 and isnull(c.vcD15b,'')<>'' then c.vcD15b*d.iBiYao  else 0 end as vcD15bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-16") + "'>=dFrom1 and '" + (dTtime + "-16") + "'<=dTo1 and isnull(c.vcD16b,'')<>'' then c.vcD16b*d.iBiYao  else 0 end as vcD16bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-17") + "'>=dFrom1 and '" + (dTtime + "-17") + "'<=dTo1 and isnull(c.vcD17b,'')<>'' then c.vcD17b*d.iBiYao  else 0 end as vcD17bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-18") + "'>=dFrom1 and '" + (dTtime + "-18") + "'<=dTo1 and isnull(c.vcD18b,'')<>'' then c.vcD18b*d.iBiYao  else 0 end as vcD18bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-19") + "'>=dFrom1 and '" + (dTtime + "-19") + "'<=dTo1 and isnull(c.vcD19b,'')<>'' then c.vcD19b*d.iBiYao  else 0 end as vcD19bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-20") + "'>=dFrom1 and '" + (dTtime + "-20") + "'<=dTo1 and isnull(c.vcD20b,'')<>'' then c.vcD20b*d.iBiYao  else 0 end as vcD20bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-21") + "'>=dFrom1 and '" + (dTtime + "-21") + "'<=dTo1 and isnull(c.vcD21b,'')<>'' then c.vcD21b*d.iBiYao  else 0 end as vcD21bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-22") + "'>=dFrom1 and '" + (dTtime + "-22") + "'<=dTo1 and isnull(c.vcD22b,'')<>'' then c.vcD22b*d.iBiYao  else 0 end as vcD22bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-23") + "'>=dFrom1 and '" + (dTtime + "-23") + "'<=dTo1 and isnull(c.vcD23b,'')<>'' then c.vcD23b*d.iBiYao  else 0 end as vcD23bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-24") + "'>=dFrom1 and '" + (dTtime + "-24") + "'<=dTo1 and isnull(c.vcD24b,'')<>'' then c.vcD24b*d.iBiYao  else 0 end as vcD24bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-25") + "'>=dFrom1 and '" + (dTtime + "-25") + "'<=dTo1 and isnull(c.vcD25b,'')<>'' then c.vcD25b*d.iBiYao  else 0 end as vcD25bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-26") + "'>=dFrom1 and '" + (dTtime + "-26") + "'<=dTo1 and isnull(c.vcD26b,'')<>'' then c.vcD26b*d.iBiYao  else 0 end as vcD26bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-27") + "'>=dFrom1 and '" + (dTtime + "-27") + "'<=dTo1 and isnull(c.vcD27b,'')<>'' then c.vcD27b*d.iBiYao  else 0 end as vcD27bT,             ");
                    strSql.AppendLine(" case when '" + (dTtime + "-28") + "'>=dFrom1 and '" + (dTtime + "-28") + "'<=dTo1 and isnull(c.vcD28b,'')<>'' then c.vcD28b*d.iBiYao  else 0 end as vcD28bT,             ");

                    if (timenow >= 29)
                    {
                        strSql.AppendLine(" case when '" + (dTtime + "-29") + "'>=dFrom1 and '" + (dTtime + "-29") + "'<=dTo1 and isnull(c.vcD29b,'')<>'' then c.vcD29b*d.iBiYao else 0 end as vcD29bT,          ");
                    }
                    else
                    {
                        strSql.AppendLine(" 0 as vcD29bT,          ");

                    }
                    if (timenow >= 30)
                        strSql.AppendLine(" case when '" + (dTtime + "-30") + "'>=dFrom1 and '" + (dTtime + "-30") + "'<=dTo1 and isnull(c.vcD30b,'')<>'' then c.vcD30b*d.iBiYao else 0 end as vcD30bT,          ");
                    else
                        strSql.AppendLine(" 0 as vcD30bT,          ");
                    if (timenow >= 31)
                        strSql.AppendLine(" case when '" + (dTtime + "-31") + "'>=dFrom1 and '" + (dTtime + "-31") + "'<=dTo1 and isnull(c.vcD31b,'')<>'' then c.vcD31b*d.iBiYao else 0 end as vcD31bT,         ");
                    else
                        strSql.AppendLine(" 0 as vcD31bT,          ");


                    strSql.AppendLine(" case when '" + (dTtime + "-01") + "'>=dFrom1 and '" + (dTtime + "-01") + "'<=dTo1 and isnull( c.vcD1y,'')<>'' then  c.vcD1y*d.iBiYao else 0 end as vcD1yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-02") + "'>=dFrom1 and '" + (dTtime + "-02") + "'<=dTo1 and isnull( c.vcD2y,'')<>'' then  c.vcD2y*d.iBiYao else 0 end as vcD2yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-03") + "'>=dFrom1 and '" + (dTtime + "-03") + "'<=dTo1 and isnull( c.vcD3y,'')<>'' then  c.vcD3y*d.iBiYao else 0 end as vcD3yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-04") + "'>=dFrom1 and '" + (dTtime + "-04") + "'<=dTo1 and isnull( c.vcD4y,'')<>'' then  c.vcD4y*d.iBiYao else 0 end as vcD4yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-05") + "'>=dFrom1 and '" + (dTtime + "-05") + "'<=dTo1 and isnull( c.vcD5y,'')<>'' then  c.vcD5y*d.iBiYao else 0 end as vcD5yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-06") + "'>=dFrom1 and '" + (dTtime + "-06") + "'<=dTo1 and isnull( c.vcD6y,'')<>'' then  c.vcD6y*d.iBiYao else 0 end as vcD6yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-07") + "'>=dFrom1 and '" + (dTtime + "-07") + "'<=dTo1 and isnull( c.vcD7y,'')<>'' then  c.vcD7y*d.iBiYao else 0 end as vcD7yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-08") + "'>=dFrom1 and '" + (dTtime + "-08") + "'<=dTo1 and isnull( c.vcD8y,'')<>'' then  c.vcD8y*d.iBiYao else 0 end as vcD8yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-09") + "'>=dFrom1 and '" + (dTtime + "-09") + "'<=dTo1 and isnull( c.vcD9y,'')<>'' then  c.vcD9y*d.iBiYao else 0 end as vcD9yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-10") + "'>=dFrom1 and '" + (dTtime + "-10") + "'<=dTo1 and isnull(c.vcD10y,'')<>'' then  c.vcD10y*d.iBiYao else 0 end as vcD10yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-11") + "'>=dFrom1 and '" + (dTtime + "-11") + "'<=dTo1 and isnull(c.vcD11y,'')<>'' then  c.vcD11y*d.iBiYao else 0 end as vcD11yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-12") + "'>=dFrom1 and '" + (dTtime + "-12") + "'<=dTo1 and isnull(c.vcD12y,'')<>'' then  c.vcD12y*d.iBiYao else 0 end as vcD12yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-13") + "'>=dFrom1 and '" + (dTtime + "-13") + "'<=dTo1 and isnull(c.vcD13y,'')<>'' then  c.vcD13y*d.iBiYao else 0 end as vcD13yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-14") + "'>=dFrom1 and '" + (dTtime + "-14") + "'<=dTo1 and isnull(c.vcD14y,'')<>'' then  c.vcD14y*d.iBiYao else 0 end as vcD14yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-15") + "'>=dFrom1 and '" + (dTtime + "-15") + "'<=dTo1 and isnull(c.vcD15y,'')<>'' then  c.vcD15y*d.iBiYao else 0 end as vcD15yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-16") + "'>=dFrom1 and '" + (dTtime + "-16") + "'<=dTo1 and isnull(c.vcD16y,'')<>'' then  c.vcD16y*d.iBiYao else 0 end as vcD16yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-17") + "'>=dFrom1 and '" + (dTtime + "-17") + "'<=dTo1 and isnull(c.vcD17y,'')<>'' then  c.vcD17y*d.iBiYao else 0 end as vcD17yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-18") + "'>=dFrom1 and '" + (dTtime + "-18") + "'<=dTo1 and isnull(c.vcD18y,'')<>'' then  c.vcD18y*d.iBiYao else 0 end as vcD18yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-19") + "'>=dFrom1 and '" + (dTtime + "-19") + "'<=dTo1 and isnull(c.vcD19y,'')<>'' then  c.vcD19y*d.iBiYao else 0 end as vcD19yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-20") + "'>=dFrom1 and '" + (dTtime + "-20") + "'<=dTo1 and isnull(c.vcD20y,'')<>'' then  c.vcD20y*d.iBiYao else 0 end as vcD20yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-21") + "'>=dFrom1 and '" + (dTtime + "-21") + "'<=dTo1 and isnull(c.vcD21y,'')<>'' then  c.vcD21y*d.iBiYao else 0 end as vcD21yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-22") + "'>=dFrom1 and '" + (dTtime + "-22") + "'<=dTo1 and isnull(c.vcD22y,'')<>'' then  c.vcD22y*d.iBiYao else 0 end as vcD22yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-23") + "'>=dFrom1 and '" + (dTtime + "-23") + "'<=dTo1 and isnull(c.vcD23y,'')<>'' then  c.vcD23y*d.iBiYao else 0 end as vcD23yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-24") + "'>=dFrom1 and '" + (dTtime + "-24") + "'<=dTo1 and isnull(c.vcD24y,'')<>'' then  c.vcD24y*d.iBiYao else 0 end as vcD24yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-25") + "'>=dFrom1 and '" + (dTtime + "-25") + "'<=dTo1 and isnull(c.vcD25y,'')<>'' then  c.vcD25y*d.iBiYao else 0 end as vcD25yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-26") + "'>=dFrom1 and '" + (dTtime + "-26") + "'<=dTo1 and isnull(c.vcD26y,'')<>'' then  c.vcD26y*d.iBiYao else 0 end as vcD26yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-27") + "'>=dFrom1 and '" + (dTtime + "-27") + "'<=dTo1 and isnull(c.vcD27y,'')<>'' then  c.vcD27y*d.iBiYao else 0 end as vcD27yT,           ");
                    strSql.AppendLine(" case when '" + (dTtime + "-28") + "'>=dFrom1 and '" + (dTtime + "-28") + "'<=dTo1 and isnull(c.vcD28y,'')<>'' then  c.vcD28y*d.iBiYao else 0 end as vcD28yT,           ");


                    if (timenow >= 29)
                    {
                        strSql.AppendLine(" case when '" + (dTtime + "-29") + "'>=dFrom1 and '" + (dTtime + "-29") + "'<=dTo1 and isnull(c.vcD29y,'')<>'' then c.vcD29y*d.iBiYao else 0 end as vcD29yT,          ");
                    }
                    else
                    {
                        strSql.AppendLine(" 0 as vcD29yT,          ");

                    }
                    if (timenow >= 30)
                        strSql.AppendLine(" case when '" + (dTtime + "-30") + "'>=dFrom1 and '" + (dTtime + "-30") + "'<=dTo1 and isnull(c.vcD30y,'')<>'' then c.vcD30y*d.iBiYao else 0 end as vcD30yT,          ");
                    else
                        strSql.AppendLine(" 0 as vcD30yT,          ");
                    if (timenow >= 31)
                        strSql.AppendLine(" case when '" + (dTtime + "-31") + "'>=dFrom1 and '" + (dTtime + "-31") + "'<=dTo1 and isnull(c.vcD31y,'')<>'' then c.vcD31y*d.iBiYao else 0 end as vcD31yT,         ");
                    else
                        strSql.AppendLine(" 0 as vcD31yT,          ");

                    strSql.AppendLine("             ");

                    strSql.AppendLine("  '' as vcD1bTShow ,  '' as vcD1yTShow,           ");
                    strSql.AppendLine("  '' as vcD2bTShow ,  '' as vcD2yTShow,           ");
                    strSql.AppendLine("  '' as vcD3bTShow ,  '' as vcD3yTShow,           ");
                    strSql.AppendLine("  '' as vcD4bTShow ,  '' as vcD4yTShow,           ");
                    strSql.AppendLine("  '' as vcD5bTShow ,  '' as vcD5yTShow,           ");
                    strSql.AppendLine("  '' as vcD6bTShow ,  '' as vcD6yTShow,           ");
                    strSql.AppendLine("  '' as vcD7bTShow ,  '' as vcD7yTShow,           ");
                    strSql.AppendLine("  '' as vcD8bTShow ,  '' as vcD8yTShow,           ");
                    strSql.AppendLine("  '' as vcD9bTShow ,  '' as vcD9yTShow,           ");
                    strSql.AppendLine("  '' as vcD10bTShow , '' as vcD10yTShow ,          ");
                    strSql.AppendLine("  '' as vcD11bTShow , '' as vcD11yTShow ,          ");
                    strSql.AppendLine("  '' as vcD12bTShow , '' as vcD12yTShow ,          ");
                    strSql.AppendLine("  '' as vcD13bTShow , '' as vcD13yTShow ,          ");
                    strSql.AppendLine("  '' as vcD14bTShow , '' as vcD14yTShow ,          ");
                    strSql.AppendLine("  '' as vcD15bTShow , '' as vcD15yTShow ,          ");
                    strSql.AppendLine("  '' as vcD16bTShow , '' as vcD16yTShow ,          ");
                    strSql.AppendLine("  '' as vcD17bTShow , '' as vcD17yTShow ,          ");
                    strSql.AppendLine("  '' as vcD18bTShow , '' as vcD18yTShow ,          ");
                    strSql.AppendLine("  '' as vcD19bTShow , '' as vcD19yTShow ,          ");
                    strSql.AppendLine("  '' as vcD20bTShow , '' as vcD20yTShow ,          ");
                    strSql.AppendLine("  '' as vcD21bTShow , '' as vcD21yTShow ,          ");
                    strSql.AppendLine("  '' as vcD22bTShow , '' as vcD22yTShow ,          ");
                    strSql.AppendLine("  '' as vcD23bTShow , '' as vcD23yTShow ,          ");
                    strSql.AppendLine("  '' as vcD24bTShow , '' as vcD24yTShow ,          ");
                    strSql.AppendLine("  '' as vcD25bTShow , '' as vcD25yTShow ,          ");
                    strSql.AppendLine("  '' as vcD26bTShow , '' as vcD26yTShow ,          ");
                    strSql.AppendLine("  '' as vcD27bTShow , '' as vcD27yTShow ,          ");
                    strSql.AppendLine("  '' as vcD28bTShow , '' as vcD28yTShow ,          ");
                    strSql.AppendLine("  '' as vcD29bTShow , '' as vcD29yTShow ,          ");
                    strSql.AppendLine("  '' as vcD30bTShow , '' as vcD30yTShow ,          ");
                    strSql.AppendLine("  '' as vcD31bTShow , '' as vcD31yTShow            ");

                }
                strSql.AppendLine("    from(                                                        ");
                switch (Kind)
                {
                    case "0":
                        #region 生产计划
                        strSql.AppendLine("  select      ");
                        strSql.AppendLine("    case when isnull(s.vcMonth,'')<>'' then s.vcMonth else ss.vcMonth end as vcMonth,     ");
                        strSql.AppendLine("    case when isnull(s.vcPartsno,'')<>'' then s.vcPartsno else ss.vcPartsno end as vcPartsno,     ");
                        strSql.AppendLine("    case when isnull(s.vcDock,'')<>'' then s.vcDock else ss.vcDock end as vcDock,     ");
                        strSql.AppendLine("    case when isnull(s.vcCarType,'')<>'' then s.vcCarType else ss.vcCarType end as vcCarType,     ");
                        strSql.AppendLine("    case when isnull(s.vcProject1,'')<>'' then s.vcProject1 else ss.vcProject1 end as vcProject1,     ");
                        strSql.AppendLine("    case when isnull(s.vcProjectName,'')<>'' then s.vcProjectName else ss.vcProjectName end as vcProjectName,     ");
                        strSql.AppendLine("    case when isnull(s.vcMonTotal,'')<>'' then s.vcMonTotal else ss.vcMonTotal end as vcMonTotal,       ");

                        strSql.AppendLine("  case when isnull(ss.vcD1b,'')<>''then ss.vcD1b else s.vcD1b end as vcD1b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD2b,'')<>''then ss.vcD2b else s.vcD2b end as vcD2b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD3b,'')<>''then ss.vcD3b else s.vcD3b end as vcD3b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD4b,'')<>''then ss.vcD4b else s.vcD4b end as vcD4b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD5b,'')<>''then ss.vcD5b else s.vcD5b end as vcD5b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD6b,'')<>''then ss.vcD6b else s.vcD6b end as vcD6b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD7b,'')<>''then ss.vcD7b else s.vcD7b end as vcD7b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD8b,'')<>''then ss.vcD8b else s.vcD8b end as vcD8b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD9b,'')<>''then ss.vcD9b else s.vcD9b end as vcD9b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD10b,'')<>''then ss.vcD10b else s.vcD10b end as vcD10b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD1y,'')<>''then ss.vcD1y else s.vcD1y end as vcD1y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD2y,'')<>''then ss.vcD2y else s.vcD2y end as vcD2y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD3y,'')<>''then ss.vcD3y else s.vcD3y end as vcD3y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD4y,'')<>''then ss.vcD4y else s.vcD4y end as vcD4y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD5y,'')<>''then ss.vcD5y else s.vcD5y end as vcD5y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD6y,'')<>''then ss.vcD6y else s.vcD6y end as vcD6y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD7y,'')<>''then ss.vcD7y else s.vcD7y end as vcD7y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD8y,'')<>''then ss.vcD8y else s.vcD8y end as vcD8y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD9y,'')<>''then ss.vcD9y else s.vcD9y end as vcD9y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD10y,'')<>''then ss.vcD10y else s.vcD10y end as vcD10y,     ");

                        strSql.AppendLine("  case when isnull(ss.vcD11b,'')<>''then ss.vcD11b else s.vcD11b end as vcD11b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD12b,'')<>''then ss.vcD12b else s.vcD12b end as vcD12b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD13b,'')<>''then ss.vcD13b else s.vcD13b end as vcD13b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD14b,'')<>''then ss.vcD14b else s.vcD14b end as vcD14b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD15b,'')<>''then ss.vcD15b else s.vcD15b end as vcD15b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD16b,'')<>''then ss.vcD16b else s.vcD16b end as vcD16b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD17b,'')<>''then ss.vcD17b else s.vcD17b end as vcD17b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD18b,'')<>''then ss.vcD18b else s.vcD18b end as vcD18b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD19b,'')<>''then ss.vcD19b else s.vcD19b end as vcD19b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD20b,'')<>''then ss.vcD20b else s.vcD20b end as vcD20b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD11y,'')<>''then ss.vcD11y else s.vcD11y end as vcD11y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD12y,'')<>''then ss.vcD12y else s.vcD12y end as vcD12y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD13y,'')<>''then ss.vcD13y else s.vcD13y end as vcD13y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD14y,'')<>''then ss.vcD14y else s.vcD14y end as vcD14y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD15y,'')<>''then ss.vcD15y else s.vcD15y end as vcD15y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD16y,'')<>''then ss.vcD16y else s.vcD16y end as vcD16y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD17y,'')<>''then ss.vcD17y else s.vcD17y end as vcD17y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD18y,'')<>''then ss.vcD18y else s.vcD18y end as vcD18y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD19y,'')<>''then ss.vcD19y else s.vcD19y end as vcD19y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD20y,'')<>''then ss.vcD20y else s.vcD20y end as vcD20y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD21b,'')<>''then ss.vcD21b else s.vcD21b end as vcD21b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD22b,'')<>''then ss.vcD22b else s.vcD22b end as vcD22b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD23b,'')<>''then ss.vcD23b else s.vcD23b end as vcD23b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD24b,'')<>''then ss.vcD24b else s.vcD24b end as vcD24b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD25b,'')<>''then ss.vcD25b else s.vcD25b end as vcD25b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD26b,'')<>''then ss.vcD26b else s.vcD26b end as vcD26b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD27b,'')<>''then ss.vcD27b else s.vcD27b end as vcD27b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD28b,'')<>''then ss.vcD28b else s.vcD28b end as vcD28b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD29b,'')<>''then ss.vcD29b else s.vcD29b end as vcD29b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD30b,'')<>''then ss.vcD30b else s.vcD30b end as vcD30b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD31b,'')<>''then ss.vcD31b else s.vcD31b end as vcD31b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD21y,'')<>''then ss.vcD21y else s.vcD21y end as vcD21y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD22y,'')<>''then ss.vcD22y else s.vcD22y end as vcD22y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD23y,'')<>''then ss.vcD23y else s.vcD23y end as vcD23y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD24y,'')<>''then ss.vcD24y else s.vcD24y end as vcD24y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD25y,'')<>''then ss.vcD25y else s.vcD25y end as vcD25y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD26y,'')<>''then ss.vcD26y else s.vcD26y end as vcD26y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD27y,'')<>''then ss.vcD27y else s.vcD27y end as vcD27y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD28y,'')<>''then ss.vcD28y else s.vcD28y end as vcD28y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD29y,'')<>''then ss.vcD29y else s.vcD29y end as vcD29y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD30y,'')<>''then ss.vcD30y else s.vcD30y end as vcD30y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD31y,'')<>''then ss.vcD31y else s.vcD31y end as vcD31y,     ");
                        strSql.AppendLine("  s.montouch,s.DADDTIME,s.DUPDTIME,s.CUPDUSER,s.vcSupplier_id     ");
                        strSql.AppendLine("  from(     ");


                        #region 筛选生产计划月度(只含本月)
                        strSql.AppendLine("  select      ");
                        strSql.AppendLine("   a1.vcMonth,a1.vcPartsno,a1.vcDock,a1.vcCarType,a1.vcProject1,a1.vcProjectName,a1.vcMonTotal,    ");
                        strSql.AppendLine("   case when isnull(b1.vcD1b,'')<>'' then b1.vcD1b else a1.vcD1b end as vcD1b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD1y,'')<>'' then b1.vcD1y else a1.vcD1y end as vcD1y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD2b,'')<>'' then b1.vcD2b else a1.vcD2b end as vcD2b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD2y,'')<>'' then b1.vcD2y else a1.vcD2y end as vcD2y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD3b,'')<>'' then b1.vcD3b else a1.vcD3b end as vcD3b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD3y,'')<>'' then b1.vcD3y else a1.vcD3y end as vcD3y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD4b,'')<>'' then b1.vcD4b else a1.vcD4b end as vcD4b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD4y,'')<>'' then b1.vcD4y else a1.vcD4y end as vcD4y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD5b,'')<>'' then b1.vcD5b else a1.vcD5b end as vcD5b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD5y,'')<>'' then b1.vcD5y else a1.vcD5y end as vcD5y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD6b,'')<>'' then b1.vcD6b else a1.vcD6b end as vcD6b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD6y,'')<>'' then b1.vcD6y else a1.vcD6y end as vcD6y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD7b,'')<>'' then b1.vcD7b else a1.vcD7b end as vcD7b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD7y,'')<>'' then b1.vcD7y else a1.vcD7y end as vcD7y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD8b,'')<>'' then b1.vcD8b else a1.vcD8b end as vcD8b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD8y,'')<>'' then b1.vcD8y else a1.vcD8y end as vcD8y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD9b,'')<>'' then b1.vcD9b else a1.vcD9b end as vcD9b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD9y,'')<>'' then b1.vcD9y else a1.vcD9y end as vcD9y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD10b,'')<>'' then b1.vcD10b else a1.vcD10b end as vcD10b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD10y,'')<>'' then b1.vcD10y else a1.vcD10y end as vcD10y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD11b,'')<>'' then b1.vcD11b else a1.vcD11b end as vcD11b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD11y,'')<>'' then b1.vcD11y else a1.vcD11y end as vcD11y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD12b,'')<>'' then b1.vcD12b else a1.vcD12b end as vcD12b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD12y,'')<>'' then b1.vcD12y else a1.vcD12y end as vcD12y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD13b,'')<>'' then b1.vcD13b else a1.vcD13b end as vcD13b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD13y,'')<>'' then b1.vcD13y else a1.vcD13y end as vcD13y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD14b,'')<>'' then b1.vcD14b else a1.vcD14b end as vcD14b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD14y,'')<>'' then b1.vcD14y else a1.vcD14y end as vcD14y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD15b,'')<>'' then b1.vcD15b else a1.vcD15b end as vcD15b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD15y,'')<>'' then b1.vcD15y else a1.vcD15y end as vcD15y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD16b,'')<>'' then b1.vcD16b else a1.vcD16b end as vcD16b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD16y,'')<>'' then b1.vcD16y else a1.vcD16y end as vcD16y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD17b,'')<>'' then b1.vcD17b else a1.vcD17b end as vcD17b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD17y,'')<>'' then b1.vcD17y else a1.vcD17y end as vcD17y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD18b,'')<>'' then b1.vcD18b else a1.vcD18b end as vcD18b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD18y,'')<>'' then b1.vcD18y else a1.vcD18y end as vcD18y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD19b,'')<>'' then b1.vcD19b else a1.vcD19b end as vcD19b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD19y,'')<>'' then b1.vcD19y else a1.vcD19y end as vcD19y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD20b,'')<>'' then b1.vcD20b else a1.vcD20b end as vcD20b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD20y,'')<>'' then b1.vcD20y else a1.vcD20y end as vcD20y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD21b,'')<>'' then b1.vcD21b else a1.vcD21b end as vcD21b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD21y,'')<>'' then b1.vcD21y else a1.vcD21y end as vcD21y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD22b,'')<>'' then b1.vcD22b else a1.vcD22b end as vcD22b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD22y,'')<>'' then b1.vcD22y else a1.vcD22y end as vcD22y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD23b,'')<>'' then b1.vcD23b else a1.vcD23b end as vcD23b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD23y,'')<>'' then b1.vcD23y else a1.vcD23y end as vcD23y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD24b,'')<>'' then b1.vcD24b else a1.vcD24b end as vcD24b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD24y,'')<>'' then b1.vcD24y else a1.vcD24y end as vcD24y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD25b,'')<>'' then b1.vcD25b else a1.vcD25b end as vcD25b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD25y,'')<>'' then b1.vcD25y else a1.vcD25y end as vcD25y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD26b,'')<>'' then b1.vcD26b else a1.vcD26b end as vcD26b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD26y,'')<>'' then b1.vcD26y else a1.vcD26y end as vcD26y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD27b,'')<>'' then b1.vcD27b else a1.vcD27b end as vcD27b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD27y,'')<>'' then b1.vcD27y else a1.vcD27y end as vcD27y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD28b,'')<>'' then b1.vcD28b else a1.vcD28b end as vcD28b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD28y,'')<>'' then b1.vcD28y else a1.vcD28y end as vcD28y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD29b,'')<>'' then b1.vcD29b else a1.vcD29b end as vcD29b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD29y,'')<>'' then b1.vcD29y else a1.vcD29y end as vcD29y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD30b,'')<>'' then b1.vcD30b else a1.vcD30b end as vcD30b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD30y,'')<>'' then b1.vcD30y else a1.vcD30y end as vcD30y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD31b,'')<>'' then b1.vcD31b else a1.vcD31b end as vcD31b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD31y,'')<>'' then b1.vcD31y else a1.vcD31y end as vcD31y,          ");
                        strSql.AppendLine("   a1.montouch,a1.DADDTIME,a1.DUPDTIME,a1.CUPDUSER,a1.vcSupplier_id        ");
                        strSql.AppendLine("  from (     ");
                        strSql.AppendLine("    select * from MonthProdPlanTbl where montouch='" + dFtime + "'     ");
                        strSql.AppendLine("  )a1      ");
                        strSql.AppendLine("  left join     ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("    select * from MonthProdPlanTbl where isnull(montouch,'')='' and vcMonth='"+ dFtime + "'     ");
                        strSql.AppendLine("   )b1 on a1.vcPartsno=b1.vcPartsno     ");
                        strSql.AppendLine("       ");
                        strSql.AppendLine("       ");

                        #endregion

                        strSql.AppendLine("  )s full join     ");
                        strSql.AppendLine("  (     ");

                        #region 筛选周度
                        strSql.AppendLine("  select t1.vcMonth,t1.vcPartsno,t1.vcDock,t1.vcCarType,t1.vcProject1,t1.vcProjectName,t1.vcMonTotal,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD1b,'')<>'' then t2.vcD1b else t1.vcD1b end as vcD1b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD1y,'')<>'' then t2.vcD1y else t1.vcD1y end as vcD1y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD2b,'')<>'' then t2.vcD2b else t1.vcD2b end as vcD2b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD2y,'')<>'' then t2.vcD2y else t1.vcD2y end as vcD2y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD3b,'')<>'' then t2.vcD3b else t1.vcD3b end as vcD3b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD3y,'')<>'' then t2.vcD3y else t1.vcD3y end as vcD3y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD4b,'')<>'' then t2.vcD4b else t1.vcD4b end as vcD4b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD4y,'')<>'' then t2.vcD4y else t1.vcD4y end as vcD4y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD5b,'')<>'' then t2.vcD5b else t1.vcD5b end as vcD5b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD5y,'')<>'' then t2.vcD5y else t1.vcD5y end as vcD5y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD6b,'')<>'' then t2.vcD6b else t1.vcD6b end as vcD6b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD6y,'')<>'' then t2.vcD6y else t1.vcD6y end as vcD6y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD7b,'')<>'' then t2.vcD7b else t1.vcD7b end as vcD7b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD7y,'')<>'' then t2.vcD7y else t1.vcD7y end as vcD7y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD8b,'')<>'' then t2.vcD8b else t1.vcD8b end as vcD8b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD8y,'')<>'' then t2.vcD8y else t1.vcD8y end as vcD8y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD9b,'')<>'' then t2.vcD9b else t1.vcD9b end as vcD9b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD9y,'')<>'' then t2.vcD9y else t1.vcD9y end as vcD9y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD10b,'')<>'' then t2.vcD10b else t1.vcD10b end as vcD10b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD10y,'')<>'' then t2.vcD10y else t1.vcD10y end as vcD10y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD11b,'')<>'' then t2.vcD11b else t1.vcD11b end as vcD11b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD11y,'')<>'' then t2.vcD11y else t1.vcD11y end as vcD11y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD12b,'')<>'' then t2.vcD12b else t1.vcD12b end as vcD12b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD12y,'')<>'' then t2.vcD12y else t1.vcD12y end as vcD12y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD13b,'')<>'' then t2.vcD13b else t1.vcD13b end as vcD13b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD13y,'')<>'' then t2.vcD13y else t1.vcD13y end as vcD13y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD14b,'')<>'' then t2.vcD14b else t1.vcD14b end as vcD14b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD14y,'')<>'' then t2.vcD14y else t1.vcD14y end as vcD14y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD15b,'')<>'' then t2.vcD15b else t1.vcD15b end as vcD15b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD15y,'')<>'' then t2.vcD15y else t1.vcD15y end as vcD15y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD16b,'')<>'' then t2.vcD16b else t1.vcD16b end as vcD16b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD16y,'')<>'' then t2.vcD16y else t1.vcD16y end as vcD16y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD17b,'')<>'' then t2.vcD17b else t1.vcD17b end as vcD17b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD17y,'')<>'' then t2.vcD17y else t1.vcD17y end as vcD17y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD18b,'')<>'' then t2.vcD18b else t1.vcD18b end as vcD18b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD18y,'')<>'' then t2.vcD18y else t1.vcD18y end as vcD18y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD19b,'')<>'' then t2.vcD19b else t1.vcD19b end as vcD19b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD19y,'')<>'' then t2.vcD19y else t1.vcD19y end as vcD19y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD20b,'')<>'' then t2.vcD20b else t1.vcD20b end as vcD20b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD20y,'')<>'' then t2.vcD20y else t1.vcD20y end as vcD20y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD21b,'')<>'' then t2.vcD21b else t1.vcD21b end as vcD21b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD21y,'')<>'' then t2.vcD21y else t1.vcD21y end as vcD21y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD22b,'')<>'' then t2.vcD22b else t1.vcD22b end as vcD22b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD22y,'')<>'' then t2.vcD22y else t1.vcD22y end as vcD22y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD23b,'')<>'' then t2.vcD23b else t1.vcD23b end as vcD23b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD23y,'')<>'' then t2.vcD23y else t1.vcD23y end as vcD23y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD24b,'')<>'' then t2.vcD24b else t1.vcD24b end as vcD24b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD24y,'')<>'' then t2.vcD24y else t1.vcD24y end as vcD24y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD25b,'')<>'' then t2.vcD25b else t1.vcD25b end as vcD25b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD25y,'')<>'' then t2.vcD25y else t1.vcD25y end as vcD25y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD26b,'')<>'' then t2.vcD26b else t1.vcD26b end as vcD26b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD26y,'')<>'' then t2.vcD26y else t1.vcD26y end as vcD26y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD27b,'')<>'' then t2.vcD27b else t1.vcD27b end as vcD27b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD27y,'')<>'' then t2.vcD27y else t1.vcD27y end as vcD27y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD28b,'')<>'' then t2.vcD28b else t1.vcD28b end as vcD28b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD28y,'')<>'' then t2.vcD28y else t1.vcD28y end as vcD28y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD29b,'')<>'' then t2.vcD29b else t1.vcD29b end as vcD29b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD29y,'')<>'' then t2.vcD29y else t1.vcD29y end as vcD29y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD30b,'')<>'' then t2.vcD30b else t1.vcD30b end as vcD30b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD30y,'')<>'' then t2.vcD30y else t1.vcD30y end as vcD30y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD31b,'')<>'' then t2.vcD31b else t1.vcD31b end as vcD31b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD31y,'')<>'' then t2.vcD31y else t1.vcD31y end as vcD31y,     ");
                        strSql.AppendLine("  t1.montouch,t1.DADDTIME,t1.DUPDTIME,t1.CUPDUSER,t1.vcSupplier_id     ");
                        strSql.AppendLine("  from      ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("  select * from WeekProdPlanTbl where vcMonth='" + dFtime + "' and isnull(montouch,'')='' and (isnull(vcMonTotal,'')<>'')     ");
                        strSql.AppendLine("  )t1     ");
                        strSql.AppendLine("  left join     ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("  select * from WeekProdPlanTbl	where montouch='" + dFtime + "'     ");
                        strSql.AppendLine("  )t2 on t1.vcPartsno=t2.vcPartsno     ");
                        #endregion

                        strSql.AppendLine("    )ss on s.vcPartsno=ss.vcPartsno   ");

                        strSql.AppendLine("    )a                                                           ");
                        if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                        {
                            strSql.AppendLine("    left join                                                   ");
                            strSql.AppendLine("    (                                                            ");
                            strSql.AppendLine("  select      ");
                            strSql.AppendLine("    case when isnull(cc.vcMonth,'')<>'' then cc.vcMonth else cc1.vcMonth end as vcMonth,     ");
                            strSql.AppendLine("    case when isnull(cc.vcPartsno,'')<>'' then cc.vcPartsno else cc1.vcPartsno end as vcPartsno,     ");
                            strSql.AppendLine("    case when isnull(cc.vcDock,'')<>'' then cc.vcDock else cc1.vcDock end as vcDock,     ");
                            strSql.AppendLine("    case when isnull(cc.vcCarType,'')<>'' then cc.vcCarType else cc1.vcCarType end as vcCarType,     ");
                            strSql.AppendLine("    case when isnull(cc.vcProject1,'')<>'' then cc.vcProject1 else cc1.vcProject1 end as vcProject1,     ");
                            strSql.AppendLine("    case when isnull(cc.vcProjectName,'')<>'' then cc.vcProjectName else cc1.vcProjectName end as vcProjectName,     ");
                            strSql.AppendLine("    case when isnull(cc.vcMonTotal,'')<>'' then cc.vcMonTotal else cc1.vcMonTotal end as vcMonTotal,       ");

                            strSql.AppendLine("  case when isnull(cc1.vcD1b,'')<>''then cc1.vcD1b else cc.vcD1b end as vcD1b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD2b,'')<>''then cc1.vcD2b else cc.vcD2b end as vcD2b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD3b,'')<>''then cc1.vcD3b else cc.vcD3b end as vcD3b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD4b,'')<>''then cc1.vcD4b else cc.vcD4b end as vcD4b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD5b,'')<>''then cc1.vcD5b else cc.vcD5b end as vcD5b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD6b,'')<>''then cc1.vcD6b else cc.vcD6b end as vcD6b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD7b,'')<>''then cc1.vcD7b else cc.vcD7b end as vcD7b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD8b,'')<>''then cc1.vcD8b else cc.vcD8b end as vcD8b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD9b,'')<>''then cc1.vcD9b else cc.vcD9b end as vcD9b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD10b,'')<>''then cc1.vcD10b else cc.vcD10b end as vcD10b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD1y,'')<>''then cc1.vcD1y else cc.vcD1y end as vcD1y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD2y,'')<>''then cc1.vcD2y else cc.vcD2y end as vcD2y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD3y,'')<>''then cc1.vcD3y else cc.vcD3y end as vcD3y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD4y,'')<>''then cc1.vcD4y else cc.vcD4y end as vcD4y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD5y,'')<>''then cc1.vcD5y else cc.vcD5y end as vcD5y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD6y,'')<>''then cc1.vcD6y else cc.vcD6y end as vcD6y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD7y,'')<>''then cc1.vcD7y else cc.vcD7y end as vcD7y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD8y,'')<>''then cc1.vcD8y else cc.vcD8y end as vcD8y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD9y,'')<>''then cc1.vcD9y else cc.vcD9y end as vcD9y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD10y,'')<>''then cc1.vcD10y else cc.vcD10y end as vcD10y,     ");

                            strSql.AppendLine("  case when isnull(cc1.vcD11b,'')<>''then cc1.vcD11b else cc.vcD11b end as vcD11b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD12b,'')<>''then cc1.vcD12b else cc.vcD12b end as vcD12b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD13b,'')<>''then cc1.vcD13b else cc.vcD13b end as vcD13b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD14b,'')<>''then cc1.vcD14b else cc.vcD14b end as vcD14b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD15b,'')<>''then cc1.vcD15b else cc.vcD15b end as vcD15b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD16b,'')<>''then cc1.vcD16b else cc.vcD16b end as vcD16b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD17b,'')<>''then cc1.vcD17b else cc.vcD17b end as vcD17b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD18b,'')<>''then cc1.vcD18b else cc.vcD18b end as vcD18b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD19b,'')<>''then cc1.vcD19b else cc.vcD19b end as vcD19b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD20b,'')<>''then cc1.vcD20b else cc.vcD20b end as vcD20b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD11y,'')<>''then cc1.vcD11y else cc.vcD11y end as vcD11y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD12y,'')<>''then cc1.vcD12y else cc.vcD12y end as vcD12y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD13y,'')<>''then cc1.vcD13y else cc.vcD13y end as vcD13y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD14y,'')<>''then cc1.vcD14y else cc.vcD14y end as vcD14y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD15y,'')<>''then cc1.vcD15y else cc.vcD15y end as vcD15y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD16y,'')<>''then cc1.vcD16y else cc.vcD16y end as vcD16y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD17y,'')<>''then cc1.vcD17y else cc.vcD17y end as vcD17y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD18y,'')<>''then cc1.vcD18y else cc.vcD18y end as vcD18y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD19y,'')<>''then cc1.vcD19y else cc.vcD19y end as vcD19y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD20y,'')<>''then cc1.vcD20y else cc.vcD20y end as vcD20y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD21b,'')<>''then cc1.vcD21b else cc.vcD21b end as vcD21b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD22b,'')<>''then cc1.vcD22b else cc.vcD22b end as vcD22b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD23b,'')<>''then cc1.vcD23b else cc.vcD23b end as vcD23b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD24b,'')<>''then cc1.vcD24b else cc.vcD24b end as vcD24b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD25b,'')<>''then cc1.vcD25b else cc.vcD25b end as vcD25b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD26b,'')<>''then cc1.vcD26b else cc.vcD26b end as vcD26b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD27b,'')<>''then cc1.vcD27b else cc.vcD27b end as vcD27b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD28b,'')<>''then cc1.vcD28b else cc.vcD28b end as vcD28b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD29b,'')<>''then cc1.vcD29b else cc.vcD29b end as vcD29b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD30b,'')<>''then cc1.vcD30b else cc.vcD30b end as vcD30b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD31b,'')<>''then cc1.vcD31b else cc.vcD31b end as vcD31b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD21y,'')<>''then cc1.vcD21y else cc.vcD21y end as vcD21y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD22y,'')<>''then cc1.vcD22y else cc.vcD22y end as vcD22y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD23y,'')<>''then cc1.vcD23y else cc.vcD23y end as vcD23y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD24y,'')<>''then cc1.vcD24y else cc.vcD24y end as vcD24y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD25y,'')<>''then cc1.vcD25y else cc.vcD25y end as vcD25y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD26y,'')<>''then cc1.vcD26y else cc.vcD26y end as vcD26y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD27y,'')<>''then cc1.vcD27y else cc.vcD27y end as vcD27y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD28y,'')<>''then cc1.vcD28y else cc.vcD28y end as vcD28y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD29y,'')<>''then cc1.vcD29y else cc.vcD29y end as vcD29y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD30y,'')<>''then cc1.vcD30y else cc.vcD30y end as vcD30y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD31y,'')<>''then cc1.vcD31y else cc.vcD31y end as vcD31y,     ");
                            strSql.AppendLine("  cc1.montouch,cc1.DADDTIME,cc1.DUPDTIME,cc1.CUPDUSER,cc1.vcSupplier_id     ");
                            strSql.AppendLine("  from(     ");


                            #region 筛选生产计划(含当前月和下月)

                            strSql.AppendLine("  select      ");
                            strSql.AppendLine("   a2.vcMonth,a2.vcPartsno,a2.vcDock,a2.vcCarType,a2.vcProject1,a2.vcProjectName,a2.vcMonTotal,    ");
                            strSql.AppendLine("   case when isnull(b2.vcD1b,'')<>'' then b2.vcD1b else a2.vcD1b end as vcD1b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD1y,'')<>'' then b2.vcD1y else a2.vcD1y end as vcD1y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD2b,'')<>'' then b2.vcD2b else a2.vcD2b end as vcD2b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD2y,'')<>'' then b2.vcD2y else a2.vcD2y end as vcD2y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD3b,'')<>'' then b2.vcD3b else a2.vcD3b end as vcD3b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD3y,'')<>'' then b2.vcD3y else a2.vcD3y end as vcD3y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD4b,'')<>'' then b2.vcD4b else a2.vcD4b end as vcD4b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD4y,'')<>'' then b2.vcD4y else a2.vcD4y end as vcD4y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD5b,'')<>'' then b2.vcD5b else a2.vcD5b end as vcD5b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD5y,'')<>'' then b2.vcD5y else a2.vcD5y end as vcD5y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD6b,'')<>'' then b2.vcD6b else a2.vcD6b end as vcD6b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD6y,'')<>'' then b2.vcD6y else a2.vcD6y end as vcD6y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD7b,'')<>'' then b2.vcD7b else a2.vcD7b end as vcD7b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD7y,'')<>'' then b2.vcD7y else a2.vcD7y end as vcD7y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD8b,'')<>'' then b2.vcD8b else a2.vcD8b end as vcD8b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD8y,'')<>'' then b2.vcD8y else a2.vcD8y end as vcD8y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD9b,'')<>'' then b2.vcD9b else a2.vcD9b end as vcD9b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD9y,'')<>'' then b2.vcD9y else a2.vcD9y end as vcD9y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD10b,'')<>'' then b2.vcD10b else a2.vcD10b end as vcD10b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD10y,'')<>'' then b2.vcD10y else a2.vcD10y end as vcD10y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD11b,'')<>'' then b2.vcD11b else a2.vcD11b end as vcD11b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD11y,'')<>'' then b2.vcD11y else a2.vcD11y end as vcD11y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD12b,'')<>'' then b2.vcD12b else a2.vcD12b end as vcD12b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD12y,'')<>'' then b2.vcD12y else a2.vcD12y end as vcD12y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD13b,'')<>'' then b2.vcD13b else a2.vcD13b end as vcD13b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD13y,'')<>'' then b2.vcD13y else a2.vcD13y end as vcD13y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD14b,'')<>'' then b2.vcD14b else a2.vcD14b end as vcD14b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD14y,'')<>'' then b2.vcD14y else a2.vcD14y end as vcD14y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD15b,'')<>'' then b2.vcD15b else a2.vcD15b end as vcD15b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD15y,'')<>'' then b2.vcD15y else a2.vcD15y end as vcD15y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD16b,'')<>'' then b2.vcD16b else a2.vcD16b end as vcD16b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD16y,'')<>'' then b2.vcD16y else a2.vcD16y end as vcD16y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD17b,'')<>'' then b2.vcD17b else a2.vcD17b end as vcD17b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD17y,'')<>'' then b2.vcD17y else a2.vcD17y end as vcD17y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD18b,'')<>'' then b2.vcD18b else a2.vcD18b end as vcD18b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD18y,'')<>'' then b2.vcD18y else a2.vcD18y end as vcD18y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD19b,'')<>'' then b2.vcD19b else a2.vcD19b end as vcD19b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD19y,'')<>'' then b2.vcD19y else a2.vcD19y end as vcD19y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD20b,'')<>'' then b2.vcD20b else a2.vcD20b end as vcD20b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD20y,'')<>'' then b2.vcD20y else a2.vcD20y end as vcD20y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD21b,'')<>'' then b2.vcD21b else a2.vcD21b end as vcD21b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD21y,'')<>'' then b2.vcD21y else a2.vcD21y end as vcD21y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD22b,'')<>'' then b2.vcD22b else a2.vcD22b end as vcD22b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD22y,'')<>'' then b2.vcD22y else a2.vcD22y end as vcD22y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD23b,'')<>'' then b2.vcD23b else a2.vcD23b end as vcD23b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD23y,'')<>'' then b2.vcD23y else a2.vcD23y end as vcD23y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD24b,'')<>'' then b2.vcD24b else a2.vcD24b end as vcD24b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD24y,'')<>'' then b2.vcD24y else a2.vcD24y end as vcD24y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD25b,'')<>'' then b2.vcD25b else a2.vcD25b end as vcD25b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD25y,'')<>'' then b2.vcD25y else a2.vcD25y end as vcD25y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD26b,'')<>'' then b2.vcD26b else a2.vcD26b end as vcD26b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD26y,'')<>'' then b2.vcD26y else a2.vcD26y end as vcD26y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD27b,'')<>'' then b2.vcD27b else a2.vcD27b end as vcD27b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD27y,'')<>'' then b2.vcD27y else a2.vcD27y end as vcD27y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD28b,'')<>'' then b2.vcD28b else a2.vcD28b end as vcD28b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD28y,'')<>'' then b2.vcD28y else a2.vcD28y end as vcD28y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD29b,'')<>'' then b2.vcD29b else a2.vcD29b end as vcD29b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD29y,'')<>'' then b2.vcD29y else a2.vcD29y end as vcD29y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD30b,'')<>'' then b2.vcD30b else a2.vcD30b end as vcD30b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD30y,'')<>'' then b2.vcD30y else a2.vcD30y end as vcD30y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD31b,'')<>'' then b2.vcD31b else a2.vcD31b end as vcD31b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD31y,'')<>'' then b2.vcD31y else a2.vcD31y end as vcD31y,          ");
                            strSql.AppendLine("   a2.montouch,a2.DADDTIME,a2.DUPDTIME,a2.CUPDUSER,a2.vcSupplier_id        ");
                            strSql.AppendLine("  from (     ");
                            strSql.AppendLine("    select * from MonthProdPlanTbl where montouch='" + dTtime + "'     ");
                            strSql.AppendLine("  )a2      ");
                            strSql.AppendLine("  left join     ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("    select * from MonthProdPlanTbl where isnull(montouch,'')='' and vcMonth='" + dTtime + "'     ");
                            strSql.AppendLine("   )b2 on a2.vcPartsno=b2.vcPartsno     ");
                            strSql.AppendLine("       ");
                            strSql.AppendLine("       ");
                            #endregion

                            strSql.AppendLine("  )cc full join     ");
                            strSql.AppendLine("  (     ");

                            #region 筛选含当月和下月的周度内示
                            strSql.AppendLine("  select t3.vcMonth,t3.vcPartsno,t3.vcDock,t3.vcCarType,t3.vcProject1,t3.vcProjectName,t3.vcMonTotal,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD1b,'')<>'' then t4.vcD1b else t3.vcD1b end as vcD1b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD1y,'')<>'' then t4.vcD1y else t3.vcD1y end as vcD1y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD2b,'')<>'' then t4.vcD2b else t3.vcD2b end as vcD2b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD2y,'')<>'' then t4.vcD2y else t3.vcD2y end as vcD2y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD3b,'')<>'' then t4.vcD3b else t3.vcD3b end as vcD3b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD3y,'')<>'' then t4.vcD3y else t3.vcD3y end as vcD3y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD4b,'')<>'' then t4.vcD4b else t3.vcD4b end as vcD4b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD4y,'')<>'' then t4.vcD4y else t3.vcD4y end as vcD4y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD5b,'')<>'' then t4.vcD5b else t3.vcD5b end as vcD5b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD5y,'')<>'' then t4.vcD5y else t3.vcD5y end as vcD5y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD6b,'')<>'' then t4.vcD6b else t3.vcD6b end as vcD6b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD6y,'')<>'' then t4.vcD6y else t3.vcD6y end as vcD6y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD7b,'')<>'' then t4.vcD7b else t3.vcD7b end as vcD7b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD7y,'')<>'' then t4.vcD7y else t3.vcD7y end as vcD7y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD8b,'')<>'' then t4.vcD8b else t3.vcD8b end as vcD8b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD8y,'')<>'' then t4.vcD8y else t3.vcD8y end as vcD8y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD9b,'')<>'' then t4.vcD9b else t3.vcD9b end as vcD9b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD9y,'')<>'' then t4.vcD9y else t3.vcD9y end as vcD9y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD10b,'')<>'' then t4.vcD10b else t3.vcD10b end as vcD10b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD10y,'')<>'' then t4.vcD10y else t3.vcD10y end as vcD10y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD11b,'')<>'' then t4.vcD11b else t3.vcD11b end as vcD11b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD11y,'')<>'' then t4.vcD11y else t3.vcD11y end as vcD11y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD12b,'')<>'' then t4.vcD12b else t3.vcD12b end as vcD12b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD12y,'')<>'' then t4.vcD12y else t3.vcD12y end as vcD12y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD13b,'')<>'' then t4.vcD13b else t3.vcD13b end as vcD13b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD13y,'')<>'' then t4.vcD13y else t3.vcD13y end as vcD13y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD14b,'')<>'' then t4.vcD14b else t3.vcD14b end as vcD14b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD14y,'')<>'' then t4.vcD14y else t3.vcD14y end as vcD14y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD15b,'')<>'' then t4.vcD15b else t3.vcD15b end as vcD15b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD15y,'')<>'' then t4.vcD15y else t3.vcD15y end as vcD15y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD16b,'')<>'' then t4.vcD16b else t3.vcD16b end as vcD16b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD16y,'')<>'' then t4.vcD16y else t3.vcD16y end as vcD16y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD17b,'')<>'' then t4.vcD17b else t3.vcD17b end as vcD17b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD17y,'')<>'' then t4.vcD17y else t3.vcD17y end as vcD17y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD18b,'')<>'' then t4.vcD18b else t3.vcD18b end as vcD18b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD18y,'')<>'' then t4.vcD18y else t3.vcD18y end as vcD18y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD19b,'')<>'' then t4.vcD19b else t3.vcD19b end as vcD19b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD19y,'')<>'' then t4.vcD19y else t3.vcD19y end as vcD19y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD20b,'')<>'' then t4.vcD20b else t3.vcD20b end as vcD20b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD20y,'')<>'' then t4.vcD20y else t3.vcD20y end as vcD20y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD21b,'')<>'' then t4.vcD21b else t3.vcD21b end as vcD21b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD21y,'')<>'' then t4.vcD21y else t3.vcD21y end as vcD21y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD22b,'')<>'' then t4.vcD22b else t3.vcD22b end as vcD22b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD22y,'')<>'' then t4.vcD22y else t3.vcD22y end as vcD22y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD23b,'')<>'' then t4.vcD23b else t3.vcD23b end as vcD23b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD23y,'')<>'' then t4.vcD23y else t3.vcD23y end as vcD23y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD24b,'')<>'' then t4.vcD24b else t3.vcD24b end as vcD24b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD24y,'')<>'' then t4.vcD24y else t3.vcD24y end as vcD24y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD25b,'')<>'' then t4.vcD25b else t3.vcD25b end as vcD25b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD25y,'')<>'' then t4.vcD25y else t3.vcD25y end as vcD25y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD26b,'')<>'' then t4.vcD26b else t3.vcD26b end as vcD26b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD26y,'')<>'' then t4.vcD26y else t3.vcD26y end as vcD26y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD27b,'')<>'' then t4.vcD27b else t3.vcD27b end as vcD27b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD27y,'')<>'' then t4.vcD27y else t3.vcD27y end as vcD27y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD28b,'')<>'' then t4.vcD28b else t3.vcD28b end as vcD28b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD28y,'')<>'' then t4.vcD28y else t3.vcD28y end as vcD28y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD29b,'')<>'' then t4.vcD29b else t3.vcD29b end as vcD29b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD29y,'')<>'' then t4.vcD29y else t3.vcD29y end as vcD29y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD30b,'')<>'' then t4.vcD30b else t3.vcD30b end as vcD30b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD30y,'')<>'' then t4.vcD30y else t3.vcD30y end as vcD30y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD31b,'')<>'' then t4.vcD31b else t3.vcD31b end as vcD31b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD31y,'')<>'' then t4.vcD31y else t3.vcD31y end as vcD31y,     ");
                            strSql.AppendLine("  t3.montouch,t3.DADDTIME,t3.DUPDTIME,t3.CUPDUSER,t3.vcSupplier_id     ");
                            strSql.AppendLine("  from      ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("  select * from WeekProdPlanTbl where vcMonth='" + dTtime + "' and isnull(montouch,'')='' and (isnull(vcMonTotal,'')<>'')     ");
                            strSql.AppendLine("  )t3     ");
                            strSql.AppendLine("  left join     ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("  select * from WeekProdPlanTbl	where montouch='" + dTtime + "'     ");
                            strSql.AppendLine("  )t4 on t3.vcPartsno=t4.vcPartsno     ");
                            #endregion


                            strSql.AppendLine("    )cc1 on cc.vcPartsno=cc1.vcPartsno   ");
                            strSql.AppendLine("    )c on a.vcPartsno=c.vcPartsno                                ");
                        }
                        #endregion
                        break;
                    case "1":
                        #region 包装计划
                        strSql.AppendLine("  select      ");
                        strSql.AppendLine("    case when isnull(s.vcMonth,'')<>'' then s.vcMonth else ss.vcMonth end as vcMonth,     ");
                        strSql.AppendLine("    case when isnull(s.vcPartsno,'')<>'' then s.vcPartsno else ss.vcPartsno end as vcPartsno,     ");
                        strSql.AppendLine("    case when isnull(s.vcDock,'')<>'' then s.vcDock else ss.vcDock end as vcDock,     ");
                        strSql.AppendLine("    case when isnull(s.vcCarType,'')<>'' then s.vcCarType else ss.vcCarType end as vcCarType,     ");
                        strSql.AppendLine("    case when isnull(s.vcProject1,'')<>'' then s.vcProject1 else ss.vcProject1 end as vcProject1,     ");
                        strSql.AppendLine("    case when isnull(s.vcProjectName,'')<>'' then s.vcProjectName else ss.vcProjectName end as vcProjectName,     ");
                        strSql.AppendLine("    case when isnull(s.vcMonTotal,'')<>'' then s.vcMonTotal else ss.vcMonTotal end as vcMonTotal,       ");

                        strSql.AppendLine("  case when isnull(ss.vcD1b,'')<>''then ss.vcD1b else s.vcD1b end as vcD1b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD2b,'')<>''then ss.vcD2b else s.vcD2b end as vcD2b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD3b,'')<>''then ss.vcD3b else s.vcD3b end as vcD3b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD4b,'')<>''then ss.vcD4b else s.vcD4b end as vcD4b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD5b,'')<>''then ss.vcD5b else s.vcD5b end as vcD5b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD6b,'')<>''then ss.vcD6b else s.vcD6b end as vcD6b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD7b,'')<>''then ss.vcD7b else s.vcD7b end as vcD7b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD8b,'')<>''then ss.vcD8b else s.vcD8b end as vcD8b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD9b,'')<>''then ss.vcD9b else s.vcD9b end as vcD9b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD10b,'')<>''then ss.vcD10b else s.vcD10b end as vcD10b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD1y,'')<>''then ss.vcD1y else s.vcD1y end as vcD1y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD2y,'')<>''then ss.vcD2y else s.vcD2y end as vcD2y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD3y,'')<>''then ss.vcD3y else s.vcD3y end as vcD3y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD4y,'')<>''then ss.vcD4y else s.vcD4y end as vcD4y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD5y,'')<>''then ss.vcD5y else s.vcD5y end as vcD5y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD6y,'')<>''then ss.vcD6y else s.vcD6y end as vcD6y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD7y,'')<>''then ss.vcD7y else s.vcD7y end as vcD7y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD8y,'')<>''then ss.vcD8y else s.vcD8y end as vcD8y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD9y,'')<>''then ss.vcD9y else s.vcD9y end as vcD9y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD10y,'')<>''then ss.vcD10y else s.vcD10y end as vcD10y,     ");

                        strSql.AppendLine("  case when isnull(ss.vcD11b,'')<>''then ss.vcD11b else s.vcD11b end as vcD11b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD12b,'')<>''then ss.vcD12b else s.vcD12b end as vcD12b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD13b,'')<>''then ss.vcD13b else s.vcD13b end as vcD13b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD14b,'')<>''then ss.vcD14b else s.vcD14b end as vcD14b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD15b,'')<>''then ss.vcD15b else s.vcD15b end as vcD15b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD16b,'')<>''then ss.vcD16b else s.vcD16b end as vcD16b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD17b,'')<>''then ss.vcD17b else s.vcD17b end as vcD17b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD18b,'')<>''then ss.vcD18b else s.vcD18b end as vcD18b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD19b,'')<>''then ss.vcD19b else s.vcD19b end as vcD19b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD20b,'')<>''then ss.vcD20b else s.vcD20b end as vcD20b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD11y,'')<>''then ss.vcD11y else s.vcD11y end as vcD11y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD12y,'')<>''then ss.vcD12y else s.vcD12y end as vcD12y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD13y,'')<>''then ss.vcD13y else s.vcD13y end as vcD13y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD14y,'')<>''then ss.vcD14y else s.vcD14y end as vcD14y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD15y,'')<>''then ss.vcD15y else s.vcD15y end as vcD15y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD16y,'')<>''then ss.vcD16y else s.vcD16y end as vcD16y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD17y,'')<>''then ss.vcD17y else s.vcD17y end as vcD17y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD18y,'')<>''then ss.vcD18y else s.vcD18y end as vcD18y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD19y,'')<>''then ss.vcD19y else s.vcD19y end as vcD19y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD20y,'')<>''then ss.vcD20y else s.vcD20y end as vcD20y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD21b,'')<>''then ss.vcD21b else s.vcD21b end as vcD21b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD22b,'')<>''then ss.vcD22b else s.vcD22b end as vcD22b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD23b,'')<>''then ss.vcD23b else s.vcD23b end as vcD23b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD24b,'')<>''then ss.vcD24b else s.vcD24b end as vcD24b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD25b,'')<>''then ss.vcD25b else s.vcD25b end as vcD25b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD26b,'')<>''then ss.vcD26b else s.vcD26b end as vcD26b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD27b,'')<>''then ss.vcD27b else s.vcD27b end as vcD27b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD28b,'')<>''then ss.vcD28b else s.vcD28b end as vcD28b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD29b,'')<>''then ss.vcD29b else s.vcD29b end as vcD29b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD30b,'')<>''then ss.vcD30b else s.vcD30b end as vcD30b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD31b,'')<>''then ss.vcD31b else s.vcD31b end as vcD31b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD21y,'')<>''then ss.vcD21y else s.vcD21y end as vcD21y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD22y,'')<>''then ss.vcD22y else s.vcD22y end as vcD22y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD23y,'')<>''then ss.vcD23y else s.vcD23y end as vcD23y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD24y,'')<>''then ss.vcD24y else s.vcD24y end as vcD24y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD25y,'')<>''then ss.vcD25y else s.vcD25y end as vcD25y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD26y,'')<>''then ss.vcD26y else s.vcD26y end as vcD26y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD27y,'')<>''then ss.vcD27y else s.vcD27y end as vcD27y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD28y,'')<>''then ss.vcD28y else s.vcD28y end as vcD28y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD29y,'')<>''then ss.vcD29y else s.vcD29y end as vcD29y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD30y,'')<>''then ss.vcD30y else s.vcD30y end as vcD30y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD31y,'')<>''then ss.vcD31y else s.vcD31y end as vcD31y,     ");
                        strSql.AppendLine("  s.montouch,s.DADDTIME,s.DUPDTIME,s.CUPDUSER,s.vcSupplier_id     ");
                        strSql.AppendLine("  from(     ");



                        #region 当月包装计划月度内示

                        strSql.AppendLine("  select      ");
                        strSql.AppendLine("   a1.vcMonth,a1.vcPartsno,a1.vcDock,a1.vcCarType,a1.vcProject1,a1.vcProjectName,a1.vcMonTotal,    ");
                        strSql.AppendLine("   case when isnull(b1.vcD1b,'')<>'' then b1.vcD1b else a1.vcD1b end as vcD1b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD1y,'')<>'' then b1.vcD1y else a1.vcD1y end as vcD1y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD2b,'')<>'' then b1.vcD2b else a1.vcD2b end as vcD2b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD2y,'')<>'' then b1.vcD2y else a1.vcD2y end as vcD2y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD3b,'')<>'' then b1.vcD3b else a1.vcD3b end as vcD3b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD3y,'')<>'' then b1.vcD3y else a1.vcD3y end as vcD3y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD4b,'')<>'' then b1.vcD4b else a1.vcD4b end as vcD4b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD4y,'')<>'' then b1.vcD4y else a1.vcD4y end as vcD4y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD5b,'')<>'' then b1.vcD5b else a1.vcD5b end as vcD5b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD5y,'')<>'' then b1.vcD5y else a1.vcD5y end as vcD5y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD6b,'')<>'' then b1.vcD6b else a1.vcD6b end as vcD6b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD6y,'')<>'' then b1.vcD6y else a1.vcD6y end as vcD6y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD7b,'')<>'' then b1.vcD7b else a1.vcD7b end as vcD7b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD7y,'')<>'' then b1.vcD7y else a1.vcD7y end as vcD7y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD8b,'')<>'' then b1.vcD8b else a1.vcD8b end as vcD8b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD8y,'')<>'' then b1.vcD8y else a1.vcD8y end as vcD8y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD9b,'')<>'' then b1.vcD9b else a1.vcD9b end as vcD9b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD9y,'')<>'' then b1.vcD9y else a1.vcD9y end as vcD9y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD10b,'')<>'' then b1.vcD10b else a1.vcD10b end as vcD10b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD10y,'')<>'' then b1.vcD10y else a1.vcD10y end as vcD10y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD11b,'')<>'' then b1.vcD11b else a1.vcD11b end as vcD11b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD11y,'')<>'' then b1.vcD11y else a1.vcD11y end as vcD11y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD12b,'')<>'' then b1.vcD12b else a1.vcD12b end as vcD12b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD12y,'')<>'' then b1.vcD12y else a1.vcD12y end as vcD12y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD13b,'')<>'' then b1.vcD13b else a1.vcD13b end as vcD13b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD13y,'')<>'' then b1.vcD13y else a1.vcD13y end as vcD13y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD14b,'')<>'' then b1.vcD14b else a1.vcD14b end as vcD14b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD14y,'')<>'' then b1.vcD14y else a1.vcD14y end as vcD14y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD15b,'')<>'' then b1.vcD15b else a1.vcD15b end as vcD15b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD15y,'')<>'' then b1.vcD15y else a1.vcD15y end as vcD15y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD16b,'')<>'' then b1.vcD16b else a1.vcD16b end as vcD16b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD16y,'')<>'' then b1.vcD16y else a1.vcD16y end as vcD16y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD17b,'')<>'' then b1.vcD17b else a1.vcD17b end as vcD17b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD17y,'')<>'' then b1.vcD17y else a1.vcD17y end as vcD17y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD18b,'')<>'' then b1.vcD18b else a1.vcD18b end as vcD18b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD18y,'')<>'' then b1.vcD18y else a1.vcD18y end as vcD18y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD19b,'')<>'' then b1.vcD19b else a1.vcD19b end as vcD19b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD19y,'')<>'' then b1.vcD19y else a1.vcD19y end as vcD19y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD20b,'')<>'' then b1.vcD20b else a1.vcD20b end as vcD20b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD20y,'')<>'' then b1.vcD20y else a1.vcD20y end as vcD20y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD21b,'')<>'' then b1.vcD21b else a1.vcD21b end as vcD21b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD21y,'')<>'' then b1.vcD21y else a1.vcD21y end as vcD21y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD22b,'')<>'' then b1.vcD22b else a1.vcD22b end as vcD22b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD22y,'')<>'' then b1.vcD22y else a1.vcD22y end as vcD22y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD23b,'')<>'' then b1.vcD23b else a1.vcD23b end as vcD23b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD23y,'')<>'' then b1.vcD23y else a1.vcD23y end as vcD23y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD24b,'')<>'' then b1.vcD24b else a1.vcD24b end as vcD24b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD24y,'')<>'' then b1.vcD24y else a1.vcD24y end as vcD24y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD25b,'')<>'' then b1.vcD25b else a1.vcD25b end as vcD25b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD25y,'')<>'' then b1.vcD25y else a1.vcD25y end as vcD25y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD26b,'')<>'' then b1.vcD26b else a1.vcD26b end as vcD26b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD26y,'')<>'' then b1.vcD26y else a1.vcD26y end as vcD26y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD27b,'')<>'' then b1.vcD27b else a1.vcD27b end as vcD27b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD27y,'')<>'' then b1.vcD27y else a1.vcD27y end as vcD27y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD28b,'')<>'' then b1.vcD28b else a1.vcD28b end as vcD28b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD28y,'')<>'' then b1.vcD28y else a1.vcD28y end as vcD28y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD29b,'')<>'' then b1.vcD29b else a1.vcD29b end as vcD29b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD29y,'')<>'' then b1.vcD29y else a1.vcD29y end as vcD29y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD30b,'')<>'' then b1.vcD30b else a1.vcD30b end as vcD30b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD30y,'')<>'' then b1.vcD30y else a1.vcD30y end as vcD30y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD31b,'')<>'' then b1.vcD31b else a1.vcD31b end as vcD31b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD31y,'')<>'' then b1.vcD31y else a1.vcD31y end as vcD31y,          ");
                        strSql.AppendLine("   a1.montouch,a1.DADDTIME,a1.DUPDTIME,a1.CUPDUSER,a1.vcSupplier_id        ");
                        strSql.AppendLine("  from (     ");
                        strSql.AppendLine("    select * from MonthPackPlanTbl where montouch='" + dFtime + "'     ");
                        strSql.AppendLine("  )a1      ");
                        strSql.AppendLine("  left join     ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("    select * from MonthPackPlanTbl where isnull(montouch,'')='' and vcMonth='" + dFtime + "'     ");
                        strSql.AppendLine("   )b1 on a1.vcPartsno=b1.vcPartsno     ");
                        strSql.AppendLine("       ");
                        strSql.AppendLine("       ");

                        #endregion




                        strSql.AppendLine("  )s full join     ");
                        strSql.AppendLine("  (     ");

                        #region 当月的包装周度计划
                        strSql.AppendLine("  select t1.vcMonth,t1.vcPartsno,t1.vcDock,t1.vcCarType,t1.vcProject1,t1.vcProjectName,t1.vcMonTotal,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD1b,'')<>'' then t2.vcD1b else t1.vcD1b end as vcD1b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD1y,'')<>'' then t2.vcD1y else t1.vcD1y end as vcD1y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD2b,'')<>'' then t2.vcD2b else t1.vcD2b end as vcD2b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD2y,'')<>'' then t2.vcD2y else t1.vcD2y end as vcD2y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD3b,'')<>'' then t2.vcD3b else t1.vcD3b end as vcD3b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD3y,'')<>'' then t2.vcD3y else t1.vcD3y end as vcD3y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD4b,'')<>'' then t2.vcD4b else t1.vcD4b end as vcD4b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD4y,'')<>'' then t2.vcD4y else t1.vcD4y end as vcD4y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD5b,'')<>'' then t2.vcD5b else t1.vcD5b end as vcD5b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD5y,'')<>'' then t2.vcD5y else t1.vcD5y end as vcD5y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD6b,'')<>'' then t2.vcD6b else t1.vcD6b end as vcD6b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD6y,'')<>'' then t2.vcD6y else t1.vcD6y end as vcD6y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD7b,'')<>'' then t2.vcD7b else t1.vcD7b end as vcD7b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD7y,'')<>'' then t2.vcD7y else t1.vcD7y end as vcD7y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD8b,'')<>'' then t2.vcD8b else t1.vcD8b end as vcD8b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD8y,'')<>'' then t2.vcD8y else t1.vcD8y end as vcD8y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD9b,'')<>'' then t2.vcD9b else t1.vcD9b end as vcD9b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD9y,'')<>'' then t2.vcD9y else t1.vcD9y end as vcD9y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD10b,'')<>'' then t2.vcD10b else t1.vcD10b end as vcD10b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD10y,'')<>'' then t2.vcD10y else t1.vcD10y end as vcD10y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD11b,'')<>'' then t2.vcD11b else t1.vcD11b end as vcD11b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD11y,'')<>'' then t2.vcD11y else t1.vcD11y end as vcD11y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD12b,'')<>'' then t2.vcD12b else t1.vcD12b end as vcD12b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD12y,'')<>'' then t2.vcD12y else t1.vcD12y end as vcD12y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD13b,'')<>'' then t2.vcD13b else t1.vcD13b end as vcD13b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD13y,'')<>'' then t2.vcD13y else t1.vcD13y end as vcD13y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD14b,'')<>'' then t2.vcD14b else t1.vcD14b end as vcD14b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD14y,'')<>'' then t2.vcD14y else t1.vcD14y end as vcD14y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD15b,'')<>'' then t2.vcD15b else t1.vcD15b end as vcD15b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD15y,'')<>'' then t2.vcD15y else t1.vcD15y end as vcD15y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD16b,'')<>'' then t2.vcD16b else t1.vcD16b end as vcD16b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD16y,'')<>'' then t2.vcD16y else t1.vcD16y end as vcD16y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD17b,'')<>'' then t2.vcD17b else t1.vcD17b end as vcD17b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD17y,'')<>'' then t2.vcD17y else t1.vcD17y end as vcD17y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD18b,'')<>'' then t2.vcD18b else t1.vcD18b end as vcD18b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD18y,'')<>'' then t2.vcD18y else t1.vcD18y end as vcD18y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD19b,'')<>'' then t2.vcD19b else t1.vcD19b end as vcD19b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD19y,'')<>'' then t2.vcD19y else t1.vcD19y end as vcD19y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD20b,'')<>'' then t2.vcD20b else t1.vcD20b end as vcD20b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD20y,'')<>'' then t2.vcD20y else t1.vcD20y end as vcD20y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD21b,'')<>'' then t2.vcD21b else t1.vcD21b end as vcD21b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD21y,'')<>'' then t2.vcD21y else t1.vcD21y end as vcD21y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD22b,'')<>'' then t2.vcD22b else t1.vcD22b end as vcD22b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD22y,'')<>'' then t2.vcD22y else t1.vcD22y end as vcD22y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD23b,'')<>'' then t2.vcD23b else t1.vcD23b end as vcD23b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD23y,'')<>'' then t2.vcD23y else t1.vcD23y end as vcD23y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD24b,'')<>'' then t2.vcD24b else t1.vcD24b end as vcD24b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD24y,'')<>'' then t2.vcD24y else t1.vcD24y end as vcD24y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD25b,'')<>'' then t2.vcD25b else t1.vcD25b end as vcD25b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD25y,'')<>'' then t2.vcD25y else t1.vcD25y end as vcD25y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD26b,'')<>'' then t2.vcD26b else t1.vcD26b end as vcD26b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD26y,'')<>'' then t2.vcD26y else t1.vcD26y end as vcD26y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD27b,'')<>'' then t2.vcD27b else t1.vcD27b end as vcD27b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD27y,'')<>'' then t2.vcD27y else t1.vcD27y end as vcD27y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD28b,'')<>'' then t2.vcD28b else t1.vcD28b end as vcD28b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD28y,'')<>'' then t2.vcD28y else t1.vcD28y end as vcD28y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD29b,'')<>'' then t2.vcD29b else t1.vcD29b end as vcD29b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD29y,'')<>'' then t2.vcD29y else t1.vcD29y end as vcD29y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD30b,'')<>'' then t2.vcD30b else t1.vcD30b end as vcD30b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD30y,'')<>'' then t2.vcD30y else t1.vcD30y end as vcD30y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD31b,'')<>'' then t2.vcD31b else t1.vcD31b end as vcD31b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD31y,'')<>'' then t2.vcD31y else t1.vcD31y end as vcD31y,     ");
                        strSql.AppendLine("  t1.montouch,t1.DADDTIME,t1.DUPDTIME,t1.CUPDUSER,t1.vcSupplier_id     ");
                        strSql.AppendLine("  from      ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("  select * from WeekPackPlanTbl where vcMonth='" + dFtime + "' and isnull(montouch,'')='' and (isnull(vcMonTotal,'')<>'')     ");
                        strSql.AppendLine("  )t1     ");
                        strSql.AppendLine("  left join     ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("  select * from WeekPackPlanTbl	where montouch='" + dFtime + "'     ");
                        strSql.AppendLine("  )t2 on t1.vcPartsno=t2.vcPartsno     ");
                        #endregion

                        strSql.AppendLine("    )ss on s.vcPartsno=ss.vcPartsno   ");



                        strSql.AppendLine("    )a                                                           ");
                        if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                        {
                            strSql.AppendLine("    left join                                                   ");
                            strSql.AppendLine("    (                                                            ");

                            strSql.AppendLine("  select      ");
                            strSql.AppendLine("    case when isnull(cc.vcMonth,'')<>'' then cc.vcMonth else cc1.vcMonth end as vcMonth,     ");
                            strSql.AppendLine("    case when isnull(cc.vcPartsno,'')<>'' then cc.vcPartsno else cc1.vcPartsno end as vcPartsno,     ");
                            strSql.AppendLine("    case when isnull(cc.vcDock,'')<>'' then cc.vcDock else cc1.vcDock end as vcDock,     ");
                            strSql.AppendLine("    case when isnull(cc.vcCarType,'')<>'' then cc.vcCarType else cc1.vcCarType end as vcCarType,     ");
                            strSql.AppendLine("    case when isnull(cc.vcProject1,'')<>'' then cc.vcProject1 else cc1.vcProject1 end as vcProject1,     ");
                            strSql.AppendLine("    case when isnull(cc.vcProjectName,'')<>'' then cc.vcProjectName else cc1.vcProjectName end as vcProjectName,     ");
                            strSql.AppendLine("    case when isnull(cc.vcMonTotal,'')<>'' then cc.vcMonTotal else cc1.vcMonTotal end as vcMonTotal,       ");

                            strSql.AppendLine("  case when isnull(cc1.vcD1b,'')<>''then cc1.vcD1b else cc.vcD1b end as vcD1b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD2b,'')<>''then cc1.vcD2b else cc.vcD2b end as vcD2b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD3b,'')<>''then cc1.vcD3b else cc.vcD3b end as vcD3b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD4b,'')<>''then cc1.vcD4b else cc.vcD4b end as vcD4b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD5b,'')<>''then cc1.vcD5b else cc.vcD5b end as vcD5b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD6b,'')<>''then cc1.vcD6b else cc.vcD6b end as vcD6b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD7b,'')<>''then cc1.vcD7b else cc.vcD7b end as vcD7b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD8b,'')<>''then cc1.vcD8b else cc.vcD8b end as vcD8b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD9b,'')<>''then cc1.vcD9b else cc.vcD9b end as vcD9b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD10b,'')<>''then cc1.vcD10b else cc.vcD10b end as vcD10b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD1y,'')<>''then cc1.vcD1y else cc.vcD1y end as vcD1y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD2y,'')<>''then cc1.vcD2y else cc.vcD2y end as vcD2y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD3y,'')<>''then cc1.vcD3y else cc.vcD3y end as vcD3y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD4y,'')<>''then cc1.vcD4y else cc.vcD4y end as vcD4y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD5y,'')<>''then cc1.vcD5y else cc.vcD5y end as vcD5y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD6y,'')<>''then cc1.vcD6y else cc.vcD6y end as vcD6y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD7y,'')<>''then cc1.vcD7y else cc.vcD7y end as vcD7y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD8y,'')<>''then cc1.vcD8y else cc.vcD8y end as vcD8y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD9y,'')<>''then cc1.vcD9y else cc.vcD9y end as vcD9y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD10y,'')<>''then cc1.vcD10y else cc.vcD10y end as vcD10y,     ");

                            strSql.AppendLine("  case when isnull(cc1.vcD11b,'')<>''then cc1.vcD11b else cc.vcD11b end as vcD11b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD12b,'')<>''then cc1.vcD12b else cc.vcD12b end as vcD12b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD13b,'')<>''then cc1.vcD13b else cc.vcD13b end as vcD13b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD14b,'')<>''then cc1.vcD14b else cc.vcD14b end as vcD14b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD15b,'')<>''then cc1.vcD15b else cc.vcD15b end as vcD15b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD16b,'')<>''then cc1.vcD16b else cc.vcD16b end as vcD16b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD17b,'')<>''then cc1.vcD17b else cc.vcD17b end as vcD17b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD18b,'')<>''then cc1.vcD18b else cc.vcD18b end as vcD18b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD19b,'')<>''then cc1.vcD19b else cc.vcD19b end as vcD19b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD20b,'')<>''then cc1.vcD20b else cc.vcD20b end as vcD20b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD11y,'')<>''then cc1.vcD11y else cc.vcD11y end as vcD11y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD12y,'')<>''then cc1.vcD12y else cc.vcD12y end as vcD12y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD13y,'')<>''then cc1.vcD13y else cc.vcD13y end as vcD13y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD14y,'')<>''then cc1.vcD14y else cc.vcD14y end as vcD14y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD15y,'')<>''then cc1.vcD15y else cc.vcD15y end as vcD15y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD16y,'')<>''then cc1.vcD16y else cc.vcD16y end as vcD16y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD17y,'')<>''then cc1.vcD17y else cc.vcD17y end as vcD17y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD18y,'')<>''then cc1.vcD18y else cc.vcD18y end as vcD18y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD19y,'')<>''then cc1.vcD19y else cc.vcD19y end as vcD19y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD20y,'')<>''then cc1.vcD20y else cc.vcD20y end as vcD20y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD21b,'')<>''then cc1.vcD21b else cc.vcD21b end as vcD21b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD22b,'')<>''then cc1.vcD22b else cc.vcD22b end as vcD22b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD23b,'')<>''then cc1.vcD23b else cc.vcD23b end as vcD23b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD24b,'')<>''then cc1.vcD24b else cc.vcD24b end as vcD24b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD25b,'')<>''then cc1.vcD25b else cc.vcD25b end as vcD25b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD26b,'')<>''then cc1.vcD26b else cc.vcD26b end as vcD26b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD27b,'')<>''then cc1.vcD27b else cc.vcD27b end as vcD27b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD28b,'')<>''then cc1.vcD28b else cc.vcD28b end as vcD28b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD29b,'')<>''then cc1.vcD29b else cc.vcD29b end as vcD29b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD30b,'')<>''then cc1.vcD30b else cc.vcD30b end as vcD30b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD31b,'')<>''then cc1.vcD31b else cc.vcD31b end as vcD31b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD21y,'')<>''then cc1.vcD21y else cc.vcD21y end as vcD21y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD22y,'')<>''then cc1.vcD22y else cc.vcD22y end as vcD22y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD23y,'')<>''then cc1.vcD23y else cc.vcD23y end as vcD23y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD24y,'')<>''then cc1.vcD24y else cc.vcD24y end as vcD24y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD25y,'')<>''then cc1.vcD25y else cc.vcD25y end as vcD25y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD26y,'')<>''then cc1.vcD26y else cc.vcD26y end as vcD26y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD27y,'')<>''then cc1.vcD27y else cc.vcD27y end as vcD27y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD28y,'')<>''then cc1.vcD28y else cc.vcD28y end as vcD28y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD29y,'')<>''then cc1.vcD29y else cc.vcD29y end as vcD29y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD30y,'')<>''then cc1.vcD30y else cc.vcD30y end as vcD30y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD31y,'')<>''then cc1.vcD31y else cc.vcD31y end as vcD31y,     ");
                            strSql.AppendLine("  cc1.montouch,cc1.DADDTIME,cc1.DUPDTIME,cc1.CUPDUSER,cc1.vcSupplier_id     ");
                            strSql.AppendLine("  from(     ");




                            #region 筛选包装计划(含当前月和下月)

                            strSql.AppendLine("  select      ");
                            strSql.AppendLine("   a2.vcMonth,a2.vcPartsno,a2.vcDock,a2.vcCarType,a2.vcProject1,a2.vcProjectName,a2.vcMonTotal,    ");
                            strSql.AppendLine("   case when isnull(b2.vcD1b,'')<>'' then b2.vcD1b else a2.vcD1b end as vcD1b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD1y,'')<>'' then b2.vcD1y else a2.vcD1y end as vcD1y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD2b,'')<>'' then b2.vcD2b else a2.vcD2b end as vcD2b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD2y,'')<>'' then b2.vcD2y else a2.vcD2y end as vcD2y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD3b,'')<>'' then b2.vcD3b else a2.vcD3b end as vcD3b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD3y,'')<>'' then b2.vcD3y else a2.vcD3y end as vcD3y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD4b,'')<>'' then b2.vcD4b else a2.vcD4b end as vcD4b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD4y,'')<>'' then b2.vcD4y else a2.vcD4y end as vcD4y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD5b,'')<>'' then b2.vcD5b else a2.vcD5b end as vcD5b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD5y,'')<>'' then b2.vcD5y else a2.vcD5y end as vcD5y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD6b,'')<>'' then b2.vcD6b else a2.vcD6b end as vcD6b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD6y,'')<>'' then b2.vcD6y else a2.vcD6y end as vcD6y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD7b,'')<>'' then b2.vcD7b else a2.vcD7b end as vcD7b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD7y,'')<>'' then b2.vcD7y else a2.vcD7y end as vcD7y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD8b,'')<>'' then b2.vcD8b else a2.vcD8b end as vcD8b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD8y,'')<>'' then b2.vcD8y else a2.vcD8y end as vcD8y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD9b,'')<>'' then b2.vcD9b else a2.vcD9b end as vcD9b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD9y,'')<>'' then b2.vcD9y else a2.vcD9y end as vcD9y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD10b,'')<>'' then b2.vcD10b else a2.vcD10b end as vcD10b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD10y,'')<>'' then b2.vcD10y else a2.vcD10y end as vcD10y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD11b,'')<>'' then b2.vcD11b else a2.vcD11b end as vcD11b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD11y,'')<>'' then b2.vcD11y else a2.vcD11y end as vcD11y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD12b,'')<>'' then b2.vcD12b else a2.vcD12b end as vcD12b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD12y,'')<>'' then b2.vcD12y else a2.vcD12y end as vcD12y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD13b,'')<>'' then b2.vcD13b else a2.vcD13b end as vcD13b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD13y,'')<>'' then b2.vcD13y else a2.vcD13y end as vcD13y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD14b,'')<>'' then b2.vcD14b else a2.vcD14b end as vcD14b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD14y,'')<>'' then b2.vcD14y else a2.vcD14y end as vcD14y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD15b,'')<>'' then b2.vcD15b else a2.vcD15b end as vcD15b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD15y,'')<>'' then b2.vcD15y else a2.vcD15y end as vcD15y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD16b,'')<>'' then b2.vcD16b else a2.vcD16b end as vcD16b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD16y,'')<>'' then b2.vcD16y else a2.vcD16y end as vcD16y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD17b,'')<>'' then b2.vcD17b else a2.vcD17b end as vcD17b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD17y,'')<>'' then b2.vcD17y else a2.vcD17y end as vcD17y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD18b,'')<>'' then b2.vcD18b else a2.vcD18b end as vcD18b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD18y,'')<>'' then b2.vcD18y else a2.vcD18y end as vcD18y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD19b,'')<>'' then b2.vcD19b else a2.vcD19b end as vcD19b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD19y,'')<>'' then b2.vcD19y else a2.vcD19y end as vcD19y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD20b,'')<>'' then b2.vcD20b else a2.vcD20b end as vcD20b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD20y,'')<>'' then b2.vcD20y else a2.vcD20y end as vcD20y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD21b,'')<>'' then b2.vcD21b else a2.vcD21b end as vcD21b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD21y,'')<>'' then b2.vcD21y else a2.vcD21y end as vcD21y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD22b,'')<>'' then b2.vcD22b else a2.vcD22b end as vcD22b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD22y,'')<>'' then b2.vcD22y else a2.vcD22y end as vcD22y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD23b,'')<>'' then b2.vcD23b else a2.vcD23b end as vcD23b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD23y,'')<>'' then b2.vcD23y else a2.vcD23y end as vcD23y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD24b,'')<>'' then b2.vcD24b else a2.vcD24b end as vcD24b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD24y,'')<>'' then b2.vcD24y else a2.vcD24y end as vcD24y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD25b,'')<>'' then b2.vcD25b else a2.vcD25b end as vcD25b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD25y,'')<>'' then b2.vcD25y else a2.vcD25y end as vcD25y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD26b,'')<>'' then b2.vcD26b else a2.vcD26b end as vcD26b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD26y,'')<>'' then b2.vcD26y else a2.vcD26y end as vcD26y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD27b,'')<>'' then b2.vcD27b else a2.vcD27b end as vcD27b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD27y,'')<>'' then b2.vcD27y else a2.vcD27y end as vcD27y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD28b,'')<>'' then b2.vcD28b else a2.vcD28b end as vcD28b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD28y,'')<>'' then b2.vcD28y else a2.vcD28y end as vcD28y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD29b,'')<>'' then b2.vcD29b else a2.vcD29b end as vcD29b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD29y,'')<>'' then b2.vcD29y else a2.vcD29y end as vcD29y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD30b,'')<>'' then b2.vcD30b else a2.vcD30b end as vcD30b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD30y,'')<>'' then b2.vcD30y else a2.vcD30y end as vcD30y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD31b,'')<>'' then b2.vcD31b else a2.vcD31b end as vcD31b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD31y,'')<>'' then b2.vcD31y else a2.vcD31y end as vcD31y,          ");
                            strSql.AppendLine("   a2.montouch,a2.DADDTIME,a2.DUPDTIME,a2.CUPDUSER,a2.vcSupplier_id        ");
                            strSql.AppendLine("  from (     ");
                            strSql.AppendLine("    select * from MonthPackPlanTbl where montouch='" + dTtime + "'     ");
                            strSql.AppendLine("  )a2      ");
                            strSql.AppendLine("  left join     ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("    select * from MonthPackPlanTbl where isnull(montouch,'')='' and vcMonth='" + dTtime + "'     ");
                            strSql.AppendLine("   )b2 on a2.vcPartsno=b2.vcPartsno     ");
                            strSql.AppendLine("       ");
                            strSql.AppendLine("       ");
                            #endregion






                            strSql.AppendLine("  )cc full join     ");
                            strSql.AppendLine("  (     ");
                            #region 当月和下月的包装计划周度内示
                            strSql.AppendLine("  select t3.vcMonth,t3.vcPartsno,t3.vcDock,t3.vcCarType,t3.vcProject1,t3.vcProjectName,t3.vcMonTotal,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD1b,'')<>'' then t4.vcD1b else t3.vcD1b end as vcD1b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD1y,'')<>'' then t4.vcD1y else t3.vcD1y end as vcD1y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD2b,'')<>'' then t4.vcD2b else t3.vcD2b end as vcD2b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD2y,'')<>'' then t4.vcD2y else t3.vcD2y end as vcD2y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD3b,'')<>'' then t4.vcD3b else t3.vcD3b end as vcD3b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD3y,'')<>'' then t4.vcD3y else t3.vcD3y end as vcD3y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD4b,'')<>'' then t4.vcD4b else t3.vcD4b end as vcD4b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD4y,'')<>'' then t4.vcD4y else t3.vcD4y end as vcD4y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD5b,'')<>'' then t4.vcD5b else t3.vcD5b end as vcD5b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD5y,'')<>'' then t4.vcD5y else t3.vcD5y end as vcD5y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD6b,'')<>'' then t4.vcD6b else t3.vcD6b end as vcD6b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD6y,'')<>'' then t4.vcD6y else t3.vcD6y end as vcD6y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD7b,'')<>'' then t4.vcD7b else t3.vcD7b end as vcD7b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD7y,'')<>'' then t4.vcD7y else t3.vcD7y end as vcD7y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD8b,'')<>'' then t4.vcD8b else t3.vcD8b end as vcD8b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD8y,'')<>'' then t4.vcD8y else t3.vcD8y end as vcD8y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD9b,'')<>'' then t4.vcD9b else t3.vcD9b end as vcD9b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD9y,'')<>'' then t4.vcD9y else t3.vcD9y end as vcD9y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD10b,'')<>'' then t4.vcD10b else t3.vcD10b end as vcD10b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD10y,'')<>'' then t4.vcD10y else t3.vcD10y end as vcD10y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD11b,'')<>'' then t4.vcD11b else t3.vcD11b end as vcD11b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD11y,'')<>'' then t4.vcD11y else t3.vcD11y end as vcD11y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD12b,'')<>'' then t4.vcD12b else t3.vcD12b end as vcD12b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD12y,'')<>'' then t4.vcD12y else t3.vcD12y end as vcD12y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD13b,'')<>'' then t4.vcD13b else t3.vcD13b end as vcD13b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD13y,'')<>'' then t4.vcD13y else t3.vcD13y end as vcD13y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD14b,'')<>'' then t4.vcD14b else t3.vcD14b end as vcD14b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD14y,'')<>'' then t4.vcD14y else t3.vcD14y end as vcD14y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD15b,'')<>'' then t4.vcD15b else t3.vcD15b end as vcD15b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD15y,'')<>'' then t4.vcD15y else t3.vcD15y end as vcD15y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD16b,'')<>'' then t4.vcD16b else t3.vcD16b end as vcD16b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD16y,'')<>'' then t4.vcD16y else t3.vcD16y end as vcD16y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD17b,'')<>'' then t4.vcD17b else t3.vcD17b end as vcD17b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD17y,'')<>'' then t4.vcD17y else t3.vcD17y end as vcD17y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD18b,'')<>'' then t4.vcD18b else t3.vcD18b end as vcD18b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD18y,'')<>'' then t4.vcD18y else t3.vcD18y end as vcD18y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD19b,'')<>'' then t4.vcD19b else t3.vcD19b end as vcD19b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD19y,'')<>'' then t4.vcD19y else t3.vcD19y end as vcD19y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD20b,'')<>'' then t4.vcD20b else t3.vcD20b end as vcD20b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD20y,'')<>'' then t4.vcD20y else t3.vcD20y end as vcD20y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD21b,'')<>'' then t4.vcD21b else t3.vcD21b end as vcD21b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD21y,'')<>'' then t4.vcD21y else t3.vcD21y end as vcD21y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD22b,'')<>'' then t4.vcD22b else t3.vcD22b end as vcD22b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD22y,'')<>'' then t4.vcD22y else t3.vcD22y end as vcD22y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD23b,'')<>'' then t4.vcD23b else t3.vcD23b end as vcD23b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD23y,'')<>'' then t4.vcD23y else t3.vcD23y end as vcD23y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD24b,'')<>'' then t4.vcD24b else t3.vcD24b end as vcD24b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD24y,'')<>'' then t4.vcD24y else t3.vcD24y end as vcD24y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD25b,'')<>'' then t4.vcD25b else t3.vcD25b end as vcD25b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD25y,'')<>'' then t4.vcD25y else t3.vcD25y end as vcD25y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD26b,'')<>'' then t4.vcD26b else t3.vcD26b end as vcD26b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD26y,'')<>'' then t4.vcD26y else t3.vcD26y end as vcD26y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD27b,'')<>'' then t4.vcD27b else t3.vcD27b end as vcD27b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD27y,'')<>'' then t4.vcD27y else t3.vcD27y end as vcD27y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD28b,'')<>'' then t4.vcD28b else t3.vcD28b end as vcD28b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD28y,'')<>'' then t4.vcD28y else t3.vcD28y end as vcD28y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD29b,'')<>'' then t4.vcD29b else t3.vcD29b end as vcD29b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD29y,'')<>'' then t4.vcD29y else t3.vcD29y end as vcD29y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD30b,'')<>'' then t4.vcD30b else t3.vcD30b end as vcD30b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD30y,'')<>'' then t4.vcD30y else t3.vcD30y end as vcD30y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD31b,'')<>'' then t4.vcD31b else t3.vcD31b end as vcD31b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD31y,'')<>'' then t4.vcD31y else t3.vcD31y end as vcD31y,     ");
                            strSql.AppendLine("  t3.montouch,t3.DADDTIME,t3.DUPDTIME,t3.CUPDUSER,t3.vcSupplier_id     ");
                            strSql.AppendLine("  from      ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("  select * from WeekPackPlanTbl where vcMonth='" + dTtime + "' and isnull(montouch,'')='' and (isnull(vcMonTotal,'')<>'')     ");
                            strSql.AppendLine("  )t3     ");
                            strSql.AppendLine("  left join     ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("  select * from WeekPackPlanTbl	where montouch='" + dTtime + "'     ");
                            strSql.AppendLine("  )t4 on t3.vcPartsno=t4.vcPartsno     ");
                            #endregion
                            strSql.AppendLine("    )cc1 on cc.vcPartsno=cc1.vcPartsno   ");
                            strSql.AppendLine("    )c on a.vcPartsno=c.vcPartsno                                ");
                        }
                        #endregion
                        break;
                    case "2":


                        strSql.AppendLine("  select      ");
                        strSql.AppendLine("    case when isnull(s.vcMonth,'')<>'' then s.vcMonth else ss.vcMonth end as vcMonth,     ");
                        strSql.AppendLine("    case when isnull(s.vcPartsno,'')<>'' then s.vcPartsno else ss.vcPartsno end as vcPartsno,     ");
                        strSql.AppendLine("    case when isnull(s.vcDock,'')<>'' then s.vcDock else ss.vcDock end as vcDock,     ");
                        strSql.AppendLine("    case when isnull(s.vcCarType,'')<>'' then s.vcCarType else ss.vcCarType end as vcCarType,     ");
                        strSql.AppendLine("    case when isnull(s.vcProject1,'')<>'' then s.vcProject1 else ss.vcProject1 end as vcProject1,     ");
                        strSql.AppendLine("    case when isnull(s.vcProjectName,'')<>'' then s.vcProjectName else ss.vcProjectName end as vcProjectName,     ");
                        strSql.AppendLine("    case when isnull(s.vcMonTotal,'')<>'' then s.vcMonTotal else ss.vcMonTotal end as vcMonTotal,       ");

                        strSql.AppendLine("  case when isnull(ss.vcD1b,'')<>''then ss.vcD1b else s.vcD1b end as vcD1b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD2b,'')<>''then ss.vcD2b else s.vcD2b end as vcD2b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD3b,'')<>''then ss.vcD3b else s.vcD3b end as vcD3b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD4b,'')<>''then ss.vcD4b else s.vcD4b end as vcD4b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD5b,'')<>''then ss.vcD5b else s.vcD5b end as vcD5b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD6b,'')<>''then ss.vcD6b else s.vcD6b end as vcD6b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD7b,'')<>''then ss.vcD7b else s.vcD7b end as vcD7b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD8b,'')<>''then ss.vcD8b else s.vcD8b end as vcD8b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD9b,'')<>''then ss.vcD9b else s.vcD9b end as vcD9b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD10b,'')<>''then ss.vcD10b else s.vcD10b end as vcD10b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD1y,'')<>''then ss.vcD1y else s.vcD1y end as vcD1y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD2y,'')<>''then ss.vcD2y else s.vcD2y end as vcD2y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD3y,'')<>''then ss.vcD3y else s.vcD3y end as vcD3y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD4y,'')<>''then ss.vcD4y else s.vcD4y end as vcD4y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD5y,'')<>''then ss.vcD5y else s.vcD5y end as vcD5y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD6y,'')<>''then ss.vcD6y else s.vcD6y end as vcD6y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD7y,'')<>''then ss.vcD7y else s.vcD7y end as vcD7y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD8y,'')<>''then ss.vcD8y else s.vcD8y end as vcD8y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD9y,'')<>''then ss.vcD9y else s.vcD9y end as vcD9y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD10y,'')<>''then ss.vcD10y else s.vcD10y end as vcD10y,     ");

                        strSql.AppendLine("  case when isnull(ss.vcD11b,'')<>''then ss.vcD11b else s.vcD11b end as vcD11b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD12b,'')<>''then ss.vcD12b else s.vcD12b end as vcD12b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD13b,'')<>''then ss.vcD13b else s.vcD13b end as vcD13b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD14b,'')<>''then ss.vcD14b else s.vcD14b end as vcD14b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD15b,'')<>''then ss.vcD15b else s.vcD15b end as vcD15b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD16b,'')<>''then ss.vcD16b else s.vcD16b end as vcD16b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD17b,'')<>''then ss.vcD17b else s.vcD17b end as vcD17b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD18b,'')<>''then ss.vcD18b else s.vcD18b end as vcD18b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD19b,'')<>''then ss.vcD19b else s.vcD19b end as vcD19b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD20b,'')<>''then ss.vcD20b else s.vcD20b end as vcD20b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD11y,'')<>''then ss.vcD11y else s.vcD11y end as vcD11y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD12y,'')<>''then ss.vcD12y else s.vcD12y end as vcD12y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD13y,'')<>''then ss.vcD13y else s.vcD13y end as vcD13y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD14y,'')<>''then ss.vcD14y else s.vcD14y end as vcD14y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD15y,'')<>''then ss.vcD15y else s.vcD15y end as vcD15y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD16y,'')<>''then ss.vcD16y else s.vcD16y end as vcD16y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD17y,'')<>''then ss.vcD17y else s.vcD17y end as vcD17y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD18y,'')<>''then ss.vcD18y else s.vcD18y end as vcD18y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD19y,'')<>''then ss.vcD19y else s.vcD19y end as vcD19y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD20y,'')<>''then ss.vcD20y else s.vcD20y end as vcD20y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD21b,'')<>''then ss.vcD21b else s.vcD21b end as vcD21b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD22b,'')<>''then ss.vcD22b else s.vcD22b end as vcD22b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD23b,'')<>''then ss.vcD23b else s.vcD23b end as vcD23b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD24b,'')<>''then ss.vcD24b else s.vcD24b end as vcD24b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD25b,'')<>''then ss.vcD25b else s.vcD25b end as vcD25b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD26b,'')<>''then ss.vcD26b else s.vcD26b end as vcD26b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD27b,'')<>''then ss.vcD27b else s.vcD27b end as vcD27b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD28b,'')<>''then ss.vcD28b else s.vcD28b end as vcD28b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD29b,'')<>''then ss.vcD29b else s.vcD29b end as vcD29b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD30b,'')<>''then ss.vcD30b else s.vcD30b end as vcD30b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD31b,'')<>''then ss.vcD31b else s.vcD31b end as vcD31b,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD21y,'')<>''then ss.vcD21y else s.vcD21y end as vcD21y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD22y,'')<>''then ss.vcD22y else s.vcD22y end as vcD22y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD23y,'')<>''then ss.vcD23y else s.vcD23y end as vcD23y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD24y,'')<>''then ss.vcD24y else s.vcD24y end as vcD24y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD25y,'')<>''then ss.vcD25y else s.vcD25y end as vcD25y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD26y,'')<>''then ss.vcD26y else s.vcD26y end as vcD26y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD27y,'')<>''then ss.vcD27y else s.vcD27y end as vcD27y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD28y,'')<>''then ss.vcD28y else s.vcD28y end as vcD28y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD29y,'')<>''then ss.vcD29y else s.vcD29y end as vcD29y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD30y,'')<>''then ss.vcD30y else s.vcD30y end as vcD30y,     ");
                        strSql.AppendLine("  case when isnull(ss.vcD31y,'')<>''then ss.vcD31y else s.vcD31y end as vcD31y,     ");
                        strSql.AppendLine("  s.montouch,s.DADDTIME,s.DUPDTIME,s.CUPDUSER,s.vcSupplier_id     ");
                        strSql.AppendLine("  from(     ");



                        #region 当月看板计划月度内示

                        strSql.AppendLine("  select      ");
                        strSql.AppendLine("   a1.vcMonth,a1.vcPartsno,a1.vcDock,a1.vcCarType,a1.vcProject1,a1.vcProjectName,a1.vcMonTotal,    ");
                        strSql.AppendLine("   case when isnull(b1.vcD1b,'')<>'' then b1.vcD1b else a1.vcD1b end as vcD1b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD1y,'')<>'' then b1.vcD1y else a1.vcD1y end as vcD1y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD2b,'')<>'' then b1.vcD2b else a1.vcD2b end as vcD2b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD2y,'')<>'' then b1.vcD2y else a1.vcD2y end as vcD2y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD3b,'')<>'' then b1.vcD3b else a1.vcD3b end as vcD3b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD3y,'')<>'' then b1.vcD3y else a1.vcD3y end as vcD3y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD4b,'')<>'' then b1.vcD4b else a1.vcD4b end as vcD4b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD4y,'')<>'' then b1.vcD4y else a1.vcD4y end as vcD4y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD5b,'')<>'' then b1.vcD5b else a1.vcD5b end as vcD5b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD5y,'')<>'' then b1.vcD5y else a1.vcD5y end as vcD5y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD6b,'')<>'' then b1.vcD6b else a1.vcD6b end as vcD6b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD6y,'')<>'' then b1.vcD6y else a1.vcD6y end as vcD6y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD7b,'')<>'' then b1.vcD7b else a1.vcD7b end as vcD7b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD7y,'')<>'' then b1.vcD7y else a1.vcD7y end as vcD7y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD8b,'')<>'' then b1.vcD8b else a1.vcD8b end as vcD8b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD8y,'')<>'' then b1.vcD8y else a1.vcD8y end as vcD8y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD9b,'')<>'' then b1.vcD9b else a1.vcD9b end as vcD9b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD9y,'')<>'' then b1.vcD9y else a1.vcD9y end as vcD9y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD10b,'')<>'' then b1.vcD10b else a1.vcD10b end as vcD10b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD10y,'')<>'' then b1.vcD10y else a1.vcD10y end as vcD10y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD11b,'')<>'' then b1.vcD11b else a1.vcD11b end as vcD11b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD11y,'')<>'' then b1.vcD11y else a1.vcD11y end as vcD11y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD12b,'')<>'' then b1.vcD12b else a1.vcD12b end as vcD12b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD12y,'')<>'' then b1.vcD12y else a1.vcD12y end as vcD12y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD13b,'')<>'' then b1.vcD13b else a1.vcD13b end as vcD13b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD13y,'')<>'' then b1.vcD13y else a1.vcD13y end as vcD13y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD14b,'')<>'' then b1.vcD14b else a1.vcD14b end as vcD14b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD14y,'')<>'' then b1.vcD14y else a1.vcD14y end as vcD14y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD15b,'')<>'' then b1.vcD15b else a1.vcD15b end as vcD15b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD15y,'')<>'' then b1.vcD15y else a1.vcD15y end as vcD15y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD16b,'')<>'' then b1.vcD16b else a1.vcD16b end as vcD16b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD16y,'')<>'' then b1.vcD16y else a1.vcD16y end as vcD16y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD17b,'')<>'' then b1.vcD17b else a1.vcD17b end as vcD17b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD17y,'')<>'' then b1.vcD17y else a1.vcD17y end as vcD17y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD18b,'')<>'' then b1.vcD18b else a1.vcD18b end as vcD18b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD18y,'')<>'' then b1.vcD18y else a1.vcD18y end as vcD18y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD19b,'')<>'' then b1.vcD19b else a1.vcD19b end as vcD19b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD19y,'')<>'' then b1.vcD19y else a1.vcD19y end as vcD19y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD20b,'')<>'' then b1.vcD20b else a1.vcD20b end as vcD20b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD20y,'')<>'' then b1.vcD20y else a1.vcD20y end as vcD20y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD21b,'')<>'' then b1.vcD21b else a1.vcD21b end as vcD21b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD21y,'')<>'' then b1.vcD21y else a1.vcD21y end as vcD21y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD22b,'')<>'' then b1.vcD22b else a1.vcD22b end as vcD22b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD22y,'')<>'' then b1.vcD22y else a1.vcD22y end as vcD22y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD23b,'')<>'' then b1.vcD23b else a1.vcD23b end as vcD23b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD23y,'')<>'' then b1.vcD23y else a1.vcD23y end as vcD23y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD24b,'')<>'' then b1.vcD24b else a1.vcD24b end as vcD24b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD24y,'')<>'' then b1.vcD24y else a1.vcD24y end as vcD24y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD25b,'')<>'' then b1.vcD25b else a1.vcD25b end as vcD25b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD25y,'')<>'' then b1.vcD25y else a1.vcD25y end as vcD25y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD26b,'')<>'' then b1.vcD26b else a1.vcD26b end as vcD26b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD26y,'')<>'' then b1.vcD26y else a1.vcD26y end as vcD26y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD27b,'')<>'' then b1.vcD27b else a1.vcD27b end as vcD27b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD27y,'')<>'' then b1.vcD27y else a1.vcD27y end as vcD27y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD28b,'')<>'' then b1.vcD28b else a1.vcD28b end as vcD28b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD28y,'')<>'' then b1.vcD28y else a1.vcD28y end as vcD28y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD29b,'')<>'' then b1.vcD29b else a1.vcD29b end as vcD29b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD29y,'')<>'' then b1.vcD29y else a1.vcD29y end as vcD29y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD30b,'')<>'' then b1.vcD30b else a1.vcD30b end as vcD30b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD30y,'')<>'' then b1.vcD30y else a1.vcD30y end as vcD30y,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD31b,'')<>'' then b1.vcD31b else a1.vcD31b end as vcD31b,          ");
                        strSql.AppendLine("   case when isnull(b1.vcD31y,'')<>'' then b1.vcD31y else a1.vcD31y end as vcD31y,          ");
                        strSql.AppendLine("   a1.montouch,a1.DADDTIME,a1.DUPDTIME,a1.CUPDUSER,a1.vcSupplier_id        ");
                        strSql.AppendLine("  from (     ");
                        strSql.AppendLine("    select * from MonthKanBanPlanTbl where montouch='" + dFtime + "'     ");
                        strSql.AppendLine("  )a1      ");
                        strSql.AppendLine("  left join     ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("    select * from MonthKanBanPlanTbl where isnull(montouch,'')='' and vcMonth='" + dFtime + "'     ");
                        strSql.AppendLine("   )b1 on a1.vcPartsno=b1.vcPartsno     ");
                        strSql.AppendLine("       ");
                        strSql.AppendLine("       ");

                        #endregion






                        strSql.AppendLine("  )s full join     ");
                        strSql.AppendLine("  (     ");

                        strSql.AppendLine("  select t1.vcMonth,t1.vcPartsno,t1.vcDock,t1.vcCarType,t1.vcProject1,t1.vcProjectName,t1.vcMonTotal,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD1b,'')<>'' then t2.vcD1b else t1.vcD1b end as vcD1b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD1y,'')<>'' then t2.vcD1y else t1.vcD1y end as vcD1y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD2b,'')<>'' then t2.vcD2b else t1.vcD2b end as vcD2b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD2y,'')<>'' then t2.vcD2y else t1.vcD2y end as vcD2y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD3b,'')<>'' then t2.vcD3b else t1.vcD3b end as vcD3b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD3y,'')<>'' then t2.vcD3y else t1.vcD3y end as vcD3y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD4b,'')<>'' then t2.vcD4b else t1.vcD4b end as vcD4b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD4y,'')<>'' then t2.vcD4y else t1.vcD4y end as vcD4y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD5b,'')<>'' then t2.vcD5b else t1.vcD5b end as vcD5b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD5y,'')<>'' then t2.vcD5y else t1.vcD5y end as vcD5y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD6b,'')<>'' then t2.vcD6b else t1.vcD6b end as vcD6b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD6y,'')<>'' then t2.vcD6y else t1.vcD6y end as vcD6y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD7b,'')<>'' then t2.vcD7b else t1.vcD7b end as vcD7b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD7y,'')<>'' then t2.vcD7y else t1.vcD7y end as vcD7y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD8b,'')<>'' then t2.vcD8b else t1.vcD8b end as vcD8b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD8y,'')<>'' then t2.vcD8y else t1.vcD8y end as vcD8y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD9b,'')<>'' then t2.vcD9b else t1.vcD9b end as vcD9b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD9y,'')<>'' then t2.vcD9y else t1.vcD9y end as vcD9y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD10b,'')<>'' then t2.vcD10b else t1.vcD10b end as vcD10b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD10y,'')<>'' then t2.vcD10y else t1.vcD10y end as vcD10y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD11b,'')<>'' then t2.vcD11b else t1.vcD11b end as vcD11b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD11y,'')<>'' then t2.vcD11y else t1.vcD11y end as vcD11y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD12b,'')<>'' then t2.vcD12b else t1.vcD12b end as vcD12b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD12y,'')<>'' then t2.vcD12y else t1.vcD12y end as vcD12y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD13b,'')<>'' then t2.vcD13b else t1.vcD13b end as vcD13b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD13y,'')<>'' then t2.vcD13y else t1.vcD13y end as vcD13y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD14b,'')<>'' then t2.vcD14b else t1.vcD14b end as vcD14b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD14y,'')<>'' then t2.vcD14y else t1.vcD14y end as vcD14y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD15b,'')<>'' then t2.vcD15b else t1.vcD15b end as vcD15b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD15y,'')<>'' then t2.vcD15y else t1.vcD15y end as vcD15y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD16b,'')<>'' then t2.vcD16b else t1.vcD16b end as vcD16b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD16y,'')<>'' then t2.vcD16y else t1.vcD16y end as vcD16y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD17b,'')<>'' then t2.vcD17b else t1.vcD17b end as vcD17b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD17y,'')<>'' then t2.vcD17y else t1.vcD17y end as vcD17y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD18b,'')<>'' then t2.vcD18b else t1.vcD18b end as vcD18b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD18y,'')<>'' then t2.vcD18y else t1.vcD18y end as vcD18y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD19b,'')<>'' then t2.vcD19b else t1.vcD19b end as vcD19b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD19y,'')<>'' then t2.vcD19y else t1.vcD19y end as vcD19y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD20b,'')<>'' then t2.vcD20b else t1.vcD20b end as vcD20b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD20y,'')<>'' then t2.vcD20y else t1.vcD20y end as vcD20y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD21b,'')<>'' then t2.vcD21b else t1.vcD21b end as vcD21b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD21y,'')<>'' then t2.vcD21y else t1.vcD21y end as vcD21y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD22b,'')<>'' then t2.vcD22b else t1.vcD22b end as vcD22b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD22y,'')<>'' then t2.vcD22y else t1.vcD22y end as vcD22y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD23b,'')<>'' then t2.vcD23b else t1.vcD23b end as vcD23b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD23y,'')<>'' then t2.vcD23y else t1.vcD23y end as vcD23y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD24b,'')<>'' then t2.vcD24b else t1.vcD24b end as vcD24b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD24y,'')<>'' then t2.vcD24y else t1.vcD24y end as vcD24y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD25b,'')<>'' then t2.vcD25b else t1.vcD25b end as vcD25b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD25y,'')<>'' then t2.vcD25y else t1.vcD25y end as vcD25y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD26b,'')<>'' then t2.vcD26b else t1.vcD26b end as vcD26b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD26y,'')<>'' then t2.vcD26y else t1.vcD26y end as vcD26y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD27b,'')<>'' then t2.vcD27b else t1.vcD27b end as vcD27b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD27y,'')<>'' then t2.vcD27y else t1.vcD27y end as vcD27y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD28b,'')<>'' then t2.vcD28b else t1.vcD28b end as vcD28b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD28y,'')<>'' then t2.vcD28y else t1.vcD28y end as vcD28y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD29b,'')<>'' then t2.vcD29b else t1.vcD29b end as vcD29b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD29y,'')<>'' then t2.vcD29y else t1.vcD29y end as vcD29y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD30b,'')<>'' then t2.vcD30b else t1.vcD30b end as vcD30b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD30y,'')<>'' then t2.vcD30y else t1.vcD30y end as vcD30y,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD31b,'')<>'' then t2.vcD31b else t1.vcD31b end as vcD31b,     ");
                        strSql.AppendLine("  case when isnull(t2.vcD31y,'')<>'' then t2.vcD31y else t1.vcD31y end as vcD31y,     ");
                        strSql.AppendLine("  t1.montouch,t1.DADDTIME,t1.DUPDTIME,t1.CUPDUSER,t1.vcSupplier_id     ");
                        strSql.AppendLine("  from      ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("  select * from WeekKanBanPlanTbl where vcMonth='" + dFtime + "' and isnull(montouch,'')='' and (isnull(vcMonTotal,'')<>'')     ");
                        strSql.AppendLine("  )t1     ");
                        strSql.AppendLine("  left join     ");
                        strSql.AppendLine("  (     ");
                        strSql.AppendLine("  select * from WeekKanBanPlanTbl	where montouch='" + dFtime + "'     ");
                        strSql.AppendLine("  )t2 on t1.vcPartsno=t2.vcPartsno     ");
                        strSql.AppendLine("  )ss on s.vcPartsno=ss.vcPartsno      ");

                        strSql.AppendLine("    )a                                                           ");
                        if (dFtime.Split("-")[1] != dTtime.Split("-")[1])
                        {
                            strSql.AppendLine("    left join                                                   ");
                            strSql.AppendLine("    (                                                            ");

                            strSql.AppendLine("  select      ");
                            strSql.AppendLine("    case when isnull(cc.vcMonth,'')<>'' then cc.vcMonth else cc1.vcMonth end as vcMonth,     ");
                            strSql.AppendLine("    case when isnull(cc.vcPartsno,'')<>'' then cc.vcPartsno else cc1.vcPartsno end as vcPartsno,     ");
                            strSql.AppendLine("    case when isnull(cc.vcDock,'')<>'' then cc.vcDock else cc1.vcDock end as vcDock,     ");
                            strSql.AppendLine("    case when isnull(cc.vcCarType,'')<>'' then cc.vcCarType else cc1.vcCarType end as vcCarType,     ");
                            strSql.AppendLine("    case when isnull(cc.vcProject1,'')<>'' then cc.vcProject1 else cc1.vcProject1 end as vcProject1,     ");
                            strSql.AppendLine("    case when isnull(cc.vcProjectName,'')<>'' then cc.vcProjectName else cc1.vcProjectName end as vcProjectName,     ");
                            strSql.AppendLine("    case when isnull(cc.vcMonTotal,'')<>'' then cc.vcMonTotal else cc1.vcMonTotal end as vcMonTotal,       ");

                            strSql.AppendLine("  case when isnull(cc1.vcD1b,'')<>''then cc1.vcD1b else cc.vcD1b end as vcD1b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD2b,'')<>''then cc1.vcD2b else cc.vcD2b end as vcD2b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD3b,'')<>''then cc1.vcD3b else cc.vcD3b end as vcD3b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD4b,'')<>''then cc1.vcD4b else cc.vcD4b end as vcD4b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD5b,'')<>''then cc1.vcD5b else cc.vcD5b end as vcD5b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD6b,'')<>''then cc1.vcD6b else cc.vcD6b end as vcD6b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD7b,'')<>''then cc1.vcD7b else cc.vcD7b end as vcD7b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD8b,'')<>''then cc1.vcD8b else cc.vcD8b end as vcD8b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD9b,'')<>''then cc1.vcD9b else cc.vcD9b end as vcD9b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD10b,'')<>''then cc1.vcD10b else cc.vcD10b end as vcD10b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD1y,'')<>''then cc1.vcD1y else cc.vcD1y end as vcD1y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD2y,'')<>''then cc1.vcD2y else cc.vcD2y end as vcD2y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD3y,'')<>''then cc1.vcD3y else cc.vcD3y end as vcD3y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD4y,'')<>''then cc1.vcD4y else cc.vcD4y end as vcD4y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD5y,'')<>''then cc1.vcD5y else cc.vcD5y end as vcD5y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD6y,'')<>''then cc1.vcD6y else cc.vcD6y end as vcD6y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD7y,'')<>''then cc1.vcD7y else cc.vcD7y end as vcD7y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD8y,'')<>''then cc1.vcD8y else cc.vcD8y end as vcD8y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD9y,'')<>''then cc1.vcD9y else cc.vcD9y end as vcD9y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD10y,'')<>''then cc1.vcD10y else cc.vcD10y end as vcD10y,     ");

                            strSql.AppendLine("  case when isnull(cc1.vcD11b,'')<>''then cc1.vcD11b else cc.vcD11b end as vcD11b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD12b,'')<>''then cc1.vcD12b else cc.vcD12b end as vcD12b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD13b,'')<>''then cc1.vcD13b else cc.vcD13b end as vcD13b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD14b,'')<>''then cc1.vcD14b else cc.vcD14b end as vcD14b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD15b,'')<>''then cc1.vcD15b else cc.vcD15b end as vcD15b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD16b,'')<>''then cc1.vcD16b else cc.vcD16b end as vcD16b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD17b,'')<>''then cc1.vcD17b else cc.vcD17b end as vcD17b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD18b,'')<>''then cc1.vcD18b else cc.vcD18b end as vcD18b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD19b,'')<>''then cc1.vcD19b else cc.vcD19b end as vcD19b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD20b,'')<>''then cc1.vcD20b else cc.vcD20b end as vcD20b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD11y,'')<>''then cc1.vcD11y else cc.vcD11y end as vcD11y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD12y,'')<>''then cc1.vcD12y else cc.vcD12y end as vcD12y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD13y,'')<>''then cc1.vcD13y else cc.vcD13y end as vcD13y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD14y,'')<>''then cc1.vcD14y else cc.vcD14y end as vcD14y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD15y,'')<>''then cc1.vcD15y else cc.vcD15y end as vcD15y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD16y,'')<>''then cc1.vcD16y else cc.vcD16y end as vcD16y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD17y,'')<>''then cc1.vcD17y else cc.vcD17y end as vcD17y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD18y,'')<>''then cc1.vcD18y else cc.vcD18y end as vcD18y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD19y,'')<>''then cc1.vcD19y else cc.vcD19y end as vcD19y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD20y,'')<>''then cc1.vcD20y else cc.vcD20y end as vcD20y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD21b,'')<>''then cc1.vcD21b else cc.vcD21b end as vcD21b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD22b,'')<>''then cc1.vcD22b else cc.vcD22b end as vcD22b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD23b,'')<>''then cc1.vcD23b else cc.vcD23b end as vcD23b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD24b,'')<>''then cc1.vcD24b else cc.vcD24b end as vcD24b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD25b,'')<>''then cc1.vcD25b else cc.vcD25b end as vcD25b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD26b,'')<>''then cc1.vcD26b else cc.vcD26b end as vcD26b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD27b,'')<>''then cc1.vcD27b else cc.vcD27b end as vcD27b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD28b,'')<>''then cc1.vcD28b else cc.vcD28b end as vcD28b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD29b,'')<>''then cc1.vcD29b else cc.vcD29b end as vcD29b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD30b,'')<>''then cc1.vcD30b else cc.vcD30b end as vcD30b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD31b,'')<>''then cc1.vcD31b else cc.vcD31b end as vcD31b,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD21y,'')<>''then cc1.vcD21y else cc.vcD21y end as vcD21y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD22y,'')<>''then cc1.vcD22y else cc.vcD22y end as vcD22y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD23y,'')<>''then cc1.vcD23y else cc.vcD23y end as vcD23y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD24y,'')<>''then cc1.vcD24y else cc.vcD24y end as vcD24y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD25y,'')<>''then cc1.vcD25y else cc.vcD25y end as vcD25y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD26y,'')<>''then cc1.vcD26y else cc.vcD26y end as vcD26y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD27y,'')<>''then cc1.vcD27y else cc.vcD27y end as vcD27y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD28y,'')<>''then cc1.vcD28y else cc.vcD28y end as vcD28y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD29y,'')<>''then cc1.vcD29y else cc.vcD29y end as vcD29y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD30y,'')<>''then cc1.vcD30y else cc.vcD30y end as vcD30y,     ");
                            strSql.AppendLine("  case when isnull(cc1.vcD31y,'')<>''then cc1.vcD31y else cc.vcD31y end as vcD31y,     ");
                            strSql.AppendLine("  cc1.montouch,cc1.DADDTIME,cc1.DUPDTIME,cc1.CUPDUSER,cc1.vcSupplier_id     ");
                            strSql.AppendLine("  from(     ");



                            #region 筛选包装计划(含当前月和下月)

                            strSql.AppendLine("  select      ");
                            strSql.AppendLine("   a2.vcMonth,a2.vcPartsno,a2.vcDock,a2.vcCarType,a2.vcProject1,a2.vcProjectName,a2.vcMonTotal,    ");
                            strSql.AppendLine("   case when isnull(b2.vcD1b,'')<>'' then b2.vcD1b else a2.vcD1b end as vcD1b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD1y,'')<>'' then b2.vcD1y else a2.vcD1y end as vcD1y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD2b,'')<>'' then b2.vcD2b else a2.vcD2b end as vcD2b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD2y,'')<>'' then b2.vcD2y else a2.vcD2y end as vcD2y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD3b,'')<>'' then b2.vcD3b else a2.vcD3b end as vcD3b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD3y,'')<>'' then b2.vcD3y else a2.vcD3y end as vcD3y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD4b,'')<>'' then b2.vcD4b else a2.vcD4b end as vcD4b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD4y,'')<>'' then b2.vcD4y else a2.vcD4y end as vcD4y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD5b,'')<>'' then b2.vcD5b else a2.vcD5b end as vcD5b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD5y,'')<>'' then b2.vcD5y else a2.vcD5y end as vcD5y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD6b,'')<>'' then b2.vcD6b else a2.vcD6b end as vcD6b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD6y,'')<>'' then b2.vcD6y else a2.vcD6y end as vcD6y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD7b,'')<>'' then b2.vcD7b else a2.vcD7b end as vcD7b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD7y,'')<>'' then b2.vcD7y else a2.vcD7y end as vcD7y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD8b,'')<>'' then b2.vcD8b else a2.vcD8b end as vcD8b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD8y,'')<>'' then b2.vcD8y else a2.vcD8y end as vcD8y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD9b,'')<>'' then b2.vcD9b else a2.vcD9b end as vcD9b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD9y,'')<>'' then b2.vcD9y else a2.vcD9y end as vcD9y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD10b,'')<>'' then b2.vcD10b else a2.vcD10b end as vcD10b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD10y,'')<>'' then b2.vcD10y else a2.vcD10y end as vcD10y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD11b,'')<>'' then b2.vcD11b else a2.vcD11b end as vcD11b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD11y,'')<>'' then b2.vcD11y else a2.vcD11y end as vcD11y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD12b,'')<>'' then b2.vcD12b else a2.vcD12b end as vcD12b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD12y,'')<>'' then b2.vcD12y else a2.vcD12y end as vcD12y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD13b,'')<>'' then b2.vcD13b else a2.vcD13b end as vcD13b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD13y,'')<>'' then b2.vcD13y else a2.vcD13y end as vcD13y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD14b,'')<>'' then b2.vcD14b else a2.vcD14b end as vcD14b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD14y,'')<>'' then b2.vcD14y else a2.vcD14y end as vcD14y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD15b,'')<>'' then b2.vcD15b else a2.vcD15b end as vcD15b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD15y,'')<>'' then b2.vcD15y else a2.vcD15y end as vcD15y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD16b,'')<>'' then b2.vcD16b else a2.vcD16b end as vcD16b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD16y,'')<>'' then b2.vcD16y else a2.vcD16y end as vcD16y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD17b,'')<>'' then b2.vcD17b else a2.vcD17b end as vcD17b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD17y,'')<>'' then b2.vcD17y else a2.vcD17y end as vcD17y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD18b,'')<>'' then b2.vcD18b else a2.vcD18b end as vcD18b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD18y,'')<>'' then b2.vcD18y else a2.vcD18y end as vcD18y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD19b,'')<>'' then b2.vcD19b else a2.vcD19b end as vcD19b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD19y,'')<>'' then b2.vcD19y else a2.vcD19y end as vcD19y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD20b,'')<>'' then b2.vcD20b else a2.vcD20b end as vcD20b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD20y,'')<>'' then b2.vcD20y else a2.vcD20y end as vcD20y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD21b,'')<>'' then b2.vcD21b else a2.vcD21b end as vcD21b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD21y,'')<>'' then b2.vcD21y else a2.vcD21y end as vcD21y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD22b,'')<>'' then b2.vcD22b else a2.vcD22b end as vcD22b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD22y,'')<>'' then b2.vcD22y else a2.vcD22y end as vcD22y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD23b,'')<>'' then b2.vcD23b else a2.vcD23b end as vcD23b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD23y,'')<>'' then b2.vcD23y else a2.vcD23y end as vcD23y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD24b,'')<>'' then b2.vcD24b else a2.vcD24b end as vcD24b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD24y,'')<>'' then b2.vcD24y else a2.vcD24y end as vcD24y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD25b,'')<>'' then b2.vcD25b else a2.vcD25b end as vcD25b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD25y,'')<>'' then b2.vcD25y else a2.vcD25y end as vcD25y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD26b,'')<>'' then b2.vcD26b else a2.vcD26b end as vcD26b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD26y,'')<>'' then b2.vcD26y else a2.vcD26y end as vcD26y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD27b,'')<>'' then b2.vcD27b else a2.vcD27b end as vcD27b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD27y,'')<>'' then b2.vcD27y else a2.vcD27y end as vcD27y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD28b,'')<>'' then b2.vcD28b else a2.vcD28b end as vcD28b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD28y,'')<>'' then b2.vcD28y else a2.vcD28y end as vcD28y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD29b,'')<>'' then b2.vcD29b else a2.vcD29b end as vcD29b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD29y,'')<>'' then b2.vcD29y else a2.vcD29y end as vcD29y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD30b,'')<>'' then b2.vcD30b else a2.vcD30b end as vcD30b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD30y,'')<>'' then b2.vcD30y else a2.vcD30y end as vcD30y,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD31b,'')<>'' then b2.vcD31b else a2.vcD31b end as vcD31b,          ");
                            strSql.AppendLine("   case when isnull(b2.vcD31y,'')<>'' then b2.vcD31y else a2.vcD31y end as vcD31y,          ");
                            strSql.AppendLine("   a2.montouch,a2.DADDTIME,a2.DUPDTIME,a2.CUPDUSER,a2.vcSupplier_id        ");
                            strSql.AppendLine("  from (     ");
                            strSql.AppendLine("    select * from MonthKanBanPlanTbl where montouch='" + dTtime + "'     ");
                            strSql.AppendLine("  )a2      ");
                            strSql.AppendLine("  left join     ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("    select * from MonthKanBanPlanTbl where isnull(montouch,'')='' and vcMonth='" + dTtime + "'     ");
                            strSql.AppendLine("   )b2 on a2.vcPartsno=b2.vcPartsno     ");
                            strSql.AppendLine("       ");
                            strSql.AppendLine("       ");
                            #endregion






                            strSql.AppendLine("  )cc full join     ");
                            strSql.AppendLine("  (     ");

                            strSql.AppendLine("  select t3.vcMonth,t3.vcPartsno,t3.vcDock,t3.vcCarType,t3.vcProject1,t3.vcProjectName,t3.vcMonTotal,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD1b,'')<>'' then t4.vcD1b else t3.vcD1b end as vcD1b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD1y,'')<>'' then t4.vcD1y else t3.vcD1y end as vcD1y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD2b,'')<>'' then t4.vcD2b else t3.vcD2b end as vcD2b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD2y,'')<>'' then t4.vcD2y else t3.vcD2y end as vcD2y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD3b,'')<>'' then t4.vcD3b else t3.vcD3b end as vcD3b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD3y,'')<>'' then t4.vcD3y else t3.vcD3y end as vcD3y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD4b,'')<>'' then t4.vcD4b else t3.vcD4b end as vcD4b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD4y,'')<>'' then t4.vcD4y else t3.vcD4y end as vcD4y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD5b,'')<>'' then t4.vcD5b else t3.vcD5b end as vcD5b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD5y,'')<>'' then t4.vcD5y else t3.vcD5y end as vcD5y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD6b,'')<>'' then t4.vcD6b else t3.vcD6b end as vcD6b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD6y,'')<>'' then t4.vcD6y else t3.vcD6y end as vcD6y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD7b,'')<>'' then t4.vcD7b else t3.vcD7b end as vcD7b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD7y,'')<>'' then t4.vcD7y else t3.vcD7y end as vcD7y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD8b,'')<>'' then t4.vcD8b else t3.vcD8b end as vcD8b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD8y,'')<>'' then t4.vcD8y else t3.vcD8y end as vcD8y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD9b,'')<>'' then t4.vcD9b else t3.vcD9b end as vcD9b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD9y,'')<>'' then t4.vcD9y else t3.vcD9y end as vcD9y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD10b,'')<>'' then t4.vcD10b else t3.vcD10b end as vcD10b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD10y,'')<>'' then t4.vcD10y else t3.vcD10y end as vcD10y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD11b,'')<>'' then t4.vcD11b else t3.vcD11b end as vcD11b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD11y,'')<>'' then t4.vcD11y else t3.vcD11y end as vcD11y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD12b,'')<>'' then t4.vcD12b else t3.vcD12b end as vcD12b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD12y,'')<>'' then t4.vcD12y else t3.vcD12y end as vcD12y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD13b,'')<>'' then t4.vcD13b else t3.vcD13b end as vcD13b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD13y,'')<>'' then t4.vcD13y else t3.vcD13y end as vcD13y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD14b,'')<>'' then t4.vcD14b else t3.vcD14b end as vcD14b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD14y,'')<>'' then t4.vcD14y else t3.vcD14y end as vcD14y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD15b,'')<>'' then t4.vcD15b else t3.vcD15b end as vcD15b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD15y,'')<>'' then t4.vcD15y else t3.vcD15y end as vcD15y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD16b,'')<>'' then t4.vcD16b else t3.vcD16b end as vcD16b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD16y,'')<>'' then t4.vcD16y else t3.vcD16y end as vcD16y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD17b,'')<>'' then t4.vcD17b else t3.vcD17b end as vcD17b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD17y,'')<>'' then t4.vcD17y else t3.vcD17y end as vcD17y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD18b,'')<>'' then t4.vcD18b else t3.vcD18b end as vcD18b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD18y,'')<>'' then t4.vcD18y else t3.vcD18y end as vcD18y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD19b,'')<>'' then t4.vcD19b else t3.vcD19b end as vcD19b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD19y,'')<>'' then t4.vcD19y else t3.vcD19y end as vcD19y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD20b,'')<>'' then t4.vcD20b else t3.vcD20b end as vcD20b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD20y,'')<>'' then t4.vcD20y else t3.vcD20y end as vcD20y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD21b,'')<>'' then t4.vcD21b else t3.vcD21b end as vcD21b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD21y,'')<>'' then t4.vcD21y else t3.vcD21y end as vcD21y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD22b,'')<>'' then t4.vcD22b else t3.vcD22b end as vcD22b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD22y,'')<>'' then t4.vcD22y else t3.vcD22y end as vcD22y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD23b,'')<>'' then t4.vcD23b else t3.vcD23b end as vcD23b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD23y,'')<>'' then t4.vcD23y else t3.vcD23y end as vcD23y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD24b,'')<>'' then t4.vcD24b else t3.vcD24b end as vcD24b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD24y,'')<>'' then t4.vcD24y else t3.vcD24y end as vcD24y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD25b,'')<>'' then t4.vcD25b else t3.vcD25b end as vcD25b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD25y,'')<>'' then t4.vcD25y else t3.vcD25y end as vcD25y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD26b,'')<>'' then t4.vcD26b else t3.vcD26b end as vcD26b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD26y,'')<>'' then t4.vcD26y else t3.vcD26y end as vcD26y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD27b,'')<>'' then t4.vcD27b else t3.vcD27b end as vcD27b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD27y,'')<>'' then t4.vcD27y else t3.vcD27y end as vcD27y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD28b,'')<>'' then t4.vcD28b else t3.vcD28b end as vcD28b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD28y,'')<>'' then t4.vcD28y else t3.vcD28y end as vcD28y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD29b,'')<>'' then t4.vcD29b else t3.vcD29b end as vcD29b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD29y,'')<>'' then t4.vcD29y else t3.vcD29y end as vcD29y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD30b,'')<>'' then t4.vcD30b else t3.vcD30b end as vcD30b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD30y,'')<>'' then t4.vcD30y else t3.vcD30y end as vcD30y,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD31b,'')<>'' then t4.vcD31b else t3.vcD31b end as vcD31b,     ");
                            strSql.AppendLine("  case when isnull(t4.vcD31y,'')<>'' then t4.vcD31y else t3.vcD31y end as vcD31y,     ");
                            strSql.AppendLine("  t3.montouch,t3.DADDTIME,t3.DUPDTIME,t3.CUPDUSER,t3.vcSupplier_id     ");
                            strSql.AppendLine("  from      ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("  select * from WeekKanBanPlanTbl where vcMonth='" + dTtime + "' and isnull(montouch,'')='' and (isnull(vcMonTotal,'')<>'')     ");
                            strSql.AppendLine("  )t3     ");
                            strSql.AppendLine("  left join     ");
                            strSql.AppendLine("  (     ");
                            strSql.AppendLine("  select * from WeekKanBanPlanTbl	where montouch='" + dTtime + "'     ");
                            strSql.AppendLine("  )t4 on t3.vcPartsno=t4.vcPartsno     ");

                            strSql.AppendLine("    )cc1 on cc.vcPartsno=cc1.vcPartsno   ");
                            strSql.AppendLine("    )c on a.vcPartsno=c.vcPartsno                                ");
                        }
                        break;
                }
                strSql.AppendLine("    left join                                                   ");
                strSql.AppendLine("    (                                                            ");

                strSql.AppendLine("   select temp1.vcPartsNo,temp2.vcPackNo,temp2.vcPackGPSNo,temp2.iBiYao,temp2.vcSupplierCode,temp2.dFrom1,temp2.dTo1   ");
                strSql.AppendLine("   ,temp2.iRelease,temp2.vcSupplierName   ");
                strSql.AppendLine("   from(   ");
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
                strSql.AppendLine("  )temp1    ");
                strSql.AppendLine("  inner join    ");
                strSql.AppendLine("  (    ");

                strSql.AppendLine("        select t.vcPartsNo,t.vcPackNo,t.vcPackGPSNo,y.iRelease,y.vcSupplierName,y.vcSupplierCode,t.iBiYao,  ");

                strSql.AppendLine("      case when dFrom>'" + dFromBegin + "' then dFrom else '" + dFromBegin + "'end as dFrom1,            ");
                strSql.AppendLine("      case when dTo>'" + dFromEnd + "' then  '" + dFromEnd + "' else dTo  end as dTo1              ");

                strSql.AppendLine("       from(                                                                                           ");


                strSql.AppendLine("      select * from TPackItem where dFrom<='" + dFromEnd + "'and dTo>='" + dFromBegin + "'        ");
                strSql.AppendLine("       )t left join                                                                                 ");
                strSql.AppendLine("       (                                                                                             ");
                strSql.AppendLine("       select * from TPackBase                                                                       ");

                strSql.AppendLine("       )y on t.vcPackNo=y.vcPackNo                                                                   ");
                strSql.AppendLine("       )temp2 on temp1.vcPartsNo=temp2.vcPartsNo              ");
                strSql.AppendLine("   )d on a.vcPartsno=d.vcPartsNo    ");
                strSql.AppendLine("  where isnull(d.vcPackNo,'')<>''    ");


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
              
                DataView dv = listInfoData.DefaultView;
                DataTable dtt = dv.ToTable(true, "vcSupplierCode");
                string dt1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                switch (strKind)
                {
                    case "0":
                        strKind = "生产计划";
                        break;
                    case "1":
                        strKind = "包装计划";
                        break;
                    case "2":
                        strKind = "看板计划";
                        break;
                }

                string Name = strBegin + "/" + strFromBeginBZ + " " + strEnd + "/" + strFromEndBZ + " " + strKind;
                sql.AppendLine("     delete from  TPackWeekInfo     \n");

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
                    sql.AppendLine("         NULL,   \n");
                    sql.AppendLine("         '周度内示',   \n");
                    sql.AppendLine("         '" + Name + "',   \n");
                    sql.AppendLine("         '0',   \n");
                    sql.AppendLine("         '" + dt1 + "',   \n");
                    sql.AppendLine("         NULL,   \n");
                    sql.AppendLine("   '" + strUserId + "',   \n");
                    sql.AppendLine("   getdate()  \n");
                    sql.AppendLine("         )   \n");

                }
               

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
                    if (strBegin.Split("-")[1] != strEnd.Split("-")[1])
                    {
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
                    }

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
                    //else
                    //{
                    //    #region
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',         \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");

                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");

                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    sql.Append("       '',          \r\n");
                    //    #endregion

                    //}

                    sql.Append("       '1' ,                 \r\n");
                    sql.Append("      '0' ,                 \r\n");
                    sql.Append("      '" + Name + "' ,                 \r\n");
                    sql.Append("       '周度内饰' ,                 \r\n");
                    sql.Append("       '" + dt1 + "' ,                 \r\n");
                    sql.Append("      '" + strKind + "' ,                 \r\n");
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
        public void Save_GS(DataTable listInfoData, string strUserId, ref string strErrorName, string strBegin, string strEnd, List<Object> OrderState)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Rows.Count; i++)
                {
                    if (OrderState.IndexOf(listInfoData.Rows[i]["vcSupplierCode"].ToString()) != -1)
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
                        if (strBegin.Split("-")[1] != strEnd.Split("-")[1])
                        {
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
                        }
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


                        sql.Append("       '0' ,                 \r\n");
                        sql.Append("      '0' ,                 \r\n");
                        sql.Append("    '" + strUserId + "'   ,                 \r\n");
                        sql.Append("      getdate()                   \r\n");
                        sql.Append("   )                       \r\n");
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

                strSql.Append("  iRelease ,            \n");
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
                strSql.Append("   group by vcPackNo,vcPackGPSNo,vcSupplierName, vcSupplierCode,iRelease,           \n");

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
