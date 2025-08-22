// UI/PizzeroForm.cs
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.UI
{
    public class PizzeroForm : Form
    {
        private readonly PedidoService _pedido;
        private readonly DataGridView dgvPendientes;
        private readonly Button btnRefrescar;

        public PizzeroForm(PedidoService pedido)
        {
            _pedido = pedido;

            Text = "Pizzero - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            dgvPendientes = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnRefrescar = new Button { Text = "Refrescar" };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8) };
            top.Controls.Add(btnRefrescar);

            Controls.Add(dgvPendientes);
            Controls.Add(top);

            Load += async (_, __) => await RefreshGrid();
            btnRefrescar.Click += async (_, __) => await RefreshGrid();
        }

        private async Task RefreshGrid()
        {
            dgvPendientes.DataSource = await _pedido.Pendientes();
        }
    }
}