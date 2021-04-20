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
    public class FS0624_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search(string strChangeDateFrom, string strChangeDateTo, string strChangeNo, string strState, string strOrderNo )
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       select a.*,        \n");
                strSql.Append("       case when isnull(a.iQuantityNow,0)=0 then 0       \n");
                strSql.Append("       else cast(        \n");
                strSql.Append("       100*(cast(a.iQuantityNow as decimal(18,2))-cast(a.iQuantityBefore as decimal(18,2)))/cast(a.iQuantityBefore as decimal(18,2))         \n");
                strSql.Append("       as decimal(18,2)         \n");
                strSql.Append("       )        \n");
                strSql.Append("       end as decChangePercent        \n");
                strSql.Append("       ,b.iQuantityBeforeTotal,b.iQuantityNowTotal,c.vcState,a.vcChangeNo as bgll,CONVERT(varchar(100),a.vcChangeDate, 111) as vcChangeDateStr        \n");
                strSql.Append("       from        \n");

                strSql.Append("       (        \n");
                strSql.Append("           select a.vcChangeDate,a.vcChangeNo,a.vcGroupName,a.dFileUpload,a.vcOrderNo             \n");
                strSql.Append("           ,sum(a.iQuantityBefore) as iQuantityBefore              \n");
                strSql.Append("           ,sum(a.iQuantityNow) as iQuantityNow              \n");
                strSql.Append("           from             \n");
                strSql.Append("           (             \n");
                strSql.Append("           	select CAST(vcDXDate AS DATETIME)  as vcChangeDate,a.vcChangeNo               \n");
                strSql.Append("           	,c.vcGroupName,a.iQuantityBefore,a.iQuantityNow,a.dFileUpload,a.vcOrderNo             \n");
                strSql.Append("           	from             \n");
                strSql.Append("           	(             \n");
                strSql.Append("           		select vcChangeNo,vcDXDate              \n");
                strSql.Append("           		,case when iQuantityBefore<>iQuantityNow then '有变更' else '无变更' end as vcChangeType        \n");
                strSql.Append("           		,vcOrderNo,iQuantityBefore,iQuantityNow             \n");
                strSql.Append("           		,vcPart_Id,dFileUpload  from TSoqDayChange             \n");
                strSql.Append("           	)a             \n");
                strSql.Append("           	left join             \n");
                strSql.Append("           	(             \n");
                strSql.Append("           		select *  from  TDaysChangeOrdersBaseData              \n");
                strSql.Append("           	)b on a.vcPart_Id=b.vcPartNo             \n");
                strSql.Append("           	left join             \n");
                strSql.Append("           	(             \n");
                strSql.Append("           		select *  from  TGroup              \n");
                strSql.Append("           	)c on b.vcGroupId=c.iAutoId             \n");
                strSql.Append("           )a             \n");
                strSql.Append("           group by a.vcChangeDate,a.vcChangeNo,a.vcGroupName,a.dFileUpload,a.vcOrderNo             \n");
                strSql.Append("       )a        \n");
                strSql.Append("       left join        \n");
                strSql.Append("       (        \n");
                strSql.Append("             select a.vcChangeDate,a.vcChangeNo,a.vcGroupName          \n");
                strSql.Append("             ,sum(a.iQuantityBefore) as iQuantityBeforeTotal            \n");
                strSql.Append("             ,sum(a.iQuantityNow) as iQuantityNowTotal            \n");
                strSql.Append("             from           \n");
                strSql.Append("             (           \n");
                strSql.Append("             	select CAST(vcDXDate AS DATETIME)  as vcChangeDate,a.vcChangeNo             \n");
                strSql.Append("             	,c.vcGroupName,a.iQuantityBefore,a.iQuantityNow,a.dFileUpload,a.vcOrderNo           \n");
                strSql.Append("             	from           \n");
                strSql.Append("             	(           \n");
                strSql.Append("             		select vcChangeNo,vcDXDate            \n");
                strSql.Append("             		,case when iQuantityBefore<>iQuantityNow then '有变更' else '无变更' end as vcChangeType          \n");
                strSql.Append("             		,vcOrderNo,iQuantityBefore,iQuantityNow           \n");
                strSql.Append("             		,vcPart_Id,dFileUpload  from TSoqDayChange           \n");
                strSql.Append("             	)a           \n");
                strSql.Append("             	left join           \n");
                strSql.Append("             	(           \n");
                strSql.Append("             		select *  from  TDaysChangeOrdersBaseData            \n");
                strSql.Append("             	)b on a.vcPart_Id=b.vcPartNo           \n");
                strSql.Append("             	left join           \n");
                strSql.Append("             	(           \n");
                strSql.Append("             		select *  from  TGroup            \n");
                strSql.Append("             	)c on b.vcGroupId=c.iAutoId           \n");
                strSql.Append("             )a           \n");
                strSql.Append("             group by a.vcChangeDate,a.vcChangeNo,a.vcGroupName         \n");

                strSql.Append("       )b on  a.vcChangeDate=b.vcChangeDate and a.vcChangeNo=b.vcChangeNo  and a.vcGroupName=b.vcGroupName          \n");
                strSql.Append("       left join           \n");//状态关联 王方 提供的视图 2021-3-9
                strSql.Append("       (           \n");
                strSql.Append("         select a.vcCLYM,           \n");
                strSql.Append("         STUFF											                 \n");
                strSql.Append("      		(											                 \n");
                strSql.Append("      			(										                 \n");
                strSql.Append("      				SELECT ','+vcPlant+'厂:'+case when vcStatus='C' then '已送信' else '处理失败' end             \n");
                strSql.Append("      			FROM VI_NQCStatus_HS_EKANBAN                \n");
                strSql.Append("      				WHERE  VI_NQCStatus_HS_EKANBAN.vcKind='日度' and VI_NQCStatus_HS_EKANBAN.vcCLYM=a.vcCLYM                  \n");
                strSql.Append("      				FOR XML PATH('')									                 \n");
                strSql.Append("      			), 1, 1, ''										                 \n");
                strSql.Append("      		)   AS vcState  		           \n");
                strSql.Append("         from VI_NQCStatus_HS_EKANBAN a           \n");
                strSql.Append("         where a.vcKind='日度'              \n");
                strSql.Append("         GROUP BY vcCLYM              \n");
                strSql.Append("       )c on left(a.vcChangeNo,6)=c.vcCLYM           \n");
                strSql.Append("       where 1=1        \n");
                if (strChangeDateFrom!=null&&strChangeDateFrom != "")//开始日：由于变更号跟变更对象日是一个字段，所以这块直接用变更号检索就可以
                {
                    strSql.Append("       and a.vcChangeDate>='" + strChangeDateFrom + "'        \n");
                }
                if (strChangeDateTo != null && strChangeDateTo != "")//结束日
                {
                    strSql.Append("       and a.vcChangeDate<='" + strChangeDateTo + "'        \n");
                }
                if (strChangeNo != null && strChangeNo != "")//变更号
                {
                    strSql.Append("       and a.vcChangeNo='" + strChangeNo + "'        \n");
                }
                if (strState != null && strState != "")//状态
                {
                    strSql.Append("      and vcState like '%"+ strState + "%'       \n");
                }
                if (strOrderNo != null && strOrderNo != "")//订单号
                {
                    strSql.Append("       and vcOrderNo='" + strOrderNo + "'        \n");
                }
                strSql.Append("       order by a.vcChangeDate,a.vcChangeNo       \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 按检索条件检索,返回dt
        public DataTable SearchDetial(string strChangeNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select left(a.vcChangeNo,4)+'/'+substring(a.vcChangeNo,5,2)+'/'+right(a.vcChangeNo,2) as vcChangeDate,a.vcChangeNo           \n");
                strSql.Append("    ,c.vcGroupName,a.vcPart_Id,a.iQuantityBefore,a.iQuantityNow,        \n");
                strSql.Append("    case when isnull(a.iQuantityBefore,0)=0 then 0               \n");
                strSql.Append("           else cast(                \n");
                strSql.Append("            (        \n");
                strSql.Append("    			cast(a.iQuantityNow as decimal(18,2))-cast(a.iQuantityBefore as decimal(18,2)))        \n");
                strSql.Append("    			/cast(a.iQuantityBefore as decimal(18,2)        \n");
                strSql.Append("    		)*100               \n");
                strSql.Append("            as decimal(18,2)                 \n");
                strSql.Append("           )                \n");
                strSql.Append("           end as decChangePercent         \n");
                strSql.Append("    from                \n");
                strSql.Append("    (                \n");
                strSql.Append("    	select vcChangeNo                 \n");
                strSql.Append("    	,case when iQuantityBefore<>iQuantityNow then '有变更' else '无变更' end as vcChangeType                \n");
                strSql.Append("    	,vcOrderNo,iQuantityBefore,iQuantityNow                \n");
                strSql.Append("    	,vcPart_Id,dFileUpload  from TSoqDayChange                \n");
                strSql.Append("    )a                \n");
                strSql.Append("    left join                \n");
                strSql.Append("    (                \n");
                strSql.Append("    	select *  from  TDaysChangeOrdersBaseData                 \n");
                strSql.Append("    )b on a.vcPart_Id=b.vcPartNo                \n");
                strSql.Append("    left join                \n");
                strSql.Append("    (                \n");
                strSql.Append("    	select *  from  TGroup                 \n");
                strSql.Append("    )c on b.vcGroupId=c.iAutoId        \n");
                strSql.Append("    where vcChangeNo='"+ strChangeNo + "'        \n");
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