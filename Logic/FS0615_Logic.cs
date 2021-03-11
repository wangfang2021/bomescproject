using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0615_Logic
    {
        FS0615_DataAccess fs0615_DataAccess;

        public FS0615_Logic()
        {
            fs0615_DataAccess = new FS0615_DataAccess();

        }
        public DataTable Search(string vcDock, string vcCarType)
        {
            return fs0615_DataAccess.Search(vcDock, vcCarType);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0615_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0615_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0615_DataAccess.Del(listInfoData, userId);
        }
    }
}
