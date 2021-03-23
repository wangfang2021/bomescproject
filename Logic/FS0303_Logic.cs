using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;
using System.Net.Sockets;
using System.Net;

namespace Logic
{
    public class FS0303_Logic
    {
        FS0303_DataAccess fs0303_DataAccess;
        IPAddress ip;

        public FS0303_Logic()
        {
            fs0303_DataAccess = new FS0303_DataAccess();
        }

        #region 检索
        public DataTable Search(string strIsShowAll, string strOriginCompany)
        {
            return fs0303_DataAccess.Search(strIsShowAll, strOriginCompany);
        }
        #endregion

        #region 检查是否有异常数据
        public DataTable getErrPartId()
        {
            return fs0303_DataAccess.getErrPartId();
        }
        #endregion

        #region 检索特记
        public DataTable SearchTeji(string strPart_id)
        {
            return fs0303_DataAccess.SearchTeji(strPart_id);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0303_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0303_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 保存特记
        public void SaveTeJi(List<Dictionary<string, Object>> listInfoData, string strUserId, string strPartId)
        {
            fs0303_DataAccess.SaveTeJi(listInfoData, strUserId, strPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorPartId)
        {
            fs0303_DataAccess.importSave(dt, strUserId, ref strErrorPartId);
        }
        #endregion

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
                foreach (NameOrValue item in lists)
                {
                    dt.Columns.Add(item.strHeader + "_Name");
                }
                #endregion

                for (int i = 0; i < dt.Rows.Count; i++) //循环table的所有行
                {
                    foreach (NameOrValue item in lists)     //遍历lists
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
                                string value = fs0303_DataAccess.Name2Value(item.strCodeid, strName, true);
                                #endregion

                                #region 给dt赋值value
                                dt.Rows[i][strNewColumnsName] = value;
                                #endregion
                            }
                            //如果Name值为空，赋值null
                            else
                            {
                                if (item.isNull)
                                {
                                    dt.Rows[i][strNewColumnsName] = null;
                                }
                            }
                        }
                        //value获取失败,表示并未找到与其对应的Value值
                        catch (Exception ex)
                        {
                            #region 提示第几行的数据不合法，提示消息赋值给strErr
                            strErr += "第" + (i + 2) + "行出现错误"+ex.Message;
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

        #region 保存操作-根据Excel中的Name获取对应的Value，并添加到ListData中
        /// <summary>
        /// 导入操作-根据Excel中的Name获取对应的Value，并添加到dt中
        /// </summary>
        /// <param name="dt">Excel表格转换的table</param>
        /// <param name="lists">表格中需要Name转Value的列集合</param>
        /// <param name="strErr">错误提示消息</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> ConverList(List<Dictionary<string, Object>> listData, List<NameOrValue> lists, ref string strErr)
        {
            try
            {
                for (int i = 0; i < listData.Count; i++) //循环listdata的所有数据集
                {
                    foreach (var item in lists) //遍历lists集合
                    {
                        try
                        {
                            if (listData[i][item.strHeader + "_Name"] == null)
                            {
                                listData[i][item.strHeader] = null;
                                continue;//前天没填写
                            }
                            #region 获取正确的Name
                            string strName = listData[i][item.strHeader + "_Name"].ToString();
                            #endregion

                            #region 根据Name获取对应的Value
                            string strValue = fs0303_DataAccess.Name2Value(item.strCodeid, strName, true);
                            #endregion

                            //if (!item.isNull && strValue == null)
                            //{
                            //    strErr = "编辑行中第" + (i + 1) + "行" + item.strTitle + "不能为空";
                            //    return null;
                            //}
                            #region 更新Key对应的值
                            listData[i][item.strHeader] = strValue;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            strErr = "编辑行中第" + (i + 1) + "行" + item.strTitle + "填写不合法";
                            return null;
                        }

                    }
                }
                return listData;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        public class NameOrValue
        {
            /// <summary>
            /// 列描述
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

        #region 生确单发行
        public void sqSend(List<Dictionary<string, Object>> listInfoData, string strSqDate, string strUserId, string strEmail, string strUserName, ref string strErr)
        {

            #region 对供应商去重
            List<string> listSupplier = new List<string>();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                listSupplier.Add(strSupplier_id);
            }
            listSupplier = listSupplier.Distinct().ToList();
            #endregion

            #region 先校验供应商是否存在
            for (int i = 0; i < listSupplier.Count; i++)
            {
                string strSupplier_id = listSupplier[i].ToString();
                DataTable receiverDt = getSupplierInfo(strSupplier_id);
                if (receiverDt == null || receiverDt.Rows.Count == 0)
                {
                    strErr += "未找到 '" + strSupplier_id + "' 供应商基础信息，操作终止!";
                    return;
                }
            }
            #endregion


            //1、更新原单位纳期 2、更新生确单
            fs0303_DataAccess.sqSend(listInfoData, strSqDate, strUserId);

            #region 获取发件人邮件模板(邮件主题、邮件内容)，未找到：返回错误进行提示
            DataTable dtSetting = getEmailSetting(strUserId, "FS0303");
            string strTitle = "";//邮件标题
            string strContent = "";//邮件内容
            if (dtSetting == null || dtSetting.Rows.Count == 0)
            {
                strErr = "数据发送成功，但用户" + strUserId + "邮件内容没配置，邮件发送终止！";
                return;
            }
            else
            {
                strTitle = dtSetting.Rows[0]["vcTitle"].ToString();
                strContent = dtSetting.Rows[0]["vcContent"].ToString();
                var dateTime = Convert.ToDateTime(strSqDate);
                strSqDate = dateTime.ToString("yyyy年MM月dd日");
                strContent = strContent.Replace("##yearmonth##", strSqDate);
            }
            #endregion

            //再向供应商发邮件
            StringBuilder strEmailBody = new StringBuilder();

            StringBuilder sbr = new StringBuilder();
            for (int i = 0; i < listSupplier.Count; i++)
            {
                string strSupplier_id = listSupplier[i].ToString();
                DataTable receiverDt = getSupplierEmail(strSupplier_id);
                if (receiverDt==null|| receiverDt.Rows.Count==0)
                {
                    sbr.Append(strSupplier_id + ",");
                }
                else
                {
                    ComFunction.SendEmailInfo(strEmail, strUserName, strContent, receiverDt, null, strTitle, "", false);
                }
            }
            if (sbr.Length>0)
            {
                strErr += "生确单发送成功,但未找到 '" + sbr.ToString() + "' 供应商邮件信息，邮件未发送。";
            }
        }
        #endregion

        #region 数据同步
        public void dataSync(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strMessage)
        {
            try
            {
                #region 将旧型1-15年转为变成int格式
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    listInfoData[i]["vcNum1"] = Conver2Int(listInfoData[i]["vcNum1"]);
                    listInfoData[i]["vcNum2"] = Conver2Int(listInfoData[i]["vcNum2"]);
                    listInfoData[i]["vcNum3"] = Conver2Int(listInfoData[i]["vcNum3"]);
                    listInfoData[i]["vcNum4"] = Conver2Int(listInfoData[i]["vcNum4"]);
                    listInfoData[i]["vcNum5"] = Conver2Int(listInfoData[i]["vcNum5"]);
                    listInfoData[i]["vcNum6"] = Conver2Int(listInfoData[i]["vcNum6"]);
                    listInfoData[i]["vcNum7"] = Conver2Int(listInfoData[i]["vcNum7"]);
                    listInfoData[i]["vcNum8"] = Conver2Int(listInfoData[i]["vcNum8"]);
                    listInfoData[i]["vcNum9"] = Conver2Int(listInfoData[i]["vcNum9"]);
                    listInfoData[i]["vcNum10"] = Conver2Int(listInfoData[i]["vcNum10"]);
                    listInfoData[i]["vcNum11"] = Conver2Int(listInfoData[i]["vcNum11"]);
                    listInfoData[i]["vcNum12"] = Conver2Int(listInfoData[i]["vcNum12"]);
                    listInfoData[i]["vcNum13"] = Conver2Int(listInfoData[i]["vcNum13"]);
                    listInfoData[i]["vcNum14"] = Conver2Int(listInfoData[i]["vcNum14"]);
                    listInfoData[i]["vcNum15"] = Conver2Int(listInfoData[i]["vcNum15"]);
                }
                #endregion

                #region 向下游同步数据
                //获取所有的事业体
                DataTable dt = ComFunction.getTCode("C016");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strSYTName = dt.Rows[i]["vcName"].ToString();
                    List<Dictionary<string, Object>> tempList = getSendData(dt.Rows[i]["vcValue"].ToString(), listInfoData);
                    if (tempList.Count > 0)  //有要发送的数据
                    {
                        strMessage += dt.Rows[i]["vcName"].ToString() + ": ";
                        /*
                         * 2021-3-5
                         * 测试事业体服务器连通性
                        */
                        #region 测试该事业体服务器连通性
                        string strIP = GetIp(dt.Rows[i]["vcName"].ToString(),ref strMessage);
                        if (!Socket(strIP))
                        {
                            strMessage += "<font color=red>连接服务器失败！</font>";
                        }
                        #endregion

                        #region 测试该事业体下游收货方是否已经维护
                        if (!GetSYT2Receiver(dt.Rows[i]["vcName"].ToString()))
                        {
                            strMessage += "<font color=red>未维护下游收货方！</font>";
                        }
                        #endregion

                        #region 准备就绪，开始同步操作
                        fs0303_DataAccess.dataSync(strSYTName, tempList, strUserId, ref strMessage);
                        #endregion
                        
                        #region 如果同步成功，将此次同步的数据的同步时间更新
                        fs0303_DataAccess.dataSync(tempList, strUserId);
                        #endregion

                        strMessage += "发送成功！ \n";
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                strMessage += "发送失败！ \n"+ex.Message;
                throw ex;
            }

        }
        #endregion

        /// <summary>
        /// 获取在集合中属于该事业体的数据集
        /// </summary>
        /// <param name="sytCode">事业体名称</param>
        /// <param name="listInfoData">数据集合</param>
        /// <returns>返回一个数据集</returns>
        public List<Dictionary<string, object>> getSendData(string sytCode, List<Dictionary<string, object>> listInfoData)
        {
            List<Dictionary<string, object>> sendList = new List<Dictionary<string, object>>();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                if (listInfoData[i]["vcSYTCode"].ToString()== sytCode)
                {
                    sendList.Add(listInfoData[i]);
                }
            }
            return sendList;
        }



        #region 旧型1-15年字符串转为数字
        public decimal Conver2Int(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region 根据供应商获取邮箱地址
        public DataTable getSupplierEmail(string strSupplierId) {
            DataTable dt= fs0303_DataAccess.getSupplierEmail(strSupplierId);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            else
                return dt;
        }
        #endregion

        #region 根据供应商基础信息
        public DataTable getSupplierInfo(string strSupplierId)
        {
            return fs0303_DataAccess.getSupplierInfo(strSupplierId);
        }
        #endregion

        #region 获取发件人的邮件内容配置
        public DataTable getEmailSetting(string strUserId,string strChildID)
        {
            DataTable dt = fs0303_DataAccess.getEmailSetting(strUserId);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            else
                return dt;
        }
        #endregion

        #region 获取原单位用户维护权限
        public string getPri(string strUserId)
        {
            DataTable dt= fs0303_DataAccess.getPri(strUserId);
            if (dt == null || dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["vcValue2"].ToString();
        }
        #endregion

        #region 验证时间先后关系
        public bool checkDataFromTo(DataRow row,string strFromField,string strToField)
        {
            if (row[strFromField] != System.DBNull.Value && row[strToField] != System.DBNull.Value
                && row[strFromField].ToString().Trim() != "" && row[strToField].ToString().Trim() != ""
                )
            {
                DateTime dFrom = DateTime.Parse(row[strFromField].ToString());
                DateTime dTo = DateTime.Parse(row[strToField].ToString());
                if (dFrom > dTo)
                    return false;
                else
                    return true;
            }
            return true;
        }
        #endregion

        #region 获取事业体服务器连通性
        public string GetIp(string strSYTCode,ref string strMessage)
        {
            try
            {
                string strConn = ComConnectionHelper.GetConnectionString_MainToUnit(strSYTCode);
                int ipIndex = strConn.IndexOf("Source=");
                string ip = strConn.Substring(ipIndex+7);
                return ip;
            }
            catch (Exception ex)
            {
                strMessage += ex.Message;
                throw ex;
            }
        }
        #endregion

        #region 测试数据库IP和1433端口能否联通
        public bool Socket(string strIp)
        {
            try
            {
                if (IPAddress.TryParse(strIp, out ip))
                {
                    if (CheckConnection(strIp, 1433, 500))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 测试连接
        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="host">地址</param>
        /// <param name="port">端口</param>
        /// <param name="millisecondsTimeout">延迟</param>
        /// <returns></returns>
        public bool CheckConnection(string host, int port, int millisecondsTimeout)
        {
            TcpClient client = new TcpClient();
            try
            {
                var ar = client.BeginConnect(host, port, null, null);
                ar.AsyncWaitHandle.WaitOne(millisecondsTimeout);
                return client.Connected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Close();
            }
        }
        #endregion

        #region 校验该事业体的下游收货方是否维护
        public bool GetSYT2Receiver(string strSYTCode)
        {
            DataTable dt = fs0303_DataAccess.getReceiver(strSYTCode);
            if (dt!=null || dt.Rows.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
