using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0502_Logic
    {
        FS0502_DataAccess fs0502_DataAccess = new FS0502_DataAccess();

        #region 按检索条件检索,返回dt
        public DataTable Search(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id)
        {
            return fs0502_DataAccess.Search(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id);
        }
        #endregion

        #region 分批纳入子画面检索数据,返回dt
        public DataTable SearchSub(string vcOrderNo,string vcPart_id,string vcSupplier_id)
        {
            return fs0502_DataAccess.SearchSub(vcOrderNo, vcPart_id, vcSupplier_id);
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string iAutoId)
        {
            return fs0502_DataAccess.initSubApi(iAutoId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string strautoid_main)
        {
            fs0502_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId,strautoid_main);
        }
        #endregion

        #region 是否可操作-按列表所选数据
        public bool IsDQR(List<Dictionary<string, Object>> listInfoData, ref string strMsg, string strType)
        {
            DataTable dt = fs0502_DataAccess.IsDQR(listInfoData, strType);
            if (dt.Rows.Count == 0)
                return true;
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strMsg += dt.Rows[i]["vcPart_id"].ToString() + "/";
                }
                strMsg = strMsg.Substring(0, strMsg.Length - 1);
                return false;
            }
        }
        #endregion

        #region 是否可操作-按检索条件
        public bool IsDQR(string vcSupplier_id,string vcStatus,string vcOrderNo,string vcPart_id, ref string strMsg)
        {
            DataTable dt = fs0502_DataAccess.IsDQR(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id,ref strMsg);
            if (dt.Rows.Count == 0)
                return true;
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strMsg += dt.Rows[i]["vcPart_id"].ToString() + "/";
                }
                strMsg = strMsg.Substring(0, strMsg.Length - 1);
                return false;
            }

        }
        #endregion

        #region 提交-按列表所选
        public int ok( List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            return fs0502_DataAccess.ok( listInfoData, strUserId);
        }
        #endregion

        #region 提交-按检索条件
        public int ok(string vcSupplier_id, string vcStatus, string vcOrderNo, string vcPart_id,string strUserId)
        {
            return fs0502_DataAccess.ok(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id, strUserId);
        }
        #endregion

        #region 分批导入子画面删除  不用
        public void DelSub(List<Dictionary<string, Object>> checkedInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0502_DataAccess.DelSub(checkedInfoData, strUserId,ref strErrorPartId);
        }
        #endregion
    }
}
