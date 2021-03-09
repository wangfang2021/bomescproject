using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0624_Logic
    {
        FS0624_DataAccess fs0624_DataAccess;

        public FS0624_Logic()
        {
            fs0624_DataAccess = new FS0624_DataAccess();
        }


        #region 按检索条件检索,返回dt 
        public DataTable Search(string strChangeDateFrom, string strChangeDateTo, string strChangeNo, string strState, string strOrderNo)
        {
            return fs0624_DataAccess.Search(strChangeDateFrom, strChangeDateTo, strChangeNo, strState, strOrderNo);
        }
        #endregion


        #region 按检索条件检索,返回dt 
        public DataTable SearchDetial(string strChangeNo)
        {
            return fs0624_DataAccess.SearchDetial(strChangeNo);
        }
        #endregion
    }
}
