using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0605_Logic
    {
        FS0605_DataAccess fs0605_DataAccess;

        public FS0605_Logic()
        {
            fs0605_DataAccess = new FS0605_DataAccess();

        }

        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcIsSureFlag)
        {
            return fs0605_DataAccess.Search(vcSupplier_id, vcWorkArea, vcIsSureFlag);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0605_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0605_DataAccess.isExistModData(dtamod);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0605_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0605_DataAccess.Del(listInfoData, userId);
        }
        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0605_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void importSave(DataTable importDt, string userId)
        {
            fs0605_DataAccess.importSave(importDt, userId);
        }

        public DataTable GetSupplier()
        {
             return fs0605_DataAccess.GetSupplier();
        }

        public DataTable GetWorkArea()
        {
            return fs0605_DataAccess.GetWorkArea();
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            return fs0605_DataAccess.GetWorkAreaBySupplier(supplierCode);
        }
    }
}
