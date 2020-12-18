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
    public class FS0309_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       select *,'0' as vcModFlag,'0' as vcAddFlag from TPrice         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if(strChange!="")
                    strSql.Append("       and vcChange='" + strChange + "'         \n");
                if (strPart_id != "")
                    strSql.Append("       and vcPart_id like '%" + strPart_id + "%'         \n");
                if (strOriginCompany != "")
                    strSql.Append("       and vcOriginCompany like '%" + strOriginCompany + "%'         \n");
                if (strHaoJiu != "")
                    strSql.Append("       and vcHaoJiu='" + strHaoJiu + "'         \n");
                if (strProjectType != "")
                    strSql.Append("       and vcProjectType='"+ strProjectType + "'         \n");
                if (strPriceChangeInfo != "")
                    strSql.Append("       and vcPriceChangeInfo='" + strPriceChangeInfo + "'         \n");
                if (strCarTypeDev != "")
                    strSql.Append("       and vcCarTypeDev='" + strCarTypeDev + "'         \n");
                if (strSupplier_id != "")
                    strSql.Append("       and vcSupplier_id='" + strSupplier_id + "'         \n");
                if (strReceiver != "")
                    strSql.Append("       and vcReceiver like '%" + strReceiver + "%'         \n");
                if (strPriceState != "")
                    strSql.Append("       and vcPriceState='" + strPriceState + "'         \n");
                strSql.Append("     order by  vcPart_id    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            try
            {
                decimal decPriceXS=Convert.ToDecimal(ComFunction.getTCode("C008").Rows[0]["vcValue"]);//价格系数
                DataTable dtGS=getAllGS();//公式列表


                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("  insert into TPrice(vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu   \r\n");
                        sql.Append("  ,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcPriceChangeInfo,vcPriceState,dPriceStateDate,vcPriceGS,decPriceOrigin,decPriceAfter,decPriceTNPWithTax   \r\n");
                        sql.Append("  ,dPricebegin,dPriceEnd,vcCarTypeDev,vcCarTypeDesign,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO,vcSumLater,vcReceiver,vcOriginCompany,vcOperatorID,dOperatorTime   \r\n");
                        sql.Append("  )   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcChange"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPart_id"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dUseBegin"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dUseEnd"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcProjectType"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dProjectBegin"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dProjectEnd"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dJiuBegin"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dJiuEnd"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dJiuBeginSustain"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPriceChangeInfo"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPriceState"], false) + ",  \r\n");
                        sql.Append("getDate(),  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPriceGS"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["decPriceOrigin"], true) + ",  \r\n");

                        //以下两个字段计算
                        if (listInfoData[i]["decPriceOrigin"] == System.DBNull.Value || listInfoData[i]["decPriceOrigin"] == null || listInfoData[i]["decPriceOrigin"].ToString()=="")
                            sql.Append("   null,   \r\n");
                        else
                            sql.Append(listInfoData[i]["decPriceOrigin"].ToString() + "*" + decPriceXS + ",   \r\n");
                        sql.Append(getJSSql(listInfoData[i]["decPriceOrigin"], listInfoData[i]["vcPriceGS"], dtGS) + ",   \r\n");

                        sql.Append(getSqlValue(listInfoData[i]["dPricebegin"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dPriceEnd"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPart_Name"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOE"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPart_id_HK"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcStateFX"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcFXNO"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcSumLater"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcReceiver"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOriginCompany"], false) + ",  \r\n");
                        sql.Append("'"+ strUserId + "',  \r\n");
                        sql.Append("getdate()  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId =Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice set    \r\n");
                        sql.Append("  vcPriceChangeInfo="+ getSqlValue(listInfoData[i]["vcPriceChangeInfo"], false) + "   \r\n");
                        sql.Append("  ,vcPriceGS=" + getSqlValue(listInfoData[i]["vcPriceGS"], false) + "   \r\n");
                        sql.Append("  ,decPriceOrigin=" + getSqlValue(listInfoData[i]["decPriceOrigin"], true) + "   \r\n");

                        //以下两个字段计算
                        if(listInfoData[i]["decPriceOrigin"] == System.DBNull.Value || listInfoData[i]["decPriceOrigin"] ==null|| listInfoData[i]["decPriceOrigin"].ToString() == "")
                            sql.Append("  ,decPriceAfter=null   \r\n");
                        else
                            sql.Append("  ,decPriceAfter="+ listInfoData[i]["decPriceOrigin"].ToString() + "*" + decPriceXS + "   \r\n");
                        sql.Append("  ,decPriceTNPWithTax=" + getJSSql(listInfoData[i]["decPriceOrigin"],listInfoData[i]["vcPriceGS"],dtGS) + "   \r\n");


                        sql.Append("  ,dPricebegin=" + getSqlValue(listInfoData[i]["dPricebegin"], true) + "   \r\n");
                        sql.Append("  ,dPriceEnd=" + getSqlValue(listInfoData[i]["dPriceEnd"], true) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  where iAutoId="+ iAutoId + "  ; \r\n");
                    }
                }
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorPart varchar(50)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcPart_id from TPrice a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from TPrice   \r\n");
                    sql.Append("  		)b on a.vcPart_id=b.vcPart_id and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		   and    \r\n");
                    sql.Append("  		   (   \r\n");
                    sql.Append("  			   (a.dUseBegin>=b.dUseBegin and a.dUseBegin<=b.dUseEnd)   \r\n");
                    sql.Append("  			   or   \r\n");
                    sql.Append("  			   (a.dUseEnd>=b.dUseBegin and a.dUseEnd<=b.dUseEnd)   \r\n");
                    sql.Append("  		   )   \r\n");
                    sql.Append("  		where b.iAutoId is not null   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorPart<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorPart+'<--')   \r\n");
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
                    strErrorPartId = ex.Message.Substring(startIndex+3, endIndex - startIndex-3);
                }
                else
                    throw ex;
            }
        }
        #endregion

        #region 根据公式返回计算语句
        public string getJSSql(Object decPriceOrigin, Object strGSName,DataTable gsdt)
        {
            if (strGSName == DBNull.Value || strGSName == null || strGSName.ToString().Trim() == "" || decPriceOrigin == DBNull.Value || decPriceOrigin == null || decPriceOrigin.ToString().Trim() == "")
                return "null";
            string strGS = "";
            for (int i = 0; i < gsdt.Rows.Count; i++)
            {
                string strTempName = gsdt.Rows[i]["vcName"].ToString();
                string strTempGS = gsdt.Rows[i]["vcGs"].ToString();
                if (strGSName.ToString() == strTempName)
                    strGS = strTempGS;

            }
            return "("+strGS.Replace("A", decPriceOrigin.ToString()) +")";
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPrice where iAutoId in(   \r\n ");
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

        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj,bool isObject)
        {
            if (obj == null)
                return "null";
            else if(obj.ToString().Trim()==""&& isObject)
                return "null";
            else
                return "'" + obj .ToString()+ "'";
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                decimal decPriceXS = Convert.ToDecimal(ComFunction.getTCode("C008").Rows[0]["vcValue"]);//价格系数
                DataTable dtGS = getAllGS();//公式列表

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPart_id = dt.Rows[i]["vcPart_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                    string dUseBegin = dt.Rows[i]["dUseBegin"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseBegin"].ToString();
                    string dUseEnd = dt.Rows[i]["dUseEnd"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseEnd"].ToString();

                    sql.Append("  update TPrice set    \r\n");
                    sql.Append("  vcPriceChangeInfo=" + getSqlValue(dt.Rows[i]["vcPriceChangeInfo"], false) + "   \r\n");
                    sql.Append("  ,vcPriceGS=" + getSqlValue(dt.Rows[i]["vcPriceGS"], false) + "   \r\n");
                    sql.Append("  ,decPriceOrigin=" + getSqlValue(dt.Rows[i]["decPriceOrigin"], false) + "   \r\n");


                    //以下两个字段计算
                    if (dt.Rows[i]["decPriceOrigin"] == System.DBNull.Value)
                        sql.Append("  ,decPriceAfter=null   \r\n");
                    else
                        sql.Append("  ,decPriceAfter=" + dt.Rows[i]["decPriceOrigin"].ToString() + "*" + decPriceXS + "   \r\n");
                    sql.Append("  ,decPriceTNPWithTax=" + getJSSql(dt.Rows[i]["decPriceOrigin"], dt.Rows[i]["vcPriceGS"], dtGS) + "   \r\n");


                    sql.Append("  ,dPricebegin=" + getSqlValue(dt.Rows[i]["dPricebegin"], true) + "   \r\n");
                    sql.Append("  ,dPriceEnd=" + getSqlValue(dt.Rows[i]["dPriceEnd"], true) + "   \r\n");
                    sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                    sql.Append("  ,dOperatorTime=getdate()   \r\n");
                    sql.Append("  where vcPart_id='" + vcPart_id + "'  and dUseBegin='"+ dUseBegin + "' and  dUseEnd='"+ dUseEnd + "' ; \r\n");

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

        #region 检索所有的公式供下拉框选择
        public DataTable getAllGS()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select vcName,vcName as vcValue,vcGs from tprice_gs        \n");
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
                    strSql.Append("   and    dBegin>='"+ strBegin + "'         \n");
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
                        sql.Append(getSqlValue(listInfoData[i]["vcName"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcGs"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcArea"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dBegin"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dEnd"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcReason"], false) + ",  \r\n");
                        sql.Append("'" + strUserId + "',  \r\n");
                        sql.Append("getdate()  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice_GS set    \r\n");
                        sql.Append("  vcName=" + getSqlValue(listInfoData[i]["vcName"], false) + "   \r\n");
                        sql.Append("  ,vcGs=" + getSqlValue(listInfoData[i]["vcGs"], false) + "   \r\n");
                        sql.Append("  ,vcArea=" + getSqlValue(listInfoData[i]["vcArea"], false) + "   \r\n");
                        sql.Append("  ,dBegin=" + getSqlValue(listInfoData[i]["dBegin"], true) + "   \r\n");
                        sql.Append("  ,dEnd=" + getSqlValue(listInfoData[i]["dEnd"], true) + "   \r\n");
                        sql.Append("  ,vcReason=" + getSqlValue(listInfoData[i]["vcReason"], false) + "   \r\n");
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

        #region 删除公式
        public void Del_GS(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPrice_GS where iAutoId in(   \r\n ");
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


    }
}
