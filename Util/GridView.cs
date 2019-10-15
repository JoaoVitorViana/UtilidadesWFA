using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UtilidadesWFA.Util
{
	public class GridView
	{
		public static void Layout(DataGridView dtGrid, int pTamanhoFonte = 12)
		{
			dtGrid.DefaultCellStyle.Font = new Font("Calibri", pTamanhoFonte);
			dtGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", pTamanhoFonte);
		}

		public static string ClickEdit(object sender, DataGridViewCellEventArgs e, DataGridView dtGrid, int index = 0)
		{
			var senderGrid = (DataGridView)sender;
			if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
				return dtGrid.Rows[e.RowIndex].Cells[index].Value.ToString();

			return null;
		}

        public static List<string> GetCheckItens(DataGridView dtGrid, int indexCheckBox = 0, int indexValor = 1)
        {
            var ret = new List<string>();
            foreach (DataGridViewRow row in dtGrid.Rows)
            {
                DataGridViewCheckBoxCell item = (DataGridViewCheckBoxCell)row.Cells[indexCheckBox];
                if (item.Value != null && (bool)item.Value == true)
                {
                    var valor = row.Cells[indexValor].Value.ToString();
                    ret.Add(valor);
                }
            }

            return ret;
        }
	}
}