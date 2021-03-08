using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0627_Logic
    {
        FS0627_DataAccess fs0627_DataAccess;

        public FS0627_Logic()
        {
            fs0627_DataAccess = new FS0627_DataAccess();

        }

        public DataSet Search(string vcInjectionFactory, string vcProject, string vcTargetYear)
        {
            return fs0627_DataAccess.Search(vcInjectionFactory, vcProject, vcTargetYear);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0627_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0627_DataAccess.isExistModData(dtamod);
        }

        public DataTable BindInsideOutsideType()
        {
            throw new NotImplementedException();
        }

        public DataTable bindInjectionFactoryApi()
        {
            return fs0627_DataAccess.bindInjectionFactoryApi();
        }

        public DataTable GetSupplier()
        {
            return fs0627_DataAccess.GetSupplier();
        }
    }
}
