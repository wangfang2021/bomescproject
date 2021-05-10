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

        #region 检索_FORECAST
        public DataTable Search(string vcCLYM)
        {
            return fs0612_DataAccess.Search(vcCLYM);
        }
        #endregion

        #region 检索_EKANBAN
        public DataTable Search2(string vcCLYM)
        {
            return fs0612_DataAccess.Search2(vcCLYM);
        }
        #endregion

        #region 记录请求时间
        public void CreateView(string vcCLYM, DataTable dtPlant, string strUserId)
        {
            fs0612_DataAccess.CreateView(vcCLYM, dtPlant,strUserId);
        }
        public void CreateView2(string vcCLYM, List<string> plantList, string strUserId,string strKind)
        {
            fs0612_DataAccess.CreateView2(vcCLYM, plantList, strUserId,strKind);
        }
        #endregion

        #region  取NQC处理结果
        public DataTable dtNQCReceive(string vcCLYM)
        {
            return fs0612_DataAccess.dtNQCReceive(vcCLYM);
        }
        #endregion

        #region  取最大次数的内制结果
        public DataTable GetMaxCLResult(string vcCLYM)
        {
            return fs0612_DataAccess.GetMaxCLResult(vcCLYM);
        }
        public DataTable GetMaxCLResult2(string vcCLYM)
        {
            return fs0612_DataAccess.GetMaxCLResult2(vcCLYM);
        }
        #endregion

        public DataTable getPlant(string strDXYM, string strKind)
        {
            return fs0612_DataAccess.getPlant(strDXYM, strKind);
        }
        public DataTable getPlantALL(string strDXYM)
        {
            return fs0612_DataAccess.getPlantALL(strDXYM);
        }
        public bool isHaveSoqData(string strDXYM, string strPlant, string strKind)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                int num = fs0612_DataAccess.isHaveSoqData(strDXYM, strPlant, strKind);
                if (num > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
