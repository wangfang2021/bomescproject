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

        //public void Check(DataTable excelTable,ref string refMsg)
        public void Check()
        {


            DataTable Calendar = fs0403_dataAccess.getCalendar();
            //各工厂的指定日
            Hashtable Day = fs0403_dataAccess.getDay(Calendar);

            //品番的数量
            Hashtable quantity = fs0403_dataAccess.getCount(Day);

            //获取波动率
            Hashtable ht = fs0403_dataAccess.getFluctuate();

            //获取品番对应工厂

            //for (int i = 0; i < excelTable.Rows.Count; i++)
            //{

            //}

            //无误则继续，修改soqreply,记录修改
        }

        public class Node
        {
            public string partId;
            public int excelQuantity;
            public int soqQuantity;
            public decimal allowPercent;
            public decimal realPercent;
            public string DXR;
            public bool flag;

            public Node(string partId)
            {
                this.partId = partId;
                this.flag = false;
            }

            public Node(string partId, string excelQuantity, string soqQuantity, string allowPercent, string DXR)
            {
                this.partId = partId;
                this.excelQuantity = ObjToInt(excelQuantity);
                this.soqQuantity = ObjToInt(soqQuantity);
                this.allowPercent = ObjToDecimal(allowPercent);
                this.DXR = DXR;
                this.realPercent = System.Math.Abs((this.excelQuantity - this.soqQuantity) / this.soqQuantity);
                this.flag = this.allowPercent >= this.realPercent ? true : false;
            }

            public bool isAllow()
            {
                return this.flag;
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
