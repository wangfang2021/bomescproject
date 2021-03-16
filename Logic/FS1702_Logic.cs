using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Logic
{
    public class FS1702_Logic
    {
        FS1702_DataAccess fs1702_DataAccess;

        public FS1702_Logic()
        {
            fs1702_DataAccess = new FS1702_DataAccess();
        }

        #region 绑定工程
        public DataTable getAllProject()
        {
            return fs1702_DataAccess.getAllProject();
        }
        #endregion

        #region 检索
        public DataTable Search(string vcProject, string dChuHeDateFrom, string dChuHeDateTo)
        {
            return fs1702_DataAccess.Search(vcProject, dChuHeDateFrom, dChuHeDateTo);
        }
        #endregion

      
    }

}
