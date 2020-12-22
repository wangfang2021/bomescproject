using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0301_Logic
    {
        FS0301_DataAccess fs0301_dataAccess = new FS0301_DataAccess();

        public DataTable getList(string iState, string dOperatorTime)
        {
            return fs0301_dataAccess.getList(iState, dOperatorTime);
        }

        public void updateState(string fileName)
        {
            fs0301_dataAccess.updateState(fileName);
        }

        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0301_dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
    }
}
