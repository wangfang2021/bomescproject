using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0107_Logic
    {
        FS0107_DataAccess fs0107_DataAccess;

        public FS0107_Logic()
        {
            fs0107_DataAccess = new FS0107_DataAccess();

        }

        public DataTable BindConst()
        {
            return fs0107_DataAccess.BindConst();
        }

        public DataTable Search(string typeCode, string vcValue1, string vcValue2)
        {
            return fs0107_DataAccess.Search(typeCode, vcValue1, vcValue2);
        }
   
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0107_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0107_DataAccess.Del(listInfoData, userId);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0107_DataAccess.CheckDistinctByTable(dtadd);
        }
    }
}
