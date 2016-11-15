using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using JetToObjects.Helpers;
using JetToObjects.Models;

namespace JetToObjects.Database
{
    public class DatabaseContext
    {
        private readonly string _connectionString;
        private readonly string _password;

        private bool _returnIdentity;

        public DatabaseContext(string connectionString, string password)
        {
            _connectionString = connectionString;
            _password = password;
        }

        public DatabaseContext ReturnIdentity()
        {
            _returnIdentity = true;
            return this;
        }

        public IEnumerable<dynamic> ExecuteMany(string query, params Param[] parameters)
        {
            using (var connection = GetConnection())
            {
                TryOpenConnection(connection);

                foreach (var result in BuildAndExecuteMany(query, parameters, connection))
                    yield return result;
            }
        }

        public IEnumerable<dynamic> ExecuteManyWithConnection(string query, OleDbConnection connection, Param[] parameters = null)
        {
            TryOpenConnection(connection);

            return BuildAndExecuteMany(query, parameters, connection);
        }
        
        #region Docs
        /// <summary>
        /// Executes the sql query and returns a single result
        /// </summary>
        /// <param name="query">The sql query string</param>
        /// <param name="parameters">The parameters for the sql query</param>
        /// <returns>The single result of the query</returns>
        #endregion
        public dynamic ExecuteSingle(string query, params Param[] parameters)
        {
            using (var connection = GetConnection())
            {
                TryOpenConnection(connection);
                return BuildAndExecuteSingle(query, parameters, connection);
            }
        }

        public dynamic ExecuteSingleWithConnection(string query, OleDbConnection connection, Param[] parameters = null)
        {
            TryOpenConnection(connection);
            return BuildAndExecuteSingle(query, parameters, connection);
        }

        public NonQueryResult ExecuteNonQuery(string query, params Param[] parameters)
        {
            using (var connection = GetConnection())
            {
                TryOpenConnection(connection);
                return BuildAndExecuteNonQuery(query, parameters, connection);
            }
        }

        public NonQueryResult ExecuteNonQueryWithConnection(string query, OleDbConnection connection, Param[] parameters = null)
        {
            TryOpenConnection(connection);
            return BuildAndExecuteNonQuery(query, parameters, connection);
        }

        public object ExecuteScalar(string query, params Param[] parameters)
        {
            using (var connection = GetConnection())
            {
                TryOpenConnection(connection);
                return BuildAndExecuteScalar(query, parameters, connection);
            }
        }

        public object ExecuteScalarWithConnection(string query, OleDbConnection connection, Param[] parameters = null)
        {
            TryOpenConnection(connection);
            return BuildAndExecuteScalar(query, parameters, connection);
        }

        public Dictionary<int, dynamic> ExecuteMultipleQueries(IEnumerable<MultipleQuery> queries)
        {
            var multipleQueryResults = new Dictionary<int, dynamic>();
            using (var connection = GetConnection())
            {
                TryOpenConnection(connection);

                foreach (var query in queries)
                {
                    if (string.IsNullOrEmpty(query.Query))
                        throw new MissingFieldException("The query field is required.");
                    if (query.Id == 0)
                        throw new MissingFieldException(
                            "The id field is required to assign results in the MultipleQueryResult object.");
                    // Parameters are only required when there are parameters in the query string.
                    if (query.Query.Contains("@"))
                    {
                        if (query.Parameters == null || query.Parameters.Length == 0)
                            throw new MissingFieldException("The parameter field is required.");
                        if (query.Parameters.ToList().Contains(null))
                            throw new MissingFieldException("Parameters must not be null.");
                    }
                    if (query.QueryType == null)
                        throw new MissingFieldException(
                            "The QueryType field is required so the function knows which query type to perform.");
                    if (multipleQueryResults.ContainsKey(query.Id))
                        throw new ArgumentException(
                            "The key supplied already exists in the dictionary; you can not have duplicate keys. Key: " +
                            query.Id);

                    switch (query.QueryType)
                    {
                        case (QueryType.ExecuteSingle):
                            multipleQueryResults.Add(query.Id,
                                                        BuildAndExecuteSingle(query.Query, query.Parameters, connection));
                            break;
                        case (QueryType.ExecuteMany):
                            multipleQueryResults.Add(query.Id,
                                                        BuildAndExecuteMany(query.Query, query.Parameters, connection).ToList());
                            break;
                        case (QueryType.ExecuteNonQuery):
                            multipleQueryResults.Add(query.Id,
                                                        BuildAndExecuteNonQuery(query.Query, query.Parameters,
                                                                                connection));
                            break;
                        case (QueryType.ExecuteScalar):
                            multipleQueryResults.Add(query.Id,
                                                        BuildAndExecuteScalar(query.Query, query.Parameters, connection));
                            break;
                        default:
                            throw new MissingMemberException(
                                "This query type does not exist or is not currently supported by this function.");
                    }
                }
            }
            return multipleQueryResults;
        }

