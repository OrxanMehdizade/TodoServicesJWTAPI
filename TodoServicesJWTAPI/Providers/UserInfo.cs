namespace TodoServicesJWTAPI.Providers
{
    public class UserInfo
    {


        public string id { get; set; }
        public string Username { get; set; }
        public UserInfo(string id, string username)
        {
            this.id = id;
            Username = username;
        }
    }
}
