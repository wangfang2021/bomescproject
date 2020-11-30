using System;
using System.Data;
using DataAccess;

namespace Logic
{
    public class FS0202_Logic
    {
        FS0202_DataAccess fs0202_dataAccess = new FS0202_DataAccess();
        public DataTable searchHistory(string filename, string timefrom, string timeto)
        {
            DataTable dt = fs0202_dataAccess.searchHistory(filename, timefrom, timeto);
            return dt;
        }
    }
}
