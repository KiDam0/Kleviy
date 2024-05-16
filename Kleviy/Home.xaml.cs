using Microsoft.Win32;
using Npgsql;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Windows.Markup;
using System.Web.UI.WebControls.WebParts;


namespace Kleviy
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home Window;
        public Home()
        {
            InitializeComponent();
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123";
            InitializeComponent();
            List<string> tables = GetTablesFromDB();
            AllComboBox.ItemsSource = tables;
            Window = this;
            ProductGrid.ItemsSource = connectionString;
            SuppliersGrid.ItemsSource = connectionString;
            StaffGrid.ItemsSource = connectionString;
        }
        private void MovingWin(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                MainWindow.Window.DragMove();
            }
        }
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

        private void SotrBt_Click(object sender, RoutedEventArgs e)
        {
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
            }
            GC.Collect();
        }

        private void ProfileBt_Click(object sender, RoutedEventArgs e)
        {
            Profile Profile = new Profile();
            Profile.Show();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
        //загрузка таблицы товаров
        private void LoadDataProduct_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Host = localhost; Port = 5433; Database = Учёт_товара; Username = postgres; Password = 123"; //строка подключения
            string query = "SELECT * FROM Товар"; //запрос

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                System.Data.DataTable dataTable = new System.Data.DataTable();

                adapter.Fill(dataTable);

                ProductGrid.ItemsSource = dataTable.DefaultView; // Привязка данных к DataGrid
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
