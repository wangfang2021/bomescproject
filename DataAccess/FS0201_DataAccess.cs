using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common;

namespace DataAccess
{
    public class FS0201_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索SPI

        public DataTable searchSPI(string vcSPINO, string vcPart_Id, string vcCarType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT iAutoId,vcSPINo,vcPart_Id_old,vcPart_Id_new,vcBJDiff, ");
                sbr.Append(" vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth, ");
                sbr.Append(" vcFXDiff,vcFXNo,vcChange,vcOldProj,vcOldProjTime,vcNewProj, ");
                sbr.Append(" vcNewProjTime,vcCZYD,dHandleTime,vcSheetName, ");
                sbr.Append(" vcFileName FROM TSPIList ");
                sbr.Append("  WHERE vcSPINo LIKE '" + vcSPINO + "%' AND (vcPart_Id_old LIKE '" + vcPart_Id + "%' OR vcPart_Id_new LIKE '" + vcPart_Id + "%') AND SUBSTRING(vcSPINo,1,4) LIKE '" + vcCarType + "%' ");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 导入SPI
        public void addSPI(DataTable dt, string userId)
        {
            //存储SPI
            StringBuilder sbr = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string vcSPINo = dt.Rows[i]["vcSPINo"].ToString().Trim();
                string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"].ToString().Replace("-", "").Trim();
                string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"].ToString().Replace("-", "").Trim();
                string vcBJDiff = dt.Rows[i]["vcBJDiff"].ToString();
                string vcDTDiff = dt.Rows[i]["vcDTDiff"].ToString();
                string vcPart_id_DT = dt.Rows[i]["vcPart_id_DT"].ToString();
                string vcPartName = dt.Rows[i]["vcPartName"].ToString();
                string vcStartYearMonth = dt.Rows[i]["vcStartYearMonth"].ToString().Replace(@"/", "").Trim();
                string vcFXDiff = dt.Rows[i]["vcFXDiff"].ToString();
                string vcFXNo = dt.Rows[i]["vcFXNo"].ToString();
                string vcChange = dt.Rows[i]["vcChange"].ToString();
                string vcOldProj = dt.Rows[i]["vcOldProj"].ToString();
                string vcOldProjTime = dt.Rows[i]["vcOldProjTime"].ToString();
                string vcNewProj = dt.Rows[i]["vcNewProj"].ToString();
                string vcNewProjTime = dt.Rows[i]["vcNewProjTime"].ToString();
                string vcCZYD = dt.Rows[i]["vcCZYD"].ToString();
                string dHandleTime = dt.Rows[i]["dHandleTime"].ToString();
                string vcSheetName = dt.Rows[i]["vcSheetName"].ToString();
                string vcFileName = dt.Rows[i]["vcFileName"].ToString();

                sbr.Append(" INSERT INTO TSPIList(vcSPINo,vcPart_Id_old,vcPart_Id_new,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff,vcFXNo,vcChange,vcOldProj,vcOldProjTime,vcNewProj,vcNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName,vcOperatorId,dOperatorTime) \r\n");
                sbr.Append(" VALUES( '" + vcSPINo + "','" + vcPart_Id_old + "','" + vcPart_Id_new + "','" + vcBJDiff + "','" + vcDTDiff + "','" + vcPart_id_DT + "','" + vcPartName + "','" + vcStartYearMonth + "','" + vcFXDiff + "','" + vcFXNo + "','" + vcChange + "','" + vcOldProj + "','" + vcOldProjTime + "','" + vcNewProj + "','" + vcNewProjTime + "','" + vcCZYD + "','" + dHandleTime + "','" + vcSheetName + "','" + vcFileName + "','" + userId + "',GETDATE()) \r\n");

                if (i % 1000 == 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                    sbr.Length = 0;
                }
            }
            if (sbr.Length > 0)
            {
                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }

            //保存履历
            List<string> fileList = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                fileList.Add(dt.Rows[i]["vcFileName"].ToString());
            }
            fileList = fileList.Distinct().ToList();
            StringBuilder sbrList = new StringBuilder();
            for (int i = 0; i < fileList.Count; i++)
            {
                sbrList.Append(" INSERT INTO TSPIHistory (vcFileName,vcRemark,vcType,iState,vcOperatorID,dOperatorTime) \r\n");
                sbrList.Append(" VALUES ('" + fileList[i].ToString() + "','','0',-2,'" + userId + "',GETDATE()) \r\n");
            }
            excute.ExcuteSqlWithStringOper(sbrList.ToString());
        }
        #endregion
    }
}