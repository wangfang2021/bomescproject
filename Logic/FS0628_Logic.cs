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

        public DataTable Search(string vcIsExportFlag, string dOrderHandleDate, string vcOrderNo, string vcPartNo, string vcInsideOutsideType, string vcNewOldFlag, string vcInjectionFactory, string vcSupplier_id,string vcWorkArea, string vcInjectionOrderNo, string dExpectReceiveDate)
        {
            return fs0628_DataAccess.Search(vcIsExportFlag, dOrderHandleDate, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcSupplier_id, vcWorkArea, vcInjectionOrderNo, dExpectReceiveDate);
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
    }
}
