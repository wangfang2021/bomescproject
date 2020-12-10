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



        #endregion

        #region 导入SPI

        public void AddSPI(DataTable dt, string userId)
        {
            //存储SPI
            StringBuilder sbr = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string vcSPINo = dt.Rows[i]["vcSPINo"].ToString();
                string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"].ToString().Replace("-", "");
                string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"].ToString().Replace("-", "");
                string vcBJDiff = dt.Rows[i]["vcBJDiff"].ToString();
                string vcDTDiff = dt.Rows[i]["vcDTDiff"].ToString();
                string vcPart_id_DT = dt.Rows[i]["vcPart_id_DT"].ToString();
                string vcPartName = dt.Rows[i]["vcPartName"].ToString();
                string vcStartYearMonth = dt.Rows[i]["vcStartYearMonth"].ToString();
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
                sbrList.Append(" VALUES ('" + fileList[i].ToString() + "','','0',-2,'" + userId + "',GETDATE() \r\n");
            }
            excute.ExcuteSqlWithStringOper(sbrList.ToString());
        }

        #endregion
    }
}