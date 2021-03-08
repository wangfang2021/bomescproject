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
    public class FS0405_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strDXDateMonth, string strInOutFlag, string strState)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("      select *,'0' as selection from (       \n");
                strSql.Append("      select T1.vcDXYM,'1' as vcInOutFlag,vcZKState,T2.dZhanKaiTime from (       \n");
                strSql.Append("      select distinct vcDXYM,case when dZhanKaiTime is not null then '可下载' else '待发送' end vcZKState from TSoqReply T1        \n");
                strSql.Append("      where  vcInOutFlag ='1') T1       \n");
                strSql.Append("      left join (       \n");
                strSql.Append("      select distinct vcDXYM,max(dZhanKaiTime) as dZhanKaiTime from TSoqReply T1        \n");
                strSql.Append("      where  vcInOutFlag ='1'       \n");
                strSql.Append("      group by vcDXYM) T2 on T1.vcDXYM = T2.vcDXYM       \n");
                strSql.Append("             \n");
                strSql.Append("      union all       \n");
                strSql.Append("             \n");
                strSql.Append("      select T1.vcDXYM,'0'as vcInOutFlag,case when T1.vcFZGC='可下载' and T2.vcZK='可下载' then '可下载' else '待发送' end vcZK,T2.dZhanKaiTime from (       \n");
                strSql.Append("      select distinct vcDXYM,case when (       \n");
                strSql.Append("      select count(*) as vcFZGC from TCode T1       \n");
                strSql.Append("      left join (select distinct vcDXYM,vcInOutFlag,vcFZGC from TSoqReply T1 where  vcInOutFlag ='0')       \n");
                strSql.Append("      T2 on T1.vcValue = T2.vcFZGC       \n");
                strSql.Append("      where vcCodeId = 'C000' and vcFZGC is null) >=1 then '待发送' else '可下载' end vcFZGC  from  TSoqReply where  vcInOutFlag ='0'       \n");
                strSql.Append("      ) T1        \n");
                strSql.Append("      left join(       \n");
                strSql.Append("      select T1.vcDXYM,vcZK,T2.dZhanKaiTime from (       \n");
                strSql.Append("      select distinct vcDXYM,case when dZhanKaiTime is not null then '可下载' else '待发送' end vcZK from TSoqReply T1        \n");
                strSql.Append("      where  vcInOutFlag ='0') T1       \n");
                strSql.Append("      left join (       \n");
                strSql.Append("      select distinct vcDXYM,max(dZhanKaiTime) as dZhanKaiTime from TSoqReply T1        \n");
                strSql.Append("      where  vcInOutFlag ='0'       \n");
                strSql.Append("      group by vcDXYM) T2 on T1.vcDXYM = T2.vcDXYM) T2 on T1.vcDXYM = T2.vcDXYM       \n");
                strSql.Append("      ) TT       \n");
                strSql.Append("      where 1=1       \n");
                if (!string.IsNullOrEmpty(strDXDateMonth))
                {
                    strSql.Append("      and vcDXYM = '" + strDXDateMonth + "'       \n");
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("      and vcInOutFlag = '" + strInOutFlag + "'       \n");
                }
                if (!string.IsNullOrEmpty(strState))
                {
                    strSql.Append("      and vcZKState = '" + strState + "'       \n");
                }
                strSql.Append("      order by TT.vcDXYM asc,vcInOutFlag desc        \n");
                
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 导出检索
        public DataTable exportSearch(string strCLYM, string strInOutFlag,string strDXYM1, string strDXYM2,string strDXYM3)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" SELECT a.*,b.[N+1 O/L],b.[N+1 Units],b.[N+1 PCS], ");
                strSql.AppendLine(" c.[N+2 O/L],c.[N+2 Units],c.[N+2 PCS] ");
                strSql.AppendLine(" FROM ");
                strSql.AppendLine(" ( ");
                strSql.AppendLine("   SELECT ");
                strSql.AppendLine("   vcPart_id as 'PartsNo',");
                //发注工厂
                strSql.AppendLine("   cast(vcFZGC as int) as '发注工厂',");
                //订货频度
                strSql.AppendLine("   cast(vcMakingOrderType as int) as '订货频度',");
                strSql.AppendLine("   vcCarType as 'CFC',");
                strSql.AppendLine("   isnull(iQuantityPercontainer,0) as 'OrdLot',");
                strSql.AppendLine("   isnull(iBoxes,0) as 'N Units',");
                strSql.AppendLine("   isnull(iPartNums,0) as 'N PCS',");
                strSql.AppendLine("   isnull(iD1,0)*isnull(iQuantityPercontainer,0) as iD1,");
                strSql.AppendLine("   isnull(iD2,0)*isnull(iQuantityPercontainer,0) as iD2,");
                strSql.AppendLine("   isnull(iD3,0)*isnull(iQuantityPercontainer,0) as iD3,");
                strSql.AppendLine("   isnull(iD4,0)*isnull(iQuantityPercontainer,0) as iD4,");
                strSql.AppendLine("   isnull(iD5,0)*isnull(iQuantityPercontainer,0) as iD5,");
                strSql.AppendLine("   isnull(iD6,0)*isnull(iQuantityPercontainer,0) as iD6,");
                strSql.AppendLine("   isnull(iD7,0)*isnull(iQuantityPercontainer,0) as iD7,");
                strSql.AppendLine("   isnull(iD8,0)*isnull(iQuantityPercontainer,0) as iD8,");
                strSql.AppendLine("   isnull(iD9,0)*isnull(iQuantityPercontainer,0) as iD9,");
                strSql.AppendLine("   isnull(iD10,0)*isnull(iQuantityPercontainer,0) as iD10,");
                strSql.AppendLine("   isnull(iD11,0)*isnull(iQuantityPercontainer,0) as iD11,");
                strSql.AppendLine("   isnull(iD12,0)*isnull(iQuantityPercontainer,0) as iD12,");
                strSql.AppendLine("   isnull(iD13,0)*isnull(iQuantityPercontainer,0) as iD13,");
                strSql.AppendLine("   isnull(iD14,0)*isnull(iQuantityPercontainer,0) as iD14,");
                strSql.AppendLine("   isnull(iD15,0)*isnull(iQuantityPercontainer,0) as iD15,");
                strSql.AppendLine("   isnull(iD16,0)*isnull(iQuantityPercontainer,0) as iD16,");
                strSql.AppendLine("   isnull(iD17,0)*isnull(iQuantityPercontainer,0) as iD17,");
                strSql.AppendLine("   isnull(iD18,0)*isnull(iQuantityPercontainer,0) as iD18,");
                strSql.AppendLine("   isnull(iD19,0)*isnull(iQuantityPercontainer,0) as iD19,");
                strSql.AppendLine("   isnull(iD20,0)*isnull(iQuantityPercontainer,0) as iD20,");
                strSql.AppendLine("   isnull(iD21,0)*isnull(iQuantityPercontainer,0) as iD21,");
                strSql.AppendLine("   isnull(iD22,0)*isnull(iQuantityPercontainer,0) as iD22,");
                strSql.AppendLine("   isnull(iD23,0)*isnull(iQuantityPercontainer,0) as iD23,");
                strSql.AppendLine("   isnull(iD24,0)*isnull(iQuantityPercontainer,0) as iD24,");
                strSql.AppendLine("   isnull(iD25,0)*isnull(iQuantityPercontainer,0) as iD25,");
                strSql.AppendLine("   isnull(iD26,0)*isnull(iQuantityPercontainer,0) as iD26,");
                strSql.AppendLine("   isnull(iD27,0)*isnull(iQuantityPercontainer,0) as iD27,");
                strSql.AppendLine("   isnull(iD28,0)*isnull(iQuantityPercontainer,0) as iD28,");
                strSql.AppendLine("   isnull(iD29,0)*isnull(iQuantityPercontainer,0) as iD29,");
                strSql.AppendLine("   isnull(iD30,0)*isnull(iQuantityPercontainer,0) as iD30,");
                strSql.AppendLine("   isnull(iD31,0)*isnull(iQuantityPercontainer,0) as iD31,");
                strSql.AppendLine("   iAutoId");
                strSql.AppendLine("   FROM TSOQReply  ");
                strSql.AppendLine("   WHERE 1=1 ");
                if (!string.IsNullOrEmpty(strCLYM))
                {
                    strSql.AppendLine("   AND vcCLYM='" + strCLYM + "'       ");//处理年月
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.AppendLine("   AND vcInOutFlag='" + strInOutFlag + "'       ");//内外
                }
                if (!string.IsNullOrEmpty(strDXYM1))
                {
                    strSql.AppendLine("   AND vcDXYM='" + strDXYM1 + "'       ");//对象年月1
                }
                strSql.AppendLine(" ) a ");

                strSql.AppendLine(" LEFT JOIN (   ");
                strSql.AppendLine("   SELECT vcPart_id,isnull(iQuantityPercontainer,0) as 'N+1 O/L',isnull(iBoxes,0) as 'N+1 Units',isnull(iPartNums,0) as 'N+1 PCS' ");
                strSql.AppendLine("   FROM TSOQReply   ");
                strSql.AppendLine("   WHERE 1=1 ");
                if (!string.IsNullOrEmpty(strCLYM))
                {
                    strSql.AppendLine("   AND vcCLYM='" + strCLYM + "'       ");//处理年月
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.AppendLine("   AND vcInOutFlag='" + strInOutFlag + "'       ");//内外
                }
                if (!string.IsNullOrEmpty(strDXYM2))
                {
                    strSql.AppendLine("   AND vcDXYM='" + strDXYM2 + "'       ");//对象年月2
                }
                strSql.AppendLine("  ) b ");
                strSql.AppendLine(" ON a.PartsNo=b.vcPart_id ");

                strSql.AppendLine(" LEFT JOIN (   ");
                strSql.AppendLine("   SELECT vcPart_id,isnull(iQuantityPercontainer,0) as 'N+2 O/L',isnull(iBoxes,0) as 'N+2 Units',isnull(iPartNums,0) as 'N+2 PCS' ");
                strSql.AppendLine("   FROM TSOQReply   ");
                strSql.AppendLine("   WHERE 1=1 ");
                if (!string.IsNullOrEmpty(strCLYM))
                {
                    strSql.AppendLine("   AND vcCLYM='" + strCLYM + "'       ");//处理年月
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.AppendLine("   AND vcInOutFlag='" + strInOutFlag + "'       ");//内外
                }
                if (!string.IsNullOrEmpty(strDXYM3))
                {
                    strSql.AppendLine("   AND vcDXYM='" + strDXYM3 + "'       ");//对象年月3
                }
                strSql.AppendLine("  ) c ");
                strSql.AppendLine(" ON a.PartsNo=c.vcPart_id ");

                strSql.AppendLine(" order by a.iAutoId ");

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
