using System.Windows.Forms;

namespace UtilidadesWFA.Mask
{
    public class Numeros
    {
        public static void KeyPress(object sender, KeyPressEventArgs evento)
        {
            if ((evento.KeyChar < 48 || evento.KeyChar > 57) && evento.KeyChar != 8 && evento.KeyChar != 46)
            {
                evento.Handled = true;
                return;
            }

            if (evento.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(evento.KeyChar) != -1)
                    evento.Handled = true;
            }
        }
    }
}
