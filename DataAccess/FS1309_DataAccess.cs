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
    public class FS1309_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getPlantInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as Plantcode,'泰达' as Plantname");//Plantcode\Plantname
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
        public DataTable getSearchInfo(string strPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("");//userid\username\rolename\bchecker\check_eableflag\blockchecker\lockcheck_eableflag\bpacker\pack_eableflag\blockpacker\lockpack_eableflag\plant
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
        public void saveDataInfo(DataTable dataTable, string strOperId)
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
                excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
