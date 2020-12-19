using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;
using DataEntity;

namespace Logic
{
    public class FS0602_Logic
    {
        FS0602_DataAccess fs0602_DataAccess = new FS0602_DataAccess();

        #region 按检索条件检索,返回dt
        public DataTable Search(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Search(searchForm);
        }
        #endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            return fs0602_DataAccess.Cr(varDxny, varDyzt, varHyzt, PARTSNO);
        }
        #endregion

        #region 展开。将初版SOQ数据复制到调整后SOQ，并改变对应状态
        public int Zk(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Zk(searchForm);
        }
        #endregion

        #region 回复。改变合意状态
        public int Hf(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Hf(searchForm);
        }
        #endregion

        #region 退回内示。删除该对象月3个月所有SOQ数据，并将soq履历表中的状态改为退回。
        public int thns(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.thns(searchForm);
        }
        #endregion
    }
}
