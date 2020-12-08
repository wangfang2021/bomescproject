using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1301_Logic
    {
        FS1301_DataAccess fs1301_DataAccess;

        public FS1301_Logic()
        {
            fs1301_DataAccess = new FS1301_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1301_DataAccess.getDataInfo();
        }
    }
}
