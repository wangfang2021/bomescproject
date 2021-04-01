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
    public class FS1202_Logic
    {
        FS1202_DataAccess dataAccess = new FS1202_DataAccess();

        public DataTable dt_GetSearch(string ddlpro, string ddlgroup)
        {
            return dataAccess.dt_GetSearch(ddlpro, ddlgroup);
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

        public DataTable bindProType()
        {
            return dataAccess.bindProType();
        }

        public DataTable bindZB()
        {
            return dataAccess.bindZB();
        }

        public DataTable bindProType(string zb)
        {
            return dataAccess.bindProType(zb);
        }


        public DataTable bindZB(string Protype)
        {
            return dataAccess.bindZB(Protype);
        }

        public string checkExcel2(string filepath, ref DataTable dtre, ref DataTable dtParts, DataTable dtTmplate)
        {
            string msg = "";
            //QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
            //// FileInfo info = new FileInfo("..\\Template\\FS0130_SOQ.xlt");
            //DataTable dt = oQMExcel.GetExcelContentByOleDb(filepath);//导入文件


            ////   DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(info.FullName);//模板文件
            //msg = checkExcelHeadpos(dt, dtTmplate);//校验模板
            //if (msg.Length > 0) return msg;
            //msg = checkExcelData2(dt);//校验数据
            //dtre = dt;
            return msg;
        }
        public string checkExcelData2(DataTable dt)
        {
            string msg = "";
            //DataTable dt_Pro = dataAccess.getProInfo();
            DataTable dt_sa = dataAccess.bindState();
            DataTable dt_sz = dataAccess.getsz();
            DataTable dt_pt = dataAccess.getPartType();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //if (dt.Rows[i][0].ToString() == "") break;
                //string protype = dt.Rows[i][0].ToString() + "-" + dt.Rows[i][1].ToString();

                try
                {
                    if (dt.Select("vcPorType='" + dt.Rows[i]["vcPorType"].ToString().ToUpper() + "' and vcZB='" + dt.Rows[i]["vcZB"].ToString().ToUpper() + "'").Length > 1)
                    {
                        msg = " 第" + (i + 2) + "行,此生产部署和组别的导入数据不唯一，导入失败！";
                        break;
                    }
                }
                catch
                {
                    msg = "第" + (i + 2) + "行,此生产部署和组别的导入数据不唯一，导入失败！";
                    break;
                }

                string sz = dt.Rows[i][0].ToString().Trim();
                string zb = dt.Rows[i][1].ToString().Trim();
                string pt = dt.Rows[i][2].ToString().Trim();

                string na0 = dt.Rows[i][3].ToString();
                string lt0 = dt.Rows[i][4].ToString();
                string sa0 = dt.Rows[i][5].ToString();

                string na1 = dt.Rows[i][6].ToString();
                string lt1 = dt.Rows[i][7].ToString();
                string sa1 = dt.Rows[i][8].ToString();

                string na2 = dt.Rows[i][9].ToString();
                string lt2 = dt.Rows[i][10].ToString();
                string sa2 = dt.Rows[i][11].ToString();

                string na3 = dt.Rows[i][12].ToString();
                string lt3 = dt.Rows[i][13].ToString();
                string sa3 = dt.Rows[i][14].ToString();

                string na4 = dt.Rows[i][15].ToString();
                string lt4 = dt.Rows[i][16].ToString();
                string sa4 = dt.Rows[i][17].ToString();

                if (sz.Length == 0 || zb.Length == 0)
                {
                    msg = "行：" + (i + 2) + "，生产部署和组别不能为空！";
                    break;
                }
                if (pt.Length == 0)
                {
                    msg = "行：" + (i + 2) + "，品番类别不能为空！";
                    break;
                }

                if (dt_sz.Select(" vcData1='" + sz + "' ").Length == 0)
                {
                    msg = "行：" + (i + 2) + "，生产部署数据在常量中不存在！";
                    break;
                }
                if (dt_sz.Select(" vcData3='" + zb + "' ").Length == 0)
                {
                    msg = "行：" + (i + 2) + "，组别数据在常量中不存在！";
                    break;
                }
                if (dt_pt.Select(" vcName='" + pt + "' ").Length == 0 && pt.Trim() != "")
                {
                    msg = "行：" + (i + 2) + "，品番类别数据在常量中不存在！";
                    break;
                }


                try
                {
                    if (na0 == "" || lt0 == "" || sa0 == "")
                    {
                        //if (na0 == "" || lt0 == "" || sa0 == "")
                        //{
                        msg = "行：" + (i + 2) + "，工程0信息不完善！";
                        break;
                        //}
                    }
                    else
                    {
                        if (na0.Length > 10)
                        {
                            msg = "行：" + (i + 2) + "，工程0 名称格式输入错误";
                            break;
                        }
                        if (lt0.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(lt0, "^[-]?\\d*$") == false)
                        {
                            msg = "行：" + (i + 2) + "，工程0 LT格式输入错误";
                            break;
                        }
                        if (dt_sa.Select(" vcName='" + sa0 + "' ").Length == 0)
                        {
                            msg = "行：" + (i + 2) + "，工程0 稼动形态格式输入错误";
                            break;
                        }
                    }
                }
                catch
                {
                    msg = "第" + (i + 2) + "行，工程0 中存在错误信息！";
                    break;
                }
                try
                {
                    if (na1 == "" || lt1 == "" || sa1 == "")
                    {
                        //if (na1 == "" || lt1 == "" || sa1 == "")
                        //{
                        msg = "行：" + (i + 2) + "，工程1信息不完善！";
                        break;
                        //}
                    }
                    else
                    {
                        if (na1.Length > 11)
                        {
                            msg = "行：" + (i + 2) + "，工程1 名称格式输入错误";
                            break;
                        }
                        if (lt1.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(lt1, "^[-]?\\d*$") == false)
                        {
                            msg = "行：" + (i + 2) + "，工程1 LT格式输入错误";
                            break;
                        }
                        if (dt_sa.Select(" vcName='" + sa1 + "' ").Length == 0)
                        {
                            msg = "行：" + (i + 2) + "，工程1 稼动形态格式输入错误";
                            break;
                        }
                    }

                }
                catch
                {
                    msg = "第" + (i + 2) + "行，工程1 中存在错误信息！";
                    break;
                }
                try
                {
                    if (na2 != "" || lt2 != "" || sa2 != "")
                    {
                        if (na2 == "" || lt2 == "" || sa2 == "")
                        {
                            msg = "行：" + (i + 2) + "，工程2信息不完善！";
                            break;
                        }
                        else
                        {
                            if (na2.Length > 12)
                            {
                                msg = "行：" + (i + 2) + "，工程2 名称格式输入错误";
                                break;
                            }
                            if (lt2.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(lt2, "^[-]?\\d*$") == false)
                            {
                                msg = "行：" + (i + 2) + "，工程2 LT格式输入错误";
                                break;
                            }
                            if (dt_sa.Select(" vcName='" + sa2 + "' ").Length == 0)
                            {
                                msg = "行：" + (i + 2) + "，工程2 稼动形态格式输入错误";
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    msg = "第" + (i + 2) + "行，工程2 中存在错误信息！";
                    break;
                }
                try
                {
                    if (na3 != "" || lt3 != "" || sa3 != "")
                    {
                        if (na3 == "" || lt3 == "" || sa3 == "")
                        {
                            msg = "行：" + (i + 2) + "，工程3信息不完善！";
                            break;
                        }
                        else
                        {
                            if (na3.Length > 13)
                            {
                                msg = "行：" + (i + 2) + "，工程3 名称格式输入错误";
                                break;
                            }
                            if (lt3.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(lt3, "^[-]?\\d*$") == false)
                            {
                                msg = "行：" + (i + 2) + "，工程3 LT格式输入错误";
                                break;
                            }
                            if (dt_sa.Select(" vcName='" + sa3 + "' ").Length == 0)
                            {
                                msg = "行：" + (i + 2) + "，工程3 稼动形态格式输入错误";
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    msg = "第" + (i + 2) + "行，工程3 中存在错误信息！";
                    break;
                }
                try
                {
                    if (na4 == "" || lt4 == "" || sa4 == "")
                    {
                        //if (na4 == "" || lt4 == "" || sa4 == "")
                        //{
                        msg = "行：" + (i + 2) + "，工程4信息不完善！";
                        break;
                        //}
                    }
                    else
                    {
                        if (na4.Length > 14)
                        {
                            msg = "行：" + (i + 2) + "，工程4 名称格式输入错误";
                            break;
                        }
                        if (lt4.Length > 3 || System.Text.RegularExpressions.Regex.IsMatch(lt4, "^[-]?\\d*$") == false)
                        {
                            msg = "行：" + (i + 2) + "，工程4 LT格式输入错误";
                            break;
                        }
                        if (dt_sa.Select(" vcName='" + sa4 + "' ").Length == 0)
                        {
                            msg = "行：" + (i + 2) + "，工程4 稼动形态格式输入错误";
                            break;
                        }
                    }

                }
                catch
                {
                    msg = "第" + (i + 2) + "行，工程4 中存在错误信息！";
                    break;
                }




                #region

                //string PZlg = dt.Rows[i][1].ToString();
                //string Pro0 = dt.Rows[i][5].ToString();
                //string Pro1 = dt.Rows[i][8].ToString();
                //string Pro2 = dt.Rows[i][11].ToString();
                //string Pro3 = dt.Rows[i][14].ToString();
                //string Pro4 = dt.Rows[i][17].ToString();
                //if (protype.Length > 4 || protype.Length==0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(0) + ",生产部署格式输入错误";
                //    break;
                //}
                //if (PZlg.Length > 3 || PZlg.Length == 0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(2) + ",组别格式输入错误";
                //    break;
                //}


                //if (dt.Rows[i][3].ToString().Length > 10 || dt.Rows[i][3].ToString().Length==0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(3) + ",工程0名称格式输入错误";
                //    break;
                //}
                //try
                //{
                //    if (Convert.ToInt32(dt.Rows[i][4]) < 0 || dt.Rows[i][4].ToString().Length == 0)
                //    {
                //        msg = "行：" + (i + 2) + "，列：" + ExcelPos(4) + ",工程0LT格式输入错误";
                //        break;
                //    }
                //}
                //catch
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(4) + ",工程0LT格式输入错误";
                //    break;
                //}

                //if (dt.Rows[i][6].ToString().Length > 10 || dt.Rows[i][6].ToString().Length == 0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(6) + ",工程1名称格式输入错误";
                //    break;
                //}
                //try
                //{
                //    if (Convert.ToInt32(dt.Rows[i][7]) < 0 || dt.Rows[i][7].ToString().Length == 0)
                //    {
                //        msg = "行：" + (i + 2) + "，列：" + ExcelPos(7) + ",工程1LT格式输入错误";
                //        break;
                //    }
                //}
                //catch
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(7) + ",工程1LT格式输入错误";
                //    break;
                //}

                //if (dt.Rows[i][9].ToString().Length > 10 || dt.Rows[i][9].ToString().Length == 0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(9) + ",工程2名称格式输入错误";
                //    break;
                //}
                //try
                //{
                //    if (Convert.ToInt32(dt.Rows[i][10]) < 0 || dt.Rows[i][10].ToString().Length == 0)
                //    {
                //        msg = "行：" + (i + 2) + "，列：" + ExcelPos(10) + ",工程2LT格式输入错误";
                //        break;
                //    }
                //}
                //catch
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(10) + ",工程2LT格式输入错误";
                //    break;
                //}

                //if (dt.Rows[i][12].ToString().Length > 10 || dt.Rows[i][12].ToString().Length == 0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(12) + ",工程3名称格式输入错误";
                //    break;
                //}
                //try
                //{
                //    if (Convert.ToInt32(dt.Rows[i][13]) < 0 || dt.Rows[i][13].ToString().Length == 0)
                //    {
                //        msg = "行：" + (i + 2) + "，列：" + ExcelPos(10) + ",工程3LT格式输入错误";
                //        break;
                //    }
                //}
                //catch
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(13) + ",工程3LT格式输入错误";
                //    break;
                //}
                //if (dt.Rows[i][15].ToString().Length > 10 || dt.Rows[i][15].ToString().Length == 0)
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(15) + ",工程3名称格式输入错误";
                //    break;
                //}
                //try
                //{
                //    if (Convert.ToInt32(dt.Rows[i][16]) < 0 || dt.Rows[i][16].ToString().Length == 0)
                //    {
                //        msg = "行：" + (i + 2) + "，列：" + ExcelPos(16) + ",工程3LT格式输入错误";
                //        break;
                //    }
                //}
                //catch
                //{
                //    msg = "行：" + (i + 2) + "，列：" + ExcelPos(16) + ",工程3LT格式输入错误";
                //    break;
                //}



                //if (dt_Pro.Select("Pro ='"+protype+"'").Length == 0)
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + protype + "\"生产部署和组别";
                //    break;
                //}
                //if (dt_Pro.Select("Pro ='" + Pro0 + "'").Length == 0)
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + Pro0 + "\"生产部署和组别";
                //    break;
                //}
                //if (dt_Pro.Select("Pro ='" + Pro1 + "'").Length == 0)
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + Pro1 + "\"生产部署和组别";
                //    break;
                //}
                //if (dt_Pro.Select("Pro ='" + Pro2 + "'").Length == 0)
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + Pro2 + "\"生产部署和组别";
                //    break;
                //}
                //if (dt_Pro.Select("Pro ='" + Pro3 + "'").Length == 0)
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + Pro3 + "\"生产部署和组别";
                //    break;
                //}
                //if (dt_Pro.Select("Pro ='" + Pro4 + "'").Length == 0)
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + Pro4 + "\"生产部署和组别";
                //    break;
                //}
                //if(PZlg.ToString()!="1"&&PZlg.ToString()!="2")
                //{
                //    msg = "行：" + (i + 2) + ",无\"" + PZlg + "\"平准逻辑";
                //    break;
                //}
                #endregion


            }
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

        public string Updatepro(DataTable dt, string user)
        {
            return dataAccess.Updatepro(dt, user);
        }

        public string UpdateTable(DataTable dt, string user)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["vcPorType"].ToString() == "" || dt.Rows[i]["vcZB"].ToString() == "")
                {
                    return "更新失败！请确保信息完善";
                }

                if (dt.Rows[i]["iflag"].ToString().Trim() == "1" || dt.Rows[i]["iflag"].ToString().Trim() == "2")
                {
                    if (dt.Rows[i]["vcProName0"].ToString().Trim() == ""
                        || dt.Rows[i]["vcLT0"].ToString().Trim() == ""
                        || dt.Rows[i]["vcCalendar0"].ToString().Trim() == "")
                    {
                        return "第'" + (i + 1) + "'行，工程0信息不完善！";
                    }
                    if (dt.Rows[i]["vcProName1"].ToString().Trim() == ""
                        || dt.Rows[i]["vcLT1"].ToString().Trim() == ""
                        || dt.Rows[i]["vcCalendar1"].ToString().Trim() == "")
                    {
                        return "第'" + (i + 1) + "'行，工程1信息不完善！";
                    }
                    if (dt.Rows[i]["vcProName2"].ToString().Trim() != "" 
                        || dt.Rows[i]["vcLT2"].ToString().Trim() != ""
                        || dt.Rows[i]["vcCalendar2"].ToString().Trim() != "")
                    {
                        if (dt.Rows[i]["vcProName2"].ToString().Trim() == ""
                            || dt.Rows[i]["vcLT2"].ToString().Trim() == "" 
                            || dt.Rows[i]["vcCalendar2"].ToString().Trim() == "")
                        {
                            return "第'" + (i + 1) + "'行，工程2信息不完善！";
                        }
                    }
                    if (dt.Rows[i]["vcProName3"].ToString().Trim() != "" 
                        || dt.Rows[i]["vcLT3"].ToString().Trim() != "" 
                        || dt.Rows[i]["vcCalendar3"].ToString().Trim() != "")
                    {
                        if (dt.Rows[i]["vcProName3"].ToString().Trim() == ""
                            || dt.Rows[i]["vcLT3"].ToString().Trim() == "" 
                            || dt.Rows[i]["vcCalendar3"].ToString().Trim() == "")
                        {
                            return "第'" + (i + 1) + "'行，工程3信息不完善！";
                        }
                    }
                    if (dt.Rows[i]["vcProName4"].ToString().Trim() == "" 
                        || dt.Rows[i]["vcLT4"].ToString().Trim() == "" 
                        || dt.Rows[i]["vcCalendar4"].ToString().Trim() == "")
                    {
                        return "第'" + (i + 1) + "'行，工程4信息不完善";
                    }
                }
            }
            string msg = dataAccess.UpdateTable(dt, user);
            return msg;
        }

        public DataTable bindState()
        {
            return dataAccess.bindState();
        }

        public DataTable bindPartType()
        {
            return dataAccess.getPartType();
        }

        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>msg</returns>
        //public string FileImport(HttpPostedFile file, string userId)
        //{
        //    FileInfo file_im = new FileInfo(file.FileName);
        //    if (file_im.Extension != ".xls" && file_im.Extension != ".xlt" && file_im.Extension != ".xlsx")
        //    {
        //        return "文件格式不正确！";
        //    }
        //    QMExcel oQMExcel = new QMExcel();
        //    string path;
        //    string tepath = HttpContext.Current.Server.MapPath("~/Templates/FS1202_Import.xlt");
        //    if (file_im.Extension == ".xls" || file_im.Extension == ".xlt")
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1202Im.xls");
        //    }
        //    else
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1202Im.xlsx");
        //    }
        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //    }
        //    file.SaveAs(path);
        //    DataTable dtParts = new DataTable();
        //    DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(tepath);//导入Excel文件到DataTable
        //    DataTable dt = new DataTable();
        //    string msg = checkExcel2(path, ref dt, ref dtParts, dtTmplate);//检验导入的数据
        //    if (msg.Length > 0)
        //    {
        //        return msg;
        //    }
        //    msg = Updatepro(dt, userId);//将导入的表格数据上传

        //    return msg;
        //}


    }
}