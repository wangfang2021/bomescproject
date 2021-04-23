using BatchProcess;
using Common;
using System;


namespace FP0009_EXE
{
    class FP0009_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0009";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0009 pb = new FP0009();
                if (!pb.main("000000"))
                    iRet = Common.ComConstant.NG_CODE;
                return;
            }
            catch (Exception ex)
            {
                iRet = Common.ComConstant.NG_CODE;
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE0903", ex, "system");
            }
            finally
            {
                Environment.Exit(iRet);
            }
        }
    }
}
