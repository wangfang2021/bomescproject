using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Common;
using DataAccess;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DataTable = System.Data.DataTable;

namespace Logic
{
    public class FS0201_Logic
    {
        FS0201_DataAccess fs0201_DataAccess = new FS0201_DataAccess();

        #region 检索SPI
        public DataTable searchApi(string vcSPINO, string vcPart_Id, string vcCarType, string State)
        {
            DataTable dt = fs0201_DataAccess.searchApi(vcSPINO, vcPart_Id, vcCarType);
            if (dt.Rows.Count > 0)
            {
                dt = RulesCheck(dt);
            }

            if (!string.IsNullOrWhiteSpace(State))
            {
                DataRow[] dr = dt.Select("State = '" + State + "'");
                if (dr.Length > 0)
                {
                    DataTable res = dr.CopyToDataTable();
                    return res;
                }
                return new DataTable();
            }

            return dt;

        }
        #endregion

        #region 传送
        //传送
        public bool transferApi(string userId)
        {
            try
            {
                DataTable dt = fs0201_DataAccess.searchApi("", "", "");

                //检测NG状态
                //if (Check(dt))
                //{
                //    return false;
                //}

                dt.Columns.Add("vcCarType");
                dt.Columns.Add("vcFileNameTJ");
                string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                string suffix = "_SPI" + time;
                //添加车种担当
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["vcCarType"] = getCarType(dt.Rows[i]["vcSPINo"].ToString().Trim(), dt.Rows[i]["vcCZYD"].ToString().Trim());
                }
                //添加文件名
                DataTable dtFileName = dt.Clone();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string oldPro = dt.Rows[i]["vcOldProj"].ToString();
                    string newPro = dt.Rows[i]["vcNewProj"].ToString();
                    List<string> fileName = this.fileName(oldPro, newPro);

                    if (fileName.Count > 0)
                    {
                        for (int j = 0; j < fileName.Count; j++)
                        {
                            DataRow dr = dtFileName.NewRow();
                            dr.ItemArray = dt.Rows[i].ItemArray;
                            dr["vcFileNameTJ"] = fileName[j] + suffix;
                            dtFileName.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        DataRow dr = dtFileName.NewRow();
                        dr.ItemArray = dt.Rows[i].ItemArray;
                        dtFileName.Rows.Add(dr);
                    }
                }
                //排除文件名为空的项
                DataTable dtImport = dtFileName.Clone();
                for (int i = 0; i < dtFileName.Rows.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(dtFileName.Rows[i]["vcFileNameTJ"].ToString()))
                    {
                        dtImport.ImportRow(dtFileName.Rows[i]);
                    }
                }
                fs0201_DataAccess.transferSB(dtImport, userId);
                //传入设变
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //判断是否全部为OK
        public bool Check(DataTable dt)
        {
            bool flag = false;
            DataTable dtR = RulesCheck(dt);
            for (int i = 0; i < dtR.Rows.Count; i++)
            {
                if (dtR.Rows[i]["State"].ToString() == "1")
                {
                    flag = true;
                }
            }

            return flag;
        }
        // 获取车种
        public string getCarType(string SPINO, string CZYD)
        {
            string carType = "";
            if (SPINO.Length >= 4)
            {
                SPINO = SPINO.Substring(0, 4);
                if (SPINO[3] == 'w' || SPINO[3] == 'W')
                {
                    carType = SPINO;
                }
            }
            if (CZYD.Length >= 4)
            {
                CZYD = CZYD.Substring(0, 4);
                if ((CZYD[3] == 'w' || CZYD[3] == 'W') && !CZYD.Equals(SPINO))
                {
                    if (!string.IsNullOrWhiteSpace(carType))
                    {
                        carType += "/";
                    }

                    carType += CZYD;
                }
            }
            return carType;
        }
        // 获取文件名
        public List<string> fileName(string oldPro, string newPro)
        {
            List<string> resList = new List<string>();

            if (!string.IsNullOrWhiteSpace(oldPro) && (oldPro.Contains("WB") || oldPro.Contains("WF") || oldPro.Contains("WL") || oldPro.Contains("WD")))
            {
                if (oldPro.Contains("WD"))
                {
                    resList.Add("WB");
                    resList.Add("WF");
                    resList.Add("WL");
                }
                else
                {
                    if (oldPro.Contains("WB"))
                    {
                        resList.Add("WB");
                    }
                    else if (oldPro.Contains("WF"))
                    {
                        resList.Add("WF");

                    }
                    else if (oldPro.Contains("WL"))
                    {
                        resList.Add("WL");

                    }
                }

            }
            if (!string.IsNullOrWhiteSpace(newPro) && (newPro.Contains("WB") || newPro.Contains("WF") || newPro.Contains("WL") || newPro.Contains("WD")))
            {
                if (newPro.Contains("WD"))
                {
                    resList.Add("WB");
                    resList.Add("WF");
                    resList.Add("WL");
                }
                else
                {

                    if (newPro.Contains("WB"))
                    {
                        resList.Add("WB");
                    }
                    else if (newPro.Contains("WF"))
                    {
                        resList.Add("WF");

                    }
                    else if (newPro.Contains("WL"))
                    {
                        resList.Add("WL");

                    }
                }

            }

            resList = resList.Distinct().ToList();

            return resList;
        }
        #endregion

