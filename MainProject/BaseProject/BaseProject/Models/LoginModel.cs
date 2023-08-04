namespace BaseProject.Models
{
    public class LoginModel
    {
        public string Id { get; set; }
        public string Password { get; set; }

        public List<LoginLogModel> LoginLogModels { get; } = new List<LoginLogModel>();
    }
}
