using DatabaseGenerationWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseGenerationWPF.Views
{
    /// <summary>
    /// Interaction logic for Login
    /// </summary>
    public partial class Login : Window
    {
        private LoginViewModel loginViewModel;

        public LoginViewModel LoginViewModel { get => loginViewModel; set => loginViewModel = value; }

        public Login()
        {
            InitializeComponent();
            this.LoginViewModel = new LoginViewModel();
            DataContext = this.LoginViewModel;
        }

        public Login(User user)
        {
            InitializeComponent();
            this.LoginViewModel = new LoginViewModel();
            DataContext = this.LoginViewModel;
            LoginViewModel.LoginUser = user;
        }

        public User User { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
