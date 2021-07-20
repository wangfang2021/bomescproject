using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using QRCoder;
using System.Data.SqlClient;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Convert = System.Convert;

namespace DataAccess
{
    public class FS0614_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable searchApi(string orderState, string targetYM, string orderNo, string vcOrderType, string dUpload, string memo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT a.iAutoId,a.vcTargetYear+a.vcTargetMonth+a.vcTargetDay AS vcTargetYM,c.vcOrderDifferentiation as vcOrderType,b.vcName as vcOrderState,a.vcMemo,REPLACE(CONVERT(VARCHAR(19),a.dUploadDate,21),'-','/') AS dUploadDate,REPLACE(CONVERT(VARCHAR(19),a.dCreateDate,21),'-','/') AS dCreateDate,a.vcFilePath,a.vcTargetWeek,a.vcFileOrderNo as vcOrderNo FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine(
                    "SELECT iAutoId,vcOrderNo,vcTargetYear,vcTargetMonth,vcTargetDay,vcOrderType,vcOrderState,vcMemo,dUploadDate,dCreateDate,vcFilePath,vcTargetWeek,vcFileOrderNo");
                sbr.AppendLine("FROM TOrderUploadManage  WHERE vcOrderShowFlag='1' ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C044'");
                sbr.AppendLine(") b ON a.vcOrderState = b.vcValue");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcOrderDifferentiation,vcOrderInitials FROM TOrderDifferentiation");
                sbr.AppendLine(") c ON a.vcOrderType = c.vcOrderInitials");
                sbr.AppendLine("WHERE 1=1");
                if (!string.IsNullOrWhiteSpace(orderState))
                {
                    sbr.AppendLine("AND a.vcOrderState = '" + orderState + "'");
                }

                if (!string.IsNullOrWhiteSpace(targetYM))
                {
                    sbr.AppendLine("AND a.vcTargetYear+a.vcTargetMonth = '" + targetYM + "'");
                }

                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    sbr.AppendLine("AND a.vcFileOrderNo like '" + orderNo + "%'");
                }

                if (!string.IsNullOrWhiteSpace(vcOrderType))
                {
                    sbr.AppendLine("AND a.vcOrderType = '" + vcOrderType + "'");
                }

                if (!string.IsNullOrWhiteSpace(dUpload))
                {
                    sbr.AppendLine("AND Convert(varchar(10),a.dUploadDate,120) = " + ComFunction.getSqlValue(dUpload, true) + "");
                }

                if (!string.IsNullOrWhiteSpace(memo))
                {
                    sbr.AppendLine("AND vcMemo LIKE '%" + memo + "%'");
                }

                sbr.AppendLine(" ORDER BY a.dUploadDate desc");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 生成
        public bool CreateOrder(List<Dictionary<string, Object>> listInfoData, string path, string userId, string uionCode, ref bool bReault, ref DataTable dtMessage, ref List<DownNode> DownList, string rootPath, string email)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                FS0403_DataAccess fs0403_dataAccess = new FS0403_DataAccess();
                List<string> TargetYM = new List<string>();

