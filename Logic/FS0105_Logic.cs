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
   
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0105_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0105_DataAccess.Del(listInfoData, userId);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0105_DataAccess.CheckDistinctByTable(dtadd);
        }
    }
}
