using System.Windows.Forms;

namespace UtilidadesWFA.Util.Mask
{
    public class Telefone
    {
        public static void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9') return;
            if (e.KeyChar == '(' || e.KeyChar == ')' || e.KeyChar == '+' || e.KeyChar == '-') return;
            if (e.KeyChar == 8) return;
            e.Handled = true;
        }
    }
}
