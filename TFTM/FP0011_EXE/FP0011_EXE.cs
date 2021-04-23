using BatchProcess;
using Common;
using System;


namespace FP0011_EXE
{
    class FP0011_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0011";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0011 pb = new FP0011();
                if (!pb.main("000000", ""))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1101", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
