using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace StudyTimeManager
{
    public class Form2 : Form
    {
        private DataGridView dgv;
        private System.Windows.Forms.Timer studyTimer;
        private Label lblTimer, lblStatus;
        private int secondsLeft;
        private bool isBelajar = false;
        private bool isIstirahat = false;

        private TextBox txtHari, txtMataPelajaran;
        private DateTimePicker dtDeadline;
        private ComboBox cmbReminder;
        private Button btnTambah, btnEdit, btnDone, btnDelete, btnBack, btnNext, btnMulaiBelajar;

        private string filePath = "jadwal.txt";
        private int selectedRowIndex = -1;

        public Form2()
        {
            InitializeUI();
            LoadJadwal();
        }

        private void InitializeUI()
        {
            // Form utama
            this.Text = "Manajemen Deadline Tugas";
            this.Size = new Size(850, 650);
            this.BackColor = Color.FromArgb(240, 225, 255);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label title = new Label()
            {
                Text = "📝 Daftar Tugas & Deadline",
                Font = new Font("Poppins", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 40, 140),
                AutoSize = true,
                Location = new Point(230, 20)
            };
            Controls.Add(title);

            // DataGridView
            dgv = new DataGridView()
            {
                Size = new Size(750, 280),
                Location = new Point(50, 80),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.Columns.Add("Day", "Hari");
            dgv.Columns.Add("Subject", "Mata Pelajaran");
            dgv.Columns.Add("Deadline", "Deadline Tugas");
            dgv.Columns.Add("Reminder", "Pengingat (H-)");
            dgv.CellClick += Dgv_CellClick;
            Controls.Add(dgv);

            // Input field
            Label lblHari = new Label() { Text = "Hari:", Location = new Point(50, 380), AutoSize = true };
            txtHari = new TextBox() { Location = new Point(90, 377), Width = 100 };

            Label lblMapel = new Label() { Text = "Mata Pelajaran:", Location = new Point(210, 380), AutoSize = true };
            txtMataPelajaran = new TextBox() { Location = new Point(320, 377), Width = 150 };

            Label lblDeadline = new Label() { Text = "Deadline Tugas:", Location = new Point(490, 380), AutoSize = true };
            dtDeadline = new DateTimePicker()
            {
                Location = new Point(600, 377),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                Width = 140
            };

            Label lblRem = new Label() { Text = "Pengingat:", Location = new Point(50, 420), AutoSize = true };
            cmbReminder = new ComboBox()
            {
                Location = new Point(130, 417),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReminder.Items.AddRange(new string[] { "H-1 hari", "H-1 jam", "H-30 menit" });
            cmbReminder.SelectedIndex = 0;

            // Tombol CRUD
            btnTambah = new Button()
            {
                Text = "Tambah",
                Location = new Point(310, 415),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(150, 100, 200),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnTambah.Click += BtnTambah_Click;

            btnEdit = new Button()
            {
                Text = "Edit",
                Location = new Point(410, 415),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(90, 140, 220),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnEdit.Click += BtnEdit_Click;

            btnDone = new Button()
            {
                Text = "Selesai ✅",
                Location = new Point(510, 415),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(100, 200, 100),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnDone.Click += BtnDone_Click;

            btnDelete = new Button()
            {
                Text = "Hapus 🗑️",
                Location = new Point(630, 415),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnDelete.Click += BtnDelete_Click;

            Controls.Add(lblHari);
            Controls.Add(txtHari);
            Controls.Add(lblMapel);
            Controls.Add(txtMataPelajaran);
            Controls.Add(lblDeadline);
            Controls.Add(dtDeadline);
            Controls.Add(lblRem);
            Controls.Add(cmbReminder);
            Controls.Add(btnTambah);
            Controls.Add(btnEdit);
            Controls.Add(btnDone);
            Controls.Add(btnDelete);

            // Status & Timer
            lblStatus = new Label()
            {
                Text = "Status: Belum mulai belajar.",
                Location = new Point(50, 470),
                Font = new Font("Poppins", 10, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(90, 40, 140)
            };
            Controls.Add(lblStatus);

            lblTimer = new Label()
            {
                Text = "⏰ Waktu: 25:00",
                Location = new Point(50, 500),
                Font = new Font("Consolas", 18, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(90, 40, 140)
            };
            Controls.Add(lblTimer);

            btnMulaiBelajar = new Button()
            {
                Text = "Mulai Belajar",
                Location = new Point(630, 460),
                Size = new Size(140, 45),
                BackColor = Color.FromArgb(100, 70, 180),
                ForeColor = Color.White,
                Font = new Font("Poppins", 10, FontStyle.Bold)
            };
            btnMulaiBelajar.Click += BtnMulaiBelajar_Click;
            Controls.Add(btnMulaiBelajar);

            // Timer
            studyTimer = new System.Windows.Forms.Timer();
            studyTimer.Interval = 1000;
            studyTimer.Tick += StudyTimer_Tick;

            // Navigasi
            btnBack = new Button()
            {
                Text = "⬅ Back",
                Location = new Point(50, 560),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(180, 120, 230),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnBack.Click += BtnBack_Click;
            Controls.Add(btnBack);

            btnNext = new Button()
            {
                Text = "Next ➡",
                Location = new Point(660, 560),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(140, 100, 210),
                ForeColor = Color.White,
                Font = new Font("Poppins", 9, FontStyle.Bold)
            };
            btnNext.Click += BtnNext_Click;
            Controls.Add(btnNext);
        }

        // ===== CRUD FUNCTION =====
        private void LoadJadwal()
        {
            dgv.Rows.Clear();
            if (!File.Exists(filePath)) File.Create(filePath).Close();

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split('|');
                if (parts.Length == 4)
                {
                    int row = dgv.Rows.Add(parts[0], parts[1], parts[2], parts[3]);
                    if (parts[2].Contains("Selesai")) dgv.Rows[row].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }

        private void SaveJadwal()
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[0].Value != null)
                        sw.WriteLine($"{row.Cells[0].Value}|{row.Cells[1].Value}|{row.Cells[2].Value}|{row.Cells[3].Value}");
                }
            }
        }

        private void BtnTambah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHari.Text) || string.IsNullOrWhiteSpace(txtMataPelajaran.Text))
            {
                MessageBox.Show("Isi semua kolom terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgv.Rows.Add(txtHari.Text, txtMataPelajaran.Text, dtDeadline.Value.ToString("dd/MM/yyyy HH:mm"), cmbReminder.SelectedItem.ToString());
            SaveJadwal();
            ClearInputs();
            MessageBox.Show("Tugas berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex < 0)
            {
                MessageBox.Show("Pilih baris yang ingin diedit!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgv.Rows[selectedRowIndex];
            row.Cells[0].Value = txtHari.Text;
            row.Cells[1].Value = txtMataPelajaran.Text;
            row.Cells[2].Value = dtDeadline.Value.ToString("dd/MM/yyyy HH:mm");
            row.Cells[3].Value = cmbReminder.SelectedItem.ToString();

            SaveJadwal();
            ClearInputs();
            MessageBox.Show("Data berhasil diperbarui!", "Edit Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex < 0)
            {
                MessageBox.Show("Pilih tugas yang sudah selesai!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgv.Rows[selectedRowIndex];
            row.Cells[2].Value = "Selesai ✅";
            row.DefaultCellStyle.BackColor = Color.LightGreen;

            SaveJadwal();
            MessageBox.Show("Tugas ditandai sebagai selesai!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex < 0)
            {
                MessageBox.Show("Pilih tugas yang ingin dihapus!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgv.Rows[selectedRowIndex];
            string namaTugas = row.Cells[1].Value?.ToString();

            var konfirmasi = MessageBox.Show(
                $"Apakah kamu yakin ingin menghapus tugas '{namaTugas}'?",
                "Konfirmasi Hapus",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (konfirmasi == DialogResult.Yes)
            {
                dgv.Rows.RemoveAt(selectedRowIndex);
                SaveJadwal();
                ClearInputs();
                MessageBox.Show("Tugas berhasil dihapus!", "Hapus Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedRowIndex = e.RowIndex;
                var row = dgv.Rows[e.RowIndex];
                txtHari.Text = row.Cells[0].Value?.ToString();
                txtMataPelajaran.Text = row.Cells[1].Value?.ToString();
                if (DateTime.TryParse(row.Cells[2].Value?.ToString(), out DateTime deadline))
                    dtDeadline.Value = deadline;
                cmbReminder.SelectedItem = row.Cells[3].Value?.ToString();
            }
        }

        private void ClearInputs()
        {
            txtHari.Clear();
            txtMataPelajaran.Clear();
            selectedRowIndex = -1;
        }

        // ===== TIMER POMODORO =====
        private void BtnMulaiBelajar_Click(object sender, EventArgs e)
        {
            if (!isBelajar && !isIstirahat)
            {
                isBelajar = true;
                secondsLeft = 25 * 60;
                lblStatus.Text = "Status: 🔥 Sedang Belajar...";
                btnMulaiBelajar.Text = "Batalkan";
                studyTimer.Start();
            }
            else
            {
                studyTimer.Stop();
                isBelajar = false;
                isIstirahat = false;
                lblStatus.Text = "Status: Dibatalkan.";
                lblTimer.Text = "⏰ Waktu: 25:00";
                btnMulaiBelajar.Text = "Mulai Belajar";
            }
        }

        private void StudyTimer_Tick(object sender, EventArgs e)
        {
            if (secondsLeft > 0)
            {
                secondsLeft--;
                lblTimer.Text = $"⏰ Waktu: {secondsLeft / 60:D2}:{secondsLeft % 60:D2}";
            }
            else
            {
                studyTimer.Stop();

                if (isBelajar)
                {
                    Notifikasi("✅ Waktu belajar selesai! Saatnya istirahat 5 menit 😌");
                    isBelajar = false;
                    isIstirahat = true;
                    secondsLeft = 5 * 60;
                    lblStatus.Text = "Status: 💤 Sedang Istirahat...";
                    studyTimer.Start();
                }
                else if (isIstirahat)
                {
                    Notifikasi("💪 Istirahat selesai! Yuk lanjut belajar!");
                    isIstirahat = false;
                    lblStatus.Text = "Status: Siap untuk sesi berikutnya.";
                    lblTimer.Text = "⏰ Waktu: 25:00";
                    btnMulaiBelajar.Text = "Mulai Belajar";
                }
            }
        }

        private void Notifikasi(string pesan)
        {
            Console.Beep();
            for (int i = 0; i < 3; i++)
            {
                this.Location = new Point(this.Location.X + 5, this.Location.Y);
                Thread.Sleep(40);
                this.Location = new Point(this.Location.X - 5, this.Location.Y);
                Thread.Sleep(40);
            }
            MessageBox.Show(pesan, "📢 Pengingat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== NAVIGASI =====
        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Form3().Show();
        }
    }
}
