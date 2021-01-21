﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Logic
{
    public class FS0503_Logic
    {
        FS0503_DataAccess fs0503_DataAccess;

        public FS0503_Logic()
        {
            fs0503_DataAccess = new FS0503_DataAccess();

        }

        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcState,  string vcPartNo, string vcCarType, string dExpectDeliveryDate,string UserId)
        {
            return fs0503_DataAccess.Search(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate, UserId);
        }

        public DataTable GetBoxType()
        {
            return fs0503_DataAccess.GetBoxType();
        }
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0503_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData,  string vcIntake, string userId)
        {
            fs0503_DataAccess.allInstall(listInfoData,  vcIntake, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0503_DataAccess.importSave(importDt, userId);
        }

        public DataTable GetTaskNum()
        {
            return fs0503_DataAccess.GetTaskNum();
        }

        public void hZZK(List<Dictionary<string, object>> listInfoData, string dExpectDeliveryDate, string userId)
        {
            fs0503_DataAccess.hZZK(listInfoData, dExpectDeliveryDate, userId);
        }

        public DataTable GetWorkArea( string supplierId)
        {
            return fs0503_DataAccess.GetWorkArea(supplierId);
        }

        public void admit(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.admit(listInfoData, userId);
        }
        public void returnHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.returnHandle(listInfoData, userId);
        }
        public void weaveHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.weaveHandle(listInfoData, userId);
        }

        public DataTable SearchByIAutoId(string strIAutoId)
        {
            return fs0503_DataAccess.SearchByIAutoId(strIAutoId); 
        }

        public bool editOk(dynamic dataForm, string userId)
        {
            return fs0503_DataAccess.editOk(dataForm, userId);
        }

        public void reply(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.reply(listInfoData, userId);
        }
    }
}
