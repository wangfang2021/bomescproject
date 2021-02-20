﻿using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0602_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getSearchInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
                  string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                  string strDyInfo, string strHyInfo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT TT.*,");
                strSql.AppendLine("case when TT.iTzhSOQN is null or TT.iTzhSOQN1 is null or TT.iTzhSOQN2 is null then 'partFS0602A' --无调整");
                strSql.AppendLine("	when TT.iTzhSOQN=TT.iCbSOQN and TT.iTzhSOQN1=TT.iCbSOQN1 and TT.iTzhSOQN2=TT.iCbSOQN2 then 'partFS0602A' --无调整");
                strSql.AppendLine("	else 'partFS0602B' --有调整");
                strSql.AppendLine("	end as vcBgColor,");
                strSql.AppendLine("	'0' as bModFlag,'0' as bAddFlag,CASE WHEN vcHyState='1' or vcHyState='2' then '1' else '0' end as bSelectFlag");
                strSql.AppendLine("FROM (");
                strSql.AppendLine("select T1.iAutoId AS LinId,");
                strSql.AppendLine("		T1.vcYearMonth AS vcYearMonth,");
                strSql.AppendLine("		T1.vcDyState,");
                strSql.AppendLine("		T6.vcName AS vcDyState_Name,");
                strSql.AppendLine("		T1.vcHyState,");
                strSql.AppendLine("		T7.vcName AS vcHyState_Name,");
                strSql.AppendLine("		T1.vcPart_id AS vcPart_id,");
                strSql.AppendLine("		T1.vcCarFamilyCode AS vcCarfamilyCode,");
                strSql.AppendLine("		T3.vcName AS vcHaoJiu,");
                strSql.AppendLine("		T4.vcName AS vcOrderingMethod,");
                strSql.AppendLine("		T5.vcName AS vcOrderPlant,");
                strSql.AppendLine("		T2.vcName AS vcInOut,");
                strSql.AppendLine("		T1.vcSupplier_id AS vcSupplierId,");
                strSql.AppendLine("		T1.vcSupplierPlant AS vcSupplierPlant,");
                strSql.AppendLine("		T1.iQuantityPercontainer AS iPackingQty,");
                strSql.AppendLine("		T1.iCbSOQN AS iCbSOQN,");
                strSql.AppendLine("		T1.decCbBdl AS decCbBdl,");
                strSql.AppendLine("		T1.iCbSOQN1 AS iCbSOQN1,");
                strSql.AppendLine("		T1.iCbSOQN2 AS iCbSOQN2,");
                strSql.AppendLine("		CASE WHEN T1.iTzhSOQN IS NULL THEN ISNULL(T8.iTzhSOQN,ISNULL(T1.iCbSOQN,0)) ELSE ISNULL(T1.iTzhSOQN,0) END AS iTzhSOQN,");
                strSql.AppendLine("		CASE WHEN T1.iTzhSOQN1 IS NULL THEN ISNULL(T8.iTzhSOQN1,ISNULL(T1.iCbSOQN1,0)) ELSE ISNULL(T1.iTzhSOQN1,0) END AS iTzhSOQN1,");
                strSql.AppendLine("		CASE WHEN T1.iTzhSOQN2 IS NULL THEN ISNULL(T8.iTzhSOQN2,ISNULL(T1.iCbSOQN2,0)) ELSE ISNULL(T1.iTzhSOQN2,0) END AS iTzhSOQN2,");
                strSql.AppendLine("		T1.iHySOQN AS iHySOQN,");
                strSql.AppendLine("		T1.iHySOQN1 AS iHySOQN1,");
                strSql.AppendLine("		T1.iHySOQN2 AS iHySOQN2,");
                strSql.AppendLine("		CASE WHEN T1.dExpectTime IS NULL THEN '' ELSE CONVERT(VARCHAR(10),T1.dExpectTime,23) END AS dExpectTime,");
                strSql.AppendLine("		CASE WHEN T1.dSReplyTime IS NULL THEN '' ELSE CONVERT(VARCHAR(10),T1.dSReplyTime,120) END AS dSReplyTime,");
                strSql.AppendLine("		CASE WHEN T1.dExpectTime IS NULL THEN '' ELSE (");
                strSql.AppendLine("			CASE WHEN T1.dSReplyTime IS NULL THEN (CASE WHEN CONVERT(VARCHAR(10),T1.dExpectTime,23)>=CONVERT(VARCHAR(10),GETDATE(),23) THEN '' ELSE '逾期' END) ");
                strSql.AppendLine("				ELSE (CASE WHEN CONVERT(VARCHAR(10),T1.dSReplyTime,23)<=CONVERT(VARCHAR(10),T1.dExpectTime,23) THEN '' ELSE '逾期' END) END ");
                strSql.AppendLine("		) END AS vcOverDue,		");
                strSql.AppendLine("		CASE WHEN T1.dHyTime IS NULL THEN '' ELSE CONVERT(VARCHAR(10),T1.dHyTime,120) END AS dHyTime");
                strSql.AppendLine("		from ");
                strSql.AppendLine("(select * from TSoq ");
                strSql.AppendLine("where 1=1 and vcDyState in (" + strDyInfo + ") and vcHyState in (" + strHyInfo + ") ");
                if (strYearMonth != "")
                {
                    strSql.AppendLine("and vcYearMonth='" + strYearMonth + "' ");
                }
                if (strDyState != "")
                {
                    strSql.AppendLine("and vcDyState ='" + strDyState + "'");
                }
                if (strHyState != "")
                {
                    strSql.AppendLine("and vcHyState ='" + strHyState + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("and vcPart_id like '%" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("and vcCarFamilyCode='" + strCarModel + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("and vcInOutFlag='" + strInOut + "'");
                }
                if (strOrderingMethod != "")
                {
                    strSql.AppendLine("and vcMakingOrderType='" + strOrderingMethod + "'");
                }
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("and vcFZGC='" + strOrderPlant + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("and vcCurrentPastcode='" + strHaoJiu + "'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("and vcSupplier_id='" + strSupplierId + "'");
                }
                if (strSupplierPlant != "")
                {
                    strSql.AppendLine("and vcSupplierPlant='" + strSupplierPlant + "'");
                }
                strSql.AppendLine(")T1 ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T2--内外区分 ");
                strSql.AppendLine("on T1.vcInOutFlag=T2.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T3--号旧区分 ");
                strSql.AppendLine("on T1.vcCurrentPastcode=T3.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T4--订货方式 ");
                strSql.AppendLine("on T1.vcMakingOrderType=T4.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T5--发注工厂 ");
                strSql.AppendLine("on T1.vcFZGC=T5.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C036')T6--对应状态 ");
                strSql.AppendLine("on T1.vcDyState=T6.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C037')T7--合意状态 ");
                strSql.AppendLine("on T1.vcHyState=T7.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select a.vcYearMonth,a.vcPart_id,a.iTzhSOQN,a.iTzhSOQN1,a.iTzhSOQN2      ");
                strSql.AppendLine("from  ");
                strSql.AppendLine("(select * from TSoq_OperHistory  ");
                strSql.AppendLine("where vcInputType in ('supplier','company'))a     ");
                strSql.AppendLine("inner join  ");
                strSql.AppendLine("(select vcYearMonth,vcPart_id,MAX(dOperatorTime) as dOperatorTime from TSoq_OperHistory ");
                strSql.AppendLine("where vcInputType in ('supplier','company')   ");
                strSql.AppendLine("group by vcYearMonth,vcPart_id     ");
                strSql.AppendLine(")b on a.vcYearMonth=b.vcYearMonth and a.vcPart_id=b.vcPart_id  and a.dOperatorTime=b.dOperatorTime)T8 ");
                strSql.AppendLine("on T1.vcYearMonth=t8.vcYearMonth and t1.vcPart_id=t8.vcPart_id");
                strSql.AppendLine(")TT");
                strSql.AppendLine("order by vcYearMonth,vcSupplierId,vcSupplierPlant,vcPart_id");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void checkSaveInfo(DataTable dtInfo, string strYearMonth, string strYearMonth1, string strYearMonth2,
            string strOperId, string strPackingPlant, string strReceiver, ref DataTable dtMessage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" delete TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "';");
                #region 插入临时表
                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {
                    strSql.AppendLine("INSERT INTO [dbo].[TSoq_temp]");
                    strSql.AppendLine("           ([vcYearMonth]");
                    strSql.AppendLine("           ,[vcDyState]");
                    strSql.AppendLine("           ,[vcHyState]");
                    strSql.AppendLine("           ,[vcPart_id]");
                    strSql.AppendLine("           ,[iCbSOQN]");
                    strSql.AppendLine("           ,[iCbSOQN1]");
                    strSql.AppendLine("           ,[iCbSOQN2]");
                    strSql.AppendLine("           ,[iTzhSOQN]");
                    strSql.AppendLine("           ,[iTzhSOQN1]");
                    strSql.AppendLine("           ,[iTzhSOQN2]");
                    strSql.AppendLine("           ,[vcSupplier_id]");
                    strSql.AppendLine("           ,[vcOperator]");
                    strSql.AppendLine("           ,[dOperatorTime])");
                    strSql.AppendLine("     VALUES");
                    strSql.AppendLine("           ('" + dtInfo.Rows[i]["vcYearMonth"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["vcDyState"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["vcHyState"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["vcPart_id"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["iCbSOQN"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["iCbSOQN1"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["iCbSOQN2"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["iTzhSOQN"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["iTzhSOQN1"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["iTzhSOQN2"].ToString() + "'");
                    strSql.AppendLine("           ,'" + dtInfo.Rows[i]["vcSupplierId"].ToString() + "'");
                    strSql.AppendLine("           ,'" + strOperId + "'");
                    strSql.AppendLine("           ,GETDATE())");
                }
                excute.ExcuteSqlWithStringOper(strSql.ToString());//先导入临时表，然后check
                #endregion

                #region 验证1：是否为TFTM品番（包装工厂）
                strSql.Length = 0;//清空
                strSql.AppendLine("select a.vcPart_id from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' )a ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TSPMaster where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' )b");
                strSql.AppendLine("on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId ");
                strSql.AppendLine("where b.vcPartId is  null");
                DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在采购品番基础情报中不存在", dt1.Rows[i]["vcPart_id"].ToString());
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证2：N 月品番有效性(N月得有数量),有数量才校验  
                strSql.Length = 0;//清空
                strSql.AppendLine("select a.vcPart_id from     ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN<>0)a     ");
                strSql.AppendLine("inner join       ");
                strSql.AppendLine("(select * from TSPMaster where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))b ");
                strSql.AppendLine("on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId        ");
                strSql.AppendLine("where b.vcPartId is  null       ");
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在采购品番基础情报中存在,但是在{1}不是有效品番", dt2.Rows[i]["vcPart_id"].ToString(), strYearMonth);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证3：N+1 月品番有效性(N+1月得有数量)，有数量才校验
                strSql.Length = 0;//清空
                strSql.AppendLine("select a.vcPart_id from    ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN1<>0)a    ");
                strSql.AppendLine("inner join      ");
                strSql.AppendLine("(select * from TSPMaster where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))b");
                strSql.AppendLine("on a.vcPart_id=b.vcPartId  and a.vcSupplier_id=b.vcSupplierId       ");
                strSql.AppendLine("where b.vcPartId is  null      ");
                DataTable dt3 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在采购品番基础情报中存在,但是在{1}不是有效品番", dt3.Rows[i]["vcPart_id"].ToString(), strYearMonth1);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证4：N+2 月品番有效性(N+2月得有数量)，有数量才校验 
                strSql.Length = 0;//清空
                strSql.AppendLine("select a.vcPart_id from    ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN2<>0)a    ");
                strSql.AppendLine("inner join      ");
                strSql.AppendLine("(select * from TSPMaster where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))b");
                strSql.AppendLine("on a.vcPart_id=b.vcPartId  and a.vcSupplier_id=b.vcSupplierId       ");
                strSql.AppendLine("where b.vcPartId is  null      ");
                DataTable dt4 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt4.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在采购品番基础情报中存在,但是在{1}不是有效品番", dt4.Rows[i]["vcPart_id"].ToString(), strYearMonth2);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证5：是否有价格，且在有效期内(只判断N月)，数量为0不校验；如果是强制订货，则没有价格也可以
                strSql.Length = 0;//清空
                strSql.AppendLine("select a.vcPart_id from     ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN<>0)a     ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPart_id,vcSupplier_id from TPrice where  convert(varchar(6),dUseBegin,112)<='" + strYearMonth + "' and convert(varchar(6),dUseEnd,112)>='" + strYearMonth + "')b ");
                strSql.AppendLine("on a.vcPart_id=b.vcPart_id  and a.vcSupplier_id=b.vcSupplier_id      ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcMandOrder,vcSupplierId from TSPMaster where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))c");
                strSql.AppendLine("on a.vcPart_id=c.vcPartId  and a.vcSupplier_id=c.vcSupplierId");
                strSql.AppendLine("where b.vcPart_id is  null  and isnull(c.vcMandOrder,'')<>'1'");
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt5.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护价格", dt5.Rows[i]["vcPart_id"].ToString(), strYearMonth);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证6：手配中是否有受入、收容数、发注工厂（N、N+1、N+2都判断），数量为0不校验
                #region N月的
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,t3.vcOrderPlant,t5.iPackingQty,t6.vcSufferIn    ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN<>0)t1        ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut from TSPMaster ");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId     ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant from TSPMaster_OrderPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t3 ");
                strSql.AppendLine("on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4 ");
                strSql.AppendLine("on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5 ");
                strSql.AppendLine("on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId and t4.vcSupplierPlant=t5.vcSupplierPlant    ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t6 ");
                strSql.AppendLine("on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    ");
                strSql.AppendLine("where t3.vcPartId is null or t5.vcPartId is null or t6.vcPartId is null");
                DataTable dt6_1 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt6_1.Rows.Count; i++)
                {
                    string strPart_id = dt6_1.Rows[i]["vcPart_id"].ToString();
                    string strOrderPlant = dt6_1.Rows[i]["vcOrderPlant"].ToString();//发注工厂
                    string strPackingQty = dt6_1.Rows[i]["iPackingQty"].ToString();//收容数
                    string strSufferIn = dt6_1.Rows[i]["vcSufferIn"].ToString();//受入
                    if (strSufferIn == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护受入信息", strPart_id, strYearMonth);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strPackingQty == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护收容数信息", strPart_id, strYearMonth);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strOrderPlant == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护发注工厂信息", strPart_id, strYearMonth);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                #endregion
                #region N+1月的
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,t3.vcOrderPlant,t5.iPackingQty,t6.vcSufferIn    ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN1<>0)t1        ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut from TSPMaster     ");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2 ");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant from TSPMaster_OrderPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t3 ");
                strSql.AppendLine("on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4 ");
                strSql.AppendLine("on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5 ");
                strSql.AppendLine("on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId and t4.vcSupplierPlant=t5.vcSupplierPlant    ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t6 ");
                strSql.AppendLine("on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    ");
                strSql.AppendLine("where t3.vcPartId is null or t5.vcPartId is null or t6.vcPartId is null");
                DataTable dt6_2 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt6_2.Rows.Count; i++)
                {
                    string strPart_id = dt6_2.Rows[i]["vcPart_id"].ToString();
                    string strOrderPlant = dt6_2.Rows[i]["vcOrderPlant"].ToString();//发注工厂
                    string strPackingQty = dt6_2.Rows[i]["iPackingQty"].ToString();//收容数
                    string strSufferIn = dt6_2.Rows[i]["vcSufferIn"].ToString();//受入
                    if (strSufferIn == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护受入信息", strPart_id, strYearMonth1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strPackingQty == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护收容数信息", strPart_id, strYearMonth1);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strOrderPlant == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护发注工厂信息", strPart_id, strYearMonth1);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                #endregion
                #region N+2月的
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,t3.vcOrderPlant,t5.iPackingQty,t6.vcSufferIn    ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN1<>0)t1        ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut from TSPMaster     ");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2 ");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant from TSPMaster_OrderPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t3 ");
                strSql.AppendLine("on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4 ");
                strSql.AppendLine("on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5 ");
                strSql.AppendLine("on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId and t4.vcSupplierPlant=t5.vcSupplierPlant    ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t6 ");
                strSql.AppendLine("on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    ");
                strSql.AppendLine("where t3.vcPartId is null or t5.vcPartId is null or t6.vcPartId is null");
                DataTable dt6_3 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt6_3.Rows.Count; i++)
                {
                    string strPart_id = dt6_3.Rows[i]["vcPart_id"].ToString();
                    string strOrderPlant = dt6_3.Rows[i]["vcOrderPlant"].ToString();//发注工厂
                    string strPackingQty = dt6_3.Rows[i]["iPackingQty"].ToString();//收容数
                    string strSufferIn = dt6_3.Rows[i]["vcSufferIn"].ToString();//受入
                    if (strSufferIn == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护受入信息", strPart_id, strYearMonth2);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strPackingQty == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护收容数信息", strPart_id, strYearMonth2);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strOrderPlant == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在{1}月未维护发注工厂信息", strPart_id, strYearMonth2);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                #endregion
                #endregion

                #region 验证7：受入、收容数、发注工厂、供应商工区：3个月必须一样
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,");
                strSql.AppendLine("t3.vcOrderPlant,t3_1.vcOrderPlant as vcOrderPlant_1,t3_2.vcOrderPlant as vcOrderPlant_2,");
                strSql.AppendLine("t5.iPackingQty,t5_1.iPackingQty as iPackingQty_1,t5_2.iPackingQty as iPackingQty_2,    ");
                strSql.AppendLine("t6.vcSufferIn, t6_1.vcSufferIn as vcSufferIn_1,t6_2.vcSufferIn as vcSufferIn_2   ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "' and (iTzhSOQN<>0 or iTzhSOQN1<>0 or iTzhSOQN2<>0))t1        ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut from TSPMaster ");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2 ");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId   ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant from TSPMaster_OrderPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t3 ");
                strSql.AppendLine("on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant from TSPMaster_OrderPlant");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t3_1 ");
                strSql.AppendLine("on t2.vcPartId=t3_1.vcPartId and t2.vcPackingPlant=t3_1.vcPackingPlant and t2.vcReceiver=t3_1.vcReceiver and t2.vcSupplierId=t3_1.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant from TSPMaster_OrderPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t3_2 ");
                strSql.AppendLine("on t2.vcPartId=t3_2.vcPartId and t2.vcPackingPlant=t3_2.vcPackingPlant and t2.vcReceiver=t3_2.vcReceiver and t2.vcSupplierId=t3_2.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4 ");
                strSql.AppendLine("on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4_1 ");
                strSql.AppendLine("on t2.vcPartId=t4_1.vcPartId and t2.vcPackingPlant=t4_1.vcPackingPlant and t2.vcReceiver=t4_1.vcReceiver and t2.vcSupplierId=t4_1.vcSupplierId    ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4_2 ");
                strSql.AppendLine("on t2.vcPartId=t4_2.vcPartId and t2.vcPackingPlant=t4_2.vcPackingPlant and t2.vcReceiver=t4_2.vcReceiver and t2.vcSupplierId=t4_2.vcSupplierId    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5 ");
                strSql.AppendLine("on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId and t4.vcSupplierPlant=t5.vcSupplierPlant    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5_1 ");
                strSql.AppendLine("on t2.vcPartId=t5_1.vcPartId and t2.vcPackingPlant=t5_1.vcPackingPlant and t2.vcReceiver=t5_1.vcReceiver and t2.vcSupplierId=t5_1.vcSupplierId and t4_1.vcSupplierPlant=t5_1.vcSupplierPlant    ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box     ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5_2 ");
                strSql.AppendLine("on t2.vcPartId=t5_2.vcPartId and t2.vcPackingPlant=t5_2.vcPackingPlant and t2.vcReceiver=t5_2.vcReceiver and t2.vcSupplierId=t5_2.vcSupplierId and t4_2.vcSupplierPlant=t5_2.vcSupplierPlant    ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t6 ");
                strSql.AppendLine("on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn where vcOperatorType='1' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t6_1");
                strSql.AppendLine("on t2.vcPartId=t6_1.vcPartId and t2.vcPackingPlant=t6_1.vcPackingPlant and t2.vcReceiver=t6_1.vcReceiver and t2.vcSupplierId=t6_1.vcSupplierId    ");
                strSql.AppendLine("left join     ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn where vcOperatorType='1' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t6_2");
                strSql.AppendLine("on t2.vcPartId=t6_2.vcPartId and t2.vcPackingPlant=t6_2.vcPackingPlant and t2.vcReceiver=t6_2.vcReceiver and t2.vcSupplierId=t6_2.vcSupplierId    ");
                strSql.AppendLine("where (t3.vcOrderPlant<>t3_1.vcOrderPlant or t3.vcOrderPlant<>t3_2.vcOrderPlant) ");
                strSql.AppendLine("or (t5.iPackingQty<>t5_1.iPackingQty or t5.iPackingQty<>t5_2.iPackingQty)");
                strSql.AppendLine("or (t6.vcSufferIn<>t6_1.vcSufferIn or t6.vcSufferIn<>t6_2.vcSufferIn) ");
                strSql.AppendLine("or (t4.vcSupplierPlant<>t4_1.vcSupplierPlant or t4.vcSupplierPlant<>t4_2.vcSupplierPlant)");
                DataTable dt7 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt7.Rows.Count; i++)
                {
                    string strPart_id = dt7.Rows[i]["vcPart_id"].ToString();
                    string vcOrderPlant = dt7.Rows[i]["vcOrderPlant"].ToString();//发注工厂 N月
                    string vcOrderPlant_1 = dt7.Rows[i]["vcOrderPlant_1"].ToString();//发注工厂 N+1月
                    string vcOrderPlant_2 = dt7.Rows[i]["vcOrderPlant_2"].ToString();//发注工厂 N+2月
                    string iPackingQty = dt7.Rows[i]["iPackingQty"].ToString();//收容数 N月
                    string iPackingQty_1 = dt7.Rows[i]["iPackingQty_1"].ToString();//收容数 N+1月
                    string iPackingQty_2 = dt7.Rows[i]["iPackingQty_2"].ToString();//收容数 N+2月
                    string vcSufferIn = dt7.Rows[i]["vcSufferIn"].ToString();//受入 N月
                    string vcSufferIn_1 = dt7.Rows[i]["vcSufferIn_1"].ToString();//受入 N+1月
                    string vcSufferIn_2 = dt7.Rows[i]["vcSufferIn_2"].ToString();//受入 N+2月
                    string vcSupplierPlant = dt7.Rows[i]["vcSupplierPlant"].ToString();//供应商工区 N月
                    string vcSupplierPlant_1 = dt7.Rows[i]["vcSupplierPlant_1"].ToString();//供应商工区 N+1月
                    string vcSupplierPlant_2 = dt7.Rows[i]["vcSupplierPlant_2"].ToString();//供应商工区 N+2月
                    if (vcOrderPlant != vcOrderPlant_1 || vcOrderPlant != vcOrderPlant_2)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在3个月维护的发注工厂不一致", strPart_id);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (iPackingQty != iPackingQty_1 || iPackingQty != iPackingQty_2)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在3个月维护的收容数不一致", strPart_id);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (vcSufferIn != vcSufferIn_1 || vcSufferIn != vcSufferIn_2)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在3个月维护的受入不一致", strPart_id);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (vcSupplierPlant != vcSupplierPlant_1 || vcSupplierPlant != vcSupplierPlant_2)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品番{0}在3个月维护的供应商工区不一致", strPart_id);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                #endregion

                #region 验证8：品番3个月数量不能全为0
                strSql.Length = 0;//清空
                strSql.AppendLine("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp ");
                strSql.AppendLine("where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "'");
                strSql.AppendLine("and ISNULL(iTzhSOQN,0)=0 and ISNULL(iTzhSOQN1,0)=0 and ISNULL(iTzhSOQN2,0)=0");
                DataTable dt8 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt8.Rows.Count; i++)
                {
                    string strPart_id = dt8.Rows[i]["vcPart_id"].ToString();
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}的3个月数量不能全部为0", strPart_id);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证8-1：调整3个月数量加总必须等于初版3个月数量加总
                strSql.Length = 0;//清空
                strSql.AppendLine("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp ");
                strSql.AppendLine("where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "'");
                strSql.AppendLine("and ISNULL(iTzhSOQN,0)+ISNULL(iTzhSOQN1,0)+ISNULL(iTzhSOQN2,0)!=ISNULL(iCbSOQN,0)+ISNULL(iCbSOQN1,0)+ISNULL(iCbSOQN2,0)");
                DataTable dt8_1 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt8_1.Rows.Count; i++)
                {
                    string strPart_id = dt8_1.Rows[i]["vcPart_id"].ToString();
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}的3个月调整数量总和与初版不一致", strPart_id);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证9：收容数整倍数
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,");
                strSql.AppendLine("t1.iTzhSOQN%t5.iPackingQty as iTzhSOQN,");
                strSql.AppendLine("t1.iTzhSOQN1%t5.iPackingQty as iTzhSOQN1,    ");
                strSql.AppendLine("t1.iTzhSOQN2%t5.iPackingQty as iTzhSOQN2    ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "')t1            ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut from TSPMaster  ");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId       ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant from TSPMaster_SupplierPlant         ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t4 ");
                strSql.AppendLine("on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId        ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty from TSPMaster_Box         ");
                strSql.AppendLine("where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t5 ");
                strSql.AppendLine("on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant         ");
                strSql.AppendLine("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
                strSql.AppendLine("and t4.vcSupplierPlant=t5.vcSupplierPlant        ");
                strSql.AppendLine("where t1.iTzhSOQN%t5.iPackingQty<>0 or t1.iTzhSOQN1%t5.iPackingQty<>0 or t1.iTzhSOQN2%t5.iPackingQty<>0");
                DataTable dt9 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt9.Rows.Count; i++)
                {
                    string strPart_id = dt9.Rows[i]["vcPart_id"].ToString();
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}数量不是收容数的整数倍", strPart_id);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion

                #region 验证10：if一括生产 校验： 对象月 >= 实施年月时间 不能订货(数量得是0) 3个月都校验
                #region N月
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,t2.dDebugTime");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and  vcYearMonth='" + strYearMonth + "')t1");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,dDebugTime,vcSupplierId from TSPMaster ");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)");
                strSql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId");
                strSql.AppendLine("where '" + strYearMonth + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iTzhSOQN>0");
                DataTable dt10 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt10.Rows.Count; i++)
                {
                    string strPart_id = dt10.Rows[i]["vcPart_id"].ToString();
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在{1}月不能订货", strPart_id, strYearMonth);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion
                #region N+1月
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,t2.dDebugTime    ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "')t1            ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,dDebugTime,vcSupplierId from TSPMaster");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth1 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId     ");
                strSql.AppendLine("where '" + strYearMonth1 + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iTzhSOQN1>0    ");
                DataTable dt10_1 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt10_1.Rows.Count; i++)
                {
                    string strPart_id = dt10_1.Rows[i]["vcPart_id"].ToString();
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在{1}月不能订货", strPart_id, strYearMonth1);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion
                #region N+2月
                strSql.Length = 0;//清空
                strSql.AppendLine("select t1.vcPart_id,t2.dDebugTime    ");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from TSoq_temp where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "')t1            ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select vcPartId,dDebugTime,vcSupplierId from TSPMaster");
                strSql.AppendLine("where vcPackingPlant='" + strPackingPlant + "' and vcReceiver='" + strReceiver + "' and '" + strYearMonth2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112))t2");
                strSql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId     ");
                strSql.AppendLine("where '" + strYearMonth2 + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iTzhSOQN2>0    ");
                DataTable dt10_2 = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                for (int i = 0; i < dt10_2.Rows.Count; i++)
                {
                    string strPart_id = dt10_2.Rows[i]["vcPart_id"].ToString();
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("品番{0}在{1}月不能订货", strPart_id, strYearMonth2);
                    dtMessage.Rows.Add(dataRow);
                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setSaveInfo(string strYearMonth, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                DateTime now = DateTime.Now;
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("--插历史");
                strSql.AppendLine("insert into TSoq_OperHistory (vcYearMonth,vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,vcInputType,vcOperator,dOperatorTime)");
                strSql.AppendLine("select vcYearMonth,vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,'company',vcOperator,GETDATE() from TSoq_temp where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strOperId + "'");
                strSql.AppendLine("--更新调整后字段");
                strSql.AppendLine("update t2 set t2.iTzhSOQN=t1.iTzhSOQN,t2.iTzhSOQN1=t1.iTzhSOQN1,t2.iTzhSOQN2=t1.iTzhSOQN2,vcOperator='" + strOperId + "',dOperatorTime=GETDATE(),vcLastTimeFlag='" + strLastTimeFlag + "'");
                strSql.AppendLine("from");
                strSql.AppendLine("(select * from TSoq_temp where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strOperId + "')t1 ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select * from  TSoq where vcYearMonth='" + strYearMonth + "')t2 ");
                strSql.AppendLine("on t1.vcYearMonth=t2.vcYearMonth and t1.vcPart_id=t2.vcPart_id");
                strSql.AppendLine("where t1.iTzhSOQN<>t2.iTzhSOQN or t1.iTzhSOQN1<>t2.iTzhSOQN1 or t1.iTzhSOQN2<>t2.iTzhSOQN2");
                strSql.AppendLine("--走到保存，则异常信息肯定没有了，删除TSoqInputErrDetail_Save");
                strSql.AppendLine("delete TSoqInputErrDetail_Save where vcOperator='" + strOperId + "' and vcYearMonth='" + strYearMonth + "'");
                strSql.AppendLine("--记录日志");
                strSql.AppendLine("INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.AppendLine("select vcYearMonth,vcPart_id,'TFTM保存成功','" + strOperId + "',getDate() from TSoq  where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strOperId + "'");
                excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
            }
        }

        public void setSaveInfo_op(DataTable dtImport, string strDyState, string strHyState, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE [dbo].[TSoq]");
                strSql_modinfo.AppendLine("   SET [vcDyState] = '" + strDyState + "'");
                strSql_modinfo.AppendLine("      ,[dExpectTime] = @dExpectTime");
                strSql_modinfo.AppendLine("      ,[dOpenTime] = GETDATE()");
                strSql_modinfo.AppendLine("      ,[vcOpenUser] = '" + strOperId + "'");
                strSql_modinfo.AppendLine(" WHERE [vcYearMonth] = @vcYearMonth");
                strSql_modinfo.AppendLine(" AND [vcPart_id] = @vcPart_id and vcHyState in ('0','3') and vcDyState='0'");
                //strSql_modinfo.AppendLine(" declare @flag int");
                //strSql_modinfo.AppendLine(" set @flag=(select Count(*) from [TSoq_OperHistory] where vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id)");
                //strSql_modinfo.AppendLine(" if(@flag=0)");
                //strSql_modinfo.AppendLine(" begin");
                strSql_modinfo.AppendLine(" INSERT INTO [dbo].[TSoq_OperHistory]([vcYearMonth],[vcPart_id],[iTzhSOQN],[iTzhSOQN1],[iTzhSOQN2],[vcInputType],[vcOperator],[dOperatorTime])");
                strSql_modinfo.AppendLine(" select [vcYearMonth],[vcPart_id],iCbSOQN,iCbSOQN1,iCbSOQN2,'supplier' as [vcInputType],'" + strOperId + "' as [vcOperator],GETDATE() as [dOperatorTime] ");
                strSql_modinfo.AppendLine(" from TSoq_temp where vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id and vcHyState in ('0','3') and vcDyState='0' AND vcOperator='" + strOperId + "' ");
                //strSql_modinfo.AppendLine(" end");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@dExpectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@dExpectTime"].Value = item["dExpectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        public DataTable getEmail(string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcLinkMan,vcEmail from [dbo].[TSupplierInfo] where vcSupplier_id in (" + vcSupplier_id + ")");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo_rp(DataTable dtImport, string strDyState, string strHyState, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("update T1 SET");
                strSql_modinfo.AppendLine("	 T1.[vcHyState]='" + strHyState + "'");
                strSql_modinfo.AppendLine("	,T1.iTzhSOQN=ISNULL(T2.iTzhSOQN,0)");
                strSql_modinfo.AppendLine("	,T1.iTzhSOQN1=ISNULL(T2.iTzhSOQN1,0)");
                strSql_modinfo.AppendLine("	,T1.iTzhSOQN2=ISNULL(T2.iTzhSOQN2,0)");
                strSql_modinfo.AppendLine("	,T1.dReplyTime=GETDATE()");
                strSql_modinfo.AppendLine("	,T1.vcReplyUser='" + strOperId + "' from ");
                strSql_modinfo.AppendLine("(select * from TSoq ");
                strSql_modinfo.AppendLine("where 1=1 and vcDyState in ('0','1','2','3') and vcHyState in ('0','3') ");
                strSql_modinfo.AppendLine("and vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id )T1");
                strSql_modinfo.AppendLine("LEFT JOIN");
                strSql_modinfo.AppendLine("(SELECT * from TSoq_temp");
                strSql_modinfo.AppendLine("where vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id and vcDyState in ('0','1','2','3') and vcHyState in ('0','3') AND vcOperator='" + strOperId + "')T2");
                strSql_modinfo.AppendLine("ON T1.vcYearMonth=T2.vcYearMonth AND T1.vcPart_id=T2.vcPart_id ");
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TSoq_OperHistory]([vcYearMonth],[vcPart_id],[iTzhSOQN],[iTzhSOQN1],[iTzhSOQN2],[vcInputType],[vcOperator],[dOperatorTime])");
                strSql_modinfo.AppendLine("select [vcYearMonth],[vcPart_id],iCbSOQN,iCbSOQN1,iCbSOQN2,'company' as [vcInputType],'" + strOperId + "' as [vcOperator],GETDATE() as [dOperatorTime] ");
                strSql_modinfo.AppendLine("from TSoq_temp where vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id and vcDyState in ('0','1','2','3') and vcHyState in ('0','3') AND vcOperator='" + strOperId + "' ");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        public void setSaveInfo_rn(string strReturnym, string strDyState, string strHyState, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("DELETE FROM [TSoq]  WHERE [vcYearMonth] = @vcYearMonth ");
                strSql_modinfo.AppendLine("DELETE FROM TSoq_OperHistory WHERE [vcYearMonth] = @vcYearMonth ");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = strReturnym.ToString();
                sqlCommand_modinfo.ExecuteNonQuery();
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        
    }
}