using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0102_Logic
    {
        FS0102_DataAccess fs0102_DataAccess;

        public FS0102_Logic()
        {
            fs0102_DataAccess = new FS0102_DataAccess();
        }


        #region 按检索条件检索,返回dt3311113333344
        public DataTable Search( string strRoleName)
        {
            return fs0102_DataAccess.Search(strRoleName);
        }
        #endregion
        #region 检索所有机能 c
        public DataTable SearchRoleFunction()
        {
            return fs0102_DataAccess.SearchRoleFunction();
        }
        #endregion

        #region 验证角色名称是否被使用 2
        public bool hasRoleName(string strRoleName)
        {
            return fs0102_DataAccess.hasRoleName(strRoleName);
        }
        #endregion

        #region 删除操作，若成功返回true；失败返回false
        public bool Delete(ArrayList delList)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs0102_DataAccess.Delete(delList);
                if (count > 0)
                    bIsOK = true;
                return bIsOK;
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
            return fs0102_DataAccess.getRoleUserList(strRoleId);
        }
        #endregion

        #region 返回当前角色对应的机能信息
        public DataTable getRoleFunctionList(string strRoleId)
        {
            return fs0102_DataAccess.getRoleFunctionList(strRoleId);
        }
        #endregion

        #region 返回当前角色之外的所有机能信息
        public DataTable getRoleWithOutFunctionList(string strRoleId)
        {
            return fs0102_DataAccess.getRoleWithOutFunctionList(strRoleId);
        }
        #endregion

        #region 插入操作,若成功返回true；失败返回false
        public bool Insert(string strRoleName, ArrayList functionList, ArrayList rightList, string strOperatorID)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs0102_DataAccess.Insert( strRoleName, functionList, rightList, strOperatorID);
                if (count > 0)
                    bIsOK = true;
                return bIsOK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region 修改操作,若成功返回true；失败返回false
        public bool Update( string strRoleId, string strRoleName, ArrayList functionList, ArrayList rightList, string strOperatorID)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs0102_DataAccess.Update(strRoleId, strRoleName, functionList, rightList, strOperatorID);
                if (count > 0)
                    bIsOK = true;
                return bIsOK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
