using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;
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

            lblTitulo = new Label
            {
                Text = "Bienvenido a Pizzería Opita",
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 20)
            };

            lblUsuario = new Label { Text = "Usuario:", Location = new Point(40, 80), Size = new Size(80, 24) };
            txtUsuario = new TextBox { PlaceholderText = "Usuario", Location = new Point(40, 110), Width = 320 };

            lblContrasena = new Label { Text = "Contraseña:", Location = new Point(40, 160), Size = new Size(80, 24) };
            txtContrasena = new TextBox { PlaceholderText = "Contraseña", Location = new Point(40, 190), Width = 320, UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "Ingresar", Location = new Point(40, 250), Size = new Size(320, 40) };
            btnLogin.Click += BtnLogin_Click;

            Controls.AddRange(new Control[] { lblTitulo, lblUsuario, txtUsuario, lblContrasena, txtContrasena, btnLogin });

            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                var usuario = await _auth.LoginAsync(txtUsuario.Text, txtContrasena.Text);
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
