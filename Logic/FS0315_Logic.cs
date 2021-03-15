using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0315_Logic
    {
        FS0315_DataAccess fs0315_dataAccess = new FS0315_DataAccess();

        public List<DataTable> searchApi(string startTime, string endTime, List<string> Type, string vcOriginCompany)
        {
            return fs0315_dataAccess.searchApi(startTime, endTime, Type, vcOriginCompany);
        }
        public DataTable getProject()
        {
            return fs0315_dataAccess.getProject();
        }
    }
}
