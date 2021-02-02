using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0403_Logic
    {
        FS0403_DataAccess fs0403_dataAccess = new FS0403_DataAccess();

        #region check

        public void Check(DataTable excelTable)
        {
            DataTable Calendar = fs0403_dataAccess.getCalendar();
            Hashtable Day = fs0403_dataAccess.getDay(Calendar);
            DataTable dt = fs0403_dataAccess.getCount(Day);
            Hashtable ht = fs0403_dataAccess.getFluctuate();

        }

        public class Node
        {
            public string partId;
            public int excelQuantity;
            public int soqQuantity;
            public decimal allowPercent;
            public decimal realPercent;
            public bool flag;

            public Node(string partId, string excelQuantity, string soqQuantity, string allowPercent)
            {
                this.partId = partId;
                this.excelQuantity = ObjToInt(excelQuantity);
                this.soqQuantity = ObjToInt(soqQuantity);
                this.allowPercent = ObjToDecimal(allowPercent);

                this.realPercent = (this.excelQuantity - this.soqQuantity) / this.soqQuantity;
                this.flag = this.allowPercent >= this.realPercent ? true : false;
            }
        }
        #endregion

        public static int ObjToInt(Object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static decimal ObjToDecimal(Object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }

}
