using BatchProcess;
using Common;
using System;

namespace FP0022_EXE
{
    public class FP0022_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0022";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0022 pb = new FP0022();
                if (!pb.main("000000"))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PE2000", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
