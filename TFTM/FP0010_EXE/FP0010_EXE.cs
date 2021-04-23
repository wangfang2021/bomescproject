using BatchProcess;
using Common;
using System;


namespace FP0010_EXE
{
    class FP0010_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0010";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0010 pb = new FP0010();
                if (!pb.main("000000"))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1001", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
