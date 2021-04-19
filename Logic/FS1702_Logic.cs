using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Logic
{
    public class FS1702_Logic
    {
        FS1702_DataAccess fs1702_DataAccess;

        public FS1702_Logic()
        {
            fs1702_DataAccess = new FS1702_DataAccess();
        }

        #region 绑定工程
        public DataTable getAllProject()
        {
            return fs1702_DataAccess.getAllProject();
        }
        #endregion

        #region 检索
        public DataTable Search(string vcProject, string dChuHeDateFrom, string dChuHeDateTo)
        {
            return fs1702_DataAccess.Search(vcProject, dChuHeDateFrom, dChuHeDateTo);
        }
        public DataTable Search_jinji(string vcPart_id)
        {
            return fs1702_DataAccess.Search_jinji(vcPart_id);
        }
        public DataTable Search_kb()
        {
            return fs1702_DataAccess.Search_kb();
        }
        #endregion

        #region 按用户文件格式读取数据
        public DataTable ExcelToDataTableformRows(string FileFullName, string sheetName, ref string RetMsg)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
            RetMsg = "";
            int startRow = 0;

            try
            {
                fs = new FileStream(FileFullName, FileMode.Open, FileAccess.Read);

                if (FileFullName.IndexOf(".xlsx") > 0 || FileFullName.IndexOf(".xlsm") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (FileFullName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    //创建Datatable的列
                    for (int i = 0; i < cellCount; i++)
                    {
                        if (i == 0)
                            data.Columns.Add("vcProject");
                        else if (i == 1)
                            data.Columns.Add("vcPart_id");
                        else
                        {
                            ICell temp = firstRow.GetCell(i);
                            if (temp != null) //同理，没有数据的单元格都默认是null
                            {
                                switch (temp.CellType)
                                {
                                    case CellType.Blank:
                                        data.Columns.Add(string.Empty);
                                        break;
                                    case CellType.Numeric:
                                        //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                        if (HSSFDateUtil.IsCellDateFormatted(temp))//日期类型
                                        {
                                            var value = temp.NumericCellValue;
                                            data.Columns.Add(DateTime.FromOADate(value).ToString("yyyy-MM-dd"));
                                        }
                                        else//其他数字类型
                                        {
                                            data.Columns.Add(temp.NumericCellValue.ToString());
                                        }
                                        break;
                                    case CellType.String:
                                        data.Columns.Add(temp.StringCellValue);
                                        break;
                                    default:
                                        data.Columns.Add(temp.ToString());
                                        break;
                                }
                            }
                        }
                    }

                    //获取数据首尾行
                    startRow = sheet.FirstRowNum + 1;
                    int rowCount = sheet.LastRowNum;

                    //读取数据
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Blank:
                                        dataRow[j] = string.Empty;
                                        break;
                                    case CellType.Numeric:
                                        //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                        if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                                        {
                                            var value = cell.NumericCellValue;
                                            dataRow[j] = DateTime.FromOADate(value).ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                        else//其他数字类型
                                        {
                                            dataRow[j] = cell.NumericCellValue;
                                        }
                                        break;
                                    case CellType.String:
                                        dataRow[j] = cell.StringCellValue;
                                        break;
                                    default:
                                        dataRow[j] = cell.ToString();
                                        break;
                                }
                            }
                        }

                        data.Rows.Add(dataRow);
                    }
                }

                for (int i = 0; i < data.Columns.Count; i++)
                {
                    data.Columns[i].DataType = typeof(string);
                }
                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorName)
        {
            fs1702_DataAccess.importSave(dt, strUserId, ref strErrorName);
        }
        #endregion

        #region 确认单打印
        public void qrdPrint(List<Dictionary<string, Object>> checkedInfoData, string strUserId, DataTable dtBJW, DataTable dtBJWHistory,DataTable dtSub)
        {
            //更新确认单打印时间
            fs1702_DataAccess.qrdPrint(checkedInfoData, strUserId, dtBJW, dtBJWHistory,dtSub);
        }
        #endregion

        #region 出荷看板打印
        public void kbPrint(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            //更新出荷看板打印时间
            fs1702_DataAccess.kbPrint(checkedInfoData, strUserId);
        }
        #endregion

        #region 出荷完了
        public void chuheOK(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            //更新出荷完了时间+更新在库
            fs1702_DataAccess.chuheOK(checkedInfoData, strUserId);
        }
        #endregion

        public DataTable GetqrdInfo(string vcProject, string dChuHeDate)
        {
            return fs1702_DataAccess.GetqrdInfo(vcProject, dChuHeDate);
        }
        public DataTable getKBData(string vcProject, string dChuHeDate)
        {
            return fs1702_DataAccess.getKBData(vcProject, dChuHeDate);
        }
        public DataTable getKBData(string vcpart_id)
        {
            return fs1702_DataAccess.getKBData(vcpart_id);
        }

        public string generateExcelWithXlt(string vcQueRenNo, DataTable dt, string[] field, string rootPath, string xltName, int sheetindex, int startRow, string strUserId, string strFunctionName)
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

                ISheet sheet = hssfworkbook.GetSheetAt(sheetindex);

                ICellStyle mystyle_querenno = hssfworkbook.CreateCellStyle();
                IFont f = hssfworkbook.CreateFont();
                f.Boldweight = (short)FontBoldWeight.Bold;
                f.FontHeightInPoints = 12;
                mystyle_querenno.SetFont(f);
                mystyle_querenno.Alignment = HorizontalAlignment.Left;
                mystyle_querenno.VerticalAlignment = VerticalAlignment.Center;
                sheet.GetRow(1).GetCell(0).CellStyle = mystyle_querenno;
                sheet.GetRow(1).GetCell(0).SetCellValue("确认单号：" + vcQueRenNo);

                ICellStyle mystyle = hssfworkbook.CreateCellStyle();
                mystyle.BorderBottom = BorderStyle.Thin;
                mystyle.BorderLeft = BorderStyle.Thin;
                mystyle.BorderTop = BorderStyle.Thin;
                mystyle.BorderRight = BorderStyle.Thin;
                mystyle.Alignment = HorizontalAlignment.Center;
                mystyle.VerticalAlignment = VerticalAlignment.Center;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    row.HeightInPoints = 25;
                    for (int j = 0; j < field.Length; j++)
                    {
                        Type type = dt.Columns[field[j]].DataType;
                        ICell cell = row.CreateCell(j);
                        cell.CellStyle = mystyle;
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dt.Rows[i][field[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                        }
                    }
                }
                ISheet sheet1 = hssfworkbook.GetSheetAt(0);//刷新第一个sheet页的公式
                sheet1.ForceFormulaRecalculation = true;
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
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        #region 保存
        public void Save_jinji(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1702_DataAccess.Save_jinji(listInfoData, strUserId);
        }
        public void Save_kb(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1702_DataAccess.Save_kb(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del_jinji(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1702_DataAccess.Del_jinji(checkedInfoData, strUserId);
        }
        public void Del_kb(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1702_DataAccess.Del_kb(checkedInfoData, strUserId);
        }
        #endregion

        public bool isExitInSSP(string vcPart_id)
        {
            int count = fs1702_DataAccess.isExitInSSP(vcPart_id);
            if (count > 0)
                return true;
            else
                return false;
        }

        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "MES")
            {
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }
        
        public bool getPrintInfo(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage,
            ref DataTable dtBJW, ref DataTable dtBJWHistory,ref DataTable dtSub)
        {
            try
            {
                dtBJW.Columns.Add("vcPart_id");
                dtBJW.Columns.Add("iRemain");
                dtBJWHistory.Columns.Add("vcPart_id");
                dtBJWHistory.Columns.Add("iA");
                dtBJWHistory.Columns.Add("iB");
                dtBJWHistory.Columns.Add("iC");
                DataTable dataTable = fs1702_DataAccess.getPrintTemp("FS1702");
                dtSub = dataTable.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string vcQueRenNo = listInfoData[i]["vcQueRenNo"] == null ? "" : listInfoData[i]["vcQueRenNo"].ToString();
                    string vcProject = listInfoData[i]["vcProject"] == null ? "" : listInfoData[i]["vcProject"].ToString();
                    string dChuHeDate = listInfoData[i]["dChuHeDate"] == null ? "" : listInfoData[i]["dChuHeDate"].ToString();
                    string QueRenPrintFlag = listInfoData[i]["QueRenPrintFlag"].ToString();
                    if (QueRenPrintFlag == "√")
                    {//确认单已经打印过，直接取之前生成好的结果 
                        DataTable dtPrintInfo = fs1702_DataAccess.GetqrdPrintInfo(vcQueRenNo);
                        string uuid = Guid.NewGuid().ToString("N");
                        for (int j = 0; j < dtPrintInfo.Rows.Count; j++)
                        {
                            DataRow dataRow = dtSub.NewRow();
                            dataRow["UUID"] = uuid;
                            dataRow["id"] = dtPrintInfo.Rows[j]["id"].ToString();
                            dataRow["vcPart_id"] = dtPrintInfo.Rows[j]["vcPart_id"].ToString();
                            dataRow["vcBackPart_id"] = dtPrintInfo.Rows[j]["vcBackPart_id"].ToString();
                            dataRow["iQuantity"] = dtPrintInfo.Rows[j]["iQuantity"].ToString();
                            dataRow["vcQueRenNo"] = vcQueRenNo;
                            dtSub.Rows.Add(dataRow);
                        }
                    }
                    else
                    {
                        DataTable dtInfo = fs1702_DataAccess.GetqrdInfo(vcProject, dChuHeDate);
                        string uuid = Guid.NewGuid().ToString("N");
                        for (int j = 0; j < dtInfo.Rows.Count; j++)
                        {
                            DataRow dataRow = dtSub.NewRow();
                            dataRow["UUID"] = uuid;
                            dataRow["id"] = dtInfo.Rows[j]["id"].ToString();
                            dataRow["vcPart_id"] = dtInfo.Rows[j]["vcPart_id"].ToString();
                            dataRow["vcBackPart_id"] = dtInfo.Rows[j]["vcBackPart_id"].ToString();
                            if (vcProject == "BJW")
                            {
                                //dataRow["iQuantity"] = ReturnQuantity();
                                string strbiyaoshu = dtInfo.Rows[j]["iQuantity"].ToString();
                                string strCapacity = dtInfo.Rows[j]["iCapacity"].ToString();
                                string strRemain = dtInfo.Rows[j]["iRemain"].ToString();
                                if (strbiyaoshu == "" || strCapacity == "" || strRemain == "")
                                {
                                    DataRow row = dtMessage.NewRow();
                                    row["vcMessage"] = dtInfo.Rows[j]["vcPart_id"].ToString() + "  没有必要数/余量/收容数，请先维护。";
                                    dtMessage.Rows.Add(row);
                                }
                                else
                                {
                                    int ibiyaoshu = Convert.ToInt32(strbiyaoshu);
                                    int iCapacity = Convert.ToInt32(strCapacity);
                                    int iRemain = Convert.ToInt32(strRemain);
                                    if (iRemain - ibiyaoshu >= 0)
                                    {
                                        //插入BJW余量历史表 iRemain ibiyaoshu
                                        DataRow drBJW_History = dtBJWHistory.NewRow();
                                        drBJW_History["vcPart_id"] = dtInfo.Rows[j]["vcPart_id"].ToString();
                                        drBJW_History["iA"] = iRemain;
                                        drBJW_History["iB"] = ibiyaoshu;
                                        drBJW_History["iC"] = iRemain - ibiyaoshu;
                                        dtBJWHistory.Rows.Add(drBJW_History);
                                        //更新余量表 iRemain-ibiyaoshu
                                        DataRow drBJW = dtBJW.NewRow();
                                        drBJW["vcPart_id"] = dtInfo.Rows[j]["vcPart_id"].ToString();
                                        drBJW["iRemain"] = iRemain - ibiyaoshu;
                                        dtBJW.Rows.Add(drBJW);
                                        //更新确认单数量
                                        dataRow["iQuantity"] = 0;
                                    }
                                    else
                                    {
                                        int iCapacityAndiRemain = iCapacity + iRemain;
                                        while (iCapacityAndiRemain - ibiyaoshu < 0)
                                        {
                                            iCapacityAndiRemain += iCapacity;
                                        }
                                        //插入BJW余量历史表 iCapacityAndiRemain ibiyaoshu
                                        DataRow drBJW_History = dtBJWHistory.NewRow();
                                        drBJW_History["vcPart_id"] = dtInfo.Rows[j]["vcPart_id"].ToString();
                                        drBJW_History["iA"] = iCapacityAndiRemain;
                                        drBJW_History["iB"] = ibiyaoshu;
                                        drBJW_History["iC"] = iCapacityAndiRemain - ibiyaoshu;
                                        dtBJWHistory.Rows.Add(drBJW_History);
                                        //更新余量表 iCapacityAndiRemain-ibiyaoshu
                                        DataRow drBJW = dtBJW.NewRow();
                                        drBJW["vcPart_id"] = dtInfo.Rows[j]["vcPart_id"].ToString();
                                        drBJW["iRemain"] = iCapacityAndiRemain - ibiyaoshu;
                                        dtBJW.Rows.Add(drBJW);
                                        //更新确认单数量
                                        dataRow["iQuantity"] = iCapacityAndiRemain - iRemain;
                                    }
                                }
                            }
                            else
                                dataRow["iQuantity"] = dtInfo.Rows[j]["iQuantity"].ToString();
                            dataRow["vcQueRenNo"] = vcQueRenNo;
                            dtSub.Rows.Add(dataRow);
                        }
                    }
                }
                if (dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的确认单数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs1702_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool getPrintInfo_kb(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dt_detail = fs1702_DataAccess.getPrintTemp("FS1702_kb_detail");
                DataTable dt_main = fs1702_DataAccess.getPrintTemp("FS1702_kb_main");
                DataTable dtSub_detail = dt_detail.Clone();
                DataTable dtSub_main = dt_main.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string vcQueRenNo = listInfoData[i]["vcQueRenNo"] == null ? "" : listInfoData[i]["vcQueRenNo"].ToString();
                    string vcProject = listInfoData[i]["vcProject"] == null ? "" : listInfoData[i]["vcProject"].ToString();
                    string dChuHeDate = listInfoData[i]["dChuHeDate"] == null ? "" : listInfoData[i]["dChuHeDate"].ToString();
                    DataTable dtInfo = fs1702_DataAccess.getKBData(vcProject, dChuHeDate);
                    for (int j = 0; j < dtInfo.Rows.Count; j++)
                    {
                        string uuid = Guid.NewGuid().ToString("N");
                        DataRow dataRow = dtSub_detail.NewRow();
                        dataRow["UUID"] = uuid;
                        dataRow["vcCarType"] = dtInfo.Rows[j]["vcCarType"].ToString();
                        dataRow["vcProject"] = dtInfo.Rows[j]["vcProject"].ToString();
                        dataRow["vcProjectPlace"] = dtInfo.Rows[j]["vcProjectPlace"].ToString();
                        dataRow["vcSR"] = dtInfo.Rows[j]["vcSR"].ToString();
                        dataRow["vcBackPart_id"] = dtInfo.Rows[j]["vcBackPart_id"].ToString();
                        dataRow["vcChuHePart_id"] = dtInfo.Rows[j]["vcChuHePart_id"].ToString();
                        dataRow["vcPart_Name"] = dtInfo.Rows[j]["vcPart_Name"].ToString();
                        dataRow["iCapacity"] = dtInfo.Rows[j]["iCapacity"].ToString();
                        dataRow["vcBoxType"] = dtInfo.Rows[j]["vcBoxType"].ToString();
                        dataRow["vcSupplierName"] = dtInfo.Rows[j]["vcSupplierName"].ToString();
                        dtSub_detail.Rows.Add(dataRow);

                    }
                }
                for (int i = 0; i < dtSub_detail.Rows.Count; i = i + 4)
                {
                    string UUID1 = "";
                    string UUID2 = "";
                    string UUID3 = "";
                    string UUID4 = "";
                    if (i < dtSub_detail.Rows.Count)
                    {
                        if (i + 1 < dtSub_detail.Rows.Count)
                        {
                            if (i + 3 < dtSub_detail.Rows.Count)
                            {
                                UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub_detail.Rows[i + 1]["UUID"].ToString();
                                UUID3 = dtSub_detail.Rows[i + 2]["UUID"].ToString();
                                UUID4 = dtSub_detail.Rows[i + 3]["UUID"].ToString();
                            }
                            else if (i + 2 < dtSub_detail.Rows.Count)
                            {
                                UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub_detail.Rows[i + 1]["UUID"].ToString();
                                UUID3 = dtSub_detail.Rows[i + 2]["UUID"].ToString();
                            }
                            else
                            {
                                UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub_detail.Rows[i + 1]["UUID"].ToString();
                            }
                        }
                        else
                        {
                            UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                        }
                    }
                    DataRow dataRow = dtSub_main.NewRow();
                    dataRow["UUID1"] = UUID1;
                    dataRow["UUID2"] = UUID2;
                    dataRow["UUID3"] = UUID3;
                    dataRow["UUID4"] = UUID4;
                    dtSub_main.Rows.Add(dataRow);
                }

                if (dtSub_detail.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的看板数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs1702_DataAccess.setPrintTemp_kb_detail(dtSub_main, dtSub_detail, strOperId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool getPrintInfo_kb_zfx(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dt_detail = fs1702_DataAccess.getPrintTemp("FS1702_kb_detail");
                DataTable dt_main = fs1702_DataAccess.getPrintTemp("FS1702_kb_main");
                DataTable dtSub_detail = dt_detail.Clone();
                DataTable dtSub_main = dt_main.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string vcPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                    DataTable dtInfo = fs1702_DataAccess.getKBData(vcPart_id);
                    for (int j = 0; j < dtInfo.Rows.Count; j++)
                    {
                        string uuid = Guid.NewGuid().ToString("N");
                        DataRow dataRow = dtSub_detail.NewRow();
                        dataRow["UUID"] = uuid;
                        dataRow["vcCarType"] = dtInfo.Rows[j]["vcCarType"].ToString();
                        dataRow["vcProject"] = dtInfo.Rows[j]["vcProject"].ToString();
                        dataRow["vcProjectPlace"] = dtInfo.Rows[j]["vcProjectPlace"].ToString();
                        dataRow["vcSR"] = dtInfo.Rows[j]["vcSR"].ToString();
                        dataRow["vcBackPart_id"] = dtInfo.Rows[j]["vcBackPart_id"].ToString();
                        dataRow["vcChuHePart_id"] = dtInfo.Rows[j]["vcChuHePart_id"].ToString();
                        dataRow["vcPart_Name"] = dtInfo.Rows[j]["vcPart_Name"].ToString();
                        dataRow["iCapacity"] = dtInfo.Rows[j]["iCapacity"].ToString();
                        dataRow["vcBoxType"] = dtInfo.Rows[j]["vcBoxType"].ToString();
                        dataRow["vcSupplierName"] = dtInfo.Rows[j]["vcSupplierName"].ToString();
                        dtSub_detail.Rows.Add(dataRow);

                    }
                }
                for (int i = 0; i < dtSub_detail.Rows.Count; i = i + 4)
                {
                    string UUID1 = "";
                    string UUID2 = "";
                    string UUID3 = "";
                    string UUID4 = "";
                    if (i < dtSub_detail.Rows.Count)
                    {
                        if (i + 1 < dtSub_detail.Rows.Count)
                        {
                            if (i + 3 < dtSub_detail.Rows.Count)
                            {
                                UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub_detail.Rows[i + 1]["UUID"].ToString();
                                UUID3 = dtSub_detail.Rows[i + 2]["UUID"].ToString();
                                UUID4 = dtSub_detail.Rows[i + 3]["UUID"].ToString();
                            }
                            else if (i + 2 < dtSub_detail.Rows.Count)
                            {
                                UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub_detail.Rows[i + 1]["UUID"].ToString();
                                UUID3 = dtSub_detail.Rows[i + 2]["UUID"].ToString();
                            }
                            else
                            {
                                UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub_detail.Rows[i + 1]["UUID"].ToString();
                            }
                        }
                        else
                        {
                            UUID1 = dtSub_detail.Rows[i]["UUID"].ToString();
                        }
                    }
                    DataRow dataRow = dtSub_main.NewRow();
                    dataRow["UUID1"] = UUID1;
                    dataRow["UUID2"] = UUID2;
                    dataRow["UUID3"] = UUID3;
                    dataRow["UUID4"] = UUID4;
                    dtSub_main.Rows.Add(dataRow);
                }

                if (dtSub_detail.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的看板数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs1702_DataAccess.setPrintTemp_kb_detail(dtSub_main, dtSub_detail, strOperId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
