using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0613_Logic
    {
        FS0613_DataAccess fs0613_DataAccess;

        public FS0613_Logic()
        {
            fs0613_DataAccess = new FS0613_DataAccess();

        }
        public DataTable Search(string vcDock, string vcCarType)
        {
            return fs0613_DataAccess.Search(vcDock, vcCarType);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0613_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0613_DataAccess.isExistModData(dtamod);
        }

        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            fs0613_DataAccess.Save(dtadd, dtmod,userId);
        }

        public void Del(DataTable dtdel, string userId)
        {
            fs0613_DataAccess.Del(dtdel,userId);
        }
       
    }
}
