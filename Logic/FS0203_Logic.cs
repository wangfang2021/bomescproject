using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using DataAccess;

namespace Logic
{
    public class FS0203_Logic
    {
        FS0203_DataAccess fs0203_dataAccess = new FS0203_DataAccess();
        public DataTable searchApi(int flag, string UploadTime)
        {
            return fs0203_dataAccess.searchApi(flag, UploadTime);
        }

        public void importPartList(List<Hashtable> list, string fileName, string userId)
        {
            try
            {
                fs0203_dataAccess.importPartList(list, fileName, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Hashtable> GetPartFromFile(string path)
        {
            return fs0203_dataAccess.GetPartFromFile(path);
        }


    }
}
