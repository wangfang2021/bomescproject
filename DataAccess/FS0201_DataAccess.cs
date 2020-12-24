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
                sbr.Append(" vcFileName,'0' as vcModFlag,'0' as vcAddFlag FROM TSPIList ");
                sbr.Append("  WHERE 1=1  ");
                if (!string.IsNullOrWhiteSpace(vcSPINO))
                {
                    sbr.Append(" AND isnull(vcSPINo,'') LIKE '" + vcSPINO.Trim() + "%' ");
                }

                if (!string.IsNullOrWhiteSpace(vcPart_Id))
                {
                    sbr.Append(" AND (isnull(vcPart_Id_old,'') LIKE '" + vcPart_Id.Trim() + "%' OR isnull(vcPart_Id_new,'') LIKE '" + vcPart_Id + "%')");
                }

                if (!string.IsNullOrWhiteSpace(vcCarType))
                {
                    sbr.Append(" AND SUBSTRING(vcSPINo,1,4) LIKE '" + vcCarType.Trim() + "%' ");
                }

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入SPI
        public void importSPI(DataTable dt, string userId)
        {
            try
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
                    sbrList.Append(" INSERT INTO TSPIHistory (vcFileName,vcRemark,vcType,vcOperatorID,dOperatorTime) \r\n");
                    sbrList.Append(" VALUES ('" + fileList[i].ToString() + "','','0','" + userId + "',GETDATE()) \r\n");
                }
                excute.ExcuteSqlWithStringOper(sbrList.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 传送至设变管理
        //1.将spi表传到设变表
        //2.匹配原单位
        //3.记录文件名
        //4.清空SPI
        public void transferSB(DataTable dt, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                List<string> fileList = new List<string>();
                //将spi表传到设变表
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcSPINo = dt.Rows[i]["vcSPINo"].ToString();
                    string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"].ToString();
                    string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"].ToString();
                    string vcCarType = dt.Rows[i]["vcCarType"].ToString();
                    string vcChange = dt.Rows[i]["vcChange"].ToString();
                    string vcBJDiff = dt.Rows[i]["vcBJDiff"].ToString();
                    string vcDTDiff = dt.Rows[i]["vcDTDiff"].ToString();
                    string vcPart_id_DT = dt.Rows[i]["vcPart_id_DT"].ToString();
                    string vcPartName = dt.Rows[i]["vcPartName"].ToString();
                    string vcStartYearMonth = dt.Rows[i]["vcStartYearMonth"].ToString();
                    string vcFXDiff = dt.Rows[i]["vcFXDiff"].ToString();
                    string vcFXNo = dt.Rows[i]["vcFXNo"].ToString();
                    string vcOldProj = dt.Rows[i]["vcOldProj"].ToString();
                    string vcOldProjTime = dt.Rows[i]["vcOldProjTime"].ToString();
                    string vcNewProj = dt.Rows[i]["vcNewProj"].ToString();
                    string vcNewProjTime = dt.Rows[i]["vcNewProjTime"].ToString();
                    string vcCZYD = dt.Rows[i]["vcCZYD"].ToString();
                    string dHandleTime = dt.Rows[i]["dHandleTime"].ToString();
                    string vcSheetName = dt.Rows[i]["vcSheetName"].ToString();
                    string vcFileName = dt.Rows[i]["vcFileName"].ToString();
                    string vcFileNameTJ = dt.Rows[i]["vcFileNameTJ"].ToString();
                    sbr.Append(" INSERT INTO TSBManager (vcSPINo,vcPart_Id_old,vcPart_Id_new,vcFinishState,vcCarType,vcChange,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff,vcFXNo,vcOldProj,vcOldProjTime,vcNewProj,vcNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName,vcFileNameTJ,vcOperatorId,dOperatorTime) values ");
                    sbr.Append(" ('" + vcSPINo + "','" + vcPart_Id_old + "','" + vcPart_Id_new + "',0,'" + vcCarType + "','" + vcChange + "','" + vcBJDiff + "','" + vcDTDiff + "','" + vcPart_id_DT + "','" + vcPartName + "','" + vcStartYearMonth + "','" + vcFXDiff + "','" + vcFXNo + "','" + vcOldProj + "','" + vcOldProjTime + "','" + vcNewProj + "','" + vcNewProjTime + "','" + vcCZYD + "','" + dHandleTime + "','" + vcSheetName + "','" + vcFileName + "','" + vcFileNameTJ + "','" + userId + "',GETDATE() ) \r\n");

                    fileList.Add(vcFileNameTJ);
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

                fileList = fileList.Distinct().ToList();

                //TODO
                ////匹配原单位
                string list = "";
                foreach (string fileName in fileList)
                {
                    if (!string.IsNullOrWhiteSpace(list))
                    {
                        list = list + ",";
                    }

                    list = list + "'" + fileName + "'";
                }
                sbr.Length = 0;
                sbr.Append(" UPDATE TSBManager SET vcDiff = b.vcDiff,vcOriginCompany = b.vcOriginCompany \r\n");
                sbr.Append(" from TSBManager a  \r\n");
                sbr.Append(" LEFT JOIN TUnit B ON a.vcPart_Id_old = b.vcPart_id OR a.vcPart_Id_new = b.vcPart_id \r\n");
                sbr.Append(" WHERE a.vcFileNameTJ IN (" + list + ") \r\n");
                excute.ExcuteSqlWithStringOper(sbr.ToString());

                //记录文件名
                sbr.Length = 0;
                for (int i = 0; i < fileList.Count; i++)
                {
                    sbr.Append(" INSERT INTO dbo.TSBFile (vcFileNameTJ,vcState,vcOperatorId,dOperatorTime)  VALUES ('" + fileList[i] + "',0,'" + userId + "',GETDATE()) \r\n");
                }
                excute.ExcuteSqlWithStringOper(sbr.ToString());

                //清空SPI
                //sbr.Length = 0;
                //sbr.Append(" TRUNCATE TABLE TSPIList\r\n");
                //excute.ExcuteSqlWithStringOper(sbr.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region 保存SPI
        public void saveSPI(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {
                        sbr.Append(" INSERT	INTO TSPIList (vcSPINo,vcPart_Id_old,vcPart_Id_new,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth, \r\n");
                        sbr.Append(" vcFXDiff,vcFXNo,vcChange,vcOldProj,vcOldProjTime,vcNewProj,vcNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName,vcOperatorId,dOperatorTime) \r\n");
                        sbr.Append(" VALUES ( \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + ", \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + "," + ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_new"], false) + ", \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ", " + ComFunction.getSqlValue(listInfoData[i]["vcDTDiff"], false) + ",  \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id_DT"], false) + ", " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + " \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + "," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ", \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ", " + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ", \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcOldProj"], false) + ", " + ComFunction.getSqlValue(listInfoData[i]["vcOldProjTime"], false) + ", \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcNewProj"], false) + ",  " + ComFunction.getSqlValue(listInfoData[i]["vcNewProjTime"], false) + ", \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcCZYD"], false) + ", " + ComFunction.getSqlValue(listInfoData[i]["dHandleTime"], true) + ",  \r\n");
                        sbr.Append(" " + ComFunction.getSqlValue(listInfoData[i]["vcSheetName"], false) + "," + ComFunction.getSqlValue(listInfoData[i]["vcFileName"], false) + ",  \r\n");
                        sbr.Append("  '" + strUserId + "' ,GETDATE() ) \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sbr.Append(" UPDATE TSPIList SET   \r\n");
                        sbr.Append(" vcPart_Id_old = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + ",vcPart_Id_new = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_new"], false) + ",  \r\n");
                        sbr.Append(" vcBJDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ",vcDTDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcDTDiff"], false) + ",  \r\n");
                        sbr.Append(" vcPart_id_DT = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id_DT"], false) + ",vcPartName = " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + ",  \r\n");
                        sbr.Append(" vcStartYearMonth = " + ComFunction.getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + ",vcFXDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ",  \r\n");
                        sbr.Append(" vcFXNo = " + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ",vcChange = " + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ",  \r\n");
                        sbr.Append(" vcOldProj = " + ComFunction.getSqlValue(listInfoData[i]["vcOldProj"], false) + ",vcOldProjTime = " + ComFunction.getSqlValue(listInfoData[i]["vcOldProjTime"], false) + ",  \r\n");
                        sbr.Append(" vcNewProj = " + ComFunction.getSqlValue(listInfoData[i]["vcNewProj"], false) + ",vcNewProjTime = " + ComFunction.getSqlValue(listInfoData[i]["vcNewProjTime"], false) + ",  \r\n");
                        sbr.Append(" vcCZYD = " + ComFunction.getSqlValue(listInfoData[i]["vcCZYD"], false) + ",dHandleTime = " + ComFunction.getSqlValue(listInfoData[i]["dHandleTime"], true) + ",  \r\n");
                        sbr.Append(" vcOperatorId = '" + strUserId + "',dOperatorTime  = GETDATE()  \r\n");
                        sbr.Append(" where iAutoId = '" + iAutoId + "' \r\n");
                    }

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除SPI
        public void delSPI(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" DELETE TSPIList WHERE iAutoId IN (  \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
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

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sbr.Append(" INSERT INTO TSPIList (vcSPINo,vcPart_Id_old,vcPart_Id_new,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff,vcFXNo,vcChange,vcOldProj,vcOldProjTime,vcNewProj,vcNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName,vcOperatorId,dOperatorTime)  \r\n ");
                        sbr.Append(" values ( \r\n");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_new"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcDTDiff"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id_DT"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcOldProj"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcOldProjTime"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcNewProj"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcNewProjTime"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcCZYD"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["dHandleTime"], true) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcSheetName"], false) + ",");
                        sbr.Append(ComFunction.getSqlValue(listInfoData[i]["vcFileName"], false) + ",");
                        sbr.Append(" '" + strUserId + "',");
                        sbr.Append(" GETDATE() ");
                        sbr.Append(" ) \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sbr.Append(" UPDATE TSPIList SET \r\n");
                        sbr.Append(" vcPart_Id_old   = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + ", ");
                        sbr.Append(" vcPart_Id_new  = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_Id_new"], false) + ", ");
                        sbr.Append(" vcBJDiff  = " + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + ", ");
                        sbr.Append(" vcDTDiff  = " + ComFunction.getSqlValue(listInfoData[i]["vcDTDiff"], false) + ", ");
                        sbr.Append(" vcPart_id_DT  = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id_DT"], false) + ", ");
                        sbr.Append(" vcPartName  = " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + ", ");
                        sbr.Append(" vcStartYearMonth  = " + ComFunction.getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + ", ");
                        sbr.Append(" vcFXDiff  = " + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + ", ");
                        sbr.Append(" vcFXNo  = " + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + ", ");
                        sbr.Append(" vcChange  = " + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + ", ");
                        sbr.Append(" vcOldProj  = " + ComFunction.getSqlValue(listInfoData[i]["vcOldProj"], false) + ", ");
                        sbr.Append(" vcOldProjTime  = " + ComFunction.getSqlValue(listInfoData[i]["vcOldProjTime"], false) + ", ");
                        sbr.Append(" vcNewProj  = " + ComFunction.getSqlValue(listInfoData[i]["vcNewProj"], false) + ", ");
                        sbr.Append(" vcNewProjTime  = " + ComFunction.getSqlValue(listInfoData[i]["vcNewProjTime"], false) + ", ");
                        sbr.Append(" vcCZYD  = " + ComFunction.getSqlValue(listInfoData[i]["vcCZYD"], false) + ", ");
                        sbr.Append(" dHandleTime  = " + ComFunction.getSqlValue(listInfoData[i]["dHandleTime"], true) + ", ");
                        sbr.Append(" vcSheetName  = " + ComFunction.getSqlValue(listInfoData[i]["vcSheetName"], false) + ", ");
                        sbr.Append(" vcFileName  = " + ComFunction.getSqlValue(listInfoData[i]["vcFileName"], false) + ", ");
                        sbr.Append(" vcOperatorId  = '" + strUserId + "', ");
                        sbr.Append(" dOperatorTime  = GETDATE() ");
                        sbr.Append(" where iAutoId = '" + iAutoId + "' \r\n");
                    }
                }
                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
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

                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcSPINo = dt.Rows[i]["vcSPINo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSPINo"].ToString();
                    string vcPart_Id_old = dt.Rows[i]["vcPart_Id_old"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_Id_old"].ToString();
                    string vcPart_Id_new = dt.Rows[i]["vcPart_Id_new"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPart_Id_new"].ToString();

                    sbr.Append(" UPDATE TSPIList SET \r\n");
                    sbr.Append(" vcPart_Id_old   = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_Id_old"], false) + ", ");
                    sbr.Append(" vcPart_Id_new  = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_Id_new"], false) + ", ");
                    sbr.Append(" vcBJDiff  = " + ComFunction.getSqlValue(dt.Rows[i]["vcBJDiff"], false) + ", ");
                    sbr.Append(" vcDTDiff  = " + ComFunction.getSqlValue(dt.Rows[i]["vcDTDiff"], false) + ", ");
                    sbr.Append(" vcPart_id_DT  = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id_DT"], false) + ", ");
                    sbr.Append(" vcPartName  = " + ComFunction.getSqlValue(dt.Rows[i]["vcPartName"], false) + ", ");
                    sbr.Append(" vcStartYearMonth  = " + ComFunction.getSqlValue(dt.Rows[i]["vcStartYearMonth"], false) + ", ");
                    sbr.Append(" vcFXDiff  = " + ComFunction.getSqlValue(dt.Rows[i]["vcFXDiff"], false) + ", ");
                    sbr.Append(" vcFXNo  = " + ComFunction.getSqlValue(dt.Rows[i]["vcFXNo"], false) + ", ");
                    sbr.Append(" vcChange  = " + ComFunction.getSqlValue(dt.Rows[i]["vcChange"], false) + ", ");
                    sbr.Append(" vcOldProj  = " + ComFunction.getSqlValue(dt.Rows[i]["vcOldProj"], false) + ", ");
                    sbr.Append(" vcOldProjTime  = " + ComFunction.getSqlValue(dt.Rows[i]["vcOldProjTime"], false) + ", ");
                    sbr.Append(" vcNewProj  = " + ComFunction.getSqlValue(dt.Rows[i]["vcNewProj"], false) + ", ");
                    sbr.Append(" vcNewProjTime  = " + ComFunction.getSqlValue(dt.Rows[i]["vcNewProjTime"], false) + ", ");
                    sbr.Append(" vcCZYD  = " + ComFunction.getSqlValue(dt.Rows[i]["vcCZYD"], false) + ", ");
                    sbr.Append(" dHandleTime  = " + ComFunction.getSqlValue(dt.Rows[i]["dHandleTime"], true) + ", ");
                    sbr.Append(" vcSheetName  = " + ComFunction.getSqlValue(dt.Rows[i]["vcSheetName"], false) + ", ");
                    sbr.Append(" vcFileName  = " + ComFunction.getSqlValue(dt.Rows[i]["vcFileName"], false) + ", ");
                    sbr.Append(" vcOperatorId  = '" + strUserId + "', ");
                    sbr.Append(" dOperatorTime  = GETDATE() ");
                    sbr.Append(" where  vcSPINo = '" + vcSPINo + "' AND (vcPart_Id_old = '" + vcPart_Id_old + "' OR vcPart_Id_new = '" + vcPart_Id_new + "') \r\n");

                }
                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
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