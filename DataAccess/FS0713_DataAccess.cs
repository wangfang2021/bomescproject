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
    public class FS0713_DataAccess
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
        public DataTable Search(List<object> PackSpot, string PackNo, string PackGPSNo, List<object> strSupplierCode)
        {
            try
            {


                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select *,'0' as vcModFlag,'0' as vcAddFlag ");
                strSql.AppendLine("      FROM");
                strSql.AppendLine("      	TPackSaveZK");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplierID in( ");
                    for (int i = 0; i < strSupplierCode.Count; i++)
                    {
                        if (strSupplierCode.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + strSupplierCode[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + strSupplierCode[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackNo LIKE '%{PackNo}%'");
                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int i = 0; i < PackSpot.Count; i++)
                    {
                        if (PackSpot.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + PackSpot[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + PackSpot[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (!string.IsNullOrEmpty(PackNo))
                    strSql.AppendLine($"      AND vcPackGPSNo LIKE '%{PackGPSNo}%'");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 更新在库安全表

        public void UpData(DataTable dt, string strUserId, ref string strErrorPartId, string strXHJiSuanType)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("  UPDATE TPackSaveZK");
                    sql.AppendLine("  SET ");
                    if (strXHJiSuanType == "平均")
                    {
                        sql.AppendLine($"   dAvgUse = {dt.Rows[i]["vcAvg"].ToString()},");
                    }
                    else
                    {
                        sql.AppendLine($"   dMax = {dt.Rows[i]["vcMax1"].ToString()},");
                    }
                    sql.AppendLine($"   vcSaveZK = '{dt.Rows[i]["dSaveZK"].ToString()}',");
                    sql.AppendLine($"   vcOperatorID = {strUserId},");
                    sql.AppendLine($"   dOperatorTime = '{DateTime.Now.ToString()}'");
                    sql.AppendLine($"  WHERE");
                    sql.AppendLine($"  vcPackSpot='{dt.Rows[i]["vcPackSpot"].ToString()}'and  vcPackNo='{dt.Rows[i]["vcPackNo"].ToString()}';");

                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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


        #region 计算建议在库
        public void JYZKCalcuate(List<object> packSpot, string strRatio, string strSaveAdvice, string strFrom, string strTo, string strXHJiSuanType, ref DataTable dtOldJS, DataTable dtendTime, DataTable dtPackBase, ref string strErrorPartId)
        {
            try
            {
               
                for (int i = 0; i < dtOldJS.Rows.Count; i++)
                {
                    DataRow[] dr = dtPackBase.Select("vcPackNo='" + dtOldJS.Rows[i]["vcPackNo"].ToString() + "'");
                    if (strSaveAdvice == "方法一")
                    {
                        double A = 0;
                        decimal B = 0;
                        double Max = 0;
                        decimal dSaveZK = 0;
                        if (strXHJiSuanType == "平均")
                        {
                            Max = Convert.ToDouble(dtOldJS.Rows[i]["vcAvg"].ToString());
                        }
                        else
                        {
                            Max = Convert.ToDouble(dtOldJS.Rows[i]["vcMax1"].ToString());
                        }

                        A = getA(dr[0]["vcCycle"].ToString().Split('-')[0], dr[0]["vcCycle"].ToString().Split('-')[1],
                            dr[0]["vcCycle"].ToString().Split('-')[2], Max,
                            Convert.ToInt32(dtOldJS.Rows[i]["iYXDate"].ToString()));
                        B = getB(A, Max, Convert.ToDouble(dr[0]["iRelease"].ToString()));
                        dSaveZK = Convert.ToDecimal(B * Convert.ToDecimal(dr[0]["iRelease"].ToString()) * Convert.ToDecimal(strRatio));
                        dtOldJS.Rows[i]["dSaveZK"] = dSaveZK;

                    }
                    else
                    {
                        if (strXHJiSuanType == "平均")
                        {
                            strErrorPartId = "方法二只针对峰值计算！";
                        }
                        else
                        {
                            decimal dSaveZK = 0;
                            decimal dSaveZK_1 = 0;
                            dSaveZK_1 = RoundUp(Convert.ToDecimal(dr[0]["dMax1_10"].ToString()), 0);
                            dSaveZK = dSaveZK_1 * Convert.ToDecimal(dr[0]["iRelease"].ToString()) + (1 * Convert.ToDecimal(dr[0]["iRelease"].ToString()));
                            dtOldJS.Rows[i]["dSaveZK"] = dSaveZK;
                        }


                    }
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


        #region 安全在库取A
        public double getA(string K1, string K2, string K3, double Max, int date)
        {
            try
            {

                double A = 0;
                int K3K1 = Convert.ToInt32(K1) + Convert.ToInt32(K3);

                int KK = Convert.ToInt32(K1) * K3K1 / Convert.ToInt32(K2);

                A = KK * Max / date;

                return A;
            }
            catch (Exception ex)
            {

                throw ex;

            }
        }
        #endregion

        #region 安全在库取B
        public decimal getB(Double A, Double Max, double dSR)
        {
            try
            {

                decimal B = 0;
                decimal value = 0;
                value = Convert.ToDecimal(A * Max / dSR);
                B = RoundUp(value, 0);
                return B;
            }
            catch (Exception ex)
            {

                throw ex;

            }
        }
        #endregion

        #region RoundUp
        private static decimal RoundUp(decimal val, int decPoint)
        {
            bool flagMinus = false;
            if (val < 0)
            {
                val = -val;
                flagMinus = true;
            }

            decimal newVal = Math.Round(val, decPoint);
            decimal difference = val - newVal;
            if (difference > 0)
            {
                decimal padding = 1 / (decimal)(Math.Pow(10, decPoint));
                newVal += padding;
            }
            if (flagMinus)
                return newVal * -1;
            else
                return newVal;
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
                        sql.AppendLine("     INSERT INTO TPackSaveZK( ");
                        sql.AppendLine("      vcPackSpot  \n\r");
                        sql.AppendLine("     , vcPackNo  \n\r");
                        sql.AppendLine("     , vcPackGPSNo  \n\r");
                        sql.AppendLine("     , vcSupplierCode \n\r");
                        sql.AppendLine("     , vcKBcycle , vcOperatorID,dOperatorTime  ) \n\r");

                        sql.AppendLine("     VALUES");
                        sql.AppendLine("     	(");

                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], true) + ",");
                        sql.AppendLine(ComFunction.getSqlValue(listInfoData[i]["vcKBcycle"], false) + ",");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("     	); ");

                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoID"]);

                        sql.AppendLine("  UPDATE TPackSaveZK");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPackSpot = {ComFunction.getSqlValue(listInfoData[i]["vcPackSpot"], false)},");
                        sql.AppendLine($"   vcPackNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackNo"], false)},");
                        sql.AppendLine($"   vcPackGPSNo = {ComFunction.getSqlValue(listInfoData[i]["vcPackGPSNo"], true)},");
                        sql.AppendLine($"   vcSupplierCode = {ComFunction.getSqlValue(listInfoData[i]["vcSupplierCode"], true)},");
                        sql.AppendLine($"   vcKBcycle = {ComFunction.getSqlValue(listInfoData[i]["vcKBcycle"], false)},");
                        sql.AppendLine($"   vcOperatorID = {strUserId},");
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

        #region 取结点时间
        public DataTable SearchEndTime(List<Object> PackSpot, string StrFrom, string StrTo, string strJiSuanType, List<Object> strSupplierCode, string PackGPSNo, string PackNo)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                if (strJiSuanType == "日")
                {
                    strSql.AppendLine("  select    ");
                    strSql.AppendLine("  TD1.vcPackSpot,TD1.vcPackNo,TD1.vcPackGPSNo,TD1.vcSupplierID,TD1.vcYMD,sum(TD1.inum) as inum   ");
                    strSql.AppendLine("  from(   ");
                }
                strSql.AppendLine(" select temp2.vcPackNo,temp2.vcPackGPSNo,temp2.vcSupplierID ,sum(temp2.iNumber) as inum,temp1.vcYMD    ");
                strSql.AppendLine(" ,temp1.vcYearMonth,temp1.vcBeginTime,temp1.vcEndTime,temp1.vcBZ,temp1.vcPackSpot    ");
                strSql.AppendLine("  from     ");
                strSql.AppendLine(" (    ");
                strSql.AppendLine(" select T1.vcPackSpot,T1.vcYearMonth,T1.vcDay,T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7) as vcYMD,    ");
                strSql.AppendLine(" CONVERT(varchar(50),T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7)+' '+T2.vcBeginTime,120) as vcBeginTime,    ");
                strSql.AppendLine(" case when T1.vcBZ='夜值'     ");
                strSql.AppendLine(" then CONVERT(varchar(50),DATEADD(DAY,1,T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7)+' '+T2.vcEndTime),120)    ");
                strSql.AppendLine(" else CONVERT(varchar(50),T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7)+' '+T2.vcEndTime,120) end as vcEndTime    ");
                strSql.AppendLine(" ,T1.vcBZ    ");
                strSql.AppendLine(" from(    ");
                strSql.AppendLine(" select a.vcPackSpot,a.vcYearMonth,a.vcDay,    ");
                strSql.AppendLine(" case when a.vcBZ='白值'or a.vcBZ='夜值' then vcBZ     ");
                strSql.AppendLine(" when a.vcBZ='无稼动' then '无稼动'    ");
                strSql.AppendLine(" else vcBZValue end as vcBZ    ");
                strSql.AppendLine("  from (    ");
                strSql.AppendLine(" SELECT vcPackSpot,vcYearMonth,vcDay=attribute, vcBZ=value     ");
                strSql.AppendLine("   FROM TPackCalendar         ");
                strSql.AppendLine("     UNPIVOT         ");
                strSql.AppendLine("     (         ");
                strSql.AppendLine("       value FOR attribute IN(vcDay01,vcDay02,vcDay03,vcDay04,vcDay05,vcDay06,vcDay07,vcDay08,vcDay09,vcDay10,    ");
                strSql.AppendLine("   	vcDay11,vcDay12,vcDay13,vcDay14,vcDay15,vcDay16,vcDay17,vcDay18,vcDay19,vcDay20,         ");
                strSql.AppendLine("   	vcDay21,vcDay22,vcDay23,vcDay24,vcDay25,vcDay26,vcDay27,vcDay28,vcDay29,vcDay30,vcDay31         ");
                strSql.AppendLine("   	)         ");
                strSql.AppendLine("     ) AS UPV         ");
                strSql.AppendLine("       WHERE    ");
                strSql.AppendLine("       	1 = 1    ");
                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int i = 0; i < PackSpot.Count; i++)
                    {
                        if (PackSpot.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + PackSpot[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + PackSpot[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                strSql.AppendLine(" 	  )a    ");
                strSql.AppendLine(" 	  left join    ");
                strSql.AppendLine(" 	  (    ");
                strSql.AppendLine(" 	  select * from TPackCalendar_bz    ");
                strSql.AppendLine(" 	  )b    ");
                strSql.AppendLine(" 	  on a.vcBZ=b.vcBanZhi    ");
                strSql.AppendLine(" )T1 left join    ");
                strSql.AppendLine(" (    ");
                strSql.AppendLine(" select vcPackSpot,    ");
                strSql.AppendLine("  case when vcBZ='DD' then '白值'else '夜值'end as vcBZ,    ");
                strSql.AppendLine("  vcBeginTime,vcEndTime    ");
                strSql.AppendLine("   from TPackSpotBZ    ");
                strSql.AppendLine(" )T2 on T1.vcBZ=T2.vcBZ    ");
                strSql.AppendLine(" where T1.vcBZ is not null and T1.vcBZ<>'无稼动'    ");
                strSql.AppendLine(" )temp1     ");
                strSql.AppendLine(" left join    ");
                strSql.AppendLine(" (    ");
                strSql.AppendLine(" select  * from TPackWork where vcZuoYeQuFen='2'    ");
                if (!string.IsNullOrEmpty(PackNo))
                {
                    strSql.AppendLine("  and vcPackNo='" + PackNo + "'  ");

                }
                if (!string.IsNullOrEmpty(PackGPSNo))
                {
                    strSql.AppendLine("  and vcPackGPSNo='" + PackGPSNo + "' ");
                }
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplierID in( ");
                    for (int i = 0; i < strSupplierCode.Count; i++)
                    {
                        if (strSupplierCode.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + strSupplierCode[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + strSupplierCode[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                strSql.AppendLine(" )temp2 on temp1.vcBeginTime<=temp2.dBuJiTime and temp1.vcEndTime>=temp2.dBuJiTime    ");
                strSql.AppendLine(" where temp1.vcEndTime between '" + StrFrom + "' and '" + StrTo + "'      ");
                strSql.AppendLine(" and vcPackNo is not null    ");
                strSql.AppendLine(" group by temp2.vcPackNo,temp2.vcPackGPSNo,temp2.vcSupplierID,temp1.vcYMD,    ");
                strSql.AppendLine(" temp1.vcYearMonth,temp1.vcBeginTime,temp1.vcEndTime,temp1.vcBZ,temp1.vcPackSpot    ");
                if (strJiSuanType == "日")
                {
                    strSql.AppendLine(" )TD1    ");
                    strSql.AppendLine(" group by TD1.vcYMD,TD1.vcPackNo,TD1.vcPackGPSNo,TD1.vcSupplierID,TD1.vcPackSpot    ");
                    strSql.AppendLine(" order by vcPackNo   ");
                }
                else
                {
                    strSql.AppendLine(" order by vcPackNo   ");

                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region  取定义时段
        public DataTable SearchSaveDate()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcFrom,vcTo,     ");
                strSql.AppendLine("  case when vcIsOrNoKT='0'then '否'else '是' end as vcIsOrNoKT ,     ");
                strSql.AppendLine("  vcFrom+'-'+vcTo as vcDate,     ");
                strSql.AppendLine("  case when vcIsOrNoKTFrom='0'then '否'else '是' end as vcIsOrNoKTFrom     ");
                strSql.AppendLine("  from TPackSaveDate     ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 时段计算获取
        public DataTable CalcuateSaveDate(List<object> packSpot, List<object> strSupplierCode, string packGPSNo, string packNo, string strFrom, string strTo, DataTable dtendTime)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select vcPackSpot,vcPackNo,vcPackGPSNo,vcBeginTime,vcEndTime,    ");
                strSql.AppendLine("  vcTime,sum(iNumber) as inum  from (    ");
                strSql.AppendLine("  select vcPackSpot,vcPackNo,vcPackGPSNo,vcBeginTime,vcEndTime,ddfrom,ddto,iNumber,vcSupplierID,    ");
                strSql.AppendLine("  SUBSTRING(CONVERT(varchar(50),ddfrom,120),0,17)+'-'+SUBSTRING(CONVERT(varchar(50),ddto,120),11,6)as vcTime from (    ");
                strSql.AppendLine("  select t1.vcYearMonth,t1.vcDay,t1.vcBeginTime,t1.vcEndTime,t1.vcBZ,    ");
                strSql.AppendLine("  CASe when t2.dfrom<t1.vcBeginTime and t2.dto<t1.vcEndTime and t2.dfrom<t1.vcEndTime and t2.dto>t1.vcBeginTime then t1.vcBeginTime    ");
                strSql.AppendLine("  	 when t2.dfrom>=t1.vcBeginTime and t2.dto<=t1.vcEndTime and t2.dfrom<t1.vcEndTime and t2.dto>t1.vcBeginTime then t2.dfrom    ");
                strSql.AppendLine("  	 when t2.dfrom>t1.vcBeginTime and t2.dto>t1.vcEndTime and t2.dfrom<t1.vcEndTime and t2.dto>t1.vcBeginTime then t2.dfrom    ");
                strSql.AppendLine("  else NULL    ");
                strSql.AppendLine("  end as ddfrom,    ");
                strSql.AppendLine("  CASe when t2.dfrom<t1.vcBeginTime and t2.dto<t1.vcEndTime and t2.dfrom<t1.vcEndTime and t2.dto>t1.vcBeginTime then t2.dto    ");
                strSql.AppendLine("  	 when t2.dfrom>=t1.vcBeginTime and t2.dto<=t1.vcEndTime and t2.dfrom<t1.vcEndTime and t2.dto>t1.vcBeginTime then t2.dto    ");
                strSql.AppendLine("  	 when t2.dfrom>t1.vcBeginTime and t2.dto>t1.vcEndTime and t2.dfrom<t1.vcEndTime and t2.dto>t1.vcBeginTime then t1.vcEndTime    ");
                strSql.AppendLine("  else NULL    ");
                strSql.AppendLine("  end as ddto    ");
                strSql.AppendLine("  from     ");
                strSql.AppendLine("  (select * from [VI_PackCalendar])t1    ");
                strSql.AppendLine("  full join    ");
                strSql.AppendLine("  (select a.vcYMD,    ");
                strSql.AppendLine("  case when b.vcIsOrNoKTFrom='0' then cast((a.vcYMD+' '+b.vcFrom) as datetime) else DATEADD(DAY,1, cast((a.vcYMD+' '+b.vcFrom) as datetime)) end    ");
                strSql.AppendLine("  case when b.vcIsOrNoKT='0' then cast((a.vcYMD+' '+b.vcTo) as datetime) else DATEADD(DAY,1, cast((a.vcYMD+' '+b.vcTo) as datetime)) end  as dto    ");
                strSql.AppendLine("  from     ");
                strSql.AppendLine("  (select distinct vcYMD from [VI_PackCalendar])a    ");
                strSql.AppendLine("  left join    ");
                strSql.AppendLine("  (select * from TPackSaveDate)b    ");
                strSql.AppendLine("  on 1=1)t2    ");
                strSql.AppendLine("  on t1.vcYMD=t2.vcYMD    ");
                strSql.AppendLine("  )tt1     ");
                strSql.AppendLine("  left join    ");
                strSql.AppendLine("  (    ");
                strSql.AppendLine("  select  * from TPackWork where vcZuoYeQuFen='2'      ");
                strSql.AppendLine("      ");
                strSql.AppendLine("  )tt2 on tt1.ddfrom<=tt2.dBuJiTime and tt1.ddto>=tt2.dBuJiTime      ");
                strSql.AppendLine("  where tt1.ddfrom is not null or tt1.ddto is not null     ");
                strSql.AppendLine("  and tt1.vcBeginTime between '"+ strFrom + "' and '"+ strTo + "'    ");
                if (packSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int i = 0; i < packSpot.Count; i++)
                    {
                        if (packSpot.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + packSpot[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + packSpot[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                if (!string.IsNullOrEmpty(packNo))
                {
                    strSql.AppendLine("  and vcPackNo='" + packNo + "'  ");

                }
                if (!string.IsNullOrEmpty(packGPSNo))
                {
                    strSql.AppendLine("  and vcPackGPSNo='" + packGPSNo + "' ");
                }
                if (strSupplierCode.Count != 0)
                {
                    strSql.AppendLine($"      AND vcSupplierID in( ");
                    for (int i = 0; i < strSupplierCode.Count; i++)
                    {
                        if (strSupplierCode.Count - i == 1)
                        {
                            strSql.AppendLine("   '" + strSupplierCode[i] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + strSupplierCode[i] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                strSql.AppendLine("  )temp1    ");
                strSql.AppendLine("  where vcPackNo is not null    ");
                strSql.AppendLine("  group by vcPackSpot,vcPackNo,vcPackGPSNo,vcBeginTime,vcEndTime,vcTime    ");
                

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 计算
        public DataTable Calcuate(List<object> packSpot, string strRatio, string strJiSuanType, string StrFrom, string StrTo, string strXHJiSuanType, DataTable dtCalcuateOld, DataTable dtendTime)
        {
            try
            {
                DataTable dtc = this.SearchDateCount(packSpot, StrFrom, StrTo, strJiSuanType);
                DataTable dtCalcuate = new DataTable();
                dtCalcuate.TableName = "123";
                int days = dtc.Rows.Count;
                if (strXHJiSuanType == "平均")
                {
                    switch (strJiSuanType)
                    {
                        case "日":
                            #region 日
                            dtCalcuate.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackGPSNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("iYXDate", Type.GetType("System.Int32"));
                            dtCalcuate.Columns.Add("dSaveZK", Type.GetType("System.Decimal"));
                            for (int i = 0; i < days; i++)
                            {
                                dtCalcuate.Columns.Add("vcDay" + dtc.Rows[i]["vcYMD"].ToString().Substring(5, 5), Type.GetType("System.String"));
                            }
                            dtCalcuate.Columns.Add("vcAvg", Type.GetType("System.Double"));
                            for (int j = 0; j < dtCalcuateOld.Rows.Count; j++)
                            {
                                DataRow drImport = dtCalcuate.NewRow();
                                string vcpackno = dtCalcuateOld.Rows[j]["vcPackNo"].ToString();
                                drImport["vcPackSpot"] = dtCalcuateOld.Rows[j]["vcPackSpot"].ToString();
                                drImport["vcPackNo"] = vcpackno;
                                drImport["vcPackGPSNo"] = dtCalcuateOld.Rows[j]["vcPackGPSNo"].ToString();
                                double count = 0;
                                int iYXDate = 0;
                                DataRow[] dr = dtCalcuateOld.Select("vcPackNo='" + vcpackno + "'");
                                for (int z = 0; z < dtCalcuate.Columns.Count - 6; z++)
                                {
                                    if (dr.Length > z)
                                    {
                                        drImport["vcDay" + dr[0]["vcYMD"].ToString().Substring(5, 4) + (z + 1).ToString()] = dtCalcuateOld.Rows[z]["inum"].ToString();
                                        count += Convert.ToDouble(dtCalcuateOld.Rows[z]["inum"].ToString());
                                        iYXDate++;
                                    }
                                }
                                drImport["iYXDate"] = iYXDate;
                                drImport["vcAvg"] = count / iYXDate;
                                dtCalcuate.Rows.Add(drImport);
                                foreach (DataRow row in dr)
                                {
                                    dtCalcuateOld.Rows.Remove(row);
                                }
                                j = -1;
                            }
                            #endregion
                            break;
                        case "班值":
                            #region 班值
                            dtCalcuate.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackGPSNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("iYXDate", Type.GetType("System.Int32"));
                            dtCalcuate.Columns.Add("dSaveZK", Type.GetType("System.Decimal"));
                            for (int i = 0; i < dtc.Rows.Count; i++)
                            {

                                dtCalcuate.Columns.Add("vcDay" + dtc.Rows[i]["vcBZ"].ToString().Substring(0, 1) + dtc.Rows[i]["vcYMD"].ToString().Substring(5, 5), Type.GetType("System.String"));

                            }

                            dtCalcuate.Columns.Add("vcAvg", Type.GetType("System.Double"));
                            for (int j = 0; j < dtCalcuateOld.Rows.Count; j++)
                            {
                                DataRow drImport = dtCalcuate.NewRow();
                                string vcpackno = dtCalcuateOld.Rows[j]["vcPackNo"].ToString();
                                drImport["vcPackSpot"] = dtCalcuateOld.Rows[j]["vcPackSpot"].ToString();
                                drImport["vcPackNo"] = vcpackno;
                                drImport["vcPackGPSNo"] = dtCalcuateOld.Rows[j]["vcPackGPSNo"].ToString();
                                double count = 0;
                                int iYXDate = 0;
                                DataRow[] dr = dtCalcuateOld.Select("vcPackNo='" + vcpackno + "'");
                                for (int z = 0; z < dtCalcuate.Columns.Count - 6; z++)
                                {
                                    if (dr.Length > z)
                                    {

                                        drImport["vcDay" + dr[z]["vcBZ"].ToString().Substring(0, 1) + dr[z]["vcYMD"].ToString().Substring(5, 5)] = dtCalcuateOld.Rows[z]["inum"].ToString();
                                        iYXDate++;
                                        count += Convert.ToDouble(dtCalcuateOld.Rows[z]["inum"].ToString());

                                    }
                                }
                                drImport["iYXDate"] = iYXDate;
                                drImport["vcAvg"] = count / iYXDate;
                                dtCalcuate.Rows.Add(drImport);
                                foreach (DataRow row in dr)
                                {
                                    dtCalcuateOld.Rows.Remove(row);
                                }
                                j = -1;
                            }
                            #endregion
                            break;
                        case "时段":
                            #region 时段
                            dtCalcuate.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackGPSNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("iYXDate", Type.GetType("System.Int32"));
                            dtCalcuate.Columns.Add("dSaveZK", Type.GetType("System.Decimal"));
                            for (int i = 0; i < days; i++)
                            {
                                for (int zz = 0; zz < dtendTime.Rows.Count; zz++)
                                {
                                    dtCalcuate.Columns.Add("vcDay" + dtc.Rows[i]["vcYMD"].ToString().Substring(5, 5) + " " + dtendTime.Rows[zz]["vcDate"].ToString(), Type.GetType("System.String"));
                                }
                            }
                            dtCalcuate.Columns.Add("vcAvg", Type.GetType("System.Double"));
                            for (int j = 0; j < dtCalcuateOld.Rows.Count; j++)
                            {
                                DataRow drImport = dtCalcuate.NewRow();
                                string vcpackno = dtCalcuateOld.Rows[j]["vcPackNo"].ToString();
                                drImport["vcPackSpot"] = dtCalcuateOld.Rows[j]["vcPackSpot"].ToString();
                                drImport["vcPackNo"] = vcpackno;
                                drImport["vcPackGPSNo"] = dtCalcuateOld.Rows[j]["vcPackGPSNo"].ToString();
                                double count = 0;
                                int iYXDate = 0;
                                DataRow[] dr = dtCalcuateOld.Select("vcPackNo='" + vcpackno + "'");
                                for (int z = 0; z < dtCalcuate.Columns.Count - 6; z++)
                                {
                                    if (dr.Length > z)
                                    {
                                        drImport["vcDay" + dr[z]["vcTime"].ToString().Substring(5, 17)] = dtCalcuateOld.Rows[z]["inum"].ToString();
                                        count += Convert.ToDouble(dtCalcuateOld.Rows[z]["inum"].ToString());
                                        iYXDate++;
                                    }
                                }
                                drImport["iYXDate"] = iYXDate;
                                drImport["vcAvg"] = count / iYXDate;
                                dtCalcuate.Rows.Add(drImport);
                                foreach (DataRow row in dr)
                                {
                                    dtCalcuateOld.Rows.Remove(row);
                                }
                                j = -1;
                            }
                            #endregion
                            break;

                    }

                }
                else
                {//峰值
                    switch (strJiSuanType)
                    {
                        case "日":
                            #region 日
                            dtCalcuate.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackGPSNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("iYXDate", Type.GetType("System.Int32"));
                            dtCalcuate.Columns.Add("dMax1_10", Type.GetType("System.Double"));
                            dtCalcuate.Columns.Add("dSaveZK", Type.GetType("System.Decimal"));
                            for (int i = 0; i < days; i++)
                            {
                                dtCalcuate.Columns.Add("vcDay" + dtc.Rows[i]["vcYMD"].ToString().Substring(5, 5), Type.GetType("System.String"));
                            }
                            for (int c = 1; c <= 10; c++)
                            {
                                dtCalcuate.Columns.Add("vcMax" + c.ToString(), Type.GetType("System.String"));
                            }
                            for (int j = 0; j < dtCalcuateOld.Rows.Count; j++)
                            {
                                DataRow drImport = dtCalcuate.NewRow();
                                string vcpackno = dtCalcuateOld.Rows[j]["vcPackNo"].ToString();
                                drImport["vcPackSpot"] = dtCalcuateOld.Rows[j]["vcPackSpot"].ToString();
                                drImport["vcPackNo"] = vcpackno;
                                drImport["vcPackGPSNo"] = dtCalcuateOld.Rows[j]["vcPackGPSNo"].ToString();
                                int iYXDate = 0;
                                double count = 0;
                                DataRow[] dr = dtCalcuateOld.Select("vcPackNo='" + vcpackno + "'", "inum desc");

                                for (int z = 0; z < dtCalcuate.Columns.Count - 6; z++)
                                {
                                    if (dr.Length > z)
                                    {
                                        drImport["vcDay" + dr[z]["vcYMD"].ToString().Substring(5, 4) + (z + 1).ToString()] = dtCalcuateOld.Rows[z]["inum"].ToString();
                                        iYXDate++;
                                        for (int cc = 1; cc <= 10; cc++)
                                        {
                                            if (cc <= dr.Length)
                                            {
                                                drImport["vcMax" + cc.ToString()] = dr[cc - 1]["inum"].ToString();
                                                count += Convert.ToDouble(dr[cc - 1]["inum"].ToString());

                                            }
                                            else
                                            {
                                                drImport["vcMax" + cc.ToString()] = "0";
                                            }
                                        }
                                    }
                                }
                                drImport["dMax1_10"] = count;
                                drImport["iYXDate"] = iYXDate;
                                dtCalcuate.Rows.Add(drImport);
                                foreach (DataRow row in dr)
                                {
                                    dtCalcuateOld.Rows.Remove(row);
                                }
                                j = -1;
                            }
                            #endregion
                            break;
                        case "班值":
                            #region 班值
                            dtCalcuate.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackGPSNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("iYXDate", Type.GetType("System.Int32"));
                            dtCalcuate.Columns.Add("dMax1_10", Type.GetType("System.Double"));
                            dtCalcuate.Columns.Add("dSaveZK", Type.GetType("System.Decimal"));
                            for (int i = 0; i < dtc.Rows.Count; i++)
                            {
                                dtCalcuate.Columns.Add("vcDay" + dtc.Rows[i]["vcBZ"].ToString().Substring(0, 1) + dtc.Rows[i]["vcYMD"].ToString().Substring(5, 5), Type.GetType("System.String"));
                            }
                            for (int c = 1; c <= 10; c++)
                            {
                                dtCalcuate.Columns.Add("vcMax" + c.ToString(), Type.GetType("System.String"));
                            }
                            for (int j = 0; j < dtCalcuateOld.Rows.Count; j++)
                            {
                                DataRow drImport = dtCalcuate.NewRow();
                                string vcpackno = dtCalcuateOld.Rows[j]["vcPackNo"].ToString();
                                drImport["vcPackSpot"] = dtCalcuateOld.Rows[j]["vcPackSpot"].ToString();
                                drImport["vcPackNo"] = vcpackno;
                                drImport["vcPackGPSNo"] = dtCalcuateOld.Rows[j]["vcPackGPSNo"].ToString();
                                DataRow[] dr = dtCalcuateOld.Select("vcPackNo='" + vcpackno + "'", "inum desc");
                                int iYXDate = 0;
                                double count = 0;
                                for (int z = 0; z < dtCalcuate.Columns.Count - 6; z++)
                                {
                                    if (dr.Length > z)
                                    {

                                        drImport["vcDay" + dr[z]["vcBZ"].ToString().Substring(0, 1) + dr[z]["vcYMD"].ToString().Substring(5, 5)] = dtCalcuateOld.Rows[z]["inum"].ToString();
                                        for (int cc = 1; cc <= 10; cc++)
                                        {
                                            if (cc <= dr.Length)
                                            {
                                                drImport["vcMax" + cc.ToString()] = dr[cc - 1]["inum"].ToString();
                                                count += Convert.ToDouble(dr[cc - 1]["inum"].ToString());
                                            }
                                            else
                                            {
                                                drImport["vcMax" + cc.ToString()] = "0";
                                            }
                                        }
                                    }
                                }
                                drImport["dMax1_10"] = count;
                                drImport["iYXDate"] = iYXDate;
                                dtCalcuate.Rows.Add(drImport);
                                foreach (DataRow row in dr)
                                {
                                    dtCalcuateOld.Rows.Remove(row);
                                }
                                j = -1;
                            }
                            #endregion
                            break;
                        case "时段":
                            #region 时段
                            dtCalcuate.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("vcPackGPSNo", Type.GetType("System.String"));
                            dtCalcuate.Columns.Add("iYXDate", Type.GetType("System.Int32"));
                            dtCalcuate.Columns.Add("dMax1_10", Type.GetType("System.Double"));
                            dtCalcuate.Columns.Add("dSaveZK", Type.GetType("System.Decimal"));
                            for (int i = 0; i < days; i++)
                            {
                                for (int zz = 0; zz < dtendTime.Rows.Count; zz++)
                                {
                                    dtCalcuate.Columns.Add("vcDay" + dtc.Rows[i]["vcYMD"].ToString().Substring(5, 5) + " " + dtendTime.Rows[zz]["vcDate"].ToString(), Type.GetType("System.String"));
                                }
                            }
                            for (int c = 1; c <= 10; c++)
                            {
                                dtCalcuate.Columns.Add("vcMax" + c.ToString(), Type.GetType("System.String"));
                            }
                            for (int j = 0; j < dtCalcuateOld.Rows.Count; j++)
                            {
                                DataRow drImport = dtCalcuate.NewRow();
                                string vcpackno = dtCalcuateOld.Rows[j]["vcPackNo"].ToString();
                                drImport["vcPackSpot"] = dtCalcuateOld.Rows[j]["vcPackSpot"].ToString();
                                drImport["vcPackNo"] = vcpackno;
                                drImport["vcPackGPSNo"] = dtCalcuateOld.Rows[j]["vcPackGPSNo"].ToString();
                                DataRow[] dr = dtCalcuateOld.Select("vcPackNo='" + vcpackno + "'", "inum desc");
                                int iYXDate = 0;
                                double count = 0;
                                for (int z = 0; z < dtCalcuate.Columns.Count - 6; z++)
                                {
                                    if (dr.Length > z)
                                    {
                                        drImport["vcDay" + dr[z]["vcTime"].ToString().Substring(5, 17)] = dtCalcuateOld.Rows[z]["inum"].ToString();
                                        iYXDate++;
                                        for (int cc = 1; cc <= 10; cc++)
                                        {
                                            if (cc <= dr.Length)
                                            {
                                                drImport["vcMax" + cc.ToString()] = dr[cc - 1]["inum"].ToString();
                                                count += Convert.ToDouble(dr[cc - 1]["inum"].ToString());
                                            }
                                            else
                                            {
                                                drImport["vcMax" + cc.ToString()] = "0";
                                            }
                                        }
                                    }
                                }
                                drImport["dMax1_10"] = count;
                                drImport["iYXDate"] = iYXDate;
                                dtCalcuate.Rows.Add(drImport);
                                foreach (DataRow row in dr)
                                {
                                    dtCalcuateOld.Rows.Remove(row);
                                }
                                j = -1;
                            }
                            #endregion
                            break;

                    }



                }

                return dtCalcuate;



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取循环列数

        public DataTable SearchDateCount(List<Object> PackSpot, string StrFrom, string StrTo, string strJiSuanType)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select     ");
                if (strJiSuanType == "日" || strJiSuanType == "时段")
                {
                    strSql.AppendLine("  distinct temp1.vcYMD    ");
                }
                else
                {
                    strSql.AppendLine("  *    ");
                }

                strSql.AppendLine("   from     ");
                strSql.AppendLine("  (    ");
                strSql.AppendLine("  select T1.vcPackSpot,T1.vcYearMonth,T1.vcDay,T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7) as vcYMD,    ");
                strSql.AppendLine("  CONVERT(varchar(50),T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7)+' '+T2.vcBeginTime,120) as vcBeginTime,    ");
                strSql.AppendLine("  case when T1.vcBZ='夜值'     ");
                strSql.AppendLine("  then CONVERT(varchar(50),DATEADD(DAY,1,T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7)+' '+T2.vcEndTime),120)    ");
                strSql.AppendLine("  else CONVERT(varchar(50),T1.vcYearMonth+'-'+SUBSTRING(T1.vcDay,6,7)+' '+T2.vcEndTime,120) end as vcEndTime    ");
                strSql.AppendLine("  ,T1.vcBZ    ");
                strSql.AppendLine("  from(    ");
                strSql.AppendLine("  select a.vcPackSpot,a.vcYearMonth,a.vcDay,    ");
                strSql.AppendLine("  case when a.vcBZ='白值'or a.vcBZ='夜值' then vcBZ     ");
                strSql.AppendLine("  when a.vcBZ='无稼动' then '无稼动'    ");
                strSql.AppendLine("  else vcBZValue end as vcBZ    ");
                strSql.AppendLine("   from (    ");
                strSql.AppendLine("  SELECT vcPackSpot,vcYearMonth,vcDay=attribute, vcBZ=value     ");
                strSql.AppendLine("    FROM TPackCalendar         ");
                strSql.AppendLine("      UNPIVOT         ");
                strSql.AppendLine("      (         ");
                strSql.AppendLine("        value FOR attribute IN(vcDay01,vcDay02,vcDay03,vcDay04,vcDay05,vcDay06,vcDay07,vcDay08,vcDay09,vcDay10,    ");
                strSql.AppendLine("    	vcDay11,vcDay12,vcDay13,vcDay14,vcDay15,vcDay16,vcDay17,vcDay18,vcDay19,vcDay20,         ");
                strSql.AppendLine("    	vcDay21,vcDay22,vcDay23,vcDay24,vcDay25,vcDay26,vcDay27,vcDay28,vcDay29,vcDay30,vcDay31         ");
                strSql.AppendLine("    	)         ");
                strSql.AppendLine("      ) AS UPV         ");
                strSql.AppendLine("        WHERE    ");
                strSql.AppendLine("        	1 = 1    ");

                if (PackSpot.Count != 0)
                {
                    strSql.AppendLine($"      AND vcPackSpot in( ");
                    for (int j = 0; j < PackSpot.Count; j++)
                    {
                        if (PackSpot.Count - j == 1)
                        {
                            strSql.AppendLine("   '" + PackSpot[j] + "'   \n");
                        }
                        else
                            strSql.AppendLine("  '" + PackSpot[j] + "' ,   \n");
                    }
                    strSql.Append("   )       \n");
                }
                strSql.AppendLine("  	  )a    ");
                strSql.AppendLine("  	  left join    ");
                strSql.AppendLine("  	  (    ");
                strSql.AppendLine("  	  select * from TPackCalendar_bz    ");
                strSql.AppendLine("  	  )b    ");
                strSql.AppendLine("  	  on a.vcBZ=b.vcBanZhi    ");
                strSql.AppendLine("  )T1 left join    ");
                strSql.AppendLine("  (    ");
                strSql.AppendLine("  select vcPackSpot,    ");
                strSql.AppendLine("   case when vcBZ='DD' then '白值'else '夜值'end as vcBZ,    ");
                strSql.AppendLine("   vcBeginTime,vcEndTime    ");
                strSql.AppendLine("    from TPackSpotBZ    ");
                strSql.AppendLine("  )T2 on T1.vcBZ=T2.vcBZ    ");
                strSql.AppendLine("  where T1.vcBZ is not null and T1.vcBZ<>'无稼动'     ");
                strSql.AppendLine("  )temp1     ");
                strSql.AppendLine("  where vcBeginTime between '" + StrFrom + "' and '" + StrTo + "'    ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region 插入定义时段
        public void InsertSaveDate(string dFrom1, string vcIsOrNoKTFrom1, string dTo1, string vcIsOrNoKT1, string dFrom2, string vcIsOrNoKTFrom2, string dTo2, string vcIsOrNoKT2, string dFrom3, string vcIsOrNoKTFrom3, string dTo3, string vcIsOrNoKT3, string dFrom4, string vcIsOrNoKTFrom4, string dTo4, string vcIsOrNoKT4, string dFrom5, string vcIsOrNoKTFrom5, string dTo5, string vcIsOrNoKT5, string dFrom6, string vcIsOrNoKTFrom6, string dTo6, string vcIsOrNoKT6, string dFrom7, string vcIsOrNoKTFrom7, string dTo7, string vcIsOrNoKT7, string dFrom8, string vcIsOrNoKTFrom8, string dTo8, string vcIsOrNoKT8, string strUserId, ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete from  TPackSaveDate  \r\n");
                if (!string.IsNullOrEmpty(dFrom1)&& !string.IsNullOrEmpty(dTo1)) {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom1 + "','" + dTo1 + "','" + vcIsOrNoKT1 + "','" + vcIsOrNoKTFrom1 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom2) && !string.IsNullOrEmpty(dTo2))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom2 + "','" + dTo2 + "','" + vcIsOrNoKT2 + "','" + vcIsOrNoKTFrom2 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom3) && !string.IsNullOrEmpty(dTo3))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom3 + "','" + dTo3 + "','" + vcIsOrNoKT3 + "','" + vcIsOrNoKTFrom3 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom4) && !string.IsNullOrEmpty(dTo4))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom4 + "','" + dTo4 + "','" + vcIsOrNoKT4 + "','" + vcIsOrNoKTFrom4 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom5) && !string.IsNullOrEmpty(dTo5))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom5 + "','" + dTo5 + "','" + vcIsOrNoKT5 + "','" + vcIsOrNoKTFrom5 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom6) && !string.IsNullOrEmpty(dTo6))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom6 + "','" + dTo6 + "','" + vcIsOrNoKT6 + "','" + vcIsOrNoKTFrom6 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom7) && !string.IsNullOrEmpty(dTo7))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom7 + "','" + dTo7 + "','" + vcIsOrNoKT7 + "','" + vcIsOrNoKTFrom7 + "') \r\n");

                }
                if (!string.IsNullOrEmpty(dFrom8) && !string.IsNullOrEmpty(dTo8))
                {
                    sql.Append("  insert into   TPackSaveDate values ('" + dFrom8 + "','" + dTo8 + "','" + vcIsOrNoKT8 + "','" + vcIsOrNoKTFrom8 + "') \r\n");

                }


                excute.ExcuteSqlWithStringOper(sql.ToString());

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






        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TPackSaveZK where iAutoId in(   \r\n ");
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
                StringBuilder sql = new StringBuilder();

                //  "vcPackSpot","vcPackNo","vcPackGPSNo","dPackFrom","dPackTo","vcParstName","vcSupplierName",
                //"vcSupplierCode","iRelease","iZCRelease","vcCycle","vcDistinguish","vcPackLocation","vcFormat","vcReleaseName"
                //先删除重复待更新的
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("  delete from  TPackBase where vcPackSpot=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + " and vcPackNo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + " and vcPackGPSNo=" + ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], false) + "   \r\n");
                }
                //插入
                sql.Append("  INSERT INTO [dbo].[TPackBase]   \r\n");
                sql.Append("             ([vcPackNo]   \r\n");
                sql.Append("             ,[vcPackSpot]   \r\n");
                sql.Append("             ,[dPackFrom]   \r\n");
                sql.Append("             ,[dPackTo]   \r\n");
                sql.Append("             ,[vcPackGPSNo]   \r\n");
                sql.Append("             ,[vcSupplierCode]   \r\n");
                sql.Append("             ,[vcSupplierPlant]   \r\n");
                sql.Append("             ,[vcCycle]   \r\n");
                sql.Append("             ,[vcSupplierName]   \r\n");
                sql.Append("             ,[vcParstName]   \r\n");
                sql.Append("             ,[vcPackLocation]   \r\n");
                sql.Append("             ,[vcDistinguish]   \r\n");
                sql.Append("             ,[vcFormat]   \r\n");
                //sql.Append("             ,[vcReleaseID]   \r\n");
                sql.Append("             ,[vcReleaseName]   \r\n");
                sql.Append("             ,[iRelease]   \r\n");
                sql.Append("             ,[iZCRelease]   \r\n");
                sql.Append("             ,[isYZC]   \r\n");
                sql.Append("             ,[vcOperatorID]   \r\n");
                sql.Append("             ,[dOperatorTime])   \r\n");
                sql.Append("       VALUES   \r\n");
                sql.Append("             (   \r\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackNo"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackSpot"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dPackFrom"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["dPackTo"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackGPSNo"], true) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierCode"], true) + ", \r\n");
                    sql.Append("   null,  \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcCycle"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcSupplierName"], true) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcParstName"], true) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcPackLocation"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcDistinguish"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcFormat"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["vcReleaseName"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["iRelease"], false) + ", \r\n");
                    sql.Append(ComFunction.getSqlValue(dt.Rows[i]["iZCRelease"], true) + ", \r\n");
                    sql.Append("   null,  \r\n");
                    sql.Append("   '" + strUserId + "',  \r\n");
                    sql.Append("   getdate())  \r\n");

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

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       SELECT * FROM TCode where vcCodeId='C023'       \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
