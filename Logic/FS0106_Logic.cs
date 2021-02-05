using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0106_Logic
    {
        FS0106_DataAccess fs0106_DataAccess;

        public FS0106_Logic()
        {
            fs0106_DataAccess = new FS0106_DataAccess();

        }

        public DataTable BindConst()
        {
            return fs0106_DataAccess.BindConst();
        }

        public DataSet Search(string typeCode)
        {
            return fs0106_DataAccess.Search(typeCode);
        }
   
        public void Save(DataTable dt, string vcCodeId, string vcCodeName, string userId, ref string strErrorPartId)
        {
            fs0106_DataAccess.Save(dt, vcCodeId, vcCodeName, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0106_DataAccess.Del(listInfoData, userId);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0106_DataAccess.CheckDistinctByTable(dtadd);
        }

        public bool isExist(string vcCodeIdStr, string vcCodeNameStr)
        {
            return fs0106_DataAccess.isExist(vcCodeIdStr, vcCodeNameStr);
        }
    }
}
