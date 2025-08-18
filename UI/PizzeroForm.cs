using PizzeriaOpita.App;
using PizzeriaOpita.App.Domain;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PizzeriaOpita.UI
{
    public class PizzeroForm : Form
    {
        private readonly Usuario _user;
        private readonly PedidoService _pedido;
        private readonly DataGridView dgvPendientes = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private readonly Button btnRefrescar = new() { Text = "Refrescar" };

        public PizzeroForm(Usuario user, PedidoService pedido)
        {
            _user = user;
            _pedido = pedido;
            Text = "Pizzero - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

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