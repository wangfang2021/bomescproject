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
        public DataSet Search(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("SELECT iAutoId,vcYearMonth,vcDyState,vcHyState,vcPart_id,iCbSOQN,        \n");
                strSql.Append("case when abs(decCbBdl)>999 then '>999%' else cast(decCbBdl as varchar(5))+'%' end as decCbBdl,    \n");
                strSql.Append("cast(iCbSOQN1 as int) as iCbSOQN1,cast(iCbSOQN2 as int) as iCbSOQN2,cast(iTzhSOQN as int) as iTzhSOQN,    \n");
                strSql.Append("cast(iTzhSOQN1 as int) as iTzhSOQN1,cast(iTzhSOQN2 as int) as iTzhSOQN2,    \n");
                strSql.Append("cast(iHySOQN as int) as iHySOQN,cast(iHySOQN1 as int) as iHySOQN1,cast(iHySOQN2 as int) as iHySOQN2,dHyTime,      \n");
                strSql.Append("case when a.vcDyState in ('0','1') then '-' else b.vcName end as 'vcDyState_Name',b2.vcName as 'vcHyState_Name',    \n");
                strSql.Append(" case       \n");
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

                //合计行
                strSql.Append("SELECT '' as vcYearMonth,'' as vcDyState_Name,'' as vcHyState_Name,'' as vcPart_id,sum(iCbSOQN) as iCbSOQN,        \n");
                strSql.Append("'' as decCbBdl,sum(iCbSOQN1) as iCbSOQN1,sum(iCbSOQN2) as iCbSOQN2,sum(iTzhSOQN) as iTzhSOQN,sum(iTzhSOQN1) as iTzhSOQN1,    \n");
                strSql.Append("sum(iTzhSOQN2) as iTzhSOQN2,sum(iHySOQN) as iHySOQN,sum(iHySOQN1) as iHySOQN1,sum(iHySOQN2) as iHySOQN2,'' as dHyTime    \n");
                strSql.Append("  FROM TSoq a  \n");
                strSql.Append("  WHERE 1=1  \n");

                if (!string.IsNullOrEmpty(strYearMonth))
                {//对象年月
                    strSql.Append(" and vcYearMonth='" + strYearMonth + "'");
                }
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                {
                    if (strDyState == "4")
                        strSql.Append(" and vcDyState in ('0','1') ");
                    else
                        strSql.Append(" and vcDyState='" + strDyState + "'");
                }
                if (!string.IsNullOrEmpty(strHyState))//合意状态
                {
                    strSql.Append(" and vcHyState='" + strHyState + "'");
                }
                if (!string.IsNullOrEmpty(strPart_id))//品番
                {
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                }

                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
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

        public Dictionary<string, string> ErrorMsg(Dictionary<string, string> errMessageDict,DataTable dtc, DataTable dt,string strMesInfo,bool isformat)
        {
            for (int i = 0; i < dtc.Rows.Count; i++)
            {
                string strpf = dtc.Rows[i]["vcPart_id"].ToString();
                DataRow[] dataRows = dt.Select("vcPart_id='"+ strpf + "'");
                if(dataRows.Length!=0)
                {
                    string strYm = "";
                    for (int j = 0; j < dataRows.Length; j++)
                    {
                        strYm = strYm + dataRows[j]["vcYM"].ToString();
                        if (j != dataRows.Length - 1)
                            strYm = strYm+ ",";
                    }
                    if(isformat)
                        errMessageDict.Add(strpf, string.Format(strMesInfo, strYm));
                    else
                        errMessageDict.Add(strpf, strMesInfo);
                }
            }
            return errMessageDict;
        }

        #region 导入后校验
        public void importCheck(DataTable dt, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3, 
            ref Dictionary<string,string> errMessageDict, string strUnit)
        {
            try
            {
                DataTable temp = excute.ExcuteSqlWithSelectToDT("select top 1 vcValue from TCode where vcCodeId='C068'");
                string vcReceiver = temp.Rows.Count == 1 ? temp.Rows[0][0].ToString() : "APC06";

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
                    string partid = "";
                    if (dt.Rows[i]["vcPart_id"].ToString().Length == 10)
                        partid = dt.Rows[i]["vcPart_id"].ToString() + "00";
                    else
                        partid = dt.Rows[i]["vcPart_id"].ToString();
                    sql.Append("'" + partid + "',");
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

                #region 品番
                sql.Length = 0;//清空
                sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
                DataTable dtc = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                #endregion

                #region 验证1：是否为TFTM品番（包装工厂）
                sql.Length = 0;//清空
                sql.Append("   select a.vcPart_id from    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
                sql.Append("   )a    \r\n ");
                sql.Append("   left join    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSPMaster where vcPackingPlant='"+strUnit+"' and vcReceiver='"+ vcReceiver + "'     \r\n ");
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
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '"+ strYearMonth + "' as vcYM ,1 as item union all select '"+ strYearMonth_2 + "' as vcYM,2 as item union all select '"+ strYearMonth_3 + "' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='"+strUserId+"')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(");
                sql.AppendLine("	select vcPartId,vcSupplierId,dFromTime,dToTime from TSPMaster where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "' and dFromTime<>dToTime  ");
                sql.AppendLine(")t2");
                sql.AppendLine("on t1.vcPart_id=t2.vcPartId ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)");
                sql.AppendLine("where cast(t1.iCbSOQN as int) <>0 and t2.vcPartId is null");
                sql.AppendLine("order by t1.vcPart_id,t1.vcYM");
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                errMessageDict = ErrorMsg(errMessageDict, dtc, dt2, "不满足{0}月有效性",true);
                #endregion

                #region 验证5：是否有价格，且在有效期内(只判断N月)，数量为0不校验；如果是强制订货，则没有价格也可以
                sql.Length = 0;
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '"+strYearMonth+"' as vcYM ,1 as item union all select '"+strYearMonth_2+"' as vcYM,2 as item union all select '"+strYearMonth_3+"' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='"+strUserId+"')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(--手配主表");
                sql.AppendLine("	select vcPartId,vcSupplierId,vcMandOrder,dFromTime,dToTime ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "' and dFromTime<>dToTime ");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
                sql.AppendLine("left join     ");
                sql.AppendLine("(--价格     ");
                sql.AppendLine("   select vcPart_id,dPricebegin,dPriceEnd from TPrice     ");
                sql.AppendLine(")t3 on t1.vcPart_id=t3.vcPart_id and t1.vcYM between convert(varchar(6),t3.dPricebegin,112) and convert(varchar(6),t3.dPriceEnd,112)   ");
                sql.AppendLine("where item=1 and cast(t1.iCbSOQN as int) <>0 and t3.vcPart_id is null and isnull(t2.vcMandOrder,'')<>'1' --vcMandOrder='1' 是强制订货");
                sql.AppendLine("order by t1.vcPart_id,t1.vcYM");
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                errMessageDict = ErrorMsg(errMessageDict, dtc, dt5, "{0}月没有价格", true);
                #endregion

                #region 验证6：手配中是否有受入、收容数、发注工厂（N、N+1、N+2都判断），数量为0不校验
                sql.Length = 0;//清空
                sql.AppendLine("select * from     ");
                sql.AppendLine("(    ");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id    ");
                sql.AppendLine("	,case when item=1 then iCbSOQN    ");
                sql.AppendLine("	  when item=2 then iCbSOQN1    ");
                sql.AppendLine("	  when item=3 then iCbSOQN2    ");
                sql.AppendLine("	  else 0  end as iCbSOQN    ");
                sql.AppendLine("	,case when item=1 then iTzhSOQN    ");
                sql.AppendLine("		  when item=2 then iTzhSOQN1    ");
                sql.AppendLine("		  when item=3 then iTzhSOQN2    ");
                sql.AppendLine("		  else 0  end as iTzhSOQN    ");
                sql.AppendLine("	from    ");
                sql.AppendLine("	(select '"+ strYearMonth + "' as vcYM ,1 as item union all select '"+ strYearMonth_2 + "' as vcYM,2 as item union all select '"+ strYearMonth_3 + "' as vcYM,3 as item)a    ");
                sql.AppendLine("	left join    ");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='"+strUserId+"')b on 1=1    ");
                sql.AppendLine(")t1    ");
                sql.AppendLine("left join    ");
                sql.AppendLine("(--手配主表    ");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime     ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "' and dFromTime<>dToTime      ");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)    ");
                sql.AppendLine("left join (    --//供应商工区 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,dFromTime,dToTime         ");
                sql.AppendLine("	from TSPMaster_SupplierPlant         ");
                sql.AppendLine("	where vcOperatorType='1'         ");
                sql.AppendLine(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId       ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t4.dFromTime,112) and convert(varchar(6),t4.dToTime,112)    ");
                sql.AppendLine("left join (    --//发注工厂 N    ");
                sql.AppendLine("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],        ");
                sql.AppendLine("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode         ");
                sql.AppendLine("	where vcCodeId='C010' and vcIsColum='0'         ");
                sql.AppendLine(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区      ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),fzgc.[开始时间],112) and convert(varchar(6),fzgc.[结束时间],112)    ");
                sql.AppendLine("left join(    --//收容数 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty,dFromTime,dToTime         ");
                sql.AppendLine("	from TSPMaster_Box         ");
                sql.AppendLine("	where vcOperatorType='1'         ");
                sql.AppendLine(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t5.dFromTime,112) and convert(varchar(6),t5.dToTime,112)    ");
                sql.AppendLine("left join         ");
                sql.AppendLine("(    --//受入 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn,dFromTime,dToTime    ");
                sql.AppendLine("	from TSPMaster_SufferIn        ");
                sql.AppendLine("	where vcOperatorType='1'        ");
                sql.AppendLine(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId      ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t6.dFromTime,112) and convert(varchar(6),t6.dToTime,112)    ");
                sql.AppendLine("where cast(t1.iCbSOQN as int) <>0 and (fzgc.供应商编号 is null or t5.vcPartId is null or t6.vcPartId is null)    ");
                sql.AppendLine("order by t1.vcPart_id,t1.vcYM");
                DataTable dt6 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                errMessageDict = ErrorMsg(errMessageDict, dtc, dt6, "{0}月无手配信息",true);
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
                sql.Length = 0;
                sql.AppendLine("select * from     ");
                sql.AppendLine("(    ");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id    ");
                sql.AppendLine("	,case when item=1 then iCbSOQN    ");
                sql.AppendLine("	  when item=2 then iCbSOQN1    ");
                sql.AppendLine("	  when item=3 then iCbSOQN2    ");
                sql.AppendLine("	  else 0  end as iCbSOQN    ");
                sql.AppendLine("	,case when item=1 then iTzhSOQN    ");
                sql.AppendLine("		  when item=2 then iTzhSOQN1    ");
                sql.AppendLine("		  when item=3 then iTzhSOQN2    ");
                sql.AppendLine("		  else 0  end as iTzhSOQN    ");
                sql.AppendLine("	from    ");
                sql.AppendLine("	(select '"+ strYearMonth + "' as vcYM ,1 as item union all select '"+strYearMonth_2+"' as vcYM,2 as item union all select '"+strYearMonth_3+"' as vcYM,3 as item)a    ");
                sql.AppendLine("	left join    ");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='"+strUserId+"')b on 1=1    ");
                sql.AppendLine(")t1    ");
                sql.AppendLine("left join    ");
                sql.AppendLine("(--手配主表    ");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime     ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "' and dFromTime<>dToTime      ");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)    ");
                sql.AppendLine("left join(    --//收容数 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty,dFromTime,dToTime         ");
                sql.AppendLine("	from TSPMaster_Box         ");
                sql.AppendLine("	where vcOperatorType='1'         ");
                sql.AppendLine(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t5.dFromTime,112) and convert(varchar(6),t5.dToTime,112)    ");
                sql.AppendLine("where cast(t1.iCbSOQN as int) <>0 and t1.iCbSOQN%t5.iPackingQty<>0    ");
                DataTable dt9 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                errMessageDict = ErrorMsg(errMessageDict, dtc, dt9, "订货数量不是收容数的整数倍",false);
                #endregion

                #region 验证10：if一括生产 校验： 对象月 > 实施年月时间 不能订货(数量得是0) 3个月都校验
                sql.Length = 0;
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '"+strYearMonth+"' as vcYM ,1 as item union all select '"+strYearMonth_2+"' as vcYM,2 as item union all select '"+strYearMonth_3+"' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='"+strUserId+"')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(--手配主表");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dDebugTime,dFromTime,dToTime ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "' and dFromTime<>dToTime  and vcOldProduction='一括生产'");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
                sql.AppendLine("and t1.vcYM>=convert(varchar(6),t2.dDebugTime,112)");
                sql.AppendLine("where cast(t1.iCbSOQN as int) <>0 and t2.vcPartId is not null");
                sql.AppendLine("order by t1.vcPart_id,t1.vcYM");
                DataTable dt10 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                errMessageDict = ErrorMsg(errMessageDict, dtc, dt10, "在{0}月以后不能订货", true);
                #endregion

                #region 验证11：特殊订货不能导入  
                sql.Length = 0;
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '"+strYearMonth+"' as vcYM ,1 as item union all select '"+strYearMonth_2+"' as vcYM,2 as item union all select '"+strYearMonth_3+"' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='"+strUserId+"')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(--手配主表");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dDebugTime,dFromTime,dToTime ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "' and dFromTime<>dToTime  and vcOrderingMethod='1'");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
                sql.AppendLine("where cast(t1.iCbSOQN as int) <>0 and t2.vcPartId is not null");
                sql.AppendLine("order by t1.vcPart_id,t1.vcYM");
                DataTable dt11 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                errMessageDict = ErrorMsg(errMessageDict, dtc, dt11, "{0}月特殊品番不能订货", true);
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
                DataTable temp = excute.ExcuteSqlWithSelectToDT("select top 1 vcValue from TCode where vcCodeId='C068'");
                string vcReceiver = temp.Rows.Count == 1 ? temp.Rows[0][0].ToString() : "APC06";

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
                    string strpartid = dt.Rows[i]["vcPart_id"].ToString().Length==10? dt.Rows[i]["vcPart_id"].ToString()+"00": dt.Rows[i]["vcPart_id"].ToString();
                    sql.Append("'" + strpartid + "',");
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
                //按N月更新
                sql.Append("update t1 set     \n");
                sql.Append("t1.vcCarFamilyCode=t2.vcCarfamilyCode,    \n");
                sql.Append("t1.vcCurrentPastcode=t2.vcHaoJiu,    \n");
                sql.Append("t1.vcMakingOrderType=t2.vcOrderingMethod,    \n");
                sql.Append("t1.vcFZGC=fzgc.发注工厂,    \n");
                sql.Append("t1.vcInOutFlag=t2.vcInOut,    \n");
                sql.Append("t1.vcSupplier_id=t2.vcSupplierId,    \n");
                sql.Append("t1.vcSupplierPlant=t4.vcSupplierPlant,    \n");
                sql.Append("t1.iQuantityPercontainer=t5.iPackingQty,   \n");
                sql.Append("t1.vcReceiver=t6.vcValue    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq where vcYearMonth='"+ strYearMonth + "')t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,vcOrderingMethod    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='"+strUnit+ "' and vcReceiver='" + vcReceiver + "'     \n");
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
                sql.Append("left join (select top 1 vcValue from TCode where vcCodeId='C068') t6 on 1=1    \n");
                //按N+1月更新
                sql.Append("update t1 set     \n");
                sql.Append("t1.vcCarFamilyCode=t2.vcCarfamilyCode,    \n");
                sql.Append("t1.vcCurrentPastcode=t2.vcHaoJiu,    \n");
                sql.Append("t1.vcMakingOrderType=t2.vcOrderingMethod,    \n");
                sql.Append("t1.vcFZGC=fzgc.发注工厂,    \n");
                sql.Append("t1.vcInOutFlag=t2.vcInOut,    \n");
                sql.Append("t1.vcSupplier_id=t2.vcSupplierId,    \n");
                sql.Append("t1.vcSupplierPlant=t4.vcSupplierPlant,    \n");
                sql.Append("t1.iQuantityPercontainer=t5.iPackingQty,   \n");
                sql.Append("t1.vcReceiver=t6.vcValue    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq where vcYearMonth='" + strYearMonth + "'      \n");
                sql.Append("    and (       \n");
                sql.Append("    	ISNULL(vcCarFamilyCode,'')='' or isnull(vcCurrentPastcode,'')=''  or        \n");
                sql.Append("    	ISNULL(vcMakingOrderType,'')='' or ISNULL(vcFZGC,'')='' or       \n");
                sql.Append("    	ISNULL(vcInOutFlag,'')='' or ISNULL(vcSupplier_id,'')='' or       \n");
                sql.Append("    	ISNULL(vcSupplierPlant,'')='' or ISNULL(iQuantityPercontainer,'')='' or       \n");
                sql.Append("    	ISNULL(vcReceiver,'')=''       \n");
                sql.Append("    )       \n");
                sql.Append(")t1           \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,vcOrderingMethod    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='" + vcReceiver + "'     \n");
                sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂
                sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                sql.Append("left join(    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("left join (select top 1 vcValue from TCode where vcCodeId='C068') t6 on 1=1    \n");
                //按N+2月更新
                sql.Append("update t1 set     \n");
                sql.Append("t1.vcCarFamilyCode=t2.vcCarfamilyCode,    \n");
                sql.Append("t1.vcCurrentPastcode=t2.vcHaoJiu,    \n");
                sql.Append("t1.vcMakingOrderType=t2.vcOrderingMethod,    \n");
                sql.Append("t1.vcFZGC=fzgc.发注工厂,    \n");
                sql.Append("t1.vcInOutFlag=t2.vcInOut,    \n");
                sql.Append("t1.vcSupplier_id=t2.vcSupplierId,    \n");
                sql.Append("t1.vcSupplierPlant=t4.vcSupplierPlant,    \n");
                sql.Append("t1.iQuantityPercontainer=t5.iPackingQty,   \n");
                sql.Append("t1.vcReceiver=t6.vcValue    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq where vcYearMonth='" + strYearMonth + "'      \n");
                sql.Append("    and (       \n");
                sql.Append("    	ISNULL(vcCarFamilyCode,'')='' or isnull(vcCurrentPastcode,'')=''  or        \n");
                sql.Append("    	ISNULL(vcMakingOrderType,'')='' or ISNULL(vcFZGC,'')='' or       \n");
                sql.Append("    	ISNULL(vcInOutFlag,'')='' or ISNULL(vcSupplier_id,'')='' or       \n");
                sql.Append("    	ISNULL(vcSupplierPlant,'')='' or ISNULL(iQuantityPercontainer,'')='' or       \n");
                sql.Append("    	ISNULL(vcReceiver,'')=''       \n");
                sql.Append("    )       \n");
                sql.Append(")t1           \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,vcOrderingMethod    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='" + vcReceiver + "'     \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append("    and dFromTime<>dToTime      \r\n ");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂
                sql.Append("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],    \n");
                sql.Append("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode     \n");
                sql.Append("	where vcCodeId='C010' and vcIsColum='0'    \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),vcValue3,112) and convert(varchar(6),vcValue4,112)     \n");
                sql.Append(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区     \n");
                sql.Append("left join(    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("left join (select top 1 vcValue from TCode where vcCodeId='C068') t6 on 1=1    \n");

                string strYear = strYearMonth.Substring(0, 4);
                string strMonth = strYearMonth.Substring(4, 2);
                DateTime dLastMonth = (DateTime.Parse(strYear + "-" + strMonth + "-01")).AddMonths(-1);
                string strLastYearMonth = dLastMonth.ToString("yyyyMM");
                //波动率计算
                //sql.AppendLine("  update TSoq set decCbBdl=100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN as decimal(18,2)))/cast(b.iHySOQN as decimal(18,2))  from TSoq a    \r\n ");
                sql.AppendLine("  update TSoq set decCbBdl=     \r\n ");
                sql.AppendLine("     case when b.iHySOQN is null then 1000     \r\n ");
                sql.AppendLine("     when a.iCbSOQN=0 or b.iHySOQN1=0 then  \r\n ");
                sql.AppendLine("     	100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN1 as decimal(18,2)))  \r\n ");
                sql.AppendLine("     else \r\n ");
                sql.AppendLine("     	100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN1 as decimal(18,2)))/cast(b.iHySOQN1 as decimal(18,2)) \r\n ");
                sql.AppendLine("     end    \r\n ");
                sql.AppendLine("  from TSoq a    \r\n ");
                sql.AppendLine("  left join    \r\n ");
                sql.AppendLine("  (    \r\n ");
                sql.AppendLine("    select * from TSoq where vcYearMonth='"+ strLastYearMonth + "'    \r\n ");
                sql.AppendLine("  )b on a.vcPart_id=b.vcPart_id    \r\n ");
                sql.AppendLine("  where a.vcYearMonth='" + strYearMonth + "' --and b.iHySOQN is not null    \r\n ");

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
                strSql.AppendLine("      ,iTzhSOQN=null,iTzhSOQN1=null,iTzhSOQN2=null,iHySOQN=null,iHySOQN1=null,iHySOQN2=null      \n");
                strSql.AppendLine("      ,dExpectTime=null,dOpenTime=null,vcOpenUser=null,dSReplyTime=null,vcSReplyUser=null,dReplyTime=null       \n");
                strSql.AppendLine(" WHERE vcYearMonth='" + strYearMonth + "' ");
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
                strSql.AppendLine("; \r\n ");


                strSql.AppendLine("delete from TSoq_OperHistory where vcYearMonth='" + strYearMonth + "'          \r\n ");
                //筛选条件：品番
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.AppendLine(" AND vcPart_id like '%" + strPart_id + "%' ");
                }

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
                strSql.AppendLine("      ,iTzhSOQN=null,iTzhSOQN1=null,iTzhSOQN2=null,iHySOQN=null,iHySOQN1=null,iHySOQN2=null      \n");
                strSql.AppendLine("      ,dExpectTime=null,dOpenTime=null,vcOpenUser=null,dSReplyTime=null,vcSReplyUser=null,dReplyTime=null       \n");
                strSql.AppendLine(" from TSoq a  \n ");
                strSql.AppendLine(" inner join  \n ");
                strSql.AppendLine(" (  \n ");
                strSql.AppendLine("    select vcPart_id from #TSoq_temp_back  \n ");
                strSql.AppendLine(" )b on a.vcPart_id=b.vcPart_id  \n ");
                strSql.Append(";  \n ");
                strSql.Append("delete t1    \n ");
                strSql.Append("from   \n ");
                strSql.Append("(select * from TSoq_OperHistory where vcYearMonth='"+strYearMonth+"')t1   \n ");
                strSql.Append("inner join (   \n ");
                strSql.Append("  select vcPart_id from #TSoq_temp_back   \n ");
                strSql.Append(")t2 on t1.vcPart_id=t2.vcPart_id   \n ");

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

        #region 返回发件人邮箱
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
                sql.Append("select vcValue2 as address,vcValue1 as displayName from TOutCode where vcCodeId='C018' and vcIsColum='0' ");
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