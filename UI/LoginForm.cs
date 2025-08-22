// UI/LoginForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using PizzeriaOpita.App.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace PizzeriaOpita.App.UI
{
    public class LoginForm : Form
    {
        private readonly TextBox txtUsuario;
        private readonly TextBox txtContrasena;
        private readonly Button btnLogin;
        private readonly Label lblUsuario;
        private readonly Label lblContrasena;
        private readonly Label lblTitulo;
        private readonly AuthService _authService;

        public LoginForm()
        {
            _authService = new AuthService();
            Text = "Pizzería Opita - Iniciar Sesión";
            Width = 400;
            Height = 500;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            lblTitulo = new Label
            {
                Text = "Bienvenido a Pizzería Opita",
                Font = FontManager.RobotoBold,
                AutoSize = true,
                Location = new Point(50, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblUsuario = new Label
            {
                Text = "Usuario:",
                Font = FontManager.RobotoRegular,
                Location = new Point(50, 100),
                Size = new Size(100, 30)
            };
            txtUsuario = new TextBox
            {
                PlaceholderText = "Usuario",
                Location = new Point(50, 130),
                Width = 300,
                Font = FontManager.RobotoRegular
            };

            lblContrasena = new Label
            {
                Text = "Contraseña:",
                Font = FontManager.RobotoRegular,
                Location = new Point(50, 180),
                Size = new Size(100, 30)
            };
            txtContrasena = new TextBox
            {
                PlaceholderText = "Contraseña",
                Location = new Point(50, 210),
                Width = 300,
                Font = FontManager.RobotoRegular,
                UseSystemPasswordChar = true
            };

            btnLogin = new Button
            {
                Text = "Ingresar",
                Font = FontManager.RobotoBold,
                Location = new Point(50, 280),
                Size = new Size(300, 40)
            };
            btnLogin.Click += BtnLogin_Click;

            Controls.AddRange(new Control[] { lblTitulo, lblUsuario, txtUsuario, lblContrasena, txtContrasena, btnLogin });

            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnLogin.PerformClick();
            };
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var usuario = _authService.Login(txtUsuario.Text, txtContrasena.Text);

            if (usuario != null)
            {
                MessageBox.Show($"Bienvenido {usuario.NombreUsuario} ({usuario.Rol})",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form? nextForm = usuario.Rol switch
                {
                    Rol.Admin => Program.ServiceProvider!.GetRequiredService<AdminForm>(),
                    Rol.Asistente => Program.ServiceProvider!.GetRequiredService<AsistenteForm>(),
                    Rol.Pizzero => Program.ServiceProvider!.GetRequiredService<PizzeroForm>(),
                    _ => null
                };

                if (nextForm != null)
                {
                    nextForm.Show();
                    Hide();
                }
            }
            else
            {
                MessageBox.Show("Credenciales inválidas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}