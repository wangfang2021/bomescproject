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

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0606_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0606_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData, DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            fs0606_DataAccess.allInstall(listInfoData,dBeginDate, dEndDate, userId);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0606_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void importSave(DataTable importDt, string userId)
        {
            fs0606_DataAccess.importSave(importDt, userId);
        }
    }
}
