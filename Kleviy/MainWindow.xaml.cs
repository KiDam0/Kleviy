using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Npgsql;

namespace Kleviy
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window;
        private string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
        public MainWindow()
        {
            InitializeComponent();
            PasswordCheck.IsChecked = Properties.Settings.Default.checkBox;
            if (PasswordCheck.IsChecked == true)
            {
                LoginBox.Text = Properties.Settings.Default.Login;
                PasswordBoxT.Password = Properties.Settings.Default.Password;
            }
            else
            {
                LoginBox.Text = "";
                PasswordBoxT.Password = "";
            }
            Window = this;
            LoadSettings();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MovingWin(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                MainWindow.Window.DragMove();
            }
            GC.Collect();
        }

        private void PasswordBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBoxT.Password;
            bool? checkBoxLogin = PasswordCheck.IsChecked == true;
            await Task.Delay(400);

            //проверка логина и пароля перед входом

            if (CheckCredentials(login, password))
            {
                // Сохраняем логин и пароль в настройках
                if (PasswordCheck.IsChecked == true)
                {
                    Properties.Settings.Default.Login = login;
                    Properties.Settings.Default.Password = password;
                    Properties.Settings.Default.checkBox = (bool)checkBoxLogin;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.Login = "";
                    Properties.Settings.Default.Password = "";
                    Properties.Settings.Default.checkBox = (bool)checkBoxLogin;
                    Properties.Settings.Default.Save();
                }

                Home glavnaya = new Home();
                glavnaya.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
            GC.Collect();
        }
        //сопоставление логина и пароля

        private bool CheckCredentials(string login, string password)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Данные_для_входа WHERE Логин = @Login AND Пароль = @Password";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        private void LoadSettings()
        {
            try
            {
                // Загружаем данные из настроек
                string login = Properties.Settings.Default.Login;
                string password = Properties.Settings.Default.Password;
                bool? checkBoxLogin = Properties.Settings.Default.checkBox;
                PasswordCheck.IsChecked = checkBoxLogin;
                // Если данные не пусты, заполняем поля
                if (!string.IsNullOrEmpty(login))
                {
                    LoginBox.Text = login;
                }
                if (PasswordCheck.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        PasswordBoxT.Password = password;
                    }
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show("Ошибка при загрузке настроек: " + ex.Message);
            }
        }
    }
}
