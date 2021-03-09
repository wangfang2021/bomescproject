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
                strSql.Append("       select *         \n");
                strSql.Append("       ,b.vcName as 'vcChange_Name'     \n");
                strSql.Append("       ,b2.vcName as 'vcHaoJiu_Name'      \n");
                strSql.Append("       ,b3.vcName as 'vcProjectType_Name'      \n");
                strSql.Append("       ,b4.vcName as 'vcOE_Name'      \n");
                strSql.Append("       ,b5.vcName as 'vcPriceState_Name'      \n");
                strSql.Append("       ,b6.vcName as 'vcOriginCompany_Name'      \n");
                strSql.Append("       ,b7.vcName as 'vcReceiver_Name'      \n");
                strSql.Append("       ,b8.vcName as 'vcPriceGS_Name'      \n");
                strSql.Append("       ,b9.vcName as 'vcPriceChangeInfo_Name'        \n");
                strSql.Append("       ,'0' as vcModFlag,'0' as vcAddFlag     \n");
                strSql.Append("       ,CONVERT(varchar(100),dUseBegin, 111) as dUseBeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dUseEnd, 111) as dUseEndStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dProjectBegin, 111) as dProjectBeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dProjectEnd, 111) as dProjectEndStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dJiuBegin, 111) as dJiuBeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dJiuEnd, 111) as dJiuEndStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dJiuBeginSustain, 111) as dJiuBeginSustainStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dPriceStateDate, 111) as dPriceStateDateStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dPricebegin, 111) as dPricebeginStr      \n");
                strSql.Append("       ,CONVERT(varchar(100),dPriceEnd, 111) as dPriceEndStr      \n");

                strSql.Append("       from TPrice a     \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C002'      \n");
                strSql.Append("       )b on a.vcChange=b.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C004'      \n");
                strSql.Append("       )b2 on a.vcHaoJiu=b2.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C003'      \n");
                strSql.Append("       )b3 on a.vcProjectType=b3.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C012'      \n");
                strSql.Append("       )b4 on a.vcOE=b4.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C013'      \n");
                strSql.Append("       )b5 on a.vcPriceState=b5.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C006'      \n");
                strSql.Append("       )b6 on a.vcOriginCompany=b6.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C005'      \n");
                strSql.Append("       )b7 on a.vcReceiver=b7.vcValue      \n");
                strSql.Append("       left join      \n");
                strSql.Append("       (      \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C038'      \n");
                strSql.Append("       )b8 on a.vcPriceGS=b8.vcValue      \n");
                strSql.Append("       left join             \n");
                strSql.Append("       (             \n");
                strSql.Append("          select vcValue,vcName from TCode where vcCodeId='C002'        \n");
                strSql.Append("       )b9 on a.vcPriceChangeInfo=b9.vcValue           \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if(strChange!=null && strChange != "" && strChange != "空")
                    strSql.Append("       and vcChange='" + strChange + "'         \n");

                if (strChange != null && strChange == "空")
                    strSql.Append("       and vcChange=''         \n");


                if (strPart_id != null && strPart_id != "")
                    strSql.Append("       and vcPart_id like '%" + strPart_id + "%'         \n");
                if (strOriginCompany != null && strOriginCompany != "")
                    strSql.Append("       and vcOriginCompany like '%" + strOriginCompany + "%'         \n");
                if (strHaoJiu != null && strHaoJiu != "")
                    strSql.Append("       and vcHaoJiu='" + strHaoJiu + "'         \n");
                if (strProjectType != null && strProjectType != "")
                    strSql.Append("       and vcProjectType='"+ strProjectType + "'         \n");
                if (strPriceChangeInfo != null && strPriceChangeInfo != "")
                    strSql.Append("       and vcPriceChangeInfo='" + strPriceChangeInfo + "'         \n");
                if (strCarTypeDev != null && strCarTypeDev != "")
                    strSql.Append("       and vcCarTypeDev like '" + strCarTypeDev + "%'         \n");
                if (strSupplier_id != null && strSupplier_id != "")
                    strSql.Append("       and vcSupplier_id like '" + strSupplier_id + "%'         \n");
                if (strReceiver != null && strReceiver != "")
                    strSql.Append("       and vcReceiver like '%" + strReceiver + "%'         \n");
                if (strPriceState != null && strPriceState != "")
                    strSql.Append("       and vcPriceState='" + strPriceState + "'         \n");
                strSql.Append("     order by  vcReceiver,vcSupplier_id,vcPart_id,dOperatorTime asc    \n");
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
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                decimal decPriceXS=Convert.ToDecimal(ComFunction.getTCode("C008").Rows[0]["vcValue"]);//价格系数
        
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("  insert into TPrice(vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu   \r\n");
                        sql.Append("  ,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcPriceChangeInfo,vcPriceState,dPriceStateDate,vcPriceGS,decPriceOrigin,decPriceAfter,decPriceTNPWithTax   \r\n");
                        sql.Append("  ,dPricebegin,dPriceEnd,vcCarTypeDev,vcCarTypeDesign,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO,vcSumLater,vcReceiver,vcOriginCompany,vcOperatorID,dOperatorTime,vcLastTimeFlag   \r\n");
                        sql.Append("  )   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dUseBegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dUseEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcProjectType"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dProjectBegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dProjectEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dJiuBeginSustain"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPriceState"], false) + ",  \r\n");
                        sql.Append("getDate(),  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPriceGS"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["decPriceOrigin"], true) + ",  \r\n");

                        //以下两个字段直接用前台输入框的金额，系统不做重新计算（防止更新的跟用户看见的不一致）
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["decPriceAfter"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["decPriceTNPWithTax"], true) + ",  \r\n");

                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dPricebegin"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["dPriceEnd"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_Name"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id_HK"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcStateFX"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXNO"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + ",  \r\n");
                        sql.Append("'"+ strUserId + "',  \r\n");
                        sql.Append("getdate(),  \r\n");
                        sql.Append("'" + strLastTimeFlag + "'");
                        sql.Append(" );  \r\n");
                        sql.Append("  update TPrice set vcPriceState='0',dPriceStateDate=GETDATE() where  vcLastTimeFlag='" + strLastTimeFlag + "' ; \r\n");
                        sql.Append("  update TPrice set vcChange="+ ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) 
                            + "  where vcPart_id="+ ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) 
                            + "  and vcSupplier_id="+ ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) 
                            + "  and vcReceiver="+ ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) 
                            + " ; \r\n");
                        //以下补全旧型十年的内容（新增的，这部分没有）
                        sql.Append("   update TPrice set    \r\n");
                        sql.Append("   vcnum1=c.vcNum1,vcnum2=c.vcNum2,vcnum3=c.vcNum3,vcnum4=c.vcNum4,vcnum5=c.vcNum5,vcnum6=c.vcNum6,vcnum7=c.vcNum7,vcnum8=c.vcNum8,vcnum9=c.vcNum9,vcnum10=c.vcNum10   \r\n");
                        sql.Append("   ,vcnum11=c.vcNum11,vcnum12=c.vcNum12,vcnum13=c.vcNum13,vcnum14=c.vcNum14,vcnum15=c.vcNum15,vcSumLater=c.vcSumLater   \r\n");
                        sql.Append("   from TPrice a   \r\n");
                        sql.Append("   inner join   \r\n");
                        sql.Append("   (   \r\n");
                        sql.Append("      select vcReceiver,vcSupplier_id,vcPart_id,MIN(iAutoId) as iAutoId from TPrice where vcLastTimeFlag<>'" + strLastTimeFlag + "'   \r\n");
                        sql.Append("      group by vcReceiver,vcSupplier_id,vcPart_id   \r\n");
                        sql.Append("   )b on a.vcPart_id=b.vcPart_id and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver   \r\n");
                        sql.Append("   inner join   \r\n");
                        sql.Append("   (   \r\n");
                        sql.Append("      select iAutoId,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10   \r\n");
                        sql.Append("      ,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcSumLater from TPrice      \r\n");
                        sql.Append("   )c on b.iAutoId=c.iAutoId    \r\n");
                        sql.Append("   where a.vcLastTimeFlag='" + strLastTimeFlag + "'   \r\n");
                        sql.Append("   and a.vcPart_id=" + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "     \r\n");
                        sql.Append("   and a.vcSupplier_id=" + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) +"     \r\n");
                        sql.Append("   and a.vcReceiver=" + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) +"    \r\n");
                        sql.Append("   ;   \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId =Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TPrice set    \r\n");
                        sql.Append("  vcChange=" + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "   \r\n");
                        sql.Append("  ,vcPriceChangeInfo="+ ComFunction.getSqlValue(listInfoData[i]["vcPriceChangeInfo"], false) + "   \r\n");
                        sql.Append("  ,vcPriceGS=" + ComFunction.getSqlValue(listInfoData[i]["vcPriceGS"], true) + "   \r\n");
                        sql.Append("  ,decPriceOrigin=" + ComFunction.getSqlValue(listInfoData[i]["decPriceOrigin"], true) + "   \r\n");

                        //以下两个字段直接用前台输入框的金额，系统不做重新计算（防止更新的跟用户看见的不一致）
                        sql.Append("  ,decPriceAfter=" + ComFunction.getSqlValue(listInfoData[i]["decPriceAfter"], true) + "   \r\n");
                        sql.Append("  ,decPriceTNPWithTax=" + ComFunction.getSqlValue(listInfoData[i]["decPriceTNPWithTax"], true) + "   \r\n");
                    
                        sql.Append("  ,dPricebegin=" + ComFunction.getSqlValue(listInfoData[i]["dPricebegin"], true) + "   \r\n");
                        sql.Append("  ,dPriceEnd=" + ComFunction.getSqlValue(listInfoData[i]["dPriceEnd"], true) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        //sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  ,vcLastTimeFlag='" + strLastTimeFlag + "'   \r\n");
                        
                        sql.Append("  where iAutoId="+ iAutoId + "  ; \r\n");
                        sql.Append("  update TPrice set vcPriceState='3',dPriceStateDate=GETDATE() where decPriceTNPWithTax is not null and vcLastTimeFlag='" + strLastTimeFlag + "'  \r\n");
                        sql.Append("  update TPrice set vcPriceState='2',dPriceStateDate=GETDATE() where isnull(vcChange,'')<>'' and decPriceOrigin is not null and decPriceTNPWithTax is null  and vcLastTimeFlag='" + strLastTimeFlag + "'    \r\n");
                    }
                }
 

                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorPart varchar(4000)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcPart_id from TPrice a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from TPrice   \r\n");
                    sql.Append("  		)b on a.vcPart_id=b.vcPart_id and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		   and    \r\n");
                    sql.Append("  		   (   \r\n");
                    sql.Append("  			   (a.dPricebegin>=b.dPricebegin and a.dPricebegin<=b.dPriceEnd)   \r\n");
                    sql.Append("  			   or   \r\n");
                    sql.Append("  			   (a.dPriceEnd>=b.dPricebegin and a.dPriceEnd<=b.dPriceEnd)   \r\n");
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

        

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, ref string strErrorPartId,bool isWuBtnVisible)
        {
            try
            {
                decimal decPriceXS = Convert.ToDecimal(ComFunction.getTCode("C008").Rows[0]["vcValue"]);//价格系数
               
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strAutoId = dt.Rows[i]["iAutoId"] == System.DBNull.Value ? "" : dt.Rows[i]["iAutoId"].ToString();
                    string vcPart_id = dt.Rows[i]["vcPart_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_id"].ToString();
                    string dUseBegin = dt.Rows[i]["dUseBegin"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseBegin"].ToString();
                    string dUseEnd = dt.Rows[i]["dUseEnd"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseEnd"].ToString();

                    if (isWuBtnVisible)
                    {
                        sql.Append("  update TPrice set    \r\n");
                        sql.Append("  vcPriceChangeInfo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPriceChangeInfo"], false) + "   \r\n");
                        sql.Append("  ,vcPriceGS=" + ComFunction.getSqlValue(dt.Rows[i]["vcPriceGS_Name"], true) + "   \r\n");
                        sql.Append("  ,decPriceOrigin=" + ComFunction.getSqlValue(dt.Rows[i]["decPriceOrigin"], false) + "   \r\n");


                        //以下两个字段直接用前台输入框的金额，系统不做重新计算（防止更新的跟用户看见的不一致）
                        sql.Append("  ,decPriceAfter=" + ComFunction.getSqlValue(dt.Rows[i]["decPriceAfter"], true) + "   \r\n");
                        sql.Append("  ,decPriceTNPWithTax=" + ComFunction.getSqlValue(dt.Rows[i]["decPriceTNPWithTax"], true) + "   \r\n");


                        sql.Append("  ,dPricebegin=" + ComFunction.getSqlValue(dt.Rows[i]["dPricebegin"], true) + "   \r\n");
                        sql.Append("  ,dPriceEnd=" + ComFunction.getSqlValue(dt.Rows[i]["dPriceEnd"], true) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        //sql.Append("  ,dOperatorTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + strAutoId + "  ; \r\n");
                        sql.Append("  update TPrice set vcPriceState='3',dPriceStateDate=GETDATE() where decPriceTNPWithTax is not null and vcPriceState is null   \r\n");
                        sql.Append("  update TPrice set vcPriceState='2',dPriceStateDate=GETDATE() where decPriceOrigin is not null and vcPriceState is null   \r\n");

                    }
                    else
                    {
                        sql.Append("  update TPrice set    \r\n");
                        sql.Append("  decPriceOrigin=" + ComFunction.getSqlValue(dt.Rows[i]["decPriceOrigin"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + strUserId + "'   \r\n");
                        sql.Append("  where iAutoId=" + strAutoId + "  ; \r\n");
                        sql.Append("  update TPrice set vcPriceState='3',dPriceStateDate=GETDATE() where decPriceTNPWithTax is not null and vcPriceState is null   \r\n");
                        sql.Append("  update TPrice set vcPriceState='2',dPriceStateDate=GETDATE() where decPriceOrigin is not null and vcPriceState is null   \r\n");
                    }
                }
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorPart varchar(4000)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcPart_id from TPrice a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from TPrice   \r\n");
                    sql.Append("  		)b on a.vcPart_id=b.vcPart_id and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		   and    \r\n");
                    sql.Append("  		   (   \r\n");
                    sql.Append("  			   (a.dPricebegin>=b.dPricebegin and a.dPricebegin<=b.dPriceEnd)   \r\n");
                    sql.Append("  			   or   \r\n");
                    sql.Append("  			   (a.dPriceEnd>=b.dPricebegin and a.dPriceEnd<=b.dPriceEnd)   \r\n");
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
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
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
                if (!string.IsNullOrEmpty(strBegin))
                    strSql.Append("   and    dBegin>='"+ strBegin + "'         \n");
                if (!string.IsNullOrEmpty(strEnd))
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
                        sql.Append("  insert into TPrice_GS(vcGSName,decXiShu,decTaxRate,vcArea,dBegin,dEnd,vcReason,vcOperatorID,dOperatorTime   \r\n");
                        sql.Append("  )   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcGSName"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["decXiShu"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["decTaxRate"], false) + ",  \r\n");
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
                        sql.Append("   vcGSName = " + ComFunction.getSqlValue(listInfoData[i]["vcGSName"], false) + "    \r\n");
                        sql.Append("  ,decXiShu = " + ComFunction.getSqlValue(listInfoData[i]["decXiShu"], false) + "   \r\n");
                        sql.Append("  ,decTaxRate = " + ComFunction.getSqlValue(listInfoData[i]["decTaxRate"], false) + "   \r\n");
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
                    sql.Append("  	  DECLARE @errorName varchar(50)      \r\n");
                    sql.Append("  	  set @errorName=''      \r\n");
                    sql.Append("  	  set @errorName=(      \r\n");
                    sql.Append("  	  	select vcName +';' from      \r\n");
                    sql.Append("  	  	(      \r\n");
                    sql.Append("  	  		select distinct a.vcName from    \r\n");
                    sql.Append("  			(   \r\n");
                    sql.Append("  				select * from TPrice_GS a   \r\n");
                    sql.Append("  				inner join   \r\n");
                    sql.Append("  				(   \r\n");
                    sql.Append("  					select vcValue,vcName from TCode where vcCodeId = 'C038'   \r\n");
                    sql.Append("  				) b   \r\n");
                    sql.Append("  				on a.vcGSName = b.vcValue   \r\n");
                    sql.Append("  			) a      \r\n");
                    sql.Append("  	  		left join      \r\n");
                    sql.Append("  	  		(      \r\n");
                    sql.Append("  	  		   select * from TPrice_GS      \r\n");
                    sql.Append("  	  		)b on a.vcGSName=b.vcGSName and a.iAutoId<>b.iAutoId      \r\n");
                    sql.Append("  	  		   and       \r\n");
                    sql.Append("  	  		   (      \r\n");
                    sql.Append("  	  			   (a.dBegin>=b.dBegin and a.dBegin<=b.dEnd)      \r\n");
                    sql.Append("  	  			   or      \r\n");
                    sql.Append("  	  			   (a.dEnd>=b.dBegin and a.dEnd<=b.dEnd)      \r\n");
                    sql.Append("  	  		   )      \r\n");
                    sql.Append("  	  		where b.iAutoId is not null      \r\n");
                    sql.Append("  	  	)a for xml path('')      \r\n");
                    sql.Append("  	  )      \r\n");
                    sql.Append("  	         \r\n");
                    sql.Append("  	  if @errorName<>''      \r\n");
                    sql.Append("  	  begin      \r\n");
                    sql.Append("  	    select CONVERT(int,'-->'+@errorName+'<--')      \r\n");
                    sql.Append("  	  end       \r\n");

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


        #region 检索所有待处理的数据
        public DataTable getAllTask()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select  * from TPrice where vcPriceState='0'      \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 获取十年年计
        public DataTable getOld_10_Year(List<Dictionary<string, object>> listInfoData)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("        if object_id('tempdb..#TPrice10Year_temp') is not null  \n");
                strSql.Append("        Begin  \n");
                strSql.Append("        drop  table #TPrice10Year_temp  \n");
                strSql.Append("        End  \n");
                strSql.Append("        select * into #TPrice10Year_temp from       \n");
                strSql.Append("        (      \n");
                strSql.Append("      	  select vcSupplier_id,vcReceiver,vcPart_id from TPrice where 1=0      \n");
                strSql.Append("        ) a      ;\n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                    string strReceiver = listInfoData[i]["vcReceiver"].ToString();
                    string strPart_id = listInfoData[i]["vcPart_id"].ToString();

                    strSql.Append("      insert into #TPrice10Year_temp       \n");
                    strSql.Append("       (         \n");
                    strSql.Append("        vcSupplier_id,vcReceiver,vcPart_id        \n");
                    strSql.Append("       ) values         \n");
                    strSql.Append("      (      \n");
                    strSql.Append("       '" + strSupplier_id + "'      \n");
                    strSql.Append("      ,'" + strReceiver + "'      \n");
                    strSql.Append("      ,'" + strPart_id + "'      \n");
                    strSql.Append("      );      \n");
                }
                strSql.Append("   select ROW_NUMBER() over(order by vcPart_id) as iNo,* from     \n");
                strSql.Append("   (      \n");
                strSql.Append("     select distinct a.vcPart_id,a.vcCarTypeDev,a.vcPart_Name,a.vcSupplier_Name,a.vcSupplier_id,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10 from TPrice a          \n");
                strSql.Append("     inner join          \n");
                strSql.Append("     (          \n");
                strSql.Append("        select * from #TPrice10Year_temp          \n");
                strSql.Append("     )b on a.vcReceiver=b.vcReceiver and a.vcSupplier_id=b.vcSupplier_id and a.vcPart_id=b.vcPart_id          \n");
                strSql.Append("     where a.vcHaoJiu='Q'         \n");//只有旧型才有十年年记
                strSql.Append("   )a     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 测试10万
        public DataTable test10W()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select top 100 '0' as selected,vcPart_Id,vcPartName,vcSupplier_Id,vccnName,iPackIngqtYout from TestABC      \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 销售展开
        public void sendMail(List<Dictionary<string,object>> listInfoData, ref string strErr)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                #region 创建临时表，将所选数据插入临时表
                strSql.Append("        if object_id('tempdb..#TPrice_temp') is not null        \r\n");
                strSql.Append("        begin         \r\n");
                strSql.Append("        drop table #TPrice_temp        \r\n");
                strSql.Append("        end        \r\n");
                strSql.Append("        select * into #TPrice_temp from         \r\n");
                strSql.Append("        (        \r\n");
                strSql.Append("        select * from TPrice where 1=0        \r\n");
                strSql.Append("        ) a ;       \r\n");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strSql.Append("       insert into #TPrice_temp        \r\n");
                    strSql.Append("       (        \r\n");
                    strSql.Append("        vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType,vcSupplier_id,vcSupplier_Name        \r\n");
                    strSql.Append("       ,dProjectBegin,dProjectEnd,vcHaoJiu,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcPriceChangeInfo        \r\n");
                    strSql.Append("       ,vcPriceState,dPriceStateDate,vcPriceGS,decPriceOrigin,decPriceAfter,decPriceTNPWithTax,dPricebegin        \r\n");
                    strSql.Append("       ,dPriceEnd,vcCarTypeDev,vcCarTypeDesign,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX        \r\n");
                    strSql.Append("       ,vcFXNO,vcReceiver,vcOriginCompany,dDataSyncTime,vcOperatorID,dOperatorTime,vcLastTimeFlag        \r\n");
                    strSql.Append("       ,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7        \r\n");
                    strSql.Append("       ,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14        \r\n");
                    strSql.Append("       ,vcNum15,vcSumLater        \r\n");
                    strSql.Append("       )        \r\n");
                    strSql.Append("       values        \r\n");
                    strSql.Append("       (        \r\n");
                    strSql.Append("       	  " + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "              \r\n");
                    strSql.Append("       	 ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "             \r\n");
                    strSql.Append("       	 ," + ComFunction.getSqlValue(listInfoData[i]["dUseBegin"], false) + "             \r\n");
                    strSql.Append("       	 ," + ComFunction.getSqlValue(listInfoData[i]["dUseEnd"], false) + "               \r\n");
                    strSql.Append("       	 ," + ComFunction.getSqlValue(listInfoData[i]["vcProjectType"], false) + "         \r\n");
                    strSql.Append("       	 ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "         \r\n");
                    strSql.Append("       	 ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "       \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dProjectBegin"], false) + "         \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dProjectEnd"], false) + "           \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "              \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], false) + "             \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dJiuBeginSustain"], false) + "      \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcPriceChangeInfo"], false) + "     \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcPriceState"], false) + "          \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dPriceStateDate"], false) + "       \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcPriceGS"], false) + "             \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["decPriceOrigin"], false) + "        \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["decPriceAfter"], false) + "         \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["decPriceTNPWithTax"], false) + "    \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dPricebegin"], false) + "           \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dPriceEnd"], false) + "             \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + "          \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "       \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_Name"], false) + "           \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "                  \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id_HK"], false) + "          \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcStateFX"], false) + "             \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNO"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "            \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + "       \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dDataSyncTime"], false) + "         \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcOperatorID"], false) + "          \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["dOperatorTime"], false) + "         \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcLastTimeFlag"], false) + "        \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + "                \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + "               \r\n");
                    strSql.Append("          ," + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "            \r\n");
                    strSql.Append("       )        \r\n");
                }

                #endregion

                #region 根据主键：品番、供应商代码、收货方，将变更事项改为null，再根据所选iAutoId将价格状态改为PIC（value值为4）
                strSql.AppendLine("        update TPrice set        \r\n");
                strSql.AppendLine("        vcChange = null        \r\n");
                strSql.AppendLine("        from TPrice a        \r\n");
                strSql.AppendLine("        inner join        \r\n");
                strSql.AppendLine("        (        \r\n");
                strSql.AppendLine("        	select vcPart_id,vcSupplier_id,vcReceiver from #TPrice_temp        \r\n");
                strSql.AppendLine("        	group by vcPart_id,vcSupplier_id,vcReceiver        \r\n");
                strSql.AppendLine("        ) b        \r\n");
                strSql.AppendLine("        on a.vcPart_id = b.vcPart_id        \r\n");
                strSql.AppendLine("        and a.vcSupplier_id = b.vcSupplier_id        \r\n");
                strSql.AppendLine("        and a.vcReceiver = b.vcReceiver        \r\n");
                strSql.AppendLine("                \r\n");
                strSql.AppendLine("                \r\n");
                strSql.AppendLine("        update TPrice set vcPriceState = '4'        \r\n");
                strSql.AppendLine("        from TPrice a        \r\n");
                strSql.AppendLine("        inner join         \r\n");
                strSql.AppendLine("        (        \r\n");
                strSql.AppendLine("        	select iAutoId from #TPrice_temp        \r\n");
                strSql.AppendLine("        ) b        \r\n");
                strSql.AppendLine("        on a.iAutoId = b.iAutoId        \r\n");
                #endregion

                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }
            }
            catch (Exception ex)
            {
                strErr += "操作失败:" + ex.Message;
                throw ex;
            }
        }
        #endregion

        #region 向调达送信后变更价格处理状态
        public void sendDiaoDaChangeState(List<Dictionary<string, object>> listInfoData, ref string strErr)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strSql.AppendLine("      update TPrice set vcPriceState='1',dPriceStateDate=GETDATE()       ");
                    strSql.AppendLine("      where iAutoId = '" + listInfoData[i]["iAutoId"] + "'  and vcPriceState='0'       ");
                }
                if (strSql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }
            }
            catch (Exception ex)
            {
                strErr += "操作失败:" + ex.Message;
                throw ex;
            }
        }
        #endregion


        #region 获取邮件内容
        public string getEmailBody(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcContent from TMailMessageSetting where vcUserId = '" + strUserId + "' and vcChildFunID = 'FS0309'       ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取邮件主题
        /// <summary>
        /// 获取邮件主题
        /// </summary>
        /// <param name="strUserId"></param>
        /// <returns></returns>
        public string getEmailSubject(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcTitle from TMailMessageSetting where vcUserId = '" + strUserId + "' and vcChildFunID = 'FS0309'       ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取收件人
        /// <summary>
        /// 获取所有已经维护的收件人信息
        /// </summary>
        /// <returns></returns>
        public DataTable getreceiverDt()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("       select vcValue2 as 'address',vcValue1 as 'displayName' from TOutCode where vcCodeId = 'C006' and vcIsColum = '0' and vcValue1 is not null and vcValue1 <> '' and vcValue2 is not null and vcValue2 <> ''      ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据选择公式返回对应金额
        public DataTable getGSChangePrice(string strPartId,string strSupplier,string strReceiver, string strAutoId, string strGSName,decimal decPriceOrigin)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    declare @lastPriceOrigin decimal(18,2) --上一状态原值          \n");
                strSql.Append("    declare @lastPriceAfter decimal(18,2) --上一状态参考值          \n");
                strSql.Append("    declare @lastPriceTNPWithTax decimal(18,2) --上一状态含税          \n");
                strSql.Append("    declare @priceOrigin decimal(18,2) --当前状态原值          \n");
                strSql.Append("    declare @priceAfter decimal(18,2) --当前状态参考值          \n");
                strSql.Append("    declare @priceTNPWithTax decimal(18,2) --当前状态含税          \n");
                strSql.Append("    declare @decXiShu decimal(18,2) --公式系数          \n");
                strSql.Append("    declare @decTaxRate decimal(18,2) --税率          \n");
                strSql.Append("              \n");
                strSql.Append("    set @priceOrigin="+ decPriceOrigin + "         \n");
                strSql.Append("              \n");
                strSql.Append("    select @lastPriceOrigin=b.decPriceOrigin,@lastPriceAfter=b.decPriceAfter,@lastPriceTNPWithTax=b.decPriceTNPWithTax from          \n");
                strSql.Append("    (          \n");
                strSql.Append("       select vcSupplier_id,vcPart_id,vcReceiver,max(iAutoId) as iMaxId from TPrice           \n");
                strSql.Append("       where   1=1       \n");
                if(strAutoId!="")
                 strSql.Append("       and iAutoId<>"+ strAutoId + "          \n");
                strSql.Append("       and vcPart_id='"+ strPartId + "'          \n");
                strSql.Append("       and vcSupplier_id='" + strSupplier + "'          \n");
                strSql.Append("       and vcReceiver='" + strReceiver + "'                     \n");
                strSql.Append("       group by vcSupplier_id,vcPart_id,vcReceiver          \n");
                strSql.Append("    )a           \n");
                strSql.Append("    left join          \n");
                strSql.Append("    (          \n");
                strSql.Append("       select * from TPrice          \n");
                strSql.Append("    )b on  a.iMaxId=b.iAutoId          \n");
                strSql.Append("              \n");
                strSql.Append("    select @decXiShu=decXiShu,@decTaxRate=decTaxRate from TPrice_GS where vcGSName='"+ strGSName + "' and GETDATE()>dBegin and GETDATE()<dEnd          \n");
                strSql.Append("              \n");

                strSql.Append("    --公式A	          \n");
                strSql.Append("    --参考值	原价*系数，不做小数保留          \n");
                strSql.Append("    --TNP含税	原价*系数*税率，保留1位          \n");

                strSql.Append("    if('" + strGSName + "'='1')          \n");//公式A
                strSql.Append("    begin           \n");
                strSql.Append("    set @priceAfter=@priceOrigin*@decXiShu            \n");
                strSql.Append("    set @priceTNPWithTax=round(@priceOrigin*@decXiShu*@decTaxRate,1)            \n");
                strSql.Append("    end          \n");
                strSql.Append("              \n");

                strSql.Append("    --公式B	          \n");
                strSql.Append("    --参考值	上一状态的参考值-上一状态的原价+本状态的原价											          \n");
                strSql.Append("    --TNP含税	(本状态参考值*税率-上一状态TNP含税)>0?本状态参考值*税率:上一状态TNP含税		          \n");

                strSql.Append("    if('" + strGSName + "'='2' )          \n");//公式B
                strSql.Append("    begin          \n");
                strSql.Append("    	set @priceAfter=@lastPriceAfter-@lastPriceOrigin+@priceOrigin          \n");
                strSql.Append("    	if(@priceAfter*@decTaxRate-@lastPriceTNPWithTax)>0          \n");
                strSql.Append("    	begin           \n");
                strSql.Append("    	  set @priceTNPWithTax=@priceAfter*@decTaxRate          \n");
                strSql.Append("    	end          \n");
                strSql.Append("    	else          \n");
                strSql.Append("    	begin           \n");
                strSql.Append("    	  set @priceTNPWithTax=@lastPriceTNPWithTax          \n");
                strSql.Append("    	end          \n");
                strSql.Append("    end          \n");
                strSql.Append("              \n");
                strSql.Append("    if('" + strGSName + "'='3')          \n");//公式C
                strSql.Append("    begin           \n");
                strSql.Append("    set @priceAfter=null          \n");
                strSql.Append("    set @priceTNPWithTax=null          \n");
                strSql.Append("    end          \n");
                strSql.Append("    select @priceAfter as priceAfter,@priceTNPWithTax as priceTNPWithTax           \n");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 公式计算B需要验证该品番是否存在上个状态的数据
        public DataTable getLastStateGsData(string strPartId, string strSupplier,string strReceiver, string strAutoId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcSupplier_id,vcPart_id,vcReceiver,max(iAutoId) as iMaxId from TPrice             \n");
                strSql.Append("    where  1=1                   \n");
                if(strAutoId!="")
                    strSql.Append("  and  iAutoId<>"+ strAutoId + "                     \n");
                strSql.Append("    and vcPart_id='"+ strPartId + "'                     \n");
                strSql.Append("    and vcSupplier_id='"+ strSupplier + "'                     \n");
                strSql.Append("    and vcReceiver='" + strReceiver + "'                     \n");
                strSql.Append("    group by vcSupplier_id,vcPart_id,vcReceiver                \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 公式是否存在
        public DataTable isGsExist(string strGs)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select  decXiShu,decTaxRate from TPrice_GS where vcGSName='"+ strGs + "' and GETDATE()>dBegin and GETDATE()<dEnd           \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
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
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString()).Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion
 
    }
}