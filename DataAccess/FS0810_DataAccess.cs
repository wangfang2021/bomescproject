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

        #region 检索_品目
        public DataTable Search_PM(string smallpm, string bigpm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select vcBigPM,vcSmallPM,'1' as vcaddflag,'1' as vcmodflag,vcBigPM as vcBigPM_init,vcSmallPM as vcSmallPM_init  \n");
                strSql.Append("from tPMRelation   \n");
                strSql.Append("where isnull(vcBigPM,'') like '%"+bigpm+"%' and isnull(vcSmallPM,'') like '%"+smallpm+"%'  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存_品目
        public void Save_pm(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    DataRow dr = dt.Rows[i];
                    if(dr["vcflag"].ToString()=="add")
                    {
                        sql.Append("insert into tPMRelation (vcBigPM,vcSmallPM) values   \n");
                        sql.Append("('" + dr["vcBigPM"].ToString() + "','" + dr["vcSmallPM"].ToString() + "')  \n");
                    }
                    if(dr["vcflag"].ToString() == "mod")
                    {
                        sql.Append("update tPMRelation set vcBigPM='" + dr["vcBigPM"].ToString() + "',vcSmallPM='" + dr["vcSmallPM"].ToString() + "'   \n");
                        sql.Append("where vcBigPM='" + dr["vcBigPM_init"].ToString() + "' and vcSmallPM='" + dr["vcSmallPM_init"].ToString() + "'   \n");
                    }
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

        #region 取得关系表中所有大品目
        public DataTable GetBigPM()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select distinct vcBigPM from tPMRelation  \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region 取得关系表中所有ih品目
        public DataTable GetSmallPM()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select distinct vcSmallPM from tPMRelation  \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region 删除_品目
        public void Del_pm(DataTable dtdel, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtdel.Rows.Count; i++)
                {
                    DataRow dr = dtdel.Rows[i];
                    sql.Append("delete from tPMRelation   \n");
                    sql.Append("where vcBigPM='" + dr["vcBigPM"].ToString() + "' and vcSmallPM='" + dr["vcSmallPM"].ToString() + "'   \n");
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
