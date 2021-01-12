using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0612_Logic
    {
        FS0612_DataAccess fs0612_DataAccess;

        public FS0612_Logic()
        {
            fs0612_DataAccess = new FS0612_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcPlant, string vcCLYM)
        {
            return fs0612_DataAccess.Search(vcPlant, vcCLYM);
        }
        #endregion

        #region 记录请求时间
        public void CreateView(string vcCLYM, List<string> plantList, string strUserId)
        {
            fs0612_DataAccess.CreateView(vcCLYM, plantList,strUserId);
        }
        #endregion

    }

}
