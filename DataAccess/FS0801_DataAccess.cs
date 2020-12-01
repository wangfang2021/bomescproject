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
                strSql.Append("select vcPlantCode,vcPlantName from SUnitPlant where vcUnitCode='TFTM' order by iSort  \n");
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
                strSql.Append("select t1.vcPartsNo,convert(varchar(10),t1.dTimeFrom,120) as dTimeFrom,  \n");
                strSql.Append("convert(varchar(10),t1.dTimeTo,120) as dTimeTo,t1.vcBZPlant,  \n");
                strSql.Append("t4.vcBigPM,t3.vcSmallPM,t5.vcStandardTime,t1.vcBZQF,t1.vcBZUnit,t1.vcRHQF,'1' as vcflag  \n");
                strSql.Append("from tPackageMaster t1  \n");
                strSql.Append("left join tBCPartsGC t2 on t1.vcPartsNo=t2.vcPartsNo  \n");
                strSql.Append("left join tPMSmall t3 on left(t1.vcPartsNo,5)=t3.vcPartsNoBefore5 and t1.vcSR=t3.vcSR   \n");
                strSql.Append("and t2.vcBCPartsNo = t3.vcBCPartsNo  \n");
                strSql.Append("left join tPMRelation t4 on t3.vcSmallPM=t4.vcSmallPM  \n");
                strSql.Append("left join tPMStandardTime t5 on t4.vcBigPM=t5.vcBigPM  \n");
                strSql.Append("where isnull(t1.vcBZPlant,'') like '"+bzplant+"%'  \n");
                strSql.Append("and isnull(t1.vcPartsNo,'') like '%"+pinfan+"%'  \n");
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
    }
}
