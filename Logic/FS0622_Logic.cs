using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0622_Logic
    {
        FS0622_DataAccess fs0622_DataAccess;

        public FS0622_Logic()
        {
            fs0622_DataAccess = new FS0622_DataAccess();

        }

        public DataTable Search(string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcCarType)
        {
            return fs0622_DataAccess.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0622_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0622_DataAccess.isExistModData(dtamod);
        }

        public DataTable BindInsideOutsideType()
        {
            throw new NotImplementedException();
        }

        public DataTable BindConsignee()
        {
            throw new NotImplementedException();
        }
    }
}
