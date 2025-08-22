using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace PizzeriaOpita.App.UI
{
    public class PizzeroForm : Form
    {
        private readonly PedidoService _pedidoService;
        private readonly AuthService _auth;

        private readonly DataGridView dgvPendientes;
        private readonly Button btnRefrescar;
        private readonly Button btnLogout;

        public PizzeroForm(PedidoService pedidoService, AuthService auth)
        {
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            Text = "Pizzero - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            dgvPendientes = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnRefrescar = new Button { Text = "Refrescar" };
            btnLogout = new Button { Text = "Cerrar sesión" };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
            top.Controls.Add(btnRefrescar);
            top.Controls.Add(btnLogout);

            Controls.Add(dgvPendientes);
            Controls.Add(top);

            Load += async (_, __) => await RefreshGrid();
            btnRefrescar.Click += async (_, __) => await RefreshGrid();
            btnLogout.Click += BtnLogout_Click;
        }

        private async Task RefreshGrid()
        {
            dgvPendientes.DataSource = await _pedidoService.Pendientes();
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            _auth.Logout();
            var login = Program.ServiceProvider!.GetRequiredService<LoginForm>();
            login.Show();
            Close();
        }
    }
}
