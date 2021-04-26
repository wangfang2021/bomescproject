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
    public class FS0711_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 
        public DataTable SearchSupplier()
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("     select distinct vcSupplierCode  as vcValue,vcSupplierName as vcName from TPackBase where isnull(vcSupplierCode,'')<>''  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 按检索条件检索,返回dt
        public DataTable Search(string PackSpot, string PackNo, string PackGPSNo, List<Object> Supplier)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("       from TPackZaiKu where 1=1  ");
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(PackGPSNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");
                if (Supplier.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplierID in( ");
                    for (int i = 0; i < Supplier.Count; i++)
                    {
                        if (Supplier.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + Supplier[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + Supplier[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 按检索条件检索,返回dt
        public DataTable Search_Sub(string PackSpot, string PackNo, string BeginTime, string EndTime)
        {
            try
            {
                if (string.IsNullOrEmpty(BeginTime))
                {
                    BeginTime = "1990-01-01 00:00:00";

                }

                if (string.IsNullOrEmpty(EndTime))
                {
                    EndTime = "9999-12-31 00:00:00";

                }

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select  ");
                strSql.AppendLine("  t1.vcPackNo,t1.vcPackSpot,   ");
                strSql.AppendLine("  t1.iLiLun+t1.iConsume-t1.iRKNum-t1.dChange as ibeginNum,   ");
                strSql.AppendLine("  t1.iConsume,t1.iRKNum,t1.iLiLun ,t1.dChange  ");
                strSql.AppendLine("  from(   ");
                strSql.AppendLine("  select a.vcPackSpot,a.vcPackNo,      ");
                strSql.AppendLine("        case when isnull(b.iRNum,0)<>0 then b.iRNum else 0 end  as iRKNum,      ");
                strSql.AppendLine(" 	   case when isnull(c.iCNum,0)<>0 then c.iCNum else 0 end  as iConsume,    ");
                strSql.AppendLine("        a.iLiLun,a.dChange from           ");
                strSql.AppendLine("   (    ");
                strSql.AppendLine("   select * from TPackZaiKu     ");
                strSql.AppendLine("   where 1=1    ");
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                strSql.AppendLine(" )a left join      ");
                strSql.AppendLine(" (      ");
                strSql.AppendLine(" select vcPackNo,sum(iNumber) as iRNum from TPackWork        ");
                strSql.AppendLine(" where vcZuoYeQuFen ='0'      ");
                strSql.AppendLine("    and dBuJiTime between '"+ BeginTime + "' and '"+ EndTime + "'   ");
                strSql.AppendLine("  group by vcPackNo   ");
                strSql.AppendLine("  )b on a.vcPackNo=b.vcPackNo     ");
                strSql.AppendLine("  left join     ");
                strSql.AppendLine("  (     ");
                strSql.AppendLine(" select vcPackNo,sum(iNumber) as iCNum from TPackWork           ");
                strSql.AppendLine("  where vcZuoYeQuFen ='2'      ");
                strSql.AppendLine("  and dBuJiTime between '"+ BeginTime + "' and '"+ EndTime + "'     ");
                strSql.AppendLine("  group by vcPackNo   ");
                strSql.AppendLine("  )c on a.vcPackNo=c.vcPackNo     ");
                strSql.AppendLine("   )t1     ");




                
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
                        sql.AppendLine("      vcReleaseID,");
                        sql.AppendLine("      vcReleaseName,");
                        sql.AppendLine("      iRelease,");
                        sql.AppendLine("      iZCRelease,");
                        sql.AppendLine("      isYZC,");
                        sql.AppendLine("      vcOperatorID,");
                        sql.AppendLine("      dOperatorTime");
                        sql.AppendLine("     )");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dPackFrom"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["dPackTo"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierPlant"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcCycle"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierName"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcParstName"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackLocation"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcDistinguish"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcFormat"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcReleaseID"], false) + ",");
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
                        sql.AppendLine($"   vcPackNo = '{ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)}',");
                        sql.AppendLine($"   vcPackSpot = '{ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)}',");
                        sql.AppendLine($"   dPackFrom = '{ComFunction.getSqlValue(listInfoData[i]["dPackFrom"], true)}',");
                        sql.AppendLine($"   dPackTo = '{ComFunction.getSqlValue(listInfoData[i]["dPackTo"], true)}',");
                        sql.AppendLine($"   vcPackGPSNo = '{ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], false)}',");
                        sql.AppendLine($"   vcSupplierCode = '{ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], false)}',");
                        sql.AppendLine($"   vcSupplierPlant = '{ComFunction.getSqlValue(listInfoData[i]["vcSupplierPlant"], false)}',");
                        sql.AppendLine($"   vcCycle = '{ComFunction.getSqlValue(listInfoData[i]["vcCycle"], false)}',");
                        sql.AppendLine($"   vcSupplierName = '{ComFunction.getSqlValue(listInfoData[i]["vcSupplierName"], false)}',");
                        sql.AppendLine($"   vcParstName = '{ComFunction.getSqlValue(listInfoData[i]["vcParstName"], false)}',");
                        sql.AppendLine($"   vcPackLocation = '{ComFunction.getSqlValue(listInfoData[i]["vcPackLocation"], false)}',");
                        sql.AppendLine($"   vcDistinguish = '{ComFunction.getSqlValue(listInfoData[i]["vcDistinguish"], false)}',");
                        sql.AppendLine($"   vcFormat = '{ComFunction.getSqlValue(listInfoData[i]["vcFormat"], false)}',");
                        sql.AppendLine($"   vcReleaseID = '{ComFunction.getSqlValue(listInfoData[i]["vcReleaseID"], false)}',");
                        sql.AppendLine($"   vcReleaseName = '{ComFunction.getSqlValue(listInfoData[i]["vcReleaseName"], false)}',");
                        sql.AppendLine($"   iRelease = '{ComFunction.getSqlValue(listInfoData[i]["iRelease"], true)}',");
                        sql.AppendLine($"   iZCRelease = '{ComFunction.getSqlValue(listInfoData[i]["iZCRelease"], true)}',");
                        sql.AppendLine($"   isYZC = '{ComFunction.getSqlValue(listInfoData[i]["isYZC"], true)}',");
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
