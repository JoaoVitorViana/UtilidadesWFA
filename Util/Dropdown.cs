using System.Collections.Generic;
using System.Windows.Forms;

namespace UtilidadesWFA.Util
{
    public class Dropdown
    {
        public static void SetValue<T>(ComboBox comboBox, List<T> valores, string Texto = "Descricao", string Valor = "Codigo")
        {
            comboBox.DisplayMember = Texto;
            comboBox.ValueMember = Valor;
            comboBox.DataSource = valores;
        }
    }
}
