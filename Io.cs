using ExcelDataReader;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace UtilidadesWFA
{
    public class Io
    {
        public static string FormatarCpf(string cpf) => Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        public static string FormatarCnpj(string cnpj) => Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
        public static decimal? GetDecimalString(string pValor)
        {
            if (!string.IsNullOrWhiteSpace(pValor))
            {
                CultureInfo culture = new CultureInfo("pt-BR", true);
                return Convert.ToDecimal(pValor.Replace("R$", "").Replace(".", "").Replace(",", ".").Replace(".", ",").Trim(), culture);
            }
            return null;
        }
        public static string GetValueDatePicker(DateTimePicker txtData) => txtData.Format != DateTimePickerFormat.Custom ? txtData.Value.ToShortDateString() : null;
        public static int? GetValueComboBoxInt(ComboBox cb)
        {
            if (cb.SelectedIndex >= 0)
                return Convert.ToInt32(cb.SelectedValue);
            return null;
        }
        public static string GetValueComboBoxString(ComboBox cb) => (cb.SelectedIndex >= 0) ? cb.SelectedValue.ToString() : null;
        public static string GetValorRS(decimal? pValor) => pValor != null ? $@"{Convert.ToDouble(pValor):C}" : "";
        public static DataTable GetExcel(FileStream pStream, string pExtensao)
        {
            IExcelDataReader excelReader = null;

            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            if (pExtensao == ".xls")
                excelReader = ExcelReaderFactory.CreateBinaryReader(pStream);
            else
                //2. lendo arquivo OpenXml do Excel(2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(pStream);

            //Cria o DataSet  
            DataSet DsResultado = excelReader.AsDataSet();

            //Convert o DataSet para Datatable
            return DsResultado.Tables[0];
        }
    }
}