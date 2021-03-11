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


        public DataTable searchApi(string orderState, string targetYM, string orderNo, string orderType, string dUpload, string memo)
        {
            return fs0614_DataAccess.searchApi(orderState, targetYM, orderNo, orderType, dUpload, memo);
        }


        public bool CreateOrder(List<Dictionary<string, Object>> listInfoData, string path, string userId, string uionCode, ref bool bReault, ref DataTable dtMessage)
        {
            return fs0614_DataAccess.CreateOrder(listInfoData, path, userId, uionCode, ref bReault, ref dtMessage);
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

        public DataTable getType()
        {
            return fs0614_DataAccess.getType();
        }

        public void cancelOrder(List<Dictionary<string, Object>> list, string userId)
        {
            fs0614_DataAccess.cancelOrder(list, userId);
        }
    }
}
