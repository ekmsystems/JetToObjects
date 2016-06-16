using JetToObjects.Database;

namespace JetToObjects.Models
{
    public enum QueryType
    {
        ExecuteSingle, ExecuteMany, ExecuteNonQuery, ExecuteScalar
    }

    public class MultipleQuery
    {
        /// <summary>
        /// The Id field helps to track which query result belongs to which query passed in. 
        /// Id's must be > 0. E.g 1,2,3,4 etc
        /// Throws MissingFieldException.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// This field is where you pass in your SQL Query. This field is required. 
        /// Throws MissingFieldException.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// The parameters required by the query field. This field is only required if your SQL Query contains parameters. 
        /// Throws MissingFieldException.
        /// </summary>
        public Param[] Parameters { get; set; }
        /// <summary>
        /// The query type for the function to execute. This field is required. 
        /// Throws MissingFieldException.
        /// </summary>
        public QueryType? QueryType { get; set; }
    }
}
