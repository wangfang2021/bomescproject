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
        public void Check(DateTime time)
        {

            DataTable Calendar = fs0403_dataAccess.getCalendar(time);
            //各工厂的指定日
            Hashtable Day = fs0403_dataAccess.getDay(Calendar, time, 5);

            //品番的数量
            Hashtable quantity = fs0403_dataAccess.getCount(Day);

            //获取波动率
            Hashtable ht = fs0403_dataAccess.getFluctuate();

            List<FS0403_DataAccess.PartIDNode> list = new List<FS0403_DataAccess.PartIDNode>();

            //for (int i = 0; i < excelTable.Rows.Count; i++)
            //{

            //}

            //无误则继续，修改soqreply,记录修改
            fs0403_dataAccess.ChangeSoq(list);
        }

        #endregion


    }

}
