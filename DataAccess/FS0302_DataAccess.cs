using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0302_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getFinishState()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }
        public DataTable getChange()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcName FROM TCode WHERE vcCodeId = 'C015' \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }

        public DataTable getData(string fileNameTJ)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT a.iAutoId,'' AS selected,a.vcSPINo,a.vcPart_Id_old,a.vcPart_Id_new,b.vcName as FinishState,e.vcName AS vcUnit,a.iDiff,a.vcCarType, \r\n");
                sbr.Append(" d.vcName AS THChange,c.vcName AS vcDD,a.vcRemark,a.vcChange,a.vcBJDiff, \r\n");
                sbr.Append(" CASE WHEN (ISNULL(a.vcDTDiff,'') = '' and ISNULL(a.vcPart_id_DT,'')= '') THEN ''  \r\n");
                sbr.Append(" WHEN (ISNULL(a.vcDTDiff,'') <> '' AND  ISNULL(a.vcPart_id_DT,'') <> '') THEN a.vcDTDiff+'/'+a.vcPart_id_DT  \r\n");
                sbr.Append(" WHEN ISNULL(a.vcDTDiff,'') <> '' THEN a.vcDTDiff WHEN ISNULL(a.vcPart_id_DT,'') <> '' THEN a.vcPart_id_DT END AS vcDT, \r\n");
                sbr.Append(" a.vcPartName,a.vcStartYearMonth,a.vcFXDiff,a.vcFXNo,a.vcOldProj,a.vcOldProjTime,a.vcNewProj, \r\n");
                sbr.Append(" a.vcNewProjTime,a.vcCZYD,a.dHandleTime,a.vcSheetName,a.vcFileName  \r\n");
                sbr.Append(" FROM \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT iAutoId,vcSPINo,vcPart_Id_old,vcPart_Id_new,iFinishState,iUnit,iDiff,vcCarType,iTHChange, \r\n");
                sbr.Append(" vcRemark,vcChange,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff, \r\n");
                sbr.Append(" vcFXNo,vcOldProj,vcOldProjTime,vcNewProj,vcNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName \r\n");
                sbr.Append(" FROM TSBManager WHERE vcFileNameTJ = '" + fileNameTJ + "' \r\n");
                sbr.Append(" ) a \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C014' \r\n");
                sbr.Append(" ) b ON a.iFinishState = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C009' \r\n");
                sbr.Append(" ) c ON a.vcCarType = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C015' \r\n");
                sbr.Append(" ) d ON a.iTHChange = d.vcValue \r\n");
                sbr.Append(" LEFT JOIN  \r\n");
                sbr.Append(" ( \r\n");
                sbr.Append(" SELECT vcValue,vcName FROM TCode WHERE vcCodeId = 'C006' \r\n");
                sbr.Append(" ) e ON a.iUnit = e.vcValue \r\n");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}