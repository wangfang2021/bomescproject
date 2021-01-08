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
    public class FS1310_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getPinMuInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcBigPM as vcValue,vcBigPM as vcName from TPMRelation");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strPlant, string strPinMu, string strPartId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select c.LinId as LinId");
                strSql.AppendLine(",d.vcName as vcPlant");
                strSql.AppendLine(",a.vcPart_id as vcPartId");
                strSql.AppendLine(",b.vcBigPM as vcPinMu");
                strSql.AppendLine(",c.vcPicUrl_small as vcOperImage_samll");
                strSql.AppendLine(",c.vcPicUrl as vcOperImage");
                strSql.AppendLine(",c.vcOperatorID as vcOperator");
                strSql.AppendLine(",c.dOperatorTime as vcOperatorTime from ");
                strSql.AppendLine("(select * from TPackageMaster)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMRelation)b");
                strSql.AppendLine("on a.vcSmallPM=b.vcSmallPM");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPackOperImage)c");
                strSql.AppendLine("on a.vcPart_id=c.vcPartId and a.vcBZPlant=c.vcPlant");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C023')d");
                strSql.AppendLine("on a.vcBZPlant=d.vcValue");
                strSql.AppendLine("where 1=1");
                if (strPlant != "")
                {
                    strSql.AppendLine("and a.vcBZPlant='" + strPlant + "' ");
                }
                if (strPlant != "")
                {
                    strSql.AppendLine("and b.vcBigPM='" + strPinMu + "'");
                }
                if (strPlant != "")
                {
                    strSql.AppendLine("and a.vcPart_id like '" + strPartId + "%'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int setDeleteInfo(ArrayList delList)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < delList.Count; i++)
                {
                    strSql.AppendLine("");
                    strSql.AppendLine("");
                    strSql.AppendLine("");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
