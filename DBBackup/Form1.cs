using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DBBackup
{
    public partial class Form1 : Form
    {
        private SqlConnection conn;
        private SqlCommand command;
        private SqlDataReader reader;
        string connectionString = "";
        string sql = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                //connecting to server
                connectionString = "Data Source = " + txtDataSource.Text + "; User Id = " + txtUserId.Text + "; Password = " + txtPassword.Text + ""; 
                conn = new SqlConnection(connectionString);
                conn.Open();
                //receving list of existing databases
                sql = "EXEC sp_databases";
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();
                cmbDatabases.Items.Clear();
                while (reader.Read())
                {
                    cmbDatabases.Items.Add(reader[0].ToString());
                }
                reader.Dispose();
                conn.Close();
                conn.Dispose();

                //disabling/enabling labels and buttons
                txtDataSource.Enabled = false;
                txtUserId.Enabled = false;
                txtPassword.Enabled = false;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                btnBackup.Enabled = true;
                btnRestore.Enabled = true;

                cmbDatabases.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            //disabling/enabling labels and buttons
            txtDataSource.Enabled = true;
            txtUserId.Enabled = true;
            txtPassword.Enabled = true;
            cmbDatabases.Enabled = false;
            btnBackup.Enabled = false;
            btnRestore.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //disabling/enabling labels and buttons
            btnDisconnect.Enabled = false;
            cmbDatabases.Enabled = false;
            btnBackup.Enabled = false;
            btnRestore.Enabled = false;
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                if(cmbDatabases.Text.CompareTo("") == 0)
                {
                    MessageBox.Show("Please select database");
                    return;
                }
                //establishing connection
                conn = new SqlConnection(connectionString);
                conn.Open();

                //preparing and executing backup query
                sql = "BACKUP DATABASE " + cmbDatabases.Text + " TO DISK = '" + txtBackupFileLoc.Text + "\\" + cmbDatabases.Text + "-" + DateTime.Now.Ticks.ToString() + ".bak'";
                command = new SqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                MessageBox.Show("Backup complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtBackupFileLoc.Text = dlg.SelectedPath;
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Backup Files(*.bak)|*.bak|All Files(*.*)|*.*";
            dlg.FilterIndex = 0;
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                txtRestoreFileLoc.Text = dlg.FileName;
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbDatabases.Text.CompareTo("") == 0)
                {
                    MessageBox.Show("Please select database");
                    return;
                }
                //establishing connection
                conn = new SqlConnection(connectionString);
                conn.Open();

                //preparing and executing restore query
                sql = "ALTER DATABASE " + cmbDatabases.Text + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                sql += "RESTORE DATABASE " + cmbDatabases.Text + " FROM DISK = '" + txtRestoreFileLoc.Text + "' WITH REPLACE;";
                command = new SqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                MessageBox.Show("Restoration complete.");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
//cyclic database backup can be achived by creating proper .bat file and setting Task Scheduler to execute the backup periodically.