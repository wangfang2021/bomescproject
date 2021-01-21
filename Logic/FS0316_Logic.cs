using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0316_Logic
    {
        FS0316_DataAccess fs0316_dataAccess = new FS0316_DataAccess();

        public DataTable searchApi(string flag, List<string> Origin, string supplierId, List<string> project)
        {
            return fs0316_dataAccess.searchApi(flag, Origin, supplierId, project);
        }
    }
}
