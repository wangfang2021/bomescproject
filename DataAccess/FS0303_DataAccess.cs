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

        #region 按检索条件返回dt
        public DataTable Search(string strIsShowAll, string strOriginCompany)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select *    \n");
                strSql.Append("     ,b.vcName as 'vcChange_Name'    \n");
                strSql.Append("     ,b2.vcName as 'vcSQState_Name'    \n");
                strSql.Append("     ,b3.vcName as 'vcCarTypeDev_Name'    \n");
                strSql.Append("     ,b4.vcName as 'vcInOutflag_Name'    \n");
                strSql.Append("     ,b5.vcName as 'vcSYTCode_Name'    \n");
                strSql.Append("     ,b6.vcName as 'vcOE_Name'    \n");
                strSql.Append("     ,b7.vcName as 'vcHaoJiu_Name'    \n");
                strSql.Append("     ,b8.vcName as 'vcReceiver_Name'    \n");
                strSql.Append("     ,b9.vcName as 'vcOriginCompany_Name'    \n");
                strSql.Append("     ,'0' as selected,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     from TUnit a    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C035'    \n");
                strSql.Append("     ) b on a.vcChange = b.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C019'    \n");
                strSql.Append("     )b2 on a.vcSQState = b2.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId='C009'    \n");
                strSql.Append("     )b3 on a.vcCarTypeDev = b3.vcValue    \n");
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
                strSql.Append("      select '0' as vcModFlag,'0' as vcAddFlag,iAutoId from TUnit         \n");
                strSql.Append("         \n");
                strSql.Append("     where     \n");
                strSql.Append("     1=1    \n");
                if (string.IsNullOrEmpty(strIsShowAll) || strIsShowAll == "0")
                {
                    strSql.Append("     and dTimeFrom <= GETDATE()    \n");
                    strSql.Append("     and dTimeTo >= GETDATE()    \n");
                }
                if (!string.IsNullOrEmpty(strOriginCompany))
                {
                    strSql.Append("     and vcOriginCompany='" + strOriginCompany + "'   \n");
                }
                strSql.Append("     order by vcPart_id asc   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                        sql.Append("       dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff      \n");
                        sql.Append("       ,vcPart_id,vcCarTypeDev,vcCarTypeDesign,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ      \n");
                        sql.Append("       ,vcBJDiff,vcPartReplace,vcPartNameEn,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag      \n");
                        sql.Append("       ,vcSupplier_id,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode,vcSCSName,vcSCSAdress      \n");
                        sql.Append("       ,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id,vcHaoJiu,dJiuBegin,dJiuEnd      \n");
                        sql.Append("       ,vcJiuYear,vcNXQF,dSSDate,vcMeno,vcFXDiff,vcFXNo,vcNum1      \n");
                        sql.Append("       ,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8      \n");
                        sql.Append("       ,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15      \n");
                        sql.Append("       ,vcZXBZNo,vcReceiver,vcOriginCompany,vcOperator,dOperatorTime      \n");
                        sql.Append("      )      \n");
                        sql.Append("      VALUES      \n");
                        sql.Append("      (      \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSyncTime"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeFromSJ"], false) + ",   \r\n");
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
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHKPart_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], false) + ",   \r\n");
                        //旧型经年由旧型开始和结束时间计算得出
                        #region 计算旧型经年
                        var datetime1 = Convert.ToDateTime(ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false));
                        var datetime2 = Convert.ToDateTime(ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], false));
                        var dJiuYear = datetime2.Year - datetime1.Year;
                        #endregion
                        sql.Append("'" + dJiuYear + "'" + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSSDate"], false) + ",   \r\n");
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
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + ",   \r\n");
                        sql.Append("'" + strUserId + "',     \r\n");
                        sql.Append("GETDATE()     \r\n");
                        sql.Append(");   \r\n");
                    }
                    //修改
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("      update TUnit set       \r\n");
                        sql.Append("      vcSQState = " + ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + "      \r\n");
                        sql.Append("      ,vcDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeDesign = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "      \r\n");
                        sql.Append("      ,vcHKGC = " + ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + "      \r\n");
                        sql.Append("      ,vcBJGC = " + ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + "      \r\n");
                        sql.Append("      ,vcInOutflag = " + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "      \r\n");
                        sql.Append("      ,vcOE = " + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "      \r\n");
                        sql.Append("      ,vcHaoJiu = " + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "      \r\n");
                        sql.Append("      ,dJiuBegin = " + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + "      \r\n");
                        sql.Append("      ,vcMeno = " + ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + "      \r\n");
                        sql.Append("      ,vcReceiver = " + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "      \r\n");
                        sql.Append("      ,vcOperator = '" + strUserId + "'      \r\n");
                        sql.Append("      ,dOperatorTime = GETDATE()      \r\n");
                        sql.Append("      where iAutoId ='" + iAutoId + "';  ");
                    }
                    //以下追加验证数据库中是否存在品番重叠判断，如果存在则终止提交
                    if (sql.Length > 0)
                    {
                        sql.Append("        declare @errorPart varchar(5000)        \r\n");
                        sql.Append("        set @errorPart =         \r\n");
                        sql.Append("        (        \r\n");
                        sql.Append("        select a.vcPart_id+';' from         \r\n");
                        sql.Append("        (        \r\n");
                        sql.Append("        select distinct a.vcPart_id,a.dTimeFrom,a.dGYSTimeFrom,a.vcReceiver from TUnit a        \r\n");
                        sql.Append("        inner join        \r\n");
                        sql.Append("        (        \r\n");
                        sql.Append("        select vcPart_id,dTimeFrom,dGYSTimeFrom,vcReceiver from TUnit        \r\n");
                        sql.Append("        group by vcPart_id,dTimeFrom,dGYSTimeFrom,vcReceiver        \r\n");
                        sql.Append("        having COUNT(*)>1        \r\n");
                        sql.Append("        )b        \r\n");
                        sql.Append("        on a.vcPart_id = b.vcPart_id        \r\n");
                        sql.Append("        and a.dTimeFrom = b.dTimeFrom        \r\n");
                        sql.Append("        and a.dGYSTimeFrom = b.dGYSTimeFrom        \r\n");
                        sql.Append("        and a.vcReceiver = b.vcReceiver        \r\n");
                        sql.Append("        ) a for xml path('')        \r\n");
                        sql.Append("        )        \r\n");
                        sql.Append("        if @errorPart<>''        \r\n");
                        sql.Append("        begin        \r\n");
                        sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                        sql.Append("        end        \r\n");

                        excute.ExcuteSqlWithStringOper(sql.ToString());
                    }
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
                excute.ExcuteSqlWithStringOper(sql.ToString());
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
                #endregion
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    sql.Append("            \n");
                    sql.Append("      insert into #TUnit_temp       \n");
                    sql.Append("       (         \n");
                    sql.Append("       dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff,vcPart_id         \n");
                    sql.Append("       ,vcCarTypeDev,vcCarTypeDesign,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ         \n");
                    sql.Append("       ,vcBJDiff,vcPartReplace,vcPartNameEn,vcPartNameCn,vcHKGC,vcBJGC         \n");
                    sql.Append("       ,vcInOutflag,vcSupplier_id,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode         \n");
                    sql.Append("       ,vcSCSName,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id         \n");
                    sql.Append("       ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF,dSSDate         \n");
                    sql.Append("       ,vcMeno,vcFXNo,vcNum1,vcNum2,vcNum3,vcNum4         \n");
                    sql.Append("       ,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10         \n");
                    sql.Append("       ,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo         \n");
                    sql.Append("       ,vcReceiver,vcOriginCompany,vcOperator,dOperatorTime         \n");
                    sql.Append("       ) values         \n");
                    sql.Append("                ");
                    sql.Append("      (      \n");
                    sql.Append("      '" + dt.Rows[i]["dSyncTime"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcChange_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSPINo"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSQState_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcDiff"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcPart_id"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcCarTypeDev"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcCarTypeDesign"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcCarTypeName"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dTimeFrom"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dTimeTo"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dTimeFromSJ"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcBJDiff"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcPartReplace"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcPartNameEn"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcPartNameCn"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcHKGC"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcBJGC"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcInOutflag_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSupplier_id"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSupplier_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSCPlace"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcCHPlace"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSYTCode_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSCSName"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcSCSAdress"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dGYSTimeFrom"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dGYSTimeTo"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcOE_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcHKPart_id"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcHaoJiu_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dJiuBegin"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dJiuEnd"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcJiuYear"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNXQF"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["dSSDate"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcMeno"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcFXNo"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum1"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum2"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum3"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum4"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum5"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum6"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum7"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum8"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum9"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum10"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum11"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum12"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum13"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum14"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcNum15"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcZXBZNo"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcReceiver_Name"] + "'      \n");
                    sql.Append("      ,'" + dt.Rows[i]["vcOriginCompany_Name"] + "'      \n");
                    sql.Append("      ,'" + strUserId + "'      \n");
                    sql.Append("      ,GETDATE()      \n");
                    sql.Append("      );      \n");
                    #endregion

                }

                #region 两张表进行连接，相同的部分进行UPDATE操作，不同的部分进行INSERT操作
                sql.Append("      update TUnit set       \n");
                sql.Append("       dSyncTime=b.dSyncTime          \n");
                sql.Append("      ,vcChange=b.vcChange          \n");
                sql.Append("      ,vcSPINo=b.vcSPINo          \n");
                sql.Append("      ,vcSQState=b.vcSQState          \n");
                sql.Append("      ,vcDiff=b.vcDiff          \n");
                sql.Append("      ,vcPart_id=b.vcPart_id          \n");
                sql.Append("      ,vcCarTypeDev=b.vcCarTypeDev          \n");
                sql.Append("      ,vcCarTypeDesign=b.vcCarTypeDesign          \n");
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
                sql.Append("      from TUnit_temp b      \n");
                sql.Append("      inner join       \n");
                sql.Append("      (      \n");
                sql.Append("      	select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver from #TUnit      \n");
                sql.Append("      ) a      \n");
                sql.Append("      on a.vcPart_id = b.vcPart_id      \n");
                sql.Append("      and a.vcSYTCode = b.vcSYTCode      \n");
                sql.Append("      and a.vcSupplier_id = b.vcSupplier_id      \n");
                sql.Append("      and a.vcReceiver = b.vcReceiver ;     \n");

                sql.Append("      insert into TUnit      \n");
                sql.Append("      (      \n");
                sql.Append("      dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff,vcPart_id,vcCarTypeDev,vcCarTypeDesign      \n");
                sql.Append("      ,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ,vcBJDiff,vcPartReplace,vcPartNameEn      \n");
                sql.Append("      ,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag,vcSupplier_id,vcSupplier_Name,vcSCPlace      \n");
                sql.Append("      ,vcCHPlace,vcSYTCode,vcSCSName,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id      \n");
                sql.Append("      ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF,dSSDate,vcMeno,vcFXDiff,vcFXNo      \n");
                sql.Append("      ,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11      \n");
                sql.Append("      ,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo,vcReceiver,vcOriginCompany,vcOperator      \n");
                sql.Append("      ,dOperatorTime      \n");
                sql.Append("      )       \n");
                sql.Append("      select a.dSyncTime,a.vcChange,a.vcSPINo,a.vcSQState,a.vcDiff,a.vcPart_id,a.vcCarTypeDev,a.vcCarTypeDesign      \n");
                sql.Append("      ,a.vcCarTypeName,a.dTimeFrom,a.dTimeTo,a.dTimeFromSJ,a.vcBJDiff,a.vcPartReplace,a.vcPartNameEn      \n");
                sql.Append("      ,a.vcPartNameCn,a.vcHKGC,a.vcBJGC,a.vcInOutflag,a.vcSupplier_id,a.vcSupplier_Name,a.vcSCPlace      \n");
                sql.Append("      ,a.vcCHPlace,a.vcSYTCode,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcOE,a.vcHKPart_id      \n");
                sql.Append("      ,a.vcHaoJiu,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcNXQF,a.dSSDate,a.vcMeno,a.vcFXDiff,a.vcFXNo      \n");
                sql.Append("      ,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10,a.vcNum11      \n");
                sql.Append("      ,a.vcNum12,a.vcNum13,a.vcNum14,a.vcNum15,a.vcZXBZNo,a.vcReceiver,a.vcOriginCompany,a.vcOperator      \n");
                sql.Append("      ,a.dOperatorTime from #TUnit_temp a      \n");
                sql.Append("      left join       \n");
                sql.Append("      (          \n");
                sql.Append("          select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver from TUnit           \n");
                sql.Append("      ) b         \n");
                sql.Append("      on a.vcPart_id = b.vcPart_id      \n");
                sql.Append("      and a.vcSYTCode = b.vcSYTCode      \n");
                sql.Append("      and a.vcSupplier_id = b.vcSupplier_id      \n");
                sql.Append("      and a.vcReceiver = b.vcReceiver      \n");
                sql.Append("      where a.vcPart_id is null      \n");
                #endregion

                if (sql.Length > 0)
                {
                    if (sql.Length > 0)
                    {
                        sql.Append("        declare @errorPart varchar(5000)        \r\n");
                        sql.Append("        set @errorPart =         \r\n");
                        sql.Append("        (        \r\n");
                        sql.Append("        select a.vcPart_id+';' from         \r\n");
                        sql.Append("        (        \r\n");
                        sql.Append("        select distinct a.vcPart_id,a.dTimeFrom,a.dGYSTimeFrom,a.vcReceiver from TUnit a        \r\n");
                        sql.Append("        inner join        \r\n");
                        sql.Append("        (        \r\n");
                        sql.Append("        select vcPart_id,dTimeFrom,dGYSTimeFrom,vcReceiver from TUnit        \r\n");
                        sql.Append("        group by vcPart_id,dTimeFrom,dGYSTimeFrom,vcReceiver        \r\n");
                        sql.Append("        having COUNT(*)>1        \r\n");
                        sql.Append("        )b        \r\n");
                        sql.Append("        on a.vcPart_id = b.vcPart_id        \r\n");
                        sql.Append("        and a.dTimeFrom = b.dTimeFrom        \r\n");
                        sql.Append("        and a.dGYSTimeFrom = b.dGYSTimeFrom        \r\n");
                        sql.Append("        and a.vcReceiver = b.vcReceiver        \r\n");
                        sql.Append("        ) a for xml path('')        \r\n");
                        sql.Append("        )        \r\n");
                        sql.Append("        if @errorPart<>''        \r\n");
                        sql.Append("        begin        \r\n");
                        sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                        sql.Append("        end        \r\n");

                        excute.ExcuteSqlWithStringOper(sql.ToString());
                    }
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
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    //根据 品番、供应商代码、包装工厂、收货方四个主键更新原单位纳期
                    //vcPart_id,vcSupplier_id,vcSYTCode,vcReceiver
                    string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                    string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                    string strSYTCode = listInfoData[i]["vcSYTCode"].ToString();
                    string strReceiver = listInfoData[i]["vcReceiver"].ToString();
                    string strChange = listInfoData[i]["vcChange"].ToString();
                    sql.Append("   update TUnit set dNqDate='" + strSqDate + "' where  vcPart_id='" + strPart_id + "' and vcSupplier_id='" + strSupplier_id + "' and vcSYTCode='" + strSYTCode + "' and vcReceiver='" + strReceiver + "'    ;   \n ");
                    //更新生确单
                    //旧型今后必要数
                    decimal sumLater = Convert.ToInt32(listInfoData[i]["vcNum1"]) + Convert.ToInt32(listInfoData[i]["vcNum2"]) + Convert.ToInt32(listInfoData[i]["vcNum3"]) + Convert.ToInt32(listInfoData[i]["vcNum4"]) + Convert.ToInt32(listInfoData[i]["vcNum5"]) + Convert.ToInt32(listInfoData[i]["vcNum6"]) + Convert.ToInt32(listInfoData[i]["vcNum7"]) + Convert.ToInt32(listInfoData[i]["vcNum8"]) + Convert.ToInt32(listInfoData[i]["vcNum9"]) + Convert.ToInt32(listInfoData[i]["vcNum10"]);
                    if (strChange == "1")//新设
                    {//新增
                        sql.Append("   update TUnit set dNqDate='" + strSqDate + "',vcSQState = (select vcValue from TCode where vcCodeId = 'C019' and vcName = '确认中') where  vcPart_id='" + strPart_id + "' and vcSupplier_id='" + strSupplier_id + "' and vcSYTCode='" + strSYTCode + "' and vcReceiver='" + strReceiver + "'    ;   \n ");
                        sql.Append("      INSERT INTO  TSQJD       \n");
                        sql.Append("      (       \n");
                        sql.Append("        dSSDate,vcJD,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id,vcFXDiff,vcFXNo        \n");
                        sql.Append("        ,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcSYTCode,vcReceiver,dNqDate        \n");
                        sql.Append("      )       \n");
                        sql.Append("      VALUES      \n");
                        sql.Append("      (      \n");
                        sql.Append("        '" + listInfoData[i]["dSSDate"] + "',           \n");           //实施日期
                        sql.Append("        '1',           \n");                                            //进度-初始状态位已联络
                        sql.Append("        '" + listInfoData[i]["vcPart_id"] + "',           \n");         //品番
                        sql.Append("        '" + listInfoData[i]["vcSPINo"] + "',           \n");           //设变号
                        sql.Append("        '" + listInfoData[i]["vcChange"] + "',           \n");          //变更事项
                        sql.Append("        '" + listInfoData[i]["vcCarTypeDesign"] + "',           \n");        //车种-原单位的车型开发
                        sql.Append("        '" + listInfoData[i]["vcInOutflag"] + "',           \n");       //内外区分
                        sql.Append("        '" + listInfoData[i]["vcPartNameEn"] + "',           \n");        //品名
                        sql.Append("        '" + listInfoData[i]["vcOE"] + "',           \n");              //OE=SP
                        sql.Append("        '" + listInfoData[i]["vcSupplier_id"] + "',           \n");     //供应商代码
                        sql.Append("        '" + listInfoData[i]["vcFXDiff"] + "',           \n");          //防锈指示
                        sql.Append("        '" + listInfoData[i]["vcFXNo"] + "',           \n");            //防锈指示书
                        sql.Append("        '" + sumLater + "',           \n");            //旧型今后必要数
                        sql.Append("        '" + listInfoData[i]["vcNum1"] + "',           \n");            //旧型1年
                        sql.Append("        '" + listInfoData[i]["vcNum2"] + "',           \n");            //旧型2年
                        sql.Append("        '" + listInfoData[i]["vcNum3"] + "',           \n");            //旧型3年
                        sql.Append("        '" + listInfoData[i]["vcNum4"] + "',           \n");            //旧型4年
                        sql.Append("        '" + listInfoData[i]["vcNum5"] + "',           \n");            //旧型5年
                        sql.Append("        '" + listInfoData[i]["vcNum6"] + "',           \n");            //旧型6年
                        sql.Append("        '" + listInfoData[i]["vcNum7"] + "',           \n");            //旧型7年
                        sql.Append("        '" + listInfoData[i]["vcNum8"] + "',           \n");            //旧型8年
                        sql.Append("        '" + listInfoData[i]["vcNum9"] + "',           \n");            //旧型9年
                        sql.Append("        '" + listInfoData[i]["vcNum10"] + "',           \n");           //旧型10年
                        sql.Append("        '" + listInfoData[i]["vcSYTCode"] + "',           \n");         //包装工厂
                        sql.Append("        '" + listInfoData[i]["vcReceiver"] + "',           \n");        //收货方
                        sql.Append("        '" + listInfoData[i]["dNqDate"] + "'           \n");           //纳期
                        sql.Append("      );      \n");
                    }
                    else
                    {//修改
                        #region 生确单发行需要更改的生确进度表
                        sql.Append("      update TSQJD set           \n");
                        sql.Append("      dSSDate= '" + listInfoData[i]["dSSDate"] + "',vcChange = '" + listInfoData[i]["vcChange"] + "',vcSPINo = '" + listInfoData[i]["vcSPINo"] + "',vcCarType = '" + listInfoData[i]["vcCarTypeDesign"] + "'          \n");
                        sql.Append("      ,vcInOutflag = '" + listInfoData[i]["vcInOutflag"] + "',vcPartName = '" + listInfoData[i]["vcPartNameCn"] + "',vcOE = '" + listInfoData[i]["vcOE"] + "',vcSupplier_id = '" + listInfoData[i]["vcSupplier_id"] + "'          \n");
                        sql.Append("      ,vcFXDiff = '" + listInfoData[i]["vcFXDiff"] + "',vcFXNo = '" + listInfoData[i]["vcFXNo"] + "',vcSumLater = '" + sumLater + "', vcNum1 = '" + listInfoData[i]["vcNum1"] + "'          \n");
                        sql.Append("      ,vcNum2 = '" + listInfoData[i]["vcNum2"] + "',vcNum3 = '" + listInfoData[i]["vcNum3"] + "',vcNum4 = '" + listInfoData[i]["vcNum4"] + "',vcNum5 = '" + listInfoData[i]["vcNum5"] + "'          \n");
                        sql.Append("      ,vcNum6 = '" + listInfoData[i]["vcNum6"] + "',vcNum7 = '" + listInfoData[i]["vcNum7"] + "', vcNum8 = '" + listInfoData[i]["vcNum8"] + "',vcNum9 = '" + listInfoData[i]["vcNum9"] + "'          \n");
                        sql.Append("      ,vcNum10 = '" + listInfoData[i]["vcNum10"] + "',dNqDate = '" + listInfoData[i]["dNqDate"] + "'          \n");
                        sql.Append("      where vcPart_id = '" + listInfoData[i]["vcPart_id"] + "'          \n");
                        sql.Append("      and vcSupplier_id = '" + listInfoData[i]["vcSupplier_id"] + "'          \n");
                        sql.Append("      and vcSYTCode = '" + listInfoData[i]["vcSYTCode"] + "'          \n");
                        sql.Append("      and vcReceiver = '" + listInfoData[i]["vcReceiver"] + "';          \n");
                        #endregion
                    }
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                #region 获取所有的变更事项
                DataTable dtChange = ComFunction.getTCode("C002");
                #endregion

                #region 根据变更事项分别创建临时表
                for (int i = 0; i < dtChange.Rows.Count; i++)
                {
                    //获取临时表的表名
                    string tempTableName = "TUnit_Temp_" + dtChange.Rows[i]["vcValue"].ToString();
                    strSql.Append("        if object_id('tempdb..#" + tempTableName + "') is not null        \r\n");
                    strSql.Append("        begin         \r\n");
                    strSql.Append("        drop table #" + tempTableName + "        \r\n");
                    strSql.Append("        end        \r\n");
                    strSql.Append("        select * into #" + tempTableName + " from         \r\n");
                    strSql.Append("        (        \r\n");
                    strSql.Append("        select * from TUnit where 1=0        \r\n");
                    strSql.Append("        ) a ;       \r\n");
                }
                #endregion

                #region 将用户勾选的数据根据变更事项插入临时表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string tempTableName = "TUnit_Temp_" + listInfoData[i]["vcChange"].ToString();
                    strSql.Append("      insert into #" + tempTableName + "       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("      dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff       \r\n");
                    strSql.Append("      ,vcPart_id,vcCarTypeDev,vcCarTypeDesign,vcCarTypeName,dTimeFrom       \r\n");
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
                    strSql.Append("      ,'" + listInfoData[i]["vcChange"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSPINo"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSQState"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcDiff"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcPart_id"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcCarTypeDev"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcCarTypeDesign"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcCarTypeName"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dTimeFrom"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dTimeTo"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dTimeFromSJ"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcBJDiff"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcPartReplace"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcPartNameEn"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcPartNameCn"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcHKGC"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcBJGC"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcInOutflag"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSupplier_id"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSupplier_Name"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSCPlace"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcCHPlace"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSYTCode"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSCSName"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcSCSAdress"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dGYSTimeFrom"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dGYSTimeTo"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcOE"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcHKPart_id"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcHaoJiu"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dJiuBegin"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dJiuEnd"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcJiuYear"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNXQF"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dSSDate"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcMeno"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcFXDiff"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcFXNo"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum1"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum2"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum3"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum4"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum5"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum6"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum7"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum8"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum9"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum10"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum11"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum12"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum13"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum14"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcNum15"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcZXBZNo"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcReceiver"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["vcOriginCompany"] + "'       \r\n");
                    strSql.Append("      ,'" + listInfoData[i]["dNqDate"] + "'       \r\n");
                    strSql.Append("      )       \r\n");

                }
                #endregion

                #region 同步数据语句
                for (int i = 0; i < dtChange.Rows.Count; i++)
                {
                    //获取表名
                    string tempTableName = "TUnit_temp_" + dtChange.Rows[i]["vcValue"].ToString();

                    switch (tempTableName)
                    {
                        case "TUnit_temp_1":
                            {
                                #region 新车新设

                                #region 价格表
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("      and a.vcSYTCode = b.vcSYTCode      \n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                                #endregion

                                #region 标签表
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion

                                #region 采购表
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSYTCode = b.vcPackingPlant       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion

                                #region 检查表

                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_2":
                            {
                                #region 设变新设

                                #region 价格表
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("      and a.vcSYTCode = b.vcSYTCode      \n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                                #endregion

                                #region 标签表
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion

                                #region 采购表
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSYTCode = b.vcPackingPlant       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion

                                #region 检查表

                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_3":
                            {
                                #region 打切旧型

                                #region 价格表

                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                                #endregion

                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("        vcProjectType = a.vcInOutflag      \r\n");
                                strSql.Append("       ,vcSupplier_Name = a.vcSupplier_Name      \r\n");
                                strSql.Append("       ,dProjectBegin  = a.dGYSTimeFrom      \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,vcHaoJiu = a.vcHaoJiu      \r\n");
                                strSql.Append("       ,vcCarTypeDev = a.vcCarTypeDev      \r\n");
                                strSql.Append("       ,vcCarTypeDesign = a.vcCarTypeDesign      \r\n");
                                strSql.Append("       ,vcPart_Name = a.vcPartNameEn      \r\n");
                                strSql.Append("       ,vcOE = a.vcOE      \r\n");
                                strSql.Append("       ,vcPart_id_HK = a.vcHKPart_id      \r\n");
                                strSql.Append("       ,vcStateFX = a.vcFXDiff      \r\n");
                                strSql.Append("       ,vcFXNO = a.vcFXNo      \r\n");
                                strSql.Append("       ,vcSumLater = a.vcSumLater      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,(      \r\n");
                                strSql.Append("       	 CONVERT(int,vcNum1)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum2)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum4)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum5)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum7)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum8)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum10)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum11)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum12)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum13)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum14)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum15)      \r\n");
                                strSql.Append("       	) as 'vcSumLater',vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion

                                #endregion

                                #region 标签表
                                //不处理，数据无变化
                                #endregion

                                #region 采购表
                                strSql.Append("       update TSPMaster set       \r\n");
                                strSql.Append("        vcHaoJiu = b.vcHaoJiu      \r\n");
                                strSql.Append("       ,vcOldProduction = b.vcNXQF      \r\n");
                                strSql.Append("       ,dOldStartTime = b.dJiuBegin      \r\n");
                                strSql.Append("       ,dDebugTime = b.vcJiuYear      \r\n");
                                strSql.Append("       ,dSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from TSPMaster a      \r\n");
                                strSql.Append("       inner join #" + tempTableName + " b      \r\n");
                                strSql.Append("       on a.vcPartId = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcReceiver = b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplierId = b.vcSupplier_id      \r\n");
                                #endregion

                                #region 检查表
                                //不处理
                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_4":
                            {
                                #region 设变废止

                                #region 价格表

                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                                #endregion

                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("        vcProjectType = a.vcInOutflag      \r\n");
                                strSql.Append("       ,vcSupplier_Name = a.vcSupplier_Name      \r\n");
                                strSql.Append("       ,dProjectBegin  = a.dGYSTimeFrom      \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,vcHaoJiu = a.vcHaoJiu      \r\n");
                                strSql.Append("       ,vcCarTypeDev = a.vcCarTypeDev      \r\n");
                                strSql.Append("       ,vcCarTypeDesign = a.vcCarTypeDesign      \r\n");
                                strSql.Append("       ,vcPart_Name = a.vcPartNameEn      \r\n");
                                strSql.Append("       ,vcOE = a.vcOE      \r\n");
                                strSql.Append("       ,vcPart_id_HK = a.vcHKPart_id      \r\n");
                                strSql.Append("       ,vcStateFX = a.vcFXDiff      \r\n");
                                strSql.Append("       ,vcFXNO = a.vcFXNo      \r\n");
                                strSql.Append("       ,vcSumLater = a.vcSumLater      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,(      \r\n");
                                strSql.Append("       	 CONVERT(int,vcNum1)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum2)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum4)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum5)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum7)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum8)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum10)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum11)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum12)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum13)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum14)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum15)      \r\n");
                                strSql.Append("       	) as 'vcSumLater',vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion

                                #endregion

                                #region 采购表
                                #region 采购品番基础信息表
                                strSql.Append("       update TSPMaster set         \r\n");
                                strSql.Append("        dToTime = a.dTimeTo        \r\n");
                                strSql.Append("       ,dSyncTime = GETDATE()        \r\n");
                                strSql.Append("       from         \r\n");
                                strSql.Append("       (        \r\n");
                                strSql.Append("       	select a.*,b.vcDownRecever from         \r\n");
                                strSql.Append("       	(        \r\n");
                                strSql.Append("       		select * from #" + tempTableName + "        \r\n");
                                strSql.Append("       	)a        \r\n");
                                strSql.Append("       	inner join TCode2 b        \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode        \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever        \r\n");
                                strSql.Append("       ) a        \r\n");
                                strSql.Append("       inner join         \r\n");
                                strSql.Append("       (        \r\n");
                                strSql.Append("       	select vcPartId,vcSupplierId,vcReceiver from TSPMaster        \r\n");
                                strSql.Append("       ) b        \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPartId        \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplierId        \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver        \r\n");
                                #endregion
                                #region 采购品番基础数据供应商工区信息子表
                                strSql.Append("       update TSPMaster_SupplierPlant set          \r\n");
                                strSql.Append("       dToTime = a.dTimeTo         \r\n");
                                strSql.Append("       from          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select a.*,b.vcDownRecever from          \r\n");
                                strSql.Append("       	(         \r\n");
                                strSql.Append("       		select * from #" + tempTableName + "         \r\n");
                                strSql.Append("       	)a         \r\n");
                                strSql.Append("       	inner join TCode2 b         \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode         \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever         \r\n");
                                strSql.Append("       ) a         \r\n");
                                strSql.Append("       inner join          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select vcPartId,vcSupplierId,vcReceiver from TSPMaster         \r\n");
                                strSql.Append("       ) b         \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPartId         \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplierId         \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver         \r\n");
                                #endregion
                                #region 采购品番基础数据箱种收容数信息子表
                                strSql.Append("       update TSPMaster_Box set          \r\n");
                                strSql.Append("       dToTime = a.dTimeTo         \r\n");
                                strSql.Append("       from          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select a.*,b.vcDownRecever from          \r\n");
                                strSql.Append("       	(         \r\n");
                                strSql.Append("       		select * from #" + tempTableName + "         \r\n");
                                strSql.Append("       	)a         \r\n");
                                strSql.Append("       	inner join TCode2 b         \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode         \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever         \r\n");
                                strSql.Append("       ) a         \r\n");
                                strSql.Append("       inner join          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select vcPartId,vcSupplierId,vcReceiver from TSPMaster         \r\n");
                                strSql.Append("       ) b         \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPartId         \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplierId         \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver         \r\n");
                                #endregion
                                #region 采购品番基础数据发注工厂信息子表
                                strSql.Append("       update TSPMaster_OrderPlant set          \r\n");
                                strSql.Append("       dToTime = a.dTimeTo         \r\n");
                                strSql.Append("       from          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select a.*,b.vcDownRecever from          \r\n");
                                strSql.Append("       	(         \r\n");
                                strSql.Append("       		select * from #" + tempTableName + "         \r\n");
                                strSql.Append("       	)a         \r\n");
                                strSql.Append("       	inner join TCode2 b         \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode         \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever         \r\n");
                                strSql.Append("       ) a         \r\n");
                                strSql.Append("       inner join          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select vcPartId,vcSupplierId,vcReceiver from TSPMaster         \r\n");
                                strSql.Append("       ) b         \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPartId         \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplierId         \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver         \r\n");
                                #endregion
                                #region 采购品番基础数据受入信息子表
                                strSql.Append("       update TSPMaster_SufferIn set          \r\n");
                                strSql.Append("       dToTime = a.dTimeTo         \r\n");
                                strSql.Append("       from          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select a.*,b.vcDownRecever from          \r\n");
                                strSql.Append("       	(         \r\n");
                                strSql.Append("       		select * from #" + tempTableName + "         \r\n");
                                strSql.Append("       	)a         \r\n");
                                strSql.Append("       	inner join TCode2 b         \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode         \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever         \r\n");
                                strSql.Append("       ) a         \r\n");
                                strSql.Append("       inner join          \r\n");
                                strSql.Append("       (         \r\n");
                                strSql.Append("       	select vcPartId,vcSupplierId,vcReceiver from TSPMaster         \r\n");
                                strSql.Append("       ) b         \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPartId         \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplierId         \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver         \r\n");
                                #endregion
                                #endregion

                                #region 标签表
                                strSql.Append("       update TtagMaster set       \r\n");
                                strSql.Append("        dTimeFrom = a.dGYSTimeFrom      \r\n");
                                strSql.Append("       ,dTimeTo = b.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDateSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from TtagMaster a      \r\n");
                                strSql.Append("       inner join #" + tempTableName + " b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcCPDCompany = b.vcReceiver      \r\n");
                                #endregion

                                #region 检查表
                                /*
                                 * 更新使用结束时间
                                 */
                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_5":
                            {
                                #region 设变旧型

                                #region 价格表

                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                                #endregion

                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("        vcProjectType = a.vcInOutflag      \r\n");
                                strSql.Append("       ,vcSupplier_Name = a.vcSupplier_Name      \r\n");
                                strSql.Append("       ,dProjectBegin  = a.dGYSTimeFrom      \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,vcHaoJiu = a.vcHaoJiu      \r\n");
                                strSql.Append("       ,vcCarTypeDev = a.vcCarTypeDev      \r\n");
                                strSql.Append("       ,vcCarTypeDesign = a.vcCarTypeDesign      \r\n");
                                strSql.Append("       ,vcPart_Name = a.vcPartNameEn      \r\n");
                                strSql.Append("       ,vcOE = a.vcOE      \r\n");
                                strSql.Append("       ,vcPart_id_HK = a.vcHKPart_id      \r\n");
                                strSql.Append("       ,vcStateFX = a.vcFXDiff      \r\n");
                                strSql.Append("       ,vcFXNO = a.vcFXNo      \r\n");
                                strSql.Append("       ,vcSumLater = a.vcSumLater      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,(      \r\n");
                                strSql.Append("       	 CONVERT(int,vcNum1)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum2)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum4)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum5)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum7)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum8)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum10)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum11)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum12)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum13)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum14)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum15)      \r\n");
                                strSql.Append("       	) as 'vcSumLater',vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion

                                #endregion

                                #region 标签表
                                //不处理，数据无变化
                                #endregion

                                #region 采购表
                                strSql.Append("       update TSPMaster set       \r\n");
                                strSql.Append("        vcHaoJiu = b.vcHaoJiu      \r\n");
                                strSql.Append("       ,vcOldProduction = b.vcNXQF      \r\n");
                                strSql.Append("       ,dOldStartTime = b.dJiuBegin      \r\n");
                                strSql.Append("       ,dDebugTime = b.vcJiuYear      \r\n");
                                strSql.Append("       ,dSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from TSPMaster a      \r\n");
                                strSql.Append("       inner join #" + tempTableName + " b      \r\n");
                                strSql.Append("       on a.vcPartId = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcReceiver = b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplierId = b.vcSupplier_id      \r\n");
                                #endregion

                                #region 检查表
                                //不处理
                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_6":
                            {
                                #region 旧型恢复现号

                                #region 价格表

                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                                #endregion

                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("        vcProjectType = a.vcInOutflag      \r\n");
                                strSql.Append("       ,vcSupplier_Name = a.vcSupplier_Name      \r\n");
                                strSql.Append("       ,dProjectBegin  = a.dGYSTimeFrom      \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,vcHaoJiu = a.vcHaoJiu      \r\n");
                                strSql.Append("       ,vcCarTypeDev = a.vcCarTypeDev      \r\n");
                                strSql.Append("       ,vcCarTypeDesign = a.vcCarTypeDesign      \r\n");
                                strSql.Append("       ,vcPart_Name = a.vcPartNameEn      \r\n");
                                strSql.Append("       ,vcOE = a.vcOE      \r\n");
                                strSql.Append("       ,vcPart_id_HK = a.vcHKPart_id      \r\n");
                                strSql.Append("       ,vcStateFX = a.vcFXDiff      \r\n");
                                strSql.Append("       ,vcFXNO = a.vcFXNo      \r\n");
                                strSql.Append("       ,vcSumLater = a.vcSumLater      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,(      \r\n");
                                strSql.Append("       	 CONVERT(int,vcNum1)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum2)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum4)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum5)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum7)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum8)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum10)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum11)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum12)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum13)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum14)      \r\n");
                                strSql.Append("       	+CONVERT(int,vcNum15)      \r\n");
                                strSql.Append("       	) as 'vcSumLater',vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion

                                #endregion

                                #region 采购表
                                strSql.Append("       update TSPMaster set       \r\n");
                                strSql.Append("        vcHaoJiu = b.vcHaoJiu      \r\n");
                                strSql.Append("       ,dSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from TSPMaster a      \r\n");
                                strSql.Append("       inner join #" + tempTableName + " b      \r\n");
                                strSql.Append("       on a.vcPartId = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcReceiver = b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplierId = b.vcSupplier_id      \r\n");
                                #endregion

                                #region 标签表
                                /*
                                 * 不处理
                                 */
                                #endregion

                                #region 检查表

                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_7":
                            {
                                #region 旧型持续生产

                                #endregion
                            }
                            break;
                        case "TUnit_temp_8":
                            {
                                #region 工程变更-新设

                                #region 价格表

                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcDownRecever<>b.vcPriceChangeInfo      \r\n");
                                #endregion
                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dUseEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion

                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_9":
                            {
                                #region 工程变更-废止

                                #region 价格表
                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dUseEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion
                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcDownRecever<>b.vcPriceChangeInfo      \r\n");
                                #endregion
                                #endregion

                                #region 采购表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TSPMaster set        \r\n");
                                strSql.Append("       dToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSupplierFromTime = b.dGYSTimeFrom       \r\n");
                                strSql.Append("      ,dSupplierToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TSPMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPartId = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplierId = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcReceiver = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion
                                #endregion

                                #region 标签表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TtagMaster set        \r\n");
                                strSql.Append("       dTimeFrom = b.dGYSTimeFrom       \r\n");
                                strSql.Append("      ,dTimeTo = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dDateSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TtagMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPart_Id = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcCPDCompany = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,a.vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion
                                #endregion

                                #region 检查表
                                #region 对不相同部分进行新增操作

                                #endregion
                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_10":
                            {
                                #region 供应商变更-新设

                                #region 价格表
                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dUseEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion
                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcDownRecever<>b.vcPriceChangeInfo      \r\n");
                                #endregion
                                #endregion

                                #region 采购表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TSPMaster set        \r\n");
                                strSql.Append("       dToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSupplierFromTime = b.dGYSTimeFrom       \r\n");
                                strSql.Append("      ,dSupplierToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TSPMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPartId = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplierId = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcReceiver = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion
                                #endregion

                                #region 标签表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TtagMaster set        \r\n");
                                strSql.Append("       dTimeFrom = b.dGYSTimeFrom       \r\n");
                                strSql.Append("      ,dTimeTo = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dDateSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TtagMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPart_Id = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcCPDCompany = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,a.vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion
                                #endregion

                                #region 检查表
                                #region 对不相同部分进行新增操作

                                #endregion
                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_11":
                            {
                                #region 供应商变更-废止

                                #region 价格表
                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dUseEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion
                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcDownRecever<>b.vcPriceChangeInfo      \r\n");
                                #endregion
                                #endregion

                                #region 采购表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TSPMaster set        \r\n");
                                strSql.Append("       dToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSupplierFromTime = b.dGYSTimeFrom       \r\n");
                                strSql.Append("      ,dSupplierToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TSPMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPartId = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplierId = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcReceiver = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion
                                #endregion

                                #region 标签表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TtagMaster set        \r\n");
                                strSql.Append("       dTimeFrom = b.dGYSTimeFrom       \r\n");
                                strSql.Append("      ,dTimeTo = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dDateSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TtagMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPart_Id = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcCPDCompany = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,a.vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion
                                #endregion

                                #region 检查表
                                #region 对不相同部分进行新增操作

                                #endregion
                                #endregion

                                #endregion
                            }
                            break;
                        case "TUnit_temp_12":
                            {
                                #region 包装工厂变更-新设
                                #region 价格表
                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dUseEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion
                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcDownRecever<>b.vcPriceChangeInfo      \r\n");
                                #endregion
                                #endregion

                                #region 采购表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TSPMaster set        \r\n");
                                strSql.Append("       dToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TSPMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPartId = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplierId = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcReceiver = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion
                                #endregion

                                #region 标签表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TtagMaster set        \r\n");
                                strSql.Append("      ,dTimeTo = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dDateSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TtagMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPart_Id = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcCPDCompany = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,a.vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion
                                #endregion

                                #region 检查表
                                #region 对不相同部分进行新增操作

                                #endregion
                                #endregion
                                #endregion
                            }
                            break;
                        case "TUnit_temp_13":
                            {
                                #region 包装工场变更-废止
                                #region 价格表
                                #region 原单位变更事项与价格表的变更事项履历相同，则更新数据
                                strSql.Append("       update TPrice set       \r\n");
                                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dUseEnd = a.dGYSTimeTo      \r\n");
                                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                                strSql.Append("       from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*       \r\n");
                                strSql.Append("       	,vcDownRecever      \r\n");
                                strSql.Append("       	from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join       \r\n");
                                strSql.Append("       	(      \r\n");
                                strSql.Append("       		select vcSYTCode,vcRecever,vcDownRecever from TCode2      \r\n");
                                strSql.Append("       	) b      \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       inner join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                                strSql.Append("       		inner join       \r\n");
                                strSql.Append("       		(      \r\n");
                                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                                strSql.Append("       		) b      \r\n");
                                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcChange = b.vcPriceChangeInfo      \r\n");
                                strSql.Append("       and a.vcPart_id = b.vcPart_id      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                                #endregion
                                #region 原单位变更事项与价格表的变更事项履历不同，则插入一条数据
                                strSql.Append("       insert into TPrice       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                                strSql.Append("       )      \r\n");
                                strSql.Append("       select       \r\n");
                                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                                strSql.Append("        from       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	select a.*      \r\n");
                                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                                strSql.Append("       	,b.vcDownRecever from #" + tempTableName + " a      \r\n");
                                strSql.Append("       	inner join TCode2 b       \r\n");
                                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                                strSql.Append("       ) a      \r\n");
                                strSql.Append("       left join       \r\n");
                                strSql.Append("       (      \r\n");
                                strSql.Append("       	 select a.* from TPrice a      \r\n");
                                strSql.Append("       	 inner join     \r\n");
                                strSql.Append("       	 (     \r\n");
                                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                                strSql.Append("       ) b      \r\n");
                                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                                strSql.Append("       where b.vcPart_Id is null      \r\n");
                                strSql.Append("       and a.vcDownRecever<>b.vcPriceChangeInfo      \r\n");
                                #endregion
                                #endregion

                                #region 采购表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TSPMaster set        \r\n");
                                strSql.Append("       dToTime = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TSPMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPartId = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplierId = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcReceiver = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TSPMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	 vcChanges,vcPackingPlant,vcPartId,vcPartENName,vcCarfamilyCode       \r\n");
                                strSql.Append("      	,vcReceiver,dFromTime,dToTime,vcPartId_Replace,vcInOut       \r\n");
                                strSql.Append("      	,vcOESP,vcHaoJiu,vcOldProduction,dOldStartTime,dDebugTime       \r\n");
                                strSql.Append("      	,vcSupplierId,dSupplierFromTime,dSupplierToTime,vcSupplierName,dSyncTime       \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select        \r\n");
                                strSql.Append("      	 a.vcChange,a.vcSYTCode,a.vcPart_id,a.vcPartNameEn,a.vcCarTypeDesign       \r\n");
                                strSql.Append("      	,a.vcDownRecever,a.dTimeFrom,a.dTimeTo,a.vcPartReplace,a.vcInOutflag       \r\n");
                                strSql.Append("      	,a.vcOE,a.vcHaoJiu,a.vcNXQF,a.dJiuBegin,a.vcJiuYear       \r\n");
                                strSql.Append("      	,a.vcSupplier_id,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcSupplier_Name,GETDATE()       \r\n");
                                strSql.Append("       from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPartId,vcPackingPlant,vcReceiver,vcSupplierId from TSPMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPartId       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplierId       \r\n");
                                strSql.Append("      and a.vcDownRecever =b.vcReceiver       \r\n");
                                strSql.Append("      where b.vcPartId is null       \r\n");
                                #endregion
                                #endregion

                                #region 标签表
                                #region 对相同部分进行更新操作
                                strSql.Append("      update TtagMaster set        \r\n");
                                strSql.Append("      ,dTimeTo = b.dGYSTimeTo       \r\n");
                                strSql.Append("      ,dDateSyncTime = GETDATE()       \r\n");
                                strSql.Append("      from TtagMaster a       \r\n");
                                strSql.Append("      inner join #" + tempTableName + " b       \r\n");
                                strSql.Append("      on a.vcPart_Id = b.vcPart_id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcCPDCompany = b.vcReceiver       \r\n");
                                #endregion
                                #region 对不相同部分进行新增操作
                                strSql.Append("      insert into TtagMaster        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	vcPart_Id,vcCPDCompany,a.vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo             \r\n");
                                strSql.Append("         ,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,dDateSyncTime         \r\n");
                                strSql.Append("      )       \r\n");
                                strSql.Append("      select a.vcPart_id,a.vcDownRecever,a.vcSupplier_id,a.vcCarTypeName,a.vcPartNameCn,a.vcZXBZNo       \r\n");
                                strSql.Append("      	  ,a.vcSCSName,a.vcSCSAdress,a.dTimeFrom,a.dTimeTo,GETDATE() from        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select a.*,b.vcDownRecever from #" + tempTableName + " a       \r\n");
                                strSql.Append("      	inner join TCode2 b        \r\n");
                                strSql.Append("      	on a.vcSYTCode = b.vcSYTCode       \r\n");
                                strSql.Append("      	and a.vcReceiver = b.vcRecever       \r\n");
                                strSql.Append("      ) a       \r\n");
                                strSql.Append("      left join        \r\n");
                                strSql.Append("      (       \r\n");
                                strSql.Append("      	select vcPart_Id,vcCPDCompany,vcSupplier_id from TtagMaster       \r\n");
                                strSql.Append("      ) b       \r\n");
                                strSql.Append("      on a.vcPart_id = b.vcPart_Id       \r\n");
                                strSql.Append("      and a.vcSupplier_id = b.vcSupplier_id       \r\n");
                                strSql.Append("      and a.vcDownRecever = b.vcCPDCompany       \r\n");
                                strSql.Append("      where b.vcPart_Id is null       \r\n");
                                #endregion
                                #endregion

                                #region 检查表
                                #region 对不相同部分进行新增操作

                                #endregion
                                #endregion
                                #endregion
                            }
                            break;
                        case "TUnit_temp_14":
                            {
                                #region 生产打切

                                #endregion
                            }
                            break;
                        case "TUnit_temp_15":
                            {
                                #region 一括生产

                                #endregion
                            }
                            break;
                        case "TUnit_temp_16":
                            {
                                #region 复活

                                #endregion
                            }
                            break;
                        case "TUnit_temp_17":
                            {
                                #region 防锈变更

                                #endregion
                            }
                            break;
                        case "TUnit_temp_18":
                            {
                                #region 价格调整

                                #endregion
                            }
                            break;
                        case "TUnit_temp_19":
                            {
                                #region 时间调整

                                #endregion
                            }
                            break;
                        default:
                            {
                                #region 共通事项

                                #endregion
                            }
                            break;
                    }
                }
                #endregion

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
                
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString()).Rows[0][0].ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }


        }
        #endregion

        #region 按检索条件返回dt
        public DataTable getSupplierEmail(string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcEmail1,vcEmail2,vcEmail3 from TSupplier where vcSupplier_id='"+ strSupplierId + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件返回dt
        public DataTable getEmailSetting(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcTitle,vcContent from TMailMessageSetting where vcUserId='"+ strUserId + "'   \n");
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
                    string strMeno =  listInfoData[i]["vcMeno"].ToString();
                    sbrTeJi.Append(strMeno + ";");
                }
                sql.Append("    update TUnit set vcMeno='"+ sbrTeJi.ToString()+ "' where vcPart_id='"+ strPartId + "'   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
