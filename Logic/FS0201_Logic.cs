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
using System.Collections;
using DataEntity;

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
                if (Check(dt))
                {
                    //return false;
                }

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

            dt.Columns.Add("OldProjTimeError");
            dt.Columns.Add("NewProjTimeError");
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
                string vcOldProjTime = dt.Rows[i]["vcOldProjTime"].ToString();
                string vcNewProjTime = dt.Rows[i]["vcNewProjTime"].ToString();

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
                int OldProjTimeError = 0;
                int NewProjTimeError = 0;
                List<string> errorlist = new List<string>();
                if (!string.IsNullOrWhiteSpace(vcOldProjTime) && !IsDate(vcOldProjTime))
                {
                    OldProjTimeError = 1;
                    errorlist.Add("工程実施時期旧/ﾏﾃﾞ格式不正确");
                }
                if (!string.IsNullOrWhiteSpace(vcNewProjTime) && !IsDate(vcNewProjTime))
                {
                    NewProjTimeError = 1;
                    errorlist.Add("工程実施時期新/ｶﾗ格式不正确");
                }
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
                dt.Rows[i]["OldProjTimeError"] = OldProjTimeError;
                dt.Rows[i]["NewProjTimeError"] = NewProjTimeError;
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
                DataTable dt = getData(fileSavePath);
                fs0201_DataAccess.importSPI(dt, strUserId);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #endregion


        #region 读取数据
        #region  
        private int TT = 15;
        private int TS = 45;

        private int ST = 51;
        private int SS = 81;
        #endregion
        public DataTable getData(string fileSavePath)
        {
            try
            {
                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
                ArrayList list1 = new ArrayList();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    if (info.Extension.ToUpper() == ".XLSX")
                    {
                        list1.Add(fileSavePath + info.Name);
                    }
                }
                FS0201_DataEntity.Entity en = new FS0201_DataEntity.Entity();
                List<FS0201_DataEntity.Part> list = new List<FS0201_DataEntity.Part>();
                DataTable dt = new DataTable();
                DataTable tempNew = new DataTable();
                DataSet ds = new DataSet();
                List<DataRow> rows = new List<DataRow>();
                for (int i = 0; i < list1.Count; i++)
                {
                    en = ReadFromExcelFile((string)list1[i]);
                    list = ReadFromExcelFile((String)list1[i]).parts;
                    dt = ListToDataTable1(list, en);
                    //ds.Tables.Add(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        rows.Add(row);
                    }
                }
                DataTable temp1 = new DataTable();
                temp1 = ListToDataTable1(rows);

                DataRow[] arrayDR = temp1.Select("vcOldProj like '%WD%' or vcOldProj like '%WB%' or vcOldProj like '%WL%'or vcOldProj like '%WF%'OR vcNewProj like '%WD%' or vcNewProj like '%WB%' or vcNewProj like '%WL%'or vcNewProj like '%WF%'", "vcFileName");

                DataTable tmp = temp1.Clone(); // 复制DataRow的表结构
                foreach (DataRow row in arrayDR)
                {
                    tmp.ImportRow(row); // 将DataRow添加到DataTable中
                }

                DataRow[] arrDR = tmp.Select("vcChange not in('代替時期変更/Sub. Effective Period Change','工程参照引当変更/Routing Address Change','補給生産区分変更') ");
                DataTable tempTable = tmp.Clone();
                foreach (DataRow row1 in arrDR)
                {

                    tempTable.ImportRow(row1); // 将DataRow添加到DataTable中
                }

                return tempTable;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FS0201_DataEntity.Entity ReadFromExcelFile(string filePath)
        {
            FS0201_DataEntity.Entity res = new FS0201_DataEntity.Entity();
            IWorkbook wk = null;
            string extension = System.IO.Path.GetExtension(filePath);
            try
            {
                FileStream fs = File.OpenRead(filePath);

                string fileName = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1);

                if (extension.Equals(".xls"))
                {
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(fs);
                }
                else
                {
                    //把xlsx文件中的数据写入wk中
                    wk = new XSSFWorkbook(fs);
                }

                fs.Close();

                int sheetIndex = wk.NumberOfSheets;
                ISheet sheet = wk.GetSheetAt(0);

                string sheetName = sheet.SheetName;
                res.vcFileName = fileName;

                //读取当前表数据
                IRow row = sheet.GetRow(5);  //读取设变号
                string vcSPINo = row.GetCell(4).ToString();
                res.vcSPINo = vcSPINo;

                row = sheet.GetRow(4);//读取处理日
                string dHandleTime = row.GetCell(66).ToString();
                res.dHandleTime = dHandleTime;
                String prePart_idS = "";
                for (int sheetI = 0; sheetI < sheetIndex; sheetI++)
                {

                    sheet = wk.GetSheetAt(sheetI);

                    GetEntity(sheet, ref res.parts, ref prePart_idS);
                }

                return res;
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        private void GetEntity(ISheet sheet, ref List<FS0201_DataEntity.Part> temp, ref String prePart_idS)
        {

            IRow row;
            IRow row1;

            #region 获取上半部分数据

            for (int i = TT; i < TS; i += 2)
            {
                row = sheet.GetRow(i);
                row1 = sheet.GetRow(i + 1);
                //新增一条Old
                if (!string.IsNullOrWhiteSpace(row.GetCell(0).ToString()))
                {

                    int flag = 0;
                    int flagN = 1;
                    FS0201_DataEntity.Part part = new FS0201_DataEntity.Part();

                    part.vcSheetName = sheet.SheetName;

                    part.flag = flag;
                    part.vcPart_Id_old = row.GetCell(0 + flag).ToString();
                    part.BJQF = row.GetCell(6 + flag).ToString();
                    part.PFSSFrom = row.GetCell(9 + flag).ToString();
                    part.PFSSTo = row.GetCell(13 + flag).ToString();
                    part.FXQF = row.GetCell(47 + flag).ToString();
                    part.FXNO = row.GetCell(49 + flag).ToString();
                    part.vcPartName = row.GetCell(54).ToString();
                    part.vcChange = row.GetCell(63).ToString();
                    if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(part.vcChange))
                        {
                            part.vcChange += ",";
                        }
                    }
                    part.vcChange += row1.GetCell(63).ToString();
                    //-----------------------代替区分-CHENYING
                    part.DTQF = row.GetCell(18 + flag).ToString();
                    part.DTPart_Id = row.GetCell(21 + flag).ToString();
                    //-------------------------
                    /*----------------------flag+1(新)------*/
                    part.vcPart_Id_new = row.GetCell(0 + flagN).ToString();
                    part.vcBJDiff = row.GetCell(6 + flagN).ToString();
                    part.vcStartYearMonth = row.GetCell(9 + flagN).ToString();
                    part.NPFSSTo = row.GetCell(13 + flagN).ToString();
                    part.vcFXDiff = row.GetCell(47 + flagN).ToString();
                    part.vcFXNo = row.GetCell(49 + flagN).ToString();
                    part.vcDTDiff = row.GetCell(18 + flagN).ToString();
                    part.vcPart_id_DT = row.GetCell(21 + flagN).ToString();
                    /*----------------------END------*/

                    temp.Add(part);

                }
                #region 
                //新增一条New
                else if (!string.IsNullOrWhiteSpace(row.GetCell(1).ToString()))
                {

                    int flag = 1;
                    FS0201_DataEntity.Part part = new FS0201_DataEntity.Part();

                    part.vcSheetName = sheet.SheetName;

                    part.flag = flag;
                    part.vcPart_Id_new = row.GetCell(0 + flag).ToString();
                    part.vcBJDiff = row.GetCell(6 + flag).ToString();
                    part.vcStartYearMonth = row.GetCell(9 + flag).ToString();
                    part.NPFSSTo = row.GetCell(13 + flag).ToString();
                    part.vcFXDiff = row.GetCell(47 + flag).ToString();
                    part.vcFXNo = row.GetCell(49 + flag).ToString();
                    part.vcPartName = row.GetCell(54).ToString();

                    //--------------代替区分-CHENYING
                    part.DTQF = row.GetCell(18 + flag).ToString();
                    part.DTPart_Id = row.GetCell(21 + flag).ToString();
                    //-------------------------
                    part.vcChange = row.GetCell(63).ToString();
                    if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(part.vcChange))
                        {
                            part.vcChange += ",";
                        }
                    }
                    part.vcChange += row1.GetCell(63).ToString();
                    temp.Add(part);
                }
                #endregion

                //修改最后一条
                else if (!string.IsNullOrWhiteSpace(row.GetCell(6).ToString()))
                {

                    int flag = 0;
                    int flagN = 1;
                    int index = temp.Count - 1;
                    temp[index].flag = flag;

                    if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].BJQF))
                        {
                            temp[index].BJQF += ",";
                        }
                    }
                    temp[index].BJQF += row.GetCell(6 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].PFSSFrom))
                        {
                            temp[index].PFSSFrom += ",";
                        }
                    }
                    temp[index].PFSSFrom += row.GetCell(9 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].PFSSTo))
                        {
                            temp[index].PFSSTo += ",";
                        }
                    }
                    temp[index].PFSSTo += row.GetCell(13 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].FXQF))
                        {
                            temp[index].FXQF += ",";
                        }
                    }
                    temp[index].FXQF += row.GetCell(47 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].FXNO))
                        {
                            temp[index].FXNO += ",";
                        }
                    }
                    temp[index].FXNO += row.GetCell(49 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].DTQF))
                        {
                            temp[index].DTQF += ",";
                        }
                    }
                    temp[index].DTQF += row.GetCell(18 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].DTPart_Id))
                        {
                            temp[index].DTPart_Id += ",";
                        }
                    }
                    temp[index].DTPart_Id += row.GetCell(21 + flag).ToString();


                    if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                        {
                            temp[index].vcChange += ",";
                        }
                    }
                    temp[index].vcChange += row.GetCell(63).ToString();
                    if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                        {
                            temp[index].vcChange += ",";
                        }
                    }
                    temp[index].vcChange += row1.GetCell(63).ToString();

                }
                #region 修改最后一条(新)
                //修改最后一条
                else if (!string.IsNullOrWhiteSpace(row.GetCell(7).ToString()))
                {
                    int flag = 1;
                    int index = temp.Count - 1;
                    temp[index].flag = flag;
                    if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcBJDiff))
                        {
                            temp[index].vcBJDiff += ",";
                        }
                    }
                    temp[index].vcBJDiff += row.GetCell(6 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcStartYearMonth))
                        {
                            temp[index].vcStartYearMonth += ",";
                        }
                    }
                    temp[index].vcStartYearMonth += row.GetCell(9 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].NPFSSTo))
                        {
                            temp[index].NPFSSTo += ",";
                        }
                    }
                    temp[index].NPFSSTo += row.GetCell(13 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcFXDiff))
                        {
                            temp[index].vcFXDiff += ",";
                        }
                    }
                    temp[index].vcFXDiff += row.GetCell(47 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcFXNo))
                        {
                            temp[index].vcFXNo += ",";
                        }
                    }
                    temp[index].vcFXNo += row.GetCell(49 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcDTDiff))
                        {
                            temp[index].vcDTDiff += ",";
                        }
                    }
                    temp[index].vcDTDiff += row.GetCell(18 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcPart_id_DT))
                        {
                            temp[index].vcPart_id_DT += ",";
                        }
                    }
                    temp[index].vcPart_id_DT += row.GetCell(21 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                        {
                            temp[index].vcChange += ",";
                        }
                    }
                    temp[index].vcChange += row.GetCell(63).ToString();
                    if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                        {
                            temp[index].vcChange += ",";
                        }
                    }
                    temp[index].vcChange += row1.GetCell(63).ToString();

                }
                #endregion
                else if (!string.IsNullOrWhiteSpace(row.GetCell(10).ToString()))
                {
                    int flag = 1;
                    int index = temp.Count - 1;
                    temp[index].flag = flag;

                    if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcBJDiff))
                        {
                            temp[index].vcBJDiff += ",";
                        }
                    }
                    temp[index].vcBJDiff += row.GetCell(6 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcStartYearMonth))
                        {
                            temp[index].vcStartYearMonth += ",";
                        }
                    }
                    temp[index].vcStartYearMonth += row.GetCell(9 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].NPFSSTo))
                        {
                            temp[index].NPFSSTo += ",";
                        }
                    }
                    temp[index].NPFSSTo += row.GetCell(13 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcFXDiff))
                        {
                            temp[index].vcFXDiff += ",";
                        }
                    }
                    temp[index].vcFXDiff += row.GetCell(47 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcFXNo))
                        {
                            temp[index].vcFXNo += ",";
                        }
                    }
                    temp[index].vcFXNo += row.GetCell(49 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcDTDiff))
                        {
                            temp[index].vcDTDiff += ",";
                        }
                    }
                    temp[index].vcDTDiff += row.GetCell(18 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcPart_id_DT))
                        {
                            temp[index].vcPart_id_DT += ",";
                        }
                    }
                    temp[index].vcPart_id_DT += row.GetCell(21 + flag).ToString();
                    //----------
                    if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                        {
                            temp[index].vcChange += ",";
                        }
                    }
                    temp[index].vcChange += row.GetCell(63).ToString();
                    if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                        {
                            temp[index].vcChange += ",";
                        }
                    }
                    temp[index].vcChange += row1.GetCell(63).ToString();

                }
                else if (!string.IsNullOrWhiteSpace(row.GetCell(48).ToString()))
                {
                    int flag = 1;
                    int index = temp.Count - 1;
                    if (index < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(row.GetCell(0 + flag).ToString()) || !string.IsNullOrWhiteSpace(row.GetCell(0).ToString()))
                        {
                            temp[index].flag = flag;

                            if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcBJDiff))
                                {
                                    temp[index].vcBJDiff += ",";
                                }
                            }
                            temp[index].vcBJDiff += row.GetCell(6 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcStartYearMonth))
                                {
                                    temp[index].vcStartYearMonth += ",";
                                }
                            }
                            temp[index].vcStartYearMonth += row.GetCell(9 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].NPFSSTo))
                                {
                                    temp[index].NPFSSTo += ",";
                                }
                            }
                            temp[index].NPFSSTo += row.GetCell(13 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXDiff))
                                {
                                    temp[index].vcFXDiff += ",";
                                }
                            }
                            temp[index].vcFXDiff += row.GetCell(47 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXNo))
                                {
                                    temp[index].vcFXNo += ",";
                                }
                            }
                            temp[index].vcFXNo += row.GetCell(49 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcDTDiff))
                                {
                                    temp[index].vcDTDiff += ",";
                                }
                            }
                            temp[index].vcDTDiff += row.GetCell(18 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcPart_id_DT))
                                {
                                    temp[index].vcPart_id_DT += ",";
                                }
                            }
                            temp[index].vcPart_id_DT += row.GetCell(21 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row.GetCell(63).ToString();
                            if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row1.GetCell(63).ToString();

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcPart_Id_new) || !string.IsNullOrWhiteSpace(temp[index].vcPart_Id_old))
                        {
                            temp[index].flag = flag;

                            if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcBJDiff))
                                {
                                    temp[index].vcBJDiff += ",";
                                }
                            }
                            temp[index].vcBJDiff += row.GetCell(6 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcStartYearMonth))
                                {
                                    temp[index].vcStartYearMonth += ",";
                                }
                            }
                            temp[index].vcStartYearMonth += row.GetCell(9 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].NPFSSTo))
                                {
                                    temp[index].NPFSSTo += ",";
                                }
                            }
                            temp[index].NPFSSTo += row.GetCell(13 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXDiff))
                                {
                                    temp[index].vcFXDiff += ",";
                                }
                            }
                            temp[index].vcFXDiff += row.GetCell(47 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXNo))
                                {
                                    temp[index].vcFXNo += ",";
                                }
                            }
                            temp[index].vcFXNo += row.GetCell(49 + flag).ToString();
                            //----------------------代替区分
                            if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcDTDiff))
                                {
                                    temp[index].vcDTDiff += ",";
                                }
                            }
                            temp[index].vcDTDiff += row.GetCell(18 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcPart_id_DT))
                                {
                                    temp[index].vcPart_id_DT += ",";
                                }
                            }
                            temp[index].vcPart_id_DT += row.GetCell(21 + flag).ToString();
                            //-----------------------

                            if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row.GetCell(63).ToString();
                            if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row1.GetCell(63).ToString();

                        }
                    }
                }
                //修改最后一条------------CHENYING
                else if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                {
                    int flag = 1;
                    int index = temp.Count - 1;
                    if (index < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(row.GetCell(0 + flag).ToString()) || !string.IsNullOrWhiteSpace(row.GetCell(0).ToString()))
                        {
                            temp[index].flag = flag;

                            if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcBJDiff))
                                {
                                    temp[index].vcBJDiff += ",";
                                }
                            }
                            temp[index].vcBJDiff += row.GetCell(6 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcStartYearMonth))
                                {
                                    temp[index].vcStartYearMonth += ",";
                                }
                            }
                            temp[index].vcStartYearMonth += row.GetCell(9 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].NPFSSTo))
                                {
                                    temp[index].NPFSSTo += ",";
                                }
                            }
                            temp[index].NPFSSTo += row.GetCell(13 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXDiff))
                                {
                                    temp[index].vcFXDiff += ",";
                                }
                            }
                            temp[index].vcFXDiff += row.GetCell(47 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXNo))
                                {
                                    temp[index].vcFXNo += ",";
                                }
                            }
                            temp[index].vcFXNo += row.GetCell(49 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcDTDiff))
                                {
                                    temp[index].vcDTDiff += ",";
                                }
                            }
                            temp[index].vcDTDiff += row.GetCell(18 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcPart_id_DT))
                                {
                                    temp[index].vcPart_id_DT += ",";
                                }
                            }
                            temp[index].vcPart_id_DT += row.GetCell(21 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row.GetCell(63).ToString();
                            if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row1.GetCell(63).ToString();

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcPart_Id_new) || !string.IsNullOrWhiteSpace(temp[index].vcPart_Id_old))
                        {
                            temp[index].flag = flag;

                            if (!string.IsNullOrWhiteSpace(row.GetCell(6 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcBJDiff))
                                {
                                    temp[index].vcBJDiff += ",";
                                }
                            }
                            temp[index].vcBJDiff += row.GetCell(6 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(9 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcStartYearMonth))
                                {
                                    temp[index].vcStartYearMonth += ",";
                                }
                            }
                            temp[index].vcStartYearMonth += row.GetCell(9 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(13 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].NPFSSTo))
                                {
                                    temp[index].NPFSSTo += ",";
                                }
                            }
                            temp[index].NPFSSTo += row.GetCell(13 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(47 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXDiff))
                                {
                                    temp[index].vcFXDiff += ",";
                                }
                            }
                            temp[index].vcFXDiff += row.GetCell(47 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(49 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcFXNo))
                                {
                                    temp[index].vcFXNo += ",";
                                }
                            }
                            temp[index].vcFXNo += row.GetCell(49 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(18 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcDTDiff))
                                {
                                    temp[index].vcDTDiff += ",";
                                }
                            }
                            temp[index].vcDTDiff += row.GetCell(18 + flag).ToString();
                            if (!string.IsNullOrWhiteSpace(row.GetCell(21 + flag).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcPart_id_DT))
                                {
                                    temp[index].vcPart_id_DT += ",";
                                }
                            }
                            temp[index].vcPart_id_DT += row.GetCell(21 + flag).ToString();

                            if (!string.IsNullOrWhiteSpace(row.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row.GetCell(63).ToString();
                            if (!string.IsNullOrWhiteSpace(row1.GetCell(63).ToString()))
                            {
                                if (!string.IsNullOrWhiteSpace(temp[index].vcChange))
                                {
                                    temp[index].vcChange += ",";
                                }
                            }
                            temp[index].vcChange += row1.GetCell(63).ToString();

                        }
                    }

                }



            }

            #endregion

            #region 获取下半部分数据
            string tempPart_id = "";

            for (int i = ST; i < SS; i += 2)
            {
                tempPart_id = prePart_idS;

                row = sheet.GetRow(i);
                IRow rowtemp = sheet.GetRow(i + 1);

                //根据品番添加
                if (!string.IsNullOrWhiteSpace(row.GetCell(0).ToString()))
                {
                    int flag = 0;
                    int flagN = 1;
                    if (row.GetCell(0 + flag).ToString() == "")
                    {
                        tempPart_id = row.GetCell(0 + flagN).ToString();
                    }
                    else
                    {
                        tempPart_id = row.GetCell(0 + flag).ToString();
                    }
                    int index = temp.FindIndex(item => tempPart_id.Equals(item.vcPart_Id_old));

                    temp[index].vcOldProj = row.GetCell(15 + flag).ToString();
                    temp[index].ZSPF = row.GetCell(50 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(rowtemp.GetCell(50 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].ZSPF))
                        {
                            temp[index].ZSPF += ",";
                        }
                    }
                    temp[index].ZSPF += rowtemp.GetCell(50 + flag).ToString();

                    temp[index].OGCSSFrom = row.GetCell(57 + flag).ToString();
                    temp[index].vcOldProjTime = row.GetCell(61 + flag).ToString();

                    //---------------------(新)
                    temp[index].vcNewProj = row.GetCell(15 + flagN).ToString();
                    temp[index].vcCZYD = row.GetCell(50 + flagN).ToString();

                    if (!string.IsNullOrWhiteSpace(rowtemp.GetCell(50 + flagN).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcCZYD))
                        {
                            temp[index].vcCZYD += ",";
                        }
                    }
                    temp[index].vcCZYD += rowtemp.GetCell(50 + flagN).ToString();

                    temp[index].vcNewProjTime = row.GetCell(57 + flagN).ToString();
                    temp[index].NGCSSTo = row.GetCell(61 + flagN).ToString();
                    //----------------------
                    prePart_idS = tempPart_id;
                }
                #region flag=1
                //根据品番添加
                else if (!string.IsNullOrWhiteSpace(row.GetCell(1).ToString()))
                {
                    int flag = 1;
                    tempPart_id = row.GetCell(0 + flag).ToString();
                    int n = temp.Count();
                    int index = temp.FindIndex(item => item.vcPart_Id_new.Equals(tempPart_id));
                    temp[index].vcNewProj = row.GetCell(15 + flag).ToString();
                    temp[index].vcCZYD = row.GetCell(50 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(rowtemp.GetCell(50 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcCZYD))
                        {
                            temp[index].vcCZYD += ",";
                        }
                    }

                    temp[index].vcCZYD += rowtemp.GetCell(50 + flag).ToString();

                    temp[index].vcNewProjTime = row.GetCell(57 + flag).ToString();
                    temp[index].NGCSSTo = row.GetCell(61 + flag).ToString();
                    prePart_idS = tempPart_id;
                }
                #endregion

                //根据品番修改
                else if (!string.IsNullOrWhiteSpace(row.GetCell(6).ToString()))
                {
                    int flag = 0;
                    int flagN = 1;
                    int index = temp.FindIndex(item => tempPart_id.Equals(item.vcPart_Id_old) || tempPart_id.Equals(item.vcPart_Id_new));

                    if (!string.IsNullOrWhiteSpace(row.GetCell(15 + flag).ToString()))
                    {
                        //工程含有两个及以上的第二个除了含有WB，WL,WF,WD，其余的不显示
                        if (row.GetCell(15 + flag).ToString().Contains("WB") || row.GetCell(15 + flag).ToString().Contains("WD") || row.GetCell(15 + flag).ToString().Contains("WL") || row.GetCell(15 + flag).ToString().Contains("WF"))
                        {
                            if (!string.IsNullOrWhiteSpace(temp[index].vcOldProj))
                            {
                                temp[index].vcOldProj += ",";
                                temp[index].vcOldProj += row.GetCell(15 + flag).ToString();

                            }
                            else
                            {
                                temp[index].vcOldProj += row.GetCell(15 + flag).ToString();
                            }
                        }


                    }

                    if (!string.IsNullOrWhiteSpace(temp[index].ZSPF))
                    {
                        temp[index].ZSPF += ",";
                    }
                    temp[index].ZSPF += row.GetCell(50 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(rowtemp.GetCell(50 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].ZSPF))
                        {
                            temp[index].ZSPF += ",";
                        }
                    }

                    temp[index].ZSPF += rowtemp.GetCell(50 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(temp[index].OGCSSFrom))
                    {
                        temp[index].OGCSSFrom += ",";
                    }
                    temp[index].OGCSSFrom += row.GetCell(57 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(61 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcOldProjTime))
                        {
                            temp[index].vcOldProjTime += ",";
                        }
                    }

                    temp[index].vcOldProjTime += row.GetCell(61 + flag).ToString();
                }
                #region flag=1
                //根据品番修改
                else if (!string.IsNullOrWhiteSpace(row.GetCell(7).ToString()))
                {
                    int flag = 1;

                    int index = temp.FindIndex(item => tempPart_id.Equals(item.vcPart_Id_old) || tempPart_id.Equals(item.vcPart_Id_new));
                    if (!string.IsNullOrWhiteSpace(temp[index].vcNewProj))
                    {
                        temp[index].vcNewProj += ",";
                    }
                    temp[index].vcNewProj += row.GetCell(15 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(temp[index].vcCZYD))
                    {
                        temp[index].vcCZYD += ",";
                    }
                    temp[index].vcCZYD += row.GetCell(50 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(rowtemp.GetCell(50 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcCZYD))
                        {
                            temp[index].vcCZYD += ",";
                        }
                    }

                    temp[index].vcCZYD += rowtemp.GetCell(50 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(temp[index].vcNewProjTime))
                    {
                        temp[index].vcNewProjTime += ",";
                    }
                    temp[index].vcNewProjTime += row.GetCell(57 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(61 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].NGCSSTo))
                        {
                            temp[index].NGCSSTo += ",";
                        }
                    }

                    temp[index].NGCSSTo += row.GetCell(61 + flag).ToString();
                }
                #endregion

                //根据品番修改------------CHENYING
                else if (!string.IsNullOrWhiteSpace(row.GetCell(58).ToString()))
                {
                    int flag = 1;

                    int index = temp.FindIndex(item => tempPart_id.Equals(item.vcPart_Id_old) || tempPart_id.Equals(item.vcPart_Id_new));
                    if (!string.IsNullOrWhiteSpace(temp[index].vcNewProj))
                    {
                        temp[index].vcNewProj += ",";
                    }
                    temp[index].vcNewProj += row.GetCell(15 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(temp[index].vcCZYD))
                    {
                        temp[index].vcCZYD += ",";
                    }
                    temp[index].vcCZYD += row.GetCell(50 + flag).ToString();
                    if (!string.IsNullOrWhiteSpace(rowtemp.GetCell(50 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].vcCZYD))
                        {
                            temp[index].vcCZYD += ",";
                        }
                    }

                    temp[index].vcCZYD += rowtemp.GetCell(50 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(temp[index].vcNewProjTime))
                    {
                        temp[index].vcNewProjTime += ",";
                    }
                    temp[index].vcNewProjTime += row.GetCell(57 + flag).ToString();

                    if (!string.IsNullOrWhiteSpace(row.GetCell(61 + flag).ToString()))
                    {
                        if (!string.IsNullOrWhiteSpace(temp[index].NGCSSTo))
                        {
                            temp[index].NGCSSTo += ",";
                        }
                    }

                    temp[index].NGCSSTo += row.GetCell(61 + flag).ToString();
                }
                else
                {
                    break;
                }
            }

            #endregion

        }

        public static DataTable ListToDataTable1(List<FS0201_DataEntity.Part> list, FS0201_DataEntity.Entity en)
        {
            //创建一个名为"tableName"的空表
            //DataTable dt = new DataTable("tableName");
            DataTable dt = new DataTable();
            FS0201_DataEntity.Part part = new FS0201_DataEntity.Part();
            //创建传入对象名称的列
            string name = list.FirstOrDefault().GetType().ToString();
            FS0201_DataEntity.Part p = list.FirstOrDefault();
            dt.Columns.Add("vcPart_Id_old");
            dt.Columns.Add("vcPart_Id_new");
            dt.Columns.Add("vcBJDiff");
            dt.Columns.Add("vcDTDiff");
            dt.Columns.Add("vcPart_id_DT");
            dt.Columns.Add("vcPartName");
            dt.Columns.Add("vcStartYearMonth");
            dt.Columns.Add("vcFXDiff");
            dt.Columns.Add("vcFXNo");
            dt.Columns.Add("vcChange");
            dt.Columns.Add("vcNewProj");
            dt.Columns.Add("vcNewProjTime");
            dt.Columns.Add("vcOldProj");
            dt.Columns.Add("vcOldProjTime");
            dt.Columns.Add("vcCZYD");
            dt.Columns.Add("vcSheetName");
            dt.Columns.Add("vcSPINo");
            dt.Columns.Add("vcFileName");
            dt.Columns.Add("dHandleTime");

            for (int i = 0; i < list.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["vcPart_Id_old"] = list[i].vcPart_Id_old;
                dr["vcPart_Id_new"] = list[i].vcPart_Id_new;
                dr["vcBJDiff"] = list[i].vcBJDiff;
                dr["vcDTDiff"] = list[i].vcDTDiff;
                dr["vcPart_id_DT"] = list[i].vcPart_id_DT;
                dr["vcPartName"] = list[i].vcPartName;
                dr["vcStartYearMonth"] = list[i].vcStartYearMonth;
                dr["vcFXDiff"] = list[i].vcFXDiff;
                dr["vcFXNo"] = list[i].vcFXNo;
                dr["vcChange"] = list[i].vcChange;
                dr["vcNewProj"] = list[i].vcNewProj;
                dr["vcNewProjTime"] = list[i].vcNewProjTime;
                dr["vcOldProj"] = list[i].vcOldProj;
                dr["vcOldProjTime"] = list[i].vcOldProjTime;
                dr["vcCZYD"] = list[i].vcCZYD;
                dr["vcSheetName"] = list[i].vcSheetName;
                dr["vcSPINo"] = en.vcSPINo;
                dr["vcFileName"] = en.vcFileName;
                dr["dHandleTime"] = en.dHandleTime;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static DataTable ListToDataTable1(List<DataRow> list)
        {
            //创建一个名为"tableName"的空表
            //DataTable dt = new DataTable("tableName");
            DataTable dt = new DataTable();
            FS0201_DataEntity.Part part = new FS0201_DataEntity.Part();
            dt.Columns.Add("vcPart_Id_old");
            dt.Columns.Add("vcPart_Id_new");
            dt.Columns.Add("vcBJDiff");
            dt.Columns.Add("vcDTDiff");
            dt.Columns.Add("vcPart_id_DT");
            dt.Columns.Add("vcPartName");
            dt.Columns.Add("vcStartYearMonth");
            dt.Columns.Add("vcFXDiff");
            dt.Columns.Add("vcFXNo");
            dt.Columns.Add("vcChange");
            dt.Columns.Add("vcNewProj");
            dt.Columns.Add("vcNewProjTime");
            dt.Columns.Add("vcOldProj");
            dt.Columns.Add("vcOldProjTime");
            dt.Columns.Add("vcCZYD");
            dt.Columns.Add("vcSheetName");
            dt.Columns.Add("vcSPINo");
            dt.Columns.Add("vcFileName");
            dt.Columns.Add("dHandleTime");

            for (int i = 0; i < list.Count; i++)
            {
                DataRow dr = dt.NewRow();

                dr["vcPart_Id_old"] = list[i].ItemArray[0];
                dr["vcPart_Id_new"] = list[i].ItemArray[1];
                dr["vcBJDiff"] = list[i].ItemArray[2];
                dr["vcDTDiff"] = list[i].ItemArray[3];
                dr["vcPart_id_DT"] = list[i].ItemArray[4];
                dr["vcPartName"] = list[i].ItemArray[5];
                dr["vcStartYearMonth"] = list[i].ItemArray[6];
                dr["vcFXDiff"] = list[i].ItemArray[7];
                dr["vcFXNo"] = list[i].ItemArray[8];
                dr["vcChange"] = list[i].ItemArray[9];
                dr["vcNewProj"] = list[i].ItemArray[10];
                dr["vcNewProjTime"] = list[i].ItemArray[11];
                dr["vcOldProj"] = list[i].ItemArray[12];
                dr["vcOldProjTime"] = list[i].ItemArray[13];
                dr["vcCZYD"] = list[i].ItemArray[14];
                dr["vcSheetName"] = list[i].ItemArray[15];
                dr["vcSPINo"] = list[i].ItemArray[16];
                dr["vcFileName"] = list[i].ItemArray[17];
                dr["dHandleTime"] = list[i].ItemArray[18];
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion


        public bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}