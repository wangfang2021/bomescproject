using BatchProcess;
using Common;
using System;


namespace FP0020_EXE
{
    class FP0020_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0020";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0020 pb = new FP0020();
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
