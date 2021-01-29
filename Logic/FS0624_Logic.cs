using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0624_Logic
    {
        FS0624_DataAccess fs0624_DataAccess;

        public FS0624_Logic()
        {
            fs0624_DataAccess = new FS0624_DataAccess();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            return fs0624_DataAccess.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
            , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
            , strReceiver, strPriceState
            );
        }
        #endregion

        
    }
}
