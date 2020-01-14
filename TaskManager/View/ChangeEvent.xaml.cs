using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для ChangeEvent.xaml
    /// </summary>
    public partial class ChangeEvent : Page
    {
        NavigationService navService;
        string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\TaskManager.mdb";
        int typeID, kindID;
        int member1, member2, member3;
        String memberName, memberSurname, memberPatronymic, memberPhone;
        public ChangeEvent()
        {
            InitializeComponent();
            FillTypeComboBox();
            FillKindComboBox();
            FillFields();
            FillMemberQuery();
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
                    comboBox_Copy.Items.Add(dataReader.GetString(1));
                }
            }
            dbConnection.Close();
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
                textBox.Text = dataAllReader[1].ToString();
                comboBox.SelectedIndex = Convert.ToInt32(dataAllReader[2]) - 1;
                comboBox_Copy.SelectedIndex = Convert.ToInt32(dataAllReader[3]) - 1;
                dateTimePicker.Value = Convert.ToDateTime(dataAllReader[4]);
                dateTimePicker1.Value = Convert.ToDateTime(dataAllReader[5]);
                textBox1.Text = dataAllReader[6].ToString();
                member1 = Convert.ToInt32(dataAllReader[7]);
                member2 = Convert.ToInt32(dataAllReader[8]);
                member3 = Convert.ToInt32(dataAllReader[9]);
                textBox_Copy.Text = dataAllReader[10].ToString();
            }
            dbConnection.Close();

            if(member1 != 0)
            {
                SetMemberByID(member1, label12);
            }
            if (member2 != 0)
            {
                SetMemberByID(member2, label13);
            }
            if (member3 != 0)
            {
                SetMemberByID(member3, label14);
            }                      
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            string selectIdTypeQuery = "SELECT ID FROM Type WHERE TypeTitle = '" + comboBox.Text + "'";
            OleDbCommand selectIdTypeCommand = new OleDbCommand(selectIdTypeQuery, dbConnection);

            dbConnection.Open();

            OleDbDataReader dataTypeReader = selectIdTypeCommand.ExecuteReader();
            if (dataTypeReader.Read())
            {
                typeID = Convert.ToInt32(dataTypeReader[0]);
            }

            dbConnection.Close();

            string selectIdKindQuery = "SELECT ID FROM Kind WHERE KindTitle = '" + comboBox_Copy.Text + "'";
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
                taskCommand.CommandText = "Update [Event] set [Title] = @title, [TypeID] = @type, [KindID] = @kind, [Start] = @start, [End] = @end, " +
                    "[Description] = @description, [Member1] = @memb1, [Member2] = @memb2, [Member3] = @memb3, [Location] = @location " +
                    "where ID = @id and Title = @oldTitle ";
                taskCommand.Parameters.AddWithValue("@title", textBox.Text);
                taskCommand.Parameters.AddWithValue("@type", typeID);
                taskCommand.Parameters.AddWithValue("@kind", kindID);
                taskCommand.Parameters.AddWithValue("@start", dateTimePicker.Value);
                taskCommand.Parameters.AddWithValue("@end", dateTimePicker1.Value);
                taskCommand.Parameters.AddWithValue("@description", textBox1.Text);
                taskCommand.Parameters.AddWithValue("@memb1", GetMemberID(label12));
                taskCommand.Parameters.AddWithValue("@memb2", GetMemberID(label13));
                taskCommand.Parameters.AddWithValue("@memb3", GetMemberID(label14));
                taskCommand.Parameters.AddWithValue("@location", textBox_Copy.Text);
                taskCommand.Parameters.AddWithValue("@id", Event.oldId);
                taskCommand.Parameters.AddWithValue("@oldTitle", Event.oldTitle);
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

                MessageBox.Show("Вы успешно обновили задачу!");

                dbConnection.Close();

                navService = NavigationService.GetNavigationService(this);
                navService.Navigate(new System.Uri("View/ShowEvents.xaml", UriKind.RelativeOrAbsolute));
                //}
            }
            catch (Exception erx)
            {
                MessageBox.Show(erx.ToString());
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            label12.Content = null;
            FillMemberQuery();
        }

        private void button3_Copy_Click(object sender, RoutedEventArgs e)
        {
            label13.Content = null;
            FillMemberQuery();
        }

        private void button3_Copy1_Click(object sender, RoutedEventArgs e)
        {
            label14.Content = null;
            FillMemberQuery();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            if (!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox2_Copy.Text) && !string.IsNullOrEmpty(maskedTextBox.Text))
            {
                OleDbCommand memberCommand = new OleDbCommand();
                memberCommand.CommandType = CommandType.Text;
                memberCommand.CommandText = "INSERT INTO [Member] ([FirstName], [SurName], [Patronymic], [PhoneNumber]) " +
                    "VALUES(@firstname, @surname, @patronymic, @phonenumber)";
                memberCommand.Parameters.AddWithValue("@firstname", textBox2_Copy.Text);
                memberCommand.Parameters.AddWithValue("@surname", textBox2.Text);
                memberCommand.Parameters.AddWithValue("@patronymic", textBox2_Copy1.Text);
                memberCommand.Parameters.AddWithValue("@phonenumber", maskedTextBox.Text);
                memberCommand.Connection = dbConnection;

                string selectIdQuery = "SELECT MAX(ID) FROM Member";
                OleDbCommand selectIdCommand = new OleDbCommand(selectIdQuery, dbConnection);

                dbConnection.Open();

                memberCommand.ExecuteNonQuery();

                //OleDbDataReader dataReader = selectIdCommand.ExecuteReader();
                //if (dataReader.Read())
                //{
                //    memberID = Convert.ToInt32(dataReader[0]);
                //}

                MessageBox.Show("Новый участник успешно добавлен!");

                dbConnection.Close();

                ChooseMember(textBox2.Text, textBox2_Copy.Text, textBox2_Copy1.Text, maskedTextBox.Text);

                FillMemberQuery();

                textBox2.Clear();
                textBox2_Copy.Clear();
                textBox2_Copy1.Clear();
                maskedTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Вы не заполнили основные поля");
            }
        }

        private void FillMemberQuery()
        {
            comboBox1.Items.Clear();

            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            string fillMemberQuery = "SELECT * FROM Member";
            OleDbCommand fillMemberCommand = new OleDbCommand(fillMemberQuery, dbConnection);

            dbConnection.Open();
            OleDbDataReader dataFillMemberReader = fillMemberCommand.ExecuteReader();
            if (dataFillMemberReader.HasRows)
            {
                while (dataFillMemberReader.Read())
                {
                    memberName = dataFillMemberReader.GetString(1);
                    memberSurname = dataFillMemberReader.GetString(2);

                    //typeName = dataReader[1].ToString();
                    comboBox1.Items.Add(memberSurname + " " + memberName);
                }
            }
            dbConnection.Close();
        }

        private void ChooseMember(string surname, string name, string patronymic, string phone)
        {
            if (String.IsNullOrEmpty((string)label12.Content))
            {
                label12.Content = surname + " " + name + " " + patronymic + " " + phone;
            }
            else if (!String.IsNullOrEmpty((string)label12.Content) && String.IsNullOrEmpty((string)label13.Content))
            {
                label13.Content = surname + " " + name + " " + patronymic + " " + phone;
            }
            else if (!String.IsNullOrEmpty((string)label12.Content) && !String.IsNullOrEmpty((string)label13.Content) && String.IsNullOrEmpty((string)label14.Content))
            {
                label14.Content = surname + " " + name + " " + patronymic + " " + phone;
            }
            else
            {
                MessageBox.Show("Можно добавить до 3-ёх участников к задаче");
            }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.HasItems)
            {
                string[] memberData = comboBox1.SelectedItem.ToString().Split(' ');

                OleDbConnection dbConnection = new OleDbConnection(connectionString);

                OleDbCommand selectMemberCommand = new OleDbCommand();
                selectMemberCommand.CommandType = CommandType.Text;
                selectMemberCommand.CommandText = "SELECT * FROM Member where [FirstName] = @name AND [SurName] = @surname";
                selectMemberCommand.Parameters.AddWithValue("@name", memberData[1]);
                selectMemberCommand.Parameters.AddWithValue("@surname", memberData[0]);
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

                ChooseMember(memberSurname, memberName, memberPatronymic, memberPhone);
            }
        }

        private void SetMemberByID(int memberID, Label label)
        {
            if (String.IsNullOrEmpty((string)label.Content))
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

                label.Content = memberSurname + " " + memberName + " " + memberPatronymic + " " + memberPhone;
            }
        }

        private int GetMemberID(Label label)
        {
            if (!String.IsNullOrEmpty((string)label.Content))
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
                selectMemberCommand.Parameters.AddWithValue("@phone", memberData[3]);
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
            textBox1.Clear();
            textBox_Copy.Clear();
        }
    }
}
