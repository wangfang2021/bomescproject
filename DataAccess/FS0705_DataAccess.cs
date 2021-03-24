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
            strSql.Append("       declare @dBegin datetime      \r\n");
            strSql.Append("       declare @dEnd datetime      \r\n");
            strSql.Append("       set @dBegin=(select max(dEnd) from TPackCompute_Time where vcFaZhuID = "+strFaZhuID+");      \r\n");
            strSql.Append("       set @dEnd = (select MAX(cast(convert(varchar(10),getdate(),120)+' '+convert(varchar(50),druHeToTime) as datetime)) from TPackFaZhuTime where dFaZhuFromTime<=CONVERT(char(8),GETDATE(),108) and CONVERT(char(8),GETDATE(),108)<=dFaZhuToTime);      \r\n");
            strSql.Append("             \r\n");
            strSql.Append("       insert into TPackCompute(vcFaZhuID,vcTimeStr,vcPackNo,iA_SRS,iB_LastShengYu,iC_LiLun,iD_TiaoZheng,iE_JinJi,iF_DingGou,iG_ShengYu,vcOperatorID,dOperatorTime)      \r\n");
            strSql.Append("       select a.*,(a.B+a.E+a.F-a.[C ]-a.D) as 'G','000000',GETDATE() from      \r\n");
            strSql.Append("       (      \r\n");
            strSql.Append("       	select "+strFaZhuID+"as vcFaZhuID,GETDATE() as vcTimeStr,a.vcPackNo as '包材品番',a.iRelease as 'A',b.iG_ShengYu as'B'       \r\n");
            strSql.Append("       	,c.iBiYao as 'C ',d.iNumber as 'D',e.iSJNumber as 'E',case when (b.iG_ShengYu+e.iSJNumber-c.iBiYao-d.iNumber)>=0 then 0 else (CEILING(c.iBiYao+d.iNumber-b.iG_ShengYu-e.iSJNumber)/a.iRelease)*a.iRelease end as 'F'      \r\n");
            strSql.Append("       	from      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select vcPackSpot,vcPackNo,iRelease from TPackBase where dPackFrom <= GETDATE() and dPackTo >= GETDATE()      \r\n");
            strSql.Append("       	) a      \r\n");
            strSql.Append("       	left join      \r\n");
            strSql.Append("       	(      \r\n");
            strSql.Append("       		select a.vcPackNo,b.iG_ShengYu from      \r\n");
            strSql.Append("       		(      \r\n");
            strSql.Append("       			select MAX(iautoId) as MaxAutoId, vcPackNo from TPackCompute where vcFaZhuID = "+strFaZhuID+" group by vcFaZhuID,vcPackNo      \r\n");
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
            strSql.Append("       insert into TPackCompute_Time(vcFaZhuID,dBegin,dEnd,vcOperatorID,dOperatorTime) values ('vcFaZhuID',@dBegin,@dEnd,'000000',GETDATE());      \r\n");
            #endregion
            
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



        #region 取工厂对应白班的起始时间
        public DataTable getBanZhiStart(string strPackPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select tFromTime from TBZTime where vcBanZhi='白' and vcPackPlant='" + strPackPlant + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 获取包材发注时间
        public DataTable getFaZhuTime(string strFaZhuID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from TPackFaZhuTime where vcFaZhuID='"+ strFaZhuID + "'   \n");
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


    }
}
