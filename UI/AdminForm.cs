using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace PizzeriaOpita.App.UI
{
    public class AdminForm : Form
    {
        private readonly PizzaService _pizzaService;
        private readonly PedidoService _pedidoService;
        private readonly AuthService _auth;

        private readonly TextBox txtNombre;
        private readonly NumericUpDown numPrecio;
        private readonly Button btnAgregar;
        private readonly Button btnVerPedidos;
        private readonly Button btnLogout;
        private readonly DataGridView dgvPizzas;

        public AdminForm(PizzaService pizzaService, PedidoService pedidoService, AuthService auth)
        {
            _pizzaService = pizzaService ?? throw new ArgumentNullException(nameof(pizzaService));
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            Text = "Administrador - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8) };

            txtNombre = new TextBox { PlaceholderText = "Nombre de la pizza", Width = 300 };
            numPrecio = new NumericUpDown { DecimalPlaces = 2, Maximum = 100000, Minimum = 0, Width = 120 };
            btnAgregar = new Button { Text = "Agregar Pizza" };
            btnVerPedidos = new Button { Text = "Ver pedidos" };
            btnLogout = new Button { Text = "Cerrar sesión" };

            btnAgregar.Click += BtnAgregar_Click;
            btnVerPedidos.Click += BtnVerPedidos_Click;
            btnLogout.Click += BtnLogout_Click;

            top.Controls.Add(new Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0,8,8,0) });
            top.Controls.Add(txtNombre);
            top.Controls.Add(new Label { Text = "Precio:", AutoSize = true, Padding = new Padding(8,8,8,0) });
            top.Controls.Add(numPrecio);
            top.Controls.Add(btnAgregar);
            top.Controls.Add(btnVerPedidos);
            top.Controls.Add(btnLogout);

            dgvPizzas = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            Controls.Add(dgvPizzas);
            Controls.Add(top);

            Load += async (_, __) => await Refrescar();
        }

        private async Task Refrescar()
        {
            dgvPizzas.DataSource = await _pizzaService.Listar();
        }

        private async void BtnAgregar_Click(object? sender, EventArgs e)
        {
            try
            {
                await _pizzaService.Registrar(txtNombre.Text, numPrecio.Value);
                txtNombre.Clear();
                numPrecio.Value = 0;
                await Refrescar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private async void BtnVerPedidos_Click(object? sender, EventArgs e)
        {
            var data = await _pedidoService.Listar();
            new PedidosListForm(data).ShowDialog(this);
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
