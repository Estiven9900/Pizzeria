using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace PizzeriaOpita.App.UI
{
    public class AsistenteForm : Form
    {
        private readonly PizzaService _pizzaService;
        private readonly PedidoService _pedidoService;
        private readonly AuthService _auth;

        private readonly ComboBox cboPizzas;
        private readonly Button btnRegistrar;
        private readonly Button btnEntregar;
        private readonly Button btnLogout;
        private readonly DataGridView dgvPedidos;

        public AsistenteForm(PizzaService pizzaService, PedidoService pedidoService, AuthService auth)
        {
            _pizzaService = pizzaService ?? throw new ArgumentNullException(nameof(pizzaService));
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            Text = "Asistente - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            cboPizzas = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
            btnRegistrar = new Button { Text = "Registrar pedido" };
            btnEntregar = new Button { Text = "Marcar entregado" };
            btnLogout = new Button { Text = "Cerrar sesión" };
            dgvPedidos = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8) };
            top.Controls.Add(new Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
            top.Controls.Add(cboPizzas);
            top.Controls.Add(btnRegistrar);
            top.Controls.Add(btnEntregar);
            top.Controls.Add(btnLogout);

            Controls.Add(dgvPedidos);
            Controls.Add(top);

            Load += async (_, __) => await Init();

            btnRegistrar.Click += async (_, __) =>
            {
                if (cboPizzas.SelectedValue is int idPizza)
                {
                    var idAsistente = _auth.GetUserIdForDb(_auth.CurrentUser!.Rol);
                    await _pedidoService.Registrar(idPizza, idAsistente);
                    await RefreshGrid();
                }
            };

            btnEntregar.Click += async (_, __) =>
            {
                if (dgvPedidos.CurrentRow?.Cells["Id"]?.Value is int id)
                {
                    await _pedidoService.Entregar(id);
                    await RefreshGrid();
                }
            };

            btnLogout.Click += BtnLogout_Click;
        }

        private async Task Init()
        {
            var pizzas = await _pizzaService.Listar();
            cboPizzas.DataSource = pizzas;
            cboPizzas.DisplayMember = "Nombre";
            cboPizzas.ValueMember = "IdPizza";
            await RefreshGrid();
        }

        private async Task RefreshGrid()
        {
            dgvPedidos.DataSource = await _pedidoService.Listar();
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
