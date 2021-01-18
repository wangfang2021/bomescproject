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
    public class FS1502_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string dBZDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcSR,t1.vcPartsNoBefore5,t1.vcBCPartsNo,t1.vcSmallPM,t2.vcBigPM,'0' as vcModFlag,'0' as vcAddFlag  \n");
                strSql.Append("from TPackingPlan_Summary t1  \n");
                strSql.Append("where 1=1  \n");
                if (dBZDate != "" && dBZDate != null)
                    strSql.Append("and t1.dPackDate = '" + dBZDate + "'  \n");
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

                    if (baddflag == true)
                    {//新增
                        
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TPackingPlan_Summary set iPlanTZ=nullif(" + strSmallPM + ",''),vcOperatorID='" + strUserId + "',dOperatorTime=getdate() " +
                            "where iAutoId=" + iAutoId + "   \n");
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
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TPackingPlan_Summary where iAutoId=" + iAutoId + "   \n");
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

        #region 导入后保存
        public void importSave_Sub(DataTable dt,string vcFZPlant,string dBZDate, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TPackingPlan] where vcFZPlant='"+vcFZPlant+"' and vcPackDate='"+dBZDate+"' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strOrderNo = dt.Rows[i]["UNLD DTE"].ToString() + dt.Rows[i]["FRQ"].ToString();
                    sql.Append("INSERT INTO [TPackingPlan]    \n");
                    sql.Append("           ([vcPlant]    \n");
                    sql.Append("           ,[vcPackDate]    \n");
                    sql.Append("           ,[vcPackBZ]    \n");
                    sql.Append("           ,[vcPartId]    \n");
                    sql.Append("           ,[iPackNum]    \n");
                    sql.Append("           ,[vcFZPlant]    \n");
                    sql.Append("           ,[vcSupplier_id]    \n");
                    sql.Append("           ,[vcGQ]    \n");
                    sql.Append("           ,[vcSR]    \n");
                    sql.Append("           ,[vcOrderNo]    \n");
                    sql.Append("           ,[vcOperatorID]    \n");
                    sql.Append("           ,[dOperatorTime])    \n");
                    sql.Append("     VALUES    \n");
                    sql.Append("           ('H1'    \n");//包装工厂，默认H1
                    sql.Append("           ,'"+dBZDate+"'    \n");
                    sql.Append("           ,null    \n");//班值，白/夜   还没取出来呢
                    sql.Append("           ,'"+dt.Rows[i]["PART #"].ToString()+"'    \n");
                    sql.Append("           ,nullif('"+dt.Rows[i]["FINL ORD(PCS)"].ToString()+"','')    \n");
                    sql.Append("           ,'"+vcFZPlant+"'    \n");
                    sql.Append("           ,'"+dt.Rows[i]["SUPL"].ToString()+"'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["PLANT"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["DOCK"].ToString() + "'    \n");
                    sql.Append("           ,'"+ strOrderNo + "'    \n");
                    sql.Append("           ,'"+strUserId+"'    \n");
                    sql.Append("           ,getdate())    \n");
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
