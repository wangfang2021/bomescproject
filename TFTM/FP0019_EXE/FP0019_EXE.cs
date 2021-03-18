using BatchProcess;
using Common;
using System;


namespace FP0019_EXE
{
    class FP00019_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0019";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0019 pb = new FP0019();
                if (!pb.main("system"))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1907", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
