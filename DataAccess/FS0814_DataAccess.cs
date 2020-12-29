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
    public class FS0814_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string strYearMonth)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from TCalendar  \n");
                if(strYearMonth!="" && strYearMonth!=null)
                    strSql.Append("where isnull(vcYearMonth,'') like '%" + strYearMonth + "%' \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TCalendar_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append("INSERT INTO [TCalendar_Temp]  \n");
                    sql.Append("           ([vcYearMonth]  \n");
                    sql.Append("           ,[vcType]  \n");
                    sql.Append("           ,[vcD1]  \n");
                    sql.Append("           ,[vcD2]  \n");
                    sql.Append("           ,[vcD3]  \n");
                    sql.Append("           ,[vcD4]  \n");
                    sql.Append("           ,[vcD5]  \n");
                    sql.Append("           ,[vcD6]  \n");
                    sql.Append("           ,[vcD7]  \n");
                    sql.Append("           ,[vcD8]  \n");
                    sql.Append("           ,[vcD9]  \n");
                    sql.Append("           ,[vcD10]  \n");
                    sql.Append("           ,[vcD11]  \n");
                    sql.Append("           ,[vcD12]  \n");
                    sql.Append("           ,[vcD13]  \n");
                    sql.Append("           ,[vcD14]  \n");
                    sql.Append("           ,[vcD15]  \n");
                    sql.Append("           ,[vcD16]  \n");
                    sql.Append("           ,[vcD17]  \n");
                    sql.Append("           ,[vcD18]  \n");
                    sql.Append("           ,[vcD19]  \n");
                    sql.Append("           ,[vcD20]  \n");
                    sql.Append("           ,[vcD21]  \n");
                    sql.Append("           ,[vcD22]  \n");
                    sql.Append("           ,[vcD23]  \n");
                    sql.Append("           ,[vcD24]  \n");
                    sql.Append("           ,[vcD25]  \n");
                    sql.Append("           ,[vcD26]  \n");
                    sql.Append("           ,[vcD27]  \n");
                    sql.Append("           ,[vcD28]  \n");
                    sql.Append("           ,[vcD29]  \n");
                    sql.Append("           ,[vcD30]  \n");
                    sql.Append("           ,[vcD31]  \n");
                    sql.Append("           ,[vcOperatorID]  \n");
                    sql.Append("           ,[dOperatorTime])  \n");
                    sql.Append("     VALUES  \n");
                    sql.Append("           ('"+dt.Rows[i]["vcYearMonth"].ToString()+"'  \n");
                    sql.Append("           ,'"+dt.Rows[i]["vcType"].ToString()+"'   \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD1"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD2"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD3"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD4"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD5"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD6"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD7"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD8"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD9"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD10"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD11"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD12"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD13"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD14"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD15"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD16"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD17"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD18"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD19"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD20"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD21"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD22"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD23"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD24"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD25"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD26"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD27"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD28"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD29"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD30"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD31"].ToString() + "'  \n");
                    sql.Append("           ,'"+strUserId+"'  \n");
                    sql.Append("           ,getdate())  \n");
                }
                sql.Append("INSERT INTO [TCalendar]  \n");
                sql.Append("           ([vcYearMonth],[vcType],[vcD1],[vcD2],[vcD3],[vcD4],[vcD5],[vcD6],[vcD7],[vcD8]  \n");
                sql.Append("           ,[vcD9],[vcD10],[vcD11],[vcD12],[vcD13],[vcD14],[vcD15],[vcD16],[vcD17],[vcD18]  \n");
                sql.Append("           ,[vcD19],[vcD20],[vcD21],[vcD22],[vcD23],[vcD24],[vcD25],[vcD26],[vcD27],[vcD28]  \n");
                sql.Append("           ,[vcD29],[vcD30],[vcD31],[vcOperatorID],[dOperatorTime])  \n");
                sql.Append("SELECT t1.[vcYearMonth],t1.[vcType],t1.[vcD1],t1.[vcD2],t1.[vcD3],t1.[vcD4],t1.[vcD5],t1.[vcD6]  \n");
                sql.Append("      ,t1.[vcD7],t1.[vcD8],t1.[vcD9],t1.[vcD10],t1.[vcD11],t1.[vcD12],t1.[vcD13],t1.[vcD14]  \n");
                sql.Append("      ,t1.[vcD15],t1.[vcD16],t1.[vcD17],t1.[vcD18],t1.[vcD19],t1.[vcD20],t1.[vcD21],t1.[vcD22]  \n");
                sql.Append("      ,t1.[vcD23],t1.[vcD24],t1.[vcD25],t1.[vcD26],t1.[vcD27],t1.[vcD28],t1.[vcD29],t1.[vcD30]  \n");
                sql.Append("      ,t1.[vcD31],t1.[vcOperatorID],t1.[dOperatorTime]  \n");
                sql.Append("  FROM [TCalendar_Temp] t1  \n");
                sql.Append("  left join TCalendar t2 on t1.vcYearMonth=t2.vcYearMonth and t1.vcType=t2.vcType  \n");
                sql.Append("  where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'  \n");

                sql.Append("update t2 set t2.vcD1=t1.vcD1,t2.vcD2=t1.vcD2,t2.vcD3=t1.vcD3,t2.vcD4=t1.vcD4,t2.vcD5=t1.vcD5, \n");
                sql.Append("t2.vcD6=t1.vcD6,t2.vcD7=t1.vcD7, t2.vcD8=t1.vcD8,t2.vcD9=t1.vcD9, t2.vcD10=t1.vcD10,t2.vcD11=t1.vcD11,   \n");
                sql.Append("    \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TCalendar_Temp) t1  \n");
                sql.Append("left join TCalendar t2 on t1.vcYearMonth=t2.vcYearMonth and t1.vcType=t2.vcType  \n");
                sql.Append("where t2.iAutoId is not null  \n");
                sql.Append("and t1.vcOperatorID='" + strUserId + "'  \n");

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
    }
}
