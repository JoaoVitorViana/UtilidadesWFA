using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace UtilidadesWFA
{
    public class Util
    {
        public static void ExporttoExcel<T>(List<T> pList)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Excel|*.xlsx";
                save.Title = "Salvar Arquivo";
                save.ShowDialog();
                if (save.FileName != "")
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(save.FileName);
                    ws.Cells["A1"].LoadFromCollection(pList, true, OfficeOpenXml.Table.TableStyles.Light18);
                    byte[] fileBytes = pck.GetAsByteArray();
                    using (var fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(fileBytes, 0, fileBytes.Length);
                    }
                    System.Diagnostics.Process.Start(save.FileName);
                }
            }
        }
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            if (data.Count > 0)
            {
                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
            }
            return table;
        }
    }
}