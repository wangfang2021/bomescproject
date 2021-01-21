using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0317_Logic
    {
        FS0317_DataAccess fs0317_dataAccess = new FS0317_DataAccess();

        public DataTable searchApi(string startTime, string endTime, string supplierId)
        {
            return fs0317_dataAccess.searchApi(startTime, endTime, supplierId);
        }
    }
}
