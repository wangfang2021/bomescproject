using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0605_Logic
    {
        FS0605_DataAccess fs0605_DataAccess;

        public FS0605_Logic()
        {
            fs0605_DataAccess = new FS0605_DataAccess();

        }

        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcIsSureFlag)
        {
            return fs0605_DataAccess.Search(vcSupplier_id, vcWorkArea, vcIsSureFlag);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0605_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0605_DataAccess.isExistModData(dtamod);
        }

        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            fs0605_DataAccess.Save(dtadd, dtmod,userId);
        }

        public void Del(DataTable dtdel, string userId)
        {
            fs0605_DataAccess.Del(dtdel,userId);
        }
    }
}
