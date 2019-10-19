using System;
using System.Windows.Forms;

namespace UtilidadesWFA.Mask
{
	public class Dinheiro
	{
		public static void KeyPress(KeyPressEventArgs evento, TextBox campo)
		{
			if (!Char.IsDigit(evento.KeyChar) && evento.KeyChar != Convert.ToChar(Keys.Back))
				evento.Handled = evento.KeyChar != ',' || campo.Text.Contains(",");
		}

		public static void Leave(TextBox campo)
		{
			string valor = campo.Text.Replace("R$", "");
			campo.Text = $@"{Convert.ToDouble(valor):C}";
		}

		public static void KeyUp(TextBox campo)
		{
			string valor = campo.Text.Replace("R$", "").Replace(",", "").Replace(" ", "").Replace("00,", "");
			if (valor.Length == 0)
				campo.Text = "0,00" + valor;
			if (valor.Length == 1)
				campo.Text = "0,0" + valor;
			if (valor.Length == 2)
				campo.Text = "0," + valor;
			else if (valor.Length >= 3)
			{
				if (campo.Text.StartsWith("0,"))
					campo.Text = valor.Insert(valor.Length - 2, ",").Replace("0,", "");
				else if (campo.Text.Contains("00,"))
					campo.Text = valor.Insert(valor.Length - 2, ",").Replace("00,", "");
				else
					campo.Text = valor.Insert(valor.Length - 2, ",");
			}
			valor = campo.Text;
			campo.Text = $@"{Convert.ToDouble(valor):C}";
			campo.Select(campo.Text.Length, 0);
		}
	}
}
