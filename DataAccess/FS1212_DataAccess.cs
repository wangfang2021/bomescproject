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
    public class FS1212_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 插入数据事务
        /// <summary>
        /// 插入数据事务
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="useid">创建人</param>
        public void DoTransactionOfInsert(DataTable dt, string useid)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("UPDATE [tPartInfoMaster]");
            strSQL.AppendLine("   SET [vcCarFamilyCode] = @vcCarFamilyCode");
            strSQL.AppendLine("      ,[vcQFflag] = @vcQFflag");
            strSQL.AppendLine("      ,[vcQJcontainer] =@vcQJcontainer");
            strSQL.AppendLine("      ,[vcPorType] = @vcPorType");
            strSQL.AppendLine("      ,[vcZB] = @vcZB");
            strSQL.AppendLine("      ,[dUpdataTime] = getdate()");
            strSQL.AppendLine("      ,[vcUpdataUser] =@useid");
            strSQL.AppendLine("      ,[vcPartPlant] =@vcPartPlant");
            strSQL.AppendLine("      ,[vcPartFrequence] = @vcPartFrequence");
            strSQL.AppendLine(" WHERE  [vcPartsNo]=@vcPartsNo AND  [vcDock]=@vcDock");
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlParameter[] paras;
                    foreach (DataRow dr in dt.Rows)
                    {
                        paras = new SqlParameter[10];
                        paras[0] = new SqlParameter("@vcCarFamilyCode", dr["vcCarFamilyCode"].ToString());
                        paras[1] = new SqlParameter("@vcQFflag", dr["vcQFflag"].ToString());
                        paras[2] = new SqlParameter("@vcQJcontainer", dr["vcQJcontainer"].ToString());
                        paras[3] = new SqlParameter("@vcPorType", dr["vcPorType"].ToString());
                        paras[4] = new SqlParameter("@vcZB", dr["vcZB"].ToString());
                        paras[5] = new SqlParameter("@useid", useid);
                        paras[6] = new SqlParameter("@vcPartPlant", dr["vcPartPlant"].ToString());
                        paras[7] = new SqlParameter("@vcPartFrequence", dr["vcPartFrequence"].ToString());
                        paras[8] = new SqlParameter("@vcPartsNo", dr["vcPartsNo"].ToString());
                        paras[9] = new SqlParameter("@vcDock", dr["vcDock"].ToString());
                        excute.ExcuteSqlWithStringOper(strSQL.ToString(), paras);
                    }
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion

        #region 函数applendWhereIfNeed
        bool hasWhere = false;
        private bool applendWhereIfNeed(StringBuilder strSQL, bool haswhere)
        {
            if (haswhere == false)
            {
                strSQL.AppendLine(" WHERE ");
                return true;
            }
            else
            {
                strSQL.AppendLine(" AND ");
                return true;
            }
        }
        #endregion

        public DataTable SearchPartData(string vcPorType, string vcZB)
        {
            StringBuilder strSQL = new StringBuilder();
            //增加dTimeFrom、dTimeTo两个字段 - 刘刚
            //增加品番频度 - 李兴旺
            strSQL.AppendLine("select vcData1,vcData3 from ConstMst where vcDataId='ProType' and " +
                " vcData1='" + vcPorType + "' and vcData3='" + vcZB + "' order by vcData1,vcData3");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        //检索数据//需要修改SQL语句关联生产部署表和组别表20121221(世界末日)
        public DataTable SearchPartData(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            //增加dTimeFrom、dTimeTo两个字段 - 刘刚
            //增加品番频度 - 李兴旺
            strSQL.AppendLine(" select substring(vcPartsNo,0, 6) + '-' + substring(vcPartsNo,6, 5) + '-' + substring(vcPartsNo,11, 2)  as vcPartsNoName,vcPartsNo,dTimeFrom,dTimeTo,vcDock,vcCarFamilyCode,vcPartsNameEN,vcPartsNameCHN,vcQFflag as qinfengtz,iQuantityPerContainer,vcQJcontainer,vcPorType as shengchanbs,vcZB as zubie,vcPartPlant as leibie,'0' as iFlag,vcPartFrequence as pinfanpindu from tPartInfoMaster");
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPartsNo like '" + vcPartsNo + "%'");
            }
            if (!string.IsNullOrEmpty(vcCarFamilyCode))
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcCarFamilyCode like '" + vcCarFamilyCode + "'");
            }
            if (vcPorType != "")
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPorType like '" + vcPorType + "'");
            }
            if (vcZB != "")
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcZB like '" + vcZB + "'");
            }
            if (vcPartPlant != "")
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPartPlant like '" + vcPartPlant + "'");
            }
            if (vcPartFrequence != "全部")//增加品番频度 - 李兴旺
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPartFrequence = '" + vcPartFrequence + "'");
            }
            strSQL.AppendLine("  order by vcPartsNo");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        //导出数据
        public DataTable SearchPartDataEX(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            //20180908导出数据，添加起止时间和品番频度 - 李兴旺
            strSQL.AppendLine(" select  vcPartsNo,dTimeFrom,dTimeTo,vcDock,vcCarFamilyCode,vcPartsNameEN,vcPartsNameCHN,vcQFflag as qinfengtz,iQuantityPerContainer,vcQJcontainer,vcPorType as shengchanbs,vcZB as zubie,vcPartPlant as leibie,'' as iFlag,vcPartFrequence as pinfanpindu from tPartInfoMaster");
            //if (1!= 0)
            //{
            //    hasWhere = applendWhereIfNeed(strSQL, hasWhere);
            //    strSQL.AppendLine(" vcInOutFlag='0' ");
            //}
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPartsNo like '" + vcPartsNo + "%'");
            }
            if (!string.IsNullOrEmpty(vcCarFamilyCode))
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcCarFamilyCode like '" + vcCarFamilyCode + "'");
            }
            if (vcPorType != "")
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPorType like '" + vcPorType + "'");
            }
            if (vcZB != "")
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcZB like '" + vcZB + "'");
            }
            if (vcPartPlant != "")
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPartPlant like '" + vcPartPlant + "'");
            }
            if (vcPartFrequence != "全部")//增加品番频度 - 李兴旺
            {
                hasWhere = applendWhereIfNeed(strSQL, hasWhere);
                strSQL.AppendLine(" vcPartFrequence = '" + vcPartFrequence + "'");
            }
            strSQL.AppendLine("  order by vcPartsNo");
            return SearchPartData(strSQL.ToString());
        }

        public DataTable dllPorType()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("  select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
            return SearchPartData(strSQL.ToString());
        }

        public DataTable dllZB()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("  select distinct [vcData3] as [Text],[vcData3] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
            return SearchPartData(strSQL.ToString());
        }

        public DataTable dllLB()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("  select distinct [vcData1] as [Value],[vcData2] as [Text]  from [ConstMst] where [vcDataId]='KBPlant'");
            return SearchPartData(strSQL.ToString());
        }

        /// <summary>
        /// 检索信息栏绑定生产部署
        /// </summary>
        /// <param name="vcZB">组别</param>
        /// <returns></returns>
        public DataTable dllPorType(string vcZB)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
            if (vcZB == "")
            {
                strSQL.AppendLine("  union all select '' as [Text],'' as [Value]  ");
            }
            if (vcZB != "")
            {
                strSQL.AppendLine(" and vcData3 = '" + vcZB + "'");
                strSQL.AppendLine("  union all select '' as [Text],'' as [Value]  ");
            }
            return SearchPartData(strSQL.ToString());
        }

        /// <summary>
        /// 检索信息栏绑定组别
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <returns></returns>
        public DataTable dllZB(string vcPorType)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine("select distinct [vcData3] as [Text],[vcData3] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
            if (vcPorType == "")
            {
                strSQL.AppendLine("  union all    select '' as [Text],'' as [Value]  ");
            }
            if (vcPorType != "")
            {
                strSQL.AppendLine(" and  vcData1 = '" + vcPorType + "'");
                strSQL.AppendLine("  union all    select '' as [Text],'' as [Value]  ");
            }
            return SearchPartData(strSQL.ToString());
        }

        /// <summary>
        /// 检索信息栏绑定品番工场
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <returns></returns>
        public DataTable dllLB1(string vcPorType)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine("select distinct vcData1 as [Value],vcData2 as [Text]  from [ConstMst] where [vcDataId]='KBPlant'");
            if (vcPorType == "")
            {
                strSQL.AppendLine("  union all    select  '' as [Value],'' as [Text]  ");
            }
            if (vcPorType != "")
            {
                strSQL.AppendLine(" and  vcData1 = '" + vcPorType + "'");
            }
            return SearchPartData(strSQL.ToString());
        }

        public bool checkmod(string vcPartsNo)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine("select * from tPartInfoMaster where vcPartsNo='" + vcPartsNo + "'");
            dt = SearchPartData(strSQL.ToString());
            if (dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新内制品品番表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="useid"></param>
        /// <returns></returns>
        public bool InUpdeOldData(DataTable dt, string useid)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                DataRow[] rows = dt.Select("iFlag='2' or iFlag='3'");
                string vcDateTime = System.DateTime.Now.ToString();
                try
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow rowdselect = rows[i];
                        string strSqlUp = "";//更新语句
                        string strSqlDe = "";//删除语句
                        string vcPartsNo = rowdselect["vcPartsNo"].ToString();
                        string vcDock = rowdselect["vcDock"].ToString();
                        string vcCarFamilyCode = rowdselect["vcCarFamilyCode"].ToString();
                        string qinfengtz = rowdselect["qinfengtz"].ToString();
                        string vcQJcontainer = rowdselect["vcQJcontainer"].ToString();
                        string shengchanbs = rowdselect["shengchanbs"].ToString();
                        string zubie = rowdselect["zubie"].ToString();
                        string leibie = rowdselect["leibie"].ToString();
                        string flag = rowdselect["iFlag"].ToString();
                        string pinfanpindu = rowdselect["pinfanpindu"].ToString();//20180908增加品番频度的更新字段 - 李兴旺
                        string dTimeFrom = rowdselect["dTimeFrom"].ToString();//20180908通过起止时间确定唯一选中的一行 - 李兴旺
                        string dTimeTo = rowdselect["dTimeTo"].ToString();//20180908通过起止时间确定唯一选中的一行 - 李兴旺
                        if (flag == "2")
                        {
                            strSqlUp = "UPDATE [tPartInfoMaster]";
                            strSqlUp += "   SET [vcCarFamilyCode] ='" + vcCarFamilyCode + "'";
                            strSqlUp += "      ,[vcQFflag] ='" + qinfengtz + "'";
                            strSqlUp += "      ,[vcQJcontainer] ='" + vcQJcontainer + "'";
                            strSqlUp += "      ,[vcPorType] ='" + shengchanbs + "'";
                            strSqlUp += "      ,[vcZB] ='" + zubie + "'";
                            strSqlUp += "      ,[dUpdataTime] =getdate()";
                            strSqlUp += "      ,[vcUpdataUser] ='" + useid + "'";
                            strSqlUp += "      ,[vcPartPlant] ='" + leibie + "'";
                            strSqlUp += "      ,[vcPartFrequence] ='" + pinfanpindu + "'";//20180908增加品番频度的更新字段 - 李兴旺
                            strSqlUp += " WHERE  [vcPartsNo]='" + vcPartsNo + "' AND  [vcDock]='" + vcDock + "'";
                            strSqlUp += "   AND  [dTimeFrom]='" + dTimeFrom + "' ";//20180908通过起止时间确定唯一选中的一行 - 李兴旺
                            strSqlUp += "   AND  [dTimeTo]='" + dTimeTo + "' ";//20180908通过起止时间确定唯一选中的一行 - 李兴旺
                        }
                        else
                            if (flag == "3")
                        {
                            strSqlDe = "DELETE FROM [tPartInfoMaster]";
                            strSqlDe += " WHERE  [vcPartsNo]='" + vcPartsNo + "' AND  [vcDock]='" + vcDock + "'";
                            strSqlDe += "   AND  [dTimeFrom]='" + dTimeFrom + "' ";//20180908通过起止时间确定唯一选中的一行 - 李兴旺
                            strSqlDe += "   AND  [dTimeTo]='" + dTimeTo + "' ";//20180908通过起止时间确定唯一选中的一行 - 李兴旺
                        }
                        if (strSqlUp != "")
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = connection;
                            cmd.Transaction = trans;
                            cmd.CommandText = strSqlUp;
                            cmd.ExecuteNonQuery();
                        }
                        if (strSqlDe != "")
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = connection;
                            cmd.Transaction = trans;
                            cmd.CommandText = strSqlDe;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable SearchPartData(string strSql)
        {
            return excute.ExcuteSqlWithSelectToDT(strSql);
        }

        #region 取得品番数据
        /// <summary>
        /// 获取品番信息方法
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns>布尔类型,True为成功,False为失败</returns>
        public bool GetTable(ref DataTable dt)
        {
            string sTimeFrom = "";
            string sTimeTo = "";
            try
            {
                string VCPIC = "c:\\TFTM";
                string strsql = "select distinct PARTSNO,TIMEFROM,TIMETO,DOCK,CPDCOMPANY,CARFAMILYCODE,INOUTFLAG,SUPPLIERCODE,SUPPLIERPLANT,QUANTITYPERCONTAINER,PACKINGFLAG,PARTSNAMEEN,PARTSNAMECHN,('" + VCPIC + "'+substring(PHOTOPATH,3,81)) as PHOTOPATH,ROUTE,CURRENTPASTCODE,REMARK1,REMARK2 from SP_M_SITEM";
                strsql += "      where  INOUTFLAG='0' AND (TIMETO>convert(varchar(8),getdate(),112) or TIMETO=convert(varchar(8),getdate(),112))";
                strsql += "      union all";
                strsql += "      select distinct PARTSNO,TIMEFROM,TIMETO,DOCK,CPDCOMPANY,CARFAMILYCODE,'1' as INOUTFLAG,SUPPLIERCODE,'' as SUPPLIERPLANT,QUANTITYPERCONTAINER,'' as PACKINGFLAG,PARTSNAMEEN,PARTSNAMECHN,('" + VCPIC + "'+substring(PHOTOPATH,3,81)) as PHOTOPATH,ROUTE,'' as CURRENTPASTCODE,REMARK1,REMARK2 from   SP_M_EDSITEM";
                strsql += "      where (TIMETO>convert(varchar(8),getdate(),112) or TIMETO=convert(varchar(8),getdate(),112))  order by PARTSNO ";
                DataSet dataSet = excute.ExcuteSqlWithSelectToDS(strsql);
                DataTable dtTemp = dataSet.Tables[0];
                foreach (DataRow addTemp in dtTemp.Rows)
                {
                    sTimeFrom = "";
                    sTimeTo = "";
                    DataRow drTemp = dt.NewRow();
                    drTemp["PARTSNO"] = addTemp["PARTSNO"];

                    sTimeFrom = sTimeFrom + addTemp["TIMEFROM"].ToString().Substring(0, 4) + "-";
                    sTimeFrom = sTimeFrom + addTemp["TIMEFROM"].ToString().Substring(4, 2) + "-";
                    sTimeFrom = sTimeFrom + addTemp["TIMEFROM"].ToString().Substring(6, 2);
                    drTemp["TIMEFROM"] = DateTime.Parse(sTimeFrom).ToString("yyyy-MM-dd");

                    sTimeTo = sTimeTo + addTemp["TIMETO"].ToString().Substring(0, 4) + "-";
                    sTimeTo = sTimeTo + addTemp["TIMETO"].ToString().Substring(4, 2) + "-";
                    sTimeTo = sTimeTo + addTemp["TIMETO"].ToString().Substring(6, 2);
                    drTemp["TIMETO"] = DateTime.Parse(sTimeTo).ToString("yyyy-MM-dd");
                    if (addTemp["DOCK"] != null)
                    {
                        drTemp["DOCK"] = addTemp["DOCK"];
                    }
                    if (addTemp["CPDCOMPANY"] != null)
                    {
                        drTemp["CPDCOMPANY"] = addTemp["CPDCOMPANY"];
                    }
                    if (addTemp["CARFAMILYCODE"] != null)
                    {
                        drTemp["CARFAMILYCODE"] = addTemp["CARFAMILYCODE"];
                    }
                    if (addTemp["INOUTFLAG"] != null)
                    {
                        drTemp["INOUTFLAG"] = addTemp["INOUTFLAG"];
                    }
                    if (addTemp["SUPPLIERCODE"] != null)
                    {
                        drTemp["SUPPLIERCODE"] = addTemp["SUPPLIERCODE"];
                    }
                    if (addTemp["SUPPLIERPLANT"] != null)
                    {
                        drTemp["SUPPLIERPLANT"] = addTemp["SUPPLIERPLANT"];
                    }

                    drTemp["QUANTITYPERCONTAINER"] = addTemp["QUANTITYPERCONTAINER"];
                    if (addTemp["PACKINGFLAG"] != null)
                    {
                        drTemp["PACKINGFLAG"] = addTemp["PACKINGFLAG"];
                    }
                    if (addTemp["PARTSNAMEEN"] != null)
                    {
                        drTemp["PARTSNAMEEN"] = addTemp["PARTSNAMEEN"];
                    }
                    if (addTemp["PARTSNAMECHN"] != null)
                    {
                        drTemp["PARTSNAMECHN"] = addTemp["PARTSNAMECHN"];
                    }
                    drTemp["PHOTOPATH"] = addTemp["PHOTOPATH"];
                    drTemp["ROUTE"] = addTemp["ROUTE"];
                    if (addTemp["CURRENTPASTCODE"] != null)
                    {
                        drTemp["CURRENTPASTCODE"] = addTemp["CURRENTPASTCODE"];
                    }
                    if (addTemp["REMARK1"] != null)
                    {
                        drTemp["REMARK1"] = addTemp["REMARK1"];
                    }
                    if (addTemp["REMARK2"] != null)
                    {
                        drTemp["REMARK2"] = addTemp["REMARK2"];
                    }
                    dt.Rows.Add(drTemp);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 清空临时表
        public bool TurnCate()
        {
            try
            {
                //清空临时表tPartInfoMaster_Temp
                SqlCommand cd = new SqlCommand();
                cd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                if (cd.Connection.State == ConnectionState.Closed)
                {
                    cd.Connection.Open();
                }
                cd.CommandText = "truncate table tPartInfoMaster_Temp";
                cd.ExecuteNonQuery();
                if (cd.Connection.State == ConnectionState.Open)
                {
                    cd.Connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 插入数据到临时表tPartInfoMaster_Temp
        public bool insertTableLN(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                DataTable dt1 = dt.Clone();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt1.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dr[j] = dt.Rows[i][j].ToString();
                    }
                    dt1.Rows.Add(dr);
                }

                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0190_InsertMasterTemp", connection, trans);
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12, "PARTSNO");
                    cmdln.Parameters.Add("@dTimeFrom", SqlDbType.VarChar, 10, "TIMEFROM");
                    cmdln.Parameters.Add("@dTimeTo", SqlDbType.VarChar, 10, "TIMETO");
                    cmdln.Parameters.Add("@vcDock", SqlDbType.VarChar, 2, "DOCK");
                    cmdln.Parameters.Add("@vcCpdCompany", SqlDbType.VarChar, 5, "CPDCOMPANY");
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "CARFAMILYCODE");
                    cmdln.Parameters.Add("@vcInOutFlag", SqlDbType.VarChar, 1, "INOUTFLAG");
                    cmdln.Parameters.Add("@vcSupplierCode", SqlDbType.VarChar, 5, "SUPPLIERCODE");
                    cmdln.Parameters.Add("@vcSupplierPlant", SqlDbType.VarChar, 1, "SUPPLIERPLANT");
                    cmdln.Parameters.Add("@iQuantityPerContainer", SqlDbType.VarChar, 8, "QUANTITYPERCONTAINER");
                    cmdln.Parameters.Add("@vcPackingFlag", SqlDbType.VarChar, 1, "PACKINGFLAG");
                    cmdln.Parameters.Add("@vcPartsNameEN", SqlDbType.VarChar, 500, "PARTSNAMEEN");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "PARTSNAMECHN");
                    cmdln.Parameters.Add("@vcPhotoPath", SqlDbType.VarChar, 500, "PHOTOPATH");
                    cmdln.Parameters.Add("@vcCurrentPastCode", SqlDbType.VarChar, 2, "CURRENTPASTCODE");
                    cmdln.Parameters.Add("@vcLogisticRoute", SqlDbType.VarChar, 500, "ROUTE");
                    cmdln.Parameters.Add("@vcRemark1", SqlDbType.VarChar, 500, "REMARK1");
                    cmdln.Parameters.Add("@vcRemark2", SqlDbType.VarChar, 500, "REMARK2");
                    SqlDataAdapter adaln = new SqlDataAdapter();
                    adaln.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    adaln.Update(dt1);
                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion

        public DataTable CompareData()
        {
            StringBuilder ssql = new StringBuilder();
            ssql.AppendLine(" select vcPartsNo,dTimeFrom,dTimeTo,vcDock,vcCpdCompany,vcCarFamilyCode,vcInOutFlag,vcSupplierCode,vcSupplierPlant,iQuantityPerContainer,");
            ssql.AppendLine("        vcPackingFlag,vcPartsNameEN,vcPartsNameCHN,vcPhotoPath,vcCurrentPastCode,vcLogisticRoute,vcRemark1,vcRemark2,");
            ssql.AppendLine("        [T2vcpartsno],[T2vcDock],  [T2TO],");
            ssql.AppendLine("       case ");
            ssql.AppendLine("           when T2TO is null then 'insert'");
            ssql.AppendLine("          else 'update' end 'flag' ");
            ssql.AppendLine(" from ( ");
            ssql.AppendLine("select T1.vcPartsNo,     T1.dTimeFrom,     T1.dTimeTo,T1.vcDock,T1.vcCpdCompany,         T1.vcCarFamilyCode, ");
            ssql.AppendLine("       T1.vcInOutFlag,   T1.vcSupplierCode,T1.vcSupplierPlant,  T1.iQuantityPerContainer,T1.vcPackingFlag,  T1.vcPartsNameEN,");
            ssql.AppendLine("       T1.vcPartsNameCHN,T1.vcPhotoPath,   T1.vcCurrentPastCode,T1.vcLogisticRoute,      T1.vcRemark1,      T1.vcRemark2,");
            ssql.AppendLine("       T2.dTimeTo as [T2TO],T2.vcpartsno as [T2vcpartsno],T2.vcDock as [T2vcDock]  ");
            ssql.AppendLine("        from ( ");
            ssql.AppendLine("              select * from tPartInfoMaster_Temp a");
            ssql.AppendLine("              where not exists(select * from  tPartInfoMaster b ");
            ssql.AppendLine("                               where a.vcPartsNo=b.vcPartsNo and a.vcDock=b.vcDock and a.dTimeFrom=b.dTimeFrom and a.dTimeTo=b.dTimeTo and a.vcCpdCompany=b.vcCpdCompany and a.vcCarFamilyCode=b.vcCarFamilyCode and a.vcInOutFlag=b.vcInOutFlag and a.vcSupplierCode=b.vcSupplierCode");
            ssql.AppendLine("                                    and a.vcSupplierPlant=b.vcSupplierPlant and a.iQuantityPerContainer=b.iQuantityPerContainer and a.vcPackingFlag=b.vcPackingFlag and a.vcPartsNameEN=b.vcPartsNameEN and a.vcPartsNameCHN=b.vcPartsNameCHN and a.vcPhotoPath=b.vcPhotoPath ");
            ssql.AppendLine("                                    and a.vcCurrentPastCode=b.vcCurrentPastCode and a.vcLogisticRoute=b.vcLogisticRoute and a.vcRemark1=b.vcRemark1 and a.vcRemark2=b.vcRemark2");
            ssql.AppendLine("");
            ssql.AppendLine("                                             )) T1");
            ssql.AppendLine("       left join(select * from tPartInfoMaster) T2 on T1.vcPartsNo = T2.vcPartsNo and T1.vcDock = T2.vcDock and T1.dTimeFrom=T2.dTimeFrom)TT");
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool InsertOrUpData(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                string vcCreatTime = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string _strSql = "";
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                        string dTimeFrom = dt.Rows[i]["dTimeFrom"].ToString();
                        string dTimeTo = dt.Rows[i]["dTimeTo"].ToString();
                        string vcDock = dt.Rows[i]["vcDock"].ToString();
                        string vcCpdCompany = dt.Rows[i]["vcCpdCompany"].ToString();
                        string vcCarFamilyCode = dt.Rows[i]["vcCarFamilyCode"].ToString();
                        string vcInOutFlag = dt.Rows[i]["vcInOutFlag"].ToString();
                        string vcSupplierCode = dt.Rows[i]["vcSupplierCode"].ToString();
                        string vcSupplierPlant = dt.Rows[i]["vcSupplierPlant"].ToString();
                        string iQuantityPerContainer = dt.Rows[i]["iQuantityPerContainer"].ToString();
                        string vcPackingFlag = dt.Rows[i]["vcPackingFlag"].ToString();
                        string vcPartsNameEN = dt.Rows[i]["vcPartsNameEN"].ToString();
                        string vcPartsNameCHN = dt.Rows[i]["vcPartsNameCHN"].ToString();
                        string vcPhotoPath = dt.Rows[i]["vcPhotoPath"].ToString();
                        string vcCurrentPastCode = dt.Rows[i]["vcCurrentPastCode"].ToString();
                        string vcLogisticRoute = dt.Rows[i]["vcLogisticRoute"].ToString();
                        string vcRemark1 = dt.Rows[i]["vcRemark1"].ToString();
                        string vcRemark2 = dt.Rows[i]["vcRemark2"].ToString();
                        string flag = dt.Rows[i]["flag"].ToString();
                        if (flag == "1")
                        {
                            strSql = "INSERT INTO [tPartInfoMaster]([vcPartsNo],[dTimeFrom],[dTimeTo],[vcDock],[vcCpdCompany],[vcCarFamilyCode],[vcInOutFlag],[vcSupplierCode],";
                            strSql += "                              [vcSupplierPlant],[iQuantityPerContainer],[vcPackingFlag],[vcPartsNameEN],[vcPartsNameCHN],[vcPhotoPath],";
                            strSql += "                              [vcCurrentPastCode],[vcLogisticRoute],[vcRemark1],[vcRemark2],[dDateTime],[dUpdataTime],[vcUpdataUser])";
                            strSql += "     VALUES";
                            strSql += "('" + vcPartsNo + "','" + dTimeFrom + "','" + dTimeTo + "','" + vcDock + "','" + vcCpdCompany + "','" + vcCarFamilyCode + "','" + vcInOutFlag + "','" + vcSupplierCode + "',";
                            strSql += "'" + vcSupplierPlant + "','" + iQuantityPerContainer + "','" + vcPackingFlag + "','" + vcPartsNameEN + "','" + vcPartsNameCHN + "','" + vcPhotoPath + "',";
                            strSql += "'" + vcCurrentPastCode + "','" + vcLogisticRoute + "','" + vcRemark1 + "','" + vcRemark2 + "','" + vcCreatTime + "','" + vcCreatTime + "','System')";
                        }
                        else if (flag == "2")
                        {
                            _strSql = "UPDATE [tPartInfoMaster]";
                            _strSql += "   SET [dTimeTo] = '" + dTimeTo + "',[vcCpdCompany] = '" + vcCpdCompany + "',[vcCarFamilyCode] ='" + vcCarFamilyCode + "',[vcInOutFlag] = '" + vcInOutFlag + "'";
                            _strSql += "      ,[vcSupplierCode] = '" + vcSupplierCode + "',[vcSupplierPlant] = '" + vcSupplierPlant + "',[iQuantityPerContainer] = '" + iQuantityPerContainer + "',[vcPackingFlag] = '" + vcPackingFlag + "'";
                            _strSql += "      ,[vcPartsNameEN] = '" + vcPartsNameEN + "',[vcPartsNameCHN] = '" + vcPartsNameCHN + "',[vcPhotoPath] = '" + vcPhotoPath + "',[vcCurrentPastCode] = '" + vcCurrentPastCode + "',[vcLogisticRoute] = '" + vcLogisticRoute + "'";
                            _strSql += "      ,[vcRemark1] ='" + vcRemark1 + "',[vcRemark2] = '" + vcRemark2 + "',[dUpdataTime] ='" + vcCreatTime + "',[vcUpdataUser] ='system' ";
                            _strSql += " WHERE  [vcPartsNo]='" + vcPartsNo + "' AND  [vcDock]='" + vcDock + "' AND [dTimeFrom]='" + dTimeFrom + "'";
                        }
                        if (strSql != "")
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = connection;
                            cmd.Transaction = trans;
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }
                        if (_strSql != "")
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = connection;
                            cmd.Transaction = trans;
                            cmd.CommandText = _strSql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }

}
