using System.Collections.Generic;
using System.Data;

namespace DataEntity
{
  public class P00001_DataEntity
  {
    public DataTable trolley { get; set; }

    public DataTable kanban { get; set; }

    public string kanbanSum1 { get; set; }

    public string lotId { get; set; }


    public string trolley1 { get; set; }


    public int trolleySum { get; set; }
    public int kanbanSum { get; set; }

    public string delRsult { get; set; }

    public string packingQuantity { get; set; }



    public string partId { get; set; }


    public string dock { get; set; }


    public string input { get; set; }


    public string check { get; set; }


    public string pack { get; set; }


    public string output { get; set; }



    public string quantity { get; set; }


    public string kanbanOrderNo { get; set; }



    public string kanbanSerial { get; set; }



    public string trolleySeqNo { get; set; }


    public string result { get; set; }



    /// <summary>
    /// 扫描信息
    /// </summary>
    public struct ScanData    //2013
    {
      /// <summary>
      /// 厂家
      /// </summary>
      public string SUPPLIER_CODE;       //厂家
      /// <summary>
      /// 工区
      /// </summary>
      public string SUPPLIER_PLANT;      //工区
      /// <summary>
      /// 出荷厂
      /// </summary>
      public string SHIPPING_DOCK;       //出荷厂
      /// <summary>
      /// 纳受领书号
      /// </summary>
      public string INVOICE_NO; //纳受领书号
      /// <summary>
      /// 订单号
      /// </summary>
      public string ORDER_NO;        //订单号
      /// <summary>
      /// 所番地
      /// </summary>
      public string KNBN_PRN_ADDRESS;  //所番地
      /// <summary>
      /// 品番
      /// </summary>
      public string PART_NO;       //品番
      /// <summary>
      /// 受入
      /// </summary>
      public string DOCK_CODE;    //受入
      /// <summary>
      /// 顺番号
      /// </summary>
      public string SERIAL_NO;   //顺番号 
      /// <summary>
      /// 背番
      /// </summary>
      public string KNBN_NO;     //背番
      /// <summary>
      /// 链号
      /// </summary>
      public string PLANE_NO;    //链号

      public bool clear()
      {
        SUPPLIER_CODE = "";       //厂家
        SUPPLIER_PLANT = ""; ;      //工区
        SHIPPING_DOCK = ""; ;       //出荷厂
        INVOICE_NO = ""; ; //纳受领书号
        ORDER_NO = ""; ;        //订单号
        KNBN_PRN_ADDRESS = ""; ;  //所番地
        PART_NO = ""; ;       //品番
        DOCK_CODE = ""; ;    //受入
        SERIAL_NO = ""; ;   //顺番号 
        KNBN_NO = ""; ;     //背番
        PLANE_NO = ""; ;
        return true;
      }

      public override string ToString()
      {
        return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", SUPPLIER_CODE, SUPPLIER_PLANT, SHIPPING_DOCK, DOCK_CODE, ORDER_NO, KNBN_NO, SERIAL_NO, PART_NO);  //2013
      }
    }


  }
}
