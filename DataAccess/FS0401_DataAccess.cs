using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0401_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string state, string TimeFrom, string TimeTo, string carType, string InOut, string DHFlag)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");
            sbr.AppendLine("");

            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }

    }
}