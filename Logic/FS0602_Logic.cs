﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using DataEntity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0602_Logic
    {
        FS0602_DataAccess fs0602_DataAccess = new FS0602_DataAccess();
        public DataTable getSearchInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
                  string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant)
        {
            string strDyInfo = "'0','1','2','3'";
            string strHyInfo = "'0','3'";
            DataTable dataTable = fs0602_DataAccess.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDyInfo, strHyInfo);
            return dataTable;
        }
        public void openPlan(List<Dictionary<string, Object>> listInfoData, dynamic dataForm, string dExpectTime, string strOperId,ref string strMessageList)
        {
            try
            {
                string strOperationType = "展开内示";
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcDyState");
                dataTable.Columns.Add("vcHyState");
                dataTable.Columns.Add("dExpectTime");
                dataTable.Columns.Add("vcYearMonth");
                dataTable.Columns.Add("vcPart_id");
                if (listInfoData == null)//按照检索条件全部提交
                {
                    string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                    string strDyState = dataForm.DyState;
                    string strHyState = dataForm.HyState;
                    string strPartId = dataForm.PartId;
                    string strCarModel = dataForm.CarModel;
                    string strInOut = dataForm.InOut;
                    string strOrderingMethod = dataForm.OrderingMethod;
                    string strOrderPlant = dataForm.OrderPlant;
                    string strHaoJiu = dataForm.HaoJiu;
                    string strSupplierId = dataForm.SupplierId;
                    string strSupplierPlant = dataForm.SupplierPlant;
                    DataTable dt = getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcDyState"] = "1";
                        dataRow["vcHyState"] = "";
                        dataRow["dExpectTime"] = dExpectTime;
                        dataRow["vcYearMonth"] = dt.Rows[i]["vcYearMonth"].ToString();
                        dataRow["vcPart_id"] = dt.Rows[i]["vcPart_id"].ToString();
                        dataTable.Rows.Add(dataRow);
                    }

                }
                else//按照勾选提交
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcDyState"] = "1";
                        dataRow["vcHyState"] = "";
                        dataRow["dExpectTime"] = dExpectTime;
                        dataRow["vcYearMonth"] = listInfoData[i]["vcYearMonth"].ToString(); ;
                        dataRow["vcPart_id"] = listInfoData[i]["vcPart_id"].ToString(); ;
                        dataTable.Rows.Add(dataRow);
                    }
                }
                if (strMessageList=="")
                {
                    fs0602_DataAccess.setSOQInfo(strOperationType, dataTable, strOperId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void replyPlan(List<Dictionary<string, Object>> listInfoData, dynamic dataForm, string strOperId, ref string strMessageList)
        {
            try
            {
                string strOperationType = "回复内示";
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcDyState");
                dataTable.Columns.Add("vcHyState");
                dataTable.Columns.Add("dExpectTime");
                dataTable.Columns.Add("vcYearMonth");
                dataTable.Columns.Add("vcPart_id");
                if (listInfoData == null)//按照检索条件全部提交
                {
                    string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                    string strDyState = dataForm.DyState;
                    string strHyState = dataForm.HyState;
                    string strPartId = dataForm.PartId;
                    string strCarModel = dataForm.CarModel;
                    string strInOut = dataForm.InOut;
                    string strOrderingMethod = dataForm.OrderingMethod;
                    string strOrderPlant = dataForm.OrderPlant;
                    string strHaoJiu = dataForm.HaoJiu;
                    string strSupplierId = dataForm.SupplierId;
                    string strSupplierPlant = dataForm.SupplierPlant;
                    DataTable dt = getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcDyState"] = "";
                        dataRow["vcHyState"] = "1";
                        dataRow["dExpectTime"] = "";
                        dataRow["vcYearMonth"] = dt.Rows[i]["vcYearMonth"].ToString();
                        dataRow["vcPart_id"] = dt.Rows[i]["vcPart_id"].ToString();
                        dataTable.Rows.Add(dataRow);
                    }

                }
                else//按照勾选提交
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcDyState"] = "";
                        dataRow["vcHyState"] = "1";
                        dataRow["dExpectTime"] = "";
                        dataRow["vcYearMonth"] = listInfoData[i]["vcYearMonth"].ToString(); ;
                        dataRow["vcPart_id"] = listInfoData[i]["vcPart_id"].ToString(); ;
                        dataTable.Rows.Add(dataRow);
                    }
                }
                if (strMessageList == "")
                {
                    fs0602_DataAccess.setSOQInfo(strOperationType, dataTable, strOperId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void returnPlan(dynamic dataForm, string strOperId, ref string strMessageList)
        {
            try
            {
                string strOperationType = "退回内示";
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcDyState");
                dataTable.Columns.Add("vcHyState");
                dataTable.Columns.Add("dExpectTime");
                dataTable.Columns.Add("vcYearMonth");
                dataTable.Columns.Add("vcPart_id");
                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                DataRow dataRow = dataTable.NewRow();
                dataRow["vcDyState"] = "";
                dataRow["vcHyState"] = "";
                dataRow["dExpectTime"] = "";
                dataRow["vcYearMonth"] = strYearMonth;
                dataRow["vcPart_id"] = "";
                dataTable.Rows.Add(dataRow);
                if (strMessageList == "")
                {
                    fs0602_DataAccess.setSOQInfo(strOperationType, dataTable, strOperId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName, string strYearMonth, string strYearMonth_2, string strYearMonth_3)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                    }
                }

                //以下业务特别处理

                int iMonth = Convert.ToInt32(strYearMonth.Substring(4, 2));//对象月
                int iMonth_2 = Convert.ToInt32(strYearMonth_2.Substring(4, 2));//内示月
                int iMonth_3 = Convert.ToInt32(strYearMonth_3.Substring(4, 2));//内内示月

                sheet.GetRow(1).GetCell(12).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(14).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(15).SetCellValue(iMonth_3 + "月");
                sheet.GetRow(1).GetCell(16).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(17).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(18).SetCellValue(iMonth_3 + "月");
                sheet.GetRow(1).GetCell(19).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(20).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(21).SetCellValue(iMonth_3 + "月");



                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string path = fileSavePath + strFileName;
                using (FileStream fs = File.OpenWrite(path))
                {
                    hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                return strFileName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            return fs0602_DataAccess.Cr(varDxny, varDyzt, varHyzt, PARTSNO);
        }
        #endregion

        #region 展开。将初版SOQ数据复制到调整后SOQ，并改变对应状态
        public int Zk(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Zk(searchForm);
        }
        #endregion

        #region 回复。改变合意状态
        public int Hf(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Hf(searchForm);
        }
        #endregion

        #region 退回内示。删除该对象月3个月所有SOQ数据，并将soq履历表中的状态改为退回。
        public int thns(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.thns(searchForm);
        }
        #endregion
    }
}
