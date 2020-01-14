using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TaskManager.View;
using Excel = Microsoft.Office.Interop.Excel;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для ShowEvents.xaml
    /// </summary>
    public partial class ShowEvents : Page
    {
        NavigationService navService;
        string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\TaskManager.mdb";
        ObservableCollection<Event> Events = new ObservableCollection<Event>();
        string typeTitle, kindTitle, doneTitle;
        public ShowEvents()
        {
            InitializeComponent();
            FillTypeComboBox();
            FillKindComboBox();
            FillList();
        }

        private void FillList()
        {
            OleDbConnection eventConnection = new OleDbConnection(connectionString);

            OleDbCommand eventFillCommand = new OleDbCommand("Select * from Event where Start Between @today and @tomorrow Order by Start");
            eventFillCommand.Parameters.AddWithValue("@today", DateTime.Today);
            eventFillCommand.Parameters.AddWithValue("@tomorrow", DateTime.Today.AddDays(1));
            eventFillCommand.Connection = eventConnection;
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(eventFillCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Event");

            eventConnection.Open();

            Events.Clear();

            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                string selectTypeQuery = "SELECT TypeTitle FROM Type where ID = " + Convert.ToInt32(dr[2]) + "";
                OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, eventConnection);

                OleDbDataReader dataTypeReader = selectTypeCommand.ExecuteReader();

                if (dataTypeReader.HasRows)
                {
                    while (dataTypeReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        typeTitle = dataTypeReader.GetString(0);
                    }
                }

                string selectKindQuery = "SELECT KindTitle FROM Kind where ID = " + Convert.ToInt32(dr[3]) + "";
                OleDbCommand selectKindCommand = new OleDbCommand(selectKindQuery, eventConnection);

                OleDbDataReader dataKindReader = selectKindCommand.ExecuteReader();

                if (dataKindReader.HasRows)
                {
                    while (dataKindReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        kindTitle = dataKindReader.GetString(0);
                    }
                }

                if (Convert.ToBoolean(dr[11]))
                {
                    doneTitle = "Завершена";
                }
                else
                {
                    doneTitle = "Не завершена";
                }

                Events.Add(new Event
                {
                    Id = Convert.ToInt32(dr[0]),
                    Title = dr[1].ToString(),
                    TypeId = Convert.ToInt32(dr[2]),
                    TypeTitle = typeTitle,
                    KindId = Convert.ToInt32(dr[3]),
                    KindTitle = kindTitle,
                    Start = Convert.ToDateTime(dr[4].ToString()),
                    End = Convert.ToDateTime(dr[5].ToString()),
                    Done = Convert.ToBoolean(dr[11]),
                    DoneText = doneTitle,
                    ReportID = Convert.ToInt32(dr[12])
                });
            }
            try
            {
                dataGrid.ItemsSource = Events;
            }

            catch (Exception)
            {

            }
            finally
            {
                dataSet = null;
                dataAdapter.Dispose();
                eventConnection.Close();
                eventConnection.Dispose();
            }
        }

        private void FillTypeComboBox()
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            string selectTypeQuery = "SELECT * FROM Type";
            OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, dbConnection);

            dbConnection.Open();
            OleDbDataReader dataReader = selectTypeCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    //typeName = dataReader[1].ToString();
                    comboBox.Items.Add(dataReader.GetString(1));
                }
            }
            dbConnection.Close();
        }

        private void FillKindComboBox()
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            string selectTypeQuery = "SELECT * FROM Kind";
            OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, dbConnection);

            dbConnection.Open();
            OleDbDataReader dataReader = selectTypeCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    //typeName = dataReader[1].ToString();
                    comboBox1.Items.Add(dataReader.GetString(1));
                }
            }
            dbConnection.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                try
                {
                    var row_list = (Event)dataGrid.SelectedItem;
                    //OrderClass.MedId = ;

                    OleDbConnection dbConnection = new OleDbConnection(connectionString);
                    OleDbCommand dbCommand = new OleDbCommand();
                    dbCommand.CommandType = CommandType.Text;

                    dbCommand.CommandText = "DELETE from Event where ID = @id and Title = @title and KindID = @kindId";

                    dbCommand.Parameters.AddWithValue("@id", row_list.Id);
                    dbCommand.Parameters.AddWithValue("@title", row_list.Title);
                    dbCommand.Parameters.AddWithValue("@kindId", row_list.KindId);

                    dbCommand.Connection = dbConnection;
                    dbConnection.Open();
                    dbCommand.ExecuteNonQuery();
                    dbConnection.Close();

                    MessageBox.Show("Выбранная задача успешно удалена!");
                    FillList();
                }
                catch
                {

                }
                dataGrid.SelectedItem = null;
            }
            else
            {
                popup1.IsOpen = true;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                try
                {
                    var row_list = (Event)dataGrid.SelectedItem;

                    Event.oldId = row_list.Id;
                    Event.oldTitle = row_list.Title;
                    navService = NavigationService.GetNavigationService(this);
                    navService.Navigate(new System.Uri("View/ChangeEvent.xaml", UriKind.RelativeOrAbsolute));
                    dataGrid.SelectedItem = null;
                }
                catch (Exception)
                {

                }
            }
            else
            {
                popup.IsOpen = true;
            }
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            navService.Navigate(new System.Uri("View/AddEvent.xaml", UriKind.RelativeOrAbsolute));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            comboBox.Text = null;
            comboBox1.Text = null;
            FillList();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                try
                {
                    var row_list = (Event)dataGrid.SelectedItem;

                    Event.oldId = row_list.Id;
                    Event.oldTitle = row_list.Title;
                    Event.DayStart = row_list.Start.Date.ToShortDateString();
                    if (row_list.Done)
                    {
                        if (row_list.ReportID == 0)
                        {
                            navService = NavigationService.GetNavigationService(this);
                            navService.Navigate(new System.Uri("View/AddReport.xaml", UriKind.RelativeOrAbsolute));
                            dataGrid.SelectedItem = null;
                        }
                        else
                        {
                            MessageBox.Show("Вы уже создали отчет к выбранной задаче!");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Вы не можете создать отчет к незавершенной задаче!");
                        //popup.IsOpen = true;
                        dataGrid.SelectedItem = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                popup2.IsOpen = true;
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox1.Text = null;
            string selectQuery = "SELECT ID FROM Type WHERE TypeTitle = '" + comboBox.SelectedItem + "'";

            OleDbConnection dbConnection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand(selectQuery, dbConnection);

            dbConnection.Open();

            OleDbDataReader dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                Type.Id = Convert.ToInt32(dataReader[0]);
            }

            dbConnection.Close();

            OleDbCommand dbCommand = new OleDbCommand();
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = "SELECT * FROM Event WHERE TypeID = @type Order by Start";
            dbCommand.Parameters.AddWithValue("@type", Type.Id);
            dbCommand.Connection = dbConnection;
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(dbCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Event");

            dbConnection.Open();

            Events.Clear();

            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                string selectTypeQuery = "SELECT TypeTitle FROM Type where ID = " + Convert.ToInt32(dr[2]) + "";
                OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, dbConnection);

                OleDbDataReader dataTypeReader = selectTypeCommand.ExecuteReader();

                if (dataTypeReader.HasRows)
                {
                    while (dataTypeReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        typeTitle = dataTypeReader.GetString(0);
                    }
                }

                string selectKindQuery = "SELECT KindTitle FROM Kind where ID = " + Convert.ToInt32(dr[3]) + "";
                OleDbCommand selectKindCommand = new OleDbCommand(selectKindQuery, dbConnection);

                OleDbDataReader dataKindReader = selectKindCommand.ExecuteReader();

                if (dataKindReader.HasRows)
                {
                    while (dataKindReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        kindTitle = dataKindReader.GetString(0);
                    }
                }

                if (Convert.ToBoolean(dr[11]))
                {
                    doneTitle = "Завершена";
                }
                else
                {
                    doneTitle = "Не завершена";
                }

                Events.Add(new Event
                {
                    Id = Convert.ToInt32(dr[0]),
                    Title = dr[1].ToString(),
                    TypeId = Convert.ToInt32(dr[2]),
                    TypeTitle = typeTitle,
                    KindId = Convert.ToInt32(dr[3]),
                    KindTitle = kindTitle,
                    Start = Convert.ToDateTime(dr[4].ToString()),
                    End = Convert.ToDateTime(dr[5].ToString()),
                    Done = Convert.ToBoolean(dr[11]),
                    DoneText = doneTitle,
                    ReportID = Convert.ToInt32(dr[12])
                });
            }
            try
            {
                //eventListBox.ItemsSource = Events;
            }

            catch (Exception)
            {
                Console.WriteLine("False");
            }
            finally
            {
                dataSet = null;
                dataAdapter.Dispose();
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox.Text = null;
            string selectQuery = "SELECT ID FROM Kind WHERE KindTitle = '" + comboBox1.SelectedItem + "'";

            OleDbConnection dbConnection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand(selectQuery, dbConnection);

            dbConnection.Open();

            OleDbDataReader dataReader = command.ExecuteReader();

            if (dataReader.Read())
            {
                Type.Id = Convert.ToInt32(dataReader[0]);
            }

            dbConnection.Close();

            OleDbCommand dbCommand = new OleDbCommand();
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = "SELECT * FROM Event WHERE KindID = @kind Order by Start";
            dbCommand.Parameters.AddWithValue("@kind", Type.Id);
            dbCommand.Connection = dbConnection;
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(dbCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Event");

            dbConnection.Open();

            Events.Clear();

            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                string selectTypeQuery = "SELECT TypeTitle FROM Type where ID = " + Convert.ToInt32(dr[2]) + "";
                OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, dbConnection);

                OleDbDataReader dataTypeReader = selectTypeCommand.ExecuteReader();

                if (dataTypeReader.HasRows)
                {
                    while (dataTypeReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        typeTitle = dataTypeReader.GetString(0);
                    }
                }

                string selectKindQuery = "SELECT KindTitle FROM Kind where ID = " + Convert.ToInt32(dr[3]) + "";
                OleDbCommand selectKindCommand = new OleDbCommand(selectKindQuery, dbConnection);

                OleDbDataReader dataKindReader = selectKindCommand.ExecuteReader();

                if (dataKindReader.HasRows)
                {
                    while (dataKindReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        kindTitle = dataKindReader.GetString(0);
                    }
                }

                if (Convert.ToBoolean(dr[11]))
                {
                    doneTitle = "Завершена";
                }
                else
                {
                    doneTitle = "Не завершена";
                }

                Events.Add(new Event
                {
                    Id = Convert.ToInt32(dr[0]),
                    Title = dr[1].ToString(),
                    TypeId = Convert.ToInt32(dr[2]),
                    TypeTitle = typeTitle,
                    KindId = Convert.ToInt32(dr[3]),
                    KindTitle = kindTitle,
                    Start = Convert.ToDateTime(dr[4].ToString()),
                    End = Convert.ToDateTime(dr[5].ToString()),
                    Done = Convert.ToBoolean(dr[11]),
                    DoneText = doneTitle,
                    ReportID = Convert.ToInt32(dr[12])
                });
            }
            try
            {
                //eventListBox.ItemsSource = Events;
            }

            catch (Exception)
            {
                Console.WriteLine("False");
            }
            finally
            {
                dataSet = null;
                dataAdapter.Dispose();
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime choosenDate = calendar.SelectedDate.Value;

            OleDbConnection dbConnection = new OleDbConnection(connectionString);
            OleDbCommand dbCommand = new OleDbCommand();
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = "SELECT * FROM Event WHERE Start between @start and @tomorrow Order by Start";
            dbCommand.Parameters.AddWithValue("@start", choosenDate);
            dbCommand.Parameters.AddWithValue("@tomorrow", choosenDate.AddDays(1));
            dbCommand.Connection = dbConnection;
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(dbCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Event");

            dbConnection.Open();

            Events.Clear();

            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                string selectTypeQuery = "SELECT TypeTitle FROM Type where ID = " + Convert.ToInt32(dr[2]) + "";
                OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, dbConnection);

                OleDbDataReader dataTypeReader = selectTypeCommand.ExecuteReader();

                if (dataTypeReader.HasRows)
                {
                    while (dataTypeReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        typeTitle = dataTypeReader.GetString(0);
                    }
                }

                string selectKindQuery = "SELECT KindTitle FROM Kind where ID = " + Convert.ToInt32(dr[3]) + "";
                OleDbCommand selectKindCommand = new OleDbCommand(selectKindQuery, dbConnection);

                OleDbDataReader dataKindReader = selectKindCommand.ExecuteReader();

                if (dataKindReader.HasRows)
                {
                    while (dataKindReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        kindTitle = dataKindReader.GetString(0);
                    }
                }

                if (Convert.ToBoolean(dr[11]))
                {
                    doneTitle = "Завершена";
                }
                else
                {
                    doneTitle = "Не завершена";
                }

                Events.Add(new Event
                {
                    Id = Convert.ToInt32(dr[0]),
                    Title = dr[1].ToString(),
                    TypeId = Convert.ToInt32(dr[2]),
                    TypeTitle = typeTitle,
                    KindId = Convert.ToInt32(dr[3]),
                    KindTitle = kindTitle,
                    Start = Convert.ToDateTime(dr[4].ToString()),
                    End = Convert.ToDateTime(dr[5].ToString()),
                    Done = Convert.ToBoolean(dr[11]),
                    DoneText = doneTitle,
                    ReportID = Convert.ToInt32(dr[12])
                });
            }
            try
            {
                //eventListBox.ItemsSource = Events;
            }

            catch (Exception)
            {
                Console.WriteLine("False");
            }
            finally
            {
                dataSet = null;
                dataAdapter.Dispose();
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                try
                {
                    var row_list = (Event)dataGrid.SelectedItem;

                    Event.oldId = row_list.Id;
                    Event.oldTitle = row_list.Title;
                    Event.DayStart = row_list.Start.ToShortDateString();
                    if (row_list.ReportID != 0)
                    {
                        navService = NavigationService.GetNavigationService(this);
                        navService.Navigate(new System.Uri("View/ShowReport.xaml", UriKind.RelativeOrAbsolute));
                        dataGrid.SelectedItem = null;
                    }
                    else
                    {
                        MessageBox.Show("Вы еще не создали отчет к данной задаче!");
                        //popup.IsOpen = true;
                        dataGrid.SelectedItem = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                popup3.IsOpen = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application application = new Excel.Application();
            application.Visible = true;

            Excel.Workbook workbook;
            Excel.Worksheet worksheet;
            workbook = application.Workbooks.Add();

            worksheet = (Excel.Worksheet)workbook.Sheets[1];
            worksheet.Name = "Мероприятия";
            worksheet.Cells[1, 1].Value = "Название задачи";
            worksheet.Cells[1, 2].Value = "Тип задачи";
            worksheet.Cells[1, 3].Value = "Вид задачи";
            worksheet.Cells[1, 4].Value = "Начало задачи";
            worksheet.Cells[1, 5].Value = "Окончание задачи";
            worksheet.Cells[1, 6].Value = "Описание задачи";
            worksheet.Cells[1, 7].Value = "Место проведения задачи";
            worksheet.Cells[1, 8].Value = "Статус";


            for (int i = 2; i <= Events.Count + 1; i++)
            {
                worksheet.Cells[i, 1].Value = Events[i - 2].Title;
                worksheet.Cells[i, 2].Value = Events[i - 2].TypeTitle;
                worksheet.Cells[i, 3].Value = Events[i - 2].KindTitle;
                worksheet.Cells[i, 4].Value = Events[i - 2].Start;
                worksheet.Cells[i, 5].Value = Events[i - 2].End;
                worksheet.Cells[i, 6].Value = Events[i - 2].Description;
                worksheet.Cells[i, 7].Value = Events[i - 2].Location;
                worksheet.Cells[i, 8].Value = Events[i - 2].DoneText;
            }

            workbook.SaveAs(Filename: System.AppDomain.CurrentDomain.BaseDirectory + "exported.xlsx");

            MessageBox.Show("Вы успешно экспортировали задачи! Ваш файл находится в корневой папке программы под названием exported.xlsx");
        }

        private void button44_Click(object sender, RoutedEventArgs e)
        {
            OleDbConnection eventConnection = new OleDbConnection(connectionString);

            OleDbCommand eventFillCommand = new OleDbCommand("Select * from Event");
            eventFillCommand.Connection = eventConnection;
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(eventFillCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Event");

            eventConnection.Open();

            Events.Clear();

            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                string selectTypeQuery = "SELECT TypeTitle FROM Type where ID = " + Convert.ToInt32(dr[2]) + "";
                OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, eventConnection);

                OleDbDataReader dataTypeReader = selectTypeCommand.ExecuteReader();

                if (dataTypeReader.HasRows)
                {
                    while (dataTypeReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        typeTitle = dataTypeReader.GetString(0);
                    }
                }

                string selectKindQuery = "SELECT KindTitle FROM Kind where ID = " + Convert.ToInt32(dr[3]) + "";
                OleDbCommand selectKindCommand = new OleDbCommand(selectKindQuery, eventConnection);

                OleDbDataReader dataKindReader = selectKindCommand.ExecuteReader();

                if (dataKindReader.HasRows)
                {
                    while (dataKindReader.Read())
                    {
                        //typeName = dataReader[1].ToString();
                        kindTitle = dataKindReader.GetString(0);
                    }
                }

                if (Convert.ToBoolean(dr[11]))
                {
                    doneTitle = "Завершена";
                }
                else
                {
                    doneTitle = "Не завершена";
                }

                Events.Add(new Event
                {
                    Id = Convert.ToInt32(dr[0]),
                    Title = dr[1].ToString(),
                    TypeId = Convert.ToInt32(dr[2]),
                    TypeTitle = typeTitle,
                    KindId = Convert.ToInt32(dr[3]),
                    KindTitle = kindTitle,
                    Start = Convert.ToDateTime(dr[4].ToString()),
                    End = Convert.ToDateTime(dr[5].ToString()),
                    Done = Convert.ToBoolean(dr[11]),
                    DoneText = doneTitle,
                    ReportID = Convert.ToInt32(dr[12])
                });
            }
            try
            {
                //dataGrid.ItemsSource = Events;
            }

            catch (Exception)
            {

            }
            finally
            {
                dataSet = null;
                dataAdapter.Dispose();
                eventConnection.Close();
                eventConnection.Dispose();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        void Handle(CheckBox checkBox)
        {
            bool flag = checkBox.IsChecked.Value;

            if (dataGrid.SelectedItem != null)
            {
                try
                {
                    var row_list = (Event)dataGrid.SelectedItem;

                    OleDbConnection dbConnection = new OleDbConnection(connectionString);
                    OleDbCommand dbCommand = new OleDbCommand();
                    dbCommand.CommandType = CommandType.Text;

                    dbCommand.CommandText = "Update Event Set Done = @done where ID = @id and Title = @title and KindID = @kindId";

                    dbCommand.Parameters.AddWithValue("@done", flag);
                    dbCommand.Parameters.AddWithValue("@id", row_list.Id);
                    dbCommand.Parameters.AddWithValue("@title", row_list.Title);
                    dbCommand.Parameters.AddWithValue("@kindId", row_list.KindId);

                    dbCommand.Connection = dbConnection;
                    dbConnection.Open();
                    dbCommand.ExecuteNonQuery();
                    dbConnection.Close();

                    if (flag)
                    {
                        MessageBox.Show("Вы завершили задачу!");
                    }
                    else
                    {
                        MessageBox.Show("Вы сняли пометку о завершении");
                    }
                    FillList();
                }
                catch
                {

                }
                dataGrid.SelectedItem = null;
            }
        }
    }
}
