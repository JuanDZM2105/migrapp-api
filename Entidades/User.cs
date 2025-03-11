namespace migrapp_api.Entidades
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public required string email { get; set; }
        public required string phone { get; set; }
        public required string password { get; set; }

    }
}
