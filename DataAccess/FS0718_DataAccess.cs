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
    public class FS0718_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件检索,返回dt
        public DataTable Search(string strDownloadDiff)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("           select * from TPackSearch where  1=1               \n");
                if (!string.IsNullOrEmpty(strDownloadDiff))
                {
                    strSql.AppendLine("       and dFirstDownload " + strDownloadDiff + "          ");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索月度内示数据,返回dt(导出下载用)
        public DataTable Search_Month(string strSupplier,string strYearMonth,string dFaBuTime)
        {
            try
            {
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
                strSql.Append("      from TPackNSCalculationCV         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if (!string.IsNullOrEmpty(strSupplier))
                {
                    strSql.AppendLine("       and vcSupplierCode = '"+strSupplier+"'      ");
                }
                if (!string.IsNullOrEmpty(strYearMonth))
                {
                    strSql.AppendLine("       and vcYearMonth = '"+strYearMonth+"'      ");
                }
                if (!string.IsNullOrEmpty(dFaBuTime))
                {
                    strSql.AppendLine("       and dSendTime = '"+dFaBuTime+"'      ");
                }
                strSql.Append("     group by vcPackNo, vcYearMonth, vcPackNo,vcPackGPSNo,vcPackSpot,vcSupplierCode,vcSupplierWork,vcSupplierName,vcSupplierPack ,vcCycle,iRelease,dZYTime        \n");
                ComFunction.ConsoleWriteLine("以下位检索SQL语句：");
                ComFunction.ConsoleWriteLine(strSql.ToString());
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索周度,返回dt(导出下载用)
        public DataTable Search_Week(string strSupplierCode,string strDFaBuTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select ''as vcNum, vcPackNo,vcPackGPSNo,vcSupplierName,           \n");
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
                strSql.Append("  sum(vcD29bT) as vcD29bT,sum(vcD30bT) as vcD30bT,sum(vcD31bT) as vcD31bT            \n");
                strSql.Append("              \n");
                strSql.Append("   from TPackWeekInfoCV            \n");
                if (!string.IsNullOrEmpty(strSupplierCode))
                {
                    strSql.Append("   where vcSupplierCode = '"+strSupplierCode+"'             \n");
                }
                if (!string.IsNullOrEmpty(strDFaBuTime))
                {
                    strSql.Append("   and dSendTime = '"+strDFaBuTime+"'             \n");
                }
                strSql.Append("   group by vcPackNo,vcPackGPSNo,vcSupplierName,            \n");

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
                strSql.Append("   ,vcD27yTShow,vcD28yTShow,vcD29yTShow,vcD30yTShow,vcD31yTShow                   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 更新包材内示检索表
        public void updateSearchTable(List<Dictionary<string, Object>> listInfoData,string strUserID)
        {
            StringBuilder strSql = new StringBuilder();
            
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strFaBuTime = listInfoData[i]["dFaBuTime"].ToString().Replace("/","-");
                strSql.AppendLine("      update TPackSearch set dFirstDownload = case when dFirstDownload is null then GETDATE() else dFirstDownload END        ");
                strSql.AppendLine("      ,vcOperatorID = '"+strUserID+"'       ");
                strSql.AppendLine("      ,dOperatorTime = GETDATE()       ");
                strSql.AppendLine("      where dFaBuTime = '"+strFaBuTime+"'       ");
            }
            excute.ExcuteSqlWithStringOper(strSql.ToString());
        }
        #endregion

    }
}
