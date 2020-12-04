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
    public class FS0810_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string smallpm, string sr, string pfbefore5)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.vcSR,t1.vcPartsNoBefore5,t1.vcBCPartsNo,t1.vcSmallPM,t2.vcBigPM,'1' as vcmodflag,'1' as vcaddflag,'uncheck' as cb  \n");
                strSql.Append("from tPMSmall t1  \n");
                strSql.Append("left join tPMRelation t2 on t1.vcSmallPM=t2.vcSmallPM  \n");
                strSql.Append("where ISNULL(t1.vcSmallPM,'') like '%" + smallpm + "%'  \n");
                strSql.Append("and ISNULL(t1.vcSR,'') like '%" + sr + "%'  \n");
                strSql.Append("and ISNULL(t1.vcPartsNoBefore5,'') like '%" + pfbefore5 + "%'  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(DataTable dtadd, DataTable dtmod, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    DataRow dr = dtadd.Rows[i];
                    sql.Append("insert into tPMSmall (vcPartsNoBefore5,vcSR,vcBCPartsNo,vcSmallPM) values   \n");
                    sql.Append("('"+ dr["vcPartsNoBefore5"].ToString() + "','"+ dr["vcSR"].ToString() + "','"+ dr["vcBCPartsNo"].ToString() + "','"+ dr["vcSmallPM"].ToString() + "')  \n");
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    DataRow dr = dtmod.Rows[i];
                    sql.Append("update tPMSmall set vcSmallPM='" + dr["vcSmallPM"].ToString() + "'   \n");
                    sql.Append("where vcPartsNoBefore5='" + dr["vcPartsNoBefore5"].ToString() + "' and vcSR='" + dr["vcSR"].ToString() + "'   \n");
                    sql.Append("and vcBCPartsNo='" + dr["vcBCPartsNo"].ToString() + "'  \n");
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

        #region 删除
        public void Del(DataTable dtdel,string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtdel.Rows.Count; i++)
                {
                    DataRow dr = dtdel.Rows[i];
                    sql.Append("delete from tPMSmall   \n");
                    sql.Append("where vcPartsNoBefore5='" + dr["vcPartsNoBefore5"].ToString() + "' and vcSR='" + dr["vcSR"].ToString() + "'   \n");
                    sql.Append("and vcBCPartsNo='" + dr["vcBCPartsNo"].ToString() + "'  \n");
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

    }
}
