using Common;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Data;

namespace Logic
{
    public class FS0603_Logic
    {
        FS0603_DataAccess fs0603_DataAccess;
        public FS0603_Logic()
        {
            fs0603_DataAccess = new FS0603_DataAccess();
        }
        public DataTable getCodeInfo(string strType)
        {
            return fs0603_DataAccess.getCodeInfo(strType);
        }
        public DataTable getFormOptions(string strInOut)
        {
            return fs0603_DataAccess.getFormOptions(strInOut);
        }
        public DataTable getSelectOptions(DataTable dataTable, string strName, string strValue)
        {
            try
            {
                DataTable dtOptions = createTable("Options");
                if (dataTable.Rows.Count == 0)
                    return dtOptions;
                if (!dataTable.Columns.Contains(strName))
                    return dtOptions;
                if (!dataTable.Columns.Contains(strValue))
                    return dtOptions;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string vcValue = dataTable.Rows[i][strValue].ToString();
                    string vcName = dataTable.Rows[i][strName].ToString();
                    if (dtOptions.Select("vcValue='" + vcValue + "' and vcName='" + vcName + "'").Length == 0)
                    {
                        DataRow drOptions = dtOptions.NewRow();
                        drOptions["vcName"] = vcName;
                        drOptions["vcValue"] = vcValue;
                        dtOptions.Rows.Add(drOptions);
                    }
                }
                dtOptions.DefaultView.Sort = "vcValue ASC";
                dtOptions = dtOptions.DefaultView.ToTable();
                return dtOptions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strSyncTime_from, string strSyncTime_to, string strChanges, string strPartId, string strCarModel, string strReceiver, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                    string strOrderPlant, string strFromTime, string strToTime, string strBoxType, string strSufferIn, string strSupplierPacking, string strOldProduction, string strDebugTime, string strPackingPlant, bool bCheck, string strOrderby, string strOrderingMethod, string strMandOrder, string strSPChild)
        {
            DataTable dataTable = fs0603_DataAccess.getSearchInfo(strSyncTime_from, strSyncTime_to, strChanges, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, strPackingPlant, bCheck, strOrderby, strOrderingMethod, strMandOrder, strSPChild);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRow["vcSupplierPlant"].ToString() == string.Empty)
                    dataRow["vcSupplierPlant"] = "待维护";
                if (dataRow["iPackingQty"].ToString() == string.Empty)
                    dataRow["iPackingQty"] = "待维护";
                if (dataRow["vcSufferIn"].ToString() == string.Empty)
                    dataRow["vcSufferIn"] = "待维护";
                if (dataRow["vcOrderPlant"].ToString() == string.Empty)
                    dataRow["vcOrderPlant"] = "待维护";
                //if (dataRow["vcPartImage"].ToString() == string.Empty)
                //    dataRow["vcPartImage"] = "暂无图像.jpg";
            }
            return dataTable;
        }
        public DataTable getChildInfo(string strSyncTime_from, string strSyncTime_to, string strChanges, string strPartId, string strCarModel, string strReceiver, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant,
            string strOrderPlant, string strFromTime, string strToTime, string strBoxType, string strSufferIn, string strSupplierPacking, string strOldProduction, string strDebugTime, string strPackingPlant, bool bCheck, string strOrderby, string strOrderingMethod, string strMandOrder, string strSPChild, string strshoupei)
        {
            DataTable dtChildInfo = new DataTable();
            if (strshoupei == "gq")
            {
                dtChildInfo = fs0603_DataAccess.getChildInfo_gq(strSyncTime_from, strSyncTime_to, strChanges, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, strPackingPlant, bCheck, strOrderby, strOrderingMethod, strMandOrder, strSPChild);
            }
            if (strshoupei == "sr")
            {
                dtChildInfo = fs0603_DataAccess.getChildInfo_sr(strSyncTime_from, strSyncTime_to, strChanges, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, strPackingPlant, bCheck, strOrderby, strOrderingMethod, strMandOrder, strSPChild);
            }
            if (strshoupei == "hz")
            {
                dtChildInfo = fs0603_DataAccess.getChildInfo_hz(strSyncTime_from, strSyncTime_to, strChanges, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, strPackingPlant, bCheck, strOrderby, strOrderingMethod, strMandOrder, strSPChild);
            }
            return dtChildInfo;
        }
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData, string strOperId)
        {
            fs0603_DataAccess.deleteInfo(listInfoData, strOperId);
        }
        public DataTable checkFileInfo(DataTable dataTable, string[,] Header, int heardrow, int datarow, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                int count = dataTable.Rows.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (dataTable.Rows[i]["vcType"].ToString().Trim() == "")
                        dataTable.Rows.RemoveAt(i);
                }
                DataTable dtSPInfo = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", true, "", "", "", "");
                DataTable dtImport = dtSPInfo.Clone();
                dtImport.Columns.Add("vcType");
                #region 检验数据重复性及数据格式
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strType = dataTable.Rows[i]["vcType"].ToString().Trim();
                    string strPackingPlant = dataTable.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dataTable.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dataTable.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dataTable.Rows[i]["vcSupplierId"].ToString().Trim();
                    if (strPackingPlant == "" || strPartId == "" || strReceiver == "" || strSupplierId == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报主Key存在空值禁止操作", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strType == "新增")
                    {
                        DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                        if (drSPInfo.Length > 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报存在重复Key禁止操作", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    if (strType == "修改" || strType == "删除")
                    {
                        DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                        if (drSPInfo.Length == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报在手配信息中不存在禁止操作", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                }
                #endregion

                DataTable dtChangesList = ComFunction.getTCode("C002");//变更事项
                DataTable dtInOutList = ComFunction.getTCode("C003");//内外区分
                DataTable dtOESPList = ComFunction.getTCode("C012");//OE=SP
                DataTable dtHaoJiuList = ComFunction.getTCode("C004");//号旧区分
                //DataTable dtOldProductionList = ComFunction.getTCode("C024");//旧型年限生产区分
                DataTable dtBillTypeList = ComFunction.getTCode("C007");//单据区分
                DataTable dtOrderingMethodList = ComFunction.getTCode("C047");//订货方式
                DataTable dtMandOrderList = ComFunction.getTCode("C048");//强制订货
                DataTable dtSupplierPackingList = ComFunction.getTCode("C059");//供应商包装

                if (dtMessage == null || dtMessage.Rows.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow drImport = dtImport.NewRow();
                        string strType = dataTable.Rows[i]["vcType"].ToString().Trim();
                        drImport["vcType"] = strType;
                        #region 同步时间
                        string strSyncTime = dataTable.Rows[i]["dSyncTime"].ToString().Trim();
                        if (strType == "新增")
                            dataTable.Rows[i]["dSyncTime"] = "";
                        else
                        {
                            if (strSyncTime != "")
                            {
                                if (!ComFunction.CheckDate(strSyncTime))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报同步时间格式不正确", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    strSyncTime = Convert.ToDateTime(strSyncTime).ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                        }
                        drImport["dSyncTime"] = strSyncTime;
                        #endregion
                        #region 变更事项
                        string strChanges_name = dataTable.Rows[i]["vcChanges_name"].ToString().Trim();
                        if (strType == "新增")
                            dataTable.Rows[i]["vcChanges_name"] = "";
                        else
                        {
                            if (strChanges_name != "")
                            {
                                DataRow[] drChangesList = dtChangesList.Select("vcName='" + strChanges_name + "'");
                                if (drChangesList.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报变更事项未维护基础数据", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                    strChanges_name = drChangesList[0]["vcValue"].ToString();
                            }
                        }
                        drImport["vcChanges"] = strChanges_name;
                        #endregion
                        //包装工厂
                        string strPackingPlant = dataTable.Rows[i]["vcPackingPlant"].ToString().Trim();
                        drImport["vcPackingPlant"] = strPackingPlant;
                        //品番
                        string strPartId = dataTable.Rows[i]["vcPartId"].ToString().Trim();
                        drImport["vcPartId"] = strPartId;
                        //品名
                        string strPartENName = dataTable.Rows[i]["vcPartENName"].ToString();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strPartENName.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护英文品名", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                        drImport["vcPartENName"] = strPartENName;
                        //车种
                        string strCarfamilyCode = dataTable.Rows[i]["vcCarfamilyCode"].ToString().Trim();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strCarfamilyCode.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护车型", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                        drImport["vcCarfamilyCode"] = strCarfamilyCode;
                        //收货方
                        string strReceiver = dataTable.Rows[i]["vcReceiver"].ToString().Trim();
                        drImport["vcReceiver"] = strReceiver;
                        #region 开始时间
                        string strFromTime = dataTable.Rows[i]["dFromTime"].ToString().Trim();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (!ComFunction.CheckDate(strFromTime))
                            {
                                strFromTime = "";
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报开始时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strFromTime = Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd");
                        }
                        drImport["dFromTime"] = strFromTime;
                        #endregion
                        #region 结束时间
                        string strToTime = dataTable.Rows[i]["dToTime"].ToString().Trim();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strToTime != "")
                            {
                                if (!ComFunction.CheckDate(strFromTime))
                                {
                                    strToTime = "";
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报结束时间格式不正确", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                    strToTime = Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd");
                            }
                            else
                                strToTime = "9999-12-31";
                        }
                        else
                            strToTime = "9999-12-31";
                        drImport["dToTime"] = strToTime;
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strFromTime != ""
                                && strToTime != ""
                                && Convert.ToDateTime(strFromTime) > Convert.ToDateTime(strToTime))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报开始时间大于结束时间", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                        #endregion
                        //替代品番
                        string strPartId_Replace = dataTable.Rows[i]["vcPartId_Replace"].ToString().Trim();
                        drImport["vcPartId_Replace"] = strPartId_Replace;
                        #region 内外区分
                        string strInOut_name = dataTable.Rows[i]["vcInOut_name"].ToString().Trim();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strInOut_name.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护内外", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                DataRow[] drInOutList = dtInOutList.Select("vcName='" + strInOut_name + "'");
                                if (drInOutList.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报内外未维护" + strInOut_name + "基础数据", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                    strInOut_name = drInOutList[0]["vcValue"].ToString();
                            }
                        }
                        drImport["vcInOut"] = strInOut_name;
                        #endregion
                        #region OE=SP
                        string strOESP_name = dataTable.Rows[i]["vcOESP_name"].ToString().Trim();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strOESP_name.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护OE=SP", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                DataRow[] drOESPList = dtOESPList.Select("vcName='" + strOESP_name + "'");
                                if (drOESPList.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报OE=SP未维护" + strOESP_name + "基础数据", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                    strOESP_name = drOESPList[0]["vcValue"].ToString();
                            }
                        }
                        drImport["vcOESP"] = strOESP_name;
                        #endregion
                        #region 号旧区分
                        string strHaoJiu_name = dataTable.Rows[i]["vcHaoJiu_name"].ToString().Trim();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strHaoJiu_name.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护号旧", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                DataRow[] drHaoJiuList = dtHaoJiuList.Select("vcName='" + strHaoJiu_name + "'");
                                if (drHaoJiuList.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报号旧未维护" + strHaoJiu_name + "基础数据", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                    strHaoJiu_name = drHaoJiuList[0]["vcValue"].ToString();
                            }
                        }
                        drImport["vcHaoJiu"] = strHaoJiu_name;
                        #endregion
                        #region 旧型年限生产区分
                        string strOldProduction_name = dataTable.Rows[i]["vcOldProduction_name"].ToString().Trim();
                        drImport["vcOldProduction"] = strOldProduction_name;
                        #endregion
                        #region 实施时间
                        string strDebugTime = dataTable.Rows[i]["dDebugTime"].ToString().Trim();
                        if (strDebugTime != "")
                        {
                            if (!ComFunction.CheckYMonth(strDebugTime))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报实施年月格式不正确", i + datarow);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strDebugTime = Convert.ToDateTime(strDebugTime).ToString("yyyy-MM-01");
                        }
                        drImport["dDebugTime"] = strDebugTime;
                        #endregion
                        //供应商
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"].ToString().Trim();
                        drImport["vcSupplierId"] = strSupplierId;
                        #region 供应商开始时间
                        string strSupplierFromTime = dataTable.Rows[i]["dSupplierFromTime"].ToString().Trim();
                        if (strSupplierFromTime != "")
                        {
                            if (!ComFunction.CheckDate(strSupplierFromTime))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报供应商开始时间格式不正确", i + datarow);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSupplierFromTime = Convert.ToDateTime(strSupplierFromTime).ToString("yyyy-MM-dd");
                        }
                        drImport["dSupplierFromTime"] = strSupplierFromTime;
                        #endregion
                        #region 供应商结束时间
                        string strSupplierToTime = dataTable.Rows[i]["dSupplierToTime"].ToString().Trim();
                        if (strSupplierToTime != "")
                        {
                            if (!ComFunction.CheckDate(strSupplierToTime))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报供应商结束时间格式不正确", i + datarow);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSupplierToTime = Convert.ToDateTime(strSupplierToTime).ToString("yyyy-MM-dd");
                        }
                        drImport["dSupplierToTime"] = strSupplierToTime;
                        #endregion
                        //供应商名称
                        string strSupplierName = dataTable.Rows[i]["vcSupplierName"].ToString();
                        if (strType == "新增" || strType == "修改")
                        {
                            if (strSupplierName.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护供应商名称", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                        drImport["vcSupplierName"] = strSupplierName;
                        #region 工区
                        string strSupplierPlantLinId_ed = "0";
                        string strSupplierPlant_ed = dataTable.Rows[i]["SupplierPlant_ed"].ToString().Trim();
                        string strSupplierPlantFromTime_ed = dataTable.Rows[i]["SupplierPlantFromTime_ed"].ToString().Trim();
                        if (strSupplierPlantFromTime_ed == "")
                            strSupplierPlantFromTime_ed = strFromTime;
                        else
                        {
                            if (!ComFunction.CheckDate(strSupplierPlantFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-开始时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSupplierPlantFromTime_ed = Convert.ToDateTime(strSupplierPlantFromTime_ed).ToString("yyyy-MM-dd");
                        }
                        string strSupplierPlantToTime_ed = dataTable.Rows[i]["SupplierPlantToTime_ed"].ToString().Trim();
                        if (strSupplierPlantToTime_ed == "")
                            strSupplierPlantToTime_ed = strToTime;
                        else
                        {
                            if (!ComFunction.CheckDate(strSupplierPlantToTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-结束时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSupplierPlantToTime_ed = Convert.ToDateTime(strSupplierPlantToTime_ed).ToString("yyyy-MM-dd");
                        }
                        if (!(strSupplierPlant_ed == ""))
                        {
                            bool bCheck_sp = false;
                            DataTable dtCheckTime = fs0603_DataAccess.getEditLoadInfo("SupplierPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                            for (int ck = 0; ck < dtCheckTime.Rows.Count; ck++)
                            {
                                string strSupplierPlant_ed_check = dtCheckTime.Rows[ck]["vcSupplierPlant"].ToString();
                                string strSupplierPlantFromTime_ed_check = Convert.ToDateTime(dtCheckTime.Rows[ck]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                                string strSupplierPlantToTime_ed_check = Convert.ToDateTime(dtCheckTime.Rows[ck]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                                if (strSupplierPlant_ed == strSupplierPlant_ed_check
                                    && strSupplierPlantFromTime_ed == strSupplierPlantFromTime_ed_check
                                    && strSupplierPlantToTime_ed == strSupplierPlantToTime_ed_check)
                                {
                                    bCheck_sp = true;
                                    break;
                                }
                            }
                            if (!bCheck_sp)
                            {
                                strSupplierPlantLinId_ed = "-1";
                                string strSupplierPlantLinId_before = "";
                                string strSupplierPlantFromTime_before = "";
                                if (dtCheckTime.Rows.Count > 0)
                                {
                                    strSupplierPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                    strSupplierPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                    if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(strSupplierPlantFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                }
                            }
                        }
                        drImport["SupplierPlantLinId_ed"] = strSupplierPlantLinId_ed;
                        drImport["SupplierPlant_ed"] = strSupplierPlant_ed;
                        drImport["SupplierPlantFromTime_ed"] = strSupplierPlantFromTime_ed;
                        drImport["SupplierPlantToTime_ed"] = strSupplierPlantToTime_ed;
                        #endregion
                        //出荷场
                        string strSupplierPlace = dataTable.Rows[i]["vcSupplierPlace"].ToString();
                        drImport["vcSupplierPlace"] = strSupplierPlace;
                        #region 收容数
                        string strBoxLinId_ed = "0";
                        string strBoxPackingQty_ed = dataTable.Rows[i]["BoxPackingQty_ed"].ToString().Trim();
                        string strBoxFromTime_ed = dataTable.Rows[i]["BoxFromTime_ed"].ToString().Trim();
                        if (strBoxFromTime_ed == "")
                            strBoxFromTime_ed = strFromTime;
                        else
                        {
                            if (!ComFunction.CheckDate(strBoxFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-开始时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strBoxFromTime_ed = Convert.ToDateTime(strBoxFromTime_ed).ToString("yyyy-MM-dd");
                        }
                        string strBoxToTime_ed = dataTable.Rows[i]["BoxToTime_ed"].ToString().Trim();
                        if (strBoxToTime_ed == "")
                            strBoxToTime_ed = strToTime;
                        else
                        {
                            if (!ComFunction.CheckDate(strBoxToTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-结束时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strBoxToTime_ed = Convert.ToDateTime(strBoxToTime_ed).ToString("yyyy-MM-dd");
                        }
                        string strBoxType_ed = dataTable.Rows[i]["BoxType_ed"].ToString().Trim();
                        string strBoxLength_ed = dataTable.Rows[i]["BoxLength_ed"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["BoxLength_ed"].ToString().Trim();
                        if (!ComFunction.CheckDecimal(strBoxLength_ed))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种长格式不正确", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                        string strBoxWidth_ed = dataTable.Rows[i]["BoxWidth_ed"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["BoxWidth_ed"].ToString().Trim();
                        if (!ComFunction.CheckDecimal(strBoxWidth_ed))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种宽格式不正确", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                        string strBoxHeight_ed = dataTable.Rows[i]["BoxHeight_ed"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["BoxHeight_ed"].ToString().Trim();
                        if (!ComFunction.CheckDecimal(strBoxHeight_ed))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种高格式不正确", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                        string strBoxVolume_ed = "";
                        if (ComFunction.CheckDecimal(strBoxLength_ed) && ComFunction.CheckDecimal(strBoxWidth_ed) && ComFunction.CheckDecimal(strBoxHeight_ed))
                        {
                            strBoxVolume_ed = ((Convert.ToInt32(strBoxLength_ed) / 1000.0) * (Convert.ToInt32(strBoxWidth_ed) / 1000.0) * (Convert.ToInt32(strBoxHeight_ed) / 1000.0)).ToString("#.0000");
                        }
                        if (strBoxPackingQty_ed != "" && strBoxPackingQty_ed != "0")
                        {
                            bool bCheck_pq = false;
                            DataTable dtCheckTime = fs0603_DataAccess.getEditLoadInfo("PackingQtyEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                            for (int ck = 0; ck < dtCheckTime.Rows.Count; ck++)
                            {
                                string strBoxPackingQty_ed_check = dtCheckTime.Rows[ck]["iPackingQty"].ToString();
                                string strBoxType_ed_check = dtCheckTime.Rows[ck]["vcBoxType"].ToString();
                                string strBoxFromTime_ed_check = Convert.ToDateTime(dtCheckTime.Rows[ck]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                                string strBoxToTime_ed_check = Convert.ToDateTime(dtCheckTime.Rows[ck]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                                if (strBoxPackingQty_ed == strBoxPackingQty_ed_check
                                    && strBoxType_ed == strBoxType_ed_check
                                    && strBoxFromTime_ed == strBoxFromTime_ed_check
                                    && strBoxToTime_ed == strBoxToTime_ed_check)
                                {
                                    bCheck_pq = true;
                                    break;
                                }
                            }
                            if (!bCheck_pq)
                            {
                                strBoxLinId_ed = "-1";
                                string strBoxLinId_before = "";
                                string strBoxFromTime_before = "";
                                if (dtCheckTime.Rows.Count > 0)
                                {
                                    strBoxLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                    strBoxFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                    if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(strBoxFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【收容数有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                }
                            }
                        }
                        drImport["BoxLinId_ed"] = strBoxLinId_ed;
                        drImport["BoxPackingQty_ed"] = strBoxPackingQty_ed == "" ? "0" : strBoxPackingQty_ed;
                        drImport["BoxFromTime_ed"] = strBoxFromTime_ed;
                        drImport["BoxToTime_ed"] = strBoxToTime_ed;
                        drImport["BoxType_ed"] = strBoxType_ed;
                        drImport["BoxLength_ed"] = strBoxLength_ed == "" ? "0" : strBoxLength_ed;
                        drImport["BoxWidth_ed"] = strBoxWidth_ed == "" ? "0" : strBoxWidth_ed;
                        drImport["BoxHeight_ed"] = strBoxHeight_ed == "" ? "0" : strBoxHeight_ed;
                        drImport["BoxVolume_ed"] = strBoxVolume_ed == "" ? "0" : strBoxVolume_ed;
                        #endregion
                        #region 受入
                        string strSufferInLinId_ed = "0";
                        string strSufferIn_ed = dataTable.Rows[i]["SufferIn_ed"].ToString().Trim();
                        string strSufferInFromTime_ed = dataTable.Rows[i]["SufferInFromTime_ed"].ToString().Trim();
                        if (strSufferInFromTime_ed == "")
                            strSufferInFromTime_ed = strFromTime;
                        else
                        {
                            if (!ComFunction.CheckDate(strSufferInFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-开始时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSufferInFromTime_ed = Convert.ToDateTime(strSufferInFromTime_ed).ToString("yyyy-MM-dd");
                        }
                        string strSufferInToTime_ed = dataTable.Rows[i]["SufferInToTime_ed"].ToString().Trim();
                        if (strSufferInToTime_ed == "")
                            strSufferInToTime_ed = strToTime;
                        else
                        {
                            if (!ComFunction.CheckDate(strSufferInToTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-结束时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSufferInToTime_ed = Convert.ToDateTime(strSufferInToTime_ed).ToString("yyyy-MM-dd");
                        }
                        if (!(strSufferIn_ed == ""))
                        {
                            bool bCheck_si = false;
                            DataTable dtCheckTime = fs0603_DataAccess.getEditLoadInfo("SufferInEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                            for (int ck = 0; ck < dtCheckTime.Rows.Count; ck++)
                            {
                                string strSufferIn_ed_check = dtCheckTime.Rows[ck]["vcSufferIn"].ToString();
                                string strSufferInFromTime_ed_check = Convert.ToDateTime(dtCheckTime.Rows[ck]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                                string strSufferInToTime_ed_check = Convert.ToDateTime(dtCheckTime.Rows[ck]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                                if (strSufferIn_ed == strSufferIn_ed_check
                                    && strSufferInFromTime_ed == strSufferInFromTime_ed_check
                                    && strSufferInToTime_ed == strSufferInToTime_ed_check)
                                {
                                    bCheck_si = true;
                                    break;
                                }
                            }
                            if (!bCheck_si)
                            {
                                strSufferInLinId_ed = "-1";
                                string strSufferInLinId_before = "";
                                string strSufferInFromTime_before = "";
                                if (dtCheckTime.Rows.Count > 0)
                                {
                                    strSufferInLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                    strSufferInFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                    if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(strSufferInFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【受入有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                }
                            }
                        }
                        drImport["SufferIn_ed"] = strSufferIn_ed;
                        drImport["SufferInLinId_ed"] = strSufferInLinId_ed;
                        drImport["SufferInFromTime_ed"] = strSufferInFromTime_ed;
                        drImport["SufferInToTime_ed"] = strSufferInToTime_ed;
                        #endregion
                        //发注工厂
                        string strOrderPlant_name = dataTable.Rows[i]["vcOrderPlant_name"].ToString();
                        drImport["vcOrderPlant"] = strOrderPlant_name;
                        //内制工程
                        string strInteriorProject = dataTable.Rows[i]["vcInteriorProject"].ToString();
                        drImport["vcInteriorProject"] = strInteriorProject;
                        //通过工程
                        string strPassProject = dataTable.Rows[i]["vcPassProject"].ToString();
                        drImport["vcPassProject"] = strPassProject;
                        //前工程
                        string strFrontProject = dataTable.Rows[i]["vcFrontProject"].ToString();
                        drImport["vcFrontProject"] = strFrontProject;
                        #region 前工程通过时间
                        string strFrontProjectTime = dataTable.Rows[i]["dFrontProjectTime"].ToString().Trim();
                        if (strFrontProjectTime != "")
                        {
                            if (!ComFunction.CheckDate(strFrontProjectTime))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报前工程通过时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strFrontProjectTime = Convert.ToDateTime(strFrontProjectTime).ToString("yyyy-MM-dd HH:mm");
                        }
                        drImport["dFrontProjectTime"] = strFrontProjectTime;
                        #endregion
                        #region 自工程出货时间
                        string strShipmentTime = dataTable.Rows[i]["dShipmentTime"].ToString().Trim();
                        if (strShipmentTime != "")
                        {
                            if (!ComFunction.CheckDate(strShipmentTime))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报自工程出货时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strShipmentTime = Convert.ToDateTime(strShipmentTime).ToString("yyyy-MM-dd HH:mm");
                        }
                        drImport["dShipmentTime"] = strShipmentTime;
                        #endregion
                        //照片
                        string strPartImage = dataTable.Rows[i]["vcPartImage"].ToString();
                        drImport["vcPartImage"] = strPartImage;
                        #region 单据区分
                        string strBillType_name = dataTable.Rows[i]["vcBillType_name"].ToString().Trim();
                        if (strBillType_name != "")
                        {
                            DataRow[] drBillTypeList = dtBillTypeList.Select("vcName='" + strBillType_name + "'");
                            if (drBillTypeList.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报单据区分未维护" + strBillType_name + "基础数据", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strBillType_name = drBillTypeList[0]["vcValue"].ToString();
                        }
                        drImport["vcBillType"] = strBillType_name;
                        #endregion
                        //备注1
                        string strRemark1 = dataTable.Rows[i]["vcRemark1"].ToString();
                        drImport["vcRemark1"] = strRemark1;
                        //备注2
                        string strRemark2 = dataTable.Rows[i]["vcRemark2"].ToString();
                        drImport["vcRemark2"] = strRemark2;
                        #region 订货方式
                        string strOrderingMethod_name = dataTable.Rows[i]["vcOrderingMethod_name"].ToString().Trim();
                        if (strOrderingMethod_name != "")
                        {
                            DataRow[] drOrderingMethodList = dtOrderingMethodList.Select("vcName='" + strOrderingMethod_name + "'");
                            if (drOrderingMethodList.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报订货方式未维护" + strOrderingMethod_name + "基础数据", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strOrderingMethod_name = drOrderingMethodList[0]["vcValue"].ToString();
                        }
                        drImport["vcOrderingMethod"] = strOrderingMethod_name;
                        #endregion
                        #region 强制订货
                        string strMandOrder_name = dataTable.Rows[i]["vcMandOrder_name"].ToString().Trim();
                        if (strMandOrder_name != "")
                        {
                            DataRow[] drMandOrderList = dtMandOrderList.Select("vcName='" + strMandOrder_name + "'");
                            if (drMandOrderList.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报强制订货未维护" + strMandOrder_name + "基础数据", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strMandOrder_name = drMandOrderList[0]["vcValue"].ToString();
                        }
                        drImport["vcMandOrder"] = strMandOrder_name;
                        #endregion
                        #region 供应商包装
                        string strSupplierPacking_name = dataTable.Rows[i]["vcSupplierPacking_name"].ToString().Trim();
                        if (strSupplierPacking_name != "")
                        {
                            DataRow[] drMandOrderList = dtMandOrderList.Select("vcName='" + strSupplierPacking_name + "'");
                            if (drMandOrderList.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报供应商包装未维护" + strSupplierPacking_name + "基础数据", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strSupplierPacking_name = drMandOrderList[0]["vcValue"].ToString();
                        }
                        drImport["vcSupplierPacking"] = strSupplierPacking_name;
                        #endregion
                        dtImport.Rows.Add(drImport);
                    }
                    if (dtMessage == null || dtMessage.Rows.Count == 0)
                    {
                        bReault = true;
                        return dtImport;
                    }
                    else
                    {
                        bReault = false;
                        return null;
                    }
                }
                else
                {
                    bReault = false;
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkFileInfo(DataTable dataTable, string strshoupei, string[,] Header, int heardrow, int datarow, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                int count = dataTable.Rows.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (dataTable.Rows[i]["vcType"].ToString().Trim() == "")
                        dataTable.Rows.RemoveAt(i);
                }
                DataTable dtSPInfo = getChildInfo("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", true, "", "", "", "", strshoupei);
                DataTable dtImport = dtSPInfo.Clone();
                dtImport.Columns.Add("vcType");
                #region 检验数据重复性及数据格式
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strType = dataTable.Rows[i]["vcType"].ToString().Trim();
                    string strPackingPlant = dataTable.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dataTable.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dataTable.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dataTable.Rows[i]["vcSupplierId"].ToString().Trim();
                    string strLINID = dataTable.Rows[i]["LINID"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["LINID"].ToString().Trim();
                    if (strPackingPlant == "" || strPartId == "" || strReceiver == "" || strSupplierId == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报主Key存在空值禁止操作", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strType == "新增")
                    {
                    }
                    if (strType == "修改" || strType == "删除")
                    {
                        if (strLINID == "0")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报隐藏ID列为空", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                }
                #endregion

                if (dtMessage == null || dtMessage.Rows.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow drImport = dtImport.NewRow();
                        string strType = dataTable.Rows[i]["vcType"].ToString().Trim();
                        drImport["vcType"] = strType;
                        //包装工厂
                        string strPackingPlant = dataTable.Rows[i]["vcPackingPlant"].ToString().Trim();
                        drImport["vcPackingPlant"] = strPackingPlant;
                        //品番
                        string strPartId = dataTable.Rows[i]["vcPartId"].ToString().Trim();
                        drImport["vcPartId"] = strPartId;
                        //收货方
                        string strReceiver = dataTable.Rows[i]["vcReceiver"].ToString().Trim();
                        drImport["vcReceiver"] = strReceiver;
                        #region 开始时间
                        string strFromTime = dataTable.Rows[i]["dFromTime"].ToString().Trim();
                        if (!ComFunction.CheckDate(strFromTime))
                        {
                            strFromTime = "";
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报开始时间格式不正确", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                            strFromTime = Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd");
                        drImport["dFromTime"] = strFromTime;
                        #endregion
                        #region 结束时间
                        string strToTime = dataTable.Rows[i]["dToTime"].ToString().Trim();
                        if (strToTime != "")
                        {
                            if (!ComFunction.CheckDate(strFromTime))
                            {
                                strToTime = "";
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报结束时间格式不正确", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                                strToTime = Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd");
                        }
                        else
                            strToTime = "9999-12-31";
                        drImport["dToTime"] = strToTime;
                        if (strFromTime != ""
                                && strToTime != ""
                                && Convert.ToDateTime(strFromTime) > Convert.ToDateTime(strToTime))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报开始时间大于结束时间", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                        #endregion
                        //供应商
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"].ToString().Trim();
                        drImport["vcSupplierId"] = strSupplierId;
                        #region 工区
                        if (strshoupei == "gq")
                        {
                            string strLINID = dataTable.Rows[i]["LINID"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["LINID"].ToString().Trim();
                            string strSupplierPlant_ed = dataTable.Rows[i]["SupplierPlant_ed"].ToString().Trim();
                            string strSupplierPlantFromTime_ed = dataTable.Rows[i]["SupplierPlantFromTime_ed"].ToString().Trim();
                            string strSupplierPlantToTime_ed = dataTable.Rows[i]["SupplierPlantToTime_ed"].ToString().Trim();
                            if (strType == "新增")
                            {
                                if (strSupplierPlantFromTime_ed == "")
                                    strSupplierPlantFromTime_ed = Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd");
                                else
                                {
                                    if (!ComFunction.CheckDate(strSupplierPlantFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-开始时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSupplierPlantFromTime_ed = Convert.ToDateTime(strSupplierPlantFromTime_ed).ToString("yyyy-MM-dd");
                                }
                                if (strSupplierPlantToTime_ed == "")
                                    strSupplierPlantToTime_ed = Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd");
                                else
                                {
                                    if (!ComFunction.CheckDate(strSupplierPlantToTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-结束时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSupplierPlantToTime_ed = Convert.ToDateTime(strSupplierPlantToTime_ed).ToString("yyyy-MM-dd");
                                }
                            }
                            if (strType == "修改")
                            {
                                if (strSupplierPlantFromTime_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-开始时间不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!ComFunction.CheckDate(strSupplierPlantFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-开始时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSupplierPlantFromTime_ed = Convert.ToDateTime(strSupplierPlantFromTime_ed).ToString("yyyy-MM-dd");
                                }
                                if (strSupplierPlantToTime_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-结束时间不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!ComFunction.CheckDate(strSupplierPlantToTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-结束时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSupplierPlantToTime_ed = Convert.ToDateTime(strSupplierPlantToTime_ed).ToString("yyyy-MM-dd");
                                }
                            }
                            if (strType == "新增" || strType == "修改")
                            {
                                if (strSupplierPlantFromTime_ed != ""
                                && strSupplierPlantToTime_ed != ""
                                && Convert.ToDateTime(strSupplierPlantFromTime_ed) > Convert.ToDateTime(strSupplierPlantToTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区-开始时间大于工区-开始时间", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (strSupplierPlant_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                            drImport["SupplierPlantLinId_ed"] = strLINID;
                            drImport["SupplierPlant_ed"] = strSupplierPlant_ed;
                            drImport["SupplierPlantFromTime_ed"] = strSupplierPlantFromTime_ed;
                            drImport["SupplierPlantToTime_ed"] = strSupplierPlantToTime_ed;
                        }
                        #endregion

                        #region 收容数
                        if (strshoupei == "hz")
                        {
                            string strLINID = dataTable.Rows[i]["LINID"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["LINID"].ToString().Trim();
                            string strBoxPackingQty_ed = dataTable.Rows[i]["BoxPackingQty_ed"].ToString().Trim();
                            string strBoxFromTime_ed = dataTable.Rows[i]["BoxFromTime_ed"].ToString().Trim();
                            string strBoxToTime_ed = dataTable.Rows[i]["BoxToTime_ed"].ToString().Trim();
                            string strBoxType_ed = dataTable.Rows[i]["BoxType_ed"].ToString().Trim();
                            string strBoxLength_ed = dataTable.Rows[i]["BoxLength_ed"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["BoxLength_ed"].ToString().Trim();
                            string strBoxWidth_ed = dataTable.Rows[i]["BoxWidth_ed"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["BoxWidth_ed"].ToString().Trim();
                            string strBoxHeight_ed = dataTable.Rows[i]["BoxHeight_ed"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["BoxHeight_ed"].ToString().Trim();
                            string strBoxVolume_ed = dataTable.Rows[i]["BoxVolume_ed"].ToString().Trim();
                            if (strType == "新增")
                            {
                                if (strBoxFromTime_ed == "")
                                    strBoxFromTime_ed =Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd");
                                else
                                {
                                    if (!ComFunction.CheckDate(strBoxFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-开始时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strBoxFromTime_ed = Convert.ToDateTime(strBoxFromTime_ed).ToString("yyyy-MM-dd");
                                }
                                if (strBoxToTime_ed == "")
                                    strBoxToTime_ed = Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd");
                                else
                                {
                                    if (!ComFunction.CheckDate(strBoxToTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-结束时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strBoxToTime_ed = Convert.ToDateTime(strBoxToTime_ed).ToString("yyyy-MM-dd");
                                }
                            }
                            if (strType == "修改")
                            {
                                if (strBoxFromTime_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-开始时间不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!ComFunction.CheckDate(strBoxFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-开始时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strBoxFromTime_ed = Convert.ToDateTime(strBoxFromTime_ed).ToString("yyyy-MM-dd");
                                }
                                if (strBoxToTime_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-结束时间不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!ComFunction.CheckDate(strBoxToTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数-结束时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strBoxToTime_ed = Convert.ToDateTime(strBoxToTime_ed).ToString("yyyy-MM-dd");
                                }
                            }
                            if (strType == "新增" || strType == "修改")
                            {
                                if (strBoxFromTime_ed != ""
                                && strBoxToTime_ed != ""
                                && Convert.ToDateTime(strBoxFromTime_ed) > Convert.ToDateTime(strBoxToTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报荷姿-开始时间大于工区-开始时间", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (strBoxPackingQty_ed == "" || strBoxPackingQty_ed == "0")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (!ComFunction.CheckDecimal(strBoxPackingQty_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数格式不正确", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (strBoxType_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种信息不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (!ComFunction.CheckDecimal(strBoxLength_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种长格式不正确", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (!ComFunction.CheckDecimal(strBoxWidth_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种宽格式不正确", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (!ComFunction.CheckDecimal(strBoxHeight_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报箱种高格式不正确", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (ComFunction.CheckDecimal(strBoxLength_ed) && ComFunction.CheckDecimal(strBoxWidth_ed) && ComFunction.CheckDecimal(strBoxHeight_ed))
                                {
                                    strBoxVolume_ed = ((Convert.ToInt32(strBoxLength_ed) / 1000.0) * (Convert.ToInt32(strBoxWidth_ed) / 1000.0) * (Convert.ToInt32(strBoxHeight_ed) / 1000.0)).ToString("#.0000");
                                }
                            }
                            drImport["BoxLinId_ed"] = strLINID;
                            drImport["BoxPackingQty_ed"] = strBoxPackingQty_ed == "" ? "0" : strBoxPackingQty_ed;
                            drImport["BoxFromTime_ed"] = strBoxFromTime_ed;
                            drImport["BoxToTime_ed"] = strBoxToTime_ed;
                            drImport["BoxType_ed"] = strBoxType_ed;
                            drImport["BoxLength_ed"] = strBoxLength_ed == "" ? "0" : strBoxLength_ed;
                            drImport["BoxWidth_ed"] = strBoxWidth_ed == "" ? "0" : strBoxWidth_ed;
                            drImport["BoxHeight_ed"] = strBoxHeight_ed == "" ? "0" : strBoxHeight_ed;
                            drImport["BoxVolume_ed"] = strBoxVolume_ed == "" ? "0" : strBoxVolume_ed;
                        }

                        #endregion

                        #region 受入
                        if (strshoupei == "sr")
                        {
                            string strLINID = dataTable.Rows[i]["LINID"].ToString().Trim() == "" ? "0" : dataTable.Rows[i]["LINID"].ToString().Trim();
                            string strSufferIn_ed = dataTable.Rows[i]["SufferIn_ed"].ToString().Trim();
                            string strSufferInFromTime_ed = dataTable.Rows[i]["SufferInFromTime_ed"].ToString().Trim();
                            string strSufferInToTime_ed = dataTable.Rows[i]["SufferInToTime_ed"].ToString().Trim();
                            if (strType == "新增")
                            {
                                if (strSufferInFromTime_ed == "")
                                    strSufferInFromTime_ed =  Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd");
                                else
                                {
                                    if (!ComFunction.CheckDate(strSufferInFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-开始时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSufferInFromTime_ed = Convert.ToDateTime(strSufferInFromTime_ed).ToString("yyyy-MM-dd");
                                }
                                if (strSufferInToTime_ed == "")
                                    strSufferInToTime_ed =  Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd");
                                else
                                {
                                    if (!ComFunction.CheckDate(strSufferInToTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-结束时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSufferInToTime_ed = Convert.ToDateTime(strSufferInToTime_ed).ToString("yyyy-MM-dd");
                                }
                            }
                            if (strType == "修改")
                            {
                                if (strSufferInFromTime_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-开始时间不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!ComFunction.CheckDate(strSufferInFromTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-开始时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSufferInFromTime_ed = Convert.ToDateTime(strSufferInFromTime_ed).ToString("yyyy-MM-dd");
                                }
                                if (strSufferInToTime_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-结束时间不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!ComFunction.CheckDate(strSufferInToTime_ed))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-结束时间格式不正确", i + 1);
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                        strSufferInToTime_ed = Convert.ToDateTime(strSufferInToTime_ed).ToString("yyyy-MM-dd");
                                }
                            }
                            if (strType == "新增" || strType == "修改")
                            {
                                if (strSufferInFromTime_ed != ""
                                && strSufferInToTime_ed != ""
                                && Convert.ToDateTime(strSufferInFromTime_ed) > Convert.ToDateTime(strSufferInToTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入-开始时间大于工区-开始时间", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (strSufferIn_ed == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入不能为空", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                            drImport["SufferInLinId_ed"] = strLINID;
                            drImport["SufferIn_ed"] = strSufferIn_ed;
                            drImport["SufferInFromTime_ed"] = strSufferInFromTime_ed;
                            drImport["SufferInToTime_ed"] = strSufferInToTime_ed;
                        }
                        #endregion
                        dtImport.Rows.Add(drImport);
                    }
                    if (dtMessage == null || dtMessage.Rows.Count == 0)
                    {
                        bReault = true;
                        return dtImport;
                    }
                    else
                    {
                        bReault = false;
                        return null;
                    }
                }
                else
                {
                    bReault = false;
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkDataInfo(List<Dictionary<string, Object>> listInfoData, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                //处理List集合
                listInfoData = setNullData(listInfoData);
                bReault = true;
                DataTable dtSPInfo = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", true, "", "", "", "");
                DataTable dtImport = dtSPInfo.Clone();
                dtImport.Columns.Add("vcType");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strType = "";
                    DataRow drImport = dtImport.NewRow();
                    drImport["bAddFlag"] = listInfoData[i]["bAddFlag"].ToString();
                    drImport["bModFlag"] = listInfoData[i]["bModFlag"].ToString();
                    if ((bool)listInfoData[i]["bAddFlag"] == true)
                    {
                        strType = "新增";
                        drImport["vcType"] = strType;
                        drImport["LinId"] = "-1";
                    }
                    if ((bool)listInfoData[i]["bAddFlag"] == false && (bool)listInfoData[i]["bModFlag"] == true)
                    {
                        strType = "修改";
                        drImport["vcType"] = strType;
                        drImport["LinId"] = listInfoData[i]["LinId"].ToString();
                    }
                    string strPackingPlant = listInfoData[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = listInfoData[i]["vcPartId"].ToString().Trim();
                    string strReceiver = listInfoData[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = listInfoData[i]["vcSupplierId"].ToString().Trim();
                    drImport["dSyncTime"] = listInfoData[i]["dSyncTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dSyncTime"].ToString()).ToString("yyyy-MM-dd");
                    drImport["vcChanges"] = "";
                    if (strPackingPlant != "")
                        drImport["vcPackingPlant"] = strPackingPlant;
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护包装工场", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    if (strPartId != "")
                    {
                        if (strPartId.Length != 12)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报品番不足12位", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        drImport["vcPartId"] = strPartId;
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护品番", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }

                    drImport["vcPartENName"] = listInfoData[i]["vcPartENName"].ToString();
                    if (listInfoData[i]["vcPartENName"].ToString().Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护英文品名", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["vcCarfamilyCode"] = listInfoData[i]["vcCarfamilyCode"].ToString();
                    if (listInfoData[i]["vcCarfamilyCode"].ToString().Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护车型", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    if (strReceiver != "")
                        drImport["vcReceiver"] = strReceiver;
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护收货发", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string strFromTime = listInfoData[i]["dFromTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                    drImport["dFromTime"] = strFromTime;
                    if (strFromTime == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护开始使用时间", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string strToTime = listInfoData[i]["dToTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                    drImport["dToTime"] = strToTime;
                    if (strToTime == "")
                        drImport["dToTime"] = "9999-12-31";
                    if (strFromTime != "" && Convert.ToDateTime(drImport["dFromTime"].ToString()) > Convert.ToDateTime(drImport["dToTime"].ToString()))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报所维护的开始使用时间大于结束时间", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["vcPartId_Replace"] = listInfoData[i]["vcPartId_Replace"].ToString();
                    drImport["vcInOut"] = listInfoData[i]["vcInOut"].ToString();
                    if (listInfoData[i]["vcInOut"].ToString().Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护内外", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["vcOESP"] = listInfoData[i]["vcOESP"].ToString();
                    if (listInfoData[i]["vcOESP"].ToString().Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护OE=SP", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["vcHaoJiu"] = listInfoData[i]["vcHaoJiu"].ToString();
                    if (listInfoData[i]["vcHaoJiu"].ToString().Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护号旧", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["vcOldProduction"] = listInfoData[i]["vcOldProduction"].ToString();
                    drImport["dDebugTime"] = listInfoData[i]["dDebugTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dDebugTime"].ToString()).ToString("yyyy-MM") + "-01";
                    if (strSupplierId != "")
                        drImport["vcSupplierId"] = strSupplierId;
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护供应商", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["dSupplierFromTime"] = listInfoData[i]["dSupplierFromTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dSupplierFromTime"].ToString()).ToString("yyyy-MM-dd");
                    drImport["dSupplierToTime"] = listInfoData[i]["dSupplierToTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dSupplierToTime"].ToString()).ToString("yyyy-MM-dd");
                    if (drImport["dSupplierFromTime"].ToString() != ""
                        && drImport["dSupplierToTime"].ToString() != ""
                        && Convert.ToDateTime(drImport["dSupplierFromTime"].ToString()) > Convert.ToDateTime(drImport["dSupplierToTime"].ToString()))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报所维护的供应商开始使用时间大于结束时间", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    drImport["vcSupplierName"] = listInfoData[i]["vcSupplierName"].ToString();
                    if (listInfoData[i]["vcSupplierName"].ToString().Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报未维护供应商名称", i + 1);
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    if ((bool)listInfoData[i]["bAddFlag"] == true)
                    {
                        DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + listInfoData[i]["vcPackingPlant"].ToString() + "' and vcPartId='" + listInfoData[i]["vcPartId"].ToString() + "' and vcReceiver='" + listInfoData[i]["vcReceiver"].ToString() + "' and vcSupplierId='" + listInfoData[i]["vcSupplierId"].ToString() + "'");
                        if (drSPInfo.Length != 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报存在重复Key禁止操作", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                    #region 验证工区
                    string strSupplierPlant = listInfoData[i]["vcSupplierPlant"].ToString();
                    string strSupplierPlantLinId_ed = listInfoData[i]["SupplierPlantLinId_ed"].ToString();
                    string strSupplierPlant_ed = listInfoData[i]["SupplierPlant_ed"].ToString();
                    string strSupplierPlantFromTime_ed = listInfoData[i]["SupplierPlantFromTime_ed"].ToString();
                    string strSupplierPlantToTime_ed = listInfoData[i]["SupplierPlantToTime_ed"].ToString();
                    if (strSupplierPlantLinId_ed == "-1")//修改新增
                    {
                        string strSupplierPlantLinId_before = "";
                        string strSupplierPlantFromTime_before = "";
                        DataTable dtCheckTime_sp = getEditLoadInfo("SupplierPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime_sp.Rows.Count > 0)
                        {
                            strSupplierPlantLinId_before = dtCheckTime_sp.Rows[dtCheckTime_sp.Rows.Count - 1]["LinId"].ToString();
                            strSupplierPlantFromTime_before = dtCheckTime_sp.Rows[dtCheckTime_sp.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(strSupplierPlantFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                    drImport["vcSupplierPlant"] = (strSupplierPlant == "待维护" || strSupplierPlant == "") ? "" : strSupplierPlant;
                    drImport["SupplierPlantLinId_ed"] = strSupplierPlantLinId_ed == "" ? "0" : strSupplierPlantLinId_ed;
                    drImport["SupplierPlant_ed"] = strSupplierPlant_ed;
                    drImport["SupplierPlantFromTime_ed"] = strSupplierPlantFromTime_ed;
                    drImport["SupplierPlantToTime_ed"] = strSupplierPlantToTime_ed;
                    #endregion
                    drImport["vcSupplierPlace"] = listInfoData[i]["vcSupplierPlace"].ToString();
                    #region 验证收容数
                    string strBoxPackingQty = listInfoData[i]["iPackingQty"].ToString();
                    string strBoxType = listInfoData[i]["vcBoxType"].ToString();
                    string strLength = listInfoData[i]["iLength"].ToString();
                    string strWidth = listInfoData[i]["iWidth"].ToString();
                    string strHeight = listInfoData[i]["iHeight"].ToString();
                    string strVolume = listInfoData[i]["iVolume"].ToString();
                    string strBoxLinId_ed = listInfoData[i]["BoxLinId_ed"].ToString();
                    string strBoxPackingQty_ed = listInfoData[i]["BoxPackingQty_ed"].ToString();
                    string strBoxFromTime_ed = listInfoData[i]["BoxFromTime_ed"].ToString();
                    string strBoxToTime_ed = listInfoData[i]["BoxToTime_ed"].ToString();
                    string strBoxType_ed = listInfoData[i]["BoxType_ed"].ToString();
                    string strBoxLength_ed = listInfoData[i]["BoxLength_ed"].ToString();
                    string strBoxWidth_ed = listInfoData[i]["BoxWidth_ed"].ToString();
                    string strBoxHeight_ed = listInfoData[i]["BoxHeight_ed"].ToString();
                    string strBoxVolume_ed = listInfoData[i]["BoxVolume_ed"].ToString();
                    if (strBoxLinId_ed == "-1")
                    {
                        string strBoxLinId_before = "";
                        string strBoxFromTime_before = "";
                        DataTable dtCheckTime_pq = getEditLoadInfo("PackingQtyEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime_pq.Rows.Count > 0)
                        {
                            strBoxLinId_before = dtCheckTime_pq.Rows[dtCheckTime_pq.Rows.Count - 1]["LinId"].ToString();
                            strBoxFromTime_before = dtCheckTime_pq.Rows[dtCheckTime_pq.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(strBoxFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【收容数有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                    drImport["iPackingQty"] = (strBoxPackingQty == "待维护" || strBoxPackingQty == "") ? "0" : strBoxPackingQty;
                    drImport["vcBoxType"] = strBoxType;
                    drImport["iLength"] = strLength == "" ? "0" : strLength;
                    drImport["iWidth"] = strWidth == "" ? "0" : strWidth;
                    drImport["iHeight"] = strHeight == "" ? "0" : strHeight;
                    drImport["iVolume"] = strVolume == "" ? "0" : strVolume;
                    drImport["BoxLinId_ed"] = strBoxLinId_ed == "" ? "0" : strBoxLinId_ed;
                    drImport["BoxPackingQty_ed"] = strBoxPackingQty_ed == "" ? "0" : strBoxPackingQty_ed;
                    drImport["BoxFromTime_ed"] = strBoxFromTime_ed;
                    drImport["BoxToTime_ed"] = strBoxToTime_ed;
                    drImport["BoxType_ed"] = strBoxType_ed;
                    drImport["BoxLength_ed"] = strBoxLength_ed == "" ? "0" : strBoxLength_ed;
                    drImport["BoxWidth_ed"] = strBoxWidth_ed == "" ? "0" : strBoxWidth_ed;
                    drImport["BoxHeight_ed"] = strBoxHeight_ed == "" ? "0" : strBoxHeight_ed;
                    drImport["BoxVolume_ed"] = strBoxVolume_ed == "" ? "0" : strBoxVolume_ed;
                    #endregion

                    #region 验证受入
                    string strSufferIn = listInfoData[i]["vcSufferIn"].ToString();
                    string strSufferIn_ed = listInfoData[i]["SufferIn_ed"].ToString();
                    string strSufferInLinId_ed = listInfoData[i]["SufferInLinId_ed"].ToString();
                    string strSufferInFromTime_ed = listInfoData[i]["SufferInFromTime_ed"].ToString();
                    string strSufferInToTime_ed = listInfoData[i]["SufferInToTime_ed"].ToString();
                    if (strSufferInLinId_ed == "-1")
                    {
                        string strSufferInLinId_before = "";
                        string strSufferInFromTime_before = "";
                        DataTable dtCheckTime_si = getEditLoadInfo("SufferInEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime_si.Rows.Count > 0)
                        {
                            strSufferInLinId_before = dtCheckTime_si.Rows[dtCheckTime_si.Rows.Count - 1]["LinId"].ToString();
                            strSufferInFromTime_before = dtCheckTime_si.Rows[dtCheckTime_si.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(strSufferInFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【受入有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                    drImport["vcSufferIn"] = (strSufferIn == "待维护" || strSufferIn == "") ? "" : strSufferIn;
                    drImport["SufferIn_ed"] = strSufferIn_ed;
                    drImport["SufferInLinId_ed"] = strSufferInLinId_ed == "" ? "0" : strSufferInLinId_ed;
                    drImport["SufferInFromTime_ed"] = strSufferInFromTime_ed;
                    drImport["SufferInToTime_ed"] = strSufferInToTime_ed;
                    #endregion

                    #region 验证发注工厂
                    string strOrderPlant = listInfoData[i]["vcOrderPlant"].ToString();
                    drImport["vcOrderPlant"] = strOrderPlant;
                    #endregion

                    drImport["vcInteriorProject"] = listInfoData[i]["vcInteriorProject"].ToString();
                    drImport["vcPassProject"] = listInfoData[i]["vcPassProject"].ToString();
                    drImport["vcFrontProject"] = listInfoData[i]["vcFrontProject"].ToString();
                    drImport["dFrontProjectTime"] = listInfoData[i]["dFrontProjectTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dFrontProjectTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                    drImport["dShipmentTime"] = listInfoData[i]["dShipmentTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dShipmentTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                    drImport["vcPartImage"] = listInfoData[i]["vcPartImage"].ToString();
                    drImport["vcBillType"] = listInfoData[i]["vcBillType"].ToString();
                    drImport["vcRemark1"] = listInfoData[i]["vcRemark1"].ToString();
                    drImport["vcRemark2"] = listInfoData[i]["vcRemark2"].ToString();
                    drImport["vcOrderingMethod"] = listInfoData[i]["vcOrderingMethod"].ToString();
                    drImport["vcMandOrder"] = listInfoData[i]["vcMandOrder"].ToString();
                    drImport["vcSupplierPacking"] = listInfoData[i]["vcSupplierPacking"].ToString();
                    dtImport.Rows.Add(drImport);
                }
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSPInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            //用于检查变更情况
            DataTable dtOperCheck = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", true, "", "", "", "");
            //用于新增数据
            DataTable dtAddInfo = dtOperCheck.Clone();
            //用于修改数据--修改主数据表
            DataTable dtModInfo = dtOperCheck.Clone();
            //用于删除数据--删除主数据表
            DataTable dtDelInfo = dtOperCheck.Clone();
            //用于修改数据--修改工区数据表
            DataTable dtModInfo_SP = createTable("SP");
            //用于修改数据--修改收容数数据表
            DataTable dtModInfo_PQ = createTable("PQ");
            //用于修改数据--修改受入数据表
            DataTable dtModInfo_SI = createTable("SI");
            //用于修改数据--修改发注工厂数据表
            DataTable dtModInfo_OP = createTable("OP");
            //用于记录修改履历
            DataTable dtOperHistory = createTable("OH");
            //用于更新手配子表--主数据时间变更引起子表变更
            DataTable dtSPChild = createTable("SPChild");
            //用于强制入力维护--工区
            DataTable dtChildInfo_gq = createTable("SP");
            //用于强制入力维护--受入
            DataTable dtChildInfo_sr = createTable("SI");
            //用于强制入力维护--荷姿
            DataTable dtChildInfo_hz = createTable("PQ");
            for (int i = 0; i < dtImport.Rows.Count; i++)
            {
                string strType = dtImport.Rows[i]["vcType"].ToString();//新增、修改、删除
                if (strType == "新增")
                {
                    string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString().Trim();
                    #region 主表
                    DataRow dataRow_add = dtAddInfo.NewRow();
                    dataRow_add["dSyncTime"] = dtImport.Rows[i]["dSyncTime"].ToString();
                    dataRow_add["vcChanges"] = dtImport.Rows[i]["vcChanges"].ToString();
                    dataRow_add["vcPackingPlant"] = strPackingPlant;
                    dataRow_add["vcPartId"] = strPartId;
                    dataRow_add["vcPartENName"] = dtImport.Rows[i]["vcPartENName"].ToString();
                    dataRow_add["vcCarfamilyCode"] = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    dataRow_add["vcReceiver"] = strReceiver;
                    dataRow_add["dFromTime"] = dtImport.Rows[i]["dFromTime"].ToString();
                    dataRow_add["dToTime"] = dtImport.Rows[i]["dToTime"].ToString();
                    dataRow_add["vcPartId_Replace"] = dtImport.Rows[i]["vcPartId_Replace"].ToString();
                    dataRow_add["vcInOut"] = dtImport.Rows[i]["vcInOut"].ToString();
                    dataRow_add["vcOESP"] = dtImport.Rows[i]["vcOESP"].ToString();
                    dataRow_add["vcHaoJiu"] = dtImport.Rows[i]["vcHaoJiu"].ToString();
                    dataRow_add["vcOldProduction"] = dtImport.Rows[i]["vcOldProduction"].ToString();
                    dataRow_add["dDebugTime"] = dtImport.Rows[i]["dDebugTime"].ToString();
                    dataRow_add["vcSupplierId"] = strSupplierId;
                    dataRow_add["dSupplierFromTime"] = dtImport.Rows[i]["dSupplierFromTime"].ToString();
                    dataRow_add["dSupplierToTime"] = dtImport.Rows[i]["dSupplierToTime"].ToString();
                    dataRow_add["vcSupplierName"] = dtImport.Rows[i]["vcSupplierName"].ToString();
                    dataRow_add["vcInteriorProject"] = dtImport.Rows[i]["vcInteriorProject"].ToString();
                    dataRow_add["vcSupplierPlace"] = dtImport.Rows[i]["vcSupplierPlace"].ToString();
                    dataRow_add["vcPassProject"] = dtImport.Rows[i]["vcPassProject"].ToString();
                    dataRow_add["vcFrontProject"] = dtImport.Rows[i]["vcFrontProject"].ToString();
                    dataRow_add["dFrontProjectTime"] = dtImport.Rows[i]["dFrontProjectTime"].ToString();
                    dataRow_add["dShipmentTime"] = dtImport.Rows[i]["dShipmentTime"].ToString();
                    dataRow_add["vcPartImage"] = dtImport.Rows[i]["vcPartImage"].ToString();
                    dataRow_add["vcBillType"] = dtImport.Rows[i]["vcBillType"].ToString();
                    dataRow_add["vcRemark1"] = dtImport.Rows[i]["vcRemark1"].ToString();
                    dataRow_add["vcRemark2"] = dtImport.Rows[i]["vcRemark2"].ToString();
                    dataRow_add["vcOrderingMethod"] = dtImport.Rows[i]["vcOrderingMethod"].ToString();
                    dataRow_add["vcMandOrder"] = dtImport.Rows[i]["vcMandOrder"].ToString();
                    dataRow_add["vcSupplierPacking"] = dtImport.Rows[i]["vcSupplierPacking"].ToString();
                    dtAddInfo.Rows.Add(dataRow_add);
                    #endregion

                    #region 子表-工区
                    string strSupplierPlant = dtImport.Rows[i]["vcSupplierPlant"].ToString();
                    string strSupplierPlantLinId_ed = dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString();
                    //1.判断vcSupplierPlant是否需要修改
                    if (strSupplierPlantLinId_ed != "0" && strSupplierPlantLinId_ed == "-1")
                    {
                        string strSupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();

                        string strSupplierPlantLinId_before = "";
                        string strSupplierPlantToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("SupplierPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strSupplierPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            string strSupplierPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(strSupplierPlantFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            strSupplierPlantToTime_before = Convert.ToDateTime(strSupplierPlantFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_SupplierPlant表
                        #region AddNewRow
                        DataRow dataRow_add_SP = dtModInfo_SP.NewRow();
                        dataRow_add_SP["vcPackingPlant"] = strPackingPlant;
                        dataRow_add_SP["vcPartId"] = strPartId;
                        dataRow_add_SP["vcReceiver"] = strReceiver;
                        dataRow_add_SP["vcSupplierId"] = strSupplierId;
                        dataRow_add_SP["dFromTime"] = strSupplierPlantFromTime_ed;
                        dataRow_add_SP["dToTime"] = strSupplierPlantToTime_ed;
                        dataRow_add_SP["vcSupplierPlant"] = strSupplierPlant_ed;
                        dataRow_add_SP["status"] = "add";
                        dataRow_add_SP["error"] = strError;
                        dtModInfo_SP.Rows.Add(dataRow_add_SP);
                        #endregion
                        //更新TSPMaster_SupplierPlant表
                        if (strSupplierPlantLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_SP = dtModInfo_SP.NewRow();
                            dataRow_mod_SP["LinId"] = strSupplierPlantLinId_before;
                            dataRow_mod_SP["dToTime"] = strSupplierPlantToTime_before;
                            dataRow_mod_SP["status"] = "mod";
                            dataRow_mod_SP["error"] = strError;
                            dtModInfo_SP.Rows.Add(dataRow_mod_SP);
                            #endregion
                        }
                    }
                    #endregion

                    #region 子表-收容数
                    string strBoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                    //2.判断iPackingQty是否需要修改
                    if (strBoxLinId_ed != "0" && strBoxLinId_ed == "-1")
                    {
                        string strBoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string strBoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();

                        string strBoxLinId_before = "";
                        string strBoxToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("PackingQtyEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strBoxLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            string strBoxFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(strBoxFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【收容数有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            strBoxToTime_before = Convert.ToDateTime(strBoxFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_Box表
                        #region AddNewRow
                        DataRow dataRow_add_PQ = dtModInfo_PQ.NewRow();
                        dataRow_add_PQ["vcPackingPlant"] = strPackingPlant;
                        dataRow_add_PQ["vcPartId"] = strPartId;
                        dataRow_add_PQ["vcReceiver"] = strReceiver;
                        dataRow_add_PQ["vcSupplierId"] = strSupplierId;
                        dataRow_add_PQ["vcSupplierPlant"] = strSupplierPlant;
                        dataRow_add_PQ["dFromTime"] = strBoxFromTime_ed;
                        dataRow_add_PQ["dToTime"] = strBoxToTime_ed;
                        dataRow_add_PQ["iPackingQty"] = strBoxPackingQty_ed;
                        dataRow_add_PQ["vcBoxType"] = strBoxType_ed;
                        dataRow_add_PQ["iLength"] = strBoxLength_ed;
                        dataRow_add_PQ["iWidth"] = strBoxWidth_ed;
                        dataRow_add_PQ["iHeight"] = strBoxHeight_ed;
                        dataRow_add_PQ["iVolume"] = strBoxVolume_ed;
                        dataRow_add_PQ["status"] = "add";
                        dataRow_add_PQ["error"] = strError;
                        dtModInfo_PQ.Rows.Add(dataRow_add_PQ);
                        #endregion
                        //更新TSPMaster_Box表
                        if (strBoxLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_PQ = dtModInfo_PQ.NewRow();
                            dataRow_mod_PQ["LinId"] = strBoxLinId_before;
                            dataRow_mod_PQ["dToTime"] = strBoxToTime_before;
                            dataRow_mod_PQ["status"] = "mod";
                            dataRow_mod_PQ["error"] = strError;
                            dtModInfo_PQ.Rows.Add(dataRow_mod_PQ);
                            #endregion
                        }
                    }
                    #endregion

                    #region 子表-受入
                    string strSufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                    //3.判断vcSufferIn是否需要修改
                    if (strSufferInLinId_ed != "0" && strSufferInLinId_ed == "-1")
                    {
                        string strSufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string strSufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();

                        string strSufferInLinId_before = "";
                        string strSufferInToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("SufferInEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strSufferInLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            string strSufferInFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(strSufferInFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【收容数有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            strSufferInToTime_before = Convert.ToDateTime(strSufferInFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_SufferIn表
                        #region AddNewRow
                        DataRow dataRow_add_SI = dtModInfo_SI.NewRow();
                        dataRow_add_SI["vcPackingPlant"] = strPackingPlant;
                        dataRow_add_SI["vcPartId"] = strPartId;
                        dataRow_add_SI["vcReceiver"] = strReceiver;
                        dataRow_add_SI["vcSupplierId"] = strSupplierId;
                        dataRow_add_SI["dFromTime"] = strSufferInFromTime_ed;
                        dataRow_add_SI["dToTime"] = strSufferInToTime_ed;
                        dataRow_add_SI["vcSufferIn"] = strSufferIn_ed;
                        dataRow_add_SI["status"] = "add";
                        dataRow_add_SI["error"] = strError;
                        dtModInfo_SI.Rows.Add(dataRow_add_SI);
                        #endregion
                        //更新TSPMaster_SufferIn表
                        if (strSufferInLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_SI = dtModInfo_SI.NewRow();
                            dataRow_mod_SI["LinId"] = strSufferInLinId_before;
                            dataRow_mod_SI["dToTime"] = strSufferInToTime_before;
                            dataRow_mod_SI["status"] = "mod";
                            dataRow_mod_SI["error"] = strError;
                            dtModInfo_SI.Rows.Add(dataRow_mod_SI);
                            #endregion
                        }
                    }
                    #endregion

                    #region HistoryNewRow
                    DataRow dataRow_History = dtOperHistory.NewRow();
                    dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                    dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                    dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                    dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_History["vcChangeItem"] = "情报新增";
                    dataRow_History["vcAction"] =
                        "包装工场:" + dtImport.Rows[i]["vcPackingPlant"].ToString() +
                        "；品番：" + dtImport.Rows[i]["vcPartId"].ToString() +
                        "；收货方：" + dtImport.Rows[i]["vcReceiver"].ToString() +
                        "；供应商代码：" + dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_History["vcActionTime"] = "";
                    dataRow_History["error"] = "";
                    dtOperHistory.Rows.Add(dataRow_History);
                    #endregion

                    #region 判断是否有变更
                    if (dtOperHistory.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报品番没有变更情报", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }
                if (strType == "修改")
                {
                    string strOperItem = string.Empty;
                    string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString().Trim();
                    DataRow[] drOperCheck = dtOperCheck.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                    bool bCheckUpdate = false;
                    #region 主表
                    DataRow dataRow_mod = dtModInfo.NewRow();
                    if (drOperCheck.Length > 0)
                    {
                        #region 判断更新
                        //同步时间  dSyncTime
                        string strSyncTime = drOperCheck[0]["dSyncTime"].ToString();
                        dataRow_mod["dSyncTime"] = strSyncTime;
                        //变更事项  vcChanges
                        string strChanges = drOperCheck[0]["vcChanges"].ToString();
                        dataRow_mod["vcChanges"] = strChanges;
                        //包装工厂
                        dataRow_mod["vcPackingPlant"] = strPackingPlant;
                        //品番vcPartId
                        dataRow_mod["vcPartId"] = strPartId;
                        //品名    vcPartENName
                        string strPartENName = drOperCheck[0]["vcPartENName"].ToString();
                        dataRow_mod["vcPartENName"] = strPartENName;
                        if (strPartENName != dtImport.Rows[i]["vcPartENName"].ToString())
                        {
                            dataRow_mod["vcPartENName"] = dtImport.Rows[i]["vcPartENName"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "英文品名修改";
                            dataRow_History["vcAction"] = strPartENName + "==>" + dtImport.Rows[i]["vcPartENName"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //车种    vcCarfamilyCode
                        string strCarfamilyCode = drOperCheck[0]["vcCarfamilyCode"].ToString();
                        dataRow_mod["vcCarfamilyCode"] = strCarfamilyCode;
                        if (strCarfamilyCode != dtImport.Rows[i]["vcCarfamilyCode"].ToString())
                        {
                            dataRow_mod["vcCarfamilyCode"] = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "车型修改";
                            dataRow_History["vcAction"] = strCarfamilyCode + "==>" + dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //收货方   vcReceiver
                        dataRow_mod["vcReceiver"] = strReceiver;
                        //开始使用    dFromTime
                        bool bFromChange = false;
                        string strFromTime = Convert.ToDateTime(drOperCheck[0]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                        dataRow_mod["dFromTime"] = strFromTime;
                        if (strFromTime != dtImport.Rows[i]["dFromTime"].ToString())
                        {
                            bFromChange = true;
                            dataRow_mod["dFromTime"] = dtImport.Rows[i]["dFromTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "品番使用开始修改";
                            dataRow_History["vcAction"] = strFromTime + "==>" + dtImport.Rows[i]["dFromTime"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                            #region SPChild
                            DataRow dataRow_SPChild = dtSPChild.NewRow();
                            dataRow_SPChild["vcPackingPlant"] = strPackingPlant;
                            dataRow_SPChild["vcPartId"] = strPartId;
                            dataRow_SPChild["vcReceiver"] = strReceiver;
                            dataRow_SPChild["vcSupplierId"] = strSupplierId;
                            dataRow_SPChild["dFromTime"] = dtImport.Rows[i]["dFromTime"].ToString();
                            dataRow_SPChild["vcFlag"] = "from";
                            dtSPChild.Rows.Add(dataRow_SPChild);
                            #endregion
                        }
                        //结束使用    dToTime
                        bool bToChange = false;
                        string strToTime = Convert.ToDateTime(drOperCheck[0]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                        dataRow_mod["dToTime"] = strToTime;
                        if (strToTime != dtImport.Rows[i]["dToTime"].ToString())
                        {
                            bToChange = true;
                            dataRow_mod["dToTime"] = dtImport.Rows[i]["dToTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "品番使用结束修改";
                            dataRow_History["vcAction"] = strToTime + "==>" + dtImport.Rows[i]["dToTime"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                            #region SPChild
                            DataRow dataRow_SPChild = dtSPChild.NewRow();
                            dataRow_SPChild["vcPackingPlant"] = strPackingPlant;
                            dataRow_SPChild["vcPartId"] = strPartId;
                            dataRow_SPChild["vcReceiver"] = strReceiver;
                            dataRow_SPChild["vcSupplierId"] = strSupplierId;
                            dataRow_SPChild["dToTime"] = dtImport.Rows[i]["dToTime"].ToString();
                            dataRow_SPChild["vcFlag"] = "to";
                            dtSPChild.Rows.Add(dataRow_SPChild);
                            #endregion
                        }
                        if (bFromChange && bToChange)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报品番使用开始和品番使用结束不能同时修改", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                        //替代品番    vcPartId_Replace
                        string strPartId_Replace = drOperCheck[0]["vcPartId_Replace"].ToString();
                        dataRow_mod["vcPartId_Replace"] = strPartId_Replace;
                        if (strPartId_Replace != dtImport.Rows[i]["vcPartId_Replace"].ToString())
                        {
                            dataRow_mod["vcPartId_Replace"] = dtImport.Rows[i]["vcPartId_Replace"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "替代品番修改";
                            dataRow_History["vcAction"] = strPartId_Replace + "==>" + dtImport.Rows[i]["vcPartId_Replace"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //内外区分    vcInOut
                        string strInOut = drOperCheck[0]["vcInOut"].ToString();
                        dataRow_mod["vcInOut"] = strInOut;
                        if (strInOut != dtImport.Rows[i]["vcInOut"].ToString())
                        {
                            dataRow_mod["vcInOut"] = dtImport.Rows[i]["vcInOut"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "内外修改";
                            dataRow_History["vcAction"] = strInOut + "==>" + dtImport.Rows[i]["vcInOut"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //OE=SP    vcOESP
                        string strOESP = drOperCheck[0]["vcOESP"].ToString();
                        dataRow_mod["vcOESP"] = strOESP;
                        if (strOESP != dtImport.Rows[i]["vcOESP"].ToString())
                        {
                            dataRow_mod["vcOESP"] = dtImport.Rows[i]["vcOESP"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "OE=SP修改";
                            dataRow_History["vcAction"] = strOESP + "==>" + dtImport.Rows[i]["vcOESP"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //号旧区分    vcHaoJiu
                        string strHaoJiu = drOperCheck[0]["vcHaoJiu"].ToString();
                        dataRow_mod["vcHaoJiu"] = strHaoJiu;
                        if (strHaoJiu != dtImport.Rows[i]["vcHaoJiu"].ToString())
                        {
                            dataRow_mod["vcHaoJiu"] = dtImport.Rows[i]["vcHaoJiu"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "号旧修改";
                            dataRow_History["vcAction"] = strHaoJiu + "==>" + dtImport.Rows[i]["vcHaoJiu"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //生产区分    vcOldProduction
                        string strOldProduction = drOperCheck[0]["vcOldProduction"].ToString();
                        dataRow_mod["vcOldProduction"] = strOldProduction;
                        if (strOldProduction != dtImport.Rows[i]["vcOldProduction"].ToString())
                        {
                            dataRow_mod["vcOldProduction"] = dtImport.Rows[i]["vcOldProduction"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "旧型年限生产区分修改";
                            dataRow_History["vcAction"] = strOldProduction + "==>" + dtImport.Rows[i]["vcOldProduction"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //实施时间    dDebugTime
                        string strDebugTime = drOperCheck[0]["dDebugTime"].ToString();
                        dataRow_mod["dDebugTime"] = strDebugTime == "" ? "" : Convert.ToDateTime(strDebugTime + "/01").ToString("yyyy-MM-01");
                        if (strDebugTime != (dtImport.Rows[i]["dDebugTime"].ToString() != "" ? Convert.ToDateTime(dtImport.Rows[i]["dDebugTime"].ToString()).ToString("yyyy/MM") : ""))
                        {
                            dataRow_mod["dDebugTime"] = dtImport.Rows[i]["dDebugTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "实施年月(年限)修改";
                            dataRow_History["vcAction"] = strDebugTime + "==>" + (dtImport.Rows[i]["dDebugTime"].ToString() != "" ? Convert.ToDateTime(dtImport.Rows[i]["dDebugTime"].ToString()).ToString("yyyy/MM") : "");
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //供应商   vcSupplierId
                        dataRow_mod["vcSupplierId"] = strSupplierId;
                        //供应商使用开始    dSupplierFromTime
                        string strSupplierFromTime = drOperCheck[0]["dSupplierFromTime"].ToString() != "" ? Convert.ToDateTime(drOperCheck[0]["dSupplierFromTime"].ToString()).ToString("yyyy-MM-dd") : "";
                        dataRow_mod["dSupplierFromTime"] = strSupplierFromTime;
                        if (strSupplierFromTime != dtImport.Rows[i]["dSupplierFromTime"].ToString())
                        {
                            dataRow_mod["dSupplierFromTime"] = dtImport.Rows[i]["dSupplierFromTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "供应商开始修改";
                            dataRow_History["vcAction"] = strSupplierFromTime + "==>" + dtImport.Rows[i]["dSupplierFromTime"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //供应商使用结束    dSupplierToTime
                        string strSupplierToTime = drOperCheck[0]["dSupplierToTime"].ToString() != "" ? Convert.ToDateTime(drOperCheck[0]["dSupplierToTime"].ToString()).ToString("yyyy-MM-dd") : "";
                        dataRow_mod["dSupplierToTime"] = strSupplierToTime;
                        if (strSupplierToTime != dtImport.Rows[i]["dSupplierToTime"].ToString())
                        {
                            dataRow_mod["dSupplierToTime"] = dtImport.Rows[i]["dSupplierToTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "供应商结束修改";
                            dataRow_History["vcAction"] = strSupplierToTime + "==>" + dtImport.Rows[i]["dSupplierToTime"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //供应商名称    vcSupplierName
                        string strSupplierName = drOperCheck[0]["vcSupplierName"].ToString();
                        dataRow_mod["vcSupplierName"] = strSupplierName;
                        if (strSupplierName != dtImport.Rows[i]["vcSupplierName"].ToString())
                        {
                            dataRow_mod["vcSupplierName"] = dtImport.Rows[i]["vcSupplierName"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "供应商名称修改";
                            dataRow_History["vcAction"] = strSupplierName + "==>" + dtImport.Rows[i]["vcSupplierName"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //出荷场    vcSupplierPlace
                        string strSupplierPlace = drOperCheck[0]["vcSupplierPlace"].ToString();
                        dataRow_mod["vcSupplierPlace"] = strSupplierPlace;
                        if (strSupplierPlace != dtImport.Rows[i]["vcSupplierPlace"].ToString())
                        {
                            dataRow_mod["vcSupplierPlace"] = dtImport.Rows[i]["vcSupplierPlace"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "出何地修改";
                            dataRow_History["vcAction"] = strSupplierPlace + "==>" + dtImport.Rows[i]["vcSupplierPlace"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //内制工程    vcInteriorProject
                        string strInteriorProject = drOperCheck[0]["vcInteriorProject"].ToString();
                        dataRow_mod["vcInteriorProject"] = strInteriorProject;
                        if (strInteriorProject != dtImport.Rows[i]["vcInteriorProject"].ToString())
                        {
                            dataRow_mod["vcInteriorProject"] = dtImport.Rows[i]["vcInteriorProject"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "内制工程修改";
                            dataRow_History["vcAction"] = strInteriorProject + "==>" + dtImport.Rows[i]["vcInteriorProject"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //通过工程    vcPassProject
                        string strPassProject = drOperCheck[0]["vcPassProject"].ToString();
                        dataRow_mod["vcPassProject"] = strPassProject;
                        if (strPassProject != dtImport.Rows[i]["vcPassProject"].ToString())
                        {
                            dataRow_mod["vcPassProject"] = dtImport.Rows[i]["vcPassProject"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "通过工程修改";
                            dataRow_History["vcAction"] = strPassProject + "==>" + dtImport.Rows[i]["vcPassProject"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //前工程    vcFrontProject
                        string strFrontProject = drOperCheck[0]["vcFrontProject"].ToString();
                        dataRow_mod["vcFrontProject"] = strFrontProject;
                        if (strFrontProject != dtImport.Rows[i]["vcFrontProject"].ToString())
                        {
                            dataRow_mod["vcFrontProject"] = dtImport.Rows[i]["vcFrontProject"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "前工程修改";
                            dataRow_History["vcAction"] = strFrontProject + "==>" + dtImport.Rows[i]["vcFrontProject"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //前工程通过时间    dFrontProjectTime
                        string strFrontProjectTime = drOperCheck[0]["dFrontProjectTime"].ToString() != "" ? Convert.ToDateTime(drOperCheck[0]["dFrontProjectTime"].ToString()).ToString("yyyy-MM-dd HH:mm") : "";
                        dataRow_mod["dFrontProjectTime"] = strFrontProjectTime;
                        if (strFrontProjectTime != dtImport.Rows[i]["dFrontProjectTime"].ToString())
                        {
                            dataRow_mod["dFrontProjectTime"] = dtImport.Rows[i]["dFrontProjectTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "前工程通过时间修改";
                            dataRow_History["vcAction"] = strFrontProjectTime + "==>" + dtImport.Rows[i]["dFrontProjectTime"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //自工程出货时间    dShipmentTime
                        string strShipmentTime = drOperCheck[0]["dShipmentTime"].ToString() != "" ? Convert.ToDateTime(drOperCheck[0]["dShipmentTime"].ToString()).ToString("yyyy-MM-dd HH:mm") : "";
                        dataRow_mod["dShipmentTime"] = strShipmentTime;
                        if (strShipmentTime != dtImport.Rows[i]["dShipmentTime"].ToString())
                        {
                            dataRow_mod["dShipmentTime"] = dtImport.Rows[i]["dShipmentTime"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "自工程出货时间修改";
                            dataRow_History["vcAction"] = strShipmentTime + "==>" + dtImport.Rows[i]["dShipmentTime"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //照片    vcPartImage
                        string strPartImage = drOperCheck[0]["vcPartImage"].ToString();
                        dataRow_mod["vcPartImage"] = strPartImage;
                        if (strPartImage != dtImport.Rows[i]["vcPartImage"].ToString())
                        {
                            dataRow_mod["vcPartImage"] = dtImport.Rows[i]["vcPartImage"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "照片修改";
                            dataRow_History["vcAction"] = strPartImage + "==>" + dtImport.Rows[i]["vcPartImage"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //单据区分    vcBillType
                        string strBillType = drOperCheck[0]["vcBillType"].ToString();
                        dataRow_mod["vcBillType"] = strBillType;
                        if (strBillType != dtImport.Rows[i]["vcBillType"].ToString())
                        {
                            dataRow_mod["vcBillType"] = dtImport.Rows[i]["vcBillType"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "单据区分修改";
                            dataRow_History["vcAction"] = strBillType + "==>" + dtImport.Rows[i]["vcBillType"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        dataRow_mod["vcRemark1"] = dtImport.Rows[i]["vcRemark1"].ToString();
                        dataRow_mod["vcRemark2"] = dtImport.Rows[i]["vcRemark2"].ToString();
                        //订货方式    vcOrderingMethod
                        string strOrderingMethod = drOperCheck[0]["vcOrderingMethod"].ToString();
                        dataRow_mod["vcOrderingMethod"] = strOrderingMethod;
                        if (strOrderingMethod != dtImport.Rows[i]["vcOrderingMethod"].ToString())
                        {
                            dataRow_mod["vcOrderingMethod"] = dtImport.Rows[i]["vcOrderingMethod"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "订货方式修改";
                            dataRow_History["vcAction"] = strOrderingMethod + "==>" + dtImport.Rows[i]["vcOrderingMethod"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //强制订货    vcMandOrder
                        string strMandOrder = drOperCheck[0]["vcMandOrder"].ToString();
                        dataRow_mod["vcMandOrder"] = strMandOrder;
                        if (strMandOrder != dtImport.Rows[i]["vcMandOrder"].ToString())
                        {
                            dataRow_mod["vcMandOrder"] = dtImport.Rows[i]["vcMandOrder"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "强制订货修改";
                            dataRow_History["vcAction"] = strMandOrder + "==>" + dtImport.Rows[i]["vcMandOrder"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        //供应商包装    vcSupplierPacking
                        string strSupplierPacking = drOperCheck[0]["vcSupplierPacking"].ToString();
                        dataRow_mod["vcSupplierPacking"] = strSupplierPacking;
                        if (strSupplierPacking != dtImport.Rows[i]["vcSupplierPacking"].ToString())
                        {
                            dataRow_mod["vcSupplierPacking"] = dtImport.Rows[i]["vcSupplierPacking"].ToString();
                            #region HistoryNewRow
                            bCheckUpdate = true;
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = strPackingPlant;
                            dataRow_History["vcPartId"] = strPartId;
                            dataRow_History["vcReceiver"] = strReceiver;
                            dataRow_History["vcSupplierId"] = strSupplierId;
                            dataRow_History["vcChangeItem"] = "供应商包装修改";
                            dataRow_History["vcAction"] = strSupplierPacking + "==>" + dtImport.Rows[i]["vcSupplierPacking"].ToString();
                            dataRow_History["vcActionTime"] = "";
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                        #endregion

                        if (bCheckUpdate)
                        {
                            #region AddNewRow
                            dtModInfo.Rows.Add(dataRow_mod);
                            #endregion
                        }
                    }
                    #endregion

                    #region 子表-工区
                    string strSupplierPlant = dtImport.Rows[i]["vcSupplierPlant"].ToString();
                    string strSupplierPlantLinId_ed = dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString();
                    //1.判断vcSupplierPlant是否需要修改
                    if (strSupplierPlantLinId_ed != "0" && strSupplierPlantLinId_ed == "-1")
                    {
                        string strSupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();

                        string strSupplierPlantLinId_before = "";
                        string strSupplierPlantToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("SupplierPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strSupplierPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            string strSupplierPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(strSupplierPlantFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            strSupplierPlantToTime_before = Convert.ToDateTime(strSupplierPlantFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_SupplierPlant表
                        #region AddNewRow
                        DataRow dataRow_add_SP = dtModInfo_SP.NewRow();
                        dataRow_add_SP["vcPackingPlant"] = strPackingPlant;
                        dataRow_add_SP["vcPartId"] = strPartId;
                        dataRow_add_SP["vcReceiver"] = strReceiver;
                        dataRow_add_SP["vcSupplierId"] = strSupplierId;
                        dataRow_add_SP["dFromTime"] = strSupplierPlantFromTime_ed;
                        dataRow_add_SP["dToTime"] = strSupplierPlantToTime_ed;
                        dataRow_add_SP["vcSupplierPlant"] = strSupplierPlant_ed;
                        dataRow_add_SP["status"] = "add";
                        dataRow_add_SP["error"] = strError;
                        dtModInfo_SP.Rows.Add(dataRow_add_SP);
                        #endregion
                        //更新TSPMaster_SupplierPlant表
                        if (strSupplierPlantLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_SP = dtModInfo_SP.NewRow();
                            dataRow_mod_SP["LinId"] = strSupplierPlantLinId_before;
                            dataRow_mod_SP["dToTime"] = strSupplierPlantToTime_before;
                            dataRow_mod_SP["status"] = "mod";
                            dataRow_mod_SP["error"] = strError;
                            dtModInfo_SP.Rows.Add(dataRow_mod_SP);
                            #endregion
                        }
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = strPackingPlant;
                        dataRow_History["vcPartId"] = strPartId;
                        dataRow_History["vcReceiver"] = strReceiver;
                        dataRow_History["vcSupplierId"] = strSupplierId;
                        dataRow_History["vcChangeItem"] = "工区修改";
                        dataRow_History["vcAction"] = "工区:" + strSupplierPlant_ed;
                        dataRow_History["vcActionTime"] = "使用开始：" + strSupplierPlantFromTime_ed + "；使用结束：" + strSupplierPlantToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    #endregion

                    #region 子表-收容数
                    string strBoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                    //2.判断iPackingQty是否需要修改
                    if (strBoxLinId_ed != "0" && strBoxLinId_ed == "-1")
                    {
                        string strBoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string strBoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();

                        string strBoxLinId_before = "";
                        string strBoxToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("PackingQtyEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strBoxLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            string strBoxFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(strBoxFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【收容数有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            strBoxToTime_before = Convert.ToDateTime(strBoxFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_Box表
                        #region AddNewRow
                        DataRow dataRow_add_PQ = dtModInfo_PQ.NewRow();
                        dataRow_add_PQ["vcPackingPlant"] = strPackingPlant;
                        dataRow_add_PQ["vcPartId"] = strPartId;
                        dataRow_add_PQ["vcReceiver"] = strReceiver;
                        dataRow_add_PQ["vcSupplierId"] = strSupplierId;
                        dataRow_add_PQ["vcSupplierPlant"] = strSupplierPlant;
                        dataRow_add_PQ["dFromTime"] = strBoxFromTime_ed;
                        dataRow_add_PQ["dToTime"] = strBoxToTime_ed;
                        dataRow_add_PQ["iPackingQty"] = strBoxPackingQty_ed;
                        dataRow_add_PQ["vcBoxType"] = strBoxType_ed;
                        dataRow_add_PQ["iLength"] = strBoxLength_ed;
                        dataRow_add_PQ["iWidth"] = strBoxWidth_ed;
                        dataRow_add_PQ["iHeight"] = strBoxHeight_ed;
                        dataRow_add_PQ["iVolume"] = strBoxVolume_ed;
                        dataRow_add_PQ["status"] = "add";
                        dataRow_add_PQ["error"] = strError;
                        dtModInfo_PQ.Rows.Add(dataRow_add_PQ);
                        #endregion
                        //更新TSPMaster_Box表
                        if (strBoxLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_PQ = dtModInfo_PQ.NewRow();
                            dataRow_mod_PQ["LinId"] = strBoxLinId_before;
                            dataRow_mod_PQ["dToTime"] = strBoxToTime_before;
                            dataRow_mod_PQ["status"] = "mod";
                            dataRow_mod_PQ["error"] = strError;
                            dtModInfo_PQ.Rows.Add(dataRow_mod_PQ);
                            #endregion
                        }
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = strPackingPlant;
                        dataRow_History["vcPartId"] = strPartId;
                        dataRow_History["vcReceiver"] = strReceiver;
                        dataRow_History["vcSupplierId"] = strSupplierId;
                        dataRow_History["vcChangeItem"] = "收容数修改";
                        dataRow_History["vcAction"] = "收容数:" + strBoxPackingQty_ed + "；箱种:" + strBoxType_ed + "；长:" + strBoxLength_ed + "；宽:" + strBoxWidth_ed + "；高:" + strBoxHeight_ed + "；体积:" + strBoxVolume_ed;
                        dataRow_History["vcActionTime"] = "使用开始：" + strBoxFromTime_ed + "；使用结束：" + strBoxToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    #endregion

                    #region 子表-受入
                    string strSufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                    //3.判断vcSufferIn是否需要修改
                    if (strSufferInLinId_ed != "0" && strSufferInLinId_ed == "-1")
                    {
                        string strSufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string strSufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();

                        string strSufferInLinId_before = "";
                        string strSufferInToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("SufferInEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strSufferInLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            string strSufferInFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(strSufferInFromTime_ed))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【收容数有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
                            }
                            strSufferInToTime_before = Convert.ToDateTime(strSufferInFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_SufferIn表
                        #region AddNewRow
                        DataRow dataRow_add_SI = dtModInfo_SI.NewRow();
                        dataRow_add_SI["vcPackingPlant"] = strPackingPlant;
                        dataRow_add_SI["vcPartId"] = strPartId;
                        dataRow_add_SI["vcReceiver"] = strReceiver;
                        dataRow_add_SI["vcSupplierId"] = strSupplierId;
                        dataRow_add_SI["dFromTime"] = strSufferInFromTime_ed;
                        dataRow_add_SI["dToTime"] = strSufferInToTime_ed;
                        dataRow_add_SI["vcSufferIn"] = strSufferIn_ed;
                        dataRow_add_SI["status"] = "add";
                        dataRow_add_SI["error"] = strError;
                        dtModInfo_SI.Rows.Add(dataRow_add_SI);
                        #endregion
                        //更新TSPMaster_SufferIn表
                        if (strSufferInLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_SI = dtModInfo_SI.NewRow();
                            dataRow_mod_SI["LinId"] = strSufferInLinId_before;
                            dataRow_mod_SI["dToTime"] = strSufferInToTime_before;
                            dataRow_mod_SI["status"] = "mod";
                            dataRow_mod_SI["error"] = strError;
                            dtModInfo_SI.Rows.Add(dataRow_mod_SI);
                            #endregion
                        }
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = strPackingPlant;
                        dataRow_History["vcPartId"] = strPartId;
                        dataRow_History["vcReceiver"] = strReceiver;
                        dataRow_History["vcSupplierId"] = strSupplierId;
                        dataRow_History["vcChangeItem"] = "受入修改";
                        dataRow_History["vcAction"] = "受入:" + strSufferIn_ed;
                        dataRow_History["vcActionTime"] = "使用开始：" + strSufferInFromTime_ed + "；使用结束：" + strSufferInToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    #endregion

                    #region 判断是否有变更
                    if (dtOperHistory.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报品番没有变更情报", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }
                if (strType == "删除")
                {
                    string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString().Trim();
                    #region 主表
                    DataRow dataRow_del = dtDelInfo.NewRow();
                    dataRow_del["vcPackingPlant"] = strPackingPlant;
                    dataRow_del["vcPartId"] = strPartId;
                    dataRow_del["vcReceiver"] = strReceiver;
                    dataRow_del["vcSupplierId"] = strSupplierId;
                    dtDelInfo.Rows.Add(dataRow_del);
                    #endregion

                    #region HistoryNewRow
                    DataRow dataRow_History = dtOperHistory.NewRow();
                    dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                    dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                    dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                    dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_History["vcChangeItem"] = "情报删除";
                    dataRow_History["vcAction"] =
                        "包装工场:" + dtImport.Rows[i]["vcPackingPlant"].ToString() +
                        "；品番：" + dtImport.Rows[i]["vcPartId"].ToString() +
                        "；收货方：" + dtImport.Rows[i]["vcReceiver"].ToString() +
                        "；供应商代码：" + dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_History["vcActionTime"] = "";
                    dataRow_History["error"] = "";
                    dtOperHistory.Rows.Add(dataRow_History);
                    #endregion

                    #region 判断是否有变更
                    if (dtOperHistory.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报品番没有变更情报", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }

            }
            if (dtMessage == null || dtMessage.Rows.Count == 0)
            {
                fs0603_DataAccess.setSPInfo(dtAddInfo, dtModInfo, dtDelInfo, dtModInfo_SP, dtModInfo_PQ, dtModInfo_SI, dtModInfo_OP, dtOperHistory, dtSPChild, dtChildInfo_gq, dtChildInfo_sr, dtChildInfo_hz,
                    strOperId, ref dtMessage);
            }
        }
        public void setSPInfo(DataTable dtImport, string strshoupei, string strOperId, ref DataTable dtMessage)
        {
            //用于检查变更情况
            DataTable dtOperCheck = getChildInfo("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", true, "", "", "", "", strshoupei);
            //用于新增数据
            DataTable dtAddInfo = dtOperCheck.Clone();
            //用于修改数据--修改主数据表
            DataTable dtModInfo = dtOperCheck.Clone();
            //用于删除数据--删除主数据表
            DataTable dtDelInfo = dtOperCheck.Clone();
            //用于修改数据--修改工区数据表
            DataTable dtModInfo_SP = createTable("SP");
            //用于修改数据--修改收容数数据表
            DataTable dtModInfo_PQ = createTable("PQ");
            //用于修改数据--修改受入数据表
            DataTable dtModInfo_SI = createTable("SI");
            //用于修改数据--修改发注工厂数据表
            DataTable dtModInfo_OP = createTable("OP");
            //用于记录修改履历
            DataTable dtOperHistory = createTable("OH");
            //用于更新手配子表--主数据时间变更引起子表变更
            DataTable dtSPChild = createTable("SPChild");
            //用于强制入力维护--工区
            DataTable dtChildInfo_gq = createTable("SP");
            //用于强制入力维护--受入
            DataTable dtChildInfo_sr = createTable("SI");
            //用于强制入力维护--荷姿
            DataTable dtChildInfo_hz = createTable("PQ");
            for (int i = 0; i < dtImport.Rows.Count; i++)
            {
                string strType = dtImport.Rows[i]["vcType"].ToString();//新增、修改、删除
                if (strType == "新增")
                {
                    string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString().Trim();

                    #region 子表-工区
                    if (strshoupei == "gq")
                    {
                        string strSupplierPlantLinId_ed = dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString();
                        string strSupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();
                        if (dtOperCheck.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "' and SupplierPlant_ed='" + strSupplierPlant_ed + "' and SupplierPlantFromTime_ed='" + strSupplierPlantFromTime_ed.Replace("-", "/") + "' and SupplierPlantToTime_ed='" + strSupplierPlantToTime_ed.Replace("-", "/") + "'").Length == 0)
                        {
                            #region AddNewRow
                            DataRow drChildInfo_gq = dtChildInfo_gq.NewRow();
                            drChildInfo_gq["LinId"] = "";
                            drChildInfo_gq["vcPackingPlant"] = strPackingPlant;
                            drChildInfo_gq["vcPartId"] = strPartId;
                            drChildInfo_gq["vcReceiver"] = strReceiver;
                            drChildInfo_gq["vcSupplierId"] = strSupplierId;
                            drChildInfo_gq["dFromTime"] = strSupplierPlantFromTime_ed;
                            drChildInfo_gq["dToTime"] = strSupplierPlantToTime_ed;
                            drChildInfo_gq["vcSupplierPlant"] = strSupplierPlant_ed;
                            drChildInfo_gq["status"] = "add";
                            drChildInfo_gq["error"] = "";
                            dtChildInfo_gq.Rows.Add(drChildInfo_gq);
                            #endregion
                            #region HistoryNewRow
                            DataRow drOperHistory = dtOperHistory.NewRow();
                            drOperHistory["vcPackingPlant"] = strPackingPlant;
                            drOperHistory["vcPartId"] = strPartId;
                            drOperHistory["vcReceiver"] = strReceiver;
                            drOperHistory["vcSupplierId"] = strSupplierId;
                            drOperHistory["vcChangeItem"] = "工区强制入力";
                            drOperHistory["vcAction"] = "工区增加:" + strSupplierPlant_ed;
                            drOperHistory["vcActionTime"] = "使用开始：" + strSupplierPlantFromTime_ed + "；使用结束：" + strSupplierPlantToTime_ed;
                            drOperHistory["error"] = "";
                            dtOperHistory.Rows.Add(drOperHistory);
                            #endregion
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报工区已经存在不能重复添加", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    #endregion

                    #region 子表-收容数
                    if (strshoupei == "hz")
                    {
                        string strBoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                        string strBoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string strBoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();
                        if (dtOperCheck.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "' and BoxPackingQty_ed='" + strBoxPackingQty_ed + "' and BoxType_ed='" + strBoxType_ed + "' and SupplierPlantFromTime_ed='" + strBoxFromTime_ed.Replace("-", "/") + "' and SupplierPlantToTime_ed='" + strBoxToTime_ed.Replace("-", "/") + "'").Length == 0)
                        {
                            #region AddNewRow
                            DataRow drChildInfo_hz = dtChildInfo_hz.NewRow();
                            drChildInfo_hz["LinId"] = "";
                            drChildInfo_hz["vcPackingPlant"] = strPackingPlant;
                            drChildInfo_hz["vcPartId"] = strPartId;
                            drChildInfo_hz["vcReceiver"] = strReceiver;
                            drChildInfo_hz["vcSupplierId"] = strSupplierId;
                            drChildInfo_hz["dFromTime"] = strBoxFromTime_ed;
                            drChildInfo_hz["dToTime"] = strBoxToTime_ed;
                            drChildInfo_hz["iPackingQty"] = strBoxPackingQty_ed;
                            drChildInfo_hz["vcBoxType"] = strBoxType_ed;
                            drChildInfo_hz["iLength"] = strBoxLength_ed;
                            drChildInfo_hz["iWidth"] = strBoxWidth_ed;
                            drChildInfo_hz["iHeight"] = strBoxHeight_ed;
                            drChildInfo_hz["iVolume"] = strBoxVolume_ed;
                            drChildInfo_hz["status"] = "add";
                            drChildInfo_hz["error"] = "";
                            dtChildInfo_hz.Rows.Add(drChildInfo_hz);
                            #endregion
                            #region HistoryNewRow
                            DataRow drOperHistory = dtOperHistory.NewRow();
                            drOperHistory["vcPackingPlant"] = strPackingPlant;
                            drOperHistory["vcPartId"] = strPartId;
                            drOperHistory["vcReceiver"] = strReceiver;
                            drOperHistory["vcSupplierId"] = strSupplierId;
                            drOperHistory["vcChangeItem"] = "收容数强制入力";
                            drOperHistory["vcAction"] = "收容数增加:" + strBoxPackingQty_ed + "；箱种:" + strBoxType_ed + "；长:" + strBoxLength_ed + "；宽:" + strBoxWidth_ed + "；高:" + strBoxHeight_ed + "；体积:" + strBoxVolume_ed;
                            drOperHistory["vcActionTime"] = "使用开始：" + strBoxFromTime_ed + "；使用结束：" + strBoxToTime_ed;
                            drOperHistory["error"] = "";
                            dtOperHistory.Rows.Add(drOperHistory);
                            #endregion
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报收容数已经存在不能重复添加", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    #endregion

                    #region 子表-受入
                    if (strshoupei == "sr")
                    {
                        string strSufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                        string strSufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string strSufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();
                        if (dtOperCheck.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "' and SufferIn_ed='" + strSufferIn_ed + "' and SufferInFromTime_ed='" + strSufferInFromTime_ed.Replace("-", "/") + "' and SufferInToTime_ed='" + strSufferInToTime_ed.Replace("-", "/") + "'").Length == 0)
                        {
                            #region AddNewRow
                            DataRow drChildInfo_sr = dtChildInfo_sr.NewRow();
                            drChildInfo_sr["LinId"] = "";
                            drChildInfo_sr["vcPackingPlant"] = strPackingPlant;
                            drChildInfo_sr["vcPartId"] = strPartId;
                            drChildInfo_sr["vcReceiver"] = strReceiver;
                            drChildInfo_sr["vcSupplierId"] = strSupplierId;
                            drChildInfo_sr["dFromTime"] = strSufferInFromTime_ed;
                            drChildInfo_sr["dToTime"] = strSufferInToTime_ed;
                            drChildInfo_sr["vcSufferIn"] = strSufferIn_ed;
                            drChildInfo_sr["status"] = "add";
                            drChildInfo_sr["error"] = "";
                            dtChildInfo_sr.Rows.Add(drChildInfo_sr);
                            #endregion
                            #region HistoryNewRow
                            DataRow drOperHistory = dtOperHistory.NewRow();
                            drOperHistory["vcPackingPlant"] = strPackingPlant;
                            drOperHistory["vcPartId"] = strPartId;
                            drOperHistory["vcReceiver"] = strReceiver;
                            drOperHistory["vcSupplierId"] = strSupplierId;
                            drOperHistory["vcChangeItem"] = "受入强制入力";
                            drOperHistory["vcAction"] = "受入增加:" + strSufferIn_ed;
                            drOperHistory["vcActionTime"] = "使用开始：" + strSufferInFromTime_ed + "；使用结束：" + strSufferInToTime_ed;
                            drOperHistory["error"] = "";
                            dtOperHistory.Rows.Add(drOperHistory);
                            #endregion
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报受入已经存在不能重复添加", i + 1);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    #endregion

                    #region 判断是否有变更
                    if (dtOperHistory.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报手配没有变更情报", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }
                if (strType == "修改")
                {
                    string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString().Trim();
                    #region 子表-工区
                    if (strshoupei == "gq")
                    {
                        string strSupplierPlantLinId_ed = dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString();
                        string strSupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();
                        #region AddNewRow
                        DataRow drChildInfo_gq = dtChildInfo_gq.NewRow();
                        drChildInfo_gq["LinId"] = strSupplierPlantLinId_ed;
                        drChildInfo_gq["vcPackingPlant"] = strPackingPlant;
                        drChildInfo_gq["vcPartId"] = strPartId;
                        drChildInfo_gq["vcReceiver"] = strReceiver;
                        drChildInfo_gq["vcSupplierId"] = strSupplierId;
                        drChildInfo_gq["dFromTime"] = strSupplierPlantFromTime_ed;
                        drChildInfo_gq["dToTime"] = strSupplierPlantToTime_ed;
                        drChildInfo_gq["vcSupplierPlant"] = strSupplierPlant_ed;
                        drChildInfo_gq["status"] = "mod";
                        drChildInfo_gq["error"] = "";
                        dtChildInfo_gq.Rows.Add(drChildInfo_gq);
                        #endregion
                        #region HistoryNewRow
                        DataRow drOperHistory = dtOperHistory.NewRow();
                        drOperHistory["vcPackingPlant"] = strPackingPlant;
                        drOperHistory["vcPartId"] = strPartId;
                        drOperHistory["vcReceiver"] = strReceiver;
                        drOperHistory["vcSupplierId"] = strSupplierId;
                        drOperHistory["vcChangeItem"] = "工区强制入力";
                        drOperHistory["vcAction"] = "工区修改:" + strSupplierPlant_ed;
                        drOperHistory["vcActionTime"] = "使用开始：" + strSupplierPlantFromTime_ed + "；使用结束：" + strSupplierPlantToTime_ed;
                        drOperHistory["error"] = "";
                        dtOperHistory.Rows.Add(drOperHistory);
                        #endregion
                    }
                    #endregion

                    #region 子表-收容数
                    if (strshoupei == "hz")
                    {
                        string strBoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                        string strBoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string strBoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();
                        #region AddNewRow
                        DataRow drChildInfo_hz = dtChildInfo_hz.NewRow();
                        drChildInfo_hz["LinId"] = strBoxLinId_ed;
                        drChildInfo_hz["vcPackingPlant"] = strPackingPlant;
                        drChildInfo_hz["vcPartId"] = strPartId;
                        drChildInfo_hz["vcReceiver"] = strReceiver;
                        drChildInfo_hz["vcSupplierId"] = strSupplierId;
                        drChildInfo_hz["dFromTime"] = strBoxFromTime_ed;
                        drChildInfo_hz["dToTime"] = strBoxToTime_ed;
                        drChildInfo_hz["iPackingQty"] = strBoxPackingQty_ed;
                        drChildInfo_hz["vcBoxType"] = strBoxType_ed;
                        drChildInfo_hz["iLength"] = strBoxLength_ed;
                        drChildInfo_hz["iWidth"] = strBoxWidth_ed;
                        drChildInfo_hz["iHeight"] = strBoxHeight_ed;
                        drChildInfo_hz["iVolume"] = strBoxVolume_ed;
                        drChildInfo_hz["status"] = "mod";
                        drChildInfo_hz["error"] = "";
                        dtChildInfo_hz.Rows.Add(drChildInfo_hz);
                        #endregion
                        #region HistoryNewRow
                        DataRow drOperHistory = dtOperHistory.NewRow();
                        drOperHistory["vcPackingPlant"] = strPackingPlant;
                        drOperHistory["vcPartId"] = strPartId;
                        drOperHistory["vcReceiver"] = strReceiver;
                        drOperHistory["vcSupplierId"] = strSupplierId;
                        drOperHistory["vcChangeItem"] = "收容数强制入力";
                        drOperHistory["vcAction"] = "收容数修改:" + strBoxPackingQty_ed + "；箱种:" + strBoxType_ed + "；长:" + strBoxLength_ed + "；宽:" + strBoxWidth_ed + "；高:" + strBoxHeight_ed + "；体积:" + strBoxVolume_ed;
                        drOperHistory["vcActionTime"] = "使用开始：" + strBoxFromTime_ed + "；使用结束：" + strBoxToTime_ed;
                        drOperHistory["error"] = "";
                        dtOperHistory.Rows.Add(drOperHistory);
                        #endregion
                    }
                    #endregion

                    #region 子表-受入
                    if (strshoupei == "sr")
                    {
                        string strSufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                        string strSufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string strSufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();
                        #region AddNewRow
                        DataRow drChildInfo_sr = dtChildInfo_sr.NewRow();
                        drChildInfo_sr["LinId"] = strSufferInLinId_ed;
                        drChildInfo_sr["vcPackingPlant"] = strPackingPlant;
                        drChildInfo_sr["vcPartId"] = strPartId;
                        drChildInfo_sr["vcReceiver"] = strReceiver;
                        drChildInfo_sr["vcSupplierId"] = strSupplierId;
                        drChildInfo_sr["dFromTime"] = strSufferInFromTime_ed;
                        drChildInfo_sr["dToTime"] = strSufferInToTime_ed;
                        drChildInfo_sr["vcSufferIn"] = strSufferIn_ed;
                        drChildInfo_sr["status"] = "mod";
                        drChildInfo_sr["error"] = "";
                        dtChildInfo_sr.Rows.Add(drChildInfo_sr);
                        #endregion
                        #region HistoryNewRow
                        DataRow drOperHistory = dtOperHistory.NewRow();
                        drOperHistory["vcPackingPlant"] = strPackingPlant;
                        drOperHistory["vcPartId"] = strPartId;
                        drOperHistory["vcReceiver"] = strReceiver;
                        drOperHistory["vcSupplierId"] = strSupplierId;
                        drOperHistory["vcChangeItem"] = "受入强制入力";
                        drOperHistory["vcAction"] = "受入修改:" + strSufferIn_ed;
                        drOperHistory["vcActionTime"] = "使用开始：" + strSufferInFromTime_ed + "；使用结束：" + strSufferInToTime_ed;
                        drOperHistory["error"] = "";
                        dtOperHistory.Rows.Add(drOperHistory);
                        #endregion
                    }
                    #endregion

                    #region 判断是否有变更
                    if (dtOperHistory.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报手配没有变更情报", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }
                if (strType == "删除")
                {
                    string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString().Trim();
                    string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString().Trim();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString().Trim();
                    #region 子表-工区
                    if (strshoupei == "gq")
                    {
                        string strSupplierPlantLinId_ed = dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString();
                        string strSupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();
                        #region AddNewRow
                        DataRow drChildInfo_gq = dtChildInfo_gq.NewRow();
                        drChildInfo_gq["LinId"] = strSupplierPlantLinId_ed;
                        drChildInfo_gq["vcPackingPlant"] = strPackingPlant;
                        drChildInfo_gq["vcPartId"] = strPartId;
                        drChildInfo_gq["vcReceiver"] = strReceiver;
                        drChildInfo_gq["vcSupplierId"] = strSupplierId;
                        drChildInfo_gq["dFromTime"] = strSupplierPlantFromTime_ed;
                        drChildInfo_gq["dToTime"] = strSupplierPlantToTime_ed;
                        drChildInfo_gq["vcSupplierPlant"] = strSupplierPlant_ed;
                        drChildInfo_gq["status"] = "del";
                        drChildInfo_gq["error"] = "";
                        dtChildInfo_gq.Rows.Add(drChildInfo_gq);
                        #endregion
                        #region HistoryNewRow
                        DataRow drOperHistory = dtOperHistory.NewRow();
                        drOperHistory["vcPackingPlant"] = strPackingPlant;
                        drOperHistory["vcPartId"] = strPartId;
                        drOperHistory["vcReceiver"] = strReceiver;
                        drOperHistory["vcSupplierId"] = strSupplierId;
                        drOperHistory["vcChangeItem"] = "工区强制入力";
                        drOperHistory["vcAction"] = "工区删除:" + strSupplierPlant_ed;
                        drOperHistory["vcActionTime"] = "使用开始：" + strSupplierPlantFromTime_ed + "；使用结束：" + strSupplierPlantToTime_ed;
                        drOperHistory["error"] = "";
                        dtOperHistory.Rows.Add(drOperHistory);
                        #endregion
                    }
                    #endregion

                    #region 子表-收容数
                    if (strshoupei == "hz")
                    {
                        string strBoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                        string strBoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string strBoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();
                        #region AddNewRow
                        DataRow drChildInfo_hz = dtChildInfo_hz.NewRow();
                        drChildInfo_hz["LinId"] = strBoxLinId_ed;
                        drChildInfo_hz["vcPackingPlant"] = strPackingPlant;
                        drChildInfo_hz["vcPartId"] = strPartId;
                        drChildInfo_hz["vcReceiver"] = strReceiver;
                        drChildInfo_hz["vcSupplierId"] = strSupplierId;
                        drChildInfo_hz["dFromTime"] = strBoxFromTime_ed;
                        drChildInfo_hz["dToTime"] = strBoxToTime_ed;
                        drChildInfo_hz["iPackingQty"] = strBoxPackingQty_ed;
                        drChildInfo_hz["vcBoxType"] = strBoxType_ed;
                        drChildInfo_hz["iLength"] = strBoxLength_ed;
                        drChildInfo_hz["iWidth"] = strBoxWidth_ed;
                        drChildInfo_hz["iHeight"] = strBoxHeight_ed;
                        drChildInfo_hz["iVolume"] = strBoxVolume_ed;
                        drChildInfo_hz["status"] = "del";
                        drChildInfo_hz["error"] = "";
                        dtChildInfo_hz.Rows.Add(drChildInfo_hz);
                        #endregion
                        #region HistoryNewRow
                        DataRow drOperHistory = dtOperHistory.NewRow();
                        drOperHistory["vcPackingPlant"] = strPackingPlant;
                        drOperHistory["vcPartId"] = strPartId;
                        drOperHistory["vcReceiver"] = strReceiver;
                        drOperHistory["vcSupplierId"] = strSupplierId;
                        drOperHistory["vcChangeItem"] = "收容数强制入力";
                        drOperHistory["vcAction"] = "收容数删除:" + strBoxPackingQty_ed + "；箱种:" + strBoxType_ed + "；长:" + strBoxLength_ed + "；宽:" + strBoxWidth_ed + "；高:" + strBoxHeight_ed + "；体积:" + strBoxVolume_ed;
                        drOperHistory["vcActionTime"] = "使用开始：" + strBoxFromTime_ed + "；使用结束：" + strBoxToTime_ed;
                        drOperHistory["error"] = "";
                        dtOperHistory.Rows.Add(drOperHistory);
                        #endregion
                    }
                    #endregion

                    #region 子表-受入
                    if (strshoupei == "sr")
                    {
                        string strSufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                        string strSufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string strSufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();
                        #region AddNewRow
                        DataRow drChildInfo_sr = dtChildInfo_sr.NewRow();
                        drChildInfo_sr["LinId"] = strSufferInLinId_ed;
                        drChildInfo_sr["vcPackingPlant"] = strPackingPlant;
                        drChildInfo_sr["vcPartId"] = strPartId;
                        drChildInfo_sr["vcReceiver"] = strReceiver;
                        drChildInfo_sr["vcSupplierId"] = strSupplierId;
                        drChildInfo_sr["dFromTime"] = strSufferInFromTime_ed;
                        drChildInfo_sr["dToTime"] = strSufferInToTime_ed;
                        drChildInfo_sr["vcSufferIn"] = strSufferIn_ed;
                        drChildInfo_sr["status"] = "del";
                        drChildInfo_sr["error"] = "";
                        dtChildInfo_sr.Rows.Add(drChildInfo_sr);
                        #endregion
                        #region HistoryNewRow
                        DataRow drOperHistory = dtOperHistory.NewRow();
                        drOperHistory["vcPackingPlant"] = strPackingPlant;
                        drOperHistory["vcPartId"] = strPartId;
                        drOperHistory["vcReceiver"] = strReceiver;
                        drOperHistory["vcSupplierId"] = strSupplierId;
                        drOperHistory["vcChangeItem"] = "受入强制入力";
                        drOperHistory["vcAction"] = "受入删除:" + strSufferIn_ed;
                        drOperHistory["vcActionTime"] = "使用开始：" + strSufferInFromTime_ed + "；使用结束：" + strSufferInToTime_ed;
                        drOperHistory["error"] = "";
                        dtOperHistory.Rows.Add(drOperHistory);
                        #endregion
                    }
                    #endregion

                    #region 判断是否有变更
                    if (dtOperHistory.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报手配没有变更情报", i + 1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }

            }
            if (dtMessage == null || dtMessage.Rows.Count == 0)
            {
                fs0603_DataAccess.setSPInfo(dtAddInfo, dtModInfo, dtDelInfo, dtModInfo_SP, dtModInfo_PQ, dtModInfo_SI, dtModInfo_OP, dtOperHistory, dtSPChild, dtChildInfo_gq, dtChildInfo_sr, dtChildInfo_hz,
                    strOperId, ref dtMessage);
            }
        }
        public DataTable searchOperHistory(string strFromTime, string strToFrom, string strOperId)
        {
            try
            {
                return fs0603_DataAccess.searchOperHistory(strFromTime, strToFrom, strOperId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool getOrderPlantInfo(string strSupplierId, string strSupplierPlant, string strToTime)
        {
            DataTable data = fs0603_DataAccess.getOrderPlantInfo(strSupplierId, strSupplierPlant, strToTime);
            if (data != null && data.Rows.Count != 0)
                return true;
            else
                return false;
        }
        public DataTable gettaskNum()
        {
            try
            {
                return fs0603_DataAccess.gettaskNum();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Object> getSyncInfo()
        {
            try
            {
                DataSet dsInfo = fs0603_DataAccess.getSyncInfo();
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                if (dsInfo.Tables[0].Rows.Count == 0)
                {
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dsInfo.Tables[0], dtConverter);
                    return dataList;
                }
                else
                {
                    List<Object> dataList = ComFunction.convertAllToResultByConverter_main(dsInfo.Tables[0], dsInfo.Tables[1], dtConverter);
                    return dataList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSyncTable()
        {
            try
            {
                DataSet dsInfo = fs0603_DataAccess.getSyncInfo();
                DataTable dtInfo = dsInfo.Tables[0].Clone();

                for (int i = 0; i < dsInfo.Tables[0].Rows.Count; i++)
                {
                    DataRow dataRow = dtInfo.NewRow();
                    string dSyncTime = dsInfo.Tables[0].Rows[i]["dSyncTime"].ToString();
                    string strSyncMessage = dsInfo.Tables[0].Rows[i]["vcSyncMessage"].ToString();
                    string strChidren = dsInfo.Tables[0].Rows[i]["children"].ToString();
                    dataRow["dSyncTime"] = dSyncTime;
                    dataRow["vcSyncMessage"] = strSyncMessage;
                    dtInfo.Rows.Add(dataRow);
                    DataRow[] drChild = dsInfo.Tables[1].Select("children='" + strChidren + "'");
                    if (drChild.Length > 0)
                    {
                        for (int j = 0; j < drChild.Length; j++)
                        {
                            string dSyncTime1 = drChild[j]["dSyncTime"].ToString();
                            string strSyncMessage1 = drChild[j]["vcSyncMessage"].ToString();
                            DataRow dataRow1 = dtInfo.NewRow();
                            dataRow1["dSyncTime"] = "   " + dSyncTime1;
                            dataRow1["vcSyncMessage"] = strSyncMessage1;
                            dtInfo.Rows.Add(dataRow1);
                        }
                    }
                }
                return dtInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSyncInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs0603_DataAccess.setSyncInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getEditLoadInfo(string strEditType, string strPackingPlant, string strPartId, string strReceiver, string strSupplierId, string strSupplierPlant)
        {
            DataTable dataTable = fs0603_DataAccess.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
            return dataTable;
        }
        public DataTable getSubInfo(string strEditType, string strPackingPlant, string strPartId, string strReceiver, string strSupplierId, string strSupplierPlant)
        {
            DataTable dataTable = fs0603_DataAccess.getSubInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
            return dataTable;
        }
        public List<Dictionary<string, Object>> setNullData(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    listInfoData[i]["LinId"] = fs0603_DataAccess.setNullValue(listInfoData[i]["LinId"], "", "");
                    listInfoData[i]["dSyncTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dSyncTime"], "date", "");
                    listInfoData[i]["vcChanges"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcChanges"], "", "");
                    listInfoData[i]["vcPackingPlant"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPackingPlant"], "", "");
                    listInfoData[i]["vcPartId"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPartId"], "", "");
                    listInfoData[i]["vcPartENName"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPartENName"], "", "");
                    listInfoData[i]["vcCarfamilyCode"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcCarfamilyCode"], "", "");
                    listInfoData[i]["vcReceiver"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcReceiver"], "", "");
                    listInfoData[i]["dFromTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dFromTime"], "date", "");
                    listInfoData[i]["dToTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dToTime"], "date", "");
                    listInfoData[i]["vcPartId_Replace"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPartId_Replace"], "", "");
                    listInfoData[i]["vcInOut"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcInOut"], "", "");
                    listInfoData[i]["vcOESP"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcOESP"], "", "");
                    listInfoData[i]["vcHaoJiu"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcHaoJiu"], "", "");
                    listInfoData[i]["vcOldProduction"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcOldProduction"], "", "");
                    //listInfoData[i]["dOldStartTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dOldStartTime"], "date", "");
                    listInfoData[i]["dDebugTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dDebugTime"], "date", "");
                    listInfoData[i]["vcSupplierId"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSupplierId"], "", "");
                    listInfoData[i]["dSupplierFromTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dSupplierFromTime"], "date", "");
                    listInfoData[i]["dSupplierToTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dSupplierToTime"], "date", "");
                    listInfoData[i]["vcSupplierName"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSupplierName"], "", "");
                    listInfoData[i]["vcSupplierPlant"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSupplierPlant"], "", "");
                    listInfoData[i]["SupplierPlant_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SupplierPlant_ed"], "", "");
                    listInfoData[i]["SupplierPlantLinId_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SupplierPlantLinId_ed"], "", "");
                    listInfoData[i]["SupplierPlantFromTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SupplierPlantFromTime_ed"], "date", "");
                    listInfoData[i]["SupplierPlantToTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SupplierPlantToTime_ed"], "date", "");
                    listInfoData[i]["vcSupplierPlace"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSupplierPlace"], "", "");
                    listInfoData[i]["iPackingQty"] = fs0603_DataAccess.setNullValue(listInfoData[i]["iPackingQty"], "", "");
                    listInfoData[i]["vcBoxType"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcBoxType"], "", "");
                    listInfoData[i]["iLength"] = fs0603_DataAccess.setNullValue(listInfoData[i]["iLength"], "", "");
                    listInfoData[i]["iWidth"] = fs0603_DataAccess.setNullValue(listInfoData[i]["iWidth"], "", "");
                    listInfoData[i]["iHeight"] = fs0603_DataAccess.setNullValue(listInfoData[i]["iHeight"], "", "");
                    listInfoData[i]["iVolume"] = fs0603_DataAccess.setNullValue(listInfoData[i]["iVolume"], "", "");
                    listInfoData[i]["BoxPackingQty_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxPackingQty_ed"], "", "");
                    listInfoData[i]["BoxLinId_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxLinId_ed"], "", "");
                    listInfoData[i]["BoxFromTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxFromTime_ed"], "date", "");
                    listInfoData[i]["BoxToTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxToTime_ed"], "date", "");
                    listInfoData[i]["BoxType_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxType_ed"], "", "");
                    listInfoData[i]["BoxLength_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxLength_ed"], "", "");
                    listInfoData[i]["BoxWidth_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxWidth_ed"], "", "");
                    listInfoData[i]["BoxHeight_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxHeight_ed"], "", "");
                    listInfoData[i]["BoxVolume_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["BoxVolume_ed"], "", "");
                    listInfoData[i]["vcSufferIn"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSufferIn"], "", "");
                    listInfoData[i]["SufferIn_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SufferIn_ed"], "", "");
                    listInfoData[i]["SufferInLinId_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SufferInLinId_ed"], "", "");
                    listInfoData[i]["SufferInFromTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SufferInFromTime_ed"], "date", "");
                    listInfoData[i]["SufferInToTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["SufferInToTime_ed"], "date", "");
                    listInfoData[i]["vcOrderPlant"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcOrderPlant"], "", "");
                    //listInfoData[i]["OrderPlant_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["OrderPlant_ed"], "", "");
                    //listInfoData[i]["OrderPlantLinId_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["OrderPlantLinId_ed"], "", "");
                    //listInfoData[i]["OrderPlantFromTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["OrderPlantFromTime_ed"], "date", "");
                    //listInfoData[i]["OrderPlantToTime_ed"] = fs0603_DataAccess.setNullValue(listInfoData[i]["OrderPlantToTime_ed"], "date", "");
                    listInfoData[i]["vcInteriorProject"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcInteriorProject"], "", "");
                    listInfoData[i]["vcPassProject"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPassProject"], "", "");
                    listInfoData[i]["vcFrontProject"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcFrontProject"], "", "");
                    listInfoData[i]["dFrontProjectTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dFrontProjectTime"], "date", "");
                    listInfoData[i]["dShipmentTime"] = fs0603_DataAccess.setNullValue(listInfoData[i]["dShipmentTime"], "date", "");
                    listInfoData[i]["vcPartImage"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcPartImage"], "", "");
                    listInfoData[i]["vcBillType"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcBillType"], "", "");
                    listInfoData[i]["vcRemark1"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcRemark1"], "", "");
                    listInfoData[i]["vcRemark2"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcRemark2"], "", "");
                    listInfoData[i]["vcOrderingMethod"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcOrderingMethod"], "", "");
                    listInfoData[i]["vcMandOrder"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcMandOrder"], "", "");
                    listInfoData[i]["vcSupplierPacking"] = fs0603_DataAccess.setNullValue(listInfoData[i]["vcSupplierPacking"], "", "");
                }
                return listInfoData;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public DataTable setExportInfo(DataTable dtMainInfo, DataTable dtSPInfo, DataTable dtPQInfo, DataTable dtSIInfo, DataTable dtOPInfo, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtExport = createTable("export0603");
                for (int i = 0; i < dtMainInfo.Rows.Count; i++)
                {
                    #region 主数据
                    string strSyncTime = dtMainInfo.Rows[i]["dSyncTime"].ToString();
                    string strChanges_name = dtMainInfo.Rows[i]["vcChanges_name"].ToString();
                    string strPartENName = dtMainInfo.Rows[i]["vcPartENName"].ToString();
                    string strCarfamilyCode = dtMainInfo.Rows[i]["vcCarfamilyCode"].ToString();
                    string strFromTime = dtMainInfo.Rows[i]["dFromTime"].ToString();
                    string strToTime = dtMainInfo.Rows[i]["dToTime"].ToString();
                    string strPartId_Replace = dtMainInfo.Rows[i]["vcPartId_Replace"].ToString();
                    string strInOut_name = dtMainInfo.Rows[i]["vcInOut_name"].ToString();
                    string strOESP_name = dtMainInfo.Rows[i]["vcOESP_name"].ToString();
                    string strHaoJiu_name = dtMainInfo.Rows[i]["vcHaoJiu_name"].ToString();
                    string strOldProduction_name = dtMainInfo.Rows[i]["vcOldProduction_name"].ToString();
                    string strDebugTime = dtMainInfo.Rows[i]["dDebugTime"].ToString();
                    string strSupplierFromTime = dtMainInfo.Rows[i]["dSupplierFromTime"].ToString();
                    string strSupplierToTime = dtMainInfo.Rows[i]["dSupplierToTime"].ToString();
                    string strSupplierName = dtMainInfo.Rows[i]["vcSupplierName"].ToString();
                    string strSupplierPlace = dtMainInfo.Rows[i]["vcSupplierPlace"].ToString();
                    string strInteriorProject = dtMainInfo.Rows[i]["vcInteriorProject"].ToString();
                    string strPassProject = dtMainInfo.Rows[i]["vcPassProject"].ToString();
                    string strFrontProject = dtMainInfo.Rows[i]["vcFrontProject"].ToString();
                    string strFrontProjectTime = dtMainInfo.Rows[i]["dFrontProjectTime"].ToString();
                    string strShipmentTime = dtMainInfo.Rows[i]["dShipmentTime"].ToString();
                    string strPartImage = dtMainInfo.Rows[i]["vcPartImage"].ToString();
                    string strBillType_name = dtMainInfo.Rows[i]["vcBillType_name"].ToString();
                    string strRemark1 = dtMainInfo.Rows[i]["vcRemark1"].ToString();
                    string strRemark2 = dtMainInfo.Rows[i]["vcRemark2"].ToString();
                    string strOrderingMethod_name = dtMainInfo.Rows[i]["vcOrderingMethod_name"].ToString();
                    string strMandOrder_name = dtMainInfo.Rows[i]["vcMandOrder_name"].ToString();
                    string strSupplierPacking_name = dtMainInfo.Rows[i]["vcSupplierPacking_name"].ToString();
                    #endregion

                    DataTable dtExport_temp = createTable("export0603_temp");
                    string strPackingPlant = dtMainInfo.Rows[i]["vcPackingPlant"].ToString();
                    string strPackingPlant_name = dtMainInfo.Rows[i]["vcPackingPlant_name"].ToString();
                    string strPartId = dtMainInfo.Rows[i]["vcPartId"].ToString();
                    string strReceiver = dtMainInfo.Rows[i]["vcReceiver"].ToString();
                    string strSupplierId = dtMainInfo.Rows[i]["vcSupplierId"].ToString();

                    DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                    DataRow[] drPQInfo = dtPQInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                    DataRow[] drSIInfo = dtSIInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                    //DataRow[] drOPInfo = dtOPInfo.Select("vcSupplierId='" + strSupplierId + "'");
                    for (int j = 0; j < drSPInfo.Length; j++)
                    {
                        DataRow drExport_temp = dtExport_temp.NewRow();
                        drExport_temp["vcPackingPlant"] = strPackingPlant;
                        drExport_temp["vcPackingPlant_name"] = strPackingPlant_name;
                        drExport_temp["vcPartId"] = strPartId;
                        drExport_temp["vcReceiver"] = strReceiver;
                        drExport_temp["vcSupplierId"] = strSupplierId;
                        drExport_temp["dFromTime"] = drSPInfo[j]["dFromTime"].ToString();
                        drExport_temp["dToTime"] = drSPInfo[j]["dToTime"].ToString();
                        dtExport_temp.Rows.Add(drExport_temp);
                    }
                    for (int j = 0; j < drPQInfo.Length; j++)
                    {
                        DataRow drExport_temp = dtExport_temp.NewRow();
                        drExport_temp["vcPackingPlant"] = strPackingPlant;
                        drExport_temp["vcPackingPlant_name"] = strPackingPlant_name;
                        drExport_temp["vcPartId"] = strPartId;
                        drExport_temp["vcReceiver"] = strReceiver;
                        drExport_temp["vcSupplierId"] = strSupplierId;
                        drExport_temp["dFromTime"] = drPQInfo[j]["dFromTime"].ToString();
                        drExport_temp["dToTime"] = drPQInfo[j]["dToTime"].ToString();
                        dtExport_temp.Rows.Add(drExport_temp);
                    }
                    for (int j = 0; j < drSIInfo.Length; j++)
                    {
                        DataRow drExport_temp = dtExport_temp.NewRow();
                        drExport_temp["vcPackingPlant"] = strPackingPlant;
                        drExport_temp["vcPackingPlant_name"] = strPackingPlant_name;
                        drExport_temp["vcPartId"] = strPartId;
                        drExport_temp["vcReceiver"] = strReceiver;
                        drExport_temp["vcSupplierId"] = strSupplierId;
                        drExport_temp["dFromTime"] = drSIInfo[j]["dFromTime"].ToString();
                        drExport_temp["dToTime"] = drSIInfo[j]["dToTime"].ToString();
                        dtExport_temp.Rows.Add(drExport_temp);
                    }
                    //for (int j = 0; j < drOPInfo.Length; j++)
                    //{
                    //    DataRow drExport_temp = dtExport_temp.NewRow();
                    //    drExport_temp["vcPackingPlant"] = strPackingPlant;
                    //    drExport_temp["vcPackingPlant_name"] = strPackingPlant_name;
                    //    drExport_temp["vcPartId"] = strPartId;
                    //    drExport_temp["vcReceiver"] = strReceiver;
                    //    drExport_temp["vcSupplierId"] = strSupplierId;
                    //    drExport_temp["dFromTime"] = drOPInfo[j]["dFromTime"].ToString();
                    //    drExport_temp["dToTime"] = drOPInfo[j]["dToTime"].ToString();
                    //    dtExport_temp.Rows.Add(drExport_temp);
                    //}
                    dtExport_temp = dtExport_temp.DefaultView.ToTable(true, "vcPackingPlant", "vcPackingPlant_name", "vcPartId", "vcReceiver", "vcSupplierId", "dFromTime", "dToTime");
                    dtExport_temp.DefaultView.Sort = "dToTime";
                    dtExport_temp = dtExport_temp.DefaultView.ToTable();
                    if (dtExport_temp.Rows.Count != 0)
                    {
                        for (int k = 0; k < dtExport_temp.Rows.Count; k++)
                        {
                            DateTime dFromTime_temp = Convert.ToDateTime(dtExport_temp.Rows[k]["dFromTime"].ToString());
                            DateTime dToTime_temp = Convert.ToDateTime(dtExport_temp.Rows[k]["dToTime"].ToString());

                            #region SupplierPlant
                            string strSupplierPlant = "";
                            string strSupplierPlantFromTime = "";
                            string strSupplierPlantToTime = "";
                            for (int q = 0; q < drSPInfo.Length; q++)
                            {
                                string strSupplierPlant_fixed = drSPInfo[q]["vcSupplierPlant"].ToString();
                                DateTime dFromTime_fixed = Convert.ToDateTime(drSPInfo[q]["dFromTime"].ToString());
                                DateTime dToTime_fixed = Convert.ToDateTime(drSPInfo[q]["dToTime"].ToString());
                                if (dToTime_temp >= dFromTime_fixed && dToTime_temp <= dToTime_fixed)
                                {
                                    strSupplierPlant = strSupplierPlant_fixed;
                                    strSupplierPlantFromTime = dFromTime_fixed.ToString("yyyy/MM/dd");
                                    strSupplierPlantToTime = dToTime_fixed.ToString("yyyy/MM/dd");
                                    break;
                                }
                            }
                            #endregion

                            #region PackingQty
                            string strPackingQty = "";
                            string strBoxType = "";
                            string strLength = "";
                            string strWidth = "";
                            string strHeight = "";
                            string strVolume = "";
                            string strBoxFromTime = "";
                            string strBoxToTime = "";
                            for (int q = 0; q < drPQInfo.Length; q++)
                            {
                                string strPackingQty_fixed = drPQInfo[q]["iPackingQty"].ToString();
                                string strBoxType_fixed = drPQInfo[q]["vcBoxType"].ToString();
                                string strLength_fixed = drPQInfo[q]["iLength"].ToString();
                                string strWidth_fixed = drPQInfo[q]["iWidth"].ToString();
                                string strHeight_fixed = drPQInfo[q]["iHeight"].ToString();
                                string strVolume_fixed = drPQInfo[q]["iVolume"].ToString();
                                DateTime dFromTime_fixed = Convert.ToDateTime(drPQInfo[q]["dFromTime"].ToString());
                                DateTime dToTime_fixed = Convert.ToDateTime(drPQInfo[q]["dToTime"].ToString());
                                if (dToTime_temp >= dFromTime_fixed && dToTime_temp <= dToTime_fixed)
                                {
                                    strPackingQty = strPackingQty_fixed;
                                    strBoxType = strBoxType_fixed;
                                    strLength = strLength_fixed;
                                    strWidth = strWidth_fixed;
                                    strHeight = strHeight_fixed;
                                    strVolume = strVolume_fixed;
                                    strBoxFromTime = dFromTime_fixed.ToString("yyyy/MM/dd");
                                    strBoxToTime = dToTime_fixed.ToString("yyyy/MM/dd");
                                    break;
                                }
                            }
                            #endregion

                            #region SufferIn
                            string strSufferIn = "";
                            string strSufferInFromTime = "";
                            string strSufferInToTime = "";
                            for (int q = 0; q < drSIInfo.Length; q++)
                            {
                                string strSufferIn_fixed = drSIInfo[q]["vcSufferIn"].ToString();
                                DateTime dFromTime_fixed = Convert.ToDateTime(drSIInfo[q]["dFromTime"].ToString());
                                DateTime dToTime_fixed = Convert.ToDateTime(drSIInfo[q]["dToTime"].ToString());
                                if (dToTime_temp >= dFromTime_fixed && dToTime_temp <= dToTime_fixed)
                                {
                                    strSufferIn = strSufferIn_fixed;
                                    strSufferInFromTime = dFromTime_fixed.ToString("yyyy/MM/dd");
                                    strSufferInToTime = dToTime_fixed.ToString("yyyy/MM/dd");
                                    break;
                                }
                            }
                            #endregion

                            #region OrderPlant
                            DataRow[] drOPInfo = dtOPInfo.Select("vcSupplierId='" + strSupplierId + "' and vcSupplierPlant='" + strSupplierPlant + "'");

                            string strOrderPlant_name = "";
                            string strOrderPlantFromTime = "";
                            string strOrderPlantToTime = "";
                            for (int q = 0; q < drOPInfo.Length; q++)
                            {
                                string strOrderPlant_name_fixed = drOPInfo[q]["vcOrderPlant_name"].ToString();
                                DateTime dFromTime_fixed = Convert.ToDateTime(drOPInfo[q]["dFromTime"].ToString());
                                DateTime dToTime_fixed = Convert.ToDateTime(drOPInfo[q]["dToTime"].ToString());
                                if (dToTime_temp >= dFromTime_fixed && dToTime_temp <= dToTime_fixed)
                                {
                                    strOrderPlant_name = strOrderPlant_name_fixed;
                                    strOrderPlantFromTime = dFromTime_fixed.ToString("yyyy/MM/dd");
                                    strOrderPlantToTime = dToTime_fixed.ToString("yyyy/MM/dd");
                                    break;
                                }
                            }
                            #endregion

                            #region AddToTable
                            DataRow drExport = dtExport.NewRow();
                            drExport["dSyncTime"] = strSyncTime;
                            drExport["vcChanges_name"] = strChanges_name;
                            drExport["vcPackingPlant_name"] = strPackingPlant_name;
                            drExport["vcPartId"] = strPartId;
                            drExport["vcPartENName"] = strPartENName;
                            drExport["vcCarfamilyCode"] = strCarfamilyCode;
                            drExport["vcReceiver"] = strReceiver;
                            drExport["dFromTime"] = strFromTime;
                            drExport["dToTime"] = dToTime_temp.ToString("yyyy/MM/dd");
                            drExport["vcPartId_Replace"] = strPartId_Replace;
                            drExport["vcInOut_name"] = strInOut_name;
                            drExport["vcOESP_name"] = strOESP_name;
                            drExport["vcHaoJiu_name"] = strHaoJiu_name;
                            drExport["vcOldProduction_name"] = strOldProduction_name;
                            drExport["dDebugTime"] = strDebugTime;
                            drExport["vcSupplierId"] = strSupplierId;
                            drExport["dSupplierFromTime"] = strSupplierFromTime;
                            drExport["dSupplierToTime"] = strSupplierToTime;
                            drExport["vcSupplierName"] = strSupplierName;
                            drExport["vcSupplierPlant"] = strSupplierPlant;
                            drExport["dSupplierPlantFromTime"] = strSupplierPlantFromTime;
                            drExport["dSupplierPlantToTime"] = strSupplierPlantToTime;
                            drExport["vcSupplierPlace"] = strSupplierPlace;
                            drExport["iPackingQty"] = strPackingQty;
                            drExport["vcBoxType"] = strBoxType;
                            drExport["iLength"] = strLength;
                            drExport["iWidth"] = strWidth;
                            drExport["iHeight"] = strHeight;
                            drExport["iVolume"] = strVolume;
                            drExport["dBoxFromTime"] = strBoxFromTime;
                            drExport["dBoxToTime"] = strBoxToTime;
                            drExport["vcSufferIn"] = strSufferIn;
                            drExport["dSufferInFromTime"] = strSufferInFromTime;
                            drExport["dSufferInToTime"] = strSufferInToTime;
                            drExport["vcOrderPlant_name"] = strOrderPlant_name;
                            drExport["dOrderPlantFromTime"] = strOrderPlantFromTime;
                            drExport["dOrderPlantToTime"] = strOrderPlantToTime;
                            drExport["vcInteriorProject"] = strInteriorProject;
                            drExport["vcPassProject"] = strPassProject;
                            drExport["vcFrontProject"] = strFrontProject;
                            drExport["dFrontProjectTime"] = strFrontProjectTime;
                            drExport["dShipmentTime"] = strShipmentTime;
                            drExport["vcPartImage"] = strPartImage;
                            drExport["vcBillType_name"] = strBillType_name;
                            drExport["vcRemark1"] = strRemark1;
                            drExport["vcRemark2"] = strRemark2;
                            drExport["vcOrderingMethod_name"] = strOrderingMethod_name;
                            drExport["vcMandOrder_name"] = strMandOrder_name;
                            drExport["vcSupplierPacking_name"] = strSupplierPacking_name;
                            #endregion
                            dtExport.Rows.Add(drExport);
                        }
                    }
                    else
                    {
                        #region AddToTable
                        DataRow drExport = dtExport.NewRow();
                        drExport["dSyncTime"] = strSyncTime;
                        drExport["vcChanges_name"] = strChanges_name;
                        drExport["vcPackingPlant_name"] = strPackingPlant_name;
                        drExport["vcPartId"] = strPartId;
                        drExport["vcPartENName"] = strPartENName;
                        drExport["vcCarfamilyCode"] = strCarfamilyCode;
                        drExport["vcReceiver"] = strReceiver;
                        drExport["dFromTime"] = strFromTime;
                        drExport["dToTime"] = strToTime;
                        drExport["vcPartId_Replace"] = strPartId_Replace;
                        drExport["vcInOut_name"] = strInOut_name;
                        drExport["vcOESP_name"] = strOESP_name;
                        drExport["vcHaoJiu_name"] = strHaoJiu_name;
                        drExport["vcOldProduction_name"] = strOldProduction_name;
                        drExport["dDebugTime"] = strDebugTime;
                        drExport["vcSupplierId"] = strSupplierId;
                        drExport["dSupplierFromTime"] = strSupplierFromTime;
                        drExport["dSupplierToTime"] = strSupplierToTime;
                        drExport["vcSupplierName"] = strSupplierName;
                        drExport["vcSupplierPlant"] = string.Empty;
                        drExport["dSupplierPlantFromTime"] = string.Empty;
                        drExport["dSupplierPlantToTime"] = string.Empty;
                        drExport["vcSupplierPlace"] = strSupplierPlace;
                        drExport["iPackingQty"] = string.Empty;
                        drExport["vcBoxType"] = string.Empty;
                        drExport["iLength"] = string.Empty;
                        drExport["iWidth"] = string.Empty;
                        drExport["iHeight"] = string.Empty;
                        drExport["iVolume"] = string.Empty;
                        drExport["dBoxFromTime"] = string.Empty;
                        drExport["dBoxToTime"] = string.Empty;
                        drExport["vcSufferIn"] = string.Empty;
                        drExport["dSufferInFromTime"] = string.Empty;
                        drExport["dSufferInToTime"] = string.Empty;
                        drExport["vcOrderPlant_name"] = string.Empty;
                        drExport["dOrderPlantFromTime"] = string.Empty;
                        drExport["dOrderPlantToTime"] = string.Empty;
                        drExport["vcInteriorProject"] = strInteriorProject;
                        drExport["vcPassProject"] = strPassProject;
                        drExport["vcFrontProject"] = strFrontProject;
                        drExport["dFrontProjectTime"] = strFrontProjectTime;
                        drExport["dShipmentTime"] = strShipmentTime;
                        drExport["vcPartImage"] = strPartImage;
                        drExport["vcBillType_name"] = strBillType_name;
                        drExport["vcRemark1"] = strRemark1;
                        drExport["vcRemark2"] = strRemark2;
                        drExport["vcOrderingMethod_name"] = strOrderingMethod_name;
                        drExport["vcMandOrder_name"] = strMandOrder_name;
                        drExport["vcSupplierPacking_name"] = strSupplierPacking_name;
                        #endregion
                        dtExport.Rows.Add(drExport);
                    }
                }


                return dtExport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getPrinterName(string strPageId, string strOperId)
        {
            try
            {
                return fs0603_DataAccess.getPrinterName(strPageId, strOperId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "Options")
            {
                dataTable.Columns.Add("vcName");
                dataTable.Columns.Add("vcValue");
            }
            if (strSpSub == "SpotQty1402")
            {
                dataTable.Columns.Add("iPackingQty");
                dataTable.Columns.Add("iSpotQty");
            }
            if (strSpSub == "SP")
            {
                dataTable.Columns.Add("LinId");
                dataTable.Columns.Add("vcPackingPlant");
                dataTable.Columns.Add("vcPartId");
                dataTable.Columns.Add("vcReceiver");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("dFromTime");
                dataTable.Columns.Add("dToTime");
                dataTable.Columns.Add("vcSupplierPlant");
                dataTable.Columns.Add("status");
                dataTable.Columns.Add("error");
            }
            if (strSpSub == "PQ")
            {
                dataTable.Columns.Add("LinId");
                dataTable.Columns.Add("vcPackingPlant");
                dataTable.Columns.Add("vcPartId");
                dataTable.Columns.Add("vcReceiver");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("vcSupplierPlant");
                dataTable.Columns.Add("dFromTime");
                dataTable.Columns.Add("dToTime");
                dataTable.Columns.Add("iPackingQty");
                dataTable.Columns.Add("vcBoxType");
                dataTable.Columns.Add("iLength");
                dataTable.Columns.Add("iWidth");
                dataTable.Columns.Add("iHeight");
                dataTable.Columns.Add("iVolume");
                dataTable.Columns.Add("status");
                dataTable.Columns.Add("error");
            }
            if (strSpSub == "SI")
            {
                dataTable.Columns.Add("LinId");
                dataTable.Columns.Add("vcPackingPlant");
                dataTable.Columns.Add("vcPartId");
                dataTable.Columns.Add("vcReceiver");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("dFromTime");
                dataTable.Columns.Add("dToTime");
                dataTable.Columns.Add("vcSufferIn");
                dataTable.Columns.Add("status");
                dataTable.Columns.Add("error");
            }
            if (strSpSub == "OP")
            {
                dataTable.Columns.Add("LinId");
                dataTable.Columns.Add("vcPackingPlant");
                dataTable.Columns.Add("vcPartId");
                dataTable.Columns.Add("vcReceiver");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("dFromTime");
                dataTable.Columns.Add("dToTime");
                dataTable.Columns.Add("vcOrderPlant");
                dataTable.Columns.Add("status");
                dataTable.Columns.Add("error");
            }
            if (strSpSub == "OH")
            {
                dataTable.Columns.Add("vcPackingPlant");
                dataTable.Columns.Add("vcPartId");
                dataTable.Columns.Add("vcReceiver");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("vcChangeItem");
                dataTable.Columns.Add("vcAction");
                dataTable.Columns.Add("vcActionTime");
                dataTable.Columns.Add("error");
            }
            if (strSpSub == "SPChild")
            {
                dataTable.Columns.Add("vcPackingPlant");
                dataTable.Columns.Add("vcPartId");
                dataTable.Columns.Add("vcReceiver");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("dFromTime");
                dataTable.Columns.Add("dToTime");
                dataTable.Columns.Add("vcFlag");
            }
            if (strSpSub == "MES")
            {
                dataTable.Columns.Add("vcMessage");
            }
            if (strSpSub == "Multipleof616")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcOrderNo", typeof(string));
                dataTable.Columns.Add("vcPart_id", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("vcSupplierPlant", typeof(string));
                dataTable.Columns.Add("iPackingQty", typeof(int));
                dataTable.Columns.Add("iOrderQuantity", typeof(int));
                dataTable.Columns.Add("iDuiYingQuantity", typeof(int));
                dataTable.Columns.Add("decBoxQuantity", typeof(decimal));
                dataTable.Columns.Add("dDeliveryDate", typeof(string));
                dataTable.Columns.Add("dOutPutDate", typeof(string));
                dataTable.Columns.Add("dReplyOverDate", typeof(string));
                dataTable.Columns.Add("vcInputType", typeof(string));//company
            }
            if (strSpSub == "Query811")
            {
                dataTable.Columns.Add("uuid", typeof(string));
                dataTable.Columns.Add("vcBigPM", typeof(string));
                dataTable.Columns.Add("vcSmallPM", typeof(string));
                dataTable.Columns.Add("vcStandard", typeof(string));
                dataTable.Columns.Add("decPackNum", typeof(string));
                dataTable.Columns.Add("decPlannedTime", typeof(string));
                dataTable.Columns.Add("decPlannedPerson", typeof(string));
                dataTable.Columns.Add("decInputPerson", typeof(string));
                dataTable.Columns.Add("decInputTime", typeof(string));
                dataTable.Columns.Add("decOverFlowTime", typeof(string));
                dataTable.Columns.Add("decSysLander", typeof(string));
                dataTable.Columns.Add("decDiffer", typeof(string));
            }
            if (strSpSub == "Power1301")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcUserId", typeof(string));
                dataTable.Columns.Add("vcPlant", typeof(string));
                dataTable.Columns.Add("vcInPut", typeof(string));
                dataTable.Columns.Add("vcInPutUnLock", typeof(string));
                dataTable.Columns.Add("vcCheck", typeof(string));
                dataTable.Columns.Add("vcCheckUnLock", typeof(string));
                dataTable.Columns.Add("vcPack", typeof(string));
                dataTable.Columns.Add("vcPackUnLock", typeof(string));
                dataTable.Columns.Add("vcOutPut", typeof(string));
                dataTable.Columns.Add("vcOutPutUnLock", typeof(string));
            }
            if (strSpSub == "Power1310")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcPlant", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("vcPicUrl", typeof(string));
                dataTable.Columns.Add("vcPicPath", typeof(string));
                dataTable.Columns.Add("vcPicUrlUUID", typeof(string));
                dataTable.Columns.Add("vcNote", typeof(string));
            }
            if (strSpSub == "SOQ602")
            {
                dataTable.Columns.Add("vcYearMonth", typeof(string));
                dataTable.Columns.Add("vcDyState", typeof(string));
                dataTable.Columns.Add("vcHyState", typeof(string));
                dataTable.Columns.Add("vcPart_id", typeof(string));
                dataTable.Columns.Add("iCbSOQN", typeof(string));
                dataTable.Columns.Add("decCbBdl", typeof(string));
                dataTable.Columns.Add("iCbSOQN1", typeof(string));
                dataTable.Columns.Add("iCbSOQN2", typeof(string));
                dataTable.Columns.Add("iTzhSOQN", typeof(string));
                dataTable.Columns.Add("iTzhSOQN1", typeof(string));
                dataTable.Columns.Add("iTzhSOQN2", typeof(string));
                dataTable.Columns.Add("iHySOQN", typeof(string));
                dataTable.Columns.Add("iHySOQN1", typeof(string));
                dataTable.Columns.Add("iHySOQN2", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("vcSupplierPlant", typeof(string));
                dataTable.Columns.Add("dExpectTime", typeof(string));
                dataTable.Columns.Add("vcInputType", typeof(string));
            }
            if (strSpSub == "mainFs0617")
            {
                dataTable.Columns.Add("UUID1", typeof(string));
                dataTable.Columns.Add("UUID2", typeof(string));
                dataTable.Columns.Add("UUID3", typeof(string));
            }
            if (strSpSub == "print1103")
            {
                dataTable.Columns.Add("vcReceiver", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("vcInPutOrderNo", typeof(string));
                dataTable.Columns.Add("vcLabelNum", typeof(string));
                dataTable.Columns.Add("bInPutOrder", typeof(string));
                dataTable.Columns.Add("bTag", typeof(string));
                dataTable.Columns.Add("vcTagLianFFrom", typeof(string));
                dataTable.Columns.Add("vcTagLianFTo", typeof(string));
            }
            if (strSpSub == "updtFs1401")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcSPISStatus", typeof(string));
            }
            if (strSpSub == "savFs1402")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("dFromTime", typeof(string));
                dataTable.Columns.Add("dToTime", typeof(string));
                dataTable.Columns.Add("vcCarfamilyCode", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("vcSupplierPlant", typeof(string));
                dataTable.Columns.Add("vcCheckP", typeof(string));
                dataTable.Columns.Add("vcChangeRea", typeof(string));
                dataTable.Columns.Add("vcTJSX", typeof(string));
                dataTable.Columns.Add("vcType", typeof(string));
            }
            if (strSpSub == "savFs1404")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("dFromTime", typeof(string));
                dataTable.Columns.Add("dToTime", typeof(string));
                dataTable.Columns.Add("vcCarfamilyCode", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("vcSupplierPlant", typeof(string));
                dataTable.Columns.Add("vcPicUrl", typeof(string));
                dataTable.Columns.Add("vcPicUrlUUID", typeof(string));
                dataTable.Columns.Add("vcChangeRea", typeof(string));
                dataTable.Columns.Add("vcTJSX", typeof(string));
                dataTable.Columns.Add("vcType", typeof(string));
            }
            if (strSpSub == "export0603")
            {
                dataTable.Columns.Add("dSyncTime", typeof(string));
                dataTable.Columns.Add("vcChanges_name", typeof(string));
                dataTable.Columns.Add("vcPackingPlant_name", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("vcPartENName", typeof(string));
                dataTable.Columns.Add("vcCarfamilyCode", typeof(string));
                dataTable.Columns.Add("vcReceiver", typeof(string));
                dataTable.Columns.Add("dFromTime", typeof(string));
                dataTable.Columns.Add("dToTime", typeof(string));
                dataTable.Columns.Add("vcPartId_Replace", typeof(string));
                dataTable.Columns.Add("vcInOut_name", typeof(string));
                dataTable.Columns.Add("vcOESP_name", typeof(string));
                dataTable.Columns.Add("vcHaoJiu_name", typeof(string));
                dataTable.Columns.Add("vcOldProduction_name", typeof(string));
                dataTable.Columns.Add("dDebugTime", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("dSupplierFromTime", typeof(string));
                dataTable.Columns.Add("dSupplierToTime", typeof(string));
                dataTable.Columns.Add("vcSupplierName", typeof(string));
                dataTable.Columns.Add("vcSupplierPlant", typeof(string));
                dataTable.Columns.Add("dSupplierPlantFromTime", typeof(string));
                dataTable.Columns.Add("dSupplierPlantToTime", typeof(string));
                dataTable.Columns.Add("vcSupplierPlace", typeof(string));
                dataTable.Columns.Add("iPackingQty", typeof(string));
                dataTable.Columns.Add("vcBoxType", typeof(string));
                dataTable.Columns.Add("iLength", typeof(string));
                dataTable.Columns.Add("iWidth", typeof(string));
                dataTable.Columns.Add("iHeight", typeof(string));
                dataTable.Columns.Add("iVolume", typeof(string));
                dataTable.Columns.Add("dBoxFromTime", typeof(string));
                dataTable.Columns.Add("dBoxToTime", typeof(string));
                dataTable.Columns.Add("vcSufferIn", typeof(string));
                dataTable.Columns.Add("dSufferInFromTime", typeof(string));
                dataTable.Columns.Add("dSufferInToTime", typeof(string));
                dataTable.Columns.Add("vcOrderPlant_name", typeof(string));
                dataTable.Columns.Add("dOrderPlantFromTime", typeof(string));
                dataTable.Columns.Add("dOrderPlantToTime", typeof(string));
                dataTable.Columns.Add("vcInteriorProject", typeof(string));
                dataTable.Columns.Add("vcPassProject", typeof(string));
                dataTable.Columns.Add("vcFrontProject", typeof(string));
                dataTable.Columns.Add("dFrontProjectTime", typeof(string));
                dataTable.Columns.Add("dShipmentTime", typeof(string));
                dataTable.Columns.Add("vcPartImage", typeof(string));
                dataTable.Columns.Add("vcBillType_name", typeof(string));
                dataTable.Columns.Add("vcRemark1", typeof(string));
                dataTable.Columns.Add("vcRemark2", typeof(string));
                dataTable.Columns.Add("vcOrderingMethod_name", typeof(string));
                dataTable.Columns.Add("vcMandOrder_name", typeof(string));
                dataTable.Columns.Add("vcSupplierPacking_name", typeof(string));
            }
            if (strSpSub == "export0603_temp")
            {
                dataTable.Columns.Add("vcPackingPlant", typeof(string));
                dataTable.Columns.Add("vcPackingPlant_name", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("vcReceiver", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("dFromTime", typeof(string));
                dataTable.Columns.Add("dToTime", typeof(string));
            }
            if (strSpSub == "SyncData")
            {
                dataTable.Columns.Add("dSyncTime", typeof(string));
                dataTable.Columns.Add("vcChanges", typeof(string));
            }
            if (strSpSub == "mailaddress")
            {
                dataTable.Columns.Add("address", typeof(string));
                dataTable.Columns.Add("displayName", typeof(string));
            }
            if (strSpSub == "ExportRef0602")
            {
                dataTable.Columns.Add("vcExportDate", typeof(string));
                dataTable.Columns.Add("vcRefDate", typeof(string));
                dataTable.Columns.Add("vcMonth_dx", typeof(string));
                dataTable.Columns.Add("vcMonth_ns", typeof(string));
                dataTable.Columns.Add("vcMonth_nns", typeof(string));
                dataTable.Columns.Add("decDXYNum", typeof(string));
                dataTable.Columns.Add("decNSYNum", typeof(string));
                dataTable.Columns.Add("decNNSYNum", typeof(string));
                dataTable.Columns.Add("vcProject", typeof(string));
                dataTable.Columns.Add("vcYearMonth_dx", typeof(string));
                dataTable.Columns.Add("decSOQ_dx", typeof(string));
                dataTable.Columns.Add("decSOQ_ns_before", typeof(string));
                dataTable.Columns.Add("vcYearMonth_ns", typeof(string));
                dataTable.Columns.Add("decSOQ_ns", typeof(string));
                dataTable.Columns.Add("decSOQ_nns_before", typeof(string));
                dataTable.Columns.Add("vcYearMonth_nns", typeof(string));
                dataTable.Columns.Add("decSOQ_nns", typeof(string));
                dataTable.Columns.Add("decNNA_dx", typeof(string));
                dataTable.Columns.Add("decNNA_ns", typeof(string));
                dataTable.Columns.Add("decNNA_nns", typeof(string));
            }
            if (strSpSub == "KanBan1306")
            {
                dataTable.Columns.Add("vcProgect", typeof(string));
                dataTable.Columns.Add("vcDeng", typeof(string));
                dataTable.Columns.Add("vcZhongxw", typeof(string));
                dataTable.Columns.Add("vcDaw", typeof(string));
                dataTable.Columns.Add("vcBol", typeof(string));
                dataTable.Columns.Add("vcBanj", typeof(string));
                dataTable.Columns.Add("vcChengx", typeof(string));
                dataTable.Columns.Add("vcHej", typeof(string));

                DataRow dataRow1 = dataTable.NewRow();
                dataRow1["vcProgect"] = "计划数量";
                dataRow1["vcDeng"] = "0.00";
                dataRow1["vcZhongxw"] = "0.00";
                dataRow1["vcDaw"] = "0.00";
                dataRow1["vcBol"] = "0.00";
                dataRow1["vcBanj"] = "0.00";
                dataRow1["vcChengx"] = "0.00";
                dataRow1["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow1);

                DataRow dataRow2 = dataTable.NewRow();
                dataRow2["vcProgect"] = "计划投入人员";
                dataRow2["vcDeng"] = "0.00";
                dataRow2["vcZhongxw"] = "0.00";
                dataRow2["vcDaw"] = "0.00";
                dataRow2["vcBol"] = "0.00";
                dataRow2["vcBanj"] = "0.00";
                dataRow2["vcChengx"] = "0.00";
                dataRow2["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow2);

                DataRow dataRow3 = dataTable.NewRow();
                dataRow3["vcProgect"] = "实绩投入人员";
                dataRow3["vcDeng"] = "0.00";
                dataRow3["vcZhongxw"] = "0.00";
                dataRow3["vcDaw"] = "0.00";
                dataRow3["vcBol"] = "0.00";
                dataRow3["vcBanj"] = "0.00";
                dataRow3["vcChengx"] = "0.00";
                dataRow3["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow3);

                DataRow dataRow4 = dataTable.NewRow();
                dataRow4["vcProgect"] = "投入人员差";
                dataRow4["vcDeng"] = "0.00";
                dataRow4["vcZhongxw"] = "0.00";
                dataRow4["vcDaw"] = "0.00";
                dataRow4["vcBol"] = "0.00";
                dataRow4["vcBanj"] = "0.00";
                dataRow4["vcChengx"] = "0.00";
                dataRow4["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow4);

                DataRow dataRow5 = dataTable.NewRow();
                dataRow5["vcProgect"] = "当日作业计划";
                dataRow5["vcDeng"] = "0.00";
                dataRow5["vcZhongxw"] = "0.00";
                dataRow5["vcDaw"] = "0.00";
                dataRow5["vcBol"] = "0.00";
                dataRow5["vcBanj"] = "0.00";
                dataRow5["vcChengx"] = "0.00";
                dataRow5["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow5);

                DataRow dataRow6 = dataTable.NewRow();
                dataRow6["vcProgect"] = "现时点计划数量";
                dataRow6["vcDeng"] = "0.00";
                dataRow6["vcZhongxw"] = "0.00";
                dataRow6["vcDaw"] = "0.00";
                dataRow6["vcBol"] = "0.00";
                dataRow6["vcBanj"] = "0.00";
                dataRow6["vcChengx"] = "0.00";
                dataRow6["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow6);

                DataRow dataRow7 = dataTable.NewRow();
                dataRow7["vcProgect"] = "实绩完成数量";
                dataRow7["vcDeng"] = "0.00";
                dataRow7["vcZhongxw"] = "0.00";
                dataRow7["vcDaw"] = "0.00";
                dataRow7["vcBol"] = "0.00";
                dataRow7["vcBanj"] = "0.00";
                dataRow7["vcChengx"] = "0.00";
                dataRow7["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow7);

                DataRow dataRow8 = dataTable.NewRow();
                dataRow8["vcProgect"] = "现实点作业数量差";
                dataRow8["vcDeng"] = "0.00";
                dataRow8["vcZhongxw"] = "0.00";
                dataRow8["vcDaw"] = "0.00";
                dataRow8["vcBol"] = "0.00";
                dataRow8["vcBanj"] = "0.00";
                dataRow8["vcChengx"] = "0.00";
                dataRow8["vcHej"] = "0.00";
                dataTable.Rows.Add(dataRow8);

                DataRow dataRow9 = dataTable.NewRow();
                dataRow9["vcProgect"] = "现实点工时完成率";
                dataRow9["vcDeng"] = "-";
                dataRow9["vcZhongxw"] = "-";
                dataRow9["vcDaw"] = "-";
                dataRow9["vcBol"] = "-";
                dataRow9["vcBanj"] = "-";
                dataRow9["vcChengx"] = "-";
                dataRow9["vcHej"] = "-";
                dataTable.Rows.Add(dataRow9);

                DataRow dataRow10 = dataTable.NewRow();
                dataRow10["vcProgect"] = "总体工时完成率";
                dataRow10["vcDeng"] = "-";
                dataRow10["vcZhongxw"] = "-";
                dataRow10["vcDaw"] = "-";
                dataRow10["vcBol"] = "-";
                dataRow10["vcBanj"] = "-";
                dataRow10["vcChengx"] = "-";
                dataRow10["vcHej"] = "-";
                dataTable.Rows.Add(dataRow10);
            }
            if (strSpSub == "Print1103")
            {
                dataTable.Columns.Add("vcReceiver", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("vcInPutOrderNo", typeof(string));
                dataTable.Columns.Add("bInPutOrder", typeof(string));
                dataTable.Columns.Add("bTag", typeof(string));
                dataTable.Columns.Add("vcTagLianFFrom", typeof(string));
                dataTable.Columns.Add("vcTagLianFTo", typeof(string));
            }
            if (strSpSub == "SPISApply")
            {
                dataTable.Columns.Add("LinId", typeof(string));
                dataTable.Columns.Add("vcApplyId", typeof(string));
                dataTable.Columns.Add("dFromTime_SPIS", typeof(string));
                dataTable.Columns.Add("dToTime_SPIS", typeof(string));
                dataTable.Columns.Add("dSPISTime", typeof(string));
                dataTable.Columns.Add("vcPartId", typeof(string));
                dataTable.Columns.Add("vcCarfamilyCode", typeof(string));
                dataTable.Columns.Add("vcSupplierId", typeof(string));
                dataTable.Columns.Add("vcSupplierName", typeof(string));
                dataTable.Columns.Add("vcPartENName", typeof(string));
                dataTable.Columns.Add("vcColourNo", typeof(string));
                dataTable.Columns.Add("vcColourCode", typeof(string));
                dataTable.Columns.Add("vcColourName", typeof(string));
                dataTable.Columns.Add("dModDate", typeof(string));
                dataTable.Columns.Add("vcModItem", typeof(string));
                dataTable.Columns.Add("vcTempUrl", typeof(string));
                dataTable.Columns.Add("vcPICUrl", typeof(string));
                dataTable.Columns.Add("vcPICPath", typeof(string));
                dataTable.Columns.Add("vcPDFUrl", typeof(string));
                dataTable.Columns.Add("vcPDFPath", typeof(string));
                dataTable.Columns.Add("vcSPISUrl", typeof(string));
                dataTable.Columns.Add("vcSPISPath", typeof(string));
                dataTable.Columns.Add("vcSupplier_1", typeof(string));
                dataTable.Columns.Add("vcSupplier_2", typeof(string));
                dataTable.Columns.Add("vcOperName", typeof(string));
                dataTable.Columns.Add("vcGM", typeof(string));
                dataTable.Columns.Add("vcSPISStatus", typeof(string));
                dataTable.Columns.Add("vcType", typeof(string));
            }
            return dataTable;
        }
    }
}
