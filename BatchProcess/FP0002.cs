using Common;
using System;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// 品番信息传送调达
/// </summary>
namespace BatchProcess
{
    public class FP0002
    {
        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0002";
            try
            {
                ComMessage.GetInstance().ProcessMessage(PageId,"M03PI0200", null, strUserId);







                //批处理
                ComMessage.GetInstance().ProcessMessage(PageId,"M03PI0201", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(PageId,"M03PE0200", null, strUserId);
                throw ex;
            }
        }
        #endregion
    }
}
