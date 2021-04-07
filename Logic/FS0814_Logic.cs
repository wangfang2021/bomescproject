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
        public void importSave_Sub(DataTable dt, string strUserId,ref string strErrorName)
        {
            fs0814_DataAccess.importSave_Sub(dt, strUserId,ref strErrorName);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorName)
        {
            fs0814_DataAccess.Save(listInfoData, strUserId,ref strErrorName);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0814_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 对象年月+白夜 不能重复
        public bool RepeatCheck(string vcYearMonth, string vcType)
        {
            int num = fs0814_DataAccess.RepeatCheck(vcYearMonth, vcType);
            if (num > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 
    }
}
