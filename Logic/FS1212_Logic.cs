/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	品番基础数据维护					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/09/16							
* 	类名			    :	FS1212_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using Common;
using System.Collections;
using System.Linq;

namespace Logic
{
    public class FS1212_Logic
    {
        FS1212_DataAccess dataAccess = new FS1212_DataAccess();

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>msg</returns>
        //public string FileImport(HttpPostedFile file, string userId)
        //{
        //    FileInfo file_im = new FileInfo(file.FileName);
        //    string msg = null;
        //    if (file_im.Extension != ".xls" && file_im.Extension != ".xlt" && file_im.Extension != ".xlsx")
        //    {
        //        return "必须导入Excel格式文件！";
        //    }
        //    try
        //    {
        //        QMExcel oQMExcel = new QMExcel();
        //        string path;
        //        string serverfilename = "FS12120000" + Guid.NewGuid().ToString();
        //        if (file_im.Extension == ".xls" || file_im.Extension == ".xlt")
        //        {
        //            path = HttpContext.Current.Server.MapPath("~/Temps/" + serverfilename + ".xls");
        //        }
        //        else
        //        {
        //            path = HttpContext.Current.Server.MapPath("~/Temps/" + serverfilename + ".xlsx");
        //        }
        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //        }
        //        file.SaveAs(path);
        //        string headColErr = CheckHead_standTime(path);// 判断导入文件格式是否正确
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
        //        msg = CheckRepeat_Excel(path);//判断是否有重复行--品番--受入
        //        if(!string.IsNullOrEmpty(msg))
        //        {
        //            return msg;
        //        }
        //        msg = CheckRepeat_ExcelDBTypeZB(path);//检查生产部署和组别
        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            return msg;
        //        }
        //        //字段格式是否正确
        //        msg = CheckGeShi_Excel(path);
        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            return msg;
        //        }
        //        int i = ImportStandTime(path, userId);//将Excel的内容导入到数据库中
        //    } catch(WebException ex) {
        //        LogHelper.ErrorLog("品番基础数据导入异常："+ex.Message);
        //        msg = "警告信息：导入失败,Excel出错,请确认...！";
        //    }
        //    return msg;
        //}

        #region 将Excel的内容导入到数据库中
        /// <summary>
        /// 将Excel的内容导入到数据库中
        /// </summary>
        /// <param name="InputFile">导入Html控件</param>
        /// <param name="vcCreaterId">创建者ID</param>
        //public int ImportStandTime(string InputFile, string vcCreaterId)
        //{
        //    try
        //    {
        //        QMExcel oQMExcel = new QMExcel();
        //        DataTable dt = oQMExcel.GetExcelContentByOleDb(InputFile);
        //        int count = dt.Rows.Count;
        //        dt.Columns[0].ColumnName = "vcPartsNo";
        //        dt.Columns[1].ColumnName = "dTimeFrom";
        //        dt.Columns[2].ColumnName = "dTimeTo";
        //        dt.Columns[3].ColumnName = "vcDock";
        //        dt.Columns[4].ColumnName = "vcPartPlant";
        //        dt.Columns[5].ColumnName = "vcCarFamilyCode";
        //        dt.Columns[6].ColumnName = "vcPartsNameEN";
        //        dt.Columns[7].ColumnName = "vcPartsNameCHN";
        //        dt.Columns[8].ColumnName = "vcQFflag";
        //        dt.Columns[9].ColumnName = "iQuantityPerContainer";
        //        dt.Columns[10].ColumnName = "vcQJcontainer";
        //        dt.Columns[11].ColumnName = "vcPorType";
        //        dt.Columns[12].ColumnName = "vcZB";
        //        dt.Columns[13].ColumnName = "vcPartFrequence";
        //        dt.Columns.Remove("vcPartsNameEN");
        //        dt.Columns.Remove("vcPartsNameCHN");
        //        dt.Columns.Remove("iQuantityPerContainer");
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            if (dt.Rows[i]["vcQFflag"].ToString() == "○")
        //            {
        //                dt.Rows[i]["vcQFflag"] = "1";
        //            }
        //            if (dt.Rows[i]["vcQFflag"].ToString() == "×")
        //            {
        //                dt.Rows[i]["vcQFflag"] = "2";
        //            }
        //            if (dt.Rows[i]["vcPartPlant"].ToString() == "#1")
        //            {
        //                dt.Rows[i]["vcPartPlant"] = "1";
        //            }
        //            if (dt.Rows[i]["vcPartPlant"].ToString() == "#2")
        //            {
        //                dt.Rows[i]["vcPartPlant"] = "2";
        //            }
        //            if (dt.Rows[i]["vcPartPlant"].ToString() == "#3")
        //            {
        //                dt.Rows[i]["vcPartPlant"] = "3";
        //            }
        //            if (dt.Rows[i]["vcPartPlant"].ToString() == "#4")
        //            {
        //                dt.Rows[i]["vcPartPlant"] = "4";
        //            }
        //        }
                
