using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Common
{
    public class ClassINI
    {
        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string def,
            StringBuilder retVal,
            int size,
            string filePath);
        public ClassINI(string INIpath)
        {
            inipath = INIpath;
        }
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            int i = GetPrivateProfileString(Section, Key, "", temp, 1024, this.inipath);
            return temp.ToString();
        }
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }
    }
}
