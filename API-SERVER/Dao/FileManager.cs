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
        private string path = System.Environment.CurrentDirectory+"\\fileupload";
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
    }
}
