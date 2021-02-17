using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using Common;

namespace Logic
{
    public class FS0309_Logic
    {
        FS0309_DataAccess fs0309_DataAccess;

        public FS0309_Logic()
        {
            fs0309_DataAccess = new FS0309_DataAccess();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string strMaxNum,string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            return convert(fs0309_DataAccess.Search(strMaxNum,strChange, strPart_id, strOriginCompany, strHaoJiu
            , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState
            ));
        }
        #endregion

        #region 相同品番行挨着，设定同一个颜色
        public DataTable convert(DataTable dt)
        {
            string strColor_A = "partFS0309A";//这两个变量是行的背景颜色class名字，具体颜色在前台画面定义
            string strColor_B = "partFS0309B";

            dt.Columns.Add("vcBgColor");
            string strTempPartId = "";
            string strTempColor = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strPart_id = dt.Rows[i]["vcPart_id"] == DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                if (strTempPartId == "")
                {
                    dt.Rows[i]["vcBgColor"] = strColor_A;
                    strTempPartId = strPart_id;
                    strTempColor = strColor_A;
                }
                else
                {
                    if (strTempPartId == strPart_id)
                    {
                        dt.Rows[i]["vcBgColor"] = strTempColor;
                    }
                    else 
                    {
                        strTempPartId = strPart_id;
                        strTempColor = strTempColor==strColor_A? strColor_B: strColor_A;
                        dt.Rows[i]["vcBgColor"] = strTempColor;
                    }
                }
            }
            return dt;
        }
        #endregion

        #region 向调达送信后变更价格处理状态
        public void sendDiaoDaChangeState(List<Dictionary<string, object>> listInfoData, ref string strErr)
        {
            fs0309_DataAccess.sendDiaoDaChangeState(listInfoData, ref strErr);
        }
        #endregion

