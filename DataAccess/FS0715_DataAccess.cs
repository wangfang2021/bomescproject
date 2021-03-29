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
    public class FS0715_DataAccess
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
        public DataTable Search(string PackSpot, string YearMonth)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select a.vcPackSpot,a.vcYearMonth,a.vcDay,a.vcBZ,a.vcModFlag,a.vcAddFlag,b.backColor from (    ");
                strSql.AppendLine("  SELECT vcPackSpot,vcYearMonth,vcDay=attribute, vcBZ=value ,'0' as vcModFlag,'0' as vcAddFlag    ");
                strSql.AppendLine("  FROM TPackCalendar     ");
                strSql.AppendLine("    UNPIVOT     ");
                strSql.AppendLine("    (     ");
                strSql.AppendLine("      value FOR attribute IN(vcDay01,vcDay02,vcDay03,vcDay04,vcDay05,vcDay06,vcDay07,vcDay08,vcDay09,vcDay10,     ");
                strSql.AppendLine("  	vcDay11,vcDay12,vcDay13,vcDay14,vcDay15,vcDay16,vcDay17,vcDay18,vcDay19,vcDay20,     ");
                strSql.AppendLine("  	vcDay21,vcDay22,vcDay23,vcDay24,vcDay25,vcDay26,vcDay27,vcDay28,vcDay29,vcDay30,vcDay31     ");
                strSql.AppendLine("  	)     ");
                strSql.AppendLine("    ) AS UPV     ");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(YearMonth))
                    strSql.AppendLine($"      AND vcYearMonth = '{YearMonth}'");
                strSql.AppendLine("  )a left join    	");
                strSql.AppendLine("  (    	");
                strSql.AppendLine("   select '无稼动'as vcName,'#adb2b8' as backColor    	");
                strSql.AppendLine("  )b on a.vcBZ=b.vcName    	");




                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导出检索
        public DataTable Search_EX(string PackSpot, string YearMonth)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from TPackCalendar   ");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");
                if (!string.IsNullOrEmpty(YearMonth))
                    strSql.AppendLine($"      AND vcYearMonth = '{YearMonth}'");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 导出检索
        public DataTable Search_BZ(string PackSpot)
        {
            try
            {

                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select *,'0' as vcModFlag,'0' as vcAddFlag  from    ");
                strSql.AppendLine("    TPackSpotBZ   ");
                strSql.AppendLine("      WHERE");
                strSql.AppendLine("      	1 = 1");
                if (!string.IsNullOrEmpty(PackSpot))
                    strSql.AppendLine($"      AND vcPackSpot = '{PackSpot}'");

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
                int count = 0;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("     delete  from TPackCalendar where vcPackSpot='" + listInfoData[0]["vcPackSpot"] + "' and vcYearMonth='" + listInfoData[0]["vcYearMonth"] + "'  ");

                sql.AppendLine("  INSERT INTO [dbo].[TPackCalendar]   ");
                sql.AppendLine("             ([vcPackSpot]   ");
                sql.AppendLine("             ,[vcYearMonth]   ");
                sql.AppendLine("             ,[vcDay01]   ");
                sql.AppendLine("             ,[vcDay02]   ");
                sql.AppendLine("             ,[vcDay03]   ");
                sql.AppendLine("             ,[vcDay04]   ");
                sql.AppendLine("             ,[vcDay05]   ");
                sql.AppendLine("             ,[vcDay06]   ");
                sql.AppendLine("             ,[vcDay07]   ");
                sql.AppendLine("             ,[vcDay08]   ");
                sql.AppendLine("             ,[vcDay09]   ");
                sql.AppendLine("             ,[vcDay10]   ");
                sql.AppendLine("             ,[vcDay11]   ");
                sql.AppendLine("             ,[vcDay12]   ");
                sql.AppendLine("             ,[vcDay13]   ");
                sql.AppendLine("             ,[vcDay14]   ");
                sql.AppendLine("             ,[vcDay15]   ");
                sql.AppendLine("             ,[vcDay16]   ");
                sql.AppendLine("             ,[vcDay17]   ");
                sql.AppendLine("             ,[vcDay18]   ");
                sql.AppendLine("             ,[vcDay19]   ");
                sql.AppendLine("             ,[vcDay20]   ");
                sql.AppendLine("             ,[vcDay21]   ");
                sql.AppendLine("             ,[vcDay22]   ");
                sql.AppendLine("             ,[vcDay23]   ");
                sql.AppendLine("             ,[vcDay24]   ");
                sql.AppendLine("             ,[vcDay25]   ");
                sql.AppendLine("             ,[vcDay26]   ");
                sql.AppendLine("             ,[vcDay27]   ");
                sql.AppendLine("             ,[vcDay28]   ");
                sql.AppendLine("             ,[vcDay29]   ");
                sql.AppendLine("             ,[vcDay30]   ");
                sql.AppendLine("             ,[vcDay31]   ");
                sql.AppendLine("             ,[vcDayTotal]   ");
                sql.AppendLine("             ,[vcOperatorID]   ");
                sql.AppendLine("             ,[dOperatorTime])   ");
                sql.AppendLine("       VALUES   ");
                sql.AppendLine("     	(  ");
                sql.AppendLine("     	'" + listInfoData[0]["vcPackSpot"] + "',  ");
                sql.AppendLine("     	'" + listInfoData[0]["vcYearMonth"] + "',  ");
                for (int i = 0; i < 31; i++)
                {
                    if (listInfoData[i]["vcBZ"].ToString() != "" && listInfoData[i]["vcBZ"].ToString() != "无稼动")
                    {
                        count++;
                    }
                    sql.AppendLine("     	'" + listInfoData[i]["vcBZ"] + "',  ");
                }
                sql.AppendLine("     	'" + count + "',  ");
                sql.AppendLine($"     		{strUserId},");
                sql.AppendLine("     		getDate()");
                sql.AppendLine("     	); ");
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


        #region 保存班制
        public void Save_BZ(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
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
                        sql.AppendLine("     delete  from TPackSpotBZ where vcPackSpot='" + listInfoData[i]["vcPackSpot"] + "'and vcBZ='" + listInfoData[i]["vcBZ"] + "'  ");

                        sql.AppendLine("  INSERT INTO [dbo].[TPackSpotBZ]    ");
                        sql.AppendLine("             ([vcPackSpot]     ");
                        sql.AppendLine("             ,[vcBZ]     ");
                        sql.AppendLine("             ,[vcBeginTime]     ");
                        sql.AppendLine("             ,[vcEndTime]     ");
                        sql.AppendLine("             ,[vcOperatorID]     ");
                        sql.AppendLine("             ,[dOperatorTime])     ");
                        sql.AppendLine("       VALUES     ");
                        sql.AppendLine("     (  ");
                        sql.AppendLine("     '" + listInfoData[i]["vcPackSpot"] + "',  ");
                        sql.AppendLine("     '" + listInfoData[i]["vcBZ"] + "',  ");
                        sql.AppendLine("     '" + listInfoData[i]["vcBeginTime"] + "',  ");
                        sql.AppendLine("     '" + listInfoData[i]["vcEndTime"] + "',  ");
                        sql.AppendLine($"     		{strUserId},");
                        sql.AppendLine("     		getDate()");
                        sql.AppendLine("      ) ");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoID"]);

                        sql.AppendLine("  UPDATE [TPackSpotBZ]");
                        sql.AppendLine("  SET ");
                        sql.AppendLine($"   vcPackSpot = '{listInfoData[i]["vcPackSpot"]}',\r\n");
                        sql.AppendLine($"   vcBZ = '{listInfoData[i]["vcBZ"]}',\r\n");
                        sql.AppendLine($"   vcBeginTime = '{listInfoData[i]["vcBeginTime"]}',\r\n");
                        sql.AppendLine($"   vcEndTime = '{listInfoData[i]["vcEndTime"]}',\r\n");
                        sql.AppendLine($"   vcOperatorID = '{strUserId}',\r\n");
                        sql.AppendLine($"   dOperatorTime = getdate() \r\n");
                        sql.AppendLine($"  WHERE \r\n");
                        sql.AppendLine($"  iAutoId='{iAutoId}'; \r\n");

                    }

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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("     delete  from TPackCalendar where vcPackSpot='" + dt.Rows[i]["vcPackSpot"] + "' and vcYearMonth='" + dt.Rows[i]["vcYearMonth"] + "'  ");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcDay01 = dt.Rows[i]["vcDay01"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay01"].ToString();
                    string vcDay02 = dt.Rows[i]["vcDay02"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay02"].ToString();
                    string vcDay03 = dt.Rows[i]["vcDay03"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay03"].ToString();
                    string vcDay04 = dt.Rows[i]["vcDay04"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay04"].ToString();
                    string vcDay05 = dt.Rows[i]["vcDay05"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay05"].ToString();
                    string vcDay06 = dt.Rows[i]["vcDay06"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay06"].ToString();
                    string vcDay07 = dt.Rows[i]["vcDay07"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay07"].ToString();
                    string vcDay08 = dt.Rows[i]["vcDay08"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay08"].ToString();
                    string vcDay09 = dt.Rows[i]["vcDay09"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay09"].ToString();
                    string vcDay10 = dt.Rows[i]["vcDay10"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay10"].ToString();
                    string vcDay11 = dt.Rows[i]["vcDay11"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay11"].ToString();
                    string vcDay12 = dt.Rows[i]["vcDay12"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay12"].ToString();
                    string vcDay13 = dt.Rows[i]["vcDay13"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay13"].ToString();
                    string vcDay14 = dt.Rows[i]["vcDay14"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay14"].ToString();
                    string vcDay15 = dt.Rows[i]["vcDay15"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay15"].ToString();
                    string vcDay16 = dt.Rows[i]["vcDay16"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay16"].ToString();
                    string vcDay17 = dt.Rows[i]["vcDay17"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay17"].ToString();
                    string vcDay18 = dt.Rows[i]["vcDay18"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay18"].ToString();
                    string vcDay19 = dt.Rows[i]["vcDay19"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay19"].ToString();
                    string vcDay20 = dt.Rows[i]["vcDay20"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay20"].ToString();
                    string vcDay21 = dt.Rows[i]["vcDay21"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay21"].ToString();
                    string vcDay22 = dt.Rows[i]["vcDay22"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay22"].ToString();
                    string vcDay23 = dt.Rows[i]["vcDay23"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay23"].ToString();
                    string vcDay24 = dt.Rows[i]["vcDay24"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay24"].ToString();
                    string vcDay25 = dt.Rows[i]["vcDay25"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay25"].ToString();
                    string vcDay26 = dt.Rows[i]["vcDay26"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay26"].ToString();
                    string vcDay27 = dt.Rows[i]["vcDay27"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay27"].ToString();
                    string vcDay28 = dt.Rows[i]["vcDay28"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay28"].ToString();
                    string vcDay29 = dt.Rows[i]["vcDay29"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay29"].ToString();
                    string vcDay30 = dt.Rows[i]["vcDay30"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay30"].ToString();
                    string vcDay31 = dt.Rows[i]["vcDay31"].ToString() == "" ? "非稼动" : dt.Rows[i]["vcDay31"].ToString();
                    sql.AppendLine("  INSERT INTO [dbo].[TPackCalendar]   ");
                    sql.AppendLine("             ([vcPackSpot]   ");
                    sql.AppendLine("             ,[vcYearMonth]   ");
                    sql.AppendLine("             ,[vcDay01]   ");
                    sql.AppendLine("             ,[vcDay02]   ");
                    sql.AppendLine("             ,[vcDay03]   ");
                    sql.AppendLine("             ,[vcDay04]   ");
                    sql.AppendLine("             ,[vcDay05]   ");
                    sql.AppendLine("             ,[vcDay06]   ");
                    sql.AppendLine("             ,[vcDay07]   ");
                    sql.AppendLine("             ,[vcDay08]   ");
                    sql.AppendLine("             ,[vcDay09]   ");
                    sql.AppendLine("             ,[vcDay10]   ");
                    sql.AppendLine("             ,[vcDay11]   ");
                    sql.AppendLine("             ,[vcDay12]   ");
                    sql.AppendLine("             ,[vcDay13]   ");
                    sql.AppendLine("             ,[vcDay14]   ");
                    sql.AppendLine("             ,[vcDay15]   ");
                    sql.AppendLine("             ,[vcDay16]   ");
                    sql.AppendLine("             ,[vcDay17]   ");
                    sql.AppendLine("             ,[vcDay18]   ");
                    sql.AppendLine("             ,[vcDay19]   ");
                    sql.AppendLine("             ,[vcDay20]   ");
                    sql.AppendLine("             ,[vcDay21]   ");
                    sql.AppendLine("             ,[vcDay22]   ");
                    sql.AppendLine("             ,[vcDay23]   ");
                    sql.AppendLine("             ,[vcDay24]   ");
                    sql.AppendLine("             ,[vcDay25]   ");
                    sql.AppendLine("             ,[vcDay26]   ");
                    sql.AppendLine("             ,[vcDay27]   ");
                    sql.AppendLine("             ,[vcDay28]   ");
                    sql.AppendLine("             ,[vcDay29]   ");
                    sql.AppendLine("             ,[vcDay30]   ");
                    sql.AppendLine("             ,[vcDay31]   ");
                    sql.AppendLine("             ,[vcDayTotal]   ");
                    sql.AppendLine("             ,[vcOperatorID]   ");
                    sql.AppendLine("             ,[dOperatorTime])   ");
                    sql.AppendLine("       VALUES   ");
                    sql.AppendLine("     	(  ");
                    sql.AppendLine("     	'" + dt.Rows[i]["vcPackSpot"].ToString() + "', ");
                    sql.AppendLine("     	'" + dt.Rows[i]["vcYearMonth"].ToString() + "', ");
                    sql.AppendLine("     	'" + vcDay01 + "', ");
                    sql.AppendLine("     	'" + vcDay02 + "', ");
                    sql.AppendLine("     	'" + vcDay03 + "', ");
                    sql.AppendLine("     	'" + vcDay04 + "', ");
                    sql.AppendLine("     	'" + vcDay05 + "', ");
                    sql.AppendLine("     	'" + vcDay06 + "', ");
                    sql.AppendLine("     	'" + vcDay07 + "', ");
                    sql.AppendLine("     	'" + vcDay08 + "', ");
                    sql.AppendLine("     	'" + vcDay09 + "', ");
                    sql.AppendLine("     	'" + vcDay10 + "', ");
                    sql.AppendLine("     	'" + vcDay11 + "', ");
                    sql.AppendLine("     	'" + vcDay12 + "', ");
                    sql.AppendLine("     	'" + vcDay13 + "', ");
                    sql.AppendLine("     	'" + vcDay14 + "', ");
                    sql.AppendLine("     	'" + vcDay15 + "', ");
                    sql.AppendLine("     	'" + vcDay16 + "', ");
                    sql.AppendLine("     	'" + vcDay17 + "', ");
                    sql.AppendLine("     	'" + vcDay18 + "', ");
                    sql.AppendLine("     	'" + vcDay19 + "', ");
                    sql.AppendLine("     	'" + vcDay20 + "', ");
                    sql.AppendLine("     	'" + vcDay21 + "', ");
                    sql.AppendLine("     	'" + vcDay22 + "', ");
                    sql.AppendLine("     	'" + vcDay23 + "', ");
                    sql.AppendLine("     	'" + vcDay24 + "', ");
                    sql.AppendLine("     	'" + vcDay25 + "', ");
                    sql.AppendLine("     	'" + vcDay26 + "', ");
                    sql.AppendLine("     	'" + vcDay27 + "', ");
                    sql.AppendLine("     	'" + vcDay28 + "', ");
                    sql.AppendLine("     	'" + vcDay29 + "', ");
                    sql.AppendLine("     	'" + vcDay30 + "', ");
                    sql.AppendLine("     	'" + vcDay31 + "', ");
                    int count = 0;
                    for (int x = 1; x <= 31; x++)
                    {
                        if (dt.Rows[i]["vcDay" + x.ToString().PadLeft(2,'0')].ToString() != "无稼动" || dt.Rows[i]["vcDay" + x.ToString()].ToString() != "")
                        {
                            count++;
                        }
                    }

                    sql.AppendLine("     	'"+ count + "', ");
                    sql.AppendLine($"     		{strUserId},");
                    sql.AppendLine("     		getDate()");
                    sql.AppendLine("     	)  ");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
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


        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete from  TPackSpotBZ where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoID"]);
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
