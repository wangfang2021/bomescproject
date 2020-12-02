using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0104_Logic
    {
        FS0104_DataAccess fs0104_DataAccess;

        public FS0104_Logic()
        {
            fs0104_DataAccess = new FS0104_DataAccess();
        }


        #region 按检索条件检索,返回
        public DataTable Search( string vcFunctionID, string vcLogType,string  vcTimeFrom,string vcTimeTo)
        {
            return fs0104_DataAccess.Search(vcFunctionID, vcLogType, vcTimeFrom, vcTimeTo);
        }
        #endregion
    }
}
