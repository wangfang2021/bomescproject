using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0630_Logic
    {
        FS0630_DataAccess fs0630_DataAccess;

        public FS0630_Logic()
        {
            fs0630_DataAccess = new FS0630_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcDXYM, string vcPlant, string vcCLYM)
        {
            return fs0630_DataAccess.Search(vcDXYM, vcPlant, vcCLYM);
        }
        #endregion

        #region 记录请求时间
        public void CreateView(string vcCLYM, List<string> plantList, List<string> lsdxym, string strUserId)
        {
            fs0630_DataAccess.CreateView(vcCLYM, plantList, lsdxym, strUserId);
        }
        #endregion

    }

}
