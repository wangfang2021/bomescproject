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
    public class FS1502_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string dBZDate, string vcBigPM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag,iLJBZRemain as iLJBZRemain_old,dPackDate from (    \n");
                strSql.Append("select vcBigPM,vcSmallPM,iBZPlan_Day,iBZPlan_Night,iBZPlan_Heji,iEmergencyOrder,iLJBZRemain,iPlanTZ,    \n");
                strSql.Append("iSSPlan_Day,iSSPlan_Night,iSSPlan_Heji,1 as id,'0' as bSelectFlag,iAutoId,dPackDate    \n");
                strSql.Append("from TPackingPlan_Summary where dPackDate='" + dBZDate + "' and vcInOutType='外注'     \n");
                if (vcBigPM != null && vcBigPM != "")
                    strSql.Append("and vcBigPM = '" + vcBigPM + "'    \n");
                strSql.Append("union all    \n");
                strSql.Append("select '外注品合计' as vcBigPM,null as vcSmallPM,null as iBZPlan_Day,null as iBZPlan_Night,null as iBZPlan_Heji,    \n");
                strSql.Append("null as iEmergencyOrder,null as iLJBZRemain,null as iPlanTZ,    \n");
                strSql.Append("sum(iSSPlan_Day),sum(iSSPlan_Night),sum(iSSPlan_Heji),2 as id,'1' as bSelectFlag,0 as iAutoId,'" + dBZDate + "' as dPackDate    \n");
                strSql.Append("from TPackingPlan_Summary where dPackDate='" + dBZDate + "' and vcInOutType='外注'    \n");
                strSql.Append("union all     \n");
                strSql.Append("select vcBigPM,vcSmallPM,iBZPlan_Day,iBZPlan_Night,iBZPlan_Heji,iEmergencyOrder,iLJBZRemain,iPlanTZ,    \n");
                strSql.Append("iSSPlan_Day,iSSPlan_Night,iSSPlan_Heji,3 as id,'0' as bSelectFlag,iAutoId,dPackDate    \n");
                strSql.Append("from TPackingPlan_Summary where dPackDate='" + dBZDate + "' and vcInOutType='内制'    \n");
                if (vcBigPM != null && vcBigPM != "")
                    strSql.Append("and vcBigPM = '" + vcBigPM + "'    \n");
                strSql.Append("union all    \n");
                strSql.Append("select '内制品合计' as vcBigPM,null as vcSmallPM,null as iBZPlan_Day,null as iBZPlan_Night,null as iBZPlan_Heji,    \n");
                strSql.Append("null as iEmergencyOrder,null as iLJBZRemain,null as iPlanTZ,    \n");
                strSql.Append("sum(iSSPlan_Day),sum(iSSPlan_Night),sum(iSSPlan_Heji),4 as id,'1' as bSelectFlag,0 as iAutoId,'" + dBZDate + "' as dPackDate    \n");
                strSql.Append("from TPackingPlan_Summary where dPackDate='" + dBZDate + "' and vcInOutType='内制'    \n");
                strSql.Append("union all    \n");
                strSql.Append("select '全体合计' as vcBigPM,null as vcSmallPM,null as iBZPlan_Day,null as iBZPlan_Night,null as iBZPlan_Heji,    \n");
                strSql.Append("null as iEmergencyOrder,null as iLJBZRemain,null as iPlanTZ,    \n");
                strSql.Append("sum(iSSPlan_Day),sum(iSSPlan_Night),sum(iSSPlan_Heji),5 as id,'1' as bSelectFlag,0 as iAutoId,'" + dBZDate + "' as dPackDate    \n");
                strSql.Append("from TPackingPlan_Summary where dPackDate='" + dBZDate + "'     \n");
                strSql.Append(")a    \n");
                strSql.Append("order by id,vcBigPM,vcSmallPM      \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索_报表
        public DataTable Search_report(string dBZDate)
        {
            try
            {
                string vcYearMonth = dBZDate.Substring(0, 4) + dBZDate.Substring(5, 2);
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select *,ROW_NUMBER() over(order by id,vcBigPM,vcSmallPM) as xuhao from (");
                strSql.AppendLine("select vcKind,vcBigPM,vcSmallPM,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,");
                strSql.AppendLine("iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31,1 as id");
                strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='纳入计划'");
                strSql.AppendLine("union all");
                //strSql.AppendLine("select vcKind,'合计' as vcBigPM,'' as vcSmallPM,sum(iD1),sum(iD2),sum(iD3),sum(iD4),sum(iD5),");
                //strSql.AppendLine("sum(iD6),sum(iD7),sum(iD8),sum(iD9),sum(iD10),sum(iD11),sum(iD12),sum(iD13),sum(iD14),sum(iD15),");
                //strSql.AppendLine("sum(iD16),sum(iD17),sum(iD18),sum(iD19),sum(iD20),sum(iD21),sum(iD22),sum(iD23),sum(iD24),sum(iD25),");
                //strSql.AppendLine("sum(iD26),sum(iD27),sum(iD28),sum(iD29),sum(iD30),sum(iD31),2 as id");
                //strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='纳入计划'");
                //strSql.AppendLine("group by vcKind");
                //strSql.AppendLine("union all");
                strSql.AppendLine("select vcKind,vcBigPM,vcSmallPM,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,");
                strSql.AppendLine("iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31,3 as id");
                strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='当日实际'");
                strSql.AppendLine("union all");
                //strSql.AppendLine("select vcKind,'合计' as vcBigPM,'' as vcSmallPM,sum(iD1),sum(iD2),sum(iD3),sum(iD4),sum(iD5),");
                //strSql.AppendLine("sum(iD6),sum(iD7),sum(iD8),sum(iD9),sum(iD10),sum(iD11),sum(iD12),sum(iD13),sum(iD14),sum(iD15),");
                //strSql.AppendLine("sum(iD16),sum(iD17),sum(iD18),sum(iD19),sum(iD20),sum(iD21),sum(iD22),sum(iD23),sum(iD24),sum(iD25),");
                //strSql.AppendLine("sum(iD26),sum(iD27),sum(iD28),sum(iD29),sum(iD30),sum(iD31),4 as id");
                //strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='当日实际'");
                //strSql.AppendLine("group by vcKind");
                //strSql.AppendLine("union all");
                strSql.AppendLine("select vcKind,vcBigPM,vcSmallPM,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,");
                strSql.AppendLine("iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31,5 as id");
                strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='当日残'");
                strSql.AppendLine("union all");
                //strSql.AppendLine("select vcKind,'合计' as vcBigPM,'' as vcSmallPM,sum(iD1),sum(iD2),sum(iD3),sum(iD4),sum(iD5),");
                //strSql.AppendLine("sum(iD6),sum(iD7),sum(iD8),sum(iD9),sum(iD10),sum(iD11),sum(iD12),sum(iD13),sum(iD14),sum(iD15),");
                //strSql.AppendLine("sum(iD16),sum(iD17),sum(iD18),sum(iD19),sum(iD20),sum(iD21),sum(iD22),sum(iD23),sum(iD24),sum(iD25),");
                //strSql.AppendLine("sum(iD26),sum(iD27),sum(iD28),sum(iD29),sum(iD30),sum(iD31),6 as id");
                //strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='当日残'");
                //strSql.AppendLine("group by vcKind");
                //strSql.AppendLine("union all");
                strSql.AppendLine("select vcKind,vcBigPM,vcSmallPM,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,");
                strSql.AppendLine("iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31,7 as id");
                strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='累计残量'");
                strSql.AppendLine("union all");
                strSql.AppendLine("select vcKind,'合计' as vcBigPM,'' as vcSmallPM,sum(iD1),sum(iD2),sum(iD3),sum(iD4),sum(iD5),");
                strSql.AppendLine("sum(iD6),sum(iD7),sum(iD8),sum(iD9),sum(iD10),sum(iD11),sum(iD12),sum(iD13),sum(iD14),sum(iD15),");
                strSql.AppendLine("sum(iD16),sum(iD17),sum(iD18),sum(iD19),sum(iD20),sum(iD21),sum(iD22),sum(iD23),sum(iD24),sum(iD25),");
                strSql.AppendLine("sum(iD26),sum(iD27),sum(iD28),sum(iD29),sum(iD30),sum(iD31),8 as id");
                strSql.AppendLine("from TPackingPlan_Report where vcYearMonth='"+ vcYearMonth + "' and vcKind='累计残量'");
                strSql.AppendLine("group by vcKind");
                strSql.AppendLine(")t");
                strSql.AppendLine("order by id,vcBigPM,vcSmallPM");

                #region not use
                //strSql.Append("select '纳入计划' as kind,t1.dt,t2.vcBigPM,isnull(t3.iBZPlan_Heji,0) as heji     \n");
                //strSql.Append("from (    \n");
                //strSql.Append("	--某个月所有天    \n");
                //strSql.Append("	select convert(varchar(10),dateadd(dd,number,'" + dBZDate.Substring(0, 7) + "-01'),120) as dt    \n");
                //strSql.Append("	from master..spt_values    \n");
                //strSql.Append("	where type='P'    \n");
                //strSql.Append("	and dateadd(dd,number,'" + dBZDate.Substring(0, 7) + "-01')<=    \n");
                //strSql.Append("	dateadd(dd,-1,convert(varchar(8),dateadd(mm,1,'" + dBZDate.Substring(0, 7) + "-01'),120)+'01')     \n");
                //strSql.Append(")t1    \n");
                //strSql.Append("left join (    \n");
                //strSql.Append("	--大品目    \n");
                //strSql.Append("	select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0'    \n");
                //strSql.Append(")t2 on 1=1    \n");
                //strSql.Append("left join (    \n");
                //strSql.Append("	--纳入计划    \n");
                //strSql.Append("	select dPackDate,vcBigPM,sum(iBZPlan_Heji) as iBZPlan_Heji     \n");
                //strSql.Append("	from TPackingPlan_Summary     \n");
                //strSql.Append("	group by dPackDate,vcBigPM       \n");
                //strSql.Append(")t3 on t1.dt=t3.dPackDate and t2.vcBigPM=t3.vcBigPM    \n");
                //strSql.Append("order by t1.dt,t2.vcBigPM    \n");
                #endregion
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    string E = listInfoData[i]["iLJBZRemain"].ToString() == "" ? "0" : listInfoData[i]["iLJBZRemain"].ToString();
                    string F = listInfoData[i]["iPlanTZ"].ToString() == "" ? "0" : listInfoData[i]["iPlanTZ"].ToString();

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增

                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        DateTime dPackDate = Convert.ToDateTime(listInfoData[i]["dPackDate"].ToString());
                        string dPackDate_yyyymm = Convert.ToDateTime(listInfoData[i]["dPackDate"].ToString()).ToString("yyyyMM");
                        string dPackDate_day = dPackDate.Day.ToString();

                        sql.Append("update TPackingPlan_Summary set iPlanTZ=" + F + ",iLJBZRemain=" + E + ", \n");
                        sql.Append("vcOperatorID='" + strUserId + "',dOperatorTime=getdate() where iAutoId=" + iAutoId + "    \n");

                        sql.Append("update t1 set     \n");
                        sql.Append("t1.iSSPlan_Day=case when vcBigPM='成型' then A+ceiling((D+F)/2.0) else ceiling((C+D+F)/2.0) end,    \n");
                        sql.Append("t1.iSSPlan_Night=case when vcBigPM='成型' then C+ D+F-(A+ceiling((D+F)/2.0)) else C+D+F-ceiling((C+D+F)/2.0) end,    \n");
                        sql.Append("t1.iSSPlan_Heji=C+D+F   \n");
                        sql.Append("from (    \n");
                        sql.Append("	select * from TPackingPlan_Summary where iAutoId=" + iAutoId + "    \n");
                        sql.Append(")t1    \n");
                        sql.Append("left join (    \n");
                        sql.Append("	select iAutoId,     \n");
                        sql.Append("	case when iBZPlan_Day is null or iBZPlan_Day='' then 0 else iBZPlan_Day end as A,    \n");
                        sql.Append("	case when iBZPlan_Night is null or iBZPlan_Night='' then 0 else iBZPlan_Night end as B,    \n");
                        sql.Append("	case when iBZPlan_Heji is null or iBZPlan_Heji='' then 0 else iBZPlan_Heji end as C,    \n");
                        sql.Append("	case when iEmergencyOrder is null or iEmergencyOrder='' then 0 else iEmergencyOrder end as D,    \n");
                        sql.Append("	case when iLJBZRemain is null or iLJBZRemain='' then 0 else iLJBZRemain end as E,    \n");
                        sql.Append("	case when iPlanTZ is null or iPlanTZ='' then 0 else iPlanTZ end as F    \n");
                        sql.Append("	from TPackingPlan_Summary where iAutoId=" + iAutoId + "    \n");
                        sql.Append(")t2 on t1.iAutoId=t2.iAutoId    \n");

                        //更新report表的累计残
                        sql.AppendLine("update t1 set t1.iD"+ dPackDate_day + "=t2.iLJBZRemain from");
                        sql.AppendLine("(select * from TPackingPlan_Report where vcKind='累计残量' and vcYearMonth='"+ dPackDate_yyyymm + "')t1");
                        sql.AppendLine("inner join (select * from TPackingPlan_Summary where iAutoId=75)t2 ");
                        sql.AppendLine("on t1.vcPlant=t2.vcPlant and t1.vcYearMonth=CONVERT(varchar(6),t2.dPackDate,112)");
                        sql.AppendLine("and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");


                    }
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string vcFZPlant, string dBZDate, string strUserId, string strUnit)
        {
            SqlConnection conn = ComConnectionHelper.CreateSqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlTransaction st = null;
            ComConnectionHelper.OpenConection_SQL(ref conn);
            st = conn.BeginTransaction();
            StringBuilder sql = new StringBuilder();
            try
            {
                //删除子表
                sql.Append("DELETE FROM [TPackingPlan] where vcFZPlant='" + vcFZPlant + "' and vcPackDate='" + dBZDate + "' \n");
                #region 插入子表
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strOrderNo = dt.Rows[i]["UNLD DTE"].ToString() + dt.Rows[i]["FRQ"].ToString();
                    #region insert sql
                    sql.Append("INSERT INTO [TPackingPlan]    \n");
                    sql.Append("           ([vcPlant]    \n");
                    sql.Append("           ,[vcPackDate]    \n");
                    sql.Append("           ,[vcPackBZ]    \n");
                    sql.Append("           ,[vcPartId]    \n");
                    sql.Append("           ,[iPackNum]    \n");
                    sql.Append("           ,[vcFZPlant]    \n");
                    sql.Append("           ,[vcSupplier_id]    \n");
                    sql.Append("           ,[vcGQ]    \n");
                    sql.Append("           ,[vcSR]    \n");
                    sql.Append("           ,[vcOrderNo]    \n");
                    sql.Append("           ,[vcOperatorID]    \n");
                    sql.Append("           ,[dOperatorTime])    \n");
                    sql.Append("     VALUES    \n");
                    sql.Append("           (null    \n");//包装工厂，下面统一更新
                    sql.Append("           ,'" + dBZDate + "'    \n");
                    sql.Append("           ,null    \n");//班值，白/夜  ，下面统一更新 
                    sql.Append("           ,'" + dt.Rows[i]["PART #"].ToString() + "'    \n");
                    sql.Append("           ,nullif('" + dt.Rows[i]["FINL ORD(PCS)"].ToString() + "','')    \n");
                    sql.Append("           ,'" + vcFZPlant + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["SUPL"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["PLANT"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["DOCK"].ToString() + "'    \n");
                    sql.Append("           ,'" + strOrderNo + "'    \n");
                    sql.Append("           ,'" + strUserId + "'    \n");
                    sql.Append("           ,getdate())    \n");
                    #endregion

                    if (i % 1000 == 0)
                    {
                        cmd = new SqlCommand(sql.ToString(), conn, st);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        sql.Length = 0;
                    }
                }
                #endregion
                #region 更新子表包装场
                sql.Append("update t1 set t1.vcPlant=t2.vcBZPlant     \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TPackingPlan where vcFZPlant='" + vcFZPlant + "' and vcPackDate='" + dBZDate + "'    \n");
                sql.Append(")t1    \n");
                sql.Append("left join (select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "')t2 on t1.vcPartId=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");

                cmd = new SqlCommand(sql.ToString(), conn, st);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                sql.Length = 0;
                #endregion
                #region 更新子表班值
                sql.Append("update t1 set t1.vcPackBZ=case when t3.vcCodeId is not null then t3.vcValue1 else t4.vcValue1 end    \n");
                sql.Append("from    \n");
                sql.Append("(select * from TPackingPlan where vcFZPlant='" + vcFZPlant + "' and vcPackDate='" + dBZDate + "'    \n");
                sql.Append(")t1    \n");
                sql.Append("left join  TNRBJSKBianCi t2 on t1.vcSupplier_id=t2.vcSupplier_id and t1.vcGQ=t2.vcGQ        \n");
                sql.Append("and t1.vcSR=t2.vcSR and t1.vcOrderNo=t2.vcOrderNo        \n");
                sql.Append("left join    \n");
                sql.Append("(    \n");
                sql.Append("	select vcCodeId,vcCodeName,vcValue1,    \n");
                sql.Append("	  case when vcValue1='白' then cast('2001-01-01 '+vcValue2 as datetime)     \n");
                sql.Append("			when vcValue1='夜' then cast('2001-01-01 '+vcValue2 as datetime)    \n");
                sql.Append("	   end as vcValue2 ,    \n");
                sql.Append("       case when vcValue1='白' then cast('2001-01-01 '+vcValue3 as datetime)     \n");
                sql.Append("	        when vcValue1='夜' then cast('2001-01-02 '+vcValue3 as datetime)    \n");
                sql.Append("	   end as vcValue3     \n");
                sql.Append("	from TOutCode where vcCodeId='C002' and vcIsColum='0'    \n");
                sql.Append(")t3 on cast('2001-01-01 '+t2.vcNRBJSK as datetime) between cast(t3.vcValue2 as datetime) and cast(t3.vcValue3 as datetime)    \n");
                sql.Append("left join    \n");
                sql.Append("(    \n");
                sql.Append("	select vcCodeId,vcCodeName,vcValue1,    \n");
                sql.Append("	  case when vcValue1='白' then cast('2001-01-01 '+vcValue2 as datetime)     \n");
                sql.Append("			when vcValue1='夜' then cast('2001-01-01 '+vcValue2 as datetime)    \n");
                sql.Append("	   end as vcValue2 ,    \n");
                sql.Append("       case when vcValue1='白' then cast('2001-01-01 '+vcValue3 as datetime)     \n");
                sql.Append("	        when vcValue1='夜' then cast('2001-01-02 '+vcValue3 as datetime)    \n");
                sql.Append("	   end as vcValue3     \n");
                sql.Append("	from TOutCode where vcCodeId='C002' and vcIsColum='0'    \n");
                sql.Append(")t4 on cast('2001-01-02 '+t2.vcNRBJSK as datetime) between cast(t4.vcValue2 as datetime) and cast(t4.vcValue3 as datetime)    \n");

                cmd = new SqlCommand(sql.ToString(), conn, st);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                sql.Length = 0;
                #endregion

                //删除主表
                sql.Append("delete from TPackingPlan_Summary where dPackDate='" + dBZDate + "'    \n");
                #region 插入主表-外注
                sql.Append("INSERT INTO [TPackingPlan_Summary]        \n");
                sql.Append("           (vcPlant    \n");
                sql.Append("		   ,[dPackDate]        \n");
                sql.Append("           ,[vcBigPM]     \n");
                sql.Append("		   ,vcSmallPM    \n");
                sql.Append("		   ,vcStandardTime    \n");
                sql.Append("           ,[iBZPlan_Day]        \n");
                sql.Append("           ,[iBZPlan_Night]        \n");
                sql.Append("           ,[iBZPlan_Heji]        \n");
                sql.Append("           ,[vcInOutType]        \n");
                sql.Append("           ,[vcOperatorID]        \n");
                sql.Append("           ,[dOperatorTime])        \n");
                sql.Append("select a1.vcPlant,'" + dBZDate + "',a1.vcBigPM,a1.vcSmallPM,a1.vcStandardTime,    \n");
                sql.Append("isnull(a2.A,0) as A,isnull(a2.B,0) as B,isnull(a2.C,0) as C,'外注' as kind,'" + strUserId + "',GETDATE()        \n");
                sql.Append("from (        \n");
                sql.Append("	--大品目    \n");
                sql.Append("	select t1.vcValue as vcPlant,t2.* from (    \n");
                sql.Append("		select * from TCode where vcCodeId='C023'    \n");
                sql.Append("	)t1    \n");
                sql.Append("	left join (    \n");
                sql.Append("		select t1.vcBigPM,t2.vcSmallPM,t2.vcStandardTime from (    \n");
                sql.Append("			select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0' and vcValue2='外注'     \n");
                sql.Append("		)t1    \n");
                sql.Append("		left join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM    \n");
                sql.Append("	)t2 on 1=1    \n");
                sql.Append(")a1        \n");
                sql.Append("left join (        \n");
                sql.Append("	--白夜合计        \n");
                sql.Append("	select t1.vcPlant,t1.vcBigPM,t1.vcSmallPM,ISNULL(t1.白,0) as A,ISNULL(t1.夜,0) as B,        \n");
                sql.Append("	ISNULL(t1.白,0)+ISNULL(t1.夜,0) as C        \n");
                sql.Append("	from        \n");
                sql.Append("	(        \n");
                sql.Append("		select t1.vcPlant,t3.vcBigPM,t2.vcSmallPM,t1.vcPackBZ,sum(t1.iPackNum) as iPackNum from (        \n");
                sql.Append("			select * from TPackingPlan where vcPackDate='" + dBZDate + "'        \n");
                sql.Append("		)t1        \n");
                sql.Append("		left join (    \n");
                sql.Append("			select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("		) t2 on t1.vcPartId=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("		left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("		group by t1.vcPlant,t3.vcBigPM,t2.vcSmallPM,t1.vcPlant,t1.vcPackBZ    \n");
                sql.Append("	) test pivot(sum(iPackNum) for vcPackBZ in(白,夜)) t1        \n");
                sql.Append(")a2 on a1.vcBigPM=a2.vcBigPM and a1.vcSmallPM=a2.vcSmallPM and a1.vcPlant=a2.vcPlant    \n");
                #endregion
                #region 插入主表-内制
                int iday = Convert.ToDateTime(dBZDate).Day;
                sql.Append("INSERT INTO [TPackingPlan_Summary]        \n");
                sql.Append("           (vcPlant    \n");
                sql.Append("		   ,[dPackDate]        \n");
                sql.Append("           ,[vcBigPM]     \n");
                sql.Append("		   ,vcSmallPM    \n");
                sql.Append("		   ,vcStandardTime    \n");
                sql.Append("           ,[iBZPlan_Day]        \n");
                sql.Append("           ,[iBZPlan_Night]        \n");
                sql.Append("           ,[iBZPlan_Heji]        \n");
                sql.Append("           ,[vcInOutType]        \n");
                sql.Append("           ,[vcOperatorID]        \n");
                sql.Append("           ,[dOperatorTime])         \n");
                sql.Append("select a1.vcPlant,'" + dBZDate + "',a1.vcBigPM,a1.vcSmallPM,a1.vcStandardTime,        \n");
                sql.Append("isnull(a2.A,0) as A,isnull(a2.B,0) as B,isnull(a2.C,0) as C,'内制' as kind,'" + strUserId + "',GETDATE()    \n");
                sql.Append("from (        \n");
                sql.Append("	--大品目        \n");
                sql.Append("	select t1.vcValue as vcPlant,t2.* from (    \n");
                sql.Append("		select * from TCode where vcCodeId='C023'    \n");
                sql.Append("	)t1    \n");
                sql.Append("	left join (    \n");
                sql.Append("		select t1.vcBigPM,t2.vcSmallPM,t2.vcStandardTime from (    \n");
                sql.Append("			select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0' and vcValue2='内制'     \n");
                sql.Append("		)t1    \n");
                sql.Append("		left join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM    \n");
                sql.Append("	)t2 on 1=1    \n");
                sql.Append(")a1        \n");
                sql.Append("left join (        \n");
                sql.Append("	--白夜合计        \n");
                sql.Append("	select vcBZPlant,vcBigPM,vcSmallPM,SUM(白) as A,SUM(夜) as B,sum(合计) as C  from         \n");
                sql.Append("	(        \n");
                sql.Append("		select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,sum(isnull(vcD" + iday + "b,0)) as 白,sum(isnull(vcD" + iday + "y,0)) as 夜,        \n");
                sql.Append("		sum(isnull(vcD" + iday + "b,0))+sum(isnull(vcD" + iday + "y,0)) as 合计        \n");
                sql.Append("		from (        \n");
                sql.Append("			select * from MonthPackPlanTbl where vcMonth='" + dBZDate.Substring(0, 7).Replace("/", "-") + "' --斜杠改成横杠       \n");
                sql.Append("		)t1        \n");
                sql.Append("		left join (    \n");
                sql.Append("			select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("		) t2 on t1.vcPartsno=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("		left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("        left join (select * from TSPMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "')t4          \n");
                sql.Append("        on t1.vcPartsno=t4.vcPartId and t1.vcSupplier_id=t4.vcSupplierId          \n");
                sql.Append("        where t4.vcOrderingMethod='0'          \n");//只找月度的
                sql.Append("		group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("		union all        \n");
                sql.Append("		select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,sum(isnull(vcD" + iday + "b,0)) as 白,sum(isnull(vcD" + iday + "y,0)) as 夜,        \n");
                sql.Append("		sum(isnull(vcD" + iday + "b,0))+sum(isnull(vcD" + iday + "y,0)) as 合计        \n");
                sql.Append("		from (        \n");
                sql.Append("			select * from WeekPackPlanTbl where vcMonth='" + dBZDate.Substring(0, 7).Replace("/", "-") + "' --斜杠改成横杠       \n");
                sql.Append("		)t1     \n");
                sql.Append("		left join (    \n");
                sql.Append("			select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUserId + "'    \n");
                sql.Append("		) t2 on t1.vcPartsno=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("		left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("		group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("	)t1        \n");
                sql.Append("	group by vcBZPlant,vcBigPM,vcSmallPM        \n");
                sql.Append(")a2 on a1.vcBigPM=a2.vcBigPM and a1.vcSmallPM=a2.vcSmallPM and a1.vcPlant=a2.vcBZPlant    \n");
                #endregion

                cmd = new SqlCommand(sql.ToString(), conn, st);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                sql.Length = 0;

                st.Commit();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
            }
            catch (Exception ex)
            {
                st.Rollback();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion

        public DataTable GetStandardTime()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select vcBigPM,vcSmallPM,vcStandardTime from TPMRelation");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 计算 not use 
        public void Cal(string dBZDate, string strUserId, string strUnit)
        {
            try
            {
                string dBZDateBefore = Convert.ToDateTime(dBZDate).AddDays(-1).ToString("yyyy/MM/dd");
                int iday = Convert.ToDateTime(dBZDate).Day;
                StringBuilder sql = new StringBuilder();
                sql.Append("delete from TPackingPlan_Summary where dPackDate='" + dBZDate + "'    \n");
                #region 外注
                sql.Append("INSERT INTO [TPackingPlan_Summary]        \n");
                sql.Append("           (vcPlant    \n");
                sql.Append("		   ,[dPackDate]        \n");
                sql.Append("           ,[vcBigPM]     \n");
                sql.Append("		   ,vcSmallPM    \n");
                sql.Append("		   ,vcStandardTime    \n");
                sql.Append("           ,[iBZPlan_Day]        \n");
                sql.Append("           ,[iBZPlan_Night]        \n");
                sql.Append("           ,[iBZPlan_Heji]        \n");
                sql.Append("           ,[iEmergencyOrder]        \n");
                sql.Append("           ,[iLJBZRemain]        \n");
                sql.Append("           ,[iPlanTZ]        \n");
                sql.Append("           ,[iSSPlan_Day]        \n");
                sql.Append("           ,[iSSPlan_Night]        \n");
                sql.Append("           ,[iSSPlan_Heji]        \n");
                sql.Append("           ,[vcInOutType]        \n");
                sql.Append("           ,[vcOperatorID]        \n");
                sql.Append("           ,[dOperatorTime])        \n");
                sql.Append("select a1.vcPlant,'" + dBZDate + "',a1.vcBigPM,a1.vcSmallPM,a1.vcStandardTime,    \n");
                sql.Append("isnull(a2.A,0) as A,isnull(a2.B,0) as B,isnull(a2.C,0) as C,        \n");
                sql.Append("isnull(a3.D,0) as D,isnull(a4.E,0) as E,0 as F,    \n");
                sql.Append("cast((isnull(a2.C,0)+isnull(a3.D,0)+0)/2.0 as decimal(18,1)) as G,        \n");
                sql.Append("isnull(a2.C,0)+isnull(a3.D,0)-cast((isnull(a2.C,0)+isnull(a3.D,0)+0)/2.0 as decimal(18,1)) as H,        \n");
                sql.Append("isnull(a2.C,0)+isnull(a3.D,0) as 合计,        \n");
                sql.Append("'外注' as kind,       \n");
                sql.Append("'" + strUserId + "',GETDATE()      \n");
                sql.Append("from (        \n");
                sql.Append("	--大品目    \n");
                sql.Append("	select t1.vcValue as vcPlant,t2.* from (    \n");
                sql.Append("		select * from TCode where vcCodeId='C023'    \n");
                sql.Append("	)t1    \n");
                sql.Append("	left join (    \n");
                sql.Append("		select t1.vcBigPM,t2.vcSmallPM,t2.vcStandardTime from (    \n");
                sql.Append("			select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0' and vcValue2='外注'     \n");
                sql.Append("		)t1    \n");
                sql.Append("		left join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM    \n");
                sql.Append("	)t2 on 1=1    \n");
                sql.Append(")a1        \n");
                sql.Append("left join (        \n");
                sql.Append("	--白夜合计        \n");
                sql.Append("	select t1.vcPlant,t1.vcBigPM,t1.vcSmallPM,ISNULL(t1.白,0) as A,ISNULL(t1.夜,0) as B,        \n");
                sql.Append("	ISNULL(t1.白,0)+ISNULL(t1.夜,0) as C        \n");
                sql.Append("	from        \n");
                sql.Append("	(        \n");
                sql.Append("		select t1.vcPlant,t3.vcBigPM,t2.vcSmallPM,t1.vcPackBZ,sum(t1.iPackNum) as iPackNum from (        \n");
                sql.Append("			select * from TPackingPlan where vcPackDate='" + dBZDate + "'        \n");
                sql.Append("		)t1        \n");
                sql.Append("		left join (    \n");
                sql.Append("			select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("		) t2 on t1.vcPartId=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("		left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("		group by t1.vcPlant,t3.vcBigPM,t2.vcSmallPM,t1.vcPlant,t1.vcPackBZ    \n");
                sql.Append("	) test pivot(sum(iPackNum) for vcPackBZ in(白,夜)) t1        \n");
                sql.Append(")a2 on a1.vcBigPM=a2.vcBigPM and a1.vcSmallPM=a2.vcSmallPM and a1.vcPlant=a2.vcPlant    \n");
                sql.Append("left join (        \n");
                sql.Append("	--紧急订单        \n");
                sql.Append("	select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,SUM(CAST(ISNULL(t1.vcOrderNum,0) as int)) as D         \n");
                sql.Append("	from         \n");
                sql.Append("	(        \n");
                sql.Append("		select vcPartNo,vcInsideOutsideType,vcDock,vcOrderNum,vcSupplier_id         \n");
                sql.Append("		from TEmergentOrderManage         \n");
                sql.Append("		where dExpectReceiveDate='" + dBZDate + "' and vcInsideOutsideType='1'        \n");
                sql.Append("	)t1        \n");
                sql.Append("	left join (    \n");
                sql.Append("		select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("	) t2 on t1.vcPartNo=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("	left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("	group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append(")a3 on a1.vcBigPM=a3.vcBigPM and a1.vcSmallPM=a3.vcSmallPM and a1.vcPlant=a3.vcBZPlant    \n");
                sql.Append("left join (        \n");
                sql.Append("	--N日累积包装残=N-1日实行计划合计-N-1日作业实绩中包装的(N-1 07:00-N 07:00)        \n");
                sql.Append("	select vcPlant,vcBigPM,vcSmallPM,sum(heji) as E  from         \n");
                sql.Append("	(        \n");
                sql.Append("		--N-1日实行计划合计        \n");
                sql.Append("		select vcPlant,vcBigPM,vcSmallPM,isnull(iSSPlan_Heji,0) as heji from TPackingPlan_Summary         \n");
                sql.Append("		where vcInOutType='1' and dPackDate between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'        \n");
                sql.Append("		union all        \n");
                sql.Append("		--N-1日作业实绩中包装的        \n");
                sql.Append("		select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,0-SUM(ISNULL(t1.iQuantity,0)) as heji         \n");
                sql.Append("		from         \n");
                sql.Append("		(        \n");
                sql.Append("			select vcPart_id,vcSR,iQuantity,vcSupplier_id from TOperateSJ         \n");
                sql.Append("			where vcZYType='S2' and vcIOType='1'        \n");
                sql.Append("			and dEnd between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'        \n");
                sql.Append("		)t1       \n");
                sql.Append("		left join (    \n");
                sql.Append("			select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("		) t2 on t1.vcPart_id=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("		left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("		group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("	)t1        \n");
                sql.Append("	group by vcPlant,vcBigPM,vcSmallPM        \n");
                sql.Append(")a4 on a1.vcBigPM=a4.vcBigPM and a1.vcSmallPM=a4.vcSmallPM and a1.vcPlant=a4.vcPlant    \n");
                #endregion

                #region 内制
                sql.Append("INSERT INTO [TPackingPlan_Summary]        \n");
                sql.Append("           (vcPlant    \n");
                sql.Append("		   ,[dPackDate]        \n");
                sql.Append("           ,[vcBigPM]     \n");
                sql.Append("		   ,vcSmallPM    \n");
                sql.Append("		   ,vcStandardTime    \n");
                sql.Append("           ,[iBZPlan_Day]        \n");
                sql.Append("           ,[iBZPlan_Night]        \n");
                sql.Append("           ,[iBZPlan_Heji]        \n");
                sql.Append("           ,[iEmergencyOrder]        \n");
                sql.Append("           ,[iLJBZRemain]        \n");
                sql.Append("           ,[iPlanTZ]        \n");
                sql.Append("           ,[iSSPlan_Day]        \n");
                sql.Append("           ,[iSSPlan_Night]        \n");
                sql.Append("           ,[iSSPlan_Heji]        \n");
                sql.Append("           ,[vcInOutType]        \n");
                sql.Append("           ,[vcOperatorID]        \n");
                sql.Append("           ,[dOperatorTime])         \n");
                sql.Append("select vcPlant,'" + dBZDate + "',vcBigPM,vcSmallPM,vcStandardTime,A,B,C,D,E,F,        \n");
                sql.Append("case when vcBigPM='成型' then A+cast((D+F)/2.0 as decimal(18,1)) else cast((C+D+F)/2.0 as decimal(18,1)) end as G,        \n");
                sql.Append("case when vcBigPM='成型' then B+ D+F-cast((D+F)/2.0 as decimal(18,1)) else C+D+F-cast((C+D+F)/2.0 as decimal(18,1)) end as H,        \n");
                sql.Append("case when vcBigPM='成型' then A+B+D+F else C+D+F end as '合计',        \n");
                sql.Append("'内制' as kind,'" + strUserId + "',GETDATE()     \n");
                sql.Append("from (        \n");
                sql.Append("	select a1.vcPlant,a1.vcBigPM,a1.vcSmallPM,a1.vcStandardTime,isnull(a2.A,0) as A,isnull(a2.B,0) as B,isnull(a2.C,0) as C,        \n");
                sql.Append("	isnull(a3.D,0) as D,isnull(a4.E,0) as E,0 as F,        \n");
                sql.Append("	'内制' as kind       \n");
                sql.Append("	from (        \n");
                sql.Append("		--大品目        \n");
                sql.Append("		select t1.vcValue as vcPlant,t2.* from (    \n");
                sql.Append("			select * from TCode where vcCodeId='C023'    \n");
                sql.Append("		)t1    \n");
                sql.Append("		left join (    \n");
                sql.Append("			select t1.vcBigPM,t2.vcSmallPM,t2.vcStandardTime from (    \n");
                sql.Append("				select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0' and vcValue2='内制'     \n");
                sql.Append("			)t1    \n");
                sql.Append("			left join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM    \n");
                sql.Append("		)t2 on 1=1    \n");
                sql.Append("	)a1        \n");
                sql.Append("	left join (        \n");
                sql.Append("		--白夜合计        \n");
                sql.Append("		select vcBZPlant,vcBigPM,vcSmallPM,SUM(白) as A,SUM(夜) as B,sum(合计) as C  from         \n");
                sql.Append("		(        \n");
                sql.Append("			select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,sum(isnull(vcD" + iday + "b,0)) as 白,sum(isnull(vcD" + iday + "y,0)) as 夜,        \n");
                sql.Append("			sum(isnull(vcD" + iday + "b,0))+sum(isnull(vcD" + iday + "y,0)) as 合计        \n");
                sql.Append("			from (        \n");
                sql.Append("				select * from MonthPackPlanTbl where vcMonth='" + dBZDate.Substring(0, 7).Replace("/", "-") + "' --斜杠改成横杠       \n");
                sql.Append("			)t1        \n");
                sql.Append("			left join (    \n");
                sql.Append("				select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("			) t2 on t1.vcPartsno=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("			left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("            left join (select * from TSPMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "')t4          \n");
                sql.Append("            on t1.vcPartsno=t4.vcPartId and t1.vcSupplier_id=t4.vcSupplierId          \n");
                sql.Append("            where t4.vcOrderingMethod='0'          \n");//只找月度的
                sql.Append("			group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("			union all        \n");
                sql.Append("			select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,sum(isnull(vcD" + iday + "b,0)) as 白,sum(isnull(vcD" + iday + "y,0)) as 夜,        \n");
                sql.Append("			sum(isnull(vcD" + iday + "b,0))+sum(isnull(vcD" + iday + "y,0)) as 合计        \n");
                sql.Append("			from (        \n");
                sql.Append("				select * from WeekPackPlanTbl where vcMonth='" + dBZDate.Substring(0, 7).Replace("/", "-") + "' --斜杠改成横杠       \n");
                sql.Append("			)t1     \n");
                sql.Append("			left join (    \n");
                sql.Append("				select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUserId + "'    \n");
                sql.Append("			) t2 on t1.vcPartsno=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("			left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("			group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("		)t1        \n");
                sql.Append("		group by vcBZPlant,vcBigPM,vcSmallPM        \n");
                sql.Append("	)a2 on a1.vcBigPM=a2.vcBigPM and a1.vcSmallPM=a2.vcSmallPM and a1.vcPlant=a2.vcBZPlant    \n");
                sql.Append("	left join (        \n");
                sql.Append("		--紧急订单        \n");
                sql.Append("		select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,SUM(CAST(ISNULL(t1.vcOrderNum,0) as int)) as D          \n");
                sql.Append("		from         \n");
                sql.Append("		(        \n");
                sql.Append("			select vcPartNo,vcInsideOutsideType,vcDock,vcOrderNum,vcSupplier_id         \n");
                sql.Append("			from TEmergentOrderManage         \n");
                sql.Append("			where dExpectReceiveDate='" + dBZDate + "' and vcInsideOutsideType='0'        \n");
                sql.Append("		)t1        \n");
                sql.Append("		left join (    \n");
                sql.Append("			select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("		) t2 on t1.vcPartNo=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("		left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("		group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("	)a3 on a1.vcBigPM=a3.vcBigPM and a1.vcSmallPM=a3.vcSmallPM and a1.vcPlant=a3.vcBZPlant    \n");
                sql.Append("	left join (        \n");
                sql.Append("		--N日累积包装残=N-1日实行计划合计-N日作业实绩中包装的(N-1 07:00-N 07:00)        \n");
                sql.Append("		select vcPlant,vcBigPM,vcSmallPM,sum(heji) as E  from         \n");
                sql.Append("		(        \n");
                sql.Append("			--N-1日实行计划合计        \n");
                sql.Append("			select vcPlant,vcBigPM,vcSmallPM,isnull(iSSPlan_Heji,0) as heji from TPackingPlan_Summary         \n");
                sql.Append("			where vcInOutType='0' and dPackDate between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'        \n");
                sql.Append("			union all        \n");
                sql.Append("			--N-1日作业实绩中包装的        \n");
                sql.Append("			select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,0-SUM(ISNULL(t1.iQuantity,0)) as heji         \n");
                sql.Append("			from         \n");
                sql.Append("			(        \n");
                sql.Append("				select vcPart_id,vcSR,iQuantity,vcSupplier_id from TOperateSJ         \n");
                sql.Append("				where vcZYType='S2' and vcIOType='0'        \n");
                sql.Append("				and dEnd between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'        \n");
                sql.Append("			)t1        \n");
                sql.Append("			left join (    \n");
                sql.Append("				select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='" + strUnit + "'    \n");
                sql.Append("			) t2 on t1.vcPart_id=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("			left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    \n");
                sql.Append("			group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    \n");
                sql.Append("		)t1        \n");
                sql.Append("		group by vcPlant,vcBigPM,vcSmallPM        \n");
                sql.Append("	)a4 on a1.vcBigPM=a4.vcBigPM and a1.vcSmallPM=a4.vcSmallPM and a1.vcPlant=a4.vcPlant    \n");
                sql.Append(")a        \n");
                #endregion

                #region not use
                //#region 外注
                //sql.Append("INSERT INTO [TPackingPlan_Summary]    \n");
                //sql.Append("           ([dPackDate]    \n");
                //sql.Append("           ,[vcBigPM]    \n");
                //sql.Append("           ,[iBZPlan_Day]    \n");
                //sql.Append("           ,[iBZPlan_Night]    \n");
                //sql.Append("           ,[iBZPlan_Heji]    \n");
                //sql.Append("           ,[iEmergencyOrder]    \n");
                //sql.Append("           ,[iLJBZRemain]    \n");
                //sql.Append("           ,[iPlanTZ]    \n");
                //sql.Append("           ,[iSSPlan_Day]    \n");
                //sql.Append("           ,[iSSPlan_Night]    \n");
                //sql.Append("           ,[iSSPlan_Heji]    \n");
                //sql.Append("           ,[vcInOutType]    \n");
                //sql.Append("           ,[vcOperatorID]    \n");
                //sql.Append("           ,[dOperatorTime])    \n");
                //sql.Append("select '" + dBZDate + "',a1.vcBigPM,isnull(a2.A,0) as A,isnull(a2.B,0) as B,isnull(a2.C,0) as C,    \n");
                //sql.Append("isnull(a3.D,0) as D,isnull(a4.E,0) as E,0 as F,(isnull(a2.C,0)+isnull(a3.D,0)+0)/2 as G,    \n");
                //sql.Append("(isnull(a2.C,0)+isnull(a3.D,0)+0)/2 as H,    \n");
                //sql.Append("(isnull(a2.C,0)+isnull(a3.D,0)+0)/2+(isnull(a2.C,0)+isnull(a3.D,0)+0)/2 as 合计,    \n");
                //sql.Append("'外注' as kind,   \n");
                //sql.Append("'" + strUserId + "',GETDATE()    \n");
                //sql.Append("from (    \n");
                //sql.Append("	--大品目    \n");
                //sql.Append("	select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0' and vcValue2='外注'   \n");
                //sql.Append(")a1    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	--白夜合计    \n");
                //sql.Append("	select t1.vcBigPM,ISNULL(t1.白,0) as A,ISNULL(t1.夜,0) as B,    \n");
                //sql.Append("	ISNULL(t1.白,0)+ISNULL(t1.夜,0) as C    \n");
                //sql.Append("	from    \n");
                //sql.Append("	(    \n");
                //sql.Append("		select t6.vcBigPM,t1.vcPackBZ,sum(t1.iPackNum) as iPackNum from (    \n");
                //sql.Append("			select * from TPackingPlan where vcPackDate='" + dBZDate + "'    \n");
                //sql.Append("		)t1    \n");
                //sql.Append("		left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4     \n");
                //sql.Append("		on t1.vcPartId=t4.vcPartsNo    \n");
                //sql.Append("		left join TPMSmall t5 on left(t1.vcPartId,5)=t5.vcPartsNoBefore5 and t1.vcSR=t5.vcSR    \n");
                //sql.Append("		and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("		left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("		group by t6.vcBigPM,t1.vcPackBZ    \n");
                //sql.Append("	) test pivot(sum(iPackNum) for vcPackBZ in(白,夜)) t1    \n");
                //sql.Append(")a2 on a1.vcBigPM=a2.vcBigPM    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	--紧急订单    \n");
                //sql.Append("	select t6.vcBigPM,SUM(CAST(ISNULL(t1.vcOrderNum,0) as int)) as D     \n");
                //sql.Append("	from     \n");
                //sql.Append("	(    \n");
                //sql.Append("		select vcPartNo,vcInsideOutsideType,vcDock,vcOrderNum     \n");
                //sql.Append("		from TEmergentOrderManage     \n");
                //sql.Append("		where dExpectReceiveDate='" + dBZDate + "' and vcInsideOutsideType='1'    \n");
                //sql.Append("	)t1    \n");
                //sql.Append("	left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4     \n");
                //sql.Append("	on t1.vcPartNo=t4.vcPartsNo    \n");
                //sql.Append("	left join TPMSmall t5 on left(t1.vcPartNo,5)=t5.vcPartsNoBefore5 and t1.vcDock=t5.vcSR    \n");
                //sql.Append("	and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("	left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("	group by t6.vcBigPM    \n");
                //sql.Append(")a3 on a1.vcBigPM=a3.vcBigPM    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	--N日累积包装残=N-1日实行计划合计-N-1日作业实绩中包装的(N-1 07:00-N 07:00)    \n");
                //sql.Append("	select vcBigPM,sum(heji) as E  from     \n");
                //sql.Append("	(    \n");
                //sql.Append("		--N-1日实行计划合计    \n");
                //sql.Append("		select vcBigPM,isnull(iSSPlan_Heji,0) as heji from TPackingPlan_Summary     \n");
                //sql.Append("		where vcInOutType='1' and dPackDate between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'    \n");
                //sql.Append("		union all    \n");
                //sql.Append("		--N-1日作业实绩中包装的    \n");
                //sql.Append("		select t6.vcBigPM,0-SUM(ISNULL(t1.iQuantity,0)) as heji     \n");
                //sql.Append("		from     \n");
                //sql.Append("		(    \n");
                //sql.Append("			select vcPart_id,vcSR,iQuantity from TOperateSJ     \n");
                //sql.Append("			where vcZYType='S2' and vcIOType='1'    \n");
                //sql.Append("			and dEnd between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'    \n");
                //sql.Append("		)t1    \n");
                //sql.Append("		left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4     \n");
                //sql.Append("		on t1.vcPart_id=t4.vcPartsNo    \n");
                //sql.Append("		left join TPMSmall t5 on left(t1.vcPart_id,5)=t5.vcPartsNoBefore5 and t1.vcSR=t5.vcSR    \n");
                //sql.Append("		and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("		left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("		group by t6.vcBigPM    \n");
                //sql.Append("	)t1    \n");
                //sql.Append("	group by vcBigPM    \n");
                //sql.Append(")a4 on a1.vcBigPM=a4.vcBigPM    \n");
                //#endregion
                //#region 内制
                //sql.Append("INSERT INTO [TPackingPlan_Summary]    \n");
                //sql.Append("           ([dPackDate]    \n");
                //sql.Append("           ,[vcBigPM]    \n");
                //sql.Append("           ,[iBZPlan_Day]    \n");
                //sql.Append("           ,[iBZPlan_Night]    \n");
                //sql.Append("           ,[iBZPlan_Heji]    \n");
                //sql.Append("           ,[iEmergencyOrder]    \n");
                //sql.Append("           ,[iLJBZRemain]    \n");
                //sql.Append("           ,[iPlanTZ]    \n");
                //sql.Append("           ,[iSSPlan_Day]    \n");
                //sql.Append("           ,[iSSPlan_Night]    \n");
                //sql.Append("           ,[iSSPlan_Heji]    \n");
                //sql.Append("           ,[vcInOutType]    \n");
                //sql.Append("           ,[vcOperatorID]    \n");
                //sql.Append("           ,[dOperatorTime])    \n");
                //sql.Append("select '" + dBZDate + "',vcBigPM,A,B,C,D,E,F,    \n");
                //sql.Append("case when vcBigPM='OR' then A+(D+F)/2 else (C+D+F)/2 end as G,    \n");
                //sql.Append("case when vcBigPM='OR' then B+(D+F)/2 else (C+D+F)/2 end as H,    \n");
                //sql.Append("case when vcBigPM='OR' then (A+(D+F)/2)+(B+(D+F)/2) else ((C+D+F)/2)+((C+D+F)/2) end as '合计',    \n");
                //sql.Append("kind,'" + strUserId + "',GETDATE()    \n");
                //sql.Append("from (    \n");
                //sql.Append("select a1.vcBigPM,isnull(a2.A,0) as A,isnull(a2.B,0) as B,isnull(a2.C,0) as C,    \n");
                //sql.Append("isnull(a3.D,0) as D,isnull(a4.E,0) as E,0 as F,    \n");
                //sql.Append("'内制' as kind   \n");
                //sql.Append("from (    \n");
                //sql.Append("	--大品目    \n");
                //sql.Append("	select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0' and vcValue2='内制'     \n");
                //sql.Append(")a1    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	--白夜合计    \n");
                //sql.Append("	select vcBigPM,SUM(白) as A,SUM(夜) as B,sum(合计) as C  from     \n");
                //sql.Append("	(    \n");
                //sql.Append("		select t6.vcBigPM,sum(isnull(vcD" + iday + "b,0)) as 白,sum(isnull(vcD" + iday + "y,0)) as 夜,    \n");///////////////////
                //sql.Append("		sum(isnull(vcD" + iday + "b,0))+sum(isnull(vcD" + iday + "y,0)) as 合计    \n");
                //sql.Append("		from (    \n");
                //sql.Append("			select * from MonthPackPlanTbl where vcMonth='" + dBZDate.Substring(0, 7) + "'    \n");
                //sql.Append("		)t1    \n");
                //sql.Append("		left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4     \n");
                //sql.Append("		on t1.vcPartsno=t4.vcPartsNo    \n");
                //sql.Append("		left join TPMSmall t5 on left(t1.vcPartsno,5)=t5.vcPartsNoBefore5 and t1.vcDock=t5.vcSR    \n");
                //sql.Append("		and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("		left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("		group by t6.vcBigPM    \n");
                //sql.Append("		union all    \n");
                //sql.Append("		select t6.vcBigPM,sum(isnull(vcD" + iday + "b,0)) as 白,sum(isnull(vcD" + iday + "y,0)) as 夜,    \n");
                //sql.Append("		sum(isnull(vcD" + iday + "b,0))+sum(isnull(vcD" + iday + "y,0)) as 合计    \n");
                //sql.Append("		from (    \n");
                //sql.Append("			select * from WeekPackPlanTbl where vcMonth='" + dBZDate.Substring(0, 7) + "'    \n");
                //sql.Append("		)t1    \n");
                //sql.Append("		left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4     \n");
                //sql.Append("		on t1.vcPartsno=t4.vcPartsNo    \n");
                //sql.Append("		left join TPMSmall t5 on left(t1.vcPartsno,5)=t5.vcPartsNoBefore5 and t1.vcDock=t5.vcSR    \n");
                //sql.Append("		and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("		left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("		group by t6.vcBigPM    \n");
                //sql.Append("	)t1    \n");
                //sql.Append("	group by vcBigPM    \n");
                //sql.Append(")a2 on a1.vcBigPM=a2.vcBigPM    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	--紧急订单    \n");
                //sql.Append("	select t6.vcBigPM,SUM(CAST(ISNULL(t1.vcOrderNum,0) as int)) as D      \n");
                //sql.Append("	from     \n");
                //sql.Append("	(    \n");
                //sql.Append("		select vcPartNo,vcInsideOutsideType,vcDock,vcOrderNum     \n");
                //sql.Append("		from TEmergentOrderManage     \n");
                //sql.Append("		where dExpectReceiveDate='" + dBZDate + "' and vcInsideOutsideType='0'    \n");
                //sql.Append("	)t1    \n");
                //sql.Append("	left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4     \n");
                //sql.Append("	on t1.vcPartNo=t4.vcPartsNo    \n");
                //sql.Append("	left join TPMSmall t5 on left(t1.vcPartNo,5)=t5.vcPartsNoBefore5 and t1.vcDock=t5.vcSR    \n");
                //sql.Append("	and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("	left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("	group by t6.vcBigPM    \n");
                //sql.Append(")a3 on a1.vcBigPM=a3.vcBigPM    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	--N日累积包装残=N-1日实行计划合计-N-1日作业实绩中包装的(N-1 07:00-N 07:00)    \n");
                //sql.Append("	select vcBigPM,sum(heji) as E  from     \n");
                //sql.Append("	(    \n");
                //sql.Append("		--N-1日实行计划合计    \n");
                //sql.Append("		select vcBigPM,isnull(iSSPlan_Heji,0) as heji from TPackingPlan_Summary     \n");
                //sql.Append("		where vcInOutType='0' and dPackDate between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'    \n");
                //sql.Append("		union all    \n");
                //sql.Append("		--N-1日作业实绩中包装的    \n");
                //sql.Append("		select t6.vcBigPM,0-SUM(ISNULL(t1.iQuantity,0)) as heji     \n");
                //sql.Append("		from     \n");
                //sql.Append("		(    \n");
                //sql.Append("			select vcPart_id,vcSR,iQuantity from TOperateSJ     \n");
                //sql.Append("			where vcZYType='S2' and vcIOType='0'    \n");
                //sql.Append("			and dEnd between '" + dBZDateBefore + " 07:00' and '" + dBZDate + " 07:00'    \n");
                //sql.Append("		)t1    \n");
                //sql.Append("		left join (select distinct vcPartsNo,vcPackNo from TPackItem where '" + dBZDate + "' between dFrom and dTo) t4    \n");
                //sql.Append("		on t1.vcPart_id=t4.vcPartsNo    \n");
                //sql.Append("		left join TPMSmall t5 on left(t1.vcPart_id,5)=t5.vcPartsNoBefore5 and t1.vcSR=t5.vcSR    \n");
                //sql.Append("		and t4.vcPackNo = t5.vcBCPartsNo    \n");
                //sql.Append("		left join TPMRelation t6 on t5.vcSmallPM=t6.vcSmallPM    \n");
                //sql.Append("		group by t6.vcBigPM    \n");
                //sql.Append("	)t1    \n");
                //sql.Append("	group by vcBigPM    \n");
                //sql.Append(")a4 on a1.vcBigPM=a4.vcBigPM    \n");
                //sql.Append(")a    \n");
                //#endregion
                #endregion
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
