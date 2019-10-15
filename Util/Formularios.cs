using System.Windows.Forms;

namespace UtilidadesWFA.Util
{
    public class Formularios
    {
        public static void Open(Form form) => form.ShowDialog();

        public static bool Confirm(string pMensagem, string pTitulo)
        {
            var confirmResult = MessageBox.Show(pMensagem, pTitulo, MessageBoxButtons.YesNo);
            return confirmResult == DialogResult.Yes;
        }
    }
}
