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

        public DataTable Search(string vcSupplier_id, string vcWorkArea)
        {
            return fs0607_DataAccess.Search(vcSupplier_id, vcWorkArea);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0607_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0607_DataAccess.isExistModData(dtamod);
        }

        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            fs0607_DataAccess.Save(dtadd, dtmod,userId);
        }

        public void Del(DataTable dtdel, string userId)
        {
            fs0607_DataAccess.Del(dtdel,userId);
        }
        public void allInstall(DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            fs0607_DataAccess.allInstall(dBeginDate, dEndDate, userId);
        }
    }
}
