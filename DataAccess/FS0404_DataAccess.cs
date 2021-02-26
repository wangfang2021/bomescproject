﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.IO;

namespace DataAccess
{
    public class FS0404_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

 

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcOrderState, string vcInOutFlag,string vcOrderNo, string dTargetDate, string vcOrderType,string userID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" select [iAutoId], [vcOrderNo],  ");
                strSql.AppendLine("    case when vcOrderType='0' then [vcTargetYear]+'/'+[vcTargetMonth]+'/'+[vcTargetDay]  ");
                strSql.AppendLine("    	when vcOrderType='1' then [vcTargetYear]+'/'+[vcTargetMonth]+''+c.vcName   ");
                strSql.AppendLine("    	when vcOrderType='2' then [vcTargetYear]+'/'+[vcTargetMonth]  ");
                strSql.AppendLine("    else '' end as [dTargetDate],  ");
                strSql.AppendLine("  b.vcOrderDifferentiation as [vcOrderType],e.vcName as  vcInOutFlag, case when [vcOrderState]=2 then '撤销' when [vcOrderState]=3 then '已修正' else '已上传' end as vcOrderState, [vcMemo],   ");
                strSql.AppendLine("  [dUploadDate], [vcFilePath], [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[TOrderUploadManage]  a  ");
                strSql.AppendLine("  left join (select vcOrderDifferentiation,vcOrderInitials from [dbo].[TOrderDifferentiation] )b on a.vcOrderType= b.vcOrderInitials     ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C046' )c on a.vcTargetWeek= c.vcValue      ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C003' )e on a.vcInOutFlag= e.vcValue      ");
                strSql.AppendLine("  left join ( select vcUnitCode,vcUserID from [dbo].[SUser] )d on a.vcOperatorID= d.vcUserID  where d.vcUnitCode=( select vcUnitCode from [dbo].[SUser] where vcUserID = '"+ userID + "')    ");
                if (vcOrderState.Length > 0)
                {
                    if (vcOrderState == "2")
                    {
                        strSql.AppendLine("  and vcOrderState = '2' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and vcOrderState <>'2' ");
                    }
                }
                if (vcInOutFlag.Length > 0)
                {
                    strSql.AppendLine("  and  vcInOutFlag =  '" + vcInOutFlag + "' ");
                }
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderNo like  '%" + vcOrderNo + "%' ");
                }
                if (dTargetDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dUploadDate,112) = '" + dTargetDate.Replace("-", "") + "' ");
                }
                if (vcOrderType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcOrderType = '" + vcOrderType + "' ");
                }
                strSql.AppendLine("  order by  dOperatorTime desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrderCodeByName()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select vcOrderInitials from [dbo].[TOrderDifferentiation] where [vcFlag]='1' ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrderType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select vcOrderInitials as vcValue,vcOrderDifferentiation as vcName from [dbo].[TOrderDifferentiation] order by iAutoId asc  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion
        /// <summary>
        /// 通过订单号查询原订单是否存在
        /// </summary>
        /// <param name="lastOrderNo"></param>
        /// <returns></returns>
        public DataTable isCheckByOrderNo(string lastOrderNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from TOrderUploadManage where vcOrderNo='"+ lastOrderNo + "'  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 修正订单操作
        /// </summary>
        /// <param name="vcOrderType"></param>
        /// <param name="dTargetDate"></param>
        /// <param name="dTargetWeek"></param>
        /// <param name="lastOrderNo"></param>
        /// <param name="newOrderNo"></param>
        /// <param name="vcMemo"></param>
        /// <param name="fileList"></param>
        public void updateBylastOrderNo(string vcOrderType,string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList,string UserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = dTargetDate.Substring(0, 4);
                string vcTargetMonth = dTargetDate.Substring(4, 2);
                string vcTargetDay = dTargetDate.Substring(6, 2);
                string filePath = fileList[0]["filePath"].ToString();
                string fileName = fileList[0]["fileName"].ToString().Trim().Substring(0, fileList[0]["fileName"].ToString().Trim().LastIndexOf("."));
                string fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);
                ////订单类型	0	日度
                //订单类型	1	周度
                //订单类型	2	月度
                //订单类型	3	紧急
                if (vcOrderType == "2")
                {
                    vcInOutFlag = fileOrderNo.Substring(fileOrderNo.Length - 1, 1);
                }
                strSql.AppendLine("  update TOrderUploadManage   ");
                strSql.AppendLine("  set vcOrderNo='" + newOrderNo + "',  ");
                strSql.AppendLine("  vcTargetYear='" + vcTargetYear + "',  ");
                strSql.AppendLine("  vcTargetMonth='" + vcTargetMonth + "',  ");
                strSql.AppendLine("  vcTargetDay='" + vcTargetDay + "',  ");
                strSql.AppendLine("  vcTargetWeek='" + dTargetWeek + "',  ");
                strSql.AppendLine("  vcOrderType='" + vcOrderType + "',  ");
                strSql.AppendLine("  vcInOutFlag='" + vcInOutFlag + "',  ");
                strSql.AppendLine("  vcOrderState='0',  ");
                strSql.AppendLine("  vcMemo='" + vcMemo + "',  ");
                strSql.AppendLine("  dUploadDate=GETDATE(),  ");
                strSql.AppendLine("  vcFilePath='" + filePath + "',vcFileOrderNo='"+ fileOrderNo + "',  ");
                strSql.AppendLine("  vcOperatorID='" + UserId + "',  ");
                strSql.AppendLine("  dOperatorTime=GETDATE()  ");
                strSql.AppendLine("  where vcOrderNo='" + lastOrderNo + "'  ");
                strSql.AppendLine("    ");
                strSql.AppendLine("    ");
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="vcOrderType"></param>
        /// <param name="dTargetDate"></param>
        /// <param name="dTargetWeek"></param>
        /// <param name="lastOrderNo"></param>
        /// <param name="newOrderNo"></param>
        /// <param name="vcMemo"></param>
        /// <param name="fileList"></param>
        /// <param name="userId"></param>
        public void addOrderNo(string realPath, string vcOrderType,string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId, string uionCode, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                StringBuilder strSql1 = new StringBuilder();
                strSql1.AppendLine(" select b.vcValue,c.vcOrderInitials from [dbo].[TOrderGoodsAndDifferentiation] a  ");
                strSql1.AppendLine(" left join TCode b on a.vcOrderGoodsId=b.iAutoId  ");
                strSql1.AppendLine(" left join [dbo].[TOrderDifferentiation] c on a.vcOrderDifferentiationId = c.iAutoId  ");

                DataTable dtOrderType= excute.ExcuteSqlWithSelectToDT(strSql1.ToString());

                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = string.Empty;
                string vcTargetMonth = string.Empty;
                string vcTargetDay = string.Empty;

                vcTargetYear = dTargetDate.Replace("-","").Substring(0, 4);
                vcTargetMonth = dTargetDate.Replace("-", "").Substring(4, 2);
                if (dTargetDate.Replace("-", "").Length == 8)
                {
                    vcTargetDay = dTargetDate.Replace("-", "").Substring(6, 2);
                }
                //dTargetWeek = getWeekNumInMonth(Convert.ToDateTime(dTargetDate)).ToString();
                //if (vcOrderType == "0")
                //{
                //    vcTargetYear = dTargetDate.Substring(0, 4);
                //    vcTargetMonth = dTargetDate.Substring(4, 2);
                //    vcTargetDay = dTargetDate.Substring(6, 2);

                //}
                //else if (vcOrderType == "1")
                //{
                //    vcTargetYear = dTargetDate.Substring(0, 4);
                //}
                //else if (vcOrderType == "2") {
                //    vcTargetYear = dTargetDate.Substring(0, 4);
                //    vcTargetMonth = dTargetDate.Substring(4, 2);
                //}

                string fileName = string.Empty;
                string filePath = string.Empty;
                string fileOrderNo = string.Empty;
                //验证品番与订货方式关系及其是否存在主数据表
                #region
                for (int i = 0; i < fileList.Count; i++)
                {
                    filePath = fileList[i]["filePath"].ToString();
                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                    DataTable dockTmp = getDockTable();
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;
                    string msg = string.Empty;
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    StringBuilder sbr = new StringBuilder();
                    //判断头
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string vcSupplierId = "";
                        //string vcCarType = "";
                        //string vcPartId_Replace = "";
                        //string inout = "";
                        string vcSupplierPlant = "";//工区
                        string vcSupplierPlace = "";//出荷场
                        string vcOrderNum = detail.QTY;
                        string vcOrderingMethod = "";

                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //inout = hashtable["vcInOut"].ToString();
                            //vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            bool IsHanYouOrderMethod = false;
                            for (int m = 0; m < dtOrderType.Rows.Count; m++)
                            {
                                if (vcOrderingMethod == dtOrderType.Rows[m]["vcValue"].ToString())
                                {
                                    if (vcOrderType == dtOrderType.Rows[m]["vcOrderInitials"].ToString())
                                    {
                                        IsHanYouOrderMethod = true;
                                        break;
                                    }
                                }
                            }
                            if (!IsHanYouOrderMethod)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = "选中的订单类型与"+fileName+"文件中品番" + vcPart_id + "的订单方式不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                        else
                        {
                            tmp += "品番基础数据表不包含品番" + vcPart_id + ";\r\n";
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = fileName+"文件中品番基础数据表不包含品番" + vcPart_id;
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                    
                }
                if (!bReault)
                {
                    return;
                }
                #endregion
                //月度订单单独处理  验证品番个数  內制外注 品番订货数
                if (vcOrderType=="S")
                {
                    List<string> TargetYM = new List<string>();
                    TargetYM.Add(dTargetDate.Replace("-", ""));
                    //获取soq验收数量
                    DataTable SoqDt = getSoqDt(TargetYM);

                    DataRow[] drArrayN = SoqDt.Select("vcInOutFlag='0'"); //0 內制 1外制
                    DataRow[] drArrayW = SoqDt.Select("vcInOutFlag='1'"); //0 內制 1外制
                    DataRow[] drArrayTmp = drArrayN;
                    //DataTable dtNei = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    //foreach (DataRow dr in drArray)
                    //{
                    //    dtNei.ImportRow(dr);
                    //}

                    for (int i = 0; i < fileList.Count; i++)
                    {
                        filePath = fileList[i]["filePath"].ToString();
                        fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                        fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                        DataTable dockTmp = getDockTable();
                        int No = 1;
                        string strInOutflag = string.Empty;
                        Dictionary<string, string> dicPartNo = new Dictionary<string, string>();
                        //读取文件

                        string vcPackingFactory = uionCode;
                        string vcOrderNo = fileName;
                        string msg = string.Empty;
                        //读取Order
                        Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                        StringBuilder sbr = new StringBuilder();
                        //判断头
                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcSupplierId = "";
                            //string vcCarType = "";
                            //string vcPartId_Replace = "";
                            string inout = "";
                            string vcSupplierPlant = "";//工区
                            string vcSupplierPlace = "";//出荷场
                            string vcOrderNum = detail.QTY;
                            string vcOrderingMethod = "";

                            string dateTime = detail.Date.Trim();
                            string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                            //检测品番表是否存在该品番
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                            if (hashtable.Keys.Count > 0)
                            {
                                inout = hashtable["vcInOut"].ToString();
                                //vcDock = hashtable["vcSufferIn"].ToString();
                                vcSupplierId = hashtable["vcSupplierId"].ToString();
                                vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                                vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                                //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                                vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                                if (No == 1)
                                {
                                    strInOutflag = inout;
                                    if (inout == "1")
                                    {
                                        drArrayTmp = drArrayW;
                                    }
                                }
                                else
                                {
                                    if (inout != strInOutflag) //判定此次文件的内外区分是否一致
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = fileName;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = fileName + "文件中的品番" + vcPart_id + "与第一个品番的内外区分不一样";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }
                                // 判定品番是否在临时品番里面
                                bool isExist = false;
                                for (int m = 0; m < drArrayTmp.Length; m++)
                                {
                                    if (drArrayTmp[m]["vcPart_id"].ToString() == vcPart_id)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = fileName + "文件中的品番" + vcPart_id + "解析的内外区分与主数据该品番的内外不对应";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                if (!dicPartNo.ContainsKey(vcPart_id))
                                {
                                    dicPartNo.Add(vcPart_id, vcPart_id);
                                }
                            }
                        }
                    }
                    if (!bReault)
                    {
                        return;
                    }
                }


                for (int i=0;i< fileList.Count;i++)
                {
                   
                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0,fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-")+1);
                    ////订单类型	0	日度
                    //订单类型	1	周度
                    //订单类型	2	月度
                    //订单类型	3	紧急
                    //if (vcOrderType == "2") {
                    //    vcInOutFlag = fileOrderNo.Substring(fileOrderNo.Length - 1, 1);
                    //}
                   
                    filePath = fileList[i]["filePath"].ToString();
                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType],[vcInOutFlag]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate],[dCreateDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],vcFileOrderNo,[vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "', '" + vcInOutFlag + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "', '" + fileOrderNo + "',  ");
                    strSql.AppendLine("  		   '"+ userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");
                }
                
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 获取soq数量

        public DataTable getSoqDt(List<string> TargetYM)
        {
            try
            {
                string target = "";
                foreach (string s in TargetYM)
                {
                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        target += ",'" + s + "'";
                    }
                    else
                    {
                        target += "'" + s + "'";
                    }
                }

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcPart_id,vcInOutFlag,vcDXYM,vcFZGC,vcCarType,vcMakingOrderType,iPartNums,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31 FROM TSOQReply");
                sbr.AppendLine(" WHERE ");
                sbr.AppendLine(" vcDXYM in (" + target + ") ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable getSoqNumMonth(string partId, string TargetYM, string vcOrderingMethod, DataTable dt)
        {
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //品番，内外，订货方式，车型
                if (partId.Trim().Equals(dt.Rows[i]["vcPart_id"].ToString().Trim()) && TargetYM.Equals(dt.Rows[i]["vcDXYM"].ToString().Trim()))
                {
                    hashtable.Add("Total", dt.Rows[i]["iPartNums"].ToString());
                    hashtable.Add("iD1", dt.Rows[i]["iD1"].ToString());
                    hashtable.Add("iD2", dt.Rows[i]["iD2"].ToString());
                    hashtable.Add("iD3", dt.Rows[i]["iD3"].ToString());
                    hashtable.Add("iD4", dt.Rows[i]["iD4"].ToString());
                    hashtable.Add("iD5", dt.Rows[i]["iD5"].ToString());
                    hashtable.Add("iD6", dt.Rows[i]["iD6"].ToString());
                    hashtable.Add("iD7", dt.Rows[i]["iD7"].ToString());
                    hashtable.Add("iD8", dt.Rows[i]["iD8"].ToString());
                    hashtable.Add("iD9", dt.Rows[i]["iD9"].ToString());
                    hashtable.Add("iD10", dt.Rows[i]["iD10"].ToString());
                    hashtable.Add("iD11", dt.Rows[i]["iD11"].ToString());
                    hashtable.Add("iD12", dt.Rows[i]["iD12"].ToString());
                    hashtable.Add("iD13", dt.Rows[i]["iD13"].ToString());
                    hashtable.Add("iD14", dt.Rows[i]["iD14"].ToString());
                    hashtable.Add("iD15", dt.Rows[i]["iD15"].ToString());
                    hashtable.Add("iD16", dt.Rows[i]["iD16"].ToString());
                    hashtable.Add("iD17", dt.Rows[i]["iD17"].ToString());
                    hashtable.Add("iD18", dt.Rows[i]["iD18"].ToString());
                    hashtable.Add("iD19", dt.Rows[i]["iD19"].ToString());
                    hashtable.Add("iD20", dt.Rows[i]["iD20"].ToString());
                    hashtable.Add("iD21", dt.Rows[i]["iD21"].ToString());
                    hashtable.Add("iD22", dt.Rows[i]["iD22"].ToString());
                    hashtable.Add("iD23", dt.Rows[i]["iD23"].ToString());
                    hashtable.Add("iD24", dt.Rows[i]["iD24"].ToString());
                    hashtable.Add("iD25", dt.Rows[i]["iD25"].ToString());
                    hashtable.Add("iD26", dt.Rows[i]["iD26"].ToString());
                    hashtable.Add("iD27", dt.Rows[i]["iD27"].ToString());
                    hashtable.Add("iD28", dt.Rows[i]["iD28"].ToString());
                    hashtable.Add("iD29", dt.Rows[i]["iD29"].ToString());
                    hashtable.Add("iD30", dt.Rows[i]["iD30"].ToString());
                    hashtable.Add("iD31", dt.Rows[i]["iD31"].ToString());
                    break;
                }
            }
            return hashtable;

        }

        public bool CheckTotalNumEqual(string TargetYM, string partId, string vcOrderingMethod, DataTable dt, string total)
        {
            try
            {
                bool flag = false;

                int totalCheck = Convert.ToInt32(getSoqNumMonth(partId, TargetYM, vcOrderingMethod, dt)["Total"].ToString());

                int totalC = Convert.ToInt32(total);

                if (totalC == totalCheck)
                {
                    flag = true;
                }

                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        /// <summary>  
        /// 计算当前是本月第几周 getWeekNumInMonth(DateTime.Now)  
        /// </summary>  
        /// <param name="daytime"></param>  
        /// <returns></returns>  
        public static int getWeekNumInMonth(DateTime daytime)
        {
            int dayInMonth = daytime.Day;
            //本月第一天  
            DateTime firstDay = daytime.AddDays(1 - daytime.Day);
            //本月第一天是周几  
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
            //本月第一周有几天  
            int firstWeekEndDay = 7 - (weekday - 1);
            //当前日期和第一周之差  
            int diffday = dayInMonth - firstWeekEndDay;
            diffday = diffday > 0 ? diffday : 1;
            //当前是第几周,如果整除7就减一天  
            int WeekNumInMonth = ((diffday % 7) == 0
                                        ? (diffday / 7 - 1)
                                        : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
            return WeekNumInMonth;

        }
        /// <summary>
        /// 紧急订单
        /// </summary>
        /// <param name="realPath"></param>
        /// <param name="vcOrderType"></param>
        /// <param name="vcInOutFlag"></param>
        /// <param name="dTargetDate"></param>
        /// <param name="dTargetWeek"></param>
        /// <param name="lastOrderNo"></param>
        /// <param name="newOrderNo"></param>
        /// <param name="vcMemo"></param>
        /// <param name="fileList"></param>
        /// <param name="userId"></param>
        public void addJinJiOrderNo(string realPath, string vcOrderType, string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId, string uionCode, ref bool bReault, ref DataTable dtMessage, ref string msg)
        {
            try
            {
                StringBuilder strSql1 = new StringBuilder();
                strSql1.AppendLine(" select b.vcValue,c.vcOrderInitials from [dbo].[TOrderGoodsAndDifferentiation] a  ");
                strSql1.AppendLine(" left join TCode b on a.vcOrderGoodsId=b.iAutoId  ");
                strSql1.AppendLine(" left join [dbo].[TOrderDifferentiation] c on a.vcOrderDifferentiationId = c.iAutoId  ");

                DataTable dtOrderType = excute.ExcuteSqlWithSelectToDT(strSql1.ToString());
                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = string.Empty;
                string vcTargetMonth = string.Empty;
                string vcTargetDay = string.Empty;
                vcTargetYear = dTargetDate.Replace("-", "").Substring(0, 4);
                vcTargetMonth = dTargetDate.Replace("-", "").Substring(4, 2);
                vcTargetDay = dTargetDate.Replace("-", "").Substring(6, 2);
                dTargetWeek = getWeekNumInMonth(Convert.ToDateTime(dTargetDate)).ToString();
                //if (vcOrderType == "0")
                //{
                //    vcTargetYear = dTargetDate.Substring(0, 4);
                //    vcTargetMonth = dTargetDate.Substring(4, 2);
                //    vcTargetDay = dTargetDate.Substring(6, 2);

                //}
                //else if (vcOrderType == "1")
                //{
                //    vcTargetYear = dTargetDate.Substring(0, 4);
                //}
                //else if (vcOrderType == "2")
                //{
                //    vcTargetYear = dTargetDate.Substring(0, 4);
                //    vcTargetMonth = dTargetDate.Substring(4, 2);
                //}

                string fileName = string.Empty;
                string filePath = string.Empty;
                string fileOrderNo = string.Empty;
                for (int i = 0; i < fileList.Count; i++)
                {
                    filePath = fileList[i]["filePath"].ToString();
                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                    DataTable dockTmp = getDockTable();
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    StringBuilder sbr = new StringBuilder();
                    //判断头
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string vcSupplierId = "";
                        //string vcCarType = "";
                        //string vcPartId_Replace = "";
                        //string inout = "";
                        string vcSupplierPlant = "";//工区
                        string vcSupplierPlace = "";//出荷场
                        string vcOrderNum = detail.QTY;
                        string vcOrderingMethod = "";

                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //inout = hashtable["vcInOut"].ToString();
                            //vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            bool IsHanYouOrderMethod = false;
                            for (int m = 0; m < dtOrderType.Rows.Count; m++)
                            {
                                if (vcOrderingMethod == dtOrderType.Rows[m]["vcValue"].ToString())
                                {
                                    if (vcOrderType == dtOrderType.Rows[m]["vcOrderInitials"].ToString())
                                    {
                                        IsHanYouOrderMethod = true;
                                        break;
                                    }
                                }
                            }
                            if (!IsHanYouOrderMethod)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = "选中的订单类型与"+ fileName + "文件中品番" + vcPart_id + "的订单方式不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = fileName+"文件中品番基础数据表不包含品番" + vcPart_id;
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                }
                if (!bReault)
                {
                    return;
                }

                for (int i = 0; i < fileList.Count; i++)
                {

                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);
                    ////订单类型	0	日度
                    //订单类型	1	周度
                    //订单类型	2	月度
                    //订单类型	3	紧急
                    //if (vcOrderType == "2")
                    //{
                    //    vcInOutFlag = fileOrderNo.Substring(fileOrderNo.Length - 1, 1);
                    //}

                    filePath = fileList[i]["filePath"].ToString();
                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType],[vcInOutFlag]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate],[dCreateDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],vcFileOrderNo,[vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "', '" + vcInOutFlag + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "', '" + fileOrderNo + "',  ");
                    strSql.AppendLine("  		   '" + userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");

                    //获取基础信息
                    DataTable dockTmp = getDockTable();
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;

                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    StringBuilder sbr = new StringBuilder();
                    //判断头
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string vcSupplierId = "";
                        //string vcCarType = "";
                        //string vcPartId_Replace = "";
                        //string inout = "";
                        string vcSupplierPlant = "";//工区
                        string vcSupplierPlace = "";//出荷场
                        string vcOrderNum = detail.QTY;

                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //inout = hashtable["vcInOut"].ToString();
                            //vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                        }
                        else
                        {
                            tmp += "品番基础数据表不包含品番" + vcPart_id + ";\r\n";
                        }

                        if (!string.IsNullOrWhiteSpace(tmp))
                        {
                            msg += tmp;
                            continue;
                        }
                        strSql.Append("  insert into  [dbo].[TUrgentOrder] (vcOrderNo,vcPart_id,vcSupplier_id,vcGQ,vcChuHePlant,  ");
                        strSql.Append("  iOrderQuantity,vcStatus,vcDelete,vcOperatorID,dOperatorTime ) values (  ");
                        strSql.Append("  '" + fileName + "',  ");
                        strSql.Append("  '"+ vcPart_id + "',  ");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierPlant, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierPlace, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcOrderNum, false) + ",");
                        strSql.Append("'0',");
                        strSql.Append("'0',");
                        strSql.AppendLine("  '" + userId + "',GETDATE())   ");
                        strSql.Append("); \r\n");
                    }
                }
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updateEditeOrderNo(string realPath,string vcOrderType, string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId, string uionCode, ref bool  bReault, ref DataTable dtMessage, ref string msg)
        {
            try
            {
                StringBuilder strSql1 = new StringBuilder();
                strSql1.AppendLine(" select b.vcValue,c.vcOrderInitials from [dbo].[TOrderGoodsAndDifferentiation] a  ");
                strSql1.AppendLine(" left join TCode b on a.vcOrderGoodsId=b.iAutoId  ");
                strSql1.AppendLine(" left join [dbo].[TOrderDifferentiation] c on a.vcOrderDifferentiationId = c.iAutoId  ");
                DataTable dtOrderType = excute.ExcuteSqlWithSelectToDT(strSql1.ToString());

                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = string.Empty;
                string vcTargetMonth = string.Empty;
                string vcTargetDay = string.Empty;
                
                string fileName = string.Empty;
                string filePath = string.Empty;
                string fileOrderNo = string.Empty;
                for (int i = 0; i < fileList.Count; i++)
                {
                    filePath = fileList[i]["filePath"].ToString();
                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                    DataTable dockTmp = getDockTable();
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    StringBuilder sbr = new StringBuilder();
                    //判断头
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string vcSupplierId = "";
                        //string vcCarType = "";
                        //string vcPartId_Replace = "";
                        //string inout = "";
                        string vcSupplierPlant = "";//工区
                        string vcSupplierPlace = "";//出荷场
                        string vcOrderNum = detail.QTY;
                        string vcOrderingMethod = "";

                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //inout = hashtable["vcInOut"].ToString();
                            //vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            bool IsHanYouOrderMethod = false;
                            for (int m = 0; m < dtOrderType.Rows.Count; m++)
                            {
                                if (vcOrderingMethod == dtOrderType.Rows[m]["vcValue"].ToString())
                                {
                                    if (vcOrderType == dtOrderType.Rows[m]["vcOrderInitials"].ToString())
                                    {
                                        IsHanYouOrderMethod = true;
                                        break;
                                    }
                                }
                            }
                            if (!IsHanYouOrderMethod)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = "选中的订单类型与" + fileName + "文件中品番" + vcPart_id + "的订单方式不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = fileName + "文件中品番基础数据表不包含品番" + vcPart_id;
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                }
                if (!bReault)
                {
                    return;
                }

                for (int i = 0; i < fileList.Count; i++)
                {

                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);

                    filePath = fileList[i]["filePath"].ToString();
                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType],[vcInOutFlag]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate],[dCreateDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],vcFileOrderNo,[vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "', '" + vcInOutFlag + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "', '" + fileOrderNo + "',  ");
                    strSql.AppendLine("  		   '" + userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");

                   
                    //获取基础信息
                    DataTable dockTmp = getDockTable();
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;

                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    StringBuilder sbr = new StringBuilder();
                    //判断头
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string vcSupplierId = "";

                        string vcOrderPlant = "";
                        string vcInOut = "";
                        string vcHaoJiu = "";
                        string vcOESP = "";
                        string vcSufferIn = "";
                        string iPackingQty = "";

                        string vcSupplierPlant = "";//工区
                        string vcSupplierPlace = "";//出荷场
                        string vcOrderNum = detail.QTY;

                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            vcOrderPlant = hashtable["vcOrderPlant"].ToString();
                            vcInOut = hashtable["vcInOut"].ToString();
                            vcHaoJiu = hashtable["vcHaoJiu"].ToString();
                            vcOESP = hashtable["vcOESP"].ToString();
                            vcSufferIn = hashtable["vcSufferIn"].ToString();
                            iPackingQty = hashtable["iPackingQty"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                        }
                        else
                        {
                            tmp += "品番基础数据表不包含品番" + vcPart_id + ";\r\n";
                        }

                        if (!string.IsNullOrWhiteSpace(tmp))
                        {
                            msg += tmp;
                            continue;
                        }
                        strSql.Append("  insert into  [dbo].[TUrgentOrder] (vcOrderNo,vcPart_id,[vcOrderPlant], [vcInOut], [vcHaoJiu], [vcOESP],[vcSupplier_id], vcGQ,vcChuHePlant,[vcSufferIn], [iPackingQty],  ");
                        strSql.Append("  iOrderQuantity,vcStatus,vcDelete,vcOperatorID,dOperatorTime ) values (  ");
                        strSql.Append("  '" + fileName + "',  ");
                        strSql.Append("  '" + vcPart_id + "',  ");
                        strSql.Append(ComFunction.getSqlValue(vcOrderPlant, true) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcInOut, true) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcHaoJiu, true) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcOESP, true) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierPlant, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierPlace, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSufferIn, true) + ",");
                        strSql.Append(ComFunction.getSqlValue(iPackingQty, true) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcOrderNum, false) + ",");
                        strSql.Append("'0',");
                        strSql.Append("'0',");
                        strSql.AppendLine("  '" + userId + "',GETDATE())   ");
                        strSql.Append("); \r\n");
                    }
                   
                }
                strSql.AppendLine("   update TOrderUploadManage  set  vcOrderState='3',vcMemo='"+ vcMemo + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where  vcOrderNo='" + lastOrderNo + "';  ");
                strSql.AppendLine("   update [TUrgentOrder] set vcDelete='1',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  where  vcOrderNo='" + lastOrderNo + "';  ");
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region  来自王继伟

        public Hashtable getDock(string PartID, string receiver, string vcPackingPlant, DataTable dt)
        {
            try
            {
                Hashtable hashtable = new Hashtable();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (PartID.Trim().Equals(dt.Rows[i]["vcPartId"].ToString().Trim())
                            && receiver.Trim().Equals(dt.Rows[i]["vcReceiver"].ToString().Trim())
                            && vcPackingPlant.Trim().Equals(dt.Rows[i]["vcPackingPlant"].ToString().Trim()))
                        {
                            hashtable.Add("vcOrderPlant", dt.Rows[i]["vcOrderPlant"].ToString());
                            hashtable.Add("vcInOut", dt.Rows[i]["vcInOut"].ToString());
                            hashtable.Add("vcHaoJiu", dt.Rows[i]["vcHaoJiu"].ToString());
                            hashtable.Add("vcOESP", dt.Rows[i]["vcOESP"].ToString());

                            hashtable.Add("vcSupplierId", dt.Rows[i]["vcSupplierId"].ToString());
                            hashtable.Add("vcSupplierPlant", dt.Rows[i]["vcSupplierPlant"].ToString());
                            hashtable.Add("vcSupplierPlace", dt.Rows[i]["vcSupplierPlace"].ToString());
                            hashtable.Add("vcSufferIn", dt.Rows[i]["vcSufferIn"].ToString());
                            hashtable.Add("iPackingQty", dt.Rows[i]["iPackingQty"].ToString());
                            hashtable.Add("vcOrderingMethod", dt.Rows[i]["vcOrderingMethod"].ToString());
                            break;
                        }
                    }
                }

                return hashtable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getDockTable()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("   SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSupplierPlant,a.vcPackingPlant,  ");
                sbr.AppendLine("   a.vcInOut,a.vcSupplierPlace,a.vcOrderingMethod,c.vcOrderPlant,a.vcOESP,a.vcHaoJiu,d.vcSufferIn,e.iPackingQty FROM     ");
                sbr.AppendLine("   (    ");
                sbr.AppendLine("   SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcSupplierPlace,vcOESP,vcHaoJiu FROM TSPMaster WHERE     ");
                sbr.AppendLine("   dFromTime <= GETDATE() AND dToTime >= GETDATE()     ");
                sbr.AppendLine("   ) a    ");
                sbr.AppendLine("   LEFT JOIN    ");
                sbr.AppendLine("   (    ");
                sbr.AppendLine("   SELECT [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [dFromTime], [dToTime], [vcSupplierPlant] FROM [TSPMaster_SupplierPlant] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
                sbr.AppendLine("   ) b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId    ");
                sbr.AppendLine("   left join  ");
                sbr.AppendLine("   (  ");
                sbr.AppendLine("   SELECT [LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [dFromTime], [dToTime], [vcOrderPlant] FROM [TSPMaster_OrderPlant] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
                sbr.AppendLine("   ) c ON a.vcPackingPlant = c.vcPackingPlant AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcSupplierId = c.vcSupplierId    ");
                sbr.AppendLine("   left join  ");
                sbr.AppendLine("   (  ");
                sbr.AppendLine("   SELECT [LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [dFromTime], [dToTime], [vcSufferIn] FROM [dbo].[TSPMaster_SufferIn] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
                sbr.AppendLine("   ) d ON a.vcPackingPlant = d.vcPackingPlant AND a.vcPartId = d.vcPartId AND a.vcReceiver = d.vcReceiver AND a.vcSupplierId = d.vcSupplierId    ");
                sbr.AppendLine("   left join  ");
                sbr.AppendLine("   (  ");
                sbr.AppendLine("   SELECT [LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [vcSupplierPlant], [dFromTime], [dToTime], [iPackingQty] FROM [TSPMaster_Box] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
                sbr.AppendLine("   ) e ON a.vcPackingPlant = e.vcPackingPlant AND a.vcPartId = e.vcPartId AND a.vcReceiver = e.vcReceiver AND a.vcSupplierId = e.vcSupplierId    ");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 读取txt

        public Order GetPartFromFile(string path, string orderNo, ref string msg)
        {
        string[] strs = File.ReadAllLines(@path);
        Order order = new Order();

        Head head = new Head();
        List<Detail> details = new List<Detail>();
        Tail tail = new Tail();

        //获取Detail
        for (int i = 0; i < strs.Length; i++)
        {
            string temp = strs[i];
            //判断空行
            if (string.IsNullOrWhiteSpace(temp))
            {
                continue;
            }
            //获取Head
            if (temp[0] == 'D')
            {
                Detail detail = new Detail();
                detail.DataId = temp.Substring(0, 1);
                detail.CPD = temp.Substring(1, 5);
                detail.Date = temp.Substring(6, 8);
                detail.Type = temp.Substring(14, 8);
                detail.ItemNo = temp.Substring(22, 4);
                detail.PartsNo = temp.Substring(26, 12);
                detail.QTY = temp.Substring(41, 7);
                detail.Price = temp.Substring(48, 9);
                details.Add(detail);
            }
            else if (temp[0] == 'H')
            {
                head.DataId = temp.Substring(0, 1);
                head.CPD = temp.Substring(1, 5);
                head.Date = temp.Substring(6, 8);
                head.No = temp.Substring(14, 8);
                head.Type = temp.Substring(22, 1);
                head.SendDate = temp.Substring(28, 8);
            }
            else if (temp[0] == 'T')
            {
                tail.DataId = temp.Substring(0, 1);
                tail.CPD = temp.Substring(1, 5);
                tail.Date = temp.Substring(6, 8);
                tail.No = temp.Substring(14, 8);
            }
        }

        order.Head = head;
        order.Details = details;
        order.Tail = tail;

        if (order.Head == null)
        {
            msg = "订单" + orderNo + "Head部分有误";
            return null;
        }
        else if (order.Details.Count == 0)
        {
            msg = "订单" + orderNo + "Detail为空";
            return null;
        }
        else if (order.Tail == null)
        {
            msg = "订单" + orderNo + "Tail部分有误";
            return null;
        }

        return order;
        }

        public class Order
        {
        public Order()
        {
            this.Details = new List<Detail>();
        }

        public Head Head;
        public List<Detail> Details;
        public Tail Tail;
        }

        public class Head
        {
        public string DataId;
        public string CPD;
        public string Date;
        public string No;
        public string Type;
        public string Code;
        public string SendDate;
        }

        public class Detail
        {
        public string DataId;
        public string CPD;
        public string Date;
        public string Type;
        public string ItemNo;
        public string PartsNo;
        public string QTY;
        public string Price;

        }

        public class Tail
        {
        public string DataId;
        public string CPD;
        public string Date;
        public string No;
        }

        #endregion

    }
}
