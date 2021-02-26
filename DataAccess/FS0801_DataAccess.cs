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
    public class FS0801_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcBZPlant, string vcPart_id, string vcBigPM, string vcSmallPM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct t1.iAutoId,t1.vcPart_id,t1.dTimeFrom,t1.dTimeTo,t1.vcBZPlant,t6.vcName as vcBZPlantName,t1.vcSR,  \n");
                strSql.Append("t4.vcBigPM,t1.vcSmallPM,t5.vcStandardTime,t1.vcBZQF,t1.vcBZUnit,t1.vcRHQF,'0' as vcModFlag,'0' as vcAddFlag  \n");
                strSql.Append("from TPackageMaster t1  \n");
                //strSql.Append("left join (select distinct vcPartsNo,vcPackNo from TPackItem where getdate() between dFrom and dTo) t2 on t1.vcPart_id=t2.vcPartsNo  \n");
                //strSql.Append("left join TPMSmall t3 on left(t1.vcPart_id,5)=t3.vcPartsNoBefore5 and t1.vcSR=t3.vcSR   \n");
                //strSql.Append("and t2.vcPackNo = t3.vcBCPartsNo and t1.vcSupplierId=t3.vcSupplier_id  \n");
                strSql.Append("left join TPMRelation t4 on t1.vcSmallPM=t4.vcSmallPM  \n");
                strSql.Append("left join TPMStandardTime t5 on t4.vcBigPM=t5.vcBigPM  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C023') t6 on t1.vcBZPlant=t6.vcValue  \n");
                strSql.Append("where 1=1   \n");
                if (vcBZPlant != "" && vcBZPlant != null)
                    strSql.Append("and isnull(t1.vcBZPlant,'') like '" + vcBZPlant + "%'  \n");
                if (vcPart_id != "" && vcPart_id != null)
                    strSql.Append("and isnull(t1.vcPart_id,'') like '%" + vcPart_id + "%'  \n");
                if (vcBigPM != "" && vcBigPM != null)
                    strSql.Append("and isnull(t4.vcBigPM,'') like '%" + vcBigPM + "%'  \n");
                if (vcSmallPM != "" && vcSmallPM != null)
                    strSql.Append("and isnull(t1.vcSmallPM,'') like '%" + vcSmallPM + "%'  \n");
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

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                        //无新增情况
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        string strBZPlant =listInfoData[i]["vcBZPlant"]==null?"":listInfoData[i]["vcBZPlant"].ToString();
                        string strBZQF = listInfoData[i]["vcBZQF"]==null?"": listInfoData[i]["vcBZQF"].ToString();
                        string strBZUnit = listInfoData[i]["vcBZUnit"]==null?"": listInfoData[i]["vcBZUnit"].ToString();
                        string strRHQF = listInfoData[i]["vcRHQF"]==null?"": listInfoData[i]["vcRHQF"].ToString();
                        string strSmallPM = listInfoData[i]["vcSmallPM"]==null?"": listInfoData[i]["vcSmallPM"].ToString();
                        string strPart_id = listInfoData[i]["vcPart_id"]==null?"": listInfoData[i]["vcPart_id"].ToString();
                        string strTimeFrom = listInfoData[i]["dTimeFrom"].ToString();
                        string strTimeTo = listInfoData[i]["dTimeTo"].ToString();

                        sql.Append("update TPackageMaster set vcBZPlant='" + strBZPlant + "',vcBZQF='" + strBZQF + "',   \n");
                        sql.Append("vcBZUnit='" + strBZUnit + "',vcRHQF='" + strRHQF + "',vcSmallPM='" + strSmallPM + "'  \n");
                        sql.Append("where vcPart_id='" + strPart_id + "' and dTimeFrom='" + strTimeFrom + "' and dTimeTo='" + strTimeTo + "'  \n");
                        #endregion
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

        #region 获取
        public void Gain(string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("delete from TPackageMaster_Temp where vcOperatorID='" + strUserId + "'    \n");

                sql.Append("insert into TPackageMaster_Temp (vcPart_id,vcReceiver,vcSupplierId,vcPackingPlant,    \n");
                sql.Append("dTimeFrom,dTimeTo,vcSR,vcOperatorID,dOperatorTime)    \n");
                sql.Append("select distinct t1.vcPartId,t1.vcReceiver,t1.vcSupplierId,t1.vcPackingPlant,    \n");
                sql.Append("t1.dFromTime,t1.dToTime,t2.vcSufferIn,'" + strUserId + "',GETDATE()    \n");
                sql.Append("from TSPMaster t1     \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from TSPMaster_SufferIn where vcOperatorType='1'    \n");
                sql.Append("	and GETDATE() between dFromTime and dToTime    \n");
                sql.Append(") t2     \n");
                sql.Append("on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPartId=t2.vcPartId    \n");
                sql.Append("and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId    \n");

                sql.Append("insert into TPackageMaster (vcPart_id,vcReceiver,vcSupplierId,vcPackingPlant,dTimeFrom,dTimeTo,vcSR)    \n");
                sql.Append("select t1.vcPart_id,t1.vcReceiver,t1.vcSupplierId,t1.vcPackingPlant,t1.dTimeFrom,t1.dTimeTo,t1.vcSR     \n");
                sql.Append("from TPackageMaster_Temp t1    \n");
                sql.Append("left join TPackageMaster t2 on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId    \n");
                sql.Append("where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'    \n");

                sql.Append("update t2 set t2.dTimeFrom=t1.dTimeFrom,t2.dTimeTo=t1.dTimeTo,t2.vcSR=t1.vcSR    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TPackageMaster_Temp    \n");
                sql.Append(")t1    \n");
                sql.Append("left join TPackageMaster t2 on t1.vcPackingPlant=t2.vcPackingPlant and t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("and t1.vcReceiver=t2.vcReceiver and t1.vcSupplierId=t2.vcSupplierId    \n");
                sql.Append("where t2.iAutoId is not null and t1.vcOperatorID='" + strUserId + "' and     \n");
                sql.Append("(t1.dTimeFrom!=t2.dTimeFrom or t1.dTimeTo!=t2.dTimeTo or t1.vcSR!=t2.vcSR)    \n");

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

        #region 导入
        public void import(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strBZPlant = dt.Rows[i]["vcBZPlant"].ToString();
                    string strBZQF = dt.Rows[i]["vcBZQF"].ToString();
                    string strBZUnit = dt.Rows[i]["vcBZUnit"].ToString();
                    string strRHQF = dt.Rows[i]["vcRHQF"].ToString();
                    string strSmallPM = dt.Rows[i]["vcSmallPM"].ToString();
                    string strPart_id = dt.Rows[i]["vcPart_id"].ToString();
                    string strTimeFrom = dt.Rows[i]["dTimeFrom"].ToString();
                    string strTimeTo = dt.Rows[i]["dTimeTo"].ToString();

                    sql.Append("update TPackageMaster set vcBZPlant='" + strBZPlant + "',vcBZQF='" + strBZQF + "',   \n");
                    sql.Append("vcBZUnit='" + strBZUnit + "',vcRHQF='" + strRHQF + "',vcSmallPM='" + strSmallPM + "'  \n");
                    sql.Append("where vcPart_id='" + strPart_id + "' and dTimeFrom='" + strTimeFrom + "' and dTimeTo='" + strTimeTo + "'  \n");
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