        #region 删除
        public void delSPI(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0201_DataAccess.delSPI(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0201_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 状态判断
        public DataTable RulesCheck(DataTable dt)
        {
            dt.Columns.Add("State");
            dt.Columns.Add("ErrorInfo");
            dt.Columns.Add("PartError");
            dt.Columns.Add("BJDiffError");
            dt.Columns.Add("DTDiffError");
            dt.Columns.Add("DTPartError");
            dt.Columns.Add("PartNameError");
            dt.Columns.Add("FXError");
            dt.Columns.Add("FXNoError");
            dt.Columns.Add("ChangeError");
            dt.Columns.Add("NewProjError");
            //判断画面显示样式
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"].ToString();
                string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"].ToString();
                string vcBJDiff = dt.Rows[i]["vcBJDiff"].ToString();
                string vcDTDiff = dt.Rows[i]["vcDTDiff"].ToString();
                string vcPart_id_DT = dt.Rows[i]["vcPart_id_DT"].ToString();
                string vcPartName = dt.Rows[i]["vcPartName"].ToString();
                string vcFXDiff = dt.Rows[i]["vcFXDiff"].ToString();
                string vcFXNo = dt.Rows[i]["vcFXNo"].ToString();
                string vcChange = dt.Rows[i]["vcChange"].ToString();
                string vcNewProj = dt.Rows[i]["vcNewProj"].ToString();

                int State = 0;
                string ErrorInfo = "";
                int PartError = 0;
                int BJDiffError = 0;
                int DTDiffError = 0;
                int DTPartError = 0;
                int PartNameError = 0;
                int FXError = 0;
                int FXNoError = 0;
                int ChangeError = 0;
                int NewProjError = 0;
                List<string> errorlist = new List<string>();
                //新旧品番都为空
                if ((string.IsNullOrWhiteSpace(vcPart_Id_old) && string.IsNullOrWhiteSpace(vcPart_Id_new)) || (!string.IsNullOrWhiteSpace(vcPart_Id_old) && !string.IsNullOrWhiteSpace(vcPart_Id_new)))
                {
                    PartError = 1;
                    errorlist.Add("新旧品番必填一项");
                }
                //补给区分为空
                if (string.IsNullOrWhiteSpace(vcBJDiff))
                {
                    BJDiffError = 1;
                    errorlist.Add("补给区分必填");

                }
                //代替区分为空
                if (string.IsNullOrWhiteSpace(vcDTDiff))
                {
                    DTDiffError = 1;
                    errorlist.Add("代替区分必填");
                }
                //代替区分为HD/NR时代替品番为空
                if ((vcDTDiff.Contains("HD") || vcDTDiff.Contains("NR")) && string.IsNullOrWhiteSpace(vcPart_id_DT))
                {
                    DTPartError = 1;
                    errorlist.Add("代替区分为HD/NR时,代替品番必填");
                }
                //品名为空
                if (string.IsNullOrWhiteSpace(vcPartName))
                {
                    PartNameError = 1;
                    errorlist.Add("品名必填");
                }
                //防锈区分为空
                if (string.IsNullOrWhiteSpace(vcFXDiff))
                {
                    FXError = 1;
                    errorlist.Add("防锈区分必填");
                }
                //防锈区分为R时，防锈指示书No为空
                if (vcFXDiff.Equals("R") && string.IsNullOrWhiteSpace(vcFXNo))
                {
                    FXNoError = 1;
                    errorlist.Add("防锈区分为R时，防锈指示书No必填");
                }
                //变更事项为空
                if (string.IsNullOrWhiteSpace(vcChange))
                {
                    ChangeError = 1;
                    errorlist.Add("变更事项必填");

                }
                //变更事项为新设，新工程为空
                if ((vcChange.Contains("新設") || vcChange.Contains("新设")) && string.IsNullOrWhiteSpace(vcNewProj))
                {
                    NewProjError = 1;
                    errorlist.Add("变更事项为新设时，新工程必填");
                }
                //变更事项为新设，旧品番不为空
                if ((vcChange.Contains("新設") || vcChange.Contains("新设")) && (!string.IsNullOrWhiteSpace(vcPart_Id_old)))
                {
                    PartError = 1;
                    errorlist.Add("变更事项为新设时，旧品番必须为空");
                }

                //汇总错误列表
                if (errorlist.Count > 0)
                {
                    State = 1;
                    for (int j = 0; j < errorlist.Count; j++)
                    {
                        ErrorInfo += errorlist[j] + ";";
                    }
                }

                dt.Rows[i]["State"] = State;
                dt.Rows[i]["ErrorInfo"] = ErrorInfo;
                dt.Rows[i]["PartError"] = PartError;
                dt.Rows[i]["BJDiffError"] = BJDiffError;
                dt.Rows[i]["DTDiffError"] = DTDiffError;
                dt.Rows[i]["DTPartError"] = DTPartError;
                dt.Rows[i]["PartNameError"] = PartNameError;
                dt.Rows[i]["FXError"] = FXError;
                dt.Rows[i]["FXNoError"] = FXNoError;
                dt.Rows[i]["ChangeError"] = ChangeError;
                dt.Rows[i]["NewProjError"] = NewProjError;
            }

            return dt;
        }
        #endregion

        #region 导入

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0201_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region SPI导入

        public void importSPI(string fileSavePath, string strUserId, ref string reMsg)
        {
            try
            {
                //获取路径下文件名
                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);

                string fileName = "Result_" + System.Guid.NewGuid().ToString("N") + ".xlsm";
                string pLocalFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + "FS0201.xlsm";//要复制的文件路径
                string pSaveFilePath = fileSavePath + Path.DirectorySeparatorChar + fileName;//指定存储的路径
                //复制模板文件
                if (File.Exists(pLocalFilePath))//必须判断要复制的文件是否存在
                {
                    File.Copy(pLocalFilePath, pSaveFilePath, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
                }

                CreateList(fileSavePath, fileName);

                //TODO 复制该文件夹至远程服务器

                //

                //TODO 将已上传文件名存储至lock表

                //
                string[,] header =
                {
                    {
                        "SPI No", "旧品番", "新品番", "補給区分（新）", "代替区分", "代替品番（新）", "品名", "品番実施時期(新/ｶﾗ)", "防錆区分", "防錆指示書№（新）",
                        "変更事項", "旧工程", "工程実施時期旧/ﾏﾃﾞ", "新工程", "工程実施時期新/ｶﾗ", "工程参照引当(直上品番)(新)", "処理日", "シート名", "ファイル名"
                    },
                    {
                        "vcSPINo", "vcPart_Id_old", "vcPart_Id_new", "vcBJDiff", "vcDTDiff", "vcPart_id_DT",
                        "vcPartName", "vcStartYearMonth", "vcFXDiff", "vcFXNo", "vcChange", "vcOldProj",
                        "vcOldProjTime", "vcNewProj", "vcNewProjTime", "vcCZYD", "dHandleTime", "vcSheetName",
                        "vcFileName"
                    },
                    {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
                    {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                    {"1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
                };
                DataTable dt = new DataTable();
                dt = ExcelToDataTable(fileSavePath + Path.DirectorySeparatorChar + fileName, "list", header, ref reMsg);
                if (dt.Rows.Count > 0)
                {
                    fs0201_DataAccess.importSPI(dt, strUserId);
                }
                else
                {
                    reMsg = "没有需要导入的数据。";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #endregion

        #region 上传SPI

        #region 导入
        /// <summary>
        /// 导入标准Excel，第一行为列头
        /// </summary>
        /// <param name="FileFullName">文件完整路径</param>
        /// <param name="sheetName">指定sheet页名称，如未匹配则默认选择第一个sheet</param>
        /// <param name="Header">5个数组,第一个为文件中列头，第二个为对应DataTable列头，第三个对应校验数据类型，第四个为最大长度限定（为0不校验），第五个为最小长度限定（为0不校验）</param>
        /// <param name="RetMsg">返回信息</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(string FileFullName, string sheetName, string[,] Header, ref string RetMsg)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
            RetMsg = "";
            int startRow = 4;

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
                    IRow firstRow = sheet.GetRow(startRow);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    //对应索引
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        bool bFound = false;
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = firstRow.GetCell(j);
                            string cellValue = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (Header[0, i] == cellValue)
                                {
                                    bFound = true;
                                    index.Add(j);
                                    break;
                                }
                            }
                        }

                        if (bFound == false)
                        {
                            RetMsg = Header[0, i] + "列不存在";
                            return null;
                        }
                    }

                    //创建Datatable的列
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        data.Columns.Add(Header[1, i].ToString().Trim());
                    }

                    //获取数据首尾行
                    int rowCount = sheet.LastRowNum;

                    //读取数据
                    for (int i = startRow + 1; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = 0; j < Header.GetLength(1); j++)
                        {
                            ICell cell = row.GetCell(index[j]);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {

                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dataRow[j] = DateTime.FromOADate(cell.NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = cell.ToString();
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

                #region 校验格式

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DataRow dr = data.Rows[i];
                    for (int j = 0; j < Header.GetLength(1); j++)
                    {
                        if (Convert.ToInt32(Header[3, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length > Convert.ToInt32(Header[3, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                    }
                }


                #endregion

                if (workbook != null)
                {
                    workbook.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                return data;
            }
            catch (Exception ex)
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
                return null;
            }
        }
        #endregion

        //调用宏
        public void CreateList(string path, string fileName)
        {
            Application app = new Application();
            app.Visible = false;
            string filePath = path + Path.DirectorySeparatorChar + fileName;
            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "getPath", new Object[] { path }, out objRtn);
                ComFunction.RunExcelMacro(app, "MakeList", new Object[] { }, out objRtn);
                wb.Close(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (wb != null)
                    Marshal.ReleaseComObject(wb);
                app.Quit();
                Marshal.ReleaseComObject(app);
            }

        }


        #endregion

    }
}