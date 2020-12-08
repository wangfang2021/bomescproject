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
                    strSql.Append("       and vcPriceChangeInfo=''         \n");
                if (strPart_id != "")
                    strSql.Append("       and vcPart_id like '%%'         \n");
                if (strOriginCompany != "")
                    strSql.Append("       and vcOriginCompany like '%%'         \n");
                if (strHaoJiu != "")
                    strSql.Append("       and vcHaoJiu=''         \n");
                if (strProjectType != "")
                    strSql.Append("       and vcProjectType=''         \n");
                if (strPriceChangeInfo != "")
                    strSql.Append("       and vcPriceChangeInfo=''         \n");
                if (strCarTypeDev != "")
                    strSql.Append("       and vcCarTypeDev=''         \n");
                if (strSupplier_id != "")
                    strSql.Append("       and vcSupplier_id=''         \n");
                if (strReceiver != "")
                    strSql.Append("       and vcReceiver like '%%'         \n");
                if (strPriceState != "")
                    strSql.Append("       and vcPriceState=''         \n");
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
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("  insert into TPrice(vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu   \r\n");
                        sql.Append("  ,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcPriceChangeInfo,vcPriceState,dPriceSendDate,vcPriceGS,decPriceOrigin,decPriceAfter,decPriceTNPWithTax   \r\n");
                        sql.Append("  ,dPricebegin,dPriceEnd,vcCarTypeDev,vcCarTypeDesign,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO,vcSumLater,vcReceiver,vcOriginCompany   \r\n");
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
                        sql.Append(getSqlValue(listInfoData[i]["dPriceSendDate"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPriceGS"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["decPriceOrigin"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["decPriceAfter"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["decPriceTNPWithTax"], true) + ",  \r\n");
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
                        sql.Append(getSqlValue(listInfoData[i]["vcOriginCompany"], false) + "  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId =Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice set    \r\n");
                        sql.Append("  vcPriceChangeInfo="+ getSqlValue(listInfoData[i]["vcPriceChangeInfo"], false) + "   \r\n");
                        sql.Append("  ,vcPriceState=" + getSqlValue(listInfoData[i]["vcPriceState"], false) + "   \r\n");
                        sql.Append("  ,dPriceSendDate=" + getSqlValue(listInfoData[i]["dPriceSendDate"], true) + "   \r\n");
                        sql.Append("  ,vcPriceGS=" + getSqlValue(listInfoData[i]["vcPriceGS"], false) + "   \r\n");
                        sql.Append("  ,decPriceOrigin=" + getSqlValue(listInfoData[i]["decPriceOrigin"], true) + "   \r\n");
                        sql.Append("  ,decPriceAfter=" + getSqlValue(listInfoData[i]["decPriceAfter"], true) + "   \r\n");
                        sql.Append("  ,decPriceTNPWithTax=" + getSqlValue(listInfoData[i]["decPriceTNPWithTax"], true) + "   \r\n");
                        sql.Append("  ,dPricebegin=" + getSqlValue(listInfoData[i]["dPricebegin"], true) + "   \r\n");
                        sql.Append("  ,dPriceEnd=" + getSqlValue(listInfoData[i]["dPriceEnd"], true) + "   \r\n");
                        sql.Append("  where iAutoId="+ iAutoId + "  ; \r\n");
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

    }
}
