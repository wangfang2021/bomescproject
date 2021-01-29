using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS1209_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索信息栏绑定生产部署 str2是权限部署
        public DataTable dllPorType(string[] str2)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            if (str2.Length != 0)
            {
                if (str2[0] != "000000")
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
                    strSQL.AppendLine("select ' ' as [Text],'0' as [Value]   ");
                    strSQL.AppendLine(" union all ");
                    strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType' and [vcData1] in(" + ProType + ") ");
                }
                else
                {
                    strSQL.AppendLine("select ' ' as [Text],'0' as [Value]   ");
                    strSQL.AppendLine(" union all ");
                    strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
                }
            }
            else
            {
                strSQL.AppendLine("select ' ' as [Text],'PP' as [Value]   ");
            }
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #region 绑定工场 1 2 3 厂
        public DataTable dllPorPlant()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select ' ' as [Text],'0' as [Value] union all ");
            strSQL.AppendLine(" select distinct vcData2 as Text, vcData1 as Value from ConstMst where vcDataId='KBPlant'");
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
            strSQL.AppendLine("SELECT vcPrinterName FROM tPrint WHERE vcUserFlag='" + userid + "'");
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
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" declare @banzhi varchar(1) ");
            strSQL.AppendLine(" set @banzhi=(select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcbaibanend as varchar(8)) from Mbaiye)) then '0' ");
            strSQL.AppendLine("                           when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),DateAdd(DAY,1,GETDATE()),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                           when GETDATE()>=(select CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                           when GETDATE()>=(select CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)+' '+(select cast(vcyebanbegin as varchar(8)) from Mbaiye)) and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' '+(select cast(vcyebanend as varchar(8)) from Mbaiye)) then '1'");
            strSQL.AppendLine("                      else '' end) as [by]) ");
            strSQL.AppendLine(" declare @vcComDate varchar(10) ");
            strSQL.AppendLine(" set @vcComDate=(select (case when GETDATE()>=(select CONVERT(varchar(10),GETDATE(),121)+' 00:00:00.000') and GETDATE()<=(select CONVERT(varchar(10),GETDATE(),121)+' 06:00:00.000') then CONVERT(varchar(10),DateAdd(DAY,-1,GETDATE()),121)");
            strSQL.AppendLine("                else CONVERT(varchar(10),GETDATE(),121) end) as [by])");

            strSQL.AppendLine("select substring(a.vcPartsNo,0, 6) + '-' + substring(a.vcPartsNo,6, 5) + '-' + substring(a.vcPartsNo,11, 2)  as vcPartsNo,A.vcDock,A.vcCarType AS vcCarFamilyCode,vcProType as vcPorType,");
            strSQL.AppendLine("CASE when A.vcEDflag='S' then '通常' when A.vcEDflag='E' then '紧急' else A.vcEDflag end vcEDflag,");
            strSQL.AppendLine(" vcKBorderno,b.vcQFflag, '' as [image],A.vcKBSerial,vcTips,vcPlanMonth,A.iNo,A.vcPartFrequence ");//品番频度
            strSQL.AppendLine(" FROM ( ");
            //strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");//给看板打印数据left join品番频度
            strSQL.AppendLine("(SELECT distinct iNo,T1.vcPartsNo,vcDock,vcCarType,vcEDflag,vcKBorderno,vcKBSerial,vcTips,vcPrintflag,vcPrintTime,vcKBType,vcProject00,vcProject01,vcProject02,vcProject03,vcProject04,vcComDate00,vcComDate01,vcComDate02,vcComDate03,vcComDate04,vcBanZhi00,vcBanZhi01,vcBanZhi02,vcBanZhi03,vcBanZhi04,vcAB00,vcAB01,vcAB02,vcAB03,vcAB04,dCreatTime,vcCreater,dUpdateTime,vcUpdater,vcPlanMonth,vcPrintSpec,vcPrintflagED,vcDockED,vcPrintTimeED,vcQuantityPerContainer,iBaiJianFlag,T2.vcPartFrequence FROM tKanbanPrintTbl T1 left join (SELECT vcPartsNo,vcPartFrequence FROM tPartInfoMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE()) T2 on T1.vcPartsNo=T2.vcPartsNo) A ");
            strSQL.AppendLine("left join ");
            strSQL.AppendLine("(select distinct vcProType,vcPartsNo,vcDock,vcMonth,vcCarType,vcQFflag,vcPlant from tPlanPartInfo) B ");
            strSQL.AppendLine("on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock and A.vcPlanMonth=B.vcMonth and A.vcCarType=B.vcCarType )");
            strSQL.AppendLine("where A.vcComDate00=CONVERT(varchar(10),@vcComDate,121) and  A.vcPrintflag is null and A.vcPrintflagED is null");

            if (vcType == "2")
            {
                strSQL.AppendLine(" and A.vcBanZhi00=@banzhi and vcQFflag='1'");
            }
            if (vcType == "1")
            {
                strSQL.AppendLine(" and A.vcBanZhi00=@banzhi and vcQFflag<>'1'");
            }
            if (!string.IsNullOrEmpty(vcPrintPartNo))
            {
                strSQL.AppendLine(" and A.vcPartsNo like '%" + vcPrintPartNo.Replace("-", "") + "%'");
            }
            if (!string.IsNullOrEmpty(vcKbOrderId))
            {
                strSQL.AppendLine(" and A.vcKBorderno = '" + vcKbOrderId + "'");
            }
            if (!string.IsNullOrEmpty(vcLianFan))
            {
                strSQL.AppendLine(" and A.vcKBSerial = '" + vcLianFan + "'");
            }
            if (vcPorType != "")
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
            if (vcPlant != "")
            {
                strSQL.AppendLine(" and B.vcPlant='" + vcPlant + "'");
            }
            strSQL.AppendLine("order by vcProType,vcKBorderno,A.vcPartFrequence,A.vcKBSerial");//添加按品番排序（取消），添加品番频度排序
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
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
            strSQL.AppendLine("SELECT substring(A.vcPrintflagED ,0, 6) + '-' + substring(A.vcPrintflagED ,6, 5) + '-' + substring(A.vcPrintflagED ,11, 2)  as vcPartsNo ,A.vcDockED as vcDock,A.vcCarType AS vcCarFamilyCode,vcPorType,");
            strSQL.AppendLine("CASE when A.vcEDflag='S' then '通常' when A.vcEDflag='E' then '紧急' else  A.vcEDflag end vcEDflag,");
            strSQL.AppendLine("  vcKBorderno,B.vcPhotoPath as [image],A.vcKBSerial,vcTips,vcPlanMonth,A.iNo");
            strSQL.AppendLine("  FROM ( ");
            strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");
            strSQL.AppendLine(" left join ");
            strSQL.AppendLine(" (select * from tPartInfoMaster) B");
            strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock)");
            strSQL.AppendLine("      where A.vcPrintflag='1' and A.vcPrintflagED is not null and (a.vcPartsNo+a.vcDock+A.vcKBorderno+a.vcKBSerial)=any(" + KBorderno + ")");
            //strSQL.AppendLine(" A.vcComDate00=CONVERT(varchar(10),GETDATE(),121) and");
            if (!string.IsNullOrEmpty(vcPrintPartNo))
            {
                strSQL.AppendLine(" and A.vcPrintflagED='" + vcPrintPartNo.Replace("-", "") + "'");
            }
            if (!string.IsNullOrEmpty(vcKbOrderId))
            {
                strSQL.AppendLine(" and A.vcKBorderno = '" + vcKbOrderId + "'");
            }
            if (!string.IsNullOrEmpty(vcLianFan))
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
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select vcPartsNo,vcDock,vcKBorderno,vcKBSerial from tKanbanPrintTbl  where (vcPrintflagED+vcDockED+vcKBorderno+vcKBSerial)=any(" + KBorderno + ")");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public string dtKBSerialUP1(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerialBefore)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcKBSerialBefore]");
            strSQL.AppendLine("  FROM [KBSerial_history]");
            strSQL.AppendLine(" WHERE vcPartsNo='" + vcPartsNo + "'");
            strSQL.AppendLine("       AND vcDock='" + vcDock + "'");
            strSQL.AppendLine("       AND vcKBorderno='" + vcKBorderno + "'");
            strSQL.AppendLine("       AND vcKBSerial='" + vcKBSerialBefore + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0]["vcKBSerialBefore"].ToString();
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

        public string resQuantityPerContainer(string vcpartno, string vcdock, string vcPlanMonth)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [iQuantityPerContainer]  FROM [tPartInfoMaster] WHERE [vcPartsNo]='" + vcpartno + "' AND [vcDock]='" + vcdock + "'  ");
            strSQL.AppendLine("  and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcPlanMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcPlanMonth.Replace("-", "") + "')");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0]["iQuantityPerContainer"].ToString();
            }
            else
            {
                return "0";
            }
        }
        public string resKBSerialend(string vcKBorderno)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select MAX(vckbserial) as vckbserial from [KBSerial_history] where [vcKBorderno]='" + vcKBorderno + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt.Rows[0]["vckbserial"].ToString() == "" ? "0000" : dt.Rows[0]["vckbserial"].ToString();
        }

        public DataTable ceshi()
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select top(20)vcPartsNo,vcCarType,vcProject00,vcProject04,'10' as meishu,vcKBSerial from tKanbanPrintTbl");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt;
        }

        public void DeleteprinterCREX(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            string strSQL = "delete from testprinterCR where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterCRMAIN] where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterExcel] where vcPorType='" + vcPorType + "' and vcKBorderno='" + vcorderno + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "' ";
            excute.ExcuteSqlWithStringOper(strSQL.ToString());
        }

        public void InsertInto(string vcorderno, string vcPorType)
        {
            string strSQL = "INSERT INTO [testprinterExcel1] select vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01,vcPCB02,vcPCB03,iQuantityPerContainer,vcPorType,vcKBorderno,vcKBSerial,vcComDate00,vcBanZhi00 from testprinterExcel where vcKBorderno='" + vcorderno + "' and vcPorType ='" + vcPorType + "'";
            excute.ExcuteSqlWithStringOper(strSQL.ToString());
        }

        public void DeleteprinterCREX1(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            string strSQL = "delete from testprinterCR where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterCRMAIN] where vcPorType='" + vcPorType + "' and vcorderno is null and vcComDate01 is null and vcBanZhi01 is null  ";
            excute.ExcuteSqlWithStringOper(strSQL.ToString());
        }

        //特殊打印删除
        public void DeleteprinterCREX2(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01)
        {
            string strSQL = "delete from testprinterCRExcep where vcPorType='" + vcPorType + "' and vcorderno='" + vcorderno + "' and vcComDate01='" + vcComDate01 + "' and vcBanZhi01='" + vcBanZhi01 + "' delete from  [testprinterCRMAIN] where vcPorType='" + vcPorType + "' and vcorderno is null and vcComDate01 is null and vcBanZhi01 is null ";
            excute.ExcuteSqlWithStringOper(strSQL.ToString());
        }

        public void InsertDate(string vcorderno, string vcPorType, string printIme, string printDay, string vcComDate01, string vcBanZhi01)
        {
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
            excute.ExcuteSqlWithStringOper(strSqlIn.ToString());
        }

        public DataTable check(string vcorderno, string vcPorType)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcOrderNo]  FROM [tKanBanQrTbl] WHERE [vcOrderNo] = '" + vcorderno + "'and [vcGC] = '" + vcPorType + "'");
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }

        public DataTable GetTable(DataTable dt)
        {
            try
            {
                string strplsql = "select t1.PARTSNO,t1.DOCK,t1.KANBANORDERNO,t1.KANBANSERIAL from ";
                strplsql += " (select PARTSNO,DOCK,ltrim(rtrim(KANBANORDERNO)) as KANBANORDERNO,ltrim(rtrim(KANBANSERIAL))  as KANBANSERIAL from sp_m_opr  where dataid='S0' and  substring(partsno,11,2)='ED' and kanbanorderno is not null)t1 ";
                //strplsql += " left join ";
                //strplsql += " (select  PARTSNO,DOCK,KBORDERNO,KBSERIAL from nz_m_inv)t2 ";
                //strplsql += " on t1.PARTSNO=t2.PARTSNO and t1.DOCK=t2.DOCK and t1.KANBANORDERNO=t2.KBORDERNO and t1.KANBANSERIAL=t2.KBSERIAL ";
                //strplsql += " where t2.PARTSNO is null ";
                DataTable dtTemp = excute.ExcuteSqlWithSelectToDT(strplsql.ToString());
                DataRow dr;
                foreach (DataRow drTemp in dtTemp.Rows)
                {
                    dr = dt.NewRow();
                    dr["PARTSNO"] = drTemp["PARTSNO"];
                    dr["DOCK"] = drTemp["DOCK"];
                    dr["KANBANORDERNO"] = drTemp["KANBANORDERNO"];
                    dr["KANBANSERIAL"] = drTemp["KANBANSERIAL"];
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InsertTable(DataTable dt)
        {
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());
            if (conn.State != ConnectionState.Open)
                conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection();
            cmd.CommandTimeout = 0;
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcKBorderno = dt.Rows[i]["vcKBorderno"].ToString();
                    string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                    string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString().Substring(0, 10) + "ED"; ;
                    string vcDock = dt.Rows[i]["vcDock"].ToString();
                    string StrOrl = "insert into nz_m_inv(partsno,dock,kborderno,kbserial,printflag) values ('" + vcPartsNo + "','" + vcDock + "','" + vcKBorderno + "','" + vcKBSerial + "','1')";
                    cmd.CommandText = StrOrl;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                return true;
            }
            catch
            {
                cmd.Transaction.Rollback();
                return false;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public string getRoleTip(string vcUserId)
        {
            string ssql = "  ";
            ssql += "select top (1) vcUserID, vcUserName, vcSpecial from sUser ";
            ssql += "where vcUserID='" + vcUserId + "'";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dt.Rows.Count == 0)
            {
                return "admin";
            }
            return dt.Rows[0]["vcSpecial"].ToString();
        }
    }
}