using PizzeriaOpita.App.Domain;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PizzeriaOpita.UI
{
    public class PedidosListForm : Form
    {
        private readonly DataGridView dgv = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public PedidosListForm(List<PedidoListItem> data)
        {
            Text = "Pedidos - Lectura";
            Width = 800; Height = 500;
            Controls.Add(dgv);
            dgv.DataSource = data;
        }
    }
}