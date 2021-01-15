using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Common;
using DataAccess;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0308_Logic
    {
        FS0308_DataAccess fs0308_dataAccess = new FS0308_DataAccess();

        #region 检索

        public DataTable searchApi(string strYear,string Receiver)
        {
            return fs0308_dataAccess.searchApi(strYear,  Receiver);
        }

        #endregion

        #region 导入后保存

        public void importSave(DataTable dt,string receiver, string strUserId)
        {
            fs0308_dataAccess.importSave(dt, receiver, strUserId);
        }

        #endregion

        #region 保存

        public void SaveApi(List<Dictionary<string, Object>> list, string strUserId)
        {
            fs0308_dataAccess.SaveApi(list, strUserId);
        }

        #endregion

    }
}
