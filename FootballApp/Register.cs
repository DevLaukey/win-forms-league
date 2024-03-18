using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FootballApp
{
    public partial class Register : Form
    {
        private TextBox txtNewUsername;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Label lblNewUsername;
        private Label lblNewPassword;
        private Label lblConfirmPassword;

        // Connection string for Access database
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\hp\Videos\tasks\c#\Football\FootballApp\football.accdb";

        public Register()
        {
            InitializeComponent();
            InitializeRegisterForm();
        }

        private void InitializeRegisterForm()
        {
            // Initialize controls
            txtNewUsername = new TextBox();
            txtNewPassword = new TextBox();
            txtConfirmPassword = new TextBox();
            btnRegister = new Button();
            lblNewUsername = new Label();
            lblNewPassword = new Label();
            lblConfirmPassword = new Label();

            // Set properties
            lblNewUsername.Text = "New Username:";
            lblNewUsername.AutoSize = true;
            lblNewUsername.Location = new System.Drawing.Point(20, 20);

            lblNewPassword.Text = "New Password:";
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new System.Drawing.Point(20, 60);

            lblConfirmPassword.Text = "Confirm Password:";
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new System.Drawing.Point(20, 100);

            txtNewUsername.Location = new System.Drawing.Point(150, 20);
            txtNewUsername.Size = new System.Drawing.Size(200, 25);

            txtNewPassword.Location = new System.Drawing.Point(150, 60);
            txtNewPassword.Size = new System.Drawing.Size(200, 25);
            txtNewPassword.PasswordChar = '*'; // Mask password input

            txtConfirmPassword.Location = new System.Drawing.Point(150, 100);
            txtConfirmPassword.Size = new System.Drawing.Size(200, 25);
            txtConfirmPassword.PasswordChar = '*'; // Mask password input

            btnRegister.Location = new System.Drawing.Point(150, 150);
            btnRegister.Size = new System.Drawing.Size(100, 30);
            btnRegister.Text = "Register";

            // Add controls to form
            Controls.Add(lblNewUsername);
            Controls.Add(txtNewUsername);
            Controls.Add(lblNewPassword);
            Controls.Add(txtNewPassword);
            Controls.Add(lblConfirmPassword);
            Controls.Add(txtConfirmPassword);
            Controls.Add(btnRegister);

            // Attach event handler
            btnRegister.Click += BtnRegister_Click;

            // Create a link label for login
            LinkLabel linkLogin = new LinkLabel();
            linkLogin.Text = "Already have an account? Login";
            linkLogin.AutoSize = true;
            linkLogin.Location = new System.Drawing.Point(20, 200);
            linkLogin.LinkClicked += LinkLogin_LinkClicked;

            // Add the link label to the form
            Controls.Add(linkLogin);
        }

        private void LinkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Hide the registration form
            this.Hide();

            // Show the login form
            Login loginForm = new Login();
            loginForm.ShowDialog();
        }
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Check if any fields are empty
            if (string.IsNullOrEmpty(txtNewUsername.Text) || string.IsNullOrEmpty(txtNewPassword.Text) || string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Check if passwords match
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // Hash the password
            string hashedPassword = HashPassword(txtNewPassword.Text);

            // Perform registration logic
            try
            {
                // Query to insert new user into the database
                string query = "INSERT INTO Users ([Username], [Password]) VALUES (@Username, @Password)";

                // Create a new connection to the database
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a command with parameters
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        // Set parameter values
                        command.Parameters.AddWithValue("@Username", txtNewUsername.Text);
                        command.Parameters.AddWithValue("@Password", hashedPassword);

                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Registration successful!");
                this.Close(); // Close the registration form after successful registration
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

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}
