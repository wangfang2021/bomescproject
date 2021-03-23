using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0628_Logic
    {
        FS0628_DataAccess fs0628_DataAccess;

        public FS0628_Logic()
        {
            fs0628_DataAccess = new FS0628_DataAccess();

        }

        public DataTable Search(string  vcIsExportFlag, string strOrderHandleTime_from, string strOrderHandleTime_to, string vcOrderNo, string vcPartNo, string vcInsideOutsideType, string vcNewOldFlag, string vcInjectionFactory, string vcSupplier_id, string vcWorkArea, string vcInjectionOrderNo, string strExpectReceiveleTime_from, string strExpectReceiveTime_to, string vcCarType)
        {
            return fs0628_DataAccess.Search(vcIsExportFlag, strOrderHandleTime_from, strOrderHandleTime_to, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcSupplier_id, vcWorkArea, vcInjectionOrderNo, strExpectReceiveleTime_from, strExpectReceiveTime_to, vcCarType);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0628_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0628_DataAccess.Del(listInfoData, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0628_DataAccess.importSave(importDt, userId);
        }

        public void ImportProgress(List<Dictionary<string, object>> listInfoData, string userId,string strUnitCode)
        {
            fs0628_DataAccess.ImportProgress(listInfoData, userId, strUnitCode);
        }

        public DataTable GetSupplier()
        {
            return fs0628_DataAccess.GetSupplier();
        }

        public DataTable GetWorkArea()
        {
            return fs0628_DataAccess.GetWorkArea();
        }

        public DataTable GetInjectionOrderNo()
        {
            return fs0628_DataAccess.GetInjectionOrderNo();
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            return fs0628_DataAccess.GetWorkAreaBySupplier(supplierCode);
        }

        public DataTable GetCarType()
        {
            return fs0628_DataAccess.GetCarType();
        }

        public void creatInjectionOrderNo(DataTable dtWZ, string userId)
        {
            fs0628_DataAccess.creatInjectionOrderNo(dtWZ, userId);
        }
    }
}
