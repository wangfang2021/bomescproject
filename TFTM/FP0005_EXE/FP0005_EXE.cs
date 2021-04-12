using BatchProcess;
using Common;
using System;

namespace FP0005_EXE
{
    class FP0005_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0005";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0005 pb = new FP0005();
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