        //        dataAccess.DoTransactionOfInsert(dt, vcCreaterId);//按照行来更新数据
        //        return 11;
        //    }
        //    catch (WebException ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        #region 判断Excel字段格式
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
        //    dt.Columns[0].ColumnName = "vcPartsNo";
        //    dt.Columns[1].ColumnName = "dTimeFrom";
        //    dt.Columns[2].ColumnName = "dTimeTo";
        //    dt.Columns[3].ColumnName = "vcDock";
        //    dt.Columns[4].ColumnName = "vcPartPlant";
        //    dt.Columns[5].ColumnName = "vcCarFamilyCode";
        //    dt.Columns[6].ColumnName = "vcPartsNameEN";
        //    dt.Columns[7].ColumnName = "vcPartsNameCHN";
        //    dt.Columns[8].ColumnName = "vcQFflag";
        //    dt.Columns[9].ColumnName = "iQuantityPerContainer";
        //    dt.Columns[10].ColumnName = "vcQJcontainer";
        //    dt.Columns[11].ColumnName = "vcPorType";
        //    dt.Columns[12].ColumnName = "vcZB";
        //    dt.Columns[13].ColumnName = "vcPartFrequence";
        //    dt.Columns.Remove("vcPartsNameEN");
        //    dt.Columns.Remove("vcPartsNameCHN");
        //    dt.Columns.Remove("iQuantityPerContainer");
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        //20180910之前所有的行号提醒是i + 1行，实际上除了序号第一个为0的问题外，还有首行标题也占一行，因此提示应该为i + 2 - 李兴旺
        //        if (dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "").Length != 12)
        //        {
        //            msg = "Excel第" + (i + 2) + "行品番格式错误";
        //            return msg;
        //        }
        //        if (dt.Rows[i]["vcDock"].ToString().Replace("-", "").Length != 2)
        //        {
        //            msg = "Excel第" + (i + 2) + "行受入格式错误";
        //            return msg;
        //        }
        //        if (!(dt.Rows[i]["vcPartPlant"].ToString().Replace("-", "") == "#1" || dt.Rows[i]["vcPartPlant"].ToString().Replace("-", "") == "#2" || dt.Rows[i]["vcPartPlant"].ToString().Replace("-", "") == "#3" || dt.Rows[i]["vcPartPlant"].ToString().Replace("-", "") == "#4"))
        //        {
        //            msg = "Excel第" + (i + 2) + "行品番工场格式错误";
        //            return msg;
        //        }
        //        if (!(dt.Rows[i]["vcQFflag"].ToString().Replace("-", "") == "○" || dt.Rows[i]["vcQFflag"].ToString().Replace("-", "") == "×"))
        //        {
        //            msg = "Excel第" + (i + 2) + "行秦丰涂装格式错误";
        //            return msg;
        //        }
        //        try
        //        {
        //            Convert.ToInt32(dt.Rows[i]["vcQJcontainer"].ToString());
        //        }
        //        catch
        //        {
        //            msg = "Excel第" + (i + 2) + "行器具收容数格式错误";
        //            return msg;
        //        }
        //        if (dt.Rows[i]["vcPorType"].ToString().Length > 4)
        //        {
        //            msg = "Excel第" + (i + 2) + "行生产部署格式错误";
        //            return msg;
        //        }
        //        //20180908增加品番频度内容格式判断 - 李兴旺
        //        if (!(dt.Rows[i]["vcPartFrequence"].ToString() == "月度" || dt.Rows[i]["vcPartFrequence"].ToString() == "周度" || string.IsNullOrEmpty(dt.Rows[i]["vcPartFrequence"].ToString())))
        //        {
        //            msg = "Excel第" + (i + 2) + "行品番频度内容不正确";
        //            return msg;
        //        }
        //    }
        //    return "";
        //}
        #endregion

