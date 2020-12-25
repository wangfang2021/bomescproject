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

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0613_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0613_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0613_DataAccess.Del(listInfoData, userId);
        }
    }
}
