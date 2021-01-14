using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace Logic
{
    public class FS1502_Logic
    {
        FS1502_DataAccess fs1502_DataAccess;

        public FS1502_Logic()
        {
            fs1502_DataAccess = new FS1502_DataAccess();
        }

        #region 检索
        public DataTable Search(string dBZDate)
        {
            return fs1502_DataAccess.Search(dBZDate);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1502_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1502_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 读取txt文件
        public DataTable ReadTxtFile(string FileName,ref string strMsg)
        {
            //从指定的目录以打开或者创建的形式读取csv文件
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            //为上面创建的文件流创建读取数据流
            StreamReader read = new StreamReader(fs, System.Text.Encoding.Default);
            try
            {
                DataTable dt = new DataTable();
                Dictionary<string, int> dict = new Dictionary<string, int>();//<字段名(string),字段名对应的列号(int)>
                int irowNo = 0;
                read.BaseStream.Seek(0, SeekOrigin.Begin);
                while (read.Peek() > -1)
                {
                    irowNo = irowNo + 1;
                    string line = read.ReadLine();
                    if (irowNo == 1)
                    {//标题行
                        string[] sArray = Regex.Split(line, ",", RegexOptions.IgnoreCase);
                        for (int i = 0; i < sArray.Length; i++)
                        {
                            string strColumnName = sArray[i].ToString();
                            if (strColumnName == "SUPL" || strColumnName == "PLANT" || strColumnName == "DOCK" || strColumnName == "UNLD DTE" ||
                                strColumnName == "FRQ" || strColumnName == "PART #" || strColumnName == "FINL ORD(PCS)")
                            {
                                if (dt.Columns.Contains(strColumnName) == false)
                                {
                                    dt.Columns.Add(strColumnName);
                                    dict.Add(strColumnName, i);
                                }
                            }
                        }
                    }
                    if (irowNo >= 2)
                    {//数据行
                        DataRow dr = dt.NewRow();
                        string[] sArray = Regex.Split(line, ",", RegexOptions.IgnoreCase);
                        if (sArray.Length == 1 && sArray[0] == "")
                        {
                            //有时txt文件最后一行是空行，会导致处理失败
                        }
                        else
                        {
                            for (int i = 0; i < sArray.Length; i++)
                            {
                                foreach (KeyValuePair<string, int> kvp in dict)
                                {
                                    string columnname = kvp.Key;//字段名
                                    int columnindex = kvp.Value;//字段名对应的列号
                                    if (i == columnindex)
                                    {
                                        dr[columnname] = sArray[i].ToString();
                                    }
                                }
                            }
                            if (Convert.ToInt32(dr["FINL ORD(PCS)"].ToString()) > 0)
                            { //只有数量>0才加入到dt中
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                read.Close();
                fs.Close();
            }
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt,string vcFZPlant,string dBZDate, string strUserId)
        {
            fs1502_DataAccess.importSave_Sub(dt, vcFZPlant, dBZDate ,strUserId);
        }
        #endregion

    }
}
