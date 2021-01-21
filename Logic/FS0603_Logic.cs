using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

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
        public DataTable getSearchInfo(string strSyncTime, string strPartId, string strCarModel, string strReceiver, string strInOut,
                    string strSupplierId, string strSupplierPlant, string strFromTime, string strToTime, string strHaoJiu, string strOrderPlant)
        {
            DataTable dataTable = fs0603_DataAccess.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut,
                    strSupplierId, strSupplierPlant, strFromTime, strToTime, strHaoJiu, strOrderPlant);
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
                if (dataRow["vcPartImage"].ToString() == string.Empty)
                    dataRow["vcPartImage"] = "暂无图像.jpg";
            }
            return dataTable;
        }
        public void setSPInfo(List<Dictionary<string, Object>> listInfoData, string strOperId, ref string strErrorPartId)
        {
            //用于检查变更情况
            DataTable dtOperCheck = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "", "", "", "", "");
            //用于新增数据
            DataTable dtAddInfo = dtOperCheck.Clone();
            //用于修改数据--修改主数据表
            DataTable dtModInfo = dtOperCheck.Clone();
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
            //处理List集合
            listInfoData = setNullData(listInfoData);
            for (int i = 0; i < listInfoData.Count; i++)
            {
                bool bModFlag = (bool)listInfoData[i]["bModFlag"];//true可编辑,false不可编辑
                bool bAddFlag = (bool)listInfoData[i]["bAddFlag"];//true可编辑,false不可编辑
                if (bAddFlag)
                {
                    #region AddNewRow
                    DataRow dataRow_add = dtAddInfo.NewRow();
                    dataRow_add["dSyncTime"] = listInfoData[i]["dSyncTime"].ToString();
                    dataRow_add["vcChanges"] = listInfoData[i]["vcChanges"].ToString();
                    dataRow_add["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                    dataRow_add["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                    dataRow_add["vcPartENName"] = listInfoData[i]["vcPartENName"].ToString();
                    dataRow_add["vcCarfamilyCode"] = listInfoData[i]["vcCarfamilyCode"].ToString();
                    dataRow_add["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                    dataRow_add["dFromTime"] = listInfoData[i]["dFromTime"].ToString();
                    dataRow_add["dToTime"] = listInfoData[i]["dToTime"].ToString();
                    dataRow_add["vcPartId_Replace"] = listInfoData[i]["vcPartId_Replace"].ToString();
                    dataRow_add["vcInOut"] = listInfoData[i]["vcInOut"].ToString();
                    dataRow_add["vcOESP"] = listInfoData[i]["vcOESP"].ToString();
                    dataRow_add["vcHaoJiu"] = listInfoData[i]["vcHaoJiu"].ToString();
                    dataRow_add["vcOldProduction"] = listInfoData[i]["vcOldProduction"].ToString();
                    dataRow_add["dOldStartTime"] = listInfoData[i]["dOldStartTime"].ToString();
                    dataRow_add["dDebugTime"] = listInfoData[i]["dDebugTime"].ToString();
                    dataRow_add["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                    dataRow_add["dSupplierFromTime"] = listInfoData[i]["dSupplierFromTime"].ToString();
                    dataRow_add["dSupplierToTime"] = listInfoData[i]["dSupplierToTime"].ToString();
                    dataRow_add["vcSupplierName"] = listInfoData[i]["vcSupplierName"].ToString();
                    dtAddInfo.Rows.Add(dataRow_add);
                    #endregion

                    #region HistoryNewRow
                    DataRow dataRow_History = dtOperHistory.NewRow();
                    dataRow_History["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                    dataRow_History["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                    dataRow_History["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                    dataRow_History["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                    dataRow_History["vcAction"] = "新增手配情报=>" +
                        "  包装厂:" + listInfoData[i]["vcPackingPlant"].ToString() +
                        "；品番：" + listInfoData[i]["vcPartId"].ToString() +
                        "；收货方：" + listInfoData[i]["vcReceiver"].ToString() +
                        "；供应商：" + listInfoData[i]["vcSupplierId"].ToString();
                    dataRow_History["error"] = "";
                    dtOperHistory.Rows.Add(dataRow_History);
                    #endregion
                }
                if (bAddFlag == false && bModFlag == true)
                {
                    string strOperItem = string.Empty;
                    string PackingPlant = listInfoData[i]["vcPackingPlant"].ToString();
                    string PartId = listInfoData[i]["vcPartId"].ToString();
                    string Receiver = listInfoData[i]["vcReceiver"].ToString();
                    string SupplierId = listInfoData[i]["vcSupplierId"].ToString();
                    string SupplierPlant = listInfoData[i]["SupplierPlant_ed"].ToString();
                    //1.判断vcSupplierPlant是否需要修改
                    if (listInfoData[i]["SupplierPlantLinId_ed"] != null && listInfoData[i]["SupplierPlantLinId_ed"].ToString() == "-1")
                    {
                        string strSupplierPlant_ed = listInfoData[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = listInfoData[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = listInfoData[i]["SupplierPlantToTime_ed"].ToString();
                        string strSupplierPlantLinId_before = "";
                        string strSupplierPlantFromTime_before = "";
                        string strSupplierPlantToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("SupplierPlantEdit", PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strSupplierPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            strSupplierPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(strSupplierPlantFromTime_ed))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                                strError = "不符合上一个当前有效的开始使用时间小于维护的开始使用时间";
                            }
                            strSupplierPlantToTime_before = Convert.ToDateTime(strSupplierPlantFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_SupplierPlant表
                        #region AddNewRow
                        DataRow dataRow_add_SP = dtModInfo_SP.NewRow();
                        dataRow_add_SP["vcPackingPlant"] = PackingPlant;
                        dataRow_add_SP["vcPartId"] = PartId;
                        dataRow_add_SP["vcReceiver"] = Receiver;
                        dataRow_add_SP["vcSupplierId"] = SupplierId;
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
                        //记录更新履历
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                        dataRow_History["vcAction"] = "增加工区信息=>" +
                            "  工区:" + strSupplierPlant_ed +
                            "；使用开始：" + strSupplierPlantFromTime_ed +
                            "；使用结束：" + strSupplierPlantToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    //2.判断iPackingQty是否需要修改
                    if (listInfoData[i]["BoxLinId_ed"] != null && listInfoData[i]["BoxLinId_ed"].ToString() == "-1")
                    {
                        SupplierPlant = listInfoData[i]["vcSupplierPlant"].ToString();
                        string strBoxPackingQty_ed = listInfoData[i]["BoxPackingQty_ed"].ToString();
                        string strBoxLinId_ed = listInfoData[i]["BoxLinId_ed"].ToString();
                        string strBoxFromTime_ed = listInfoData[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = listInfoData[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = listInfoData[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = listInfoData[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = listInfoData[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = listInfoData[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = listInfoData[i]["BoxVolume_ed"].ToString();
                        string strBoxLinId_before = "";
                        string strBoxFromTime_before = "";
                        string strBoxToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("PackingQtyEdit", PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strBoxLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            strBoxFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(strBoxFromTime_ed))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                                strError = "不符合上一个当前有效的开始使用时间小于维护的开始使用时间";
                            }
                            strBoxToTime_before = Convert.ToDateTime(strBoxFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_Box表
                        #region AddNewRow
                        DataRow dataRow_add_PQ = dtModInfo_PQ.NewRow();
                        dataRow_add_PQ["vcPackingPlant"] = PackingPlant;
                        dataRow_add_PQ["vcPartId"] = PartId;
                        dataRow_add_PQ["vcReceiver"] = Receiver;
                        dataRow_add_PQ["vcSupplierId"] = SupplierId;
                        dataRow_add_PQ["vcSupplierPlant"] = SupplierPlant;
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
                        //记录更新履历
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                        dataRow_History["vcAction"] = "增加收容数信息=>" +
                            "  收容数:" + strBoxPackingQty_ed +
                            "；箱种:" + strBoxType_ed +
                            "；长:" + strBoxLength_ed +
                            "；宽:" + strBoxWidth_ed +
                            "；高:" + strBoxHeight_ed +
                            "；体积:" + strBoxVolume_ed +
                            "；使用开始：" + strBoxFromTime_ed +
                            "；使用结束：" + strBoxToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    //3.判断vcSufferIn是否需要修改
                    if (listInfoData[i]["SufferInLinId_ed"] != null && listInfoData[i]["SufferInLinId_ed"].ToString() == "-1")
                    {
                        SupplierPlant = listInfoData[i]["vcSupplierPlant"].ToString();
                        string strSufferIn_ed = listInfoData[i]["SufferIn_ed"].ToString();
                        string strSufferInLinId_ed = listInfoData[i]["SufferInLinId_ed"].ToString();
                        string strSufferInFromTime_ed = listInfoData[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = listInfoData[i]["SufferInToTime_ed"].ToString();
                        string strSufferInLinId_before = "";
                        string strSufferInFromTime_before = "";
                        string strSufferInToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("SufferInEdit", PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strSufferInLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            strSufferInFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(strSufferInFromTime_ed))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                                strError = "不符合上一个当前有效的开始使用时间小于维护的开始使用时间";
                            }
                            strSufferInToTime_before = Convert.ToDateTime(strSufferInFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_SufferIn表
                        #region AddNewRow
                        DataRow dataRow_add_SI = dtModInfo_SI.NewRow();
                        dataRow_add_SI["LinId"] = "";
                        dataRow_add_SI["vcPackingPlant"] = PackingPlant;
                        dataRow_add_SI["vcPartId"] = PartId;
                        dataRow_add_SI["vcReceiver"] = Receiver;
                        dataRow_add_SI["vcSupplierId"] = SupplierId;
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
                        //记录更新履历
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                        dataRow_History["vcAction"] = "增加受入信息=>" +
                            "  受入:" + strSufferIn_ed +
                            "；使用开始：" + strSufferInFromTime_ed +
                            "；使用结束：" + strSufferInToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    //4.判断vcOrderPlant是否需要修改
                    if (listInfoData[i]["OrderPlantLinId_ed"] != null && listInfoData[i]["OrderPlantLinId_ed"].ToString() == "-1")
                    {
                        SupplierPlant = listInfoData[i]["vcSupplierPlant"].ToString();
                        string strOrderPlant_ed = listInfoData[i]["OrderPlant_ed"].ToString();
                        string strOrderPlantLinId_ed = listInfoData[i]["OrderPlantLinId_ed"].ToString();
                        string strOrderPlantFromTime_ed = listInfoData[i]["OrderPlantFromTime_ed"].ToString();
                        string strOrderPlantToTime_ed = listInfoData[i]["OrderPlantToTime_ed"].ToString();
                        string strOrderPlantLinId_before = "";
                        string strOrderPlantFromTime_before = "";
                        string strOrderPlantToTime_before = "";
                        string strError = "";
                        DataTable dtCheckTime = getEditLoadInfo("OrderPlantEdit", PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strOrderPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            strOrderPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                            if (Convert.ToDateTime(strOrderPlantFromTime_before) >= Convert.ToDateTime(strOrderPlantFromTime_ed))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                                strError = "不符合上一个当前有效的开始使用时间小于维护的开始使用时间";
                            }
                            strOrderPlantToTime_before = Convert.ToDateTime(strOrderPlantFromTime_ed).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        //插入TSPMaster_OrderPlant表
                        #region AddNewRow
                        DataRow dataRow_add_OP = dtModInfo_OP.NewRow();
                        dataRow_add_OP["LinId"] = "";
                        dataRow_add_OP["vcPackingPlant"] = PackingPlant;
                        dataRow_add_OP["vcPartId"] = PartId;
                        dataRow_add_OP["vcReceiver"] = Receiver;
                        dataRow_add_OP["vcSupplierId"] = SupplierId;
                        dataRow_add_OP["dFromTime"] = strOrderPlantFromTime_ed;
                        dataRow_add_OP["dToTime"] = strOrderPlantToTime_ed;
                        dataRow_add_OP["vcOrderPlant"] = strOrderPlant_ed;
                        dataRow_add_OP["status"] = "add";
                        dataRow_add_OP["error"] = strError;
                        dtModInfo_OP.Rows.Add(dataRow_add_OP);
                        #endregion
                        //更新TSPMaster_OrderPlant表
                        if (strOrderPlantLinId_before != "")
                        {
                            #region AddNewRow
                            DataRow dataRow_mod_OP = dtModInfo_OP.NewRow();
                            dataRow_mod_OP["LinId"] = strOrderPlantLinId_before;
                            dataRow_mod_OP["dToTime"] = strOrderPlantToTime_before;
                            dataRow_mod_OP["status"] = "mod";
                            dataRow_mod_OP["error"] = strError;
                            dtModInfo_OP.Rows.Add(dataRow_mod_OP);
                            #endregion
                        }
                        //记录更新履历
                        #region HistoryNewRow
                        DataRow dataRow_History = dtOperHistory.NewRow();
                        dataRow_History["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                        dataRow_History["vcAction"] = "增加发注工厂信息=>" +
                            "  发注工厂:" + strOrderPlant_ed +
                            "；使用开始：" + strOrderPlantFromTime_ed +
                            "；使用结束：" + strOrderPlantToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    //5.判断主表是否进行了修改
                    DataRow[] drOperCheck = dtOperCheck.Select("vcPackingPlant='" + PackingPlant + "' and vcPartId='" + PartId + "' and vcReceiver='" + Receiver + "' and vcSupplierId='" + SupplierId + "'");
                    bool bCheckUpdate = false;
                    if (drOperCheck.Length > 0)
                    {
                        #region 履历
                        //5.1   vcSupplierPlace	
                        if (drOperCheck[0]["vcSupplierPlace"].ToString() != listInfoData[i]["vcSupplierPlace"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改供应商出荷地信息:" +
                                drOperCheck[0]["vcSupplierPlace"].ToString() + "==>" + listInfoData[i]["vcSupplierPlace"].ToString();
                        }
                        //5.2   vcInteriorProject
                        if (drOperCheck[0]["vcInteriorProject"].ToString() != listInfoData[i]["vcInteriorProject"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改内制工程信息:" +
                                drOperCheck[0]["vcInteriorProject"].ToString() + "==>" + listInfoData[i]["vcInteriorProject"].ToString();
                        }
                        //5.3   vcPassProject
                        if (drOperCheck[0]["vcPassProject"].ToString() != listInfoData[i]["vcPassProject"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改通过工程信息:" +
                                drOperCheck[0]["vcPassProject"].ToString() + "==>" + listInfoData[i]["vcPassProject"].ToString();
                        }
                        //5.4   vcFrontProject
                        if (drOperCheck[0]["vcFrontProject"].ToString() != listInfoData[i]["vcFrontProject"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改前工程信息:" +
                                drOperCheck[0]["vcFrontProject"].ToString() + "==>" + listInfoData[i]["vcFrontProject"].ToString();
                        }
                        //5.5   dFrontProjectTime
                        if (drOperCheck[0]["dFrontProjectTime"].ToString() != listInfoData[i]["dFrontProjectTime"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改前工程通过时间信息:" +
                                drOperCheck[0]["dFrontProjectTime"].ToString() + "==>" + listInfoData[i]["dFrontProjectTime"].ToString();
                        }
                        //5.6   dShipmentTime
                        if (drOperCheck[0]["dShipmentTime"].ToString() != listInfoData[i]["dShipmentTime"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改自工程出货时间信息:" +
                                drOperCheck[0]["dShipmentTime"].ToString() + "==>" + listInfoData[i]["dShipmentTime"].ToString();
                        }
                        //5.7   vcBillType
                        if (drOperCheck[0]["vcBillType"].ToString() != listInfoData[i]["vcBillType"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改单据区分信息:" +
                                drOperCheck[0]["vcBillType"].ToString() + "==>" + listInfoData[i]["vcBillType"].ToString();
                        }
                        //5.8   vcRemark1
                        if (drOperCheck[0]["vcRemark1"].ToString() != listInfoData[i]["vcRemark1"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改备注1信息:" +
                                drOperCheck[0]["vcRemark1"].ToString() + "==>" + listInfoData[i]["vcRemark1"].ToString();
                        }
                        //5.9   vcRemark2
                        if (drOperCheck[0]["vcRemark2"].ToString() != listInfoData[i]["vcRemark2"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改备注2信息:" +
                                drOperCheck[0]["vcRemark2"].ToString() + "==>" + listInfoData[i]["vcRemark2"].ToString();
                        }
                        //5.10   vcOrderingMethod
                        if (drOperCheck[0]["vcOrderingMethod"].ToString() != listInfoData[i]["vcOrderingMethod"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改订货方式信息:" +
                                drOperCheck[0]["vcOrderingMethod"].ToString() + "==>" + listInfoData[i]["vcOrderingMethod"].ToString();
                        }
                        //5.11   vcMandOrder
                        if (drOperCheck[0]["vcMandOrder"].ToString() != listInfoData[i]["vcMandOrder"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改强制订货信息:" +
                                drOperCheck[0]["vcMandOrder"].ToString() + "==>" + listInfoData[i]["vcMandOrder"].ToString();
                        }
                        #endregion
                        if (bCheckUpdate)
                        {
                            #region AddNewRow
                            DataRow dataRow_mod = dtModInfo.NewRow();
                            dataRow_mod["LinId"] = listInfoData[i]["LinId"].ToString();
                            dataRow_mod["vcSupplierPlace"] = listInfoData[i]["vcSupplierPlace"].ToString();
                            dataRow_mod["vcInteriorProject"] = listInfoData[i]["vcInteriorProject"].ToString();
                            dataRow_mod["vcPassProject"] = listInfoData[i]["vcPassProject"].ToString();
                            dataRow_mod["vcFrontProject"] = listInfoData[i]["vcFrontProject"].ToString();
                            dataRow_mod["dFrontProjectTime"] = listInfoData[i]["dFrontProjectTime"].ToString();
                            dataRow_mod["dShipmentTime"] = listInfoData[i]["dShipmentTime"].ToString();
                            dataRow_mod["vcBillType"] = listInfoData[i]["vcBillType"].ToString();
                            dataRow_mod["vcOrderingMethod"] = listInfoData[i]["vcOrderingMethod"].ToString();
                            dataRow_mod["vcMandOrder"] = listInfoData[i]["vcMandOrder"].ToString();
                            dataRow_mod["vcRemark1"] = listInfoData[i]["vcRemark1"].ToString();
                            dataRow_mod["vcRemark2"] = listInfoData[i]["vcRemark2"].ToString();
                            dtModInfo.Rows.Add(dataRow_mod);
                            #endregion

                            //记录更新履历
                            #region HistoryNewRow
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                            dataRow_History["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                            dataRow_History["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                            dataRow_History["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                            dataRow_History["vcAction"] = strOperItem;
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                    }
                }
            }
            fs0603_DataAccess.setSPInfo(dtAddInfo, dtModInfo, dtModInfo_SP, dtModInfo_PQ, dtModInfo_SI, dtModInfo_OP, dtOperHistory,
                strOperId, ref strErrorPartId);
            //fs0603_DataAccess.setSPInfo(listInfoData, strOperId, ref strErrorPartId);
        }
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData, string strOperId)
        {
            fs0603_DataAccess.deleteInfo(listInfoData, strOperId);
        }

        public void setAllInTime(DataTable dataTable, DateTime dFromTime, DateTime dToTime, string strOperId, ref string strErrorPartId,
            string strSyncTime, string strPartId, string strCarModel, string strReceiver, string strInOut,
            string strSupplierId, string strSupplierPlant, string strFromTime, string strToTime, string strHaoJiu, string strOrderPlant)
        {
            try
            {
                //用于记录修改履历
                DataTable dtOperHistory = createTable("OH");

                #region HistoryNewRow
                DataRow dataRow_History = dtOperHistory.NewRow();
                dataRow_History["vcPackingPlant"] = "AllIn";
                dataRow_History["vcPartId"] = "AllIn";
                dataRow_History["vcReceiver"] = "AllIn";
                dataRow_History["vcSupplierId"] = "AllIn";
                dataRow_History["vcAction"] = "通过一括设置有效期=>" +
                    "使用开始:" + dFromTime.ToString("yyyy-MM-dd") +
                    "使用结束:" + dToTime.ToString("yyyy-MM-dd") +
                    "||影响数据范围==>" +
                    "  同步时间:" + strSyncTime.ToString() +
                    "；补给品番：" + strPartId.ToString() +
                    "；车型：" + strCarModel.ToString() +
                    "；收货方：" + strReceiver.ToString() +
                    "；内外区分：" + strInOut.ToString() +
                    "；供应商编码：" + strSupplierId.ToString() +
                    "；工区：" + strSupplierPlant.ToString() +
                    "；使用开始：" + strFromTime.ToString() +
                    "；使用结束：" + strToTime.ToString() +
                    "；号旧区分：" + strHaoJiu.ToString() +
                    "；发注工厂：" + strOrderPlant.ToString();
                dataRow_History["error"] = "";
                dtOperHistory.Rows.Add(dataRow_History);
                #endregion

                fs0603_DataAccess.setAllInTime(dataTable, dtOperHistory, dFromTime, dToTime, strOperId, ref strErrorPartId);
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
        public List<Dictionary<string, Object>> setNullData(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    listInfoData[i]["LinId"] = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                    listInfoData[i]["dSyncTime"] = listInfoData[i]["dSyncTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dSyncTime"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcChanges"] = listInfoData[i]["vcChanges"] == null ? "" : listInfoData[i]["vcChanges"].ToString();
                    listInfoData[i]["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"] == null ? "" : listInfoData[i]["vcPackingPlant"].ToString();
                    listInfoData[i]["vcPartId"] = listInfoData[i]["vcPartId"] == null ? "" : listInfoData[i]["vcPartId"].ToString();
                    listInfoData[i]["vcPartENName"] = listInfoData[i]["vcPartENName"] == null ? "" : listInfoData[i]["vcPartENName"].ToString();
                    listInfoData[i]["vcCarfamilyCode"] = listInfoData[i]["vcCarfamilyCode"] == null ? "" : listInfoData[i]["vcCarfamilyCode"].ToString();
                    listInfoData[i]["vcReceiver"] = listInfoData[i]["vcReceiver"] == null ? "" : listInfoData[i]["vcReceiver"].ToString();
                    listInfoData[i]["dFromTime"] = listInfoData[i]["dFromTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["dToTime"] = listInfoData[i]["dToTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcPartId_Replace"] = listInfoData[i]["vcPartId_Replace"] == null ? "" : listInfoData[i]["vcPartId_Replace"].ToString();
                    listInfoData[i]["vcInOut"] = listInfoData[i]["vcInOut"] == null ? "" : listInfoData[i]["vcInOut"].ToString();
                    listInfoData[i]["vcOESP"] = listInfoData[i]["vcOESP"] == null ? "" : listInfoData[i]["vcOESP"].ToString();
                    listInfoData[i]["vcHaoJiu"] = listInfoData[i]["vcHaoJiu"] == null ? "" : listInfoData[i]["vcHaoJiu"].ToString();
                    listInfoData[i]["vcOldProduction"] = listInfoData[i]["vcOldProduction"] == null ? "" : listInfoData[i]["vcOldProduction"].ToString();
                    listInfoData[i]["dOldStartTime"] = listInfoData[i]["dOldStartTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dOldStartTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                    listInfoData[i]["dDebugTime"] = listInfoData[i]["dDebugTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dDebugTime"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcSupplierId"] = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                    listInfoData[i]["dSupplierFromTime"] = listInfoData[i]["dSupplierFromTime"] == null ? "" : listInfoData[i]["dSupplierFromTime"].ToString();
                    listInfoData[i]["dSupplierToTime"] = listInfoData[i]["dSupplierToTime"] == null ? "" : listInfoData[i]["dSupplierToTime"].ToString();
                    listInfoData[i]["vcSupplierName"] = listInfoData[i]["vcSupplierName"] == null ? "" : listInfoData[i]["vcSupplierName"].ToString();
                    listInfoData[i]["vcSupplierPlant"] = listInfoData[i]["vcSupplierPlant"] == null ? "" : listInfoData[i]["vcSupplierPlant"].ToString();
                    listInfoData[i]["SupplierPlant_ed"] = listInfoData[i]["SupplierPlant_ed"] == null ? "" : listInfoData[i]["SupplierPlant_ed"].ToString();
                    listInfoData[i]["SupplierPlantLinId_ed"] = listInfoData[i]["SupplierPlantLinId_ed"] == null ? "" : listInfoData[i]["SupplierPlantLinId_ed"].ToString();
                    listInfoData[i]["SupplierPlantFromTime_ed"] = listInfoData[i]["SupplierPlantFromTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["SupplierPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["SupplierPlantToTime_ed"] = listInfoData[i]["SupplierPlantToTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["SupplierPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcSupplierPlace"] = listInfoData[i]["vcSupplierPlace"] == null ? "" : listInfoData[i]["vcSupplierPlace"].ToString();
                    listInfoData[i]["iPackingQty"] = listInfoData[i]["iPackingQty"] == null ? "" : listInfoData[i]["iPackingQty"].ToString();
                    listInfoData[i]["vcBoxType"] = listInfoData[i]["vcBoxType"] == null ? "" : listInfoData[i]["vcBoxType"].ToString();
                    listInfoData[i]["iLength"] = listInfoData[i]["iLength"] == null ? "" : listInfoData[i]["iLength"].ToString();
                    listInfoData[i]["iWidth"] = listInfoData[i]["iWidth"] == null ? "" : listInfoData[i]["iWidth"].ToString();
                    listInfoData[i]["iHeight"] = listInfoData[i]["iHeight"] == null ? "" : listInfoData[i]["iHeight"].ToString();
                    listInfoData[i]["iVolume"] = listInfoData[i]["iVolume"] == null ? "" : listInfoData[i]["iVolume"].ToString();
                    listInfoData[i]["BoxPackingQty_ed"] = listInfoData[i]["BoxPackingQty_ed"] == null ? "" : listInfoData[i]["BoxPackingQty_ed"].ToString();
                    listInfoData[i]["BoxLinId_ed"] = listInfoData[i]["BoxLinId_ed"] == null ? "" : listInfoData[i]["BoxLinId_ed"].ToString();
                    listInfoData[i]["BoxFromTime_ed"] = listInfoData[i]["BoxFromTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["BoxFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["BoxToTime_ed"] = listInfoData[i]["BoxToTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["BoxToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["BoxType_ed"] = listInfoData[i]["BoxType_ed"] == null ? "" : listInfoData[i]["BoxType_ed"].ToString();
                    listInfoData[i]["BoxLength_ed"] = listInfoData[i]["BoxLength_ed"] == null ? "" : listInfoData[i]["BoxLength_ed"].ToString();
                    listInfoData[i]["BoxWidth_ed"] = listInfoData[i]["BoxWidth_ed"] == null ? "" : listInfoData[i]["BoxWidth_ed"].ToString();
                    listInfoData[i]["BoxHeight_ed"] = listInfoData[i]["BoxHeight_ed"] == null ? "" : listInfoData[i]["BoxHeight_ed"].ToString();
                    listInfoData[i]["BoxVolume_ed"] = listInfoData[i]["BoxVolume_ed"] == null ? "" : listInfoData[i]["BoxVolume_ed"].ToString();
                    listInfoData[i]["vcSufferIn"] = listInfoData[i]["vcSufferIn"] == null ? "" : listInfoData[i]["vcSufferIn"].ToString();
                    listInfoData[i]["SufferIn_ed"] = listInfoData[i]["SufferIn_ed"] == null ? "" : listInfoData[i]["SufferIn_ed"].ToString();
                    listInfoData[i]["SufferInLinId_ed"] = listInfoData[i]["SufferInLinId_ed"] == null ? "" : listInfoData[i]["SufferInLinId_ed"].ToString();
                    listInfoData[i]["SufferInFromTime_ed"] = listInfoData[i]["SufferInFromTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["SufferInFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["SufferInToTime_ed"] = listInfoData[i]["SufferInToTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["SufferInToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcOrderPlant"] = listInfoData[i]["vcOrderPlant"] == null ? "" : listInfoData[i]["vcOrderPlant"].ToString();
                    listInfoData[i]["OrderPlant_ed"] = listInfoData[i]["OrderPlant_ed"] == null ? "" : listInfoData[i]["OrderPlant_ed"].ToString();
                    listInfoData[i]["OrderPlantLinId_ed"] = listInfoData[i]["OrderPlantLinId_ed"] == null ? "" : listInfoData[i]["OrderPlantLinId_ed"].ToString();
                    listInfoData[i]["OrderPlantFromTime_ed"] = listInfoData[i]["OrderPlantFromTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["OrderPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["OrderPlantToTime_ed"] = listInfoData[i]["OrderPlantToTime_ed"] == null ? "" : Convert.ToDateTime(listInfoData[i]["OrderPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcInteriorProject"] = listInfoData[i]["vcInteriorProject"] == null ? "" : listInfoData[i]["vcInteriorProject"].ToString();
                    listInfoData[i]["vcPassProject"] = listInfoData[i]["vcPassProject"] == null ? "" : listInfoData[i]["vcPassProject"].ToString();
                    listInfoData[i]["vcFrontProject"] = listInfoData[i]["vcFrontProject"] == null ? "" : listInfoData[i]["vcFrontProject"].ToString();
                    listInfoData[i]["dFrontProjectTime"] = listInfoData[i]["dFrontProjectTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dFrontProjectTime"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["dShipmentTime"] = listInfoData[i]["dShipmentTime"] == null ? "" : Convert.ToDateTime(listInfoData[i]["dShipmentTime"].ToString()).ToString("yyyy-MM-dd");
                    listInfoData[i]["vcPartImage"] = listInfoData[i]["vcPartImage"] == null ? "" : listInfoData[i]["vcPartImage"].ToString();
                    listInfoData[i]["vcBillType"] = listInfoData[i]["vcBillType"] == null ? "" : listInfoData[i]["vcBillType"].ToString();
                    listInfoData[i]["vcRemark1"] = listInfoData[i]["vcRemark1"] == null ? "" : listInfoData[i]["vcRemark1"].ToString();
                    listInfoData[i]["vcRemark2"] = listInfoData[i]["vcRemark2"] == null ? "" : listInfoData[i]["vcRemark2"].ToString();
                    listInfoData[i]["vcOrderingMethod"] = listInfoData[i]["vcOrderingMethod"] == null ? "" : listInfoData[i]["vcOrderingMethod"].ToString();
                    listInfoData[i]["vcMandOrder"] = listInfoData[i]["vcMandOrder"] == null ? "" : listInfoData[i]["vcMandOrder"].ToString();
                }
                return listInfoData;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
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
                dataTable.Columns.Add("vcAction");
                dataTable.Columns.Add("error");
            }
            return dataTable;
        }

    }
}
