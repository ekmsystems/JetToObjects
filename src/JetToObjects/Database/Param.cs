using System.Data.OleDb;

namespace JetToObjects.Database
{
    public class Param
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        public OleDbType Type { get; private set; }

        public Param(string name, object value, OleDbType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}