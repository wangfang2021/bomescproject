using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
  public class P00004_Logic
  {
    static P00004_DataAccess P00004_DataAccess = new P00004_DataAccess();







    public DataTable GetShipData(string dock)
    {
      return P00004_DataAccess.GetShipData(dock);
    }

    public static DataTable ValidateQB(string caseNo)
    {
      return P00004_DataAccess.ValidateQB(caseNo);
    }

    public static DataTable ValidateQB1(string caseNo)
    {
      return P00004_DataAccess.ValidateQB1(caseNo);
    }



    public static DataTable ValidateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00004_DataAccess.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public static DataTable GetPlant()
    {
      return P00004_DataAccess.GetPlant();
    }

    public static DataTable ValidateData(string partId, string scanTime, string dock)
    {
      return P00004_DataAccess.ValidateData(partId, scanTime, dock);
    }

    public static DataTable ValidateOrd(string partId)
    {
      return P00004_DataAccess.ValidateOrd(partId);
    }

    public static DataTable ValidateSeqNo(string packingSpot, string formatServerTime, string tmpString)
    {
      return P00004_DataAccess.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
    }

    public DataTable GetDockInfo(string opearteId)
    {
      return P00004_DataAccess.getDockInfo(opearteId);
    }

    public DataTable GetData(string dock, string fork)
    {
      return P00004_DataAccess.GetData(dock, fork);
    }

    public static DataTable GetToolInfo(string sellNo)
    {
      return P00004_DataAccess.GetToolInfo(sellNo);
    }

    public int UpdateDock2(string dock, string fork, string opearteId, string serverTime)
    {
      return P00004_DataAccess.UpdateDock2(dock, fork, opearteId, serverTime);
    }

    public DataTable ValidateDock(string dock, string fork)
    {
      return P00004_DataAccess.ValidateDock(dock, fork);
    }

    public int InsertDock(string dock, string fork, string scanTime, string opearteId)
    {
      return P00004_DataAccess.InsertDock(dock, fork, scanTime, opearteId);
    }

    public DataTable ValidateDock1(string dock)
    {
      return P00004_DataAccess.ValidateDock1(dock);
    }

    public int UpdateDock(string dock, string fork, string scanTime, string opearteId)
    {
      return P00004_DataAccess.UpdateDock(dock, fork, scanTime, opearteId);
    }

    public static DataTable GetCaseSum(string dock)
    {
      return P00004_DataAccess.GetCaseSum(dock);
    }

    public static int InsertSeqNo(string packingSpot, string formatServerTime, string tmpString)
    {
      return P00004_DataAccess.InsertSeqNo(packingSpot, formatServerTime, tmpString);
    }

    public static DataTable GetSellInfo(string caseNo)
    {
      return P00004_DataAccess.GetSellInfo(caseNo);
    }

    public DataTable GetSellInfo2(string caseNo)
    {
      return P00004_DataAccess.GetSellInfo2(caseNo);
    }

    public DataTable GetSellInfo1(string caseNo)
    {
      return P00004_DataAccess.GetSellInfo1(caseNo);
    }

    public static DataTable GetSellData(string timeFrom, string timeEnd, string type, string date, string banZhi)
    {
      return P00004_DataAccess.GetSellData(timeFrom, timeEnd, type, date, banZhi);
    }

    public static DataTable GetQBData(string inputNo)
    {
      return P00004_DataAccess.GetQBData(inputNo);
    }

    public int UpdateDock1(string dock, string fork, string scanTime, string opearteId)
    {
      return P00004_DataAccess.UpdateDock1(dock, fork, scanTime, opearteId);
    }

    public DataTable ValidateShip(string fork, string dock)
    {
      return P00004_DataAccess.ValidateShip(fork, dock);
    }

    public static DataTable ValidateRinv(string bzPlant, string partId, string cpdCompany)
    {
      return P00004_DataAccess.ValidateRinv(bzPlant, partId, cpdCompany);
    }

    public int UpdateSeqNo(string packingSpot, string formatServerTime, string shpSqnNoNew, string tmpString)
    {
      return P00004_DataAccess.UpdateSeqNo(packingSpot, formatServerTime, shpSqnNoNew, tmpString);
    }

    public static int UpdateRinv(string bzPlant, string partId, string cpdCompany, int v)
    {
      return P00004_DataAccess.UpdateRinv(bzPlant, partId, cpdCompany, v);
    }

    public static DataTable ValidateOpr(string caseNo, string caseNo1)
    {
      return P00004_DataAccess.ValidateOpr(caseNo);
    }





    public DataTable ValidateParts(string caseNo)
    {
      return P00004_DataAccess.ValidateParts(caseNo);
    }

    public static int UpdateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string serverTime)
    {
      return P00004_DataAccess.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, serverTime);
    }

    public static DataTable ValidatePrice(string partId, string scanTime)
    {
      return P00004_DataAccess.ValidatePrice(partId, scanTime);
    }

    public static int Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, string iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packingQuatity, string lblStart, string lblEnd, string supplierId, string supplierPlant, string lotId, string inoutFlag, string checkType, string caseNo, string dockSell)
    {
      return P00004_DataAccess.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packingQuatity, lblStart, lblEnd, supplierId, supplierPlant, lotId, inoutFlag, checkType, caseNo, dockSell);
    }

    public DataTable GetPlantCode(string partsNoFirst, string serverTime)
    {
      return P00004_DataAccess.GetPlantCode(partsNoFirst, serverTime);
    }

    public void DelData(string dockSell)
    {
      P00004_DataAccess.DelData(dockSell);
    }

    public static int InsertOpr(string bzPlant, string inoNo, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplier_id, string supplierGQ, string serverTime, string quantity, string bZUnit, string cpdCompany, string dock, string checkType, string labelStart, string labelEnd, string caseNo, string checkStatus, string sellNo)
    {
      return P00004_DataAccess.InsertOpr(bzPlant, inoNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, serverTime, quantity, bZUnit, cpdCompany, dock, checkType, labelStart, labelEnd, caseNo, checkStatus, sellNo);
    }



    public static int Truncate()
    {
      return P00004_DataAccess.Truncate();
    }

    public static int UpdateOrd(string targetMonth, string orderNo, string seqNo, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v10, int v11, int v12, int v13, int v14, int v15, int v16, int v17, int v18, int v19, int v20, int v21, int v22, int v23, int v24, int v25, int v26, int v27, int v28, int v29, int v30, int v31, int newSum, string partId)
    {
      return P00004_DataAccess.UpdateOrd(targetMonth, orderNo, seqNo, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15, v16, v17, v18, v19, v20, v21, v22, v23, v24, v25, v26, v27, v28, v29, v30, v31, newSum, partId);
    }

    public DataTable ValidateInv(string partId, string kanbanOrderNo, string kanbanSerial)
    {
      return P00004_DataAccess.ValidateInv(partId, kanbanOrderNo, kanbanSerial);
    }

    public static DataTable GetPartsName(string serverTime, string partId)
    {
      return P00004_DataAccess.GetPartsName(serverTime, partId);
    }

    public int InsertShip(string cpdName, string cpdCompany, string packingSpot, string sellNo, string partId, string orderNo, string seqNo, string quantity, string caseNo, string partsName, string opearteId, string iP, string partsNameCn, string price)
    {
      return P00004_DataAccess.InsertShip(cpdName, cpdCompany, packingSpot, sellNo, partId, orderNo, seqNo, quantity, caseNo, partsName, opearteId, iP, partsNameCn, price);
    }

    public static DataTable GetQBData1(string dockSell)
    {
      return P00004_DataAccess.GetQBData1(dockSell);
    }

    public static int InsertPrint(string opearteId, string iP, string serverTime, string sellNo)
    {
      return P00004_DataAccess.InsertPrint(opearteId, iP, serverTime, sellNo);
    }

    public static int DelCase(string caseNo)
    {
      return P00004_DataAccess.DelCase(caseNo);
    }

    public DataTable GetSeqNo(string tmpString, string formatDate)
    {
      return P00004_DataAccess.GetSeqNo(tmpString, formatDate);
    }

    public static int InsertSeqNo(string tmpString, string formatDate)
    {
      return P00004_DataAccess.InsertSeqNo(tmpString, formatDate);
    }

    public int InsertTool(string sellNo, string opearteId, string scanTime, string hUQuantity, string hUQuantity1, string bPQuantity, string pCQuantity, string cBQuantity, string bianCi)
    {
      return P00004_DataAccess.InsertTool(sellNo, opearteId, scanTime, hUQuantity, hUQuantity1, bPQuantity, pCQuantity, cBQuantity, bianCi);
    }

    public int UpdateSeqNo(int seqNoNew, string formatDate, string tmpString)
    {
      return P00004_DataAccess.UpdateSeqNo(seqNoNew, formatDate, tmpString);
    }

    public DataTable ValidateOpr1(string inputNo, string caseNo)
    {
      return P00004_DataAccess.ValidateOpr1(inputNo, caseNo);
    }

    public DataTable GetOprData(string caseNo, string inputNo)
    {
      return P00004_DataAccess.GetOprData(caseNo, inputNo);
    }

    public int InsertSj(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuatity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string checkStatus, string caseNo, string sellNo)
    {
      return P00004_DataAccess.InsertSj(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuatity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, checkStatus, caseNo, sellNo);
    }

    public int InsertSell(string seqNo, string sellNo, string truckNo, string cpdCompany, string partId, string kanbanOrderNo, string kanbanSerial, string invoiceNo, string caseNo, string partsNameEn, string quantity, string bianCi, string opearteId, string scanTime, string supplierId, string lblStart, string lblEnd, string price)
    {
      return P00004_DataAccess.InsertSell(seqNo, sellNo, truckNo, cpdCompany, partId, kanbanOrderNo, kanbanSerial, invoiceNo, caseNo, partsNameEn, quantity, bianCi, opearteId, scanTime, supplierId, lblStart, lblEnd, price);
    }

    public int UpdateDock(string dockSell, string opearteId, string serverTime)
    {
      return P00004_DataAccess.UpdateDock(dockSell, opearteId, serverTime);
    }

    public int UpdateInv1(string partId, string kanbanOrderNo, string kanbanSerial, string quantity)
    {
      return P00004_DataAccess.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);
    }

    public int UpdateShip(string dockSell, string opearteId, string serverTime)
    {
      return P00004_DataAccess.UpdateShip(dockSell, opearteId, serverTime);
    }

    public DataTable ValidateOrd1(string partId)
    {
      return P00004_DataAccess.ValidateOrd1(partId);
    }

    public DataTable GetCount(string partId)
    {
      return P00004_DataAccess.GetCount(partId);
    }

    public DataTable ValiateOrd2(string partId)
    {
      return P00004_DataAccess.ValiateOrd2(partId);
    }

    public int InsertSum(string seqNo, string sellNo, string truckNo, int caseSum, string bianCi, string opearteId, string serverTime, string date, string banzhi, string qianFen)
    {
      return P00004_DataAccess.InsertSum(seqNo, sellNo, truckNo, caseSum, bianCi, opearteId, serverTime, date, banzhi, qianFen);
    }

    public int SendMail()
    {
      MailMessage mailMsg = new MailMessage();//实例化对象
      mailMsg.From = new MailAddress("fqm_wufan@tftm.com.cn", "laowu");//源邮件地址和发件人
      mailMsg.To.Add(new MailAddress("1195800598@qq.com"));//收件人地址
      mailMsg.Subject = "邮件发送测试";//发送邮件的标题
      StringBuilder sb = new StringBuilder();
      sb.Append("测试测试测试测试");

      mailMsg.Body = sb.ToString();//发送邮件的内容
                                   //指定smtp服务地址（根据发件人邮箱指定对应SMTP服务器地址）
      SmtpClient client = new SmtpClient();//格式：smtp.126.com  smtp.164.com
      client.Host = "casnlb.tftm.com.cn";
      //要用587端口
      client.Port = 587;//端口
                        //加密
      client.EnableSsl = true;
      //通过用户名和密码验证发件人身份
      client.Credentials = new NetworkCredential("fqm_wufan@tftm.com.cn", "1195800598wf"); // 
                                                                                           //发送邮件
      try
      {
        client.Send(mailMsg);
      }
      catch (SmtpException ex)
      {

      }
      return 1;
    }



    public DataTable GetData1(string sellNo)
    {
      return P00004_DataAccess.GetData1(sellNo);
    }

    public string DataTableToExcel(string[] head, string[] field, DataTable dt, string rootPath, string opearteId, string v2, ref string msg, string sellNo)
    //head, field, getData, ".", opearteId, "P00001", ref msg
    {
      bool result = false;
      string RetMsg = "";
      FileStream fs = null;
      int size = 1048576 - 1;

      string strFileName = "INVINTERFACE_" + sellNo + "_APC06_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
      string fileSavePath = @"C:\Users\Administrator\Desktop\laowu 0531修改\打印程序\FILE\";//文件临时目录，导入完成后 删除

      string path = fileSavePath + strFileName;


      if (System.IO.File.Exists(path))
      {
        System.IO.File.Delete(path);
      }
      try
      {
        if (dt != null && dt.Rows.Count > 0)
        {
          int page = dt.Rows.Count / size;
          IWorkbook workbook = new XSSFWorkbook();
          for (int i = 0; i < page + 1; i++)
          {
            string sheetname = "Sheet" + (i + 1).ToString();
            ISheet sheet = workbook.CreateSheet(sheetname);
            int rowCount = dt.Rows.Count - i * size > size ? size : dt.Rows.Count - i * size;//行数  
            int columnCount = dt.Columns.Count;//列数  

            //设置列头  
            IRow row = sheet.CreateRow(0);
            ICell cell;
            for (int h = 0; h < head.Length; h++)
            {
              cell = row.CreateCell(h);
              cell.SetCellValue(head[h]);
            }
            List<ICellStyle> styles = new List<ICellStyle>();
            //设置每列单元格属性
            for (int h = 0; h < field.Length; h++)
            {
              Type type = dt.Columns[field[h]].DataType;
              ICellStyle dateStyle = workbook.CreateCellStyle();
              IDataFormat dataFormat = workbook.CreateDataFormat();
              if (type == Type.GetType("System.DateTime"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("yyyy-m-d hh:mm:ss");
              }
              else if (type == Type.GetType("System.Decimal"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("General");
              }
              else if (type == Type.GetType("System.Int32"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("General");
              }
              else if (type == Type.GetType("System.Int16"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("General");
              }
              else if (type == Type.GetType("System.Int64"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("General");
              }
              else if (type == Type.GetType("System.String") && field[h].StartsWith("d") && !field[h].StartsWith("dec"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("@");
              }
              else if (type == Type.GetType("System.String") && field[h].StartsWith("vc"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("@");
              }
              else if (type == Type.GetType("System.String"))
              {
                dateStyle.DataFormat = dataFormat.GetFormat("General");
              }
              styles.Add(dateStyle);
            }
            //设置每行每列的单元格,  
            for (int j = 0; j < rowCount; j++)
            {
              row = sheet.CreateRow(j + 1);
              for (int l = 0; l < field.Length; l++)
              {
                Type type = dt.Columns[field[l]].DataType;
                cell = row.CreateCell(l);//excel第二行开始写入数据 
                cell.CellStyle = styles[l];
                if (type == Type.GetType("System.Decimal"))
                {
                  if (dt.Rows[j][field[l]].ToString().Trim() != "")
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j + i * size][field[l]].ToString()));
                }
                else if (type == Type.GetType("System.Int32"))
                {
                  if (dt.Rows[j][field[l]].ToString().Trim() != "")
                    cell.SetCellValue(Convert.ToInt32(dt.Rows[j + i * size][field[l]].ToString()));
                }
                else if (type == Type.GetType("System.Int16"))
                {
                  if (dt.Rows[j][field[l]].ToString().Trim() != "")
                    cell.SetCellValue(Convert.ToInt16(dt.Rows[j + i * size][field[l]].ToString()));
                }
                else if (type == Type.GetType("System.Int64"))
                {
                  if (dt.Rows[j][field[l]].ToString().Trim() != "")
                    cell.SetCellValue(Convert.ToInt64(dt.Rows[j + i * size][field[l]].ToString()));
                }
                else
                {
                  cell.SetCellValue(dt.Rows[j + i * size][field[l]].ToString());
                }
                //cell.SetCellValue(dt.Rows[j + i * size][field[l]].ToString());
              }
            }
            using (fs = File.OpenWrite(path))
            {
              workbook.Write(fs);//向打开的这个xls文件中写入数据  
            }
          }
          result = true;
        }
        else
        {
          RetMsg = "传入数据为空。";
        }

        return strFileName;
      }
      catch (Exception ex)
      {
        if (fs != null)
        {
          fs.Close();
        }
        Console.WriteLine(ex.Message);
        RetMsg = "导出文件失败";
        return "";
      }
    }



    public DataTable GetBanZhi(string serverTime)
    {
      return P00004_DataAccess.GetBanZhi(serverTime);
    }

    public DataTable GetCode()
    {
      return P00004_DataAccess.GetCode();
    }

    public DataTable GetDataInfo(string sellNo, string bianCi)
    {
      return P00004_DataAccess.GetDataInfo(sellNo, bianCi);
    }

    public DataTable GetSellInfo3(string sellNo, string bianCi)
    {
      return P00004_DataAccess.GetSellInfo3(sellNo, bianCi);
    }

    public int InsertShip1(string cpdCompany, string packingSpot, string sellNo, string partId, string invoiceNo, string seqNo, string quantity, string caseNo, string partsNameEn, string opearteId, string iP, string partsNameCn, string price, string serverTime, string supplierId)
    {
      return P00004_DataAccess.InsertShip1(cpdCompany, packingSpot, sellNo, partId, invoiceNo, seqNo, quantity, caseNo, partsNameEn, opearteId, iP, partsNameCn, price, serverTime, supplierId);
    }

    public int UpdateQB(string dockSell)
    {
      return P00004_DataAccess.UpdateQB(dockSell);
    }
  }
}
