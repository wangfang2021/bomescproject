using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Logic
{
    public class FS0614_Logic
    {
        FS0614_DataAccess fs0614_DataAccess = new FS0614_DataAccess();


        public DataTable searchApi(string orderState, string targetYM, string orderNo, string orderType, string dUpload)
        {
            return fs0614_DataAccess.searchApi(orderState, targetYM, orderNo, orderType, dUpload);
        }

        public void cancelFile(List<Dictionary<string, Object>> list, string strUserId)
        {
            fs0614_DataAccess.cancelFile(list, strUserId);
        }

        public bool CreateOrder(List<Dictionary<string, Object>> listInfoData, string path, string userId, ref string msg)
        {
            return fs0614_DataAccess.CreateOrder(listInfoData, path, userId, ref msg);
        }


        public string getPath(string orderNo)
        {
            return fs0614_DataAccess.getPath(orderNo);
        }

        public bool checkType(List<Dictionary<string, Object>> listInfoData, ref string msg)
        {
            try
            {
                bool flag = true;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string vcOrderNo = listInfoData[i]["vcOrderNo"].ToString();
                    string vcOrderType = listInfoData[i]["vcOrderType"].ToString();
                    string vcOrderState = listInfoData[i]["vcOrderState"].ToString();
                    if (vcOrderType == "日度")
                    {
                        msg += "订单" + vcOrderNo + "为日度订单,请重新选择;\r\n";
                        flag = false;
                    }

                    if (vcOrderState == "已做成")
                    {
                        msg += "订单" + vcOrderNo + "已做成,请重新选择;\r\n";
                        flag = false;
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
