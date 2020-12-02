/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	构成件发注管理					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/08/29							
* 	类名			    :	FS1206_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1206_Logic
    {
        FS1206_DataAccess dataAccess = new FS1206_DataAccess();
        public DataTable Search(string strPartsNo, string mon)
        {
            return dataAccess.Search(strPartsNo, mon);

        }
        public string UpdateTable(DataTable dt, string userid)
        {
            return dataAccess.UpdateTable(dt, userid);
        }

        public string checkExcel(string excelpath, ref DataTable dtre, DataTable dtTmplate)
        {
            string msg = "";
            //QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();

            //DataTable dt = oQMExcel.GetExcelContentByOleDb(excelpath);//导入文件

            //msg = checkExcelHeadpos(dt, dtTmplate);//校验模板
            //if (msg.Length > 0) return msg;
            //msg = checkExcelData(dt);//校验数据
            //dtre = dt;
            return msg;
        }

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
        public string checkExcelData(DataTable dt)
        {
            string msg = "";
            DataTable dt_chk = new DataTable();

            for (int i = 1; i < dt.Rows.Count; i++)
            {
                string vcPartsNo = dt.Rows[i][0].ToString().Trim().ToUpper();
                string vcPartsNoFZ = dt.Rows[i][1].ToString().Trim().ToUpper();
                string vcSourse = dt.Rows[i][2].ToString().Trim().ToUpper();
                string iSRNum = dt.Rows[i][5].ToString().Trim();

                if (vcPartsNo.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，品番不能为空。";
                    return msg;
                }
                if (vcPartsNo.Length != 10 && vcPartsNo.Length != 12)
                {
                    msg = "第" + (i + 2) + "行，品番格式不正确。";
                    return msg;
                }

                if (vcPartsNoFZ.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，发注品番不能为空。";
                    return msg;
                }
                if (vcPartsNoFZ.Length != 10 && vcPartsNoFZ.Length != 12)
                {
                    msg = "第" + (i + 2) + "行，发注品番格式不正确。";
                    return msg;
                }
                if (vcSourse.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，SOURCE不能为空。";
                    return msg;
                }

                if (iSRNum.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，收容数不能为空。";
                    return msg;
                }

            }
            return msg;
        }
        #region 判断Excel是否有重复行
        /// <summary>
        /// 判断Excel是否有重复行
        /// </summary>
        /// <param name="InputFile"></param>
        /// <returns></returns>
        public string CheckRepeat_Excel(string InputFile)
        {
            //QMExcel oQMExcel = new QMExcel();
            //string[] primaryKeys = { "品番", "发注品番" };
            string msg = string.Empty;
            //msg = oQMExcel.CheckRepeat(InputFile, primaryKeys);
            //if (msg.Length > 0)
            //    return "模板含重复行[" + msg + "]";
            return msg;
        }
        #endregion

        public string ExcelPos(int i)//取得列位置
        {
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

        public string InUpdeOldData(DataTable dt, string useid)
        {
            return dataAccess.InUpdeOldData(dt, useid);
        }

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
            //        string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1206.xlt");
            //        string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
            //        oQMExcel.ExportFromTemplate(dt, tmplatePath, path, 2, 1, true);

            //        dt.Dispose();
            //    }
            //    else
            //    {
            //        msg = "无检索数据,无法导出数据";
            //    }
                              
            //}
            //catch (WebException ex)
            //{
            //    LogHelper.ErrorLog("构成件发注管理导出异常：" + ex.Message);
            //    msg = "导出失败！";
            //}
            return msg;
        }

        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>msg</returns>
        //public string FileImport(HttpPostedFile file, string userId)
        //{
        //    //FileInfo file_im = new FileInfo(file.FileName);
        //    //string msg;
        //    //if (file_im.Extension != ".xls" && file_im.Extension != ".xlt" && file_im.Extension != ".xlsx")
        //    //{
        //    //    return "文件格式不正确！";
        //    //}
        //    //QMExcel oQMExcel = new QMExcel();
        //    //string path;
        //    //string tepath = HttpContext.Current.Server.MapPath("~/Templates/FS1206.xlt");
        //    //if (file_im.Extension == ".xls" || file_im.Extension == ".xlt")
        //    //{
        //    //    path = HttpContext.Current.Server.MapPath("~/Temps/FS1206.xls");
        //    //}
        //    //else
        //    //{
        //    //    path = HttpContext.Current.Server.MapPath("~/Temps/FS1206.xlsx");
        //    //}
        //    //if (File.Exists(path))
        //    //{
        //    //    File.Delete(path);
        //    //}
        //    //file.SaveAs(path);
        //    //DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(tepath);//导入Excel文件到DataTable
        //    //DataTable dt = new DataTable();
        //    //msg = checkExcel(path, ref dt, dtTmplate);//检验导入的数据
        //    //if (msg.Length > 0)
        //    //{
        //    //    return msg;
        //    //}
        //    //msg = CheckRepeat_Excel(path);
        //    //if (msg.Length > 0)
        //    //{
        //    //    return msg;
        //    //}
        //    //dataAccess.UpdateTable(dt, userId);//将导入的表格数据上传

        //    return "";
        //}

    }
}