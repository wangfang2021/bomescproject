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

        #region 按检索条件检索,返回dt
        public DataTable Search(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                
                strSql.Append("SELECT iAutoId");
                strSql.Append("      ,vcYearMonth");
                strSql.Append("      ,vcDyState");
                strSql.Append("      ,vcHyState");
                strSql.Append("      ,vcPart_id");
                strSql.Append("      ,iCbSOQN");
                strSql.Append("      ,decCbBdl");
                strSql.Append("      ,iCbSOQN1");
                strSql.Append("      ,iCbSOQN2");
                strSql.Append("      ,iTzhSOQN");
                strSql.Append("      ,iTzhSOQN1");
                strSql.Append("      ,iTzhSOQN2");
                strSql.Append("      ,iHySOQN");
                strSql.Append("      ,iHySOQN1");
                strSql.Append("      ,iHySOQN2");
                strSql.Append("      ,dHyTime");
                strSql.Append("      ,b.vcName as 'vcDyState_Name'     \n");
                strSql.Append("      ,b2.vcName as 'vcHyState_Name'      \n");
                strSql.Append("  FROM TSoq a");
                strSql.Append("  left join      \n");
                strSql.Append("  (      \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId='C036'      \n");
                strSql.Append("  )b on a.vcDyState=b.vcValue      \n");
                strSql.Append("  left join      \n");
                strSql.Append("  (      \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId='C037'      \n");
                strSql.Append("  )b2 on a.vcHyState=b2.vcValue      \n");
                strSql.Append("  WHERE 1=1");

                if (!string.IsNullOrEmpty(strYearMonth)) {//对象年月
                    strSql.Append(" and vcYearMonth='"+ strYearMonth + "'");
                }
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                {
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

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importCheck(DataTable dt, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3, ref List<string> errMessageList)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" delete TSoq_temp where vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                //1、先插入
                sql.AppendLine("  INSERT INTO TSoq_temp( ");
                sql.AppendLine("vcYearMonth,");
                sql.AppendLine("vcDyState,");
                sql.AppendLine("vcHyState,");
                sql.AppendLine("vcPart_id,");
                sql.AppendLine("iCbSOQN,");
                sql.AppendLine("iCbSOQN1,");
                sql.AppendLine("iCbSOQN2,");
                sql.AppendLine("dDrTime,");
                sql.AppendLine("vcOperator,");
                sql.AppendLine("dOperatorTime");
                sql.AppendLine(")");
                sql.AppendLine("VALUES");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("('" + strYearMonth + "',");
                    sql.AppendLine("'0',");
                    sql.AppendLine("'0',");
                    sql.AppendLine("'" + dt.Rows[i]["vcPart_id"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN1"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN2"] + "',");
                    sql.AppendLine("getDate(),");
                    sql.AppendLine("'" + strUserId + "',");
                    sql.AppendLine("getDate()");
                    sql.AppendLine(")");

                    if (i < dt.Rows.Count - 1)
                    {
                        sql.Append(",");
                    }
                }
                sql.Append("; \r\n ");

                excute.ExcuteSqlWithStringOper(sql.ToString());//先导入临时表，然后check

                //验证1：是否为TFTM品番（包装工厂）
                sql.Length = 0;//清空
                sql.Append("   select a.vcPart_id from    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSoq_temp where vcYearMonth='"+ strYearMonth + "'    \r\n ");
                sql.Append("   )a    \r\n ");
                sql.Append("   left join    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from SP_M_SITEM    \r\n ");
                sql.Append("   )b on a.vcPart_id=b.PARTSNO    \r\n ");
                sql.Append("   where b.PARTSNO is  null    \r\n ");
                DataTable dt1=excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for(int i = 0; i < dt1.Rows.Count; i++) 
                {
                    string strPart_id=dt1.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add(strPart_id+ "在品番基础信息里不存在");
                }
                //验证2：N 月品番有效性  SP_M_SITEM    TIMEFROM  TIMETO   ，品番在时间区间内有数据     
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcYearMonth='" + strYearMonth + "'    \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    inner join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from SP_M_SITEM       \r\n ");
                sql.Append("    )a2 on a.vcPart_id=a2.PARTSNO      \r\n ");
                sql.Append("    left join    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from SP_M_SITEM where TIMEFROM<='" + strYearMonth + "' and TIMETO>='" + strYearMonth + "'    \r\n ");
                sql.Append("    )b on a.vcPart_id=b.PARTSNO    \r\n ");
                sql.Append("    where b.PARTSNO is  null      \r\n ");
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    string strPart_id = dt2.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add(strPart_id + "在品番基础信息存在，但不满足"+ strYearMonth + "月有效性条件");
                }
                //验证3：N+1 月品番有效性  SP_M_SITEM    TIMEFROM  TIMETO   ，品番在时间区间内有数据     
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcYearMonth='" + strYearMonth_2 + "'    \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    inner join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from SP_M_SITEM       \r\n ");
                sql.Append("    )a2 on a.vcPart_id=a2.PARTSNO      \r\n ");
                sql.Append("    left join    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from SP_M_SITEM where TIMEFROM<='" + strYearMonth_2 + "' and TIMETO>='" + strYearMonth_2 + "'    \r\n ");
                sql.Append("    )b on a.vcPart_id=b.PARTSNO    \r\n ");
                sql.Append("    where b.PARTSNO is  null      \r\n ");
                DataTable dt3 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    string strPart_id = dt3.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add(strPart_id + "在品番基础信息存在，但不满足" + strYearMonth_2 + "月有效性条件");
                }
                //验证4：N+2 月品番有效性  SP_M_SITEM    TIMEFROM  TIMETO   ，品番在时间区间内有数据     
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcYearMonth='" + strYearMonth_3 + "'    \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    inner join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from SP_M_SITEM       \r\n ");
                sql.Append("    )a2 on a.vcPart_id=a2.PARTSNO      \r\n ");
                sql.Append("    left join    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from SP_M_SITEM where TIMEFROM<='" + strYearMonth_3 + "' and TIMETO>='" + strYearMonth_3 + "'    \r\n ");
                sql.Append("    )b on a.vcPart_id=b.PARTSNO    \r\n ");
                sql.Append("    where b.PARTSNO is  null      \r\n ");
                DataTable dt4 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt4.Rows.Count; i++)
                {
                    string strPart_id = dt4.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add(strPart_id + "在品番基础信息存在，但不满足" + strYearMonth_3 + "月有效性条件");
                }
                //验证5：是否有价格，且在有效期内            TPrice  dUseBegin    dUseEnd ，品番在时间区间内有数据
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from     \r\n ");
                sql.Append("    (     \r\n ");
                sql.Append("       select * from TSoq_temp where vcYearMonth='"+ strYearMonth + "'     \r\n ");
                sql.Append("    )a     \r\n ");
                sql.Append("    left join     \r\n ");
                sql.Append("    (     \r\n ");
                sql.Append("       select vcPart_id from TPrice where  convert(varchar(6),dUseBegin,112)<='" + strYearMonth + "' and convert(varchar(6),dUseEnd,112)>='" + strYearMonth + "'     \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPart_id     \r\n ");
                sql.Append("    where b.vcPart_id is  null       \r\n ");
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt5.Rows.Count; i++)
                {
                    string strPart_id = dt5.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add(strPart_id + "在" + strYearMonth + "月没有维护价格");
                }
                //验证6：手配中是否有受入、收容数、发注工厂

                //验证7：收容数整倍数

                //验证8：if一括生产 校验： 对象月 > 实施年月时间 不能订货

                //验证9：如果是强制订货，没有价格也可以定。



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,string strYearMonth)
        {
            try
            {
                DateTime now = DateTime.Now;

                StringBuilder sql = new StringBuilder();

                sql.Append(" delete TSoq where vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                //1、先插入
                sql.AppendLine("  INSERT INTO TSoq( ");
                sql.AppendLine("vcYearMonth,");
                sql.AppendLine("vcDyState,");
                sql.AppendLine("vcHyState,");
                sql.AppendLine("vcPart_id,");
                sql.AppendLine("iCbSOQN,");
                sql.AppendLine("iCbSOQN1,");
                sql.AppendLine("iCbSOQN2,");
                sql.AppendLine("dDrTime,");
                sql.AppendLine("vcOperator,");
                sql.AppendLine("dOperatorTime");
                sql.AppendLine(")");

                sql.AppendLine("VALUES");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("('"+ strYearMonth + "',");
                    sql.AppendLine("'0',");
                    sql.AppendLine("'0',");
                    sql.AppendLine("'" + dt.Rows[i]["vcPart_id"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN1"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN2"] + "',");
                    sql.AppendLine("getDate(),");
                    sql.AppendLine("'"+ strUserId + "',");
                    sql.AppendLine("getDate()");
                    sql.AppendLine(")");

                    if (i < dt.Rows.Count - 1) {
                        sql.Append(",");
                    }
                }

                sql.Append("; \r\n ");
                //更新TSoqInput设定为导入成功，删除TSoqInputErrDetail
                sql.Append(" delete TSoqInput where vcYearMonth='" + strYearMonth + "' ;  \r\n ");
                sql.Append(" insert into TSoqInput(vcYearMonth,iState,vcOperator,dOperatorTime)values('" + strYearMonth + "',2,'"+ strUserId + "',getdate());  \r\n ");
                sql.Append(" delete TSoqInputErrDetail where vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                //2、再更新关联数据
                sql.AppendLine("  UPDATE TSoq SET \r\n ");
                sql.AppendLine("    vcCarFamilyCode=b.CARFAMILYCODE, \r\n ");
                sql.AppendLine("    vcCurrentPastcode=b.CURRENTPASTCODE, \r\n ");
                sql.AppendLine("    vcMakingOrderType=b.vcPackingFactory, \r\n ");
                sql.AppendLine("    vcFZGC=b.vcPlantCode, \r\n ");
                sql.AppendLine("    vcInOutFlag=b.INOUTFLAG, \r\n ");
                sql.AppendLine("    vcSupplier_id=b.SUPPLIERCODE, \r\n ");
                sql.AppendLine("    vcSupplierPlant=b.iSupplierPlant, \r\n ");
                sql.AppendLine("    iQuantityPercontainer=b.QUANTITYPERCONTAINER \r\n ");
                
                sql.AppendLine(" FROM TSoq a  \r\n ");
                sql.AppendLine(" LEFT JOIN    \r\n ");
                sql.AppendLine(" (   \r\n ");
                sql.AppendLine("    select PARTSNO,CARFAMILYCODE,CURRENTPASTCODE,vcPackingFactory,vcPlantCode,INOUTFLAG,SUPPLIERCODE,iSupplierPlant,QUANTITYPERCONTAINER from SP_M_SITEM    \r\n ");
                sql.AppendLine(" )b  ON b.PARTSNO=a.vcPart_id  \r\n ");//按照逻辑，需要按照品番、包装工厂、TC来连表查询
                sql.AppendLine("WHERE a.vcYearMonth='"+ strYearMonth + "'; \r\n ");


                string strYear = strYearMonth.Substring(0, 4);
                string strMonth = strYearMonth.Substring(4, 2);
                DateTime dLastMonth = (DateTime.Parse(strYear + "-" + strMonth + "-01")).AddMonths(-1);
                string strLastYearMonth = dLastMonth.ToString("yyyyMM");
                //波动率计算
                sql.AppendLine("  update TSoq set decCbBdl=100*(cast(a.iCbSOQN as decimal(18,2))-cast(b.iHySOQN as decimal(18,2)))/cast(b.iHySOQN as decimal(18,2))  from TSoq a    \r\n ");
                sql.AppendLine("  left join    \r\n ");
                sql.AppendLine("  (    \r\n ");
                sql.AppendLine("    select * from TSoq where vcYearMonth='"+ strLastYearMonth + "'    \r\n ");
                sql.AppendLine("  )b on a.vcPart_id=b.vcPart_id    \r\n ");
                sql.AppendLine("  where a.vcYearMonth='" + strYearMonth + "' and b.iHySOQN is not null    \r\n ");



                //在SOQprocess表中插入状态
                sql.AppendLine("DELETE TSOQProcess WHERE vcYearMonth='"+ strYearMonth + "'; \r\n ");
                sql.AppendLine("INSERT INTO TSOQProcess(INOUTFLAG,vcYearMonth,iStatus)  \r\n ");
                sql.AppendLine("VALUES('0','"+ strYearMonth + "',0), \r\n ");
                sql.AppendLine("('1','"+ strYearMonth + "',0); \r\n ");

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

        #region 向SOQ导入履历中新增数据
        public int importHistory(string strYearMonth, List<string> errMessageList)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //先删除本对象月再新增
                strSql.AppendLine(" DELETE FROM TSoqInput  WHERE vcYearMonth='"+ strYearMonth + "'; ");
                strSql.AppendLine(" ");

                for(int i=0;i< errMessageList.Count; i++)
                {
                    string msg = errMessageList[i].ToString();
                    strSql.AppendLine(" INSERT INTO TSoqInputErrDetail ");
                    strSql.AppendLine("      (vcYearMonth,");
                    strSql.AppendLine("       vcMessage");
                    strSql.AppendLine("      )     ");
                    strSql.AppendLine("      VALUES( ");
                    strSql.AppendLine("       '"+ strYearMonth + "',");
                    strSql.AppendLine("       '"+ msg + "'");
                    strSql.AppendLine("      );");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iHySOQN=iTzhSOQN,");
                strSql.AppendLine("      iHySOQN1=iTzhSOQN1,");
                strSql.AppendLine("      iHySOQN2=iTzhSOQN2,");
                strSql.AppendLine("      vcHyState='2', ");
                strSql.AppendLine("      dHyTime=getdate() ");
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
                
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}