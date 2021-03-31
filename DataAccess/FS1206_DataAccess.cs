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
    public class FS1206_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string strPartsNo, string mon)
        {
            try
            {
                StringBuilder ssql = new StringBuilder();
                ssql.AppendLine(" select t1.vcPartsNo,vcPartsNoFZ,vcSource,vcFactory,vcBF,convert(int,iSRNum) iSRNum,");
                ssql.AppendLine("isnull(t2.iCONum,0) as iCONum, '0' as iFlag,'0' as vcModFlag,'0' as vcAddFlag,iAutoId from tSSPMaster t1");
                ssql.AppendLine(" left join (");
                ssql.AppendLine("		select distinct C.vcPartsNo,C.iCONum from tSSP C");
                ssql.AppendLine("		inner join (	");
                ssql.AppendLine("			select vcPartsNo,MAX(iAutoId) as iAutoId from tSSP");
                ssql.AppendLine("			group by vcPartsNo");
                ssql.AppendLine("		) D on C.vcPartsNo=D.vcPartsNo and C.iAutoId=D.iAutoId");
                ssql.AppendLine(") t2");
                ssql.AppendLine("on t1.vcPartsNo=t2.vcPartsNo");
                if (!string.IsNullOrEmpty(strPartsNo))
                {
                    ssql.AppendLine("where t1.vcPartsNo like '" + strPartsNo + "%'");
                }
                ssql.AppendLine("order by t1.vcPartsNo");
                DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM tSSPMaster where iAutoId in(   \r\n ");
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
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        bool b = isExists(listInfoData[i]["vcPartsNo"] != null ? listInfoData[i]["vcPartsNo"].ToString() : "");
                        if (b)
                        {
                            strErrorPartId = "第" + (i + 1).ToString() + "行，品番已存在！";
                            return;
                        }
                        sql.Append("insert into tSSPMaster(vcPartsNo, vcPartsNoFZ, vcSource, vcFactory, vcBF, iSRNum, vcCreateUser, dCreateTime)  \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartsNo"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPartsNoFZ"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcSource"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcFactory"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcBF"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["iSRNum"], false) + ",  \r\n");
                        sql.Append("'" + strUserId + "',  \r\n");
                        sql.Append("getdate()  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update tSSPMaster set    \r\n");
                        sql.Append("  vcPartsNoFZ=" + ComFunction.getSqlValue(listInfoData[i]["vcPartsNoFZ"], false) + "   \r\n");
                        sql.Append("  ,vcSource=" + ComFunction.getSqlValue(listInfoData[i]["vcSource"], false) + "   \r\n");
                        sql.Append("  ,vcFactory=" + ComFunction.getSqlValue(listInfoData[i]["vcFactory"], true) + "   \r\n");
                        sql.Append("  ,vcBF=" + ComFunction.getSqlValue(listInfoData[i]["vcBF"], true) + "   \r\n");
                        sql.Append("  ,iSRNum=" + ComFunction.getSqlValue(listInfoData[i]["iSRNum"], true) + "   \r\n");
                        sql.Append("  ,vcUpdateUser='" + strUserId + "'  \r\n");
                        sql.Append("  ,dUpdateTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");

                        sql.Append("  update TSSP  \r\n");
                        sql.Append("  set iCONum= " + listInfoData[i]["iCONum"] + "  \r\n");
                        sql.Append("  where iAutoId=(select top 1 iAutoId from TSSP  \r\n");
                        sql.Append("  where vcPartsNo=" + ComFunction.getSqlValue(listInfoData[i]["vcPartsNo"], false) + " order by vcMonth desc) \r\n");
                    }
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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

        public bool isExists(string vcPartsNo)
        {
            try
            {
                string ssql = "select count(*) from tSSPMaster where vcPartsNo='" + vcPartsNo + "'";
                if (excute.ExecuteScalar(ssql) > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 导入后保存
        public string UpdateTable(DataTable dt, string user)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                string vcCLYM = DateTime.Now.ToString("yyyy-MM");
                DataTable dttmp = new DataTable();
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                //cmd.Connection.BeginTransaction();
                cmd.CommandText = "select vcPartsNo,vcPartsNoFZ,vcSource,vcFactory,vcBF,iSRNum,vcCreateUser,dCreateTime,vcUpdateUser,dUpdateTime from tSSPMaster";

                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                apt.Fill(dttmp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dttmp.Rows.Add(
                                    dt.Rows[i][0].ToString(),
                                    dt.Rows[i][1].ToString(),
                                    dt.Rows[i][2].ToString().ToUpper(),
                                    dt.Rows[i][3].ToString().ToUpper(),
                                    dt.Rows[i][4].ToString(),
                                    dt.Rows[i][5].ToString(),
                                    user,
                                    DateTime.Now,
                                    null,
                                    null
                                   );
                    StringBuilder sb = new StringBuilder();
                    sb.Append("insert into TSSP (vcMonth, vcPartsNo, Total, iXZNum, iFZNum, iCO, iCONum, iFZFlg, Creater, dCreatDate)");
                    sb.Append("values ('" + vcCLYM + "','" + dt.Rows[i][0].ToString() + "',0,0,0,0," + Convert.ToInt32(dt.Rows[i]["iCONum"].ToString()) + ",0,'" + user + "','" + DateTime.Now.ToString() + "');");
                    excute.ExecuteSQLNoQuery(sb.ToString());
                }
                SqlCommandBuilder cmdbuild = new SqlCommandBuilder(apt);
                //apt.UpdateCommand = cmdbuild.GetUpdateCommand();
                apt.Update(dttmp);
            }
            catch (Exception ex)
            {
                //cmd.Transaction.Rollback();
                cmd.Connection.Close();
                return "更新失败！";
            }
            cmd.Dispose();
            return "";

        }
        #endregion

        public string InUpdeOldData(DataTable dt, string useid)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                DataRow[] rows = dt.Select("iFlag='1'or iFlag='2' or iFlag='3'");
                string msg = "";
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["vcPartsNo"].ToString().Length <= 0)
                        {
                            msg = "品番信息不能为空";
                            return msg;
                        }
                        if (dt.Rows[i]["vcPartsNoFZ"].ToString().Length <= 0)
                        {
                            msg = "发注品番信息不能为空";
                            return msg;
                        }
                        if (dt.Rows[i]["vcSource"].ToString().Length <= 0)
                        {
                            msg = "SOURCE信息不能为空";
                            return msg;
                        }
                        if (dt.Rows[i]["iSRNum"].ToString().Length <= 0)
                        {
                            msg = "收容数信息不能为空";
                            return msg;
                        }
                    }
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow rowdselect = rows[i];
                        string strSqlIn = "";
                        string strSqlUp = "";//更新语句
                        string strSqlDe = "";//删除语句
                        string vcPartsNo = rowdselect["vcPartsNo"].ToString();
                        string vcPartsNoFZ = rowdselect["vcPartsNoFZ"].ToString();
                        string vcSource = rowdselect["vcSource"].ToString().ToUpper();
                        string vcFactory = rowdselect["vcFactory"].ToString().ToUpper();
                        string vcBF = rowdselect["vcBF"].ToString();
                        string iSRNum = rowdselect["iSRNum"].ToString();
                        string iFlag = rowdselect["iFlag"].ToString();

                        if (iFlag == "1")
                        {
                            string str = "select vcPartsNo,vcPartsNoFZ  from tSSPMaster where vcPartsNo='" + vcPartsNo + "'";
                            DataTable dtExist = excute.ExcuteSqlWithSelectToDT(str);
                            if (dtExist.Rows.Count > 0)
                            {
                                msg = "品番：" + vcPartsNo + " 数据重复";
                                return msg;
                            }
                            strSqlIn = " INSERT INTO [tSSPMaster]";
                            strSqlIn += " ([vcPartsNo]  ,[vcPartsNoFZ] ,[vcSource]  ,[vcFactory] ,[vcBF]  ,[iSRNum] ,[vcCreateUser]  ,[dCreateTime])";
                            strSqlIn += " VALUES ( '" + vcPartsNo + "','" + vcPartsNoFZ + "','" + vcSource + "','" + vcFactory + "','" + vcBF + "','" + iSRNum + "','" + useid + "',getdate())";
                        }

                        else if (iFlag == "2")
                        {
                            strSqlUp = "UPDATE [tSSPMaster]";
                            strSqlUp += "   SET [vcSource] ='" + vcSource + "'";
                            strSqlUp += "      ,[vcPartsNoFZ] ='" + vcPartsNoFZ + "'";
                            strSqlUp += "      ,[vcFactory] ='" + vcFactory + "'";
                            strSqlUp += "      ,[vcBF] ='" + vcBF + "'";
                            strSqlUp += "      ,[iSRNum] ='" + iSRNum + "'";
                            strSqlUp += "      ,[dUpdateTime] =getdate()";
                            strSqlUp += "      ,[vcUpdateUser] ='" + useid + "'";
                            strSqlUp += " WHERE  [vcPartsNo]='" + vcPartsNo + "'";
                        }
                        else if (iFlag == "3")
                        {
                            strSqlDe = "DELETE FROM [tSSPMaster]";
                            strSqlDe += " WHERE  [vcPartsNo]='" + vcPartsNo + "' ";
                        }
                        if (strSqlIn != "")
                        {
                            excute.ExcuteSqlWithStringOper(strSqlIn);
                            //SqlHelper.ExecuteNonQuery(trans, CommandType.Text, );
                        }
                        if (strSqlUp != "")
                        {
                            excute.ExcuteSqlWithStringOper(strSqlUp);
                            //SqlHelper.ExecuteNonQuery(trans, CommandType.Text, strSqlUp);
                        }
                        if (strSqlDe != "")
                        {
                            excute.ExcuteSqlWithStringOper(strSqlDe);
                            //SqlHelper.ExecuteNonQuery(trans, CommandType.Text, strSqlDe);
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                return msg;
            }
        }

    }

}
