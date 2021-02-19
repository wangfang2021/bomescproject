using BatchProcess;
using Common;
using System;

namespace FP0007_EXE
{
    public class FP0007_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0007";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0007 pb = new FP0007();
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
