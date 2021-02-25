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
    public class FS0312_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strPart_id, string strSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select *    \n");
                strSql.Append("     ,b.vcName as 'vcHaoJiu_Name'    \n");
                strSql.Append("     ,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     from TPart_JX a    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C004'    \n");
                strSql.Append("     ) b on a.vcHaoJiu = b.vcValue    \n");
                strSql.Append("      where 1=1   ");
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.Append("      and vcPart_id like '"+strPart_id+"%'  ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id like '"+strSupplier_id+"%'   ");
                }
                strSql.Append("      order by vcPart_id       ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TFTM");
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
                        sql.Append("      insert into TPart_JX      \n");
                        sql.Append("      (      \n");
                        sql.Append("      vcPart_id,        \n");
                        sql.Append("      vcHaoJiu,         \n");
                        sql.Append("      dJiuBegin,        \n");
                        sql.Append("      vcSupplier_id,    \n");
                        sql.Append("      vcSupplier_Name,  \n");
                        sql.Append("      vcCarTypeDesign,  \n");
                        sql.Append("      vcPartName,       \n");
                        sql.Append("      vcNum1,       \n");
                        sql.Append("      vcNum2,       \n");
                        sql.Append("      vcNum3,       \n");
                        sql.Append("      vcNum4,       \n");
                        sql.Append("      vcNum5,       \n");
                        sql.Append("      vcNum6,       \n");
                        sql.Append("      vcNum7,       \n");
                        sql.Append("      vcNum8,       \n");
                        sql.Append("      vcNum9,       \n");
                        sql.Append("      vcNum10,       \n");
                        sql.Append("      dSendTime,        \n");
                        sql.Append("      vcOperatorID,     \n");
                        sql.Append("      dOperatorTime     \n");
                        sql.Append("      )       \n");
                        sql.Append("      values      \n");
                        sql.Append("      (      \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false)+",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",     \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + ",    \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",    \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",  \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",  \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum1"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum2"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum3"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum4"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum5"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum6"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum7"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum8"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum9"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcNum10"], true) + ",   \n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dSendTime"], true) + ",    \n");
                        sql.Append("'" + strUserId + "',     \r\n");
                        sql.Append("    GETDATE()     \r\n");
                        sql.Append("      )      \n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("      update TPart_JX set       \r\n");
                        sql.Append("      vcPart_id = "+ComFunction.getSqlValue(listInfoData[i]["vcPart_id"],false)+"      \r\n");
                        sql.Append("      ,vcHaoJiu = " + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], true) + "      \r\n");
                        sql.Append("      ,dJiuBegin = " + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "      \r\n");
                        sql.Append("      ,vcSupplier_Name = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "      \r\n");
                        sql.Append("      ,vcCarTypeDesign = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "      \r\n");
                        sql.Append("      ,vcPartName = " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "      \r\n");
                        sql.Append("      ,vcNum1 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], true) + "      \r\n");
                        sql.Append("      ,vcNum2 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], true) + "      \r\n");
                        sql.Append("      ,vcNum3 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], true) + "      \r\n");
                        sql.Append("      ,vcNum4 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], true) + "      \r\n");
                        sql.Append("      ,vcNum5 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], true) + "      \r\n");
                        sql.Append("      ,vcNum6 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], true) + "      \r\n");
                        sql.Append("      ,vcNum7 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], true) + "      \r\n");
                        sql.Append("      ,vcNum8 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], true) + "      \r\n");
                        sql.Append("      ,vcNum9 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], true) + "      \r\n");
                        sql.Append("      ,vcNum10 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], true) + "      \r\n");
                        sql.Append("      ,dSendTime = " + ComFunction.getSqlValue(listInfoData[i]["dSendTime"], true) + "      \r\n");
                        sql.Append("      ,vcOperatorID = " + strUserId+ "      \r\n");
                        sql.Append("      ,dOperatorTime = getdate()      \r\n");
                        sql.Append("      where iAutoId = "+iAutoId+"      \r\n");
                    }
                }
                //以下追加验证数据库中是否存在品番重叠判断，如果存在则终止提交，原单位数据唯一性：品番、包装工厂、供应商代码、收货方
                if (sql.Length > 0)
                {
                    sql.Append("        declare @errorPart varchar(5000)        \r\n");
                    sql.Append("        set @errorPart =         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("        select a.vcPart_id+';' from         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("           select distinct vcPart_id from         \r\n");
                    sql.Append("           (         \r\n");
                    sql.Append("               select vcPart_id,dJiuBegin from TPart_JX          \r\n");
                    sql.Append("         	  group by vcPart_id,dJiuBegin                 \r\n");
                    sql.Append("         	  having COUNT(*)>1                 \r\n");
                    sql.Append("           )a         \r\n");
                    sql.Append("        ) a for xml path('')        \r\n");
                    sql.Append("        )        \r\n");
                    sql.Append("        if @errorPart<>''        \r\n");
                    sql.Append("        begin        \r\n");
                    sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                    sql.Append("        end        \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TFTM");
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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("      insert into TPart_JX      \n");
                    sql.Append("      (      \n");
                    sql.Append("      vcPart_id,        \n");
                    sql.Append("      vcHaoJiu,         \n");
                    sql.Append("      dJiuBegin,        \n");
                    sql.Append("      vcSupplier_id,    \n");
                    sql.Append("      vcSupplier_Name,  \n");
                    sql.Append("      vcCarTypeDesign,  \n");
                    sql.Append("      vcPartName,       \n");
                    sql.Append("      vcNum1,       \n");
                    sql.Append("      vcNum2,       \n");
                    sql.Append("      vcNum3,       \n");
                    sql.Append("      vcNum4,       \n");
                    sql.Append("      vcNum5,       \n");
                    sql.Append("      vcNum6,       \n");
                    sql.Append("      vcNum7,       \n");
                    sql.Append("      vcNum8,       \n");
                    sql.Append("      vcNum9,       \n");
                    sql.Append("      vcNum10,       \n");
                    sql.Append("      dSendTime,        \n");
                    sql.Append("      vcOperatorID,     \n");
                    sql.Append("      dOperatorTime     \n");
                    sql.Append("      )       \n");
                    sql.Append("      values      \n");
                    sql.Append("      (      \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcHaoJiu_Name"], false) + ",     \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dJiuBegin"], false) + ",    \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false) + ",    \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_Name"], false) + ",  \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeDesign"], false) + ",  \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPartName"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum1"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum2"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum3"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum4"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum5"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum6"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum7"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum8"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum9"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcNum10"], false) + ",   \n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dSendTime"], false) + ",    \n");
                    sql.Append("'" + strUserId + "',     \r\n");
                    sql.Append("    GETDATE()     \r\n");
                    sql.Append("      )      \n");
                }
                if (sql.Length > 0)
                {
                    sql.Append("        declare @errorPart varchar(5000)        \r\n");
                    sql.Append("        set @errorPart =         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("        select a.vcPart_id+';' from         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("           select distinct vcPart_id from         \r\n");
                    sql.Append("           (         \r\n");
                    sql.Append("               select vcPart_id,dJiuBegin from TPart_JX          \r\n");
                    sql.Append("         	  group by vcPart_id,dJiuBegin                 \r\n");
                    sql.Append("         	  having COUNT(*)>1                 \r\n");
                    sql.Append("           )a         \r\n");
                    sql.Append("        ) a for xml path('')        \r\n");
                    sql.Append("        )        \r\n");
                    sql.Append("        if @errorPart<>''        \r\n");
                    sql.Append("        begin        \r\n");
                    sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                    sql.Append("        end        \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TFTM");
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

        #region 根据Name获取Value或者根据Value获取Name
        /// <summary>
        /// 根据Name获取Value或者根据Value获取Name
        /// </summary>
        /// <param name="codeId">Codeid</param>
        /// <param name="strNameOrValue">Name或Value</param>
        /// <param name="_bool">true:返回Value    false:返回Name</param>
        /// <returns></returns>
        public string Name2Value(string codeId, string strNameOrValue, bool _bool)
        {
            StringBuilder strSql = new StringBuilder();
            if (string.IsNullOrEmpty(strNameOrValue))
            {
                return null;
            }
            try
            {
                if (_bool)
                {
                    strSql.Append("     select vcValue from TCode     \n");
                    strSql.Append("     where vcCodeid = '" + codeId + "'    \n");
                    strSql.Append("     and vcName = '" + strNameOrValue + "'    \n");
                }
                else
                {
                    strSql.Append("     select vcName from TCode     \n");
                    strSql.Append("     where vcCodeid = '" + codeId + "'    \n");
                    strSql.Append("     and vcValue = '" + strNameOrValue + "'    \n");
                }

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK").Rows[0][0].ToString();
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
                sql.Append("  delete TPart_JX where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TFTM");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
