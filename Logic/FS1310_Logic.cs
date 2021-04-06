using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using SharpCompress.Readers;
using SharpCompress.Common;
using SharpCompress.Archives;

namespace Logic
{
    public class FS1310_Logic
    {
        FS1310_DataAccess fs1310_DataAccess;

        public FS1310_Logic()
        {
            fs1310_DataAccess = new FS1310_DataAccess();
        }
        public DataTable getSearchInfo(string strPackPlant, string strPinMu, string strPartId, string strOperImage)
        {
            DataTable dataTable = fs1310_DataAccess.getSearchInfo(strPackPlant, strPinMu, strPartId, strOperImage);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i]["vcOperImage"].ToString() == "")
                {
                    dataTable.Rows[i]["vcOperImage"] = "暂无图像.jpg";
                }
            }
            return dataTable;
        }
        public DataTable checkSaveInfo(DataTable dtImport, string strPath, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strPackPlant = dtImport.Rows[i]["vcPlant"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strOperImage = dtImport.Rows[i]["vcPicUrl"].ToString();
                    if (strPackPlant == "" || strPartId == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "缺少必填项请完善数据。";
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        if (strOperImage == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = strPartId + "包装四分图路径为空请完善数据。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (!System.IO.File.Exists(strPath + strOperImage))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = strPartId + "包装四分图路径没有图片请重新上传。";
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                dtImport.Rows[i]["vcPicUrlUUID"] = strPackPlant + strPartId + "." + strOperImage.Substring(strOperImage.LastIndexOf('.') + 1).Replace("\"", "").ToLower();// 扩展名
                            }
                        }
                    }
                }
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable checkSaveInfo(DataTable dtImport, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtCheck = getSearchInfo("", "", "", "");
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strOperImage = dtImport.Rows[i]["vcPicUrl"].ToString();
                    string strOperPath = dtImport.Rows[i]["vcPicPath"].ToString();
                    string strImageName = strOperImage.Replace("." + strOperImage.Substring(strOperImage.LastIndexOf('.') + 1).Replace("\"", ""), "");
                    if (strImageName.Length < 14)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = strOperImage + "文件名位数不足14位，无法识别。";
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        string strPackPlant = strImageName.Substring(0, strImageName.Length - 12);
                        dtImport.Rows[i]["vcPlant"] = strPackPlant;
                        string strPartId = strImageName.Substring(strImageName.Length - 12, 12);
                        dtImport.Rows[i]["vcPartId"] = strPartId;
                        if (strPackPlant == "" || strPartId == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "缺少必填项请完善数据。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            //校验品番
                            DataRow[] drCheck = dtCheck.Select("vcPackPlant='" + strPackPlant + "' and vcPartId='" + strPartId + "'");
                            if (drCheck.Length != 0)
                            {
                                if (strOperImage == "")
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = strPartId + "包装四分图路径为空请完善数据。";
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    if (!System.IO.File.Exists(strOperPath))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcMessage"] = strPartId + "包装四分图路径没有图片请重新上传。";
                                        dtMessage.Rows.Add(dataRow);
                                    }
                                    else
                                    {
                                        dtImport.Rows[i]["vcPicUrlUUID"] = strPackPlant + strPartId + "." + strOperImage.Substring(strOperImage.LastIndexOf('.') + 1).Replace("\"", "").ToLower();// 扩展名
                                    }
                                }
                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = "包装场：" + strPackPlant + "，品番：" + strPartId + "有误，请确认是否真实存在";
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                    }
                }
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1310_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                fs1310_DataAccess.deleteInfo(listInfoData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void sharpCompress(string strFileType, string strSourcesPath, string strDestPath)
        {
            try
            {
                if (strFileType.ToLower() == "rar")
                {
                    //.rar文件解压
                    using (Stream stream = File.OpenRead(strSourcesPath))
                    //using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress.rar"))
                    {
                        var reader = ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                Console.WriteLine(reader.Entry.Key);
                                reader.WriteEntryToDirectory(strDestPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                                //reader.WriteEntryToDirectory(@"C:\temp", new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            }
                        }
                    }
                }
                if (strFileType.ToLower() == "zip")
                {
                    //.zip文件解压
                    var archive = ArchiveFactory.Open(strSourcesPath);
                    //var archive = ArchiveFactory.Open(@"C:\\test.zip");
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            Console.WriteLine(entry.Key);
                            entry.WriteToDirectory(strDestPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            //entry.WriteToDirectory(@"C:\temp", new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                        }
                    }
                }
                if (strFileType.ToLower() == "7z")
                {
                    //.7z文件解压
                    var archive = ArchiveFactory.Open(strSourcesPath);
                    //var archive = ArchiveFactory.Open(@"F:\Python35-32.7z");
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            Console.WriteLine(entry.Key);
                            entry.WriteToDirectory(strDestPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            //entry.WriteToDirectory(@"C:\temp", new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
