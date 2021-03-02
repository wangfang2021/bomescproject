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
    public class FS0702_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 
        public DataTable SearchSupplier()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcPackSupplierCode as vcValue,vcPackSupplierName as vcName from TPackSupplier; ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search(string Note, string PackSpot, string Shouhuofang, string Pinfan, string Car, string PackNO, string PackGPSNo, string dtFromBegin, string dtFromEnd, string dtToBegin, string dtToEnd)
        {
            try
            {

                if (string.IsNullOrEmpty(dtFromBegin))
                {
                    dtFromBegin = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dtFromEnd))
                {
                    dtFromEnd = "9999-12-31 0:00:00";

                }
                if (string.IsNullOrEmpty(dtToBegin))
                {
                    dtToBegin = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dtToEnd))
                {
                    dtToEnd = "9999-12-31 0:00:00";

                }

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    select a.iAutoId,a.vcModFlag,a.vcAddFlag,a.varChangedItem,a.vcPackSpot,a.vcPartsNo,   ");
                strSql.AppendLine("    a.vcCar,substring(CONVERT(varchar, a.dUsedFrom,120),0,11) as dUsedFrom ,substring(CONVERT(varchar, a.dUsedTo,120),0,11) as dUsedTo ,a.dFrom,a.dTo,a.vcDistinguish,a.vcPackGPSNo,a.iBiYao,a.vcPackNo ");
                strSql.AppendLine("    b.vcValue as vcShouhuofangID,b.vcName as  from (       ");
                strSql.AppendLine("     select *,'0' as vcModFlag,'0' as vcAddFlag from TPackItem    ");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackNO))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNO}%'");
                if (!string.IsNullOrEmpty(Pinfan))
                    strSql.AppendLine($"      AND vcPartsNo LIKE '%{Pinfan}%'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcShouhuofangID = '{Shouhuofang}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcCar = '{Car}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                if (!string.IsNullOrEmpty(Note))
                    strSql.AppendLine($"      AND vcNote LIKE '%{Note}%'");

                strSql.AppendLine($"      AND dFrom BETWEEN '{dtFromBegin}' and '{dtFromEnd}'");
                strSql.AppendLine($"      AND dTo BETWEEN '{dtToBegin}' and '{dtToEnd}'");
                strSql.AppendLine("  	)a left join    ");
                strSql.AppendLine("  	(    ");
                strSql.AppendLine("   select * from TCode where vcCodeName='收货方'  and vcCodeId='C018'     ");
                strSql.AppendLine("   )b on a.vcShouhuofangID=b.vcValue    ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion


        #region 纵向导出
        public DataTable SearchEXZ(string iAutoID, string Note, string PackSpot, string Shouhuofang, string Pinfan, string Car, string PackNO, string PackGPSNo, string dtFromBegin, string dtFromEnd, string dtToBegin, string dtToEnd)
        {
            try
            {

                if (string.IsNullOrEmpty(dtFromBegin))
                {
                    dtFromBegin = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dtFromEnd))
                {
                    dtFromEnd = "3000-01-01 0:00:00";

                }
                if (string.IsNullOrEmpty(dtToBegin))
                {
                    dtToBegin = "1990-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dtToEnd))
                {
                    dtToEnd = "3000-01-01 0:00:00";

                }

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("    select a.varChangedItem,a.vcPackSpot,a.vcPartsNo,   ");
                strSql.AppendLine("    a.vcCar,a.dUsedFrom,a.dUsedTo,a.dFrom,a.dTo,a.vcDistinguish,a.vcPackGPSNo,a.iBiYao,a.vcPackNo   ");
                strSql.AppendLine("    ,b.vcName as vcShouhuofang from (       ");
                strSql.AppendLine("     select *,'0' as vcModFlag,'0' as vcAddFlag from TPackItem    ");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackNO))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNO}%'");
                if (!string.IsNullOrEmpty(Pinfan))
                    strSql.AppendLine($"      AND vcPartsNo LIKE '%{Pinfan}%'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcShouhuofangID = '{Shouhuofang}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcCar = '{Car}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                if (!string.IsNullOrEmpty(Note))
                    strSql.AppendLine($"      AND vcNote LIKE '%{Note}%'");

                strSql.AppendLine($"      AND dFrom BETWEEN '{dtFromBegin}' and '{dtFromEnd}'");
                strSql.AppendLine($"      AND dTo BETWEEN '{dtToBegin}' and '{dtToEnd}'");
                strSql.AppendLine("  	)a left join    ");
                strSql.AppendLine("  	(    ");
                strSql.AppendLine("   select * from TCode where vcCodeName='收货方'  and vcCodeId='C018'    ");
                strSql.AppendLine("   )b on a.vcShouhuofangID=b.vcValue    ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, DataTable dtPackitem, DataTable dtPackBase)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                 

                    string dfrom = listInfoData[i]["dFrom"].ToString() == "" ? "1990-01-01 0:00:00" : listInfoData[i]["dFrom"].ToString();
                    string dto = listInfoData[i]["dTo"].ToString() == "" ? "3000-01-01 0:00:00" : listInfoData[i]["dTo"].ToString(); ;

                    //if (listInfoData[i]["vcShouhuofang"].ToString() != "")
                    //{
                    //    strSHFID = ComFunction.getSqlValue(listInfoData[i]["vcShouhuofang"], true) != "" ? listInfoData[i]["vcShouhuofang"].ToString() : "";

                    //}
                    string PackSpot = "";
                    string PackGPSNo = "";
                    string dUserFrom = "";
                    string dUserTo = "";
                    string vcChange = "";
                    string vcCar = "";
                    string strSHFID = "";
                    DataRow[] dr1 = dtPackBase.Select("vcPackNo='" + listInfoData[i]["vcPackNo"] + "'");
                    if (dr1.Length == 0)
                    {
                        PackGPSNo = "";
                    }
                    else
                    {
                        PackGPSNo = dr1[0]["vcPackGPSNo"].ToString();
                    }
                    DataRow[] dr2 = dtPackitem.Select("vcPartsNo='" + listInfoData[i]["vcPartsNo"] + "'");
                    if (dr2.Length == 0)
                    {
                        dUserFrom = "1990-01-01";
                        dUserTo = "9999-12-31";
                        vcChange = "";
                        vcCar = "";
                        strSHFID = "";
                        PackSpot = "";
                    }
                    else
                    {
                        dUserFrom = dr2[0]["dUsedFrom"].ToString();
                        dUserTo = dr2[0]["dUsedTo"].ToString();
                        vcChange = dr2[0]["varChangedItem"].ToString();
                        vcCar = dr2[0]["vcCar"].ToString();
                        strSHFID = dr2[0]["vcShouhuofangID"].ToString();
                        PackSpot= dr2[0]["vcPackSpot"].ToString();
                    }



                    if (bAddFlag == true)
                    {//新增
                        sql.AppendLine("     INSERT INTO [TPackItem]( \r\n");
                        sql.AppendLine("     [vcPartsNo] \r\n");
                        sql.AppendLine("     ,[vcPackNo] \r\n");
                        sql.AppendLine("     ,[vcPackGPSNo] \r\n");
                        sql.AppendLine("     ,[vcShouhuofangID] \r\n");
                        sql.AppendLine("     ,[vcCar] \r\n");
                        sql.AppendLine("     ,[dUsedFrom] \r\n");
                        sql.AppendLine("     ,[dUsedTo] \r\n");
                        sql.AppendLine("     ,[dFrom] \r\n");
                        sql.AppendLine("     ,[dTo] \r\n");
                        sql.AppendLine("     ,[vcDistinguish] \r\n");
                        sql.AppendLine("     ,[iBiYao] \r\n");
                        sql.AppendLine("     ,[vcOperatorID] \r\n");
                        sql.AppendLine("     ,[dOperatorTime] \r\n");
                        sql.AppendLine("     ,[vcPackSpot] \r\n");
                        sql.AppendLine("     ,[varChangedItem]) \r\n");
                        sql.AppendLine("     VALUES \r\n");
                        sql.AppendLine("     ( \r\n");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPartsNo"].ToString().Trim(), false) + ",\r\n");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"].ToString().Trim(), false) + ",\r\n");
                        sql.AppendLine(" '"+ PackGPSNo + "',   \r\n");
                        sql.AppendLine(" '" + strSHFID + "',   \r\n");
                        sql.AppendLine(" '" + vcCar + "',   \r\n");
                        sql.AppendLine(" '" + dUserFrom + "',   \r\n");
                        sql.AppendLine(" '" + dUserTo + "',   \r\n");
                        sql.AppendLine(ComFunction.getSqlValue(dfrom, false) + ",\r\n");
                        sql.AppendLine(ComFunction.getSqlValue(dto, false) + ",\r\n");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcDistinguish"], false) + ",\r\n");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["iBiYao"], false) + ",\r\n");
                        sql.AppendLine($"     		{strUserId},\r\n");
                        sql.AppendLine("     		getDate(),\r\n");
                        sql.AppendLine(" '" + PackSpot + "',   \r\n");
                        sql.AppendLine(" '" + vcChange + "'   \r\n");
                        sql.AppendLine("     	); ");


                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackItem");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPartsNo = {ComFunction.getSqlValue(listInfoData[i]["vcPartsNo"], false)},\r\n");
                        sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)},\r\n");
                        sql.AppendLine($"   vcPackGPSNo = {PackGPSNo},\r\n");
                        //sql.AppendLine($"   dPackTo = '{ComFunction.getSqlValue(listInfoData[i]["dPackTo"], true)}',\r\n");
                        sql.AppendLine($"   vcPackSpot = {PackSpot},\r\n");
                        sql.AppendLine($"   vcShouhuofangID = '{strSHFID}',\r\n");
                        sql.AppendLine($"   vcCar ='"+ vcCar + "',\r\n");
                        sql.AppendLine($"   dUsedFrom = '"+ dUserFrom + "',\r\n");
                        sql.AppendLine($"   dUsedTo ='"+ dUserTo + "',\r\n");
                        sql.AppendLine($"   dFrom ={ComFunction.getSqlValue(dfrom, false)},\r\n");
                        sql.AppendLine($"   dTo = {ComFunction.getSqlValue(dto, false)},\r\n");
                        sql.AppendLine($"   vcDistinguish = {ComFunction.getSqlValue(listInfoData[i]["vcDistinguish"], false)},\r\n");
                        sql.AppendLine($"   iBiYao = {ComFunction.getSqlValue(listInfoData[i]["iBiYao"], false)},\r\n");
                        sql.AppendLine($"   vcOperatorID = '{strUserId}',\r\n");
                        sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}',\r\n");
                        sql.AppendLine($"   varChangedItem ='"+ vcChange + "'\r\n");
                        sql.AppendLine($"  WHERE \r\n");
                        sql.AppendLine($"  iAutoId='{iAutoId}'; \r\n");


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
                sql.Append("  delete from  TPackItem where iAutoId in(   \r\n ");
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
        public void importSave(DataTable dt, string strUserId, DataTable dtPackBase,DataTable dtPackitem)
        {
            try
            {

                DataTable dtSH = ComFunction.getTCode("C018");//价格系数

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("  delete  from TPackItem where vcPartsNo='" + dt.Rows[i]["vcPartsNo"].ToString() + "' and vcPackSpot='" + dt.Rows[i]["vcPackSpot"].ToString() + "' \r\n");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    string strSHFAll = dtSH.Select("vcValue='" + dt.Rows[i]["vcShouhuofangID"].ToString() + "'")[0]["vcName"].ToString() == "" ?
                   "null" : dtSH.Select("vcValue='" + dt.Rows[i]["vcShouhuofangID"].ToString() + "'")[0]["vcName"].ToString();
                    string strSHFID = strSHFAll == "null" ? "" : strSHFAll.Split(":")[0];
                    string PackGPSNo = "";
                    string dUserFrom = "";
                    string dUserTo = "";
                    string vcChange = "";
                    string vcCar = "";
                    DataRow[] dr1 = dtPackBase.Select("vcPackNo='" + dt.Rows[i]["vcPackNo"].ToString() + "'");
                    if (dr1.Length == 0)
                    {
                        PackGPSNo = "";
                    }
                    else
                    {
                        PackGPSNo = dr1[0]["vcPackGPSNo"].ToString();
                    }
                    DataRow[] dr2 = dtPackitem.Select("vcPartsNo='" + dt.Rows[i]["vcPartsNo"].ToString() + "'");
                    if (dr2.Length == 0)
                    {
                        dUserFrom = "1990-01-01";
                        dUserTo = "9999-12-31";
                        vcChange = "";
                        vcCar = "";
                    }
                    else
                    {
                        dUserFrom = dr2[0]["dUsedFrom"].ToString();
                        dUserTo = dr2[0]["dUsedTo"].ToString();
                        vcChange = dr2[0]["varChangedItem"].ToString();
                        vcCar = dr2[0]["vcCar"].ToString();
                    }


                    sql.AppendLine("     INSERT INTO [TPackItem] \r\n");
                    sql.AppendLine("     ([vcPartsNo] \r\n");
                    sql.AppendLine("     ,[vcPackNo] \r\n");
                    sql.AppendLine("     ,[vcPackGPSNo] \r\n");
                    sql.AppendLine("     ,[vcShouhuofangID] \r\n");
                    sql.AppendLine("     ,[vcCar] \r\n");
                    sql.AppendLine("     ,[dUsedFrom] \r\n");
                    sql.AppendLine("     ,[dUsedTo] \r\n");
                    sql.AppendLine("     ,[dFrom] \r\n");
                    sql.AppendLine("     ,[dTo] \r\n");
                    sql.AppendLine("     ,[vcDistinguish] \r\n");
                    sql.AppendLine("     ,[iBiYao] \r\n");
                    sql.AppendLine("     ,[vcOperatorID] \r\n");
                    sql.AppendLine("     ,[dOperatorTime] \r\n");
                    sql.AppendLine("     ,[varChangedItem] \r\n");
                    sql.AppendLine("     ,[vcPackSpot]) \r\n");
                    sql.AppendLine("     VALUES \r\n");

                    sql.AppendLine("     ( \r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPartsNo"], false) + ",\r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + ",\r\n");
                    sql.AppendLine("'" + PackGPSNo + "',\r\n");
                    sql.AppendLine("     '" + strSHFID + "',  \r\n");
                    sql.AppendLine("'"+ vcCar + "',\r\n");
                    sql.AppendLine( "'"+ dUserFrom + "',\r\n");
                    sql.AppendLine("'"+ dUserTo + "',\r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dFrom"], false) + ",\r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dTo"], false) + ",\r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcDistinguish"], false) + ",\r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["iBiYao"], false) + ",\r\n");
                    sql.AppendLine("     '" + strUserId + "',  \r\n");
                    sql.AppendLine("     getdate(),  \r\n");
                    sql.AppendLine("'"+ vcChange + "',\r\n");
                    sql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + "\r\n");
                    sql.AppendLine("     )  \r\n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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

        #region 检查品番
        public bool CheckPartsNo(string strShouhuofang, string strPartsNo)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DataTable dt = new DataTable();
                //and vcReceiver='" + strShouhuofang + "'
                sql.Append("  select vcPartsNo from TPackItem where vcPartsNo='" + strPartsNo + "' \r\n ");

                dt = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {

                    return false;
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
