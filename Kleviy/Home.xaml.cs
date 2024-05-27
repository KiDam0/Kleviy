using Microsoft.Win32;
using Npgsql;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Spire.Doc;
using Spire.Doc.Documents;


namespace Kleviy
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home Window;
        string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
        public Home()
        {
            InitializeComponent();
            List<string> tables = GetTablesFromDB();
            AllComboBox.ItemsSource = tables;
            Window = this;
            SuppliersGrid.ItemsSource = connectionString;
            StaffGrid.ItemsSource = connectionString;
            dataGridProducts.ItemsSource = GetProducts();
            dataGridStaff.ItemsSource = GetStaff();
        }

        //движение окна

        private void MovingWin(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Window.DragMove();
            }
        }
        /// <summary>
        /// ОКНО ТОВАРОВ
        /// </summary>
        private void TovBt_Click(object sender, RoutedEventArgs e)
        {
            if (Main.Visibility == Visibility.Visible | Suppliers.Visibility == Visibility.Visible | Staff.Visibility == Visibility.Visible)
            {
                Main.Visibility = Visibility.Collapsed;
                Main.IsEnabled = false;

                Suppliers.Visibility = Visibility.Collapsed;
                Suppliers.IsEnabled = false;

                Staff.Visibility = Visibility.Collapsed;
                Staff.IsEnabled = false;

                Products.Visibility = Visibility.Visible;
                Products.IsEnabled = true;
            }
            GC.Collect();
        }
        private string _connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";

        //получение информации из базы данных

        private List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Товар", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Colums = (int)reader.GetFieldValue<decimal>(2),
                                Price = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }
            return products;
        }

        //кнопка сохранения данных

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE Товар SET Наименование = @name, Цена = @price, Количество=@colums WHERE id_товар = @id", connection))
                    {
                        command.Parameters.AddWithValue("name", ((Product)dataGridProducts.SelectedItem).Name);
                        command.Parameters.AddWithValue("price", ((Product)dataGridProducts.SelectedItem).Price);
                        command.Parameters.AddWithValue("id", ((Product)dataGridProducts.SelectedItem).Id);
                        command.Parameters.AddWithValue("colums", ((Product)dataGridProducts.SelectedItem).Colums);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Изменения сохранены");
                dataGridProducts.ItemsSource = GetProducts();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
            }
        }

        //кнопка добавления данных

        private void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Товар (Наименование, Цена, Количество) VALUES (@name, @price, @colums)", connection))
                {
                    command.Parameters.AddWithValue("name", "Новый продукт");
                    command.Parameters.AddWithValue("price", 0);
                    command.Parameters.AddWithValue("colums", 0);
                    command.ExecuteNonQuery();
                }
            }
            dataGridProducts.ItemsSource = GetProducts();
        }

        //кнопка удаления данных

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Товар WHERE id_товар = @id", connection))
                {
                    command.Parameters.AddWithValue("id", ((Product)dataGridProducts.SelectedItem).Id);
                    command.ExecuteNonQuery();
                }
            }
            dataGridProducts.ItemsSource = GetProducts();
        }

        //класс для шапки таблицы товаров
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Colums { get; set; }
        }
        /// <summary>
        /// ОКНО ПОСТАВЩИКОВ
        /// </summary>
        private void PostBt_Click(object sender, RoutedEventArgs e)
        {
            if (Main.Visibility == Visibility.Visible | Products.Visibility == Visibility.Visible | Staff.Visibility == Visibility.Visible)
            {
                Main.Visibility = Visibility.Collapsed;
                Main.IsEnabled = false;

                Staff.Visibility = Visibility.Collapsed;
                Staff.IsEnabled = false;

                Products.Visibility = Visibility.Collapsed;
                Products.IsEnabled = false;

                Suppliers.Visibility = Visibility.Visible;
                Suppliers.IsEnabled = true;
            }
            GC.Collect();
        }
        /// <summary>
        /// ГЛАВНОЕ ОКНО
        /// </summary>
        private void GlavBt_Click(object sender, RoutedEventArgs e)
        {
            if (Suppliers.Visibility == Visibility.Visible | Products.Visibility == Visibility.Visible | Staff.Visibility == Visibility.Visible)
            {
                Staff.Visibility = Visibility.Collapsed;
                Staff.IsEnabled = false;

                Products.Visibility = Visibility.Collapsed;
                Products.IsEnabled = false;

                Suppliers.Visibility = Visibility.Collapsed;
                Suppliers.IsEnabled = false;

                Main.Visibility = Visibility.Visible;
                Main.IsEnabled = true;
            }
            GC.Collect();
        }
        /// <summary>
        /// ОКНО СОТРУДНИКОВ
        /// </summary>
        private void SotrBt_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            string login = mainWindow.LoginBox.Text;
            Properties.Settings.Default.Login = login;
            if (Suppliers.Visibility == Visibility.Visible | Products.Visibility == Visibility.Visible | Main.Visibility == Visibility.Visible)
            {
                Products.Visibility = Visibility.Collapsed;
                Products.IsEnabled = false;

                Suppliers.Visibility = Visibility.Collapsed;
                Suppliers.IsEnabled = false;

                Main.Visibility = Visibility.Collapsed;
                Main.IsEnabled = false;

                Staff.Visibility = Visibility.Visible;
                Staff.IsEnabled = true;

                if (login == "Adm1")
                {
                    dataGridStaff.Visibility = Visibility.Visible;
                    StaffGrid.Visibility = Visibility.Collapsed;

                    SaveDataStaff.Visibility = Visibility.Visible;
                    AddDataStaff.Visibility = Visibility.Visible;
                    DeleteDataStaff.Visibility = Visibility.Visible;
                    LoadDataStaff.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dataGridStaff.Visibility = Visibility.Collapsed;
                    StaffGrid.Visibility = Visibility.Visible;

                    SaveDataStaff.Visibility = Visibility.Collapsed;
                    AddDataStaff.Visibility = Visibility.Collapsed;
                    DeleteDataStaff.Visibility = Visibility.Collapsed;
                    LoadDataStaff.Visibility = Visibility.Visible;
                }
            }
            GC.Collect();
        }
        //класс для данных
        public class User
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }

            public User(string login, string password, string role)
            {
                Login = login;
                Password = password;
                Role = role;
            }
        }

        //"UPDATE Сотрудник SET Фамилия = @surnameStaff, Имя = @nameStaff, Отчество = @patronymicStaff, Дата_рождения = @dateStaff, id_должность = @postStaff, id_Вход = @loginStaff WHERE id_сотрудник = @idStaff", connection
        //сохранить изменения 

        private void SaveChangesStaff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE Сотрудник SET Фамилия = @surnameStaff, Имя = @nameStaff, Отчество = @patronymicStaff, Дата_рождения = @dateStaff, id_должность = @postStaff, id_Вход = @loginStaff WHERE id_сотрудник = @idStaff", connection))
                    {
                        foreach (Staffs staff in dataGridStaff.Items.OfType<Staffs>())
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("surnameStaff", staff.surnameStaff);
                            command.Parameters.AddWithValue("nameStaff", staff.nameStaff);
                            command.Parameters.AddWithValue("patronymicStaff", staff.patronymicStaff);
                            command.Parameters.AddWithValue("dateStaff", staff.dateStaff);
                            command.Parameters.AddWithValue("postStaff", staff.postStaff);
                            command.Parameters.AddWithValue("loginStaff", staff.loginStaff);
                            command.Parameters.AddWithValue("idStaff", staff.idStaff);
                            command.ExecuteNonQuery();
                        }

                    }
                }
                MessageBox.Show("Изменения сохранены");
                dataGridStaff.ItemsSource = GetStaff();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
            }
        }

        //кнопка добавления данных

        private void AddNewProductStaff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridStaff.SelectedItem == null)
                {
                    MessageBox.Show("Please select a staff member to edit.");
                    return;
                }

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Сотрудник (Фамилия, Имя, Отчество, Дата_рождения, id_должность, id_Вход) VALUES (@surnameStaff, @nameStaff, @patronymicStaff, @dateStaff, @postStaff, @loginStaff)", connection))
                    {
                        command.Parameters.AddWithValue("surnameStaff", ((Staffs)dataGridStaff.SelectedItem).surnameStaff);
                        command.Parameters.AddWithValue("nameStaff", ((Staffs)dataGridStaff.SelectedItem).nameStaff);
                        command.Parameters.AddWithValue("patronymicStaff", ((Staffs)dataGridStaff.SelectedItem).patronymicStaff);
                        command.Parameters.AddWithValue("dateStaff", ((Staffs)dataGridStaff.SelectedItem).dateStaff);
                        command.Parameters.AddWithValue("postStaff", ((Staffs)dataGridStaff.SelectedItem).postStaff);
                        command.Parameters.AddWithValue("loginStaff", ((Staffs)dataGridStaff.SelectedItem).loginStaff);
                        command.ExecuteNonQuery();
                    }
                }
                dataGridStaff.ItemsSource = GetStaff();
                MessageBox.Show("Новый сотрудник добавлен");
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message);
            }
        }

        //кнопка удаления данных

        private void DeleteProductStaff_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Сотрудник WHERE id_сотрудник = @idStaff", connection))
                {
                    command.Parameters.AddWithValue("idStaff", ((Staffs)dataGridStaff.SelectedItem).idStaff);
                    command.ExecuteNonQuery();
                }
            }
            dataGridStaff.ItemsSource = GetStaff();
        }

        //элементы для базы данных

        public class Staffs
        {
            public int idStaff { get; set; }
            public string surnameStaff { get; set; }
            public string nameStaff { get; set; }
            public string patronymicStaff { get; set; }
            public DateTime dateStaff { get; set; } = new DateTime(1, 1, 1);
            public int postStaff { get; set; }
            public int loginStaff { get; set; }
        }

        //присвоение значений для datagrid из базы данных

        private List<Staffs> GetStaff()
        {
            List<Staffs> staff = new List<Staffs>();
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Сотрудник", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            staff.Add(new Staffs
                            {
                                idStaff = reader.GetInt32(0),
                                surnameStaff = reader.GetString(1),
                                nameStaff = reader.GetString(2),
                                patronymicStaff = reader.GetString(3),
                                dateStaff = reader.GetDateTime(4),
                                postStaff = reader.GetInt32(5),
                                loginStaff = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            return staff;
        }
        /// <summary>
        /// ПРОФИЛЬ
        /// </summary>
        private void ProfileBt_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile();
            profile.Show();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
            GC.Collect();
        }

        //сохранение таблицы в документе word

        private void ExportToWord(DataTable dt)
        {
            // Создаем новый документ
            Document document = new Document();
            Section section = document.AddSection();

            //Текст
            Paragraph Para = section.AddParagraph();
            Para.AppendText("Учёт товара");
            Para.Format.BeforeSpacing = 10;
            Para.Format.AfterSpacing = 10;

            //сотрудник
            Paragraph Para2 = section.AddParagraph();
            Para2.AppendText("Сотрудник: ");
            Para2.Format.BeforeSpacing = 10;
            Para2.Format.AfterSpacing = 10;

            //поиск сотрудника
            string fieldName = "DropDownList";
            DropDownFormField list = Para2.AppendField(fieldName, FieldType.FieldFormDropDown) as DropDownFormField;
            list.DropDownItems.Add("Ольга");
            list.DropDownItems.Add("Нина");
            list.DropDownItems.Add("Ульяна");

            //дата
            Paragraph Para3 = section.AddParagraph();
            Para3.AppendText("Дата: ");
            Para3.Format.BeforeSpacing = 10;
            Para3.Format.AfterSpacing = 10;

            // Добавляем столбцы в таблицу
            Table table = section.AddTable(true);
            Paragraph Para4 = section.AddParagraph();
            Para4.AppendText("Таблица учёта товаров.");
            Para4.Format.BeforeSpacing = 10;
            Para4.Format.AfterSpacing = 20;
            table.ResetCells(dt.Rows.Count + 1, dt.Columns.Count);

            // Наполняем строку заголовков
            TableRow headerRow = table.Rows[0];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                // Добавляем ячейку и наполняем текстом из DataGrid
                headerRow.Cells[i].AddParagraph().AppendText(dt.Columns[i].ColumnName);
                headerRow.Cells[i].Paragraphs[0].Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                headerRow.Cells[i].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
            }

            // Заполняем таблицу данными
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                TableRow dataRow = table.Rows[r + 1];
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    dataRow.Cells[c].AddParagraph().AppendText(dt.Rows[r][c].ToString());
                    dataRow.Cells[c].Paragraphs[0].Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                    dataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                }
            }

            //Сохранение
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Word file(*.docx) |*.docx";
            dlg.InitialDirectory = @"C:";
            if (dlg.ShowDialog() == true)
            {
                document.SaveToFile(dlg.FileName);
            }
            GC.Collect();
        }

        private void AllComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedTable = AllComboBox.SelectedItem as string;
            if (selectedTable != null)
            {
                LoadDataAll(selectedTable);
            }
        }

        private void AllGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoadDataAll(string tableName)
        {
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = $"SELECT * FROM {tableName}";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        AllGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            GC.Collect();
        }

        //получение таблиц из базы данных для combobox

        private List<string> GetTablesFromDB()
        {
            List<string> tables = new List<string>();
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_name NOT IN ('Данные_для_входа', 'Должность', 'Поставщик', 'Сотрудник');", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
                GC.Collect();
            }
            return tables;
        }

        private void SuppliersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //загрузка таблицы поставщиков

        private void LoadDataSuppliers_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
            string query = "SELECT * FROM Поставщик";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                DataTable suppliersDataTable = new DataTable();

                adapter.Fill(suppliersDataTable);

                SuppliersGrid.ItemsSource = suppliersDataTable.DefaultView; // Привязка данных к DataGrid
            }
            GC.Collect();
        }

        //загрузка таблицы сотрудников

        private void LoadDataStaff_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
            string query = "SELECT * FROM Сотрудник";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                DataTable staffDataTable = new DataTable();

                adapter.Fill(staffDataTable);

                StaffGrid.ItemsSource = staffDataTable.DefaultView; // Привязка данных к DataGrid
            }
            GC.Collect();
        }

        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = new DataTable();
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
            try
            {
                // Получаем запрос из текстового поля
                string query = $"SELECT p.id_товар, p.id_поставщик, s.Наименование FROM \"Товарная_накладная\" p JOIN Товар s ON p.id_товар = s.id_товар WHERE p.id_поставщик = {NumberSupl.Text};";

                // Подключаемся к базе данных
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    // Выполняем запрос
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Заполняем таблицу данных результатами запроса
                        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                        adapter.Fill(dataTable);

                        // Привязываем таблицу данных к DataGrid
                        AllGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            GC.Collect();
        }

        private void LoadToWord_Click(object sender, RoutedEventArgs e)
        {
            var dt = ((DataView)AllGrid.ItemsSource).ToTable();
            ExportToWord(dt);
        }
    }
}
//мусор
//GC.Collect();
