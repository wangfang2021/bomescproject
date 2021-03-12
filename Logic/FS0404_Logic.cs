using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0404_Logic
    {
        FS0404_DataAccess fs0404_DataAccess;

        public FS0404_Logic()
        {
            fs0404_DataAccess = new FS0404_DataAccess();

        }

        public DataTable Search(string vcOrderState,string vcInOutFlag, string vcOrderNo, string dTargetDate,  string vcOrderType,string vcMemo,string userID)
        {
            return fs0404_DataAccess.Search(vcOrderState, vcInOutFlag, vcOrderNo, dTargetDate, vcOrderType, vcMemo, userID);
        }

        public DataTable isCheckByOrderNo(string lastOrderNo)
        {
            return fs0404_DataAccess.isCheckByOrderNo(lastOrderNo);
        }

        public void updateBylastOrderNo(string vcOrderType,string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList,string UserId)
        {
            fs0404_DataAccess.updateBylastOrderNo(vcOrderType, vcInOutFlag, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, UserId);
        }

        public void addOrderNo(string realPath, string vcOrderType,string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId,string uincode, ref bool bReault, ref DataTable dtMessage)
        {
            fs0404_DataAccess.addOrderNo(realPath,vcOrderType, vcInOutFlag, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, userId, uincode, ref bReault, ref dtMessage);
        }

        public void updateEditeOrderNo(string realPath,string vcOrderType, string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId, string uionCode, ref bool bReault, ref DataTable dtMessage, ref string msg,string vcJiLuLastOrderNo)
        {
            fs0404_DataAccess.updateEditeOrderNo(realPath,vcOrderType, vcInOutFlag, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, userId, uionCode, ref bReault, ref dtMessage, ref msg, vcJiLuLastOrderNo);
        }

        public DataTable getOrderType()
        {
            return fs0404_DataAccess.getOrderType();
        }

        public void addJinJiOrderNo(string realPath, string vcOrderType, string vcInOutFlag, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId,string uionCode, ref bool bReault, ref DataTable dtMessage, ref string msg)
        {
            fs0404_DataAccess.addJinJiOrderNo(realPath,vcOrderType, vcInOutFlag, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, userId, uionCode, ref  bReault, ref dtMessage, ref msg);
        }

        public DataTable getOrderCodeByName()
        {
            return fs0404_DataAccess.getOrderCodeByName();
        }

        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "Order")
            {
                dataTable.Columns.Add("vcOrder");
                dataTable.Columns.Add("vcPartNo");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
         }

        public DataTable getOrderCodeByName(string vcOrderTypeName)
        {
            return fs0404_DataAccess.getOrderCodeByName(vcOrderTypeName);
        }

        public DataTable checkTSoqDayChange(string dTargetDate)
        {
            return fs0404_DataAccess.checkTSoqDayChange(dTargetDate);
        }
    }
}
