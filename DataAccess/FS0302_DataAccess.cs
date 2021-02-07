using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class FS0302_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索


        public DataTable SearchApi(string fileNameTJ)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT a.iAutoId,'0' AS selected,a.vcSPINo,a.vcPart_Id_old,a.vcPart_Id_new,b.vcName as FinishState,e.vcName AS vcUnit,a.vcCarType,ISNULL(f.num,'0') AS vcNum, \r\n");
                //sbr.Append(" SELECT a.iAutoId,'0' AS selected,a.vcSPINo,a.vcPart_Id_old,a.vcPart_Id_new,b.vcName as FinishState,e.vcName AS vcUnit,f.vcDiff,a.vcCarType, \r\n");
                sbr.Append(" d.vcName AS THChange,c.vcName AS vcDD,a.vcRemark,a.vcChange,a.vcBJDiff, \r\n");
                sbr.Append(" CASE WHEN (ISNULL(a.vcDTDiff,'') = '' and ISNULL(a.vcPart_id_DT,'')= '') THEN ''  \r\n");
                sbr.Append(" WHEN (ISNULL(a.vcDTDiff,'') <> '' AND  ISNULL(a.vcPart_id_DT,'') <> '') THEN a.vcDTDiff+'/'+a.vcPart_id_DT  \r\n");
                sbr.Append(" WHEN ISNULL(a.vcDTDiff,'') <> '' THEN a.vcDTDiff WHEN ISNULL(a.vcPart_id_DT,'') <> '' THEN a.vcPart_id_DT END AS vcDT, \r\n");
                sbr.Append(" a.vcPartName,a.vcStartYearMonth,a.vcFXDiff,a.vcFXNo,a.vcOldProj,a.dOldProjTime,a.vcNewProj, \r\n");
                sbr.Append(" a.dNewProjTime,a.vcCZYD,a.dHandleTime,a.vcSheetName,a.vcFileName,'0' as vcModFlag,'0' as vcAddFlag,a.vcType  \r\n");
                sbr.Append(" FROM \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT iAutoId,vcSPINo,vcPart_Id_old,vcPart_Id_new,vcFinishState,vcOriginCompany,vcDiff,vcCarType,vcTHChange, \r\n");
                sbr.Append(" vcRemark,vcChange,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff, \r\n");
                sbr.Append(" vcFXNo,vcOldProj,dOldProjTime,vcNewProj,dNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName,vcType \r\n");
                sbr.Append(" FROM TSBManager WHERE vcFileNameTJ = '" + fileNameTJ + "' \r\n");
                sbr.Append(" ) a \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
                sbr.Append(" ) b ON a.vcFinishState = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                //sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C009' \r\n");
                sbr.Append("   SELECT vcValue1 as vcValue,vcValue2 as vcName FROM TOutCode WHERE vcCodeId = 'C009' AND vcIsColum = '0' \r\n");
                sbr.Append(" ) c ON a.vcCarType = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C002' \r\n");
                sbr.Append(" ) d ON a.vcTHChange = d.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C006' \r\n");
                sbr.Append(" ) e ON a.vcOriginCompany = e.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcPart_id,COUNT(*) AS num FROM TUnit WHERE dTimeFrom<=GETDATE() AND dTimeTo >= GETDATE() AND ISNULL(vcPart_id,'') <> '' GROUP BY vcPart_id \r\n");
                sbr.Append(" ) f ON a.vcPart_Id_old = f.vcPart_id OR a.vcPart_Id_new = f.vcPart_id \r\n");
                //sbr.Append(" LEFT JOIN  \r\n");
                //sbr.Append(" (SELECT vcPart_id,MAX(vcDiff) AS vcDiff FROM TUnit WHERE dTimeFrom<=GETDATE() AND dTimeTo >= GETDATE() GROUP BY vcPart_id) f \r\n");
                //sbr.Append(" ON (a.vcPart_Id_old = f.vcPart_id AND ISNULL(a.vcPart_Id_old,'')<> '') OR (a.vcPart_Id_new = f.vcPart_id AND ISNULL(a.vcPart_Id_new,'')<> '' ) \r\n");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");

                dt.Columns.Add("vcDiff");
                DataTable Diff = getDiff();
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    string vcPart_Id_old = ObjToString(dt.Rows[i]["vcPart_Id_old"]);
                //    string vcPart_Id_new = ObjToString(dt.Rows[i]["vcPart_Id_new"]);
                //    if (!string.IsNullOrWhiteSpace(vcPart_Id_old))
                //    {
                //        DataRow[] tmp = dt.Select("vcPart_Id_old = '" + vcPart_Id_old + "'");
                //        if (tmp.Length > 0)
                //        {
                //            dt.Rows[i]["vcDiff"] = tmp[0]["vcDiff"];
                //        }
                //        else
                //        {
                //            dt.Rows[i]["vcDiff"] = "";
                //        }
                //    }
                //    else if (!string.IsNullOrWhiteSpace(vcPart_Id_new))
                //    {
                //        DataRow[] tmp = dt.Select("vcPart_Id_new = '" + vcPart_Id_new + "'");
                //        if (tmp.Length > 0)
                //        {
                //            dt.Rows[i]["vcDiff"] = tmp[0]["vcDiff"];
                //        }
                //        else
                //        {
                //            dt.Rows[i]["vcDiff"] = "";
                //        }
                //    }
                //    else
                //    {
                //        dt.Rows[i]["vcDiff"] = "";
                //    }
                //}
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPart_Id_old = ObjToString(dt.Rows[i]["vcPart_Id_old"]);
                    string vcPart_Id_new = ObjToString(dt.Rows[i]["vcPart_Id_new"]);
                    if (!string.IsNullOrWhiteSpace(vcPart_Id_old))
                    {
                        DataRow[] tmp = Diff.Select("vcPart_id = '" + vcPart_Id_old + "'");
                        if (tmp.Length > 0)
                        {
                            dt.Rows[i]["vcDiff"] = tmp[0]["vcDiff"];
                        }
                        else
                        {
                            dt.Rows[i]["vcDiff"] = "";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(vcPart_Id_new))
                    {
                        DataRow[] tmp = Diff.Select("vcPart_id = '" + vcPart_Id_new + "'");
                        if (tmp.Length > 0)
                        {
                            dt.Rows[i]["vcDiff"] = tmp[0]["vcDiff"];
                        }
                        else
                        {
                            dt.Rows[i]["vcDiff"] = "";
                        }
                    }
                    else
                    {
                        dt.Rows[i]["vcDiff"] = "";
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getDiff()
        {
            try
            {
                StringBuilder sbrDiff = new StringBuilder();
                sbrDiff.AppendLine("SELECT vcPart_id,vcDiff FROM TUnit WHERE dTimeFrom<=GETDATE() AND dTimeTo >= GETDATE()");
                sbrDiff.AppendLine("ORDER BY vcPart_id,");
                sbrDiff.AppendLine("CASE vcDiff ");
                sbrDiff.AppendLine("	WHEN '1' then 1");
                sbrDiff.AppendLine("	when '2' then 2");
                sbrDiff.AppendLine("	when '9' then 3 	");
                sbrDiff.AppendLine("	WHEN '4' then 3 ");
                sbrDiff.AppendLine("end");
                return excute.ExcuteSqlWithSelectToDT(sbrDiff.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 织入原单位

        public void weaveUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, string SYTCode, ref string refMsg)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                //可选择的变更事项
                List<string> changeList = new List<string>() { "1", "2", "3", "4", "5", "6", "8", "10", "11", "12", "13", "16", "21" };
                //品番check
                List<string> partCheck = getPart();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string change = getValue("C002", ObjToString(listInfoData[i]["THChange"]).Trim());
                    string vcPart_Id_new = ObjToString(listInfoData[i]["vcPart_Id_new"]).Trim();
                    string vcPart_Id_old = ObjToString(listInfoData[i]["vcPart_Id_old"]).Trim();
                    if (!changeList.Contains(change))
                    {
                        string partId = string.IsNullOrWhiteSpace(vcPart_Id_new) ? vcPart_Id_old : vcPart_Id_new;
                        refMsg += "品番" + partId + "变更事项选择有误;";
                    }


                    if (change != "1" && change != "2" && change != "10" && change != "12" && change != "8")
                    {
                        if (!string.IsNullOrWhiteSpace(vcPart_Id_new))
                        {
                            if (!partCheck.Contains(vcPart_Id_new))
                            {
                                refMsg += "品番" + vcPart_Id_new + "在原单位中不存在;";
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(vcPart_Id_old))
                        {
                            if (!partCheck.Contains(vcPart_Id_old))
                            {
                                refMsg += "品番" + vcPart_Id_old + "在原单位中不存在;";
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(refMsg))
                {
                    return;
                }

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    string finishstate = getValue("C014", ObjToString(listInfoData[i]["FinishState"]).Trim());
                    string change = getValue("C002", ObjToString(listInfoData[i]["THChange"]).Trim());

                    if (!finishstate.Equals("1") && !finishstate.Equals("3"))
                    {
                        if (change.Equals("1") || change.Equals("2") || change.Equals("10") || change.Equals("12") || change.Equals("8"))//新设
                        {
                            string CarType = ObjToString(listInfoData[i]["vcCarType"]).Trim();
                            string vcPart_Id = ObjToString(listInfoData[i]["vcPart_Id_new"]).Trim();
                            string vcType = ObjToString(listInfoData[i]["vcType"]).Trim();
                            string vcNewProj = ObjToString(listInfoData[i]["vcNewProj"]).Trim();
                            string vcStartYearMonth = ObjToString(listInfoData[i]["vcStartYearMonth"]).Trim();
                            string vcSYTCode = getSYTCode(SYTCode);
                            string vcPartNameEn = ObjToString(listInfoData[i]["vcPartName"]);
                            string vcSPINo = ObjToString(listInfoData[i]["vcSPINo"]);

                            if (!string.IsNullOrWhiteSpace(vcStartYearMonth))
                            {
                                vcStartYearMonth = vcStartYearMonth.Substring(0, 4) + "/" + vcStartYearMonth.Substring(4, 2) + "/01";
                            }

                            string partId = vcPart_Id;
                            string NRPartId = "";
                            if (vcType.Equals("1"))
                            {
                                NRPartId = getPartId(CarType, vcPart_Id, vcNewProj);
                            }

                            sbr.Append(" INSERT INTO TUnit  \r\n");
                            sbr.Append(" (vcPart_id,vcChange,dTimeFrom,dTimeTo,vcMeno,vcHaoJiu,vcDiff,vcCarTypeDev,vcOriginCompany,vcOperator,dOperatorTime,vcSYTCode,vcPartNameEn,vcSPINo,vcHKPart_id,vcSQState) values\r\n");
                            sbr.Append(" (" + ComFunction.getSqlValue(partId, false) + ",'" + change + "'," + ComFunction.getSqlValue(vcStartYearMonth, true) + ",CONVERT(DATE,'99991231')," + ComFunction.getSqlValue(listInfoData[i]["THChange"], false) + ",'H','2'," + ComFunction.getSqlValue(CarType, false) + ",'" + getValue("C006", listInfoData[i]["vcUnit"].ToString()) + "','" + strUserId + "', GETDATE(),'" + vcSYTCode + "','" + vcPartNameEn + "','" + vcSPINo + "','" + NRPartId + "','0')  \r\n");

                            sbr.Append(" UPDATE TSBManager \r\n");
                            sbr.Append(" SET vcFinishState = '3', \r\n");
                            sbr.Append("     vcOperatorId = '" + strUserId + "', \r\n");
                            sbr.Append("     dOperatorTime = GETDATE() \r\n");
                            sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");

                            //添加到新品表
                            sbr.Append("INSERT INTO TPartNameCN(vcPart_id,vcPartNameEn,vcOperator,dOperatorTime,vcIsLock)");
                            sbr.Append("VALUES");
                            sbr.Append("(");
                            sbr.Append(ComFunction.getSqlValue(vcPart_Id, false) + ",");
                            sbr.Append(ComFunction.getSqlValue(vcPartNameEn, false) + ",");
                            sbr.Append("'" + strUserId + "'");
                            sbr.Append(",GETDATE(),'0'");
                            sbr.Append(") \r\n");

                        }
                        else if (change.Equals("4") || change.Equals("11") || change.Equals("13") || change.Equals("21"))//废止
                        {
                            sbr.Append(" UPDATE a SET \r\n");
                            sbr.Append(" a.vcChange = '" + change + "', \r\n");
                            sbr.Append(" a.dSyncTime = NULL, \r\n");
                            //不更新使用结束时间
                            //sbr.Append(" a.dTimeTo = b.vcStartYearMonth, \r\n");
                            sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                            sbr.Append(" a.vcMeno = isnull(vcMeno,'')+'废止;', \r\n");
                            sbr.Append(" a.vcSQState = '0', \r\n");
                            sbr.Append(" a.vcDiff = '4', \r\n");
                            //Add,TODO 工程结束时间
                            sbr.Append(" a.vcBJDiff = b.vcBJDiff, \r\n");
                            sbr.Append(" a.vcPartReplace = b.vcPart_id_DT, \r\n");
                            sbr.Append(" a.dGYSTimeTo = b.vcStartYearMonth, ");
                            //
                            sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                            sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                            sbr.Append(" FROM TUnit a \r\n");
                            sbr.Append(" LEFT JOIN(SELECT iAutoId, vcPart_Id_old AS vcPart_Id, CONVERT(DATE, vcStartYearMonth + '01') AS vcStartYearMonth, vcSPINo,vcBJDiff,vcPart_id_DT FROM TSBManager) b ON a.vcPart_id = b.vcPart_Id \r\n");
                            sbr.Append(" WHERE b.iAutoId = " + iAutoId + " \r\n");
                            sbr.Append(" UPDATE TSBManager \r\n");
                            sbr.Append(" SET vcFinishState = '3', \r\n");
                            sbr.Append(" vcOperatorId = '" + strUserId + "', \r\n");
                            sbr.Append(" dOperatorTime = GETDATE() \r\n");
                            sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");

                        }
                        else if (change.Equals("5") || change.Equals("3"))//旧型
                        {
                            sbr.Append(" UPDATE a SET \r\n");
                            sbr.Append(" a.vcChange = '" + change + "', \r\n");
                            sbr.Append(" a.dSyncTime = NULL, \r\n");
                            sbr.Append(" a.vcHaoJiu = 'Q', \r\n");
                            sbr.Append(" a.dJiuBegin = b.vcStartYearMonth,  \r\n");
                            sbr.Append(" a.vcMeno = isnull(vcMeno,'')+'旧型;' , \r\n");
                            sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                            sbr.Append(" a.vcDiff = '9', \r\n");
                            sbr.Append(" a.vcSQState = '0', \r\n");
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


                            sbr.Append("INSERT INTO TJiuTenYear(vcPart_id,vcChange,vcCarTypeDesign,dJiuBegin,vcOperator,dOperatorTime,vcIsLock)");
                            sbr.Append("VALUES");
                            sbr.Append("(");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + ",");
                            sbr.Append("'3',");
                            sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + ",");
                            //sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + ", ");
                            sbr.Append("CONVERT(DATE, '" + listInfoData[i]["vcStartYearMonth"].ToString() + "' + '01'), ");
                            sbr.Append("'" + strUserId + "',");
                            sbr.Append("GETDATE(),'0') \r\n");

                        }
                        else if (change.Equals("6"))//旧型恢复现号
                        {
                            sbr.Append(" UPDATE a SET \r\n");
                            sbr.Append(" a.vcChange = '4', \r\n");
                            sbr.Append(" a.dSyncTime = NULL, \r\n");
                            sbr.Append(" a.vcHaoJiu = 'H', \r\n");
                            sbr.Append(" a.dJiuEnd = b.vcStartYearMonth, \r\n");
                            sbr.Append(" a.vcMeno = isnull(vcMeno,'')+'旧型恢复现号;' , \r\n");
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
                        else if (change.Equals("16"))//复活
                        {
                            sbr.Append(" UPDATE a SET \r\n");
                            sbr.Append(" a.vcChange = '5', \r\n");
                            sbr.Append(" a.dSyncTime = NULL, \r\n");
                            sbr.Append(" a.vcSQState = '0', \r\n");
                            sbr.Append(" a.dTimeFrom = b.vcStartYearMonth, \r\n");
                            sbr.Append(" a.dTimeTo = CONVERT(DATE,'99991231'), \r\n");
                            sbr.Append(" a.vcHaoJiu = 'H', \r\n");
                            sbr.Append(" a.vcMeno = isnull(vcMeno,'')+'复活;' , \r\n");
                            sbr.Append(" a.vcSPINo = b.vcSPINo, \r\n");
                            sbr.Append(" a.vcDiff = '2', \r\n");
                            sbr.Append(" a.vcCarTypeDesign = b.vcCarType, \r\n");
                            sbr.Append(" a.vcBJDiff = b.vcBJDiff, \r\n");
                            sbr.Append(" a.vcPartReplace = b.vcPart_id_DT, \r\n");
                            sbr.Append(" a.vcFXDiff = b.vcFXDiff, \r\n");
                            sbr.Append(" a.vcFXNo = b.vcFXNo, \r\n");
                            //ADD,TODO 工程结束时间
                            sbr.Append(" a.vcPartNameEn = b.vcPartName, \r\n");
                            sbr.Append(" a.dGYSTimeTo = CONVERT(DATE,'99991231'), ");
                            //
                            sbr.Append(" a.vcOperator = '" + strUserId + "', \r\n");
                            sbr.Append(" a.dOperatorTime = GETDATE() \r\n");
                            sbr.Append(" FROM TUnit a \r\n");
                            sbr.Append(" LEFT JOIN (SELECT iAutoId,vcCarType,vcBJDiff,vcPart_id_DT,vcFXDiff,vcFXNo,vcPart_Id_old AS vcPart_Id,CONVERT(DATE,vcStartYearMonth+'01') AS vcStartYearMonth,vcSPINo,vcPartName FROM TSBManager) b \r\n");
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

                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
                }

                if (listInfoData.Count > 0)
                {
                    int id = Convert.ToInt32(listInfoData[0]["iAutoId"]);
                    changeFileState(id, strUserId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取纳入品番
        public string getPartId(string vcCarType, string vcPart_Id, string vcParent)
        {
            try
            {
                string partId = "";
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT iLV,vcUseLocation,vcPart_Id,vcPart_Id_Father,vcParent  FROM TPartList  \r\n");
                sbr.Append(" WHERE vcCarType = '" + vcCarType + "' \r\n");
                sbr.Append(" AND vcUseLocation IN (SELECT DISTINCT vcUseLocation FROM TPartList WHERE vcPart_Id = '" + vcPart_Id + "' ) \r\n");
                sbr.Append(" ORDER BY vcUseLocation,iAutoId \r\n");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
                List<FatherNode> list = new List<FatherNode>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string UseLocation = dt.Rows[i]["vcUseLocation"].ToString();
                    string Part_Id = dt.Rows[i]["vcPart_Id"].ToString();
                    string Part_Id_Father = dt.Rows[i]["vcPart_Id_Father"].ToString();
                    string Parent = dt.Rows[i]["vcParent"].ToString();
                    int iLV = Convert.ToInt32(dt.Rows[i]["iLV"]);
                    ParentEntity entity = new ParentEntity(Part_Id, Parent, iLV);
                    Node node = new Node(entity);
                    int hasExist = -1;

                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].UseLocation.Equals(UseLocation))
                        {
                            hasExist = j;
                            break;
                        }
                    }
                    //没找到创建一个父节点
                    if (hasExist == -1)
                    {
                        list.Add(new FatherNode(UseLocation, node));
                        hasExist = list.Count;
                    }
                    //找到创建一个子节点
                    else
                    {
                        if (iLV == 1)
                        {
                            list[hasExist].childNodes.Add(node);
                        }
                        else
                        {
                            BLSearch(iLV - 1, ref list[hasExist].childNodes, Part_Id_Father, entity);
                        }
                    }


                }

                List<List<ParentEntity>> listPath = new List<List<ParentEntity>>();
                for (int i = 0; i < list.Count; i++)
                {
                    List<Node> nodes = list[i].childNodes;
                    foreach (Node node in nodes)
                    {
                        //获取路径
                        listPath.AddRange(getList(node));
                    }
                }

                for (int i = 0; i < listPath.Count; i++)
                {
                    int index = getIndex(vcPart_Id, listPath[i]);
                    if (index != -1)
                    {
                        for (int j = index; j >= 0; j--)
                        {
                            if (vcParent.Equals(listPath[i][j].Parent))
                            {
                                partId = listPath[i][j].PartId;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(partId))
                        {
                            break;
                        }
                    }
                }

                return partId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public class ParentEntity
        {
            public ParentEntity(string PartId, string Parent, int LV)
            {
                this.Parent = Parent;
                this.PartId = PartId;
                this.LV = LV;
            }
            public string PartId;
            public string Parent;
            public int LV;
        }
        public class FatherNode
        {
            public FatherNode(string useLocation, Node node)
            {
                this.UseLocation = useLocation;
                this.childNodes = new List<Node>();
                this.childNodes.Add(node);
            }
            public string UseLocation;
            public List<Node> childNodes;
        }
        public class Node
        {
            public Node(ParentEntity entity)
            {
                childNodes = new List<Node>();
                Entity = entity;

            }
            public List<Node> childNodes;
            public ParentEntity Entity;

        }
        //嵌套添加
        public void BLSearch(int times, ref List<Node> nodes, string parentId, ParentEntity entity)
        {
            List<Node> res = new List<Node>();
            while (times > 0)
            {
                times--;
                for (int i = 0; i < nodes.Count; i++)
                {
                    res.AddRange(nodes[i].childNodes);
                }
                BLSearch(times, ref res, parentId, entity);
            }

            res = nodes.Distinct().ToList();


            for (int i = 0; i < res.Count; i++)
            {
                if (res[i].Entity.PartId.Equals(parentId) && res[i].Entity.LV == entity.LV - 1)
                {
                    Node node = new Node(entity);
                    if (!isExist(res[i].childNodes, node))
                        res[i].childNodes.Add(node);
                    break;
                }
            }
        }
        //判断节点是否已存在
        public bool isExist(List<Node> nodes, Node node)
        {
            bool Exist = false;
            foreach (Node temp in nodes)
            {
                if (temp.Entity == node.Entity)
                {
                    Exist = true;
                    break;
                }
            }
            return Exist;
        }
        //遍历形成list
        public List<List<ParentEntity>> getList(Node node)
        {
            List<List<ParentEntity>> list = new List<List<ParentEntity>>();
            List<ParentEntity> res = new List<ParentEntity>();
            //定义栈
            Stack<ParentEntity> stack = new Stack<ParentEntity>();

            res.Add(node.Entity);
            stack.Push(node.Entity);
            if (node.childNodes.Count > 0)
            {
                //LV2
                List<Node> temp2 = node.childNodes;
                for (int a = 0; a < temp2.Count; a++)
                {
                    Node node2 = temp2[a];
                    List<ParentEntity> res2 = new List<ParentEntity>();
                    //res2 = res;
                    res2.Add(node2.Entity);
                    stack.Push(node2.Entity);
                    //有LV3
                    if (node2.childNodes.Count > 0)
                    {
                        List<Node> temp3 = node2.childNodes;
                        for (int b = 0; b < temp3.Count; b++)
                        {
                            Node node3 = temp3[b];
                            List<ParentEntity> res3 = new List<ParentEntity>();
                            //res3 = res2;
                            res3.Add(node3.Entity);
                            stack.Push(node3.Entity);
                            //有LV4
                            if (node3.childNodes.Count > 0)
                            {
                                List<Node> temp4 = node3.childNodes;
                                for (int c = 0; c < temp4.Count; c++)
                                {
                                    Node node4 = temp4[c];
                                    List<ParentEntity> res4 = new List<ParentEntity>();
                                    //res4 = res3;
                                    res4.Add(node4.Entity);
                                    stack.Push(node4.Entity);
                                    //有LV5
                                    if (node4.childNodes.Count > 0)
                                    {
                                        List<Node> temp5 = node4.childNodes;
                                        for (int d = 0; d < temp5.Count; d++)
                                        {
                                            Node node5 = temp5[d];
                                            List<ParentEntity> res5 = res4;
                                            res5.Add(node5.Entity);
                                            stack.Push(node5.Entity);
                                            //list.Add(res5);
                                            List<ParentEntity> stack1 = new List<ParentEntity>();

                                            foreach (var item in stack)
                                            {
                                                stack1.Add(item);
                                            }
                                            //正序输出
                                            stack1.Reverse();
                                            list.Add(stack1);
                                            stack.Pop();
                                        }
                                        //返回到上一节点
                                        stack.Pop();
                                    }
                                    else
                                    {
                                        List<ParentEntity> stack1 = new List<ParentEntity>();
                                        foreach (var item in stack)
                                        {
                                            stack1.Add(item);
                                        }
                                        //正序输出
                                        stack1.Reverse();
                                        list.Add(stack1);
                                        stack.Pop();
                                        //list.Add(res4);
                                    }
                                }
                                //返回到上一节点
                                stack.Pop();
                            }
                            else
                            {
                                List<ParentEntity> stack1 = new List<ParentEntity>();

                                foreach (var item in stack)
                                {
                                    stack1.Add(item);
                                }
                                //正序输出
                                stack1.Reverse();
                                list.Add(stack1);
                                stack.Pop();
                            }
                        }
                        //返回到上一节点
                        stack.Pop();
                    }
                    else
                    {
                        //list.Add(res2);
                        List<ParentEntity> stack1 = new List<ParentEntity>();
                        foreach (var item in stack)
                        {
                            stack1.Add(item);
                        }
                        //正序输出
                        stack1.Reverse();
                        list.Add(stack1);
                        stack.Pop();
                        //list.Add(stack.Peek());
                    }
                }
                //返回到上一节点
                stack.Pop();
            }
            else
            {
                List<ParentEntity> stack1 = new List<ParentEntity>();
                foreach (var item in stack)
                {
                    stack1.Add(item);
                }
                //正序输出
                stack1.Reverse();
                //只有LV1
                list.Add(stack1);
            }

            return list;
        }


        //获取路径包含品番的
        public int getIndex(string partId, List<ParentEntity> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].PartId.Equals(partId))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"]; //true可编辑,false不可编辑

                    if (bModFlag == true)
                    {
                        //修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        string finishState = getValue("C014", ObjToString(listInfoData[i]["FinishState"]));
                        sbr.AppendLine(" UPDATE TSBManager SET ");
                        if (!finishState.Equals("3"))
                        {
                            sbr.AppendLine(" vcFinishState = " + ComFunction.getSqlValue(finishState, false) + ",");
                        }
                        sbr.AppendLine(" vcOriginCompany = " +
                                       ComFunction.getSqlValue(getValue("C006", listInfoData[i]["vcUnit"].ToString()),
                                           false) + ",");
                        sbr.AppendLine(" vcDiff = " +
                                       ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + ",");
                        sbr.AppendLine(" vcCarType = " +
                                       ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + ",");
                        sbr.AppendLine(" vcTHChange = " +
                                       ComFunction.getSqlValue(getValue("C002", listInfoData[i]["THChange"].ToString()),
                                           false) + ",");
                        sbr.AppendLine(" vcRemark = " +
                                       ComFunction.getSqlValue(listInfoData[i]["vcRemark"], false) + ",");
                        sbr.AppendLine(" vcOperatorId = '" + strUserId + "',");
                        sbr.AppendLine(" dOperatorTime = GETDATE()");
                        sbr.AppendLine(" WHERE iAutoId = " + iAutoId + "");
                        //TODO 已织入时不能修改
                        sbr.AppendLine(" AND vcFinishState <> '3' ");

                    }


                }
                excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
                if (listInfoData.Count > 0)
                {
                    int id = Convert.ToInt32(listInfoData[0]["iAutoId"]);
                    changeFileState(id, strUserId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改开包状态

        public void changeFileState(int id, string strUserId)
        {

            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine(" SELECT distinct vcFinishState FROM ");
            sbr.AppendLine(" TSBManager");
            sbr.AppendLine(" WHERE vcFileNameTJ = ");
            sbr.AppendLine(" (SELECT vcFileNameTJ FROM TSBManager");
            sbr.AppendLine(" WHERE iAutoId = " + id);
            sbr.AppendLine(" )");

            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            List<string> list = new List<string>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                list.Add(dt.Rows[j][0].ToString().Trim());
            }
            sbr.Length = 0;

            if (!list.Contains("0") && !list.Contains("2") && !list.Contains(""))
            {
                sbr.AppendLine("UPDATE TSBFile");
                sbr.AppendLine("SET vcState = '2',");
                sbr.AppendLine("vcOperatorId = '" + strUserId + "',");
                sbr.AppendLine("dOperatorTime = GETDATE()");
                sbr.AppendLine("WHERE vcFileNameTJ = ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcFileNameTJ FROM TSBManager");
                sbr.AppendLine("WHERE iAutoId = " + id + "");
                sbr.AppendLine(")");
            }
            else
            {
                sbr.AppendLine("UPDATE TSBFile");
                sbr.AppendLine("SET vcState = '1',");
                sbr.AppendLine("vcOperatorId = '" + strUserId + "',");
                sbr.AppendLine("dOperatorTime = GETDATE()");
                sbr.AppendLine("WHERE vcFileNameTJ = ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcFileNameTJ FROM TSBManager");
                sbr.AppendLine("WHERE iAutoId = " + id + "");
                sbr.AppendLine(")");
            }
            excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
        }


        #endregion

        #region 共同方法
        public string getValue(string strCodeId, string vcName)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcName,vcValue from TCode where vcCodeId='" + strCodeId + "'     \n");
                dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcName"].ToString().Equals(vcName))
                    {
                        return dt.Rows[i]["vcValue"].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ObjToString(Object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion

        #region 获取事业体编号

        public string getSYTCode(string SYTCode)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C016'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcName"].ToString().Equals(SYTCode))
                    {
                        return dt.Rows[i]["vcValue"].ToString();
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取原单位品番

        public List<string> getPart()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT distinct vcPart_id FROM TUnit WHERE dTimeFrom<=GETDATE() AND dTimeTo >= GETDATE()");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
                List<string> res = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    res.Add(dt.Rows[i]["vcPart_id"].ToString());
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}