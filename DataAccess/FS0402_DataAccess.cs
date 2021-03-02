using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0402_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 获取数据字典
        public DataTable getTCode(string strCodeId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct case when vcValue in ('0','1') then '-' else vcName end as vcName,    \n");
                strSql.Append("case when vcValue in ('0','1') then '4' else vcValue end as vcValue    \n");
                strSql.Append("from TCode where vcCodeId='C036'    \n");
                strSql.Append("order by vcValue    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("SELECT iAutoId,vcYearMonth,vcDyState,vcHyState,vcPart_id,iCbSOQN,        \n");
                strSql.Append("case when decCbBdl>999 then '>999%' else cast(decCbBdl as varchar(5))+'%' end as decCbBdl,iCbSOQN1,iCbSOQN2,iTzhSOQN,iTzhSOQN1,iTzhSOQN2    \n");
                strSql.Append(",iHySOQN,iHySOQN1,iHySOQN2,dHyTime,case when a.vcDyState in ('0','1') then '-' else b.vcName end as 'vcDyState_Name',b2.vcName as 'vcHyState_Name'      \n");
                strSql.Append(" ,case       \n");
                strSql.Append(" when iTzhSOQN is null or iTzhSOQN1 is null or iTzhSOQN2 is null then 'partFS0402A' --无调整      \n");
                strSql.Append(" when iTzhSOQN=iCbSOQN and iTzhSOQN1=iCbSOQN1 and iTzhSOQN2=iCbSOQN2 then 'partFS0402A' --无调整      \n");
                strSql.Append(" else 'partFS0402B' --有调整      \n");
                strSql.Append(" end as vcBgColor                \n");
                strSql.Append("  FROM TSoq a  \n");
                strSql.Append("  left join      \n");
                strSql.Append("  (      \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId='C036'      \n");
                strSql.Append("  )b on a.vcDyState=b.vcValue      \n");
                strSql.Append("  left join      \n");
                strSql.Append("  (      \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId='C037'      \n");
                strSql.Append("  )b2 on a.vcHyState=b2.vcValue      \n");
                strSql.Append("  WHERE 1=1  \n");

                if (!string.IsNullOrEmpty(strYearMonth)) {//对象年月
                    strSql.Append(" and vcYearMonth='"+ strYearMonth + "'");
                }
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                {
                    if(strDyState=="4")
                        strSql.Append(" and vcDyState in ('0','1') ");
                    else
                        strSql.Append(" and vcDyState='"+ strDyState + "'");
                }
                if (!string.IsNullOrEmpty(strHyState))//合意状态
                {
                    strSql.Append(" and vcHyState='" + strHyState + "'");
                }
                if (!string.IsNullOrEmpty(strPart_id))//品番
                {
                    strSql.Append(" and vcPart_id like '%"+ strPart_id + "%'");
                }
                strSql.Append("order by a.iAutoId    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索导入履历
        public DataTable SearchHistory()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from TSoqInputErrDetail    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后校验
        public void importCheck(DataTable dt, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3, 
            ref Dictionary<string,string> errMessageDict, string strUnit)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" delete TSoq_temp where vcOperator='"+strUserId+"' and vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                #region 先插入临时表
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("  INSERT INTO TSoq_temp( ");
                    sql.Append("vcYearMonth,");
                    sql.Append("vcDyState,");
                    sql.Append("vcHyState,");
                    sql.Append("vcPart_id,");
                    sql.Append("iCbSOQN,");
                    sql.Append("iCbSOQN1,");
                    sql.Append("iCbSOQN2,");
                    sql.Append("dDrTime,");
                    sql.Append("vcOperator,");
                    sql.Append("dOperatorTime");
                    sql.Append(")");
                    sql.Append("VALUES");
                    sql.Append("('" + strYearMonth + "',");
                    sql.Append("'0',");
                    sql.Append("'0',");
                    sql.Append("'" + dt.Rows[i]["vcPart_id"] + "',");
                    sql.Append("'" + dt.Rows[i]["iCbSOQN"] + "',");
                    sql.Append("'" + dt.Rows[i]["iCbSOQN1"] + "',");
                    sql.Append("'" + dt.Rows[i]["iCbSOQN2"] + "',");
                    sql.Append("getDate(),");
                    sql.Append("'" + strUserId + "',");
                    sql.Append("getDate()");
                    sql.Append(")");

                    //if (i < dt.Rows.Count - 1)
                    //{
                    //    sql.Append(",");
                    //}
                }
                sql.Append("; \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());//先导入临时表，然后check
                #endregion

                #region 验证1：是否为TFTM品番（包装工厂）
                sql.Length = 0;//清空
                sql.Append("   select a.vcPart_id from    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
                sql.Append("   )a    \r\n ");
                sql.Append("   left join    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSPMaster where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'     \r\n ");
                sql.Append("   )b on a.vcPart_id=b.vcPartId    \r\n ");
                sql.Append("   where b.vcPartId is  null    \r\n ");
                DataTable dt1=excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for(int i = 0; i < dt1.Rows.Count; i++) 
                {
                    string strPart_id=dt1.Rows[i]["vcPart_id"].ToString();
                    errMessageDict.Add(strPart_id,"此品番不存在");
                }
                #endregion

                #region 验证2：N 月品番有效性(N月得有数量),有数量才校验  
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id,b.vcPartId as vcPartId_1,c.vcPartId as vcPartId_2,d.vcPartId as vcPartId_3 from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN<>0    \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    left join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'    \r\n ");
                sql.Append("       and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)      \r\n ");
                sql.Append("       and dFromTime<>dToTime      \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPartId      \r\n ");
                sql.Append("    left join          \r\n");
                sql.Append("    (          \r\n");
                sql.Append("    	select * from TSPMaster where vcPackingPlant='"+ strUnit + "' and vcReceiver='APC06'           \r\n");
                sql.Append("       and '"+ strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)             \r\n");
                sql.Append("       and dFromTime<>dToTime      \r\n ");
                sql.Append("    )c on a.vcPart_id=c.vcPartId          \r\n");
                sql.Append("    left join          \r\n");
                sql.Append("    (          \r\n");
                sql.Append("    	select * from TSPMaster where vcPackingPlant='"+ strUnit + "' and vcReceiver='APC06'           \r\n");
                sql.Append("       and '"+ strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)           \r\n");
                sql.Append("       and dFromTime<>dToTime      \r\n ");
                sql.Append("    )d on a.vcPart_id=d.vcPartId          \r\n");
                sql.Append("    where b.vcPartId is null or c.vcPartId is null or d.vcPartId is null           \r\n");
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                string month_temp = "";
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    string strPart_id = dt2.Rows[i]["vcPart_id"].ToString();
                    string strPart1 = dt2.Rows[i]["vcPartId_1"].ToString();
                    string strPart2 = dt2.Rows[i]["vcPartId_2"].ToString();
                    string strPart3 = dt2.Rows[i]["vcPartId_3"].ToString();
                    if (strPart1 == "")
                        month_temp += strYearMonth+",";
                    if (strPart2 == "")
                        month_temp += strYearMonth_2 + ",";
                    if (strPart3 == "")
                        month_temp += strYearMonth_3 + ",";

                    errMessageDict.Add(strPart_id, "不满足"+ month_temp.Substring(0,month_temp.Length-1) + "月有效性");
                }
                #endregion

                #region not use 验证3：N+1 月品番有效性(N+1月得有数量)，有数量才校验
                //sql.Length = 0;//清空
                //sql.Append("    select a.vcPart_id from    \r\n ");
                //sql.Append("    (    \r\n ");
                //sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN1<>0   \r\n ");
                //sql.Append("    )a    \r\n ");
                //sql.Append("    left join      \r\n ");
                //sql.Append("    (      \r\n ");
                //sql.Append("       select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)  \r\n ");
                //sql.Append("    )b on a.vcPart_id=b.vcPartId      \r\n ");
                //sql.Append("    where b.vcPartId is  null      \r\n ");
                //DataTable dt3 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                //for (int i = 0; i < dt3.Rows.Count; i++)
                //{
                //    string strPart_id = dt3.Rows[i]["vcPart_id"].ToString();
                //    errMessageDict.Add(strPart_id , "在品番基础信息存在，但不满足" + strYearMonth_2 + "月有效性条件");
                //}
                #endregion

                #region not use 验证4：N+2 月品番有效性(N+2月得有数量)，有数量才校验 
                //sql.Length = 0;//清空
                //sql.Append("    select a.vcPart_id from    \r\n ");
                //sql.Append("    (    \r\n ");
                //sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN2<>0    \r\n ");
                //sql.Append("    )a    \r\n ");
                //sql.Append("    left join      \r\n ");
                //sql.Append("    (      \r\n ");
                //sql.Append("       select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and '" + strYearMonth_3 + "'  between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)       \r\n ");
                //sql.Append("    )b on a.vcPart_id=b.vcPartId      \r\n ");
                //sql.Append("    where b.vcPartId is  null      \r\n ");
                //DataTable dt4 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                //for (int i = 0; i < dt4.Rows.Count; i++)
                //{
                //    string strPart_id = dt4.Rows[i]["vcPart_id"].ToString();
                //    errMessageDict.Add(strPart_id , "在品番基础信息存在，但不满足" + strYearMonth_3 + "月有效性条件");
                //}
                #endregion

                #region 验证5：是否有价格，且在有效期内(只判断N月)，数量为0不校验；如果是强制订货，则没有价格也可以
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from     \r\n ");
                sql.Append("    (     \r\n ");
                sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN<>0    \r\n ");
                sql.Append("    )a     \r\n ");
                sql.Append("    left join     \r\n ");
                sql.Append("    (     \r\n ");
                sql.Append("       select vcPart_id from TPrice where  convert(varchar(6),dUseBegin,112)<='" + strYearMonth + "' and convert(varchar(6),dUseEnd,112)>='" + strYearMonth + "'     \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPart_id     \r\n ");
                sql.Append("    left join       \n");
                sql.Append("    (      \n");
                sql.Append("    	select vcPartId,vcMandOrder      \n");
                sql.Append("    	from TSPMaster           \n");
                sql.Append("    	where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'           \n");
                sql.Append("    	and '"+ strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)       \n");
                sql.Append("        and dFromTime<>dToTime      \r\n ");
                sql.Append("    )c on a.vcPart_id=c.vcPartId         \n");
                sql.Append("    where b.vcPart_id is  null  and isnull(c.vcMandOrder,'')<>'1'      \r\n ");// --  vcMandOrder='1' 是强制订货 
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt5.Rows.Count; i++)
                {
                    string strPart_id = dt5.Rows[i]["vcPart_id"].ToString();
                    errMessageDict.Add(strPart_id , strYearMonth + "月没有价格");
                }
                #endregion

                #region 验证6：手配中是否有受入、收容数、发注工厂（N、N+1、N+2都判断），数量为0不校验
                #region N月的
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,fzgc.发注工厂,t5.iPackingQty,t6.vcSufferIn,    \n");
                sql.Append("fzgc_2.发注工厂 as 发注工厂_2,t5_2.iPackingQty as iPackingQty_2,t6_2.vcSufferIn as vcSufferIn_2,    \n");
                sql.Append("fzgc_3.发注工厂 as 发注工厂_3,t5_3.iPackingQty as iPackingQty_3,t6_3.vcSufferIn as vcSufferIn_3    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN<>0    \n");
                sql.Append(")t1        \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'     \n");
                sql.Append("	and '"+ strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                //sql.Append("left join (    \n");//发注工厂
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                //sql.Append("	from TSPMaster_OrderPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区 N
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区 N+1
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4_2 on t2.vcPartId=t4_2.vcPartId and t2.vcPackingPlant=t4_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4_2.vcReceiver and t2.vcSupplierId=t4_2.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区 N+2
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4_3 on t2.vcPartId=t4_3.vcPartId and t2.vcPackingPlant=t4_3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4_3.vcReceiver and t2.vcSupplierId=t4_3.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂 N
                sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                sql.Append("	and '"+strYearMonth+"' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                sql.Append("left join (    \n");//发注工厂 N+1
                sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                sql.Append(")fzgc_2 on t2.vcSupplierId=fzgc_2.供应商编号 and t4_2.vcSupplierPlant=fzgc_2.工区     \n");
                sql.Append("left join (    \n");//发注工厂 N+2
                sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                sql.Append(")fzgc_3 on t2.vcSupplierId=fzgc_3.供应商编号 and t4_3.vcSupplierPlant=fzgc_3.工区     \n");
                sql.Append("left join(    \n");//收容数 N
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                //sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                sql.Append("left join(    \n");//收容数 N+1
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5_2 on t2.vcPartId=t5_2.vcPartId and t2.vcPackingPlant=t5_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5_2.vcReceiver and t2.vcSupplierId=t5_2.vcSupplierId     \n");
                sql.Append("left join(    \n");//收容数 N+2
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5_3 on t2.vcPartId=t5_3.vcPartId and t2.vcPackingPlant=t5_3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5_3.vcReceiver and t2.vcSupplierId=t5_3.vcSupplierId     \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入 N
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入 N+1
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6_2 on t2.vcPartId=t6_2.vcPartId and t2.vcPackingPlant=t6_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6_2.vcReceiver and t2.vcSupplierId=t6_2.vcSupplierId    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入 N+2
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6_3 on t2.vcPartId=t6_3.vcPartId and t2.vcPackingPlant=t6_3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6_3.vcReceiver and t2.vcSupplierId=t6_3.vcSupplierId    \n");
                sql.Append("where (fzgc.供应商编号 is null or t5.vcPartId is null or t6.vcPartId is null) or       \r\n ");
                sql.Append(" (fzgc_2.供应商编号 is null or t5_2.vcPartId is null or t6_2.vcPartId is null) or       \r\n ");
                sql.Append(" (fzgc_3.供应商编号 is null or t5_3.vcPartId is null or t6_3.vcPartId is null)       \r\n ");
                DataTable dt6_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                month_temp = "";
                for (int i = 0; i < dt6_1.Rows.Count; i++)
                {
                    string strPart_id = dt6_1.Rows[i]["vcPart_id"].ToString();
                    string vcOrderPlant = dt6_1.Rows[i]["发注工厂"].ToString();//发注工厂
                    string iPackingQty = dt6_1.Rows[i]["iPackingQty"].ToString();//收容数
                    string vcSufferIn = dt6_1.Rows[i]["vcSufferIn"].ToString();//受入

                    string vcOrderPlant_2 = dt6_1.Rows[i]["发注工厂_2"].ToString();//发注工厂
                    string iPackingQty_2 = dt6_1.Rows[i]["iPackingQty_2"].ToString();//收容数
                    string vcSufferIn_2 = dt6_1.Rows[i]["vcSufferIn_2"].ToString();//受入

                    string vcOrderPlant_3 = dt6_1.Rows[i]["发注工厂_3"].ToString();//发注工厂
                    string iPackingQty_3 = dt6_1.Rows[i]["iPackingQty_3"].ToString();//收容数
                    string vcSufferIn_3 = dt6_1.Rows[i]["vcSufferIn_3"].ToString();//受入

                    if (vcSufferIn == "" || iPackingQty=="" || vcOrderPlant=="")
                        month_temp += strYearMonth + ",";
                    if (vcSufferIn_2 == "" || iPackingQty_2 == "" || vcOrderPlant_2 == "")
                        month_temp += strYearMonth_2 + ",";
                    if (vcSufferIn_3 == "" || iPackingQty_3 == "" || vcOrderPlant_3 == "")
                        month_temp += strYearMonth_3 + ",";

                    errMessageDict.Add(strPart_id,month_temp.Substring(0,month_temp.Length-1) + "月无手配信息");//无手配信息
                }
                #endregion
                #region not use N+1月的
                //sql.Length = 0;//清空
                //sql.Append("select t1.vcPart_id,fzgc.发注工厂,t5.iPackingQty,t6.vcSufferIn    \n");
                //sql.Append("from (    \n");
                //sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN1<>0    \n");
                //sql.Append(")t1        \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                //sql.Append("	from TSPMaster     \n");
                //sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                //sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                ////sql.Append("left join (    \n");//发注工厂
                ////sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                ////sql.Append("	from TSPMaster_OrderPlant     \n");
                ////sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                ////sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                ////sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                //sql.Append("left join (    \n");//供应商工区
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                //sql.Append("	from TSPMaster_SupplierPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                //sql.Append("left join (    \n");//发注工厂
                //sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                //sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                //sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                //sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                //sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                //sql.Append("left join(    \n");//收容数
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                //sql.Append("	from TSPMaster_Box     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                ////sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                //sql.Append("left join     \n");
                //sql.Append("(    \n");//受入
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                //sql.Append("where fzgc.供应商编号 is null or t5.vcPartId is null or t6.vcPartId is null       \r\n ");
                //DataTable dt6_2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                //for (int i = 0; i < dt6_2.Rows.Count; i++)
                //{
                //    string strPart_id = dt6_2.Rows[i]["vcPart_id"].ToString();
                //    string vcOrderPlant = dt6_2.Rows[i]["发注工厂"].ToString();//发注工厂
                //    string iPackingQty = dt6_2.Rows[i]["iPackingQty"].ToString();//收容数
                //    string vcSufferIn = dt6_2.Rows[i]["vcSufferIn"].ToString();//受入
                //    if (vcSufferIn == "")
                //        errMessageDict.Add(strPart_id,"在" + strYearMonth_2 + "月没有维护受入");
                //    if (iPackingQty == "")
                //        errMessageDict.Add(strPart_id ,"在" + strYearMonth_2 + "月没有维护收容数");
                //    if (vcOrderPlant == "")
                //        errMessageDict.Add(strPart_id , "在" + strYearMonth_2 + "月没有维护发注工厂");
                //}
                #endregion
                #region not use N+2月的
                //sql.Length = 0;//清空
                //sql.Append("select t1.vcPart_id,fzgc.发注工厂,t5.iPackingQty,t6.vcSufferIn    \n");
                //sql.Append("from (    \n");
                //sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iCbSOQN2<>0    \n");
                //sql.Append(")t1        \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                //sql.Append("	from TSPMaster     \n");
                //sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                //sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                ////sql.Append("left join (    \n");//发注工厂
                ////sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                ////sql.Append("	from TSPMaster_OrderPlant     \n");
                ////sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                ////sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                ////sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                //sql.Append("left join (    \n");//供应商工区
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                //sql.Append("	from TSPMaster_SupplierPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                //sql.Append("left join (    \n");//发注工厂
                //sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                //sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                //sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                //sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                //sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                //sql.Append("left join(    \n");//收容数
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                //sql.Append("	from TSPMaster_Box     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                ////sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                //sql.Append("left join     \n");
                //sql.Append("(    \n");//受入
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                //sql.Append("where fzgc.供应商编号 is null or t5.vcPartId is null or t6.vcPartId is null       \r\n ");
                //DataTable dt6_3 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                //for (int i = 0; i < dt6_3.Rows.Count; i++)
                //{
                //    string strPart_id = dt6_3.Rows[i]["vcPart_id"].ToString();
                //    string vcOrderPlant = dt6_3.Rows[i]["发注工厂"].ToString();//发注工厂
                //    string iPackingQty = dt6_3.Rows[i]["iPackingQty"].ToString();//收容数
                //    string vcSufferIn = dt6_3.Rows[i]["vcSufferIn"].ToString();//受入
                //    if (vcSufferIn == "")
                //        errMessageDict.Add(strPart_id , "在" + strYearMonth_3 + "月没有维护受入");
                //    if (iPackingQty == "")
                //        errMessageDict.Add(strPart_id , "在" + strYearMonth_3 + "月没有维护收容数");
                //    if (vcOrderPlant == "")
                //        errMessageDict.Add(strPart_id , "在" + strYearMonth_3 + "月没有维护发注工厂");
                //}
                #endregion
                #endregion

                #region not use 不验证  验证7：受入、收容数、发注工厂、供应商工区：3个月必须一样
                //sql.Length = 0;//清空
                //sql.Append("select t1.vcPart_id,fzgc.发注工厂,fzgc_1.发注工厂 as 发注工厂_1,fzgc_2.发注工厂 as 发注工厂_2,    \n");
                //sql.Append("t5.iPackingQty,t5_1.iPackingQty as iPackingQty_1,t5_2.iPackingQty as iPackingQty_2,    \n");
                //sql.Append("t6.vcSufferIn, t6_1.vcSufferIn as vcSufferIn_1,t6_2.vcSufferIn as vcSufferIn_2   \n");
                //sql.Append("from (    \n");
                //sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and (iCbSOQN<>0 or iCbSOQN1<>0 or iCbSOQN2<>0)    \n");
                //sql.Append(")t1        \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                //sql.Append("	from TSPMaster     \n");
                //sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                //sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                ////sql.Append("left join (    \n");//发注工厂 N月
                ////sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                ////sql.Append("	from TSPMaster_OrderPlant     \n");
                ////sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                ////sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                ////sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                ////sql.Append("left join (    \n");//发注工厂 N+1月
                ////sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                ////sql.Append("	from TSPMaster_OrderPlant     \n");
                ////sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                ////sql.Append(")t3_1 on t2.vcPartId=t3_1.vcPartId and t2.vcPackingPlant=t3_1.vcPackingPlant     \n");
                ////sql.Append("and t2.vcReceiver=t3_1.vcReceiver and t2.vcSupplierId=t3_1.vcSupplierId    \n");
                ////sql.Append("left join (    \n");//发注工厂 N+2月
                ////sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                ////sql.Append("	from TSPMaster_OrderPlant     \n");
                ////sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                ////sql.Append(")t3_2 on t2.vcPartId=t3_2.vcPartId and t2.vcPackingPlant=t3_2.vcPackingPlant     \n");
                ////sql.Append("and t2.vcReceiver=t3_2.vcReceiver and t2.vcSupplierId=t3_2.vcSupplierId    \n");
                //sql.Append("left join (    \n");//供应商工区 N月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                //sql.Append("	from TSPMaster_SupplierPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                //sql.Append("left join (    \n");//供应商工区 N+1月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                //sql.Append("	from TSPMaster_SupplierPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t4_1 on t2.vcPartId=t4_1.vcPartId and t2.vcPackingPlant=t4_1.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t4_1.vcReceiver and t2.vcSupplierId=t4_1.vcSupplierId    \n");
                //sql.Append("left join (    \n");//供应商工区 N+2月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                //sql.Append("	from TSPMaster_SupplierPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t4_2 on t2.vcPartId=t4_2.vcPartId and t2.vcPackingPlant=t4_2.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t4_2.vcReceiver and t2.vcSupplierId=t4_2.vcSupplierId    \n");
                //sql.Append("left join (    \n");//发注工厂 N月
                //sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                //sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                //sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                //sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                //sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                //sql.Append("left join (    \n");//发注工厂 N+1月
                //sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                //sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                //sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                //sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                //sql.Append(")fzgc_1 on t2.vcSupplierId=fzgc_1.供应商编号 and t4.vcSupplierPlant=fzgc_1.工区     \n");
                //sql.Append("left join (    \n");//发注工厂 N+2月
                //sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                //sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                //sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                //sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                //sql.Append(")fzgc_2 on t2.vcSupplierId=fzgc_2.供应商编号 and t4.vcSupplierPlant=fzgc_2.工区     \n");
                //sql.Append("left join(    \n");//收容数 N月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                //sql.Append("	from TSPMaster_Box     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                ////sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                //sql.Append("left join(    \n");//收容数 N+1月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                //sql.Append("	from TSPMaster_Box     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t5_1 on t2.vcPartId=t5_1.vcPartId and t2.vcPackingPlant=t5_1.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t5_1.vcReceiver and t2.vcSupplierId=t5_1.vcSupplierId     \n");
                ////sql.Append("and t4_1.vcSupplierPlant=t5_1.vcSupplierPlant    \n");
                //sql.Append("left join(    \n");//收容数 N+2月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                //sql.Append("	from TSPMaster_Box     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t5_2 on t2.vcPartId=t5_2.vcPartId and t2.vcPackingPlant=t5_2.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t5_2.vcReceiver and t2.vcSupplierId=t5_2.vcSupplierId     \n");
                ////sql.Append("and t4_2.vcSupplierPlant=t5_2.vcSupplierPlant    \n");
                //sql.Append("left join     \n");
                //sql.Append("(    \n");//受入 N月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                //sql.Append("left join     \n");
                //sql.Append("(    \n");//受入 N+1月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t6_1 on t2.vcPartId=t6_1.vcPartId and t2.vcPackingPlant=t6_1.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t6_1.vcReceiver and t2.vcSupplierId=t6_1.vcSupplierId    \n");
                //sql.Append("left join     \n");
                //sql.Append("(    \n");//受入 N+2月
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t6_2 on t2.vcPartId=t6_2.vcPartId and t2.vcPackingPlant=t6_2.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t6_2.vcReceiver and t2.vcSupplierId=t6_2.vcSupplierId    \n");
                //sql.Append("where (fzgc.发注工厂<>fzgc_1.发注工厂 or fzgc.发注工厂<>fzgc_2.发注工厂) or     \r\n ");//发注工厂
                //sql.Append(" (t5.iPackingQty<>t5_1.iPackingQty or t5.iPackingQty<>t5_2.iPackingQty) or     \r\n ");//收容数
                //sql.Append(" (t6.vcSufferIn<>t6_1.vcSufferIn or t6.vcSufferIn<>t6_2.vcSufferIn) or     \r\n ");//受入
                //sql.Append(" (t4.vcSupplierPlant<>t4_1.vcSupplierPlant or t4.vcSupplierPlant<>t4_2.vcSupplierPlant)      \r\n ");//供应商工区
                //DataTable dt7 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                //for (int i = 0; i < dt7.Rows.Count; i++)
                //{
                //    string strPart_id = dt7.Rows[i]["vcPart_id"].ToString();
                //    string vcOrderPlant = dt7.Rows[i]["发注工厂"].ToString();//发注工厂 N月
                //    string vcOrderPlant_1 = dt7.Rows[i]["发注工厂_1"].ToString();//发注工厂 N+1月
                //    string vcOrderPlant_2 = dt7.Rows[i]["发注工厂_2"].ToString();//发注工厂 N+2月
                //    string iPackingQty = dt7.Rows[i]["iPackingQty"].ToString();//收容数 N月
                //    string iPackingQty_1 = dt7.Rows[i]["iPackingQty_1"].ToString();//收容数 N+1月
                //    string iPackingQty_2 = dt7.Rows[i]["iPackingQty_2"].ToString();//收容数 N+2月
                //    string vcSufferIn = dt7.Rows[i]["vcSufferIn"].ToString();//受入 N月
                //    string vcSufferIn_1 = dt7.Rows[i]["vcSufferIn_1"].ToString();//受入 N+1月
                //    string vcSufferIn_2 = dt7.Rows[i]["vcSufferIn_2"].ToString();//受入 N+2月
                //    string vcSupplierPlant= dt7.Rows[i]["vcSupplierPlant"].ToString();//供应商工区 N月
                //    string vcSupplierPlant_1 = dt7.Rows[i]["vcSupplierPlant_1"].ToString();//供应商工区 N+1月
                //    string vcSupplierPlant_2 = dt7.Rows[i]["vcSupplierPlant_2"].ToString();//供应商工区 N+2月
                //    if (vcOrderPlant !=vcOrderPlant_1 || vcOrderPlant != vcOrderPlant_2)
                //        errMessageDict.Add(strPart_id , "在3个月维护的发注工厂不一致");//不需要这个校验，删掉
                //    if (iPackingQty != iPackingQty_1 || iPackingQty!= iPackingQty_2)
                //        errMessageDict.Add(strPart_id , "在3个月维护的收容数不一致");
                //    if (vcSufferIn != vcSufferIn_1 || vcSufferIn != vcSufferIn_2)
                //        errMessageDict.Add(strPart_id , "在3个月维护的受入不一致");
                //    if (vcSupplierPlant != vcSupplierPlant_1 || vcSupplierPlant != vcSupplierPlant_2)
                //        errMessageDict.Add(strPart_id , "在3个月维护的供应商工区不一致");
                //}
                #endregion

                #region 验证8：品番3个月数量不能全为0(N月可以为)
                sql.Length = 0;//清空
                sql.Append("select vcPart_id,iCbSOQN,iCbSOQN1,iCbSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth+"'    \n");
                sql.Append("and ISNULL(iCbSOQN,0)=0 and ISNULL(iCbSOQN1,0)=0 and ISNULL(iCbSOQN2,0)=0     \n");
                DataTable dt8 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt8.Rows.Count; i++)
                {
                    string strPart_id = dt8.Rows[i]["vcPart_id"].ToString();
                    errMessageDict.Add(strPart_id , "3个月数量全为0");
                }
                #endregion

                #region 验证9：收容数整倍数
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t1.iCbSOQN%t5.iPackingQty as iCbSOQN,t1.iCbSOQN1%t5.iPackingQty as iCbSOQN1,    \n");
                sql.Append("t1.iCbSOQN2%t5.iPackingQty as iCbSOQN2    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth+"'         \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut        \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'         \n");
                sql.Append("	and '"+strYearMonth+"' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)        \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId        \n");
                //sql.Append("left join (    --//供应商工区    \n");
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant         \n");
                //sql.Append("	from TSPMaster_SupplierPlant         \n");
                //sql.Append("	where vcOperatorType='1' and '"+strYearMonth+"' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)        \n");
                //sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant         \n");
                //sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId        \n");
                sql.Append("left join(    --//收容数    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty         \n");
                sql.Append("	from TSPMaster_Box         \n");
                sql.Append("	where vcOperatorType='1' and '"+ strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)        \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant         \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         \n");
                //sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant        \n");
                sql.Append("where t1.iCbSOQN%t5.iPackingQty<>0 or t1.iCbSOQN1%t5.iPackingQty<>0 or t1.iCbSOQN2%t5.iPackingQty<>0       \n");
                DataTable dt9 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt9.Rows.Count; i++)
                {
                    string strPart_id = dt9.Rows[i]["vcPart_id"].ToString();
                    errMessageDict.Add(strPart_id , "订货数量不是收容数的整数倍");
                }
                #endregion

                #region 验证10：if一括生产 校验： 对象月 > 实施年月时间 不能订货(数量得是0) 3个月都校验
                #region N月
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t2.dDebugTime as dDebugTime_1,t3.dDebugTime as dDebugTime_2,t4.dDebugTime as dDebugTime_3    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and  vcYearMonth='" + strYearMonth+ "'  and iCbSOQN>0       \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'         \n");
                sql.Append("	and '"+strYearMonth+"' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append("    and '" + strYearMonth + "'>=convert(varchar(6),dDebugTime,112)    \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId     \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append("    and '" + strYearMonth_2 + "'>=convert(varchar(6),dDebugTime,112)    \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t3 on t1.vcPart_id=t3.vcPartId     \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append("    and '" + strYearMonth_3 + "'>=convert(varchar(6),dDebugTime,112)    \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t4 on t1.vcPart_id=t4.vcPartId     \n");
                sql.Append("where t2.vcPartId is not null or t3.vcPartId is not null or t4.vcPartId is not null    \n");
                DataTable dt10 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                month_temp = "";
                for (int i = 0; i < dt10.Rows.Count; i++)
                {
                    string strPart_id = dt10.Rows[i]["vcPart_id"].ToString();
                    string dDebugTime1 = dt2.Rows[i]["dDebugTime_1"].ToString();
                    string dDebugTime2 = dt2.Rows[i]["dDebugTime_2"].ToString();
                    string dDebugTime3 = dt2.Rows[i]["dDebugTime_3"].ToString();
                    if (dDebugTime1 == "")
                        month_temp += strYearMonth + ",";
                    if (dDebugTime2 == "")
                        month_temp += strYearMonth_2 + ",";
                    if (dDebugTime3 == "")
                        month_temp += strYearMonth_3 + ",";

                    errMessageDict.Add(strPart_id , "在" + month_temp.Substring(0,month_temp.Length-1) + "月以后不能订货");
                }
                #endregion
                #region not use N+1月
                //sql.Length = 0;//清空
                //sql.Append("select t1.vcPart_id,t2.dDebugTime    \n");
                //sql.Append("from (        \n");
                //sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'         \n");
                //sql.Append(")t1            \n");
                //sql.Append("left join (        \n");
                //sql.Append("	select vcPartId,dDebugTime       \n");
                //sql.Append("	from TSPMaster         \n");
                //sql.Append("	where vcPackingPlant='"+ strUnit + "' and vcReceiver='APC06'         \n");
                //sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                //sql.Append(")t2 on t1.vcPart_id=t2.vcPartId     \n");
                //sql.Append("where '" + strYearMonth_2 + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iCbSOQN1>0    \n");
                //DataTable dt10_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                //for (int i = 0; i < dt10_1.Rows.Count; i++)
                //{
                //    string strPart_id = dt10_1.Rows[i]["vcPart_id"].ToString();
                //    errMessageDict.Add(strPart_id ,"在" + strYearMonth_2 + "月不能订货。");
                //}
                #endregion
                #region not use N+2月
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t2.dDebugTime    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'         \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='"+ strUnit + "' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId     \n");
                sql.Append("where '" + strYearMonth_3 + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iCbSOQN2>0    \n");
                DataTable dt10_2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt10_2.Rows.Count; i++)
                {
                    string strPart_id = dt10_2.Rows[i]["vcPart_id"].ToString();
                    errMessageDict.Add(strPart_id , "在" + strYearMonth_3 + "月不能订货。");
                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,string strYearMonth,string strYearMonth_2,string strYearMonth_3,string strUnit)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");

                DateTime now = DateTime.Now;

                StringBuilder sql = new StringBuilder();

                sql.Append(" delete TSoq where vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                //1、先插入
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("  INSERT INTO TSoq( ");
                    sql.Append("vcYearMonth,");
                    sql.Append("vcDyState,");
                    sql.Append("vcHyState,");
                    sql.Append("vcPart_id,");
                    sql.Append("iCbSOQN,");
                    sql.Append("iCbSOQN1,");
                    sql.Append("iCbSOQN2,");
                    sql.Append("dDrTime,");
                    sql.Append("vcOperator,");
                    sql.Append("dOperatorTime,");
                    sql.Append("vcLastTimeFlag");
                    sql.Append(")");
                    sql.Append("VALUES");
                    sql.Append("('"+ strYearMonth + "',");
                    sql.Append("'0',");
                    sql.Append("'0',");
                    sql.Append("'" + dt.Rows[i]["vcPart_id"] + "',");
                    sql.Append("'" + dt.Rows[i]["iCbSOQN"] + "',");
                    sql.Append("'" + dt.Rows[i]["iCbSOQN1"] + "',");
                    sql.Append("'" + dt.Rows[i]["iCbSOQN2"] + "',");
                    sql.Append("getDate(),");
                    sql.Append("'"+ strUserId + "',");
                    sql.Append("getDate(),");
                    sql.Append("'" + strLastTimeFlag + "'");
                    sql.Append(")");

                    //if (i < dt.Rows.Count - 1) {
                    //    sql.Append(",");
                    //}
                }
                sql.Append("; \r\n ");

                //sql.Append(" delete TSoqInput where vcYearMonth='" + strYearMonth + "' ;  \r\n ");
                //sql.Append(" insert into TSoqInput(vcYearMonth,iState,vcOperator,dOperatorTime)values('" + strYearMonth + "',2,'"+ strUserId + "',getdate());  \r\n ");
                
                //走到保存，则异常信息肯定没有了，删除TSoqInputErrDetail
                sql.Append(" delete TSoqInputErrDetail where vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                //2、再更新关联数据
                sql.Append("update t1 set     \n");
                sql.Append("t1.vcCarFamilyCode=t2.vcCarfamilyCode,    \n");
                sql.Append("t1.vcCurrentPastcode=t2.vcHaoJiu,    \n");
                sql.Append("t1.vcMakingOrderType=t2.vcOrderingMethod,    \n");
                sql.Append("t1.vcFZGC=fzgc.发注工厂,    \n");
                sql.Append("t1.vcInOutFlag=t2.vcInOut,    \n");
                sql.Append("t1.vcSupplier_id=t2.vcSupplierId,    \n");
                sql.Append("t1.vcSupplierPlant=t4.vcSupplierPlant,    \n");
                sql.Append("t1.iQuantityPercontainer=t5.iPackingQty   \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq where vcYearMonth='"+ strYearMonth + "')t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,vcOrderingMethod    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'     \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                //sql.Append("	from TSPMaster_OrderPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n"); 
                //sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂
                sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                sql.Append("left join(    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n"); 
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                //sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");

                string strYear = strYearMonth.Substring(0, 4);
                string strMonth = strYearMonth.Substring(4, 2);
                DateTime dLastMonth = (DateTime.Parse(strYear + "-" + strMonth + "-01")).AddMonths(-1);
                string strLastYearMonth = dLastMonth.ToString("yyyyMM");
                //波动率计算
                //sql.AppendLine("  update TSoq set decCbBdl=100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN as decimal(18,2)))/cast(b.iHySOQN as decimal(18,2))  from TSoq a    \r\n ");
                sql.AppendLine("  update TSoq set decCbBdl=     \r\n ");
                sql.AppendLine("     case when a.iCbSOQN=0 or b.iHySOQN=0 then  \r\n ");
                sql.AppendLine("     	ABS(100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN as decimal(18,2))))  \r\n ");
                sql.AppendLine("     else \r\n ");
                sql.AppendLine("     	ABS(100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN as decimal(18,2))))/cast(b.iHySOQN as decimal(18,2)) \r\n ");
                sql.AppendLine("     end    \r\n ");
                sql.AppendLine("  from TSoq a    \r\n ");
                sql.AppendLine("  left join    \r\n ");
                sql.AppendLine("  (    \r\n ");
                sql.AppendLine("    select * from TSoq where vcYearMonth='"+ strLastYearMonth + "'    \r\n ");
                sql.AppendLine("  )b on a.vcPart_id=b.vcPart_id    \r\n ");
                sql.AppendLine("  where a.vcYearMonth='" + strYearMonth + "' and b.iHySOQN is not null    \r\n ");

                //在SOQprocess表中插入状态
                //sql.AppendLine("DELETE TSOQProcess WHERE vcYearMonth='"+ strYearMonth + "'; \r\n ");
                //sql.AppendLine("INSERT INTO TSOQProcess(INOUTFLAG,vcYearMonth,iStatus)  \r\n ");
                //sql.AppendLine("VALUES('0','"+ strYearMonth + "',0), \r\n ");
                //sql.AppendLine("('1','"+ strYearMonth + "',0); \r\n ");

                //记录日志
                sql.AppendLine("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                sql.AppendLine("  select vcYearMonth,vcPart_id,'FTMS导入成功','" + strUserId + "',getDate() from TSoq  where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "' ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向SOQ导入履历中新增数据
        public int importHistory(string strYearMonth, Dictionary<string,string> errMessageDict)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                //先删除本对象月再新增
                strSql.Append(" DELETE FROM TSoqInputErrDetail  WHERE vcYearMonth='" + strYearMonth + "'  \n");
                foreach(KeyValuePair<string,string> kvp in errMessageDict)
                {
                    string strPart_id = kvp.Key;
                    string strMsg = kvp.Value;
                    strSql.Append("insert into TSoqInputErrDetail (vcYearMonth,vcPart_id,vcMessage) values   \n");
                    strSql.Append("('"+strYearMonth+"','"+strPart_id+"','"+strMsg+"')    \n");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否待确认
        public DataTable IsDQR(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select * from TSoq ");
                strSql.AppendLine(" WHERE vcHyState<>'1' ");//1是待确认
                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(strYearMonth))
                {
                    strSql.AppendLine(" AND vcYearMonth='" + strYearMonth + "' ");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(strDyState))
                {
                    strSql.AppendLine(" AND vcDyState='" + strDyState + "' ");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(strHyState))
                {
                    strSql.AppendLine(" AND vcHyState='" + strHyState + "' ");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.AppendLine(" AND vcPart_id like '%" + strPart_id + "%' ");
                }
                strSql.Append("; \r\n ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否待确认
        public DataTable IsDQR(string strYearMonth, List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TSoq_temp_cr') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TSoq_temp_cr       \n");
                strSql.Append("      End      \n");
                strSql.Append("      select * into #TSoq_temp_cr from       \n");
                strSql.Append("      (      \n");
                strSql.Append("      	select vcPart_id,vcHyState from TSoq where 1=0      \n");
                strSql.Append("      ) a      ;\n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    strSql.Append("      insert into #TSoq_temp_cr       \n");
                    strSql.Append("       (         \n");
                    strSql.Append("       vcPart_id,vcHyState        \n");
                    strSql.Append("       ) values         \n");
                    strSql.Append("      (      \n");
                    strSql.Append("       '"+ listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                    strSql.Append("       '" + listInfoData[i]["vcHyState"].ToString() + "'     \n");
                    strSql.Append("      );      \n");
                    #endregion
                }
                strSql.AppendLine(" select * from #TSoq_temp_cr where vcHyState<>'1'  ");//1是待确认
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int ok(string strYearMonth, string strDyState, string strHyState, string strPart_id,string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iHySOQN=iTzhSOQN,");
                strSql.AppendLine("      iHySOQN1=iTzhSOQN1,");
                strSql.AppendLine("      iHySOQN2=iTzhSOQN2,");
                strSql.AppendLine("      vcHyState='2', ");
                strSql.AppendLine("      dHyTime=getdate() ");
                strSql.AppendLine("      ,vcOperator='" + strUserId + "' ");
                strSql.AppendLine("      ,dOperatorTime=getDate() ");
                strSql.AppendLine("      ,vcLastTimeFlag='" + strLastTimeFlag + "' ");
                strSql.AppendLine(" WHERE 1=1 ");
                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(strYearMonth))
                {
                    strSql.AppendLine(" AND vcYearMonth='"+ strYearMonth + "' ");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(strDyState))
                {
                    strSql.AppendLine(" AND vcDyState='" + strDyState + "' ");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(strHyState))
                {
                    strSql.AppendLine(" AND vcHyState='" + strHyState + "' ");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.AppendLine(" AND vcPart_id like '%"+ strPart_id + "%' ");
                }
                strSql.Append("; \r\n ");

                //记录日志
                strSql.AppendLine("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.AppendLine("  select vcYearMonth,vcPart_id,'FTMS承认','"+ strUserId + "',getDate() from TSoq  where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "' ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int ok(string strYearMonth, List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");

                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TSoq_temp_cr') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TSoq_temp_cr       \n");
                strSql.Append("      End      \n");
                strSql.Append("      select * into #TSoq_temp_cr from       \n");
                strSql.Append("      (      \n");
                strSql.Append("      	select vcPart_id from TSoq where 1=0      \n");
                strSql.Append("      ) a      ;\n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    strSql.Append("      insert into #TSoq_temp_cr       \n");
                    strSql.Append("       (         \n");
                    strSql.Append("       vcPart_id        \n");
                    strSql.Append("       ) values         \n");
                    strSql.Append("      (      \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "   \n");
                    strSql.Append("      );      \n");
                    #endregion
                }

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iHySOQN=iTzhSOQN,");
                strSql.AppendLine("      iHySOQN1=iTzhSOQN1,");
                strSql.AppendLine("      iHySOQN2=iTzhSOQN2,");
                strSql.AppendLine("      vcHyState='2', ");
                strSql.AppendLine("      dHyTime=getdate() ");
                strSql.AppendLine("      ,vcOperator='" + strUserId + "' ");
                strSql.AppendLine("      ,dOperatorTime=getDate() ");
                strSql.AppendLine("      ,vcLastTimeFlag='" + strLastTimeFlag + "' ");
                strSql.AppendLine(" from TSoq a  \n ");
                strSql.AppendLine(" inner join  \n ");
                strSql.AppendLine(" (  \n ");
                strSql.AppendLine("    select vcPart_id from #TSoq_temp_cr  \n ");
                strSql.AppendLine(" )b on a.vcPart_id=b.vcPart_id  \n ");
                strSql.Append(";  \n ");

                //记录日志
                strSql.AppendLine("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.AppendLine("  select '"+ strYearMonth + "' as vcYearMonth,vcPart_id,'FTMS承认','" + strUserId + "',getDate() from TSoq where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "'  ");
                strSql.Append(";  \n ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 退回
        public int ng(string strYearMonth, string strDyState, string strHyState, string strPart_id,string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      vcHyState='3' ");
                strSql.AppendLine("      ,vcOperator='" + strUserId + "' ");
                strSql.AppendLine("      ,dOperatorTime=getDate() ");
                strSql.AppendLine("      ,vcLastTimeFlag='" + strLastTimeFlag + "' ");
                strSql.AppendLine(" WHERE 1=1 ");
                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(strYearMonth))
                {
                    strSql.AppendLine(" AND vcYearMonth='" + strYearMonth + "' ");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(strDyState))
                {
                    strSql.AppendLine(" AND vcDyState='" + strDyState + "' ");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(strHyState))
                {
                    strSql.AppendLine(" AND vcHyState='" + strHyState + "' ");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.AppendLine(" AND vcPart_id like '%" + strPart_id + "%' ");
                }
                strSql.Append("; \r\n ");

                //本次修改的数据记录日志
                strSql.AppendLine("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.AppendLine("  select vcYearMonth,vcPart_id,'FTMS退回','" + strUserId + "',getDate() from TSoq  ");
                strSql.AppendLine("  where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "' ");

                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 退回。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int ng(string strYearMonth, List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TSoq_temp_back') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TSoq_temp_back       \n");
                strSql.Append("      End      \n");
                strSql.Append("      select * into #TSoq_temp_back from       \n");
                strSql.Append("      (      \n");
                strSql.Append("      	select vcPart_id from TSoq where 1=0      \n");
                strSql.Append("      ) a      ;\n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    strSql.Append("      insert into #TSoq_temp_back       \n");
                    strSql.Append("       (         \n");
                    strSql.Append("       vcPart_id        \n");
                    strSql.Append("       ) values         \n");
                    strSql.Append("      (      \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "   \n");
                    strSql.Append("      );      \n");
                    #endregion
                }

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      vcHyState='3'  ");
                strSql.AppendLine("      ,vcOperator='" + strUserId + "' ");
                strSql.AppendLine("      ,dOperatorTime=getDate() ");
                strSql.AppendLine("      ,vcLastTimeFlag='" + strLastTimeFlag + "' ");
                strSql.AppendLine(" from TSoq a  \n ");
                strSql.AppendLine(" inner join  \n ");
                strSql.AppendLine(" (  \n ");
                strSql.AppendLine("    select vcPart_id from #TSoq_temp_back  \n ");
                strSql.AppendLine(" )b on a.vcPart_id=b.vcPart_id  \n ");
                strSql.Append(";  \n ");

                //本次修改的数据记录日志
                strSql.AppendLine("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.AppendLine("  select '" + strYearMonth + "' as vcYearMonth,vcPart_id,'FTMS退回','" + strUserId + "',getDate() from TSoq where vcLastTimeFlag='"+ strLastTimeFlag + "' and vcOperator='" + strUserId + "'  ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回用户邮箱
        public DataTable getEmail(string strSendUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select vcEmail from SUser where vcUserID='"+strSendUserId+"'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回接收人邮箱
        public DataTable getReciveEmail()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select vcName as address,vcMeaning as displayName from TCode where vcCodeId='C050'");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否重复导入
        public int isRepeatImport(string strYearMonth)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(1) from TSoq where vcYearMonth='"+strYearMonth+"'    \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}