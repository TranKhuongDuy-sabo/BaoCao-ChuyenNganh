namespace project.Models
{
    public class ContactInfo
    {
        public string Address { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string MapLink { get; set; } = ""; // Thêm cái link Google Map nếu thích
    }
}