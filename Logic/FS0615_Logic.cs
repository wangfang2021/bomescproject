using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0615_Logic
    {
        FS0615_DataAccess da;

        public FS0615_Logic()
        {
            da = new FS0615_DataAccess();

        }
        #region 检索
        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="vcOrderNo"></param>
        /// <param name="vcOrderState"></param>
        /// <returns></returns>
        public DataTable Search(string vcOrderNo, string vcOrderState)
        {
            return da.Search(vcOrderNo, vcOrderState);
        }
        #endregion

        #region 纳期确认
        /// <summary>
        /// 纳期确认
        /// </summary>
        /// <param name="vcOrderNo"></param>
        /// <param name="vcOrderState"></param>
        /// <returns></returns>
        public void dateMake(List<Dictionary<string, object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            da.dateMake(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 订单做成
        /// <summary>
        /// 订单做成
        /// </summary>
        /// <returns></returns>
        public void orderMake(List<Dictionary<string, object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            da.orderMake(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 紧急订单导航页初始化
        public void getCounts(ref int counts1, ref int counts2)
        {
            da.getCounts(ref counts1, ref counts2);
        }
        #endregion
    }
}
