﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS0705_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
 
        /// <summary>
        /// 调整页面
        /// </summary>


        #region 按检索条件检索,返回dt
        public DataTable Search_TiaoZheng(string PackGPSNo, string PackNo, string TiaoZhengType, string dFromB, string dToE)
        {
            try
            {

                if (string.IsNullOrEmpty(dFromB))
                {
                    dFromB = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dToE))
                {
                    dToE = "3000-01-01 0:00:00";

                }
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackFaZhu_TiaoZheng");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (!string.IsNullOrEmpty(TiaoZhengType))
                    strSql.AppendLine($"      AND vcType = '{TiaoZhengType}'");
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                strSql.AppendLine($"      AND dTime BETWEEN '{dFromB}' and '{dToE}'");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 保存
        public void Save_TiaoZheng(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.AppendLine("     INSERT INTO TPackFaZhu_TiaoZheng( ");
                        sql.AppendLine("      vcPackNo,");
                        sql.AppendLine("      vcPackGPSNo,");
                        sql.AppendLine("      iNumber,");
                        sql.AppendLine("      vcType,");
                        sql.AppendLine("      dTime,");
                        sql.AppendLine("      vcReason,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iNumber"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcType"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");


                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackFaZhu_TiaoZheng");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPackNo = '{ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)}',");
                        sql.AppendLine($"   vcPackGPSNo = '{ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false)}',");
                        sql.AppendLine($"   iNumber = '{ComFunction.getSqlValue(listInfoData[i]["iNumber"], true)}',");
                        sql.AppendLine($"   vcType = '{ComFunction.getSqlValue(listInfoData[i]["vcType"], false)}',");
                        sql.AppendLine($"   dTime = '{ComFunction.getSqlValue(listInfoData[i]["dTime"], true)}',");
                        sql.AppendLine($"   vcReason = '{ComFunction.getSqlValue(listInfoData[i]["vcReason"], false)}',");
                        sql.AppendLine($"   vcOperatorID = '{strUserId}',");
                        sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                        sql.AppendLine($"  WHERE");
                        sql.AppendLine($"  iAutoId='{iAutoId}';");


                    }
                    excute.ExcuteSqlWithStringOper(sql.ToString());

                }

            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion
 

        #region 获取系统当前需要计算的数据范围
        public DataTable SearchFaZhuLastTime(string strPackSpot)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("     select a.vcPackSpot,a.vcFaZhuID,b.dEnd  from     \n");
                strSql.AppendLine("     (     \n");
                strSql.AppendLine("        select distinct vcPackSpot,vcFaZhuID from TPackFaZhuTime  where vcPackSpot='"+ strPackSpot + "'     \n");
                strSql.AppendLine("     )a     \n");
                strSql.AppendLine("     left join     \n");
                strSql.AppendLine("     (     \n");
                strSql.AppendLine("        select vcPackSpot,vcFaZhuID,max(dEnd) as dEnd from TPackCompute_Time     \n");
                strSql.AppendLine("    	 group by  vcPackSpot,vcFaZhuID     \n");
                strSql.AppendLine("     )b on   a.vcPackSpot=b.vcPackSpot and a.vcFaZhuID=b.vcFaZhuID     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

 

        #region 发注数量计算
        public void computer(string strFaZhuID) 
        {
            StringBuilder strSql = new StringBuilder();
            string strFlag = DateTime.Now.ToString("yyyyMMddhhmmss");

            #region 删除未生成发注订单的数据
            strSql.AppendLine("        delete TPackCompute from TPackCompute a         ");
            strSql.AppendLine("        left join TPackCompute_Time b on a.vcFlag = b.vcFlag         ");
            strSql.AppendLine("        where a.vcOrderNo is null         ");
            strSql.AppendLine("        delete TPackCompute where vcOrderNo is null         ");
            #endregion

            strSql.Append("       declare @dBegin datetime      \r\n");
            strSql.Append("       declare @dEnd datetime      \r\n");
            strSql.Append("       set @dBegin=(select max(dEnd) from TPackCompute_Time where vcFaZhuID = '"+strFaZhuID+"');      \r\n");
            strSql.Append("       set @dEnd = (select MAX(cast(convert(varchar(10),getdate(),120)+' '+convert(varchar(50),druHeToTime) as datetime)) from TPackFaZhuTime where dFaZhuFromTime<=CONVERT(char(8),GETDATE(),108) and CONVERT(char(8),GETDATE(),108)<=dFaZhuToTime);      \r\n");
            strSql.Append("             \r\n");
            strSql.Append("       insert into TPackCompute(vcFaZhuID,dTimeStr,vcPackNo,vcPackGPSNo,iA_SRS,iB_LastShengYu,iC_LiLun,iD_TiaoZheng,iE_JinJi,iF_DingGou,iG_ShengYu,vcOperatorID,dOperatorTime,vcFlag)      \r\n");
            strSql.Append("       select a.*,(a.B+a.E+a.F-a.[C ]-a.D) as 'G','000000',GETDATE(),'"+ strFlag + "' from      \r\n");
            strSql.Append("       (      \r\n");
            strSql.Append("       	select '"+strFaZhuID+"' as vcFaZhuID,GETDATE() as vcTimeStr,a.vcPackNo as '包材品番',a.vcPackGPSNo,a.iRelease as 'A',b.iG_ShengYu as'B'       \r\n");
            strSql.Append("       	,c.iBiYao as 'C ',d.iNumber as 'D',e.iSJNumber as 'E',case when (b.iG_ShengYu+e.iSJNumber-c.iBiYao-d.iNumber)>=0 then 0 else (CEILING(c.iBiYao+d.iNumber-b.iG_ShengYu-e.iSJNumber)/a.iRelease)*a.iRelease end as 'F'      \r\n");
            strSql.Append("       	from      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select vcPackSpot,vcPackNo,iRelease,vcPackGPSNo from TPackBase where dPackFrom <= GETDATE() and dPackTo >= GETDATE()      \r\n");
            strSql.Append("       	) a      \r\n");
            strSql.Append("       	left join      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select a.vcPackNo,b.iG_ShengYu from      \r\n");
            strSql.Append("       		(      \r\n");
            strSql.Append("       			select MAX(iautoId) as MaxAutoId, vcPackNo from TPackCompute where vcFaZhuID = '"+strFaZhuID+"' group by vcFaZhuID,vcPackNo      \r\n");
            strSql.Append("       		)a      \r\n");
            strSql.Append("       		inner join       \r\n");
            strSql.Append("       		(      \r\n");
            strSql.Append("       			select iAutoId,vcPackNo,iG_ShengYu from TPackCompute      \r\n");
            strSql.Append("       		)b on a.MaxAutoId = b.iAutoId      \r\n");
            strSql.Append("       	) b on a.vcPackNo = b.vcPackNo      \r\n");
            strSql.Append("       	left join      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select vcBZPlant,vcPart_id,vcPackNo,SUM(iBiYao)as iBiYao from      \r\n");
            strSql.Append("       		(      \r\n");
            strSql.Append("       			select * from      \r\n");
            strSql.Append("       			(      \r\n");
            strSql.Append("       				select vcBZPlant,vcPart_id,dEnd from TOperateSJ where vcZYType='S0' and @dBegin<=dEnd and dEnd<=@dEnd      \r\n");
            strSql.Append("       			) a      \r\n");
            strSql.Append("       			left join      \r\n");
            strSql.Append("       			(      \r\n");
            strSql.Append("       				select * from TPackItem       \r\n");
            strSql.Append("       			) b on a.vcPart_id = b.vcPartsNo      \r\n");
            strSql.Append("       		) a group by vcPart_id,vcPackNo,vcBZPlant      \r\n");
            strSql.Append("       	) c on a.vcPackNo = c.vcPackNo and a.vcPackSpot = c.vcBZPlant      \r\n");
            strSql.Append("       	left join      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select vcPackNo,iNumber from TPackCompute_Ajust      \r\n");
            strSql.Append("       	) d on a.vcPackNo = d.vcPackNo      \r\n");
            strSql.Append("       	left join      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select a.vcPackSpot,a.vcPackNo,b.iSJNumber from       \r\n");
            strSql.Append("       		(      \r\n");
            strSql.Append("       			select vcPackSpot,vcPackNo from TPackOrderFaZhu where VCFaBuType = '1'      \r\n");
            strSql.Append("       		) a      \r\n");
            strSql.Append("       		left join      \r\n");
            strSql.Append("       		(      \r\n");
            strSql.Append("       			select vcPackNo,iSJNumber from TPack_FaZhu_ShiJi where dNaRuShiJi is not null and @dBegin<=dNaRuShiJi and dNaRuShiJi<=@dEnd      \r\n");
            strSql.Append("       		) b on a.vcPackNo = b.vcPackNo      \r\n");
            strSql.Append("       	) e on a.vcPackNo = e.vcPackNo and a.vcPackSpot = e.vcPackSpot      \r\n");
            strSql.Append("       ) a;      \r\n");
            #region 插入计算时间履历表
            strSql.Append("       insert into TPackCompute_Time(vcFaZhuID,dBegin,dEnd,vcOperatorID,dOperatorTime,vcFlag) values ('"+strFaZhuID+"',@dBegin,@dEnd,'000000',GETDATE(),'"+strFlag+"');      \r\n");
            #endregion
            excute.ExcuteSqlWithStringOper(strSql.ToString());
        }
        #endregion

        #region 检索计算结果
        public DataTable searchComputeJG()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendLine("            select * from TPackCompute where vcOrderNo is null         ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索计算结果
        public DataTable searchComputeJGAll()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendLine("        select * from TPackCompute         ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 生成发注数据
        /// <summary>
        /// 更新一条计算结果的订单号
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="strOrderNo"></param>
        public void SCFZData(DataTable dt,string strOrderNo)
        {
            StringBuilder strSql = new StringBuilder();
            int err = 0;
            try
            {
                #region 创建临时表
                strSql.AppendLine("        if object_id('tempdb..#TPackCompute_temp') is not null  ");
                strSql.AppendLine("        Begin  ");
                strSql.AppendLine("        drop  table #TPackCompute_temp  ");
                strSql.AppendLine("        End  ");
                strSql.AppendLine("        select * into #TPackCompute_temp from       ");
                strSql.AppendLine("        (      ");
                strSql.AppendLine("      	  select vcOrderNo from TPackCompute where 1=0      ");
                strSql.AppendLine("        ) a      ;");
                strSql.AppendLine("        ALTER TABLE #TPackCompute_temp ADD  iAutoId int     ;");
                #endregion

                #region 更新临时表的订单号
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    err = i;
                    if (err==256)
                    {
                        string ss = "";
                    }
                    string strNewOrderNo = getNewOrderNo(strOrderNo);
                    strOrderNo = strNewOrderNo;

                    if (i <= 0)
                    {
                        strSql.AppendLine("       insert into #TPackCompute_temp      ");
                        strSql.AppendLine("       (      ");
                        strSql.AppendLine("           iAutoId      ");
                        strSql.AppendLine("           ,vcOrderNo      ");
                        strSql.AppendLine("       )      ");
                        strSql.AppendLine("       VALUES      ");
                        strSql.AppendLine("       (      ");
                        strSql.AppendLine("           " + ComFunction.getSqlValue(dt.Rows[i]["iAutoId"], true) + "      ");
                        strSql.AppendLine("           ,'" + strOrderNo + "'      ");
                        strSql.AppendLine("       )      ");
                    }
                    else
                    {
                        strSql.AppendLine("       insert into #TPackCompute_temp      ");
                        strSql.AppendLine("       (      ");
                        strSql.AppendLine("           iAutoId      ");
                        strSql.AppendLine("           ,vcOrderNo      ");
                        strSql.AppendLine("       )      ");
                        strSql.AppendLine("       VALUES      ");
                        strSql.AppendLine("       (      ");
                        strSql.AppendLine("           " + ComFunction.getSqlValue(dt.Rows[i]["iAutoId"], true) + "      ");
                        strSql.AppendLine("           ,'" + strNewOrderNo + "'      ");
                        strSql.AppendLine("       )      ");
                    }

                }
                #endregion

                #region 临时表与计算结果表关联，更新计算结果表的订单号
                strSql.AppendLine("        UPDATE TPackCompute set vcOrderNo = b.vcOrderNo         ");
                strSql.AppendLine("        from TPackCompute  a         ");
                strSql.AppendLine("        inner join          ");
                strSql.AppendLine("        (         ");
                strSql.AppendLine("            select * from #TPackCompute_temp             ");
                strSql.AppendLine("        ) b on a.iAutoId = b.iAutoId         ");
                #endregion

                #region 将此次的数据结果插入包材发注表中
                strSql.AppendLine("       insert into TPackOrderFaZhu          ");
                strSql.AppendLine("       (          ");
                strSql.AppendLine("           vcOrderNo,          ");//
                strSql.AppendLine("           vcPackNo,           ");//
                strSql.AppendLine("           vcPackGPSNo,        ");//
                strSql.AppendLine("           vcPartName,         ");//
                strSql.AppendLine("           iOrderNumber,       ");//
                strSql.AppendLine("           vcIsorNoFaZhu,      ");//
                strSql.AppendLine("           VCFaBuType,         ");//
                strSql.AppendLine("           dNaRuTime,          ");
                strSql.AppendLine("           vcNaRuBianci,       ");
                strSql.AppendLine("           vcNaRuUnit,         ");
                strSql.AppendLine("           vcSupplierCode,     ");
                strSql.AppendLine("           vcSupplierName,     ");
                strSql.AppendLine("           vcBuShu,            ");
                strSql.AppendLine("           vcPackSpot,         ");//
                strSql.AppendLine("           vcCangKuCode,       ");
                strSql.AppendLine("           vcOperatorID,       ");
                strSql.AppendLine("           dOperatorTime,      ");
                strSql.AppendLine("       )                       ");
                strSql.AppendLine("       select                  ");
                strSql.AppendLine("       b.vcOrderNo             ");//
                strSql.AppendLine("       b.vcPackNo              ");//
                strSql.AppendLine("       b.vcPackGPSNo           ");//
                strSql.AppendLine("       c.vcParstName           ");//
                strSql.AppendLine("       b.iF_DingGou,           ");//
                strSql.AppendLine("       '0',                    ");//
                strSql.AppendLine("       '0',                    ");//
                strSql.AppendLine("       dNaRuTime,              ");
                strSql.AppendLine("       vcNaRuBianci,           ");
                strSql.AppendLine("       vcNaRuUnit,             ");
                strSql.AppendLine("       c.vcSupplierCode        ");
                strSql.AppendLine("       c.vcSupplierName        ");
                strSql.AppendLine("       vcBuShu,                ");
                strSql.AppendLine("       vcPackSpot,             ");//
                strSql.AppendLine("       vcCangKuCode,           ");
                strSql.AppendLine("       vcOperatorID,           ");
                strSql.AppendLine("       dOperatorTime,          ");
                strSql.AppendLine("                               ");
                strSql.AppendLine("       from          ");
                strSql.AppendLine("       (          ");
                strSql.AppendLine("           select * from #TPackCompute_temp          ");
                strSql.AppendLine("       ) a          ");
                strSql.AppendLine("       inner join          ");
                strSql.AppendLine("       (          ");
                strSql.AppendLine("           select * from TPackCompute          ");
                strSql.AppendLine("       )b on a.iAutoId = b.iAutoId          ");
                strSql.AppendLine("       left JOIN          ");
                strSql.AppendLine("       (          ");
                strSql.AppendLine("           select * from TPackBase          ");
                strSql.AppendLine("       ) c on b.vcPackNo = c.vcPackNo          ");
                strSql.AppendLine("       left JOIN          ");
                strSql.AppendLine("       (          ");
                strSql.AppendLine("           select * from TPackBase          ");
                strSql.AppendLine("       ) c on b.vcPackNo = c.vcPackNo          ");
                #endregion

                #region 将此次的数据结果插入包材实际表中
                strSql.AppendLine("        insert into TPack_FaZhu_ShiJi(         ");
                strSql.AppendLine("            vcOrderNo,         ");
                strSql.AppendLine("            vcPackNo,         ");
                strSql.AppendLine("            vcPackGPSNo,         ");
                strSql.AppendLine("            vcPartName,         ");
                strSql.AppendLine("            iOrderNumber,         ");
                strSql.AppendLine("            vcType,         ");
                strSql.AppendLine("            vcState,         ");
                strSql.AppendLine("            vcPackSupplierID,         ");
                strSql.AppendLine("            vcPackSpot         ");
                strSql.AppendLine("        )         ");
                strSql.AppendLine("        select b.vcOrderNo,         ");
                strSql.AppendLine("            b.vcPackNo,         ");
                strSql.AppendLine("            b.vcPackGPSNo,         ");
                strSql.AppendLine("            c.vcParstName,         ");
                strSql.AppendLine("            b.iF_DingGou,         ");
                strSql.AppendLine("            '0',         ");
                strSql.AppendLine("            '0',         ");
                strSql.AppendLine("            c.vcSupplierCode,         ");
                strSql.AppendLine("            c.vcPackSpot          ");
                strSql.AppendLine("        from(         ");
                strSql.AppendLine("            select * from #TPackCompute_temp         ");
                strSql.AppendLine("        )a          ");
                strSql.AppendLine("       inner join          ");
                strSql.AppendLine("       (          ");
                strSql.AppendLine("           select * from TPackCompute          ");
                strSql.AppendLine("       )b on a.iAutoId = b.iAutoId          ");
                strSql.AppendLine("        left JOIN(         ");
                strSql.AppendLine("            select * from TPackBase         ");
                strSql.AppendLine("        )c on b.vcPackNo = b.vcPackNo         ");
                #endregion

                excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 从发注订单表取得最大订单号
        public DataTable getMAXOrderNo()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("      select  MAX(vcOrderNo) from TPackOrderFaZhu       ");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        #endregion

        #region 获取新订单号
        public string getNewOrderNo(string strOrderNo)
        {
            string strOrderNoMAx = "";
            string strOrderNo_1 = strOrderNo.Substring(0, 1);
            string strOrderNo_2 = strOrderNo.Substring(1, 1);
            string strOrderNo_3 = strOrderNo.Substring(2, 5);
            if (strOrderNo_3 == "99999")
            {
                int iOrderNo_2 = (int)Convert.ToByte(strOrderNo_2[0]);

                if (strOrderNo_2 == "Z")
                {
                    int iOrderNo_1 = (int)Convert.ToByte(strOrderNo_1[0]);
                    strOrderNo_1 = Convert.ToChar(iOrderNo_1 + 1).ToString();
                    strOrderNo_2 = "A";
                    strOrderNo_3 = "00001";
                }
                else
                {
                    strOrderNo_3 = "00001";
                    strOrderNo_2 = Convert.ToChar(iOrderNo_2 + 1).ToString();
                }
            }
            else
            {
                int iOrderNo_3 = (int)Convert.ToInt32(strOrderNo_3) + 1;
                strOrderNo_3 = iOrderNo_3.ToString().PadLeft(5, '0');
            }
            strOrderNoMAx = strOrderNo_1 + strOrderNo_2 + strOrderNo_3;

            return strOrderNoMAx;

        }
        #endregion

        #region 取包装班值稼动
        public DataTable getPackBanZhi(string strYearMonth, int iDay)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("     select vcDay" + iDay.ToString("00") + " as vcBanZhi from TPackCalendar where vcYearMonth='" + strYearMonth + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 取工厂对应班值的时间范围
        public DataTable getBanZhi(string strPackPlant,string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from TBZTime where vcBanZhi='"+ strBanZhi + "' and vcPackPlant='" + strPackPlant + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
 


        #region 获取包材发注时间
        public DataTable getFaZhuTime(string strFaZhuID,string strPackSpot)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from TPackFaZhuTime where vcFaZhuID='"+ strFaZhuID + "' and vcPackSpot='"+ strPackSpot + "' order by charindex(vcBianCi,'白'),vcBianCi   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion










        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcPackSupplierCode,vcPackSupplierName from TPackSupplier;");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt----公式
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select *,'0' as vcModFlag,'0' as vcAddFlag from TPrice_GS         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if (strBegin != "")
                    strSql.Append("   and    dBegin>='" + strBegin + "'         \n");
                if (strEnd != "")
                    strSql.Append("   and    dEnd<='" + strEnd + "'         \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存-公式
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("  insert into TPrice_GS(vcName,vcGs,vcArea,dBegin,dEnd,vcReason,vcOperatorID,dOperatorTime   \r\n");
                        sql.Append("  )   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcName"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcGs"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcArea"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dBegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + ",  \r\n");
                        sql.Append("'" + strUserId + "',  \r\n");
                        sql.Append("getdate()  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice_GS set    \r\n");
                        sql.Append("  vcName=" + ComFunction.getSqlValue(listInfoData[i]["vcName"], false) + "   \r\n");
                        sql.Append("  ,vcGs=" + ComFunction.getSqlValue(listInfoData[i]["vcGs"], false) + "   \r\n");
                        sql.Append("  ,vcArea=" + ComFunction.getSqlValue(listInfoData[i]["vcArea"], false) + "   \r\n");
                        sql.Append("  ,dBegin=" + ComFunction.getSqlValue(listInfoData[i]["dBegin"], true) + "   \r\n");
                        sql.Append("  ,dEnd=" + ComFunction.getSqlValue(listInfoData[i]["dEnd"], true) + "   \r\n");
                        sql.Append("  ,vcReason=" + ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    }
                }
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorName varchar(50)   \r\n");
                    sql.Append("  set @errorName=''   \r\n");
                    sql.Append("  set @errorName=(   \r\n");
                    sql.Append("  	select a.vcName+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcName from TPrice_GS a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from TPrice_GS   \r\n");
                    sql.Append("  		)b on a.vcName=b.vcName and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		   and    \r\n");
                    sql.Append("  		   (   \r\n");
                    sql.Append("  			   (a.dBegin>=b.dBegin and a.dBegin<=b.dEnd)   \r\n");
                    sql.Append("  			   or   \r\n");
                    sql.Append("  			   (a.dEnd>=b.dBegin and a.dEnd<=b.dEnd)   \r\n");
                    sql.Append("  		   )   \r\n");
                    sql.Append("  		where b.iAutoId is not null   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorName<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorName+'<--')   \r\n");
                    sql.Append("  end    \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorName = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion









        #region 调整数据输入-检索
        public DataTable search_Sub(string strPackNo, string strPackGPSNo, string strFrom, string strTo, string strType)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendLine("        select *,'0' as selected,'0' as vcModFlag,'0' as AddFlag from  TPackCompute_Ajust         ");
                strSql.AppendLine("        where 1=1         ");
                if (!string.IsNullOrEmpty(strPackNo))
                {
                    strSql.AppendLine("        and vcPackNo = '" + strPackNo + "'         ");
                }
                if (!string.IsNullOrEmpty(strPackGPSNo))
                {
                    strSql.AppendLine("        and vcPackGPSNo = '" + strPackGPSNo + "'         ");
                }
                if (!string.IsNullOrEmpty(strFrom))
                {
                    strSql.AppendLine("        and '" + strFrom + "'<=dTime          ");
                }
                if (!string.IsNullOrEmpty(strTo))
                {
                    strSql.AppendLine("        and dTime<='" + strTo + "'         ");
                }
                if (!string.IsNullOrEmpty(strType) && strType != "0")
                {
                    strSql.AppendLine("        and vcType ='" + strType + "'         ");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 调整数据输入-保存
        public void save_Sub(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strSql.AppendLine("      insert into TPackCompute_Ajust(vcPackNo,vcPackGPSNo,iNumber,vcType,dTime,vcReason,vcOperatorID,dOperatorTime)       ");
                    strSql.AppendLine("      values       ");
                    strSql.AppendLine("      (       ");
                    strSql.AppendLine("       " + ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false) + "       ");
                    strSql.AppendLine("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false) + "       ");
                    strSql.AppendLine("      ," + ComFunction.getSqlValue(listInfoData[i]["iNumber"], true) + "       ");

                    #region 调整类别：1:调增    2:调减
                    int iNumber = Convert.ToInt32(listInfoData[i]["iNumber"].ToString());
                    if (iNumber > 0)
                    {
                        strSql.AppendLine("      ,'1'       ");
                    }
                    else if (iNumber < 0)
                    {
                        strSql.AppendLine("      ,'2'       ");
                    }
                    #endregion

                    strSql.AppendLine("      ," + ComFunction.getSqlValue(listInfoData[i]["dTime"], true) + "       ");
                    strSql.AppendLine("      ," + ComFunction.getSqlValue(listInfoData[i]["vcReason"], false) + "       ");
                    strSql.AppendLine("      ,'" + strUserId + "'       ");
                    strSql.AppendLine("      ,GETDATE()       ");
                    strSql.AppendLine("      )       ");
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 调整数据输入-导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strSql.AppendLine("      insert into TPackCompute_Ajust(vcPackNo,vcPackGPSNo,iNumber,vcType,dTime,vcReason,vcOperatorID,dOperatorTime)       ");
                    strSql.AppendLine("      values       ");
                    strSql.AppendLine("      (       ");
                    strSql.AppendLine("       " + ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + "       ");
                    strSql.AppendLine("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], false) + "       ");
                    strSql.AppendLine("      ," + ComFunction.getSqlValue(dt.Rows[i]["iNumber"], true) + "       ");

                    #region 调整类别：1:调增    2:调减
                    int iNumber = Convert.ToInt32(dt.Rows[i]["iNumber"].ToString());
                    if (iNumber > 0)
                    {
                        strSql.AppendLine("      ,'1'       ");
                    }
                    else if (iNumber < 0)
                    {
                        strSql.AppendLine("      ,'2'       ");
                    }
                    #endregion

                    strSql.AppendLine("      ," + ComFunction.getSqlValue(dt.Rows[i]["dTime"], true) + "       ");
                    strSql.AppendLine("      ," + ComFunction.getSqlValue(dt.Rows[i]["vcReason"], false) + "       ");
                    strSql.AppendLine("      ,'" + strUserId + "'       ");
                    strSql.AppendLine("      ,GETDATE()       ");
                    strSql.AppendLine("      )       ");
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion






    }
}
