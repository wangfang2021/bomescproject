using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0618_Logic
    {
        FS0618_DataAccess fs0618_DataAccess;

        public FS0618_Logic()
        {
            fs0618_DataAccess = new FS0618_DataAccess();

        }

        public DataTable Search(string vcTargetYearMonth, string vcCpdcompany, string vcOrderNo, string vcDock, string vcPartNo, string vcOrderType, string vcSupplier_id, string dOrderExportDate)
        {
            return fs0618_DataAccess.Search(vcTargetYearMonth, vcCpdcompany, vcOrderNo, vcDock, vcPartNo, vcOrderType, vcSupplier_id, dOrderExportDate);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0618_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0618_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0618_DataAccess.Del(listInfoData, userId);
        }

        public DataTable GetCpdcompany()
        {
            throw new NotImplementedException();
        }

        public DataTable getDock()
        {
            return fs0618_DataAccess.getDock();
        }

        public DataTable getSupplier()
        {
            return fs0618_DataAccess.getSupplier();
        }
    }
}