        #region 检查生产部署和组别
        /// <summary>
        /// 检查生产部署和组别
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckRepeat_ExcelDBTypeZB(string InputFile)
        //{
        //    QMExcel oQMExcel = new QMExcel();
        //    DataTable dt = oQMExcel.GetExcelContentByOleDb(InputFile);
        //    int count = dt.Rows.Count;
            
        //    dt.Columns[0].ColumnName = "vcPartsNo";
        //    dt.Columns[1].ColumnName = "dTimeFrom";
        //    dt.Columns[2].ColumnName = "dTimeTo";
        //    dt.Columns[3].ColumnName = "vcDock";
        //    dt.Columns[4].ColumnName = "vcPartPlant";
        //    dt.Columns[5].ColumnName = "vcCarFamilyCode";
        //    dt.Columns[6].ColumnName = "vcPartsNameEN";
        //    dt.Columns[7].ColumnName = "vcPartsNameCHN";
        //    dt.Columns[8].ColumnName = "vcQFflag";
        //    dt.Columns[9].ColumnName = "iQuantityPerContainer";
        //    dt.Columns[10].ColumnName = "vcQJcontainer";
        //    dt.Columns[11].ColumnName = "vcPorType";
        //    dt.Columns[12].ColumnName = "vcZB";
        //    dt.Columns[13].ColumnName = "vcPartFrequence";
        //    dt.Columns.Remove("vcPartsNameEN");
        //    dt.Columns.Remove("vcPartsNameCHN");
        //    dt.Columns.Remove("iQuantityPerContainer");

        //    DataTable dtt = dt.Clone();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        DataTable ds = dataAccess.SearchPartData(dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
        //        if (ds.Rows.Count == 0)
        //        {
        //            return "Excel数据中存在生产部署、组别未在数据库中维护的数据";
        //        }
        //    }
        //    return "";
        //}
        #endregion

        #region 判断Excel是否有重复行
        /// <summary>
        /// 判断Excel是否有重复行
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckRepeat_Excel(string InputFile)
        //{
        //    QMExcel oQMExcel = new QMExcel();
        //    string[] primaryKeys = { "品番", "开始时间", "截止时间", "受入", "品番工场", "车型" };
        //    string msg = string.Empty;
        //    msg = oQMExcel.CheckRepeat(InputFile, primaryKeys);
        //    if (msg.Length > 0)
        //        return "模板含重复行[" + msg + "]";
        //    return msg;
        //}
        #endregion

        #region 判断导入文件格式是否正确
        /// <summary>
        /// 判断导入文件格式是否正确
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        //public string CheckHead_standTime(string InputFile)
        //{
        //    QMExcel oQMExcel = new QMExcel();
        //    List<string> lists = oQMExcel.GetExcelSchema(InputFile);
        //    if (lists.Count == 14)
        //    {
        //        if (lists[0] != "品番")
        //            return "A";
        //        if (lists[1] != "开始时间")
        //            return "B";
        //        if (lists[2] != "截止时间")
        //            return "C";
        //        if (lists[3] != "受入")
        //            return "D";
        //        if (lists[4] != "品番工场")
        //            return "E";
        //        if (lists[5] != "车型")
        //            return "F";
        //        if (lists[6] != "英文品名")
        //            return "G";
        //        if (lists[7] != "中文品名")
        //            return "H";
        //        if (lists[8] != "秦丰涂装")
        //            return "I";
        //        if (lists[9] != "看板收容数")
        //            return "J";
        //        if (lists[10] != "器具收容数")
        //            return "K";
        //        if (lists[11] != "生产部署")
        //            return "L";
        //        if (lists[12] != "组别")
        //            return "M";
        //        if (lists[13] != "品番频度")
        //            return "N";
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        return "ALL";
        //    }
        //}
        #endregion

