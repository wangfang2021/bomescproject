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
    public class FS1209_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 

        public string getUserInfo(string ID)
        {
            string ssql = "select vcTips from SUserPorType where vcUserID ='" + ID + "' ";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0] != null ? dt.Rows[0][0].ToString() : string.Empty;
            return string.Empty;
        }

        #region 检索信息栏绑定生产部署 str2是权限部署
        public DataTable dllPorType_Print(string[] str2)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            if (str2.Length != 0)
            {
                if (str2[0] != "admin")
                {
                    string ProType = "";
                    if (str2.Length != 0)
                    {
                        ProType += "'";
                        for (int i = 0; i < str2.Length; i++)
                        {
                            ProType += str2[i].ToString();
                            if (i < str2.Length - 1)
                            {
                                ProType += "','";
                            }
                            else
                            {
                                ProType += "'";
                            }
                        }
                    }
                    strSQL.AppendLine("select '' as [Text],'0' as [Value]   ");
                    strSQL.AppendLine(" union all ");
                    strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType' and [vcData1] in(" + ProType + ") ");
                }
                else
                {
                    strSQL.AppendLine("select '' as [Text],'0' as [Value]   ");
                    strSQL.AppendLine(" union all ");
                    strSQL.AppendLine(" select distinct [vcData1] as [Text],[vcData1] as [Value]  from [ConstMst] where [vcDataId]='ProType'");
                }
            }
            else
            {
                strSQL.AppendLine("select '' as [Text],'PP' as [Value]   ");
            }
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #endregion
    }
}