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
                strSql.Append("       ABS(100*(cast(a.iQuantityBefore as decimal(18,2))-cast(a.iQuantityNow as decimal(18,2))))/cast(a.iQuantityNow as decimal(18,2))         \n");
                strSql.Append("       as decimal(18,2)         \n");
                strSql.Append("       )        \n");
                strSql.Append("       end as decChangePercent        \n");
                strSql.Append("       ,b.iQuantityBeforeTotal,b.iQuantityNowTotal,'' as vcState  from        \n");
                strSql.Append("       (        \n");
                strSql.Append("       	select a.vcChangeDate,a.vcChangeNo,a.vcChangeType,a.vcGroupName,a.dFileUpload,a.vcOrderNo        \n");
                strSql.Append("       	,sum(a.iQuantityBefore) as iQuantityBefore         \n");
                strSql.Append("       	,sum(a.iQuantityNow) as iQuantityNow         \n");
                strSql.Append("       	from        \n");
                strSql.Append("       	(        \n");
                strSql.Append("       		select left(a.vcChangeNo,4)+'/'+substring(a.vcChangeNo,5,2)+'/'+right(a.vcChangeNo,2) as vcChangeDate,a.vcChangeNo          \n");
                strSql.Append("       		,a.vcChangeType,c.vcGroupName,a.iQuantityBefore,a.iQuantityNow,a.dFileUpload,a.vcOrderNo        \n");
                strSql.Append("       		from        \n");
                strSql.Append("       		(        \n");
                strSql.Append("       			select vcChangeNo         \n");
                strSql.Append("       			,case when iQuantityBefore<>iQuantityNow then '有变更' else '无变更' end as vcChangeType        \n");
                strSql.Append("       			,vcOrderNo,iQuantityBefore,iQuantityNow        \n");
                strSql.Append("       			,vcPart_Id,dFileUpload  from TSoqDayChange        \n");
                strSql.Append("       		)a        \n");
                strSql.Append("       		left join        \n");
                strSql.Append("       		(        \n");
                strSql.Append("       			select *  from  TDaysChangeOrdersBaseData         \n");
                strSql.Append("       		)b on a.vcPart_Id=b.vcPartNo        \n");
                strSql.Append("       		left join        \n");
                strSql.Append("       		(        \n");
                strSql.Append("       			select *  from  TGroup         \n");
                strSql.Append("       		)c on b.vcGroupId=c.iAutoId        \n");
                strSql.Append("       	)a        \n");
                strSql.Append("       	group by a.vcChangeDate,a.vcChangeNo,a.vcChangeType,a.vcGroupName,a.dFileUpload,a.vcOrderNo        \n");
                strSql.Append("       )a        \n");
                strSql.Append("       left join        \n");
                strSql.Append("       (        \n");
                strSql.Append("           select a.vcChangeNo,a.vcChangeType         \n");
                strSql.Append("       	,sum(a.iQuantityBefore) as iQuantityBeforeTotal         \n");
                strSql.Append("       	,sum(a.iQuantityNow) as iQuantityNowTotal        \n");
                strSql.Append("       	from        \n");
                strSql.Append("       	(        \n");
                strSql.Append("       		select left(a.vcChangeNo,4)+'/'+substring(a.vcChangeNo,5,2)+'/'+right(a.vcChangeNo,2) as vcChangeDate,a.vcChangeNo          \n");
                strSql.Append("       		,a.vcChangeType,a.iQuantityBefore,a.iQuantityNow,a.dFileUpload,a.vcOrderNo        \n");
                strSql.Append("       		from        \n");
                strSql.Append("       		(        \n");
                strSql.Append("       			select vcChangeNo         \n");
                strSql.Append("       			,case when iQuantityBefore<>iQuantityNow then '有变更' else '无变更' end as vcChangeType        \n");
                strSql.Append("       			,vcOrderNo,iQuantityBefore,iQuantityNow        \n");
                strSql.Append("       			,vcPart_Id,dFileUpload  from TSoqDayChange        \n");
                strSql.Append("       		)a        \n");
                strSql.Append("       	)a        \n");
                strSql.Append("       	group by a.vcChangeNo,a.vcChangeType         \n");
                strSql.Append("       )b on a.vcChangeNo=b.vcChangeNo and a.vcChangeType=b.vcChangeType        \n");
                strSql.Append("       where 1=1        \n");
                if (strChangeDateFrom != "")//开始日：由于变更号跟变更对象日是一个字段，所以这块直接用变更号检索就可以
                {
                    strSql.Append("       and vcChangeNo>='"+ strChangeDateFrom + "'        \n");
                }
                if (strChangeDateTo != "")//结束日
                {
                    strSql.Append("       and vcChangeNo<='" + strChangeDateTo + "'        \n");
                }
                if (strChangeNo != "")//变更号
                {
                    strSql.Append("       and vcChangeNo='" + strChangeDateTo + "'        \n");
                }
                if (strState != "")//状态
                {

                }
                if (strOrderNo != "")//订单号
                {
                    strSql.Append("       and vcOrderNo='" + strOrderNo + "'        \n");
                }
                strSql.Append("       order by a.vcChangeDate,a.vcChangeNo,a.vcChangeType        \n");
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