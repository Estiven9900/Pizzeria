// UI/AsistenteForm.cs
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.UI
{
    public class AsistenteForm : Form
    {
        private readonly PizzaService _pizza;
        private readonly PedidoService _pedido;
        private readonly AuthService _authService;

        private readonly ComboBox cboPizzas;
        private readonly Button btnRegistrar;
        private readonly Button btnEntregar;
        private readonly DataGridView dgvPedidos;

        public AsistenteForm(PizzaService pizza, PedidoService pedido, AuthService authService)
        {
            _pizza = pizza;
            _pedido = pedido;
            _authService = authService;
            Text = "Asistente - Pizzería Opita";
            WindowState = FormWindowState.Maximized;

            cboPizzas = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };
            btnRegistrar = new Button { Text = "Registrar pedido" };
            btnEntregar = new Button { Text = "Marcar entregado" };
            dgvPedidos = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
            top.Controls.Add(new Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
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
                    var idAsistente = _authService.GetUserIdForDb(Rol.Asistente);
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