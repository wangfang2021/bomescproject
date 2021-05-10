using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Common;
using System.Text;

namespace Logic
{
    public class FS1209_Logic
    {
        public string getRoleTip(string ID)
        {
            string ssql = "select vcTips from SUserPorType where vcUserID ='" + ID + "' ";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0] != null ? dt.Rows[0][0].ToString() : string.Empty;
            return string.Empty;
        }

        public int Create_uuidTable(DataTable pDt, string uuid_Name)
        {
            string sql = "";
            sql = "CREATE TABLE " + uuid_Name + @" (";
            foreach (DataColumn col in pDt.Columns)
            {
                sql = sql + col.ColumnName;
                sql = sql + @"  " + SqlDbType.VarChar + @"(1000) ,";
                #region 
                //switch (col.DataType.Name)
                //{

                //    case "Int":
                //    case "Int32":
                //    case "Int16":
                //    case "Int64":
                //        sql = sql + @"  " + SqlDbType.Int + @" ,";
                //        break;
                //    //case TypeCode.Decimal:
                //    //case TypeCode.Double:
                //    case "Decimal":
                //    case "Single":
                //    case "Double":
                //        sql = sql + @"  " + SqlDbType.Decimal + @" ,";
                //        break;
                //    //case TypeCode.DateTime:
                //    case "DateTime":
                //    case "Date":
                //        sql = sql + @"  " + SqlDbType.DateTime2 + @" ,";
                //        break;

                //    //case TypeCode.String:
                //    case "String":
                //        sql = sql + @"  " + SqlDbType.NVarChar + @" ,";
                //        break;

                //    //case TypeCode.Boolean:
                //    case "Boolean":
                //        sql = sql + @"  " + SqlDbType.Bit + @" ,";
                //        break;

                //    default:
                //        sql = sql + @"  " + SqlDbType.NVarChar + @" ,";
                //        break;

                //}
                #endregion
            }
            sql = sql.Substring(0, sql.Length - 1) + ")";
            return excute.ExecuteSQLNoQuery(sql);
        }

        public string CreateTempTable(DataTable dt)
        {
            string uuid = "FS1209" + System.Guid.NewGuid().ToString().Replace("-", "");
            DataTable uuid_tb = excute.ExcuteSqlWithSelectToDT("select * from sysobjects where id=object_id(N'" + uuid + "') and OBJECTPROPERTY(id, N'IsUserTable')=1");
            if (uuid_tb != null && uuid_tb.Rows.Count > 0)
                return "已在被打印中";
            Create_uuidTable(uuid_tb, uuid);

            using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                conn.Open();
                //批量数据处理SQL表
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                try
                {
                    bulkCopy.DestinationTableName = uuid;//要插入的SQL表的表名
                    bulkCopy.BatchSize = dt.Rows.Count;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        bulkCopy.ColumnMappings.Add(i, i);//映射字段名 DataTable列 ,数据库 对应的列  
                    }
                    bulkCopy.WriteToServer(dt);     //复制到SQL指定表
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return "";
        }


        #region 检索
        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="vcType"></param>
        /// <param name="vcPrintPartNo"></param>
        /// <param name="vcLianFan"></param>
        /// <param name="vcPorPlant"></param>
        /// <param name="vcKbOrderId"></param>
        /// <param name="vcPorType"></param>
        /// <param name="vcUserId"></param>
        /// <returns></returns>
        public DataTable Search(string vcType, string vcPrintPartNo, string vcLianFan, string vcPorPlant, string vcKbOrderId, string vcPorType, string vcUserId)
        {
            string RolePorType = getRoleTip(vcUserId);
            DataTable dtportype = dllPorType(RolePorType.Split('*'));
            DataTable resutPrint = new DataTable();
            if (vcPorType != "PP")
            {
                if (vcType == "3")
                {
                    resutPrint = searchPrint(vcPrintPartNo, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, dtportype);
                }
                else
                {
                    resutPrint = searchPrint(vcPrintPartNo, vcType, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, dtportype);
                }
                if (resutPrint.Rows.Count != 0)
                {
                    resutPrint = returnED(resutPrint);
                }
            }
            return resutPrint;
        }
        #endregion


        #region 原代码搬移

        private MultiExcute excute = new MultiExcute();

        #region 函数applendWhereIfNeed
        bool hasWhere = false;
        private bool applendWhereIfNeed(StringBuilder strSQL, bool haswhere)
        {
            if (haswhere == false)
            {
                strSQL.AppendLine(" WHERE ");
                return true;
            }
            else
            {
                strSQL.AppendLine(" AND ");
                return true;
            }
        }
        #endregion

