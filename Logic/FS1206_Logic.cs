using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;
using System;
using System.Linq;
using System.Text;

namespace Logic
{
    public class FS1206_Logic
    {
        FS1206_DataAccess dataAccess = new FS1206_DataAccess();
        public DataTable Search(string strPartsNo, string mon)
        {
            return dataAccess.Search(strPartsNo, mon);
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public string UpdateTable(DataTable dt, string userid)
        {
            return dataAccess.UpdateTable(dt, userid);
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