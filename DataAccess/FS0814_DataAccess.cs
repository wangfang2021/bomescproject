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
                strSql.Append("select   \n");
                strSql.Append(" [iAutoId]    \n");
                strSql.Append(",[vcYearMonth]    \n");
                strSql.Append(",[vcType]    \n");
                strSql.Append(",isnull([vcD1],'') as vcD1    \n");
                strSql.Append(",isnull([vcD2],'') as vcD2    \n");
                strSql.Append(",isnull([vcD3],'') as vcD3    \n");
                strSql.Append(",isnull([vcD4],'') as vcD4    \n");
                strSql.Append(",isnull([vcD5],'') as vcD5    \n");
                strSql.Append(",isnull([vcD6],'') as vcD6    \n");
                strSql.Append(",isnull([vcD7],'') as vcD7    \n");
                strSql.Append(",isnull([vcD8],'') as vcD8    \n");
                strSql.Append(",isnull([vcD9],'') as vcD9    \n");
                strSql.Append(",isnull([vcD10],'') as vcD10   \n");
                strSql.Append(",isnull([vcD11],'') as vcD11    \n");
                strSql.Append(",isnull([vcD12],'') as vcD12    \n");
                strSql.Append(",isnull([vcD13],'') as vcD13    \n");
                strSql.Append(",isnull([vcD14],'') as vcD14    \n");
                strSql.Append(",isnull([vcD15],'') as vcD15   \n");
                strSql.Append(",isnull([vcD16],'') as vcD16    \n");
                strSql.Append(",isnull([vcD17],'') as vcD17    \n");
                strSql.Append(",isnull([vcD18],'') as vcD18    \n");
                strSql.Append(",isnull([vcD19],'') as vcD19    \n");
                strSql.Append(",isnull([vcD20],'') as vcD20    \n");
                strSql.Append(",isnull([vcD21],'') as vcD21    \n");
                strSql.Append(",isnull([vcD22],'') as vcD22    \n");
                strSql.Append(",isnull([vcD23],'') as vcD23    \n");
                strSql.Append(",isnull([vcD24],'') as vcD24    \n");
                strSql.Append(",isnull([vcD25],'') as vcD25    \n");
                strSql.Append(",isnull([vcD26],'') as vcD26    \n");
                strSql.Append(",isnull([vcD27],'') as vcD27    \n");
                strSql.Append(",isnull([vcD28],'') as vcD28    \n");
                strSql.Append(",isnull([vcD29],'') as vcD29    \n");
                strSql.Append(",isnull([vcD30],'') as vcD30    \n");
                strSql.Append(",isnull([vcD31],'') as vcD31    \n");
                strSql.Append(",[vcOperatorID]    \n");
                strSql.Append(",[dOperatorTime]    \n");
                strSql.Append(",'0' as vcModFlag,'0' as vcAddFlag from TCalendar    \n");
                if (strYearMonth != "" && strYearMonth != null)
                    strSql.Append("where isnull(vcYearMonth,'') = '" + strYearMonth + "' \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string strUserId, ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TCalendar_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 插入临时表
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
                    sql.Append("           ('" + dt.Rows[i]["vcYearMonth"].ToString() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcType"].ToString() + "'   \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD1"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD2"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD3"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD4"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD5"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD6"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD7"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD8"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD9"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD10"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD11"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD12"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD13"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD14"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD15"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD16"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD17"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD18"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD19"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD20"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD21"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD22"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD23"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD24"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD25"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD26"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD27"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD28"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD29"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD30"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcD31"].ToString().ToUpper() + "'  \n");
                    sql.Append("           ,'" + strUserId + "'  \n");
                    sql.Append("           ,getdate())  \n");
                    #endregion
                }
                #region insert
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
                #endregion

                #region update
                sql.Append("update t2 set t2.vcD1=t1.vcD1,t2.vcD2=t1.vcD2,t2.vcD3=t1.vcD3,t2.vcD4=t1.vcD4,t2.vcD5=t1.vcD5, \n");
                sql.Append("t2.vcD6=t1.vcD6,t2.vcD7=t1.vcD7, t2.vcD8=t1.vcD8,t2.vcD9=t1.vcD9, t2.vcD10=t1.vcD10,t2.vcD11=t1.vcD11,   \n");
                sql.Append("    \n");
                sql.Append("t2.vcOperatorID=t1.vcOperatorID,t2.dOperatorTime=t1.dOperatorTime  \n");
                sql.Append("from  \n");
                sql.Append("(select * from TCalendar_Temp) t1  \n");
                sql.Append("left join TCalendar t2 on t1.vcYearMonth=t2.vcYearMonth and t1.vcType=t2.vcType  \n");
                sql.Append("where t2.iAutoId is not null  \n");
                sql.Append("and t1.vcOperatorID='" + strUserId + "'  \n");
                #endregion

                if (sql.Length > 0)
                {
                    #region 以下追加验证数据库中白夜是否有相同班值情况，如果存在则终止提交
                    sql.Append("  	  DECLARE @errorName varchar(50)      \r\n");
                    sql.Append("  	  set @errorName=''      \r\n");
                    sql.Append("  	  set @errorName=(      \r\n");
                    sql.Append("  	  	select vcYearMonth+'-'+vcday +';' from      \r\n");
                    sql.Append("  	  	(      \r\n");
                    sql.Append("            select distinct t1.vcYearMonth,          \r\n");
                    sql.Append("            case           \r\n");
                    sql.Append("            when t1.vcD1=t2.vcD1 then 'D1'           \r\n");
                    sql.Append("            when t1.vcD2=t2.vcD2 then 'D2'          \r\n");
                    sql.Append("            when t1.vcD3=t2.vcD3 then 'D3'          \r\n");
                    sql.Append("            when t1.vcD4=t2.vcD4 then 'D4'          \r\n");
                    sql.Append("            when t1.vcD5=t2.vcD5 then 'D5'          \r\n");
                    sql.Append("            when t1.vcD6=t2.vcD6 then 'D6'          \r\n");
                    sql.Append("            when t1.vcD7=t2.vcD7 then 'D7'          \r\n");
                    sql.Append("            when t1.vcD8=t2.vcD8 then 'D8'          \r\n");
                    sql.Append("            when t1.vcD9=t2.vcD9 then 'D9'          \r\n");
                    sql.Append("            when t1.vcD10=t2.vcD10 then 'D10'          \r\n");
                    sql.Append("            when t1.vcD11=t2.vcD11 then 'D11'          \r\n");
                    sql.Append("            when t1.vcD12=t2.vcD12 then 'D12'          \r\n");
                    sql.Append("            when t1.vcD13=t2.vcD13 then 'D13'          \r\n");
                    sql.Append("            when t1.vcD14=t2.vcD14 then 'D14'          \r\n");
                    sql.Append("            when t1.vcD15=t2.vcD15 then 'D15'          \r\n");
                    sql.Append("            when t1.vcD16=t2.vcD16 then 'D16'          \r\n");
                    sql.Append("            when t1.vcD17=t2.vcD17 then 'D17'          \r\n");
                    sql.Append("            when t1.vcD18=t2.vcD18 then 'D18'          \r\n");
                    sql.Append("            when t1.vcD19=t2.vcD19 then 'D19'          \r\n");
                    sql.Append("            when t1.vcD20=t2.vcD20 then 'D20'          \r\n");
                    sql.Append("            when t1.vcD21=t2.vcD21 then 'D21'          \r\n");
                    sql.Append("            when t1.vcD22=t2.vcD22 then 'D22'          \r\n");
                    sql.Append("            when t1.vcD23=t2.vcD23 then 'D23'          \r\n");
                    sql.Append("            when t1.vcD24=t2.vcD24 then 'D24'          \r\n");
                    sql.Append("            when t1.vcD25=t2.vcD25 then 'D25'          \r\n");
                    sql.Append("            when t1.vcD26=t2.vcD26 then 'D26'          \r\n");
                    sql.Append("            when t1.vcD27=t2.vcD27 then 'D27'          \r\n");
                    sql.Append("            when t1.vcD28=t2.vcD28 then 'D28'          \r\n");
                    sql.Append("            when t1.vcD29=t2.vcD29 then 'D29'          \r\n");
                    sql.Append("            when t1.vcD30=t2.vcD30 then 'D30'          \r\n");
                    sql.Append("            when t1.vcD31=t2.vcD31 then 'D31'          \r\n");
                    sql.Append("            end as vcday          \r\n");
                    sql.Append("            from (          \r\n");
                    sql.Append("            	select * from TCalendar where vcType='白'          \r\n");
                    sql.Append("            )t1          \r\n");
                    sql.Append("            left join (select * from TCalendar where vcType='夜') t2 on t1.vcYearMonth=t2.vcYearMonth          \r\n");
                    sql.Append("            where (isnull(t1.vcD1,'')!=''  and isnull(t2.vcD1,'')!='' and t1.vcD1=t2.vcD1) or           \r\n");
                    sql.Append("            (isnull(t1.vcD2,'')!=''  and isnull(t2.vcD2,'')!='' and t1.vcD2=t2.vcD2) or           \r\n");
                    sql.Append("            (isnull(t1.vcD3,'')!=''  and isnull(t2.vcD3,'')!='' and t1.vcD3=t2.vcD3) or           \r\n");
                    sql.Append("            (isnull(t1.vcD4,'')!=''  and isnull(t2.vcD4,'')!='' and t1.vcD4=t2.vcD4) or           \r\n");
                    sql.Append("            (isnull(t1.vcD5,'')!=''  and isnull(t2.vcD5,'')!='' and t1.vcD5=t2.vcD5) or          \r\n");
                    sql.Append("            (isnull(t1.vcD6,'')!=''  and isnull(t2.vcD6,'')!='' and t1.vcD6=t2.vcD6) or           \r\n");
                    sql.Append("            (isnull(t1.vcD7,'')!=''  and isnull(t2.vcD7,'')!='' and t1.vcD7=t2.vcD7) or           \r\n");
                    sql.Append("            (isnull(t1.vcD8,'')!=''  and isnull(t2.vcD8,'')!='' and t1.vcD8=t2.vcD8) or           \r\n");
                    sql.Append("            (isnull(t1.vcD9,'')!=''  and isnull(t2.vcD9,'')!='' and t1.vcD9=t2.vcD9) or           \r\n");
                    sql.Append("            (isnull(t1.vcD10,'')!=''  and isnull(t2.vcD10,'')!='' and t1.vcD10=t2.vcD10) or          \r\n");
                    sql.Append("            (isnull(t1.vcD11,'')!=''  and isnull(t2.vcD11,'')!='' and t1.vcD11=t2.vcD11) or           \r\n");
                    sql.Append("            (isnull(t1.vcD12,'')!=''  and isnull(t2.vcD12,'')!='' and t1.vcD12=t2.vcD12) or           \r\n");
                    sql.Append("            (isnull(t1.vcD13,'')!=''  and isnull(t2.vcD13,'')!='' and t1.vcD13=t2.vcD13) or           \r\n");
                    sql.Append("            (isnull(t1.vcD14,'')!=''  and isnull(t2.vcD14,'')!='' and t1.vcD14=t2.vcD14) or           \r\n");
                    sql.Append("            (isnull(t1.vcD15,'')!=''  and isnull(t2.vcD15,'')!='' and t1.vcD15=t2.vcD15) or          \r\n");
                    sql.Append("            (isnull(t1.vcD16,'')!=''  and isnull(t2.vcD16,'')!='' and t1.vcD16=t2.vcD16) or           \r\n");
                    sql.Append("            (isnull(t1.vcD17,'')!=''  and isnull(t2.vcD17,'')!='' and t1.vcD17=t2.vcD17) or           \r\n");
                    sql.Append("            (isnull(t1.vcD18,'')!=''  and isnull(t2.vcD18,'')!='' and t1.vcD18=t2.vcD18) or           \r\n");
                    sql.Append("            (isnull(t1.vcD19,'')!=''  and isnull(t2.vcD19,'')!='' and t1.vcD19=t2.vcD19) or           \r\n");
                    sql.Append("            (isnull(t1.vcD20,'')!=''  and isnull(t2.vcD20,'')!='' and t1.vcD20=t2.vcD20) or          \r\n");
                    sql.Append("            (isnull(t1.vcD21,'')!=''  and isnull(t2.vcD21,'')!='' and t1.vcD21=t2.vcD21) or           \r\n");
                    sql.Append("            (isnull(t1.vcD22,'')!=''  and isnull(t2.vcD22,'')!='' and t1.vcD22=t2.vcD22) or           \r\n");
                    sql.Append("            (isnull(t1.vcD23,'')!=''  and isnull(t2.vcD23,'')!='' and t1.vcD23=t2.vcD23) or           \r\n");
                    sql.Append("            (isnull(t1.vcD24,'')!=''  and isnull(t2.vcD24,'')!='' and t1.vcD24=t2.vcD24) or           \r\n");
                    sql.Append("            (isnull(t1.vcD25,'')!=''  and isnull(t2.vcD25,'')!='' and t1.vcD25=t2.vcD25) or          \r\n");
                    sql.Append("            (isnull(t1.vcD26,'')!=''  and isnull(t2.vcD26,'')!='' and t1.vcD26=t2.vcD26) or           \r\n");
                    sql.Append("            (isnull(t1.vcD27,'')!=''  and isnull(t2.vcD27,'')!='' and t1.vcD27=t2.vcD27) or           \r\n");
                    sql.Append("            (isnull(t1.vcD28,'')!=''  and isnull(t2.vcD28,'')!='' and t1.vcD28=t2.vcD28) or           \r\n");
                    sql.Append("            (isnull(t1.vcD29,'')!=''  and isnull(t2.vcD29,'')!='' and t1.vcD29=t2.vcD29) or           \r\n");
                    sql.Append("            (isnull(t1.vcD30,'')!=''  and isnull(t2.vcD30,'')!='' and t1.vcD30=t2.vcD30) or          \r\n");
                    sql.Append("            (isnull(t1.vcD31,'')!=''  and isnull(t2.vcD31,'')!='' and t1.vcD31=t2.vcD31)          \r\n");
                    sql.Append("  	  	)a for xml path('')      \r\n");
                    sql.Append("  	  )      \r\n");
                    sql.Append("  	         \r\n");
                    sql.Append("  	  if @errorName<>''      \r\n");
                    sql.Append("  	  begin      \r\n");
                    sql.Append("  	    select CONVERT(int,'-->'+@errorName+'<--')      \r\n");
                    sql.Append("  	  end       \r\n");
                    #endregion

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

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        #region update sql
                        sql.Append("UPDATE [TCalendar]    \n");
                        sql.Append("   SET [vcD1] = '" + listInfoData[i]["vcD1"].ToString().ToUpper() + "'  \n");
                        sql.Append("      ,[vcD2] = '" + listInfoData[i]["vcD2"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD3] = '" + listInfoData[i]["vcD3"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD4] = '" + listInfoData[i]["vcD4"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD5] = '" + listInfoData[i]["vcD5"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD6] = '" + listInfoData[i]["vcD6"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD7] = '" + listInfoData[i]["vcD7"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD8] = '" + listInfoData[i]["vcD8"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD9] = '" + listInfoData[i]["vcD9"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD10] = '" + listInfoData[i]["vcD10"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD11] = '" + listInfoData[i]["vcD11"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD12] = '" + listInfoData[i]["vcD12"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD13] = '" + listInfoData[i]["vcD13"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD14] = '" + listInfoData[i]["vcD14"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD15] = '" + listInfoData[i]["vcD15"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD16] = '" + listInfoData[i]["vcD16"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD17] = '" + listInfoData[i]["vcD17"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD18] = '" + listInfoData[i]["vcD18"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD19] = '" + listInfoData[i]["vcD19"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD20] = '" + listInfoData[i]["vcD20"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD21] = '" + listInfoData[i]["vcD21"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD22] = '" + listInfoData[i]["vcD22"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD23] = '" + listInfoData[i]["vcD23"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD24] = '" + listInfoData[i]["vcD24"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD25] = '" + listInfoData[i]["vcD25"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD26] = '" + listInfoData[i]["vcD26"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD27] = '" + listInfoData[i]["vcD27"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD28] = '" + listInfoData[i]["vcD28"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD29] = '" + listInfoData[i]["vcD29"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD30] = '" + listInfoData[i]["vcD30"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcD31] = '" + listInfoData[i]["vcD31"].ToString().ToUpper() + "'    \n");
                        sql.Append("      ,[vcOperatorID] = '"+strUserId+"'    \n");
                        sql.Append("      ,[dOperatorTime] = getdate()    \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "    \n");
                        #endregion
                    }
                }
                if (sql.Length > 0)
                {
                    #region 以下追加验证数据库中白夜是否有相同班值情况，如果存在则终止提交
                    sql.Append("  	  DECLARE @errorName varchar(50)      \r\n");
                    sql.Append("  	  set @errorName=''      \r\n");
                    sql.Append("  	  set @errorName=(      \r\n");
                    sql.Append("  	  	select vcYearMonth+'-'+vcday +';' from      \r\n");
                    sql.Append("  	  	(      \r\n");
                    sql.Append("            select distinct t1.vcYearMonth,          \r\n");
                    sql.Append("            case           \r\n");
                    sql.Append("            when t1.vcD1=t2.vcD1 then 'D1'           \r\n");
                    sql.Append("            when t1.vcD2=t2.vcD2 then 'D2'          \r\n");
                    sql.Append("            when t1.vcD3=t2.vcD3 then 'D3'          \r\n");
                    sql.Append("            when t1.vcD4=t2.vcD4 then 'D4'          \r\n");
                    sql.Append("            when t1.vcD5=t2.vcD5 then 'D5'          \r\n");
                    sql.Append("            when t1.vcD6=t2.vcD6 then 'D6'          \r\n");
                    sql.Append("            when t1.vcD7=t2.vcD7 then 'D7'          \r\n");
                    sql.Append("            when t1.vcD8=t2.vcD8 then 'D8'          \r\n");
                    sql.Append("            when t1.vcD9=t2.vcD9 then 'D9'          \r\n");
                    sql.Append("            when t1.vcD10=t2.vcD10 then 'D10'          \r\n");
                    sql.Append("            when t1.vcD11=t2.vcD11 then 'D11'          \r\n");
                    sql.Append("            when t1.vcD12=t2.vcD12 then 'D12'          \r\n");
                    sql.Append("            when t1.vcD13=t2.vcD13 then 'D13'          \r\n");
                    sql.Append("            when t1.vcD14=t2.vcD14 then 'D14'          \r\n");
                    sql.Append("            when t1.vcD15=t2.vcD15 then 'D15'          \r\n");
                    sql.Append("            when t1.vcD16=t2.vcD16 then 'D16'          \r\n");
                    sql.Append("            when t1.vcD17=t2.vcD17 then 'D17'          \r\n");
                    sql.Append("            when t1.vcD18=t2.vcD18 then 'D18'          \r\n");
                    sql.Append("            when t1.vcD19=t2.vcD19 then 'D19'          \r\n");
                    sql.Append("            when t1.vcD20=t2.vcD20 then 'D20'          \r\n");
                    sql.Append("            when t1.vcD21=t2.vcD21 then 'D21'          \r\n");
                    sql.Append("            when t1.vcD22=t2.vcD22 then 'D22'          \r\n");
                    sql.Append("            when t1.vcD23=t2.vcD23 then 'D23'          \r\n");
                    sql.Append("            when t1.vcD24=t2.vcD24 then 'D24'          \r\n");
                    sql.Append("            when t1.vcD25=t2.vcD25 then 'D25'          \r\n");
                    sql.Append("            when t1.vcD26=t2.vcD26 then 'D26'          \r\n");
                    sql.Append("            when t1.vcD27=t2.vcD27 then 'D27'          \r\n");
                    sql.Append("            when t1.vcD28=t2.vcD28 then 'D28'          \r\n");
                    sql.Append("            when t1.vcD29=t2.vcD29 then 'D29'          \r\n");
                    sql.Append("            when t1.vcD30=t2.vcD30 then 'D30'          \r\n");
                    sql.Append("            when t1.vcD31=t2.vcD31 then 'D31'          \r\n");
                    sql.Append("            end as vcday          \r\n");
                    sql.Append("            from (          \r\n");
                    sql.Append("            	select * from TCalendar where vcType='白'          \r\n");
                    sql.Append("            )t1          \r\n");
                    sql.Append("            left join (select * from TCalendar where vcType='夜') t2 on t1.vcYearMonth=t2.vcYearMonth          \r\n");
                    sql.Append("            where (isnull(t1.vcD1,'')!=''  and isnull(t2.vcD1,'')!='' and t1.vcD1=t2.vcD1) or           \r\n");
                    sql.Append("            (isnull(t1.vcD2,'')!=''  and isnull(t2.vcD2,'')!='' and t1.vcD2=t2.vcD2) or           \r\n");
                    sql.Append("            (isnull(t1.vcD3,'')!=''  and isnull(t2.vcD3,'')!='' and t1.vcD3=t2.vcD3) or           \r\n");
                    sql.Append("            (isnull(t1.vcD4,'')!=''  and isnull(t2.vcD4,'')!='' and t1.vcD4=t2.vcD4) or           \r\n");
                    sql.Append("            (isnull(t1.vcD5,'')!=''  and isnull(t2.vcD5,'')!='' and t1.vcD5=t2.vcD5) or          \r\n");
                    sql.Append("            (isnull(t1.vcD6,'')!=''  and isnull(t2.vcD6,'')!='' and t1.vcD6=t2.vcD6) or           \r\n");
                    sql.Append("            (isnull(t1.vcD7,'')!=''  and isnull(t2.vcD7,'')!='' and t1.vcD7=t2.vcD7) or           \r\n");
                    sql.Append("            (isnull(t1.vcD8,'')!=''  and isnull(t2.vcD8,'')!='' and t1.vcD8=t2.vcD8) or           \r\n");
                    sql.Append("            (isnull(t1.vcD9,'')!=''  and isnull(t2.vcD9,'')!='' and t1.vcD9=t2.vcD9) or           \r\n");
                    sql.Append("            (isnull(t1.vcD10,'')!=''  and isnull(t2.vcD10,'')!='' and t1.vcD10=t2.vcD10) or          \r\n");
                    sql.Append("            (isnull(t1.vcD11,'')!=''  and isnull(t2.vcD11,'')!='' and t1.vcD11=t2.vcD11) or           \r\n");
                    sql.Append("            (isnull(t1.vcD12,'')!=''  and isnull(t2.vcD12,'')!='' and t1.vcD12=t2.vcD12) or           \r\n");
                    sql.Append("            (isnull(t1.vcD13,'')!=''  and isnull(t2.vcD13,'')!='' and t1.vcD13=t2.vcD13) or           \r\n");
                    sql.Append("            (isnull(t1.vcD14,'')!=''  and isnull(t2.vcD14,'')!='' and t1.vcD14=t2.vcD14) or           \r\n");
                    sql.Append("            (isnull(t1.vcD15,'')!=''  and isnull(t2.vcD15,'')!='' and t1.vcD15=t2.vcD15) or          \r\n");
                    sql.Append("            (isnull(t1.vcD16,'')!=''  and isnull(t2.vcD16,'')!='' and t1.vcD16=t2.vcD16) or           \r\n");
                    sql.Append("            (isnull(t1.vcD17,'')!=''  and isnull(t2.vcD17,'')!='' and t1.vcD17=t2.vcD17) or           \r\n");
                    sql.Append("            (isnull(t1.vcD18,'')!=''  and isnull(t2.vcD18,'')!='' and t1.vcD18=t2.vcD18) or           \r\n");
                    sql.Append("            (isnull(t1.vcD19,'')!=''  and isnull(t2.vcD19,'')!='' and t1.vcD19=t2.vcD19) or           \r\n");
                    sql.Append("            (isnull(t1.vcD20,'')!=''  and isnull(t2.vcD20,'')!='' and t1.vcD20=t2.vcD20) or          \r\n");
                    sql.Append("            (isnull(t1.vcD21,'')!=''  and isnull(t2.vcD21,'')!='' and t1.vcD21=t2.vcD21) or           \r\n");
                    sql.Append("            (isnull(t1.vcD22,'')!=''  and isnull(t2.vcD22,'')!='' and t1.vcD22=t2.vcD22) or           \r\n");
                    sql.Append("            (isnull(t1.vcD23,'')!=''  and isnull(t2.vcD23,'')!='' and t1.vcD23=t2.vcD23) or           \r\n");
                    sql.Append("            (isnull(t1.vcD24,'')!=''  and isnull(t2.vcD24,'')!='' and t1.vcD24=t2.vcD24) or           \r\n");
                    sql.Append("            (isnull(t1.vcD25,'')!=''  and isnull(t2.vcD25,'')!='' and t1.vcD25=t2.vcD25) or          \r\n");
                    sql.Append("            (isnull(t1.vcD26,'')!=''  and isnull(t2.vcD26,'')!='' and t1.vcD26=t2.vcD26) or           \r\n");
                    sql.Append("            (isnull(t1.vcD27,'')!=''  and isnull(t2.vcD27,'')!='' and t1.vcD27=t2.vcD27) or           \r\n");
                    sql.Append("            (isnull(t1.vcD28,'')!=''  and isnull(t2.vcD28,'')!='' and t1.vcD28=t2.vcD28) or           \r\n");
                    sql.Append("            (isnull(t1.vcD29,'')!=''  and isnull(t2.vcD29,'')!='' and t1.vcD29=t2.vcD29) or           \r\n");
                    sql.Append("            (isnull(t1.vcD30,'')!=''  and isnull(t2.vcD30,'')!='' and t1.vcD30=t2.vcD30) or          \r\n");
                    sql.Append("            (isnull(t1.vcD31,'')!=''  and isnull(t2.vcD31,'')!='' and t1.vcD31=t2.vcD31)          \r\n");
                    sql.Append("  	  	)a for xml path('')      \r\n");
                    sql.Append("  	  )      \r\n");
                    sql.Append("  	         \r\n");
                    sql.Append("  	  if @errorName<>''      \r\n");
                    sql.Append("  	  begin      \r\n");
                    sql.Append("  	    select CONVERT(int,'-->'+@errorName+'<--')      \r\n");
                    sql.Append("  	  end       \r\n");
                    #endregion

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

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TCalendar where iAutoId=" + iAutoId + "   \n");
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
    }
}
