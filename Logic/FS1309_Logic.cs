using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1309_Logic
    {
        FS1309_DataAccess fs1309_DataAccess;

        public FS1309_Logic()
        {
            fs1309_DataAccess = new FS1309_DataAccess();
        }
        public DataSet getSearchInfo(string strPackPlant)
        {
            return fs1309_DataAccess.getSearchInfo(strPackPlant);
        }
        public void setDisplayInfo(string strPackPlant, string strPageClientNum, string strGZTTongjiFre, string strBZLTongjiFre, string strGZTZhuangTaiFre, string strGZTQieHuanFre, string strGZTShowType
                    , string strBFromTime, string strBCross, string strBToTime, string strYFromTime, string strYCross, string strYToTime, string strOperId)
        {
            fs1309_DataAccess.setDisplayInfo(strPackPlant, strPageClientNum, strGZTTongjiFre, strBZLTongjiFre, strGZTZhuangTaiFre, strGZTQieHuanFre, strGZTShowType
                   , strBFromTime, strBCross, strBToTime, strYFromTime, strYCross, strYToTime, strOperId);
        }
    }
}
