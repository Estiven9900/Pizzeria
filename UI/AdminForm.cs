using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.UI
{
    public class AdminForm : Form
    {
        private readonly PizzaService _pizza;
        private readonly PedidoService _pedido;

        private readonly TextBox txtNombre;
        private readonly NumericUpDown numPrecio;
        private readonly Button btnAgregar;
        private readonly Button btnVerPedidos;
        private readonly DataGridView dgvPizzas;

        public AdminForm(PizzaService pizza, PedidoService pedido)
        {
            _pizza = pizza;
            _pedido = pedido;
            Text = "Administrador - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            txtNombre = new TextBox { PlaceholderText = "Nombre de la pizza" };
            numPrecio = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Minimum = 0, Increment = 100 };
            btnAgregar = new Button { Text = "Agregar Pizza" };
            btnVerPedidos = new Button { Text = "Ver pedidos" };
            dgvPizzas = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
            top.Controls.Add(new Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
            top.Controls.Add(txtNombre);
            top.Controls.Add(new Label { Text = "Precio:", AutoSize = true, Padding = new Padding(8, 8, 8, 0) });
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