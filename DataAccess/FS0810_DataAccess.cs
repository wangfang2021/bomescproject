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
        public DataTable Search(string smallpm, string sr, string vcPartsNoBefore5,string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcSR,t1.vcSupplier_id,t1.vcPartsNoBefore5,t1.vcBCPartsNo,t1.vcSmallPM,t2.vcBigPM,'0' as vcModFlag,'0' as vcAddFlag  \n");
                strSql.Append("from TPMSmall t1  \n");
                strSql.Append("left join TPMRelation t2 on t1.vcSmallPM=t2.vcSmallPM  \n");
                strSql.Append("where 1=1  \n");
                if (smallpm != "" && smallpm != null)
                    strSql.Append("and ISNULL(t1.vcSmallPM,'') like '%" + smallpm + "%'  \n");
                if (sr != "" && sr != null)
                    strSql.Append("and ISNULL(t1.vcSR,'') like '%" + sr + "%'  \n");
                if (vcPartsNoBefore5 != "" && vcPartsNoBefore5 != null)
                    strSql.Append("and ISNULL(t1.vcPartsNoBefore5,'') like '%" + vcPartsNoBefore5 + "%'  \n");
                if(vcSupplier_id!="" && vcSupplier_id!=null)
                    strSql.Append("and ISNULL(t1.vcSupplier_id,'') like '%" + vcSupplier_id + "%'  \n");
                strSql.Append("order by t1.vcSR,t1.vcSupplier_id,t1.vcPartsNoBefore5,t1.vcBCPartsNo,t2.vcBigPM,t1.vcSmallPM    \n");
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
                    string strSupplier_id= listInfoData[i]["vcSupplier_id"].ToString();
                    string strPartsNoBefore5 = listInfoData[i]["vcPartsNoBefore5"].ToString();
                    string strBCPartsNo = listInfoData[i]["vcBCPartsNo"].ToString();
                    string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                        sql.Append("insert into TPMSmall (vcPartsNoBefore5,vcSR,vcSupplier_id,vcBCPartsNo,vcSmallPM,vcOperatorID,dOperatorTime) values   \n");
                        sql.Append("('" + strPartsNoBefore5 + "','" + strSR + "','"+strSupplier_id+"','" + strBCPartsNo + "','" + strSmallPM + "','" + strUserId + "',getdate())  \n");
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TPMSmall set vcSmallPM='" + strSmallPM + "',vcOperatorID='" + strUserId + "',dOperatorTime=getdate() " +
                            "where iAutoId=" + iAutoId + "   \n");
                    }
                    //更新TPackageMaster中的小品目
                    //sql.Append("update t1 set t1.vcSmallPM='"+strSmallPM+"'     \n");
                    //sql.Append("from (     \n");
                    //sql.Append("	select * from TPackageMaster      \n");
                    //sql.Append("	where vcSR='"+ strSR + "'       \n");
                    //if (strSupplier_id != "")
                    //    sql.Append("   and vcSupplierId='"+strSupplier_id+"'   \n");
                    //if (strPartsNoBefore5 != "")
                    //    sql.Append("  and LEFT(vcPart_id,5)='"+strPartsNoBefore5+"'  \n");
                    //sql.Append(")t1      \n");
                    //sql.Append("left join (     \n");
                    //sql.Append("	select distinct vcPartsNo,vcPackNo from TPackItem where getdate() between dFrom and dTo     \n");
                    //sql.Append(") t2 on t1.vcPart_id=t2.vcPartsNo     \n");
                    //if(strBCPartsNo!="")
                    //    sql.Append("where  t2.vcPackNo='"+strBCPartsNo+"'      \n");
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
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    string strSR = checkedInfoData[i]["vcSR"].ToString();
                    string strSupplier_id = checkedInfoData[i]["vcSupplier_id"].ToString();
                    string strPartsNoBefore5 = checkedInfoData[i]["vcPartsNoBefore5"].ToString();
                    string strBCPartsNo = checkedInfoData[i]["vcBCPartsNo"].ToString();
                    string strSmallPM = checkedInfoData[i]["vcSmallPM"].ToString();

                    sql.Append("delete from TPMSmall where iAutoId=" + iAutoId + "   \n");
                    //更新TPackageMaster中的小品目
                    //sql.Append("update t1 set t1.vcSmallPM='无'     \n");
                    //sql.Append("from (     \n");
                    //sql.Append("	select * from TPackageMaster      \n");
                    //sql.Append("	where vcSR='" + strSR + "'       \n");
                    //if (strSupplier_id != "")
                    //    sql.Append("   and vcSupplierId='" + strSupplier_id + "'   \n");
                    //if (strPartsNoBefore5 != "")
                    //    sql.Append("  and LEFT(vcPart_id,5)='" + strPartsNoBefore5 + "'  \n");
                    //sql.Append(")t1      \n");
                    //sql.Append("left join (     \n");
                    //sql.Append("	select distinct vcPartsNo,vcPackNo from TPackItem where getdate() between dFrom and dTo     \n");
                    //sql.Append(") t2 on t1.vcPart_id=t2.vcPartsNo     \n");
                    //if (strBCPartsNo != "")
                    //    sql.Append("where  t2.vcPackNo='" + strBCPartsNo + "'      \n");
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

        #region 获取数据字典
        public DataTable getTCode(string strCodeId)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select vcValue1 as vcName,vcValue1 as vcValue from TOutCode where vcCodeId='"+strCodeId+"' and vcIsColum='0' order by vcValue2,vcValue1    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                strSql.Append("select iAutoId,vcBigPM,vcSmallPM,'0' as vcAddFlag,'0' as vcModFlag,vcStandardTime  \n");
                strSql.Append("from TPMRelation where 1=1  \n");
                if(smallpm!="" && smallpm!=null)
                    strSql.Append("and isnull(vcSmallPM,'') like '%" + smallpm + "%'  \n");
                if(bigpm!="" && bigpm!=null)
                    strSql.Append("and isnull(vcBigPM,'') like '%" + bigpm + "%'  \n");
                strSql.Append("order by vcBigPM,vcSmallPM    \n");
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    string strBigPM = listInfoData[i]["vcBigPM"].ToString();
                    string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();
                    string strStandardTime= listInfoData[i]["vcStandardTime"].ToString();

                    if (baddflag == true && bmodflag == true)
                    {//新增
                        sql.Append("insert into TPMRelation (vcBigPM,vcSmallPM,vcStandardTime) values   \n");
                        sql.Append("('" + strBigPM + "','" + strSmallPM + "','"+strStandardTime+"')  \n");
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TPMRelation set vcBigPM='" + strBigPM + "',vcSmallPM='" + strSmallPM + "',vcStandardTime='"+strStandardTime+"' where iAutoId=" + iAutoId + "    \n");
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

        #region 删除_品目
        public void Del_pm(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TPMRelation where iAutoId=" + iAutoId + "   \n");
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
                strSql.Append("select iAutoId,vcBigPM,vcStandardTime,'0' as vcAddFlag,'0' as vcModFlag  \n");
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

                    if (baddflag == true && bmodflag == true)
                    {//新增
                        sql.Append("insert into TPMStandardTime (vcBigPM,vcStandardTime) values   \n");
                        sql.Append("('" + strBigPM + "','" + strStandardTime + "')  \n");
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TPMStandardTime set vcStandardTime='" + strStandardTime + "' where iAutoId=" + iAutoId + "   \n");
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
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TPMStandardTime where iAutoId=" + iAutoId + "   \n");
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

        #region 受入号+厂家代码+品番前5位+包材品番 不能重复
        public int RepeatCheck(string vcSR,string vcSupplier_id, string vcPartsNoBefore5, string vcBCPartsNo)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select COUNT(1) from TPMSmall   \n");
                sql.Append("where vcSR='" + vcSR + "' and vcSupplier_id='"+vcSupplier_id+"' and vcPartsNoBefore5='" + vcPartsNoBefore5 + "' and vcBCPartsNo='" + vcBCPartsNo + "'   \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 小品目 不能重复
        public int RepeatCheckSmall(string vcSmallPM, string strMode, string strAutoId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select COUNT(1) from TPMRelation where vcSmallPM='" + vcSmallPM + "'  \n");
                if (strMode == "mod")
                {
                    sql.Append("and iAutoId!=" + strAutoId + "  \n");
                }
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存-品目关系
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TPMRelation_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("INSERT INTO [TPMRelation_Temp] \n");
                    sql.Append("           ([vcBigPM] \n");
                    sql.Append("           ,[vcSmallPM] \n");
                    sql.Append("           ,[vcStandardTime] \n");
                    sql.Append("           ,[vcOperatorID] \n");
                    sql.Append("           ,[dOperatorTime]) \n");
                    sql.Append("     VALUES \n");
                    sql.Append("           ('" + dt.Rows[i]["vcBigPM"].ToString() + "' \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSmallPM"].ToString() + "' \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcStandardTime"].ToString() + "' \n");
                    sql.Append("           ,'" + strUserId + "' \n");
                    sql.Append("           ,getdate()) \n");
                }
                sql.Append("insert into TPMRelation (vcBigPM,vcSmallPM,vcStandardTime,vcOperatorID,dOperatorTime) \n");
                sql.Append("select t1.vcBigPM,t1.vcSmallPM,t1.vcStandardTime,t1.vcOperatorID,t1.dOperatorTime from TPMRelation_Temp t1 \n");
                sql.Append("left join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='"+strUserId+"' \n");

                sql.Append("update t2 set t2.vcStandardTime=t1.vcStandardTime,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TPMRelation_Temp) t1  \n");
                sql.Append("inner join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM  \n");
                sql.Append("where isnull(t1.vcStandardTime,'')!=isnull(t2.vcStandardTime,'')  \n");
                sql.Append("and t1.vcOperatorID='" + strUserId + "'  \n");

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

        #region 大品目 不能重复
        public int RepeatCheckStandardTime(string vcBigPM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select COUNT(1) from TPMStandardTime where vcBigPM='" + vcBigPM + "'  \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存-基准时间
        public void importSave_StandardTime(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TPMStandardTime_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("INSERT INTO [TPMStandardTime_Temp] \n");
                    sql.Append("           ([vcBigPM] \n");
                    sql.Append("           ,[vcStandardTime] \n");
                    sql.Append("           ,[vcOperatorID] \n");
                    sql.Append("           ,[dOperatorTime]) \n");
                    sql.Append("     VALUES \n");
                    sql.Append("           ('" + dt.Rows[i]["vcBigPM"].ToString() + "' \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcStandardTime"].ToString() + "' \n");
                    sql.Append("           ,'" + strUserId + "' \n");
                    sql.Append("           ,getdate()) \n");
                }
                sql.Append("insert into TPMStandardTime (vcBigPM,vcStandardTime,vcOperatorID,dOperatorTime)  \n");
                sql.Append("select t1.vcBigPM,t1.vcStandardTime,t1.vcOperatorID,t1.dOperatorTime from TPMStandardTime_Temp t1  \n");
                sql.Append("left join TPMStandardTime t2 on t1.vcBigPM=t2.vcBigPM  \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='"+strUserId+"'  \n");
              
                sql.Append("update t2 set t2.vcStandardTime=t1.vcStandardTime,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TPMStandardTime_Temp) t1  \n");
                sql.Append("left join TPMStandardTime t2 on t1.vcBigPM=t2.vcBigPM  \n");
                sql.Append("where t2.iAutoId is not null and t1.vcStandardTime!=t2.vcStandardTime  \n");
                sql.Append("and t1.vcOperatorID='"+strUserId+"'  \n");

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

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TPMSmall_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("INSERT INTO [TPMSmall_Temp]  \n");
                    sql.Append("           ([vcPartsNoBefore5]  \n");
                    sql.Append("           ,[vcSR]  \n");
                    sql.Append("           ,[vcSupplier_id]  \n");
                    sql.Append("           ,[vcBCPartsNo]  \n");
                    sql.Append("           ,[vcSmallPM]  \n");
                    sql.Append("           ,[vcOperatorID]  \n");
                    sql.Append("           ,[dOperatorTime])  \n");
                    sql.Append("     VALUES  \n");
                    sql.Append("           ('"+ dt.Rows[i]["vcPartsNoBefore5"].ToString() + "'   \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSR"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSupplier_id"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcBCPartsNo"].ToString() + "' \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSmallPM"].ToString() + "' \n"); 
                    sql.Append("           ,'"+strUserId+"'  \n");
                    sql.Append("           ,getdate())  \n");
                   
                }
                sql.Append("insert into TPMSmall (vcPartsNoBefore5,vcSR,vcBCPartsNo,vcSupplier_id,vcSmallPM,vcOperatorID,dOperatorTime)  \n");
                sql.Append("select t1.vcPartsNoBefore5,t1.vcSR,t1.vcBCPartsNo,t1.vcSupplier_id,t1.vcSmallPM,t1.vcOperatorID,t1.dOperatorTime from TPMSmall_Temp t1  \n");
                sql.Append("left join TPMSmall t2 on isnull(t1.vcPartsNoBefore5,'')=isnull(t2.vcPartsNoBefore5,'') and isnull(t1.vcSR,'')=isnull(t2.vcSR,'') " +
                    "and isnull(t1.vcBCPartsNo,'')=isnull(t2.vcBCPartsNo,'') and isnull(t1.vcSupplier_id,'')=isnull(t2.vcSupplier_id,'')  \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'  \n");

                sql.Append("update t2 set t2.vcSmallPM=t1.vcSmallPM,  \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TPMSmall_Temp) t1  \n");
                sql.Append("left join TPMSmall t2 on isnull(t1.vcPartsNoBefore5,'')=isnull(t2.vcPartsNoBefore5,'') and isnull(t1.vcSR,'')=isnull(t2.vcSR,'') " +
                    "and isnull(t1.vcBCPartsNo,'')=isnull(t2.vcBCPartsNo,'') and isnull(t1.vcSupplier_id,'')=isnull(t2.vcSupplier_id,'') \n");
                sql.Append("where t2.iAutoId is not null and isnull(t1.vcSmallPM,'') != isnull(t2.vcSmallPM,'')  \n");
                sql.Append("and t1.vcOperatorID='" + strUserId + "'  \n");

                #region not use
                //sql.Append("update t1 set t1.vcSmallPM=    \n");
                //sql.Append("isnull(t3.vcSmallPM,isnull(t4.vcSmallPM,isnull(t5.vcSmallPM,isnull(t6.vcSmallPM,    \n");
                //sql.Append("isnull(t7.vcSmallPM,isnull(t8.vcSmallPM,isnull(t9.vcSmallPM,isnull(t10.vcSmallPM,    \n");
                //sql.Append("isnull(t11.vcSmallPM,isnull(t12.vcSmallPM,isnull(t13.vcSmallPM,isnull(t14.vcSmallPM,    \n");
                //sql.Append("isnull(t15.vcSmallPM,'')))))))))))))    \n");
                //sql.Append("from (    \n");
                //sql.Append("	select vcSR,vcSupplierId,LEFT(vcPart_id,5) as vcPart,vcPart_id,vcSmallPM    \n");
                //sql.Append("	from TPackageMaster      \n");
                //sql.Append(")t1    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select distinct vcPartsNo,vcPackNo from TPackItem --where getdate() between dFrom and dTo    \n");
                //sql.Append(") t2 on t1.vcPart_id=t2.vcPartsNo    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id!='' and vcPartsNoBefore5!='' and vcBCPartsNo!=''    \n");
                //sql.Append(") t3 on t1.vcSR=t3.vcSR and t1.vcSupplierId=t3.vcSupplier_id    \n");
                //sql.Append("and t1.vcPart=t3.vcPartsNoBefore5 and t2.vcPackNo=t3.vcBCPartsNo    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id!='' and vcPartsNoBefore5!='' and vcBCPartsNo=''    \n");
                //sql.Append(") t4 on t1.vcSR=t4.vcSR and t1.vcSupplierId=t4.vcSupplier_id    \n");
                //sql.Append("and t1.vcPart=t4.vcPartsNoBefore5    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id!='' and vcPartsNoBefore5='' and vcBCPartsNo!=''    \n");
                //sql.Append(") t5 on t1.vcSR=t5.vcSR and t1.vcSupplierId=t5.vcSupplier_id    \n");
                //sql.Append("and t2.vcPackNo=t5.vcBCPartsNo    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id!='' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(") t6 on t1.vcSR=t6.vcSR and t1.vcSupplierId=t6.vcSupplier_id    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id='' and vcPartsNoBefore5!='' and vcBCPartsNo!=''    \n");
                //sql.Append(") t7 on t1.vcSR=t7.vcSR    \n");
                //sql.Append("and t1.vcPart=t7.vcPartsNoBefore5 and t2.vcPackNo=t7.vcBCPartsNo    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id='' and vcPartsNoBefore5!='' and vcBCPartsNo=''    \n");
                //sql.Append(") t8 on t1.vcSR=t8.vcSR and t1.vcPart=t8.vcPartsNoBefore5     \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id='' and vcPartsNoBefore5='' and vcBCPartsNo!=''    \n");
                //sql.Append(") t9 on t1.vcSR=t9.vcSR and t2.vcPackNo=t9.vcBCPartsNo    \n");
                //sql.Append(" left join (    \n");
                //sql.Append("	select * from TPMSmall     \n");
                //sql.Append("	where vcSR!='' and vcSupplier_id='' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(") t10 on t1.vcSR=t10.vcSR    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall where vcSR='HA,HB,HD,HE,6H,8H,H1,H9'     \n");
                //sql.Append("	and vcSupplier_id='' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(")t11 on t1.vcSR=t11.vcSR    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall where vcSR='H2,HC,HF,7H'     \n");
                //sql.Append("	and vcSupplier_id='' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(")t12 on t1.vcSR=t12.vcSR    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall where vcSR='N1,N2,N3,N4'     \n");
                //sql.Append("	and vcSupplier_id='TF1W,TF2W,TF3W,TF2T' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(")t13 on t1.vcSR=t13.vcSR and t1.vcSupplierId=t13.vcSupplier_id    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall where vcSR='N1,N2,N3,N4'     \n");
                //sql.Append("	and vcSupplier_id='TF1R,TF2R,TF3R' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(")t14 on t1.vcSR=t14.vcSR and t1.vcSupplierId=t14.vcSupplier_id    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from TPMSmall where vcSR='N1,N2,N3,N4'     \n");
                //sql.Append("	and vcSupplier_id='TF1A,TF2A,TF3A' and vcPartsNoBefore5='' and vcBCPartsNo=''    \n");
                //sql.Append(")t15 on t1.vcSR=t15.vcSR and t1.vcSupplierId=t15.vcSupplier_id    \n");
                ////剩下没带出小品目的，都更新成无
                //sql.Append("update t1 set t1.vcSmallPM='无' from     \n");
                //sql.Append("(select * from TPackageMaster where ISNULL(vcSmallPM,'')='')t1    \n");
                #endregion

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
