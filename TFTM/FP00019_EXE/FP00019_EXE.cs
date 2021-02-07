using BatchProcess;
using Common;
using System;


namespace FP00019_EXE
{
    class FP00019_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP00019";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP00019 pb = new FP00019();
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
