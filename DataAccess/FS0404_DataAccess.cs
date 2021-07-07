using System;
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
        public DataTable Search(string vcOrderState, string vcInOutFlag,string vcOrderNo, string dTargetDate, string vcOrderType,string vcMemo,string userID)
        {
            try
            {
                //case when [vcOrderState]=2 then '撤销' when [vcOrderState]=3 then '已修正' else '已上传' end as
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" select [iAutoId], [vcOrderNo],  ");
                strSql.AppendLine("    case when vcOrderType='D' then [vcTargetYear]+'/'+[vcTargetMonth]+'/'+[vcTargetDay]  ");
                strSql.AppendLine("    	when vcOrderType='W' then [vcTargetYear]+'/'+[vcTargetMonth]+''+c.vcName   ");
                strSql.AppendLine("    	when vcOrderType='S' then [vcTargetYear]+'/'+[vcTargetMonth]  ");
                strSql.AppendLine("    else [vcTargetYear]+'/'+[vcTargetMonth] end as [dTargetDate],  ");
                strSql.AppendLine("  b.vcOrderDifferentiation as [vcOrderType],e.vcName as  vcInOutFlag,f.vcName  as  vcOrderState, [vcMemo],   ");
                strSql.AppendLine("  [dUploadDate], [vcFilePath], [vcOperatorID], [dOperatorTime],vcLastOrderNo,'0' as vcModFlag,'0' as vcAddFlag from [dbo].[TOrderUploadManage]  a  ");
                strSql.AppendLine("  left join (select vcOrderDifferentiation,vcOrderInitials from [dbo].[TOrderDifferentiation] )b on a.vcOrderType= b.vcOrderInitials     ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C046' )c on a.vcTargetWeek= c.vcValue      ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C003' )e on a.vcInOutFlag= e.vcValue      ");
                strSql.AppendLine("  left join (select vcValue,vcName from Tcode where vcCodeId='C044' )f on a.vcOrderState= f.vcValue      ");
                strSql.AppendLine("  left join ( select vcUnitCode,vcUserID from [dbo].[SUser] )d on a.vcOperatorID= d.vcUserID  where d.vcUnitCode=( select vcUnitCode from [dbo].[SUser] where vcUserID = '"+ userID + "')    ");
                if (vcOrderState.Length > 0)
                {
                    //if (vcOrderState == "2")
                    //{
                    //    strSql.AppendLine("  and vcOrderState = '2' ");
                    //}
                    //else
                    //{
                    //    strSql.AppendLine("  and vcOrderState <>'2' ");
                    //}
                    strSql.AppendLine("  and vcOrderState =  '" + vcOrderState + "' ");
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
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dUploadDate,112) = '" + dTargetDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (vcOrderType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcOrderType = '" + vcOrderType + "' ");
                }
                if (vcMemo.Length>0)
                {
                    strSql.AppendLine("  and  a.vcMemo  like  '" + vcMemo + "%' ");
                }

                strSql.AppendLine("  order by  dOperatorTime desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getStateType()
        {
            throw new NotImplementedException();
        }

        public DataTable checkTSoqDayChange(string dTargetDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" select * from [dbo].[TSoqDayChange] where vcChangeNo='" + dTargetDate + "' ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrderCodeByName(string vcOrderTypeName)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from [dbo].[TOrderDifferentiation] where  vcOrderDifferentiation='"+ vcOrderTypeName + "' ");

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

                vcTargetYear = dTargetDate.Replace("-","").Replace("/", "").Substring(0, 4);
                vcTargetMonth = dTargetDate.Replace("-", "").Replace("/", "").Substring(4, 2);
                if (dTargetDate.Replace("-", "").Replace("/", "").Length == 8)
                {
                    vcTargetDay = dTargetDate.Replace("-", "").Replace("/", "").Substring(6, 2);
                }

                string fileName = string.Empty;
                string filePath = string.Empty;
                string fileOrderNo = string.Empty;
                DataRow[] drArrayZD = null;
                if (vcOrderType == "W")
                {
                    List<string> TargetYM = new List<string>();
                    TargetYM.Add(dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 6));
                    //获取soq验收数量
                    DataTable SoqDt = getSoqDt(TargetYM);
                    string vcMakingOrderType = getTypeMethod(vcOrderType);
                    drArrayZD = SoqDt.Select("vcMakingOrderType in (" + vcMakingOrderType + ") "); //
                }
                //验证品番与订货方式关系及其是否存在主数据表 非月度校验
                #region
                if (vcOrderType!="S")
                { 
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        filePath = fileList[i]["filePath"].ToString();
                        fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                        fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                        DataTable dockTmp = getDockTable(dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 6));
                        DataTable dtRiDuCheck = null;
                        if (vcOrderType == "D")
                        {
                            FS0403_DataAccess fs0403DataAccess = new FS0403_DataAccess();
                            dtRiDuCheck= fs0403DataAccess.getModify(Convert.ToDateTime(dTargetDate));
                        }
                        
                        //读取文件
                        Dictionary<string, string> dicPartNo = new Dictionary<string, string>();
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
                           
                            #region
                             //单条的订单类型是否匹配
                            string vcClassType = detail.ClassType;
                            if (vcClassType!=vcOrderType)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "选中的订单类型与该品番的txt文件的订单类型不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            #endregion
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
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = "订货方式不匹配";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                if (!dicPartNo.ContainsKey(vcPart_id))
                                {
                                    dicPartNo.Add(vcPart_id, vcPart_id);
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = "该品番重复存在！";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                #region 追加日度订单
                                if (vcOrderType == "D")
                                {
                                    if (dtRiDuCheck.Rows.Count == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = fileName;
                                        dataRow["vcPartNo"] = "";
                                        dataRow["vcMessage"] = "当前日的Soq日度订单平准化的品番数量为0条,无法进行日度校验!";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                    else
                                    {
                                        DataRow[] drArrayPart = dtRiDuCheck.Select("vcPart_id='"+vcPart_id+"'"); //获取品番的数量
                                        if (drArrayPart.Length == 0)
                                        {
                                            DataRow dataRow = dtMessage.NewRow();
                                            dataRow["vcOrder"] = fileName;
                                            dataRow["vcPartNo"] = vcPart_id;
                                            dataRow["vcMessage"] = "当前日的Soq日度订单平准化未找到该品番!";
                                            dtMessage.Rows.Add(dataRow);
                                            bReault = false;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(drArrayPart[0]["DayNum"]) == 0)
                                            {
                                                DataRow dataRow = dtMessage.NewRow();
                                                dataRow["vcOrder"] = fileName;
                                                dataRow["vcPartNo"] = vcPart_id;
                                                dataRow["vcMessage"] = "当前日的Soq日度订单平准化品番数量为0!";
                                                dtMessage.Rows.Add(dataRow);
                                                bReault = false;
                                            }
                                            else
                                            {
                                                if (Convert.ToInt32(drArrayPart[0]["DayNum"])!=Convert.ToInt32(vcOrderNum)) {
                                                    DataRow dataRow = dtMessage.NewRow();
                                                    dataRow["vcOrder"] = fileName;
                                                    dataRow["vcPartNo"] = vcPart_id;
                                                    dataRow["vcMessage"] = "当前日的日度订单与Soq日度订单平准化品番的数量"+ Convert.ToInt32(drArrayPart[0]["DayNum"]) + "无法匹配";
                                                    dtMessage.Rows.Add(dataRow);
                                                    bReault = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region
                                if (vcOrderType=="W") {
                                    // 判定品番是否在临时品番里面
                                    bool isExist = false;
                                    for (int m = 0; m < drArrayZD.Length; m++)
                                    {
                                        if (drArrayZD[m]["vcPart_id"].ToString() == vcPart_id)
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
                                        dataRow["vcMessage"] = "文件中的品番不属于Soq周度订单";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }

                                }
                                #endregion


                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "文件中品番基础数据表不包含该品番";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    
                    }
                    if (!bReault)
                    {
                        return;
                    }
                }
                #endregion
                //月度订单单独处理  验证品番个数  內制外注 品番订货数 月度校验
                #region
                
                if (vcOrderType=="S")
                {
                    List<string> TargetYM = new List<string>();
                    TargetYM.Add(dTargetDate.Replace("-", "").Replace("/", ""));
                    //获取soq验收数量
                    DataTable SoqDt = getSoqDt(TargetYM);
                    string vcMakingOrderType = getTypeMethod(vcOrderType);
                    DataRow[] drArrayN = SoqDt.Select("vcInOutFlag='0' AND vcMakingOrderType in (" + vcMakingOrderType + ") "); //0 內制 1外制
                    DataRow[] drArrayW = SoqDt.Select("vcInOutFlag='1' AND vcMakingOrderType in (" + vcMakingOrderType + ") "); //0 內制 1外制
                    DataRow[] drArrayTmp = drArrayN;
                    //DataTable dtNei = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    //foreach (DataRow dr in drArray)
                    //{
                    //    dtNei.ImportRow(dr);
                    //}
                    DataTable dockTmp = getDockTable(dTargetDate.Replace("-", "").Replace("/", ""));
                    Dictionary<string, string> dicPartNoN = new Dictionary<string, string>();
                    Dictionary<string, string> dicPartNoW = new Dictionary<string, string>();
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        filePath = fileList[i]["filePath"].ToString();
                        fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                        fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                        
                        int No = 1;
                        string strInOutflag = string.Empty;
                        //Dictionary<string, string> dicPartNo = new Dictionary<string, string>();
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
                            #region
                            //单条的订单类型是否匹配
                            string vcClassType = detail.ClassType;
                            if (vcClassType != vcOrderType)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "选中的订单类型与该品番的txt文件的订单类型不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            #endregion
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

                                //验证订货方式
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
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = "订货方式不匹配";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                //验证品番个数 及其总数 以及内外区分
                                if (No == 1)
                                {
                                    strInOutflag = inout;
                                    if (inout == "0")
                                    {
                                        drArrayTmp = drArrayN;
                                    }
                                    if (inout == "1")
                                    {
                                        drArrayTmp = drArrayW;
                                    }
                                    No++;
                                }
                                else
                                {
                                    if (inout != strInOutflag) //判定此次文件的内外区分是否一致
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = fileName;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = "文件中的其他品番与第一个品番的内外区分不一样";
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
                                    dataRow["vcMessage"] = "与主数据该品番的内外不对应";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                if (inout == "0")
                                {
                                    if (!dicPartNoN.ContainsKey(vcPart_id))
                                    {
                                        dicPartNoN.Add(vcPart_id, vcPart_id);
                                    }
                                    else
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = fileName;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = "该品番重复存在！";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }

                                } else
                                {
                                    if (!dicPartNoW.ContainsKey(vcPart_id))
                                    {
                                        dicPartNoW.Add(vcPart_id, vcPart_id);
                                    }
                                    else
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = fileName;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = "该品番重复存在！";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }

                                /*if (!dicPartNo.ContainsKey(vcPart_id))
                                {
                                    dicPartNo.Add(vcPart_id, vcPart_id);
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = "该品番重复存在！";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }*/
                                
                                ////检测数量
                                if (SoqDt.Rows.Count>0)
                                {
                                    if (!CheckTotalNumEqual(dTargetDate.Replace("-", "").Substring(0, 6), vcPart_id, vcOrderingMethod, SoqDt, detail.QTY))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = fileName;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = "订单数量不正确";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                } else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = "";
                                    dataRow["vcMessage"] = dTargetDate.Replace("-", "").Substring(0, 6)+"月的月度Replay的数据为0条";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                
                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "文件中品番基础数据表不包含该品番" ;
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                        /*if (dicPartNo.Count != drArrayTmp.Length)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = fileName;
                            dataRow["vcPartNo"] = "";
                            dataRow["vcMessage"] = "文件的品番个数与月度replay的品番个数不一样";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }*/
                    }
                    if (dicPartNoN.Count != drArrayN.Length)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcOrder"] = fileName;
                        dataRow["vcPartNo"] = "";
                        dataRow["vcMessage"] = "文件的內制品番个数与月度replay的內制品番个数不一样";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    if (dicPartNoW.Count != drArrayW.Length)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcOrder"] = fileName;
                        dataRow["vcPartNo"] = "";
                        dataRow["vcMessage"] = "文件的外注品番个数与月度replay的外注品番个数不一样";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    if (!bReault)
                    {
                        return;
                    }
                }
                #endregion
                for (int i=0;i< fileList.Count;i++)
                {
                    filePath = fileList[i]["filePath"].ToString();
                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0,fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-")+1);
                    string msg = string.Empty;
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    string fileNameOrder = order.Head.No;
                    ////订单类型	0	日度
                    //订单类型	1	周度
                    //订单类型	2	月度
                    //订单类型	3	紧急
                    //if (vcOrderType == "2") {
                    //    vcInOutFlag = fileOrderNo.Substring(fileOrderNo.Length - 1, 1);
                    //}

                   
                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType],[vcInOutFlag]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],vcFileOrderNo,vcOrderShowFlag, [vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    //strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("             ('" + fileNameOrder + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "', '" + vcInOutFlag + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "', '" + fileNameOrder + "',1,  ");
                    strSql.AppendLine("  		   '"+ userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");

                    if (vcOrderType == "D")
                    {
                        strSql.AppendLine("   update [dbo].[TSoqDayChange] set vcOrderNo='"+ fileNameOrder + "' where vcChangeNo='"+ dTargetDate.Replace("-", "").Replace("/", "") + "'  ");
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

        #region 获取soq数量

        public DataTable getSoqDt(List<string> TargetYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                foreach (string s in TargetYM)
                {
                    DateTime t2;
                    DateTime.TryParseExact(s + "01", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out t2);
                    string CLYM = t2.AddMonths(-1).ToString("yyyyMM");
                    if (sql.Length > 0)
                    {
                        sql.Append(" OR ");
                    }
                    sql.Append("(vcDXYM = '" + s + "' AND vcCLYM = '" + CLYM + "')");
                }

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcPart_id,vcInOutFlag,vcDXYM,vcFZGC,vcCarType,vcMakingOrderType,iPartNums,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31 FROM TSOQReply");
                sbr.AppendLine(" WHERE 1=1 and isnull(iPartNums,0)<>0 AND ");
                sbr.AppendLine(sql.ToString());
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

                int totalCheck = 0;
                if (getSoqNumMonth(partId, TargetYM, vcOrderingMethod, dt).Count>0)
                {
                    totalCheck = Convert.ToInt32(getSoqNumMonth(partId, TargetYM, vcOrderingMethod, dt)["Total"].ToString());
                }
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

                //vcTargetYear = "";
                //vcTargetMonth = "";
                vcTargetYear = dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 4).ToString();
                vcTargetMonth = dTargetDate.Replace("-", "").Replace("/", "").Substring(4, 2).ToString();
                vcTargetDay = "";
                dTargetWeek = "";
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
                    //DataTable dockTmp = getJinJiDockTable(DateTime.Now.ToString("yyyyMM"));
                    DataTable dockTmp = getJinJiDockTable(dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 6).ToString());
                    Dictionary<string, string> dicPartNo = new Dictionary<string, string>();
                    List<string> TargetYM = new List<string>();

                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    StringBuilder sbr = new StringBuilder();
                    ////追加紧急日期
                    //foreach (Detail detail in order.Details)
                    //{
                    //    string vcPart_id = detail.PartsNo.Trim();
                    //    string CPD = detail.CPD.Trim();
                    //    string vcSeqno = detail.ItemNo.Trim();
                    //    string date = detail.Date;
                    //    if (!TargetYM.Contains(date.Substring(0,6)))
                    //    {
                    //        TargetYM.Add(date.Substring(0, 6));
                    //    }
                    //}
                    //获取dockTmp
                    //Hashtable dockTmp = new Hashtable();
                    //for (int k = 0; k < TargetYM.Count; k++)
                    //{
                    //    DataTable dt = getDockTable(TargetYM[k]);
                    //    dockTmp.Add(TargetYM[k].ToString(), dt);
                    //}
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
                        string TargetTmp = detail.Date.Trim();
                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();

                        string decPriceTNPWithTax = "";
                        string iPackingQty = "";
                        string vcSufferIn = "";
                        string vcOrderPlant = "";
                        #region
                        //单条的订单类型是否匹配
                        string vcClassType = detail.ClassType;
                        if (vcClassType != vcOrderType)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = fileName;
                            dataRow["vcPartNo"] = vcPart_id;
                            dataRow["vcMessage"] = "选中的订单类型与该品番的txt文件的订单类型不匹配";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        #endregion
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getJinJiDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //inout = hashtable["vcInOut"].ToString();
                            //vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();

                            decPriceTNPWithTax = hashtable["decPriceTNPWithTax"].ToString();
                            iPackingQty = hashtable["iPackingQty"].ToString();
                            vcSufferIn = hashtable["vcSufferIn"].ToString();
                            vcOrderPlant = hashtable["vcOrderPlant"].ToString();


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
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "订货方式不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }

                            if (!dicPartNo.ContainsKey(vcPart_id))
                            {
                                dicPartNo.Add(vcPart_id, vcPart_id);
                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "该品番重复存在！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            //判断价格 手配品
                            #region
                            if (decPriceTNPWithTax.Length==0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "价格为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (vcSupplierPlant.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "工区为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (iPackingQty.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "收容数为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (vcSufferIn.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "受入为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (vcOrderPlant.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "发注工场为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            #endregion

                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = fileName;
                            dataRow["vcPartNo"] = vcPart_id;
                            dataRow["vcMessage"] = "文件中品番基础数据表不包含该品番";
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
                    filePath = fileList[i]["filePath"].ToString();
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
                    //获取基础信息
                    DataTable dockTmp = getJinJiDockTable(DateTime.Now.ToString("yyyyMM"));
                    //读取文件
                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    string fileNameOrder = order.Head.No;

                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType],[vcInOutFlag]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],vcFileOrderNo,vcOrderShowFlag,[vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    //strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("             ('" + fileNameOrder + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "', '" + vcInOutFlag + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "', '" + fileNameOrder + "',0,  ");
                    strSql.AppendLine("  		   '" + userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");

                   
                    //List<string> TargetYM = new List<string>();
                    //追加紧急日期
                    //foreach (Detail detail in order.Details)
                    //{
                    //    string vcPart_id = detail.PartsNo.Trim();
                    //    string CPD = detail.CPD.Trim();
                    //    string vcSeqno = detail.ItemNo.Trim();
                    //    string date = detail.Date;
                    //    if (!TargetYM.Contains(date.Substring(0, 6)))
                    //    {
                    //        TargetYM.Add(date.Substring(0, 6));
                    //    }
                    //}
                    //获取dockTmp
                    //Hashtable dockTmp = new Hashtable();
                    //for (int k = 0; k < TargetYM.Count; k++)
                    //{
                    //    DataTable dt = getDockTable(TargetYM[k]);
                    //    dockTmp.Add(TargetYM[k].ToString(), dt);
                    //}

                    StringBuilder sbr = new StringBuilder();
                    //判断头
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string vcSupplierId = "";

                        string vcInOut = "";
                        string vcSupplierPlant = "";//工区
                        string vcSupplierPlace = "";//出荷场
                        string vcOrderNum = detail.QTY;
                        string vcOrderPlant = "";
                        string vcHaoJiu = "";
                        string vcOESP = "";
                        string vcSufferIn = "";
                        string iPackingQty = "";
                        string TargetTmp = detail.Date.Trim();
                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getJinJiDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //sbr.AppendLine("   SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSupplierPlant,a.vcPackingPlant,  ");
                            //sbr.AppendLine("   a.vcInOut,a.vcSupplierPlace,a.vcOrderingMethod,c.vcOrderPlant,a.vcOESP,a.vcHaoJiu,d.vcSufferIn,e.iPackingQty FROM     ");
                            vcInOut = hashtable["vcInOut"].ToString();
                            vcOrderPlant = hashtable["vcOrderPlant"].ToString();
                            vcHaoJiu = hashtable["vcHaoJiu"].ToString();
                            vcOESP = hashtable["vcOESP"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            vcSufferIn = hashtable["vcSufferIn"].ToString();
                            iPackingQty = hashtable["iPackingQty"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                        }
                        
                        strSql.Append("  insert into  [dbo].[TUrgentOrder] (vcOrderNo,vcPart_id,vcSupplier_id,vcGQ,vcChuHePlant,  ");
                        strSql.Append("  iOrderQuantity,vcStatus,vcDelete,vcInOut,vcOrderPlant,vcHaoJiu,vcOESP,vcSufferIn,iPackingQty,vcOperatorID,dOperatorTime ) values (  ");
                        //strSql.Append("  '" + fileName + "',  ");
                        strSql.Append("  '" + fileNameOrder + "',  ");
                        strSql.Append("  '"+ vcPart_id + "',  ");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierPlant, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSupplierPlace, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcOrderNum, false) + ",");
                        strSql.Append("'0',");
                        strSql.Append("'0',");
                        strSql.Append(ComFunction.getSqlValue(vcInOut, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcOrderPlant, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcHaoJiu, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcOESP, false) + ",");
                        //strSql.Append(ComFunction.getSqlValue(vcSupplierPlace, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(vcSufferIn, false) + ",");
                        strSql.Append(ComFunction.getSqlValue(iPackingQty, false) + ",");
                        strSql.AppendLine("  '" + userId + "',GETDATE());   ");
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

        public void updateEditeOrderNo(string realPath,string vcOrderType, string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId, string uionCode, ref bool  bReault, ref DataTable dtMessage, ref string msg,string vcJiLuLastOrderNo)
        {
            try
            {
                #region 获取原订单
                StringBuilder strSq0 = new StringBuilder();
                strSq0.AppendLine(" select * from TOrderUploadManage where vcOrderNo='"+ lastOrderNo + "'  ");
                DataTable dtLastOrder = excute.ExcuteSqlWithSelectToDT(strSq0.ToString());
                string filePathYS = dtLastOrder.Rows[0]["vcFilePath"].ToString();
                string fileNameYS = dtLastOrder.Rows[0]["vcOrderNo"].ToString();
                Dictionary<string, int> dicOrderYS = new Dictionary<string, int>();
                //读取Order
                Order orderYS = GetPartFromFile(realPath + filePathYS, fileNameYS, ref msg);//判断头
                foreach (Detail detail in orderYS.Details)
                {
                    string vcPart_id = detail.PartsNo.Trim();
                    string CPD = detail.CPD.Trim();
                    string vcSeqno = detail.ItemNo.Trim();
                    int vcOrderNum = Convert.ToInt32(detail.QTY);
                    dicOrderYS.Add(vcPart_id, vcOrderNum);
                }
                StringBuilder strSq01 = new StringBuilder();
                strSq01.AppendLine(" select * from TOrderUploadManage where vcLastOrderNo='" + lastOrderNo + "'  ");
                DataTable dtLastOrderChild = excute.ExcuteSqlWithSelectToDT(strSq01.ToString());
                for (int i=0;i<dtLastOrderChild.Rows.Count;i++) {

                    string filePathYSChild = dtLastOrderChild.Rows[0]["vcFilePath"].ToString();
                    string fileNameYSChild = dtLastOrderChild.Rows[0]["vcOrderNo"].ToString();
                    //读取Order
                    Order orderYSChild = GetPartFromFile(realPath + filePathYSChild, fileNameYSChild, ref msg);//判断头
                    foreach (Detail detail in orderYSChild.Details)
                    {
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        int vcOrderNum = Convert.ToInt32(detail.QTY);
                        if (dicOrderYS.ContainsKey(vcPart_id))
                        {
                            int num = dicOrderYS[vcPart_id];
                            dicOrderYS[vcPart_id] = num - Convert.ToInt32(vcOrderNum);
                        }
                    }
                }
                #endregion

                StringBuilder strSql1 = new StringBuilder();
                strSql1.AppendLine(" select b.vcValue,c.vcOrderInitials from [dbo].[TOrderGoodsAndDifferentiation] a  ");
                strSql1.AppendLine(" left join TCode b on a.vcOrderGoodsId=b.iAutoId  ");
                strSql1.AppendLine(" left join [dbo].[TOrderDifferentiation] c on a.vcOrderDifferentiationId = c.iAutoId  ");
                DataTable dtOrderType = excute.ExcuteSqlWithSelectToDT(strSql1.ToString());

                StringBuilder strSql = new StringBuilder();
                string vcTargetYear = string.Empty;
                string vcTargetMonth = string.Empty;
                string vcTargetDay = string.Empty;
                vcTargetYear = dTargetDate.Replace("-", "").Replace("/", "").Substring(0,4).ToString();
                vcTargetMonth = dTargetDate.Replace("-", "").Replace("/", "").Substring(4, 2).ToString();
                string fileName = string.Empty;
                string filePath = string.Empty;
                string fileOrderNo = string.Empty;
                DataTable dockTmpNQ = new DataTable();
                if (vcJiLuLastOrderNo.Length > 0)
                {
                     dockTmpNQ = getJinJiNQ(vcJiLuLastOrderNo, dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 6).ToString());
                }
                else
                {
                     dockTmpNQ = getJinJiNQ(lastOrderNo, dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 6).ToString());
                }
                

                for (int i = 0; i < fileList.Count; i++)
                {
                    filePath = fileList[i]["filePath"].ToString();
                    fileName = fileList[i]["fileName"].ToString().Trim().Substring(0, fileList[i]["fileName"].ToString().Trim().LastIndexOf("."));
                    fileOrderNo = fileName.Substring(fileName.LastIndexOf("-") + 1);//获取基础信息
                    //DataTable dockTmp = getJinJiDockTable(DateTime.Now.ToString("yyyyMM"));
                    DataTable dockTmp = getJinJiDockTable(dTargetDate.Replace("-", "").Replace("/", "").Substring(0, 6).ToString());
                    Dictionary<string, string> dicPartNo = new Dictionary<string, string>();
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    //List<string> TargetYM = new List<string>();
                    //追加紧急日期
                    //foreach (Detail detail in order.Details)
                    //{
                    //    string vcPart_id = detail.PartsNo.Trim();
                    //    string CPD = detail.CPD.Trim();
                    //    string vcSeqno = detail.ItemNo.Trim();
                    //    string date = detail.Date;
                    //    if (!TargetYM.Contains(date.Substring(0, 6)))
                    //    {
                    //        TargetYM.Add(date.Substring(0, 6));
                    //    }
                    //}
                    ////获取dockTmp
                    //Hashtable dockTmp = new Hashtable();
                    //for (int k = 0; k < TargetYM.Count; k++)
                    //{
                    //    DataTable dt = getDockTable(TargetYM[k]);
                    //    dockTmp.Add(TargetYM[k].ToString(), dt);
                    //}
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
                        string TargetTmp = detail.Date.Trim();
                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();

                        string decPriceTNPWithTax = "";
                        string iPackingQty = "";
                        string vcSufferIn = "";
                        string vcOrderPlant = "";
                        #region
                        //单条的订单类型是否匹配
                        string vcClassType = detail.ClassType;
                        if (vcClassType != vcOrderType)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = fileName;
                            dataRow["vcPartNo"] = vcPart_id;
                            dataRow["vcMessage"] = "选中的订单类型与该品番的txt文件的订单类型不匹配";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        #endregion
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getJinJiDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            //inout = hashtable["vcInOut"].ToString();
                            //vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcSupplierPlant = hashtable["vcSupplierPlant"].ToString();
                            vcSupplierPlace = hashtable["vcSupplierPlace"].ToString();
                            //vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();

                            decPriceTNPWithTax = hashtable["decPriceTNPWithTax"].ToString();
                            iPackingQty = hashtable["iPackingQty"].ToString();
                            vcSufferIn = hashtable["vcSufferIn"].ToString();
                            vcOrderPlant = hashtable["vcOrderPlant"].ToString();

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
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "订货方式不匹配";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (!dicPartNo.ContainsKey(vcPart_id))
                            {
                                dicPartNo.Add(vcPart_id, vcPart_id);
                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "该品番重复存在！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (dicOrderYS.ContainsKey(vcPart_id))
                            {
                                int num = dicOrderYS[vcPart_id];
                                if (num - Convert.ToInt32(vcOrderNum) >= 0)
                                {
                                    dicOrderYS[vcPart_id] = num - Convert.ToInt32(vcOrderNum);
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = "本次修正的品番数量+累计修正订单的品番数量不能大于原始文件的品番数量";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "修正订单的品番不存在于原始订单文件里面";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            //
                            Hashtable hashtableNQ = getJinJiDockNQ(vcPart_id, dockTmpNQ);
                            if (hashtableNQ.Keys.Count > 0)
                            {
                                string numNQ = hashtableNQ["num"].ToString();
                                if (Convert.ToInt32(numNQ) - Convert.ToInt32(vcOrderNum) < 0) {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = fileName;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = "修正订单的品番的数量不应该大于纳期表确认的数量";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "纳期表未找到数量";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }

                            //判断价格 手配品
                            #region
                            if (decPriceTNPWithTax.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "价格为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (vcSupplierPlant.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "工区为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (iPackingQty.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "收容数为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (vcSufferIn.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "受入为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            if (vcOrderPlant.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = fileName;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = "发注工场为空！";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                            #endregion
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = fileName;
                            dataRow["vcPartNo"] = vcPart_id;
                            dataRow["vcMessage"] = "文件中品番基础数据表不包含该品番";
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
                    string chushiLastOrderNo = string.Empty;
                    if (vcJiLuLastOrderNo.Length > 0)
                    {
                        chushiLastOrderNo = vcJiLuLastOrderNo;
                    }
                    else
                    {
                        chushiLastOrderNo = lastOrderNo;
                    }
                    //读取Order
                    Order order = GetPartFromFile(realPath + filePath, fileName, ref msg);
                    string fileNameOrder = order.Head.No;
                    vcMemo = lastOrderNo + "----->" + fileNameOrder;
                    strSql.AppendLine("  INSERT INTO [dbo].[TOrderUploadManage]   ");
                    strSql.AppendLine("             ([vcOrderNo] ,[vcTargetYear]   ");
                    strSql.AppendLine("             ,[vcTargetMonth] ,[vcTargetDay]   ");
                    strSql.AppendLine("             ,[vcTargetWeek]  ,[vcOrderType],[vcInOutFlag]   ");
                    strSql.AppendLine("             ,[vcOrderState],[vcMemo]   ");
                    strSql.AppendLine("             ,[dUploadDate]   ");
                    strSql.AppendLine("             ,[vcFilePath],vcFileOrderNo,vcOrderShowFlag,vcLastOrderNo,[vcOperatorID],[dOperatorTime])   ");
                    strSql.AppendLine("       VALUES   ");
                    //strSql.AppendLine("             ('" + fileName + "',   ");
                    strSql.AppendLine("             ('" + fileNameOrder + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetYear + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetMonth + "',   ");
                    strSql.AppendLine("  		   '" + vcTargetDay + "',   ");
                    strSql.AppendLine("  		   '" + dTargetWeek + "',   ");
                    strSql.AppendLine("  		   '" + vcOrderType + "', '" + vcInOutFlag + "',   ");
                    strSql.AppendLine("  		   '" + 0 + "',   ");
                    strSql.AppendLine("  		   '" + vcMemo + "',   ");
                    strSql.AppendLine("  		    GETDATE(),   ");
                    strSql.AppendLine("  		   '" + filePath + "', '" + fileNameOrder + "',0,'"+ chushiLastOrderNo + "',  ");
                    strSql.AppendLine("  		   '" + userId + "',GETDATE())   ");
                    strSql.AppendLine("   ;  ");
                    strSql.AppendLine("   insert into TUploadOrderRelation (vcLastOrderNo,vcNewOrderNo) values ('"+ chushiLastOrderNo + "','"+ fileNameOrder + "');   ");
                   
                    //获取基础信息
                    DataTable dockTmp = getJinJiDockTable(DateTime.Now.ToString("yyyyMM"));
                    //读取文件

                    string vcPackingFactory = uionCode;
                    string vcOrderNo = fileName;

                   
                    //List<string> TargetYM = new List<string>();
                    //foreach (Detail detail in order.Details)
                    //{
                    //    string vcPart_id = detail.PartsNo.Trim();
                    //    string CPD = detail.CPD.Trim();
                    //    string vcSeqno = detail.ItemNo.Trim();
                    //    string date = detail.Date;
                    //    if (!TargetYM.Contains(date.Substring(0, 6)))
                    //    {
                    //        TargetYM.Add(date.Substring(0, 6));
                    //    }
                    //}
                    ////获取dockTmp
                    //Hashtable dockTmp = new Hashtable();
                    //for (int k = 0; k < TargetYM.Count; k++)
                    //{
                    //    DataTable dt = getDockTable(TargetYM[k]);
                    //    dockTmp.Add(TargetYM[k].ToString(), dt);
                    //}
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
                        string TargetTmp = detail.Date.Trim();
                        string dateTime = detail.Date.Trim();
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getJinJiDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
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
                        
                        strSql.Append("  insert into  [dbo].[TUrgentOrder] (vcOrderNo,vcPart_id,[vcOrderPlant], [vcInOut], [vcHaoJiu], [vcOESP],[vcSupplier_id], vcGQ,vcChuHePlant,[vcSufferIn], [iPackingQty],  ");
                        strSql.Append("  iOrderQuantity,vcStatus,vcDelete,vcOperatorID,dOperatorTime ) values (  ");
                        strSql.Append("  '" + fileNameOrder + "',  ");
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
                        strSql.AppendLine("  '" + userId + "',GETDATE());   ");
                    }
                   
                }
                strSql.AppendLine("   update TOrderUploadManage  set  vcOrderState='3',vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where  vcOrderNo='" + lastOrderNo + "';  ");
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

        private Hashtable getJinJiDockNQ(string PartID, DataTable dt)
        {
            try
            {
                Hashtable hashtable = new Hashtable();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (PartID.Trim().Equals(dt.Rows[i]["vcPart_id"].ToString().Trim()))
                        {
                            hashtable.Add("num", dt.Rows[i]["num"].ToString());
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

        /// <summary>
        /// 得到紧急纳期
        /// </summary>
        /// <param name="vcJiLuLastOrderNo"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private DataTable getJinJiNQ(string vcJiLuLastOrderNo, string TargetYM)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("   select vcOrderNo,vcPart_id, vcTargetYM,sum(iDuiYingQuantity) as num from    ");
                sbr.AppendLine("   (    ");
                sbr.AppendLine("   select vcOrderNo,vcPart_id,  CONVERT(varchar(6),dOutPutDate,112) as vcTargetYM, iDuiYingQuantity     ");
                sbr.AppendLine("   from VI_UrgentOrder_OperHistory where CONVERT(varchar(6),dOutPutDate,112) ='"+ TargetYM + "' and  vcOrderNo='"+ vcJiLuLastOrderNo + "'  ");
                sbr.AppendLine("   ) a group by a.vcOrderNo,a.vcPart_id, a.vcTargetYM    ");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region  来自王继伟
        public string getTypeMethod(string vcType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcValue FROM TCode WHERE iAutoId IN(");
                sbr.AppendLine("SELECT vcOrderGoodsId FROM TOrderGoodsAndDifferentiation WHERE vcOrderDifferentiationId in");
                sbr.AppendLine("(SELECT iAutoId FROM TOrderDifferentiation WHERE vcOrderInitials = '" + vcType + "')) ");

                string res = "";

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            res += ",";
                        }
                        res += "'" + dt.Rows[i]["vcValue"] + "'";
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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

        public Hashtable getJinJiDock(string PartID, string receiver, string vcPackingPlant, DataTable dt)
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
                            hashtable.Add("decPriceTNPWithTax", dt.Rows[i]["decPriceTNPWithTax"].ToString());
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
        //public DataTable getDockTable()
        //{
        //    try
        //    {
        //        StringBuilder sbr = new StringBuilder();
        //        sbr.AppendLine("   SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSupplierPlant,a.vcPackingPlant,  ");
        //        sbr.AppendLine("   a.vcInOut,a.vcSupplierPlace,a.vcOrderingMethod,c.vcOrderPlant,a.vcOESP,a.vcHaoJiu,d.vcSufferIn,e.iPackingQty FROM     ");
        //        sbr.AppendLine("   (    ");
        //        sbr.AppendLine("   SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcSupplierPlace,vcOESP,vcHaoJiu FROM TSPMaster WHERE     ");
        //        sbr.AppendLine("    isnull(vcDelete, '') <> '1' and  dFromTime <= GETDATE() AND dToTime >= GETDATE()     ");
        //        sbr.AppendLine("   ) a    ");
        //        sbr.AppendLine("   LEFT JOIN    ");
        //        sbr.AppendLine("   (    ");
        //        sbr.AppendLine("   SELECT [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [dFromTime], [dToTime], [vcSupplierPlant] FROM [TSPMaster_SupplierPlant] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
        //        sbr.AppendLine("   ) b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId    ");
        //        sbr.AppendLine("   left join  ");
        //        sbr.AppendLine("   (  ");
        //        //sbr.AppendLine("   SELECT [LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [dFromTime], [dToTime], [vcOrderPlant] FROM [TSPMaster_OrderPlant] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
        //        //sbr.AppendLine("   ) c ON a.vcPackingPlant = c.vcPackingPlant AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcSupplierId = c.vcSupplierId    ");
        //        sbr.AppendLine("  select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],  ");
        //        sbr.AppendLine("  vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'   ");
        //        sbr.AppendLine("  and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23)  ");
        //        sbr.AppendLine("  ) c on a.[vcSupplierId]=c.[vcSupplierId] AND b.vcSupplierPlant=c.vcSupplierPlant    ");
        //        sbr.AppendLine("   left join  ");
        //        sbr.AppendLine("   (  ");
        //        sbr.AppendLine("   SELECT [LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [dFromTime], [dToTime], [vcSufferIn] FROM [dbo].[TSPMaster_SufferIn] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
        //        sbr.AppendLine("   ) d ON a.vcPackingPlant = d.vcPackingPlant AND a.vcPartId = d.vcPartId AND a.vcReceiver = d.vcReceiver AND a.vcSupplierId = d.vcSupplierId    ");
        //        sbr.AppendLine("   left join  ");
        //        sbr.AppendLine("   (  ");
        //        sbr.AppendLine("   SELECT [LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [vcSupplierPlant], [dFromTime], [dToTime], [iPackingQty] FROM [TSPMaster_Box] WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1'     ");
        //        sbr.AppendLine("   ) e ON a.vcPackingPlant = e.vcPackingPlant AND a.vcPartId = e.vcPartId AND a.vcReceiver = e.vcReceiver AND a.vcSupplierId = e.vcSupplierId    ");
        //        return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public DataTable getDockTable(string TargetYM)
        {
            try
            {
                DateTime timeFrom = DateTime.Parse(TargetYM.Substring(0, 4) + "-" + TargetYM.Substring(4, 2) + "-01");
                DateTime timeTo = timeFrom;
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcOESP,e.iPackingQty,a.vcSupplierPlace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod,c.vcSupplierPlant,vcHaoJiu,d.vcOrderPlant,vcSupplierPacking FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT vcSupplierId,vcOESP,vcSupplierPlace,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcHaoJiu,vcSupplierPacking FROM TSPMaster WHERE ");
                sbr.AppendLine("	dFromTime <= '" + timeFrom + "' AND dToTime >='" + timeTo + "' and isnull(vcDelete, '') <> '1' ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "' AND vcOperatorType = '1' ");
                sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcSupplierPlant,vcSupplierId,vcPartId,vcReceiver,vcPackingPlant FROM TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "'");
                sbr.AppendLine(") c ON a.vcSupplierId = b.vcSupplierId AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcPackingPlant = c.vcPackingPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcValue1 as vcSupplierId,vcValue2 as vcSupplierPlant,vcValue3 as dFromTime,vcValue4 as dToTime,vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0' AND vcValue3<=CONVERT(VARCHAR(10),CONVERT(datetime,'" + timeFrom + "'),23) AND vcValue4>=CONVERT(VARCHAR(10),CONVERT(datetime,'" + timeTo + "'),23)");
                sbr.AppendLine(") d ON a.vcSupplierId = d.vcSupplierId AND c.vcSupplierPlant = d.vcSupplierPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select  vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime, iPackingQty,  vcOperatorType, dOperatorTime from TSPMaster_Box WHERE vcOperatorType = '1' AND dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "'");
                sbr.AppendLine(") e ON a.vcSupplierId = e.vcSupplierId AND a.vcPartId = e.vcPartId AND a.vcReceiver = e.vcReceiver AND a.vcPackingPlant = e.vcPackingPlant");
                //sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod,c.vcSupplierPlant,vcHaoJiu,d.vcOrderPlant,vcSupplierPacking FROM ");
                //sbr.AppendLine("(");
                //sbr.AppendLine("	SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcHaoJiu,vcSupplierPacking FROM TSPMaster WHERE ");
                //sbr.AppendLine("	dFromTime <= GETDATE() AND dToTime >= GETDATE() ");
                //sbr.AppendLine(") a");
                //sbr.AppendLine("LEFT JOIN");
                //sbr.AppendLine("(");
                //sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1' ");
                //sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                //sbr.AppendLine("LEFT JOIN");
                //sbr.AppendLine("(");
                //sbr.AppendLine("SELECT vcSupplierPlant,vcSupplierId,vcPartId,vcReceiver,vcPackingPlant FROM TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= GETDATE() AND dToTime >= GETDATE()");
                //sbr.AppendLine(") c ON a.vcSupplierId = b.vcSupplierId AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcPackingPlant = c.vcPackingPlant");
                //sbr.AppendLine("LEFT JOIN");
                //sbr.AppendLine("(");
                //sbr.AppendLine("select vcValue1 as vcSupplierId,vcValue2 as vcSupplierPlant,vcValue3 as dFromTime,vcValue4 as dToTime,vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0' AND vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23)");
                //sbr.AppendLine(") d ON a.vcSupplierId = d.vcSupplierId AND c.vcSupplierPlant = d.vcSupplierPlant");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getJinJiDockTable(string TargetYM)
        {
            try
            {
                DateTime timeFrom = DateTime.Parse(TargetYM.Substring(0, 4) + "-" + TargetYM.Substring(4, 2) + "-01");
                DateTime timeTo = timeFrom;
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcOESP,f.decPriceTNPWithTax,e.iPackingQty,a.vcSupplierPlace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod,c.vcSupplierPlant,vcHaoJiu,d.vcOrderPlant,vcSupplierPacking FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT vcSupplierId,vcOESP,vcSupplierPlace,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcHaoJiu,vcSupplierPacking FROM TSPMaster WHERE ");
                sbr.AppendLine("	dFromTime <= '" + timeFrom + "' AND dToTime >='" + timeTo + "' and isnull(vcDelete, '') <> '1' ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "' AND vcOperatorType = '1' ");
                sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcSupplierPlant,vcSupplierId,vcPartId,vcReceiver,vcPackingPlant FROM TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "'");
                sbr.AppendLine(") c ON a.vcSupplierId = b.vcSupplierId AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcPackingPlant = c.vcPackingPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcValue1 as vcSupplierId,vcValue2 as vcSupplierPlant,vcValue3 as dFromTime,vcValue4 as dToTime,vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0' AND vcValue3<=CONVERT(VARCHAR(10),CONVERT(datetime,'" + timeFrom + "'),23) AND vcValue4>=CONVERT(VARCHAR(10),CONVERT(datetime,'" + timeTo + "'),23)");
                sbr.AppendLine(") d ON a.vcSupplierId = d.vcSupplierId AND c.vcSupplierPlant = d.vcSupplierPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select  vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime, iPackingQty,  vcOperatorType, dOperatorTime from TSPMaster_Box WHERE vcOperatorType = '1' AND dFromTime <= '" + timeFrom + "' AND dToTime >= '" + timeTo + "'");
                sbr.AppendLine(") e ON a.vcSupplierId = e.vcSupplierId AND a.vcPartId = e.vcPartId AND a.vcReceiver = e.vcReceiver AND a.vcPackingPlant = e.vcPackingPlant");
                sbr.AppendLine("    left join     \r\n ");
                sbr.AppendLine("    (     \r\n ");
                sbr.AppendLine("       select decPriceTNPWithTax,vcPart_id from TPrice where  convert(varchar(6),dPricebegin,112)<='" + TargetYM + "' and convert(varchar(6),dPriceEnd,112)>='" + TargetYM + "'     \r\n ");
                sbr.AppendLine("    )f on a.vcPartId=f.vcPart_id     \r\n ");

                //sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod,c.vcSupplierPlant,vcHaoJiu,d.vcOrderPlant,vcSupplierPacking FROM ");
                //sbr.AppendLine("(");
                //sbr.AppendLine("	SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcHaoJiu,vcSupplierPacking FROM TSPMaster WHERE ");
                //sbr.AppendLine("	dFromTime <= GETDATE() AND dToTime >= GETDATE() ");
                //sbr.AppendLine(") a");
                //sbr.AppendLine("LEFT JOIN");
                //sbr.AppendLine("(");
                //sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1' ");
                //sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                //sbr.AppendLine("LEFT JOIN");
                //sbr.AppendLine("(");
                //sbr.AppendLine("SELECT vcSupplierPlant,vcSupplierId,vcPartId,vcReceiver,vcPackingPlant FROM TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= GETDATE() AND dToTime >= GETDATE()");
                //sbr.AppendLine(") c ON a.vcSupplierId = b.vcSupplierId AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcPackingPlant = c.vcPackingPlant");
                //sbr.AppendLine("LEFT JOIN");
                //sbr.AppendLine("(");
                //sbr.AppendLine("select vcValue1 as vcSupplierId,vcValue2 as vcSupplierPlant,vcValue3 as dFromTime,vcValue4 as dToTime,vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0' AND vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23)");
                //sbr.AppendLine(") d ON a.vcSupplierId = d.vcSupplierId AND c.vcSupplierPlant = d.vcSupplierPlant");

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
                detail.ClassType = temp.Substring(14, 1);
                detail.Type = temp.Substring(14, 8);
                detail.ItemNo = temp.Substring(22, 4);
                detail.PartsNo = temp.Substring(26, 12).Replace(" ","0");
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
        public string ClassType;
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
