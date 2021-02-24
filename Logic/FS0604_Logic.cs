using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0604_Logic
    {
        FS0604_DataAccess fs0604_DataAccess;

        public FS0604_Logic()
        {
            fs0604_DataAccess = new FS0604_DataAccess();

        }

        public DataTable Search(string dSynchronizationDate, string vcState, string vcPartNo, string vcSupplier_id, string vcWorkArea, string vcCarType, string dExpectDeliveryDate, string vcOEOrSP, string vcBoxType)
        {
            return fs0604_DataAccess.Search(dSynchronizationDate, vcState, vcPartNo, vcSupplier_id, vcWorkArea, vcCarType, dExpectDeliveryDate, vcOEOrSP, vcBoxType);
        }

        public DataTable GetBoxType()
        {
            return fs0604_DataAccess.GetBoxType();
        }
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0604_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData, string vcIntake, string userId)
        {
            fs0604_DataAccess.allInstall(listInfoData, vcIntake, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0604_DataAccess.importSave(importDt, userId);
        }

        public DataTable GetSupplier()
        {
            return fs0604_DataAccess.GetSupplier();
        }

        public DataTable GetCarType()
        {
            return fs0604_DataAccess.GetCarType();
        }

        public DataTable GetExpectDeliveryDate()
        {
            return fs0604_DataAccess.GetExpectDeliveryDate();
        }

        public DataTable GetWorkArea()
        {
            return fs0604_DataAccess.GetWorkArea();
        }

        public DataTable GetTaskNum1()
        {
            return fs0604_DataAccess.GetTaskNum1();
        }

        public DataTable GetWorkAreaBySupplier(string vcSupplier_id)
        {
            return fs0604_DataAccess.GetWorkAreaBySupplier(vcSupplier_id);
        }

        public DataTable GetTaskNum()
        {
            return fs0604_DataAccess.GetTaskNum();
        }

        public void hZZK(List<Dictionary<string, object>> listInfoData, string dExpectDeliveryDate, string userId)
        {
            fs0604_DataAccess.hZZK(listInfoData, dExpectDeliveryDate, userId);
        }
        public void hZZK(DataTable dtNewSupplierand, string dExpectDeliveryDate, string userId)
        {
            fs0604_DataAccess.hZZK(dtNewSupplierand, dExpectDeliveryDate, userId);
        }

        public void admit(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.admit(listInfoData, userId);
        }
        public void returnHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.returnHandle(listInfoData, userId);
        }
        public void weaveHandle(List<Dictionary<string, object>> listInfoData, string userId, ref bool bReault, ref DataTable dtMessage)
        {
            fs0604_DataAccess.weaveHandle(listInfoData, userId, ref bReault, ref dtMessage);
        }

        public void sdweaveHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0604_DataAccess.sdweaveHandle(listInfoData, userId);
        }

        public bool isCheckImportAddData(string vcPackingPlant, string vcReceiver, string vcSupplier_id, string vcPartNo)
        {
            DataTable dt = fs0604_DataAccess.isCheckImportAddData(vcPackingPlant, vcReceiver, vcSupplier_id, vcPartNo);
            return dt.Rows.Count > 0;
        }

        public DataTable CheckEmail(string strSupplier)
        {
            DataTable dt = fs0604_DataAccess.CheckEmail(strSupplier);
            return dt;
        }
    }
}
