using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0718_Logic
    {
        FS0718_DataAccess fs0718_DataAccess;

        public FS0718_Logic()
        {
            fs0718_DataAccess = new FS0718_DataAccess();
        }

        #region 按检索条件检索,返回dt
        public DataTable Search()
        {
            return fs0718_DataAccess.Search();
        }
        #endregion

    }
}
