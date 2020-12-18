/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	看板再发行					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/09/7							
* 	类名			    :	FS1210_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;
using System.Linq;

namespace Logic
{
    public class FS1210_Logic
    {
        FS1210_DataAccess dataAccess = new FS1210_DataAccess();

        /// <summary>
        /// 订单号连番查找
        /// <returns></returns>
        /// </summary>
        public DataTable isKanBanSea(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            return dataAccess.isKanBanSea(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
        }
            /// <summary>
            /// 检索数据
            /// </summary>
            public DataTable PrintData(string vcKbOrderId, string vcTF, string vcFBZ, string vcTT, string vcTFZ, string vcPartsNo, string vcCarType, string vcGC, string vcType, string vcplant, DataTable dtflag)
        {
            return dataAccess.PrintData(vcKbOrderId, vcTF, vcFBZ, vcTT, vcTFZ, vcPartsNo, vcCarType, vcGC, vcType, vcplant, dtflag);
        }

        /// <summary>
        /// 检查所打印的看板是否是已经打印状态
        /// </summary>
        public bool IfPrintKB(string vcNo)
        {
            return dataAccess.IfPrintKB(vcNo);
        }
        /// <summary>
        /// 判断在打印的打印类型 |秦丰ED、秦丰非ED、非秦丰|
        /// </summary>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <param name="vcPlanMonth"></param>
        /// <returns></returns>
        public DataTable QFED00QuFen(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial, string vcPlanMonth, string vcNo)
        {
            return dataAccess.QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        }
        public DataTable rePrintDataED(string vcPartsNo, string vcDock, string vcPlanMonth, string vcKBorderno, string vcKBSerial, string vcNo, string vcCarFamilyCode)
        {
            return dataAccess.rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo, vcCarFamilyCode);
        }
        /// <summary>
        /// 秦丰ED和非秦丰的看板再发行
        /// </summary>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <param name="vcplantMonth"></param>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable rePrintData(string vcPartsNo, string vcDock, string vcPlanMonth, string vcKBorderno, string vcKBSerial, string vcNo)
        {
            return dataAccess.rePrintData(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo);
        }
        /// <summary>
        /// 获取再打印页面中非再打印的数据
        /// </summary>
        public DataTable GetPrintFZData(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial, string vcPlanMonth, string vcNo)
        {
            return dataAccess.GetPrintFZData(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        }
        public DataTable dtKBSerial_history(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial)
        {
            return dataAccess.dtKBSerial_history(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
        }
        public string dtKBSerialUP(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial)
        {
            return dataAccess.dtKBSerialUP(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
        }
        public string dtMasteriQuantity(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            return dataAccess.dtMasteriQuantity(vcPartsNo, vcDock, vcPlanMonth);
        }
        /// <summary>
        /// 关联数据查看是否存在在打印数据
        /// </summary>
        public DataTable seaKBnoser(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            return dataAccess.seaKBnoser(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
        }
        /// <summary>
        /// 关联数据查看是否存在在打印数据连番表中
        /// </summary>
        public DataTable seaKBSerial_history(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            return dataAccess.seaKBSerial_history(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
        }
        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCR(DataTable dt)
        {
            return dataAccess.insertTableCR(dt);
        }
        /// <summary>
        /// 插入看板确认单Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableExcel00(DataTable dt)
        {
            return dataAccess.insertTableExcel00(dt);
        }
        public DataSet PrintExcel(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00)
        {
            return dataAccess.PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);
        }
        /// <summary>
        /// 更新看板打印表170
        /// </summary>
        public bool UpdatePrintKANB(DataTable dt)
        {
            return dataAccess.UpdatePrintKANB(dt);
        }

        public DataTable CreatDataTable()
        {
            #region 定义数据DataTable
            DataTable dt = new DataTable();
            // 定义列
            DataColumn dc_vcpartsNo = new DataColumn();
            DataColumn dc_vcCarFamlyCode = new DataColumn();
            DataColumn dc_vcPartsNameCHN = new DataColumn();
            DataColumn dc_vcPCB01 = new DataColumn();//生产日期
            DataColumn dc_vcPCB02 = new DataColumn();
            DataColumn dc_vcPCB03 = new DataColumn();
            DataColumn dc_iQuantityPerContainer = new DataColumn();
            DataColumn dc_vcPorType = new DataColumn();
            DataColumn dc_vcKBorderno = new DataColumn();
            DataColumn dc_vcKBSerial = new DataColumn();
            DataColumn dc_vcComDate00 = new DataColumn();
            DataColumn dc_vcBanZhi00 = new DataColumn();

            // 定义列名
            dc_vcpartsNo.ColumnName = "vcpartsNo";
            dc_vcCarFamlyCode.ColumnName = "vcCarFamlyCode";
            dc_vcPartsNameCHN.ColumnName = "vcPartsNameCHN";
            dc_vcPCB01.ColumnName = "vcPCB01";
            dc_vcPCB02.ColumnName = "vcPCB02";
            dc_vcPCB03.ColumnName = "vcPCB03";
            dc_iQuantityPerContainer.ColumnName = "iQuantityPerContainer";
            dc_vcPorType.ColumnName = "vcPorType";
            dc_vcKBorderno.ColumnName = "vcKBorderno";
            dc_vcKBSerial.ColumnName = "vcKBSerial";
            dc_vcComDate00.ColumnName = "vcComDate00";
            dc_vcBanZhi00.ColumnName = "vcBanZhi00";

            // 将定义的列加入到dtTemp中
            dt.Columns.Add(dc_vcpartsNo);
            dt.Columns.Add(dc_vcCarFamlyCode);
            dt.Columns.Add(dc_vcPartsNameCHN);
            dt.Columns.Add(dc_vcPCB01);
            dt.Columns.Add(dc_vcPCB02);
            dt.Columns.Add(dc_vcPCB03);
            dt.Columns.Add(dc_iQuantityPerContainer);
            dt.Columns.Add(dc_vcPorType);
            dt.Columns.Add(dc_vcKBorderno);
            dt.Columns.Add(dc_vcKBSerial);
            dt.Columns.Add(dc_vcComDate00);
            dt.Columns.Add(dc_vcBanZhi00);
            #endregion
            return dt;
        }

        //public string BtnPrint(FS1210_Print_ViewModel fS1210_Print_ViewModel, string userId)
        //{
        //    FS1209_Logic FS1209_Logic = new FS1209_Logic();
        //    DataTable dtPorType = null;
        //    try
        //    {
        //        DataTable gvPrint = fS1210_Print_ViewModel.gvPrint;
        //        string tmplatePath = HttpContext.Current.Server.MapPath("~/Templates/FS1210.xlt");//Excel模板
        //        string gud = "";
        //        string vcFlagZ = "";
        //        string picnull = HttpContext.Current.Server.MapPath("~/images/picnull.JPG");
        //        PrinterCR print = new PrinterCR();
        //        QMPrinter exprint = new QMPrinter();
        //        byte[] vcPhotoPath = print.PhotoToArray("", picnull);//照片初始化
        //        DataTable dtPrintCR = new DataTable();
        //        DataTable dtPrintCRLone = print.searchTBCreate();//获取表testprinterCR结构，为打印数据填充提供DataTable
        //        DataTable dtPrint = dtPrintCRLone.Clone();//创建看板打印DataTable
        //        DataTable exdt = CreatDataTable();//创建看板投放确认单Excel打印DataTable
        //        bool check = true;
        //        string QFlag = "2";
        //        for (int i = 0; i < gvPrint.Rows.Count; i++)
        //        {
        //            string vcSupplierCode = ""; string vcSupplierPlant = ""; string vcCpdCompany = ""; string vcPartsNameEN = ""; string vcPartsNameCHN = "";
        //            string vcLogisticRoute = ""; string iQuantityPerContainer = "";
        //            string vcProject01 = ""; string vcComDate01 = ""; string vcBanZhi01 = ""; string vcAB01 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcProject02 = ""; string vcComDate02 = ""; string vcBanZhi02 = ""; string vcAB02 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcProject03 = ""; string vcComDate03 = ""; string vcBanZhi03 = ""; string vcAB03 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcProject04 = ""; string vcComDate04 = ""; string vcBanZhi04 = ""; string vcAB04 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcRemark1 = ""; string vcRemark2 = "";
        //            string PorType = ""; string vcKBorser = "";
        //            string vcComDate00 = ""; string vcBanZhi00 = "";
        //            DataTable dtKANB = new DataTable();

        //            #region 整理数据
        //            string vcPartsNo = gvPrint.Rows[i]["vcPartsNo"].ToString().Replace("-", "");//品番
        //            string vcDock = gvPrint.Rows[i]["vcDock"].ToString();//受入
        //            string vcCarFamilyCode = gvPrint.Rows[i]["vcCarType"].ToString();//车型
        //            string vcEDflag = gvPrint.Rows[i]["jinjiqufen"].ToString();//紧急区分
        //            if (vcEDflag == "通常")
        //            {
        //                vcEDflag = "S";
        //            }
        //            else if (vcEDflag == "紧急")
        //            {
        //                vcEDflag = "E";
        //            }
        //            string vcKBorderno = gvPrint.Rows[i]["vcKBorderno"].ToString();//看板订单号
        //            string vcKBSerial = gvPrint.Rows[i]["vcKBSerial"].ToString();//连番
        //            string vcPlanMonth = "";
        //            string vcNo = "";
        //            string vcPorType = "";
        //            if (gvPrint.Columns.IndexOf("vcPlanMonth") >= 0)
        //            {
        //                vcPlanMonth = gvPrint.Rows[i]["vcPlanMonth"].ToString();
        //            }
        //            if (gvPrint.Columns.IndexOf("iNo") >= 0)
        //            {
        //                vcNo = gvPrint.Rows[i]["iNo"].ToString();
        //            }
        //            if (gvPrint.Columns.IndexOf("vcPorType") >= 0)
        //            {
        //                vcPorType = gvPrint.Rows[i]["vcPorType"].ToString();
        //            }

        //            //标记再打或者提前延迟打印区分是否打印Excel表
        //            #region 整理数据 判断打印类型
        //            check = IfPrintKB(vcNo);
        //            if (check)//已经打印过 属于再发行
        //            {
        //                DataTable QFED = QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        //                if (QFED.Rows.Count != 0)
        //                {
        //                    if (QFED.Rows[0]["iBaiJianFlag"].ToString() == "1" && vcPartsNo.Substring(10, 2) != "ED")//秦丰非ED再发行
        //                    {
        //                        dtKANB = rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo, vcCarFamilyCode);
        //                    }
        //                    else//秦丰ED再发行//非秦丰再发行
        //                    {
        //                        dtKANB = rePrintData(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo);
        //                    }
        //                }
        //                vcFlagZ = "Z";
        //            }
        //            else//未打印过非再发行
        //            {
        //                dtKANB = GetPrintFZData(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        //                vcFlagZ = "TY";
        //            }
        //            #endregion
        //            if (dtKANB.Rows.Count != 0)
        //            {
        //                #region 取非空数据
        //                vcPhotoPath = print.PhotoToArray(dtKANB.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
        //                vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
        //                vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
        //                vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
        //                vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
        //                vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
        //                vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
        //                iQuantityPerContainer = dtKANB.Rows[0]["iQuantityPerContainer"].ToString();//收容数
        //                                                                                           //工程、完成日、班值
        //                vcComDate00 = dtKANB.Rows[0]["vcComDate00"].ToString();
        //                vcBanZhi00 = dtKANB.Rows[0]["vcBanZhi00"].ToString();
        //                vcProject01 = dtKANB.Rows[0]["vcProject01"].ToString();
        //                vcComDate01 = dtKANB.Rows[0]["vcComDate01"].ToString();
        //                vcBanZhi01 = dtKANB.Rows[0]["vcBanZhi01"].ToString();
        //                vcAB01 = dtKANB.Rows[0]["vcAB01"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcProject02 = dtKANB.Rows[0]["vcProject02"].ToString();
        //                vcComDate02 = dtKANB.Rows[0]["vcComDate02"].ToString();
        //                vcBanZhi02 = dtKANB.Rows[0]["vcBanZhi02"].ToString();
        //                vcAB02 = dtKANB.Rows[0]["vcAB02"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcProject03 = dtKANB.Rows[0]["vcProject03"].ToString();
        //                vcComDate03 = dtKANB.Rows[0]["vcComDate03"].ToString();
        //                vcBanZhi03 = dtKANB.Rows[0]["vcBanZhi03"].ToString();
        //                vcAB03 = dtKANB.Rows[0]["vcAB03"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcProject04 = dtKANB.Rows[0]["vcProject04"].ToString();
        //                vcComDate04 = dtKANB.Rows[0]["vcComDate04"].ToString();
        //                vcBanZhi04 = dtKANB.Rows[0]["vcBanZhi04"].ToString();
        //                vcAB04 = dtKANB.Rows[0]["vcAB04"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
        //                vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
        //                PorType = vcPorType;
        //                vcKBorser = vcKBorderno + vcDock;
        //                #endregion
        //                string QuantityPerContainerFED = FS1209_Logic.resQuantityPerContainer(vcPartsNo, vcDock, vcPlanMonth);
        //                if (QuantityPerContainerFED != iQuantityPerContainer)
        //                {
        //                    DataTable dtKBhistory = dtKBSerial_history(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
        //                    for (int a = 0; a < dtKBhistory.Rows.Count; a++)
        //                    {
        //                        gud = Guid.NewGuid().ToString("N");
        //                        iQuantityPerContainer = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(iQuantityPerContainer)) ? Convert.ToString((Convert.ToInt32(iQuantityPerContainer))) : Convert.ToString((Convert.ToInt32(QuantityPerContainerFED)));
        //                        dtPrint = dtPrintRE(print, vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, dtKBhistory.Rows[a]["vcKBSerial"].ToString(), vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
        //                        if (vcFlagZ == "TY")
        //                        {
        //                            DataRow exrow = exdt.NewRow();
        //                            exrow[0] = vcPartsNo;
        //                            exrow[1] = vcCarFamilyCode;
        //                            exrow[2] = vcPartsNameCHN;
        //                            exrow[3] = vcProject01;
        //                            exrow[4] = vcComDate01;
        //                            exrow[5] = vcBanZhi01;
        //                            exrow[6] = "1";//计数
        //                            exrow[7] = PorType;
        //                            exrow[8] = vcKBorderno;
        //                            exrow[9] = dtKBhistory.Rows[a]["vcKBSerial"].ToString();
        //                            exrow[10] = vcComDate00;
        //                            exrow[11] = vcBanZhi00;
        //                            exdt.Rows.Add(exrow);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    gud = Guid.NewGuid().ToString("N");
        //                    dtPrint = dtPrintRE(print, vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
        //                    if (vcFlagZ == "TY")
        //                    {
        //                        DataRow exrow = exdt.NewRow();
        //                        exrow[0] = vcPartsNo;
        //                        exrow[1] = vcCarFamilyCode;
        //                        exrow[2] = vcPartsNameCHN;
        //                        exrow[3] = vcProject01;
        //                        exrow[4] = vcComDate01;
        //                        exrow[5] = vcBanZhi01;
        //                        exrow[6] = "1";//计数
        //                        exrow[7] = PorType;
        //                        exrow[8] = vcKBorderno;
        //                        exrow[9] = vcKBSerial;
        //                        exrow[10] = vcComDate00;
        //                        exrow[11] = vcBanZhi00;
        //                        exdt.Rows.Add(exrow);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                #region 打印空数据
        //                gud = Guid.NewGuid().ToString("N");
        //                dtPrint = dtPrintRE(print, vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
        //                if (vcFlagZ == "TY")
        //                {
        //                    DataRow exrow = exdt.NewRow();
        //                    exrow[0] = vcPartsNo;
        //                    exrow[1] = vcCarFamilyCode;
        //                    exrow[2] = vcPartsNameCHN;
        //                    exrow[3] = vcProject01;
        //                    exrow[4] = vcComDate01;
        //                    exrow[5] = vcBanZhi01;
        //                    exrow[6] = "1";//计数
        //                    exrow[7] = PorType;
        //                    exrow[8] = vcKBorderno;
        //                    exrow[9] = vcKBSerial;
        //                    exrow[10] = vcComDate00;
        //                    exrow[11] = vcBanZhi00;
        //                    exdt.Rows.Add(exrow);
        //                }
        //                #endregion
        //            }
        //            #endregion

        //        }
        //        #region 打印处理
        //        dtPrint = print.orderDataTable(dtPrint);//排序
        //        insertTableCR(dtPrint);//插入打印临时子表
        //        insertTableExcel00(exdt);//插入看板确认单Excel
        //        //DataTable dtPorType = print.searchPorType00();//取生产部署
        //        //六项数据
        //        dtPorType = QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
        //        print.insertTableCRMain00(dtPrint, dtPorType);//插入打印临时主表
        //        string printDay = "";
        //        //FS1209_Logic.KanBIfPrintDay();//获取班值信息
        //        string reportPath = HttpContext.Current.Server.MapPath("~/CrReport.rpt");
        //        string strLoginId = userId;
        //        for (int z = 0; z < dtPorType.Rows.Count; z++)
        //        {
        //            //DataTable exdtt = exdt.Clone();
        //            DataTable exdttt = new DataTable();
        //            DataTable exdthj = new DataTable();
        //            //exdttt.Clear();
        //            string vcPorType = dtPorType.Rows[z]["vcPorType"].ToString();
        //            string vcorderno = dtPorType.Rows[z]["vcorderno"].ToString();
        //            string vcComDate01 = dtPorType.Rows[z]["vcComDate01"].ToString();
        //            string vcBanZhi01 = dtPorType.Rows[z]["vcBanZhi01"].ToString();
        //            string vcComDate00 = dtPorType.Rows[z]["vcComDate00"].ToString();
        //            string vcBanZhi00 = dtPorType.Rows[z]["vcBanZhi00"].ToString();

        //            bool retb = print.printCr(reportPath, vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00, strLoginId, fS1210_Print_ViewModel.vcPrinterName);//打印水晶报表
        //            DataSet ds = PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);
        //            exdttt = ds.Tables[0];
        //            if (exdttt.Rows.Count != 0)
        //            {
        //                #region Excel打印
        //                exdthj = ds.Tables[1];
        //                for (int p = 0; p < exdttt.Rows.Count; p++)
        //                {
        //                    exdttt.Rows[p]["no"] = p + 1;
        //                }
        //                int dsRowsCount = exdttt.Rows.Count;
        //                int dsRow = dsRowsCount / 43;
        //                int dsrows = dsRowsCount % 43;
        //                //总页数
        //                int pagetotle = 0;
        //                //页数
        //                int pageno = 0;
        //                if (dsRow != 0)
        //                {
        //                    pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
        //                }
        //                else
        //                {
        //                    pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
        //                }
        //                if (dsRow > 0)
        //                {
        //                    DataTable inTable = exdttt.Clone();
        //                    for (int i = 0; i < exdttt.Rows.Count; i++)
        //                    {
        //                        DataRow dr = exdttt.Rows[i];
        //                        DataRow add = inTable.NewRow();
        //                        add.ItemArray = dr.ItemArray;
        //                        inTable.Rows.Add(add);
        //                        if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
        //                        {
        //                            pageno = pageno + 1;
        //                            string pageB = "0";
        //                            if (inTable.Rows.Count <= 43)
        //                            {
        //                                pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
        //                            }
        //                            //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
        //                            exprint.PrintTemplateFromDataTable(inTable, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", fS1210_Print_ViewModel.vcPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
        //                            inTable = exdt.Clone();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    string pageB = "0";
        //                    pageno = pageno + 1;
        //                    pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
        //                    //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
        //                    exprint.PrintTemplateFromDataTable(exdttt, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", fS1210_Print_ViewModel.vcPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
        //                }
        //                #endregion
        //            }
        //            //删除看板打印的临时文件
        //            FS1209_Logic.DeleteprinterCREX(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
        //        }
        //        #endregion
        //        if (!(check))
        //        {
        //            UpdatePrintKANB(dtPrint);//更新看板打印表
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {

        //        if (dtPorType != null)
        //        {
        //            DataTable dt = dtPorType;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                FS1209_Logic.DeleteprinterCREX(dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcorderno"].ToString(), dt.Rows[i]["vcComDate01"].ToString(), dt.Rows[i]["vcBanZhi01"].ToString());
        //            }
        //        }
        //        LogHelper.ErrorLog("打印失败：" + ex.Message);
        //        return "打印失败：" + ex.Message;
        //    }
        //    return "";
        //}

        //public string BtnAllPrint(FS1210_Print_ViewModel fS1210_Print_ViewModel, string userId)
        //{
        //    FS1209_Logic FS1209_Logic = new FS1209_Logic();
        //    DataTable dtPorType = null;
        //    try
        //    {
        //        DataTable gvPrint = fS1210_Print_ViewModel.gvPrint;
        //        string tmplatePath = HttpContext.Current.Server.MapPath("~/Templates/FS1210.xlt");//Excel模板
        //        string gud = "";
        //        string vcFlagZ = "";
        //        string picnull = HttpContext.Current.Server.MapPath("~/images/picnull.JPG");
        //        PrinterCR print = new PrinterCR();
        //        QMPrinter exprint = new QMPrinter();
        //        byte[] vcPhotoPath = print.PhotoToArray("", picnull);//照片初始化
        //        DataTable dtPrintCR = new DataTable();
        //        DataTable dtPrintCRLone = print.searchTBCreate();//获取表testprinterCR结构，为打印数据填充提供DataTable
        //        DataTable dtPrint = dtPrintCRLone.Clone();//创建看板打印DataTable
        //        DataTable exdt = CreatDataTable();//创建看板投放确认单Excel打印DataTable
        //        bool check = true;
        //        string QFlag = "2";
        //        print.TurnCate();
        //        for (int i = 0; i < gvPrint.Rows.Count; i++)
        //        {
        //            string vcSupplierCode = ""; string vcSupplierPlant = ""; string vcCpdCompany = ""; string vcPartsNameEN = ""; string vcPartsNameCHN = "";
        //            string vcLogisticRoute = ""; string iQuantityPerContainer = "";
        //            string vcProject01 = ""; string vcComDate01 = ""; string vcBanZhi01 = ""; string vcAB01 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcProject02 = ""; string vcComDate02 = ""; string vcBanZhi02 = ""; string vcAB02 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcProject03 = ""; string vcComDate03 = ""; string vcBanZhi03 = ""; string vcAB03 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcProject04 = ""; string vcComDate04 = ""; string vcBanZhi04 = ""; string vcAB04 = "";//20181010添加AB值信息 - 李兴旺
        //            string vcRemark1 = ""; string vcRemark2 = "";
        //            string PorType = ""; string vcKBorser = "";
        //            string vcComDate00 = ""; string vcBanZhi00 = "";
        //            DataTable dtKANB = new DataTable();

        //            #region 整理数据
        //            string vcPartsNo = gvPrint.Rows[i]["vcPartsNo"].ToString().Replace("-", "");//品番
        //            string vcDock = gvPrint.Rows[i]["vcDock"].ToString();//受入
        //            string vcCarFamilyCode = gvPrint.Rows[i]["vcCarType"].ToString();//车型
        //            string vcEDflag = gvPrint.Rows[i]["jinjiqufen"].ToString();//紧急区分
        //            if (vcEDflag == "通常")
        //            {
        //                vcEDflag = "S";
        //            }
        //            else if (vcEDflag == "紧急")
        //            {
        //                vcEDflag = "E";
        //            }
        //            else
        //            {
        //                vcEDflag = " ";
        //            }
        //            string vcKBorderno = gvPrint.Rows[i]["vcKBorderno"].ToString();//看板订单号
        //            string vcKBSerial = gvPrint.Rows[i]["vcKBSerial"].ToString();//连番
        //            string vcPlanMonth = "";
        //            string vcNo = "";
        //            string vcPorType = "";
        //            if (gvPrint.Columns.IndexOf("vcPlanMonth") >= 0)
        //            {
        //                vcPlanMonth = gvPrint.Rows[i]["vcPlanMonth"].ToString();
        //            }
        //            if (gvPrint.Columns.IndexOf("iNo") >= 0)
        //            {
        //                vcNo = gvPrint.Rows[i]["iNo"].ToString();
        //            }
        //            if (gvPrint.Columns.IndexOf("vcPorType") >= 0)
        //            {
        //                vcPorType = gvPrint.Rows[i]["vcPorType"].ToString();
        //            }

        //            //标记再打或者提前延迟打印区分是否打印Excel表
        //            #region 整理数据 判断打印类型
        //            check = IfPrintKB(vcNo);
        //            if (check)//已经打印过 属于再发行
        //            {
        //                DataTable QFED = QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        //                if (QFED.Rows.Count != 0)
        //                {
        //                    if (QFED.Rows[0]["iBaiJianFlag"].ToString() == "1" && vcPartsNo.Substring(10, 2) != "ED")//秦丰非ED再发行
        //                    {
        //                        dtKANB = rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo, vcCarFamilyCode);
        //                    }
        //                    else//秦丰ED再发行//非秦丰再发行
        //                    {
        //                        dtKANB = rePrintData(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo);
        //                    }
        //                }
        //                vcFlagZ = "Z";
        //            }
        //            else//未打印过非再发行
        //            {
        //                dtKANB = GetPrintFZData(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        //                vcFlagZ = "TY";
        //            }
        //            #endregion
        //            if (dtKANB.Rows.Count != 0)
        //            {
        //                #region 取非空数据
        //                vcPhotoPath = print.PhotoToArray(dtKANB.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
        //                vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
        //                vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
        //                vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
        //                vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
        //                vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
        //                vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
        //                iQuantityPerContainer = dtKANB.Rows[0]["iQuantityPerContainer"].ToString();//收容数
        //                                                                                           //工程、完成日、班值
        //                vcComDate00 = dtKANB.Rows[0]["vcComDate00"].ToString();
        //                vcBanZhi00 = dtKANB.Rows[0]["vcBanZhi00"].ToString();
        //                vcProject01 = dtKANB.Rows[0]["vcProject01"].ToString();
        //                vcComDate01 = dtKANB.Rows[0]["vcComDate01"].ToString();
        //                vcBanZhi01 = dtKANB.Rows[0]["vcBanZhi01"].ToString();
        //                vcAB01 = dtKANB.Rows[0]["vcAB01"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcProject02 = dtKANB.Rows[0]["vcProject02"].ToString();
        //                vcComDate02 = dtKANB.Rows[0]["vcComDate02"].ToString();
        //                vcBanZhi02 = dtKANB.Rows[0]["vcBanZhi02"].ToString();
        //                vcAB02 = dtKANB.Rows[0]["vcAB02"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcProject03 = dtKANB.Rows[0]["vcProject03"].ToString();
        //                vcComDate03 = dtKANB.Rows[0]["vcComDate03"].ToString();
        //                vcBanZhi03 = dtKANB.Rows[0]["vcBanZhi03"].ToString();
        //                vcAB03 = dtKANB.Rows[0]["vcAB03"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcProject04 = dtKANB.Rows[0]["vcProject04"].ToString();
        //                vcComDate04 = dtKANB.Rows[0]["vcComDate04"].ToString();
        //                vcBanZhi04 = dtKANB.Rows[0]["vcBanZhi04"].ToString();
        //                vcAB04 = dtKANB.Rows[0]["vcAB04"].ToString();//20181010添加AB值信息 - 李兴旺
        //                vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
        //                vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
        //                PorType = vcPorType;
        //                vcKBorser = vcKBorderno + vcDock;
        //                #endregion
        //                string QuantityPerContainerFED = FS1209_Logic.resQuantityPerContainer(vcPartsNo, vcDock, vcPlanMonth);
        //                if (QuantityPerContainerFED != iQuantityPerContainer)
        //                {
        //                    DataTable dtKBhistory = dtKBSerial_history(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
        //                    for (int a = 0; a < dtKBhistory.Rows.Count; a++)
        //                    {
        //                        gud = Guid.NewGuid().ToString("N");
        //                        iQuantityPerContainer = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(iQuantityPerContainer)) ? Convert.ToString((Convert.ToInt32(iQuantityPerContainer))) : Convert.ToString((Convert.ToInt32(QuantityPerContainerFED)));
        //                        dtPrint = dtPrintRE(print, vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, dtKBhistory.Rows[a]["vcKBSerial"].ToString(), vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
        //                        if (vcFlagZ == "TY")
        //                        {
        //                            DataRow exrow = exdt.NewRow();
        //                            exrow[0] = vcPartsNo;
        //                            exrow[1] = vcCarFamilyCode;
        //                            exrow[2] = vcPartsNameCHN;
        //                            exrow[3] = vcProject01;
        //                            exrow[4] = vcComDate01;
        //                            exrow[5] = vcBanZhi01;
        //                            exrow[6] = "1";//计数
        //                            exrow[7] = PorType;
        //                            exrow[8] = vcKBorderno;
        //                            exrow[9] = dtKBhistory.Rows[a]["vcKBSerial"].ToString();
        //                            exrow[10] = vcComDate00;
        //                            exrow[11] = vcBanZhi00;
        //                            exdt.Rows.Add(exrow);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    gud = Guid.NewGuid().ToString("N");
        //                    dtPrint = dtPrintRE(print, vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
        //                    if (vcFlagZ == "TY")
        //                    {
        //                        DataRow exrow = exdt.NewRow();
        //                        exrow[0] = vcPartsNo;
        //                        exrow[1] = vcCarFamilyCode;
        //                        exrow[2] = vcPartsNameCHN;
        //                        exrow[3] = vcProject01;
        //                        exrow[4] = vcComDate01;
        //                        exrow[5] = vcBanZhi01;
        //                        exrow[6] = "1";//计数
        //                        exrow[7] = PorType;
        //                        exrow[8] = vcKBorderno;
        //                        exrow[9] = vcKBSerial;
        //                        exrow[10] = vcComDate00;
        //                        exrow[11] = vcBanZhi00;
        //                        exdt.Rows.Add(exrow);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                #region 打印空数据
        //                gud = Guid.NewGuid().ToString("N");
        //                dtPrint = dtPrintRE(print, vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
        //                if (vcFlagZ == "TY")
        //                {
        //                    DataRow exrow = exdt.NewRow();
        //                    exrow[0] = vcPartsNo;
        //                    exrow[1] = vcCarFamilyCode;
        //                    exrow[2] = vcPartsNameCHN;
        //                    exrow[3] = vcProject01;
        //                    exrow[4] = vcComDate01;
        //                    exrow[5] = vcBanZhi01;
        //                    exrow[6] = "1";//计数
        //                    exrow[7] = PorType;
        //                    exrow[8] = vcKBorderno;
        //                    exrow[9] = vcKBSerial;
        //                    exrow[10] = vcComDate00;
        //                    exrow[11] = vcBanZhi00;
        //                    exdt.Rows.Add(exrow);
        //                }
        //                #endregion
        //            }
        //            #endregion

        //        }
        //        #region 打印处理
        //        dtPrint = print.orderDataTable(dtPrint);//排序
        //        insertTableCR(dtPrint);//插入打印临时子表
        //        insertTableExcel00(exdt);//插入看板确认单Excel
        //        //DataTable dtPorType = print.searchPorType00();//取生产部署
        //        //六项数据
        //        dtPorType = QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
        //        print.insertTableCRMain00(dtPrint, dtPorType);//插入打印临时主表
        //        string printDay = "";
        //        //FS1209_Logic.KanBIfPrintDay();//获取班值信息
        //        string reportPath = HttpContext.Current.Server.MapPath("~/CrReport.rpt");
        //        string strLoginId = userId;
        //        for (int z = 0; z < dtPorType.Rows.Count; z++)
        //        {
        //            //DataTable exdtt = exdt.Clone();
        //            DataTable exdttt = new DataTable();
        //            DataTable exdthj = new DataTable();
        //            //exdttt.Clear();
        //            string vcPorType = dtPorType.Rows[z]["vcPorType"].ToString();
        //            string vcorderno = dtPorType.Rows[z]["vcorderno"].ToString();
        //            string vcComDate01 = dtPorType.Rows[z]["vcComDate01"].ToString();
        //            string vcBanZhi01 = dtPorType.Rows[z]["vcBanZhi01"].ToString();
        //            string vcComDate00 = dtPorType.Rows[z]["vcComDate00"].ToString();
        //            string vcBanZhi00 = dtPorType.Rows[z]["vcBanZhi00"].ToString();

        //            bool retb = print.printCr(reportPath, vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00, strLoginId, fS1210_Print_ViewModel.vcPrinterName);//打印水晶报表
        //            DataSet ds = PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);
        //            exdttt = ds.Tables[0];
        //            if (exdttt.Rows.Count != 0)
        //            {
        //                #region Excel打印
        //                exdthj = ds.Tables[1];
        //                for (int p = 0; p < exdttt.Rows.Count; p++)
        //                {
        //                    exdttt.Rows[p]["no"] = p + 1;
        //                }
        //                int dsRowsCount = exdttt.Rows.Count;
        //                int dsRow = dsRowsCount / 43;
        //                int dsrows = dsRowsCount % 43;
        //                //总页数
        //                int pagetotle = 0;
        //                //页数
        //                int pageno = 0;
        //                if (dsRow != 0)
        //                {
        //                    pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
        //                }
        //                else
        //                {
        //                    pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
        //                }
        //                if (dsRow > 0)
        //                {
        //                    DataTable inTable = exdttt.Clone();
        //                    for (int i = 0; i < exdttt.Rows.Count; i++)
        //                    {
        //                        DataRow dr = exdttt.Rows[i];
        //                        DataRow add = inTable.NewRow();
        //                        add.ItemArray = dr.ItemArray;
        //                        inTable.Rows.Add(add);
        //                        if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
        //                        {
        //                            pageno = pageno + 1;
        //                            string pageB = "0";
        //                            if (inTable.Rows.Count < 43)
        //                            {
        //                                pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
        //                            }
        //                            //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
        //                            exprint.PrintTemplateFromDataTable(inTable, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", fS1210_Print_ViewModel.vcPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);

        //                            if (fS1210_Print_ViewModel.vcType != "再发行")
        //                            {
        //                                DataTable checkorderno = new DataTable();
        //                                checkorderno = FS1209_Logic.check(vcorderno, vcPorType);
        //                                int rows;
        //                                rows = checkorderno.Rows.Count;
        //                                //向tKanBanQrTbl表中插入数据
        //                                if (rows == 0)
        //                                {
        //                                    //将testprinterExcel表中数据存入到testprinterExcel1中
        //                                    FS1209_Logic.InsertInto(vcorderno, vcPorType);
        //                                    FS1209_Logic.InsertDate(vcorderno, vcPorType, vcComDate00, vcBanZhi00 == "白" ? "0" : "1", vcComDate01, vcBanZhi01 == "白" ? "0" : "1");
        //                                }
        //                            }
        //                            inTable = exdt.Clone();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    string pageB = "0";
        //                    pageno = pageno + 1;
        //                    pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
        //                    //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
        //                    exprint.PrintTemplateFromDataTable(exdttt, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", fS1210_Print_ViewModel.vcPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);

        //                    if (fS1210_Print_ViewModel.vcType != "再发行")
        //                    {
        //                        DataTable checkorderno = new DataTable();
        //                        checkorderno = FS1209_Logic.check(vcorderno, vcPorType);
        //                        int rows;
        //                        rows = checkorderno.Rows.Count;
        //                        //向tKanBanQrTbl表中插入数据
        //                        if (rows == 0)
        //                        {
        //                            //将testprinterExcel表中数据存入到testprinterExcel1中
        //                            FS1209_Logic.InsertInto(vcorderno, vcPorType);
        //                            FS1209_Logic.InsertDate(vcorderno, vcPorType, vcComDate00, vcBanZhi00 == "白" ? "0" : "1", vcComDate01, vcBanZhi01 == "白" ? "0" : "1");
        //                        }
        //                    }
        //                }
        //                #endregion
        //            }
        //            //删除看板打印的临时文件
        //            FS1209_Logic.DeleteprinterCREX(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
        //        }
        //        #endregion
        //        if (!(check))
        //        {
        //            UpdatePrintKANB(dtPrint);//更新看板打印表
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {

        //        if (dtPorType != null)
        //        {
        //            DataTable dt = dtPorType;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                FS1209_Logic.DeleteprinterCREX(dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcorderno"].ToString(), dt.Rows[i]["vcComDate01"].ToString(), dt.Rows[i]["vcBanZhi01"].ToString());
        //            }
        //        }
        //        LogHelper.ErrorLog("打印失败：" + ex.Message);
        //        return "打印失败：" + ex.Message;
        //    }
        //    return "";
        //}

        //public string BtnEDPrint(FS1210_Print_ViewModel fS1210_Print_ViewModel, string userId)
        //{
        //    FS1209_Logic FS1209_Logic = new FS1209_Logic();
        //    DataTable dtPorType = null;
        //    try
        //    {
        //        DataTable gvPrint = fS1210_Print_ViewModel.gvPrint;
        //        string tmplatePath = HttpContext.Current.Server.MapPath("~/Templates/FS1210.xlt");//Excel模板
        //        String ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
        //        string gud = "";
        //        string vcFlagZ = "";
        //        string picnull = HttpContext.Current.Server.MapPath("~/images/picnull.JPG");
        //        PrinterCR print = new PrinterCR();
        //        QMPrinter exprint = new QMPrinter();
        //        byte[] vcPhotoPath = print.PhotoToArray("", picnull);
        //        DataTable dtPrintCR = new DataTable();
        //        DataTable dtPrintCRLone = print.searchTBCreate();//获得数据库表结构
        //        DataTable dtPrint = dtPrintCRLone.Clone();
        //        bool check = true;
        //        string QFlag = "2";
        //        for (int i = 0; i < gvPrint.Rows.Count; i++)
        //        {
        //            string vcSupplierCode = ""; string vcSupplierPlant = ""; string vcCpdCompany = ""; string vcPartsNameEN = ""; string vcPartsNameCHN = "";
        //            string vcLogisticRoute = ""; string iQuantityPerContainer = "0";
        //            string vcProject01 = ""; string vcComDate01 = ""; string vcBanZhi01 = "";
        //            string vcProject02 = ""; string vcComDate02 = ""; string vcBanZhi02 = "";
        //            string vcProject03 = ""; string vcComDate03 = ""; string vcBanZhi03 = "";
        //            string vcProject04 = ""; string vcComDate04 = ""; string vcBanZhi04 = "";
        //            string vcRemark1 = ""; string vcRemark2 = "";
        //            string PorType = ""; string vcKBorser = "";
        //            DataTable dtKANB = new DataTable();
        //            #region 整理数据
        //            string vcPartsNo = gvPrint.Rows[i]["vcPartsNo"].ToString().Replace("-", "").ToString();//品番
        //            if (gvPrint.Rows[i]["iFlag"].ToString() != "insert")
        //            {
        //                return "选择的行不属于该打印类别，请选择后点击打印。";
        //            }

        //            string vcDock = gvPrint.Rows[i]["vcDock"].ToString();//受入
        //            string vcCarFamilyCode = gvPrint.Rows[i]["vcCarType"].ToString();//车型
        //            string vcEDflag = gvPrint.Rows[i]["jinjiqufen"].ToString();//紧急区分
        //            if (vcEDflag == "通常")
        //            {
        //                vcEDflag = "S";
        //            }
        //            else if (vcEDflag == "紧急")
        //            {
        //                vcEDflag = "E";
        //            }
        //            else
        //            {
        //                vcEDflag = " ";
        //            }
        //            string vcKBorderno = gvPrint.Rows[i]["vcKBorderno"].ToString();//看板订单号
        //            string vcKBSerial = gvPrint.Rows[i]["vcKBSerial"].ToString();//连番
        //            string vcPlanMonth = gvPrint.Rows[i]["vcPlanMonth"].ToString();
        //            string vcNo = gvPrint.Rows[i]["iNo"].ToString();
        //            string vcPorType = gvPrint.Rows[i]["vcPorType"].ToString();
        //            string vcKBSerialup = dtKBSerialUP(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);//获取打印的ED连番
        //            string QuantityPerContainerFED = dtMasteriQuantity(vcPartsNo, vcDock, vcPlanMonth);
        //            //dtKANB = print.searchPrintKANBALL(vcPartsNo, vcDock, vcKBorderno, vcKBSerialup);
        //            DataTable QFED = QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
        //            if (QFED.Rows.Count != 0)
        //            {
        //                if (QFED.Rows[0]["iBaiJianFlag"].ToString() == "1" && vcPartsNo.Substring(10, 2) != "ED")//秦丰非ED再发行
        //                {
        //                    dtKANB = rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerialup, vcNo, vcCarFamilyCode);
        //                }
        //            }
        //            if (dtKANB.Rows.Count != 0)
        //            {
        //                #region 取非空数据
        //                vcPhotoPath = print.PhotoToArray(dtKANB.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
        //                vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
        //                vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
        //                vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
        //                vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
        //                vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
        //                vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
        //                iQuantityPerContainer = dtKANB.Rows[0]["iQuantityPerContainer"].ToString();//收容数
        //                vcProject01 = dtKANB.Rows[0]["vcProject01"].ToString();//工程、完成日、班值
        //                vcComDate01 = dtKANB.Rows[0]["vcComDate01"].ToString();
        //                vcBanZhi01 = dtKANB.Rows[0]["vcBanZhi01"].ToString();
        //                vcProject02 = dtKANB.Rows[0]["vcProject02"].ToString();
        //                vcComDate02 = dtKANB.Rows[0]["vcComDate02"].ToString();
        //                vcBanZhi02 = dtKANB.Rows[0]["vcBanZhi02"].ToString();
        //                vcProject03 = dtKANB.Rows[0]["vcProject03"].ToString();
        //                vcComDate03 = dtKANB.Rows[0]["vcComDate03"].ToString();
        //                vcBanZhi03 = dtKANB.Rows[0]["vcBanZhi03"].ToString();
        //                vcProject04 = dtKANB.Rows[0]["vcProject04"].ToString();
        //                vcComDate04 = dtKANB.Rows[0]["vcComDate04"].ToString();
        //                vcBanZhi04 = dtKANB.Rows[0]["vcBanZhi04"].ToString();
        //                vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
        //                vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
        //                PorType = vcPorType;
        //                vcKBorser = vcKBorderno + vcDock;
        //                #endregion
        //            }
        //            gud = Guid.NewGuid().ToString("N");

        //            String ls_savePath = HttpContext.Current.Server.MapPath("~/QRCodeImages/" + ls_fileName);
        //            string reCode = print.reCode(vcSupplierCode, vcSupplierPlant, vcDock, vcPartsNo, iQuantityPerContainer, vcKBSerial, vcEDflag, vcKBorderno);
        //            byte[] vcQRCodeImge = print.GenGenerateQRCode(reCode, ls_savePath);
        //            DataRow row = dtPrint.NewRow();
        //            row[0] = vcSupplierCode.ToUpper();
        //            row[1] = vcCpdCompany.ToUpper();
        //            row[2] = vcCarFamilyCode.ToUpper();
        //            row[3] = vcPartsNo;
        //            row[4] = vcPartsNameEN;
        //            row[5] = vcPartsNameCHN;
        //            row[6] = vcLogisticRoute;
        //            row[7] = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(iQuantityPerContainer)) ? Convert.ToString((Convert.ToInt32(iQuantityPerContainer))) : Convert.ToString((Convert.ToInt32(QuantityPerContainerFED)));
        //            row[8] = vcProject01;
        //            row[9] = vcComDate01;
        //            row[10] = vcBanZhi01;
        //            row[11] = vcProject02;
        //            row[12] = vcComDate02;
        //            row[13] = vcBanZhi02;
        //            row[14] = vcProject03;
        //            row[15] = vcComDate03;
        //            row[16] = vcBanZhi03;
        //            row[17] = vcProject04;
        //            row[18] = vcComDate04;
        //            row[19] = vcBanZhi04;
        //            row[20] = vcRemark1;
        //            row[21] = vcRemark2;
        //            row[22] = vcKBSerial;
        //            row[23] = vcPhotoPath;
        //            row[24] = vcQRCodeImge;
        //            row[25] = vcDock;//标记类型
        //            row[26] = vcKBorser.ToUpper();
        //            row[27] = gud;
        //            row[28] = vcKBorderno.ToUpper();
        //            row[29] = PorType;
        //            row[30] = vcEDflag == "E" ? "紧急" : "";
        //            dtPrint.Rows.Add(row);
        //            #endregion
        //        }

        //        dtPrint = print.orderDataTable(dtPrint);//排序
        //        print.insertTableCR(dtPrint);//插入打印临时子表
        //        //DataTable dtPorType = print.searchPorType();//取生产部署
        //        dtPorType = QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
        //        print.insertTableCRMain(dtPrint, dtPorType);//插入打印临时主表
        //        string printDay = FS1209_Logic.KanBIfPrintDay();//获取班值信息
        //        string reportPath = HttpContext.Current.Server.MapPath("~/CrReport.rpt");
        //        string strLoginId = userId;
        //        for (int z = 0; z < dtPorType.Rows.Count; z++)
        //        {
        //            bool retb = print.printCr(reportPath, dtPorType.Rows[z]["vcPorType"].ToString(), "", "", "", "", "", strLoginId, fS1210_Print_ViewModel.vcPrinterName);//打印水晶报表
        //            //删除看板打印的临时文件
        //            FS1209_Logic.DeleteprinterCREX1(dtPorType.Rows[z]["vcPorType"].ToString(), dtPorType.Rows[z]["vcorderno"].ToString(), dtPorType.Rows[z]["vcComDate01"].ToString(), dtPorType.Rows[z]["vcBanZhi01"].ToString());
        //        }
        //        if (!(check))
        //        {
        //            UpdatePrintKANB(dtPrint);//更新看板打印表
        //        }
        //        return "";
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (dtPorType != null)
        //        {
        //            DataTable dt = dtPorType;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                FS1209_Logic.DeleteprinterCREX(dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcorderno"].ToString(), dt.Rows[i]["vcComDate01"].ToString(), dt.Rows[i]["vcBanZhi01"].ToString());
        //            }
        //        }
        //        LogHelper.ErrorLog("打印失败：" + ex.Message);
        //    }
        //    return "";
        //}

        public DataTable QueryGroup(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("vcComDate00", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("vcBanZhi00", Type.GetType("System.String"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            dtPorType.Columns.Add(dc5);
            dtPorType.Columns.Add(dc6);
            var query = from t in dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("vcorderno"), t2 = t.Field<string>("vcPorType"), t3 = t.Field<string>("vcComDate01"), t4 = t.Field<string>("vcBanZhi01"), t5 = t.Field<string>("vcComDate00"), t6 = t.Field<string>("vcBanZhi00") } into m
                        select new
                        {
                            vcorderno = m.Key.t1,
                            vcPorType = m.Key.t2,
                            vcComDate01 = m.Key.t3,
                            vcBanZhi01 = m.Key.t4,
                            vcComDate00 = m.Key.t5,
                            vcBanZhi00 = m.Key.t6
                        };
            foreach (var item in query.ToList())
            {
                DataRow dr = dtPorType.NewRow();
                dr["vcorderno"] = item.vcorderno;
                dr["vcPorType"] = item.vcPorType;
                dr["vcComDate01"] = item.vcComDate01;
                dr["vcBanZhi01"] = item.vcBanZhi01;
                dr["vcComDate00"] = item.vcComDate00;
                dr["vcBanZhi00"] = item.vcBanZhi00;
                dtPorType.Rows.Add(dr);
            }
            return dtPorType;
        }

        //private DataTable dtPrintRE(PrinterCR print, string vcSupplierCode, string vcCpdCompany, string vcCarFamilyCode, string vcPartsNo, string vcPartsNameEN, string vcPartsNameCHN, string vcLogisticRoute,
        //   string iQuantityPerContainer, string vcProject01, string vcComDate01, string vcBanZhi01, string vcProject02, string vcComDate02, string vcBanZhi02, string vcProject03,
        //   string vcComDate03, string vcBanZhi03, string vcProject04, string vcComDate04, string vcBanZhi04, string vcRemark1, string vcRemark2, string vcKBSerial, byte[] vcPhotoPath,
        //   string vcDock, string vcKBorser, string i, string vcKBorderno, string PorType, string vcEDflag, string vcSupplierPlant, DataTable dtPrint, string vcComDate00, string vcBanZhi00, string vcPlanMonth,
        //   string vcAB01, string vcAB02, string vcAB03, string vcAB04)//20181010添加AB值信息 - 李兴旺
        //{
        //    String ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
        //    String ls_savePath = HttpContext.Current.Server.MapPath("~/QRCodeImages/" + ls_fileName);
        //    string reCode = print.reCode(vcSupplierCode, vcSupplierPlant, vcDock, vcPartsNo, iQuantityPerContainer, vcKBSerial, vcEDflag, vcKBorderno);
        //    byte[] vcQRCodeImge = print.GenGenerateQRCode(reCode, ls_savePath);
        //    DataRow row = dtPrint.NewRow();
        //    row[0] = vcSupplierCode.ToUpper();
        //    row[1] = vcCpdCompany.ToUpper();
        //    row[2] = vcCarFamilyCode.ToUpper();
        //    row[3] = vcPartsNo;
        //    row[4] = vcPartsNameEN;
        //    row[5] = vcPartsNameCHN;
        //    row[6] = vcLogisticRoute;
        //    row[7] = iQuantityPerContainer;
        //    row[8] = vcProject01;
        //    row[9] = vcComDate01;
        //    row[10] = vcBanZhi01;
        //    row[11] = vcProject02;
        //    row[12] = vcComDate02;
        //    row[13] = vcBanZhi02;
        //    row[14] = vcProject03;
        //    row[15] = vcComDate03;
        //    row[16] = vcBanZhi03;
        //    row[17] = vcProject04;
        //    row[18] = vcComDate04;
        //    row[19] = vcBanZhi04;
        //    row[20] = vcRemark1;
        //    row[21] = vcRemark2;
        //    row[22] = vcKBSerial;
        //    row[23] = vcPhotoPath;
        //    row[24] = vcQRCodeImge;
        //    row[25] = vcDock;//标记类型
        //    row[26] = vcKBorser.ToUpper();
        //    row[27] = i;
        //    row[28] = vcKBorderno.ToUpper();
        //    row[29] = PorType;
        //    row[30] = vcEDflag == "E" ? "紧急" : "";
        //    row[31] = vcComDate00;
        //    row[32] = vcBanZhi00;
        //    row[34] = vcPlanMonth;
        //    row[35] = vcAB01;//20181010添加AB值 - 李兴旺
        //    row[36] = vcAB02;//20181010添加AB值 - 李兴旺
        //    row[37] = vcAB03;//20181010添加AB值 - 李兴旺
        //    row[38] = vcAB04;//20181010添加AB值 - 李兴旺
        //    dtPrint.Rows.Add(row);
        //    return dtPrint;
        //}

        //public bool UploadExcel(HttpPostedFile flie, string fsName, string username, out string serverfilename)
        //{
        //    bool isSucc = false;
        //    try
        //    {
        //        string guidName = Guid.NewGuid().ToString();

        //        if (Path.GetExtension(flie.FileName) == ".xls")
        //        {
        //            serverfilename = HttpContext.Current.Server.MapPath("~/Temps/" + fsName + username + guidName + ".xls");
        //        }
        //        else
        //        {
        //            serverfilename = HttpContext.Current.Server.MapPath("~/Temps/" + fsName + username + guidName + ".xlsx");
        //        }
        //        flie.SaveAs(serverfilename);
        //        isSucc = true;
        //        return isSucc;
        //    }
        //    catch
        //    {
        //        serverfilename = string.Empty;
        //        return isSucc;
        //    }
        //}
        /// <summary>
        /// 判断导入文件格式是否正确
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckHead_standTime(string InputFile)
        //{
        //    QMExcel oQMExcel = new QMExcel();
        //    List<string> lists = oQMExcel.GetExcelSchema(InputFile);
        //    if (lists.Count == 7)
        //    {
        //        if (lists[0] != "品番")
        //            return "A";
        //        if (lists[1] != "受入")
        //            return "B";
        //        if (lists[2] != "车型")
        //            return "C";
        //        if (lists[3] != "紧急区分")
        //            return "D";
        //        if (lists[4] != "看板订单号")
        //            return "E";
        //        if (lists[5] != "连番")
        //            return "F";
        //        if (lists[6] != "备注")
        //            return "G";
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        return "ALL";
        //    }
        //}
        /// <summary>
        /// 判断Excel是否有重复行
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckRepeat_Excel(string InputFile)
        //{
        //    QMExcel oQMExcel = new QMExcel();
        //    string[] primaryKeys = { "品番", "受入", "看板订单号", "连番", "紧急区分" };
        //    string msg = string.Empty;
        //    msg = oQMExcel.CheckRepeat(InputFile, primaryKeys);
        //    if (msg.Length > 0)
        //        return "模板含重复行[" + msg + "]";
        //    return msg;
        //}
        /// <summary>
        /// 判断Excel字段格式
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckGeShi_Excel(string InputFile)
        //{
        //    string msg = string.Empty;
        //    QMExcel oQMExcel = new QMExcel();
        //    DataTable dt = oQMExcel.GetExcelContentByOleDb(InputFile);
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        if (dt.Rows[i][0].ToString().Length != 12)
        //        {
        //            msg = "Excel第" + (i + 1) + "行品番格式错误";
        //            return msg;
        //        }
        //        if (dt.Rows[i][1].ToString().Length != 2)
        //        {
        //            msg = "Excel第" + (i + 1) + "行受入格式错误";
        //            return msg;
        //        }
        //        if (!(dt.Rows[i][3].ToString() == "通常" || dt.Rows[i][3].ToString() == "紧急"))
        //        {
        //            msg = "Excel第" + (i + 1) + "行紧急区分存在错误";
        //            return msg;
        //        }
        //        if (dt.Rows[i][4].ToString().Length != 10 && dt.Rows[i][4].ToString().Length != 12)
        //        {
        //            msg = "Excel第" + (i + 1) + "行看板订单号存在错误";
        //            return msg;
        //        }
        //        if (dt.Rows[i][5].ToString().Length != 4)
        //        {
        //            msg = "Excel第" + (i + 1) + "行连番存在错误";
        //            return msg;
        //        }
        //    }
        //    return "";
        //}
        //public DataTable CreatDataTable(string InputFile)
        //{

        //    DataTable dt_Init = new DataTable();
        //    dt_Init.Columns.Add("vcPartsNo", typeof(string));
        //    dt_Init.Columns.Add("vcDock", typeof(string));
        //    dt_Init.Columns.Add("vcCarType", typeof(string));
        //    dt_Init.Columns.Add("vcEDflag", typeof(string));
        //    dt_Init.Columns.Add("vcKBorderno", typeof(string));
        //    dt_Init.Columns.Add("vcKBSerial", typeof(string));
        //    dt_Init.Columns.Add("vcTips", typeof(string));
        //    dt_Init.Columns.Add("jinjiqufen", typeof(string));
        //    dt_Init.Columns.Add("vcPlanMonth", typeof(string));
        //    dt_Init.Columns.Add("iNo", typeof(string));
        //    dt_Init.Columns.Add("vcPorType", typeof(string));


        //    QMExcel oQMExcel = new QMExcel();
        //    DataTable dt = oQMExcel.GetExcelContentByOleDb(InputFile);
        //    int count = dt.Rows.Count;
        //    dt.Columns[0].ColumnName = "vcPartsNo";
        //    dt.Columns[1].ColumnName = "vcDock";
        //    dt.Columns[2].ColumnName = "vcCarType";
        //    dt.Columns[3].ColumnName = "jinjiqufen";
        //    dt.Columns[4].ColumnName = "vcKBorderno";
        //    dt.Columns[5].ColumnName = "vcKBSerial";
        //    dt.Columns[6].ColumnName = "vcTips";

        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        DataRow dr = dt_Init.NewRow();
        //        dr["vcPartsNo"] = dt.Rows[i]["vcPartsNo"].ToString();
        //        dr["vcDock"] = dt.Rows[i]["vcDock"].ToString();
        //        dr["vcCarType"] = dt.Rows[i]["vcCarType"].ToString();
        //        dr["jinjiqufen"] = dt.Rows[i]["jinjiqufen"].ToString();
        //        dr["vcKBorderno"] = dt.Rows[i]["vcKBorderno"].ToString();
        //        dr["vcKBSerial"] = dt.Rows[i]["vcKBSerial"].ToString();
        //        dr["vcTips"] = dt.Rows[i]["vcTips"].ToString();
        //        if (dt.Rows[i]["jinjiqufen"].ToString() == "通常")
        //        {
        //            dr["jinjiqufen"] = "S";
        //        }
        //        if (dt.Rows[i]["jinjiqufen"].ToString() == "紧急")
        //        {
        //            dr["jinjiqufen"] = "E";
        //        }
        //        DataTable dtget = dataAccess.SearchPartData(dr);
        //        if (dtget.Rows.Count > 0)
        //        {

        //            dr["vcEDflag"] = dtget.Rows[0]["vcEDflag"].ToString();
        //            dr["vcPlanMonth"] = dtget.Rows[0]["vcPlanMonth"].ToString();
        //            dr["vcPorType"] = dtget.Rows[0]["vcProType"].ToString();
        //            dr["iNo"] = dtget.Rows[0]["iNo"].ToString();

        //        }
        //        else
        //        {
        //            dr["vcEDflag"] = "";
        //            dr["vcPlanMonth"] = "";
        //            dr["vcPorType"] = "";
        //            dr["iNo"] = "";
        //        }


        //        dt_Init.Rows.Add(dr);
        //    }
        //    return dt_Init;
        //}
        /// <summary>
        /// 判断是否存在允许再发行的看板
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckPrint(string InputFile)
        //{
        //    try
        //    {
        //        DataTable dt = CreatDataTable(InputFile);
        //        int rowsi = dt.Rows.Count;
        //        int j = 0;
        //        string vcKBnoser = "";
        //        vcKBnoser += "'";
        //        if (dt.Rows.Count > 0)
        //        {
        //            foreach (DataRow rows in dt.Rows)
        //            {
        //                vcKBnoser += rows["vcKBorderno"].ToString() + rows["vcKBSerial"].ToString();
        //                if (j < rowsi - 1)
        //                {
        //                    vcKBnoser += "','";
        //                }
        //                else
        //                {
        //                    vcKBnoser += "'";
        //                }
        //                j++;
        //            }

        //            DataTable dtt = dataAccess.CheckPrint(vcKBnoser);
        //            if (dtt.Rows.Count != dt.Rows.Count)
        //            {
        //                string msg = "订单号";
        //                for (int x = 0; x < dt.Rows.Count; x++)
        //                {
        //                    int xy = 0;
        //                    string vcKBnoalx = dt.Rows[x]["vcKBorderno"].ToString() + dt.Rows[x]["vcKBSerial"].ToString();
        //                    for (int y = 0; y < dtt.Rows.Count; y++)
        //                    {
        //                        string vcKBnoaly = dtt.Rows[y]["vcKBorderno"].ToString() + dtt.Rows[y]["vcKBSerial"].ToString();
        //                        if (vcKBnoalx == vcKBnoaly)
        //                        {
        //                            break;
        //                        }
        //                        xy++;
        //                    }
        //                    if (xy == dtt.Rows.Count)
        //                    {
        //                        msg += dt.Rows[x]["vcKBorderno"].ToString();
        //                    }
        //                }
        //                if (msg != "订单号")
        //                {
        //                    return "";
        //                }
        //                else
        //                {
        //                    return msg;
        //                }

        //            }
        //            else
        //            {
        //                return "";
        //            }
        //        }
        //        else
        //        {
        //            return "导入文件没有数据。";
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 判断是否excel文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        //private bool IsExcel(HtmlInputFile file)
        //{
        //    string contentType = file.PostedFile.ContentType;
        //    return (contentType == "application/vnd.ms-excel" ||
        //             contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") ||
        //             (contentType == "application/octet-stream" && (Path.GetExtension(file.PostedFile.FileName) == ".xls"))
        //             || Path.GetExtension(file.PostedFile.FileName) == ".xlsx";
        //}

        //public string ExportFile(HttpPostedFile flie, ref DataTable dt)
        //{
        //    try
        //    {
        //        string msg = string.Empty;
        //        string filePathName = string.Empty;
        //        bool isOk = true;
        //        isOk = UploadExcel(flie, "FS1210", "0000", out filePathName);
        //        if (!isOk)
        //        {
        //            return "警告信息：导入失败,Excel出错,请确认...";
        //        }
        //        string headColErr = CheckHead_standTime(filePathName);
        //        //模板错误
        //        if (headColErr.Length > 0)
        //        {
        //            if (headColErr == "ALL")
        //            {
        //                return "被导入文件模板格式错误！";
        //            }
        //            else
        //            {
        //                return "被导入文件第'" + headColErr + "'列表头名称错误！";
        //            }
        //        }

        //        //重复行
        //        msg = CheckRepeat_Excel(filePathName);//判断是否有重复行--品番--受入
        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            return msg;
        //        }

        //        //字段格式是否正确
        //        msg = CheckGeShi_Excel(filePathName);
        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            return msg;
        //        }

        //        //判断是否有可以打印的再发行看板
        //        msg = CheckPrint(filePathName);
        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            return msg;
        //        }

        //        dt = CreatDataTable(filePathName);
        //        dt.Columns.Add("iFlag");
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            dt.Rows[i]["iFlag"] = "insert";
        //        }

        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogHelper.ErrorLog("导入失败：" + ex.Message);
        //    }
        //    return "";
        //}
        public DataTable SearchRePrintKBQR(string OrderNo, string GC, string PlanPrintDate, string PlanPrintBZ, string PlanProcDate, string PlanProcBZ, string PrintDate)
        {
            return dataAccess.SearchRePrintKBQR(OrderNo, GC, PlanPrintDate, PlanPrintBZ, PlanProcDate, PlanProcBZ, PrintDate);
        }
        //public string BtnKBQRPrint(FS1210_KBQR_ViewModel fS1210_KBQR_ViewModel, string strLoginId)
        //{
        //    try
        //    {
        //        string tmplatePath = HttpContext.Current.Server.MapPath("~/Templates/FS1210.xlt");//Excel模板
        //        PrinterCR print = new PrinterCR();
        //        QMPrinter exprint = new QMPrinter();
        //        string vcKbOrderId = fS1210_KBQR_ViewModel.vcKbOrderId;
        //        string vcGC = fS1210_KBQR_ViewModel.vcGC;
        //        string vcPlanPrintDate = fS1210_KBQR_ViewModel.vcPlanPrintDate;
        //        string vcPlanPrintBZ = fS1210_KBQR_ViewModel.vcPlanPrintBZ;
        //        string vcPlanProcDate = fS1210_KBQR_ViewModel.vcPlanProcDate;
        //        string vcPlanProcBZ = fS1210_KBQR_ViewModel.vcPlanProcBZ;
        //        string vcPrintDate = fS1210_KBQR_ViewModel.vcPrintDate;
        //        DataTable returndata = SearchRePrintKBQR(fS1210_KBQR_ViewModel.vcKbOrderId, fS1210_KBQR_ViewModel.vcGC
        //            , fS1210_KBQR_ViewModel.vcPlanPrintDate, fS1210_KBQR_ViewModel.vcPlanPrintBZ, fS1210_KBQR_ViewModel.vcPlanProcDate
        //            , fS1210_KBQR_ViewModel.vcPlanProcBZ, fS1210_KBQR_ViewModel.vcPrintDate);
        //        for (int z = 0; z < returndata.Rows.Count; z++)
        //        {
        //            if (fS1210_KBQR_ViewModel.listIndex.Contains(z))
        //            {
        //                //DataTable exdtt = exdt.Clone();
        //                DataTable exdttt = new DataTable();
        //                DataTable exdthj = new DataTable();
        //                //exdttt.Clear();

        //                vcGC = returndata.Rows[z]["vcGC"].ToString();
        //                vcKbOrderId = returndata.Rows[z]["vcOrderNo"].ToString();
        //                vcPlanPrintDate = returndata.Rows[z]["vcPlanPrintDate"].ToString();
        //                vcPlanPrintBZ = returndata.Rows[z]["vcPlanPrintBZ"].ToString();
        //                if (vcPlanPrintBZ == "白值") { vcPlanPrintBZ = "0"; }
        //                if (vcPlanPrintBZ == "夜值") { vcPlanPrintBZ = "1"; }
        //                vcPlanProcDate = returndata.Rows[z]["vcPlanProcDate"].ToString();
        //                vcPlanProcBZ = returndata.Rows[z]["vcPlanProcBZ"].ToString();
        //                if (vcPlanProcBZ == "白值") { vcPlanProcBZ = "0"; }
        //                if (vcPlanProcBZ == "夜值") { vcPlanProcBZ = "1"; }
        //                vcPrintDate = returndata.Rows[z]["vcPrintDate"].ToString();


        //                DataTable exdt = new DataTable();
        //                DataTable dtHis = new DataTable();
        //                exdt = exprint.CreatDataTable();//创建ExcelDataTable
        //                dtHis = exprint.CreatDataTableHis();//创建连番DataTable

        //                // 打印看板确认单

        //                //数据库取出Excel的数据进行打印
        //                DataSet ds = dataAccess.aPrintExcel(vcGC, vcKbOrderId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白" : "夜", vcPlanProcDate, vcPlanProcBZ == "0" ? "白" : "夜");


        //                exdttt = ds.Tables[0];
        //                exdthj = ds.Tables[1];
        //                for (int p = 0; p < exdttt.Rows.Count; p++)
        //                {
        //                    exdttt.Rows[p]["no"] = p + 1;
        //                }
        //                int dsRowsCount = exdttt.Rows.Count;
        //                int dsRow = dsRowsCount / 43;
        //                int dsrows = dsRowsCount % 43;
        //                //总页数
        //                int pagetotle = 0;
        //                //页数.
        //                int pageno = 0;
        //                if (dsRow != 0)
        //                {
        //                    pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
        //                }
        //                else
        //                {
        //                    pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
        //                }

        //                if (dsRow > 0)
        //                {
        //                    DataTable inTable = exdttt.Clone();

        //                    for (int i = 0; i < exdttt.Rows.Count; i++)
        //                    {
        //                        DataRow dr = exdttt.Rows[i];
        //                        DataRow add = inTable.NewRow();
        //                        add.ItemArray = dr.ItemArray;
        //                        inTable.Rows.Add(add);
        //                        if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
        //                        {
        //                            pageno = pageno + 1;
        //                            string pageB = "0";
        //                            if (inTable.Rows.Count < 43)
        //                            {
        //                                pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
        //                            }
        //                            //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
        //                            exprint.PrintTemplateFromDataTable1(inTable, exdthj, tmplatePath, vcKbOrderId, vcGC, strLoginId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白值" : "夜值", vcPlanProcDate, vcPlanProcBZ == "0" ? "白值" : "夜值", fS1210_KBQR_ViewModel.vcPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB, vcPrintDate);

        //                            inTable = exdt.Clone();

        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    string pageB = "0";
        //                    pageno = pageno + 1;
        //                    pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";

        //                    //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
        //                    exprint.PrintTemplateFromDataTable1(exdttt, exdthj, tmplatePath, vcKbOrderId, vcGC, strLoginId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白值" : "夜值", vcPlanProcDate, vcPlanProcBZ == "0" ? "白值" : "夜值", fS1210_KBQR_ViewModel.vcPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB, vcPrintDate);

        //                }

        //            }

        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //if (fS1210_KBQR_ViewModel.vcDtportype != null)
        //        //{
        //        //    FS1209_Logic FS1209_Logic = new FS1209_Logic();
        //        //    DataTable dt = fS1210_KBQR_ViewModel.vcDtportype;
        //        //    for (int i = 0; i < dt.Rows.Count; i++)
        //        //    {
        //        //        FS1209_Logic.DeleteprinterCREX(dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcorderno"].ToString(), dt.Rows[i]["vcComDate01"].ToString(), dt.Rows[i]["vcBanZhi01"].ToString());
        //        //    }
        //        //}
        //        LogHelper.ErrorLog("打印失败：" + ex.Message);
        //        return "打印失败：" + ex.Message;
        //    }
        //    return "";
        //}

        public DataTable SearchPrintTDB(string vcPrintflag, string[] str2)
        {
            return dataAccess.searchPrintTDB(vcPrintflag, str2);
        }

        //public string BtnPrintTDB(FS1210_TDB_ViewModel fFS1210_TDB_ViewModel, string strLoginId)
        //{
        //    FS1209_Logic FS1209_Logic = new FS1209_Logic();
        //    DataTable dtPorType = null;
        //    try
        //    {
        //        PrinterCR print = new PrinterCR();
        //        QMPrinter exprint = new QMPrinter();
        //        DataTable dtPrintCR = new DataTable();
        //        DataTable dtPrintCRLone = print.searchTBCreateCRExcep();//获得数据库表结构
        //        DataTable dtPrint = dtPrintCRLone.Clone();
        //        DataTable dt = fFS1210_TDB_ViewModel.gvPrint;
        //        byte[] vcPhotoPath;
        //        string picnull = HttpContext.Current.Server.MapPath("~/images/picnull.JPG");
        //        //print.TurnCate();
        //        if (dt.Rows.Count == 0)
        //        {
        //            return "无可打印数据,请添加";
        //        }
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            #region 整理数据
        //            string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();//品番
        //            string vcDock = dt.Rows[i]["vcDock"].ToString();//受入
        //            string vcQuantityPerContainer = dt.Rows[i]["vcQuantityPerContainer"].ToString();//收容数
        //            string vcCarFamilyCode = dt.Rows[i]["vcCarType"].ToString();//车型
        //            string vcEDflag = dt.Rows[i]["vcEDflag"].ToString();//紧急区分

        //            string vcKBorderno = dt.Rows[i]["vcKBorderno"].ToString();//看板订单号
        //            string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();//连番
        //            string vcTips = dt.Rows[i]["vcTips"].ToString();//备注
        //            string vcComDate01 = dt.Rows[i]["vcComDate01"].ToString();//完成日、班值
        //            string vcBanZhi01 = dt.Rows[i]["vcBanZhi01"].ToString();
        //            string vcComDate02 = dt.Rows[i]["vcComDate02"].ToString();
        //            string vcBanZhi02 = dt.Rows[i]["vcBanZhi02"].ToString();
        //            string vcComDate03 = dt.Rows[i]["vcComDate03"].ToString();
        //            string vcBanZhi03 = dt.Rows[i]["vcBanZhi03"].ToString();
        //            string vcComDate04 = dt.Rows[i]["vcComDate04"].ToString();
        //            string vcBanZhi04 = dt.Rows[i]["vcBanZhi04"].ToString();

        //            string vcSupplierCode = String.Empty;
        //            string vcSupplierPlant = String.Empty;
        //            string vcCpdCompany = String.Empty;
        //            string vcPartsNameEN = String.Empty;
        //            string vcPartsNameCHN = String.Empty;
        //            string vcLogisticRoute = String.Empty;
        //            string vcProject01 = String.Empty;
        //            string vcProject02 = String.Empty;
        //            string vcProject03 = String.Empty;
        //            string vcProject04 = String.Empty;
        //            string vcRemark1 = String.Empty;
        //            string vcRemark2 = String.Empty;

        //            DataTable dtKANB = print.searchProRuleMst(vcPartsNo, vcDock);
        //            string vcmage = "";//图片
        //            vcPhotoPath = print.PhotoToArray(vcmage, picnull);//图片二进制流
        //            if (dtKANB.Rows.Count != 0)
        //            {
        //                //检索打印看板的其他字段 
        //                //订单号、紧急区分searchPrintKANB
        //                vcmage = dtKANB.Rows[0]["vcPhotoPath"].ToString();//图片
        //                vcPhotoPath = print.PhotoToArray(vcmage, picnull);//图片二进制流
        //                vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
        //                vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
        //                vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
        //                vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
        //                vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
        //                vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
        //                vcProject01 = dtKANB.Rows[0]["vcProName1"].ToString();//工程
        //                vcProject02 = dtKANB.Rows[0]["vcProName2"].ToString();
        //                vcProject03 = dtKANB.Rows[0]["vcProName3"].ToString();
        //                vcProject04 = dtKANB.Rows[0]["vcProName4"].ToString();
        //                vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
        //                vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
        //            }
        //            string vcKBorser = vcKBorderno + vcDock;
        //            DataRow row = dtPrint.NewRow();
        //            row[0] = vcSupplierCode.ToUpper();
        //            row[1] = vcCpdCompany.ToUpper();
        //            row[2] = vcCarFamilyCode;
        //            row[3] = (vcPartsNo.Length == 12 ? (vcPartsNo.Substring(0, 5) + "-" + vcPartsNo.Substring(5, 5) + "-" + vcPartsNo.Substring(10, 2)) : (vcPartsNo.Substring(0, 5) + "-" + vcPartsNo.Substring(5, 5))).ToUpper();
        //            row[4] = vcPartsNameEN;
        //            row[5] = vcPartsNameCHN;
        //            row[6] = vcLogisticRoute;
        //            row[7] = vcQuantityPerContainer;
        //            row[8] = vcProject01;
        //            row[9] = vcComDate01;
        //            row[10] = vcBanZhi01;
        //            row[11] = vcProject02;
        //            row[12] = vcComDate02;
        //            row[13] = vcBanZhi02;
        //            row[14] = vcProject03;
        //            row[15] = vcComDate03;
        //            row[16] = vcBanZhi03;
        //            row[17] = vcProject04;
        //            row[18] = vcComDate04;
        //            row[19] = vcBanZhi04;
        //            row[20] = vcRemark1;
        //            row[21] = vcRemark2;
        //            row[22] = vcKBSerial;
        //            row[23] = vcPhotoPath;
        //            row[24] = vcTips;
        //            row[25] = vcDock;
        //            row[26] = vcKBorser.ToUpper();
        //            row[27] = Convert.ToString(i + 1).ToString();
        //            row[28] = vcKBorderno.ToUpper();
        //            row[29] = "QW";
        //            row[30] = vcEDflag == "E" ? "紧急" : "";
        //            dtPrint.Rows.Add(row);
        //            #endregion
        //        }

        //        dtPrint = print.orderDataTable(dtPrint);//排序
                
        //        print.insertTableCRExcep(dtPrint);//插入打印临时子表
        //        dtPorType = QueryGroupTS(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值

        //        print.insertTableCRMainEND(dtPrint, dtPorType);//插入打印临时主表
        //        string reportPath = HttpContext.Current.Server.MapPath("../CryReportEX.rpt");
        //        for (int z = 0; z < dtPorType.Rows.Count; z++)
        //        {
        //            bool retb = print.printCr(reportPath, dtPorType.Rows[z]["vcPorType"].ToString(), "", "", "", "", "", strLoginId, fFS1210_TDB_ViewModel.vcPrinterName);//打印水晶报表
        //            //删除看板打印的临时文件
        //            FS1209_Logic.DeleteprinterCREX2(dtPorType.Rows[z]["vcPorType"].ToString(), dtPorType.Rows[z]["vcorderno"].ToString(), dtPorType.Rows[z]["vcComDate01"].ToString(), dtPorType.Rows[z]["vcBanZhi01"].ToString());

        //        }
        //        print.UpdatePrintKANBCRExcep(dtPrint);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (dtPorType != null)
        //        {
        //            DataTable dt = dtPorType;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                FS1209_Logic.DeleteprinterCREX2(dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcorderno"].ToString(), dt.Rows[i]["vcComDate01"].ToString(), dt.Rows[i]["vcBanZhi01"].ToString());
        //            }
        //        }
        //        LogHelper.ErrorLog("打印失败：" + ex.Message);
        //        return "打印失败：" + ex.Message;
        //    }
        //    return "";
        //}

        public DataTable QueryGroupTS(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.String"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            var query = from t in dt.AsEnumerable()
                        group t by new { 
                            t1 = t.Field<string>("vcorderno"), 
                            t2 = t.Field<string>("vcPorType"), 
                            t3 = t.Field<string>("vcComDate01"), 
                            t4 = t.Field<string>("vcBanZhi01") 
                        } into m
                        select new
                        {
                            vcorderno = m.Key.t1,
                            vcPorType = m.Key.t2,
                            vcComDate01 = m.Key.t3,
                            vcBanZhi01 = m.Key.t4
                        };
            foreach (var item in query.ToList())
            {
                DataRow dr = dtPorType.NewRow();
                dr["vcorderno"] = item.vcorderno;
                dr["vcPorType"] = item.vcPorType;
                dr["vcComDate01"] = item.vcComDate01; ;
                dr["vcBanZhi01"] = item.vcBanZhi01;
                dtPorType.Rows.Add(dr);
            }
            return dtPorType;
        }

        public DataTable searchPrintT()
        {
            return dataAccess.searchPrintT();
        }

        public string InUpdeOldData(DataTable dt)
        {
            try
            {
                DataRow[] rowceck = dt.Select("iFlag='insert' or iFlag='delete'");
                if (rowceck.Length == 0)
                {
                    return "不存在更新数据!";
                }
                DataRow[] row = dt.Select("iFlag='insert' or iFlag='update'");
                if (row.Length != 0)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        DataRow rowdelete = row[i];

                        if (rowdelete["vcPartsNo"].ToString() == "" || rowdelete["vcDock"].ToString() == "")
                        {
                            return "数据填写不完整，品番和开始时间按不能为空!";
                        }
                    }
                }
                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "_1")
                {
                    bool reyurnc = dataAccess.partChongFu(dt);
                    if (!reyurnc)
                    {
                        return "新增数据中存在和数据库中重复的数据请确认!";
                    }
                    else
                    {
                        bool ruturn = dataAccess.InUpdeOldData(dt, "");
                        return  "更新数据库成功！";
                    }
                }
                else
                {
                    return "不存在用于更新操作的数据,请重新检索！";
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}