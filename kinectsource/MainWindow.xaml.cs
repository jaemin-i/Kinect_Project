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
using System.Configuration;
using MySql.Data.MySqlClient;

namespace kinectsource
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Page
    {
        MySqlCommand selectCommand;
        MySqlDataReader resultInfo;
        MySqlConnection conn;
        public MainWindow()
        {
            InitializeComponent();

        }
        public void openDB()
        {
            try
            {
                conn = new MySqlConnection("Server=192.168.219.101;Database=project;Uid=test;Pwd=test1234");
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("DB연결실패" + e.ToString());
            }
        }

        public bool dateUpdate(string id)
        {
            openDB();
            DateTime dt = new DateTime();
            string lastLoginTime = dt.ToString("yyyy-MM-dd");
            try
            {
                string sql = "UPDATE login SET last_login =\'" + lastLoginTime + "\' WHERE = id =\'" + id + "\'";
                selectCommand = new MySqlCommand(sql, conn);
                if (selectCommand.ExecuteNonQuery() != 1)
                {
                    conn.Close();

                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("DB업데이트 실패" + e.ToString());
                conn.Close();

            }
            conn.Close();
            return true;
        }
        public void startSelect(string id)
        {
            openDB();
            string sql = "SELECT name, phone_num, email FROM member WHERE id = \'" + id + "\'";
            try
            {
                selectCommand = new MySqlCommand(sql, conn);
                resultInfo = selectCommand.ExecuteReader();
                if (resultInfo.Read())
                {

                    string name = resultInfo["name"].ToString();
                    string phone = resultInfo["phone_num"].ToString();
                    string email = resultInfo["email"].ToString();

                    MessageBox.Show("로그인 성공!");
                    conn.Close();
                    cctv test = new cctv(name, phone, email); // 화면전환시 이름, 전화번호, 이메일을 CCTV로 보낸다.
                    NavigationService.Navigate(test);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("DB 정보 검색 실패" + e.ToString());
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)//로그인버튼 클릭했을때발생
        {
            openDB();
            if (tbxId.Text == "")//아이디 공백 확인
            {
                MessageBox.Show("아이디를 입력해주세요.");
                tbxId.Focus();
                return;
            }
            else if (tbxPs.Password == "")//비밀번호 공백 확인
            {
                MessageBox.Show("비밀번호를 입력해주세요.");
                tbxPs.Focus();
                return;
            }
            try
            {
                string loginid = tbxId.Text;
                string loginpw = tbxPs.Password;
                string sql = "select * from login where id = \'" + loginid + "\' and password = sha2(\'" + loginpw + "\',256)";

                selectCommand = new MySqlCommand(sql, conn);
                resultInfo = selectCommand.ExecuteReader();

                //if(tbxId.Text == "admin" && tbxPs.Password == "admin")//아이디비번 일치 확인(db연동부분)
                if (resultInfo.Read())
                {
                    conn.Close();
                    if (!dateUpdate(loginid))
                    {
                        return;
                    }
                    startSelect(loginid);
                }
                else
                {
                    MessageBox.Show("아이디와 비밀번호가 일치하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB 로그인 정보 조회 실패" + ex.ToString());
            }


        }

        private void TbxId_KeyDown(object sender, KeyEventArgs e)//아이디박스에서 엔터눌렀을경우발생
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void TbxPs_KeyDown(object sender, KeyEventArgs e)//패스워드박스에서 엔터눌렀을경우발생
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }
    }
}