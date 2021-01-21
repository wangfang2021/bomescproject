using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Logic
{
    public class FS0614_Logic
    {
        FS0614_DataAccess fs0614_DataAccess = new FS0614_DataAccess();

        public bool CreateOrder(string type, string inout, string path, string userId, ref string msg)
        {
            return fs0614_DataAccess.CreateOrder(type, inout, path, userId, ref msg);
        }

    }
}
