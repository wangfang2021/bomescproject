using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1306_Logic
    {
        FS1306_DataAccess fs1306_DataAccess;

        public FS1306_Logic()
        {
            fs1306_DataAccess = new FS1306_DataAccess();
        }
        public DataTable getDataInfo()
        {
            return fs1306_DataAccess.getDataInfo();
        }
    }
}
