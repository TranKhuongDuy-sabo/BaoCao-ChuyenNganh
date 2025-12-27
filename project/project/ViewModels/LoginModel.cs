using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace project.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập!")]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
