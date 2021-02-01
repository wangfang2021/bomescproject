using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using DataEntity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0616_Logic
    {
        private MultiExcute excute = new MultiExcute();
        FS0616_DataAccess fs0616_DataAccess = new FS0616_DataAccess();
        FS0625_Logic fs0625_Logic = new FS0625_Logic();

        public DataTable getOrderNoList()
        {
            try
            {
                return fs0616_DataAccess.getOrderNoList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strDelete, string strState, List<Object> listOrderNo, string strPartId, string strOrderPlant, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant)
        {
            string strOrderNoList = "";
            if (listOrderNo.Count != 0)
            {

                strOrderNoList += "select '";
                for (int i = 0; i < listOrderNo.Count; i++)
                {

                    strOrderNoList += listOrderNo[i].ToString();
                    if (i < listOrderNo.Count - 1)
                    {
                        strOrderNoList += "' union select '";
                    }
                    else
                    {
                        strOrderNoList += "'";
                    }
                }
            }
            DataTable dataTable = fs0616_DataAccess.getSearchInfo(strDelete, strState, strOrderNoList, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
            return dataTable;
        }

        public DataTable checkSaveInfo(List<Dictionary<string, Object>> listInfoData,DataTable dataTable, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = new DataTable();

                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport,string strOperId,ref DataTable dtMessage)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkReplyInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = new DataTable();

                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setReplyInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkOpenInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = new DataTable();

                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setOpenInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkOutputInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = new DataTable();

                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setOutputInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
