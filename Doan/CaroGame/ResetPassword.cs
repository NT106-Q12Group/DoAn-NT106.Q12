using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class ResetPassword : Form
    {
        private readonly TCPClient _client;
        private readonly PlayerView _player;
        private string _currentOtp = string.Empty;
        public ResetPassword(TCPClient client)
        {
            InitializeComponent();
            _client = client;
 

        }

        public static string GenerateOtp()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }

        public static bool SendOtp(string toEmail, string otp, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                string fromEmail = "xuanninaa123@gmail.com";
                string appPassword = "ktctntyubgmyndgc";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = "OTP Reset Password";
                mail.Body = $"Mã OTP của bạn là: {otp}. Vui lòng không chia sẻ mã này.";
                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromEmail, appPassword);

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }


        private void sendOtp_btn_Click(object sender, EventArgs e)
        {
            string username = username_tb.Text.Trim();
            if (username == "")
            {
                MessageBox.Show("Vui lòng nhập Username.", "Cảnh báo");
                return;
            }

            string response = _client.GetEmail(username);
            var p = response.Split('|');
            if (p[0] != "OK")
            {
                MessageBox.Show($"Không tìm thấy email hoặc lỗi kết nối: {p[1]}", "Lỗi");
                return;
            }

            string realEmail = p[1];   

            _currentOtp = GenerateOtp(); 

            string err;
            bool ok = SendOtp(realEmail, _currentOtp, out err);

            if (ok)
            {
            
                MessageBox.Show("Mã OTP đã được gửi đến email đăng ký. Vui lòng kiểm tra hộp thư.", "Thành công");


                OTP_tb.Enabled = true;
                confirmOtp_btn.Enabled = true;
                sendOtp_btn.Enabled = false; 
            }
            else
            {
                MessageBox.Show("Gửi OTP thất bại! Lỗi:\n" + err, "Lỗi Gửi Email");
            }
        }

        private void confirmOtp_btn_Click(object sender, EventArgs e)
        {
            string enteredOtp = OTP_tb.Text.Trim();

            if (string.IsNullOrEmpty(_currentOtp))
            {
                MessageBox.Show("Vui lòng gửi mã OTP trước.", "Lỗi");
                return;
            }

            if (enteredOtp == _currentOtp)
            {
                MessageBox.Show("Xác nhận OTP thành công! Vui lòng đặt mật khẩu mới.", "Thành công");

               
                newPassword_tb.Enabled = true;
                confirmPassword_tb.Enabled = true;
                save_btn.Enabled = true;

                confirmOtp_btn.Enabled = false;
                OTP_tb.Enabled = false;
            }
            else
            {
                MessageBox.Show("Mã OTP không chính xác. Vui lòng kiểm tra lại.", "Lỗi");
            }
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            string username = username_tb.Text.Trim();
            string newPass = newPassword_tb.Text;
            string confirmPass = confirmPassword_tb.Text;

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không khớp!", "Lỗi");
                return;
            }

            if (newPass.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi");
                return;
            }

            string response = _client.UpdatePassword(username, newPass);
            var p = response.Split('|');

            if (p[0] == "OK")
            {
                MessageBox.Show("Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay.", "Thành công");
                this.Close(); 
            }
            else
            {
                MessageBox.Show($"Lỗi khi cập nhật mật khẩu: {p[1]}", "Lỗi");
            }
        }
    }
}
