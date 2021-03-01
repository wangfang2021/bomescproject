using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0629_Logic
    {
        FS0629_DataAccess fs0629_DataAccess;

        public FS0629_Logic()
        {
            fs0629_DataAccess = new FS0629_DataAccess();

        }

        public DataSet Search(string vcConsignee, string vcInjectionFactory, string vcTargetMonth, string vcLastTargetMonth)
        {
            return fs0629_DataAccess.Search(vcConsignee, vcInjectionFactory, vcTargetMonth, vcLastTargetMonth);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0629_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0629_DataAccess.isExistModData(dtamod);
        }

        public DataTable BindInsideOutsideType()
        {
            throw new NotImplementedException();
        }

        public DataTable BindConsignee()
        {
            throw new NotImplementedException();
        }

        public DataSet GetQianPin(string vcConsignee, string vcInjectionFactory, string vcTargetMonth)
        {
            return fs0629_DataAccess.GetQianPin(vcConsignee, vcInjectionFactory, vcTargetMonth);
        }
    }
}
