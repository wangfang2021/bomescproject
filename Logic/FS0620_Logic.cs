using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0620_Logic
    {
        FS0620_DataAccess fs0620_DataAccess;

        public FS0620_Logic()
        {
            fs0620_DataAccess = new FS0620_DataAccess();

        }

        public DataTable Search(string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcCarType)
        {
            return fs0620_DataAccess.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0620_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0620_DataAccess.isExistModData(dtamod);
        }

        public void importSave(DataTable importDt, string userId)
        {
            fs0620_DataAccess.importSave(importDt, userId);
        }
    }
}
