using PizzeriaOpita.App;
using PizzeriaOpita.App.Domain;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PizzeriaOpita.UI
{
    public class AdminForm : Form
    {
        private readonly Usuario _user;
        private readonly PizzaService _pizza;
        private readonly PedidoService _pedido;

        private readonly TextBox txtNombre = new() { PlaceholderText = "Nombre de la pizza" };
        private readonly NumericUpDown numPrecio = new() { DecimalPlaces = 2, Maximum = 1000000, Minimum = 0, Increment = 100 };
        private readonly Button btnAgregar = new() { Text = "Agregar Pizza" };
        private readonly Button btnVerPedidos = new() { Text = "Ver pedidos" };
        private readonly DataGridView dgvPizzas = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public AdminForm(Usuario user, PizzaService pizza, PedidoService pedido)
        {
            _user = user;
            _pizza = pizza;
            _pedido = pedido;
            Text = "Administrador - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
            top.Controls.Add(new System.Windows.Forms.Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
            top.Controls.Add(txtNombre);
            top.Controls.Add(new System.Windows.Forms.Label { Text = "Precio:", AutoSize = true, Padding = new Padding(8, 8, 8, 0) });
            top.Controls.Add(numPrecio);
            top.Controls.Add(btnAgregar);
            top.Controls.Add(btnVerPedidos);

            Controls.Add(dgvPizzas);
            Controls.Add(top);

            Load += async (_, __) => await Refrescar();
            btnAgregar.Click += async (_, __) =>
            {
                try
                {
                    await _pizza.Registrar(txtNombre.Text, numPrecio.Value);
                    txtNombre.Clear(); numPrecio.Value = 0;
                    await Refrescar();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Validación"); }
            };

            btnVerPedidos.Click += async (_, __) =>
            {
                var data = await _pedido.Listar();
                new PedidosListForm(data).ShowDialog(this);
            };
        }

        private async Task Refrescar()
        {
            dgvPizzas.DataSource = await _pizza.Listar();
        }
    }
}