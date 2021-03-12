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
                strSql.Append("       select * from ( select vcCLYM,vcInOutFlag,case when Flag1='OK' and Flag = 'OK' then '可下载' else '待发送' end [State] ,dZhanKaiTime from(      \n");
                strSql.Append("             \n");
                strSql.Append("       select distinct TT1.vcCLYM,TT1.vcInOutFlag,isnull(TT2.Flag1,'OK') as Flag1,isnull(TT3.Flag,'OK') Flag,TT4.dZhanKaiTime from TSoqReply TT1      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("       select vcCLYM,vcInOutFlag,Flag1 from (      \n");
                strSql.Append("       select distinct vcCLYM,vcInOutFlag,case when dZhanKaiTime is not null then 'OK' else 'NG' end Flag1 from (      \n");
                strSql.Append("       select distinct T1.vcCLYM,T1.vcDXYM,T1.vcFZGC,T1.vcInOutFlag,T2.dZhanKaiTime from TSoqReply T1      \n");
                strSql.Append("       left join(      \n");
                strSql.Append("       select distinct vcCLYM,vcDXYM,vcFZGC,vcInOutFlag,max(dZhanKaiTime) as dZhanKaiTime from TSoqReply       \n");
                strSql.Append("       group by vcCLYM,vcDXYM,vcFZGC,vcInOutFlag )T2       \n");
                strSql.Append("       on T1.vcCLYM = T2.vcCLYM and T1.vcDXYM = T2.vcDXYM and T1.vcFZGC = T2.vcFZGC and T1.vcInOutFlag = T2.vcInOutFlag      \n");
                strSql.Append("       ) TT ) TALL      \n");
                strSql.Append("        where Flag1 = 'NG'      \n");
                strSql.Append("       ) TT2 on TT1.vcCLYM = TT2.vcCLYM and TT1.vcInOutFlag = TT2.vcInOutFlag      \n");
                strSql.Append("       left join(      \n");
                strSql.Append("       select distinct vcCLYM,TT.vcInOutFlag,Flag from (      \n");
                strSql.Append("       --判断处理年月是否全部OK      \n");
                strSql.Append("       select vcCLYM,TA.vcInOutFlag,case when TA.flag-TB.flag = 0 then 'OK' else 'NG' end Flag from (      \n");
                strSql.Append("       --判断发注工厂是否全部SOQReply      \n");
                strSql.Append("       select vcCLYM,vcDXYM,vcInOutFlag,count(vcFZGC) as flag from (      \n");
                strSql.Append("       select distinct vcCLYM,vcDXYM,vcFZGC,vcInOutFlag from TSoqReply  ) T1      \n");
                strSql.Append("       group by vcCLYM,vcDXYM,vcInOutFlag ) TA      \n");
                strSql.Append("       left join(      \n");
                strSql.Append("       select vcYearMonth,vcInOutFlag,count(vcFZGC) as flag from (      \n");
                strSql.Append("       select distinct vcYearMonth,vcFZGC,vcInOutFlag from tsoq ) T2      \n");
                strSql.Append("       group by vcYearMonth,vcInOutFlag ) TB on TA.vcDXYM = TB.vcYearMonth and TA.flag = TB.flag and TA.vcInOutFlag = TB.vcInOutFlag      \n");
                strSql.Append("             \n");
                strSql.Append("       ) TT where Flag = 'NG'      \n");
                strSql.Append("       ) TT3 on TT1.vcCLYM = TT3.vcCLYM and TT1.vcInOutFlag = TT3.vcInOutFlag       \n");
                strSql.Append("       left join(      \n");
                strSql.Append("       select vcCLYM,vcInOutFlag,max(dZhanKaiTime) as dZhanKaiTime from TSoqReply       \n");
                strSql.Append("       group by vcCLYM,vcInOutFlag ) TT4 on TT1.vcCLYM = TT4.vcCLYM and TT1.vcInOutFlag = TT4.vcInOutFlag       \n");
                strSql.Append("       ) TT5    ) TAB    \n");
                strSql.Append("       where 1=1      \n");
                if (!string.IsNullOrEmpty(strDXDateMonth))
                {
                    strSql.Append("    and vcCLYM = '" + strDXDateMonth.Replace("/", "") + "'         \n");
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("    and vcInOutFlag = '" + strInOutFlag+"'         \n");
                }
                if (!string.IsNullOrEmpty(strState))
                {
                    strSql.Append("    and [State] = '" + strState + "'         \n");
                }

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