        #region 检索信息栏绑定生产部署 str2是权限部署
        public DataTable dllPorType(string[] str2)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            if (str2.Length != 0)
            {
                if (str2[0] != "admin")
                {
                    string ProType = "";
                    if (str2.Length != 0)
                    {
                        ProType += "'";
                        for (int i = 0; i < str2.Length; i++)
                        {
                            ProType += str2[i].ToString();
                            if (i < str2.Length - 1)
                            {
                                ProType += "','";
                            }
                            else
                            {
                                ProType += "'";
                            }
                        }
                    }
                    strSQL.AppendLine("select '' as [Text],'0' as [Value]   ");
                    strSQL.AppendLine(" union all ");
                    strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType' and [vcData1] in(" + ProType + ") ");
                }
                else
                {
                    strSQL.AppendLine("select '' as [Text],'0' as [Value]   ");
                    strSQL.AppendLine(" union all ");
                    strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
                }
            }
            else
            {
                strSQL.AppendLine("select '' as [Text],'PP' as [Value]   ");
            }
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #region 绑定工场 1 2 3 厂
        public DataTable dllPorPlant()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select '' as [Text],'0' as [Value]  union all ");
            strSQL.AppendLine(" select distinct [vcData2] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='KBPlant'");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #region 确认当值是否有未打印的数据 适用于看板打印页面和看板再发行页面
        public bool KanBIfPrint(DataTable dtflag)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("declare @banzhi varchar(1) ");
            strSQL.AppendLine("declare @vcComDate varchar(10) ");
            strSQL.AppendLine("set @banzhi=(select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanend as varchar(8)) from Mbaiye)) then '0' ");
            strSQL.AppendLine("                          when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),DateAdd(DAY,1,GETDATE()),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                          when GETDATE()>=(select CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                          else'' end)as [by] )");
            strSQL.AppendLine("set @vcComDate=(select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' 00:00:00.000') and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' 06:00:00.000') then CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)");
            strSQL.AppendLine("                          else  CONVERT(varchar(10),GETDATE(),121) end) as [by1])");
            strSQL.AppendLine(" SELECT COUNT(*) FROM ( ");
            strSQL.AppendLine(" (select * from tKanbanPrintTbl)A ");
            strSQL.AppendLine(" left join  ");
            strSQL.AppendLine(" (select * from tPartInfoMaster)B ");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock) ");
            strSQL.AppendLine("  where A.vcPrintflag is null and A.vcComDate00=CONVERT(varchar(10),@vcComDate,121) and A.vcBanZhi00=@banzhi ");
            string flag = "";
            if (dtflag.Rows.Count != 0)
            {
                if (!(dtflag.Rows[0]["Text"].ToString() == "admin" && dtflag.Rows.Count == 1))
                {
                    flag += "'";
                    for (int i = 0; i < dtflag.Rows.Count; i++)
                    {
                        flag += dtflag.Rows[i]["Text"].ToString().Trim();
                        if (i < dtflag.Rows.Count - 1)
                        {
                            flag += "','";
                        }
                        else
                        {
                            flag += "'";
                        }
                    }
                }
                strSQL.AppendLine(" and B.vcPorType in( " + flag + ")");
            }
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows[0][0].ToString() != "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 获取所属打印机的名称
        public string PrintMess(string userid)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcPrinterName]");
            strSQL.AppendLine("  FROM [tPrint]");
            strSQL.AppendLine(" WHERE [vcUserFlag]='" + userid + "'");
            strSQL.AppendLine("");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 检索看板打印信息 检索的是非秦丰和秦丰ED的看板数据
        public DataTable searchPrint(string vcPrintPartNo, string vcType, string vcKbOrderId, string vcLianFan, string vcPorType, string vcPlant, DataTable dtflag)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" declare @banzhi varchar(1) ");
            strSQL.AppendLine(" set @banzhi=(select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanend as varchar(8)) from Mbaiye)) then '0' ");
            strSQL.AppendLine("                           when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),DateAdd(DAY,1,GETDATE()),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                           when GETDATE()>=(select CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                           when GETDATE()>=(select CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                      else '' end) as [by]) ");
            strSQL.AppendLine(" declare @vcComDate varchar(10) ");
            strSQL.AppendLine("    set @vcComDate=(select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' 00:00:00.000') and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' 06:00:00.000') then CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)");
            strSQL.AppendLine("                           else  CONVERT(varchar(10),GETDATE(),121) end) as [by])");

            strSQL.AppendLine("select a.vcPartsNo,A.vcDock,A.vcCarType AS vcCarFamilyCode,vcProType as vcPorType,A.vcEDflag,vcKBorderno,b.vcQFflag,");
            strSQL.AppendLine("       '' as [image],A.vcKBSerial,vcTips,vcPlanMonth,A.iNo,A.vcPartFrequence");//品番频度
            strSQL.AppendLine("  FROM ( ");
            //strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");//给看板打印数据left join品番频度
            strSQL.AppendLine(" (SELECT distinct iNo,T1.vcPartsNo,vcDock,vcCarType,vcEDflag,vcKBorderno,vcKBSerial,vcTips,vcPrintflag,vcPrintTime,vcKBType,vcProject00,vcProject01,vcProject02,vcProject03,vcProject04,vcComDate00,vcComDate01,vcComDate02,vcComDate03,vcComDate04,vcBanZhi00,vcBanZhi01,vcBanZhi02,vcBanZhi03,vcBanZhi04,vcAB00,vcAB01,vcAB02,vcAB03,vcAB04,dCreatTime,vcCreater,dUpdateTime,vcUpdater,vcPlanMonth,vcPrintSpec,vcPrintflagED,vcDockED,vcPrintTimeED,vcQuantityPerContainer,iBaiJianFlag,T2.vcPartFrequence FROM tKanbanPrintTbl T1 left join (SELECT vcPartsNo,vcPartFrequence,dTimeFrom,dTimeTo FROM tPartInfoMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE()) T2 on T1.vcPartsNo=T2.vcPartsNo and T2.dTimeFrom<=T1.vcPlanMonth and T2.dTimeTo>=T1.vcPlanMonth) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine(" (select distinct vcProType,vcPartsNo,vcDock,vcMonth,vcCarType,vcQFflag,vcPlant from tPlanPartInfo) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock  and A.vcPlanMonth = B.vcMonth and A.vcCarType = B.vcCarType)");
            strSQL.AppendLine("      where A.vcComDate00=CONVERT(varchar(10),@vcComDate,121) and  A.vcPrintflag is null and A.vcPrintflagED is null");

            if (vcType == "2")
            {
                strSQL.AppendLine(" and  A.vcBanZhi00=@banzhi and vcQFflag='1'");
            }
            if (vcType == "1")
            {
                strSQL.AppendLine(" and  A.vcBanZhi00=@banzhi and vcQFflag<>'1'");
            }
            if (vcPrintPartNo.Length != 0)
            {
                strSQL.AppendLine(" and A.vcPartsNo like '%" + vcPrintPartNo.Replace("-", "") + "%'");
            }
            if (vcKbOrderId.Length != 0)
            {
                strSQL.AppendLine(" and A.vcKBorderno = '" + vcKbOrderId + "'");
            }
            if (vcLianFan.Length != 0)
            {
                strSQL.AppendLine(" and A.vcKBSerial = '" + vcLianFan + "'");
            }
            if (vcPorType != "0")
            {
                strSQL.AppendLine(" and B.vcProType = '" + vcPorType + "'");
            }
            else
            {
                string flag = "";
                if (dtflag.Rows.Count != 0)
                {
                    if (!(dtflag.Rows[0]["Text"].ToString() == "admin" && dtflag.Rows.Count == 1))
                    {
                        flag += "'";
                        for (int i = 0; i < dtflag.Rows.Count; i++)
                        {
                            flag += dtflag.Rows[i]["Text"].ToString().Trim();
                            if (i < dtflag.Rows.Count - 1)
                            {
                                flag += "','";
                            }
                            else
                            {
                                flag += "'";
                            }
                        }
                    }
                    strSQL.AppendLine(" and B.vcProType in( " + flag + ")");
                }
            }
            if (vcPlant != "0")
            {
                strSQL.AppendLine(" and B.vcPlant = '" + vcPlant + "'");
            }
            strSQL.AppendLine("order by vcProType,vcKBorderno,A.vcPartFrequence,A.vcKBSerial");//添加按品番排序（取消），添加品番频度排序
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            dt.Columns.Remove("vcQFflag");
            dt.Columns.Remove("vcPartFrequence");
            return dt;
        }
        #endregion

        #region 检索打印数据 检索的是秦丰非ED的看板信息 检索流程：从补给系统获取已入库白件的品番、受入、订单号、连番，到看板打印表中检索相应的秦丰非ED看板信息
        public DataTable searchPrint(string vcPrintPartNo, string vcKbOrderId, string vcLianFan, string vcPorType, string vcPlant, DataTable dtflag)
        {
            #region 定义数据DataTable
            DataTable dtsend = new DataTable();
            // 定义列
            DataColumn dc_PARTSNO = new DataColumn();
            DataColumn dc_DOCK = new DataColumn();
            DataColumn dc_KANBANORDERNO = new DataColumn();
            DataColumn dc_KANBANSERIAL = new DataColumn();
            // 定义列名
            dc_PARTSNO.ColumnName = "PARTSNO";
            dc_DOCK.ColumnName = "DOCK";
            dc_KANBANORDERNO.ColumnName = "KANBANORDERNO";
            dc_KANBANSERIAL.ColumnName = "KANBANSERIAL";
            // 将定义的列加入到dtTemp中
            dtsend.Columns.Add(dc_PARTSNO);
            dtsend.Columns.Add(dc_DOCK);
            dtsend.Columns.Add(dc_KANBANORDERNO);
            dtsend.Columns.Add(dc_KANBANSERIAL);
            #endregion
            //获取订单号从补给系统中
            DataTable dtorl = GetTable(dtsend);
            string KBorderno = "";
            if (dtorl.Rows.Count != 0)
            {
                KBorderno += " select'";
                for (int i = 0; i < dtorl.Rows.Count; i++)
                {
                    KBorderno += dtorl.Rows[i]["PARTSNO"].ToString().Trim() + dtorl.Rows[i]["DOCK"].ToString().Trim() + dtorl.Rows[i]["KANBANORDERNO"].ToString().Trim() + dtorl.Rows[i]["KANBANSERIAL"].ToString().Trim();
                    if (i < dtorl.Rows.Count - 1)
                    {
                        KBorderno += "'union all select '";
                    }
                    else
                    {
                        KBorderno += "'";
                    }
                }
            }
            else
            {
                return dtorl;
            }
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT A.vcPrintflagED as vcPartsNo,A.vcDockED as vcDock,A.vcCarType AS vcCarFamilyCode,vcPorType,A.vcEDflag,vcKBorderno,");
            strSQL.AppendLine("       B.vcPhotoPath as [image],A.vcKBSerial,vcTips,vcPlanMonth,A.iNo");
            strSQL.AppendLine("  FROM ( ");
            strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");
            strSQL.AppendLine(" left join ");
            strSQL.AppendLine(" (select * from tPartInfoMaster) B");
            strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock)");
            strSQL.AppendLine("      where A.vcPrintflag='1' and A.vcPrintflagED is not null and (a.vcPartsNo+a.vcDock+A.vcKBorderno+a.vcKBSerial)=any(" + KBorderno + ")");
            //strSQL.AppendLine(" A.vcComDate00=CONVERT(varchar(10),GETDATE(),121) and");
            if (vcPrintPartNo.Length != 0)
            {
                strSQL.AppendLine(" and A.vcPrintflagED='" + vcPrintPartNo.Replace("-", "") + "'");
            }
            if (vcKbOrderId.Length != 0)
            {
                strSQL.AppendLine(" and A.vcKBorderno = '" + vcKbOrderId + "'");
            }
            if (vcLianFan.Length != 0)
            {
                strSQL.AppendLine(" and A.vcKBSerial = '" + vcLianFan + "'");
            }
            if (vcPorType != "0")
            {
                strSQL.AppendLine(" and B.vcPorType = '" + vcPorType + "'");
            }
            else
            {
                string flag = "";
                if (dtflag.Rows.Count != 0)
                {
                    flag += "'";
                    for (int i = 0; i < dtflag.Rows.Count; i++)
                    {
                        flag += dtflag.Rows[i]["Text"].ToString().Trim();
                        if (i < dtflag.Rows.Count - 1)
                        {
                            flag += "','";
                        }
                        else
                        {
                            flag += "'";
                        }
                    }
                }
                strSQL.AppendLine(" and B.vcPorType in( " + flag + ")");
            }
            if (vcPlant != "0")
            {
                strSQL.AppendLine(" and B.vcPartPlant = '" + vcPlant + "'");
            }
            strSQL.AppendLine("order by  vcPorType,vcKBorderno,A.vcKBSerial");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        /// <summary>
        ///检索已入库白件的黑件看板品番
        /// </summary>
        /// <param name="vcPrintPartNo"></param>
        /// <param name="vcKbOrderId"></param>
        /// <param name="vcLianFan"></param>
        /// <param name="vcPorType"></param>
        /// <param name="vcPlant"></param>
        /// <returns></returns>
        public string searchMasterED(string PartNo, string vcDock)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcPartsNo from tPartInfoMaster where  substring(vcPartsNo,0,11)='" + PartNo.Substring(0, 10) + "' and substring(vcPartsNo,11,2)<>'ED'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0]["vcPartsNo"].ToString();
            }
            else
            {
                return PartNo;
            }
        }

        public bool InsertPrint(DataTable dt)
        {
            DataTable dtBaijian = reDataMaster(dt);
            bool orlbool = InsertTable(dtBaijian);
            return orlbool;
        }

        public DataTable reDataMaster(DataTable dtol)
        {
            string KBorderno = "";
            KBorderno += " select'";
            for (int i = 0; i < dtol.Rows.Count; i++)
            {
                string up = dtKBSerialUP1(dtol.Rows[i]["vcPartsNo"].ToString().Trim(), dtol.Rows[i]["vcDock"].ToString().Trim(), dtol.Rows[i]["vcorderno"].ToString().Trim(), dtol.Rows[i]["vcKBSerial"].ToString().Trim());
                string vcKBSerial = dtol.Rows[i]["vcKBSerial"].ToString().Trim();
                if (up != "")
                {
                    vcKBSerial = up;
                }
                KBorderno += dtol.Rows[i]["vcPartsNo"].ToString().Trim() + dtol.Rows[i]["vcDock"].ToString().Trim() + dtol.Rows[i]["vcorderno"].ToString().Trim() + vcKBSerial;
                if (i < dtol.Rows.Count - 1)
                {
                    KBorderno += "'union all select '";
                }
                else
                {
                    KBorderno += "'";
                }
            }
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcPartsNo,vcDock,vcKBorderno,vcKBSerial from tKanbanPrintTbl  where (vcPrintflagED+vcDockED+vcKBorderno+vcKBSerial)=any(" + KBorderno + ")");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public string dtKBSerialUP1(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerialBefore)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcKBSerialBefore]");
            strSQL.AppendLine("  FROM [KBSerial_history]");
            strSQL.AppendLine(" WHERE vcPartsNo='" + vcPartsNo + "'");
            strSQL.AppendLine("       AND vcDock='" + vcDock + "'");
            strSQL.AppendLine("       AND vcKBorderno='" + vcKBorderno + "'");
            strSQL.AppendLine("       AND vcKBSerial='" + vcKBSerialBefore + "'");
            if (excute.ExcuteSqlWithSelectToDT(strSQL.ToString()).Rows.Count != 0)
            {
                return excute.ExcuteSqlWithSelectToDT(strSQL.ToString()).Rows[0]["vcKBSerialBefore"].ToString();
            }
            else
            {
                return "";
            }
        }

        public string KanBIfPrintDay()
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanend as varchar(8)) from Mbaiye)) then '白值'");
            strSQL.AppendLine("                          when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '夜值'");
            strSQL.AppendLine("                     else '' end) as [by]");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt.Rows[0]["by"].ToString();
        }

        public DataSet PrintExcel(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select '' as no,(case when SUBSTRING(a.vcpartsNo,11,2)='00' then SUBSTRING(a.vcpartsNo,1,5)+'-'+SUBSTRING(a.vcpartsNo,6,5) else SUBSTRING(a.vcpartsNo,1,5)+'-'+SUBSTRING(a.vcpartsNo,6,5)+'-'+ SUBSTRING(a.vcpartsNo,11,2) end)  as vcpartsNo,a.vcCarFamlyCode as vcCarFamlyCode,a.vcPartsNameCHN as vcPartsNameCHN,");
            strSQL.AppendLine("       a.vcPCB01 as vcPCB01,a.meishu as meishu,a.minal+'-'+a.maxal as vcKBSerial from (");
            strSQL.AppendLine("select vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01,");
            strSQL.AppendLine("       COUNT(iQuantityPerContainer) as meishu,");
            strSQL.AppendLine("       MAX(vcKBSerial) as maxal,");
            strSQL.AppendLine("       MIN(vcKBSerial) as minal from [testprinterExcel] where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "'");
            strSQL.AppendLine("       group by vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01");
            strSQL.AppendLine("       )a order by vcKBSerial");

            strSQL.AppendLine("select B.vcPCB01,B.meishu AS meishu, B.minal+'-'+B.maxal AS vcKBSerial,ROW_NUMBER() over(order by  minal) as aaa  FROM ");
            strSQL.AppendLine("       (select vcPCB01, ");
            strSQL.AppendLine("              COUNT(iQuantityPerContainer) as meishu, ");
            strSQL.AppendLine("              MAX(vcKBSerial) as maxal, ");
            strSQL.AppendLine("              MIN(vcKBSerial) as minal ");
            strSQL.AppendLine("         from [testprinterExcel]  where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "'");
            strSQL.AppendLine("              group by vcPCB01 )B");
            strSQL.AppendLine("       UNION all");
            strSQL.AppendLine("       (");
            strSQL.AppendLine("select '合计' as vcPCB01,B.meishu AS meishu, B.minal+'-'+B.maxal AS vcKBSerial,''  FROM ");
            strSQL.AppendLine("       (select  ");
            strSQL.AppendLine("              COUNT(iQuantityPerContainer) as meishu, ");
            strSQL.AppendLine("              MAX(vcKBSerial) as maxal, ");
            strSQL.AppendLine("              MIN(vcKBSerial) as minal   ");
            strSQL.AppendLine("         from [testprinterExcel]   where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "'");
            strSQL.AppendLine("              )B  )");

            return excute.ExcuteSqlWithSelectToDS(strSQL.ToString());
        }

        public DataTable searchPrintKANB(string vcKBorderno, string vcKBSerial)//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select B.vcSupplierCode,B.vcSupplierPlant,B.vcCpdCompany,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute,");
            strSQL.AppendLine("       B.iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2 from");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE A.[vcKBorderno]='" + vcKBorderno + "' and A.[vcKBSerial]='" + vcKBSerial + "'");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public DataTable searchTBCreate()//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select top(1)* from testprinterCR");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        #region 插入数据到临时表tPartInfoMaster_Temp
        public bool insertTableCR(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableCR");//这里也有存储过程
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcSupplierCode", SqlDbType.VarChar, 5, "vcSupplierCode");
                    cmdln.Parameters.Add("@vcCpdCompany", SqlDbType.VarChar, 5, "vcCpdCompany");
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamilyCode");
                    cmdln.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12, "vcPartsNo");
                    cmdln.Parameters.Add("@vcPartsNameEN", SqlDbType.VarChar, 500, "vcPartsNameEN");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@vcLogisticRoute", SqlDbType.VarChar, 500, "vcLogisticRoute");
                    cmdln.Parameters.Add("@iQuantityPerContainer", SqlDbType.VarChar, 8, "iQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject00", SqlDbType.VarChar, 20, "vcProject00");
                    cmdln.Parameters.Add("@vcComDate00", SqlDbType.VarChar, 10, "vcComDate00");
                    cmdln.Parameters.Add("@vcBanZhi00", SqlDbType.VarChar, 1, "vcBanZhi00");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 1, "vcBanZhi01");
                    cmdln.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                    cmdln.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                    cmdln.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 1, "vcBanZhi02");
                    cmdln.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                    cmdln.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                    cmdln.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 1, "vcBanZhi03");
                    cmdln.Parameters.Add("@vcRemark1", SqlDbType.VarChar, 500, "vcRemark1");
                    cmdln.Parameters.Add("@vcRemark2", SqlDbType.VarChar, 500, "vcRemark2");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcPhotoPath", SqlDbType.Image, 999999, "vcPhotoPath");
                    cmdln.Parameters.Add("@vcQRCodeImge", SqlDbType.Image, 999999, "vcQRCodeImge");
                    cmdln.Parameters.Add("@vcDock", SqlDbType.VarChar, 4, "vcDock");
                    cmdln.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 14, "vcKBorderno");
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        public DataTable searchPrintCR()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcSupplierCode],[vcCpdCompany],[vcCarFamilyCode],[vcPartsNo], ");
            strSQL.AppendLine("       [vcPartsNameEN],[vcPartsNameCHN],[vcLogisticRoute],[iQuantityPerContainer],");
            strSQL.AppendLine("       [vcProject00],[vcComDate00],[vcBanZhi00],[vcProject01],[vcComDate01],");
            strSQL.AppendLine("       [vcBanZhi01],[vcProject02],[vcComDate02],[vcBanZhi02],[vcProject03],");
            strSQL.AppendLine("       [vcComDate03],[vcBanZhi03],[vcRemark1],[vcRemark2],[vcKBSerial],");
            strSQL.AppendLine("       [vcPhotoPath],[vcQRCodeImge]");
            strSQL.AppendLine("  FROM [testprinterCR]");
            strSQL.AppendLine("");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public DataTable searchPrintCR(string type)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcPartsNo],[vcDock],[vcCarType],[vcEDflag],[vcKBorderno],[vcKBSerial],[vcTips],[vcPrintflag]  FROM [tKanbanPrintTbl]");
            strSQL.AppendLine("");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public bool UpdatePrintKANB(DataTable dt, string type)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                string vcDateTime = System.DateTime.Now.ToString();
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcKBorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        if (type == "1")
                        {
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "'";
                        }
                        else
                            if (type == "2")
                        {
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',vcKBType='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "'";
                        }
                        if (strSql != "")
                        {
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = strSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        #region 检查秦丰非ED 的收容数
        public string resQuantityPerContainer(string vcpartno, string vcdock)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [iQuantityPerContainer]  FROM [tPartInfoMaster] WHERE [vcPartsNo]='" + vcpartno + "' AND [vcDock]='" + vcdock + "'");
            strSQL.AppendLine("");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0]["iQuantityPerContainer"].ToString();
            }
            else
            {
                return "0";
            }
        }
        public string resQuantityPerContainer(string vcpartno, string vcdock, string vcPlanMonth)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [iQuantityPerContainer]  FROM [tPartInfoMaster] WHERE [vcPartsNo]='" + vcpartno + "' AND [vcDock]='" + vcdock + "'  ");
            strSQL.AppendLine("  and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcPlanMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcPlanMonth.Replace("-", "") + "')");
            strSQL.AppendLine("");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0]["iQuantityPerContainer"].ToString();
            }
            else
            {
                return "0";
            }
        }
        #endregion
        public string resKBSerialend(string vcKBorderno)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select MAX(vckbserial) as vckbserial from [KBSerial_history] where [vcKBorderno]='" + vcKBorderno + "'");
            strSQL.AppendLine("");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt.Rows[0]["vckbserial"].ToString() == "" ? "0000" : dt.Rows[0]["vckbserial"].ToString();
        }
        public DataTable ceshi()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select top(20)vcPartsNo,vcCarType,vcProject00,vcProject04,'10' as meishu,vcKBSerial from tKanbanPrintTbl");
            strSQL.AppendLine("");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt;
        }
        public void DeleteprinterCREX(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            //清空临时表tPartInfoMaster_Temp
            SqlCommand cd = new SqlCommand();
            cd.Connection = new SqlConnection();
            cd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            if (cd.Connection.State == ConnectionState.Closed)
            {
                cd.Connection.Open();
            }
            cd.CommandText = "delete from testprinterCR where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterCRMAIN] where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterExcel] where vcPorType='" + vcPorType + "' and vcKBorderno='" + vcorderno + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "' ";
            cd.ExecuteNonQuery();
            if (cd.Connection.State == ConnectionState.Open)
            {
                cd.Connection.Close();
            }
        }

        public void InsertInto(string vcorderno, string vcPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string strSqlIn = "";
                strSqlIn = "INSERT INTO [testprinterExcel1] select vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01,vcPCB02,vcPCB03,iQuantityPerContainer,vcPorType,vcKBorderno,vcKBSerial,vcComDate00,vcBanZhi00 from testprinterExcel where vcKBorderno='" + vcorderno + "' and vcPorType ='" + vcPorType + "'";
                SqlCommand _Command = new SqlCommand()
                {
                    Connection = connln,
                    CommandText = strSqlIn,
                    CommandType = CommandType.Text,
                    Transaction = trans,
                    CommandTimeout = 0
                };
                _Command.ExecuteNonQuery();
                trans.Commit();
                return;
            }
        }

        public void DeleteprinterCREX1(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            //清空临时表tPartInfoMaster_Temp
            SqlCommand cd = new SqlCommand();
            cd.Connection = new SqlConnection();
            cd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            if (cd.Connection.State == ConnectionState.Closed)
            {
                cd.Connection.Open();
            }
            //
            cd.CommandText = "delete from testprinterCR where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterCRMAIN] where vcPorType='" + vcPorType + "' and vcorderno is null and vcComDate01 is null and vcBanZhi01 is null  ";
            cd.ExecuteNonQuery();
            if (cd.Connection.State == ConnectionState.Open)
            {
                cd.Connection.Close();
            }
        }

        //特殊打印删除
        public void DeleteprinterCREX2(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            //清空临时表tPartInfoMaster_Temp
            SqlCommand cd = new SqlCommand();
            cd.Connection = new SqlConnection();
            cd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            if (cd.Connection.State == ConnectionState.Closed)
            {
                cd.Connection.Open();
            }
            //
            cd.CommandText = "delete from testprinterCRExcep where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterCRMAIN] where vcPorType='" + vcPorType + "' and vcorderno is null and vcComDate01 is null and vcBanZhi01 is null ";
            cd.ExecuteNonQuery();
            if (cd.Connection.State == ConnectionState.Open)
            {
                cd.Connection.Close();
            }
        }

        public void InsertDate(string vcorderno, string vcPorType, string printIme, string printDay, string vcComDate01, string vcBanZhi01)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string strSqlIn = "";
                strSqlIn = "INSERT INTO [tKanBanQrTbl]([vcOrderNo],[vcGC],[vcPlanPrintDate],[vcPlanPrintBZ],[vcPlanProcDate],[vcPlanProcBZ],[vcPrintDate])";
                strSqlIn += "     VALUES";
                strSqlIn += "           ('" + vcorderno + "'";
                strSqlIn += "            ,'" + vcPorType + "'";
                strSqlIn += "            ,'" + printIme + "'";
                strSqlIn += "            ,'" + printDay + "'";
                strSqlIn += "            ,'" + vcComDate01 + "'";
                strSqlIn += "            ,'" + vcBanZhi01 + "'";
                strSqlIn += "            ,getdate())";
                SqlCommand _Command = new SqlCommand()
                {
                    Connection = connln,
                    CommandText = strSqlIn,
                    CommandType = CommandType.Text,
                    Transaction = trans,
                    CommandTimeout = 0
                };
                _Command.ExecuteNonQuery();
                trans.Commit();
                return;
            }
        }

        public DataTable check(string vcorderno, string vcPorType)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcOrderNo]  FROM [tKanBanQrTbl] WHERE [vcOrderNo] = '" + vcorderno + "'and [vcGC] = '" + vcPorType + "'");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt;
        }

        #region 2021-2-8 增加
        public DataTable GetTable(DataTable dt)
        {
            try
            {
                string strplsql = "select t1.vcPart_id as PARTSNO, t1.vcSR as DOCK, t1.vcKBOrderNo as KANBANORDERNO, t1.vcKBLFNo as KANBANSERIAL from ";
                strplsql += " (select vcPart_id,vcSR,vcKBOrderNo,vcKBLFNo from TOperateSJ where vcZYType='S0' and substring(vcPart_id,-2,2)='ED' and vcKBOrderNo is not null) t1 ";
                strplsql += " left join ";
                strplsql += " (select PARTSNO,DOCK,KBORDERNO,KBSERIAL from TNZ_M_INV)t2 ";
                strplsql += " on t1.vcPart_id=t2.PARTSNO and t1.vcSR=t2.DOCK and t1.vcKBOrderNo=t2.KBORDERNO and t1.vcKBLFNo=t2.KBSERIAL ";
                strplsql += " where t2.PARTSNO is null ";

                SqlDataReader or = ExecuteReader(CommandType.Text, strplsql);
                while (or.Read())
                {
                    DataRow drTemp = dt.NewRow();
                    if (!or.GetSqlString(0).IsNull)
                    {
                        drTemp["PARTSNO"] = or.GetSqlString(0);
                    }
                    if (!or.GetSqlString(1).IsNull)
                    {
                        drTemp["DOCK"] = or.GetSqlString(1);
                    }
                    if (!or.GetSqlString(2).IsNull)
                    {
                        drTemp["KANBANORDERNO"] = or.GetSqlString(2);
                    }
                    if (!or.GetSqlString(3).IsNull)
                    {
                        drTemp["KANBANSERIAL"] = or.GetSqlString(3);
                    }
                    dt.Rows.Add(drTemp);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText)
        {
            //Create the command and connection
            //创建一个command和connection对象
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());
            try
            {
                //Prepare the command to execute
                //准备执行这个command
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                //Execute the query, stating that the connection should close when the resulting datareader has been read
                //执行查询,当read到数据结果后关闭连接
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                //当发生错误时或者我们希望时关闭连接
                conn.Close();
                throw;
            }
        }
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText)
        {
            //Open the connection if required
            //如果必须则打开一个连接
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //Set up the command
            //建立command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //Bind it to the transaction if it exists
            //绑定一个事务如果它存在
            if (trans != null)
                cmd.Transaction = trans;
        }
        public bool InsertTable(DataTable dt)
        {
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcKBorderno = dt.Rows[i]["vcKBorderno"].ToString();
                    string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                    string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Substring(0, 10) + "ED"; ;
                    string vcDock = dt.Rows[i]["vcDock"].ToString();
                    string StrOrl = "insert into TNZ_M_INV(partsno,dock,kborderno,kbserial,printflag) values ('" + vcPartsNo + "','" + vcDock + "','" + vcKBorderno + "','" + vcKBSerial + "','1')";
                    ExecuteNonQuery(trans, CommandType.Text, StrOrl, null);
                }
                trans.Commit();
                return true;
            }
            catch (Exception e)
            {
                trans.Rollback();
                return false; ;
            }
            finally
            {
                conn.Close();
            }
        }
        private int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;
            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (SqlParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }
        public DataTable returnED(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["vcEDflag"].ToString() == "S")
                {
                    dt.Rows[i]["vcEDflag"] = "通常";
                }
                if (dt.Rows[i]["vcEDflag"].ToString() == "E")
                {
                    dt.Rows[i]["vcEDflag"] = "紧急";
                }
            }
            return dt;
        }

        #region 删除第一行空数据
        public DataTable deletenull(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["vcPartsNo"].ToString() == "")
                {
                    dt.Rows[i].Delete();
                }
            }
            return dt;
        }
        #endregion

        #region 建立DataTable 为打印看板确认单提供DataTable
        public DataTable CreatDataTable()
        {
            #region 定义数据DataTable
            DataTable dt = new DataTable();
            // 定义列
            DataColumn dc_vcpartsNo = new DataColumn();
            DataColumn dc_vcCarFamlyCode = new DataColumn();
            DataColumn dc_vcPartsNameCHN = new DataColumn();
            DataColumn dc_vcPCB01 = new DataColumn();
            DataColumn dc_vcPCB02 = new DataColumn();
            DataColumn dc_vcPCB03 = new DataColumn();
            DataColumn dc_iQuantityPerContainer = new DataColumn();
            DataColumn dc_vcPorType = new DataColumn();
            DataColumn dc_vcKBorderno = new DataColumn();
            DataColumn dc_vcKBSerial = new DataColumn();

            // 定义列名
            dc_vcpartsNo.ColumnName = "vcpartsNo";
            dc_vcCarFamlyCode.ColumnName = "vcCarFamlyCode";
            dc_vcPartsNameCHN.ColumnName = "vcPartsNameCHN";
            dc_vcPCB01.ColumnName = "vcPCB01";
            dc_vcPCB02.ColumnName = "vcPCB02";
            dc_vcPCB03.ColumnName = "vcPCB03";
            dc_iQuantityPerContainer.ColumnName = "iQuantityPerContainer";
            dc_vcPorType.ColumnName = "vcPorType";
            dc_vcKBorderno.ColumnName = "vcKBorderno";
            dc_vcKBSerial.ColumnName = "vcKBSerial";

            // 将定义的列加入到dtTemp中
            dt.Columns.Add(dc_vcpartsNo);
            dt.Columns.Add(dc_vcCarFamlyCode);
            dt.Columns.Add(dc_vcPartsNameCHN);
            dt.Columns.Add(dc_vcPCB01);
            dt.Columns.Add(dc_vcPCB02);
            dt.Columns.Add(dc_vcPCB03);
            dt.Columns.Add(dc_iQuantityPerContainer);
            dt.Columns.Add(dc_vcPorType);
            dt.Columns.Add(dc_vcKBorderno);
            dt.Columns.Add(dc_vcKBSerial);
            #endregion
            return dt;
        }
        #endregion 
        #region 建立DataTable 为打印已入库白件的连番存储提供DataTable
        public DataTable CreatDataTableHis()
        {
            #region 定义数据DataTable
            DataTable dt = new DataTable();
            // 定义列
            DataColumn dc_vcPartsNo = new DataColumn();
            DataColumn dc_vcDock = new DataColumn();
            DataColumn dc_vcKBorderno = new DataColumn();
            DataColumn dc_vcKBSerial = new DataColumn();
            DataColumn dc_vcKBSerialBefore = new DataColumn();

            // 定义列名
            dc_vcPartsNo.ColumnName = "vcPartsNo";
            dc_vcDock.ColumnName = "vcDock";
            dc_vcKBorderno.ColumnName = "vcKBorderno";
            dc_vcKBSerial.ColumnName = "vcKBSerial";
            dc_vcKBSerialBefore.ColumnName = "vcKBSerialBefore";

            // 将定义的列加入到dtTemp中
            dt.Columns.Add(dc_vcPartsNo);
            dt.Columns.Add(dc_vcDock);
            dt.Columns.Add(dc_vcKBorderno);
            dt.Columns.Add(dc_vcKBSerial);
            dt.Columns.Add(dc_vcKBSerialBefore);
            #endregion
            return dt;
        }
        #endregion
        #region DataTable分组
        public DataTable QueryGroup(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.String"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            var query = from t in dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("vcorderno"), t2 = t.Field<string>("vcPorType"), t3 = t.Field<string>("vcComDate01"), t4 = t.Field<string>("vcBanZhi01") } into m
                        select new
                        {
                            vcorderno = m.Key.t1,
                            vcPorType = m.Key.t2,
                            vcComDate01 = m.Key.t3,
                            vcBanZhi01 = m.Key.t4
                        };
            foreach (var item in query.ToList())
            {
                DataRow dr = dtPorType.NewRow();
                dr["vcorderno"] = item.vcorderno;
                dr["vcPorType"] = item.vcPorType;
                dr["vcComDate01"] = item.vcComDate01; ;
                dr["vcBanZhi01"] = item.vcBanZhi01;
                dtPorType.Rows.Add(dr);
            }
            return dtPorType;
        }
        #endregion
        #endregion

        #endregion
    }

    public class PrinterCR
    {
        private MultiExcute excute = new MultiExcute();

        #region 将照片转换为二进制数组
        /// <param name="path">文件路径</param>
        /// <returns>二进制流</returns>
        public byte[] PhotoToArray(string path, string path2)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
            catch
            {
                FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
        }
        #endregion

        #region 生成QRCODE二维码
        /// <param name="msg">二维码序列号</param>
        /// <returns>二维码二进制流</returns>
        public byte[] GenGenerateQRCode(string msg, String ls_savePath)
        {
            //QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            //qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //qrCodeEncoder.QRCodeVersion = 11;
            //qrCodeEncoder.QRCodeScale = 2;
            //String data = msg;

            //String ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";

            //qrCodeEncoder.Encode(data).Save(ls_savePath);
            //FileStream stream = new FileStream(ls_savePath, FileMode.Open, FileAccess.Read);
            //byte[] bufferPhoto = new byte[stream.Length];
            //stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
            //stream.Flush();
            //stream.Close();
            ////删除该照片
            //if (File.Exists(ls_savePath))
            //{
            //    File.Delete(ls_savePath);
            //}
            //return bufferPhoto;
            return null;
        }
        #endregion

        #region 二维码数据整理
        public string reCode(string vcSupplierCode, string vcSupplierPlant, string vcDock, string vcPartsNo, string iQuantityPerContainer, string vcKBSerial, string vcEDflag, string vcKBorderno)
        {
            string strcode = "";
            strcode += " ";
            strcode += vcSupplierCode != "" ? vcSupplierCode : "    ";
            strcode += vcSupplierPlant != "" ? vcSupplierPlant : " ";
            strcode += "        ";
            strcode += vcDock != "" ? vcDock : "  ";
            strcode += vcPartsNo != "" ? vcPartsNo : "            ";
            strcode += iQuantityPerContainer != "" ? iQuantityPerContainer.PadLeft(5, '0').ToString() : "     ";
            strcode += "                        ";
            strcode += vcKBSerial != "" ? vcKBSerial : "    ";
            strcode += "                                                           ";
            strcode += "NZ";
            strcode += "                                                        ";
            strcode += vcEDflag != "" ? vcEDflag : " ";
            strcode += "                                        ";
            if (vcKBorderno.Length < 12)
            {
                int kblen = vcKBorderno.Length;
                for (int i = 0; i < 12 - kblen; i++)
                {
                    vcKBorderno = vcKBorderno + " ";
                }
            }
            strcode += vcKBorderno;

            strcode += "  ";
            int qq = strcode.Length;
            return strcode;
        }
        #endregion

        /// <summary>
        /// 打印水晶报表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="printerName"></param>
        /// <param name="reportName"></param>
        /// <param name="left"></param>
        /// <param name="up"></param>
        public void printAllCrReport(DataTable dt, string printerName, string reportName, string left, string up, string SerPath)
        {

            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.CloseTimeout = TimeSpan.MaxValue;
            //binding.OpenTimeout = TimeSpan.MaxValue;
            //binding.ReceiveTimeout = TimeSpan.MaxValue;
            //binding.SendTimeout = TimeSpan.MaxValue;
            //EndpointAddress address = new EndpointAddress("http://localhost:8089/PrintTable.asmx");
            //PrintTableSoapClient client = new PrintTableSoapClient(binding, address);
            //Task<PrinterResponse> responseTask = client.PrinterAsync("test20210205", "\\\\172.23.129.181\\刷卡打印机黑白", "C:\\inetpub\\SPPSPrint\\Test.rpt", "172.23.140.169", "SPPSdb", "sa", "Sa123");
            //PrinterResponse response = responseTask.Result;
            //// 获取HelloWorld方法的返回值
            //return response.Body.PrinterResult;



            //ClassINI ini = new ClassINI(System.AppDomain.CurrentDomain.BaseDirectory + "\\QMClass.ini");
            //string ServerName = "";
            //string DatabaseName = "";
            //string UserID = "";
            //string Password = "";
            //if (ini.IniReadValue("classini", "ServerName") != "")
            //{
            //    ServerName = ini.IniReadValue("classini", "ServerName");
            //}
            //if (ini.IniReadValue("classini", "DatabaseName") != "")
            //{
            //    DatabaseName = ini.IniReadValue("classini", "DatabaseName");
            //}
            //if (ini.IniReadValue("classini", "UserID") != "")
            //{
            //    UserID = ini.IniReadValue("classini", "UserID");
            //}
            //if (ini.IniReadValue("classini", "Password") != "")
            //{
            //    Password = ini.IniReadValue("classini", "Password");
            //}
            //ReportDocument rd = new ReportDocument();
            //try
            //{
            //    rd.Load(reportName);
            //    TableLogOnInfo logOnInfo = new TableLogOnInfo();
            //    logOnInfo.ConnectionInfo.ServerName = ServerName;
            //    logOnInfo.ConnectionInfo.DatabaseName = DatabaseName;
            //    logOnInfo.ConnectionInfo.UserID = UserID;
            //    logOnInfo.ConnectionInfo.Password = Password;
            //    rd.Database.Tables[0].ApplyLogOnInfo(logOnInfo);
            //    rd.PrintOptions.PrinterName = printerName;
            //    //PageMargins margins = rd.PrintOptions.PageMargins;
            //    //margins.leftMargin = 233;
            //    //margins.bottomMargin = 232;
            //    //margins.rightMargin = 232;
            //    //margins.topMargin = 233;
            //    //rd.PrintOptions.ApplyPageMargins(margins);
            //    rd.SetDataSource(dt);
            //    //FileInfo file = new FileInfo(SerPath);
            //    //file.Delete();
            //    //rd.ExportToDisk(ExportFormatType.WordForWindows, SerPath);
            //    rd.PrintToPrinter(1, true, 0, 0);
            //    //rd.Close();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    rd.Close();
            //    rd.Dispose();
            //    if (dt.Rows.Count < 10)
            //    {
            //        System.Threading.Thread.Sleep(5000);
            //    }
            //}
        }

        private void KillExcelProcess(string p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打印水晶报表
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        public bool printCr(string reportPath, string vcProType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00, string vcUser, string strPrinterName)
        {
            try
            {
                DataTable dt = searchPrintCRMain(vcProType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);//检索打印数据主表
                //QMCommon.OfficeCommon.QMCrPrinter crystalReportCommon = new QMCrPrinter();
                //获取打印机名
                if (dt.Rows.Count > 0)
                {
                    //调用webservice打印
                    printAllCrReport(dt, strPrinterName, reportPath, "0", "0", "");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检索打印数据主表
        /// </summary>
        /// <returns></returns>
        private DataTable searchPrintCRMain(string vcProType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT vcNo1,vcNo2,vcNo3 FROM [testprinterCRMAIN]   where vcPorType='" + vcProType + "' ");

            if (vcorderno != "")
            {
                strSQL.AppendLine("  and vcorderno='" + vcorderno + "'");
            }
            if (vcComDate01 != "")
            {
                strSQL.AppendLine("  and vcComDate01='" + vcComDate01 + "'");
            }
            if (vcBanZhi01 != "")
            {
                strSQL.AppendLine("  and vcBanZhi01='" + vcBanZhi01 + "'");
            }
            if (vcComDate00 != "")
            {
                strSQL.AppendLine(" and vcComDate00='" + vcComDate00 + "'");
            }
            if (vcBanZhi00 != "")
            {
                strSQL.AppendLine("  and vcBanZhi00='" + vcBanZhi00 + "'");
            }
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public DataTable searchPorTypeExcep()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select distinct vcPorType  from testprinterCRExcep");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        #region 看板打印信息160 目前没有引用该方法的实例
        /// <summary>
        /// 看板打印信息160 目前没有引用该方法的实例
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable searchPrintKANB(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock, string vctype)
        {
            DataTable dt = new DataTable();
            vcPartsNo = typesertch(vctype, vcPartsNo, vcDock);
            if (vctype == "3")
            {
                vcDock = vcPartsNo.Substring(0, 2).ToString();
                vcPartsNo = vcPartsNo.Substring(2, 12).ToString();
            }
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select B.vcSupplierCode,B.vcSupplierPlant,B.vcCpdCompany,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute,");
            strSQL.AppendLine("       A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,b.vcPorType from");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE A.[vcKBorderno]='" + vcKBorderno + "' and A.[vcKBSerial]='" + vcKBSerial + "' and A.vcPartsNo='" + vcPartsNo + "' and  A.vcDock='" + vcDock + "'");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #region 获取FS0160看板打印页面的看板打印的打印信息，vctype是标示符|3 是秦丰非ED |<>3 是秦丰ED和非秦丰，在秦丰非ED的打印中需要获取相应的ED的供应商工区
        public DataTable searchPrintKANBALL(DataTable dt, string vctype, int i)
        {
            StringBuilder strSQL = new StringBuilder();
            DataTable dtreturn = new DataTable();
            if (vctype != "3")
            {
                string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "").Trim();
                string vcDock = dt.Rows[i]["vcDock"].ToString().Trim();
                string vcplantMonth = dt.Rows[i]["vcPlanMonth"].ToString().Trim();
                string iNo = dt.Rows[i]["iNo"].ToString().Trim();
                strSQL.AppendLine("SELECT (case when A.vcPrintflagED is not null then A.vcPrintflagED else A.vcPartsNo END) AS vcPartsNo, ");
                strSQL.AppendLine("        B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,(case when A.vcDockED is not null then A.vcDockED else A.vcDock END) AS vcDock,");
                strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
                strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer,");
                strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04,");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
                strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");
                strSQL.AppendLine(" left join ");
                strSQL.AppendLine(" (select * from tPartInfoMaster) B");
                strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
                strSQL.AppendLine(" where A.iNo='" + iNo + "'");
                //strSQL.AppendLine(" where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPartsNo+A.vcDock)=('"+dt.Rows[i]["vcKBorderno"].ToString().Trim() + dt.Rows[i]["vcKBSerial"].ToString().Trim() + vcPartsNo.Trim() + vcDock.Trim()+"')");
                strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')");
                dtreturn = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            }
            if (vctype == "3")
            {
                string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "").Trim();
                string vcDock = dt.Rows[i]["vcDock"].ToString().Trim();
                string vcplantMonth = dt.Rows[i]["vcPlanMonth"].ToString().Trim();
                string iNo = dt.Rows[i]["iNo"].ToString().Trim();
                string vcCarType = dt.Rows[i]["vcCarFamilyCode"].ToString().Trim();
                //获取白件相应黑件在Master中的车型信息
                //string vcCarType = BreakCarType(vcPartsNo, vcDock, vcplantMonth);
                strSQL.AppendLine("SELECT  A.vcPrintflagED AS vcPartsNo,A.vcDockED AS vcDock, ");
                strSQL.AppendLine("        B.vcSupplierCode,'vcSupplierPlant' as vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno, ");
                strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute,   ");
                strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer, ");
                strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04, ");//20180921添加AB值信息 - 李兴旺
                strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
                strSQL.AppendLine("  (select * from tKanbanPrintTbl) A ");
                strSQL.AppendLine(" left join ");
                strSQL.AppendLine(" (select * from tPartInfoMaster) B");
                strSQL.AppendLine("  on A.vcPrintflagED=B.vcPartsNo");
                strSQL.AppendLine(" where A.iNo='" + iNo + "'");
                //strSQL.AppendLine("  where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPrintflagED)=('" + dt.Rows[i]["vcKBorderno"].ToString().Trim() + dt.Rows[i]["vcKBSerial"].ToString().Trim() + vcPartsNo.Trim() + "')");
                strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')  and B.vcCarFamilyCode='" + vcCarType + "'");
                dtreturn = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
                if (dtreturn.Rows.Count != 0)
                {
                    if (dtreturn.Rows[0]["vcSupplierPlant"].ToString() == "vcSupplierPlant")
                    {
                        string SupplPlant = BreakSupplPlant(vcPartsNo, vcDock, vcplantMonth);
                        dtreturn.Rows[0]["vcSupplierPlant"] = SupplPlant;
                    }
                }
            }
            return dtreturn;
        }
        #endregion

        #region 获取FS0160看板打印页面的看板打印的打印信息 获取对应黑件的白件的Master的车型
        private string BreakCarType(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartsNo,vcDock from tKanbanPrintTbl  where vcPrintflagED='" + vcPartsNo + "' and vcDockED='" + vcDock + "' and vcPlanMonth='" + vcPlanMonth + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            StringBuilder strSQL1 = new StringBuilder();
            strSQL1.AppendLine("select vcCarFamilyCode from tPartInfoMaster where vcPartsNo='" + dt.Rows[0]["vcPartsNo"] + "' and vcDock='" + dt.Rows[0]["vcDock"] + "' and ( CONVERT(varchar(7),dTimeFrom,120)<='" + vcPlanMonth + "' and CONVERT(varchar(7),dTimeTo,120)>='" + vcPlanMonth + "')");
            DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSQL1.ToString());
            return dt1.Rows[0]["vcCarFamilyCode"].ToString();
        }
        #endregion

        #region 获取FS0160看板打印页面的看板打印的打印信息 获取供应商工区
        private string BreakSupplPlant(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartsNo,vcDock from tKanbanPrintTbl  where vcPrintflagED='" + vcPartsNo + "' and vcDockED='" + vcDock + "' and vcPlanMonth='" + vcPlanMonth + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            StringBuilder strSQL1 = new StringBuilder();
            strSQL1.AppendLine("select vcSupplierPlant from tPartInfoMaster where vcPartsNo='" + dt.Rows[0]["vcPartsNo"] + "' and vcDock='" + dt.Rows[0]["vcDock"] + "' and ( CONVERT(varchar(7),dTimeFrom,120)<='" + vcPlanMonth + "' and CONVERT(varchar(7),dTimeTo,120)>='" + vcPlanMonth + "')");
            DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSQL1.ToString());
            return dt1.Rows[0]["vcSupplierPlant"].ToString();
        }
        #endregion

        #region 仅被一个不被引用的方法使用
        public string typesertch(string vctype, string vcPartsNo, string vcDock)
        {
            string vcPartsNoED = vcPartsNo;
            switch (vctype)
            {
                case "1":
                    vcPartsNoED = vcPartsNo;
                    break;
                case "2":
                    vcPartsNoED = vcPartsNo;
                    break;
                case "3":
                    vcPartsNoED = searchMasterED(vcPartsNo, vcDock);
                    break;
                default:
                    vcPartsNoED = vcPartsNo;
                    break;
            }
            return vcPartsNoED;
        }
        #endregion

        public string searchMasterED(string PartNo, string vcDock)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcPartsNo,vcDock from tPartInfoMaster where substring(vcPartsNo,0,11)='" + PartNo.Substring(0, 10) + "' and substring(vcPartsNo,11,2)='ED'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return (dt.Rows[0]["vcDock"].ToString() + dt.Rows[0]["vcPartsNo"].ToString());
            }
            else
            {
                return vcDock + PartNo;
            }
        }

        /// <summary>
        /// DataTable内部排序
        /// </summary>
        /// <param name="dt1"></param>
        /// <returns></returns>
        public DataTable orderDataTable(DataTable dt1)
        {
            if (dt1.Rows.Count != 0)
            {
                DataRow[] rows = dt1.Select("", "vcPorType desc");
                DataTable t = dt1.Clone();
                t.Clear();
                foreach (DataRow row in rows)
                {
                    t.ImportRow(row);
                }
                dt1 = t;
            }
            return dt1;
        }

        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCR(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableCR");
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcSupplierCode", SqlDbType.VarChar, 5, "vcSupplierCode");
                    cmdln.Parameters.Add("@vcCpdCompany", SqlDbType.VarChar, 5, "vcCpdCompany");
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamilyCode");
                    cmdln.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 14, "vcPartsNo");
                    cmdln.Parameters.Add("@vcPartsNameEN", SqlDbType.VarChar, 500, "vcPartsNameEN");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@vcLogisticRoute", SqlDbType.VarChar, 500, "vcLogisticRoute");
                    cmdln.Parameters.Add("@iQuantityPerContainer", SqlDbType.VarChar, 8, "iQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 2, "vcBanZhi01");
                    cmdln.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                    cmdln.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                    cmdln.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 2, "vcBanZhi02");
                    cmdln.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                    cmdln.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                    cmdln.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 2, "vcBanZhi03");
                    cmdln.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                    cmdln.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                    cmdln.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 2, "vcBanZhi04");
                    cmdln.Parameters.Add("@vcRemark1", SqlDbType.VarChar, 500, "vcRemark1");
                    cmdln.Parameters.Add("@vcRemark2", SqlDbType.VarChar, 500, "vcRemark2");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcPhotoPath", SqlDbType.Image, 999999, "vcPhotoPath");
                    cmdln.Parameters.Add("@vcQRCodeImge", SqlDbType.Image, 999999, "vcQRCodeImge");
                    cmdln.Parameters.Add("@vcDock", SqlDbType.VarChar, 4, "vcDock");
                    cmdln.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 14, "vcKBorderno");
                    cmdln.Parameters.Add("@vcNo", SqlDbType.VarChar, 50, "vcNo");
                    cmdln.Parameters.Add("@vcorderno", SqlDbType.VarChar, 50, "vcorderno");
                    cmdln.Parameters.Add("@vcPorType", SqlDbType.VarChar, 50, "vcPorType");
                    cmdln.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 50, "vcEDflag");
                    cmdln.Parameters.Add("@vcAB01", SqlDbType.VarChar, 10, "vcAB01");//20180921添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB02", SqlDbType.VarChar, 10, "vcAB02");//20180921添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB03", SqlDbType.VarChar, 10, "vcAB03");//20180921添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB04", SqlDbType.VarChar, 10, "vcAB04");//20180921添加AB值信息 - 李兴旺
                    SqlDataAdapter adaln = new SqlDataAdapter();
                    adaln.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入看板确认单Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableExcel(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableExcel");
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamlyCode");
                    cmdln.Parameters.Add("@vcpartsNo", SqlDbType.VarChar, 12, "vcpartsNo");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@iQuantityPerContainer", SqlDbType.VarChar, 8, "iQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcPCB01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcPCB02");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 2, "vcPCB03");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcorderno", SqlDbType.VarChar, 50, "vcKBorderno");
                    cmdln.Parameters.Add("@vcPorType", SqlDbType.VarChar, 50, "vcPorType");
                    SqlDataAdapter adaln = new SqlDataAdapter();
                    adaln.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取要从临时表中获取打印的生产部署160
        /// </summary>
        /// <returns></returns>
        public DataTable searchPorType()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcorderno,vcPorType,vcComDate01,vcBanZhi01 from testprinterCR group by vcorderno,vcPorType,vcComDate01,vcBanZhi01 ");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 获取要从临时表中获取打印的生产部署170
        /// </summary>
        /// <returns></returns>
        public DataTable searchPorType00()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcorderno,vcPorType,vcComDate01,vcBanZhi01,vcBanZhi00,vcComDate00 from testprinterCR group by vcorderno,vcPorType,vcComDate01,vcBanZhi01,vcBanZhi00,vcComDate00 ");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 插入到主表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRMain(DataTable dt, DataTable dtPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                string vcSql = "";
                try
                {
                    for (int z = 0; z < dtPorType.Rows.Count; z++)
                    {
                        DataRow[] row = dt.Select("vcPorType='" + dtPorType.Rows[z]["vcPorType"].ToString() + "' and vcorderno='" + dtPorType.Rows[z]["vcorderno"].ToString() + "' and vcComDate01='" + dtPorType.Rows[z]["vcComDate01"].ToString() + "' and vcBanZhi01='" + dtPorType.Rows[z]["vcBanZhi01"].ToString() + "'");
                        for (int i = 0; i < row.Length; i = i + 3)
                        {
                            if (i < row.Length)
                            {
                                if (i + 1 < row.Length)
                                {
                                    if (i + 2 < row.Length)
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],[vcNo3],vcPorType,vcorderno,vcComDate01,vcBanZhi01) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + row[i + 2]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "')";
                                    }
                                    else
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],vcPorType,vcorderno,vcComDate01,vcBanZhi01) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "')";
                                    }
                                }
                                else
                                {
                                    vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],vcPorType,vcorderno,vcComDate01,vcBanZhi01) VALUES ('" + row[i]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "')";
                                }
                            }
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = vcSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入到特殊打印主表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRMainEND(DataTable dt, DataTable dtPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                string vcSql = "";
                try
                {
                    for (int z = 0; z < dtPorType.Rows.Count; z++)
                    {
                        DataRow[] row = dt.Select("vcPorType='" + dtPorType.Rows[z]["vcPorType"].ToString() + "'");
                        for (int i = 0; i < row.Length; i = i + 3)
                        {
                            if (i < row.Length)
                            {
                                if (i + 1 < row.Length)
                                {
                                    if (i + 2 < row.Length)
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],[vcNo3],vcPorType) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + row[i + 2]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "')";
                                    }
                                    else
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],vcPorType) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "')";
                                    }
                                }
                                else
                                {
                                    vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],vcPorType) VALUES ('" + row[i]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "')";
                                }
                            }
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = vcSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入到主表临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRMain00(DataTable dt, DataTable dtPorType)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                string vcSql = "";
                try
                {
                    for (int z = 0; z < dtPorType.Rows.Count; z++)
                    {
                        DataRow[] row = dt.Select("vcPorType='" + dtPorType.Rows[z]["vcPorType"].ToString() + "' and vcorderno='" + dtPorType.Rows[z]["vcorderno"].ToString() + "' and vcComDate01='" + dtPorType.Rows[z]["vcComDate01"].ToString() + "' and vcBanZhi01='" + dtPorType.Rows[z]["vcBanZhi01"].ToString() + "'and vcComDate00='" + dtPorType.Rows[z]["vcComDate00"].ToString() + "'and vcBanZhi00='" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "'");
                        for (int i = 0; i < row.Length; i = i + 3)
                        {
                            if (i < row.Length)
                            {
                                if (i + 1 < row.Length)
                                {
                                    if (i + 2 < row.Length)
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],[vcNo3],vcPorType,vcorderno,vcComDate01,vcBanZhi01,vcComDate00,vcBanZhi00) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + row[i + 2]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "','" + dtPorType.Rows[z]["vcComDate00"].ToString() + "','" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "')";
                                    }
                                    else
                                    {
                                        vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],[vcNo2],vcPorType,vcorderno,vcComDate01,vcBanZhi01,vcComDate00,vcBanZhi00) VALUES ('" + row[i]["vcNo"] + "','" + row[i + 1]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "','" + dtPorType.Rows[z]["vcComDate00"].ToString() + "','" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "')";
                                    }
                                }
                                else
                                {
                                    vcSql = "INSERT INTO [testprinterCRMAIN]([vcNo1],vcPorType,vcorderno,vcComDate01,vcBanZhi01,vcComDate00,vcBanZhi00) VALUES ('" + row[i]["vcNo"] + "','" + dtPorType.Rows[z]["vcPorType"] + "','" + dtPorType.Rows[z]["vcorderno"] + "','" + dtPorType.Rows[z]["vcComDate01"] + "','" + dtPorType.Rows[z]["vcBanZhi01"] + "','" + dtPorType.Rows[z]["vcComDate00"].ToString() + "','" + dtPorType.Rows[z]["vcBanZhi00"].ToString() + "')";
                                }
                            }
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = vcSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 检索打印数据
        /// </summary>
        /// <returns></returns>
        private DataTable searchPrintCR()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcSupplierCode],[vcCpdCompany],[vcCarFamilyCode],[vcPartsNo], ");
            strSQL.AppendLine("       [vcPartsNameEN],[vcPartsNameCHN],[vcLogisticRoute],[iQuantityPerContainer],");
            strSQL.AppendLine("       [vcProject01],[vcComDate01],[vcBanZhi01],[vcProject02],[vcComDate02],");
            strSQL.AppendLine("       [vcBanZhi02],[vcProject03],[vcComDate03],[vcBanZhi03],[vcProject04],");
            strSQL.AppendLine("       [vcComDate04],[vcBanZhi04],[vcRemark1],[vcRemark2],[vcKBSerial],");
            strSQL.AppendLine("       [vcPhotoPath],[vcQRCodeImge],vcKBorderno");
            strSQL.AppendLine("  FROM [testprinterCR]");
            strSQL.AppendLine("");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 更新看板打印表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool UpdatePrintKANB(DataTable dt, string type)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcPartsNo = "";
                        string vcDock = "";
                        string vcPlanMonth = "";
                        if (type == "1")
                        {
                            vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        else if (type == "2")
                        {
                            vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            vcPlanMonth = dt.Rows[i]["vcplanMoth"].ToString();
                            DataTable dts = serachMaster(vcPartsNo, vcDock, vcPlanMonth);
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',vcKBType='1',[vcPrintTime] = GETDATE(),vcPrintflagED='" + dts.Rows[0]["vcPartsNo"].ToString() + "',vcDockED='" + dts.Rows[0]["vcDock"].ToString() + "',vcPrintTimeED=getdate() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        else if (type == "3")
                        {
                            //插入补给系统区分表
                            //更新内制看板系统iBaijianFlag字段
                            vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                            vcDock = dt.Rows[i]["vcDock"].ToString();
                            string up = dtKBSerialUP(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
                            if (up != "")
                            {
                                vcKBSerial = up;
                            }
                            strSql = "UPDATE [tKanbanPrintTbl] SET [iBaiJianFlag] ='1' where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPrintflagED]='" + vcPartsNo + "' and [vcDockED]='" + vcDock + "'";
                        }
                        if (strSql != "")
                        {
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = strSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        public string dtKBSerialUP(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerialBefore)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcKBSerialBefore]");
            strSQL.AppendLine("  FROM [KBSerial_history]");
            strSQL.AppendLine(" WHERE vcPartsNo='" + vcPartsNo + "'");
            strSQL.AppendLine("       AND vcDock='" + vcDock + "'");
            strSQL.AppendLine("       AND vcKBorderno='" + vcKBorderno + "'");
            strSQL.AppendLine("       AND vcKBSerial='" + vcKBSerialBefore + "'");
            if (excute.ExcuteSqlWithSelectToDT(strSQL.ToString()).Rows.Count != 0)
            {
                return excute.ExcuteSqlWithSelectToDT(strSQL.ToString()).Rows[0]["vcKBSerialBefore"].ToString();
            }
            else
            {
                return "";
            }
        }

        public DataTable serachMaster(string vcpart, string vcdock, string vcPlanMonth)
        {
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "select vcPartsNo,vcDock from tPartInfoMaster where vcPartsNo like '" + vcpart.Substring(0, 10).ToString() + "%' and substring(vcPartsNo,11,2)<>'ED' and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcPlanMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcPlanMonth.Replace("-", "") + "')";
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 更新看板打印表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool UpdatePrintKANBCRExcep(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "");
                        string vcDock = dt.Rows[i]["vcDock"].ToString();

                        strSql = "UPDATE [tKanbanPrintTblExcep] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        if (strSql != "")
                        {
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = strSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 更新看板打印表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type">再发行提前打印延迟打印</param>
        /// <returns></returns>
        public bool UpdatePrintKANB170(DataTable dt, string type)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                string vcDateTime = System.DateTime.Now.ToString();
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();

                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();

                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();

                        string vcDock = dt.Rows[i]["vcDock"].ToString();

                        if (type != "再发行")
                        {
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "'  and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        if (strSql != "")
                        {
                            SqlCommand _Command = new SqlCommand()
                            {
                                Connection = connln,
                                CommandText = strSql,
                                CommandType = CommandType.Text,
                                Transaction = trans,
                                CommandTimeout = 0
                            };
                            _Command.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCRExcep(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableCRExcep");
                    cmdln.CommandType = CommandType.StoredProcedure;
                    cmdln.Parameters.Add("@vcSupplierCode", SqlDbType.VarChar, 5, "vcSupplierCode");
                    cmdln.Parameters.Add("@vcCpdCompany", SqlDbType.VarChar, 5, "vcCpdCompany");
                    cmdln.Parameters.Add("@vcCarFamilyCode", SqlDbType.VarChar, 5, "vcCarFamilyCode");
                    cmdln.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 14, "vcPartsNo");
                    cmdln.Parameters.Add("@vcPartsNameEN", SqlDbType.VarChar, 500, "vcPartsNameEN");
                    cmdln.Parameters.Add("@vcPartsNameCHN", SqlDbType.VarChar, 500, "vcPartsNameCHN");
                    cmdln.Parameters.Add("@vcLogisticRoute", SqlDbType.VarChar, 500, "vcLogisticRoute");
                    cmdln.Parameters.Add("@vcQuantityPerContainer", SqlDbType.VarChar, 8, "vcQuantityPerContainer");
                    cmdln.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                    cmdln.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                    cmdln.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 2, "vcBanZhi01");
                    cmdln.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                    cmdln.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                    cmdln.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 2, "vcBanZhi02");
                    cmdln.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                    cmdln.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                    cmdln.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 2, "vcBanZhi03");
                    cmdln.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                    cmdln.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                    cmdln.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 2, "vcBanZhi04");
                    cmdln.Parameters.Add("@vcRemark1", SqlDbType.VarChar, 500, "vcRemark1");
                    cmdln.Parameters.Add("@vcRemark2", SqlDbType.VarChar, 500, "vcRemark2");
                    cmdln.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                    cmdln.Parameters.Add("@vcPhotoPath", SqlDbType.Image, 999999, "vcPhotoPath");
                    cmdln.Parameters.Add("@vcTip", SqlDbType.VarChar, 500, "vcTip");
                    cmdln.Parameters.Add("@vcDock", SqlDbType.VarChar, 4, "vcDock");
                    cmdln.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 14, "vcKBorderno");
                    cmdln.Parameters.Add("@vcNo", SqlDbType.VarChar, 50, "vcNo");
                    cmdln.Parameters.Add("@vcorderno", SqlDbType.VarChar, 50, "vcorderno");
                    cmdln.Parameters.Add("@vcPorType", SqlDbType.VarChar, 50, "vcPorType");
                    cmdln.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 50, "vcEDflag");
                    SqlDataAdapter adaln = new SqlDataAdapter();
                    adaln.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    if (adapter.InsertCommand != null)
                    {
                        adapter.InsertCommand.Connection = connln;
                        adapter.InsertCommand.Transaction = trans;
                        adapter.InsertCommand.CommandTimeout = 0;
                    }
                    if (adapter.DeleteCommand != null)
                    {
                        adapter.DeleteCommand.Connection = connln;
                        adapter.DeleteCommand.Transaction = trans;
                        adapter.DeleteCommand.CommandTimeout = 0;
                    }
                    if (adapter.UpdateCommand != null)
                    {
                        adapter.UpdateCommand.Connection = connln;
                        adapter.UpdateCommand.Transaction = trans;
                        adapter.UpdateCommand.CommandTimeout = 0;
                    }
                    adapter.Update(dt);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 看板打印信息ALL0170
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable searchPrintKANBALL(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial)//不必修改
        {
            string partfrom = "";
            StringBuilder strSQL = new StringBuilder();
            partfrom += vcKBorderno + vcKBSerial + vcPartsNo.ToString().Replace("-", "") + vcDock;
            strSQL.AppendLine("SELECT A.vcPartsNo  as vcPartsNo,");
            strSQL.AppendLine("       A.vcDock as vcDock,");
            strSQL.AppendLine("       B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,a.vcEDflag,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
            strSQL.AppendLine("       A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType,A.iBaiJianFlag,ISNULL(A.vcPrintflagED,'') AS vcPrintflagED,ISNULL(A.vcDockED,'') AS vcDockED from ");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPartsNo+A.vcDock) in ('" + partfrom + "')");
            strSQL.AppendLine("    OR    (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPrintflagED+A.vcDockED) in ('" + partfrom + "')");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        /// <summary>
        /// 看板打印信息ALL0170
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable searchPrintKANBALL(string vcPartsNo, string vcDock, string vcKBorderno)//不必修改
        {
            string partfrom = "";
            StringBuilder strSQL = new StringBuilder();
            partfrom += vcKBorderno + vcPartsNo.ToString().Replace("-", "") + vcDock;
            strSQL.AppendLine("SELECT A.vcPartsNo  as vcPartsNo,");
            strSQL.AppendLine("       A.vcDock as vcDock,");
            strSQL.AppendLine("       B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,a.vcEDflag,");
            strSQL.AppendLine("       B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
            strSQL.AppendLine("       A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("       isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,");
            strSQL.AppendLine("       isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,");
            strSQL.AppendLine("       isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,");
            strSQL.AppendLine("       isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,");
            strSQL.AppendLine("       isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType,A.iBaiJianFlag,ISNULL(A.vcPrintflagED,'') AS vcPrintflagED,ISNULL(A.vcDockED,'') AS vcDockED from ");
            strSQL.AppendLine("(select * from tKanbanPrintTbl) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select * from tPartInfoMaster) B");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            strSQL.AppendLine("    WHERE (A.[vcKBorderno]+A.vcPartsNo+A.vcDock) in ('" + partfrom + "')");
            strSQL.AppendLine("    OR    (A.[vcKBorderno]+A.vcPrintflagED+A.vcDockED) in ('" + partfrom + "')");

            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #region 获取表结构 为打印数据填充提供DataTable
        public DataTable searchTBCreate()//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select top(1)* from testprinterCR");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion
        /// <summary>
        /// CRExcep表结构
        /// </summary>
        /// <returns></returns>
        public DataTable searchTBCreateCRExcep()//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select top(1)* from testprinterCRExcep");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        /// <summary>
        /// 清空打印临时表
        /// </summary>
        /// <returns></returns>
        public bool TurnCate()
        {
            //清空临时表tPartInfoMaster_Temp
            SqlCommand cd = new SqlCommand();
            cd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            if (cd.Connection.State == ConnectionState.Closed)
            {
                cd.Connection.Open();
            }
            //
            cd.CommandText = "truncate table testprinterCR truncate table [testprinterCRMAIN] truncate table [testprinterCRExcep] truncate table testprinterExcel";
            cd.ExecuteNonQuery();
            if (cd.Connection.State == ConnectionState.Open)
            {
                cd.Connection.Close();
            }
            return true;
        }
        /// <summary>
        /// 获取照片等信息
        /// </summary>
        /// <returns></returns>
        public DataTable searchProRuleMst(string vcPartsNo, string vcDock)//不必修改
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT A.vcSupplierCode,A.vcSupplierPlant,A.vcCarFamilyCode,A.vcPartsNameEN,A.vcPartsNameCHN,A.vcLogisticRoute,A.iQuantityPerContainer,a.vcCpdCompany,");
            strSQL.AppendLine("       B. vcProName1,B.vcProName2,B.vcProName3,B.vcProName4,A.vcRemark1,A.vcRemark2,A.vcPhotoPath FROM (");
            strSQL.AppendLine("(select * from tPartInfoMaster) A");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select vcPorType,vcZB,vcProName1,vcProName2,vcProName3,vcProName4 from ProRuleMst) B");
            strSQL.AppendLine("on A.vcPorType=B.vcPorType AND A.vcZB=B.vcZB)");
            strSQL.AppendLine("WHERE A.vcPartsNo='" + vcPartsNo + "' and A.vcDock='" + vcDock + "'");
            strSQL.AppendLine("");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        /// <summary>
        /// 插入连番表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableKBSerial(DataTable dt)
        {
            using (SqlConnection connln = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connln.State != ConnectionState.Open)
                    connln.Open();
                SqlTransaction trans = connln.BeginTransaction();
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSqlIn = "";
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcDock = dt.Rows[i]["vcDock"].ToString();
                        string vcKBorderno = dt.Rows[i]["vcKBorderno"].ToString();
                        string vcKBSerialBefore = dt.Rows[i]["vcKBSerialBefore"].ToString();
                        strSqlIn += "INSERT INTO [KBSerial_history]";
                        strSqlIn += "           ([vcPartsNo]";
                        strSqlIn += "           ,[vcDock]";
                        strSqlIn += "           ,[vcKBorderno]";
                        strSqlIn += "           ,[vcKBSerial]";
                        strSqlIn += "           ,[vcKBSerialBefore]";
                        strSqlIn += "           ,[dCreatTime])";
                        strSqlIn += "     VALUES";
                        strSqlIn += "           ('" + vcPartsNo + "'";
                        strSqlIn += "            ,'" + vcDock + "'";
                        strSqlIn += "            ,'" + vcKBorderno + "'";
                        strSqlIn += "            ,'" + vcKBSerial + "'";
                        strSqlIn += "            ,'" + vcKBSerialBefore + "'";
                        strSqlIn += "            ,getdate())";

                        SqlCommand _Command = new SqlCommand()
                        {
                            Connection = connln,
                            CommandText = strSqlIn,
                            CommandType = CommandType.Text,
                            Transaction = trans,
                            CommandTimeout = 0
                        };
                        _Command.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

    }
}