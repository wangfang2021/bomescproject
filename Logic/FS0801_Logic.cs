using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0801_Logic
    {
        FS0801_DataAccess fs0801_DataAccess;

        public FS0801_Logic()
        {
            fs0801_DataAccess = new FS0801_DataAccess();
        }


        #region 绑定包装厂
        public DataTable BindPlant()
        {
            return fs0801_DataAccess.BindPlant();
        }
        #endregion

    }
}
