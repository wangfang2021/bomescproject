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
    public class FS0704_DataAccess
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
        public DataTable Search(string PackSpot, string FaZhu, string dFromB, string dFromE, string dToB, string dToE)
        {
            try
            {

                if (string.IsNullOrEmpty(dFromB))
                {
                    dFromB = "1900-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dFromE))
                {
                    dFromE = "9999-12-31 0:00:00";

                }
                if (string.IsNullOrEmpty(dToB))
                {
                    dToB = "1900-01-01 0:00:00";

                }

                if (string.IsNullOrEmpty(dToE))
                {
                    dToE = "9999-12-31 0:00:00";

                }
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select '0' as vcModFlag,'0' as vcAddFlag, ");
                strSql.AppendLine("   vcFaZhuID,vcRuHeFromDay,dRuHeFromTime,vcRuHeToDay,druHeToTime,   ");
                strSql.AppendLine("   vcFaZhuFromDay,dFaZhuFromTime,vcFaZhuToDay,dFaZhuToTime,   ");
                strSql.AppendLine("   vcNaQiFromDay,dNaQiFromTime,vcNaQiToDay,dNaQiToTime,vcBianCi,   ");
                strSql.AppendLine("   vcPackSpot,CONVERT(varchar(100),dFrom,21) as dFrom,CONVERT(varchar(100),dTo,21) as dTo   ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackFaZhuTime");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(FaZhu))
                    strSql.AppendLine($"      AND vcFaZhuID = '{FaZhu}'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");

                strSql.AppendLine($"      AND dFrom BETWEEN '{dFromB}' and '{dFromE}'");
                strSql.AppendLine($"      AND dTo BETWEEN '{dToB}' and '{dToE}'");
                strSql.AppendLine("      order by vcFaZhuID");
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.AppendLine("     INSERT INTO TPackFaZhuTime( ");
                        sql.AppendLine("      vcFaZhuID,");
                        //sql.AppendLine("      vcFaZhu,");
                        sql.AppendLine("      vcRuHeFromDay,");
                        sql.AppendLine("      dRuHeFromTime,");
                        sql.AppendLine("      vcRuHeToDay,");
                        sql.AppendLine("      druHeToTime,");
                        sql.AppendLine("      vcFaZhuFromDay,");
                        sql.AppendLine("      dFaZhuFromTime,");
                        sql.AppendLine("      vcFaZhuToDay,");
                        sql.AppendLine("      dFaZhuToTime,");
                        sql.AppendLine("      vcNaQiFromDay,");
                        sql.AppendLine("      dNaQiFromTime,");
                        sql.AppendLine("      vcNaQiToDay,");
                        sql.AppendLine("      dNaQiToTime,");
                        sql.AppendLine("      vcBianCi,");
                        sql.AppendLine("      vcPackSpot,");
                        sql.AppendLine("      dFrom,");
                        sql.AppendLine("      dTo,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFaZhuID"], false) + ",");
                        //sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFaZhu"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcRuHeFromDay"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dRuHeFromTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcRuHeToDay"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["druHeToTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFaZhuFromDay"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dFaZhuFromTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFaZhuToDay"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dFaZhuToTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcNaQiFromDay"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dNaQiFromTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcNaQiToDay"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dNaQiToTime"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcBianCi"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dFrom"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dTo"], true) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");


                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改

                        //限制



                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.AppendLine("  UPDATE TPackFaZhuTime");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcFaZhuID = {ComFunction.getSqlValue(listInfoData[i]["vcFaZhuID"], false)},");
                        sql.AppendLine($"   vcRuHeFromDay = {ComFunction.getSqlValue(listInfoData[i]["vcRuHeFromDay"], false)},");
                        sql.AppendLine($"   dRuHeFromTime = {ComFunction.getSqlValue(listInfoData[i]["dRuHeFromTime"], true)},");
                        sql.AppendLine($"   vcRuHeToDay = {ComFunction.getSqlValue(listInfoData[i]["vcRuHeToDay"], false)},");
                        sql.AppendLine($"   druHeToTime = {ComFunction.getSqlValue(listInfoData[i]["druHeToTime"], true)},");
                        sql.AppendLine($"   vcFaZhuFromDay = {ComFunction.getSqlValue(listInfoData[i]["vcFaZhuFromDay"], false)},");
                        sql.AppendLine($"   dFaZhuFromTime = {ComFunction.getSqlValue(listInfoData[i]["dFaZhuFromTime"], true)},");
                        sql.AppendLine($"   vcFaZhuToDay = {ComFunction.getSqlValue(listInfoData[i]["vcFaZhuToDay"], false)},");
                        sql.AppendLine($"   dFaZhuToTime = {ComFunction.getSqlValue(listInfoData[i]["dFaZhuToTime"], true)},");
                        sql.AppendLine($"   vcNaQiFromDay = {ComFunction.getSqlValue(listInfoData[i]["vcNaQiFromDay"], false)},");
                        sql.AppendLine($"   dNaQiFromTime = {ComFunction.getSqlValue(listInfoData[i]["dNaQiFromTime"], true)},");
                        sql.AppendLine($"   vcNaQiToDay = {ComFunction.getSqlValue(listInfoData[i]["vcNaQiToDay"], false)},");
                        sql.AppendLine($"   dNaQiToTime = {ComFunction.getSqlValue(listInfoData[i]["dNaQiToTime"], true)},");
                        sql.AppendLine($"   vcBianCi = {ComFunction.getSqlValue(listInfoData[i]["vcBianCi"], false)},");
                        sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)},");
                        sql.AppendLine($"   dFrom = {ComFunction.getSqlValue(listInfoData[i]["dFrom"], true)},");
                        sql.AppendLine($"   dTo = {ComFunction.getSqlValue(listInfoData[i]["dTo"], true)},");
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

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPackFaZhuTime where iAutoId in(   \r\n ");
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

        #region 判断时间重复性
        public DataTable SearchLJTime(string strFaZhu, string iAutoId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (iAutoId == "")
                {
                    strSql.Append("  select* from TPackFaZhuTime where vcFaZhuID='" + strFaZhu + "'   order by dRuHeFromTime asc      \n");
                }
                else
                {
                    strSql.Append("  select* from TPackFaZhuTime where vcFaZhuID='" + strFaZhu + "' and iAutoId <>'" + iAutoId + "'  order by dRuHeFromTime asc    \n");
                }

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
                StringBuilder strSql = new StringBuilder();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strSql.Append("  delete from  TPackFaZhuTime  where vcFaZhuID='"+ dt.Rows[i]["vcFaZhuID"].ToString() + "'and  vcBianCi='"+ dt.Rows[i]["vcBianCi"].ToString() + "' \n");
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strSql.Append("  insert into TPackFaZhuTime    \n");
                    strSql.Append("       ( vcFaZhuID     \n");

                    strSql.Append("        , vcRuHeFromDay     \n");
                    strSql.Append("        , dRuHeFromTime     \n");
                    strSql.Append("        , vcRuHeToDay     \n");
                    strSql.Append("        , druHeToTime     \n");

                    strSql.Append("        , vcFaZhuFromDay     \n");
                    strSql.Append("        , dFaZhuFromTime     \n");
                    strSql.Append("        , vcFaZhuToDay     \n");
                    strSql.Append("        , dFaZhuToTime     \n");

                    strSql.Append("        , vcNaQiFromDay     \n");
                    strSql.Append("        , dNaQiFromTime     \n");
                    strSql.Append("        , vcNaQiToDay     \n");
                    strSql.Append("        , dNaQiToTime     \n");
                    strSql.Append("        , vcBianCi     \n");

                    strSql.Append("        , vcPackSpot     \n");
                    strSql.Append("        , dFrom     \n");
                    strSql.Append("        , dTo     \n");
                    strSql.Append("        , vcOperatorID     \n");
                    strSql.Append("        , dOperatorTime )    \n");
                    strSql.Append("  	  values    \n");
                    strSql.AppendLine("     	( ");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcFaZhuID"], false) + ",");

                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcRuHeFromDay"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dRuHeFromTime"], true) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcRuHeToDay"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["druHeToTime"], true) + ",");

                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcFaZhuFromDay"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dFaZhuFromTime"], true) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcFaZhuToDay"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dFaZhuToTime"], true) + ",");

                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcNaQiFromDay"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dNaQiFromTime"], true) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcNaQiToDay"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dNaQiToTime"], true) + ",");

                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcBianCi"], false) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], true) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dFrom"], true) + ",");
                    strSql.AppendLine(ComFunction.getSqlValue(dt.Rows[i]["dTo"], true) + ",");
                    strSql.AppendLine($"     		{strUserId},");
                    strSql.AppendLine("     		getDate()");
                    strSql.AppendLine("     	); ");
                }
                excute.ExcuteSqlWithStringOper(strSql.ToString());
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
