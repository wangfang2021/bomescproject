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

        //检索数据//需要修改SQL语句关联生产部署表和组别表20121221(世界末日)
        public DataTable SearchPartData(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append("select substring(vcPartsNo,0, 6) + '-' + substring(vcPartsNo,6, 5) + '-' + substring(vcPartsNo,11, 2) as vcPartsNo,");
            strSQL.Append("dTimeFrom, dTimeTo, vcPartPlant, vcDock, vcCarFamilyCode, vcPartsNameEN,");
            strSQL.Append("vcQFflag, iQuantityPerContainer, vcQJcontainer, vcPorType, vcZB, t.vcName as vcPartFrequence, vcCpdCompany, vcSupplierCode,");
            strSQL.Append("'0' as iFlag, '0' as vcModFlag, '0' as vcAddFlag, iAutoId ");
            strSQL.Append("from TPartInfoMaster ");
            strSQL.Append("left join (select vcCodeId, vcName, vcValue from TCode where vcCodeId='C047') t ");
            strSQL.Append("on vcPartFrequence=t.vcValue ");
            strSQL.Append("where 1=1 ");
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                strSQL.AppendLine(" and vcPartsNo like '" + vcPartsNo + "%'");
            }
            if (!string.IsNullOrEmpty(vcCarFamilyCode))
            {
                strSQL.AppendLine(" and vcCarFamilyCode like '" + vcCarFamilyCode + "%'");
            }
            if (vcPorType != "")
            {
                strSQL.AppendLine(" and vcPorType like '%" + vcPorType + "%'");
            }
            if (vcZB != "")
            {
                strSQL.AppendLine(" and vcZB like '" + vcZB + "%'");
            }
            if (vcPartPlant != "")
            {
                strSQL.AppendLine(" and vcPartPlant like '" + vcPartPlant + "%'");
            }
            if (vcPartFrequence != "")
            {
                strSQL.AppendLine(" and vcPartFrequence='" + vcPartFrequence + "'");
            }
            strSQL.AppendLine(" order by vcPartsNo");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public DataTable OutputPartData(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append("select substring(vcPartsNo,0, 6) + '-' + substring(vcPartsNo,6, 5) + '-' + substring(vcPartsNo,11, 2) as vcPartsNo,");
            strSQL.Append("dTimeFrom, dTimeTo, vcPartPlant, vcDock, vcCarFamilyCode, vcPartsNameEN,");
            strSQL.Append("vcQFflag, case when len(iQuantityPerContainer)>0 then convert(int,iQuantityPerContainer) end as iQuantityPerContainer,  case when len(vcQJcontainer)>0 then convert(int,vcQJcontainer) end as vcQJcontainer, vcPorType, case when len(vcZB)>0 then convert(int,vcZB) end as vcZB, t.vcName as vcPartFrequence, vcCpdCompany, vcSupplierCode,");
            strSQL.Append("'0' as iFlag, '0' as vcModFlag, '0' as vcAddFlag, iAutoId ");
            strSQL.Append("from TPartInfoMaster ");
            strSQL.Append("left join (select vcCodeId, vcName, vcValue from TCode where vcCodeId='C047') t ");
            strSQL.Append("on vcPartFrequence=t.vcValue ");
            strSQL.Append("where vcInOutFlag='0' ");
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                strSQL.AppendLine(" and vcPartsNo like '" + vcPartsNo + "%'");
            }
            if (!string.IsNullOrEmpty(vcCarFamilyCode))
            {
                strSQL.AppendLine(" and vcCarFamilyCode like '" + vcCarFamilyCode + "%'");
            }
            if (vcPorType != "")
            {
                strSQL.AppendLine(" and vcPorType like '%" + vcPorType + "%'");
            }
            if (vcZB != "")
            {
                strSQL.AppendLine(" and vcZB like '" + vcZB + "%'");
            }
            if (vcPartPlant != "")
            {
                strSQL.AppendLine(" and vcPartPlant like '" + vcPartPlant + "%'");
            }
            if (vcPartFrequence != "")
            {
                strSQL.AppendLine(" and vcPartFrequence='" + vcPartFrequence + "'");
            }
            strSQL.AppendLine(" order by vcPartsNo");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
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
            if (vcZB == "")
            {
                strSQL.AppendLine("select distinct [vcData1] as [vcName],[vcData1] as [vcValue] from [ConstMst] where [vcDataId]='ProType'");
            }
            else
            {
                strSQL.AppendLine("select distinct [vcData1] as [vcName],[vcData1] as [vcValue] from [ConstMst] where [vcDataId]='ProType' and vcData3='" + vcZB + "'");
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
            if (vcPorType == "")
            {
                strSQL.AppendLine("select distinct [vcData3] as [vcName],[vcData3] as [vcValue] from [ConstMst] where [vcDataId]='ProType'");
            }
            else
            {
                strSQL.AppendLine("select distinct [vcData3] as [vcName],[vcData3] as [vcValue] from [ConstMst] where [vcDataId]='ProType' and vcData1='" + vcPorType + "'");
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

            strSQL.AppendLine("select distinct vcData1 as [vcValue],vcData2 as [vcName]  from [ConstMst] where [vcDataId]='KBPlant'");
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

        public DataTable ddlPartFrequence()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcName, vcValue from TCode where vcCodeId='C047' and vcValue in ('0','2')");
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

        #region 保存
        public int Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == false && bModFlag == true)//修改
                    {
                        string iAutoId = getSqlValue(listInfoData[i]["iAutoId"], false);
                        string vcPartsNo = getSqlValue(listInfoData[i]["vcPartsNo"], false).Replace("-", "");
                        string vcCpdCompany = getSqlValue(listInfoData[i]["vcCpdCompany"], false);
                        string vcTimeFrom = getSqlValue(listInfoData[i]["dTimeFrom"], false);
                        string vcTimeTo = getSqlValue(listInfoData[i]["dTimeTo"], false);
                        string vcSupplierCode = getSqlValue(listInfoData[i]["vcSupplierCode"], false);
                        string vcPartPlant = getSqlValue(listInfoData[i]["vcPartPlant"], false);
                        string vcCarFamilyCode = getSqlValue(listInfoData[i]["vcCarFamilyCode"], false);
                        string vcQFflag = getSqlValue(listInfoData[i]["vcQFflag"], false);
                        string vcQJcontainer = getSqlValue(listInfoData[i]["vcQJcontainer"], false);
                        string vcPorType = getSqlValue(listInfoData[i]["vcPorType"], false);
                        string vcZB = getSqlValue(listInfoData[i]["vcZB"], false);

                        bool isExists = existParts(vcPartsNo, "TFTM", vcCpdCompany, vcSupplierCode);
                        if (isExists)//如果品番表存在，则更新
                        {
                            sql.Append("  update TPartInfoMaster set  \r\n");
                            sql.Append("  vcPartPlant='" + vcPartPlant + "'   \r\n");
                            //sql.Append("  ,dTimeFrom='" + vcTimeFrom + "'   \r\n");
                            //sql.Append("  ,dTimeTo='" + vcTimeTo + "'   \r\n");
                            //sql.Append("  ,vcCarFamilyCode='" + vcCarFamilyCode + "'   \r\n");
                            sql.Append("  ,vcQFflag='" + vcQFflag + "'   \r\n");
                            sql.Append("  ,vcQJcontainer='" + vcQJcontainer + "'   \r\n");
                            sql.Append("  ,vcPorType='" + vcPorType + "'   \r\n");
                            sql.Append("  ,vcZB='" + vcZB + "'   \r\n");
                            sql.Append("  ,vcUpdataUser='" + strUserId + "'   \r\n");
                            sql.Append("  ,dUpdataTime=getdate()   \r\n");
                            sql.Append("  where iAutoId=" + iAutoId + "  \r\n");
                            //sql.Append("  where vcPartsNo='" + vcPartsNo + "'  \r\n");
                            //sql.Append("  and vcCpdCompany='" + vcCpdCompany + "' \r\n");
                            //sql.Append("  and vcSupplierCode='" + vcSupplierCode + "' \r\n");
                        }
                        else//如果品番表不存在，则插入
                        {
                            sql.Append("insert into TPartInfoMaster(vcPartsNo, dTimeFrom, dTimeTo, vcCpdCompany, vcSupplierCode, vcInOutFlag, ");
                            sql.Append("vcPartPlant, vcCarFamilyCode, vcQFflag, vcQJcontainer, vcPorType, vcZB, vcUpdataUser, dUpdataTime) ");
                            sql.Append("values('" + vcPartsNo + "',");
                            sql.Append("'" + vcTimeFrom + "',");
                            sql.Append("'" + vcTimeTo + "',");
                            sql.Append("'" + vcCpdCompany + "',");
                            sql.Append("'" + vcSupplierCode + "',");
                            sql.Append("'0',");
                            sql.Append("'" + vcPartPlant + "',");
                            sql.Append("'" + vcCarFamilyCode + "',");
                            sql.Append("'" + vcQFflag + "',");
                            sql.Append("'" + vcQJcontainer + "',");
                            sql.Append("'" + vcPorType + "',");
                            sql.Append("'" + vcZB + "',");
                            sql.Append("'" + strUserId + "',");
                            sql.Append("getdate())");
                        }
                    }
                }
                return excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
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
                sql.Append("  delete tPartInfoMaster where iAutoId in(   \r\n ");
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

        #region 判断是否存在品番信息 wlw
        /// <summary>
        /// 判断是否存在品番信息
        /// </summary>
        /// <param name="vcPartId">品番</param>
        /// <param name="vcPackingPlant">包装工厂</param>
        /// <param name="vcReceiver">收货方</param>
        /// <param name="vcSupplierId">供应商ID</param>
        /// <returns></returns>
        public bool existParts(string vcPartId, string vcPackingPlant, string vcReceiver, string vcSupplierId)
        {
            string sql = "select count(vcPartsNo) from TPartInfoMaster where vcPartsNo='" + vcPartId + "' and vcCpdCompany='" + vcReceiver + "' and vcSupplierCode='" + vcSupplierId + "'";
            int i = excute.ExecuteScalar(sql);
            if (i > 0)
                return true;
            return false;
        }

        public bool existPartInMaster(string vcPartId, string vcPackingPlant, string vcReceiver, string vcSupplierId)
        {
            string sql = "select count(vcPartId) from TSPMaster where vcPartId='" + vcPartId + "' and vcReceiver='" + vcReceiver + "' and vcSupplierId='" + vcSupplierId + "' and vcPackingPlant='" + vcPackingPlant + "'";
            int i = excute.ExecuteScalar(sql);
            if (i > 0)
                return true;
            return false;
        }
        #endregion

        public DataTable SearchPartData(string strSql)
        {
            return excute.ExcuteSqlWithSelectToDT(strSql);
        }


        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return null;
            else if (obj.ToString().Trim() == "" && isObject)
                return null;
            else
                return obj.ToString();
        }
        #endregion

        #region 检查生产部署和组别
        /// <summary>
        /// 检查生产部署和组别
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        public string CheckRepeat_ExcelDBTypeZB(DataTable dt)
        {
            int count = dt.Rows.Count;
            StringBuilder strSQL = new StringBuilder();
            dt.Columns[0].ColumnName = "vcPartsNo";
            dt.Columns[1].ColumnName = "dFromTime";//20180908增加起止时间 - 李兴旺
            dt.Columns[2].ColumnName = "dToTime";//20180908增加起止时间 - 李兴旺
            dt.Columns[3].ColumnName = "vcDock";
            dt.Columns[4].ColumnName = "vcPartPlant";
            dt.Columns[5].ColumnName = "vcCarFamilyCode";
            dt.Columns[6].ColumnName = "vcPartENName";
            dt.Columns[7].ColumnName = "vcQFflag";
            dt.Columns[8].ColumnName = "iQuantityPerContainer";
            dt.Columns[9].ColumnName = "vcQJcontainer";
            dt.Columns[10].ColumnName = "vcPorType";
            dt.Columns[11].ColumnName = "vcZB";
            dt.Columns[12].ColumnName = "vcOrderingMethod";//20180908增加品番频度 - 李兴旺
            dt.Columns[13].ColumnName = "vcReceiver";
            dt.Columns[14].ColumnName = "vcSupplierId";
            //dt.Columns.Remove("vcPartsNameEN");
            //dt.Columns.Remove("vcPartsNameCHN");
            //dt.Columns.Remove("iQuantityPerContainer");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strSQL.AppendLine("select count(1) from ConstMst where vcDataId='ProType' and vcData1='" + dt.Rows[i]["vcPorType"].ToString() + "' and vcData3='" + dt.Rows[i]["vcZB"].ToString() + "'");
                int nums = excute.ExecuteSQLNoQuery(strSQL.ToString());
                if (nums == 0)
                {
                    return "Excel数据中存在生产部署、组别未在数据库中维护的数据";
                }
            }
            return "";
        }
        #endregion

        #region 将Excel的内容导入到数据库中
        /// <summary>
        /// 将Excel的内容导入到数据库中
        /// </summary>
        /// <param name="InputFile">导入Html控件</param>
        /// <param name="vcCreaterId">创建者ID</param>
        public string ImportStandTime(DataTable dt, string vcCreaterId)
        {
            string msg = string.Empty;
            try
            {
                int count = dt.Rows.Count;
                dt.Columns[0].ColumnName = "vcPartsNo";
                dt.Columns[1].ColumnName = "dFromTime";//20180908增加起止时间 - 李兴旺
                dt.Columns[2].ColumnName = "dToTime";//20180908增加起止时间 - 李兴旺
                dt.Columns[3].ColumnName = "vcDock";
                dt.Columns[4].ColumnName = "vcPartPlant";
                dt.Columns[5].ColumnName = "vcCarFamilyCode";
                dt.Columns[6].ColumnName = "vcPartENName";
                dt.Columns[7].ColumnName = "vcQFflag";
                dt.Columns[8].ColumnName = "iQuantityPerContainer";
                dt.Columns[9].ColumnName = "vcQJcontainer";
                dt.Columns[10].ColumnName = "vcPorType";
                dt.Columns[11].ColumnName = "vcZB";
                dt.Columns[12].ColumnName = "vcOrderingMethod";//20180908增加品番频度 - 李兴旺
                dt.Columns[13].ColumnName = "vcReceiver";
                dt.Columns[14].ColumnName = "vcSupplierId";
                dt.Columns.Remove("vcPartENName");
                dt.Columns.Remove("iQuantityPerContainer");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcQFflag"].ToString() == "○")
                    {
                        dt.Rows[i]["vcQFflag"] = "1";
                    }
                    else if (dt.Rows[i]["vcQFflag"].ToString() == "×")
                    {
                        dt.Rows[i]["vcQFflag"] = "2";
                    }
                    else if (dt.Rows[i]["vcQFflag"].ToString() == "")
                    {
                        dt.Rows[i]["vcQFflag"] = "";
                    }
                    else
                    {
                        return "第" + (i + 1).ToString() + "行‘秦丰涂装’ 格式输入错误！";
                    }
                    if (dt.Rows[i]["vcPartPlant"].ToString() == "#1")
                    {
                        dt.Rows[i]["vcPartPlant"] = "1";
                    }
                    else if (dt.Rows[i]["vcPartPlant"].ToString() == "#2")
                    {
                        dt.Rows[i]["vcPartPlant"] = "2";
                    }
                    else if (dt.Rows[i]["vcPartPlant"].ToString() == "#3")
                    {
                        dt.Rows[i]["vcPartPlant"] = "3";
                    }
                    else if (dt.Rows[i]["vcPartPlant"].ToString() == "#4")
                    {
                        dt.Rows[i]["vcPartPlant"] = "4";
                    }
                    else if (dt.Rows[i]["vcPartPlant"].ToString().Trim() == "")
                    {
                        dt.Rows[i]["vcPartPlant"] = "";
                    }
                    else
                    {
                        return "第" + (i + 1).ToString() + "行‘品番工场’格式输入错误！";
                    }
                    bool b = existPartInMaster(dt.Rows[i]["vcPartsNo"].ToString(), "TFTM", dt.Rows[i]["vcReceiver"].ToString(), dt.Rows[i]["vcSupplierId"].ToString());
                    if (!b)
                        return "第" + (i + 1).ToString() + "行，‘品番、收货方、供应商编码’在基础数据中不存在！";
                }
                SqlTransaction tran = null;
                using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = tran = conn.BeginTransaction();
                        string sql = "";
                        //事务
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            sql += DoTransactionOfInsert(dt.Rows[j], vcCreaterId);//按照行来更新数据
                        }
                        cmd.CommandText = sql;
                        int counts = cmd.ExecuteNonQuery();
                        tran.Commit();
                        if (counts > 0)
                            return "";
                        return "未导入更新";
                    }
                    catch (Exception ex)
                    {
                        //回滚
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 更新数据事务
        /// <summary>
        /// 更新数据事务2012-6-10
        /// </summary>
        /// <param name="cmd">SQL类</param>
        /// <param name="dr">数据行</param>
        /// <param name="vcCreater">创建人</param>
        private string DoTransactionOfInsert(DataRow dr, string useid)
        {
            bool isExists = existParts(dr["vcPartsNo"].ToString(), "TFTM", dr["vcReceiver"].ToString(), dr["vcSupplierId"].ToString());
            StringBuilder sql = new StringBuilder();
            if (isExists)//如果品番表存在，则更新
            {
                sql.Append("  update TPartInfoMaster set ");
                sql.Append("  vcPartPlant='" + dr["vcPartPlant"].ToString() + "' ");
                sql.Append("  ,dTimeFrom='" + dr["dFromTime"].ToString() + "' ");
                sql.Append("  ,dTimeTo='" + dr["dToTime"].ToString() + "' ");
                sql.Append("  ,vcCarFamilyCode='" + dr["vcCarFamilyCode"].ToString() + "' ");
                sql.Append("  ,vcQFflag='" + dr["vcQFflag"].ToString() + "' ");
                sql.Append("  ,vcQJcontainer='" + dr["vcQJcontainer"].ToString() + "' ");
                sql.Append("  ,vcPorType='" + dr["vcPorType"].ToString() + "' ");
                sql.Append("  ,vcZB='" + dr["vcZB"].ToString() + "' ");
                sql.Append("  ,vcUpdataUser='" + useid + "' ");
                sql.Append("  ,dUpdataTime=getdate() ");
                sql.Append("  where vcPartsNo='" + dr["vcPartsNo"].ToString() + "'");
                sql.Append("  and vcCpdCompany='" + dr["vcReceiver"].ToString() + "'");
                sql.Append("  and vcSupplierCode='" + dr["vcSupplierId"].ToString() + "';");
            }
            else//如果品番表存在，则插入
            {
                sql.Append("insert into TPartInfoMaster(vcPartsNo, dTimeFrom, dTimeTo, vcCpdCompany, vcSupplierCode, vcInOutFlag, ");
                sql.Append("vcPartPlant, vcCarFamilyCode, vcQFflag, vcQJcontainer, vcPorType, vcZB, vcUpdataUser, dUpdataTime) ");
                sql.Append("values('" + dr["vcPartsNo"].ToString() + "',");
                sql.Append("'" + dr["dFromTime"].ToString() + "',");
                sql.Append("'" + dr["dToTime"].ToString() + "',");
                sql.Append("'" + dr["vcReceiver"].ToString() + "',");
                sql.Append("'" + dr["vcSupplierId"].ToString() + "',");
                sql.Append("'0',");
                sql.Append("'" + dr["vcPartPlant"].ToString() + "',");
                sql.Append("'" + dr["vcCarFamilyCode"].ToString() + "',");
                sql.Append("'" + dr["vcQFflag"].ToString() + "',");
                sql.Append("'" + dr["vcQJcontainer"].ToString() + "',");
                sql.Append("'" + dr["vcPorType"].ToString() + "',");
                sql.Append("'" + dr["vcZB"].ToString() + "',");
                sql.Append("'" + useid + "',");
                sql.Append("getdate());");
            }
            return sql.ToString();
        }
        #endregion
    }
}
