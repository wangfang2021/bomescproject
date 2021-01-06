using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0308_DataAccess
    {
        private MultiExcute excute = new MultiExcute();


        //检索
        public DataTable searchApi(string strYear)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                sbr.Append(" SELECT a.vcYear, a.vcSupplier_id, a.vcPart_id, a.vcPartNameEn,d.vcName AS vcInOutflag, a.vcCarTypeDev, \r\n");
                sbr.Append(" a.dJiuBegin, a.vcRemark, a.vcOld10, a.vcOld9, a.vcOld7,c.vcName AS vcPM, a.vcNum1, a.vcNum2, a.vcNum3, a.vcNXQF, \r\n");
                sbr.Append(" a.dTimeFrom, a.vcDY, a.vcNum11, a.vcNum12, a.vcNum13, a.vcNum14, a.vcNum15, a.vcNum16, a.vcNum17, a.vcNum18, a.vcNum19, a.vcNum20, a.vcNum21,'0' as vcModFlag \r\n");
                sbr.Append(" FROM TOldYearManager a \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C099') c ON SUBSTRING(a.vcPart_id,1,5) = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C003') d ON a.vcInOutflag = d.vcValue \r\n");
                sbr.Append(" WHERE a.vcYear = '" + strYear + "'  \r\n");
                sbr.Append(" AND a.vcFinish = '1' \r\n");



                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}