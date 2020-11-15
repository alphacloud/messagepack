namespace _build
{
    public class Credentials {
        public string UserName { get; }
        public string Password { get; }

        public Credentials(string userName, string password) {
            UserName = userName;
            Password = password;
        }
    }
}