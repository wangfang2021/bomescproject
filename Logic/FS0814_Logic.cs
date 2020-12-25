using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0814_Logic
    {
        FS0814_DataAccess fs0814_DataAccess;

        public FS0814_Logic()
        {
            fs0814_DataAccess = new FS0814_DataAccess();
        }

        #region 检索
        public DataTable Search(string strYearMonth)
        {
            return fs0814_DataAccess.Search(strYearMonth);
        }
        #endregion

        #region 导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            fs0814_DataAccess.importSave_Sub(dt, strUserId);
        }
        #endregion

    }
}
