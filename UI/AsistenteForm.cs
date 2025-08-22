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

            // Estilo general / sangrado
            Padding = new Padding(20);
            BackColor = Color.WhiteSmoke;

            cboPizzas = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 360, Margin = new Padding(0, 10, 10, 10) };
            btnRegistrar = new Button { Text = "Registrar pedido", AutoSize = true, Margin = new Padding(0, 10, 10, 10) };
            btnEntregar = new Button { Text = "Marcar entregado", AutoSize = true, Margin = new Padding(0, 10, 10, 10) };
            btnLogout = new Button { Text = "Cerrar sesión", AutoSize = true, Margin = new Padding(0, 10, 10, 10) };

            dgvPedidos = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, Margin = new Padding(0, 10, 0, 0) };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(8) };
            top.Controls.Add(new Label { Text = "Pizza:", AutoSize = true, Padding = new Padding(0, 12, 8, 0) });
            top.Controls.Add(cboPizzas);
            top.Controls.Add(btnRegistrar);
            top.Controls.Add(btnEntregar);
            top.Controls.Add(btnLogout);

            Controls.Add(dgvPedidos);
            Controls.Add(top);

            Load += async (_, __) => await Init();

            btnRegistrar.Click += async (_, __) =>
            {
                try
                {
                    if (cboPizzas.SelectedValue is int idPizza)
                    {
                        var rolActual = _auth.CurrentUser?.Rol ?? Rol.Asistente;
                        var idAsistente = _auth.GetUserIdForDb(rolActual);
                        await _pedidoService.Registrar(idPizza, idAsistente);
                        await RefreshGrid();
                    }
                    else
                    {
                        MessageBox.Show("Seleccione una pizza válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al registrar pedido: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnEntregar.Click += async (_, __) =>
            {
                try
                {
                    if (dgvPedidos.CurrentRow?.Cells["Id"]?.Value is int id)
                    {
                        await _pedidoService.Entregar(id);
                        await RefreshGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar pedido: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnLogout.Click += BtnLogout_Click;
        }

        private async Task Init()
        {
            try
            {
                var pizzas = await _pizzaService.Listar();
                cboPizzas.DataSource = pizzas;
                cboPizzas.DisplayMember = "Nombre";
                cboPizzas.ValueMember = "IdPizza";
                await RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshGrid()
        {
            try
            {
                dgvPedidos.DataSource = await _pedidoService.Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pedidos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
