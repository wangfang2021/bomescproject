using BatchProcess;
using Common;
using System;

namespace FP0023_EXE
{
    public class FP0023_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0023";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0023 pb = new FP0023();
                if (!pb.main())
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE2300", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
