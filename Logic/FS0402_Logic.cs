using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0402_Logic
    {
        FS0402_DataAccess fs0402_DataAccess = new FS0402_DataAccess();

        #region 按检索条件检索,返回dt
        public DataTable Search(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            return fs0402_DataAccess.Search(varDxny, varDyzt, varHyzt, PARTSNO);
        }
        #endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            return fs0402_DataAccess.Cr(varDxny, varDyzt, varHyzt, PARTSNO);
        }
        #endregion
    }
}
