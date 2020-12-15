using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0606_Logic
    {
        FS0606_DataAccess fs0606_DataAccess;

        public FS0606_Logic()
        {
            fs0606_DataAccess = new FS0606_DataAccess();

        }

        public DataTable Search(string vcPartNo)
        {
            return fs0606_DataAccess.Search(vcPartNo);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0606_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0606_DataAccess.isExistModData(dtamod);
        }

        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            fs0606_DataAccess.Save(dtadd, dtmod,userId);
        }

        public void Del(DataTable dtdel, string userId)
        {
            fs0606_DataAccess.Del(dtdel,userId);
        }
        public void allInstall(DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            fs0606_DataAccess.allInstall(dBeginDate, dEndDate, userId);
        }
    }
}
