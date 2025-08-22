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

            // Estilo general / sangrado
            Padding = new Padding(20);
            BackColor = Color.WhiteSmoke;

            dgvPendientes = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Margin = new Padding(0, 10, 0, 0) };
            btnRefrescar = new Button { Text = "Refrescar", AutoSize = true, Margin = new Padding(0, 10, 10, 10) };
            btnLogout = new Button { Text = "Cerrar sesión", AutoSize = true, Margin = new Padding(0, 10, 10, 10) };

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
            try
            {
                dgvPendientes.DataSource = await _pedidoService.Pendientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pendientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
