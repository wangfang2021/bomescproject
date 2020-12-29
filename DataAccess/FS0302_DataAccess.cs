using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0302_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 页面初始化

        public DataTable getFinishState()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }
        public DataTable getChange()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcName FROM TCode WHERE vcCodeId = 'C015' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }
        public DataTable SearchApi(string fileNameTJ)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT a.iAutoId,'' AS selected,a.vcSPINo,a.vcPart_Id_old,a.vcPart_Id_new,b.vcName as FinishState,e.vcName AS vcUnit,a.vcDiff,a.vcCarType, \r\n");
                sbr.Append(" d.vcName AS THChange,c.vcName AS vcDD,a.vcRemark,a.vcChange,a.vcBJDiff, \r\n");
                sbr.Append(" CASE WHEN (ISNULL(a.vcDTDiff,'') = '' and ISNULL(a.vcPart_id_DT,'')= '') THEN ''  \r\n");
                sbr.Append(" WHEN (ISNULL(a.vcDTDiff,'') <> '' AND  ISNULL(a.vcPart_id_DT,'') <> '') THEN a.vcDTDiff+'/'+a.vcPart_id_DT  \r\n");
                sbr.Append(" WHEN ISNULL(a.vcDTDiff,'') <> '' THEN a.vcDTDiff WHEN ISNULL(a.vcPart_id_DT,'') <> '' THEN a.vcPart_id_DT END AS vcDT, \r\n");
                sbr.Append(" a.vcPartName,a.vcStartYearMonth,a.vcFXDiff,a.vcFXNo,a.dOldProjTime,a.dOldProjTime,a.vcNewProj, \r\n");
                sbr.Append(" a.dNewProjTime,a.vcCZYD,a.dHandleTime,a.vcSheetName,a.vcFileName  \r\n");
                sbr.Append(" FROM \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT iAutoId,vcSPINo,vcPart_Id_old,vcPart_Id_new,vcFinishState,vcOriginCompany,vcDiff,vcCarType,vcTHChange, \r\n");
                sbr.Append(" vcRemark,vcChange,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff, \r\n");
                sbr.Append(" vcFXNo,vcOldProj,dOldProjTime,vcNewProj,dNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName \r\n");
                sbr.Append(" FROM TSBManager WHERE vcFileNameTJ = '" + fileNameTJ + "' \r\n");
                sbr.Append(" ) a \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
                sbr.Append(" ) b ON a.vcFinishState = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C009' \r\n");
                sbr.Append(" ) c ON a.vcCarType = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C015' \r\n");
                sbr.Append(" ) d ON a.vcTHChange = d.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C006' \r\n");
                sbr.Append(" ) e ON a.vcOriginCompany = e.vcValue \r\n");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        //织入原单位
        public void weaveUnit(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    string change = listInfoData[i]["vcTHChange"].ToString().Trim();
                    if (change.Equals("1"))//新设/新车新设
                    {
                        //TODO
                        /*
                    INSERT INTO TUnit 
                    (vcPart_id,vcChange,dTimeFrom,dTimeTo,vcMeno,vcHaoJiu,vcDiff,vcOperator,dOperatorTime)
                    (SELECT '' AS vcPart_id,'1' AS vcChange,CONVERT(DATE,vcStartYearMonth+'01') AS dTimeFrom,CONVERT(DATE,'99991231') AS dTimeTo,'新设/新车新设' AS vcMeno,'H' AS vcHaojiu,'2' AS vcDiff,'000000' AS vcOperator,GETDATE() AS dOperatorTime FROM TSBManager WHERE iAutoId = 1)
                     */
                    }
                    else if (change.Equals("2"))//废止
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '2', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.dJiuEnd = b.vcStartYearMonth, \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'废止;', \r\n");
                        sbr.Append(" a.vcSQState = '0', \r\n");
                        sbr.Append(" a.vcDiff = '4', \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append(" LEFT JOIN(SELECT iAutoId, vcPart_Id_old AS vcPart_Id, CONVERT(DATE, vcStartYearMonth + '01') AS vcStartYearMonth, vcSPINo FROM TSBManager) b ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append(" dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");

                    }
                    else if (change.Equals("3"))//旧型
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '3', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.vcHaoJiu = 'Q', \r\n");
                        sbr.Append(" a.dJiuBegin = b.vcStartYearMonth,  \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'旧型;' , \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcDiff = '9', \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append(" LEFT JOIN (SELECT iAutoId,vcPart_Id_old AS vcPart_Id,CONVERT(DATE,vcStartYearMonth+'01') AS vcStartYearMonth,vcSPINo FROM TSBManager) b \r\n");
                        sbr.Append(" ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");

                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append(" dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");

                    }
                    else if (change.Equals("4"))//旧型恢复现号
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '4', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.vcHaoJiu = 'H', \r\n");
                        sbr.Append(" a.dJiuEnd = b.vcStartYearMonth, \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'旧型恢复现号;' , \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcDiff = '1', \r\n");
                        sbr.Append(" a.vcCarTypeDesign = b.vcCarType, \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append("     LEFT JOIN(SELECT iAutoId, vcCarType, vcPart_Id_old AS vcPart_Id, CONVERT(DATE, vcStartYearMonth + '01') AS vcStartYearMonth, vcSPINo FROM TSBManager) b \r\n");
                        sbr.Append("     ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                        sbr.Append("  \r\n");
                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append("     vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append("     dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");


                    }
                    else if (change.Equals("5"))//复活
                    {
                        sbr.Append(" UPDATE a SET \r\n");
                        sbr.Append(" a.vcChange = '5', \r\n");
                        sbr.Append(" a.dSyncTime = NULL, \r\n");
                        sbr.Append(" a.vcSQState = '0', \r\n");
                        sbr.Append(" a.dTimeFrom = b.vcStartYearMonth, \r\n");
                        sbr.Append(" a.dTimeTo = CONVERT(DATE,'99991231'), \r\n");
                        sbr.Append(" a.vcHaoJiu = 'H', \r\n");
                        sbr.Append(" a.vcMeno = vcMeno+'复活;' , \r\n");
                        sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                        sbr.Append(" a.vcDiff = '2', \r\n");
                        sbr.Append(" a.vcCarTypeDesign = b.vcCarType, \r\n");
                        sbr.Append(" a.vcBJDiff = b.vcBJDiff, \r\n");
                        sbr.Append(" a.vcPartReplace = b.vcPart_id_DT, \r\n");
                        sbr.Append(" a.vcFXDiff = b.vcFXDiff, \r\n");
                        sbr.Append(" a.vcFXNo = b.vcFXNo, \r\n");
                        sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                        sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" FROM TUnit a \r\n");
                        sbr.Append(" LEFT JOIN (SELECT iAutoId,vcCarType,vcBJDiff,vcPart_id_DT,vcFXDiff,vcFXNo,vcPart_Id_old AS vcPart_Id,CONVERT(DATE,vcStartYearMonth+'01') AS vcStartYearMonth,vcSPINo FROM TSBManager) b \r\n");
                        sbr.Append(" ON a.vcPart_id = b.vcPart_Id \r\n");
                        sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                        sbr.Append("  \r\n");
                        sbr.Append(" UPDATE TSBManager \r\n");
                        sbr.Append(" SET vcFinishState = '3', \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                        sbr.Append(" dOperatorTime = GETDATE() \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");
                    }
                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}