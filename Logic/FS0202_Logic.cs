﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0202_Logic
    {
        FS0202_DataAccess fs0202_dataAccess = new FS0202_DataAccess();
        public DataTable searchHistory(string filename, string timefrom, string timeto)
        {
            DataTable dt = fs0202_dataAccess.searchHistory(filename, timefrom, timeto);
            return dt;
        }

        #region 读取部品文本文件
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
        public DataTable GetPartDataTable(List<Hashtable> list)
        {
            DataTable dt = new DataTable();
            string[] field = { };

            for (int i = 0; i < field.Length; i++)
            {
                dt.Columns.Add(field[i]);
            }

            for (int i = 0; i < list.Count; i++)
            {

                DataRow row = dt.NewRow();
                for (int j = 0; j < field.Length; j++)
                {
                    row[field[j]] = list[i][field[j]];
                }

                dt.Rows.Add(row);
            }

            return dt;
        }
        #endregion


    }
}
