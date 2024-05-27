using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Npgsql;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Threading.Tasks;


namespace Kleviy
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();
            Profile_Loaded();
            LoadImage();
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
        public void Profile_Loaded()
        {
            LoadUserData();
            LoadImage();
        }
        private void LoadUserData()
        {
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Сотрудник.Имя, Сотрудник.Фамилия FROM Сотрудник " +
                              "INNER JOIN Данные_для_входа ON Сотрудник.id_Вход = Данные_для_входа.id_Вход " +
                              "WHERE Данные_для_входа.Логин = @login AND Данные_для_входа.Пароль = @password";

                string login = Properties.Settings.Default.Login;
                string password = Properties.Settings.Default.Password;
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("login", login);
                    command.Parameters.AddWithValue("password", password);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            secondnameText.Text = reader.GetString(1);
                            nameText.Text = reader.GetString(0);
                            Console.WriteLine($"{secondnameText.Text}, {nameText.Text}");
                        }
                    }
                }
                connection.Close();
            }
        }
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // Откройте диалоговое окно для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                // Create a new BitmapImage object from the selected fill
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                image.EndInit();

                // Unload the previous image from memory
                if (ProfilePNG.Source != null)
                {
                    ((BitmapImage)ProfilePNG.Source).UriSource = null;
                }

                // Сохраните путь к выбранному изображению
                string oldImagePath = Properties.Settings.Default.UserImagePath;
                string newImagePath = Path.Combine(Path.GetDirectoryName(oldImagePath), $"image_{DateTime.Now:yyyyMMddHHmmss}.png");
                File.Copy(openFileDialog.FileName, newImagePath, true);

                // Обновите путь к изображению в настройках
                Properties.Settings.Default.UserImagePath = newImagePath;
                Properties.Settings.Default.Save();

                // Задержка перед удалением файла
                Task.Delay(1500).Wait();

                // Удалите старое изображение
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }

                // Загрузите изображение в элемент Image
                ProfilePNG.Source = image;
            }
        }

        private void Image_DownloadFailed(object sender, ExceptionEventArgs e)
        {
            // Handle the DownloadFailed event
            // This event is raised if the image cannot be loaded
            MessageBox.Show("Failed to load image: " + e.ErrorException.Message);
        }
        private void LoadImage()
        {
            string login = Properties.Settings.Default.Login;
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", login);
            if (Directory.Exists(folderPath))
            {
                string[] filePaths = Directory.GetFiles(folderPath, "image_*.png");
                if (filePaths.Length > 0 && File.Exists(filePaths[0]))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(filePaths[0], UriKind.Absolute);
                    image.EndInit();

                    ProfilePNG.Source = image;
                }
                else
                {
                    Console.WriteLine("Не найдено изображение профиля: " + login);
                }
            }
            else
            {
                Console.WriteLine("Не найден путь изображения для этого пользователя: " + login);
            }

            // Проверка на существование файла по пути в настройках
            if (!File.Exists(Properties.Settings.Default.UserImagePath))
            {
                Console.WriteLine("Файл изображения не найден по пути в настройках");
            }
        }



        private string GetHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
