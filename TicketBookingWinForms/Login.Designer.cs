using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TicketBooking_BusinessDataLogic;
using TicketBookingCommon;

namespace TicketBookingWinForms
{
    public partial class LoginForm : Form
    {
        private UserService userService = new UserService();
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private int loginAttempts = 0;
        private const int maxAttempts = 3;

        public bool IsAdmin { get; private set; }
        public string CurrentUsername { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Movie Ticket Booking System - Login";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "MOVIE TICKET BOOKING SYSTEM";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Location = new Point(50, 30);
            lblTitle.Size = new Size(300, 30);
            lblTitle.ForeColor = Color.DarkBlue;

            // Username Label
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(50, 80);
            lblUsername.Size = new Size(80, 20);

            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Location = new Point(140, 78);
            txtUsername.Size = new Size(200, 20);

            // Password Label
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(50, 120);
            lblPassword.Size = new Size(80, 20);

            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Location = new Point(140, 118);
            txtPassword.Size = new Size(200, 20);
            txtPassword.UseSystemPasswordChar = true;

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Location = new Point(140, 160);
            btnLogin.Size = new Size(120, 30);
            btnLogin.BackColor = Color.LightBlue;
            btnLogin.Click += BtnLogin_Click;

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);

            // Set default button
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (userService.AuthenticateUser(username, password, out bool userIsAdmin))
            {
                IsAdmin = userIsAdmin;
                CurrentUsername = username;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                loginAttempts++;
                int remainingAttempts = maxAttempts - loginAttempts;

                if (remainingAttempts > 0)
                {
                    MessageBox.Show($"Invalid username or password.\nAttempts remaining: {remainingAttempts}",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
                else
                {
                    MessageBox.Show("Maximum login attempts exceeded.\nApplication will close.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }
    }
    public partial class MainForm : Form
    {
        private MovieService movieService = new MovieService();
        private UserService userService = new UserService();
        private bool isAdmin;
        private string currentUsername;

        // Controls
        private MenuStrip menuStrip;
        private ToolStripMenuItem movieMenuItem;
        private ToolStripMenuItem adminMenuItem;
        private ListBox lstMovies;
        private GroupBox grpMovieDetails;
        private Label lblSelectedMovie;
        private Label lblAvailableTickets;
        private Label lblBookedTickets;
        private GroupBox grpActions;
        private Button btnBookTicket;
        private Button btnCancelTicket;
        private Button btnRefresh;
        private NumericUpDown numTickets;
        private Label lblTicketCount;
        private TextBox txtSearch;
        private Button btnSearch;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        public MainForm(bool isAdmin, string username)
        {
            this.isAdmin = isAdmin;
            this.currentUsername = username;
            InitializeComponent();
            LoadMovies();
        }

        private void InitializeComponent()
        {
            this.Text = $"Movie Ticket Booking System - {currentUsername}{(isAdmin ? " (Admin)" : "")}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Menu Strip
            menuStrip = new MenuStrip();
            movieMenuItem = new ToolStripMenuItem("Movies");
            movieMenuItem.DropDownItems.Add("Search Movies", null, SearchMovies_Click);
            movieMenuItem.DropDownItems.Add("Refresh", null, RefreshMovies_Click);

            if (isAdmin)
            {
                adminMenuItem = new ToolStripMenuItem("Admin");
                adminMenuItem.DropDownItems.Add("Add Movie", null, AddMovie_Click);
                adminMenuItem.DropDownItems.Add("Delete Movie", null, DeleteMovie_Click);
                adminMenuItem.DropDownItems.Add("Add User", null, AddUser_Click);
                menuStrip.Items.Add(adminMenuItem);
            }

            menuStrip.Items.Add(movieMenuItem);

            // Search controls
            Label lblSearch = new Label();
            lblSearch.Text = "Search Movies:";
            lblSearch.Location = new Point(20, 40);
            lblSearch.Size = new Size(100, 20);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(130, 38);
            txtSearch.Size = new Size(200, 20);

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(340, 36);
            btnSearch.Size = new Size(80, 25);
            btnSearch.Click += BtnSearch_Click;

            // Movies ListBox
            Label lblMovies = new Label();
            lblMovies.Text = "Available Movies:";
            lblMovies.Location = new Point(20, 80);
            lblMovies.Size = new Size(120, 20);

            lstMovies = new ListBox();
            lstMovies.Location = new Point(20, 100);
            lstMovies.Size = new Size(350, 200);
            lstMovies.SelectedIndexChanged += LstMovies_SelectedIndexChanged;

            // Movie Details GroupBox
            grpMovieDetails = new GroupBox();
            grpMovieDetails.Text = "Movie Details";
            grpMovieDetails.Location = new Point(400, 100);
            grpMovieDetails.Size = new Size(350, 150);

            lblSelectedMovie = new Label();
            lblSelectedMovie.Text = "Selected Movie: None";
            lblSelectedMovie.Location = new Point(10, 25);
            lblSelectedMovie.Size = new Size(330, 20);
            lblSelectedMovie.Font = new Font("Arial", 9, FontStyle.Bold);

            lblAvailableTickets = new Label();
            lblAvailableTickets.Text = "Available Tickets: 0";
            lblAvailableTickets.Location = new Point(10, 50);
            lblAvailableTickets.Size = new Size(200, 20);

            lblBookedTickets = new Label();
            lblBookedTickets.Text = "Booked Tickets: 0";
            lblBookedTickets.Location = new Point(10, 75);
            lblBookedTickets.Size = new Size(200, 20);

            grpMovieDetails.Controls.Add(lblSelectedMovie);
            grpMovieDetails.Controls.Add(lblAvailableTickets);
            grpMovieDetails.Controls.Add(lblBookedTickets);

            // Actions GroupBox
            grpActions = new GroupBox();
            grpActions.Text = "Actions";
            grpActions.Location = new Point(400, 270);
            grpActions.Size = new Size(350, 120);

            lblTicketCount = new Label();
            lblTicketCount.Text = "Number of Tickets:";
            lblTicketCount.Location = new Point(10, 25);
            lblTicketCount.Size = new Size(120, 20);

            numTickets = new NumericUpDown();
            numTickets.Location = new Point(140, 23);
            numTickets.Size = new Size(80, 20);
            numTickets.Minimum = 1;
            numTickets.Maximum = 50;
            numTickets.Value = 1;

            btnBookTicket = new Button();
            btnBookTicket.Text = "Book Ticket";
            btnBookTicket.Location = new Point(10, 60);
            btnBookTicket.Size = new Size(100, 30);
            btnBookTicket.BackColor = Color.LightGreen;
            btnBookTicket.Click += BtnBookTicket_Click;

            btnCancelTicket = new Button();
            btnCancelTicket.Text = "Cancel Ticket";
            btnCancelTicket.Location = new Point(120, 60);
            btnCancelTicket.Size = new Size(100, 30);
            btnCancelTicket.BackColor = Color.LightCoral;
            btnCancelTicket.Click += BtnCancelTicket_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(230, 60);
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.BackColor = Color.LightBlue;
            btnRefresh.Click += BtnRefresh_Click;

            grpActions.Controls.Add(lblTicketCount);
            grpActions.Controls.Add(numTickets);
            grpActions.Controls.Add(btnBookTicket);
            grpActions.Controls.Add(btnCancelTicket);
            grpActions.Controls.Add(btnRefresh);

            // Status Strip
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Ready";
            statusStrip.Items.Add(statusLabel);

            // Add controls to form
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(lblMovies);
            this.Controls.Add(lstMovies);
            this.Controls.Add(grpMovieDetails);
            this.Controls.Add(grpActions);
            this.Controls.Add(statusStrip);

            // Initially disable action buttons
            btnBookTicket.Enabled = false;
            btnCancelTicket.Enabled = false;
        }

        private void LoadMovies()
        {
            try
            {
                lstMovies.Items.Clear();
                string[] movies = movieService.GetMovies();
                int[] availableTickets = movieService.GetAvailableTickets();

                for (int i = 0; i < movies.Length; i++)
                {
                    lstMovies.Items.Add($"{movies[i]} - Available: {availableTickets[i]}");
                }

                statusLabel.Text = $"Loaded {movies.Length} movies";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMovies.SelectedIndex >= 0)
            {
                int movieIndex = lstMovies.SelectedIndex;
                string[] movies = movieService.GetMovies();

                lblSelectedMovie.Text = $"Selected Movie: {movies[movieIndex]}";
                lblAvailableTickets.Text = $"Available Tickets: {movieService.GetAvailableTicketsForMovie(movieIndex)}";
                lblBookedTickets.Text = $"Booked Tickets: {movieService.GetBookedTicketsForMovie(movieIndex)}";

                btnBookTicket.Enabled = movieService.GetAvailableTicketsForMovie(movieIndex) > 0;
                btnCancelTicket.Enabled = movieService.GetBookedTicketsForMovie(movieIndex) > 0;

                numTickets.Maximum = Math.Max(1, movieService.GetAvailableTicketsForMovie(movieIndex));
            }
            else
            {
                lblSelectedMovie.Text = "Selected Movie: None";
                lblAvailableTickets.Text = "Available Tickets: 0";
                lblBookedTickets.Text = "Booked Tickets: 0";
                btnBookTicket.Enabled = false;
                btnCancelTicket.Enabled = false;
            }
        }

        private void BtnBookTicket_Click(object sender, EventArgs e)
        {
            if (lstMovies.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a movie first.", "No Movie Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int movieIndex = lstMovies.SelectedIndex;
            int ticketCount = (int)numTickets.Value;

            if (movieService.CheckTicketAvailability(movieIndex, ticketCount))
            {
                if (movieService.UpdateTickets(Actions.BookTicket, movieIndex, ticketCount))
                {
                    MessageBox.Show($"Successfully booked {ticketCount} ticket(s)!", "Booking Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMovies();
                    LstMovies_SelectedIndexChanged(sender, e);
                    statusLabel.Text = $"Booked {ticketCount} tickets";
                }
                else
                {
                    MessageBox.Show("Booking failed. Please try again.", "Booking Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Sorry, only {movieService.GetAvailableTicketsForMovie(movieIndex)} tickets are available.",
                    "Insufficient Tickets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCancelTicket_Click(object sender, EventArgs e)
        {
            if (lstMovies.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a movie first.", "No Movie Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int movieIndex = lstMovies.SelectedIndex;
            int ticketCount = (int)numTickets.Value;
            int bookedTickets = movieService.GetBookedTicketsForMovie(movieIndex);

            if (ticketCount > bookedTickets)
            {
                MessageBox.Show($"You can only cancel up to {bookedTickets} tickets.",
                    "Invalid Cancellation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Are you sure you want to cancel {ticketCount} ticket(s)?",
                "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (movieService.UpdateTickets(Actions.CancelTicket, movieIndex, ticketCount))
                {
                    MessageBox.Show($"Successfully cancelled {ticketCount} ticket(s)!", "Cancellation Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMovies();
                    LstMovies_SelectedIndexChanged(sender, e);
                    statusLabel.Text = $"Cancelled {ticketCount} tickets";
                }
                else
                {
                    MessageBox.Show("Cancellation failed. Please try again.", "Cancellation Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadMovies();
            statusLabel.Text = "Movies refreshed";
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchMovies_Click(sender, e);
        }

        private void SearchMovies_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadMovies();
                return;
            }

            var results = movieService.SearchMovies(searchTerm);
            lstMovies.Items.Clear();

            if (results.Count == 0)
            {
                MessageBox.Show("No movies found matching your search.", "Search Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMovies();
            }
            else
            {
                foreach (var movie in results)
                {
                    lstMovies.Items.Add($"{movie.Title} - Available: {movie.AvailableTickets}");
                }
                statusLabel.Text = $"Found {results.Count} movie(s)";
            }
        }

        private void RefreshMovies_Click(object sender, EventArgs e)
        {
            LoadMovies();
        }

        private void AddMovie_Click(object sender, EventArgs e)
        {
            using (var addMovieForm = new AddMovieForm())
            {
                if (addMovieForm.ShowDialog() == DialogResult.OK)
                {
                    if (movieService.AddMovie(addMovieForm.MovieTitle, addMovieForm.TotalSeats))
                    {
                        MessageBox.Show($"Movie '{addMovieForm.MovieTitle}' added successfully!",
                            "Movie Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadMovies();
                        statusLabel.Text = "Movie added successfully";
                    }
                    else
                    {
                        MessageBox.Show("Failed to add movie. Movie may already exist.",
                            "Add Movie Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DeleteMovie_Click(object sender, EventArgs e)
        {
            if (lstMovies.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a movie to delete.", "No Movie Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int movieIndex = lstMovies.SelectedIndex;
            string[] movies = movieService.GetMovies();
            string movieTitle = movies[movieIndex];

            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{movieTitle}'?",
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (movieService.DeleteMovie(movieIndex))
                {
                    MessageBox.Show("Movie deleted successfully!", "Movie Deleted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMovies();
                    statusLabel.Text = "Movie deleted successfully";
                }
                else
                {
                    MessageBox.Show("Failed to delete movie.", "Delete Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddUser_Click(object sender, EventArgs e)
        {
            using (var addUserForm = new AddUserForm())
            {
                if (addUserForm.ShowDialog() == DialogResult.OK)
                {
                    if (userService.RegisterUser(addUserForm.Username, addUserForm.Password, addUserForm.IsAdmin))
                    {
                        MessageBox.Show($"User '{addUserForm.Username}' added successfully!",
                            "User Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        statusLabel.Text = "User added successfully";
                    }
                    else
                    {
                        MessageBox.Show("Failed to add user. Username may already exist.",
                            "Add User Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    public partial class AddMovieForm : Form
    {
        private TextBox txtTitle;
        private NumericUpDown numSeats;
        private Button btnAdd;
        private Button btnCancel;

        public string MovieTitle { get; private set; }
        public int TotalSeats { get; private set; }

        public AddMovieForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add New Movie";
            this.Size = new Size(350, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitle = new Label();
            lblTitle.Text = "Movie Title:";
            lblTitle.Location = new Point(20, 30);
            lblTitle.Size = new Size(80, 20);

            txtTitle = new TextBox();
            txtTitle.Location = new Point(110, 28);
            txtTitle.Size = new Size(200, 20);

            Label lblSeats = new Label();
            lblSeats.Text = "Total Seats:";
            lblSeats.Location = new Point(20, 70);
            lblSeats.Size = new Size(80, 20);

            numSeats = new NumericUpDown();
            numSeats.Location = new Point(110, 68);
            numSeats.Size = new Size(100, 20);
            numSeats.Minimum = 1;
            numSeats.Maximum = 1000;
            numSeats.Value = 50;

            btnAdd = new Button();
            btnAdd.Text = "Add Movie";
            btnAdd.Location = new Point(110, 110);
            btnAdd.Size = new Size(80, 30);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(200, 110);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblSeats);
            this.Controls.Add(numSeats);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnAdd;
            this.CancelButton = btnCancel;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a movie title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MovieTitle = title;
            TotalSeats = (int)numSeats.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    public partial class AddUserForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkIsAdmin;
        private Button btnAdd;
        private Button btnCancel;

        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool IsAdmin { get; private set; }

        public AddUserForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add New User";
            this.Size = new Size(350, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new Point(20, 30);
            lblUsername.Size = new Size(80, 20);

            txtUsername = new TextBox();
            txtUsername.Location = new Point(110, 28);
            txtUsername.Size = new Size(200, 20);

            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(20, 70);
            lblPassword.Size = new Size(80, 20);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(110, 68);
            txtPassword.Size = new Size(200, 20);
            txtPassword.UseSystemPasswordChar = true;

            chkIsAdmin = new CheckBox();
            chkIsAdmin.Text = "Admin User";
            chkIsAdmin.Location = new Point(110, 110);
            chkIsAdmin.Size = new Size(100, 20);

            btnAdd = new Button();
            btnAdd.Text = "Add User";
            btnAdd.Location = new Point(110, 150);
            btnAdd.Size = new Size(80, 30);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(200, 150);
            btnCancel.Size = new Size(80, 30);
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(chkIsAdmin);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnAdd;
            this.CancelButton = btnCancel;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Username = username;
            Password = password;
            IsAdmin = chkIsAdmin.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}