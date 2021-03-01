using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0607_Logic
    {
        FS0607_DataAccess fs0607_DataAccess;

        public FS0607_Logic()
        {
            fs0607_DataAccess = new FS0607_DataAccess();

        }

        public DataTable Search(string vcSupplier_id, string vcWorkArea, string dBeginDate, string dEndDate, string vcMemo)
        {
            return fs0607_DataAccess.Search(vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcMemo);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0607_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0607_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0607_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData, DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            fs0607_DataAccess.allInstall(listInfoData, dBeginDate, dEndDate, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0607_DataAccess.importSave(importDt, userId);
        }
    }
}
