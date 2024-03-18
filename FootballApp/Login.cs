using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FootballApp
{
    partial class Login : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblUsername;
        private Label lblPassword;
        private bool buttonClicked = false;

        // Connection string for Access database
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\hp\Videos\tasks\c#\Football\FootballApp\football.accdb";

        public Login()
        {
            InitializeComponent();
            InitializeLoginForm();
        }

        private void InitializeLoginForm()
        {
            // Initialize controls
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            lblUsername = new Label();
            lblPassword = new Label();

            // Set properties
            lblUsername.Text = "Username:";
            lblUsername.AutoSize = true;
            lblUsername.Location = new System.Drawing.Point(20, 50);

            lblPassword.Text = "Password:";
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(20, 100);

            txtUsername.Location = new System.Drawing.Point(100, 50);
            txtUsername.Size = new System.Drawing.Size(200, 25);

            txtPassword.Location = new System.Drawing.Point(100, 100);
            txtPassword.Size = new System.Drawing.Size(200, 25);
            txtPassword.PasswordChar = '*'; // Mask password input

            btnLogin.Location = new System.Drawing.Point(150, 150);
            btnLogin.Size = new System.Drawing.Size(100, 30);
            btnLogin.Text = "Login";

            // Add controls to form
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);

            // Attach event handler
            btnLogin.Click += BtnLogin_Click;

            // Create a link label for registration
            LinkLabel linkRegister = new LinkLabel();
            linkRegister.Text = "Click Here To Register";
            linkRegister.AutoSize = true;
            linkRegister.Location = new System.Drawing.Point(20, 200);
            linkRegister.LinkClicked += LinkRegister_LinkClicked;

            // Add the link label to the form
            Controls.Add(linkRegister);
        }
        private void LinkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            //Hide Login
            this.Hide();

            // Open the registration form when the link is clicked
            Register registerForm = new Register();
            registerForm.Show();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Set buttonClicked flag to true
            buttonClicked = true;

            // Check if both username and password are empty only if button is clicked
            if (buttonClicked && (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text)))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            // Perform database login logic
            try
            {
                // Hash the entered password
                string hashedPassword = HashPassword(txtPassword.Text);

                // Query to check if the provided username and password match any records in the database
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

                // Create a new connection to the database
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a command with parameters
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.AddWithValue("@Username", txtUsername.Text);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        // Execute the command and get the result (number of matching records)
                        int count = (int)command.ExecuteScalar();

                        // Check if there's at least one matching record
                        if (count > 0)
                        {
                            MessageBox.Show("Login successful!");
                            // Proceed to the next form or action
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (display error message or log)
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            // Create a new instance of the SHA256 hashing algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute hash from the password bytes
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}