        #region 获取十年年计
        public DataTable getOld_10_Year(List<Dictionary<string, object>> listInfoData, string strProjectType)
        {
            return fs0309_DataAccess.getOld_10_Year(listInfoData, strProjectType);
        }
        #endregion


        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            fs0309_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorPartId,bool isWuBtnVisible)
        {
            fs0309_DataAccess.importSave(dt, strUserId, ref strErrorPartId, isWuBtnVisible);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0309_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return fs0309_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            fs0309_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion

        #region 删除
        public void Del_GS(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0309_DataAccess.Del_GS(listInfoData, strUserId);
        }
        #endregion

        #region 检索所有待处理的数据
        public int getAllTask()
        {
            DataTable dt= fs0309_DataAccess.getAllTask();
            if (dt == null || dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(dt.Rows[0]["iNum"]);
        }
        #endregion

        #region 测试10万
        public DataTable test10W()
        {
            return fs0309_DataAccess.test10W();
        }
        #endregion

        #region 销售展开更新价格表
        public void updateXiaoShouZhanKaiState(List<Dictionary<string, object>> listInfoData, string strDanhao)
        {
            #region 更新价格表
            fs0309_DataAccess.updateXiaoShouZhanKaiState(listInfoData, strDanhao);
            #endregion

            #region 发送邮件 （已舍弃）

            //#region 邮件发送准备

            //#region 用户邮箱
            //string UserEmail = strUserEmail;
            //if (string.IsNullOrEmpty(strUserEmail))
            //{
            //    strErr += "获取用户邮箱失败!\n";
            //    return;
            //}
            //#endregion

            //#region 用户名称
            //string UserName = strUserName;
            //if (string.IsNullOrEmpty(strUserEmail))
            //{
            //    strErr += "获取用户名称失败!\n";
            //    return;
            //}
            //#endregion

            //#region 邮件内容
            //string strEmailBody = fs0309_DataAccess.getEmailBody(strUserId);
            //if (string.IsNullOrEmpty(strEmailBody))
            //{
            //    strErr += "获取邮箱内容失败!\n";
            //    return;
            //}
            ////这里做了年月的转换
            //strEmailBody = strEmailBody.Replace("##yearmonth##", DateTime.Now.ToString("yyyy年MM月"));
            //#endregion

            //#region 收件人
            ///*
            // * 修改时间：2020-2-18
            // * 修改人：董镇
            // * 修改内同：获取收件人时需要返回哪些收件人(收件人就是收货方)维护了，哪些收件人未维护，对于未维护的收件人要进行提示
            // * 功能描述：1、获取所有的维护了的收件人，从数据库获取
            // *           2、获取所选择的收件人，判断所选择的收件人是否在所有已维护收件人中存在
            // *           3、对于不存在的收件人进行提示，并停止继续销售展开
            // *           4、如果都存在，获取收件人邮箱，继续执行销售展开操作
            // */
            //#region 获取所有维护的收件人信息
            ////获取数据库中所有已经维护的收件人信息（收件人、邮箱）
            //DataTable allreceiverDt = fs0309_DataAccess.getreceiverDt();
            //if (allreceiverDt == null || allreceiverDt.Rows.Count <= 0)       //执行SQL查询，但未检索到任何数据，可能原因：未维护任何收件人邮箱信息
            //{
            //    strErr = "未维护任何收货方邮箱信息";
            //    return;
            //}
            ////获取数据库中所有已经维护的收件人信息（收件人）
            //List<string> allLists = new List<string>();
            //for (int i = 0; i < allreceiverDt.Rows.Count; i++)
            //{
            //    allLists.Add(allreceiverDt.Rows[i]["displayName"].ToString());
            //}
            //allLists = allLists.Distinct().ToList();
            //#endregion

            //#region 获取所选择的收件人
            ////界面中用户勾选的收件人
            //List<string> lists = new List<string>();
            //for (int i = 0; i < listInfoData.Count; i++)
            //{
            //    lists.Add(listInfoData[i]["vcReceiver"].ToString());
            //}
            //lists = lists.Distinct().ToList();

            ////判断所选的收件人是否存在，并记录未维护的收件人邮箱
            //for (int i = 0; i < lists.Count; i++)
            //{
            //    if (!allLists.Contains(lists[i]))
            //    {
            //        strErr += "收件人:" + lists[i].ToString() + "邮箱未维护！";
            //    }
            //}
            //if (string.IsNullOrEmpty(strErr))
            //{
            //    return;
            //}
            //#endregion

            //#region 获取用户勾选的收件人的邮箱
            //DataTable receiverDt = new DataTable();
            //receiverDt.Columns.Add("displayName");
            //receiverDt.Columns.Add("address");
            //for (int i = 0; i < lists.Count; i++)
            //{
            //    for (int j = 0; j < allreceiverDt.Rows.Count; j++)
            //    {
            //        if (lists[i] == allreceiverDt.Rows[j]["displayName"].ToString())
            //        {
            //            DataRow dr = receiverDt.NewRow();
            //            dr["displayName"] = allreceiverDt.Rows[j]["displayName"];
            //            dr["address"] = allreceiverDt.Rows[j]["address"];
            //            receiverDt.Rows.Add(dr);
            //        }
            //    }
            //}

            //if (receiverDt.Rows.Count <= 0)
            //{
            //    strErr += "所选择用户为配置任何邮箱！\n";
            //}

            //#endregion


            //#endregion

            //#region 抄送人
            ///*
            // * 注意：抄送人不需要判断是否拿到数据，如果没有拿到数据，说明没有添加抄送人，对于发送邮件无影响
            // */
            //DataTable cCDt = null;
            //#endregion

            //#region 邮件主题
            //string strSubject = "";
            //strSubject = fs0309_DataAccess.getEmailSubject(strUserId);
            //if (string.IsNullOrEmpty(strSubject))
            //{
            //    strErr += "未设置邮件主题！\n";
            //}
            //#endregion

            //#region 附件
            ///*
            // * 有附件给地址，无给null
            // */
            //string strFilePath = null;
            //#endregion

            //#region 传入附件后，是否需要删除附件
            ///*
            // * true:需要删除附件
            // * false:需要删除附件/没有附件
            // */
            //bool delFileNameFlag = false;
            //#endregion

            //#endregion

            //#region 开始发送邮件
            ////记录错误信息
            //ComFunction.SendEmailInfo(strUserEmail, strUserName, strEmailBody, receiverDt, cCDt, strSubject, strFilePath, delFileNameFlag);
            //#endregion

            #endregion

        }
#endregion


        #region 导出带模板-内制
        public string generateExcelWithXlt_Nei(DataTable dt, DataTable dt10Year, string[] field, string[] fields10Year, string rootPath, string xltName, string strUserId, string strFunctionName)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();
                int startRow = 9;

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);
                ISheet sheet_10_Year = hssfworkbook.GetSheetAt(1);

                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j+1);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                        cell.CellStyle = style;
                    }
                }

                for (int i = 0; i < dt10Year.Rows.Count; i++)
                {
                    IRow row = sheet_10_Year.CreateRow(2 + i);
                    for (int j = 0; j < fields10Year.Length; j++)
                    {
                        Type type = dt10Year.Columns[fields10Year[j]].DataType;
                        ICell cell = row.CreateCell(j + 1);
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dt10Year.Rows[i][fields10Year[j]].ToString());
                        }
                        cell.CellStyle = style;
                    }
                }
                if (dt10Year.Rows.Count == 0)
                {
                    hssfworkbook.RemoveSheetAt(1);
                }


                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_N_" + strUserId + ".xlsx";
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
                return "";
            }
        }
        #endregion

        #region 导出带模板-外注
        public string generateExcelWithXlt_Wai(DataTable dt,DataTable dt10Year, string[] field,string [] fields10Year, string rootPath, string xltName, string strUserId, string strFunctionName)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();
                int startRow = 8;

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);
                ISheet sheet_10_Year = hssfworkbook.GetSheetAt(1);

                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                        cell.CellStyle = style;
                    }
                }
                for (int i = 0; i < dt10Year.Rows.Count; i++)
                {
                    IRow row = sheet_10_Year.CreateRow(2 + i);
                    for (int j = 0; j < fields10Year.Length; j++)
                    {
                        Type type = dt10Year.Columns[fields10Year[j]].DataType;
                        ICell cell = row.CreateCell(j+1);
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dt10Year.Rows[i][fields10Year[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dt10Year.Rows[i][fields10Year[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dt10Year.Rows[i][fields10Year[j]].ToString());
                        }
                        cell.CellStyle = style;
                    }
                }
                if (dt10Year.Rows.Count == 0)
                {
                    hssfworkbook.RemoveSheetAt(1);
                }

                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_W_" + strUserId + ".xlsx";
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
                return "";
            }
        }
        #endregion


        #region 根据选择公式返回对应金额
        public DataTable getGSChangePrice(string strPartId, string strSupplier, string strReceiver, string strAutoId, string strGSName, decimal decPriceOrigin)
        {
            return fs0309_DataAccess.getGSChangePrice(strPartId,strSupplier, strReceiver, strAutoId, strGSName,decPriceOrigin);
        }
        #endregion


        #region 公式计算B需要验证该品番是否存在上个状态的数据
        public bool getLastStateGsData(string strPartId, string strSupplier, string strReceiver, string strAutoId)
        {
            DataTable dt=fs0309_DataAccess.getLastStateGsData(strPartId, strSupplier, strReceiver, strAutoId);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        #endregion

        #region 公式计算B需要验证该品番是否存在上个状态的数据
        public bool isGsExist(string strGs)
        {
            DataTable dt = fs0309_DataAccess.isGsExist(strGs);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        #endregion


        #region 获取销售展开数据
        public DataTable getXiaoShouZhanKai(List<Dictionary<string, object>> listInfoData)
        {
            return fs0309_DataAccess.getXiaoShouZhanKai(listInfoData);
        }
        #endregion

        #region 导出带模板-销售展开
        public string generateExcelWithXlt_XiaoShou(DataTable dt, string[] field, string rootPath, string xltName, string strUserId, string strFunctionName,string strDanhao)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();
                int startRow = 23;

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);

                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;

                sheet.GetRow(2).GetCell(1).SetCellValue(strDanhao);

                sheet.ShiftRows(23, sheet.LastRowNum, dt.Rows.Count, true, false);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                     IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        Type type = dt.Columns[field[j]].DataType;
                        ICell cell = row.CreateCell(j);
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dt.Rows[i][field[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                        }
                        cell.CellStyle = style;


                    }
                }
                


                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_N_" + strUserId + ".xlsx";
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
                return "";
            }
        }
        #endregion

        #region 给财务发提醒邮件
        public void  sendEmailToCaiWu(string strUserEmail,string strUserName,ref string strErr)
        {
            if (string.IsNullOrEmpty(strUserEmail))
            {
                strErr += "当前登录用户的邮箱地址在用户管理中没有配置!\n";
                return;
            }
            if (string.IsNullOrEmpty(strUserEmail))
            {
                strErr += "当前登录用户的用户名称在用户管理中没有配置!\n";
                return;
            }
            DataTable dtSetting = getCaiWuEmailSetting();
            if (dtSetting.Rows.Count == 0)
            {
                strErr += "给财务发送邮件标题、内容在常量中没有维护!\n";
                return;
            }
            string strSubject = dtSetting.Rows[0]["vcValue1"] == System.DBNull.Value ? "" : dtSetting.Rows[0]["vcValue1"].ToString();
            string strEmailBody = dtSetting.Rows[0]["vcValue2"] == System.DBNull.Value ? "" : dtSetting.Rows[0]["vcValue2"].ToString();
            if (strSubject.Trim()=="")
            {
                strErr += "给财务发送邮件标题在常量中不能维护空!\n";
                return;
            }
            if (strEmailBody.Trim() == "")
            {
                strErr += "给财务发送邮件内容在常量中不能维护空!\n";
                return;
            }

            DataTable receiverDt = new DataTable();
            receiverDt.Columns.Add("displayName");
            receiverDt.Columns.Add("address");
            DataTable dt = getCaiWuEmailAddRess();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = receiverDt.NewRow();
                dr["address"] = dt.Rows[i]["vcValue2"].ToString();
                dr["displayName"] = dt.Rows[i]["vcValue1"].ToString();
                receiverDt.Rows.Add(dr);
            }
            if (receiverDt.Rows.Count <= 0)
            {
                strErr += "所选择用户为配置任何邮箱！\n";
            }
            ComFunction.SendEmailInfo(strUserEmail, strUserName, strEmailBody, receiverDt, null, strSubject, "", false);
        }
        #endregion
        #region 获取价格Master财务接收邮箱
        public DataTable getCaiWuEmailAddRess()
        {
            return fs0309_DataAccess.getCaiWuEmailAddRess();
        }
        #endregion

        #region 获取价格Master财务通知邮件配置
        public DataTable getCaiWuEmailSetting()
        {
            return fs0309_DataAccess.getCaiWuEmailSetting();
        }
        #endregion

        #region 取当天最新单号连番
        public int getNewDanHao(string strSYTCode)
        {
            DataTable dt = fs0309_DataAccess.getNewDanHao(strSYTCode);
            if (dt == null || dt.Rows.Count == 0)
            {
                return 1;
            }
            else
            {
                if (dt.Rows[0]["vcDanHao"] == DBNull.Value || dt.Rows[0]["vcDanHao"].ToString() == "")
                    return 1;
                else
                    return Convert.ToInt32(dt.Rows[0]["vcDanHao"].ToString());
            }
        }
        #endregion

        public class NameOrValue
        {
            /// <summary>
            /// 列说明
            /// </summary>
            public string strTitle { get; set; }
            /// <summary>
            /// 列名
            /// </summary>
            public string strHeader { get; set; }
            /// <summary>
            /// 对应的CodeId
            /// </summary>
            public string strCodeid { get; set; }
            /// <summary>
            /// 能否为空
            /// </summary>
            public bool isNull { get; set; }
        }

        #region 导入操作-根据Excel中的Name获取对应的Value，并添加到dt中
        /// <summary>
        /// 导入操作-根据Excel中的Name获取对应的Value，并添加到dt中
        /// </summary>
        /// <param name="dt">Excel表格转换的table</param>
        /// <param name="lists">表格中需要Name转Value的列集合</param>
        /// <param name="strErr">错误提示消息</param>
        /// <returns></returns>
        public DataTable ConverDT(DataTable dt, List<NameOrValue> lists, ref string strErr)
        {
            try
            {
                #region 先在dt中添加新列
                foreach (var item in lists)
                {
                    dt.Columns.Add(item.strHeader + "_Name");
                }
                #endregion

                for (int i = 0; i < dt.Rows.Count; i++) //循环table的所有行
                {
                    foreach (var item in lists)     //遍历lists
                    {

                        try
                        {
                            #region 获取table中需要name转value的列的name值
                            string strName = dt.Rows[i][item.strHeader].ToString();
                            #endregion

                            #region 锁定到对应的列
                            string strNewColumnsName = item.strHeader + "_Name";
                            #endregion

                            //如果name值合法,进行获取value值，赋值value
                            if (!string.IsNullOrEmpty(strName))
                            {
                                #region 获取Name对应的Value值
                                string value = fs0309_DataAccess.Name2Value(item.strCodeid, strName, true);
                                #endregion

                                #region 给dt赋值value
                                dt.Rows[i][strNewColumnsName] = value;
                                #endregion
                            }
                            //如果Name值不合法，不获取value值，赋值null
                            else
                            {
                                if (item.isNull)
                                {
                                    #region 给dt赋值null
                                    dt.Rows[i][strNewColumnsName] = null;
                                    #endregion
                                }
                                else
                                {
                                    //strErr = "第" + (i + 2) + "行的" + item.strTitle + "不能为空";
                                    return null;
                                }

                            }
                        }
                        //value获取失败,表示并未找到与其对应的Value值
                        catch (Exception)
                        {
                            #region 提示第几行的数据不合法，提示消息赋值给strErr
                            strErr = "第" + (i + 2) + "行的" + item.strTitle + "填写不合法";
                            return null;
                            #endregion
                        }
                    }
                }
                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

    }
}
