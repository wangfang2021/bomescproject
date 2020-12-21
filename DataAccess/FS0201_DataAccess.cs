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
                    sbrList.Append(" INSERT INTO TSPIHistory (vcFileName,vcRemark,vcType,iState,vcOperatorID,dOperatorTime) \r\n");
                    sbrList.Append(" VALUES ('" + fileList[i].ToString() + "','','0',-2,'" + userId + "',GETDATE()) \r\n");
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
                    sbr.Append(" INSERT INTO TSBManager (vcSPINo,vcPart_Id_old,vcPart_Id_new,iFinishState,vcCarType,vcChange,vcBJDiff,vcDTDiff,vcPart_id_DT,vcPartName,vcStartYearMonth,vcFXDiff,vcFXNo,vcOldProj,vcOldProjTime,vcNewProj,vcNewProjTime,vcCZYD,dHandleTime,vcSheetName,vcFileName,vcFileNameTJ,vcOperatorId,dOperatorTime) values ");
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
                //匹配原单位
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
                sbr.Append(" UPDATE TSBManager SET iDiff = b.iDiff,iUnit = b.iUnit \r\n");
                sbr.Append(" from TSBManager a  \r\n");
                sbr.Append(" LEFT JOIN TUnit B ON a.vcPart_Id_old = b.vcPart_id OR a.vcPart_Id_new = b.vcPart_id \r\n");
                sbr.Append(" WHERE a.vcFileNameTJ IN (" + list + ") \r\n");
                excute.ExcuteSqlWithStringOper(sbr.ToString());

                //记录文件名
                sbr.Length = 0;
                for (int i = 0; i < fileList.Count; i++)
                {
                    sbr.Append(" INSERT INTO dbo.TSBFile (vcFileNameTJ,iState,vcOperatorId,dOperatorTime)  VALUES ('" + fileList[i] + "',0,'" + userId + "',GETDATE()) \r\n");
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
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcSPINo"], false) + ", \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + "," + getSqlValue(listInfoData[i]["vcPart_Id_new"], false) + ", \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcBJDiff"], false) + ", " + getSqlValue(listInfoData[i]["vcDTDiff"], false) + ",  \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcPart_id_DT"], false) + ", " + getSqlValue(listInfoData[i]["vcPartName"], false) + " \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + "," + getSqlValue(listInfoData[i]["vcFXDiff"], false) + ", \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcFXNo"], false) + ", " + getSqlValue(listInfoData[i]["vcChange"], false) + ", \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcOldProj"], false) + ", " + getSqlValue(listInfoData[i]["vcOldProjTime"], false) + ", \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcNewProj"], false) + ",  " + getSqlValue(listInfoData[i]["vcNewProjTime"], false) + ", \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcCZYD"], false) + ", " + getSqlValue(listInfoData[i]["dHandleTime"], true) + ",  \r\n");
                        sbr.Append(" " + getSqlValue(listInfoData[i]["vcSheetName"], false) + "," + getSqlValue(listInfoData[i]["vcFileName"], false) + ",  \r\n");
                        sbr.Append("  '" + strUserId + "' ,GETDATE() ) \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sbr.Append(" UPDATE TSPIList SET   \r\n");
                        sbr.Append(" vcPart_Id_old = " + getSqlValue(listInfoData[i]["vcPart_Id_old"], false) + ",vcPart_Id_new = " + getSqlValue(listInfoData[i]["vcPart_Id_new"], false) + ",  \r\n");
                        sbr.Append(" vcBJDiff = " + getSqlValue(listInfoData[i]["vcBJDiff"], false) + ",vcDTDiff = " + getSqlValue(listInfoData[i]["vcDTDiff"], false) + ",  \r\n");
                        sbr.Append(" vcPart_id_DT = " + getSqlValue(listInfoData[i]["vcPart_id_DT"], false) + ",vcPartName = " + getSqlValue(listInfoData[i]["vcPartName"], false) + ",  \r\n");
                        sbr.Append(" vcStartYearMonth = " + getSqlValue(listInfoData[i]["vcStartYearMonth"], false) + ",vcFXDiff = " + getSqlValue(listInfoData[i]["vcFXDiff"], false) + ",  \r\n");
                        sbr.Append(" vcFXNo = " + getSqlValue(listInfoData[i]["vcFXNo"], false) + ",vcChange = " + getSqlValue(listInfoData[i]["vcChange"], false) + ",  \r\n");
                        sbr.Append(" vcOldProj = " + getSqlValue(listInfoData[i]["vcOldProj"], false) + ",vcOldProjTime = " + getSqlValue(listInfoData[i]["vcOldProjTime"], false) + ",  \r\n");
                        sbr.Append(" vcNewProj = " + getSqlValue(listInfoData[i]["vcNewProj"], false) + ",vcNewProjTime = " + getSqlValue(listInfoData[i]["vcNewProjTime"], false) + ",  \r\n");
                        sbr.Append(" vcCZYD = " + getSqlValue(listInfoData[i]["vcCZYD"], false) + ",dHandleTime = " + getSqlValue(listInfoData[i]["dHandleTime"], true) + ",  \r\n");
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

        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion
    }
}