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
                strSql.Append("select t1.vcSR,t1.vcPartsNoBefore5,t1.vcBCPartsNo,t1.vcSmallPM,t2.vcBigPM,'0' as vcModFlag,'0' as vcAddFlag  \n");
                strSql.Append("from TPMSmall t1  \n");
                strSql.Append("left join TPMRelation t2 on t1.vcSmallPM=t2.vcSmallPM  \n");
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
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    string strSR = listInfoData[i]["vcSR"].ToString();
                    string strPartsNoBefore5 = listInfoData[i]["vcPartsNoBefore5"].ToString();
                    string strBCPartsNo = listInfoData[i]["vcBCPartsNo"].ToString();
                    string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag)
                    {//新增
                        sql.Append("insert into TPMSmall (vcPartsNoBefore5,vcSR,vcBCPartsNo,vcSmallPM) values   \n");
                        sql.Append("('" + strPartsNoBefore5 + "','" + strSR + "','" + strBCPartsNo + "','" + strSmallPM + "')  \n");
                    }
                    else
                    {
                        sql.Append("update TPMSmall set vcSmallPM='" + strSmallPM + "'   \n");
                        sql.Append("where vcPartsNoBefore5='" + strPartsNoBefore5 + "' and vcSR='" + strSR + "'   \n");
                        sql.Append("and vcBCPartsNo='" + strBCPartsNo + "'  \n");
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

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string vcPartsNoBefore5= checkedInfoData[i]["vcPartsNoBefore5"].ToString();
                    string vcSR= checkedInfoData[i]["vcSR"].ToString();
                    string vcBCPartsNo= checkedInfoData[i]["vcBCPartsNo"].ToString();

                    sql.Append("delete from TPMSmall   \n");
                    sql.Append("where vcPartsNoBefore5='" + vcPartsNoBefore5 + "' and vcSR='" + vcSR + "'   \n");
                    sql.Append("and vcBCPartsNo='" + vcBCPartsNo + "'  \n");
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
                strSql.Append("select vcBigPM,vcSmallPM,'0' as vcAddFlag,'0' as vcModFlag,vcBigPM as vcBigPM_init,vcSmallPM as vcSmallPM_init  \n");
                strSql.Append("from TPMRelation   \n");
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
        public void Save_pm(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for(int i=0;i< listInfoData.Count;i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    string strBigPM = listInfoData[i]["vcBigPM"].ToString();
                    string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();
                    string strBigPM_init = listInfoData[i]["vcBigPM_init"].ToString();
                    string strSmallPM_init = listInfoData[i]["vcSmallPM_init"].ToString();

                    if (baddflag)
                    {//新增
                        sql.Append("insert into TPMRelation (vcBigPM,vcSmallPM) values   \n");
                        sql.Append("('" + strBigPM + "','" + strSmallPM + "')  \n");
                    }
                    else
                    {
                        sql.Append("update TPMRelation set vcBigPM='" + strBigPM + "',vcSmallPM='" + strSmallPM + "'   \n");
                        sql.Append("where vcBigPM='" + strBigPM_init + "' and vcSmallPM='" + strSmallPM_init + "'   \n");
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
                sql.Append("select distinct vcBigPM from TPMRelation  \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region 取得关系表中所有小品目
        public DataTable GetSmallPM()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select distinct vcSmallPM from TPMRelation  \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region 删除_品目
        public void Del_pm(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string strBigPM = checkedInfoData[i]["vcBigPM"].ToString();
                    string strSmallPM = checkedInfoData[i]["vcSmallPM"].ToString();

                    sql.Append("delete from TPMRelation   \n");
                    sql.Append("where vcBigPM='" + strBigPM + "' and vcSmallPM='" + strSmallPM + "'   \n");
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

        #region 检索_基准时间
        public DataTable Search_StandardTime(string bigpm, string standardtime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select vcBigPM,vcStandardTime,'0' as vcAddFlag,'0' as vcModFlag  \n");
                strSql.Append("from TPMStandardTime   \n");
                strSql.Append("where isnull(vcBigPM,'') like '%" + bigpm + "%' and isnull(vcStandardTime,'') like '%" + standardtime + "%'  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取得大品目和基准时间关系
        public DataTable GetStandardTime()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select distinct vcBigPM from TPMStandardTime  \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存_品目
        public void Save_Standardtime(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    string strBigPM = listInfoData[i]["vcBigPM"].ToString();
                    string strStandardTime = listInfoData[i]["vcStandardTime"].ToString();

                    if (baddflag)
                    {//新增
                        sql.Append("insert into TPMStandardTime (vcBigPM,vcStandardTime) values   \n");
                        sql.Append("('" + strBigPM + "','" + strStandardTime + "')  \n");
                    }
                    else
                    {
                        sql.Append("update TPMStandardTime set vcStandardTime='" + strStandardTime + "'   \n");
                        sql.Append("where vcBigPM='" + strBigPM + "'   \n");
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

        #region 删除_基准时间
        public void Del_Standardtime(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string strBigPM = checkedInfoData[i]["vcBigPM"].ToString();
                    string strStandardTime = checkedInfoData[i]["vcStandardTime"].ToString();

                    sql.Append("delete from TPMStandardTime   \n");
                    sql.Append("where vcBigPM='" + strBigPM + "' and vcStandardTime='" + strStandardTime + "'   \n");
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


        #region 取得品目信息维护表中信息
        public DataTable GetPMSmall()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select distinct vcPartsNoBefore5,vcSR,vcBCPartsNo from TPMSmall   \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
