// UI/PedidosListForm.cs
using System.Collections.Generic;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.UI
{
    public class PedidosListForm : Form
    {
        private readonly DataGridView dgv;

        public PedidosListForm(List<PedidoListItem> data)
        {
            Text = "Pedidos - Lectura";
            Width = 800;
            Height = 500;

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgv);
            dgv.DataSource = data;
        }
    }
}