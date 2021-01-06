using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0202_Logic
    {
        FS0202_DataAccess fs0202_dataAccess = new FS0202_DataAccess();

        #region 检索SPI上传履历

        public DataTable searchHistory(string filename, string timefrom, string timeto)
        {
            DataTable dt = fs0202_dataAccess.searchHistory(filename, timefrom, timeto);
            return dt;
        }


        #endregion

        public string getPartId(string vcCarType, string vcPart_Id, string vcParent)
        {
            FS0302_DataAccess dataAccess = new FS0302_DataAccess();
            return dataAccess.getPartId(vcCarType, vcPart_Id, vcParent);
        }
    }

}
