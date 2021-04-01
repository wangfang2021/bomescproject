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
            DataTable dataTable= fs0502_DataAccess.Search(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string decBoxQuantity = dataTable.Rows[i]["decBoxQuantity"].ToString();
                if (decBoxQuantity != "")
                {
                    string strBoxColor = IsInteger(decBoxQuantity);
                    dataTable.Rows[i]["boxColor"] = strBoxColor;
                }
            }

            return dataTable;
        }
        public string IsInteger(string s)
        {
            int i;
            double d;
            if (int.TryParse(s, out i))
                return "0";
            else if (double.TryParse(s, out d))
                return (d == Math.Truncate(d) ? "0" : "red");
            else
                return "0";
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

        #region 取C056中2个状态
        public DataTable getTCode(string strCodeId)
        {
            return fs0502_DataAccess.getTCode(strCodeId);
        }
        #endregion

        public DataTable getOrderNo(string vcSupplier_id)
        {
            return fs0502_DataAccess.getOrderNo(vcSupplier_id);
        }

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string strautoid_main, 
            string vcPart_id, string vcOrderNo, string vcSupplier_id,ref string infopart,string iPackingQty)
        {
            fs0502_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId,strautoid_main,  vcPart_id, vcOrderNo, vcSupplier_id,ref infopart, iPackingQty);
        }
        #endregion

        #region 是否可操作-按列表所选数据
        public bool IsDQR(List<Dictionary<string, Object>> listInfoData, ref string strMsg_status,ref string strMsg_null, string strType)
        {
            DataSet ds = fs0502_DataAccess.IsDQR(listInfoData, strType);
            DataTable dt_status = ds.Tables[0];
            //DataTable dt_null = ds.Tables[1];

            if (dt_status.Rows.Count == 0 )
                return true;
            else
            {
                if(dt_status.Rows.Count>0)
                {
                    for (int i = 0; i < dt_status.Rows.Count; i++)
                    {
                        strMsg_status += dt_status.Rows[i]["vcPart_id"].ToString() + "/";
                    }
                    strMsg_status = strMsg_status.Substring(0, strMsg_status.Length - 1);
                }
                //if(dt_null.Rows.Count>0)
                //{
                //    for(int i=0;i<dt_null.Rows.Count;i++)
                //    {
                //        strMsg_null += dt_null.Rows[i]["vcPart_id"].ToString() + "/";
                //    }
                //    strMsg_null = strMsg_null.Substring(0, strMsg_null.Length - 1);
                //}
                return false;
            }
        }
        #endregion

        #region 是否可操作-按检索条件
        public bool IsDQR(string vcSupplier_id,string vcStatus,string vcOrderNo,string vcPart_id, ref string strMsg_status,ref string strMsg_null)
        {
            DataSet ds = fs0502_DataAccess.IsDQR(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id);
            DataTable dt_status = ds.Tables[0];
            DataTable dt_null = ds.Tables[1];

            if (dt_status.Rows.Count == 0 && dt_null.Rows.Count == 0)
                return true;
            else
            {
                if (dt_status.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_status.Rows.Count; i++)
                    {
                        strMsg_status += dt_status.Rows[i]["vcPart_id"].ToString() + "/";
                    }
                    strMsg_status = strMsg_status.Substring(0, strMsg_status.Length - 1);
                }
                if (dt_null.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_null.Rows.Count; i++)
                    {
                        strMsg_null += dt_null.Rows[i]["vcPart_id"].ToString() + "/";
                    }
                    strMsg_null = strMsg_null.Substring(0, strMsg_null.Length - 1);
                }
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
