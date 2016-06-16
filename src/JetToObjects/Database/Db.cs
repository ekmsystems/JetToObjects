namespace JetToObjects.Database
{
    public static class Db
    {
        #region Docs
        /// <summary>
        /// Creates a database context for the specified database
        /// </summary>
        /// <param name="connectionString">OleDb connection string</param>
        /// <returns>The database context for the specified database</returns>
        #endregion
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
