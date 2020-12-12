﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0621_Logic
    {
        FS0621_DataAccess fs0621_DataAccess;

        public FS0621_Logic()
        {
            fs0621_DataAccess = new FS0621_DataAccess();

        }

        public DataTable Search(string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcCarType)
        {
            return fs0621_DataAccess.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0621_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0621_DataAccess.isExistModData(dtamod);
        }

        public DataTable BindInsideOutsideType()
        {
            throw new NotImplementedException();
        }

        public DataTable BindInjectionFactory()
        {
            throw new NotImplementedException();
        }

        public DataTable BindConsignee()
        {
            throw new NotImplementedException();
        }
    }
}
