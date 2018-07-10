using Aliyun.OSS;
using API_SERVER.Common;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class FileManager
    {
        private string path = System.Environment.CurrentDirectory + "\\fileupload";
        private string filePath = System.Environment.CurrentDirectory + "\\file";
        /// <summary>
        /// 将Base64位码保存成图片
        /// </summary>
        /// <param name="base64Img">Base64位码</param>
        /// <param name="fileName">图片名</param>
        /// <returns></returns>
        public bool saveFileByBase64String(string base64, string fileName)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                base64 = base64.Split("base64,")[1];
                byte[] bt = Convert.FromBase64String(base64);//获取图片base64
                string ImageFilePath = path + "\\" + fileName;
                File.WriteAllBytes(ImageFilePath, bt); //保存图片到服务器，然后获取路径 
                return true;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataTable readExcelFileToDataTable(string fileName)
        {
            FileInfo file = new FileInfo(path + "\\" + fileName);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    DataTable dt = new DataTable();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row==1)
                        {
                            for (int col = 1; col <= ColCount; col++)
                            {
                                dt.Columns.Add(worksheet.Cells[row, col].Value.ToString());
                            }
                        }
                        else
                        {
                            DataRow dr = dt.NewRow();
                            for (int col = 1; col <= ColCount; col++)
                            {
                                dr[col - 1] = worksheet.Cells[row, col].Value.ToString();
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet readExcelToDataSet(string fileName)
        {
            FileInfo file = new FileInfo(filePath + "\\" + fileName);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    DataSet ds = new DataSet();
                    int count = package.Workbook.Worksheets.Count;
                    for (int j = 1; j <= count; j++)
                    {
                        DataTable dt = new DataTable();
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[j];
                        dt.TableName = worksheet.Name;
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;
                        for (int row = 1; row <= rowCount; row++)
                        {
                            if (row == 1)
                            {
                                for (int col = 1; col <= ColCount; col++)
                                {
                                    dt.Columns.Add(worksheet.Cells[row, col].Value.ToString());
                                }
                            }
                            else
                            {
                                DataRow dr = dt.NewRow();
                                for (int col = 1; col <= ColCount; col++)
                                {
                                    if (worksheet.Cells[row, col].Value!=null)
                                    {
                                        dr[col - 1] = worksheet.Cells[row, col].Value.ToString();
                                    }
                                }
                                dt.Rows.Add(dr);
                            }
                        }
                        ds.Tables.Add(dt);
                    }
                    
                    return ds;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool writeDataSetToExcel(DataSet ds,string fileName)
        {
            FileInfo file = new FileInfo(path + "\\" + fileName);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(ds.Tables[i].TableName);
                        for (int j = 0; j <= ds.Tables[i].Rows.Count; j++)
                        {
                            for (int k = 0; k < ds.Tables[i].Columns.Count; k++)
                            {
                                if (j==0)
                                {
                                    worksheet.Cells[j+1, k+1].Value = ds.Tables[i].Columns[k].ColumnName;
                                }
                                else
                                {
                                    worksheet.Cells[j + 1, k + 1].Value = ds.Tables[i].Rows[j-1][k].ToString();
                                }
                            }
                        }
                    }
                    package.Save(); 
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false ;
            }
        }

        /// <summary>
        /// 上传文件到oss
        /// </summary>
        /// <param name="fileName">文件名，不带路径</param>
        /// <param name="ossDir">oss的文件夹路径</param>
        /// <returns></returns>
        private bool updateFileToOSS(string fileName,string ossDir)
        {
            try
            {
                OssClient client = OssManager.GetInstance();
                ObjectMetadata metadata = new ObjectMetadata();
                // 可以设定自定义的metadata。
                metadata.UserMetadata.Add("uname", "airong");
                metadata.UserMetadata.Add("fromfileName", fileName);
                using (var fs = File.OpenRead(path + "\\" + fileName))
                {
                    var ret = client.PutObject(Global.OssBucket, ossDir + fileName, fs, metadata);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
