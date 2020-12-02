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

        public DataTable Search(string strPartsNo, string mon)
        {
            try
            {
                StringBuilder ssql = new StringBuilder();
                ssql.AppendLine(" select t1.vcPartsNo,vcPartsNoFZ,vcSource,vcFactory,vcBF,iSRNum,");
                ssql.AppendLine("isnull(t2.iCONum,0) as iCONum,'0' as iFlag from tSSPMaster t1");
                ssql.AppendLine(" left join (");
                ssql.AppendLine("		select distinct C.vcPartsNo,C.iCONum from tSSP C");
                ssql.AppendLine("		inner join 	(				");
                ssql.AppendLine("			select vcPartsNo,MAX(vcMonth) as vcMonth from tSSP where vcMonth<='" + mon + "'");
                ssql.AppendLine("			group by vcPartsNo	");
                ssql.AppendLine("		)D on C.vcPartsNo=D.vcPartsNo and C.vcMonth=D.vcMonth");
                ssql.AppendLine(") t2");
                ssql.AppendLine("on t1.vcPartsNo=t2.vcPartsNo");
                if (!string.IsNullOrEmpty(strPartsNo))
                {
                    ssql.AppendLine("where t1.vcPartsNo='" + strPartsNo + "'");
                }
                DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateTable(DataTable dt, string user)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            try
            {
                DataTable dttmp = new DataTable();
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                //cmd.Connection.BeginTransaction();
                cmd.CommandText = "select vcPartsNo,vcPartsNoFZ,vcSource,vcFactory,vcBF,iSRNum,vcCreateUser," +
                    "dCreateTime,vcUpdateUser,dUpdateTime from tSSPMaster";
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                apt.Fill(dttmp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow[] dr = dttmp.Select("vcPartsNo='" + dt.Rows[i][0].ToString() + "' ");
                    if (dr.Length > 0)
                    {
                        dr[0]["vcPartsNoFZ"] = dt.Rows[i][1].ToString();
                        dr[0]["vcSource"] = dt.Rows[i][2].ToString().ToUpper();
                        dr[0]["vcFactory"] = dt.Rows[i][3].ToString().ToUpper();
                        dr[0]["vcBF"] = dt.Rows[i][4].ToString();
                        dr[0]["iSRNum"] = dt.Rows[i][5].ToString();
                        dr[0]["vcUpdateUser"] = user;
                        dr[0]["dUpdateTime"] = DateTime.Now.ToString();
                    }
                    else
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
                    }
                }
                SqlCommandBuilder cmdbuild = new SqlCommandBuilder(apt);
                // apt.UpdateCommand = cmdbuild.GetUpdateCommand();
                apt.Update(dttmp);
            }
            catch
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                return "更新失败！";
            }
            cmd.Dispose();
            return "";
        }
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
