using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0101_Logic
    {
        FS0101_DataAccess fs0101_DataAccess;

        public FS0101_Logic()
        {
            fs0101_DataAccess = new FS0101_DataAccess();
        }


        #region 按检索条件检索,返回dt
        public DataTable Search(string strSiteId,string strUserId,string strUserName)
        {
            return fs0101_DataAccess.Search(strSiteId, strUserId, strUserName);
        }
        #endregion

        #region 根据用户编码，获取所属角色列表
        public DataTable getRoleList(string strUserId)
        {
            try
            {
                return fs0101_DataAccess.getRoleList(strUserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 删除操作，若成功返回true；失败返回false
        public bool Delete(ArrayList delList)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs0101_DataAccess.Delete(delList);
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

        #region 获取所有角色
        public DataTable getAllRole()
        {
            return fs0101_DataAccess.getAllRole();
        }
        #endregion

        #region 获取所有特殊权限
        public DataTable getAllSpecial()
        {
            return fs0101_DataAccess.getAllSpecial();
        }
        #endregion

        #region 获取该用户拥有的角色
        public DataTable getRoleByUserId(string strUserId)
        {
            return fs0101_DataAccess.getRoleByUserId(strUserId);
        }
        #endregion

        #region 验证用户代码是否被使用
        public bool hasUserId(string strUserId)
        {
            return fs0101_DataAccess.hasUserId(strUserId);
        }
        #endregion

        #region 验证用户数据是否发生变化
        public bool checkUserChange(string strUserId,string strLastDate)
        {
            return fs0101_DataAccess.checkUserChange(strUserId, strLastDate);
        }
        #endregion

        //#region 验证GTMC用户是否有取消收车的权限，有则返回数据
        //public DataTable hasCancelPri(ArrayList roleList)
        //{
        //    return fs0101_DataAccess.hasCancelPri(roleList);
        //}
        //#endregion

        #region 插入操作,若成功返回true；失败返回false
        public bool Insert(string strUserId, string strUserName, string strPwd, string strPalnt, string strUnit, List<string> roleList, string strOperatorID,string strMail,string strStop, string strSpecial, string vcBanZhi, string vcBaoZhuangPlace, string vcPlatForm)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs0101_DataAccess.Insert(strUserId, strUserName, strPwd, strPalnt, strUnit, roleList, strOperatorID, strMail, strStop, strSpecial, vcBanZhi, vcBaoZhuangPlace, vcPlatForm);
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
        public bool Update(string strUserId, string strUserName, string strPwd, string strPalnt, string strUnit, List<string> roleList, string strOperatorID, string strMail, string strStop, string strSpecial, string vcBanZhi,string vcBaoZhuangPlace, string vcPlatForm)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs0101_DataAccess.Update(strUserId, strUserName, strPwd, strPalnt, strUnit, roleList, strOperatorID, strMail,strStop, strSpecial, vcBanZhi, vcBaoZhuangPlace, vcPlatForm);
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

        #region 根据事业体返回工厂下拉列表
        public DataTable getPlantList(string strUnitCode)
        {
            return fs0101_DataAccess.getPlantList(strUnitCode);
        }
        #endregion


        #region 清除IP
        public void clearIp(string strUserId)
        {
            try
            {
                fs0101_DataAccess.clearIp(strUserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 转换工厂选项，从list转成string存储
        public string getStrByList(List<string> list)
        {
            if (list == null || list.Count == 0)
                return "";
            StringBuilder sbr = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                    sbr.Append(",");
                sbr.Append(list[i]);
            }
            return sbr.ToString();
        }
        #endregion

        #region 获取所有班值
        public DataTable getBanZhi()
        {
            return fs0101_DataAccess.getBanZhi();
        }
        #endregion

        #region 获取所有包装场
        public DataTable getBaoZhuangPlace()
        {
            return fs0101_DataAccess.getBaoZhuangPlace();
        }
        #endregion

        #region 获取所有用户编号
        public DataTable getUserID()
        {
            return fs0101_DataAccess.getUserID();
        }
        #endregion

        #region 导入用户数据
        public void ImportAddUsers(string strUnit, DataTable dt, string vcPlatForm, string strUserID)
        {
            fs0101_DataAccess.ImportAddUsers(strUnit, dt, vcPlatForm, strUserID);
        }
        #endregion
    }
}
