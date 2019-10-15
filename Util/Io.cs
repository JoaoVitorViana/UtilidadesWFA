using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UtilidadesWFA.Util
{
    public class Io
    {
        public static bool ValidaDate(string pValor)
        {
            if (!string.IsNullOrWhiteSpace(pValor) && !pValor.Contains("  /  /"))
                return DateTime.TryParse(pValor, out _);
            return false;
        }

        public static bool ValidaPeriodo(string pDataInicial, string pDataFinal, int pPeriodo = 30)
        {
            DateTime dataInicial;
            if (!DateTime.TryParse(pDataInicial, out dataInicial))
                throw new Exception("Data inicial inválida, Verifique!");
            DateTime dataFinal;
            if (!DateTime.TryParse(pDataFinal, out dataFinal))
                throw new Exception("Data final inválida, Verifique!");

            if (dataFinal < dataInicial)
                throw new Exception("Data final maior que a inicial, Verifique!");

            if ((int)dataFinal.Subtract(dataInicial).TotalDays > pPeriodo)
                throw new Exception($"Périodo não pode ser maior que {pPeriodo} dias, Verifique!");

            return true;
        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "").Replace("_", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Replace("_", "");

            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

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

        public static bool ValidaEmail(string pEmail)
        {
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            return rg.IsMatch(pEmail);
        }
    }
}
