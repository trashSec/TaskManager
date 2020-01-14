using Microsoft.Win32;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для AddReport.xaml
    /// </summary>
    public partial class AddReport : Page
    {
        NavigationService navService;
        string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\TaskManager.mdb";
        int typeID, kindID;
        int member1, member2, member3;
        String memberName, memberSurname, memberPatronymic, memberPhone;
        String fileName, file, newFile, path, ext;

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", System.AppDomain.CurrentDomain.BaseDirectory + Event.DayStart.ToString() + @"\" + Event.oldTitle.ToString());
        }

        int num = 0;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            num += 1;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            file = num + ".jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
                ext = Path.GetExtension(fileName);
                file = num + ext;              
                newFile = System.AppDomain.CurrentDomain.BaseDirectory + @"\" + Event.DayStart.ToString() + @"\" + Event.oldTitle.ToString() + @"\" + file;                
                try
                {
                    File.Copy(fileName, newFile);
                    MessageBox.Show("Файл успешно загружен!");
                }
                catch (Exception)
                {
                    File.Delete(newFile);
                    File.Copy(fileName, newFile);
                    MessageBox.Show("Файл успешно перезаписан!");
                }
            }
        }

        public AddReport()
        {
            InitializeComponent();
            FillFields();
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Event.DayStart.ToString()));
            Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + Event.DayStart.ToString(), Event.oldTitle.ToString()));
        }

        private void FillFields()
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);
            OleDbCommand dbCommand = new OleDbCommand();
            dbCommand.CommandType = CommandType.Text;

            dbCommand.CommandText = "SELECT * FROM Event WHERE [ID] = @id and [Title] = @oldTitle";

            dbCommand.Parameters.AddWithValue("@id", Event.oldId);
            dbCommand.Parameters.AddWithValue("@oldTitle", Event.oldTitle);

            dbCommand.Connection = dbConnection;

            dbConnection.Open();
            OleDbDataReader dataAllReader = dbCommand.ExecuteReader();

            if (dataAllReader.Read())
            {
                string selectTypeQuery = "SELECT TypeTitle FROM Type where ID = " + Convert.ToInt32(dataAllReader[2]) + "";
                OleDbCommand selectTypeCommand = new OleDbCommand(selectTypeQuery, dbConnection);

                OleDbDataReader dataTypeReader = selectTypeCommand.ExecuteReader();

                if (dataTypeReader.HasRows)
                {
                    while (dataTypeReader.Read())
                    {
                        label4.Content = dataTypeReader[0].ToString();
                    }
                }

                string selectKindQuery = "SELECT KindTitle FROM Kind where ID = " + Convert.ToInt32(dataAllReader[3]) + "";
                OleDbCommand selectKindCommand = new OleDbCommand(selectKindQuery, dbConnection);

                OleDbDataReader dataKindReader = selectKindCommand.ExecuteReader();

                if (dataKindReader.HasRows)
                {
                    while (dataKindReader.Read())
                    {
                        label5.Content = dataKindReader[0].ToString();
                    }
                }
                label3.Content = dataAllReader[1].ToString();
                //comboBox.SelectedIndex = Convert.ToInt32(dataAllReader[2]) - 1;
                //comboBox_Copy.SelectedIndex = Convert.ToInt32(dataAllReader[3]) - 1;
                label6.Content = dataAllReader[4].ToString();
                label7.Content = dataAllReader[5].ToString();
                label8.Content = dataAllReader[6].ToString();
                member1 = Convert.ToInt32(dataAllReader[7]);
                member2 = Convert.ToInt32(dataAllReader[8]);
                member3 = Convert.ToInt32(dataAllReader[9]);
                label9.Content = dataAllReader[10].ToString();
            }
            dbConnection.Close();

            if (member1 != 0)
            {
                SetMemberByID(member1, label13, label13_Copy);
            }
            if (member2 != 0)
            {
                SetMemberByID(member2, label13_Copy1, label13_Copy2);
            }
            if (member3 != 0)
            {
                SetMemberByID(member3, label13_Copy3, label13_Copy4);
            }           
        }

        private void SetMemberByID(int memberID, Label label, Label phone)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            OleDbCommand selectMemberCommand = new OleDbCommand();
            selectMemberCommand.CommandType = CommandType.Text;
            selectMemberCommand.CommandText = "SELECT * FROM Member where [ID] = @id";
            selectMemberCommand.Parameters.AddWithValue("@id", memberID);
            selectMemberCommand.Connection = dbConnection;

            dbConnection.Open();
            OleDbDataReader dataMemberReader = selectMemberCommand.ExecuteReader();
            if (dataMemberReader.HasRows)
            {
                while (dataMemberReader.Read())
                {
                    //memberID = Convert.ToInt32(dataMemberReader[0]);
                    memberName = dataMemberReader[1].ToString();
                    memberSurname = dataMemberReader[2].ToString();
                    memberPatronymic = dataMemberReader[3].ToString();
                    memberPhone = dataMemberReader[4].ToString();

                    //textBox3.Text = membName;
                    //textBox4.Text = membSurname;
                    //textBox5.Text = membPatr;
                    //maskedTextBox1.Text = membPhone;
                }
            }
            dbConnection.Close();

            label.Content = memberSurname + " " + memberName + " " + memberPatronymic;
            phone.Content = memberPhone;
        }

        private int GetMemberID(Label label, Label phone)
        {
            if (!String.IsNullOrEmpty((string)label.Content) && !String.IsNullOrEmpty((string)phone.Content))
            {
                int id = 0;
                string[] memberData = label.Content.ToString().Split(' ');

                OleDbConnection dbConnection = new OleDbConnection(connectionString);

                OleDbCommand selectMemberCommand = new OleDbCommand();
                selectMemberCommand.CommandType = CommandType.Text;
                selectMemberCommand.CommandText = "SELECT ID FROM Member where [FirstName] = @name AND [SurName] = @surname AND [Patronymic] = @patr AND [PhoneNumber] = @phone";
                selectMemberCommand.Parameters.AddWithValue("@name", memberData[1]);
                selectMemberCommand.Parameters.AddWithValue("@surname", memberData[0]);
                selectMemberCommand.Parameters.AddWithValue("@patr", memberData[2]);
                selectMemberCommand.Parameters.AddWithValue("@phone", phone.Content.ToString());
                selectMemberCommand.Connection = dbConnection;

                dbConnection.Open();
                OleDbDataReader dataMemberReader = selectMemberCommand.ExecuteReader();
                if (dataMemberReader.HasRows)
                {
                    while (dataMemberReader.Read())
                    {
                        id = Convert.ToInt32(dataMemberReader[0]);
                        //memberName = dataMemberReader[1].ToString();
                        //memberSurname = dataMemberReader[2].ToString();
                        //memberPatronymic = dataMemberReader[3].ToString();
                        //memberPhone = dataMemberReader[4].ToString();

                        //textBox3.Text = membName;
                        //textBox4.Text = membSurname;
                        //textBox5.Text = membPatr;
                        //maskedTextBox1.Text = membPhone;
                    }
                }
                dbConnection.Close();

                return id;
            }
            else
            {
                return 0;
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            string selectIdTypeQuery = "SELECT ID FROM Type WHERE TypeTitle = '" + label4.Content + "'";
            OleDbCommand selectIdTypeCommand = new OleDbCommand(selectIdTypeQuery, dbConnection);

            dbConnection.Open();

            OleDbDataReader dataTypeReader = selectIdTypeCommand.ExecuteReader();
            if (dataTypeReader.Read())
            {
                typeID = Convert.ToInt32(dataTypeReader[0]);
            }

            dbConnection.Close();

            string selectIdKindQuery = "SELECT ID FROM Kind WHERE KindTitle = '" + label5.Content + "'";
            OleDbCommand selectIdKindCommand = new OleDbCommand(selectIdKindQuery, dbConnection);

            dbConnection.Open();

            OleDbDataReader dataKindReader = selectIdKindCommand.ExecuteReader();
            if (dataKindReader.Read())
            {
                kindID = Convert.ToInt32(dataKindReader[0]);
            }

            dbConnection.Close();

            try
            {
                OleDbCommand taskCommand = new OleDbCommand();
                taskCommand.CommandType = CommandType.Text;
                taskCommand.CommandText = "INSERT INTO [Report] ([ID], [Title], [TypeID], [KindID], [Start], [End], [Description], " +
                    "[Member1], [Member2], [Member3], [Location]) " +
                    "VALUES(@id, @title, @type, @kind, @start, @end, @description, @memb1, @memb2, @memb3, @location)";
                taskCommand.Parameters.AddWithValue("@id", Event.oldId);
                taskCommand.Parameters.AddWithValue("@title", label3.Content);
                taskCommand.Parameters.AddWithValue("@type", typeID);
                taskCommand.Parameters.AddWithValue("@kind", kindID);
                taskCommand.Parameters.AddWithValue("@start", Convert.ToDateTime(label6.Content));
                taskCommand.Parameters.AddWithValue("@end", Convert.ToDateTime(label7.Content));
                taskCommand.Parameters.AddWithValue("@description", label8.Content);
                taskCommand.Parameters.AddWithValue("@memb1", GetMemberID(label13, label13_Copy));
                taskCommand.Parameters.AddWithValue("@memb2", GetMemberID(label13_Copy1, label13_Copy2));
                taskCommand.Parameters.AddWithValue("@memb3", GetMemberID(label13_Copy3, label13_Copy4));
                taskCommand.Parameters.AddWithValue("@location", label9.Content);
                taskCommand.Connection = dbConnection;

                //OleDbCommand selectNearDateCommand = new OleDbCommand();
                //selectNearDateCommand.CommandType = CommandType.Text;
                //selectNearDateCommand.CommandText = "SELECT TOP 1 * FROM Event WHERE [End] > @startDate ORDER BY [End] DESC";
                //selectNearDateCommand.Parameters.AddWithValue("@startDate", dateTimePicker.SelectedDate);
                //selectNearDateCommand.Connection = dbConnection;


                //OleDbCommand selectNearStartDateCommand = new OleDbCommand();
                //selectNearStartDateCommand.CommandType = CommandType.Text;
                //selectNearStartDateCommand.CommandText = "SELECT TOP 1 * FROM Event WHERE [Start] < @startDate ORDER BY [End] ASC";
                //selectNearStartDateCommand.Parameters.AddWithValue("@startDate", dateTimePicker1.SelectedDate);
                //selectNearStartDateCommand.Connection = dbConnection;

                //dbConnection.Open();

                //OleDbDataReader nearDateReader = selectNearDateCommand.ExecuteReader();
                //OleDbDataReader nearStartDateReader = selectNearStartDateCommand.ExecuteReader();

                //while (nearDateReader.Read() && nearStartDateReader.Read())
                //{
                //    nearStartDate = Convert.ToDateTime(nearStartDateReader[4]);
                //    nearEndDate = Convert.ToDateTime(nearDateReader[5]);
                //    //MessageBox.Show(nearDateReader[3].ToString());
                //}

                ////MessageBox.Show(nearEndDate + "\n" + nearStartDate);

                //dbConnection.Close();

                //if ((dateTimePicker.SelectedDate > nearStartDate && dateTimePicker1.SelectedDate < nearEndDate)/* || (dateTimePicker2.Value > nearStartDate && dateTimePicker2.Value < nearEndDate)*/)
                //{
                //    MessageBox.Show("Вы не можете добавить задачу, так как время уже занято.");
                //}
                //else
                //{
                dbConnection.Open();

                taskCommand.ExecuteNonQuery();

                MessageBox.Show("Отчет к задаче успешно создан!");

                dbConnection.Close();

                OleDbCommand updCommand = new OleDbCommand();
                updCommand.CommandType = CommandType.Text;
                updCommand.CommandText = "Update [Event] set [ReportID] = @repID " +
                    "where ID = @id and Title = @oldTitle ";
                updCommand.Parameters.AddWithValue("@repID", Event.oldId);
                updCommand.Parameters.AddWithValue("@id", Event.oldId);
                updCommand.Parameters.AddWithValue("@oldTitle", Event.oldTitle);
                updCommand.Connection = dbConnection;

                dbConnection.Open();

                updCommand.ExecuteNonQuery();

                //MessageBox.Show("Вы успешно обновили задачу!");

                dbConnection.Close();

                navService = NavigationService.GetNavigationService(this);
                navService.Navigate(new System.Uri("View/ShowEvents.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
