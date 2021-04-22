using BatchProcess;
using Common;
using System;


namespace FP0033_EXE
{
    class FP0033_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0033";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0033 pb = new FP0033();
                if (!pb.main("000000"))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE3301", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
