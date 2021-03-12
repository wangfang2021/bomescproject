using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0504_Logic
    {
        FS0504_DataAccess fS0504_DataAccess;

        public FS0504_Logic()
        {
            fS0504_DataAccess = new FS0504_DataAccess();
        }
        public DataTable getDataInfo(string strSupplierId)
        {
            return fS0504_DataAccess.getDataInfo(strSupplierId);
        }
        public void setDataInfo(string strLinId, string strOperId, ref DataTable dtMessage)
        {
            fS0504_DataAccess.setDataInfo(strLinId, strOperId,ref dtMessage);
        }
    }
}
