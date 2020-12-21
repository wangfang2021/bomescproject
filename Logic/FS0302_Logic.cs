using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0302_Logic
    {
        FS0302_DataAccess fs0302_dataAccess = new FS0302_DataAccess();

        public DataTable getFinishState()
        {
            return fs0302_dataAccess.getFinishState();
        }
        public DataTable getChange()
        {
            return fs0302_dataAccess.getChange();
        }

        public DataTable getData(string fileNameTJ)
        {
            return fs0302_dataAccess.getData(fileNameTJ);
        }
    }
}
