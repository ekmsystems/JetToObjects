namespace JetToObjects.Database
{
    public static class Db
    {
        public static DatabaseContext Open(string connectionString)
        {
            return new DatabaseContext(connectionString, null);
        }

        public static DatabaseContext Open(string connectionString, string password)
        {
            return new DatabaseContext(connectionString, password);
        }

        public static int MaxConnectionAttempts
        {
            get; set;
        }

        static Db()
        {
            MaxConnectionAttempts = 5;
        }
    }
}
