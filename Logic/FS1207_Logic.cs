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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS1207_Logic
    {
        FS1207_DataAccess dataAccess = new FS1207_DataAccess();

        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <param name="mon"></param>
        /// <param name="plant"></param>
        /// <returns>msg</returns>
        //public string FileImport(HttpPostedFile file, string userId, string mon, string plant)
        //{
        //    FileInfo file_im = new FileInfo(file.FileName);
        //    if (file_im.Extension != ".txt")
        //    {
        //        return "文件格式不正确！";
        //    }
        //    try
        //    {
        //        QMExcel oQMExcel = new QMExcel();
        //        string path;
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1207.txt");
        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //        }
        //        file.SaveAs(path);
        //        //判断此对象月信息是否已发注
        //        string msg = dataAccess.checkFZ(mon);
        //        if (msg.Length > 0)
        //        {
        //            return msg;
        //        }


        //        string[] Line = File.ReadAllLines(path);

        //        DataTable dttmp = dataAccess.dtResultClone();

        //        for (int i = 0; i < Line.Length; i++)
        //        {

        //            DataRow dr = dttmp.NewRow();
        //            string vcMonth = Line[i].Substring(189, 4) + '-' + Line[i].Substring(193, 2);
        //            string vcPartsNo = Line[i].Substring(13, 12).Trim();
        //            string vcSouce = Line[i].Substring(25, 1).Trim();
        //            string vcDock = Line[i].Substring(26, 3).Trim();
        //            int D1 = int.Parse(Line[i].Substring(200, 9));
        //            int D2 = int.Parse(Line[i].Substring(210, 9));
        //            int D3 = int.Parse(Line[i].Substring(220, 9));
        //            int D4 = int.Parse(Line[i].Substring(230, 9));
        //            int D5 = int.Parse(Line[i].Substring(240, 9));
        //            int D6 = int.Parse(Line[i].Substring(250, 9));
        //            int D7 = int.Parse(Line[i].Substring(260, 9));
        //            int D8 = int.Parse(Line[i].Substring(270, 9));
        //            int D9 = int.Parse(Line[i].Substring(280, 9));
        //            int D10 = int.Parse(Line[i].Substring(290, 9));
        //            int D11 = int.Parse(Line[i].Substring(300, 9));
        //            int D12 = int.Parse(Line[i].Substring(310, 9));
        //            int D13 = int.Parse(Line[i].Substring(320, 9));
        //            int D14 = int.Parse(Line[i].Substring(330, 9));
        //            int D15 = int.Parse(Line[i].Substring(340, 9));
        //            int D16 = int.Parse(Line[i].Substring(350, 9));
        //            int D17 = int.Parse(Line[i].Substring(360, 9));
        //            int D18 = int.Parse(Line[i].Substring(370, 9));
        //            int D19 = int.Parse(Line[i].Substring(380, 9));
        //            int D20 = int.Parse(Line[i].Substring(390, 9));
        //            int D21 = int.Parse(Line[i].Substring(400, 9));
        //            int D22 = int.Parse(Line[i].Substring(410, 9));
        //            int D23 = int.Parse(Line[i].Substring(420, 9));
        //            int D24 = int.Parse(Line[i].Substring(430, 9));
        //            int D25 = int.Parse(Line[i].Substring(440, 9));
        //            int D26 = int.Parse(Line[i].Substring(450, 9));
        //            int D27 = int.Parse(Line[i].Substring(460, 9));
        //            int D28 = int.Parse(Line[i].Substring(470, 9));
        //            int D29 = int.Parse(Line[i].Substring(480, 9));
        //            int D30 = int.Parse(Line[i].Substring(490, 9));
        //            int D31 = int.Parse(Line[i].Substring(500, 9));

        //            dr["vcMonth"] = vcMonth;
        //            dr["vcPartsNo"] = vcPartsNo;
        //            dr["vcSource"] = vcSouce;
        //            dr["vcDock"] = vcDock;
        //            dr["D1"] = D1;
        //            dr["D2"] = D2;
        //            dr["D3"] = D3;
        //            dr["D4"] = D4;
        //            dr["D5"] = D5;
        //            dr["D6"] = D6;
        //            dr["D7"] = D7;
        //            dr["D8"] = D8;
        //            dr["D9"] = D9;
        //            dr["D10"] = D10;
        //            dr["D11"] = D11;
        //            dr["D12"] = D12;
        //            dr["D13"] = D13;
        //            dr["D14"] = D14;
        //            dr["D15"] = D15;
        //            dr["D16"] = D16;
        //            dr["D17"] = D17;
        //            dr["D18"] = D18;
        //            dr["D19"] = D19;
        //            dr["D20"] = D20;
        //            dr["D21"] = D21;
        //            dr["D22"] = D22;
        //            dr["D23"] = D23;
        //            dr["D24"] = D24;
        //            dr["D25"] = D25;
        //            dr["D26"] = D26;
        //            dr["D27"] = D27;
        //            dr["D28"] = D28;
        //            dr["D29"] = D29;
        //            dr["D30"] = D30;
        //            dr["D31"] = D31;
        //            //dr["iCONum"] = 0; //当月余数
        //            //dr["iFZNum"] = 0; //当月发注数

        //            //dr["iXZNum"] = 0; // 当月修正数
        //            dr["vcPlant"] = plant;
        //            dr["Creater"] = userId;
        //            dr["dCreateDate"] = DateTime.Now.ToString();

        //            //导入标志“0”，发注时更新为“1”

        //            // dr["iFZFlg"] = "0";

        //            dttmp.Rows.Add(dr);
        //            //计算总数
        //            int a = 0;
        //            for (int j = 5; j <= 35; j++)
        //            {

        //                a += Convert.ToInt32(dttmp.Rows[i][j].ToString());
        //            }

        //            dttmp.Rows[i]["Total"] = a;

        //        }

        //        DataTable dtSource = getSource();

        //        DataTable dt = new DataTable();
        //        dt = dttmp.Clone();
        //        for (int i = 0; i < dtSource.Rows.Count; i++)
        //        {
        //            DataRow[] rows = dttmp.Select("vcMonth='" + mon.ToString() + "' and vcSource='" + dtSource.Rows[i]["vcSource"] + "' and vcDock='" + dtSource.Rows[i]["vcDock"] + "'");

        //            if (rows.Length > 0)
        //            {
        //                foreach (DataRow row in rows)
        //                {
        //                    dt.ImportRow(row);
        //                }

        //            }
        //        }

        //        if (dt.Rows.Count > 0)
        //        {
        //            msg = dataAccess.UpdateTable(dt, userId, mon, plant);
        //            if (msg.Length > 0)
        //            {
        //                return "导入失败！";

        //            }
        //        }
        //        else
        //        {
        //            return "对象月数据不存在！";
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        LogHelper.ErrorLog("构成件必要数计算导入异常:" + ex.Message);
        //        return "导入失败！";
        //    }

        //    return "";
        //}

        public string updSSP(DataTable dt, string strUser)
        {
            return dataAccess.updSSP(dt, strUser);
        }


        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>msg</returns>
        //public string ZJFZFileImport(HttpPostedFile file, string userId)
        //{
        //    FileInfo file_im = new FileInfo(file.FileName);
        //    if (file_im.Extension != ".xls" && file_im.Extension != ".xlt" && file_im.Extension != ".xlsx")
        //    {
        //        return "文件格式不正确！";
        //    }
        //    QMExcel oQMExcel = new QMExcel();
        //    string path;
        //    string tepath = HttpContext.Current.Server.MapPath("~/Templates/FS1207_ADD.xlt");
        //    if (file_im.Extension == ".xls" || file_im.Extension == ".xlt")
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1207_ADD.xls");
        //    }
        //    else
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1207_ADD.xlsx");
        //    }
        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //    }
        //    file.SaveAs(path);
        //    DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(tepath);//导入Excel文件到DataTable
        //    DataTable dt = new DataTable();
        //    string msg = checkExcel1(path, ref dt, dtTmplate);//检验导入的数据
        //    if (msg.Length > 0)
        //    {
        //        return msg;
        //    }
        //    dataAccess.UpdateAddFZIM(dt, userId);//将导入的表格数据上传

        //    return "";
        //}

        public string UpdateFZJSEditFZ(DataTable dtSSPtmp, string type, string orderlb, string UserId, out string exlName)
        {
            string msg = "";
            exlName = "";
            try
            {
                string date = DateTime.Now.ToString("yyyy年M月d日");
                string date2 = DateTime.Now.ToString("yy-M");

                DataTable dtHeader = new DataTable();//表头信息
                dtHeader.Columns.Add("vcDate");//日期
                dtHeader.Columns.Add("vcSalerName");//高新
                dtHeader.Columns.Add("vcSalerPhone");//高新电话
                dtHeader.Columns.Add("vcSalerEMail");//高新邮箱
                dtHeader.Columns.Add("vcOrderNo");//订单编号（不带TM2）
                dtHeader.Columns.Add("vcItemTotal");//项目总数
                dtHeader.Columns.Add("vcOrderQtyTotal");//订货总件数
                dtHeader.Columns.Add("vcUserName");//刘家成
                dtHeader.Columns.Add("vcUserPhone");//刘家成电话
                dtHeader.Columns.Add("vcUserEMail");//刘家成邮箱
                dtHeader.Columns.Add("vcOrderType");//订单类别
                dtHeader.Columns.Add("vcChuanZhen");//丰田传真
                dtHeader.Columns.Add("vcChuanZhenSale"); //销售公司传真

                DataTable dtDetail = new DataTable();//数据信息
                dtDetail.Columns.Add("TASSCODE");//经销商代码
                dtDetail.Columns.Add("B");//B
                dtDetail.Columns.Add("C");//C
                dtDetail.Columns.Add("OrderDate");//订购日期
                dtDetail.Columns.Add("E");//E
                dtDetail.Columns.Add("F");//F
                dtDetail.Columns.Add("OrderNo");//订单编号（带TM2）
                dtDetail.Columns.Add("H");//H
                dtDetail.Columns.Add("I");//I
                dtDetail.Columns.Add("TYPE");//订单类型
                dtDetail.Columns.Add("ItemNo");//项目号（0000）
                dtDetail.Columns.Add("L");//L
                dtDetail.Columns.Add("PartNo");//零件编号即发注品番
                dtDetail.Columns.Add("N");//N
                dtDetail.Columns.Add("O");//O
                dtDetail.Columns.Add("P");//P
                dtDetail.Columns.Add("OrderQty", typeof(int));//订购数量
                dtDetail.Columns.Add("PToPCust");//代订客户

                if (dtSSPtmp.Rows.Count == 0 || dtSSPtmp == null)
                {
                    return "无发注数据，请进行检索检索数据！";
                }
                DataTable dtSSP = new DataTable();
                dtSSP = dtSSPtmp.Clone();
                DataRow[] rows = dtSSPtmp.Select("vcSource='" + type + "' and iFZNum>=0 ");
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        dtSSP.ImportRow(row);
                    }
                }
                else
                {
                    return "无发注数据！";
                }

                //获取销售公司信息

                DataTable dtSaleuser = dataAccess.GetSaleuser(UserId);
                //获取发注担当信息
                DataTable dtUser = dataAccess.GetUser(UserId);
                if (dtUser.Rows.Count == 0)
                {
                    return "不是发注担当，不能进行发注！";
                }

                //订单号作成：
                string Year = date2.Split('-')[0];
                string Month = date2.Split('-')[1];
                string OrderType = string.Empty;
                string TYPE = string.Empty;
                int OrderQtyTotal = 0;//订货总件数

                if (Month == "10")
                {
                    Month = "J";
                }
                else if (Month == "11")
                {
                    Month = "Q";
                }
                else if (Month == "12")
                {
                    Month = "K";
                }
                switch (type)
                {
                    case "JSP": OrderType = "01"; break;
                    case "MSP": OrderType = "02"; break;
                }
                switch (orderlb)
                {
                    case "S/O": TYPE = "S"; break;
                    case "E/O": TYPE = "E"; break;
                    case "F/O": TYPE = "F"; break;
                }
                string vcOrderNo = Year + Month + OrderType;//表头用
                string OrderNo = "TM2" + vcOrderNo;//数据用

                //将信息写到模板中 单元格必须留一行空行(dtSSP中已有发注品番、数量、类别）
                //数据信息：

                for (int i = 0; i < dtSSP.Rows.Count; i++)
                {
                    if (dtSSP.Rows[i]["iFZNum"].ToString() != "0")//去除发注数为0
                    {
                        int ItemNo = i + 1;
                        DataRow drDetail = dtDetail.NewRow();
                        drDetail["TASSCODE"] = "TFTM2";
                        drDetail["OrderDate"] = DateTime.Now.ToString("yyyyMMdd");
                        drDetail["OrderNo"] = OrderNo;
                        drDetail["TYPE"] = TYPE;
                        drDetail["ItemNo"] = ItemNo.ToString("0000");
                        drDetail["PartNo"] = dtSSP.Rows[i]["vcPartsNoFZ"].ToString();
                        drDetail["OrderQty"] = Convert.ToInt32(dtSSP.Rows[i]["iFZNum"].ToString());
                        OrderQtyTotal = OrderQtyTotal + Convert.ToInt32(dtSSP.Rows[i]["iFZNum"].ToString());//总件数累加
                        dtDetail.Rows.Add(drDetail);
                    }
                }

                //排序
                DataView dv = dtDetail.DefaultView;
                dv.Sort = "ItemNo";
                dtDetail = dv.ToTable();
                //添加空行
                //DataRow drDNull = dtDetail.NewRow();
                //dtDetail.Rows.Add(drDNull);
                //表头信息：

                DataRow drHeader = dtHeader.NewRow();
                drHeader["vcDate"] = date;
                drHeader["vcSalerName"] = dtSaleuser.Rows[0]["name"].ToString();
                drHeader["vcSalerPhone"] = dtSaleuser.Rows[0]["pphone"].ToString();
                drHeader["vcSalerEMail"] = dtSaleuser.Rows[0]["email"].ToString();
                drHeader["vcChuanZhenSale"] = dtSaleuser.Rows[0]["ChuanZhenSale"].ToString();
                drHeader["vcOrderNo"] = vcOrderNo;
                drHeader["vcItemTotal"] = (dtDetail.Rows.Count).ToString();//多一空行，算总项目数时要减掉
                drHeader["vcOrderQtyTotal"] = OrderQtyTotal.ToString();
                drHeader["vcUserName"] = dtUser.Rows[0]["Username"].ToString();
                drHeader["vcUserPhone"] = dtUser.Rows[0]["UserPhone"].ToString();
                drHeader["vcUserEMail"] = dtUser.Rows[0]["Useremail"].ToString();
                drHeader["vcOrderType"] = orderlb;
                drHeader["vcChuanZhen"] = dtUser.Rows[0]["ChuanZhen"].ToString();
                dtHeader.Rows.Add(drHeader);
                //更新tSSP表中 iFZFlg='1',iXZNum ,iFZNum,iCONum
                //201903  修改发注数为0，更新余数
                msg = dataAccess.updSSP(dtSSP, UserId);
                //end
                if (msg.Length <= 0)
                {
                    if (type == "JSP")
                    {
                        exlName = "发注订货单" + System.DateTime.Now.ToString("yyyyMMdd") + "";
                    }
                    else
                    {
                        exlName = "发注订货单" + System.DateTime.Now.ToString("yyyyMMdd") + "B";
                    }
                    if (dtDetail.Rows.Count == 0)
                    {
                        msg = "无发注数据！";
                    }
                    else
                    {
                        msg = "发注成功！";
                        exlName = exlName + ".xls";
                        //QMExcel oQMExcel = new QMExcel();
                        //string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1207_SSP.xlt");//模板路径
                        //string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);//文件路径
                        //oQMExcel.ExportFromTemplate(dtHeader, dtDetail, tmplatePath, path, 19, 1, false);
                    }
                    dtSSP.Dispose();
                }
                else
                {
                    msg = "导出失败：";
                }
            }
            catch (WebException ex)
            {
                //LogHelper.ErrorLog("发注异常：" + ex.Message);
                msg = ex.Message;
            }
            return msg;
        }




        /// <summary>
        /// 发注计算-导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="exlName"></param>
        //public string FZJSFileExport(DataTable dt, string exlName)
        //{
        //    string msg = null;
        //    try
        //    {
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            dt.Columns.Remove("iFlag");
        //            dt.Columns.Remove("vcPartsNoFZ");
        //            dt.Columns.Remove("vcSource");
        //            QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
        //            string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1207_FZJS.xlt");
        //            string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
        //            oQMExcel.ExportFromTemplate(dt, tmplatePath, path, 2, 1, true);

        //            dt.Dispose();
        //        }
        //        else
        //        {
        //            msg = "无检索数据,无法导出数据";
        //        }

        //    }
        //    catch (WebException ex)
        //    {
        //        LogHelper.ErrorLog("发注计算导出异常：" + ex.Message);
        //        msg = "导出失败！";
        //    }
        //    return msg;
        //}

        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>msg</returns>
        //public string FZJSFileImport(HttpPostedFile file, string userId)
        //{
        //    FileInfo file_im = new FileInfo(file.FileName);
        //    if (file_im.Extension != ".xls" && file_im.Extension != ".xlt" && file_im.Extension != ".xlsx")
        //    {
        //        return "文件格式不正确！";
        //    }
        //    QMExcel oQMExcel = new QMExcel();
        //    string path;
        //    string tepath = HttpContext.Current.Server.MapPath("~/Templates/FS1207_FZJS.xlt");
        //    if (file_im.Extension == ".xls" || file_im.Extension == ".xlt")
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1207_FZJS.xls");
        //    }
        //    else
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1207_FZJS.xlsx");
        //    }
        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //    }
        //    file.SaveAs(path);
        //    DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(tepath);//导入Excel文件到DataTable
        //    DataTable dt = new DataTable();
        //    string msg = checkExcel(path, ref dt, dtTmplate);//检验导入的数据
        //    if (msg.Length > 0)
        //    {
        //        return msg;
        //    }
        //    dataAccess.UpdateFZJS(dt, userId);//将导入的表格数据上传

        //    return "";
        //}

        public DataTable GetFzjsRenders(string vcMon, string vcPartsNo, string vcYesOrNo, out string msg)
        {
            DataTable dt = null;
            msg = "";
            if (vcYesOrNo == "未发注")
            {
                if (vcMon == "")
                {
                    msg = "请选择计划对象月！";
                    return null;
                }
                DataTable dtNoExict = NoExict(vcMon);
                if (dtNoExict.Rows.Count > 0)
                {
                    msg = "品番" + dtNoExict.Rows[0]["vcPartsNo"] + "未在基础数据中维护";
                    return null;
                }
                dt = searchFZJS(vcMon, vcPartsNo);
                if (dt.Rows.Count == 0)
                {
                    msg = "没有检索到数据，请重新检索";
                }
            }
            else
            {
                dt = searchFZFinsh(vcMon, vcPartsNo);
                if (dt.Rows.Count == 0)
                {
                    msg = "没有检索到数据，请重新检索";
                }
            }
            return dt;
        }

        #region 判断是否已发注
        /// <summary>
        /// 判断是否已发注
        /// </summary>
        /// <param name="mon"></param>
        /// <returns></returns>
        public string checkFZ(string mon)
        {
            return dataAccess.checkFZ(mon);
        }
        #endregion

        #region 获取SOQReply数据
        public DataTable getNQCReceiveInfo(string vcDXYM, string vcPlant)
        {
            return dataAccess.getNQCReceiveInfo(vcDXYM, vcPlant);
        }
        #endregion




        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="exlName"></param>
        public string FileExport(DataTable dt, string exlName)
        {
            string msg = null;
            //try
            //{
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
            //        string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1207.xlt");
            //        string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
            //        oQMExcel.ExportFromTemplate(dt, tmplatePath, path, 2, 1, true);

            //        dt.Dispose();
            //    }
            //    else
            //    {
            //        msg = "无导出数据,请确认条件!";
            //    }

            //}
            //catch (WebException ex)
            //{
            //    LogHelper.ErrorLog("构成件必要数计算导出异常：" + ex.Message);
            //    msg = "导出失败！";
            //}
            return msg;
        }

        public DataTable getPlant()
        {
            return dataAccess.getPlant();
        }
        public DataTable getClass()
        {
            return dataAccess.getClass();
        }
        public DataTable getSource()
        {
            return dataAccess.getSourse();
        }
        public DataTable ddlSaleUser()
        {
            return dataAccess.ddlSaleuser();
        }
        public DataTable search(string Mon, string ddlType, string Partsno, string vcPlant)
        {
            return dataAccess.search(Mon, ddlType, Partsno, vcPlant);
        }
        public DataTable dtResultClone()
        {
            return dataAccess.dtResultClone();
        }
        public string UpdateTable(DataTable dt, string user, string mon, string plant)
        {
            return dataAccess.UpdateTable(dt, user, mon, plant);
        }

        public DataTable NoExict(string mon)
        {
            return dataAccess.NoExict(mon);
        }

        public DataTable searchFZJS(string mon, string partsno)
        {
            return dataAccess.search_FZJS(mon, partsno);
        }

        public DataTable searchAll(string mon, string partsno)
        {
            return dataAccess.search_All(mon, partsno);
        }


        public DataTable searchFZFinsh(string mon, string partsno)
        {
            return dataAccess.searchFZFinsh(mon, partsno);
        }
        public string UpdateFZJS(DataTable dt, string user)
        {
            return dataAccess.UpdateFZJS(dt, user);
        }
        public string UpdateFZJSEdit(DataTable dt, string user)
        {
            return dataAccess.UpdateFZJSEdit(dt, user);
        }

        #region 导出带模板
        public string ExportFrom_1207(DataTable dtHeader, DataTable dt, string rootPath, string xltName, string strUserId, string strFileName, bool isAlignCenter)
        {
            try
            {
                string[] field = { "TASSCODE", "B", "C", "OrderDate", "E", "F", "OrderNo", "H", "I", "TYPE", "ItemNo", "L", "PartNo", "N", "O", "P", "OrderQty", "PToPCust" };
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();
                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                IDataFormat dataformat = hssfworkbook.CreateDataFormat();
                ISheet sheet = hssfworkbook.GetSheetAt(0);
                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;

                #region 表头
                sheet.GetRow(0).GetCell(11).SetCellValue(dtHeader.Rows[0]["vcDate"].ToString());//日期L1
                sheet.GetRow(4).GetCell(1).SetCellValue(dtHeader.Rows[0]["vcSalerName"].ToString());//高新B5
                sheet.GetRow(5).GetCell(1).SetCellValue(dtHeader.Rows[0]["vcSalerPhone"].ToString());//高新电话B6
                sheet.GetRow(6).GetCell(2).SetCellValue(dtHeader.Rows[0]["vcSalerEMail"].ToString());//高新邮箱C7
                sheet.GetRow(5).GetCell(8).SetCellValue(dtHeader.Rows[0]["vcChuanZhenSale"].ToString());
                sheet.GetRow(10).GetCell(4).SetCellValue(int.Parse(dtHeader.Rows[0]["vcOrderNo"].ToString().Substring(0, 1)));//订单编号（不带TM2）E11
                sheet.GetRow(10).GetCell(5).SetCellValue(int.Parse(dtHeader.Rows[0]["vcOrderNo"].ToString().Substring(1, 1)));//订单编号（不带TM2）F11
                sheet.GetRow(10).GetCell(6).SetCellValue(int.Parse(dtHeader.Rows[0]["vcOrderNo"].ToString().Substring(2, 1)));//订单编号（不带TM2）G11
                sheet.GetRow(10).GetCell(7).SetCellValue(int.Parse(dtHeader.Rows[0]["vcOrderNo"].ToString().Substring(3, 1)));//订单编号（不带TM2）H11
                sheet.GetRow(10).GetCell(8).SetCellValue(int.Parse(dtHeader.Rows[0]["vcOrderNo"].ToString().Substring(4, 1)));//订单编号（不带TM2）I11
                sheet.GetRow(12).GetCell(4).SetCellValue(int.Parse(dtHeader.Rows[0]["vcItemTotal"].ToString()));//项目总数E13
                sheet.GetRow(13).GetCell(4).SetCellValue(int.Parse(dtHeader.Rows[0]["vcOrderQtyTotal"].ToString()));//订货总件数E14
                sheet.GetRow(5).GetCell(11).SetCellValue(dtHeader.Rows[0]["vcUserName"].ToString());//刘家成L6
                sheet.GetRow(7).GetCell(11).SetCellValue(dtHeader.Rows[0]["vcUserPhone"].ToString());//刘家成电话L8
                sheet.GetRow(8).GetCell(11).SetCellValue(dtHeader.Rows[0]["vcUserEMail"].ToString());//刘家成邮箱L9
                sheet.GetRow(7).GetCell(15).SetCellValue(dtHeader.Rows[0]["vcChuanZhen"].ToString());//传真
                switch (dtHeader.Rows[0]["vcOrderType"].ToString())
                {
                    case "S/O": sheet.GetRow(10).GetCell(11).SetCellValue("√"); break;//订单类别S:L11
                    case "E/O": sheet.GetRow(10).GetCell(13).SetCellValue("√"); break;//订单类别E:N11
                    case "F/O": sheet.GetRow(10).GetCell(15).SetCellValue("√"); break;//订单类别F:P11
                }
                #endregion

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sheet.GetRow(i + 18).GetCell(0).SetCellValue(dt.Rows[i]["TASSCODE"].ToString());
                    sheet.GetRow(i + 18).GetCell(3).SetCellValue(int.Parse(dt.Rows[i]["OrderDate"].ToString()));
                    sheet.GetRow(i + 18).GetCell(6).SetCellValue(dt.Rows[i]["OrderNo"].ToString());
                    sheet.GetRow(i + 18).GetCell(9).SetCellValue(dt.Rows[i]["TYPE"].ToString());
                    sheet.GetRow(i + 18).GetCell(10).SetCellValue(dt.Rows[i]["ItemNo"].ToString());
                    sheet.GetRow(i + 18).GetCell(12).SetCellValue(dt.Rows[i]["PartNo"].ToString());
                    sheet.GetRow(i + 18).GetCell(16).SetCellValue(int.Parse(dt.Rows[i]["OrderQty"].ToString()));
                    sheet.GetRow(i + 18).GetCell(17).SetCellValue(dt.Rows[i]["PToPCust"].ToString());
                }
                strFileName = strFileName + ".xlsx";
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
        #endregion


        public string checkExcelHeadpos(DataTable dt, DataTable dtTmplate)
        {
            string msg = "";
            if (dt.Columns.Count != dtTmplate.Columns.Count)
            {
                return msg = "使用模板错误！";
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].ColumnName.ToString().Trim() != dtTmplate.Columns[i].ColumnName.ToString().Trim())
                {
                    if (ExcelPos(i) != "error")
                        return msg = "模板" + ExcelPos(i) + "列错误！";
                }
            }
            return msg;
        }
        public string ExcelPos(int i)
        {
            //取得列位置

            string re = "error";
            List<string> A = new List<string>();
            A.Add("A");
            A.Add("B");
            A.Add("C");
            A.Add("D");
            A.Add("E");
            A.Add("F");
            A.Add("G");
            A.Add("H");
            A.Add("I");
            A.Add("J");
            A.Add("K");
            A.Add("L");
            A.Add("M");
            A.Add("N");
            A.Add("O");
            A.Add("P");
            A.Add("Q");
            A.Add("R");
            A.Add("S");
            A.Add("T");
            A.Add("U");
            A.Add("V");
            A.Add("W");
            A.Add("X");
            A.Add("Y");
            A.Add("Z");
            if (i < 26) re = A[i];
            if (i >= 26) re = A[(i / 26) - 1] + A[i % 26];
            return re;
        }
        public string checkExcelData(DataTable dt)
        {
            string msg = "";

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    //if (dt.Rows[i][0].ToString() == "") break;
            //    string vcMonth = dt.Rows[i][0].ToString();
            //    string partsno = dt.Rows[i][1].ToString().Trim();
            //    int SRNum = Convert.ToInt32(dt.Rows[i][2]);
            //    int NQC = Convert.ToInt32(dt.Rows[i][3]);
            //    int XZNum = Convert.ToInt32(dt.Rows[i][4]);
            //    int FZNum = Convert.ToInt32(dt.Rows[i][6]);
            //    if (vcMonth.Length <= 0)
            //    {
            //        msg = "第" + (i + 2) + "行，对象月不能为空。";
            //    }

            //    if (partsno.Length != 10 && partsno.Length != 12)
            //    {
            //        msg = "第" + (i + 2) + "行，PartsNo输入格式错误。";
            //        return msg;
            //    }

            //    DataTable dttmp = this.dataAccess.getResult(vcMonth, partsno);
            //    if (dttmp.Rows.Count == 0)
            //    {
            //        msg = "对象月：" + vcMonth + ",品番：" + partsno + "信息不存在或已经发注不可以再计算。";
            //        return msg;
            //    }
            //    if (FZNum % SRNum != 0)
            //    {
            //        msg = "对象月：" + vcMonth + ",品番：" + partsno + "数量应为收容数的倍数。";
            //        return msg;
            //    }
            //}

            return msg;
        }
        public DataTable GetSaleuser(string user)
        {
            return dataAccess.GetSaleuser(user);
        }
        public DataTable GetUser(string userid)
        {
            return dataAccess.GetUser(userid);
        }

        public DataTable searchAddFZ(string mon, string partsno)
        {
            return dataAccess.searchAddFZ(mon, partsno);
        }
        public DataTable searchAddFinsh(string mon, string partsno)
        {
            return dataAccess.searchAddFinsh(mon, partsno);
        }
        public string updateAddFZ(DataTable dt, string userid)
        {
            return dataAccess.updateAddFZ(dt, userid);
        }
        public string UpdateAddFZIM(DataTable dt, string user)
        {
            return dataAccess.UpdateAddFZIM(dt, user);
        }

        public string UpdateSSP(DataTable dt, string strUser)
        {
            return dataAccess.updSSP(dt, strUser);
        }
        public string UpdateAddSSP(DataTable dt, string strUser)
        {
            return dataAccess.updAddSSP(dt, strUser);
        }
    }

}