using ekm.oledb.data;

namespace ekm.oledb.data
{
    public enum QueryType
    {
        ExecuteSingle,
        ExecuteMany,
        ExecuteNonQuery,
        ExecuteScalar
    }

    public class MultipleQuery
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public Param[] Parameters { get; set; }
        public QueryType? QueryType { get; set; }
    }
}