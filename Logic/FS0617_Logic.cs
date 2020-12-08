using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0617_Logic
    {
        FS0617_DataAccess fs0617_DataAccess;

        public FS0617_Logic()
        {
            fs0617_DataAccess = new FS0617_DataAccess();
        }
        public DataTable getPlantInfo()
        {
            return fs0617_DataAccess.getPlantInfo();
        }
        public DataTable getCarTypeInfo()
        {
            return fs0617_DataAccess.getCarTypeInfo();
        }
        public DataTable getRePartyInfo()
        {
            return fs0617_DataAccess.getRePartyInfo();
        }
        public DataTable getSuPartyInfo()
        {
            return fs0617_DataAccess.getSuPartyInfo();
        }
        public DataTable getSearchInfo(string strPlant, string strPartid, string strCarType, string strReParty, string strSuParty)
        {
            return fs0617_DataAccess.getSearchInfo(strPlant, strPartid, strCarType, strReParty, strSuParty);
        }
        public string getPrintFile(string strPlant, string strPartid, string strCarType, string strReParty, string strSuParty)
        {
            return "";
        }
        
    }
}
