using BatchProcess;
using Common;
using System;

namespace FP0015_EXE
{
    class FP0015_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0015";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0015 pb = new FP0015();
                if (!pb.main("system"))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PE0200", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
