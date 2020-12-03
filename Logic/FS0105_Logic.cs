using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0105_Logic
    {
        FS0105_DataAccess fs0105_DataAccess;

        public FS0105_Logic()
        {
            fs0105_DataAccess = new FS0105_DataAccess();

        }

        public DataTable BindConst()
        {
            return fs0105_DataAccess.BindConst();
        }

        public DataTable Search(string typeCode)
        {
            return fs0105_DataAccess.Search(typeCode);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0105_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0105_DataAccess.isExistModData(dtamod);
        }

        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            fs0105_DataAccess.Save(dtadd, dtmod,userId);
        }

        public void Del(DataTable dtdel, string userId)
        {
            fs0105_DataAccess.Del(dtdel,userId);
        }
    }
}
