using System;
using System.Drawing;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace PizzeriaOpita.App.UI
{
    public class LoginForm : Form
    {
        private readonly AuthService _auth;
        private readonly TextBox txtUsuario;
        private readonly TextBox txtContrasena;
        private readonly Button btnLogin;
        private readonly Label lblTitulo;
        private readonly Label lblUsuario;
        private readonly Label lblContrasena;

        public LoginForm(AuthService auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            Text = "Pizzería Opita - Iniciar Sesión";
            Width = 420;
            Height = 420;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            // Layout / sangrado
            this.Padding = new Padding(20);
            this.BackColor = Color.WhiteSmoke;

            lblTitulo = new Label
            {
                Text = "Bienvenido a Pizzería Opita",
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 20)
            };

            lblUsuario = new Label { Text = "Usuario:", Location = new Point(40, 80), Size = new Size(80, 24) };
            txtUsuario = new TextBox { PlaceholderText = "Usuario", Location = new Point(40, 110), Width = 320, Margin = new Padding(0,5,0,5) };

            lblContrasena = new Label { Text = "Contraseña:", Location = new Point(40, 160), Size = new Size(80, 24) };
            txtContrasena = new TextBox { PlaceholderText = "Contraseña", Location = new Point(40, 190), Width = 320, UseSystemPasswordChar = true, Margin = new Padding(0,5,0,15) };

            btnLogin = new Button { Text = "Ingresar", Location = new Point(40, 250), Size = new Size(320, 40), Margin = new Padding(0,10,0,0) };
            btnLogin.Click += BtnLogin_Click;

            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 1;
            btnLogin.FlatAppearance.BorderColor = Color.DarkGray;
            btnLogin.BackColor = Color.White;
            btnLogin.ForeColor = Color.Black;

            Controls.AddRange(new Control[] { lblTitulo, lblUsuario, txtUsuario, lblContrasena, txtContrasena, btnLogin });

            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                var usuario = await _auth.LoginAsync(txtUsuario.Text?.Trim() ?? string.Empty, txtContrasena.Text ?? string.Empty);
                if (usuario == null)
                {
                    MessageBox.Show("Credenciales inválidas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Form? nextForm = usuario.Rol switch
                {
                    Rol.Admin => Program.ServiceProvider!.GetRequiredService<AdminForm>(),
                    Rol.Asistente => Program.ServiceProvider!.GetRequiredService<AsistenteForm>(),
                    Rol.Pizzero => Program.ServiceProvider!.GetRequiredService<PizzeroForm>(),
                    _ => null
                };

                if (nextForm != null)
                {
                    Hide();
                    nextForm.FormClosed += (_, __) => Close();
                    nextForm.Show();
                }
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}
