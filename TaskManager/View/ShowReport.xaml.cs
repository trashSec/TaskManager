using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;

namespace TaskManager.View
{
    /// <summary>
    /// Логика взаимодействия для ShowReport.xaml
    /// </summary>
    public partial class ShowReport : Page
    {
        string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\TaskManager.mdb; Persist Security Info=True;Jet OLEDB:Database Password = 2182315Dimas";
        int typeID, kindID;
        int member1, member2, member3;
        String memberName, memberSurname, memberPatronymic, memberPhone;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", System.AppDomain.CurrentDomain.BaseDirectory + Event.DayStart.ToString() + @"\" + Event.oldTitle.ToString());
        }

        public ShowReport()
        {
            InitializeComponent();
            FillFields();
        }

        private void FillFields()
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);
            OleDbCommand dbCommand = new OleDbCommand();
            dbCommand.CommandType = CommandType.Text;

            dbCommand.CommandText = "SELECT * FROM Report WHERE [ID] = @id and [Title] = @oldTitle";

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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Word.Application application = new Word.Application();
            application.Visible = true;
            Word.Document document = application.Documents.Open(System.AppDomain.CurrentDomain.BaseDirectory + @"\report.dotx");

            document.Bookmarks["Title"].Range.Text = label3.Content.ToString();
            document.Bookmarks["Type"].Range.Text = label4.Content.ToString();
            document.Bookmarks["Kind"].Range.Text = label5.Content.ToString();
            document.Bookmarks["Start"].Range.Text = label6.Content.ToString();
            document.Bookmarks["End"].Range.Text = label7.Content.ToString();
            document.Bookmarks["Desc"].Range.Text = label8.Content.ToString();
            document.Bookmarks["Loc"].Range.Text = label9.Content.ToString();
            document.Bookmarks["Memb1Name"].Range.Text = label13.Content.ToString();
            document.Bookmarks["Memb1Phone"].Range.Text = label13_Copy.Content.ToString();
            document.Bookmarks["Memb2Name"].Range.Text = label13_Copy1.Content.ToString();
            document.Bookmarks["Memb2Phone"].Range.Text = label13_Copy2.Content.ToString();
            document.Bookmarks["Memb3Name"].Range.Text = label13_Copy3.Content.ToString();
            document.Bookmarks["Memb3Phone"].Range.Text = label13_Copy4.Content.ToString();

            document.SaveAs2(FileName: System.AppDomain.CurrentDomain.BaseDirectory + Event.DayStart.ToString() + @"\" + Event.oldTitle.ToString() + @"\" + "Отчет.dotx");
            //document.Close();
            //application.Quit();

            MessageBox.Show("Ваш отчет успешно создан и находится в папке " + Event.DayStart.ToString() + @"\" + Event.oldTitle.ToString());
        }
    }
}
