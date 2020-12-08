using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1309_Logic
    {
        FS1309_DataAccess fs1309_DataAccess;

        public FS1309_Logic()
        {
            fs1309_DataAccess = new FS1309_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1309_DataAccess.getDataInfo();
        }
    }
}
