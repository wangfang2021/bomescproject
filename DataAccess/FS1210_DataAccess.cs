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
    public class FS1210_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        private DataTable getDataTableBySql(string sql)
        {
            return excute.ExcuteSqlWithSelectToDT(sql);
        }

        /// <summary>
        /// 检索数据
        /// </summary>
        public DataTable PrintData(string vcKbOrderId, string vcTF, string vcFBZ, string vcTT, string vcTBZ, string vcPartsNo, string vcCarType, string vcGC, string vcType, string vcplant, DataTable dtflag)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            if (vcType == "A")
            {
                strSQL.AppendLine("select distinct T1.vcPartsNo,vcDock,vcCarType,jinjiqufen,vcKBorderno,vcKBSerial,vcTips,iFlag,vcPlanMonth,iNo,vcPorType,T2.vcPartFrequence, '0' as vcModFlag,'0' as vcAddFlag from (");
                strSQL.AppendLine("SELECT vcPartsNo,vcDock,vcCarType,jinjiqufen,vcKBorderno,vcKBSerial,vcTips,iFlag,vcPlanMonth,iNo,vcPorType FROM");
                strSQL.AppendLine("(SELECT A.vcPartsNo,A.vcDock,A.vcCarType,jinjiqufen,vcKBorderno,vcKBSerial,vcTips,iFlag,B.vcPlant as vcPartPlant,B.vcProType as vcPorType,vcComDate00,vcBanZhi00,vcPlanMonth,iNo FROM ");
                strSQL.AppendLine("      (select vcPartsNo,vcDock,vcPrintflagED ,vcDockED,vcCarType,vcEDflag as jinjiqufen,vcKBorderno,vcKBSerial,'' as vcTips,''  as iFlag,vcComDate00,vcBanZhi00,vcPlanMonth,iNo ");
                strSQL.AppendLine("         from tKanbanPrintTbl where vcPrintflag='1')A");
                strSQL.AppendLine("       left join ");
                strSQL.AppendLine("      (select distinct vcPlant,vcPartsNo,vcDock,vcMonth,vcProType,vcCarType from tPlanPartInfo)B");
                strSQL.AppendLine("      on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock and A.vcPlanMonth = B.vcMonth and A.vcCarType = B.vcCarType");
                strSQL.AppendLine("");
                strSQL.AppendLine("union all");
                strSQL.AppendLine("select AA.vcPrintflagED AS vcPartsNo,AA.vcDockED AS vcDock,vcCarType,jinjiqufen,vcKBorderno,vcKBSerial,vcTips,iFlag,BB.vcPartPlant,BB.vcPorType,vcComDate00,vcBanZhi00,vcPlanMonth,iNo from ");
                strSQL.AppendLine("      (select vcPartsNo,vcDock,vcPrintflagED,vcDockED,vcCarType,vcEDflag as jinjiqufen,vcKBorderno,vcKBSerial,'' as vcTips,'' as iFlag,vcComDate00,vcBanZhi00,vcPlanMonth,iNo");
                strSQL.AppendLine("         from tKanbanPrintTbl where iBaiJianFlag='1')AA");
                strSQL.AppendLine("       left join ");
                strSQL.AppendLine("      (select * from tPartInfoMaster) BB");
                strSQL.AppendLine("       on AA.vcPartsNo=BB.vcPartsNo and AA.vcDock=BB.vcDock");
                strSQL.AppendLine(") X where 1=1");
                if (vcKbOrderId.Length != 0)
                    strSQL.AppendLine(" and X.vcKBorderno='" + vcKbOrderId + "'");
                if (vcTF.Length != 0 || vcTT.Length != 0)
                {
                    if (vcTF.Length != 0 && vcTT.Length != 0)
                        strSQL.AppendLine(" and (X.vcComDate00+X.vcBanZhi00>='" + vcTF + "'+'" + vcFBZ + "' and X.vcComDate00+X.vcBanZhi00<='" + vcTT + "'+'" + vcTBZ + "')");
                    else
                    {
                        if (vcTF != "")
                            strSQL.AppendLine(" and (X.vcComDate00+X.vcBanZhi00>='" + vcTF + "'+'" + vcFBZ + "' and X.vcComDate00+X.vcBanZhi00<='9999-12-31'+'1')");
                        else
                            strSQL.AppendLine(" and (X.vcComDate00+X.vcBanZhi00>='1900-01-01'+'0' and X.vcComDate00+X.vcBanZhi00<='" + vcTT + "'+'" + vcTBZ + "')");
                    }
                }
                if (vcPartsNo.Length != 0)
                {
                    strSQL.AppendLine(" and (X.vcPartsNo like '%" + vcPartsNo + "%')");
                }
                if (vcCarType.Length != 0)
                {
                    strSQL.AppendLine(" and X.vcCarType='" + vcCarType + "'");
                }
                if (vcGC != "")
                    strSQL.AppendLine(" and X.vcPorType='" + vcGC + "'");
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
                                flag += "','";
                            else
                                flag += "'";
                        }
                    }
                    strSQL.AppendLine(" and X.vcPorType in( " + flag + ")");
                }
                if (vcplant != "")
                    strSQL.AppendLine(" and X.vcPartPlant='" + vcplant + "'");
                //strSQL.AppendLine(" order by X.vcPorType,vcKBorderno,vcKBSerial");//原SQL文注释掉
                strSQL.AppendLine(" ) T1");
                strSQL.AppendLine("  left join (SELECT vcPartsNo,vcPartFrequence FROM tPartInfoMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE()) T2");//给看板打印数据left join品番频度 - 20190104李兴旺
                strSQL.AppendLine("  on T1.vcPartsNo=T2.vcPartsNo");
                strSQL.AppendLine("  order by T1.vcPorType,T1.vcKBorderno,T2.vcPartFrequence,T1.vcKBSerial");
            }
            else
            {
                strSQL.AppendLine("SELECT (CASE WHEN A.vcPrintflagED IS NOT NULL THEN A.vcPrintflagED ELSE A.vcPartsNo END ) AS vcPartsNo,");
                strSQL.AppendLine("       (CASE WHEN A.vcDockED IS NOT NULL THEN A.vcDockED ELSE A.vcDock END ) as vcDock,A.vcCarType as vcCarType,");
                strSQL.AppendLine("       A.vcEDflag as jinjiqufen,A.vcKBorderno as vcKBorderno,A.vcKBSerial as vcKBSerial, A.vcPlanMonth,");
                strSQL.AppendLine("       A.vcTips as vcTips,'' as iFlag,iNo,B.vcProType  as vcPorType,A.vcPartFrequence as vcPartFrequence, '0' as vcModFlag,'0' as vcAddFlag");//品番频度
                strSQL.AppendLine("FROM ( ");
                //strSQL.AppendLine(" (select * from tKanbanPrintTbl )A ");//给看板打印数据left join品番频度 - 20190104李兴旺
                strSQL.AppendLine(" (SELECT distinct iNo,T1.vcPartsNo,vcDock,vcCarType,vcEDflag,vcKBorderno,vcKBSerial,vcTips,vcPrintflag,vcPrintTime,vcKBType,vcProject00,vcProject01,vcProject02,vcProject03,vcProject04,vcComDate00,vcComDate01,vcComDate02,vcComDate03,vcComDate04,vcBanZhi00,vcBanZhi01,vcBanZhi02,vcBanZhi03,vcBanZhi04,vcAB00,vcAB01,vcAB02,vcAB03,vcAB04,dCreatTime,vcCreater,dUpdateTime,vcUpdater,vcPlanMonth,vcPrintSpec,vcPrintflagED,vcDockED,vcPrintTimeED,vcQuantityPerContainer,iBaiJianFlag,T2.vcPartFrequence FROM tKanbanPrintTbl T1 left join (SELECT vcPartsNo,vcPartFrequence FROM tPartInfoMaster where dTimeFrom<=GETDATE() and dTimeTo>=GETDATE()) T2 on T1.vcPartsNo=T2.vcPartsNo) A");
                strSQL.AppendLine(" left join ");
                strSQL.AppendLine(" (select distinct vcProType,vcPartsNo,vcDock,vcMonth,vcCarType,vcPlant  from tPlanPartInfo) B ");
                strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo AND A.vcDock=B.vcDock  and A.vcPlanMonth = B.vcMonth and A.vcCarType = B.vcCarType)");
                if (vcType == "B")
                    strSQL.AppendLine(" where A.vcComDate00>=CONVERT(varchar(10),GETDATE(),120) and  ((A.vcPrintflag is null) or (A.vcKBType is null and substring(A.vcPartsNo,11,2)='ED'))");
                if (vcType == "C")
                    strSQL.AppendLine(" where A.vcComDate00<=CONVERT(varchar(10),GETDATE(),120) and  ((A.vcPrintflag is null) or (A.vcKBType is null and substring(A.vcPartsNo,11,2)='ED'))");
                if (vcKbOrderId.Length != 0)
                    strSQL.AppendLine(" and A.vcKBorderno='" + vcKbOrderId + "'");
                if (vcTF.Length != 0 || vcTT.Length != 0)
                {
                    if (vcTF.Length != 0 && vcTT.Length != 0)
                        strSQL.AppendLine(" and (A.vcComDate00+A.vcBanZhi00>='" + vcTF + "'+'" + vcFBZ + "' and A.vcComDate00+A.vcBanZhi00<='" + vcTT + "'+'" + vcTBZ + "')");
                    else
                    {
                        if (vcTF != "")
                            strSQL.AppendLine(" and (A.vcComDate00+A.vcBanZhi00>='" + vcTF + "'+'" + vcFBZ + "' and A.vcComDate00+A.vcBanZhi00<='9999-12-31'+'1')");
                        else
                            strSQL.AppendLine(" and (A.vcComDate00+A.vcBanZhi00>='1900-01-01'+'0' and A.vcComDate00+A.vcBanZhi00<='" + vcTT + "'+'" + vcTBZ + "')");
                    }
                }
                if (vcPartsNo.Length != 0)
                    strSQL.AppendLine(" and (A.vcPartsNo like '%" + vcPartsNo + "%' or A.vcPrintflagED like '%" + vcPartsNo + "%')");
                if (vcCarType.Length != 0)
                    strSQL.AppendLine(" and A.vcCarType='" + vcCarType + "'");
                if (vcGC != "")
                    strSQL.AppendLine(" and B.vcProType='" + vcGC + "'");
                else
                {
                    string flag = "";
                    if (dtflag.Rows.Count != 0)
                    {
                        if (!(dtflag.Rows[0]["Text"].ToString() == "000000" && dtflag.Rows.Count == 1))
                        {
                            flag += "'";
                            for (int i = 0; i < dtflag.Rows.Count; i++)
                            {
                                flag += dtflag.Rows[i]["Text"].ToString().Trim();
                                if (i < dtflag.Rows.Count - 1)
                                    flag += "','";
                                else
                                    flag += "'";
                            }
                            strSQL.AppendLine(" and vcProType in( " + flag + ")");
                        }
                    }
                }
                if (vcplant != "")
                    strSQL.AppendLine(" and B.vcPlant='" + vcplant + "'");
                strSQL.AppendLine(" order by b.vcProType,vcKBorderno,A.vcPartFrequence,vcKBSerial");//添加品番频度排序
            }
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        /// <summary>
        /// 关联数据查看是否存在在打印数据
        /// </summary>
        public DataTable seaKBnoser(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            StringBuilder strSQL1 = new StringBuilder();
            strSQL.AppendLine("SELECT [iNo],[vcPartsNo],[vcDock],[vcCarType],[vcKBorderno],[vcKBSerial],[vcTips],[vcPrintflag],[vcPrintTime],[vcKBType],[vcProject00],[vcProject01],[vcProject02]");
            strSQL.AppendLine("      ,[vcProject03],[vcProject04],[vcComDate00],[vcComDate01],[vcComDate02],[vcComDate03],[vcComDate04],[vcBanZhi00],[vcBanZhi01],[vcBanZhi02],[vcBanZhi03],[vcBanZhi04]");
            strSQL.AppendLine("      ,[dCreatTime],[vcCreater],[dUpdateTime],[vcUpdater],[vcPlanMonth],[vcPrintSpec],[vcPrintflagED],[vcDockED],[vcPrintTimeED],[vcQuantityPerContainer],[iBaiJianFlag]");
            strSQL.AppendLine("      ,'' as vcPorType,vcEDflag,case when vcEDflag='S' then '通常' when vcEDflag='E' then '紧急' else vcEDflag END AS vcEDflagShow");
            strSQL.AppendLine("  FROM [tKanbanPrintTbl]");
            strSQL.AppendLine("  where ((vcPartsNo='" + vcPartsNo + "' and vcDock='" + vcDock + "')or (vcPrintflagED='" + vcPartsNo + "' and vcDockED='" + vcDock + "')) and [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "'");

            dt = getDataTableBySql(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                strSQL1.AppendLine("  select * from tPartInfoMaster where vcPartsNo='" + vcPartsNo + "' and vcDock='" + vcDock + "' and  (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + dt.Rows[0]["vcPlanMonth"].ToString().Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + dt.Rows[0]["vcPlanMonth"].ToString().Replace("-", "") + "')");
                dt1 = getDataTableBySql(strSQL1.ToString());
                if (dt1.Rows.Count != 0)
                {
                    dt.Rows[0]["vcPorType"] = dt1.Rows[0]["vcPorType"].ToString();
                }
                else
                {
                    dt.Rows[0]["vcPorType"] = "XXXX";
                }
            }
            return dt;
        }

        public DataTable SearchPartData(DataRow dr)
        {
            StringBuilder sqlget = new StringBuilder();
            sqlget.AppendLine("SELECT  t1.[iNo]      ,t1.[vcEDflag]      ,t1.[vcPlanMonth]      ,t2.vcProType");
            sqlget.AppendLine("  FROM [tKanbanPrintTbl] t1");
            sqlget.AppendLine("  left join  tPlanPartInfo t2 on t1.vcPlanMonth = t2.vcMonth  and t1.vcPartsNo =  t2.vcPartsNo ");
            sqlget.AppendLine("                             and t1.vcDock = t2.vcDock and t1.vcEDflag = t2.vcEDFlag  ");
            sqlget.AppendLine("  where t1.vcPartsNo = '" + dr["vcPartsNo"].ToString() + "' and t1.vcDock = '" + dr["vcDock"].ToString() + "' and t1.vcCarType = '" + dr["vcCarType"].ToString() + "' ");
            sqlget.AppendLine("    and t1.vcKBorderno = '" + dr["vcKBorderno"].ToString() + "' and t1.vcKBSerial = '" + dr["vcKBSerial"].ToString() + "' ");
            return getDataTableBySql(sqlget.ToString());
        }

        public DataTable CheckPrint(string vcKBnoser)
        {
            StringBuilder sqlget = new StringBuilder();
            sqlget.AppendLine("select * from tKanbanPrintTbl where vcPrintflag='1' and vcKBorderno+vcKBSerial in (" + vcKBnoser + ")");
            return getDataTableBySql(sqlget.ToString());
        }


        /// <summary>
        /// 关联数据查看是否存在在打印数据连番表中
        /// </summary>
        public DataTable seaKBSerial_history(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" ");
            strSQL.AppendLine(" select * from [KBSerial_history] where [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "' and [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "'");
            return getDataTableBySql(strSQL.ToString());
        }

        /// <summary>
        /// 检查所打印的看板是否是已经打印状态
        /// </summary>
        public bool IfPrintKB(string vcNo)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select * from tKanbanPrintTbl where iNo='" + vcNo + "'");
            dt = getDataTableBySql(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                if (dt.Rows[0]["vcPrintflag"].ToString() == "1")
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

        /// <summary>
        /// 获取再打印页面中非再打印的数据
        /// </summary>
        public DataTable GetPrintFZData(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial, string vcplantMonth, string vcNo)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT (case when A.vcPrintflagED is not null then A.vcPrintflagED else A.vcPartsNo END) AS vcPartsNo, ");
            strSQL.AppendLine("        B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,(case when A.vcDockED is not null then A.vcDockED else A.vcDock END) AS vcDock,");
            strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
            strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("isnull(A.vcComDate00,'') as vcComDate00,(case when A.vcBanZhi00='1' then '夜' when A.vcBanZhi00='0' then '白' else '' end) as vcBanZhi00, ");
            strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
            strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");
            strSQL.AppendLine(" left join ");
            strSQL.AppendLine(" (select * from tPartInfoMaster) B");
            strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            //strSQL.AppendLine(" where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPartsNo+A.vcDock)=('" + vcKBorderno + vcKBSerial + vcPartsNo.Trim() + vcDock.Trim() + "')");
            strSQL.AppendLine(" where A.iNo='" + vcNo + "'");
            strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')");
            return getDataTableBySql(strSQL.ToString());
        }

        /// <summary>
        /// 更新看板打印表170
        /// </summary>
        public bool UpdatePrintKANB(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.Transaction = trans;
                string vcDateTime = DateTime.Now.ToString();
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strSql = "";
                        string vcKBorderno = dt.Rows[i]["vcorderno"].ToString();
                        string vcKBSerial = dt.Rows[i]["vcKBSerial"].ToString();
                        string vcPartsNo = dt.Rows[i]["vcPartsNo"].ToString();
                        string vcDock = dt.Rows[i]["vcDock"].ToString();
                        //dt.Rows[i]["vcDock"].ToString();
                        string vcPlanMonth = dt.Rows[i]["vcplanMoth"].ToString().Trim();
                        if (printQF(vcPartsNo, vcDock) == "1")
                        {
                            DataTable dts = serachMaster(vcPartsNo, vcDock, vcPlanMonth);
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',vcKBType='1',[vcPrintTime] = GETDATE(),vcPrintflagED='" + dts.Rows[0]["vcPartsNo"].ToString() + "',vcDockED='" + dts.Rows[0]["vcDock"].ToString() + "',vcPrintTimeED=getdate() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        else
                        {
                            strSql = "UPDATE [tKanbanPrintTbl] SET [vcPrintflag] ='1',[vcPrintTime] = GETDATE() where [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and [vcPartsNo]='" + vcPartsNo + "' and [vcDock]='" + vcDock + "'";
                        }
                        if (strSql != "")
                        {
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
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

        public string printQF(string vcpart, string vcdock)
        {
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "select isnull(vcQFflag,'2') as vcQFflag from tPartInfoMaster WHERE vcPartsNo='" + vcpart + "' and  vcDock='" + vcdock + "' ";
            dt = getDataTableBySql(strSQL.ToString());
            if (dt.Rows.Count != 0)
            {
                return dt.Rows[0]["vcQFflag"].ToString();
            }
            else
            {
                return "2";
            }
        }
        public DataTable serachMaster(string vcpart, string vcdock, string vcPlanMonth)
        {
            DataTable dt = new DataTable();
            string strSQL = "";
            strSQL += "select vcPartsNo,vcDock from tPartInfoMaster where vcPartsNo like '" + vcpart.Substring(0, 10).ToString() + "%' and substring(vcPartsNo,11,2)<>'ED' and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcPlanMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcPlanMonth.Replace("-", "") + "')";
            return getDataTableBySql(strSQL.ToString());
        }
        /// <summary>
        /// 订单号连番查找
        /// </summary>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <returns></returns>
        public DataTable isKanBanSea(string vcKBorderno, string vcKBSerial, string vcPartsNo, string vcDock)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" ");
            strSQL.AppendLine(" select * from tKanbanPrintTbl where vcPartsNo='" + vcPartsNo + "' and vcDock='" + vcDock + "' and  [vcKBorderno]='" + vcKBorderno + "' and [vcKBSerial]='" + vcKBSerial + "' and vcPrintflag='1'");
            return getDataTableBySql(strSQL.ToString());
        }

        #region 特殊打印打印FS0170_tPrintTDB
        public DataTable searchPrintTDB(string vcPrintflag, string[] str2)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT '' as iFlag, [vcPartsNo] as vcPartsNo");
            strSQL.AppendLine("     ,[vcDock] as vcDock");
            strSQL.AppendLine("     ,[vcQuantityPerContainer] as vcQuantityPerContainer");
            strSQL.AppendLine("     ,[vcCarType] as vcCarType");
            strSQL.AppendLine("     ,[vcEDflag] as vcEDflag");
            strSQL.AppendLine("     ,[vcKBorderno] as vcKBorderno");
            strSQL.AppendLine("     ,[vcKBSerial] as vcKBSerial");
            strSQL.AppendLine("     ,[vcTips] as vcTips");
            strSQL.AppendLine("     ,[vcComDate01] as vcComDate01");
            strSQL.AppendLine("     ,[vcComDate02] as vcComDate02");
            strSQL.AppendLine("     ,[vcComDate03] as vcComDate03");
            strSQL.AppendLine("     ,[vcComDate04] as vcComDate04");
            strSQL.AppendLine("     ,(Case when [vcBanZhi01]='1' then '夜' when [vcBanZhi01]='0' then '白' else '' end) as vcBanZhi01");
            strSQL.AppendLine("     ,(Case when [vcBanZhi02]='1' then '夜' when [vcBanZhi02]='0' then '白' else '' end) as vcBanZhi02");
            strSQL.AppendLine("     ,(Case when [vcBanZhi03]='1' then '夜' when [vcBanZhi03]='0' then '白' else '' end) as vcBanZhi03");
            strSQL.AppendLine("     ,(Case when [vcBanZhi04]='1' then '夜' when [vcBanZhi04]='0' then '白' else '' end) as vcBanZhi04");
            strSQL.AppendLine("     ,(Case when [vcPrintflag]='1' then '√' else '×' end) as vcPrintFalg");
            strSQL.AppendLine(" FROM [tKanbanPrintTblExcep] where 1=1");
            string flag = "";
            if (str2.Length != 0)
            {
                if (!(str2.Length == 1 && str2[0].ToString() == "000000"))
                {
                    flag += "'";
                    for (int i = 0; i < str2.Length; i++)
                    {
                        flag += str2[i].ToString().Trim();
                        if (i < str2.Length - 1)
                        {
                            flag += "','";
                        }
                        else
                        {
                            flag += "'";
                        }

                    }
                    strSQL.AppendLine(" and vcPorType in( " + flag + ")");
                }
            }

            if (vcPrintflag != "0")
            {
                if (vcPrintflag == "1")
                {
                    strSQL.AppendLine(" and vcPrintflag<>'1'");
                }
                else
                {
                    strSQL.AppendLine(" and vcPrintflag='1'");
                }

            }
            dt = getDataTableBySql(strSQL.ToString());
            return dt;
        }
        #endregion

        #region 特殊打印打印FS0170_tPrintT
        public DataTable searchPrintT()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcPartsNo] as vcPartsNo");
            strSQL.AppendLine("      ,[vcDock] as vcDock");
            strSQL.AppendLine("      ,[vcPorType] as vcPorType");
            strSQL.AppendLine("      ,[vcQuantityPerContainer] as vcQuantityPerContainer");
            strSQL.AppendLine("      ,[vcCarType] as vcCarType");
            strSQL.AppendLine("      ,[vcEDflag] as vcEDflag");
            strSQL.AppendLine("      ,[vcKBorderno] as vcKBorderno");
            strSQL.AppendLine("      ,[vcKBSerial] as vcKBSerial");
            strSQL.AppendLine("      ,[vcTips] as vcTips");
            strSQL.AppendLine("      ,[vcComDate01] as vcComDate01");
            strSQL.AppendLine("      ,[vcComDate02] as vcComDate02");
            strSQL.AppendLine("      ,[vcComDate03] as vcComDate03");
            strSQL.AppendLine("      ,[vcComDate04] as vcComDate04");
            strSQL.AppendLine("      ,[vcBanZhi01] as vcBanZhi01");
            strSQL.AppendLine("      ,[vcBanZhi02] as vcBanZhi02");
            strSQL.AppendLine("      ,[vcBanZhi03] as vcBanZhi03");
            strSQL.AppendLine("      ,[vcBanZhi04] as vcBanZhi04,'' as iFlag, '0' as vcModFlag,'0' as vcAddFlag,iAutoId");
            strSQL.AppendLine("  FROM [tKanbanPrintTblExcep]");
            dt = getDataTableBySql(strSQL.ToString());
            return dt;
        }

        public bool InUpdeOldData(DataTable dt, string useid)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.Transaction = trans;

                //DataRow[] rows = dt.Select("iFlag='insert' or iFlag='update' or iFlag='delete'");
                DataRow[] rows = dt.Select("1=1");
                string vcDateTime = System.DateTime.Now.ToString();
                try
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow rowdselect = rows[i];
                        string strSqlIn = "";
                        string strSqlUp = "";
                        string strSqlDe = "";
                        string vcPartsNo = rowdselect["vcPartsNo"].ToString();
                        string vcDock = rowdselect["vcDock"].ToString();
                        string vcPorType = rowdselect["vcPorType"].ToString();
                        string vcQuantityPerContainer = rowdselect["vcQuantityPerContainer"].ToString();
                        string vcCarType = rowdselect["vcCarType"].ToString();
                        string vcEDflag = rowdselect["vcEDflag"].ToString();
                        string vcKBorderno = rowdselect["vcKBorderno"].ToString();
                        string vcKBSerial = rowdselect["vcKBSerial"].ToString();
                        string vcTips = rowdselect["vcTips"].ToString();
                        string vcComDate01 = rowdselect["vcComDate01"].ToString();
                        string vcComDate02 = rowdselect["vcComDate02"].ToString();
                        string vcComDate03 = rowdselect["vcComDate03"].ToString();
                        string vcComDate04 = rowdselect["vcComDate04"].ToString();
                        string vcBanZhi01 = rowdselect["vcBanZhi01"].ToString();
                        string vcBanZhi02 = rowdselect["vcBanZhi02"].ToString();
                        string vcBanZhi03 = rowdselect["vcBanZhi03"].ToString();
                        string vcBanZhi04 = rowdselect["vcBanZhi04"].ToString();
                        string vcPrintflag = "";
                        //string flag = rowdselect["iFlag"].ToString();
                        //if (flag == "insert")
                        //{
                            strSqlIn += "INSERT INTO [tKanbanPrintTblExcep]";
                            strSqlIn += "           ([vcPartsNo]";
                            strSqlIn += "           ,[vcDock]";
                            strSqlIn += "           ,[vcPorType]";
                            strSqlIn += "           ,[vcQuantityPerContainer]";
                            strSqlIn += "           ,[vcCarType]";
                            strSqlIn += "           ,[vcEDflag]";
                            strSqlIn += "           ,[vcKBorderno]";
                            strSqlIn += "           ,[vcKBSerial]";
                            strSqlIn += "           ,[vcTips]";
                            strSqlIn += "           ,[vcComDate01]";
                            strSqlIn += "           ,[vcComDate02]";
                            strSqlIn += "           ,[vcComDate03]";
                            strSqlIn += "           ,[vcComDate04]";
                            strSqlIn += "           ,[vcBanZhi01]";
                            strSqlIn += "           ,[vcBanZhi02]";
                            strSqlIn += "           ,[vcBanZhi03]";
                            strSqlIn += "           ,[vcBanZhi04]";
                            strSqlIn += "           ,[vcPrintflag]";
                            strSqlIn += "           ,[dCreatTime]";
                            strSqlIn += "           ,[vcCreater]";
                            strSqlIn += "           ,[dUpdateTime]";
                            strSqlIn += "           ,[vcUpdater])";
                            strSqlIn += "     VALUES";
                            strSqlIn += "           ('" + vcPartsNo + "'";
                            strSqlIn += "            ,'" + vcDock + "'";
                            strSqlIn += "            ,'" + vcPorType + "'";
                            strSqlIn += "            ,'" + vcQuantityPerContainer + "'";
                            strSqlIn += "            ,'" + vcCarType + "'";
                            strSqlIn += "            ,'" + vcEDflag + "'";
                            strSqlIn += "            ,'" + vcKBorderno + "'";
                            strSqlIn += "            ,'" + vcKBSerial + "'";
                            strSqlIn += "            ,'" + vcTips + "'";
                            strSqlIn += "            ,'" + vcComDate01 + "'";
                            strSqlIn += "            ,'" + vcComDate02 + "'";
                            strSqlIn += "            ,'" + vcComDate03 + "'";
                            strSqlIn += "            ,'" + vcComDate04 + "'";
                            strSqlIn += "            ,'" + vcBanZhi01 + "'";
                            strSqlIn += "            ,'" + vcBanZhi02 + "'";
                            strSqlIn += "            ,'" + vcBanZhi03 + "'";
                            strSqlIn += "            ,'" + vcBanZhi04 + "'";
                            strSqlIn += "            ,'" + vcPrintflag + "'";
                            strSqlIn += "            ,getdate()";
                            strSqlIn += "            ,'" + useid + "'";
                            strSqlIn += "            ,getdate()";
                            strSqlIn += "            ,'" + useid + "')";
                        //}
                        //else
                        //    if (flag == "update")
                        //{
                        //    strSqlUp += "UPDATE [tKanbanPrintTblExcep]";
                        //    strSqlUp += "   SET [vcQuantityPerContainer] ='" + vcQuantityPerContainer + "'";
                        //    strSqlUp += "      ,[vcPorType] ='" + vcPorType + "'";
                        //    strSqlUp += "      ,[vcCarType] ='" + vcCarType + "'";
                        //    strSqlUp += "      ,[vcEDflag] ='" + vcEDflag + "'";
                        //    strSqlUp += "      ,[vcTips] ='" + vcTips + "'";
                        //    strSqlUp += "      ,[vcComDate01] ='" + vcComDate01 + "'";
                        //    strSqlUp += "      ,[vcComDate02] ='" + vcComDate02 + "'";
                        //    strSqlUp += "      ,[vcComDate03] ='" + vcComDate03 + "'";
                        //    strSqlUp += "      ,[vcComDate04] ='" + vcComDate04 + "'";
                        //    strSqlUp += "      ,[vcBanZhi01] ='" + vcBanZhi01 + "'";
                        //    strSqlUp += "      ,[vcBanZhi02] ='" + vcBanZhi02 + "'";
                        //    strSqlUp += "      ,[vcBanZhi03] ='" + vcBanZhi03 + "'";
                        //    strSqlUp += "      ,[vcBanZhi04] ='" + vcBanZhi04 + "'";
                        //    strSqlUp += "      ,[dUpdateTime] =getdate()";
                        //    strSqlUp += "      ,[vcUpdater] = '" + useid + "'";
                        //    strSqlUp += "WHERE [vcPartsNo] ='" + vcPartsNo + "'";
                        //    strSqlUp += "      AND [vcDock] ='" + vcDock + "'";
                        //    strSqlUp += "      AND [vcKBorderno] ='" + vcKBorderno + "'";
                        //    strSqlUp += "      AND [vcKBSerial] ='" + vcKBSerial + "'";
                        //    strSqlUp += "";
                        //}
                        //else
                        //        if (flag == "delete")
                        //{
                        //    strSqlDe += "DELETE FROM [tKanbanPrintTblExcep]";
                        //    strSqlDe += "       WHERE  [vcPartsNo]='" + vcPartsNo + "' AND  [vcDock]='" + vcDock + "' AND [vcKBorderno]='" + vcKBorderno + "' AND [vcKBSerial]='" + vcKBSerial + "'";
                        //}
                        if (strSqlIn != "")
                        {
                            cmd.CommandText = strSqlIn;
                            cmd.ExecuteNonQuery();
                        }
                        if (strSqlUp != "")
                        {
                            cmd.CommandText = strSqlUp;
                            cmd.ExecuteNonQuery();
                        }
                        if (strSqlDe != "")
                        {
                            cmd.CommandText = strSqlDe;
                            cmd.ExecuteNonQuery();
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
        /// 更新时新增数据重复性判断
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool partChongFu(DataTable dt)
        {
            StringBuilder strSQL = new StringBuilder();
            //DataRow[] rows = dt.Select("iFlag='insert'");
            DataRow[] rows = dt.Select("1=1");
            if (rows.Length != 0)
            {
                string partfrom = "";
                partfrom += "'";
                for (int i = 0; i < rows.Length; i++)
                {
                    DataRow rowdelete = rows[i];
                    partfrom += rowdelete["vcPartsNo"].ToString() + rowdelete["vcDock"].ToString() + rowdelete["vcKBorderno"].ToString() + rowdelete["vcKBSerial"].ToString();
                    if (i < rows.Length - 1)
                    {
                        partfrom += "','";
                    }
                    else
                    {
                        partfrom += "'";
                    }
                }
                strSQL.AppendLine("SELECT top(1) *  FROM [tKanbanPrintTblExcep] where [vcPartsNo]+[vcDock]+[vcKBorderno]+[vcKBSerial] in (" + partfrom + ")");
                DataTable ds = getDataTableBySql(strSQL.ToString());
                if (ds.Rows.Count == 0)
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
                return true;
            }
        }
        #endregion

        public DataTable dllPorType()
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("  select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
            return getDataTableBySql(strSQL.ToString());
        }

        public DataTable dtKBSerial_history(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerialBefore)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [vcPartsNo]");
            strSQL.AppendLine("      ,[vcDock]");
            strSQL.AppendLine("      ,[vcKBorderno]");
            strSQL.AppendLine("      ,[vcKBSerial]");
            strSQL.AppendLine("  FROM [KBSerial_history]");
            strSQL.AppendLine(" WHERE vcPartsNo='" + vcPartsNo + "'");
            strSQL.AppendLine("       AND vcDock='" + vcDock + "'");
            strSQL.AppendLine("       AND vcKBorderno='" + vcKBorderno + "'");
            strSQL.AppendLine("       AND vcKBSerialBefore='" + vcKBSerialBefore + "'");
            return getDataTableBySql(strSQL.ToString());
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
            if (getDataTableBySql(strSQL.ToString()).Rows.Count != 0)
            {
                return getDataTableBySql(strSQL.ToString()).Rows[0]["vcKBSerialBefore"].ToString();
            }
            else
            {
                return "";
            }
        }
        public string dtMasteriQuantity(string vcPartsNo, string vcDock, string vcplantMonth)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT [iQuantityPerContainer]");
            strSQL.AppendLine("  FROM [tPartInfoMaster]");
            strSQL.AppendLine("  WHERE [vcPartsNo]='" + vcPartsNo + "'");
            strSQL.AppendLine("  AND   [vcDock]='" + vcDock + "'");
            strSQL.AppendLine("  and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')");
            if (getDataTableBySql(strSQL.ToString()).Rows.Count != 0)
            {
                return getDataTableBySql(strSQL.ToString()).Rows[0]["iQuantityPerContainer"].ToString();
            }
            else
            {
                return "9999";
            }
        }

        /// <summary>
        /// 判断在打印的打印类型 |秦丰ED、秦丰非ED、非秦丰|
        /// </summary>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <param name="vcPlanMonth"></param>
        /// <returns></returns>
        public DataTable QFED00QuFen(string vcPartsNo, string vcDock, string vcKBorderno, string vcKBSerial, string vcPlanMonth, string vcNo)
        {
            StringBuilder strSQL = new StringBuilder();
            string partfrom = vcKBorderno + vcKBSerial + vcPartsNo.ToString().Replace("-", "") + vcDock;
            strSQL.AppendLine("select * from tKanbanPrintTbl where iNo='" + vcNo + "'");
            //strSQL.AppendLine("    WHERE ([vcKBorderno]+[vcKBSerial]+vcPartsNo+vcDock) in ('" + partfrom + "')");
            //strSQL.AppendLine("    OR    ([vcKBorderno]+[vcKBSerial]+vcPrintflagED+vcDockED) in ('" + partfrom + "') and vcPlanMonth='" + vcPlanMonth + "'");
            return getDataTableBySql(strSQL.ToString());
        }

        /// <summary>
        /// 秦丰ED和非秦丰的看板再发行
        /// </summary>
        /// <param name="vcPartsNo"></param>
        /// <param name="vcDock"></param>
        /// <param name="vcplantMonth"></param>
        /// <param name="vcKBorderno"></param>
        /// <param name="vcKBSerial"></param>
        /// <returns></returns>
        public DataTable rePrintData(string vcPartsNo, string vcDock, string vcplantMonth, string vcKBorderno, string vcKBSerial, string vcNo)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("SELECT (case when A.vcPrintflagED is not null then A.vcPrintflagED else A.vcPartsNo END) AS vcPartsNo, ");
            strSQL.AppendLine("        B.vcSupplierCode,b.vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno,(case when A.vcDockED is not null then A.vcDockED else A.vcDock END) AS vcDock,");
            strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute, ");
            strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer,");
            strSQL.AppendLine("isnull(A.vcComDate00,'') as vcComDate00,(case when A.vcBanZhi00='1' then '夜' when A.vcBanZhi00='0' then '白' else '' end) as vcBanZhi00, ");
            strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
            strSQL.AppendLine(" (select * from tKanbanPrintTbl) A");
            strSQL.AppendLine(" left join ");
            strSQL.AppendLine(" (select * from tPartInfoMaster) B");
            strSQL.AppendLine(" on A.vcPartsNo=B.vcPartsNo and   A.vcDock=B.vcDock");
            //strSQL.AppendLine(" where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPartsNo+A.vcDock)=('" + vcKBorderno + vcKBSerial + vcPartsNo.Trim() + vcDock.Trim() + "')");
            strSQL.AppendLine(" where A.iNo='" + vcNo + "'");
            strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth.Replace("-", "") + "')");
            return getDataTableBySql(strSQL.ToString());
        }

        #region 适用FS170的看板打印 打印秦丰非ED的看板
        public DataTable rePrintDataED(string vcPartsNo, string vcDock, string vcplantMonth, string vcKBorderno, string vcKBSerial, string vcNo, string vcCarFamilyCode)
        {
            StringBuilder strSQL = new StringBuilder();
            //获取白件相应黑件在Master中的车型信息
            //string vcCarType = BreakCarType(vcPartsNo, vcDock, vcplantMonth);
            strSQL.AppendLine("SELECT  A.vcPrintflagED AS vcPartsNo,A.vcDockED AS vcDock, ");
            strSQL.AppendLine("        B.vcSupplierCode,'vcSupplierPlant' as vcSupplierPlant, B.vcCpdCompany,A.vcCarType AS vcCarFamilyCode,A.vcKBorderno, ");
            strSQL.AppendLine("        A.vcEDflag,B.vcPartsNameEN,B.vcPartsNameCHN,B.vcLogisticRoute,   ");
            strSQL.AppendLine("        A.vcQuantityPerContainer as iQuantityPerContainer, ");
            strSQL.AppendLine("isnull(A.vcComDate00,'') as vcComDate00,(case when A.vcBanZhi00='1' then '夜' when A.vcBanZhi00='0' then '白' else '' end) as vcBanZhi00, ");
            strSQL.AppendLine("isnull(A.vcProject01,'') as vcProject01,isnull(A.vcComDate01,'') as vcComDate01,(case when A.vcBanZhi01='1' then '夜' when A.vcBanZhi01='0' then '白' else '' end) as vcBanZhi01,isnull(A.vcAB01,'') as vcAB01, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject02,'') as vcProject02,isnull(A.vcComDate02,'') as vcComDate02,(case when A.vcBanZhi02='1' then '夜' when A.vcBanZhi02='0' then '白' else '' end) as vcBanZhi02,isnull(A.vcAB02,'') as vcAB02, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject03,'') as vcProject03,isnull(A.vcComDate03,'') as vcComDate03,(case when A.vcBanZhi03='1' then '夜' when A.vcBanZhi03='0' then '白' else '' end) as vcBanZhi03,isnull(A.vcAB03,'') as vcAB03, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(A.vcProject04,'') as vcProject04,isnull(A.vcComDate04,'') as vcComDate04,(case when A.vcBanZhi04='1' then '夜' when A.vcBanZhi04='0' then '白' else '' end) as vcBanZhi04,isnull(A.vcAB04,'') as vcAB04, ");//20181010添加AB值信息 - 李兴旺
            strSQL.AppendLine("isnull(B.vcRemark1,'') as vcRemark1,isnull(B.vcRemark2,'') as vcRemark2,A.vcKBSerial,B.vcPhotoPath,B.vcPorType from ");
            strSQL.AppendLine("  (select * from tKanbanPrintTbl) A ");
            strSQL.AppendLine(" left join ");
            strSQL.AppendLine(" (select * from tPartInfoMaster) B");
            strSQL.AppendLine("  on A.vcPrintflagED=B.vcPartsNo");
            //strSQL.AppendLine("  where (A.[vcKBorderno]+A.[vcKBSerial]+A.vcPrintflagED)=('" + vcKBorderno + vcKBSerial + vcPartsNo.Trim() + "')");
            strSQL.AppendLine(" where A.iNo='" + vcNo + "'");
            strSQL.AppendLine(" and (Convert(varchar(6),(CONVERT(datetime,B.dTimeFrom,101)),112)<='" + vcplantMonth + "' and Convert(varchar(6),(CONVERT(datetime,B.dTimeTo,101)),112)>='" + vcplantMonth + "')  and B.vcCarFamilyCode='" + vcCarFamilyCode + "'");
            DataTable dtreturn = getDataTableBySql(strSQL.ToString());
            if (dtreturn.Rows.Count != 0)
            {
                if (dtreturn.Rows[0]["vcSupplierPlant"].ToString() == "vcSupplierPlant")
                {
                    string SupplPlant = BreakSupplPlant(vcPartsNo, vcDock, vcplantMonth);
                    dtreturn.Rows[0]["vcSupplierPlant"] = SupplPlant;
                }
            }
            return dtreturn;
        }
        #endregion

        //获取供应商工区
        private string BreakSupplPlant(string vcPartsNo, string vcDock, string vcPlanMonth)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartsNo,vcDock from tKanbanPrintTbl  where vcPrintflagED='" + vcPartsNo + "' and vcDockED='" + vcDock + "' and vcPlanMonth='" + vcPlanMonth + "'");
            DataTable dt = getDataTableBySql(strSQL.ToString());
            StringBuilder strSQL1 = new StringBuilder();
            strSQL1.AppendLine("select vcSupplierPlant from tPartInfoMaster where vcPartsNo='" + dt.Rows[0]["vcPartsNo"] + "' and vcDock='" + dt.Rows[0]["vcDock"] + "' and (Convert(varchar(6),(CONVERT(datetime,dTimeFrom,101)),112)<='" + vcPlanMonth.Replace("-", "") + "' and Convert(varchar(6),(CONVERT(datetime,dTimeTo,101)),112)>='" + vcPlanMonth.Replace("-", "") + "')");
            DataTable dt1 = getDataTableBySql(strSQL1.ToString());
            return dt1.Rows[0]["vcSupplierPlant"].ToString();
        }

        /// <summary>
        /// 插入看板打印临时表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool insertTableCR(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableCR00", connection, trans);
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

                    cmdln.Parameters.Add("@vcComDate00", SqlDbType.VarChar, 50, "vcComDate00");
                    cmdln.Parameters.Add("@vcBanZhi00", SqlDbType.VarChar, 50, "vcBanZhi00");
                    cmdln.Parameters.Add("@vcAB01", SqlDbType.VarChar, 10, "vcAB01");//20181010添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB02", SqlDbType.VarChar, 10, "vcAB02");//20181010添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB03", SqlDbType.VarChar, 10, "vcAB03");//20181010添加AB值信息 - 李兴旺
                    cmdln.Parameters.Add("@vcAB04", SqlDbType.VarChar, 10, "vcAB04");//20181010添加AB值信息 - 李兴旺
                    SqlDataAdapter adaln = new SqlDataAdapter();
                    adaln.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    adaln.Update(dt);
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
        public bool insertTableExcel00(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlCommand cmdln = new SqlCommand("PK_PS0160_InsertTableExcel00", connection, trans);
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
                    cmdln.Parameters.Add("@vcComDate00", SqlDbType.VarChar, 50, "vcComDate00");
                    cmdln.Parameters.Add("@vcBanZhi00", SqlDbType.VarChar, 50, "vcBanZhi00");
                    SqlDataAdapter adaln = new SqlDataAdapter();
                    adaln.InsertCommand = cmdln;
                    cmdln.CommandTimeout = 0;
                    adaln.Update(dt);
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

        public DataSet PrintExcel(string vcPorType, string vcorderno, string vcComDate01, string vcBanZhi01, string vcComDate00, string vcBanZhi00)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select '' as no,(case when SUBSTRING(a.vcpartsNo,11,2)='00' then SUBSTRING(a.vcpartsNo,1,5)+'-'+SUBSTRING(a.vcpartsNo,6,5) else SUBSTRING(a.vcpartsNo,1,5)+'-'+SUBSTRING(a.vcpartsNo,6,5)+'-'+ SUBSTRING(a.vcpartsNo,11,2) end)  as vcpartsNo,a.vcCarFamlyCode as vcCarFamlyCode,a.vcPartsNameCHN as vcPartsNameCHN,");
            strSQL.AppendLine("       a.vcPCB01 as vcPCB01,a.meishu as meishu,a.minal+'-'+a.maxal as vcKBSerial from (");
            strSQL.AppendLine("select vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01,");
            strSQL.AppendLine("       COUNT(iQuantityPerContainer) as meishu,");
            strSQL.AppendLine("       MAX(vcKBSerial) as maxal,");
            strSQL.AppendLine("       MIN(vcKBSerial) as minal from [testprinterExcel] where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "' and vcComDate00='" + vcComDate00 + "' and vcBanZhi00='" + vcBanZhi00 + "'");
            strSQL.AppendLine("       group by vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01");
            strSQL.AppendLine("       )a order by vcKBSerial");

            strSQL.AppendLine("select B.vcPCB01,B.meishu AS meishu, B.minal+'-'+B.maxal AS vcKBSerial,ROW_NUMBER() over(order by  minal) as aaa  FROM ");
            strSQL.AppendLine("       (select vcPCB01, ");
            strSQL.AppendLine("              COUNT(iQuantityPerContainer) as meishu, ");
            strSQL.AppendLine("              MAX(vcKBSerial) as maxal, ");
            strSQL.AppendLine("              MIN(vcKBSerial) as minal ");
            strSQL.AppendLine("         from [testprinterExcel]  where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "' and vcComDate00='" + vcComDate00 + "' and vcBanZhi00='" + vcBanZhi00 + "'  ");
            strSQL.AppendLine("              group by vcPCB01 )B");
            strSQL.AppendLine("       UNION all");
            strSQL.AppendLine("       (");
            strSQL.AppendLine("select '合计' as vcPCB01,B.meishu AS meishu, B.minal+'-'+B.maxal AS vcKBSerial,''  FROM ");
            strSQL.AppendLine("       (select  ");
            strSQL.AppendLine("              COUNT(iQuantityPerContainer) as meishu, ");
            strSQL.AppendLine("              MAX(vcKBSerial) as maxal, ");
            strSQL.AppendLine("              MIN(vcKBSerial) as minal   ");
            strSQL.AppendLine("         from [testprinterExcel]   where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "' and vcComDate00='" + vcComDate00 + "' and vcBanZhi00='" + vcBanZhi00 + "' ");
            strSQL.AppendLine("              ) B  )");
            return excute.ExcuteSqlWithSelectToDS(strSQL.ToString());
        }

        public DataSet aPrintExcel(string vcPorType, string vcorderno, string vcComDate00, string vcBanZhi00, string vcComDate01, string vcBanZhi01)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select '' as no,(case when SUBSTRING(a.vcpartsNo,11,2)='00' then SUBSTRING(a.vcpartsNo,1,5)+'-'+SUBSTRING(a.vcpartsNo,6,5) else SUBSTRING(a.vcpartsNo,1,5)+'-'+SUBSTRING(a.vcpartsNo,6,5)+'-'+ SUBSTRING(a.vcpartsNo,11,2) end)  as vcpartsNo,a.vcCarFamlyCode as vcCarFamlyCode,a.vcPartsNameCHN as vcPartsNameCHN,");
            strSQL.AppendLine("       a.vcPCB01 as vcPCB01,a.meishu as meishu,a.minal+'-'+a.maxal as vcKBSerial from (");
            strSQL.AppendLine("select vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01,");
            strSQL.AppendLine("       COUNT(iQuantityPerContainer) as meishu,");
            strSQL.AppendLine("       MAX(vcKBSerial) as maxal,");
            strSQL.AppendLine("       MIN(vcKBSerial) as minal from [testprinterExcel1] where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "'");
            strSQL.AppendLine("       group by vcpartsNo,vcCarFamlyCode,vcPartsNameCHN,vcPCB01");
            strSQL.AppendLine("       ) a order by vcKBSerial");

            strSQL.AppendLine("select B.vcPCB01,B.meishu AS meishu, B.minal+'-'+B.maxal AS vcKBSerial,ROW_NUMBER() over(order by  minal) as aaa  FROM ");
            strSQL.AppendLine("       (select vcPCB01, ");
            strSQL.AppendLine("              COUNT(iQuantityPerContainer) as meishu, ");
            strSQL.AppendLine("              MAX(vcKBSerial) as maxal, ");
            strSQL.AppendLine("              MIN(vcKBSerial) as minal ");
            strSQL.AppendLine("         from [testprinterExcel1]  where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "'");
            strSQL.AppendLine("              group by vcPCB01 ) B");
            strSQL.AppendLine("       UNION all");
            strSQL.AppendLine("       (");
            strSQL.AppendLine("select '合计' as vcPCB01,B.meishu AS meishu, B.minal+'-'+B.maxal AS vcKBSerial,''  FROM ");
            strSQL.AppendLine("       (select  ");
            strSQL.AppendLine("              COUNT(iQuantityPerContainer) as meishu, ");
            strSQL.AppendLine("              MAX(vcKBSerial) as maxal, ");
            strSQL.AppendLine("              MIN(vcKBSerial) as minal   ");
            strSQL.AppendLine("         from [testprinterExcel1]   where vcKBorderno='" + vcorderno + "' and vcPorType='" + vcPorType + "' and vcPCB02='" + vcComDate01 + "' and vcPCB03='" + vcBanZhi01 + "' ");
            strSQL.AppendLine("              ) B  )");
            return excute.ExcuteSqlWithSelectToDS(strSQL.ToString());
        }

        public DataTable SearchRePrintKBQR(string OrderNo, string GC, string PlanPrintDate, string PlanPrintBZ, string PlanProcDate, string PlanProcBZ, string PrintDate)
        {
            DataTable dtReturn = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append("SELECT [iautoid] ,[vcOrderNo]");
            strSQL.Append(",(Case when [vcGC] = '0' then '' else [vcGC] end) as vcGC");
            strSQL.Append(",Convert(varchar(20),[vcPlanPrintDate], 120) as vcPlanPrintDate");
            strSQL.Append(",(Case when [vcPlanPrintBZ] = '0' then '白值' when [vcPlanPrintBZ] = '1' then '夜值' else '' end) as vcPlanPrintBZ");
            strSQL.Append(",Convert(varchar(20),[vcPlanProcDate], 120) as vcPlanProcDate");
            strSQL.Append(",(Case when [vcPlanProcBZ] = '0' then '白值' when [vcPlanProcBZ] = '1' then '夜值' else '' end) as vcPlanProcBZ");
            strSQL.Append(",Convert(varchar(20),[vcPrintDate], 120) as vcPrintDate");
            strSQL.Append("  FROM tKanBanQrTbl");
            strSQL.Append("  where 1=1 ");
            if (OrderNo.Trim() != "")
            {
                strSQL.Append("        and  vcOrderNo like '" + OrderNo + "%' ");
            }
            if (GC.Trim() != "")
            {
                strSQL.Append("        and vcGC = '" + GC + "' ");
            }
            if (PlanPrintDate.Trim() != "")
            {
                strSQL.Append("        and vcPlanPrintDate = '" + PlanPrintDate + "' ");
            }
            if (PlanPrintBZ.Trim() != "")
            {
                strSQL.Append("        and vcPlanPrintBZ = '" + PlanPrintBZ + "'");
            }
            if (PlanProcDate.Trim() != "")
            {
                strSQL.Append("        and  vcPlanProcDate = '" + PlanProcDate + "' ");
            }
            if (PlanProcBZ.Trim() != "")
            {
                strSQL.Append("        and  vcPlanProcBZ = '" + PlanProcBZ + "' ");
            }
            if (PrintDate.Trim() != "")
            {
                strSQL.Append("        and  vcPrintDate = '" + PrintDate + "' ");
            }
            dtReturn = getDataTableBySql(strSQL.ToString());
            return dtReturn;
        }

        #region 删除（特殊打印录入）
        public void Del(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM tKanbanPrintTblExcep where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
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
    }
}
