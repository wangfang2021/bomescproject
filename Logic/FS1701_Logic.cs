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
    public class FS1701_Logic
    {
        FS1701_DataAccess fs1701_DataAccess;

        public FS1701_Logic()
        {
            fs1701_DataAccess = new FS1701_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcIsDQ, string vcTicketNo, string vcLJNo, string vcOldOrderNo)
        {
            return fs1701_DataAccess.Search(vcIsDQ, vcTicketNo, vcLJNo, vcOldOrderNo);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1701_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1701_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataSet ds, string strUserId)
        {
            fs1701_DataAccess.importSave_Sub(ds, strUserId);
        }
        #endregion

        #region 按用户文件格式读取数据
        public DataTable ExcelToDataTable(string FileFullName, string sheetName, ref string RetMsg,ref string strTickNo)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
            RetMsg = "";
            int startRow = 0;
            int startCol = 0;

            try
            {
                fs = new FileStream(FileFullName, FileMode.Open, FileAccess.Read);

                if (FileFullName.ToLower().IndexOf(".xlsx") > 0 || FileFullName.ToLower().IndexOf(".xlsm") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (FileFullName.ToLower().IndexOf(".xls") > 0) // 2003版本
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
                    strTickNo = sheet.GetRow(7).GetCell(34).StringCellValue;//票号
                    data.Columns.Add("vcLJNo");
                    data.Columns.Add("vcLJName");
                    data.Columns.Add("vcCarType");
                    data.Columns.Add("vcOldOrderNo");
                    data.Columns.Add("vcUnit");
                    data.Columns.Add("iQuantity");
                    data.Columns.Add("decPrice");
                    data.Columns.Add("decMoney");
                    data.Columns.Add("vcIsDQ");
                    startRow = 19;
                    int startRowCopy = 19;
                    startCol = 7;
                    int rowCount = 0;
                    int colCount = 9;
                    bool read = true;
                    while(read)
                    {
                        ICell cellLJNo = sheet.GetRow(startRowCopy).GetCell(7);
                        string strLJNo= cellLJNo==null?"": sheet.GetRow(startRowCopy).GetCell(7).StringCellValue;
                        if (strLJNo != "")
                        {
                            rowCount++;
                            startRowCopy++;
                        }
                        else
                        {
                            read = false;
                            break;
                        }
                    }
                    index.Add(7);//零件号码 0
                    index.Add(13);//零件名称 1
                    index.Add(17);//车型 2
                    index.Add(18);//原订单号 3
                    index.Add(21);//单位 4
                    index.Add(23);//数量 5
                    index.Add(25);//价格 6
                    index.Add(30);//金额 7
                    index.Add(37);//是否到齐 8
                    //读取数据
                    for (int i = startRow; i < startRow+rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = 0; j < colCount; j++)
                        {
                            ICell cell = row.GetCell(index[j]);
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
                throw ex;
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

        public bool isImport(string strTickNo)
        {
            int num = fs1701_DataAccess.isImport(strTickNo);
            if (num > 0)
                return true;
            else
                return false;
        }

        public bool getPrintInfo_kb(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage,ref DataSet dsyushu)
        {
            try
            {
                FS1702_DataAccess fs1702_DataAccess = new FS1702_DataAccess();

                DataTable dt_detail = fs1702_DataAccess.getPrintTemp("FS1702_kb_detail");
                DataTable dt_main = fs1702_DataAccess.getPrintTemp("FS1702_kb_main");
                DataTable dtSub_detail = dt_detail.Clone();
                DataTable dtSub_main = dt_main.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string iAutoId = listInfoData[i]["iAutoId"] == null ? "" : listInfoData[i]["iAutoId"].ToString();
                    DataSet ds = fs1701_DataAccess.getKBData(iAutoId);
                    DataTable dtInfo = ds.Tables[0];
                    dsyushu.Tables.Add(ds.Tables[1].Copy());
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

        #region 纳入看板打印
        public void kbPrint(List<Dictionary<string, Object>> checkedInfoData, string strUserId,DataSet dsyushu)
        {
            //更新纳入看板打印时间
            fs1701_DataAccess.kbPrint(checkedInfoData, strUserId,dsyushu);
        }
        #endregion
    }
}