        /// <summary>
        /// 品番基础数据获取批处理
        /// </summary>
        public bool BatchProcess()
        {
           
            DataTable dt = new DataTable();
            // 定义列
            DataColumn dc_PARTSNO = new DataColumn();//品番 
            DataColumn dc_TIMEFROM = new DataColumn();//有效期起
            DataColumn dc_TIMETO = new DataColumn();//有效期止
            DataColumn dc_DOCK = new DataColumn();//受入
            DataColumn dc_CPDCOMPANY = new DataColumn();//收货方
            DataColumn dc_CARFAMILYCODE = new DataColumn();//车种代码
            DataColumn dc_INOUTFLAG = new DataColumn();//内外区分
            DataColumn dc_SUPPLIERCODE = new DataColumn();//供应商编号
            DataColumn dc_SUPPLIERPLANT = new DataColumn();//供应商工区
            DataColumn dc_QUANTITYPERCONTAINER = new DataColumn();//收容数
            DataColumn dc_PACKINGFLAG = new DataColumn();//包装区分
            DataColumn dc_PARTSNAMEEN = new DataColumn();//中文名
            DataColumn dc_PARTSNAMECHN = new DataColumn();//英文名
            DataColumn dc_PHOTOPATH = new DataColumn();//照片路径
            DataColumn dc_ROUTE = new DataColumn();//物流路径
            DataColumn dc_CURRENTPASTCODE = new DataColumn();//好旧区分
            DataColumn dc_REMARK1 = new DataColumn();//特记事项1
            DataColumn dc_REMARK2 = new DataColumn();//特记事项2

            // 定义列名
            dc_PARTSNO.ColumnName = "PARTSNO";
            dc_TIMEFROM.ColumnName = "TIMEFROM";
            dc_TIMETO.ColumnName = "TIMETO";
            dc_DOCK.ColumnName = "DOCK";
            dc_CPDCOMPANY.ColumnName = "CPDCOMPANY";
            dc_CARFAMILYCODE.ColumnName = "CARFAMILYCODE";
            dc_INOUTFLAG.ColumnName = "INOUTFLAG";
            dc_SUPPLIERCODE.ColumnName = "SUPPLIERCODE";
            dc_SUPPLIERPLANT.ColumnName = "SUPPLIERPLANT";
            dc_QUANTITYPERCONTAINER.ColumnName = "QUANTITYPERCONTAINER";
            dc_PACKINGFLAG.ColumnName = "PACKINGFLAG";
            dc_PARTSNAMEEN.ColumnName = "PARTSNAMEEN";
            dc_PARTSNAMECHN.ColumnName = "PARTSNAMECHN";
            dc_PHOTOPATH.ColumnName = "PHOTOPATH";
            dc_ROUTE.ColumnName = "ROUTE";
            dc_CURRENTPASTCODE.ColumnName = "CURRENTPASTCODE";
            dc_REMARK1.ColumnName = "REMARK1";
            dc_REMARK2.ColumnName = "REMARK2";

            // 将定义的列加入到dtTemp中
            dt.Columns.Add(dc_PARTSNO);
            dt.Columns.Add(dc_TIMEFROM);
            dt.Columns.Add(dc_TIMETO);
            dt.Columns.Add(dc_DOCK);
            dt.Columns.Add(dc_CPDCOMPANY);
            dt.Columns.Add(dc_CARFAMILYCODE);
            dt.Columns.Add(dc_INOUTFLAG);
            dt.Columns.Add(dc_SUPPLIERCODE);
            dt.Columns.Add(dc_SUPPLIERPLANT);
            dt.Columns.Add(dc_QUANTITYPERCONTAINER);
            dt.Columns.Add(dc_PACKINGFLAG);
            dt.Columns.Add(dc_PARTSNAMEEN);
            dt.Columns.Add(dc_PARTSNAMECHN);
            dt.Columns.Add(dc_PHOTOPATH);
            dt.Columns.Add(dc_ROUTE);
            dt.Columns.Add(dc_CURRENTPASTCODE);
            dt.Columns.Add(dc_REMARK1);
            dt.Columns.Add(dc_REMARK2);
            try
            {

                bool result = false;
                //批处理正常启动，记录数据库日志
                //LogHelper.WriteLog("批处理获取品番基础数据正常启动");

                if (dataAccess.GetTable(ref dt))
                {
                    if (dataAccess.TurnCate())
                    {
                        dataAccess.insertTableLN(dt);
                        ComUpdataTable();
                        result = true;
                    }
                }

                //批处理正常结束，记录数据库日志
                //LogHelper.WriteLog("获取品番基础数据结束");
                //批处理后数据的整理、比较

                return result;
            }
            catch (WebException ex)
            {

                //LogHelper.ErrorLog("批处理获取品番基础数据异常");
                throw ex;
            }
        }

