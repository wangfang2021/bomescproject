using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1405_Logic
    {
        FS1405_DataAccess fs1405_DataAccess;

        public FS1405_Logic()
        {
            fs1405_DataAccess = new FS1405_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strHaoJiu, string strInOut, string strOrderPlant, string strFrom, string strTo, string strCarModel, string strSPISStatus, List<Object> listTime)
        {
            return fs1405_DataAccess.getSearchInfo(strPartId, strSupplierId, strHaoJiu, strInOut, strOrderPlant, strFrom, strTo, strCarModel, strSPISStatus, listTime);
        }
        public DataTable checksendtoInfo(List<Dictionary<string, Object>> checkedInfoData, string strOperId, string strPackingPlant, ref DataTable dtMessage)
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
        public DataTable checkadmitInfo(List<Dictionary<string, Object>> checkedInfoData, string strOperId, string strPackingPlant, ref DataTable dtMessage)
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
        public DataTable checkrejectInfo(List<Dictionary<string, Object>> checkedInfoData, string strOperId, string strPackingPlant, ref DataTable dtMessage)
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
        public void sendtoInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1405_DataAccess.setSendtoInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void admitInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1405_DataAccess.setAdmitInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void rejectInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1405_DataAccess.setRejectInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
