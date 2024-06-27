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
using System.Threading;


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

            string savedImagePath = Properties.Settings.Default.UserImagePath;

            if (!string.IsNullOrEmpty(savedImagePath) && File.Exists(savedImagePath))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(savedImagePath);
                image.EndInit();

                ProfilePNG.Source = image;
                _previousImage = image;
            }
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
        private BitmapImage _previousImage;

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                // Hide the image and release the memory
                ProfilePNG.Source = null;

                string oldImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserProfileImage.png");

                // Save the new image to a temporary file
                string tempFilePath = Path.GetTempFileName();
                SaveImageToApplicationFolder(openFileDialog.FileName, tempFilePath);

                // Replace the original file with the temporary file
                File.Delete(oldImagePath);
                File.Move(tempFilePath, oldImagePath);

                // Load the new image and display it
                BitmapImage newImage = new BitmapImage();
                newImage.BeginInit();
                newImage.CacheOption = BitmapCacheOption.OnLoad;
                newImage.UriSource = new Uri(oldImagePath, UriKind.Absolute);
                newImage.EndInit();

                // Create a new stream and load the image from it
                using (FileStream fs = new FileStream(oldImagePath, FileMode.Open, FileAccess.Read))
                {
                    newImage.StreamSource = fs;
                }

                ProfilePNG.Source = newImage;

                // Сохраняем путь к новому изображению в настройках
                Properties.Settings.Default.UserImagePath = oldImagePath;
                Properties.Settings.Default.Save();
            }
        }

        private void SaveImageToApplicationFolder(string sourcePath, string destinationPath)
        {
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = sourceStream;
                    image.EndInit();

                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(destinationStream);
                }
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
            string imagePath = Properties.Settings.Default.UserImagePath;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imagePath, UriKind.Absolute);
                image.EndInit();

                ProfilePNG.Source = image;
            }
            else
            {
                Console.WriteLine("Не найдено изображение профиля: " + login);
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
