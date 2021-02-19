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
        public DataTable Search(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            return convert(fs0309_DataAccess.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
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

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            fs0309_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0309_DataAccess.importSave(dt, strUserId);
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
        public DataTable getAllTask()
        {
            return fs0309_DataAccess.getAllTask();
        }
        #endregion


        #region 测试10万
        public DataTable test10W()
        {
            return fs0309_DataAccess.test10W();
        }
        #endregion

        #region 销售展开（根据检索条件）
        public void sendMail(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState,ref string strErr
            )
        {
            fs0309_DataAccess.sendMail(strChange, strPart_id, strOriginCompany, strHaoJiu
            , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState, ref strErr);
        }
        #endregion

        #region 销售展开（根据所选）
        public void sendMail(List<Dictionary<string,object>> listInfoData,string strUserEmail,string strUserId,string strUserName ,ref string strErr)
        {
            #region 更新价格表
            fs0309_DataAccess.sendMail(listInfoData, ref strErr);
            #endregion

            #region 发送邮件

            #region 邮件发送准备

            #region 用户邮箱
            string UserEmail = strUserEmail;
            if (string.IsNullOrEmpty(strUserEmail))
            {
                strErr += "获取用户邮箱失败!\n";
                return;
            }
            #endregion

            #region 用户名称
            string UserName = strUserName;
            if (string.IsNullOrEmpty(strUserEmail))
            {
                strErr += "获取用户名称失败!\n";
                return;
            }
            #endregion

            #region 邮件内容
            string EmailBody = fs0309_DataAccess.getEmailBody(strUserId);
            if (string.IsNullOrEmpty(EmailBody))
            {
                strErr += "获取邮箱内容失败!\n";
                return;
            }
            //这里做了年月的转换
            EmailBody = EmailBody.Replace("##yearmonth##", DateTime.Now.ToString("yyyy年MM月"));
            #endregion

            #region 收件人
            /*
             * 修改时间：2020-2-18
             * 修改人：董镇
             * 修改内同：获取收件人时需要返回哪些收件人(收件人就是收货方)维护了，哪些收件人未维护，对于未维护的收件人要进行提示
             * 功能描述：1、获取所有的维护了的收件人，从数据库获取
             *           2、获取所选择的收件人，判断所选择的收件人是否在所有已维护收件人中存在
             *           3、对于不存在的收件人进行提示，并停止继续销售展开
             *           4、如果都存在，获取收件人邮箱，继续执行销售展开操作
             */
            #region 获取所有维护的收件人信息
            DataTable allreceiverDt = fs0309_DataAccess.getreceiverDt();
            if (allreceiverDt == null || allreceiverDt.Rows.Count <= 0)       //执行SQL查询，但未检索到任何数据，可能原因：未维护任何收件人邮箱信息
            {
                strErr = "未维护任何收货方邮箱信息";
                return;
            }

            List<string> allLists = new List<string>();     //所有的不重复的收件人信息
            for (int i = 0; i < allreceiverDt.Rows.Count; i++)
            {
                allLists.Add(allreceiverDt.Rows[i]["displayName"].ToString());
            }
            allLists = allLists.Distinct().ToList();
            #endregion

            #region 获取所选择的收件人
            List<string> lists = new List<string>();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                lists.Add(listInfoData[i]["vcReceiver"].ToString());
            }
            lists = lists.Distinct().ToList();

            //判断所选的收件人是否存在，并记录未维护的收件人邮箱
            for (int i = 0; i < lists.Count; i++)
            {
                if (!allLists.Contains(lists[i]))
                {
                    strErr += "收件人:" + lists[i].ToString() + "邮箱未维护！";
                }
            }
            #endregion

            #region 判断所选择的收件人是否在所有已维护收件人中存在
            

            #endregion


            #endregion

            #region 抄送人
            /*
             * 注意：抄送人不需要判断是否拿到数据，如果没有拿到数据，说明没有添加抄送人，对于发送邮件无影响
             */
            DataTable cCDt = null;
            #endregion

            #region 邮件主题
            string strSubject = "";
            if (string.IsNullOrEmpty(strSubject))
            {
                
            }
            #endregion

            #region 附件
            /*
             * 有附件给地址，无给null
             */
            string strFilePath = null;
            #endregion

            #region 传入附件后，是否需要删除附件
            /*
             * true:需要删除附件
             * false:需要删除附件/没有附件
             */
            bool delFileNameFlag = false;
            #endregion

            #endregion

            #region 开始发送邮件
            //记录错误信息
            
            #endregion

            #endregion

        }
        #endregion

        #region 导出带模板
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

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
                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xlsx";
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
        public DataTable getGSChangePrice(string strPartId, string strSupplier, int iAutoId, string strGSName, decimal decPriceOrigin)
        {
            return fs0309_DataAccess.getGSChangePrice(strPartId,strSupplier,iAutoId,strGSName,decPriceOrigin);
        }
        #endregion


        #region 公式计算B、C需要验证该品番是否存在上个状态的数据
        public bool getLastStateGsData(string strPartId, string strSupplier, int iAutoId)
        {
            DataTable dt=fs0309_DataAccess.getLastStateGsData(strPartId, strSupplier, iAutoId);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        #endregion

        #region 公式计算B、C需要验证该品番是否存在上个状态的数据
        public bool isGsExist(string strGs)
        {
            DataTable dt = fs0309_DataAccess.isGsExist(strGs);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        #endregion
    }
}
