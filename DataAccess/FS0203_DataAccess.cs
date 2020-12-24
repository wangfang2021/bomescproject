using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Common;

namespace DataAccess
{
    public class FS0203_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索上传列表
        public DataTable searchApi(int flag, string UploadTime)
        {
            string TableName = flag == 0 ? "TPartHistory" : "TSPIHistory";
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcFileName,vcOperatorID,dOperatorTime FROM " + TableName + " \r\n");
            sbr.Append(" WHERE 1=1  \r\n");
            if (flag == 1)
            {
                sbr.Append(" and vcType = '1' \r\n");
            }
            if (!string.IsNullOrWhiteSpace(UploadTime))
            {
                sbr.Append(" and dOperatorTime = '" + UploadTime + "' \r\n");
            }

            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }


        #endregion

        #region 添加部品表
        public void importPartList(List<Hashtable> list, string fileName, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                for (int i = 0; i < list.Count; i++)
                {
                    string vcCarType = list[i]["Model"].ToString();
                    string vcUseLocation = list[i]["Location of Usage"].ToString();
                    int iLV = Convert.ToInt32(list[i]["LV"]);
                    string vcPart_Id = list[i]["Part No."].ToString();
                    string vcPart_Name = list[i]["Part Name"].ToString();
                    string vcParent = list[i]["Parent RTG"].ToString();

                    sbr.Append(" INSERT INTO TPartList (vcCarType,vcUseLocation,iLV,vcPart_Id,vcPart_Name,vcParent,vcFileName,dOperatorTime,vcOperatorID) VALUES \r\n");
                    sbr.Append(" ('" + vcCarType + "','" + vcUseLocation + "'," + iLV + ",'" + vcPart_Id + "','" + vcPart_Name + "','" + vcParent + "','" + fileName + "',GETDATE(),'" + userId + "') \r\n");

                    if (i % 1000 == 0)
                    {
                        excute.ExcuteSqlWithStringOper(sbr.ToString());
                        sbr.Length = 0;
                    }
                }
                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }

                StringBuilder sbrList = new StringBuilder();
                sbrList.Append(" INSERT INTO TPartHistory (vcFileName,vcOperatorID,dOperatorTime,vcType) VALUES \r\n");
                sbrList.Append(" ('" + fileName + "','" + userId + "',GETDATE(),0 ) \r\n");
                excute.ExcuteSqlWithStringOper(sbrList.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 读取部品TXT
        public List<Hashtable> GetPartFromFile(string path)
        {
            string[] strs = File.ReadAllLines(@path);
            List<Hashtable> list = new List<Hashtable>();
            string startFlag = "\"Model\"	\"Location of Usage\"	\"MARK\"	\"SK\"	\"LV\"	\"GC\"	\"Part No.\"	\"MINOR\"	\"Part Name\"	\"QTY\"	\"Sel\"	\"Material\"	\"Thickness\"	\"DWG\"	\"2D\"	\"3D\"	\"M\"	\"S/P\"	\"Mft./RTG Code\"	\"Self RTG 1\"	\"Self RTG 2\"	\"Self RTG 3\"	\"Self RTG 4\"	\"Self RTG 5\"	\"Self RTG 6\"	\"Self RTG 7\"	\"Self RTG 8\"";
            bool flag = false;
            List<String> title = new List<string>()
            {
                "Model", "Location of Usage", "MARK", "SK", "LV", "GC", "Part No.", "MINOR", "Part Name", "QTY", "Sel",
                "Material", "Thickness", "DWG", "2D", "3D", "M", "S/P", "Mft./RTG Code", "Self RTG 1", "Self RTG 2",
                "Self RTG 3", "Self RTG 4", "Self RTG 5", "Self RTG 6", "Self RTG 7", "Self RTG 8", "Self RTG 9",
                "Self RTG 10", "Self RTG 11", "Self RTG 12", "Self RTG 13", "Self RTG 14", "Parent RTG", "C", "11=",
                "12=", "15=", "16=", "18=", "19=", "Prod. Comment"
            };
            foreach (string str in strs)
            {
                if (str.Contains(startFlag))
                {
                    flag = true;
                    continue;
                }
                if (flag == true && !str.Equals("\"\""))
                {
                    List<string> tempList = new List<string>();
                    string[] temp = str.Replace("\t", "").Split('\"');
                    for (int i = 0; i < temp.Length - 1; i++)
                    {
                        if (i % 2 == 1)
                            tempList.Add(temp[i]);
                    }

                    Hashtable tempHashtable = new Hashtable();
                    for (int i = 0; i < title.Count; i++)
                    {
                        tempHashtable.Add(title[i], tempList.Count - 1 > i ? tempList[i] : "");
                    }
                    list.Add(tempHashtable);
                }
                else if (flag == true && str.Equals("\"\""))
                {
                    flag = false;
                }
            }

            return list;
        }
        #endregion
    }
}