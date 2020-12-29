using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0623_Logic
    {
        FS0623_DataAccess fs0623_DataAccess;

        public FS0623_Logic()
        {
            fs0623_DataAccess = new FS0623_DataAccess();

        }

        public DataTable Search(string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcCarType)
        {
            return fs0623_DataAccess.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
        }

        public DataTable Search_Sub()
        {
            return fs0623_DataAccess.Search_Sub();
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0623_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save_Sub(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorName)
        {
            fs0623_DataAccess.Save_Sub(listInfoData,userId, strErrorName);
        }

        public void Del_Sub(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0623_DataAccess.Del_Sub(listInfoData, userId);
        }
    }
}
