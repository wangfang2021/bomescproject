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
    public class FS0617_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        
        public DataTable getPlantInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as plantcode,'泰达' as plantname");//plantcode\plantname
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
        public DataTable getCarTypeInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as cartypecode,'卡罗拉' as cartypename");//cartypecode\cartypename
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
        public DataTable getRePartyInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as repartycode,'一丰补给' as repartyname");//repartycode\repartyname
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
        public DataTable getSuPartyInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as supartycode,'上海萨克斯' as supartyname");//supartycode\supartyname
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
        public DataTable getSearchInfo(string strPlant, string strPartid, string strCarType, string strReParty, string strSuParty)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select  '10' as fxnum");//eableflag控制枚数可输入
                strSql.AppendLine("		 ,'0' as eableflag");//
                strSql.AppendLine("		 ,'泰达' as plant");
                strSql.AppendLine("		 ,'616120N03000' as partno");
                strSql.AppendLine("		 ,'2017-01-01' as fromtime");
                strSql.AppendLine("		 ,'2021-12-31' as totime");
                strSql.AppendLine("		 ,'280B' as cartype");
                strSql.AppendLine("		 ,'一丰' as reparty");
                strSql.AppendLine("		 ,'萨克斯' as suparty");
                strSql.AppendLine("		 ,'2S' as sr");
                strSql.AppendLine("		 ,'2BT' as bf");
                strSql.AppendLine("		 ,'10' as qty");
                strSql.AppendLine("		 ,'PANEL, FR FENDER, LH' as partname");
                strSql.AppendLine("		 ,'2b-01' as route");
                strSql.AppendLine("		 ,'2A' as adworks");
                strSql.AppendLine("		 ,'2020-11-30' as adworkstime");
                strSql.AppendLine("		 ,'2020-12-02' as outputtime");
                strSql.AppendLine("		 ,'特记事项' as item1,'' as item2");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
