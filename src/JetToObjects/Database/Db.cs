namespace JetToObjects.Database
{
    public static class Db
    {
        static Db()
        {
            MaxConnectionAttempts = 5;
        }

        public static int MaxConnectionAttempts { get; set; }

        public static DatabaseContext Open(string connectionString)
        {
            return new DatabaseContext(connectionString, null);
        }

        public static DatabaseContext Open(string connectionString, string password)
        {
            return new DatabaseContext(connectionString, password);
        }
    }
}