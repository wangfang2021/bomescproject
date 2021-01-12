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
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C002'    \n");
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
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("      INSERT INTO  TUnit       \n");
                        sql.Append("      (       \n");
                        sql.Append("      dSyncTime , vcSPINo , vcSQState , vcDiff , vcPart_id , vcCarTypeDev       \n");
                        sql.Append("      , vcCarTypeDesign , vcCarTypeName , dTimeFrom , dTimeTo , vcBJDiff , vcPartNameEn       \n");
                        sql.Append("      , vcPartNameCn , vcHKGC , vcBJGC , vcInOutflag , vcSupplier_id , vcSupplier_Name       \n");
                        sql.Append("      , vcSCPlace , vcCHPlace , vcSYTCode , vcSCSName , vcSCSAdress , dGYSTimeFrom       \n");
                        sql.Append("      , dGYSTimeTo , vcOE , vcHaoJiu , vcMeno , vcFXDiff , vcFXNo       \n");
                        sql.Append("      , vcReceiver , vcOriginCompany , vcOperator,dOperatorTime      \n");
                        sql.Append("      )      \n");
                        sql.Append("      VALUES      \n");
                        sql.Append("      (      \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSyncTime"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + ",   \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + ",   \r\n");
                        sql.Append("'" + strUserId + "'     \r\n");
                        sql.Append("'GETDATE()'     \r\n");
                    }
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
                        sql.Append("      ,vcOperator = " + strUserId + "      \r\n");
                        sql.Append("      ,dOperatorTime = GETDATE()      \r\n");
                        sql.Append("      where iAutoId =" + iAutoId + "  ");
                    }
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                #region 创建临时表
                sql.Append("      select * into #TUnit_temp from       \n");
                sql.Append("      (      \n");
                sql.Append("      	select * from TUnit where 1=0      \n");
                sql.Append("      ) a      ;\n");
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
                sql.Append("      update a set a.vcSQState=b.vcSQState,a.vcDiff=b.vcDiff,a.vcCarTypeDesign=b.vcCarTypeDesign      \n");
                sql.Append("      ,a.vcHKGC=b.vcHKGC,a.vcBJGC=b.vcBJGC,a.vcInOutflag=b.vcInOutflag,a.vcOE=b.vcOE      \n");
                sql.Append("      ,a.vcHaoJiu=b.vcHaoJiu,a.dJiuBegin=b.dJiuBegin,a.vcMeno=b.vcMeno,a.vcReceiver=b.vcReceiver      \n");
                sql.Append("      ,a.vcOperator=b.vcOperator,a.dOperatorTime=b.dOperatorTime      \n");
                sql.Append("      from       \n");
                sql.Append("      (      \n");
                sql.Append("      select * from #TUnit_temp      \n");
                sql.Append("      ) b      \n");
                sql.Append("      left join TUnit a on a.vcPart_id=b.vcPart_id      \n");
                sql.Append("      and a.vcSupplier_id=b.vcSupplier_id      \n");
                sql.Append("      and a.vcSYTCode=b.vcSYTCode      \n");
                sql.Append("      and a.vcReceiver=b.vcReceiver      \n");
                sql.Append("      where a.vcPart_id=b.vcPart_id      \n");
                sql.Append("      and a.vcSupplier_id=b.vcSupplier_id      \n");
                sql.Append("      and a.vcSYTCode=b.vcSYTCode      \n");
                sql.Append("      and	a.vcReceiver=b.vcReceiver      \n");
                sql.Append("            \n");
                sql.Append("      insert into TUnit      \n");
                sql.Append("      (      \n");
                sql.Append("      dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff,vcPart_id,vcCarTypeDev,vcCarTypeDesign      \n");
                sql.Append("      ,vcCarTypeName,dTimeFrom,dTimeTo,dTimeFromSJ,vcBJDiff,vcPartReplace,vcPartNameEn      \n");
                sql.Append("      ,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag,vcSupplier_id,vcSupplier_Name,vcSCPlace      \n");
                sql.Append("      ,vcCHPlace,vcSYTCode,vcSCSName,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id      \n");
                sql.Append("      ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF,dSSDate,vcMeno,vcFXDiff,vcFXNo      \n");
                sql.Append("      ,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11      \n");
                sql.Append("      ,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo,vcReceiver,vcOriginCompany,vcOperator      \n");
                sql.Append("      ,dOperatorTime          \n");
                sql.Append("      )      \n");
                sql.Append("      select a.dSyncTime,a.vcChange,a.vcSPINo,a.vcSQState,a.vcDiff,a.vcPart_id,a.vcCarTypeDev,a.vcCarTypeDesign      \n");
                sql.Append("      ,a.vcCarTypeName,a.dTimeFrom,a.dTimeTo,a.dTimeFromSJ,a.vcBJDiff,a.vcPartReplace,a.vcPartNameEn      \n");
                sql.Append("      ,a.vcPartNameCn,a.vcHKGC,a.vcBJGC,a.vcInOutflag,a.vcSupplier_id,a.vcSupplier_Name,a.vcSCPlace      \n");
                sql.Append("      ,a.vcCHPlace,a.vcSYTCode,a.vcSCSName,a.vcSCSAdress,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcOE,a.vcHKPart_id      \n");
                sql.Append("      ,a.vcHaoJiu,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcNXQF,a.dSSDate,a.vcMeno,a.vcFXDiff,a.vcFXNo      \n");
                sql.Append("      ,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10,a.vcNum11      \n");
                sql.Append("      ,a.vcNum12,a.vcNum13,a.vcNum14,a.vcNum15,a.vcZXBZNo,a.vcReceiver,a.vcOriginCompany,a.vcOperator,a.dOperatorTime      \n");
                sql.Append("      from #TUnit_temp a          ");
                sql.Append("      left join TUnit b on a.vcPart_id = b.vcPart_id      \n");
                sql.Append("      and a.vcSupplier_id=b.vcSupplier_id      \n");
                sql.Append("      and a.vcSYTCode=b.vcSYTCode      \n");
                sql.Append("      and a.vcReceiver = b.vcReceiver      \n");
                sql.Append("      where a.vcPart_id not in (select vcPart_id from TUnit)       \n");
                sql.Append("      or a.vcSupplier_id not in (select vcSupplier_id from TUnit)      \n");
                sql.Append("      or a.vcSYTCode not in (select vcSYTCode from TUnit)      \n");
                sql.Append("      or a.vcReceiver not in (select vcReceiver from TUnit)    ;  \n");

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

        #region 生确单发行
        public void sqSend(List<Dictionary<string, Object>> listInfoData,string strSqDate, string strUserId)
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
                    sql.Append("   update TUnit set dNqDate='"+ strSqDate + "' where  vcPart_id='"+ strPart_id + "' and vcSupplier_id='"+ strSupplier_id + "' and vcSYTCode='"+ strSYTCode + "' and vcReceiver='"+ strReceiver + "'    ;   \n ");
                    //更新生确单
                    //旧型今后必要数
                    decimal sumLater = Convert.ToDecimal(listInfoData[i]["vcNum1"]) + Convert.ToDecimal(listInfoData[i]["vcNum2"]) + Convert.ToDecimal(listInfoData[i]["vcNum3"]) + Convert.ToDecimal(listInfoData[i]["vcNum4"]) + Convert.ToDecimal(listInfoData[i]["vcNum5"]) + Convert.ToDecimal(listInfoData[i]["vcNum6"]) + Convert.ToDecimal(listInfoData[i]["vcNum7"]) + Convert.ToDecimal(listInfoData[i]["vcNum8"]) + Convert.ToDecimal(listInfoData[i]["vcNum9"]) + Convert.ToDecimal(listInfoData[i]["vcNum10"]);
                    if (strChange == "1")//新设
                    {//新增
                        sql.Append("      INSERT INTO  TSQJD       \n");
                        sql.Append("      (       \n");
                        sql.Append("        dSSDate,vcJD,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id,vcFXDiff,vcFXNo        \n");
                        sql.Append("        ,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcSYTCode,vcReceiver,dNqDate        \n");
                        sql.Append("      )       \n");
                        sql.Append("      VALUES      \n");
                        sql.Append("      (      \n");
                        sql.Append("        '" + listInfoData[i]["dSSDate"] + "',           \n");      //实施日期
                        sql.Append("        '1',           \n");                                            //进度-初始状态位已联络
                        sql.Append("        '" + listInfoData[i]["vcPart_id"] + "',           \n");         //品番
                        sql.Append("        '" + listInfoData[i]["vcSPINo"] + "',           \n");           //设变号
                        sql.Append("        '" + listInfoData[i]["vcChange"] + "',           \n");          //变更事项
                        sql.Append("        '" + listInfoData[i]["vcCarTypeDesign "] + "',           \n");  //车种-原单位的车型开发
                        sql.Append("        '" + listInfoData[i]["vcInOutflag"] + "',           \n");       //内外区分
                        sql.Append("        '" + listInfoData[i]["vcPartName"] + "',           \n");        //品名
                        sql.Append("        '" + listInfoData[i]["vcOE"] + "',           \n");              //OE=SP
                        sql.Append("        '" + listInfoData[i]["vcSupplier_id"] + "',           \n");     //供应商代码
                        sql.Append("        '" + listInfoData[i]["vcFXDiff"] + "',           \n");          //防锈指示
                        sql.Append("        '" + listInfoData[i]["vcFXNo"] + "',           \n");            //防锈指示书
                        sql.Append("        '"+sumLater+"',           \n");            //旧型今后必要数
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
                        sql.Append("        '" + listInfoData[i]["dNqDate"] + "',           \n");           //纳期
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
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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
        public string Name2Value(string codeId, string strNameOrValue)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("     select vcValue from TCode     \n");
                strSql.Append("     where vcCodeid = '" + codeId + "'    \n");
                strSql.Append("     and vcName = '" + strNameOrValue + "'    \n");
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
    }
}