        #region 比较更新新增并更新Master
        public bool ComUpdataTable()
        {
            DataTable Comda = dataAccess.CompareData();
            if (Comda.Rows.Count != 0)
            {
                return dataAccess.InsertOrUpData(Comda);
            }
            else
            {
                return true;
            }
        }
        #endregion

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="exlName"></param>
        public string FileExport(DataTable Exdt, string exlName)
        {
            string msg = null;
            try
            {
                Exdt.Columns["vcPartsNo"].SetOrdinal(0);
                Exdt.Columns["dTimeFrom"].SetOrdinal(1);
                Exdt.Columns["dTimeTo"].SetOrdinal(2);
                Exdt.Columns["vcDock"].SetOrdinal(3);
                Exdt.Columns["leibie"].SetOrdinal(4);
                Exdt.Columns["vcCarFamilyCode"].SetOrdinal(5);
                Exdt.Columns["vcPartsNameEN"].SetOrdinal(6);
                Exdt.Columns["vcPartsNameCHN"].SetOrdinal(7);
                Exdt.Columns["qinfengtz"].SetOrdinal(8);
                Exdt.Columns["iQuantityPerContainer"].SetOrdinal(9);
                Exdt.Columns["vcQJcontainer"].SetOrdinal(10);
                Exdt.Columns["shengchanbs"].SetOrdinal(11);
                Exdt.Columns["zubie"].SetOrdinal(12);
                Exdt.Columns["pinfanpindu"].SetOrdinal(13);
                for (int i = 0; i < Exdt.Rows.Count; i++)
                {
                    if (Exdt.Rows[i]["qinfengtz"].ToString() == "1")
                    {
                        Exdt.Rows[i]["qinfengtz"] = "○";
                    }
                    if (Exdt.Rows[i]["qinfengtz"].ToString() == "2")
                    {
                        Exdt.Rows[i]["qinfengtz"] = "×";
                    }
                    if (Exdt.Rows[i]["leibie"].ToString() == "1")
                    {
                        Exdt.Rows[i]["leibie"] = "#1";
                    }
                    if (Exdt.Rows[i]["leibie"].ToString() == "2")
                    {
                        Exdt.Rows[i]["leibie"] = "#2";
                    }
                    if (Exdt.Rows[i]["leibie"].ToString() == "3")
                    {
                        Exdt.Rows[i]["leibie"] = "#3";
                    }
                    if (Exdt.Rows[i]["leibie"].ToString() == "4")
                    {
                        Exdt.Rows[i]["leibie"] = "#4";
                    }
                }
                //QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
                //    string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1212.xlt");
                //    string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
                //    oQMExcel.ExportFromTemplate(Exdt, tmplatePath, path, 2, 1, true);

                Exdt.Dispose();

            }
            catch (WebException ex)
            {
                //LogHelper.ErrorLog("品番基础数据导出异常：" + ex.Message);
                msg = "导出失败！";
            }
            return msg;
        }

        ///检索数据 需要修改SQL语句关联生产部署表和组别表
        public DataTable SearchPartData(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {
            return dataAccess.SearchPartData(vcPartsNo, vcCarFamilyCode, vcPorType, vcZB, vcPartPlant, vcPartFrequence);
        }

        //导出数据
        public DataTable SearchPartDataEX(string vcPartsNo, string vcCarFamilyCode, string vcPorType, string vcZB, string vcPartPlant, string vcPartFrequence)
        {            
            return dataAccess.SearchPartDataEX(vcPartsNo, vcCarFamilyCode, vcPorType, vcZB, vcPartPlant, vcPartFrequence);
        }

        public DataTable dllPorType()
        {           
            return dataAccess.dllPorType();
        }

        public DataTable dllZB()
        {
            return dataAccess.dllZB();
        }

        public DataTable dllLB()
        {
            return dataAccess.dllLB();
        }        

        /// <summary>
        /// 检索信息栏绑定生产部署
        /// </summary>
        /// <param name="vcZB">组别</param>
        /// <returns></returns>
        public DataTable dllPorType(string vcZB)
        {
            return dataAccess.dllPorType(vcZB);
        }

        /// <summary>
        /// 检索信息栏绑定组别
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <returns></returns>
        public DataTable dllZB(string vcPorType)
        {
            return dataAccess.dllZB(vcPorType);
        }

        /// <summary>
        /// 检索信息栏绑定品番工场
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <returns></returns>
        public DataTable dllLB1(string vcPorType)
        {
            return dataAccess.dllLB1(vcPorType);
        }

        public bool checkmod(string vcPartsNo)
        {
            return dataAccess.checkmod(vcPartsNo);
        }

        /// <summary>
        /// 更新内制品品番表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="useid"></param>
        /// <returns></returns>
        public bool InUpdeOldData(DataTable dt, string useid)
        {
            return dataAccess.InUpdeOldData(dt, useid);
        }
    }
}