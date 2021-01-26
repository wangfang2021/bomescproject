using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0626_Logic
    {
        FS0626_DataAccess fs0626_DataAccess;

        public FS0626_Logic()
        {
            fs0626_DataAccess = new FS0626_DataAccess();

        }

        public DataTable Search(string vcInjectionFactory, string vcTargetMonth, string vcSupplier_id, string vcWorkArea, string vcDock, string vcOrderNo, string vcPartNo, string vcReceiveFlag)
        {
            return fs0626_DataAccess.Search(vcInjectionFactory, vcTargetMonth, vcSupplier_id, vcWorkArea, vcDock, vcOrderNo, vcPartNo, vcReceiveFlag);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0626_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0626_DataAccess.isExistModData(dtamod);
        }

        public DataTable BindInsideOutsideType()
        {
            throw new NotImplementedException();
        }

        public DataTable bindInjectionFactory()
        {
            return fs0626_DataAccess.bindInjectionFactory();
        }

        public DataTable bindplant()
        {
            return fs0626_DataAccess.bindplant();
        }
    }
}
