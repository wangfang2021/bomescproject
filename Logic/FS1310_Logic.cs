using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1310_Logic
    {
        FS1310_DataAccess fs1310_DataAccess;

        public FS1310_Logic()
        {
            fs1310_DataAccess = new FS1310_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1310_DataAccess.getDataInfo();
        }
    }
}
