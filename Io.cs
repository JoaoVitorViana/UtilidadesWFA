using System;
using System.Globalization;
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
    }
}