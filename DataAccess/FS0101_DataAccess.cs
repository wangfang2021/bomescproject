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
    public class FS0101_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search(string strUnitCode, string strUserId, string strUserName)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@vcUserName", SqlDbType.NVarChar,20),
                };
                parameters[0].Value = strUserName;
                strSql.Append("      select *         \n");
                strSql.Append("       ,d.vcName as 'vcBaoZhuangPlace_Name'     \n");
                strSql.Append("       ,e.vcName as 'vcBanZhi_Name'      \n");
                strSql.Append("      from          \n");
                strSql.Append("      (         \n");
                strSql.Append("      	select a.vcUserID,a.vcUserName,a.vcBaoZhuangPlace,a.vcBanZhi       \n");
                strSql.Append("      	,case when isnull(vcStop,'0')='0' then '启用' else '停用' end as vcStop,a.vcUserName as vcOperatorName,dOperatorTime,vcIp,vcUnitCode,vcPlantCode,vcEmail,vcSpecial,vcPlatForm      \n");
                strSql.Append("      	from SUser a         \n");
                strSql.Append("      	left join         \n");
                strSql.Append("      	(         \n");
                strSql.Append("      	  select vcUserID,vcUserName as 'vcOperatorName' from SUser         \n");
                strSql.Append("      	)b on a.vcOperatorID=b.vcUserID         \n");
                strSql.Append("      )a      \n");
                strSql.Append("      left join      \n");
                strSql.Append("      (      \n");
                strSql.Append("        	SELECT a.vcUserID,											      \n");
                strSql.Append("        	STUFF											      \n");
                strSql.Append("        	(											      \n");
                strSql.Append("        		(										      \n");
                strSql.Append("        			SELECT ','+SRole.vcRoleName FROM SUserRole,SRole       \n");
                strSql.Append("        			WHERE SUserRole.vcUserId=a.vcUserId and SUserRole.vcRoleID=SRole.vcRoleID 	      \n");
                strSql.Append("        			FOR XML PATH('')									      \n");
                strSql.Append("        		), 1, 1, ''										      \n");
                strSql.Append("        	)   AS vcRoleName  											      \n");
                strSql.Append("        	FROM SUser a 											      \n");
                strSql.Append("        	GROUP BY vcUserID       \n");
                strSql.Append("      )b on a.vcUserID=b.vcUserID      \n");
                strSql.Append("      left join       \n");
                strSql.Append("      (       \n");
                strSql.Append("         select vcUnitCode,vcUnitName from SUnitInfo       \n");
                strSql.Append("      )c on a.vcUnitCode=c.vcUnitCode       \n");
                strSql.Append("      left join      \n");
                strSql.Append("      (      \n");
                strSql.Append("         select vcValue,vcName from TCode where vcCodeId='C001'      \n");
                strSql.Append("      )d on a.vcBaoZhuangPlace=d.vcValue      \n");
                strSql.Append("      left join      \n");
                strSql.Append("      (      \n");
                strSql.Append("         select vcValue,vcName from TCode where vcCodeId='C010'      \n");
                strSql.Append("      )e on a.vcBanZhi=e.vcValue      \n");

                strSql.Append("      where 1=1      \n");
                if (strUnitCode.Trim() != "")
                    strSql.Append("   and a.vcUnitCode='" + strUnitCode.Trim() + "'        \n");
                if (strUserId.Trim() != "")
                    strSql.Append("   and a.vcUserID like '%" + strUserId.Trim() + "%'        \n");
                if (strUserName.Trim() != "")
                    strSql.Append("   and a.vcUserName like '%'+@vcUserName+'%'        \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据用户编码，获取所属角色列表
        public DataTable getRoleList(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select SRole.vcRoleID,SRole.vcRoleName from SUserRole,SRole where            \n");
                strSql.Append("    vcUserId='" + strUserId + "'            \n");
                strSql.Append("    and SUserRole.vcRoleID=SRole.vcRoleID           \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 删除操作，返回影响行数
        public int Delete(ArrayList delList)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < delList.Count; i++)
                {
                    strSql.Append(" delete SUSer where vcUserID='" + delList[i].ToString() + "';    \n");
                    strSql.Append(" delete SUserRole where vcUserID='" + delList[i].ToString() + "';    \n");
                    strSql.Append(" delete tPointPower where vcUserID='" + delList[i].ToString() + "';    \n");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取所有角色
        public DataTable getAllRole()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcRoleID as [key],vcRoleName as label from SRole    ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 获取所有特殊权限
        public DataTable getAllSpecial()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcValue as [label],vcMeaning as [value] from TCode where vcCodeId='C011'    ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 根据事业体返回工厂下拉列表
        public DataTable getPlantList(string strUnitCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcValue as vcPlantCode,vcMeaning as vcPlantName from TCode where vcCodeId='C000' order by vcValue asc   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取所有班值
        public DataTable getBanZhi()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("      select vcValue as 'key',vcName as 'label' from TCode where vcCodeId = 'C010'       ");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        #endregion

        #region 获取所有包装场
        public DataTable getBaoZhuangPlace()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("      select vcValue as 'key',vcName as 'label' from TCode where vcCodeId = 'C023'       ");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        #endregion

        #region 获取所有用户编号
        public DataTable getUserID()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("      select vcUserID from SUser       ");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        #endregion

        #region 获取该用户拥有的角色
        public DataTable getRoleByUserId(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select a.vcRoleID,b.vcRoleName from    \n ");
                strSql.Append("  (    \n ");
                strSql.Append("  	select * from SUserRole    \n ");
                strSql.Append("  	where vcUserId='" + strUserId + "'    \n ");
                strSql.Append("  )a    \n ");
                strSql.Append("  left join    \n ");
                strSql.Append("  (    \n ");
                strSql.Append("  	select * from SRole    \n ");
                strSql.Append("  )b  on a.vcRoleID=b.vcRoleID    \n ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 验证用户代码是否被使用
        public bool hasUserId(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select * from SUSer where vcUserID='" + strUserId + "'  ");
                DataTable dt= excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt != null && dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 验证用户数据是否发生变化
        public bool checkUserChange(string strUserId, string strLastDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select * from SUSer where vcUserID='" + strUserId + "'  ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strOpDate = dt.Rows[0]["dOperatorTime"].ToString();
                    if (strOpDate == strLastDate)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 插入操作,若成功返回true；失败返回false
        public int Insert(string strUserId, string strUserName, string strPwd, string strPalnt, string strUnit, List<string> roleList, string strOperatorID,string strMail,string strStop,string strSpecial, string strBanZhi, string strBaoZhuangPlace, string vcPlatForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  insert into SUser     \n");
                strSql.Append("  (vcUnitCode,vcPlantCode,vcUserID,vcUserName,vcPassWord,vcOperatorID,dOperatorTime,vcStop,vcEmail,vcSpecial,vcBanZhi, vcBaoZhuangPlace,vcPlatForm)      \n");
                strSql.Append("  values      \n");
                strSql.Append("  (      \n");
                strSql.Append("   '" + strUnit + "',     \n");
                strSql.Append("   '" + strPalnt + "',     \n");
                strSql.Append("   '" + strUserId + "',     \n");
                strSql.Append("   '" + strUserName + "',     \n");
                strSql.Append("   '" + strPwd + "',     \n");
                strSql.Append("   '" + strOperatorID + "',     \n");
                strSql.Append("  getDate(),     \n");
                strSql.Append("   '" + strStop + "',     \n");
                strSql.Append("   '" + strMail + "',     \n");
                strSql.Append("   '" + strSpecial + "',     \n");
                strSql.Append("   '" + strBanZhi + "',     \n");
                strSql.Append("   '" + strBaoZhuangPlace + "',     \n");
                strSql.Append("   '" + vcPlatForm + "'     \n");
                strSql.Append("  );      \n");

                for (int i = 0; i < roleList.Count; i++)
                {
                    strSql.Append("   insert into SUserRole     \n");
                    strSql.Append("   select '" + strUserId + "',vcRoleID from SRole where vcRoleID='" + roleList[i] + "';     \n");
                }
                strSql.Append("  insert into tPointPower     \n");
                strSql.Append("  (vcUserId,vcPlant)      \n");
                strSql.Append("  values      \n");
                strSql.Append("  (      \n");
                strSql.Append("   '" + strUserId + "',     \n");
                strSql.Append("   '" + strPalnt + "'     \n");
                strSql.Append("  );      \n");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改操作,若成功返回true；失败返回false
        public int Update(string strUserId, string strUserName, string strPwd, string strPalnt, string strUnit, List<string> roleList, string strOperatorID,string strMail, string strStop,string strSpecial, string strBanZhi, string strBaoZhuangPlace, string vcPlatForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  update SUser     \n");
                strSql.Append("  set vcUserName='" + strUserName + "',     \n");
                if (strPwd!="")
                    strSql.Append("  vcPassWord='" + strPwd + "',     \n");
                strSql.Append("  vcOperatorID='" + strOperatorID + "',     \n");
                strSql.Append("  dOperatorTime=getDate(),     \n");
                strSql.Append("  vcStop='" + strStop + "',     \n");
                strSql.Append("  vcUnitCode='" + strUnit + "',     \n");
                strSql.Append("  vcPlantCode='" + strPalnt + "',     \n");
                strSql.Append("  vcEmail='" + strMail + "',     \n");
                strSql.Append("  vcSpecial='" + strSpecial + "',     \n");
                strSql.Append("  vcBanZhi='" + strBanZhi + "',     \n");
                strSql.Append("  vcBaoZhuangPlace='" + strBaoZhuangPlace + "',     \n");
                strSql.Append("  vcPlatForm='" + vcPlatForm + "'     \n");
                
                strSql.Append("  where vcUserId='" + strUserId + "'  ;   \n");

                strSql.Append("   delete SUserRole  where vcUserId='" + strUserId + "' ;   \n");
                for (int i = 0; i < roleList.Count; i++)
                {
                    strSql.Append("   insert into SUserRole     \n");
                    strSql.Append("   select '" + strUserId + "',vcRoleID from SRole where vcRoleID='" + roleList[i] + "';     \n");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 清除IP
        public void clearIp(string strUserId)
        {
            try
            {
                StringBuilder strSql_temp = new StringBuilder();
                strSql_temp.Append("  update SUser set vcIp=null where vcUserID='" + strUserId + "'     \n");
                excute.ExcuteSqlWithStringOper(strSql_temp.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入新增用户
        public void ImportAddUsers(string strUnit, DataTable dt, string vcPlatForm, string strUserID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //发注工厂  
                    strSql.Append("  insert into SUser     \n");
                    strSql.Append("  (vcUnitCode,vcPlantCode,vcUserID,vcUserName,vcPassWord,vcOperatorID,dOperatorTime,vcStop,vcEmail,vcSpecial,vcBanZhi, vcBaoZhuangPlace,vcPlatForm)      \n");
                    strSql.Append("  values      \n");
                    strSql.Append("  (      \n");
                    strSql.Append("   '" + strUnit + "',     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcPlantCode"], false) + ",     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcUserID"], false) + ",     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcUserName"], false) + ",     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(ComFunction.encodePwd(dt.Rows[i]["vcPassWord"].ToString()), false) + ",     \n");
                    strSql.Append("   '" + strUserID + "',     \n");
                    strSql.Append("  getDate(),     \n");
                    string strStop = dt.Rows[i]["vcStop"] == null || dt.Rows[i]["vcStop"].ToString() == "" ? "0" : "1";
                    strSql.Append("   '" + strStop + "',     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcEmail"], false) + ",     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcSpecial"], false) + ",     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcBanZhi"], false) + ",     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcBaoZhuangPlace"], false) + ",     \n");
                    strSql.Append("   '" + vcPlatForm + "'     \n");
                    strSql.Append("  );      \n");

                    string strRole = dt.Rows[i]["vcUserRole"].ToString();
                    List<string> roleList = strRole.Split(';').ToList();

                    for (int rolesIndex = 0; rolesIndex < roleList.Count; rolesIndex++)
                    {
                        strSql.Append("   insert into SUserRole     \n");
                        strSql.Append("   select '" + strUserID + "',vcRoleID from SRole where vcRoleID='" + roleList[rolesIndex] + "';     \n");
                    }

                    strSql.Append("  insert into tPointPower     \n");
                    strSql.Append("  (vcUserId,vcPlant)      \n");
                    strSql.Append("  values      \n");
                    strSql.Append("  (      \n");
                    strSql.Append("   '" + strUserID + "',     \n");
                    strSql.Append("   " + ComFunction.getSqlValue(dt.Rows[i]["vcPlantCode"],false) + "     \n");
                    strSql.Append("  );      \n");

                }
                excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
