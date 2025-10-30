using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StudyTimeManager
{
    public class LoginForm : Form
    {
        private TextBox txtUsername, txtPassword;
        private Button btnLogin, btnRegister;
        private Label lblStatus;

        private string akunFile = "akun.txt"; // tempat menyimpan akun lokal

        public LoginForm()
        {
            InitUI();
            CekAutoLogin();
        }

        private void InitUI()
        {
            this.Text = "Manajemen Waktu Belajar";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(230, 210, 250);

            Label lblTitle = new Label()
            {
                Text = "🕒 Manajemen Waktu Belajar",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(230, 80),
                ForeColor = Color.FromArgb(90, 40, 140)
            };
            Controls.Add(lblTitle);

            Label lblUser = new Label()
            {
                Text = "Username:",
                Location = new Point(280, 180),
                AutoSize = true
            };
            Controls.Add(lblUser);

            txtUsername = new TextBox()
            {
                Location = new Point(280, 200),
                Width = 250
            };
            Controls.Add(txtUsername);

            Label lblPass = new Label()
            {
                Text = "Password:",
                Location = new Point(280, 240),
                AutoSize = true
            };
            Controls.Add(lblPass);

            txtPassword = new TextBox()
            {
                Location = new Point(280, 260),
                Width = 250,
                PasswordChar = '*'
            };
            Controls.Add(txtPassword);

            btnLogin = new Button()
            {
                Text = "Login",
                BackColor = Color.FromArgb(140, 100, 210),
                ForeColor = Color.White,
                Location = new Point(280, 320),
                Width = 110,
                Height = 40
            };
            btnLogin.Click += BtnLogin_Click;
            Controls.Add(btnLogin);

            btnRegister = new Button()
            {
                Text = "Buat Akun",
                BackColor = Color.FromArgb(180, 120, 230),
                ForeColor = Color.White,
                Location = new Point(420, 320),
                Width = 110,
                Height = 40
            };
            btnRegister.Click += BtnRegister_Click;
            Controls.Add(btnRegister);

            lblStatus = new Label()
            {
                Text = "",
                ForeColor = Color.DarkRed,
                Location = new Point(280, 380),
                AutoSize = true
            };
            Controls.Add(lblStatus);
        }

        // ✅ Mengecek apakah sudah ada akun tersimpan dan langsung masuk
        private void CekAutoLogin()
        {
            if (File.Exists(akunFile))
            {
                string[] data = File.ReadAllLines(akunFile);
                if (data.Length >= 2)
                {
                    string username = data[0];
                    string password = data[1];

                    // langsung masuk tanpa login manual
                    BukaMenuUtama(username);
                }
            }
        }

        // 🔑 Tombol Login ditekan
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (!File.Exists(akunFile))
            {
                lblStatus.Text = "Belum ada akun. Silakan buat akun terlebih dahulu.";
                return;
            }

            string[] data = File.ReadAllLines(akunFile);
            string savedUser = data[0];
            string savedPass = data[1];

            if (txtUsername.Text == savedUser && txtPassword.Text == savedPass)
            {
                // Simpan login agar auto-login nanti
                File.WriteAllLines(akunFile, new string[] { txtUsername.Text, txtPassword.Text });

                BukaMenuUtama(txtUsername.Text);
            }
            else
            {
                lblStatus.Text = "Username atau password salah!";
            }
        }

        // 🧾 Tombol Buat Akun ditekan
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Isi username dan password untuk membuat akun.";
                return;
            }

            File.WriteAllLines(akunFile, new string[] { txtUsername.Text, txtPassword.Text });
            lblStatus.ForeColor = Color.Green;
            lblStatus.Text = "Akun berhasil dibuat! Anda akan langsung masuk.";

            BukaMenuUtama(txtUsername.Text);
        }

        // 🚪 Buka form utama setelah login atau register
        private void BukaMenuUtama(string username)
        {
            this.Hide();
            Form2 mainMenu = new Form2(); // ubah ke form utama kamu
            mainMenu.Text = $"Jadwal Belajar - {username}";
            mainMenu.Show();
        }
    }
}
