using PizzeriaOpita.App;
using PizzeriaOpita.App.Domain;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PizzeriaOpita.UI
{
    public class AsistenteForm : Form
    {
        private readonly Usuario _user;
        private readonly PizzaService _pizza;
        private readonly PedidoService _pedido;

        private readonly ComboBox cboPizzas = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };
        private readonly Button btnRegistrar = new() { Text = "Registrar pedido" };
        private readonly Button btnEntregar = new() { Text = "Marcar entregado" };
        private readonly DataGridView dgvPedidos = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public AsistenteForm(Usuario user, PizzaService pizza, PedidoService pedido)
        {
            _user = user;
            _pizza = pizza;
            _pedido = pedido;
            Text = "Asistente - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
            top.Controls.Add(new System.Windows.Forms.Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
            top.Controls.Add(cboPizzas);
            top.Controls.Add(btnRegistrar);
            top.Controls.Add(btnEntregar);

            Controls.Add(dgvPedidos);
            Controls.Add(top);

            Load += async (_, __) => await Init();

            btnRegistrar.Click += async (_, __) =>
            {
                if (cboPizzas.SelectedValue is int idPizza)
                {
                    var idAsistente = AuthService.GetUserIdForDb(_user.Rol);
                    await _pedido.Registrar(idPizza, idAsistente);
                    await RefreshGrid();
                }
            };

            btnEntregar.Click += async (_, __) =>
            {
                if (dgvPedidos.CurrentRow?.Cells["Id"]?.Value is int id)
                {
                    await _pedido.Entregar(id);
                    await RefreshGrid();
                }
            };
        }

        private async Task Init()
        {
            var pizzas = await _pizza.Listar();
            cboPizzas.DataSource = pizzas;
            cboPizzas.DisplayMember = "Nombre";
            cboPizzas.ValueMember = "IdPizza";
            await RefreshGrid();
        }

        private async Task RefreshGrid()
        {
            dgvPedidos.DataSource = await _pedido.Listar();
        }
    }
}