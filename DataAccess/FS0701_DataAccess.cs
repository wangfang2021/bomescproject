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
    public class FS0701_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 
        public DataTable SearchSupplier()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where isnull(vcSupplierCode,'')<>''  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 
        public DataTable SearchFZLJ()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    select distinct vcFaZhuID as vcValue,vcFaZhuID as vcName  from TPackFaZhuTime ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search(List<object> PackSpot, string PackNo, string PackGPSNo, List<Object> strSupplierCode, string dFromB, string dFromE, string dToB, string dToE)
        {
            try
            {

                if (string.IsNullOrEmpty(dFromB))
                {
                    dFromB = "1900-01-01 00:00:00";

                }
                else
                {

                    dFromB = dFromB + " 00:00:00";
                }

                if (string.IsNullOrEmpty(dFromE))
                {
                    dFromE = "9999-12-31 23:59:59";

                }
                else
                {
                    dFromE = dFromE + " 23:59:59";
                }
                if (string.IsNullOrEmpty(dToB))
                {
                    dToB = "1900-01-01 00:00:00";

                }
                else
                {

                    dToB = dToB + " 00:00:00";
                }

                if (string.IsNullOrEmpty(dToE))
                {
                    dToE = "9999-12-31 23:59:59";

                }
                else
                {
                    dToE = dToE + " 23:59:59";


                }
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select iAutoId,vcPackNo,vcPackSpot, CONVERT(varchar(100),dPackFrom,23)as dPackFrom, CONVERT(varchar(100),dPackTo,23)as dPackTo, ");
                strSql.AppendLine("vcPackGPSNo,vcSupplierCode,vcSupplierName,vcSupplierPlant,vcCycle,vcParstName,vcPackLocation,vcDistinguish,vcFormat,vcReleaseID               ");
                strSql.AppendLine(",RTRIM(vcReleaseName) as vcReleaseName,iRelease,iZCRelease,isYZC              ");
                strSql.AppendLine("   ,'0' as vcModFlag,'0' as vcAddFlag,'' as vcIsorNo  ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackBase");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplierCode in( ");
                    for (int i = 0; i < strSupplierCode.Count; i++)
                    {
                        if (strSupplierCode.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + strSupplierCode[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + strSupplierCode[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");

                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int i = 0; i < PackSpot.Count; i++)
                    {
                        if (PackSpot.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + PackSpot[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + PackSpot[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");

                strSql.AppendLine($"      AND dPackFrom BETWEEN '{dFromB}' and '{dFromE}'");
                strSql.AppendLine($"      AND dPackTo BETWEEN '{dToB}' and '{dToE}'");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_1()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    select * from TPackBase   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion



        #region checktime
        public DataTable searchcheckTime(string vcPackSpot, string strPackNo, string dFrom, string dTo, int iAutoId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("   select * from TPackBase where  iAutoId<>'" + iAutoId + "'  ");
                if (!string.IsNullOrEmpty(strPackNo))
                {
                    strSql.AppendLine("and vcPackNo='" + strPackNo + "'   ");
                }
                if (!string.IsNullOrEmpty(vcPackSpot))
                {
                    strSql.AppendLine("and vcPackSpot='" + vcPackSpot + "'   ");
                }
                strSql.AppendLine("and  dPackFrom <='" + dTo + "' and dPackTo>='" + dFrom + "'  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion



        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                DataTable dtS = this.SearchSupplier();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow[] dr = dtS.Select("vcValue='" + listInfoData[i]["vcSupplierCode"] + "'");
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.AppendLine("     INSERT INTO TPackBase( ");
                        sql.AppendLine("      vcPackNo,");
                        sql.AppendLine("      vcPackSpot,");
                        sql.AppendLine("      dPackFrom,");
                        sql.AppendLine("      dPackTo,");
                        sql.AppendLine("      vcPackGPSNo,");
                        sql.AppendLine("      vcSupplierCode,");
                        sql.AppendLine("      vcSupplierPlant,");
                        sql.AppendLine("      vcCycle,");
                        sql.AppendLine("      vcSupplierName,");
                        sql.AppendLine("      vcParstName,");
                        sql.AppendLine("      vcPackLocation,");
                        sql.AppendLine("      vcDistinguish,");
                        sql.AppendLine("      vcFormat,");
                        //sql.AppendLine("      vcReleaseID,");
                        sql.AppendLine("      vcReleaseName,");
                        sql.AppendLine("      iRelease,");
                        sql.AppendLine("      iZCRelease,");
                        sql.AppendLine("      isYZC,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false).Replace(" ", "") + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dPackFrom"].ToString().Split(' ')[0], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dPackTo"].ToString().Split(' ')[0] + " 23:59:59", true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierPlant"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcCycle"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(dr[i]["vcName"].ToString(), false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcParstName"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackLocation"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcDistinguish"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFormat"], false) + ",");
                        //sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcReleaseID"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcReleaseName"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iRelease"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iZCRelease"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["isYZC"], true) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");

                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackBase");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false).Replace(" ", "")},");
                        sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)},");
                        sql.AppendLine($"   dPackFrom = {ComFunction.getSqlValue(listInfoData[i]["dPackFrom"].ToString().Split(' ')[0], true)},");
                        sql.AppendLine($"   dPackTo = {ComFunction.getSqlValue(listInfoData[i]["dPackTo"].ToString().Split(' ')[0] + " 23:59:59", true)},");
                        sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false)},");
                        sql.AppendLine($"   vcSupplierCode = {ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], false)},");
                        sql.AppendLine($"   vcSupplierPlant = {ComFunction.getSqlValue(listInfoData[i]["vcSupplierPlant"], false)},");
                        sql.AppendLine($"   vcCycle = {ComFunction.getSqlValue(listInfoData[i]["vcCycle"], false)},");
                        if (dr.Length == 0)
                        {

                            sql.AppendLine($"   vcSupplierName ='',");
                        }
                        else
                        {
                            sql.AppendLine($"   vcSupplierName = {ComFunction.getSqlValue(dr[0]["vcName"].ToString(), false)},");

                        }

                        sql.AppendLine($"   vcParstName = {ComFunction.getSqlValue(listInfoData[i]["vcParstName"], false)},");
                        sql.AppendLine($"   vcPackLocation = {ComFunction.getSqlValue(listInfoData[i]["vcPackLocation"], false)},");
                        sql.AppendLine($"   vcDistinguish = {ComFunction.getSqlValue(listInfoData[i]["vcDistinguish"], false)},");
                        sql.AppendLine($"   vcFormat = {ComFunction.getSqlValue(listInfoData[i]["vcFormat"], false)},");
                        //sql.AppendLine($"   vcReleaseID = '{ComFunction.getSqlValue(listInfoData[i]["vcReleaseID"], false)}',");
                        sql.AppendLine($"   vcReleaseName = {ComFunction.getSqlValue(listInfoData[i]["vcReleaseName"], false)},");
                        sql.AppendLine($"   iRelease = {ComFunction.getSqlValue(listInfoData[i]["iRelease"], true)},");
                        sql.AppendLine($"   iZCRelease = {ComFunction.getSqlValue(listInfoData[i]["iZCRelease"], true)},");
                        sql.AppendLine($"   isYZC = {ComFunction.getSqlValue(listInfoData[i]["isYZC"], true)},");
                        sql.AppendLine($"   vcOperatorID = {strUserId},");
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

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPackBase where iAutoId in(   \r\n ");
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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                //  "vcPackSpot","vcPackNo","vcPackGPSNo","dPackFrom","dPackTo","vcParstName","vcSupplierName",
                //"vcSupplierCode","iRelease","iZCRelease","vcCycle","vcDistinguish","vcPackLocation","vcFormat","vcReleaseName"
                //先删除重复待更新的


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcIsorNo"].ToString() == "新增")
                    {
                        sql.Append("  INSERT INTO [dbo].[TPackBase]   \r\n");
                        sql.Append("             ([vcPackNo]   \r\n");
                        sql.Append("             ,[vcPackSpot]   \r\n");
                        sql.Append("             ,[dPackFrom]   \r\n");
                        sql.Append("             ,[dPackTo]   \r\n");
                        sql.Append("             ,[vcPackGPSNo]   \r\n");
                        sql.Append("             ,[vcSupplierCode]   \r\n");
                        sql.Append("             ,[vcSupplierPlant]   \r\n");
                        sql.Append("             ,[vcCycle]   \r\n");
                        sql.Append("             ,[vcSupplierName]   \r\n");
                        sql.Append("             ,[vcParstName]   \r\n");
                        sql.Append("             ,[vcPackLocation]   \r\n");
                        sql.Append("             ,[vcDistinguish]   \r\n");
                        sql.Append("             ,[vcFormat]   \r\n");
                        //sql.Append("             ,[vcReleaseID]   \r\n");
                        sql.Append("             ,[vcReleaseName]   \r\n");
                        sql.Append("             ,[iRelease]   \r\n");
                        sql.Append("             ,[iZCRelease]   \r\n");
                        sql.Append("             ,[isYZC]   \r\n");
                        sql.Append("             ,[vcOperatorID]   \r\n");
                        sql.Append("             ,[dOperatorTime])   \r\n");
                        sql.Append("       VALUES   \r\n");
                        sql.Append("             (   \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false).Replace(" ", "") + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dPackFrom"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dPackTo"].ToString().Split(' ')[0] + " 23:59:59", false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], true) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierCode"], true) + ", \r\n");
                        sql.Append("   null,  \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcCycle"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierName"], true) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcParstName"], true) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackLocation"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcDistinguish"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcFormat"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcReleaseName"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["iRelease"], false) + ", \r\n");
                        sql.Append(ComFunction.getSqlValue(dt.Rows[i]["iZCRelease"], true) + ", \r\n");
                        sql.Append("   null,  \r\n");
                        sql.Append("   '" + strUserId + "',  \r\n");
                        sql.Append("   getdate())  \r\n");


                    }
                    else
                    {

                        int iAutoId = Convert.ToInt32(dt.Rows[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackBase");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false).Replace(" ", "")},");
                        sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false)},");
                        sql.AppendLine($"   dPackFrom = {ComFunction.getSqlValue(dt.Rows[i]["dPackFrom"].ToString().Split(' ')[0], true)},");
                        sql.AppendLine($"   dPackTo = {ComFunction.getSqlValue(dt.Rows[i]["dPackTo"].ToString().Split(' ')[0] + " 23:59:59", true)},");
                        sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], false)},");
                        sql.AppendLine($"   vcSupplierCode = {ComFunction.getSqlValue(dt.Rows[i]["vcSupplierCode"], false)},");
                        sql.AppendLine($"   vcSupplierPlant = NULL,");
                        sql.AppendLine($"   vcCycle = {ComFunction.getSqlValue(dt.Rows[i]["vcCycle"], false)},");
                        sql.AppendLine($"   vcSupplierName = {ComFunction.getSqlValue(dt.Rows[i]["vcSupplierName"], false)},");
                        sql.AppendLine($"   vcParstName = {ComFunction.getSqlValue(dt.Rows[i]["vcParstName"], false)},");
                        sql.AppendLine($"   vcPackLocation = {ComFunction.getSqlValue(dt.Rows[i]["vcPackLocation"], false)},");
                        sql.AppendLine($"   vcDistinguish = {ComFunction.getSqlValue(dt.Rows[i]["vcDistinguish"], false)},");
                        sql.AppendLine($"   vcFormat = {ComFunction.getSqlValue(dt.Rows[i]["vcFormat"], false)},");
                        //sql.AppendLine($"   vcReleaseID = '{ComFunction.getSqlValue(listInfoData[i]["vcReleaseID"], false)}',");
                        sql.AppendLine($"   vcReleaseName = {ComFunction.getSqlValue(dt.Rows[i]["vcReleaseName"], false)},");
                        sql.AppendLine($"   iRelease = {ComFunction.getSqlValue(dt.Rows[i]["iRelease"], true)},");
                        sql.AppendLine($"   iZCRelease = {ComFunction.getSqlValue(dt.Rows[i]["iZCRelease"], true)},");
                        sql.AppendLine($"   isYZC = NULL,");
                        sql.AppendLine($"   vcOperatorID = {strUserId},");
                        sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                        sql.AppendLine($"  WHERE");
                        sql.AppendLine($"  iAutoId='{iAutoId}';");


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

        #region 查找工厂
        public DataTable SearchPackSpot(string userid)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select a.vcBaoZhuangPlace as vcValue,b.vcName as vcName from (      \n");
                strSql.Append("   select vcBaoZhuangPlace from SUser where vcUserID='" + userid + "'      \n");
                strSql.Append("   )a left join      \n");
                strSql.Append("   (      \n");
                strSql.Append("   select * from TCode where vcCodeId='C023'      \n");
                strSql.Append("   )b on a.vcBaoZhuangPlace=b.vcValue      \n");
                strSql.Append("         \n");
                strSql.Append("         \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 资材系统取数据连接
        public DataTable searchmaps()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DataTable dt = new DataTable();
                sql.Append("  select a.SUPPLIER_CODE,a.PART_NAME,a.SPECIFICATION,a.ORDER_LOT,b.SUPPLIER_NAME,a.PART_NO from (  \n");
                sql.Append("  select SUPPLIER_CODE,PART_NAME,SPECIFICATION,ORDER_LOT,PART_NO   \n");
                sql.Append("  from TB_M0050 where   DEL_FLAG='0'  \n");
                sql.Append("  )a left join  \n");
                sql.Append("  (  \n");
                sql.Append("  SELECT * from TB_M0100  \n");
                sql.Append("  )b on a.SUPPLIER_CODE=b.SUPPLIER_CODE  \n");

                dt = this.MAPSSearch(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable MAPSSearch(string sql)
        {
            SqlConnection conn = Common.ComConnectionHelper.CreateConnection_MAPS();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = sql;
                Common.ComConnectionHelper.OpenConection_SQL(ref conn);
                da.Fill(dt);
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                return dt;
            }
            catch (Exception ex)
            {
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion


    }
}
