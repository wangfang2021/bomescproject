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

        //#region 保存
        //public void importSave(DataTable dt, string strUserId)
        //{
        //    try
        //    {

        //        StringBuilder sql = new StringBuilder();
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            string vcFinish = dt.Rows[i]["vcFinish"].ToString();

        //            sql.Append(" UPDATE TOldYearManager SET \r\n");
        //            if (vcFinish.Equals("对象外"))
        //            {
        //                sql.Append(" vcFinish = '" + getValue("C024", vcFinish) + "', \r\n");
        //                sql.Append(" dFinishYMD = GETDATE(), \r\n");
        //            }
        //            sql.Append(" vcNum1 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum1"], false) + ", \r\n");
        //            sql.Append(" vcNum2 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum2"], false) + ", \r\n");
        //            sql.Append(" vcNum3 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum3"], false) + ", \r\n");
        //            sql.Append(" vcNXQF = " + ComFunction.getSqlValue(dt.Rows[i]["vcNXQF"], false) + ", \r\n");
        //            sql.Append(" dTimeFrom = " + ComFunction.getSqlValue(dt.Rows[i]["dTimeFrom"], true) + ", \r\n");
        //            sql.Append(" vcDY = " + ComFunction.getSqlValue(dt.Rows[i]["vcDY"], false) + ", \r\n");
        //            sql.Append(" vcNum11=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum11"], false) + ", \r\n");
        //            sql.Append(" vcNum12=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum12"], false) + ", \r\n");
        //            sql.Append(" vcNum13=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum13"], false) + ", \r\n");
        //            sql.Append(" vcNum14=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum14"], false) + ", \r\n");
        //            sql.Append(" vcNum15=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum15"], false) + ", \r\n");
        //            sql.Append(" vcNum16=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum16"], false) + ", \r\n");
        //            sql.Append(" vcNum17=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum17"], false) + ", \r\n");
        //            sql.Append(" vcNum18=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum18"], false) + ", \r\n");
        //            sql.Append(" vcNum19=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum19"], false) + ", \r\n");
        //            sql.Append(" vcNum20=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum20"], false) + ", \r\n");
        //            sql.Append(" vcNum21=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum21"], false) + ", \r\n");
        //            sql.Append(" vcOperatorID='" + strUserId + "', \r\n");
        //            sql.Append(" dOperatorTime = GETDATE() \r\n");
        //            sql.Append(" WHERE \r\n");
        //            sql.Append(" vcYear = " + ComFunction.getSqlValue(dt.Rows[i]["vcYear"], false) + " \r\n");
        //            sql.Append(" AND vcPart_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + " \r\n");
        //            sql.Append(" AND vcCarTypeDev = " + ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeDev"], false) + " \r\n");
        //            sql.Append(" AND vcSupplier_id =" + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false) + " \r\n");
        //            sql.Append(" AND vcSYTCode = '" + getValue("C016", dt.Rows[i]["vcSYTCode"].ToString()) + "' \r\n");
        //            sql.Append(" AND vcOriginCompany = '" + getValue("C006", dt.Rows[i]["vcOriginCompany"].ToString()) + "' \r\n");
        //            sql.Append(" AND vcReceiver = '" + getValue("C005", dt.Rows[i]["vcReceiver"].ToString()) + "' \r\n");
        //            sql.Append(" AND vcInOutflag = '" + getValue("C003", dt.Rows[i]["vcInOutflag"].ToString()) + "' \r\n");
        //            if (!vcFinish.Equals("对象外"))
        //            {
        //                sql.Append(" AND vcFinish = '" + getValue("C024", vcFinish) + "' \r\n");
        //            }


        //        }
        //        if (sql.Length > 0)
        //        {
        //            excute.ExcuteSqlWithStringOper(sql.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public void SaveApi(List<Dictionary<string, Object>> list, string strUserId)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            string vcFinish = list[i]["vcFinish"].ToString();
        //            int iAutoId = Convert.ToInt32(list[i]["iAuto_id"]);
        //            sql.Append(" UPDATE TOldYearManager SET \r\n");
        //            if (vcFinish.Equals("对象外"))
        //            {
        //                sql.Append(" vcFinish = '" + getValue("C024", vcFinish) + "', \r\n");
        //                sql.Append(" dFinishYMD = GETDATE(), \r\n");
        //            }
        //            sql.Append(" vcNum1 = " + ComFunction.getSqlValue(list[i]["vcNum1"], false) + ", \r\n");
        //            sql.Append(" vcNum2 = " + ComFunction.getSqlValue(list[i]["vcNum2"], false) + ", \r\n");
        //            sql.Append(" vcNum3 = " + ComFunction.getSqlValue(list[i]["vcNum3"], false) + ", \r\n");
        //            sql.Append(" vcNXQF = " + ComFunction.getSqlValue(list[i]["vcNXQF"], false) + ", \r\n");
        //            sql.Append(" dTimeFrom = " + ComFunction.getSqlValue(list[i]["dTimeFrom"], true) + ", \r\n");
        //            sql.Append(" vcDY = " + ComFunction.getSqlValue(list[i]["vcDY"], false) + ", \r\n");
        //            sql.Append(" vcNum11=" + ComFunction.getSqlValue(list[i]["vcNum11"], false) + ", \r\n");
        //            sql.Append(" vcNum12=" + ComFunction.getSqlValue(list[i]["vcNum12"], false) + ", \r\n");
        //            sql.Append(" vcNum13=" + ComFunction.getSqlValue(list[i]["vcNum13"], false) + ", \r\n");
        //            sql.Append(" vcNum14=" + ComFunction.getSqlValue(list[i]["vcNum14"], false) + ", \r\n");
        //            sql.Append(" vcNum15=" + ComFunction.getSqlValue(list[i]["vcNum15"], false) + ", \r\n");
        //            sql.Append(" vcNum16=" + ComFunction.getSqlValue(list[i]["vcNum16"], false) + ", \r\n");
        //            sql.Append(" vcNum17=" + ComFunction.getSqlValue(list[i]["vcNum17"], false) + ", \r\n");
        //            sql.Append(" vcNum18=" + ComFunction.getSqlValue(list[i]["vcNum18"], false) + ", \r\n");
        //            sql.Append(" vcNum19=" + ComFunction.getSqlValue(list[i]["vcNum19"], false) + ", \r\n");
        //            sql.Append(" vcNum20=" + ComFunction.getSqlValue(list[i]["vcNum20"], false) + ", \r\n");
        //            sql.Append(" vcNum21=" + ComFunction.getSqlValue(list[i]["vcNum21"], false) + ", \r\n");
        //            sql.Append(" vcOperatorID='" + strUserId + "', \r\n");
        //            sql.Append(" dOperatorTime = GETDATE() \r\n");
        //            sql.Append(" WHERE \r\n");
        //            sql.Append(" iAuto_Id = " + iAutoId + " \r\n");


        //        }
        //        if (sql.Length > 0)
        //        {
        //            excute.ExcuteSqlWithStringOper(sql.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //#endregion

    }
}