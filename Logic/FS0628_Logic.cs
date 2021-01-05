using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0628_Logic
    {
        FS0628_DataAccess fs0628_DataAccess;

        public FS0628_Logic()
        {
            fs0628_DataAccess = new FS0628_DataAccess();

        }

        public DataTable Search(string vcOrderNo, string vcPartNo, string vcInsideOutsideType, string vcNewOldFlag, string vcInjectionFactory, string vcWorkArea, string dExpectReceiveDate)
        {
            return fs0628_DataAccess.Search(vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcWorkArea, dExpectReceiveDate);
        }

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0628_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0628_DataAccess.Del(listInfoData, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0628_DataAccess.importSave(importDt, userId);
        }
    }
}
