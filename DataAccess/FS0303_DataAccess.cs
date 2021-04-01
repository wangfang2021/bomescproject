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
    public class FS0303_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        FS0303_DataAccess_Sync sync = new FS0303_DataAccess_Sync();

        #region 按检索条件返回dt
        public DataTable Search(string strIsShowAll, string strOriginCompany)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select *    \n");
                strSql.Append("     ,b.vcName as 'vcChange_Name'    \n");
                strSql.Append("     ,b2.vcName as 'vcSQState_Name'    \n");
                strSql.Append("     ,b4.vcName as 'vcInOutflag_Name'    \n");
                strSql.Append("     ,b5.vcName as 'vcSYTCode_Name'    \n");
                strSql.Append("     ,b6.vcName as 'vcOE_Name'    \n");
                strSql.Append("     ,b7.vcName as 'vcHaoJiu_Name'    \n");
                strSql.Append("     ,b8.vcName as 'vcReceiver_Name'    \n");
                strSql.Append("     ,b9.vcName as 'vcOriginCompany_Name'    \n");
                strSql.Append("     ,b10.vcName as 'vcFXDiff_Name'    \n");
                strSql.Append("     ,case when vcSQState='0' then '未确认' when vcSQState='2' then 'OK' when vcSQState='3' then 'NG' end as 'vcSQContent'    \n");
                strSql.Append("     ,'0' as selected,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     ,CONVERT(varchar(100),dSyncTime, 111) as dSyncTimeStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dTimeFrom, 111) as dTimeFromStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dTimeTo, 111) as dTimeToStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dTimeFromSJ, 111) as dTimeFromSJStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dGYSTimeFrom, 111) as dGYSTimeFromStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dGYSTimeTo, 111) as dGYSTimeToStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dJiuBegin, 111) as dJiuBeginStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dJiuEnd, 111) as dJiuEndStr    \n");
                strSql.Append("     ,CONVERT(varchar(100),dSSDate, 111) as dSSDateStr    \n");
                strSql.Append("     ,case when dJiuBegin is not null and dJiuEnd is null and year(GETDATE())-year(dJiuBegin)>0    \n");
                strSql.Append("     then cast(year(GETDATE())-year(dJiuBegin)  as varchar(10))     \n");
                strSql.Append("     else '' end as vcJiuYearSearch    \n");
                strSql.Append("     from     \n");
                strSql.Append("     (     \n");
                strSql.Append("         select * from TUnit \n");
                strSql.Append("         where 1=1  \n");
                if (!string.IsNullOrEmpty(strOriginCompany))
                {
                    strSql.Append("         and vcOriginCompany = '"+strOriginCompany+"'  \n");
                }
                strSql.Append("     ) a    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C002'    \n");
                strSql.Append("     ) b on a.vcChange = b.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C019'    \n");
                strSql.Append("     )b2 on a.vcSQState = b2.vcValue    \n");
                strSql.Append("     left join    \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C003'    \n");
                strSql.Append("     )b4 on a.vcInOutflag = b4.vcValue    \n");
                strSql.Append("     left join    \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C016'    \n");
                strSql.Append("     )b5 on a.vcSYTCode = b5.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C012'    \n");
                strSql.Append("     )b6 on a.vcOE = b6.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C004'    \n");
                strSql.Append("     ) b7 on a.vcHaoJiu = b7.vcValue    \n");
                strSql.Append("     left join    \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C005'    \n");
                strSql.Append("     )b8 on a.vcReceiver = b8.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C006'    \n");
                strSql.Append("     )b9 on a.vcOriginCompany = b9.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C028'    \n");
                strSql.Append("     )b10 on a.vcFXDiff = b10.vcValue    \n");
                strSql.Append("     where     \n");
                strSql.Append("     1=1    \n");
                if (string.IsNullOrEmpty(strIsShowAll) || strIsShowAll == "0")//如果没点击显示全部，则附加常规条件：变更事项不为空
                {
                    strSql.Append("     and (   \n");
                    strSql.Append("     isnull(vcChange,'')<>''    \n");
                    strSql.Append("     or vcSQState not in('2','3')   \n");                //生确状态  0：未确认    1：确认中  2：OK    3：NG
                    strSql.Append("     or dSyncTime is null   \n");
                    strSql.Append("     or dSyncTime = ''   \n");
                    strSql.Append("     )   \n");
                }
                strSql.Append("     order by vcPart_id asc   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索特记
        public DataTable SearchTeji(string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select top 1 vcMeno,'0' as vcModFlag,'0' as vcAddFlag from TUnit where vcPart_id='" + strPart_id + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    //新增
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("      INSERT INTO  TUnit       \n");
                        sql.Append("      (       \n");
                        sql.Append("       dSyncTime,vcChange,vcSPINo,vcSQContent,vcSQState,vcDiff      \n");
                        sql.Append("       ,vcPart_id,vcCarTypeDesign,vcCarTypeDev,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ      \n");
                        sql.Append("       ,vcBJDiff,vcPartReplace,vcPartNameEn,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag      \n");
                        sql.Append("       ,vcSupplier_id,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode,vcSCSName,vcSCSAdress      \n");
                        sql.Append("       ,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id,vcHaoJiu,dJiuBegin,dJiuEnd      \n");
                        sql.Append("       ,vcJiuYear,vcNXQF,dSSDate,vcMeno,vcFXDiff,vcFXNo,vcNum1      \n");
                        sql.Append("       ,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8      \n");
                        sql.Append("       ,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15      \n");
                        sql.Append("       ,vcZXBZNo,vcReceiver,vcOriginCompany,vcOperator,dOperatorTime,vcRemark      \n");
                        sql.Append("      )      \n");
                        sql.Append("      VALUES      \n");
                        sql.Append("      (      \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSyncTime"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + ",   \r\n");
                        sql.Append("'未确认',   \r\n");
                        sql.Append("'0',   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeFromSJ"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartReplace"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCHPlace"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHKPart_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], true) + ",   \r\n");
                        //旧型经年由旧型开始和结束时间计算得出
                        #region 计算旧型经年
                        if (
                                (listInfoData[i]["dJiuBegin"] != null && listInfoData[i]["dJiuBegin"].ToString() != "")
                                 &&
                                (listInfoData[i]["dJiuEnd"] == null || listInfoData[i]["dJiuEnd"].ToString() == "")
                            )
                        {
                            DateTime datetime1 = Convert.ToDateTime(listInfoData[i]["dJiuBegin"]);
                            int iJiuYear = DateTime.Now.Year - datetime1.Year;
                            sql.Append("'" + iJiuYear + "',   \r\n");
                        }
                        else
                        {
                            sql.Append("null,   \r\n");
                        }
                        #endregion

                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + ",   \r\n");
                        sql.Append(@""+ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + ",   \r\n");
                        sql.Append("'" + strUserId + "',     \r\n");
                        sql.Append("GETDATE(),     \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcRemark"], false) + "     \r\n");
                        sql.Append(");   \r\n");
                    }
                    //修改
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("      update TUnit set       \r\n");
                        sql.Append("      dSyncTime = " + ComFunction.getSqlValue(listInfoData[i]["dSyncTime"], true) + "      \r\n");
                        sql.Append("      ,vcChange = " + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "      \r\n");
                        sql.Append("      ,vcSPINo = " + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "      \r\n");
                        if(listInfoData[i]["vcSQState"]!=null)
                            sql.Append("      ,vcSQState = " + ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + "      \r\n");
                        sql.Append("      ,vcSQContent = " + ComFunction.getSqlValue(listInfoData[i]["vcSQContent"], false) + "      \r\n");
                        sql.Append("      ,vcDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeDesign = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeDev = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeName = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + "      \r\n");
                        sql.Append("      ,dTimeFrom = " + ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + "      \r\n");
                        sql.Append("      ,dTimeTo = " + ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + "      \r\n");
                        sql.Append("      ,dTimeFromSJ = " + ComFunction.getSqlValue(listInfoData[i]["dTimeFromSJ"], true) + "      \r\n");
                        sql.Append("      ,vcBJDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + "      \r\n");
                        sql.Append("      ,vcPartReplace = " + ComFunction.getSqlValue(listInfoData[i]["vcPartReplace"], false) + "      \r\n");
                        sql.Append("      ,vcPartNameEn = " + ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + "      \r\n");
                        sql.Append("      ,vcPartNameCn = " + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + "      \r\n");
                        sql.Append("      ,vcHKGC = " + ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + "      \r\n");
                        sql.Append("      ,vcBJGC = " + ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + "      \r\n");
                        sql.Append("      ,vcInOutflag = " + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_Name = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "      \r\n");
                        sql.Append("      ,vcSCPlace = " + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + "      \r\n");
                        sql.Append("      ,vcCHPlace = " + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace"], false) + "      \r\n");
                        sql.Append("      ,vcSYTCode = " + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "      \r\n");
                        sql.Append("      ,vcSCSName = " + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "      \r\n");
                        sql.Append("      ,vcSCSAdress = " + ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + "      \r\n");
                        sql.Append("      ,dGYSTimeFrom = " + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + "      \r\n");
                        sql.Append("      ,dGYSTimeTo = " + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + "      \r\n");
                        sql.Append("      ,vcOE = " + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "      \r\n");
                        sql.Append("      ,vcHKPart_id = " + ComFunction.getSqlValue(listInfoData[i]["vcHKPart_id"], false) + "      \r\n");
                        sql.Append("      ,vcHaoJiu = " + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "      \r\n");
                        sql.Append("      ,dJiuBegin = " + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], true) + "      \r\n");
                        sql.Append("      ,dJiuEnd = " + ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], true) + "      \r\n");
                        //旧型经年由旧型开始和结束时间计算得出
                        #region 计算旧型经年

                        if (
                                (listInfoData[i]["dJiuBegin"] != null && listInfoData[i]["dJiuBegin"].ToString() != "")
                                 &&
                                (listInfoData[i]["dJiuEnd"] == null || listInfoData[i]["dJiuEnd"].ToString() == "")
                            )
                        {
                            DateTime datetime1 = Convert.ToDateTime(listInfoData[i]["dJiuBegin"]);
                            int iJiuYear = DateTime.Now.Year - datetime1.Year;
                            sql.Append(",vcJiuYear = '" + iJiuYear + "'   \r\n");
                        }
                        else
                        {
                            sql.Append(",vcJiuYear = null   \r\n");
                        }
                        #endregion

                        sql.Append("      ,vcNXQF = " + ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + "      \r\n");
                        sql.Append("      ,dSSDate = " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "      \r\n");
                        sql.Append("      ,vcFXDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "      \r\n");
                        sql.Append("      ,vcFXNo = " + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "      \r\n");
                        sql.Append("      ,vcNum1 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + "      \r\n");
                        sql.Append("      ,vcNum2 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + "      \r\n");
                        sql.Append("      ,vcNum3 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + "      \r\n");
                        sql.Append("      ,vcNum4 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + "      \r\n");
                        sql.Append("      ,vcNum5 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + "      \r\n");
                        sql.Append("      ,vcNum6 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + "      \r\n");
                        sql.Append("      ,vcNum7 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + "      \r\n");
                        sql.Append("      ,vcNum8 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + "      \r\n");
                        sql.Append("      ,vcNum9 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + "      \r\n");
                        sql.Append("      ,vcNum10 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + "      \r\n");
                        sql.Append("      ,vcNum11 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + "      \r\n");
                        sql.Append("      ,vcNum12 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + "      \r\n");
                        sql.Append("      ,vcNum13 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + "      \r\n");
                        sql.Append("      ,vcNum14 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + "      \r\n");
                        sql.Append("      ,vcNum15 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + "      \r\n");
                        sql.Append(@"      ,vcZXBZNo = " + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "      \r\n");
                        sql.Append("      ,vcReceiver = " + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "      \r\n");
                        sql.Append("      ,vcOriginCompany = " + ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + "      \r\n");
                        sql.Append("      ,vcOperator = '" + strUserId + "'      \r\n");
                        sql.Append("      ,dOperatorTime = GETDATE()      \r\n");//按品番、包装工厂、供应商代码、收货方 更新
                        sql.Append("      ,vcRemark = " + ComFunction.getSqlValue(listInfoData[i]["vcRemark"], false) + "      \r\n");
                        sql.Append("      where iAutoId=" + listInfoData[i]["iAutoId"] + "   \r\n ");
                        sql.Append("      ;       \r\n ");
                    }
                   
                }
                //以下追加验证数据库中是否存在品番重叠判断，如果存在则终止提交，原单位数据唯一性：品番、包装工厂、供应商代码、收货方
                if (sql.Length > 0)
                {
                    sql.Append("        declare @errorPart varchar(5000)        \r\n");
                    sql.Append("        set @errorPart =         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("        select a.vcPart_id+';' from         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("           select distinct vcPart_id from         \r\n");
                    sql.Append("           (         \r\n");
                    sql.Append("               select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver from TUnit          \r\n");
                    sql.Append("         	  group by vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver                 \r\n");
                    sql.Append("         	  having COUNT(*)>1                 \r\n");
                    sql.Append("           )a         \r\n");
                    sql.Append("        ) a for xml path('')        \r\n");
                    sql.Append("        )        \r\n");
                    sql.Append("        if @errorPart<>''        \r\n");
                    sql.Append("        begin        \r\n");
                    sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                    sql.Append("        end        \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TUnit where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorPartId)
        {

            try
            {
                StringBuilder sql = new StringBuilder();
                #region 创建临时表
                sql.Append("        if object_id('tempdb..#TUnit_temp') is not null  \n");
                sql.Append("        Begin  \n");
                sql.Append("        drop  table #TUnit_temp  \n");
                sql.Append("        End  \n");
                sql.Append("        select * into #TUnit_temp from       \n");
                sql.Append("        (      \n");
                sql.Append("      	  select * from TUnit where 1=0      \n");
                sql.Append("        ) a      ;\n");
                sql.Append("        ALTER TABLE #TUnit_temp drop column iAutoId  ;\n");
                sql.Append("        ALTER TABLE #TUnit_temp ADD  iAutoId int     ;\n");


                #endregion
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    sql.Append("            \n");
                    sql.Append("      insert into #TUnit_temp       \n");
                    sql.Append("       (         \n");
                    sql.Append("       iAutoId,dSyncTime,vcChange,vcSPINo,vcSQState,vcSQContent,vcDiff,vcPart_id         \n");
                    sql.Append("       ,vcCarTypeDesign,vcCarTypeDev,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ         \n");
                    sql.Append("       ,vcBJDiff,vcPartReplace,vcPartNameEn,vcPartNameCn,vcHKGC,vcBJGC         \n");
                    sql.Append("       ,vcInOutflag,vcSupplier_id,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode         \n");
                    sql.Append("       ,vcSCSName,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id         \n");
                    sql.Append("       ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF,dSSDate         \n");
                    sql.Append("       ,vcMeno,vcFXDiff,vcFXNo,vcNum1,vcNum2,vcNum3,vcNum4         \n");
                    sql.Append("       ,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10         \n");
                    sql.Append("       ,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo         \n");
                    sql.Append("       ,vcReceiver,vcOriginCompany,vcOperator,dOperatorTime,vcRemark         \n");
                    sql.Append("       ) values         \n");
                    sql.Append("                ");
                    sql.Append("      (      \n");
                    sql.Append("      " + ComFunction.getSqlValue(dt.Rows[i]["iAutoId"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dSyncTime"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcChange_Name"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSPINo"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSQState"], true) + "      \n");//注意生确状态没内容应该是null
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSQContent"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcDiff"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeDesign"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeDev"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeName"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dTimeFrom"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dTimeTo"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dTimeFromSJ"], true) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcBJDiff"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcPartReplace"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcPartNameEn"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcPartNameCn"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcHKGC"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcBJGC"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcInOutflag_Name"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_Name"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSCPlace"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcCHPlace"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSYTCode_Name"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSCSName"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcSCSAdress"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dGYSTimeFrom"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dGYSTimeTo"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcOE_Name"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcHKPart_id"], false) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcHaoJiu_Name"], false) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dJiuBegin"], true) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dJiuEnd"], true) + "        \n");

                    //有旧型开始时间没有结束时间的，用当前时间-旧型开始时间=旧型经年
                    #region 计算旧型经年
                    if (
                        (dt.Rows[i]["dJiuBegin"] != null && dt.Rows[i]["dJiuBegin"].ToString() != "")
                        && (dt.Rows[i]["dJiuEnd"] == null || dt.Rows[i]["dJiuEnd"].ToString() == "")
                        )
                    {
                        DateTime datetime1 = Convert.ToDateTime(dt.Rows[i]["dJiuBegin"]);
                        int iJiuYear = DateTime.Now.Year - datetime1.Year;
                        sql.Append(",'" + iJiuYear + "'   \r\n");
                    }
                    else
                    {
                        sql.Append(",null   \r\n");
                    }
                    #endregion
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNXQF"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["dSSDate"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcMeno"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcFXDiff"], true) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcFXNo"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum1"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum2"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum3"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum4"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum5"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum6"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum7"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum8"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum9"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum10"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum11"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum12"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum13"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum14"], true) + "      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcNum15"], true) + "      \n");
                    sql.Append(@"      ," + ComFunction.getSqlValue(dt.Rows[i]["vcZXBZNo"], true) + "       \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcReceiver_Name"], true) + "     \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcOriginCompany_Name"], true) + "     \n");
                    sql.Append("      ,'" + strUserId + "'      \n");
                    sql.Append("      ,GETDATE()      \n");
                    sql.Append("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcRemark"], false) + "     \n");
                    sql.Append("      );      \n");
                    #endregion

                }

                #region 两张表进行连接，相同的部分进行UPDATE操作，不同的部分进行INSERT操作
                sql.Append("      update TUnit set       \n");
                sql.Append("       dSyncTime=b.dSyncTime          \n");
                sql.Append("      ,vcChange=b.vcChange          \n");
                sql.Append("      ,vcSPINo=b.vcSPINo          \n");
                sql.Append("      ,vcSQState=isnull(b.vcSQState,a.vcSQState)          \n");//注意null的代表不需要修改生确状态，还等于自己
                sql.Append("      ,vcSQContent=b.vcSQContent          \n");
                sql.Append("      ,vcDiff=b.vcDiff          \n");
                sql.Append("      ,vcCarTypeDesign=b.vcCarTypeDesign          \n");
                sql.Append("      ,vcCarTypeDev=b.vcCarTypeDev          \n");
                sql.Append("      ,vcCarTypeName=b.vcCarTypeName          \n");
                sql.Append("      ,dTimeFrom=b.dTimeFrom          \n");
                sql.Append("      ,dTimeTo=b.dTimeTo          \n");
                sql.Append("      ,dTimeFromSJ=b.dTimeFromSJ          \n");
                sql.Append("      ,vcBJDiff=b.vcBJDiff          \n");
                sql.Append("      ,vcPartReplace=b.vcPartReplace          \n");
                sql.Append("      ,vcPartNameEn=b.vcPartNameEn          \n");
                sql.Append("      ,vcPartNameCn=b.vcPartNameCn          \n");
                sql.Append("      ,vcHKGC=b.vcHKGC          \n");
                sql.Append("      ,vcBJGC=b.vcBJGC          \n");
                sql.Append("      ,vcInOutflag=b.vcInOutflag          \n");
                sql.Append("      ,vcSupplier_id=b.vcSupplier_id          \n");
                sql.Append("      ,vcSupplier_Name=b.vcSupplier_Name          \n");
                sql.Append("      ,vcSCPlace=b.vcSCPlace          \n");
                sql.Append("      ,vcCHPlace=b.vcCHPlace          \n");
                sql.Append("      ,vcSYTCode=b.vcSYTCode          \n");
                sql.Append("      ,vcSCSName=b.vcSCSName          \n");
                sql.Append("      ,vcSCSAdress=b.vcSCSAdress          \n");
                sql.Append("      ,dGYSTimeFrom=b.dGYSTimeFrom          \n");
                sql.Append("      ,dGYSTimeTo=b.dGYSTimeTo          \n");
                sql.Append("      ,vcOE=b.vcOE          \n");
                sql.Append("      ,vcHKPart_id=b.vcHKPart_id          \n");
                sql.Append("      ,vcHaoJiu=b.vcHaoJiu          \n");
                sql.Append("      ,dJiuBegin=b.dJiuBegin          \n");
                sql.Append("      ,dJiuEnd=b.dJiuEnd          \n");
                sql.Append("      ,vcJiuYear=b.vcJiuYear          \n");
                sql.Append("      ,vcNXQF=b.vcNXQF          \n");
                sql.Append("      ,dSSDate=b.dSSDate          \n");
                sql.Append("      ,vcMeno=b.vcMeno          \n");
                sql.Append("      ,vcFXDiff=b.vcFXDiff          \n");
                sql.Append("      ,vcFXNo=b.vcFXNo          \n");
                sql.Append("      ,vcNum1=b.vcNum1          \n");
                sql.Append("      ,vcNum2=b.vcNum2          \n");
                sql.Append("      ,vcNum3=b.vcNum3          \n");
                sql.Append("      ,vcNum4=b.vcNum4          \n");
                sql.Append("      ,vcNum5=b.vcNum5          \n");
                sql.Append("      ,vcNum6=b.vcNum6          \n");
                sql.Append("      ,vcNum7=b.vcNum7          \n");
                sql.Append("      ,vcNum8=b.vcNum8          \n");
                sql.Append("      ,vcNum9=b.vcNum9          \n");
                sql.Append("      ,vcNum10=b.vcNum10          \n");
                sql.Append("      ,vcNum11=b.vcNum11          \n");
                sql.Append("      ,vcNum12=b.vcNum12          \n");
                sql.Append("      ,vcNum13=b.vcNum13          \n");
                sql.Append("      ,vcNum14=b.vcNum14          \n");
                sql.Append("      ,vcNum15=b.vcNum15          \n");
                sql.Append("      ,vcZXBZNo=b.vcZXBZNo          \n");
                sql.Append("      ,vcReceiver=b.vcReceiver          \n");
                sql.Append("      ,vcOriginCompany=b.vcOriginCompany          \n");
                sql.Append("      ,vcOperator=b.vcOperator          \n");
                sql.Append("      ,dOperatorTime=b.dOperatorTime          \n");
                sql.Append("      ,vcRemark=b.vcRemark          \n");
                sql.Append("      from TUnit a      \n");
                sql.Append("      inner join       \n");
                sql.Append("      (      \n");
                sql.Append("      	select * from #TUnit_temp      \n");
                sql.Append("      ) b      \n");
                sql.Append("      on a.iAutoId = b.iAutoId and a.vcPart_id=b.vcPart_id     \n");

                sql.Append("      insert into TUnit      \n");
                sql.Append("      (      \n");
                sql.Append("      dSyncTime,vcChange,vcSPINo,vcSQContent,vcSQState,vcDiff,vcPart_id,vcCarTypeDesign,vcCarTypeDev      \n");
                sql.Append("      ,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ,vcBJDiff,vcPartReplace,vcPartNameEn      \n");
                sql.Append("      ,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag,vcSupplier_id,vcSupplier_Name,vcSCPlace      \n");
                sql.Append("      ,vcCHPlace,vcSYTCode,vcSCSName,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id      \n");
                sql.Append("      ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF,dSSDate,vcMeno,vcFXDiff,vcFXNo      \n");
                sql.Append("      ,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11      \n");
                sql.Append("      ,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo,vcReceiver,vcOriginCompany,vcOperator      \n");
                sql.Append("      ,dOperatorTime,vcRemark      \n");
                sql.Append("      )       \n");
                sql.Append("      select a.dSyncTime,a.vcChange,a.vcSPINo,a.vcSQContent,a.vcSQState,a.vcDiff,a.vcPart_id,a.vcCarTypeDesign,a.vcCarTypeDev      \n");
                sql.Append("      ,a.vcCarTypeName,a.dTimeFrom,a.dTimeTo,a.dTimeFromSJ,a.vcBJDiff,a.vcPartReplace,a.vcPartNameEn      \n");
                sql.Append("      ,a.vcPartNameCn,a.vcHKGC,a.vcBJGC,a.vcInOutflag,a.vcSupplier_id,a.vcSupplier_Name,a.vcSCPlace      \n");
                sql.Append("      ,a.vcCHPlace,a.vcSYTCode,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcOE,a.vcHKPart_id      \n");
                sql.Append("      ,a.vcHaoJiu,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcNXQF,a.dSSDate,a.vcMeno,a.vcFXDiff,a.vcFXNo      \n");
                sql.Append("      ,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10,a.vcNum11      \n");
                sql.Append("      ,a.vcNum12,a.vcNum13,a.vcNum14,a.vcNum15,a.vcZXBZNo,a.vcReceiver,a.vcOriginCompany,a.vcOperator      \n");
                sql.Append("      ,a.dOperatorTime,vcRemark from #TUnit_temp a      \n");
                sql.Append("      left join       \n");
                sql.Append("      (          \n");
                sql.Append("          select iAutoId from TUnit           \n");
                sql.Append("      ) b         \n");
                sql.Append("      on a.iAutoId = b.iAutoId    \n");
                sql.Append("      where b.iAutoId is null      \n");
                #endregion

                //以下追加验证数据库中是否存在品番重叠判断，如果存在则终止提交，原单位数据唯一性：品番、包装工厂、供应商代码、收货方
                if (sql.Length > 0)
                {
                    sql.Append("        declare @errorPart varchar(5000)        \r\n");
                    sql.Append("        set @errorPart =         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("        select a.vcPart_id+';' from         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("           select distinct vcPart_id from         \r\n");
                    sql.Append("           (         \r\n");
                    sql.Append("               select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver from TUnit          \r\n");
                    sql.Append("         	  group by vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver                 \r\n");
                    sql.Append("         	  having COUNT(*)>1                 \r\n");
                    sql.Append("           )a         \r\n");
                    sql.Append("        ) a for xml path('')        \r\n");
                    sql.Append("        )        \r\n");
                    sql.Append("        if @errorPart<>''        \r\n");
                    sql.Append("        begin        \r\n");
                    sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                    sql.Append("        end        \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion

        #region 生确单发行
        public void sqSend(List<Dictionary<string, Object>> listInfoData, string strSqDate, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                #region 创建临时表
                strSql.AppendLine("        if object_id('tempdb..#TUnit_temp') is not null        ");
                strSql.AppendLine("        begin         ");
                strSql.AppendLine("        drop table #TUnit_temp        ");
                strSql.AppendLine("        end        ");
                strSql.AppendLine("        select * into #TUnit_temp from TUnit       ");
                strSql.AppendLine("        where 1=0        ");
                #endregion

                DataTable dt_CCCFlag = getCCCFlag();

                #region 创建TOutCode临时表，用于插入生确表时关联CCC
                strSql.AppendLine("        if object_id('tempdb..#TOutCode_temp') is not null        ");
                strSql.AppendLine("        begin         ");
                strSql.AppendLine("        drop table #TOutCode_temp        ");
                strSql.AppendLine("        end        ");

                strSql.AppendLine("        create table #TOutCode_temp        ");
                strSql.AppendLine("        (        ");
                strSql.AppendLine("        	vcCodeId varchar(50) null,        ");
                strSql.AppendLine("        	vcCodeName varchar(300) null,        ");
                strSql.AppendLine("        	vcIsColum varchar(1) null,        ");
                strSql.AppendLine("        	vcValue1 varchar(500) null,        ");
                strSql.AppendLine("        	vcValue2 varchar(500) null        ");
                
                strSql.AppendLine("        )insert into #TOutCode_temp(vcCodeId,vcCodeName,vcIsColum,vcValue1,vcValue2) values ('C007','CCC品番认证','1','品番','CCC')       ");
                for (int i = 0; i < dt_CCCFlag.Rows.Count; i++)
                {
                    strSql.AppendLine("        insert into #TOutCode_temp(vcCodeId,vcCodeName,vcIsColum,vcValue1,vcValue2) values ('C007','CCC品番认证','0','"+dt_CCCFlag.Rows[i]["vcValue1"]+"','"+dt_CCCFlag.Rows[i]["vcValue2"]+"')       ");
                }
                #endregion

                #region 向临时表中插入数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strSql.Append("      insert into #TUnit_temp       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("      dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff       \r\n");
                    strSql.Append("      ,vcPart_id,vcCarTypeDesign,vcCarTypeDev,vcCarTypeName,dTimeFrom       \r\n");
                    strSql.Append("      ,dTimeTo,dTimeFromSJ,vcBJDiff,vcPartReplace,vcPartNameEn       \r\n");
                    strSql.Append("      ,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag,vcSupplier_id       \r\n");
                    strSql.Append("      ,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode,vcSCSName       \r\n");
                    strSql.Append("      ,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id       \r\n");
                    strSql.Append("      ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF       \r\n");
                    strSql.Append("      ,dSSDate,vcMeno,vcFXDiff,vcFXNo,vcNum1       \r\n");
                    strSql.Append("      ,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6       \r\n");
                    strSql.Append("      ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11       \r\n");
                    strSql.Append("      ,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo       \r\n");
                    strSql.Append("      ,vcReceiver,vcOriginCompany,dNqDate       \r\n");
                    strSql.Append("      )       \r\n");
                    strSql.Append("      values       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("       GETDATE()       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + "       \r\n");

                    #region 品番特殊处理
                    string vcPart_id = listInfoData[i]["vcPart_id"].ToString();
                    if (vcPart_id.Length == 11)
                    {
                        vcPart_id = vcPart_id.Replace("-", "") + "00";
                        strSql.Append("      ,'" + vcPart_id + "'       \r\n");
                    }
                    else if (vcPart_id.Length == 14)
                    {
                        vcPart_id = vcPart_id.Replace("-", "");
                        strSql.Append("      ,'" + vcPart_id + "'       \r\n");
                    }
                    else
                    {
                        vcPart_id = vcPart_id.Replace("-", "");
                        strSql.Append("      ,'" + vcPart_id + "'       \r\n");
                    }
                    #endregion
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeFromSJ"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartReplace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHKPart_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcJiuYear"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "       \r\n");
                    strSql.Append("      );       \r\n");
                }
                #endregion

                #region 将数据插入生确表，不校验重复性
                strSql.AppendLine("        insert into TSQJD         ");
                strSql.AppendLine("        (         ");
                strSql.AppendLine("         dSSDate,vcJD,vcPart_id,vcSPINo,vcChange,vcCarType         ");
                strSql.AppendLine("        ,vcInOutflag,vcPartName,vcOE,vcSupplier_id,vcFXDiff,vcFXNo         ");
                strSql.AppendLine("        ,vcSumLater         ");
                strSql.AppendLine("        ,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6         ");
                strSql.AppendLine("        ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcSYTCode,vcSCSName         ");
                strSql.AppendLine("        ,vcSCPlace_City,vcCHPlace_City,vcSCSPlace,vcReceiver,dNqDate,GUID,vcZXBZDiff,vcZXBZNo         ");
                strSql.AppendLine("        )         ");
                strSql.AppendLine("        select          ");
                strSql.AppendLine("         GETDATE(),'1' as 'vcJD',a.vcPart_id,a.vcSPINo,a.vcChange,a.vcCarTypeDev         ");
                strSql.AppendLine("        ,a.vcInOutflag,a.vcPartNameEn,a.vcOE,a.vcSupplier_id,a.vcFXDiff,a.vcFXNo         ");
                strSql.AppendLine("        ,a.vcSumLater         ");
                strSql.AppendLine("        ,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6         ");
                strSql.AppendLine("        ,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,a.vcSYTCode,a.vcProduct_name         ");
                strSql.AppendLine("        ,a.vcSCPlace,a.vcCHPlace,a.vcAddress,a.vcReceiver,'"+strSqDate+"',REPLACE( NEWID(),'-',''),vcZXBZDiff,vcZXBZNo         ");
                strSql.AppendLine("         from          ");
                strSql.AppendLine("        (         ");
                strSql.AppendLine("        	select a.*         ");
                strSql.AppendLine("        	, case when (vcNum1 is null and vcNum2 is null and vcNum3 is null and vcNum4 is null and vcNum5 is null and vcNum6  is null and vcNum7  is null  and vcNum8  is null  and vcNum9  is  null and vcNum10  is  null and vcNum11  is null  and vcNum12  is null  and vcNum13  is null  and vcNum14  is  null and vcNum15  is  null) then null         ");
                strSql.AppendLine("        	  else (CONVERT(int,ISNULL(vcNum1,'0'))+ CONVERT(int,ISNULL(vcNum2,'0'))+ CONVERT(int,ISNULL(vcNum3,'0'))+ CONVERT(int,ISNULL(vcNum4,'0'))+ CONVERT(int,ISNULL(vcNum5,'0'))+ CONVERT(int,ISNULL(vcNum6,'0'))+ CONVERT(int,ISNULL(vcNum7,'0'))+ CONVERT(int,ISNULL(vcNum8,'0'))+ CONVERT(int,ISNULL(vcNum9,'0'))+ CONVERT(int,ISNULL(vcNum10,'0'))+ CONVERT(int,ISNULL(vcNum11,'0'))+ CONVERT(int,ISNULL(vcNum12,'0'))+ CONVERT(int,ISNULL(vcNum13,'0'))+ CONVERT(int,ISNULL(vcNum14,'0'))+ CONVERT(int,ISNULL(vcNum15,'0')))          ");
                strSql.AppendLine("        	  end as 'vcSumLater',b.vcProduct_name,b.vcAddress,c.vcValue2 as 'vcZXBZDiff' from #TUnit_temp a         ");
                strSql.AppendLine("        	inner join          ");
                strSql.AppendLine("        	(         ");
                strSql.AppendLine("        		select vcsupplier_id,vcProduct_name,vcAddress from TSupplier         ");
                strSql.AppendLine("        	) b on a.vcSupplier_id = b.vcSupplier_id         ");
                strSql.AppendLine("        	left join          ");
                strSql.AppendLine("        	(         ");
                strSql.AppendLine("        		select vcValue1,vcValue2 from #TOutCode_temp where vcCodeId = 'C007' and vcIsColum = '0'         ");
                strSql.AppendLine("        	) c on SUBSTRING( a.vcPart_id,1,5) = c.vcValue1         ");
                strSql.AppendLine("        ) a         ");
                #endregion

                #region 更新原单位的生确状态、纳期日期、生确描述（生确状态+日期）
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string iAutoId = listInfoData[i]["iAutoId"].ToString();
                    strSql.AppendLine("        update TUnit set          ");
                    strSql.AppendLine("         vcSQState = '1'         ");
                    strSql.AppendLine("        ,dNqDate = '"+strSqDate+"'         ");
                    string vcSQContent = "确认中" + DateTime.Now.ToString("yyyy-MM-dd");
                    strSql.AppendLine("        ,vcSQContent = '" + vcSQContent + "'         ");
                    strSql.AppendLine("        where iAutoId = '"+iAutoId+"'         ");
                }
                
                #endregion

                excute.ExcuteSqlWithStringOper(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取CCC标识
        public DataTable getCCCFlag()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("      select vcValue1,vcValue2 from TOutCode where vcCodeId = 'C007' and vcIsColum = 0       \n");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        #endregion

        #region 数据同步-更新源数据的同步时间
        public void dataSync(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                #region 更新同步日期
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string autoId = listInfoData[i]["iAutoId"].ToString();
                    strSql.Append("     update TUnit set dSyncTime = GETDATE() where iAutoId = '" + autoId + "'        \n");
                }
                #endregion

                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 数据同步
        public void dataSync(string sytCode, List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strMessage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                FS0303_DataAccess_Sync2 sync2 = new FS0303_DataAccess_Sync2("#TUnit_temp",strSql,strUserId);

                sync2.setTempTalbeData(listInfoData, strSql);

                sync2.getUnit2TagMasterSync();

                sync2.getUnit2PriceSync();

                sync2.getUnit2SpMasterSync();

                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString(), sytCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据Name获取Value或者根据Value获取Name
        /// <summary>
        /// 根据Name获取Value或者根据Value获取Name
        /// </summary>
        /// <param name="codeId">Codeid</param>
        /// <param name="strNameOrValue">Name或Value</param>
        /// <param name="_bool">true:返回Value    false:返回Name</param>
        /// <returns></returns>
        public string Name2Value(string codeId, string strNameOrValue, bool _bool)
        {
            StringBuilder strSql = new StringBuilder();
            if (string.IsNullOrEmpty(strNameOrValue))
            {
                return null;
            }
            try
            {
                if (_bool)
                {
                    strSql.Append("     select vcValue from TCode     \n");
                    strSql.Append("     where vcCodeid = '" + codeId + "'    \n");
                    strSql.Append("     and vcName = '" + strNameOrValue + "'    \n");
                }
                else
                {
                    strSql.Append("     select vcName from TCode     \n");
                    strSql.Append("     where vcCodeid = '" + codeId + "'    \n");
                    strSql.Append("     and vcValue = '" + strNameOrValue + "'    \n");
                }

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK").Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion

        #region 获取供应商邮箱地址
        public DataTable getSupplierEmail(string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select address,displayName from   \n");
                strSql.Append("    (   \n");
                strSql.Append("    select vcEmail1 as 'address',vcEmail1 as 'displayName' from TSupplier where vcSupplier_id = '" + strSupplierId + "'   \n");
                strSql.Append("    union all   \n");
                strSql.Append("    select vcEmail2 as 'address',vcEmail2 as 'displayName' from TSupplier where vcSupplier_id = '" + strSupplierId + "'   \n");
                strSql.Append("    union all   \n");
                strSql.Append("    select vcEmail3 as 'address',vcEmail3 as 'displayName' from TSupplier where vcSupplier_id = '" + strSupplierId + "'   \n");
                strSql.Append("    )a where (address is not null and address <>'') and (displayName is not null and displayName <> '')   \n");
                strSql.Append("    group by address,displayName   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取供应商基础数据
        public DataTable getSupplierInfo(string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select * from TSupplier where vcSupplier_id = '" + strSupplierId + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检查是否有异常数据
        public DataTable getErrPartId()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select distinct vcPart_id from              \n");
                strSql.Append("      (               \n");
                strSql.Append("          select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver from TUnit      \n");
                strSql.Append("    	  group by vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver               \n");
                strSql.Append("    	  having COUNT(*)>1                      \n");
                strSql.Append("      )a               \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取当前登陆用户的邮箱模板(邮件主题、邮件内容)
        public DataTable getEmailSetting(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcValue3 as 'vcTitle',vcValue4 as 'vcContent' from TOutCode where vcCodeId = 'C016' and vcIsColum = '0' and vcValue1 = 'FS0303' and vcValue2 = '"+strUserId+"'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存特记
        public void SaveTeJi(List<Dictionary<string, Object>> listInfoData, string strUserId, string strPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sbrTeJi = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strMeno = listInfoData[i]["vcMeno"].ToString();
                    sbrTeJi.Append(strMeno + ";");
                }
                sql.Append("    update TUnit set vcMeno='" + sbrTeJi.ToString() + "' where vcPart_id='" + strPartId + "'   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取原单位用户维护权限
        public DataTable getPri(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select vcValue2 from TOutCode where vcCodeId='C011' and  vcValue1='"+ strUserId + "'            \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取事业体的下游收货方
        public DataTable getReceiver(string strSYTCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select vcValue3 from TOutCode where vcCodeId = 'C004' and vcIsColum = '0' and vcValue1 = 'TFTM'    ");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        #endregion
    }
}