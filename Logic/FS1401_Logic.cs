using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1401_Logic
    {
        FS1401_DataAccess fs1401_DataAccess;

        public FS1401_Logic()
        {
            fs1401_DataAccess = new FS1401_DataAccess();
        }
        public DataSet getDllOptionsList()
        {
            return fs1401_DataAccess.getDllOptionsList();
        }
        public DataTable getSearchInfo(string strPartNo, string strSuplier, string strHJ, string strInOut, string strPartArea, string strSPISqufen, string strCheckP,
                   string strTimeFrom, string strTimeTo, string strCarFamily, string strSPISInPut,
                   string strcboxnow, string strcboxtom, string strcboxyes)
        {
            return fs1401_DataAccess.getSearchInfo(strPartNo, strSuplier, strHJ, strInOut, strPartArea, strSPISqufen, strCheckP,
                    strTimeFrom, strTimeTo, strCarFamily, strSPISInPut,
                    strcboxnow, strcboxtom, strcboxyes);
        }
    }
}
