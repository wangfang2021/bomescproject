using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0810_Logic
    {
        FS0810_DataAccess fs0810_DataAccess;

        public FS0810_Logic()
        {
            fs0810_DataAccess = new FS0810_DataAccess();
        }

        #region 检索
        public DataTable Search(string smallpm, string sr, string pfbefore5)
        {
            return fs0810_DataAccess.Search(smallpm, sr, pfbefore5);
        }
        #endregion

        #region 保存
        public void Save(DataTable dtadd, DataTable dtmod,string strUserId)
        {
            fs0810_DataAccess.Save(dtadd,dtmod, strUserId);
        }
        #endregion

    }
}
