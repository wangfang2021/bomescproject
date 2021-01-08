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
    public class FS1401_DataAccess
    {
        private MultiExcute excute = new MultiExcute();


        public DataSet getDllOptionsList()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '' as HJcode,'' as HJname;--HJOptions");
                strSql.AppendLine("select '' as InOutcode,'' as InOutname;--InOutOptions");
                strSql.AppendLine("select '' as PartAreacode,'' as PartAreaname;--PartAreaOptions");
                strSql.AppendLine("select '' as SPISqufencode,'' as SPISqufenname;--SPISqufenOptions");
                strSql.AppendLine("select '' as CheckPcode,'' as CheckPname;--CheckPOptions");
                strSql.AppendLine("select '' as SPISInPutcode,'' as SPISInPutname;--SPISInPutOptions");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strPartNo, string strSuplier, string strHJ, string strInOut, string strPartArea, string strSPISqufen, string strCheckP,
                   string strTimeFrom, string strTimeTo, string strCarFamily, string strSPISInPut,
                   string strcboxnow, string strcboxtom, string strcboxyes)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
