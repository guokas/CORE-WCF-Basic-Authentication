namespace MyFirstService.Dto
{
    public class User
    {
        public User() { }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string country { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string localeID { get; set; }
    }
}