        public Dictionary<int, dynamic> ExecuteMultipleQueriesWithConnection(IEnumerable<MultipleQuery> queries, OleDbConnection connection)
        {
            var multipleQueryResults = new Dictionary<int, dynamic>();
            TryOpenConnection(connection);

            foreach (var query in queries)
            {
                if (string.IsNullOrEmpty(query.Query))
                    throw new MissingFieldException("The query field is required.");
                if (query.Id == 0)
                    throw new MissingFieldException(
                        "The id field is required to assign results in the MultipleQueryResult object.");
                // Parameters are only required when there are parameters in the query string.
                if (query.Query.Contains("@"))
                {
                    if (query.Parameters == null || query.Parameters.Length == 0)
                        throw new MissingFieldException("The parameter field is required.");
                    if (query.Parameters.ToList().Contains(null))
                        throw new MissingFieldException("Parameters must not be null.");
                }
                if (query.QueryType == null)
                    throw new MissingFieldException(
                        "The QueryType field is required so the function knows which query type to perform.");
                if (multipleQueryResults.ContainsKey(query.Id))
                    throw new ArgumentException(
                        "The key supplied already exists in the dictionary; you can not have duplicate keys. Key: " +
                        query.Id);

                switch (query.QueryType)
                {
                    case (QueryType.ExecuteSingle):
                        multipleQueryResults.Add(query.Id, BuildAndExecuteSingle(query.Query, query.Parameters, connection));
                        break;
                    case (QueryType.ExecuteMany):
                        multipleQueryResults.Add(query.Id, BuildAndExecuteMany(query.Query, query.Parameters, connection));
                        break;
                    case (QueryType.ExecuteNonQuery):
                        multipleQueryResults.Add(query.Id, BuildAndExecuteNonQuery(query.Query, query.Parameters, connection));
                        break;
                    case (QueryType.ExecuteScalar):
                        multipleQueryResults.Add(query.Id, BuildAndExecuteScalar(query.Query, query.Parameters, connection));
                        break;
                    default:
                        throw new MissingMemberException(
                            "This query type does not exist or is not currently supported by this function.");
                }

            }
            
            return multipleQueryResults;
        }

        public bool CompactRepair(string newDbPath)
        {
            try
            {
                object objJRO = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));
                object[] oParams = { ConnectionStrings.OleDb(_connectionString, _password), ConnectionStrings.OleDb(newDbPath, _password) + ";" };
                objJRO.GetType().InvokeMember("CompactDatabase",
                                              System.Reflection.BindingFlags.InvokeMethod,
                                              null,
                                              objJRO,
                                              oParams);
                File.Delete(_connectionString);
                File.Move(newDbPath, _connectionString);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objJRO);
                objJRO = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        private static dynamic BuildAndExecuteSingle(string query, Param[] parameters, OleDbConnection connection)
        {
            dynamic result = null;

            using (var command = BuildCommand(query, parameters, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        try
                        {
                            result = BuildDynamicResult(reader);
                        }
                        catch
                        {
                        }
                    }
                    return result; 
                }
            }
        }

        private static IEnumerable<dynamic> BuildAndExecuteMany(string query, Param[] parameters, OleDbConnection connection)
        {
            using (var command = BuildCommand(query, parameters, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader == null)
                        yield break;

                    while (reader.Read())
                    {
                        yield return BuildDynamicResult(reader);
                    }
                }
            }
        }

        private NonQueryResult BuildAndExecuteNonQuery(string query, Param[] parameters, OleDbConnection connection)
        {
            using (var command = BuildCommand(query, parameters, connection))
            {
                var result = new NonQueryResult { RowsAffected = command.ExecuteNonQuery() };
                if (_returnIdentity)
                {
                    command.CommandText = "select @@identity";
                    result.Identity = Convert.ToInt64(command.ExecuteScalar());
                }

                return result;    
            }
        }

        private static object BuildAndExecuteScalar(string query, Param[] parameters, OleDbConnection connection)
        {
            using (var command = BuildCommand(query, parameters, connection))
            {
                return command.ExecuteScalar();
            }
        }

        private OleDbConnection GetConnection()
        {
            return new OleDbConnection(ConnectionStrings.OleDb(_connectionString, _password));
        }

        private static void TryOpenConnection(OleDbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
                return;

            for (var attempts = 1; attempts <= Db.MaxConnectionAttempts; attempts++)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (Exception e)
                {
                    if (attempts == Db.MaxConnectionAttempts)
                    {
                        throw new Exception(string.Format("Unable to open database connection after {0} attempts", attempts), new Exception(connection.DataSource, e));
                    }

                    Thread.Sleep(attempts * 1000);
                }
            }
        }

        private static OleDbCommand BuildCommand(string query, Param[] parameters, OleDbConnection connection)
        {
            var command = new OleDbCommand(query, connection);

            if(parameters != null && parameters.Any())
            {
                var pattern = string.Format("({0})", string.Join(@"\b|", parameters.Select(x => x.Name)));
                var queryParameters = Regex.Matches(query, pattern);

                foreach (Match queryParameter in queryParameters)
                {
                    var parameter = parameters.SingleOrDefault(x => x.Name == queryParameter.Value);

                    if (parameter == null)
                        continue;

                    var p = command.CreateParameter();

                    p.ParameterName = parameter.Name;
                    p.OleDbType = parameter.Type;
                    p.Value = parameter.Value;

                    command.Parameters.Add(p);
                }
            }
            
            return command;
        }
		
        private static dynamic BuildDynamicResult(DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return null;
            }

            dynamic result = new ExpandoObject();
            var resultDictionary = (IDictionary<string, object>) result;

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var type = reader.GetFieldType(i);

                try
                {
                    resultDictionary.Add(name, Convert.ChangeType(reader.GetValue(i), type));
                }
                catch
                {
                    resultDictionary.Add(name, GetDefaultValue(type));
                }
            }

            return result;
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}