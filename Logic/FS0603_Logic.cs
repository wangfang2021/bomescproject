using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;
using System.Text.RegularExpressions;

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
        public DataTable checkFileInfo(DataTable dataTable, string[,] Header, int heardrow, int datarow, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                dataTable.Columns.Add("LinId");
                #region 校验数据库
                DataTable dtSPInfo = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "", "", "", "", "");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strPackingPlant = dataTable.Rows[i]["vcPackingPlant"].ToString();
                    string strPartId = dataTable.Rows[i]["vcPartId"].ToString();
                    string strReceiver = dataTable.Rows[i]["vcReceiver"].ToString();
                    string strSupplierId = dataTable.Rows[i]["vcSupplierId"].ToString();
                    DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                    if (drSPInfo.Length != 1)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("第{0}行情报不在已有基础情报中,请勿修改固定数据(Excel蓝色标记情报)", i + datarow);
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        dataTable.Rows[i]["LinId"] = drSPInfo[0]["LinId"].ToString();
                    }

                }
                #endregion

                #region 校验数据格式
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dr = dataTable.Rows[i];
                    for (int j = 0; j < Header.GetLength(1); j++)
                    {
                        //校验长度
                        if (Convert.ToInt32(Header[3, j]) > 0 && dr[Header[1, j]].ToString().Length > Convert.ToInt32(Header[3, j]))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行{1}大于设定长度", i + datarow, Header[0, j]);
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行{1}小于设定长度", i + datarow, Header[0, j]);
                            dtMessage.Rows.Add(dataRow);
                        }
                        //校验类型
                        switch (Header[2, j])
                        {
                            case "decimal":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}不是合法数值", i + datarow, Header[0, j]);
                                    dtMessage.Rows.Add(dataRow);
                                }

                                break;
                            case "d":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}不是合法日期", i + datarow, Header[0, j]);
                                    dtMessage.Rows.Add(dataRow);
                                }

                                break;
                            case "ym":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckYearMonth(dr[Header[1, j]].ToString()))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}不是合法日期", i + datarow, Header[0, j]);
                                    dtMessage.Rows.Add(dataRow);
                                }

                                break;
                            default:
                                if (Header[2, j].Length > 0 && Regex.Match(dr[Header[1, j]].ToString(), Header[2, j],
                                    RegexOptions.None).Success)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}有非法字符", i + datarow, Header[0, j]);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                break;
                        }
                    }
                }
                #endregion

                #region 校验提交数据--待用户提供

                #endregion

                #region 校验数据内容
                DataTable OrderPlantList = ComFunction.getTCode("C000");//发注工厂
                DataTable BillTypeList = ComFunction.getTCode("C007");//单据区分
                DataTable OrderingMethodList = ComFunction.getTCode("C047");//订货方式
                DataTable MandOrderList = ComFunction.getTCode("C048");//强制订货
                dataTable.Columns.Add("OrderPlant_ed");
                dataTable.Columns.Add("vcBillType");
                dataTable.Columns.Add("vcOrderingMethod");
                dataTable.Columns.Add("vcMandOrder");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strOrderPlant_name = dataTable.Rows[i]["vcOrderPlant_name"].ToString();
                    string strBillType_name = dataTable.Rows[i]["vcBillType_name"].ToString();
                    string strOrderingMethod_name = dataTable.Rows[i]["vcOrderingMethod_name"].ToString();
                    string strMandOrder_name = dataTable.Rows[i]["vcMandOrder_name"].ToString();
                    DataRow[] drOrderPlant = OrderPlantList.Select("vcName='" + strOrderPlant_name + "'");
                    DataRow[] drBillType = BillTypeList.Select("vcName='" + strBillType_name + "'");
                    DataRow[] drOrderingMethod = OrderingMethodList.Select("vcName='" + strOrderingMethod_name + "'");
                    DataRow[] drMandOrder = MandOrderList.Select("vcName='" + strMandOrder_name + "'");
                    if (strOrderPlant_name != "")
                    {
                        if (drOrderPlant.Length == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行发注工厂情报不存在,请确认无误后再重新操作", i + datarow);
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            dataTable.Rows[i]["OrderPlant_ed"] = drOrderPlant[0]["vcValue"].ToString();
                        }
                    }
                    else
                    {
                        dataTable.Rows[i]["OrderPlant_ed"] = strOrderPlant_name;
                    }
                    if (strBillType_name != "")
                    {
                        if (drBillType.Length == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行单据区分情报不存在,请确认无误后再重新操作", i + datarow);
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            dataTable.Rows[i]["vcBillType"] = drBillType[0]["vcValue"].ToString();
                        }
                    }
                    else
                    {
                        dataTable.Rows[i]["vcBillType"] = strBillType_name;
                    }
                    if (strOrderingMethod_name != "")
                    {
                        if (drOrderingMethod.Length == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行订货方式情报不存在,请确认无误后再重新操作", i + datarow);
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            dataTable.Rows[i]["vcOrderingMethod"] = drOrderingMethod[0]["vcValue"].ToString();
                        }
                    }
                    else
                    {
                        dataTable.Rows[i]["vcOrderingMethod"] = strOrderingMethod_name;
                    }
                    if (strMandOrder_name != "")
                    {
                        if (drMandOrder.Length == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("第{0}行强制订货类型情报不存在,请确认无误后再重新操作", i + datarow);
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            dataTable.Rows[i]["vcMandOrder"] = drMandOrder[0]["vcValue"].ToString();
                        }
                    }
                    else
                    {
                        dataTable.Rows[i]["vcMandOrder"] = strMandOrder_name;
                    }
                }
                #endregion

                if (dtMessage == null || dtMessage.Rows.Count == 0)
                {
                    #region 整理数据内容
                    DataTable dtImport = dtSPInfo.Clone();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string LinId = dataTable.Rows[i]["LinId"].ToString();
                        string strPackingPlant = dataTable.Rows[i]["vcPackingPlant"].ToString();
                        string strPartId = dataTable.Rows[i]["vcPartId"].ToString();
                        string strReceiver = dataTable.Rows[i]["vcReceiver"].ToString();
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"].ToString();
                        DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "'");
                        //工区类
                        string SupplierPlant_ed = "";
                        string SupplierPlantLinId_ed = "";
                        string SupplierPlantFromTime_ed = "";
                        string SupplierPlantToTime_ed = "";
                        if (drSPInfo[0]["SupplierPlant_ed"].ToString() == dataTable.Rows[i]["SupplierPlant_ed"].ToString() &&
                           (drSPInfo[0]["SupplierPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SupplierPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["SupplierPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SupplierPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) &&
                           (drSPInfo[0]["SupplierPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SupplierPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["SupplierPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SupplierPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd")))
                        {
                            SupplierPlant_ed = drSPInfo[0]["SupplierPlant_ed"].ToString();
                            SupplierPlantLinId_ed = "999";
                            SupplierPlantFromTime_ed = drSPInfo[0]["SupplierPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SupplierPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                            SupplierPlantToTime_ed = drSPInfo[0]["SupplierPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SupplierPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            SupplierPlant_ed = dataTable.Rows[i]["SupplierPlant_ed"].ToString();
                            SupplierPlantLinId_ed = "-1";
                            SupplierPlantFromTime_ed = dataTable.Rows[i]["SupplierPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SupplierPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                            SupplierPlantToTime_ed = dataTable.Rows[i]["SupplierPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SupplierPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                        }
                        //补给收容数类
                        string BoxPackingQty_ed = "";
                        string BoxLinId_ed = "";
                        string BoxFromTime_ed = "";
                        string BoxToTime_ed = "";
                        string BoxType_ed = "";
                        string BoxLength_ed = "";
                        string BoxWidth_ed = "";
                        string BoxHeight_ed = "";
                        string BoxVolume_ed = "";
                        DataRow[] drSPInfo_qty = dtSPInfo.Select("vcPackingPlant='" + strPackingPlant + "' and vcPartId='" + strPartId + "' and vcReceiver='" + strReceiver + "' and vcSupplierId='" + strSupplierId + "' and vcSupplierPlant='" + SupplierPlant_ed + "'");
                        if (drSPInfo_qty.Length != 0 && (drSPInfo_qty[0]["BoxPackingQty_ed"].ToString() == dataTable.Rows[i]["BoxPackingQty_ed"].ToString() &&
                           (drSPInfo_qty[0]["BoxFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo_qty[0]["BoxFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["BoxFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["BoxFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) &&
                               (drSPInfo_qty[0]["BoxToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo_qty[0]["BoxToTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["BoxToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["BoxToTime_ed"].ToString()).ToString("yyyy-MM-dd"))))
                        {
                            BoxPackingQty_ed = drSPInfo_qty[0]["BoxPackingQty_ed"].ToString();
                            BoxLinId_ed = "999";
                            BoxFromTime_ed = drSPInfo_qty[0]["BoxFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo_qty[0]["BoxFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                            BoxToTime_ed = drSPInfo_qty[0]["BoxToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo_qty[0]["BoxToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                            BoxType_ed = drSPInfo_qty[0]["BoxType_ed"].ToString() == "" ? "" : drSPInfo_qty[0]["BoxType_ed"].ToString();
                            BoxLength_ed = drSPInfo_qty[0]["BoxLength_ed"].ToString() == "" ? "0" : drSPInfo_qty[0]["BoxLength_ed"].ToString();
                            BoxWidth_ed = drSPInfo_qty[0]["BoxWidth_ed"].ToString() == "" ? "0" : drSPInfo_qty[0]["BoxWidth_ed"].ToString();
                            BoxHeight_ed = drSPInfo_qty[0]["BoxHeight_ed"].ToString() == "" ? "0" : drSPInfo_qty[0]["BoxHeight_ed"].ToString();
                            BoxVolume_ed = drSPInfo_qty[0]["BoxVolume_ed"].ToString() == "" ? "0" : drSPInfo_qty[0]["BoxVolume_ed"].ToString();
                        }
                        else
                        {
                            BoxPackingQty_ed = dataTable.Rows[i]["BoxPackingQty_ed"].ToString();
                            BoxLinId_ed = dataTable.Rows[i]["BoxPackingQty_ed"].ToString() == "" ? "999" : "-1";
                            BoxFromTime_ed = (dataTable.Rows[i]["BoxFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["BoxFromTime_ed"].ToString()).ToString("yyyy-MM-dd"));
                            BoxToTime_ed = (dataTable.Rows[i]["BoxToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["BoxToTime_ed"].ToString()).ToString("yyyy-MM-dd"));
                            BoxType_ed = dataTable.Rows[i]["BoxType_ed"].ToString() == "" ? "" : dataTable.Rows[i]["BoxType_ed"].ToString();
                            BoxLength_ed = dataTable.Rows[i]["BoxLength_ed"].ToString() == "" ? "0" : dataTable.Rows[i]["BoxLength_ed"].ToString();
                            BoxWidth_ed = dataTable.Rows[i]["BoxWidth_ed"].ToString() == "" ? "0" : dataTable.Rows[i]["BoxWidth_ed"].ToString();
                            BoxHeight_ed = dataTable.Rows[i]["BoxHeight_ed"].ToString() == "" ? "0" : dataTable.Rows[i]["BoxHeight_ed"].ToString();
                            BoxVolume_ed = (Convert.ToInt32(BoxLength_ed == "" ? "0" : BoxLength_ed) * Convert.ToInt32(BoxWidth_ed == "" ? "0" : BoxWidth_ed) * Convert.ToInt32(BoxHeight_ed == "" ? "0" : BoxHeight_ed)).ToString();
                        }
                        //受入类
                        string SufferIn_ed = "";
                        string SufferInLinId_ed = "";
                        string SufferInFromTime_ed = "";
                        string SufferInToTime_ed = "";
                        if (drSPInfo[0]["SufferIn_ed"].ToString() == dataTable.Rows[i]["SufferIn_ed"].ToString() &&
                           (drSPInfo[0]["SufferInFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SufferInFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["SufferInFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SufferInFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) &&
                           (drSPInfo[0]["SufferInToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SufferInToTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["SufferInToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SufferInToTime_ed"].ToString()).ToString("yyyy-MM-dd")))
                        {
                            SufferIn_ed = drSPInfo[0]["SufferIn_ed"].ToString();
                            SufferInLinId_ed = "999";
                            SufferInFromTime_ed = drSPInfo[0]["SufferInFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SufferInFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                            SufferInToTime_ed = drSPInfo[0]["SufferInToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["SufferInToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            SufferIn_ed = dataTable.Rows[i]["SufferIn_ed"].ToString();
                            SufferInLinId_ed = "-1";
                            SufferInFromTime_ed = (dataTable.Rows[i]["SufferInFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SufferInFromTime_ed"].ToString()).ToString("yyyy-MM-dd"));
                            SufferInToTime_ed = (dataTable.Rows[i]["SufferInToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["SufferInToTime_ed"].ToString()).ToString("yyyy-MM-dd"));
                        }
                        //发注工厂类
                        string OrderPlant_ed = "";
                        string OrderPlantLinId_ed = "";
                        string OrderPlantFromTime_ed = "";
                        string OrderPlantToTime_ed = "";
                        if (drSPInfo[0]["OrderPlant_ed"].ToString() == dataTable.Rows[i]["OrderPlant_ed"].ToString() &&
                           (drSPInfo[0]["OrderPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["OrderPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["OrderPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["OrderPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd")) &&
                           (drSPInfo[0]["OrderPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["OrderPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd")) == (dataTable.Rows[i]["OrderPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["OrderPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd")))
                        {
                            OrderPlant_ed = drSPInfo[0]["OrderPlant_ed"].ToString();
                            OrderPlantLinId_ed = "999";
                            OrderPlantFromTime_ed = drSPInfo[0]["OrderPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["OrderPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                            OrderPlantToTime_ed = drSPInfo[0]["OrderPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(drSPInfo[0]["OrderPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            OrderPlant_ed = dataTable.Rows[i]["OrderPlant_ed"].ToString();
                            OrderPlantLinId_ed = "-1";
                            OrderPlantFromTime_ed = (dataTable.Rows[i]["OrderPlantFromTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["OrderPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd"));
                            OrderPlantToTime_ed = (dataTable.Rows[i]["OrderPlantToTime_ed"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["OrderPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd"));
                        }
                        //供应商出荷地
                        string vcSupplierPlace = dataTable.Rows[i]["vcSupplierPlace"].ToString();
                        //内制工程
                        string vcInteriorProject = dataTable.Rows[i]["vcInteriorProject"].ToString();
                        //通过工程
                        string vcPassProject = dataTable.Rows[i]["vcPassProject"].ToString();
                        //前工程
                        string vcFrontProject = dataTable.Rows[i]["vcFrontProject"].ToString();
                        //前工程通过时间
                        string dFrontProjectTime = dataTable.Rows[i]["dFrontProjectTime"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["dFrontProjectTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        //自工程出荷时间
                        string dShipmentTime = dataTable.Rows[i]["dShipmentTime"].ToString() == "" ? "" : Convert.ToDateTime(dataTable.Rows[i]["dShipmentTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        //单据区分
                        string vcBillType = dataTable.Rows[i]["vcBillType"].ToString();
                        //备注1
                        string vcRemark1 = dataTable.Rows[i]["vcRemark1"].ToString();
                        //备注2
                        string vcRemark2 = dataTable.Rows[i]["vcRemark2"].ToString();
                        //订货方式
                        string vcOrderingMethod = dataTable.Rows[i]["vcOrderingMethod"].ToString();
                        //强制订货
                        string vcMandOrder = dataTable.Rows[i]["vcMandOrder"].ToString();

                        DataRow drImport = dtImport.NewRow();
                        drImport["bModFlag"] = "true";
                        drImport["bAddFlag"] = "false";
                        drImport["LinId"] = LinId == "" ? "999" : LinId;
                        drImport["vcPackingPlant"] = strPackingPlant;
                        drImport["vcPartId"] = strPartId;
                        drImport["vcReceiver"] = strReceiver;
                        drImport["vcSupplierId"] = strSupplierId;
                        drImport["SupplierPlant_ed"] = SupplierPlant_ed;
                        drImport["SupplierPlantLinId_ed"] = SupplierPlantLinId_ed;
                        drImport["SupplierPlantFromTime_ed"] = SupplierPlantFromTime_ed;
                        drImport["SupplierPlantToTime_ed"] = SupplierPlantToTime_ed;
                        drImport["BoxPackingQty_ed"] = BoxPackingQty_ed;
                        drImport["BoxLinId_ed"] = BoxLinId_ed;
                        drImport["BoxFromTime_ed"] = BoxFromTime_ed;
                        drImport["BoxToTime_ed"] = BoxToTime_ed;
                        drImport["BoxType_ed"] = BoxType_ed;
                        drImport["BoxLength_ed"] = BoxLength_ed;
                        drImport["BoxWidth_ed"] = BoxWidth_ed;
                        drImport["BoxHeight_ed"] = BoxHeight_ed;
                        drImport["BoxVolume_ed"] = BoxVolume_ed;
                        drImport["SufferIn_ed"] = SufferIn_ed;
                        drImport["SufferInLinId_ed"] = SufferInLinId_ed;
                        drImport["SufferInFromTime_ed"] = SufferInFromTime_ed;
                        drImport["SufferInToTime_ed"] = SufferInToTime_ed;
                        drImport["OrderPlant_ed"] = OrderPlant_ed;
                        drImport["OrderPlantLinId_ed"] = OrderPlantLinId_ed;
                        drImport["OrderPlantFromTime_ed"] = OrderPlantFromTime_ed;
                        drImport["OrderPlantToTime_ed"] = OrderPlantToTime_ed;
                        drImport["vcSupplierPlace"] = vcSupplierPlace;
                        drImport["vcInteriorProject"] = vcInteriorProject;
                        drImport["vcPassProject"] = vcPassProject;
                        drImport["vcFrontProject"] = vcFrontProject;
                        drImport["dFrontProjectTime"] = dFrontProjectTime;
                        drImport["dShipmentTime"] = dShipmentTime;
                        drImport["vcBillType"] = vcBillType;
                        drImport["vcRemark1"] = vcRemark1;
                        drImport["vcRemark2"] = vcRemark2;
                        drImport["vcOrderingMethod"] = vcOrderingMethod;
                        drImport["vcMandOrder"] = vcMandOrder;
                        dtImport.Rows.Add(drImport);
                    }
                    #endregion

                    #region 验证修改项有效期
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        //基础信息
                        string strPackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString();
                        string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                        string strReceiver = dtImport.Rows[i]["vcReceiver"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                        //工区
                        string SupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string SupplierPlantLinId_ed = dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString();
                        string SupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string SupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();
                        if (SupplierPlantLinId_ed == "-1")
                        {
                            string strSupplierPlantLinId_before = "";
                            string strSupplierPlantFromTime_before = "";
                            DataTable dtCheckTime = getEditLoadInfo("SupplierPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, SupplierPlant_ed);
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                strSupplierPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                strSupplierPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(SupplierPlantFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行工区有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + datarow);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                        }
                        //收容数
                        string BoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string BoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                        string BoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string BoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string BoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string BoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string BoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string BoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string BoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();
                        if (BoxLinId_ed == "-1")
                        {
                            string strBoxLinId_before = "";
                            string strBoxFromTime_before = "";
                            DataTable dtCheckTime = getEditLoadInfo("PackingQtyEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, SupplierPlant_ed);
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                strBoxLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                strBoxFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(BoxFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行收容数有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + datarow);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                        }
                        //受入
                        string SufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string SufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                        string SufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string SufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();
                        if (SufferInLinId_ed == "-1")
                        {
                            string strSufferInLinId_before = "";
                            string strSufferInFromTime_before = "";
                            DataTable dtCheckTime = getEditLoadInfo("SufferInEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, SupplierPlant_ed);
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                strSufferInLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                strSufferInFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(SufferInFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行受入有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + datarow);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                        }
                        //发注工厂
                        string OrderPlant_ed = dtImport.Rows[i]["OrderPlant_ed"].ToString();
                        string OrderPlantLinId_ed = dtImport.Rows[i]["OrderPlantLinId_ed"].ToString();
                        string OrderPlantFromTime_ed = dtImport.Rows[i]["OrderPlantFromTime_ed"].ToString();
                        string OrderPlantToTime_ed = dtImport.Rows[i]["OrderPlantToTime_ed"].ToString();
                        if (OrderPlantLinId_ed == "-1")
                        {
                            string strOrderPlantLinId_before = "";
                            string strOrderPlantFromTime_before = "";
                            DataTable dtCheckTime = getEditLoadInfo("OrderPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, SupplierPlant_ed);
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                strOrderPlantLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                strOrderPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strOrderPlantFromTime_before) >= Convert.ToDateTime(OrderPlantFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("第{0}行发注工厂有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + datarow);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                    #endregion
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
                DataTable dtSPInfo = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "", "", "", "", "");
                DataTable dtImport = dtSPInfo.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow drImport = dtImport.NewRow();
                    drImport["bAddFlag"] = listInfoData[i]["bAddFlag"].ToString();
                    drImport["bModFlag"] = listInfoData[i]["bModFlag"].ToString();
                    if ((bool)listInfoData[i]["bAddFlag"])
                    {
                        drImport["dSyncTime"] = listInfoData[i]["dSyncTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dSyncTime"].ToString()).ToString("yyyy-MM-dd");
                        if (listInfoData[i]["vcChanges"].ToString() != "")
                        {
                            drImport["vcChanges"] = listInfoData[i]["vcChanges"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护变更事项情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcPackingPlant"].ToString() != "")
                        {
                            drImport["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护包装工厂情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcPartId"].ToString() != "")
                        {
                            drImport["vcPartId"] = listInfoData[i]["vcPartId"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护补给品番情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcPartENName"].ToString() != "")
                        {
                            drImport["vcPartENName"] = listInfoData[i]["vcPartENName"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护品名情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcCarfamilyCode"].ToString() != "")
                        {
                            drImport["vcCarfamilyCode"] = listInfoData[i]["vcCarfamilyCode"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护车种情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcReceiver"].ToString() != "")
                        {
                            drImport["vcReceiver"] = listInfoData[i]["vcReceiver"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护收货方情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["dFromTime"].ToString() != "")
                        {
                            drImport["dFromTime"] = listInfoData[i]["dFromTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dFromTime"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护使用开始情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["dToTime"].ToString() != "")
                        {
                            drImport["dToTime"] = listInfoData[i]["dToTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dToTime"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护使用结束情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcPartId_Replace"].ToString() != "")
                        {
                            drImport["vcPartId_Replace"] = listInfoData[i]["vcPartId_Replace"].ToString();
                        }
                        else
                        {
                            drImport["vcPartId_Replace"] = listInfoData[i]["vcPartId_Replace"].ToString();
                        }
                        if (listInfoData[i]["vcInOut"].ToString() != "")
                        {
                            drImport["vcInOut"] = listInfoData[i]["vcInOut"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护内外区分情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcOESP"].ToString() != "")
                        {
                            drImport["vcOESP"] = listInfoData[i]["vcOESP"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护OE=SP情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcHaoJiu"].ToString() != "")
                        {
                            drImport["vcHaoJiu"] = listInfoData[i]["vcHaoJiu"].ToString();
                            if (listInfoData[i]["vcHaoJiu"].ToString() == "Q")
                            {
                                if (listInfoData[i]["vcOldProduction"].ToString() != "")
                                {
                                    drImport["vcOldProduction"] = listInfoData[i]["vcOldProduction"].ToString();
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护旧型年限生产区分情报", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                                if (listInfoData[i]["dOldStartTime"].ToString() != "")
                                {
                                    drImport["dOldStartTime"] = listInfoData[i]["dOldStartTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dOldStartTime"].ToString()).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护旧型开始时间情报", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护号旧区分情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["dDebugTime"].ToString() != "")
                        {
                            drImport["dDebugTime"] = listInfoData[i]["dDebugTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dDebugTime"].ToString()).ToString("yyyy-MM");
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护实施年月(年限)情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcSupplierId"].ToString() != "")
                        {
                            drImport["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护供应商编号情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["dSupplierFromTime"].ToString() != "")
                        {
                            drImport["dSupplierFromTime"] = listInfoData[i]["dSupplierFromTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dSupplierFromTime"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护供应商使用开始情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["dSupplierToTime"].ToString() != "")
                        {
                            drImport["dSupplierToTime"] = listInfoData[i]["dSupplierToTime"].ToString() == "" ? "" : Convert.ToDateTime(listInfoData[i]["dSupplierToTime"].ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护供应商使用结束情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        if (listInfoData[i]["vcSupplierName"].ToString() != "")
                        {
                            drImport["vcSupplierName"] = listInfoData[i]["vcSupplierName"].ToString();
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行新增信息请维护供应商名称情报", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        DataRow[] drSPInfo = dtSPInfo.Select("vcPackingPlant='" + listInfoData[i]["vcPackingPlant"].ToString() + "' and vcPartId='" + listInfoData[i]["vcPartId"].ToString() + "' and vcReceiver='" + listInfoData[i]["vcReceiver"].ToString() + "' and vcSupplierId='" + listInfoData[i]["vcSupplierId"].ToString() + "'");
                        if (drSPInfo.Length != 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("新增第{0}行情报已经存在无法在新增", i + 1);
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        dtImport.Rows.Add(drImport);
                    }
                    if ((bool)listInfoData[i]["bAddFlag"] == false && (bool)listInfoData[i]["bModFlag"] == true)
                    {
                        drImport["LinId"] = listInfoData[i]["LinId"].ToString();
                        string strPackingPlant = listInfoData[i]["vcPackingPlant"].ToString();
                        drImport["vcPackingPlant"] = strPackingPlant;
                        string strPartId = listInfoData[i]["vcPartId"].ToString();
                        drImport["vcPartId"] = strPartId;
                        string strReceiver = listInfoData[i]["vcReceiver"].ToString();
                        drImport["vcReceiver"] = strReceiver;
                        string strSupplierId = listInfoData[i]["vcSupplierId"].ToString();
                        drImport["vcSupplierId"] = strSupplierId;
                        #region 校验提交数据--待用户提供

                        #endregion

                        #region 验证工区
                        string strSupplierPlant = listInfoData[i]["vcSupplierPlant"].ToString();
                        string strSupplierPlant_ed = listInfoData[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantLinId_ed = listInfoData[i]["SupplierPlantLinId_ed"] == null ? "" : listInfoData[i]["SupplierPlantLinId_ed"].ToString();
                        string strSupplierPlantFromTime_ed = Convert.ToDateTime(listInfoData[i]["SupplierPlantFromTime_ed"].ToString()).ToString("yyyy-MM-dd");
                        string strSupplierPlantToTime_ed = Convert.ToDateTime(listInfoData[i]["SupplierPlantToTime_ed"].ToString()).ToString("yyyy-MM-dd");
                        if (strSupplierPlantLinId_ed == "-1")
                        {
                            string strSupplierPlantLinId_before = "";
                            string strSupplierPlantFromTime_before = "";
                            DataTable dtCheckTime_sp = getEditLoadInfo("SupplierPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant_ed);
                            if (dtCheckTime_sp.Rows.Count > 0)
                            {
                                strSupplierPlantLinId_before = dtCheckTime_sp.Rows[dtCheckTime_sp.Rows.Count - 1]["LinId"].ToString();
                                strSupplierPlantFromTime_before = dtCheckTime_sp.Rows[dtCheckTime_sp.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(strSupplierPlantFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("修改第{0}行工区有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                        }
                        drImport["vcSupplierPlant"] = strSupplierPlant;
                        drImport["SupplierPlant_ed"] = strSupplierPlant_ed;
                        drImport["SupplierPlantLinId_ed"] = strSupplierPlantLinId_ed == "" ? "999" : strSupplierPlantLinId_ed;
                        drImport["SupplierPlantFromTime_ed"] = strSupplierPlantFromTime_ed;
                        drImport["SupplierPlantToTime_ed"] = strSupplierPlantToTime_ed;
                        #endregion

                        #region 验证收容数
                        string strBoxPackingQty_ed = listInfoData[i]["BoxPackingQty_ed"].ToString();
                        string strBoxLinId_ed = listInfoData[i]["BoxLinId_ed"] == null ? "" : listInfoData[i]["BoxLinId_ed"].ToString();
                        string strBoxFromTime_ed = listInfoData[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = listInfoData[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = listInfoData[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = listInfoData[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = listInfoData[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = listInfoData[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = listInfoData[i]["BoxVolume_ed"].ToString();
                        if (strSupplierPlantLinId_ed == "-1")
                        {
                            string strBoxLinId_before = "";
                            string strBoxFromTime_before = "";
                            DataTable dtCheckTime_pq = getEditLoadInfo("PackingQtyEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant_ed);
                            if (dtCheckTime_pq.Rows.Count > 0)
                            {
                                strBoxLinId_before = dtCheckTime_pq.Rows[dtCheckTime_pq.Rows.Count - 1]["LinId"].ToString();
                                strBoxFromTime_before = dtCheckTime_pq.Rows[dtCheckTime_pq.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strBoxFromTime_before) >= Convert.ToDateTime(strBoxFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("修改第{0}行收容数有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                        }
                        drImport["BoxPackingQty_ed"] = strBoxPackingQty_ed;
                        drImport["BoxLinId_ed"] = strBoxLinId_ed == "" ? "999" : strBoxLinId_ed;
                        drImport["BoxFromTime_ed"] = strBoxFromTime_ed;
                        drImport["BoxToTime_ed"] = strBoxToTime_ed;
                        drImport["BoxType_ed"] = strBoxType_ed;
                        drImport["BoxLength_ed"] = strBoxLength_ed == "" ? "0" : strBoxLength_ed;
                        drImport["BoxWidth_ed"] = strBoxWidth_ed == "" ? "0" : strBoxWidth_ed;
                        drImport["BoxHeight_ed"] = strBoxHeight_ed == "" ? "0" : strBoxHeight_ed;
                        drImport["BoxVolume_ed"] = strBoxVolume_ed == "" ? "0" : strBoxVolume_ed;
                        #endregion

                        #region 验证受入
                        string strSufferIn_ed = listInfoData[i]["SufferIn_ed"].ToString();
                        string strSufferInLinId_ed = listInfoData[i]["SufferInLinId_ed"].ToString();
                        string strSufferInFromTime_ed = listInfoData[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = listInfoData[i]["SufferInToTime_ed"].ToString();
                        if (strSupplierPlantLinId_ed == "-1")
                        {
                            string strSufferInLinId_before = "";
                            string strSufferInFromTime_before = "";
                            DataTable dtCheckTime_si = getEditLoadInfo("SufferInEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant_ed);
                            if (dtCheckTime_si.Rows.Count > 0)
                            {
                                strSufferInLinId_before = dtCheckTime_si.Rows[dtCheckTime_si.Rows.Count - 1]["LinId"].ToString();
                                strSufferInFromTime_before = dtCheckTime_si.Rows[dtCheckTime_si.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(strSufferInFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("修改第{0}行受入有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                        }
                        drImport["SufferIn_ed"] = strSufferIn_ed;
                        drImport["SufferInLinId_ed"] = strSufferInLinId_ed == "" ? "999" : strSufferInLinId_ed; ;
                        drImport["SufferInFromTime_ed"] = strSufferInFromTime_ed;
                        drImport["SufferInToTime_ed"] = strSufferInToTime_ed;
                        #endregion

                        #region 验证发注工厂
                        string strOrderPlant_ed = listInfoData[i]["OrderPlant_ed"].ToString();
                        string strOrderPlantLinId_ed = listInfoData[i]["OrderPlantLinId_ed"].ToString();
                        string strOrderPlantFromTime_ed = listInfoData[i]["OrderPlantFromTime_ed"].ToString();
                        string strOrderPlantToTime_ed = listInfoData[i]["OrderPlantToTime_ed"].ToString();
                        if (strSupplierPlantLinId_ed == "-1")
                        {
                            string strOrderPlantLinId_before = "";
                            string strOrderPlantFromTime_before = "";
                            DataTable dtCheckTime_op = getEditLoadInfo("OrderPlantEdit", strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant_ed);
                            if (dtCheckTime_op.Rows.Count > 0)
                            {
                                strOrderPlantLinId_before = dtCheckTime_op.Rows[dtCheckTime_op.Rows.Count - 1]["LinId"].ToString();
                                strOrderPlantFromTime_before = dtCheckTime_op.Rows[dtCheckTime_op.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strOrderPlantFromTime_before) >= Convert.ToDateTime(strOrderPlantFromTime_ed))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("修改第{0}行发注工厂有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                        }
                        drImport["OrderPlant_ed"] = strOrderPlant_ed;
                        drImport["OrderPlantLinId_ed"] = strOrderPlantLinId_ed == "" ? "999" : strOrderPlantLinId_ed; ; ;
                        drImport["OrderPlantFromTime_ed"] = strOrderPlantFromTime_ed;
                        drImport["OrderPlantToTime_ed"] = strOrderPlantToTime_ed;
                        #endregion

                        drImport["vcSupplierPlace"] = listInfoData[i]["vcSupplierPlace"].ToString();
                        drImport["vcInteriorProject"] = listInfoData[i]["vcInteriorProject"].ToString();
                        drImport["vcPassProject"] = listInfoData[i]["vcPassProject"].ToString();
                        drImport["vcFrontProject"] = listInfoData[i]["vcFrontProject"].ToString();
                        drImport["dFrontProjectTime"] = listInfoData[i]["dFrontProjectTime"].ToString();
                        drImport["dShipmentTime"] = listInfoData[i]["dShipmentTime"].ToString();
                        drImport["vcBillType"] = listInfoData[i]["vcBillType"].ToString();
                        drImport["vcRemark1"] = listInfoData[i]["vcRemark1"].ToString();
                        drImport["vcRemark2"] = listInfoData[i]["vcRemark2"].ToString();
                        drImport["vcOrderingMethod"] = listInfoData[i]["vcOrderingMethod"].ToString();
                        drImport["vcMandOrder"] = listInfoData[i]["vcMandOrder"].ToString();
                        dtImport.Rows.Add(drImport);
                    }
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
            for (int i = 0; i < dtImport.Rows.Count; i++)
            {
                bool bModFlag = true;
                bool bAddFlag = true;
                if (dtImport.Rows[i]["bModFlag"].ToString().ToUpper() == "TRUE")
                    bModFlag = true;
                else
                    bModFlag = false;
                if (dtImport.Rows[i]["bAddFlag"].ToString().ToUpper() == "TRUE")
                    bAddFlag = true;
                else
                    bAddFlag = false;
                if (bAddFlag)
                {
                    #region AddNewRow
                    DataRow dataRow_add = dtAddInfo.NewRow();
                    dataRow_add["dSyncTime"] = dtImport.Rows[i]["dSyncTime"].ToString();
                    dataRow_add["vcChanges"] = dtImport.Rows[i]["vcChanges"].ToString();
                    dataRow_add["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                    dataRow_add["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                    dataRow_add["vcPartENName"] = dtImport.Rows[i]["vcPartENName"].ToString();
                    dataRow_add["vcCarfamilyCode"] = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    dataRow_add["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                    dataRow_add["dFromTime"] = dtImport.Rows[i]["dFromTime"].ToString();
                    dataRow_add["dToTime"] = dtImport.Rows[i]["dToTime"].ToString();
                    dataRow_add["vcPartId_Replace"] = dtImport.Rows[i]["vcPartId_Replace"].ToString();
                    dataRow_add["vcInOut"] = dtImport.Rows[i]["vcInOut"].ToString();
                    dataRow_add["vcOESP"] = dtImport.Rows[i]["vcOESP"].ToString();
                    dataRow_add["vcHaoJiu"] = dtImport.Rows[i]["vcHaoJiu"].ToString();
                    dataRow_add["vcOldProduction"] = dtImport.Rows[i]["vcOldProduction"].ToString();
                    dataRow_add["dOldStartTime"] = dtImport.Rows[i]["dOldStartTime"].ToString();
                    dataRow_add["dDebugTime"] = dtImport.Rows[i]["dDebugTime"].ToString();
                    dataRow_add["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_add["dSupplierFromTime"] = dtImport.Rows[i]["dSupplierFromTime"].ToString();
                    dataRow_add["dSupplierToTime"] = dtImport.Rows[i]["dSupplierToTime"].ToString();
                    dataRow_add["vcSupplierName"] = dtImport.Rows[i]["vcSupplierName"].ToString();
                    dtAddInfo.Rows.Add(dataRow_add);
                    #endregion

                    #region HistoryNewRow
                    DataRow dataRow_History = dtOperHistory.NewRow();
                    dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                    dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                    dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                    dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_History["vcAction"] = "新增手配情报=>" +
                        "  包装厂:" + dtImport.Rows[i]["vcPackingPlant"].ToString() +
                        "；品番：" + dtImport.Rows[i]["vcPartId"].ToString() +
                        "；收货方：" + dtImport.Rows[i]["vcReceiver"].ToString() +
                        "；供应商：" + dtImport.Rows[i]["vcSupplierId"].ToString();
                    dataRow_History["error"] = "";
                    dtOperHistory.Rows.Add(dataRow_History);
                    #endregion
                }
                if (bAddFlag == false && bModFlag == true)
                {
                    string strOperItem = string.Empty;
                    string PackingPlant = dtImport.Rows[i]["vcPackingPlant"].ToString();
                    string PartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string Receiver = dtImport.Rows[i]["vcReceiver"].ToString();
                    string SupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string SupplierPlant = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                    //1.判断vcSupplierPlant是否需要修改
                    if (dtImport.Rows[i]["SupplierPlantLinId_ed"] != null && dtImport.Rows[i]["SupplierPlantLinId_ed"].ToString() == "-1")
                    {
                        string strSupplierPlant_ed = dtImport.Rows[i]["SupplierPlant_ed"].ToString();
                        string strSupplierPlantFromTime_ed = dtImport.Rows[i]["SupplierPlantFromTime_ed"].ToString();
                        string strSupplierPlantToTime_ed = dtImport.Rows[i]["SupplierPlantToTime_ed"].ToString();
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
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行工区有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
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
                        dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                        dataRow_History["vcAction"] = "增加工区信息=>" +
                            "  工区:" + strSupplierPlant_ed +
                            "；使用开始：" + strSupplierPlantFromTime_ed +
                            "；使用结束：" + strSupplierPlantToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    //2.判断iPackingQty是否需要修改
                    if (dtImport.Rows[i]["BoxLinId_ed"] != null && dtImport.Rows[i]["BoxLinId_ed"].ToString() == "-1")
                    {
                        SupplierPlant = dtImport.Rows[i]["vcSupplierPlant"].ToString();
                        string strBoxPackingQty_ed = dtImport.Rows[i]["BoxPackingQty_ed"].ToString();
                        string strBoxLinId_ed = dtImport.Rows[i]["BoxLinId_ed"].ToString();
                        string strBoxFromTime_ed = dtImport.Rows[i]["BoxFromTime_ed"].ToString();
                        string strBoxToTime_ed = dtImport.Rows[i]["BoxToTime_ed"].ToString();
                        string strBoxType_ed = dtImport.Rows[i]["BoxType_ed"].ToString();
                        string strBoxLength_ed = dtImport.Rows[i]["BoxLength_ed"].ToString();
                        string strBoxWidth_ed = dtImport.Rows[i]["BoxWidth_ed"].ToString();
                        string strBoxHeight_ed = dtImport.Rows[i]["BoxHeight_ed"].ToString();
                        string strBoxVolume_ed = dtImport.Rows[i]["BoxVolume_ed"].ToString();
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
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行收容数有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
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
                        dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
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
                    if (dtImport.Rows[i]["SufferInLinId_ed"] != null && dtImport.Rows[i]["SufferInLinId_ed"].ToString() == "-1")
                    {
                        SupplierPlant = dtImport.Rows[i]["vcSupplierPlant"].ToString();
                        string strSufferIn_ed = dtImport.Rows[i]["SufferIn_ed"].ToString();
                        string strSufferInLinId_ed = dtImport.Rows[i]["SufferInLinId_ed"].ToString();
                        string strSufferInFromTime_ed = dtImport.Rows[i]["SufferInFromTime_ed"].ToString();
                        string strSufferInToTime_ed = dtImport.Rows[i]["SufferInToTime_ed"].ToString();
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
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行受入有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
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
                        dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                        dataRow_History["vcAction"] = "增加受入信息=>" +
                            "  受入:" + strSufferIn_ed +
                            "；使用开始：" + strSufferInFromTime_ed +
                            "；使用结束：" + strSufferInToTime_ed;
                        dataRow_History["error"] = strError;
                        dtOperHistory.Rows.Add(dataRow_History);
                        #endregion
                    }
                    //4.判断vcOrderPlant是否需要修改
                    if (dtImport.Rows[i]["OrderPlantLinId_ed"] != null && dtImport.Rows[i]["OrderPlantLinId_ed"].ToString() == "-1")
                    {
                        SupplierPlant = dtImport.Rows[i]["vcSupplierPlant"].ToString();
                        string strOrderPlant_ed = dtImport.Rows[i]["OrderPlant_ed"].ToString();
                        string strOrderPlantLinId_ed = dtImport.Rows[i]["OrderPlantLinId_ed"].ToString();
                        string strOrderPlantFromTime_ed = dtImport.Rows[i]["OrderPlantFromTime_ed"].ToString();
                        string strOrderPlantToTime_ed = dtImport.Rows[i]["OrderPlantToTime_ed"].ToString();
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
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("第{0}行发注工厂有效期维护有错误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                dtMessage.Rows.Add(dataRow);
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
                        dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                        dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                        dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                        dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
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
                        if (drOperCheck[0]["vcSupplierPlace"].ToString() != dtImport.Rows[i]["vcSupplierPlace"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改供应商出荷地信息:" +
                                drOperCheck[0]["vcSupplierPlace"].ToString() + "==>" + dtImport.Rows[i]["vcSupplierPlace"].ToString();
                        }
                        //5.2   vcInteriorProject
                        if (drOperCheck[0]["vcInteriorProject"].ToString() != dtImport.Rows[i]["vcInteriorProject"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改内制工程信息:" +
                                drOperCheck[0]["vcInteriorProject"].ToString() + "==>" + dtImport.Rows[i]["vcInteriorProject"].ToString();
                        }
                        //5.3   vcPassProject
                        if (drOperCheck[0]["vcPassProject"].ToString() != dtImport.Rows[i]["vcPassProject"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改通过工程信息:" +
                                drOperCheck[0]["vcPassProject"].ToString() + "==>" + dtImport.Rows[i]["vcPassProject"].ToString();
                        }
                        //5.4   vcFrontProject
                        if (drOperCheck[0]["vcFrontProject"].ToString() != dtImport.Rows[i]["vcFrontProject"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改前工程信息:" +
                                drOperCheck[0]["vcFrontProject"].ToString() + "==>" + dtImport.Rows[i]["vcFrontProject"].ToString();
                        }
                        //5.5   dFrontProjectTime
                        if (drOperCheck[0]["dFrontProjectTime"].ToString() != dtImport.Rows[i]["dFrontProjectTime"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改前工程通过时间信息:" +
                                drOperCheck[0]["dFrontProjectTime"].ToString() + "==>" + dtImport.Rows[i]["dFrontProjectTime"].ToString();
                        }
                        //5.6   dShipmentTime
                        if (drOperCheck[0]["dShipmentTime"].ToString() != dtImport.Rows[i]["dShipmentTime"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改自工程出货时间信息:" +
                                drOperCheck[0]["dShipmentTime"].ToString() + "==>" + dtImport.Rows[i]["dShipmentTime"].ToString();
                        }
                        //5.7   vcBillType
                        if (drOperCheck[0]["vcBillType"].ToString() != dtImport.Rows[i]["vcBillType"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改单据区分信息:" +
                                drOperCheck[0]["vcBillType"].ToString() + "==>" + dtImport.Rows[i]["vcBillType"].ToString();
                        }
                        //5.8   vcRemark1
                        if (drOperCheck[0]["vcRemark1"].ToString() != dtImport.Rows[i]["vcRemark1"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改备注1信息:" +
                                drOperCheck[0]["vcRemark1"].ToString() + "==>" + dtImport.Rows[i]["vcRemark1"].ToString();
                        }
                        //5.9   vcRemark2
                        if (drOperCheck[0]["vcRemark2"].ToString() != dtImport.Rows[i]["vcRemark2"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改备注2信息:" +
                                drOperCheck[0]["vcRemark2"].ToString() + "==>" + dtImport.Rows[i]["vcRemark2"].ToString();
                        }
                        //5.10   vcOrderingMethod
                        if (drOperCheck[0]["vcOrderingMethod"].ToString() != dtImport.Rows[i]["vcOrderingMethod"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改订货方式信息:" +
                                drOperCheck[0]["vcOrderingMethod"].ToString() + "==>" + dtImport.Rows[i]["vcOrderingMethod"].ToString();
                        }
                        //5.11   vcMandOrder
                        if (drOperCheck[0]["vcMandOrder"].ToString() != dtImport.Rows[i]["vcMandOrder"].ToString())
                        {
                            bCheckUpdate = true;
                            strOperItem += "||修改强制订货信息:" +
                                drOperCheck[0]["vcMandOrder"].ToString() + "==>" + dtImport.Rows[i]["vcMandOrder"].ToString();
                        }
                        #endregion
                        if (bCheckUpdate)
                        {
                            #region AddNewRow
                            DataRow dataRow_mod = dtModInfo.NewRow();
                            dataRow_mod["LinId"] = dtImport.Rows[i]["LinId"].ToString() == "" ? "999" : dtImport.Rows[i]["LinId"].ToString();
                            dataRow_mod["vcSupplierPlace"] = dtImport.Rows[i]["vcSupplierPlace"].ToString();
                            dataRow_mod["vcInteriorProject"] = dtImport.Rows[i]["vcInteriorProject"].ToString();
                            dataRow_mod["vcPassProject"] = dtImport.Rows[i]["vcPassProject"].ToString();
                            dataRow_mod["vcFrontProject"] = dtImport.Rows[i]["vcFrontProject"].ToString();
                            dataRow_mod["dFrontProjectTime"] = dtImport.Rows[i]["dFrontProjectTime"].ToString() == "" ? "" : Convert.ToDateTime(dtImport.Rows[i]["dFrontProjectTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            dataRow_mod["dShipmentTime"] = dtImport.Rows[i]["dShipmentTime"].ToString() == "" ? "" : Convert.ToDateTime(dtImport.Rows[i]["dShipmentTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            dataRow_mod["vcBillType"] = dtImport.Rows[i]["vcBillType"].ToString();
                            dataRow_mod["vcOrderingMethod"] = dtImport.Rows[i]["vcOrderingMethod"].ToString();
                            dataRow_mod["vcMandOrder"] = dtImport.Rows[i]["vcMandOrder"].ToString();
                            dataRow_mod["vcRemark1"] = dtImport.Rows[i]["vcRemark1"].ToString();
                            dataRow_mod["vcRemark2"] = dtImport.Rows[i]["vcRemark2"].ToString();
                            dtModInfo.Rows.Add(dataRow_mod);
                            #endregion

                            //记录更新履历
                            #region HistoryNewRow
                            DataRow dataRow_History = dtOperHistory.NewRow();
                            dataRow_History["vcPackingPlant"] = dtImport.Rows[i]["vcPackingPlant"].ToString();
                            dataRow_History["vcPartId"] = dtImport.Rows[i]["vcPartId"].ToString();
                            dataRow_History["vcReceiver"] = dtImport.Rows[i]["vcReceiver"].ToString();
                            dataRow_History["vcSupplierId"] = dtImport.Rows[i]["vcSupplierId"].ToString();
                            dataRow_History["vcAction"] = strOperItem;
                            dataRow_History["error"] = "";
                            dtOperHistory.Rows.Add(dataRow_History);
                            #endregion
                        }
                    }
                }
            }
            if (dtMessage == null || dtMessage.Rows.Count == 0)
            {
                fs0603_DataAccess.setSPInfo(dtAddInfo, dtModInfo, dtModInfo_SP, dtModInfo_PQ, dtModInfo_SI, dtModInfo_OP, dtOperHistory,
                    strOperId, ref dtMessage);
            }
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
                dataTable.Columns.Add("iOrderQuantity", typeof(int));
                dataTable.Columns.Add("iDuiYingQuantity", typeof(int));
                dataTable.Columns.Add("dDeliveryDate", typeof(string));
                dataTable.Columns.Add("dOutPutDate", typeof(string));
                dataTable.Columns.Add("dReplyOverDate", typeof(string));
                dataTable.Columns.Add("vcInputType", typeof(string));//company
            }
            return dataTable;
        }
    }
}
