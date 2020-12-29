using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntity
{
    public class FS0201_DataEntity
    {
        public class Entity
        {
            public string vcSPINo;
            public string dHandleTime;
            public string vcFileName;
            public List<Part> parts = new List<Part>();

        }
        public class Part
        {
            public int flag;

            public string vcPart_Id_old;
            public string BJQF;
            //---------------补给区分/品番（新）
            public string vcPart_Id_new;
            public string vcBJDiff;
            //----------------end

            public string DTQF;
            public string DTPart_Id;
            //---------------代替区分（新）
            public string vcDTDiff;
            public string vcPart_id_DT;
            //----------------end
            public string vcPartName;

            public string PFSSFrom;
            public string PFSSTo;
            //---------------品番实施时期（新）
            public string vcStartYearMonth;
            public string NPFSSTo;
            //--------------end
            public string FXQF;
            public string FXNO;

            //---------------防錆区分/防錆指示書№(新)
            public string vcFXDiff;
            public string vcFXNo;
            //---------------end

            public string vcChange;

            public string vcNewProj;
            public string vcNewProjTime;
            public string NGCSSTo;

            public string vcOldProj;
            public string OGCSSFrom;
            public string vcOldProjTime;

            public string ZSPF;

            //---------------直上品番（新）
            public string vcCZYD;
            //---------------END
            public string vcSheetName;


        }

    }
}
