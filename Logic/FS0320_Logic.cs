using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0320_Logic
    {
        FS0320_DataAccess fs0320_dataAccess = new FS0320_DataAccess();

        public DataTable searchApi(string vcPart_id, string vcPartNameEn, string vcPartNameCn)
        {
            return fs0320_dataAccess.searchApi(vcPart_id, vcPartNameEn, vcPartNameCn);
        }

        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0320_dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }

        public void importSave(DataTable dt, string strUserId, string strUnitCode)
        {
            fs0320_dataAccess.importSave(dt, strUserId, strUnitCode);

        }


    }
}
