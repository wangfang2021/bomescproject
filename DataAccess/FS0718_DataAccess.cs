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

        #region 检索月度内饰数据,返回dt(导出下载用)
        public DataTable SearchNSMonth(string strSupplier,string strYearMonth,string strNSState,string dFaBuTime,string dFirstDownload)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("       select * from TPackNSCalculationCV      ");
                strSql.AppendLine("       where 1=1      ");
                if (!string.IsNullOrEmpty(strSupplier))
                {
                    strSql.AppendLine("       and vcSupplierCode = '"+strSupplier+"'      ");
                }
                if (!string.IsNullOrEmpty(strYearMonth))
                {
                    strSql.AppendLine("       and vcYearMonth = '"+strYearMonth+"'      ");
                }
                if (!string.IsNullOrEmpty(strNSState))
                {
                    strSql.AppendLine("       and vcNSState = '"+strNSState+"'      ");
                }
                if (!string.IsNullOrEmpty(dFaBuTime))
                {
                    strSql.AppendLine("       and dSendTime = '"+dFaBuTime+"'      ");
                }
                if (!string.IsNullOrEmpty(dFirstDownload))
                {
                    strSql.AppendLine("       and dFirstDownload = '"+dFirstDownload+"'      ");
                }
                
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索周度,返回dt(导出下载用)
        public DataTable SearchNSWeek()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("       select * from TPackWeekInfoCV      ");
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
