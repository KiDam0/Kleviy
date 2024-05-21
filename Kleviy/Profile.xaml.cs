using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Npgsql;
using Newtonsoft.Json;
using System.Security.Cryptography;


namespace Kleviy
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        private string _userImagePath;
        MainWindow aut = new MainWindow();
        private string _connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
        public Profile()
        {

            InitializeComponent();
            Profile_Loaded();
            // Загрузите изображение после загрузки формы профиля
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
        //изображение
        private void Image_Loaded(object sender, RoutedEventArgs e)
        {

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
            // Get the login and password from the settings
            string login = Properties.Settings.Default.Login;
            string password = Properties.Settings.Default.Password;

            using (var conn = new NpgsqlConnection("Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123"))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Фамилия, Имя FROM Сотрудник JOIN Данные_для_входа ON id_Вход = id_Вход WHERE Логин = @login AND Пароль = @password", conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var lastName = reader.GetString(0);
                            var firstName = reader.GetString(1);
                            Console.WriteLine($"First name: {firstName}, Last name: {lastName}");
                            nameText.Text = firstName;
                            secondnameText.Text = lastName;
                            // Используйте полученные данные в приложении
                        }
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                // Create a new BitmapImage object from the selected file
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(op.FileName, UriKind.Absolute);
                image.EndInit();

                ProfilePNG.Source = image; // Display the selected image

                string pathToImage = op.FileName;

                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = System.IO.Path.Combine(appPath, "Images");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                // Создайте объект FileStream для записи изображения
                string fileName = "image_" + GetHash(aut.LoginBox.Text) + ".png"; // Имя файла изображения
                string filePath = System.IO.Path.Combine(imagePath, fileName);

                // Создайте объект PngBitmapEncoder для сохранения изображения в формате PNG
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image)); // Добавьте изображение в объект BitmapFrame
                try
                {
                    FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    encoder.Save(stream); // Сохраните изображение в файл
                    stream.Close();

                    // Обновите путь к изображению в настройках
                    SaveImage(filePath);
                }
                catch (IOException ex)
                {
                    // Handle the exception
                    Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
                }
            }
        }

        private void LoadImage()
        {
            string login = Properties.Settings.Default.Login;
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", login);
            if (Directory.Exists(folderPath))
            {
                string[] filePaths = Directory.GetFiles(folderPath, "image_*.png");
                if (filePaths.Length > 0)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(filePaths[0], UriKind.Absolute);
                    image.EndInit();

                    ProfilePNG.Source = image;
                }
                else
                {
                    Console.WriteLine("No user image found for login: " + login);
                }
            }
            else
            {
                Console.WriteLine("User image folder not found for login: " + login);
            }
        }

        private void SaveImage(string filePath)
        {
            string login = Properties.Settings.Default.Login;
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", login);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = "image_" + GetHash(login) + ".png"; // Имя файла изображения
            string filePathInFolder = System.IO.Path.Combine(folderPath, fileName);

            File.Copy(filePath, filePathInFolder, true);

            // Обновите путь к изображению в настройках
            Properties.Settings.Default.UserImagePath = filePathInFolder;
            Properties.Settings.Default.Save();
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
