using BatchProcess;
using Common;
using System;


namespace FP0021_EXE
{
    class FP0021_EXE
    {
        [STAThread]
        static void Main(string[] args)
        {
            string PageId = "FP0021";
            int iRet = Common.ComConstant.OK_CODE;
            try
            {
                FP0021 pb = new FP0021();
                if (!pb.main("000000", "1"))
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
