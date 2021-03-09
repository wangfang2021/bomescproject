using BatchProcess;
using Common;
using System;


namespace FP0017_EXE
{
    class FP0017_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0017";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0017 pb = new FP0017();
                if (!pb.main("000000"))
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