                string yyMMdd = DateTime.Now.ToString("yyMMdd");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string tmp = ObjToString(listInfoData[i]["vcTargetYM"]);
                    if (!string.IsNullOrWhiteSpace(tmp))
                    {
                        TargetYM.Add(tmp.Substring(0, 6));
                    }
                }


                string url = "https://wx-m.ftms.com.cn/carowner/part?tabindex=3&tracingcode=";

                TargetYM = TargetYM.Distinct().ToList();
                //获取基础信息
                Hashtable dockTmp = new Hashtable();
                for (int i = 0; i < TargetYM.Count; i++)
                {
                    DataTable dt = getDockTable(TargetYM[i]);
                    dockTmp.Add(TargetYM[i].ToString(), dt);
                }
                //获取soq验收数量
                DataTable SoqDt = new DataTable();
                DataRow[] drArrayN = null; //0 內制 1外制
                DataRow[] drArrayW = null; //0 內制 1外制
                DataRow[] drArrayTmp = null;
                if (TargetYM.Count > 0)
                {
                    //获取soq验收数量
                    SoqDt = getSoqDt(TargetYM);
                }

                //获取订单号
                DataTable OrderNo = getOrderNo();
                //获取纳期
                DataTable OrderNQ = getNQ();
                //获取包装厂
                DataTable PackSpot = getPackSpot();
                //获取包装工厂
                string vcPackingFactory = uionCode;

                ////获取标签号
                //int SeqNo = getNum();
                //获取包装单位
                DataTable PackUnit = getPackUnit();
                //获取供应商信息
                DataTable Supplier = getSupplier();
                //获取ED信息
                DataTable EDPart = getEDPart();

                //获取包材信息
                DataTable PackInfo = getPackInfo();
                PackEmail packEmail = new PackEmail(rootPath);

                SqlCommand sqlCommand_deleteinfo = sqlConnection.CreateCommand();
                sqlCommand_deleteinfo.Transaction = sqlTransaction;
                sqlCommand_deleteinfo.CommandType = CommandType.Text;
                StringBuilder strSql_deleteinfo = new StringBuilder();
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_tag_FS1103 where vcOperatorID='" + userId + "'");
                sqlCommand_deleteinfo.CommandText = strSql_deleteinfo.ToString();
                sqlCommand_deleteinfo.ExecuteNonQuery();


                //读取文件
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string msg = "";
                    string Type = getOrderType(listInfoData[i]["vcOrderType"].ToString());
                    Order order = GetPartFromFile(path + listInfoData[i]["vcFilePath"].ToString(), listInfoData[i]["vcOrderNo"].ToString(), ref msg);
                    string vcOrderNo = order.Head.No;
                    StringBuilder sbr = new StringBuilder();
                    List<EDNode> EDList = new List<EDNode>();

                    if (Type.Equals("S"))
                    {
                        #region 获取基础数据

                        string vcMakingOrderType = getTypeMethod(Type);
                        drArrayN = SoqDt.Select("vcInOutFlag='0' AND vcMakingOrderType in (" + vcMakingOrderType + ")"); //0 內制 1外制
                        drArrayW = SoqDt.Select("vcInOutFlag='1' AND vcMakingOrderType in (" + vcMakingOrderType + ")"); //0 內制 1外制
                        drArrayTmp = drArrayN;

                        string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6);
                        string TargetTmp = ObjToString(listInfoData[i]["vcTargetYM"]);
                        DateTime Time = DateTime.Parse(TargetTmp.Substring(0, 4) + "-" + TargetTmp.Substring(4, 2) + "-01");
                        DateTime LastTime = Time.AddMonths(1).AddDays(-1);

                        #endregion

                        #region 月度校验
                        int No = 1;
                        string strInOutflag = string.Empty;
                        Dictionary<string, string> dicPartNo = new Dictionary<string, string>();

                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcSupplierId = "";
                            string inout = "";
                            string vcOrderNum = detail.QTY;
                            string vcOrderingMethod = "";

                            string dateTime = detail.Date.Trim();
                            string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                            string isTag = "";
                            //检测品番表是否存在该品番
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, (DataTable)dockTmp[TargetTmp.Substring(0, 6)]);
                            if (hashtable.Keys.Count > 0)
                            {
                                inout = ObjToString(hashtable["vcInOut"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcOrderingMethod = ObjToString(hashtable["vcOrderingMethod"]);
                                isTag = ObjToString(hashtable["vcSupplierPacking"]);


                                //验证品番个数 及其总数 以及内外区分
                                if (No == 1)
                                {
                                    strInOutflag = inout;
                                    if (inout == "0")
                                    {
                                        drArrayTmp = drArrayN;
                                    }
                                    if (inout == "1")
                                    {
                                        drArrayTmp = drArrayW;
                                    }
                                    No++;
                                }
                                else
                                {
                                    if (inout != strInOutflag) //判定此次文件的内外区分是否一致
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "与第一个品番的内外区分不一样";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }
                                // 判定品番是否在临时品番里面
                                bool isExist = false;
                                for (int m = 0; m < drArrayTmp.Length; m++)
                                {
                                    if (drArrayTmp[m]["vcPart_id"].ToString() == vcPart_id)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "解析的内外区分与主数据该品番的内外不对应";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }

                                //检测数量
                                if (SoqDt.Rows.Count > 0)
                                {
                                    if (!CheckTotalNumEqual(OrderTargetYM, vcPart_id, vcOrderingMethod, SoqDt, detail.QTY))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "订单数量不正确";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = "";
                                    dataRow["vcMessage"] = OrderTargetYM + "月的月度Replay的数据为0条";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }

                                if (isTag.Equals("1"))
                                {
                                    //检索包装工厂
                                    DataRow[] row = PackUnit.Select("vcPart_id = '" + detail.PartsNo + "' and vcReceiver = '" + detail.CPD + "' and vcSupplierId = '" + vcSupplierId + "' and dTimeFrom<='" + Time + "' and dTimeTo>='" + Time + "' ");
                                    if (row.Length == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "没有设定包装单位";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }

                                    //检查供应商信息
                                    DataRow[] row1 = Supplier.Select("vcPart_Id = '" + detail.PartsNo + "' AND vcCPDCompany = '" + detail.CPD + "' AND vcSupplier_id = '" + vcSupplierId + "' AND dTimeFrom <= '" + Time + "' AND dTimeTo >= '" + Time + "'");
                                    if (row1.Length == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "没有标签信息";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }

                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = vcOrderNo;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = vcOrderNo + "文件中品番基础数据表不包含品番" + vcPart_id;
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                        #endregion

                        #region 生成订单

                        if (bReault)
                        {
                            foreach (Detail detail in order.Details)
                            {
                                #region 获取基础数据

                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string vcDock = "";
                                string vcSupplierId = "";
                                string vcSupplierPlant = "";//工区

                                string vcCarType = "";
                                string vcPartId_Replace = "";
                                string inout = "";
                                string packingSpot = "";

                                string isTag = "";

                                Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, (DataTable)dockTmp[TargetTmp.Substring(0, 6)]);

                                inout = ObjToString(hashtable["vcInOut"]);
                                vcDock = ObjToString(hashtable["vcSufferIn"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcCarType = ObjToString(hashtable["vcCarfamilyCode"]);
                                vcPartId_Replace = ObjToString(hashtable["vcPartId_Replace"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);
                                isTag = ObjToString(hashtable["vcSupplierPacking"]);

                                DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");

                                if (packingSpotRow.Length > 0)
                                {
                                    packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                }

                                DataRow[] rows = SoqDt.Select(" vcPart_id = '" + vcPart_id + "' ");
                                string dateTime = detail.Date.Trim();

                                #endregion

                                #region 插入正常订单
                                sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily1,vcPlantQtyDaily2,vcPlantQtyDaily3,vcPlantQtyDaily4,vcPlantQtyDaily5,vcPlantQtyDaily6,vcPlantQtyDaily7,vcPlantQtyDaily8,vcPlantQtyDaily9,vcPlantQtyDaily10,vcPlantQtyDaily11,vcPlantQtyDaily12,vcPlantQtyDaily13,vcPlantQtyDaily14,vcPlantQtyDaily15,vcPlantQtyDaily16,vcPlantQtyDaily17,vcPlantQtyDaily18,vcPlantQtyDaily19,vcPlantQtyDaily20,vcPlantQtyDaily21,vcPlantQtyDaily22,vcPlantQtyDaily23,vcPlantQtyDaily24,vcPlantQtyDaily25,vcPlantQtyDaily26,vcPlantQtyDaily27,vcPlantQtyDaily28,vcPlantQtyDaily29,vcPlantQtyDaily30,vcPlantQtyDaily31,  vcTargetMonthFlag, vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                sbr.Append(" VALUES( ");
                                sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(OrderTargetYM, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(Type, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(dateTime, true) + ",");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(packingSpot, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD1"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD2"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD3"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD4"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD5"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD6"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD7"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD8"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD9"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD10"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD11"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD12"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD13"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD14"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD15"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD16"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD17"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD18"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD19"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD20"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD21"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD22"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD23"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD24"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD25"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD26"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD27"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD28"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD29"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD30"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD31"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue('0', false) + ",");
                                sbr.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                sbr.Append("'" + userId + "',");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iPartNums"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, true) + ",");
                                sbr.Append("'0','0') \r\n");

                                #endregion

                                #region 插入标签打印
                                if (isTag.Equals("1"))
                                {
                                    int qty = Convert.ToInt32(detail.QTY);
                                    //Add
                                    DataRow[] row = PackUnit.Select("vcPart_id = '" + detail.PartsNo + "' and vcReceiver = '" + detail.CPD + "' and vcSupplierId = '" + vcSupplierId + "' and dTimeFrom<='" + Time + "' and dTimeTo>='" + Time + "' ");
                                    int BZUnit = Convert.ToInt32(row[0]["vcBZUnit"].ToString());

                                    string tmpString1 = "LBLH2";//标签连番、
                                    string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                                    string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                                    //5.1 标签顺番更新==锁定顺番占用
                                    DataTable dtTagSeq = new P00001_DataAccess().setSeqNo(tmpString1, Convert.ToInt32(qty / BZUnit), formatServerTime);
                                    //5.2 标签号生成并更新TOperatorQB（需要一起更新）
                                    string strTagSeqNo = dtTagSeq.Rows[0][0].ToString().PadLeft(5, '0');//标签连番

                                    int SeqNo = Convert.ToInt32(strTagSeqNo) - 1;

                                    DataRow[] row1 = Supplier.Select("vcPart_Id = '" + detail.PartsNo + "' AND vcCPDCompany = '" + detail.CPD + "' AND vcSupplier_id = '" + vcSupplierId + "' AND dTimeFrom <= '" + Time + "' AND dTimeTo >= '" + Time + "'");
                                    string partNameCN = row1[0]["vcPartNameCN"].ToString();
                                    string vcSCSName = row1[0]["vcSCSName"].ToString();
                                    string vcSCSAdress = row1[0]["vcSCSAdress"].ToString();
                                    string vcZXBZNo = row1[0]["vcZXBZNo"].ToString();

                                    string partNameEN = ObjToString(hashtable["vcPartENName"]);

                                    string carName = row1[0]["vcCarTypeName"].ToString();
                                    string vcPart_id1 = detail.PartsNo.Substring(0, 5) + "-" + detail.PartsNo.Substring(5, 5) + "-" + detail.PartsNo.Substring(10, 2);
                                    bool isExist = true;
                                    DownNode node = new DownNode(vcOrderNo, vcSupplierId);
                                    for (int n = 0; n < DownList.Count; n++)
                                    {
                                        if (DownList[n].isExist(node))
                                        {
                                            isExist = false;
                                            break;
                                        }
                                    }

                                    if (isExist)
                                    {
                                        DownList.Add(node);
                                    }

                                    while (qty - BZUnit >= 0)
                                    {
                                        SeqNo = SeqNo + 1;

                                        string sf = yyMMdd + string.Format("{0:d5}", SeqNo);

                                        string qr = url + detail.PartsNo + sf;
                                        string qr1 = url + detail.PartsNo + sf + "B";


                                        byte[] iCodemage = GenerateQRCode(qr);//二维码信息
                                        byte[] iCodemage1 = GenerateQRCode(qr1);//二维码信息

                                        StringBuilder tagsbr = new StringBuilder();
                                        tagsbr.AppendLine("INSERT INTO dbo.tPrintTemp_tag_FS1103(vcPartsnameen,vcPart_id,vcCpdcompany,vcLabel,vcInno,vcPrintcount,vcGetnum,iQrcode,vcPartnamechineese,vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcSupplierId,vcOperatorID,dOperatorTime) VALUES (");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameEN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.CPD, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue("", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(BZUnit, false) + ",");
                                        tagsbr.AppendLine("@iQrcode,");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameCN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSAdress, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcZXBZNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(carName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                        tagsbr.AppendLine("'" + userId + "',GETDATE())");


                                        tagsbr.AppendLine("INSERT INTO dbo.TLabelList(vcPart_id1,vcPrintcount1,vcPartsnameen,vcPart_id,vcCpdcompany,vcLabel,vcInno,vcPrintcount,vcLabel1,vcGetnum,iQrcode,iQrcode1,vcPartnamechineese,vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcOperatorID,dOperatorTime,dFirstPrintTime)VALUES(");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcPart_id1, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf + "B", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameEN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.CPD, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue("", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(BZUnit, false) + ",");
                                        tagsbr.AppendLine("@iQrcode1,");
                                        tagsbr.AppendLine("@iQrcode2,");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameCN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSAdress, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcZXBZNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(carName, false) + ",");
                                        tagsbr.AppendLine("'" + userId + "',GETDATE(),GETDATE())");

                                        tagsbr.AppendLine("INSERT INTO dbo.TLabelList_KEY(vcLabelCode,vcHostIp,vcOperatorID,dOperatorTime) VALUES('" + sf + "','127.0.0.1', '" + userId + "', GETDATE() )");

                                        SqlCommand sqlCommandTag = sqlConnection.CreateCommand();
                                        sqlCommandTag.Transaction = sqlTransaction;
                                        sqlCommandTag.CommandType = CommandType.Text;
                                        sqlCommandTag.CommandText = tagsbr.ToString();
                                        sqlCommandTag.Parameters.Add("@iQrcode", SqlDbType.Image);
                                        sqlCommandTag.Parameters.Add("@iQrcode1", SqlDbType.Image);
                                        sqlCommandTag.Parameters.Add("@iQrcode2", SqlDbType.Image);
                                        sqlCommandTag.Parameters["@iQrcode"].Value = iCodemage;
                                        sqlCommandTag.Parameters["@iQrcode1"].Value = iCodemage;
                                        sqlCommandTag.Parameters["@iQrcode2"].Value = iCodemage1;
                                        sqlCommandTag.ExecuteNonQuery();

                                        qty = qty - BZUnit;
                                    }

                                }
                                #endregion

                                #region 插入ED订单

                                DataRow[] edRows = EDPart.Select("vcPart_id = '" + detail.PartsNo.Trim() + "' and vcSHF = '" + detail.CPD.Trim() + "' and vcSupplier_id = '" + vcSupplierId + "'");
                                if (edRows.Length > 0)
                                {
                                    string vcPart_idED = vcPart_id.Substring(0, 10) + "ED";

                                    Hashtable hashtableED = getDock(vcPart_idED, vcPackingFactory, (DataTable)dockTmp[TargetTmp.Substring(0, 6)]);

                                    vcSeqno = detail.ItemNo.Trim();

                                    detail.ItemNo.Trim();
                                    CPD = "";
                                    vcDock = "";
                                    vcSupplierId = "";
                                    vcSupplierPlant = "";//工区
                                    vcCarType = "";
                                    vcPartId_Replace = "";
                                    inout = "";
                                    packingSpot = "";

                                    CPD = ObjToString(hashtableED["vcReceiver"]);
                                    inout = ObjToString(hashtableED["vcInOut"]);
                                    vcDock = ObjToString(hashtableED["vcSufferIn"]);
                                    vcSupplierId = ObjToString(hashtableED["vcSupplierId"]);
                                    vcCarType = ObjToString(hashtableED["vcCarfamilyCode"]);
                                    vcPartId_Replace = ObjToString(hashtableED["vcPartId_Replace"]);
                                    vcSupplierPlant = ObjToString(hashtableED["vcSupplierPlant"]);


                                    DataRow[] packingSpotRow1 = PackSpot.Select("vcPart_id = '" + vcPart_idED + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");

                                    if (packingSpotRow1.Length > 0)
                                    {
                                        packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                    }

                                    EDNode node = new EDNode();

                                    node.vcPackingFactory = vcPackingFactory;
                                    node.OrderTargetYM = OrderTargetYM;
                                    node.vcDock = vcDock;
                                    node.CPD = CPD;
                                    node.Type = Type;
                                    node.vcCarType = vcCarType;
                                    node.vcOrderNo = vcOrderNo + "ED";
                                    node.vcSeqno = vcSeqno;
                                    node.dOrderDate = dateTime;
                                    node.vcPart_id = vcPart_idED;
                                    node.inout = inout;
                                    node.packingSpot = packingSpot;
                                    node.vcSupplierId = vcSupplierId;
                                    node.vcSupplierPlant = vcSupplierPlant;

                                    node.id1 = Convert.ToInt32(rows[0]["iD1"].ToString() == "" ? "0" : rows[0]["iD1"].ToString());
                                    node.id2 = Convert.ToInt32(rows[0]["iD2"].ToString() == "" ? "0" : rows[0]["iD2"].ToString());
                                    node.id3 = Convert.ToInt32(rows[0]["iD3"].ToString() == "" ? "0" : rows[0]["iD3"].ToString());
                                    node.id4 = Convert.ToInt32(rows[0]["iD4"].ToString() == "" ? "0" : rows[0]["iD4"].ToString());
                                    node.id5 = Convert.ToInt32(rows[0]["iD5"].ToString() == "" ? "0" : rows[0]["iD5"].ToString());
                                    node.id6 = Convert.ToInt32(rows[0]["iD6"].ToString() == "" ? "0" : rows[0]["iD6"].ToString());
                                    node.id7 = Convert.ToInt32(rows[0]["iD7"].ToString() == "" ? "0" : rows[0]["iD7"].ToString());
                                    node.id8 = Convert.ToInt32(rows[0]["iD8"].ToString() == "" ? "0" : rows[0]["iD8"].ToString());
                                    node.id9 = Convert.ToInt32(rows[0]["iD9"].ToString() == "" ? "0" : rows[0]["iD9"].ToString());
                                    node.id10 = Convert.ToInt32(rows[0]["iD10"].ToString() == "" ? "0" : rows[0]["iD10"].ToString());
                                    node.id11 = Convert.ToInt32(rows[0]["iD11"].ToString() == "" ? "0" : rows[0]["iD11"].ToString());
                                    node.id12 = Convert.ToInt32(rows[0]["iD12"].ToString() == "" ? "0" : rows[0]["iD12"].ToString());
                                    node.id13 = Convert.ToInt32(rows[0]["iD13"].ToString() == "" ? "0" : rows[0]["iD13"].ToString());
                                    node.id14 = Convert.ToInt32(rows[0]["iD14"].ToString() == "" ? "0" : rows[0]["iD14"].ToString());
                                    node.id15 = Convert.ToInt32(rows[0]["iD15"].ToString() == "" ? "0" : rows[0]["iD15"].ToString());
                                    node.id16 = Convert.ToInt32(rows[0]["iD16"].ToString() == "" ? "0" : rows[0]["iD16"].ToString());
                                    node.id17 = Convert.ToInt32(rows[0]["iD17"].ToString() == "" ? "0" : rows[0]["iD17"].ToString());
                                    node.id18 = Convert.ToInt32(rows[0]["iD18"].ToString() == "" ? "0" : rows[0]["iD18"].ToString());
                                    node.id19 = Convert.ToInt32(rows[0]["iD19"].ToString() == "" ? "0" : rows[0]["iD19"].ToString());
                                    node.id20 = Convert.ToInt32(rows[0]["iD20"].ToString() == "" ? "0" : rows[0]["iD20"].ToString());
                                    node.id21 = Convert.ToInt32(rows[0]["iD21"].ToString() == "" ? "0" : rows[0]["iD21"].ToString());
                                    node.id22 = Convert.ToInt32(rows[0]["iD22"].ToString() == "" ? "0" : rows[0]["iD22"].ToString());
                                    node.id23 = Convert.ToInt32(rows[0]["iD23"].ToString() == "" ? "0" : rows[0]["iD23"].ToString());
                                    node.id24 = Convert.ToInt32(rows[0]["iD24"].ToString() == "" ? "0" : rows[0]["iD24"].ToString());
                                    node.id25 = Convert.ToInt32(rows[0]["iD25"].ToString() == "" ? "0" : rows[0]["iD25"].ToString());
                                    node.id26 = Convert.ToInt32(rows[0]["iD26"].ToString() == "" ? "0" : rows[0]["iD26"].ToString());
                                    node.id27 = Convert.ToInt32(rows[0]["iD27"].ToString() == "" ? "0" : rows[0]["iD27"].ToString());
                                    node.id28 = Convert.ToInt32(rows[0]["iD28"].ToString() == "" ? "0" : rows[0]["iD28"].ToString());
                                    node.id29 = Convert.ToInt32(rows[0]["iD29"].ToString() == "" ? "0" : rows[0]["iD29"].ToString());
                                    node.id30 = Convert.ToInt32(rows[0]["iD30"].ToString() == "" ? "0" : rows[0]["iD30"].ToString());
                                    node.id31 = Convert.ToInt32(rows[0]["iD31"].ToString() == "" ? "0" : rows[0]["iD31"].ToString());

                                    bool flag = true;
                                    for (int m = 0; m < EDList.Count; m++)
                                    {
                                        if (EDList[m].isExist(node))
                                        {
                                            EDList[m].Add(node);
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag)
                                        EDList.Add(node);
                                }

                                #endregion
                            }

                            #region 月度内示
                            SqlCommand sqlCommandNeiShi = sqlConnection.CreateCommand();
                            sqlCommandNeiShi.Transaction = sqlTransaction;
                            sqlCommandNeiShi.CommandType = CommandType.Text;
                            sqlCommandNeiShi.CommandText = MonthToNeishi(Time, userId);
                            sqlCommandNeiShi.ExecuteNonQuery();
                            #endregion

                            #region ED订单
                            SqlCommand sqlCommandED = sqlConnection.CreateCommand();
                            sqlCommandED.Transaction = sqlTransaction;
                            sqlCommandED.CommandType = CommandType.Text;

                            for (int m = 0; m < EDList.Count; m++)
                            {
                                StringBuilder edBuilder = new StringBuilder();
                                edBuilder.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily1,vcPlantQtyDaily2,vcPlantQtyDaily3,vcPlantQtyDaily4,vcPlantQtyDaily5,vcPlantQtyDaily6,vcPlantQtyDaily7,vcPlantQtyDaily8,vcPlantQtyDaily9,vcPlantQtyDaily10,vcPlantQtyDaily11,vcPlantQtyDaily12,vcPlantQtyDaily13,vcPlantQtyDaily14,vcPlantQtyDaily15,vcPlantQtyDaily16,vcPlantQtyDaily17,vcPlantQtyDaily18,vcPlantQtyDaily19,vcPlantQtyDaily20,vcPlantQtyDaily21,vcPlantQtyDaily22,vcPlantQtyDaily23,vcPlantQtyDaily24,vcPlantQtyDaily25,vcPlantQtyDaily26,vcPlantQtyDaily27,vcPlantQtyDaily28,vcPlantQtyDaily29,vcPlantQtyDaily30,vcPlantQtyDaily31,  vcTargetMonthFlag, vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                edBuilder.Append(" VALUES( ");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcPackingFactory, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].OrderTargetYM, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcDock, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].CPD, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].Type, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcOrderNo, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcSeqno, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].dOrderDate, true) + ",");
                                edBuilder.Append("GetDate(),");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcPart_id, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].inout, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcCarType, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcPart_id, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].packingSpot, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcSupplierId, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id1, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id2, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id3, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id4, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id5, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id6, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id7, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id8, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id9, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id10, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id11, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id12, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id13, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id14, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id15, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id16, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id17, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id18, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id19, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id20, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id21, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id22, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id23, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id24, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id25, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id26, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id27, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id28, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id29, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id30, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id31, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue('0', false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                edBuilder.Append("'" + userId + "',");
                                edBuilder.Append("GetDate(),");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].iPartNums(), true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcSupplierPlant, true) + ",");
                                edBuilder.Append("'0','0') \r\n");
                                sqlCommandED.CommandText = edBuilder.ToString();
                                sqlCommandED.ExecuteNonQuery();
                            }

                            #endregion

                            #region 修改订单状态
                            SqlCommand sqlCommandState = sqlConnection.CreateCommand();
                            sqlCommandState.Transaction = sqlTransaction;
                            sqlCommandState.CommandType = CommandType.Text;
                            int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                            StringBuilder sbrState = new StringBuilder();
                            sbrState.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE(),dCreateDate = GETDATE() WHERE iAutoId =" + iAutoId + " ");
                            sqlCommandState.CommandText = sbrState.ToString();
                            sqlCommandState.ExecuteNonQuery();
                            #endregion

                        }

                        #endregion

                    }
                    else if (Type.Equals("H") || Type.Equals("F") || Type.Equals("C"))
                    {
                        string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6);


                        #region 紧急检测
                        string vcOrderNoOld = getOrderNo(OrderNo, vcOrderNo);
                        string TargetTmp = ObjToString(listInfoData[i]["vcTargetYM"]);
                        DateTime Timet = DateTime.Parse(TargetTmp.Substring(0, 4) + "-" + TargetTmp.Substring(4, 2) + "-01");
                        //判断头
                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcSupplierId = "";
                            string vcOrderNum = detail.QTY;
                            string vcOrderingMethod = "";
                            string isTag = "";

                            string dateTime = detail.Date.Trim();
                            //检测品番表是否存在该品番
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, (DataTable)dockTmp[ObjToString(listInfoData[i]["vcTargetYM"])]);
                            if (hashtable.Keys.Count == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = vcOrderNo;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = vcOrderNo + "文件中品番基础数据表不包含品番" + vcPart_id;
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }

                            DataRow[] rowNQ = OrderNQ.Select(" vcOrderNo = '" + vcOrderNoOld + "' AND vcPart_id = '" + vcPart_id + "' AND TargetYM = '" + ObjToString(listInfoData[i]["vcTargetYM"]) + "'");

                            if (rowNQ.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = vcOrderNo;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = vcOrderNo + "文件中" + vcPart_id + "纳期不存在";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }


                            isTag = ObjToString(hashtable["vcSupplierPacking"]);
                            vcSupplierId = ObjToString(hashtable["vcSupplierId"]);

                            if (isTag.Equals("1"))
                            {
                                //检索包装工厂
                                DataRow[] row = PackUnit.Select("vcPart_id = '" + detail.PartsNo + "' and vcReceiver = '" + detail.CPD + "' and vcSupplierId = '" + vcSupplierId + "' and dTimeFrom<='" + Timet + "' and dTimeTo>='" + Timet + "' ");
                                if (row.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "没有设定包装单位";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }

                                //检查供应商信息
                                DataRow[] row1 = Supplier.Select("vcPart_Id = '" + detail.PartsNo + "' AND vcCPDCompany = '" + detail.CPD + "' AND vcSupplier_id = '" + vcSupplierId + "' AND dTimeFrom <= '" + Timet + "' AND dTimeTo >= '" + Timet + "'");
                                if (row1.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "没有标签信息";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                        }

                        #endregion

                        #region 订单做成

                        if (bReault)
                        {
                            string TargetYMJJ = ObjToString(listInfoData[i]["vcTargetYM"]);
                            DateTime Time = DateTime.Parse(TargetYMJJ.Substring(0, 4) + "-" + TargetYMJJ.Substring(4, 2) + "-01");
                            DateTime LastTime = Time.AddMonths(1).AddDays(-1);


                            foreach (Detail detail in order.Details)
                            {
                                #region 获取基础数据

                                string tmp = "";
                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string QTY = detail.QTY;
                                string vcDock = "";
                                string vcSupplierId = "";
                                string vcCarType = "";
                                string vcPartId_Replace = "";
                                string inout = "";
                                string packingSpot = "";
                                string vcSupplierPlant = ""; //工区
                                string vcHaoJiu = "";
                                string vcOrderPlant = "";
                                string vcSupplierPacking = "";
                                string vcSupplierPlace = "";
                                string isTag = "";
                                Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, (DataTable)dockTmp[ObjToString(listInfoData[i]["vcTargetYM"])]);

                                inout = ObjToString(hashtable["vcInOut"]);
                                vcDock = ObjToString(hashtable["vcSufferIn"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcCarType = ObjToString(hashtable["vcCarfamilyCode"]);
                                vcPartId_Replace = ObjToString(hashtable["vcPartId_Replace"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);
                                vcHaoJiu = ObjToString(hashtable["vcHaoJiu"]);
                                vcOrderPlant = ObjToString(hashtable["vcOrderPlant"]);
                                vcSupplierPacking = ObjToString(hashtable["vcSupplierPacking"]);
                                vcSupplierPlace = ObjToString(hashtable["vcSupplierPlace"]);
                                isTag = ObjToString(hashtable["vcSupplierPacking"]);

                                DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" +
                                                                           vcPackingFactory + "'");
                                if (packingSpotRow.Length > 0)
                                {
                                    packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                }

                                DataRow[] rowNQ = OrderNQ.Select(" vcOrderNo = '" + vcOrderNoOld + "' AND vcPart_id = '" + vcPart_id + "' AND TargetYM = '" + TargetYMJJ + "'");
                                Hashtable NQ = new Hashtable();

                                int sum = Convert.ToInt32(QTY);


                                for (int m = 0; m < rowNQ.Length; m++)
                                {
                                    if (sum == 0)
                                    {
                                        break;
                                    }

                                    List<string> tmp1 = new List<string>();
                                    int tmp2 = Convert.ToInt32(rowNQ[m]["iDuiYingQuantity"]);
                                    if (tmp2 > sum)
                                    {
                                        tmp2 = sum;
                                    }

                                    tmp1.Add(tmp2.ToString());
                                    tmp1.Add(rowNQ[m]["dDeliveryDate"].ToString());
                                    sum = sum - tmp2;
                                    NQ.Add(rowNQ[m]["dOutPutDate"].ToString(), tmp1);
                                }





                                string dateTime = detail.Date.Trim();

                                string name = "";
                                string value = "";

                                foreach (string key in NQ.Keys)
                                {
                                    name += "vcPlantQtyDaily" + Convert.ToInt32(key.Substring(6, 2)) + ",";
                                    List<string> num = (List<string>)NQ[key];
                                    value += ComFunction.getSqlValue(num[0], true) + ",";
                                }

                                #endregion

                                #region 新增订单
                                sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory,vcTargetYearMonth,vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id," + name + " vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                sbr.Append(" VALUES( ");
                                sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(TargetYMJJ, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(Type, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(dateTime, true) + ",");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(packingSpot, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                sbr.Append(value + "");
                                sbr.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                sbr.Append("'" + userId + "',");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, true) + ",");
                                sbr.Append("'0','0') \r\n");

                                #endregion

                                #region 紧急台账

                                foreach (string nqKey in NQ.Keys)
                                {
                                    List<string> list = (List<string>)NQ[nqKey];

                                    sbr.Append("INSERT INTO TEmergentOrderManage(dOrderHandleDate, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcDock, vcSupplier_id, vcWorkArea, vcCHCCode, vcCarType, vcOrderNum, dExpectReceiveDate, vcOderTimes, vcInjectionOrderNo, vcMemo, vcOperatorID, dOperatorTime, vcIsExportFlag)");
                                    sbr.Append("VALUES(GETDATE(),");
                                    sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + "  , ");
                                    sbr.Append(ComFunction.getSqlValue(inout, false) + "  , ");
                                    sbr.Append(ComFunction.getSqlValue(vcHaoJiu, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(vcOrderPlant, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(vcDock, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + "  , ");
                                    sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(vcSupplierPlace, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(vcCarType, false) + "  ,");
                                    sbr.Append(ComFunction.getSqlValue(list[0], false) + "  , ");
                                    sbr.Append(ComFunction.getSqlValue(list[1], true) + ",");
                                    sbr.Append("'01'  , ");
                                    sbr.Append("null  ,");
                                    sbr.Append("null  ,");
                                    sbr.Append("'" + userId + "'  ,");
                                    sbr.Append("GETDATE(),");
                                    sbr.Append("'0') \r\n");
                                }

                                #endregion

                                #region 插入标签打印
                                if (isTag.Equals("1"))
                                {
                                    int qty = Convert.ToInt32(detail.QTY);
                                    //Add
                                    DataRow[] row = PackUnit.Select("vcPart_id = '" + detail.PartsNo + "' and vcReceiver = '" + detail.CPD + "' and vcSupplierId = '" + vcSupplierId + "' and dTimeFrom<='" + Time + "' and dTimeTo>='" + Time + "' ");
                                    int BZUnit = Convert.ToInt32(row[0]["vcBZUnit"].ToString());

                                    string tmpString1 = "LBLH2";//标签连番、
                                    string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                                    string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                                    //5.1 标签顺番更新==锁定顺番占用
                                    DataTable dtTagSeq = new P00001_DataAccess().setSeqNo(tmpString1, Convert.ToInt32(qty / BZUnit), formatServerTime);
                                    //5.2 标签号生成并更新TOperatorQB（需要一起更新）
                                    string strTagSeqNo = dtTagSeq.Rows[0][0].ToString().PadLeft(5, '0');//标签连番

                                    int SeqNo = Convert.ToInt32(strTagSeqNo) - 1;

                                    DataRow[] row1 = Supplier.Select("vcPart_Id = '" + detail.PartsNo + "' AND vcCPDCompany = '" + detail.CPD + "' AND vcSupplier_id = '" + vcSupplierId + "' AND dTimeFrom <= '" + Time + "' AND dTimeTo >= '" + Time + "'");
                                    string partNameCN = row1[0]["vcPartNameCN"].ToString();
                                    string vcSCSName = row1[0]["vcSCSName"].ToString();
                                    string vcSCSAdress = row1[0]["vcSCSAdress"].ToString();
                                    string vcZXBZNo = row1[0]["vcZXBZNo"].ToString();

                                    string partNameEN = ObjToString(hashtable["vcPartENName"]);

                                    string carName = row1[0]["vcCarTypeName"].ToString();
                                    string vcPart_id1 = detail.PartsNo.Substring(0, 5) + "-" + detail.PartsNo.Substring(5, 5) + "-" + detail.PartsNo.Substring(10, 2);
                                    bool isExist = true;
                                    DownNode node = new DownNode(vcOrderNo, vcSupplierId);
                                    for (int n = 0; n < DownList.Count; n++)
                                    {
                                        if (DownList[n].isExist(node))
                                        {
                                            isExist = false;
                                            break;
                                        }
                                    }

                                    if (isExist)
                                    {
                                        DownList.Add(node);
                                    }

                                    while (qty - BZUnit >= 0)
                                    {
                                        SeqNo = SeqNo + 1;

                                        string sf = yyMMdd + string.Format("{0:d5}", SeqNo);

                                        string qr = url + detail.PartsNo + sf;
                                        string qr1 = url + detail.PartsNo + sf + "B";


                                        byte[] iCodemage = GenerateQRCode(qr);//二维码信息
                                        byte[] iCodemage1 = GenerateQRCode(qr1);//二维码信息

                                        StringBuilder tagsbr = new StringBuilder();
                                        tagsbr.AppendLine("INSERT INTO dbo.tPrintTemp_tag_FS1103(vcPartsnameen,vcPart_id,vcCpdcompany,vcLabel,vcInno,vcPrintcount,vcGetnum,iQrcode,vcPartnamechineese,vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcSupplierId,vcOperatorID,dOperatorTime) VALUES (");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameEN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.CPD, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue("", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(BZUnit, false) + ",");
                                        tagsbr.AppendLine("@iQrcode,");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameCN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSAdress, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcZXBZNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(carName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                        tagsbr.AppendLine("'" + userId + "',GETDATE())");


                                        tagsbr.AppendLine("INSERT INTO dbo.TLabelList(vcPart_id1,vcPrintcount1,vcPartsnameen,vcPart_id,vcCpdcompany,vcLabel,vcInno,vcPrintcount,vcLabel1,vcGetnum,iQrcode,iQrcode1,vcPartnamechineese,vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcOperatorID,dOperatorTime,dFirstPrintTime)VALUES(");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcPart_id1, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf + "B", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameEN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.CPD, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue("", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(BZUnit, false) + ",");
                                        tagsbr.AppendLine("@iQrcode1,");
                                        tagsbr.AppendLine("@iQrcode2,");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameCN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSAdress, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcZXBZNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(carName, false) + ",");
                                        tagsbr.AppendLine("'" + userId + "',GETDATE(),GETDATE())");

                                        tagsbr.AppendLine("INSERT INTO dbo.TLabelList_KEY(vcLabelCode,vcHostIp,vcOperatorID,dOperatorTime) VALUES('" + sf + "','127.0.0.1', '" + userId + "', GETDATE() )");

                                        SqlCommand sqlCommandTag = sqlConnection.CreateCommand();
                                        sqlCommandTag.Transaction = sqlTransaction;
                                        sqlCommandTag.CommandType = CommandType.Text;
                                        sqlCommandTag.CommandText = tagsbr.ToString();
                                        sqlCommandTag.Parameters.Add("@iQrcode", SqlDbType.Image);
                                        sqlCommandTag.Parameters.Add("@iQrcode1", SqlDbType.Image);
                                        sqlCommandTag.Parameters.Add("@iQrcode2", SqlDbType.Image);
                                        sqlCommandTag.Parameters["@iQrcode"].Value = iCodemage;
                                        sqlCommandTag.Parameters["@iQrcode1"].Value = iCodemage;
                                        sqlCommandTag.Parameters["@iQrcode2"].Value = iCodemage1;
                                        sqlCommandTag.ExecuteNonQuery();

                                        qty = qty - BZUnit;
                                    }

                                }
                                #endregion

                                #region 插入ED订单

                                DataRow[] edRows = EDPart.Select("vcPart_id = '" + detail.PartsNo.Trim() + "' and vcSHF = '" + detail.CPD.Trim() + "' and vcSupplier_id = '" + vcSupplierId + "'");
                                if (edRows.Length > 0)
                                {
                                    string vcPart_idED = vcPart_id.Substring(0, 10) + "ED";

                                    Hashtable hashtableED = getDock(vcPart_idED, vcPackingFactory, (DataTable)dockTmp[TargetTmp.Substring(0, 6)]);

                                    vcSeqno = detail.ItemNo.Trim();

                                    detail.ItemNo.Trim();
                                    CPD = "";
                                    vcDock = "";
                                    vcSupplierId = "";
                                    vcSupplierPlant = "";//工区
                                    vcCarType = "";
                                    vcPartId_Replace = "";
                                    inout = "";
                                    packingSpot = "";

                                    CPD = ObjToString(hashtableED["vcReceiver"]);
                                    inout = ObjToString(hashtableED["vcInOut"]);
                                    vcDock = ObjToString(hashtableED["vcSufferIn"]);
                                    vcSupplierId = ObjToString(hashtableED["vcSupplierId"]);
                                    vcCarType = ObjToString(hashtableED["vcCarfamilyCode"]);
                                    vcPartId_Replace = ObjToString(hashtableED["vcPartId_Replace"]);
                                    vcSupplierPlant = ObjToString(hashtableED["vcSupplierPlant"]);


                                    DataRow[] packingSpotRow1 = PackSpot.Select("vcPart_id = '" + vcPart_idED + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");

                                    if (packingSpotRow1.Length > 0)
                                    {
                                        packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                    }

                                    EDNode node = new EDNode();

                                    node.vcPackingFactory = vcPackingFactory;
                                    node.OrderTargetYM = OrderTargetYM;
                                    node.vcDock = vcDock;
                                    node.CPD = CPD;
                                    node.Type = Type;
                                    node.vcCarType = vcCarType;
                                    node.vcOrderNo = vcOrderNo + "ED";
                                    node.vcSeqno = vcSeqno;
                                    node.dOrderDate = dateTime;
                                    node.vcPart_id = vcPart_idED;
                                    node.inout = inout;
                                    node.packingSpot = packingSpot;
                                    node.vcSupplierId = vcSupplierId;
                                    node.vcSupplierPlant = vcSupplierPlant;


                                    foreach (string key in NQ.Keys)
                                    {
                                        List<string> num = (List<string>)NQ[key];
                                        int index = Convert.ToInt32(key.Substring(6, 2));
                                        int val = Convert.ToInt32(num[0]);
                                        node.setValue(index, val);
                                    }


                                    bool flag = true;
                                    for (int m = 0; m < EDList.Count; m++)
                                    {
                                        if (EDList[m].isExist(node))
                                        {
                                            EDList[m].Add(node);
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag)
                                        EDList.Add(node);
                                }

                                #endregion


                                #region 计算包材
                                foreach (string key in NQ.Keys)
                                {
                                    DataRow[] packrows = PackInfo.Select("dFrom<='" + key + "' AND dTo>='" + key + "' AND dPackFrom<='" + key + "' AND dPackTo>='" + key + "' AND vcPartsNo = '" + vcPart_id + "'");
                                    string Year = ((List<string>)NQ[key])[1].Substring(0, 4);
                                    string Month = ((List<string>)NQ[key])[1].Substring(4, 2);
                                    int day = Convert.ToInt32(((List<string>)NQ[key])[1].Substring(6, 2));
                                    double partnum = Convert.ToDouble(((List<string>)NQ[key])[0]);

                                    if (packrows.Length > 0)
                                    {
                                        for (int p = 0; p < packrows.Length; p++)
                                        {
                                            string packNo = packrows[p]["vcPackNo"].ToString();
                                            if (!packNo.ToUpper().Equals("MBC"))
                                            {
                                                double num = Convert.ToDouble(packrows[p]["iBiYao"]);
                                                double packnum = num * partnum;
                                                string supplierId = packrows[p]["vcSupplierCode"].ToString();
                                                PackSuccess pack = new PackSuccess(Year, Month, supplierId);
                                                pack.list.Add(new PackItem(Year, Month, packNo, day, packnum));
                                                packEmail.addSuccess(pack);
                                            }
                                            else
                                            {
                                                PackFail pack = new PackFail(Year, Month);
                                                pack.list.Add(new PackItem(Year, Month, vcPart_id + "(MBC)", day, partnum));
                                                packEmail.addFail(pack);
                                            }


                                        }
                                    }
                                    else
                                    {
                                        PackFail pack = new PackFail(Year, Month);
                                        pack.list.Add(new PackItem(Year, Month, vcPart_id, day, partnum));
                                        packEmail.addFail(pack);
                                    }
                                }


                                #endregion
                            }

                            #region ED订单
                            SqlCommand sqlCommandED = sqlConnection.CreateCommand();
                            sqlCommandED.Transaction = sqlTransaction;
                            sqlCommandED.CommandType = CommandType.Text;

                            for (int m = 0; m < EDList.Count; m++)
                            {
                                StringBuilder edBuilder = new StringBuilder();
                                edBuilder.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily1,vcPlantQtyDaily2,vcPlantQtyDaily3,vcPlantQtyDaily4,vcPlantQtyDaily5,vcPlantQtyDaily6,vcPlantQtyDaily7,vcPlantQtyDaily8,vcPlantQtyDaily9,vcPlantQtyDaily10,vcPlantQtyDaily11,vcPlantQtyDaily12,vcPlantQtyDaily13,vcPlantQtyDaily14,vcPlantQtyDaily15,vcPlantQtyDaily16,vcPlantQtyDaily17,vcPlantQtyDaily18,vcPlantQtyDaily19,vcPlantQtyDaily20,vcPlantQtyDaily21,vcPlantQtyDaily22,vcPlantQtyDaily23,vcPlantQtyDaily24,vcPlantQtyDaily25,vcPlantQtyDaily26,vcPlantQtyDaily27,vcPlantQtyDaily28,vcPlantQtyDaily29,vcPlantQtyDaily30,vcPlantQtyDaily31, vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                edBuilder.Append(" VALUES( ");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcPackingFactory, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].OrderTargetYM, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcDock, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].CPD, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].Type, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcOrderNo, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcSeqno, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].dOrderDate, true) + ",");
                                edBuilder.Append("GetDate(),");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcPart_id, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].inout, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcCarType, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcPart_id, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].packingSpot, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcSupplierId, false) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id1, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id2, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id3, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id4, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id5, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id6, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id7, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id8, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id9, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id10, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id11, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id12, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id13, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id14, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id15, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id16, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id17, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id18, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id19, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id20, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id21, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id22, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id23, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id24, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id25, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id26, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id27, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id28, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id29, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id30, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].id31, true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                edBuilder.Append("'" + userId + "',");
                                edBuilder.Append("GetDate(),");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].iPartNums(), true) + ",");
                                edBuilder.Append(ComFunction.getSqlValue(EDList[m].vcSupplierPlant, true) + ",");
                                edBuilder.Append("'0','0') \r\n");
                                sqlCommandED.CommandText = edBuilder.ToString();
                                sqlCommandED.ExecuteNonQuery();
                            }

                            #endregion

                            #region 修改订单状态

                            SqlCommand sqlCommandState = sqlConnection.CreateCommand();
                            sqlCommandState.Transaction = sqlTransaction;
                            sqlCommandState.CommandType = CommandType.Text;
                            int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                            StringBuilder sbrState = new StringBuilder();
                            sbrState.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE(),dCreateDate = GETDATE() WHERE iAutoId =" + iAutoId + " ");
                            sqlCommandState.CommandText = sbrState.ToString();
                            sqlCommandState.ExecuteNonQuery();

                            #endregion
                        }

                        #endregion
                    }
                    else if (Type.Equals("D"))
                    {
                        #region 日度订单校验
                        FS0403_DataAccess fs0403DataAccess = new FS0403_DataAccess();
                        string tm = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 4) + "-" + listInfoData[i]["vcTargetYM"].ToString().Substring(4, 2) + "-" + listInfoData[i]["vcTargetYM"].ToString().Substring(6, 2);
                        DataTable dtRiDuCheck = fs0403DataAccess.getModify(Convert.ToDateTime(tm));
                        if (dtRiDuCheck.Rows.Count == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = vcOrderNo;
                            dataRow["vcPartNo"] = "";
                            dataRow["vcMessage"] = "当前日的Soq日度订单平准化的品番数量为0条,无法进行日度校验!";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        else
                        {
                            string TargetTmp = ObjToString(listInfoData[i]["vcTargetYM"]);
                            DateTime Timet = DateTime.Parse(TargetTmp.Substring(0, 4) + "-" + TargetTmp.Substring(4, 2) + "-01");

                            foreach (Detail detail in order.Details)
                            {
                                string tmp = "";
                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string vcSupplierId = "";
                                string vcOrderNum = detail.QTY;
                                string vcOrderingMethod = "";
                                string isTag = "";
                                Hashtable hashtable = getDock(detail.PartsNo, detail.CPD, vcPackingFactory, (DataTable)dockTmp[ObjToString(listInfoData[i]["vcTargetYM"]).Substring(0, 6)]);
                                isTag = ObjToString(hashtable["vcSupplierPacking"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);

                                if (isTag.Equals("1"))
                                {
                                    //检索包装工厂
                                    DataRow[] row = PackUnit.Select("vcPart_id = '" + detail.PartsNo + "' and vcReceiver = '" + detail.CPD + "' and vcSupplierId = '" + vcSupplierId + "' and dTimeFrom<='" + Timet + "' and dTimeTo>='" + Timet + "' ");
                                    if (row.Length == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "没有设定包装单位";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }

                                    //检查供应商信息
                                    DataRow[] row1 = Supplier.Select("vcPart_Id = '" + detail.PartsNo + "' AND vcCPDCompany = '" + detail.CPD + "' AND vcSupplier_id = '" + vcSupplierId + "' AND dTimeFrom <= '" + Timet + "' AND dTimeTo >= '" + Timet + "'");
                                    if (row1.Length == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "没有标签信息";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }


                                DataRow[] drArrayPart = dtRiDuCheck.Select("vcPart_id='" + detail.PartsNo.Trim() + "'"); //获取品番的数量
                                if (drArrayPart.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = detail.PartsNo.Trim();
                                    dataRow["vcMessage"] = "当前日的Soq日度订单平准化未找到该品番!";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                else
                                {
                                    if (Convert.ToInt32(drArrayPart[0]["DayNum"]) == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = detail.PartsNo.Trim();
                                        dataRow["vcMessage"] = "当前日的Soq日度订单平准化品番数量为0!";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(drArrayPart[0]["DayNum"]) != Convert.ToInt32(detail.QTY))
                                        {
                                            DataRow dataRow = dtMessage.NewRow();
                                            dataRow["vcOrder"] = vcOrderNo;
                                            dataRow["vcPartNo"] = detail.PartsNo.Trim();
                                            dataRow["vcMessage"] = "当前日的日度订单与Soq日度订单平准化品番的数量" + Convert.ToInt32(drArrayPart[0]["DayNum"]) + "无法匹配";
                                            dtMessage.Rows.Add(dataRow);
                                            bReault = false;
                                        }
                                    }


                                }
                            }
                        }
                        #endregion

                        #region 生成
                        if (bReault)
                        {
                            #region 获取基础数据

                            string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 8);
                            DateTime DXR = DateTime.Parse(OrderTargetYM.Substring(0, 4) + "-" + OrderTargetYM.Substring(4, 2) + "-" + OrderTargetYM.Substring(6, 2));

                            DataTable dt = fs0403_dataAccess.getCalendar(DXR);
                            int count = fs0403_dataAccess.getCountDay();

                            DataRow[] rowIn = dt.Select("Flag = '0'");
                            DataRow[] rowOut = dt.Select("Flag = '1'");

                            DataTable dtIn = ToDataTable(rowIn);
                            DataTable dtOut = ToDataTable(rowOut);

                            Hashtable hsIN = fs0403_dataAccess.getDay(dtIn, DXR, count);
                            Hashtable hsOut = fs0403_dataAccess.getDay(dtOut, DXR, count);

                            #endregion

                            foreach (Detail detail in order.Details)
                            {
                                #region 获取基础数据

                                string tmp = "";
                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string QTY = Convert.ToInt32(detail.QTY).ToString();
                                string vcDock = "";
                                string vcSupplierId = "";
                                string vcCarType = "";
                                string vcPartId_Replace = "";
                                string inout = "";
                                string packingSpot = "";
                                string vcSupplierPlant = "";//工区
                                string vcHaoJiu = "";
                                string vcOrderPlant = "";
                                string vcSupplierPacking = "";
                                string isTag = "";
                                Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, (DataTable)dockTmp[detail.Date.Substring(0, 6)]);
                                isTag = ObjToString(hashtable["vcSupplierPacking"]);

                                inout = ObjToString(hashtable["vcInOut"]);
                                vcDock = ObjToString(hashtable["vcSufferIn"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcCarType = ObjToString(hashtable["vcCarfamilyCode"]);
                                vcPartId_Replace = ObjToString(hashtable["vcPartId_Replace"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);
                                vcHaoJiu = ObjToString(hashtable["vcHaoJiu"]);
                                vcOrderPlant = ObjToString(hashtable["vcOrderPlant"]);
                                vcSupplierPacking = ObjToString(hashtable["vcSupplierPacking"]);
                                DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");
                                if (packingSpotRow.Length > 0)
                                {
                                    packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                }

                                string timeYM = "";
                                string timeD = "";
                                if (inout == "0")
                                {
                                    timeYM = hsIN[vcOrderPlant].ToString().Substring(0, 6);
                                    timeD = hsIN[vcOrderPlant].ToString().Substring(6, 2);
                                }
                                else if (inout == "1")
                                {
                                    timeYM = hsOut[vcOrderPlant].ToString().Substring(0, 6);
                                    timeD = hsOut[vcOrderPlant].ToString().Substring(6, 2);
                                }


                                string dateTime = detail.Date.Trim();

                                DateTime Time = DateTime.Parse(timeYM.Substring(0, 4) + "-" + timeYM.Substring(4, 2) + "-01");
                                DateTime LastTime = Time.AddMonths(1).AddDays(-1);

                                #endregion

                                #region 新增订单

                                sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily" + timeD + ", vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                sbr.Append(" VALUES( ");
                                sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(timeYM, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(Type, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(dateTime, true) + ",");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(packingSpot, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                sbr.Append("'" + userId + "',");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, true) + ",");
                                sbr.Append("'0','0') \r\n");

                                #endregion

                                #region 插入标签打印
                                if (isTag.Equals("1"))
                                {
                                    int qty = Convert.ToInt32(detail.QTY);
                                    //Add
                                    DataRow[] row = PackUnit.Select("vcPart_id = '" + detail.PartsNo + "' and vcReceiver = '" + detail.CPD + "' and vcSupplierId = '" + vcSupplierId + "' and dTimeFrom<='" + Time + "' and dTimeTo>='" + Time + "' ");
                                    int BZUnit = Convert.ToInt32(row[0]["vcBZUnit"].ToString());

                                    string tmpString1 = "LBLH2";//标签连番、
                                    string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                                    string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                                    //5.1 标签顺番更新==锁定顺番占用
                                    DataTable dtTagSeq = new P00001_DataAccess().setSeqNo(tmpString1, Convert.ToInt32(qty / BZUnit), formatServerTime);
                                    //5.2 标签号生成并更新TOperatorQB（需要一起更新）
                                    string strTagSeqNo = dtTagSeq.Rows[0][0].ToString().PadLeft(5, '0');//标签连番

                                    int SeqNo = Convert.ToInt32(strTagSeqNo) - 1;

                                    DataRow[] row1 = Supplier.Select("vcPart_Id = '" + detail.PartsNo + "' AND vcCPDCompany = '" + detail.CPD + "' AND vcSupplier_id = '" + vcSupplierId + "' AND dTimeFrom <= '" + Time + "' AND dTimeTo >= '" + Time + "'");
                                    string partNameCN = row1[0]["vcPartNameCN"].ToString();
                                    string vcSCSName = row1[0]["vcSCSName"].ToString();
                                    string vcSCSAdress = row1[0]["vcSCSAdress"].ToString();
                                    string vcZXBZNo = row1[0]["vcZXBZNo"].ToString();

                                    string partNameEN = ObjToString(hashtable["vcPartENName"]);

                                    string carName = row1[0]["vcCarTypeName"].ToString();
                                    string vcPart_id1 = detail.PartsNo.Substring(0, 5) + "-" + detail.PartsNo.Substring(5, 5) + "-" + detail.PartsNo.Substring(10, 2);
                                    bool isExist = true;
                                    DownNode node = new DownNode(vcOrderNo, vcSupplierId);
                                    for (int n = 0; n < DownList.Count; n++)
                                    {
                                        if (DownList[n].isExist(node))
                                        {
                                            isExist = false;
                                            break;
                                        }
                                    }

                                    if (isExist)
                                    {
                                        DownList.Add(node);
                                    }

                                    while (qty - BZUnit >= 0)
                                    {
                                        SeqNo = SeqNo + 1;

                                        string sf = yyMMdd + string.Format("{0:d5}", SeqNo);

                                        string qr = url + detail.PartsNo + sf;
                                        string qr1 = url + detail.PartsNo + sf + "B";


                                        byte[] iCodemage = GenerateQRCode(qr);//二维码信息
                                        byte[] iCodemage1 = GenerateQRCode(qr1);//二维码信息

                                        StringBuilder tagsbr = new StringBuilder();
                                        tagsbr.AppendLine("INSERT INTO dbo.tPrintTemp_tag_FS1103(vcPartsnameen,vcPart_id,vcCpdcompany,vcLabel,vcInno,vcPrintcount,vcGetnum,iQrcode,vcPartnamechineese,vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcSupplierId,vcOperatorID,dOperatorTime) VALUES (");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameEN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.CPD, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue("", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(BZUnit, false) + ",");
                                        tagsbr.AppendLine("@iQrcode,");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameCN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSAdress, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcZXBZNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(carName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                        tagsbr.AppendLine("'" + userId + "',GETDATE())");


                                        tagsbr.AppendLine("INSERT INTO dbo.TLabelList(vcPart_id1,vcPrintcount1,vcPartsnameen,vcPart_id,vcCpdcompany,vcLabel,vcInno,vcPrintcount,vcLabel1,vcGetnum,iQrcode,iQrcode1,vcPartnamechineese,vcSuppliername,vcSupplieraddress,vcExecutestandard,vcCartype,vcOperatorID,dOperatorTime,dFirstPrintTime)VALUES(");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcPart_id1, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf + "B", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameEN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.CPD, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue("", false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(detail.PartsNo + sf, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(ChangeBarCode(detail.PartsNo), false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(BZUnit, false) + ",");
                                        tagsbr.AppendLine("@iQrcode1,");
                                        tagsbr.AppendLine("@iQrcode2,");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(partNameCN, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSName, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcSCSAdress, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(vcZXBZNo, false) + ",");
                                        tagsbr.AppendLine(ComFunction.getSqlValue(carName, false) + ",");
                                        tagsbr.AppendLine("'" + userId + "',GETDATE(),GETDATE())");

                                        tagsbr.AppendLine("INSERT INTO dbo.TLabelList_KEY(vcLabelCode,vcHostIp,vcOperatorID,dOperatorTime) VALUES('"+ sf + "','127.0.0.1', '"+userId+"', GETDATE() )");

                                        SqlCommand sqlCommandTag = sqlConnection.CreateCommand();
                                        sqlCommandTag.Transaction = sqlTransaction;
                                        sqlCommandTag.CommandType = CommandType.Text;
                                        sqlCommandTag.CommandText = tagsbr.ToString();
                                        sqlCommandTag.Parameters.Add("@iQrcode", SqlDbType.Image);
                                        sqlCommandTag.Parameters.Add("@iQrcode1", SqlDbType.Image);
                                        sqlCommandTag.Parameters.Add("@iQrcode2", SqlDbType.Image);
                                        sqlCommandTag.Parameters["@iQrcode"].Value = iCodemage;
                                        sqlCommandTag.Parameters["@iQrcode1"].Value = iCodemage;
                                        sqlCommandTag.Parameters["@iQrcode2"].Value = iCodemage1;
                                        sqlCommandTag.ExecuteNonQuery();

                                        qty = qty - BZUnit;
                                    }

                                }
                                #endregion
                            }

                            #region 修改订单状态
                            SqlCommand sqlCommandState = sqlConnection.CreateCommand();
                            sqlCommandState.Transaction = sqlTransaction;
                            sqlCommandState.CommandType = CommandType.Text;
                            int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                            StringBuilder sbrState = new StringBuilder();
                            sbrState.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE(),dCreateDate = GETDATE() WHERE iAutoId =" + iAutoId + " ");
                            sqlCommandState.CommandText = sbrState.ToString();
                            sqlCommandState.ExecuteNonQuery();
                            #endregion

                        }

                        #endregion
                    }

                    if (dtMessage.Rows.Count > 0)
                    {
                        sqlConnection.Close();
                        return false;
                    }


                    #region 订单做成
                    if (sbr.Length > 0)
                    {
                        SqlCommand sqlCommandOrder = sqlConnection.CreateCommand();
                        sqlCommandOrder.Transaction = sqlTransaction;
                        sqlCommandOrder.CommandType = CommandType.Text;
                        sqlCommandOrder.CommandText = sbr.ToString();
                        sqlCommandOrder.ExecuteNonQuery();
                    }
                    #endregion

                    //#region 记录今日的标签数
                    //SqlCommand sqlCommandSeqNo = sqlConnection.CreateCommand();
                    //sqlCommandSeqNo.Transaction = sqlTransaction;
                    //sqlCommandSeqNo.CommandType = CommandType.Text;
                    //StringBuilder sbrSeqNo = new StringBuilder();
                    //string time = DateTime.Now.ToString("yyyy/MM/dd");
                    //sbrSeqNo.AppendLine("DELETE TSeqNo WHERE DDATE = '" + time + "' AND FLAG = 'OrdH2'");
                    //sbrSeqNo.AppendLine("INSERT INTO dbo.TSeqNo(FLAG,DDATE,SEQNO) VALUES('OrdH2','" + time + "'," + SeqNo + ")");
                    //sqlCommandSeqNo.CommandText = sbrSeqNo.ToString();
                    //sqlCommandSeqNo.ExecuteNonQuery();
                    //#endregion

                }

                #region 提交事务

                sqlTransaction.Commit();
                sqlConnection.Close();
                sqlTransaction = null;
                sqlConnection = null;
                #endregion

                packEmail.getEmail(email, SupplierEmail(), getEmail("C057"), getEmail("C058"));
                packEmail.sendMail();

                return true;
            }
            catch (Exception ex)
            {
                if (sqlTransaction != null && sqlConnection!=null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                throw ex;
            }
        }
        #endregion

        #region 月度订单传入内示

        public string MonthToNeishi(DateTime time, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                //N+1
                DateTime N = time;
                DateTime N1 = time.AddMonths(1);
                DateTime N2 = time.AddMonths(2);
                DateTime CL = time.AddMonths(-1);

                sbr.AppendLine("DECLARE @CLYM VARCHAR(6)");
                sbr.AppendLine("DECLARE @TargetYM VARCHAR(6)");
                sbr.AppendLine("DECLARE @TargetYM1 VARCHAR(6)");
                sbr.AppendLine("DECLARE @TargetYM2 VARCHAR(6)");
                sbr.AppendLine("");
                sbr.AppendLine("SET @CLYM = '" + CL.ToString("yyyyMM") + "'");
                sbr.AppendLine("SET @TargetYM = '" + N.ToString("yyyyMM") + "'");
                sbr.AppendLine("SET @TargetYM1 = '" + N1.ToString("yyyyMM") + "'");
                sbr.AppendLine("SET @TargetYM2 = '" + N2.ToString("yyyyMM") + "'");
                sbr.AppendLine("---N+1");
                sbr.AppendLine("");
                sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM1");
                sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY,dInputDate,TARGETMONTH,PARTSNO,CARFAMCODE,INOUTFLAG,SUPPLIERCODE,RESULTQTYTOTAL,varInputUser,vcOperatorID,dOperatorTime,DOCK)");
                sbr.AppendLine("SELECT vcReceiver AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM1 AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,vcSupplier_id AS SUPPLIERCODE,ISNULL(iPartNums,0) AS RESULTQTYTOTAL,'" + userId + "' AS varInputUser,'" + userId + "' AS vcOperatorID,GETDATE() AS dOperatorTime,vcSR AS DOCK FROM  dbo.TSoqReply WHERE vcDXYM = @TargetYM1  AND vcCLYM = @CLYM");
                sbr.AppendLine("");
                sbr.AppendLine("UPDATE a SET a.iSupplierPlant = b.vcSupplierPlant");
                sbr.AppendLine("from TNeiShi a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant FROM (");
                sbr.AppendLine("SELECT vcPartId,vcReceiver,vcSupplierId FROM TSPMaster WHERE @TargetYM BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND ISNULL(vcDelete,0) <> '1' and dFromTime<>dToTime ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant where vcOperatorType='1' AND @TargetYM BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND dFromTime<>dToTime ");
                sbr.AppendLine(") b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId) b");
                sbr.AppendLine("ON a.PARTSNO = b.vcPartId AND a.CPDCOMPANY = b.vcReceiver AND a.SUPPLIERCODE = b.vcSupplierId");
                sbr.AppendLine("WHERE a.iSupplierPlant IS NULL AND a.TARGETMONTH = @TargetYM1 ");
                sbr.AppendLine("");
                sbr.AppendLine("UPDATE a SET a.iSupplierPlant = b.vcSupplierPlant");
                sbr.AppendLine("from TNeiShi a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant FROM (");
                sbr.AppendLine("SELECT vcPartId,vcReceiver,vcSupplierId FROM TSPMaster WHERE @TargetYM1 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND ISNULL(vcDelete,0) <> '1' and dFromTime<>dToTime ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant where vcOperatorType='1' AND @TargetYM1 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND dFromTime<>dToTime ");
                sbr.AppendLine(") b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId) b");
                sbr.AppendLine("ON a.PARTSNO = b.vcPartId AND a.CPDCOMPANY = b.vcReceiver AND a.SUPPLIERCODE = b.vcSupplierId");
                sbr.AppendLine("WHERE a.iSupplierPlant IS NULL AND a.TARGETMONTH = @TargetYM1 ");
                sbr.AppendLine("");
                sbr.AppendLine("");
                sbr.AppendLine("UPDATE a SET a.iSupplierPlant = b.vcSupplierPlant");
                sbr.AppendLine("from TNeiShi a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant FROM (");
                sbr.AppendLine("SELECT vcPartId,vcReceiver,vcSupplierId FROM TSPMaster WHERE @TargetYM2 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND ISNULL(vcDelete,0) <> '1' and dFromTime<>dToTime ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant where vcOperatorType='1' AND @TargetYM2 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND dFromTime<>dToTime ");
                sbr.AppendLine(") b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId) b");
                sbr.AppendLine("ON a.PARTSNO = b.vcPartId AND a.CPDCOMPANY = b.vcReceiver AND a.SUPPLIERCODE = b.vcSupplierId");
                sbr.AppendLine("WHERE a.iSupplierPlant IS NULL AND a.TARGETMONTH = @TargetYM1 ");
                sbr.AppendLine("");
                sbr.AppendLine("--N+2");
                sbr.AppendLine("");
                sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM2");
                sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY,dInputDate,TARGETMONTH,PARTSNO,CARFAMCODE,INOUTFLAG,SUPPLIERCODE,RESULTQTYTOTAL,varInputUser,vcOperatorID,dOperatorTime,DOCK)");
                sbr.AppendLine("SELECT vcReceiver AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM2 AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,vcSupplier_id AS SUPPLIERCODE,ISNULL(iPartNums,0) AS RESULTQTYTOTAL,'" + userId + "' AS varInputUser,'" + userId + "' AS vcOperatorID,GETDATE() AS dOperatorTime,vcSR AS DOCK FROM  dbo.TSoqReply WHERE vcDXYM = @TargetYM2  AND vcCLYM = @CLYM");
                sbr.AppendLine("");
                sbr.AppendLine("UPDATE a SET a.iSupplierPlant = b.vcSupplierPlant");
                sbr.AppendLine("from TNeiShi a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant FROM (");
                sbr.AppendLine("SELECT vcPartId,vcReceiver,vcSupplierId FROM TSPMaster WHERE @TargetYM BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND ISNULL(vcDelete,0) <> '1' and dFromTime<>dToTime ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant where vcOperatorType='1' AND @TargetYM BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND dFromTime<>dToTime ");
                sbr.AppendLine(") b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId) b");
                sbr.AppendLine("ON a.PARTSNO = b.vcPartId AND a.CPDCOMPANY = b.vcReceiver AND a.SUPPLIERCODE = b.vcSupplierId");
                sbr.AppendLine("WHERE a.iSupplierPlant IS NULL AND a.TARGETMONTH = @TargetYM2 ");
                sbr.AppendLine("");
                sbr.AppendLine("UPDATE a SET a.iSupplierPlant = b.vcSupplierPlant");
                sbr.AppendLine("from TNeiShi a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant FROM (");
                sbr.AppendLine("SELECT vcPartId,vcReceiver,vcSupplierId FROM TSPMaster WHERE @TargetYM1 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND ISNULL(vcDelete,0) <> '1' and dFromTime<>dToTime ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant where vcOperatorType='1' AND @TargetYM1 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND dFromTime<>dToTime ");
                sbr.AppendLine(") b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId) b");
                sbr.AppendLine("ON a.PARTSNO = b.vcPartId AND a.CPDCOMPANY = b.vcReceiver AND a.SUPPLIERCODE = b.vcSupplierId");
                sbr.AppendLine("WHERE a.iSupplierPlant IS NULL AND a.TARGETMONTH = @TargetYM2 ");
                sbr.AppendLine("");
                sbr.AppendLine("");
                sbr.AppendLine("UPDATE a SET a.iSupplierPlant = b.vcSupplierPlant");
                sbr.AppendLine("from TNeiShi a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT a.vcPartId,a.vcReceiver,a.vcSupplierId,b.vcSupplierPlant FROM (");
                sbr.AppendLine("SELECT vcPartId,vcReceiver,vcSupplierId FROM TSPMaster WHERE @TargetYM2 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND ISNULL(vcDelete,0) <> '1' and dFromTime<>dToTime ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant where vcOperatorType='1' AND @TargetYM2 BETWEEN convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112) AND dFromTime<>dToTime ");
                sbr.AppendLine(") b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId) b");
                sbr.AppendLine("ON a.PARTSNO = b.vcPartId AND a.CPDCOMPANY = b.vcReceiver AND a.SUPPLIERCODE = b.vcSupplierId");
                sbr.AppendLine("WHERE a.iSupplierPlant IS NULL AND a.TARGETMONTH = @TargetYM2");

                return sbr.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 读取txt

        public Order GetPartFromFile(string path, string orderNo, ref string msg)
        {
            string[] strs = File.ReadAllLines(@path);
            Order order = new Order();

            Head head = new Head();
            List<Detail> details = new List<Detail>();
            Tail tail = new Tail();

            //获取Detail
            for (int i = 0; i < strs.Length; i++)
            {
                string temp = strs[i];
                //判断空行
                if (string.IsNullOrWhiteSpace(temp))
                {
                    continue;
                }
                //获取Head
                if (temp[0] == 'D')
                {
                    Detail detail = new Detail();
                    detail.DataId = temp.Substring(0, 1);
                    detail.CPD = temp.Substring(1, 5);
                    detail.Date = temp.Substring(6, 8);
                    detail.Type = temp.Substring(14, 8);
                    detail.ItemNo = temp.Substring(22, 4);
                    detail.PartsNo = temp.Substring(26, 12).Replace(" ", "0");
                    detail.QTY = temp.Substring(41, 7);
                    detail.Price = temp.Substring(48, 9);
                    details.Add(detail);
                }
                else if (temp[0] == 'H')
                {
                    head.DataId = temp.Substring(0, 1);
                    head.CPD = temp.Substring(1, 5);
                    head.Date = temp.Substring(6, 8);
                    head.No = temp.Substring(14, 8);
                    head.Type = temp.Substring(22, 1);
                    head.SendDate = temp.Substring(28, 8);
                }
                else if (temp[0] == 'T')
                {
                    tail.DataId = temp.Substring(0, 1);
                    tail.CPD = temp.Substring(1, 5);
                    tail.Date = temp.Substring(6, 8);
                    tail.No = temp.Substring(14, 8);
                }
            }

            order.Head = head;
            order.Details = details;
            order.Tail = tail;

            if (order.Head == null)
            {
                msg = "订单" + orderNo + "Head部分有误";
                return null;
            }
            else if (order.Details.Count == 0)
            {
                msg = "订单" + orderNo + "Detail为空";
                return null;
            }
            else if (order.Tail == null)
            {
                msg = "订单" + orderNo + "Tail部分有误";
                return null;
            }

            return order;
        }

        public class Order
        {
            public Order()
            {
                this.Details = new List<Detail>();
            }

            public Head Head;
            public List<Detail> Details;
            public Tail Tail;
        }

        public class Head
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
            public string Type;
            public string Code;
            public string SendDate;
        }

        public class Detail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string Type;
            public string ItemNo;
            public string PartsNo;
            public string QTY;
            public string Price;

        }

        public class Tail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
        }


        #endregion

        #region 获取soq数量
        public DataTable getSoqDt(List<string> TargetYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                foreach (string s in TargetYM)
                {
                    DateTime t2;
                    //DateTime.TryParseExact(s + "01", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out t2);
                    t2 = DateTime.Parse(s.Substring(0, 4) + "-" + s.Substring(4, 2) + "-" + "01");
                    string CLYM = t2.AddMonths(-1).ToString("yyyyMM");
                    if (sql.Length > 0)
                    {
                        sql.Append(" OR ");
                    }
                    sql.Append("(vcDXYM = '" + s + "' AND vcCLYM = '" + CLYM + "')");
                }

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcPart_id,vcInOutFlag,vcDXYM,vcFZGC,vcCarType,vcMakingOrderType,iPartNums,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31 FROM TSOQReply");
                sbr.AppendLine(" WHERE 1=1 AND  ISNULL(iPartNums,0) <> 0 AND ");
                sbr.AppendLine(sql.ToString());
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable getSoqNumMonth(string partId, string TargetYM, string vcOrderingMethod, DataTable dt)
        {
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //品番，内外，订货方式，车型
                if (partId.Trim().Equals(dt.Rows[i]["vcPart_id"].ToString().Trim()) && TargetYM.Equals(dt.Rows[i]["vcDXYM"].ToString().Trim()))
                {
                    hashtable.Add("Total", dt.Rows[i]["iPartNums"].ToString());
                    hashtable.Add("iD1", dt.Rows[i]["iD1"].ToString());
                    hashtable.Add("iD2", dt.Rows[i]["iD2"].ToString());
                    hashtable.Add("iD3", dt.Rows[i]["iD3"].ToString());
                    hashtable.Add("iD4", dt.Rows[i]["iD4"].ToString());
                    hashtable.Add("iD5", dt.Rows[i]["iD5"].ToString());
                    hashtable.Add("iD6", dt.Rows[i]["iD6"].ToString());
                    hashtable.Add("iD7", dt.Rows[i]["iD7"].ToString());
                    hashtable.Add("iD8", dt.Rows[i]["iD8"].ToString());
                    hashtable.Add("iD9", dt.Rows[i]["iD9"].ToString());
                    hashtable.Add("iD10", dt.Rows[i]["iD10"].ToString());
                    hashtable.Add("iD11", dt.Rows[i]["iD11"].ToString());
                    hashtable.Add("iD12", dt.Rows[i]["iD12"].ToString());
                    hashtable.Add("iD13", dt.Rows[i]["iD13"].ToString());
                    hashtable.Add("iD14", dt.Rows[i]["iD14"].ToString());
                    hashtable.Add("iD15", dt.Rows[i]["iD15"].ToString());
                    hashtable.Add("iD16", dt.Rows[i]["iD16"].ToString());
                    hashtable.Add("iD17", dt.Rows[i]["iD17"].ToString());
                    hashtable.Add("iD18", dt.Rows[i]["iD18"].ToString());
                    hashtable.Add("iD19", dt.Rows[i]["iD19"].ToString());
                    hashtable.Add("iD20", dt.Rows[i]["iD20"].ToString());
                    hashtable.Add("iD21", dt.Rows[i]["iD21"].ToString());
                    hashtable.Add("iD22", dt.Rows[i]["iD22"].ToString());
                    hashtable.Add("iD23", dt.Rows[i]["iD23"].ToString());
                    hashtable.Add("iD24", dt.Rows[i]["iD24"].ToString());
                    hashtable.Add("iD25", dt.Rows[i]["iD25"].ToString());
                    hashtable.Add("iD26", dt.Rows[i]["iD26"].ToString());
                    hashtable.Add("iD27", dt.Rows[i]["iD27"].ToString());
                    hashtable.Add("iD28", dt.Rows[i]["iD28"].ToString());
                    hashtable.Add("iD29", dt.Rows[i]["iD29"].ToString());
                    hashtable.Add("iD30", dt.Rows[i]["iD30"].ToString());
                    hashtable.Add("iD31", dt.Rows[i]["iD31"].ToString());
                    break;
                }
            }
            return hashtable;

        }

        public bool CheckTotalNumEqual(string TargetYM, string partId, string vcOrderingMethod, DataTable dt, string total)
        {
            try
            {
                bool flag = false;

                int totalCheck = Convert.ToInt32(getSoqNumMonth(partId, TargetYM, vcOrderingMethod, dt)["Total"].ToString());

                int totalC = Convert.ToInt32(total);

                if (totalC == totalCheck)
                {
                    flag = true;
                }

                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 获取Dock

        public Hashtable getDock(string PartID, string receiver, string vcPackingPlant, DataTable dt)
        {
            try
            {
                Hashtable hashtable = new Hashtable();

                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (PartID.Trim().Equals(dt.Rows[i]["vcPartId"].ToString().Trim())
                            && receiver.Trim().Equals(dt.Rows[i]["vcReceiver"].ToString().Trim())
                            && vcPackingPlant.Trim().Equals(dt.Rows[i]["vcPackingPlant"].ToString().Trim()))
                        {
                            hashtable.Add("vcPartId_Replace", dt.Rows[i]["vcPartId_Replace"].ToString());
                            hashtable.Add("vcSupplierId", dt.Rows[i]["vcSupplierId"].ToString());
                            hashtable.Add("vcCarfamilyCode", dt.Rows[i]["vcCarfamilyCode"].ToString());
                            hashtable.Add("vcSufferIn", dt.Rows[i]["vcSufferIn"].ToString());
                            hashtable.Add("vcInOut", dt.Rows[i]["vcInOut"].ToString());
                            hashtable.Add("vcOrderingMethod", dt.Rows[i]["vcOrderingMethod"].ToString());
                            hashtable.Add("vcHaoJiu", dt.Rows[i]["vcHaoJiu"].ToString());
                            hashtable.Add("vcSupplierPlant", dt.Rows[i]["vcSupplierPlant"].ToString());
                            hashtable.Add("vcOrderPlant", dt.Rows[i]["vcOrderPlant"].ToString());
                            hashtable.Add("vcSupplierPacking", dt.Rows[i]["vcSupplierPacking"].ToString());
                            hashtable.Add("vcPartNameCn", dt.Rows[i]["vcPartNameCn"].ToString());
                            hashtable.Add("vcPartENName", dt.Rows[i]["vcPartENName"].ToString());
                            hashtable.Add("vcSupplierPlace", dt.Rows[i]["vcSupplierPlace"].ToString());
                            break;
                        }
                    }

                }
                return hashtable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable getDock(string PartID, string vcPackingPlant, DataTable dt)
        {
            try
            {
                Hashtable hashtable = new Hashtable();

                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (PartID.Trim().Equals(dt.Rows[i]["vcPartId"].ToString().Trim())
                            && vcPackingPlant.Trim().Equals(dt.Rows[i]["vcPackingPlant"].ToString().Trim()))
                        {
                            hashtable.Add("vcPartId_Replace", dt.Rows[i]["vcPartId_Replace"].ToString());
                            hashtable.Add("vcSupplierId", dt.Rows[i]["vcSupplierId"].ToString());
                            hashtable.Add("vcCarfamilyCode", dt.Rows[i]["vcCarfamilyCode"].ToString());
                            hashtable.Add("vcSufferIn", dt.Rows[i]["vcSufferIn"].ToString());
                            hashtable.Add("vcInOut", dt.Rows[i]["vcInOut"].ToString());
                            hashtable.Add("vcOrderingMethod", dt.Rows[i]["vcOrderingMethod"].ToString());
                            hashtable.Add("vcHaoJiu", dt.Rows[i]["vcHaoJiu"].ToString());
                            hashtable.Add("vcSupplierPlant", dt.Rows[i]["vcSupplierPlant"].ToString());
                            hashtable.Add("vcOrderPlant", dt.Rows[i]["vcOrderPlant"].ToString());
                            hashtable.Add("vcSupplierPacking", dt.Rows[i]["vcSupplierPacking"].ToString());
                            hashtable.Add("vcPartNameCn", dt.Rows[i]["vcPartNameCn"].ToString());
                            hashtable.Add("vcPartENName", dt.Rows[i]["vcPartENName"].ToString());
                            hashtable.Add("vcReceiver", dt.Rows[i]["vcReceiver"].ToString());

                            break;
                        }
                    }
                }
                return hashtable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getDockTable(string TargetYM)
        {
            try
            {
                DateTime timeFrom = DateTime.Parse(TargetYM.Substring(0, 4) + "-" + TargetYM.Substring(4, 2) + "-01");
                DateTime timeTo = timeFrom;
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcOESP,e.iPackingQty,a.vcSupplierPlace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod,c.vcSupplierPlant,vcHaoJiu,d.vcOrderPlant,vcSupplierPacking,vcPartNameCn,vcPartENName,vcSupplierPacking,a.vcReceiver FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT vcSupplierId,vcOESP,vcSupplierPlace,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcHaoJiu,vcSupplierPacking,vcPartNameCn,vcPartENName FROM TSPMaster WHERE ");
                sbr.AppendLine("	dFromTime <= '" + timeFrom + "' AND dToTime >='" + timeTo + "' and isnull(vcDelete, '') <> '1' ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "' AND vcOperatorType = '1' ");
                sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcSupplierPlant,vcSupplierId,vcPartId,vcReceiver,vcPackingPlant FROM TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "'");
                sbr.AppendLine(") c ON a.vcSupplierId = b.vcSupplierId AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcPackingPlant = c.vcPackingPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcValue1 as vcSupplierId,vcValue2 as vcSupplierPlant,vcValue3 as dFromTime,vcValue4 as dToTime,vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0' AND vcValue3<=CONVERT(VARCHAR(10),CONVERT(datetime,'" + timeFrom + "'),23) AND vcValue4>=CONVERT(VARCHAR(10),CONVERT(datetime,'" + timeTo + "'),23)");
                sbr.AppendLine(") d ON a.vcSupplierId = d.vcSupplierId AND c.vcSupplierPlant = d.vcSupplierPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select  vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime, iPackingQty,  vcOperatorType, dOperatorTime from TSPMaster_Box WHERE vcOperatorType = '1' AND dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "'");
                sbr.AppendLine(") e ON a.vcSupplierId = e.vcSupplierId AND a.vcPartId = e.vcPartId AND a.vcReceiver = e.vcReceiver AND a.vcPackingPlant = e.vcPackingPlant");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取路径
        public string getPath(string orderNo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcFilePath FROM TOrderUploadManage WHERE vcFileOrderNo = '" + orderNo + "'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["vcFilePath"].ToString();
                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取包装厂

        public DataTable getPackSpot()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcPart_id,vcReceiver,vcSupplierId,vcPackingPlant,vcBZPlant FROM dbo.TPackageMaster");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取订单类型
        public DataTable getType()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT vcOrderDifferentiation AS vcName,vcOrderInitials AS vcValue FROM TOrderDifferentiation");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public static string ObjToString(Object obj)
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

        public string getOrderType(string type)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcOrderInitials FROM TOrderDifferentiation WHERE vcOrderDifferentiation = '" + type + "'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                    return dt.Rows[0]["vcOrderInitials"].ToString();
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrderNo()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcLastOrderNo,vcNewOrderNo FROM TUploadOrderRelation");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getNQ()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcOrderNo,vcPart_id,CONVERT(VARCHAR(8),dOutPutDate,112) AS dOutPutDate,CONVERT(VARCHAR(8),dDeliveryDate,112) AS dDeliveryDate,CONVERT(VARCHAR(6),dOutPutDate,112) AS TargetYM,iDuiYingQuantity FROM dbo.VI_UrgentOrder_OperHistory WHERE dOutPutDate IS NOT null ORDER BY dOutPutDate");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getOrderNo(DataTable dt, string orderNo)
        {
            try
            {
                DataRow[] row = dt.Select("vcNewOrderNo = '" + orderNo + "'");
                if (row.Length > 0)
                    return row[0]["vcLastOrderNo"].ToString();
                return orderNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getTypeMethod(string vcType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcValue FROM TCode WHERE iAutoId IN(");
                sbr.AppendLine("SELECT vcOrderGoodsId FROM TOrderGoodsAndDifferentiation WHERE vcOrderDifferentiationId in");
                sbr.AppendLine("(SELECT iAutoId FROM TOrderDifferentiation WHERE vcOrderInitials = '" + vcType + "')) ");

                string res = "";

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            res += ",";
                        }
                        res += "'" + dt.Rows[i]["vcValue"].ToString() + "'";
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void cancelOrder(List<Dictionary<string, Object>> list, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '2',vcOperatorID = '" + userId + "',dOperatorTime = GETDATE() WHERE iAutoId = " + list[i]["iAutoId"].ToString() + " AND vcOrderState <> 1");
                }
                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0)
                return null;
            DataTable tmp = rows[0].Table.Clone();
            foreach (DataRow dataRow in rows)
            {
                tmp.Rows.Add(dataRow.ItemArray);
            }

            return tmp;
        }

        public DataTable orderList()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT a.vcFileOrderNo AS vcName,a.vcFileOrderNo AS vcValue FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT DISTINCT vcFileOrderNo FROM dbo.TOrderUploadManage WHERE vcOrderShowFlag='1'");
                sbr.AppendLine(") a");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int getNum()
        {
            try
            {
                string time = DateTime.Now.ToString("yyyy/MM/dd");
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT SEQNO FROM dbo.TSeqNo WHERE DDATE = '" + time + "' AND FLAG = 'OrdH2'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["SEQNO"].ToString());
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getPackUnit()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT vcPart_id,vcReceiver,vcSupplierId,vcBZUnit,dTimeFrom,dTimeTo FROM TPackageMaster");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getSupplier()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT vcPart_Id,vcCPDCompany,vcSupplier_id,vcCarTypeName,vcPartNameCN,vcZXBZNo,vcSCSName,vcSCSAdress,dTimeFrom,dTimeTo,vcSYTCode,vcZXBZNo FROM dbo.TtagMaster WHERE dTimeFrom<>dTimeTo");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getEDPart()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcPart_id,vcSHF,vcSupplier_id,dTimeFrom,dTimeTo FROM dbo.TEDTZPartsNoMaster");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 生成QRCODE二维码
        public byte[] GenerateQRCode(string content)
        {
            var generator = new QRCodeGenerator();

            var codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);

            var bitmapImg = qrcode.GetGraphic(10, Color.Black, Color.White, false);

            using MemoryStream stream = new MemoryStream();
            bitmapImg.Save(stream, ImageFormat.Jpeg);
            return stream.GetBuffer();
        }
        #endregion

        class EDNode
        {
            public string vcPackingFactory;
            public string OrderTargetYM;
            public string vcDock;
            public string CPD;
            public string Type;
            public string vcOrderNo;
            public string vcSeqno;
            public string vcCarType;
            public string dOrderDate;
            public string vcPart_id;
            public string inout;
            public string packingSpot;
            public string vcSupplierId;
            public string vcSupplierPlant;
            public int id1;
            public int id2;
            public int id3;
            public int id4;
            public int id5;
            public int id6;
            public int id7;
            public int id8;
            public int id9;
            public int id10;
            public int id11;
            public int id12;
            public int id13;
            public int id14;
            public int id15;
            public int id16;
            public int id17;
            public int id18;
            public int id19;
            public int id20;
            public int id21;
            public int id22;
            public int id23;
            public int id24;
            public int id25;
            public int id26;
            public int id27;
            public int id28;
            public int id29;
            public int id30;
            public int id31;

            public int iPartNums()
            {
                return id1 + id2 + id3 + id4 + id5 + id6 + id7 + id8 + id9 + id10 + id11 + id12 + id13 + id14 + id15 +
                       id16 + id17 + id18 + id19 + id20 + id21 + id22 + id23 + id24 + id25 + id26 + id27 + id28 + id29 +
                       id30 + id31;
            }

            public EDNode()
            {
                id1 = 0;
                id2 = 0;
                id3 = 0;
                id4 = 0;
                id5 = 0;
                id6 = 0;
                id7 = 0;
                id8 = 0;
                id9 = 0;
                id10 = 0;
                id11 = 0;
                id12 = 0;
                id13 = 0;
                id14 = 0;
                id15 = 0;
                id16 = 0;
                id17 = 0;
                id18 = 0;
                id19 = 0;
                id20 = 0;
                id21 = 0;
                id22 = 0;
                id23 = 0;
                id24 = 0;
                id25 = 0;
                id26 = 0;
                id27 = 0;
                id28 = 0;
                id29 = 0;
                id30 = 0;
                id31 = 0;
            }

            public bool isExist(EDNode node)
            {
                if (this.vcPart_id != node.vcPart_id)
                    return false;
                if (this.CPD != node.CPD)
                    return false;
                if (this.vcSupplierId != node.vcSupplierId)
                    return false;
                return true;
            }

            public void Add(EDNode node)
            {
                this.id1 = this.id1 + node.id1;
                this.id2 = this.id2 + node.id2;
                this.id3 = this.id3 + node.id3;
                this.id4 = this.id4 + node.id4;
                this.id5 = this.id5 + node.id5;
                this.id6 = this.id6 + node.id6;
                this.id7 = this.id7 + node.id7;
                this.id8 = this.id8 + node.id8;
                this.id9 = this.id9 + node.id9;
                this.id10 = this.id10 + node.id10;
                this.id11 = this.id11 + node.id11;
                this.id12 = this.id12 + node.id12;
                this.id13 = this.id13 + node.id13;
                this.id14 = this.id14 + node.id14;
                this.id15 = this.id15 + node.id15;
                this.id16 = this.id16 + node.id16;
                this.id17 = this.id17 + node.id17;
                this.id18 = this.id18 + node.id18;
                this.id19 = this.id19 + node.id19;
                this.id20 = this.id20 + node.id20;
                this.id21 = this.id21 + node.id21;
                this.id22 = this.id22 + node.id22;
                this.id23 = this.id23 + node.id23;
                this.id24 = this.id24 + node.id24;
                this.id25 = this.id25 + node.id25;
                this.id26 = this.id26 + node.id26;
                this.id27 = this.id27 + node.id27;
                this.id28 = this.id28 + node.id28;
                this.id29 = this.id29 + node.id29;
                this.id30 = this.id30 + node.id30;
                this.id31 = this.id31 + node.id31;
            }

            public void setValue(int index, int num)
            {
                switch (index)
                {
                    case 1:
                        this.id1 = num;
                        break;
                    case 2:
                        this.id2 = num;
                        break;
                    case 3:
                        this.id3 = num;
                        break;
                    case 4:
                        this.id4 = num;
                        break;
                    case 5:
                        this.id5 = num;
                        break;
                    case 6:
                        this.id6 = num;
                        break;
                    case 7:
                        this.id7 = num;
                        break;
                    case 8:
                        this.id8 = num;
                        break;
                    case 9:
                        this.id9 = num;
                        break;
                    case 10:
                        this.id10 = num;
                        break;
                    case 11:
                        this.id11 = num;
                        break;
                    case 12:
                        this.id12 = num;
                        break;
                    case 13:
                        this.id13 = num;
                        break;
                    case 14:
                        this.id14 = num;
                        break;
                    case 15:
                        this.id15 = num;
                        break;
                    case 16:
                        this.id16 = num;
                        break;
                    case 17:
                        this.id17 = num;
                        break;
                    case 18:
                        this.id18 = num;
                        break;
                    case 19:
                        this.id19 = num;
                        break;
                    case 20:
                        this.id20 = num;
                        break;
                    case 21:
                        this.id21 = num;
                        break;
                    case 22:
                        this.id22 = num;
                        break;
                    case 23:
                        this.id23 = num;
                        break;
                    case 24:
                        this.id24 = num;
                        break;
                    case 25:
                        this.id25 = num;
                        break;
                    case 26:
                        this.id26 = num;
                        break;
                    case 27:
                        this.id27 = num;
                        break;
                    case 28:
                        this.id28 = num;
                        break;
                    case 29:
                        this.id29 = num;
                        break;
                    case 30:
                        this.id30 = num;
                        break;
                    case 31:
                        this.id31 = num;
                        break;
                }
            }

        }
        public class DownNode
        {
            public string orderNo;
            public string supplier;

            public DownNode(string orderNo, string supplier)
            {
                this.orderNo = orderNo;
                this.supplier = supplier;
            }

            public bool isExist(DownNode node)
            {
                if (node.orderNo != this.orderNo)
                    return false;
                if (node.supplier != this.supplier)
                    return false;
                return true;
            }

        }

        public DataTable getPackInfo()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcPartsNo,a.vcPackNo,CONVERT(VARCHAR(8),dFrom,112) AS dFrom,CONVERT(VARCHAR(8),dTo,112) AS dTo,CONVERT(VARCHAR(8),dPackFrom,112) AS dPackFrom,CONVERT(VARCHAR(8),dPackTo,112) AS dPackTo,b.vcSupplierCode,iBiYao FROM ");
                sbr.AppendLine("TPackItem a");
                sbr.AppendLine("LEFT JOIN(");
                sbr.AppendLine("SELECT vcPackNo,dPackFrom,dPackTo,vcSupplierCode FROM TPackBase");
                sbr.AppendLine(") b ON a.vcPackNo = b.vcPackNo");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 包材类

        public class PackEmail
        {
            List<PackSuccess> success;
            List<PackFail> fail;
            private string rootpath;
            string Address;
            public DataTable supplierMail;
            public List<mailClass> copyEmail;
            public List<mailClass> FailEmail;
            public PackEmail(string rootPath)
            {
                this.success = new List<PackSuccess>();
                this.rootpath = rootPath;
                this.fail = new List<PackFail>();
            }

            public void getEmail(string sendMail, DataTable dt, List<mailClass> copy, List<mailClass> fail)
            {
                this.Address = sendMail;
                this.supplierMail = dt;
                this.copyEmail = copy;
                this.FailEmail = fail;
            }

            public void addSuccess(PackSuccess pack)
            {
                bool exist = false;
                for (int i = 0; i < success.Count; i++)
                {
                    if (success[i].Year == pack.Year && success[i].Month == pack.Month &&
                        success[i].Supplier == pack.Supplier)
                    {
                        success[i].AddPack(pack.list[0]);
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    this.success.Add(pack);
                }
            }
            public void addFail(PackFail pack)
            {
                bool exist = false;
                for (int i = 0; i < fail.Count; i++)
                {
                    if (fail[i].Year == pack.Year && fail[i].Month == pack.Month)
                    {
                        fail[i].AddPack(pack.list[0]);
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    this.fail.Add(pack);
                }
            }

            public void sendMail()
            {
                if (success.Count > 0)
                {
                    foreach (PackSuccess pack in success)
                    {
                        DataTable dt = pack.getDt();
                        string path = DataTableToExcel(dt, rootpath, pack.Supplier + "_", "包材构成");
                        DataRow[] rows = this.supplierMail.Select("vcValue1 = '" + pack.Supplier + "'");
                        if (rows.Length > 0)
                        {
                            DataTable maildt = rows.CopyToDataTable();
                            StringBuilder subject = new StringBuilder();
                            subject.Append("包装材紧急订单");
                            StringBuilder body = new StringBuilder();
                            body.AppendLine("<p>包材厂家" + pack.Supplier + " 担当</p><p>你好：</p><p><br></p><p>拜托按照附件提示包材及日期进行包材生产准备</p>");

                            bool flag = SendMail(this.Address, subject.ToString(), body.ToString(), path, maildt, this.copyEmail, this.FailEmail, "S");
                            if (!flag)
                            {
                                subject.Length = 0;
                                body.Length = 0;
                                subject.Append("紧急订单邮件发送失败联络");
                                body.AppendLine("附件紧急订单邮件发送失败。");
                                SendMail(this.Address, subject.ToString(), body.ToString(), path, maildt, this.copyEmail, this.FailEmail, "F");
                            }
                        }
                    }
                }
                if (fail.Count > 0)
                {
                    foreach (PackFail pack in fail)
                    {
                        DataTable dt = pack.getDt();
                        string path = DataTableToExcel(dt, rootpath, "", "包材构成失败");
                        StringBuilder subject = new StringBuilder();
                        StringBuilder body = new StringBuilder();
                        subject.Append("未维护包材构成品番明细");
                        body.AppendLine("未维护包材构成品番明细请见附件。");
                        SendMail(this.Address, subject.ToString(), body.ToString(), path, null, this.copyEmail, this.FailEmail, "F");
                    }
                }
            }
        }

        public class PackSuccess
        {
            public string Year;
            public string Month;
            public string Supplier;
            public int length;

            public List<PackItem> list;

            public PackSuccess(string Year, string Month, string Supplier)
            {
                this.Year = Year;
                this.Month = Month;
                this.Supplier = Supplier;
                this.length = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month));
                this.list = new List<PackItem>();
            }

            public void AddPack(PackItem newPack)
            {
                bool isExist = false;
                for (int i = 0; i < list.Count; i++)
                {
                    isExist = list[i].isExist(newPack);
                    if (isExist)
                    {
                        list[i].Add(newPack);
                        break;
                    }
                }

                if (!isExist)
                {
                    list.Add(newPack);
                }
            }

            public DataTable getDt()
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("PackNo");
                for (int i = 1; i <= length; i++)
                {
                    dt.Columns.Add("day" + i);
                }

                DataRow title = dt.NewRow();
                title["PackNo"] = "包材品番";
                for (int i = 1; i <= length; i++)
                {
                    title["day" + i] = Year + "/" + Month + "/" + i;
                }

                dt.Rows.Add(title);

                for (int i = 0; i < list.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["PackNo"] = list[i].PackNo;
                    for (int j = 1; j <= length; j++)
                        row["day" + j] = list[i].nums[j];
                    dt.Rows.Add(row);
                }

                return dt;
            }
        }

        public class PackItem
        {
            public string PackNo;
            public double[] nums = new double[32];

            public PackItem(string Year, string Month, string PackNo, int day, double num)
            {
                this.PackNo = PackNo;
                nums[day] = num;
            }

            public void Add(PackItem newPack)
            {
                this.nums[1] = this.nums[1] + newPack.nums[1];
                this.nums[2] = this.nums[2] + newPack.nums[2];
                this.nums[3] = this.nums[3] + newPack.nums[3];
                this.nums[4] = this.nums[4] + newPack.nums[4];
                this.nums[5] = this.nums[5] + newPack.nums[5];
                this.nums[6] = this.nums[6] + newPack.nums[6];
                this.nums[7] = this.nums[7] + newPack.nums[7];
                this.nums[8] = this.nums[8] + newPack.nums[8];
                this.nums[9] = this.nums[9] + newPack.nums[9];
                this.nums[10] = this.nums[10] + newPack.nums[10];
                this.nums[11] = this.nums[11] + newPack.nums[11];
                this.nums[12] = this.nums[12] + newPack.nums[12];
                this.nums[13] = this.nums[13] + newPack.nums[13];
                this.nums[14] = this.nums[14] + newPack.nums[14];
                this.nums[15] = this.nums[15] + newPack.nums[15];
                this.nums[16] = this.nums[16] + newPack.nums[16];
                this.nums[17] = this.nums[17] + newPack.nums[17];
                this.nums[18] = this.nums[18] + newPack.nums[18];
                this.nums[19] = this.nums[19] + newPack.nums[19];
                this.nums[20] = this.nums[20] + newPack.nums[20];
                this.nums[21] = this.nums[21] + newPack.nums[21];
                this.nums[22] = this.nums[22] + newPack.nums[22];
                this.nums[23] = this.nums[23] + newPack.nums[23];
                this.nums[24] = this.nums[24] + newPack.nums[24];
                this.nums[25] = this.nums[25] + newPack.nums[25];
                this.nums[26] = this.nums[26] + newPack.nums[26];
                this.nums[27] = this.nums[27] + newPack.nums[27];
                this.nums[28] = this.nums[28] + newPack.nums[28];
                this.nums[29] = this.nums[29] + newPack.nums[29];
                this.nums[30] = this.nums[30] + newPack.nums[30];
                this.nums[31] = this.nums[31] + newPack.nums[31];
            }

            public bool isExist(PackItem newPack)
            {
                return this.PackNo.Equals(newPack.PackNo);
            }

        }

        public class PackFail
        {
            public string Year;
            public string Month;
            public int length;

            public List<PackItem> list;

            public PackFail(string Year, string Month)
            {
                this.Year = Year;
                this.Month = Month;
                this.length = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month));
                this.list = new List<PackItem>();
            }

            public void AddPack(PackItem newPack)
            {
                bool isExist = false;
                for (int i = 0; i < list.Count; i++)
                {
                    isExist = list[i].isExist(newPack);
                    if (isExist)
                    {
                        list[i].Add(newPack);
                        break;
                    }
                }

                if (!isExist)
                {
                    list.Add(newPack);
                }
            }

            public DataTable getDt()
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("PackNo");
                for (int i = 1; i <= length; i++)
                {
                    dt.Columns.Add("day" + i);
                }

                DataRow title = dt.NewRow();
                title["PackNo"] = "部品品番";
                for (int i = 1; i <= length; i++)
                {
                    title["day" + i] = Year + "/" + Month + "/" + i;
                }

                dt.Rows.Add(title);

                for (int i = 0; i < list.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["PackNo"] = list[i].PackNo;
                    for (int j = 1; j <= length; j++)
                        row["day" + j] = list[i].nums[j];
                    dt.Rows.Add(row);
                }

                return dt;
            }
        }

        public DataTable SupplierEmail()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcValue1,vcValue2,vcValue3,vcValue4 FROM TOutCode WHERE vcCodeId = 'C056' AND vcIsColum = '0'");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<mailClass> getEmail(string codeId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcValue1,vcValue2 FROM TOutCode WHERE vcCodeId = '" + codeId + "' AND vcIsColum = '0'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                List<mailClass> list = new List<mailClass>();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        list.Add(new mailClass(ObjToString(dt.Rows[i]["vcValue2"]), ObjToString(dt.Rows[i]["vcValue1"])));
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public class mailClass
        {
            public string address;
            public string display;

            public mailClass(string address, string display)
            {
                this.address = address;
                this.display = display;
            }
        }

        #region 生成Excel

        public static string DataTableToExcel(DataTable dt, string rootPath, string supplierId, string strFunctionName)
        {
            bool result = false;
            FileStream fs = null;
            int size = 1048576 - 1;

            string strFileName = supplierId + strFunctionName + "导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除

            string path = fileSavePath + strFileName;

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    int page = dt.Rows.Count / size;
                    IWorkbook workbook = new XSSFWorkbook();
                    for (int i = 0; i < page + 1; i++)
                    {
                        string sheetname = "Sheet" + (i + 1).ToString();
                        ISheet sheet = workbook.CreateSheet(sheetname);
                        int rowCount = dt.Rows.Count - i * size > size ? size : dt.Rows.Count - i * size;//行数  
                        int columnCount = dt.Columns.Count;//列数  
                        List<ICellStyle> styles = new List<ICellStyle>();
                        IRow row = sheet.CreateRow(0);
                        ICell cell;
                        //设置每列单元格属性
                        for (int h = 0; h < columnCount; h++)
                        {
                            Type type = dt.Columns[h].DataType;
                            ICellStyle dateStyle = workbook.CreateCellStyle();
                            IDataFormat dataFormat = workbook.CreateDataFormat();
                            dateStyle.DataFormat = dataFormat.GetFormat("General");
                            styles.Add(dateStyle);
                        }
                        //设置每行每列的单元格,  
                        for (int j = 0; j < rowCount; j++)
                        {
                            row = sheet.CreateRow(j);
                            for (int l = 0; l < columnCount; l++)
                            {
                                Type type = dt.Columns[l].DataType;
                                cell = row.CreateCell(l);
                                cell.CellStyle = styles[l];
                                if (type == Type.GetType("System.Decimal"))
                                {
                                    if (dt.Rows[j][l].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToDouble(dt.Rows[j + i * size][l].ToString()));
                                }
                                else if (type == Type.GetType("System.Int32"))
                                {
                                    if (dt.Rows[j][l].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToInt32(dt.Rows[j + i * size][l].ToString()));
                                }
                                else if (type == Type.GetType("System.Int16"))
                                {
                                    if (dt.Rows[j][l].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToInt16(dt.Rows[j + i * size][l].ToString()));
                                }
                                else if (type == Type.GetType("System.Int64"))
                                {
                                    if (dt.Rows[j][l].ToString().Trim() != "")
                                        cell.SetCellValue(Convert.ToInt64(dt.Rows[j + i * size][l].ToString()));
                                }
                                else
                                {
                                    cell.SetCellValue(dt.Rows[j + i * size][l].ToString());
                                }
                            }
                        }
                        using (fs = File.OpenWrite(path))
                        {
                            workbook.Write(fs);//向打开的这个xls文件中写入数据  
                        }
                    }
                    result = true;
                }


                return path;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                ComFunction.ConsoleWriteLine(ex.Message);
                return "";
            }
        }

        #endregion

        #region 发送邮件

        public static bool SendMail(string userEmail, string Subject, string Body, string path, DataTable supplierdt, List<mailClass> copy, List<mailClass> Fail, string flag)
        {
            try
            {
                string strSubject = Subject;

                DataTable cCDt = new DataTable();
                DataTable receiverDt = new DataTable();
                receiverDt.Columns.Add("address");
                receiverDt.Columns.Add("displayName");
                cCDt.Columns.Add("address");
                cCDt.Columns.Add("displayName");
                if (flag.Equals("S"))
                {

                    DataTable dt = supplierdt;
                    if (dt.Rows.Count > 0)
                    {
                        string supplierId = ObjToString(dt.Rows[0]["vcValue1"]);
                        List<string> EmailList = new List<string>();
                        for (int i = 2; i <= 4; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(ObjToString(dt.Rows[0]["vcValue" + i])))
                            {
                                EmailList.Add(ObjToString(dt.Rows[0]["vcValue" + i]));
                            }
                        }

                        for (int i = 0; i < EmailList.Count; i++)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = EmailList[i];
                            dr["displayName"] = supplierId;
                            receiverDt.Rows.Add(dr);
                        }
                    }
                    if (copy.Count > 0)
                    {
                        foreach (mailClass mail in copy)
                        {
                            DataRow dr = cCDt.NewRow();
                            dr["address"] = mail.address;
                            dr["displayName"] = mail.display;
                            cCDt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        cCDt = null;
                    }
                }
                else if (flag.Equals("F"))
                {
                    cCDt = null;
                    foreach (mailClass mailClass in Fail)
                    {
                        DataRow dr = receiverDt.NewRow();
                        dr["address"] = mailClass.address;
                        dr["displayName"] = mailClass.display;
                        receiverDt.Rows.Add(dr);
                    }
                }

                string EmailBody = Body;
                string result = ComFunction.SendEmailInfo(userEmail, "补给系统", EmailBody, receiverDt, cCDt, strSubject, path, false);

                //邮件发送失败
                if (result.Equals("Error"))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        #endregion

        #region 获取已做成订单

        public List<string> getFinish()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcOrderNo FROM dbo.TOrderUploadManage WHERE vcOrderState = '1'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                List<string> list = new List<string>();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        list.Add(ObjToString(dt.Rows[i]["vcOrderNo"]).Trim());
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public string ChangeBarCode(string strPartsNo)
        {
            string strBarCode = "";
            try
            {
                int lngBarCodeCount = 0;
                int lngAscCode = 0;
                if (strPartsNo.Substring(10, 2) == "00")
                    strPartsNo = strPartsNo.Substring(0, 10) + "  ";
                int PLen = strPartsNo.Length;
                for (int i = 0; i < PLen; i++)
                {
                    char asc = char.Parse(strPartsNo.Substring(i, 1));
                    lngAscCode = (int)asc;
                    if (lngAscCode != 32)
                    {
                        if (lngAscCode < 65)
                        {
                            lngBarCodeCount = lngBarCodeCount + (lngAscCode - 48);
                        }
                        else
                        {
                            lngBarCodeCount = lngBarCodeCount + (lngAscCode - 55);
                        }
                    }
                    else
                    {
                        lngBarCodeCount = lngBarCodeCount + 38;
                    }
                }
                lngAscCode = lngBarCodeCount % 43;
                if (lngAscCode < 10)
                {
                    strBarCode = Convert.ToChar(lngAscCode + 48).ToString();
                }
                else if (lngAscCode > 9 && lngAscCode < 36)
                {
                    strBarCode = Convert.ToChar(lngAscCode + 55).ToString();
                }
                else
                {
                    switch (lngAscCode)
                    {
                        case 36:
                            strBarCode = "-";
                            break;
                        case 37:
                            strBarCode = ".";
                            break;
                        case 38:
                            strBarCode = " ";
                            break;
                        case 39:
                            strBarCode = "$";
                            break;
                        case 40:
                            strBarCode = "/";
                            break;
                        case 41:
                            strBarCode = "+";
                            break;
                        case 42:
                            strBarCode = "%";
                            break;
                        default:
                            break;

                    }
                }
                strBarCode = strPartsNo + strBarCode;
                return "*" + strBarCode + "*";
            }
            catch (Exception ex)
            {
                strBarCode = strPartsNo + strBarCode;
                return "*" + strBarCode + "*";
            }
        }


    }
}