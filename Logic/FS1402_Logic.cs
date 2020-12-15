using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1402_Logic
    {
        FS1402_DataAccess fs1402_DataAccess;

        public FS1402_Logic()
        {
            fs1402_DataAccess = new FS1402_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1402_DataAccess.getDataInfo();
        }
    }
}
