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

        #region 绑定包装厂
        public DataTable BindPlant()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select 'H1' as vcPlantCode,'H1' as vcPlantName  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索
        public DataTable Search(string bzplant, string pinfan, string bigpm, string smallpm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcPart_id,t1.dTimeFrom,t1.dTimeTo,t1.vcBZPlant,t2.vcBCPartsNo,t1.vcSR,  \n");
                strSql.Append("t4.vcBigPM,t3.vcSmallPM,t5.vcStandardTime,t1.vcBZQF,t1.vcBZUnit,t1.vcRHQF,'0' as vcModFlag  \n");
                strSql.Append("from TPackageMaster t1  \n");
                strSql.Append("left join tBCPartsGC t2 on t1.vcPart_id=t2.vcPartsNo  \n");
                strSql.Append("left join TPMSmall t3 on left(t1.vcPart_id,5)=t3.vcPartsNoBefore5 and t1.vcSR=t3.vcSR   \n");
                strSql.Append("and t2.vcBCPartsNo = t3.vcBCPartsNo  \n");
                strSql.Append("left join TPMRelation t4 on t3.vcSmallPM=t4.vcSmallPM  \n");
                strSql.Append("left join TPMStandardTime t5 on t4.vcBigPM=t5.vcBigPM  \n");
                strSql.Append("where isnull(t1.vcBZPlant,'') like '"+bzplant+"%'  \n");
                strSql.Append("and isnull(t1.vcPart_id,'') like '%" + pinfan+"%'  \n");
                strSql.Append("and isnull(t4.vcBigPM,'') like '%"+bigpm+"%'  \n");
                strSql.Append("and isnull(t3.vcSmallPM,'') like '%"+smallpm+"%'  \n");
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
                for(int i=0;i< listInfoData.Count;i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    string strBZPlant = listInfoData[i]["vcBZPlant"].ToString();
                    string strBZQF = listInfoData[i]["vcBZQF"].ToString();
                    string strBZUnit = listInfoData[i]["vcBZUnit"].ToString();
                    string strRHQF = listInfoData[i]["vcRHQF"].ToString();

                    //标识说明
                    //默认  bmodflag:false  
                    //修改  bmodflag:true   

                    if(bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TPackageMaster set vcBZPlant='" + strBZPlant + "',vcBZQF='" + strBZQF + "',   \n");
                        sql.Append("vcBZUnit='" + strBZUnit + "',vcRHQF='" + strRHQF + "'  \n");
                        sql.Append("where iAutoId="+iAutoId+"  \n");
                    }
                }
                if(sql.Length>0)
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
