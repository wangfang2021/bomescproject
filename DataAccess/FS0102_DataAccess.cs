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
    public class FS0102_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search( string strRoleName)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@vcRoleName", SqlDbType.NVarChar,20),
                };
                parameters[0].Value = strRoleName;
                strSql.Append("  select *   \n");
                strSql.Append("  from    \n");
                strSql.Append("  (   \n");
                strSql.Append("  	select  a.vcRoleId,a.vcRoleName,b.vcOperatorName,CONVERT(varchar(30), dOperatorTime, 120) as dOperatorTime   \n");
                strSql.Append("  	from SRole a   \n");
                strSql.Append("  	left join   \n");
                strSql.Append("  	(   \n");
                strSql.Append("  	  select vcUserID,vcUserName as 'vcOperatorName' from SUser   \n");
                strSql.Append("  	)b on a.vcOperatorID=b.vcUserID   \n");
                strSql.Append("  )a  where 1=1  \n");
                if (strRoleName.Trim() != "")
                    strSql.Append("   and a.vcRoleName like '%'+@vcRoleName+'%'        \n");
                strSql.Append("   order by  dOperatorTime desc       \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索所有机能
        public DataTable SearchRoleFunction()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select vcChildFunID,vcChildFunName,'0' as 'vcRead','0' as 'vcWrite',case vcType when '0' then 'BS' else 'CS' end as 'vcFunctionType' from  SFunction      \n");
                strSql.Append("  order by vcChildFunID asc     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回当前角色对应的机能信息
        public DataTable getRoleFunctionList(string strRoleId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select a.vcChildFunID,a.vcChildFunName,isnull(b.vcRead,'0') as 'vcRead',a.vcFunctionType        \n");
                strSql.Append("     ,isnull(b.vcWrite,'0') as 'vcWrite' from        \n");
                strSql.Append("     (        \n");
                strSql.Append("        select vcChildFunID,vcChildFunName,case vcType when '0' then 'BS' else 'CS' end as 'vcFunctionType' from  SFunction         \n");
                strSql.Append("     )a         \n");
                strSql.Append("     left join        \n");
                strSql.Append("     (               \n");
                strSql.Append("        select vcChildFunID,vcRead,vcWrite from SRoleFunction where vcRoleID='" + strRoleId + "'   and vcRead<>'0'         \n");
                strSql.Append("     )b on a.vcChildFunID=b.vcChildFunID        \n");
                //strSql.Append("     where b.vcRead is not null        \n");去掉这个，显示所有机能
                strSql.Append("     order by a.vcChildFunID asc        \n");
                 
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回当前角色之外的所有机能信息
        public DataTable getRoleWithOutFunctionList(string strRoleId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select vcChildFunID,vcChildFunName,'0' as 'vcRead','0' as 'vcWrite',case vcType when '0' then 'BS' else 'CS' end as 'vcFunctionType' from SFunction          \n");
                strSql.Append("      where vcChildFunID not in          \n");
                strSql.Append("      (         \n");
                strSql.Append("           select a.vcChildFunID  from                 \n");
                strSql.Append("           (                 \n");
                strSql.Append("              select vcChildFunID,vcChildFunName from  SFunction                  \n");
                strSql.Append("           )a                  \n");
                strSql.Append("           left join                 \n");
                strSql.Append("           (                        \n");
                strSql.Append("              select vcChildFunID,vcRead,vcWrite from SRoleFunction where vcRoleID='" + strRoleId + "' and vcRead<>'0'           \n");
                strSql.Append("           )b on a.vcChildFunID=b.vcChildFunID           \n");
                strSql.Append("           where b.vcRead is not null                 \n");
                strSql.Append("      )         \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 验证角色名称是否被使用
        public bool hasRoleName(string strRoleName)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select * from SRole where vcRoleName='" + strRoleName + "'  ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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

        #region 删除操作，返回影响行数
        public int Delete(ArrayList delList)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < delList.Count; i++)
                {
                    strSql.Append(" delete SRole where vcRoleID='" + delList[i].ToString() + "';    \n");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回当前角色对应的用户信息
        public DataTable getRoleUserList(string strRoleId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select vcUserId from SUserRole where vcRoleID='" + strRoleId + "'     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 插入操作,若成功返回true；失败返回false
        public int Insert( string strRoleName, ArrayList functionList, ArrayList rightList, string strOperatorID)
        {
            try
            {
                string strUUID=Guid.NewGuid().ToString();//数据唯一标识
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  insert into SRole     \n");
                strSql.Append("  (vcRoleID,vcRoleName,vcOperatorID,dOperatorTime)      \n");
                strSql.Append("  values      \n");
                strSql.Append("  (      \n");
                strSql.Append("   '" + strUUID + "',     \n");
                strSql.Append("   '" + strRoleName + "',     \n");
                strSql.Append("   '" + strOperatorID + "',     \n");
                strSql.Append("  getDate()     \n");
                strSql.Append("  );      \n");
                for (int i = 0; i < functionList.Count; i++)
                {
                    strSql.Append("   insert into SRoleFunction     \n");
                    strSql.Append("   (vcRoleID,vcChildFunID,vcRead,vcWrite,vcOperatorID,dOperatorTime)     \n");
                    strSql.Append("   values      \n");
                    strSql.Append("   (      \n");
                    strSql.Append("    '" + strUUID + "',     \n");
                    strSql.Append("    '" + functionList[i].ToString() + "',     \n");
                    string strRight=rightList[i].ToString();//"10" or "11"
                    strSql.Append("    '" + strRight[0] + "',     \n");
                    strSql.Append("    '" + strRight[1] + "',     \n");
                    strSql.Append("    '" + strOperatorID + "',     \n");
                    strSql.Append("    getDate()     \n");
                    strSql.Append("   );      \n");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改操作,若成功返回true；失败返回false
        public int Update(string strRoleId, string strRoleName, ArrayList functionList, ArrayList rightList, string strOperatorID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  update SRole     \n");
                strSql.Append("  set vcRoleName='" + strRoleName + "',     \n");
                strSql.Append("  vcOperatorID='" + strOperatorID + "',     \n");
                strSql.Append("  dOperatorTime=getDate()    \n");
                strSql.Append("  where  vcRoleID='" + strRoleId + "'  ;  \n");

                strSql.Append("   delete SRoleFunction   where   vcRoleID='" + strRoleId + "';   \n");
                for (int i = 0; i < functionList.Count; i++)
                {
                    strSql.Append("   insert into SRoleFunction     \n");
                    strSql.Append("   (vcRoleID,vcChildFunID,vcRead,vcWrite,vcOperatorID,dOperatorTime)     \n");
                    strSql.Append("   values      \n");
                    strSql.Append("   (      \n");
                    strSql.Append("    '" + strRoleId + "',     \n");
                    strSql.Append("    '" + functionList[i].ToString() + "',     \n");
                    string strRight = rightList[i].ToString();//"10" or "11"
                    strSql.Append("    '" + strRight[0] + "',     \n");
                    strSql.Append("    '" + strRight[1] + "',     \n");
                    strSql.Append("    '" + strOperatorID + "',     \n");
                    strSql.Append("    getDate()     \n");
                    strSql.Append("   );      \n");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
