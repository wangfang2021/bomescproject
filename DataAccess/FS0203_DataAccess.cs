﻿using System;
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
                    string vcFather = list[i]["FatherPart_Id"].ToString();
                    sbr.Append(" INSERT INTO TPartList (vcCarType,vcUseLocation,iLV,vcPart_Id,vcPart_Id_Father,vcPart_Name,vcParent,vcFileName,dOperatorTime,vcOperatorID) VALUES \r\n");
                    sbr.Append(" ('" + vcCarType + "','" + vcUseLocation + "'," + iLV + ",'" + vcPart_Id.Replace("-", "") + "','" + vcFather.Trim().Replace("-", "") + "','" + vcPart_Name + "','" + vcParent + "','" + fileName + "',GETDATE(),'" + userId + "') \r\n");

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
            string[] Father = new String[4];
            foreach (string str in strs)
            {
                if (str.Contains(startFlag))
                {
                    Father = new String[4];
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


                    if (temp[9].ToString().Trim().Equals("01") || temp[9].ToString().Trim().Equals("1"))
                    {
                        Father[0] = temp[13].ToString().Trim();
                    }
                    else if (temp[9].ToString().Trim().Equals("02") || temp[9].ToString().Trim().Equals("2"))
                    {
                        Father[1] = temp[13].ToString().Trim();
                    }
                    else if (temp[9].ToString().Trim().Equals("03") || temp[9].ToString().Trim().Equals("3"))
                    {
                        Father[2] = temp[13].ToString().Trim();
                    }
                    else if (temp[9].ToString().Trim().Equals("04") || temp[9].ToString().Trim().Equals("4"))
                    {
                        Father[3] = temp[13].ToString().Trim();
                    }



                    Hashtable tempHashtable = new Hashtable();
                    for (int i = 0; i < title.Count; i++)
                    {
                        tempHashtable.Add(title[i], tempList.Count - 1 > i ? tempList[i] : "");
                    }

                    if (temp[9].ToString().Trim().Equals("01") || temp[9].ToString().Trim().Equals("1"))
                    {
                        tempHashtable.Add("FatherPart_Id", "");
                    }
                    else if (temp[9].ToString().Trim().Equals("02") || temp[9].ToString().Trim().Equals("2"))
                    {
                        tempHashtable.Add("FatherPart_Id", Father[0]);
                    }
                    else if (temp[9].ToString().Trim().Equals("03") || temp[9].ToString().Trim().Equals("3"))
                    {
                        tempHashtable.Add("FatherPart_Id", Father[1]);
                    }
                    else if (temp[9].ToString().Trim().Equals("04") || temp[9].ToString().Trim().Equals("4"))
                    {
                        tempHashtable.Add("FatherPart_Id", Father[2]);
                    }
                    else if (temp[9].ToString().Trim().Equals("05") || temp[9].ToString().Trim().Equals("5"))
                    {
                        tempHashtable.Add("FatherPart_Id", Father[3]);
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

        public void importSPRL(DataTable dt, string fileName, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                string carType = fileName.Substring(0, 4);
                string vcPlant = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(dt.Rows[i]["vcPlant"].ToString()))
                    {
                        vcPlant = dt.Rows[i]["vcPlant"].ToString();
                        break;
                    }
                }
                string FileNameTJ = vcPlant + "_" + "SPRL" + carType;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbr.Append(" INSERT INTO TSBManager (vcSPINo,vcPart_Id_new,vcFinishState,vcCarType,vcChange,vcBJDiff,vcPartName,vcStartYearMonth,vcFXDiff,vcFXNo,vcNewProj,dNewProjTime,vcFileName,vcFileNameTJ,vcOperatorId,dOperatorTime,vcType) \r\n");
                    sbr.Append(" values ( \r\n");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSPINo"].ToString().Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPart_Id_new"].ToString().Replace("-", "").Trim(), false) + ",");
                    sbr.Append("'0',");
                    sbr.Append("'" + carType + "',");
                    sbr.Append("'新车新设',");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcBJDiff"].ToString().Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPartName"].ToString().Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcStartYearMonth"].ToString().Replace("*", "").Replace("/", "").Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcFXDiff"].ToString().Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcFXNo"].ToString().Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNewProj"].ToString().Trim(), false) + ",");
                    sbr.Append(ComFunction.getSqlValue(dt.Rows[i]["vcStartYearMonth"].ToString().Replace("**", "/01").Replace("/", "").Trim(), true) + ",");
                    sbr.Append("'" + fileName + "',");
                    sbr.Append("'" + FileNameTJ + "',");
                    sbr.Append("'" + userId + "',");
                    sbr.Append(" GETDATE(),'SPRL' as vcType ");
                    sbr.Append(" ) \r\n");
                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }

                sbr.Length = 0;
                sbr.Append(" INSERT INTO TSPIHistory (vcFileName,vcRemark,vcType,vcOperatorID,dOperatorTime) \r\n");
                sbr.Append(" values ( \r\n");
                sbr.Append(" '" + fileName + "',");
                sbr.Append("'',");
                sbr.Append("'1',");
                sbr.Append("'" + userId + "',");
                sbr.Append(" GETDATE() ) \r\n");

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }

                sbr.Length = 0;
                sbr.Append(" INSERT INTO dbo.TSBFile (vcFileNameTJ,vcState,vcRemark,vcOperatorId,dOperatorTime) \r\n");
                sbr.Append(" values ( \r\n");
                sbr.Append(" '" + FileNameTJ + "','0','','" + userId + "',GETDATE()) \r\n ");
                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}