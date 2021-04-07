using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0100_Logic
    {
        FS0100_DataAccess fs0100_DataAccess;

        public FS0100_Logic()
        {
            fs0100_DataAccess = new FS0100_DataAccess();
        }

        #region 确认当前输入密码是否正确
        public bool checkPwd(string strUserId, string strPWD)
        {
            DataTable dt = fs0100_DataAccess.checkPwd(strUserId, strPWD);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region 修改密码
        public bool changePwd(string strUserId, string strPWD)
        {
            int count = fs0100_DataAccess.changePwd(strUserId, strPWD);
            if (count > 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
