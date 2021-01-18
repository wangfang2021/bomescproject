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

        public DataTable Search(string vcOrderState, string vcOrderNo, string dTargetDate,  string vcOrderType,string userID)
        {
            return fs0404_DataAccess.Search(vcOrderState, vcOrderNo, dTargetDate, vcOrderType, userID);
        }

        public DataTable isCheckByOrderNo(string lastOrderNo)
        {
            return fs0404_DataAccess.isCheckByOrderNo(lastOrderNo);
        }

        public void updateBylastOrderNo(string vcOrderType, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList,string UserId)
        {
            fs0404_DataAccess.updateBylastOrderNo(vcOrderType, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, UserId);
        }

        public void addOrderNo(string vcOrderType, string dTargetDate, string dTargetWeek, string lastOrderNo, string newOrderNo, string vcMemo, List<Dictionary<string, object>> fileList, string userId)
        {
            fs0404_DataAccess.addOrderNo(vcOrderType, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, userId);
        }
    }
}
