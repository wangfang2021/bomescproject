using BatchProcess;
using Common;
using System;

namespace FP0302_EXE
{
    class FP0302_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0302";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0302 pb = new FP0302();
                if (!pb.main("000000", 0))
                    iRet = Common.ComConstant.NG_CODE;
                if (!pb.main("000000", 1))
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
