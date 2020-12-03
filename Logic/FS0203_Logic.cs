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
        public DataTable searchHistory(int flag, string UploadTime)
        {
            return fs0203_dataAccess.searchHistory(flag, UploadTime);
        }

        public void addPartList(string path, string fileName, string userId)
        {
            try
            {
                fs0203_dataAccess.addPartList(path, fileName, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
