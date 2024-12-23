using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Npgsql;

namespace Kleviy
{
    /// <summary>
    /// Логика взаимодействия для AddStaff.xaml
    /// </summary>
    public partial class AddStaff : Window
    {
        private string _connectionString = "Host = dpg-ctcmjq23esus73bingtg-a.frankfurt-postgres.render.com; Port = 5432; Database = kleviy; Username = kleviy_user; Password = bt5fPvG8DrpyvGRGHUMPAc6lJz2mdBjE";
        public AddStaff()
        {
            InitializeComponent();
        }
        //движение окна
        private void MovingWin(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        //минимизация
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        //закрытие
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            GC.Collect();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string lastName = SecondNameTextBox.Text;
            string firstName = NameTextBox.Text;
            string middleName = FirstNameTextBox.Text;
            DateTime? birthDate = DatePicker.SelectedDate;
            try
            {
                InsertData(login, password, lastName, firstName, middleName, birthDate);
                MessageBox.Show("Сотрудник добавлен успешно!", "Успешное добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                // Update the DataGrid in the other form
                home.RefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления сотрудника: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            GC.Collect();
        }
        public void InsertData(string login, string password, string lastName, string firstName, string middleName, DateTime? birthDate)
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                // Create a command object
                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Данные_для_входа (Логин, Пароль) VALUES (@login, @password) RETURNING id_Вход", connection);

                // Add parameters to the command
                command.Parameters.AddWithValue("login", login);
                command.Parameters.AddWithValue("password", password);

                // Execute the command and get the returned id_Вход value
                int idВход = (int)command.ExecuteScalar();

                // Create another command object
                command = new NpgsqlCommand("INSERT INTO Сотрудник (Фамилия, Имя, Отчество, Дата_рождения, id_должность, id_Вход) VALUES (@lastName, @firstName, @middleName, @birthDate, '2', @idВход)", connection);

                // Add parameters to the command
                command.Parameters.AddWithValue("lastName", lastName);
                command.Parameters.AddWithValue("firstName", firstName);
                command.Parameters.AddWithValue("middleName", middleName);
                if (birthDate.HasValue)
                {
                    command.Parameters.AddWithValue("birthDate", birthDate.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("birthDate", DBNull.Value);
                }
                command.Parameters.AddWithValue("idВход", idВход);

                // Execute the command
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            GC.Collect();
        }
    }
}
