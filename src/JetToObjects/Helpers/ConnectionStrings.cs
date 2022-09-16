using System;
using System.Data.OleDb;

namespace ekm.oledb.data.Helpers
{
    public static class ConnectionStrings
    {
        public static string OleDb(string connectionString, string password)
        {
            var provider = Environment.Is64BitProcess
                ? "Microsoft.ACE.OLEDB.12.0"
                : "Microsoft.Jet.OLEDB.4.0";
            var builder = new OleDbConnectionStringBuilder(string.Format("Provider={0};", provider))
            {
                DataSource = connectionString
            };

            if (password != null)
                builder.Add("Jet OLEDB:Database Password", password);

            return builder.ConnectionString;
        }
    }
}