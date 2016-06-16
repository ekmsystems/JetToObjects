using System.Data.OleDb;
using System.IO;
using JetToObjects.Database;
using NUnit.Framework;

namespace JetToObjects.Tests.Database
{
    [TestFixture]
    public class ExecuteScalarTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
        }

        [Test]
        public void ExecuteScalar_WithSelectStatement_ReturnsScalarValueFromRowInDatabase()
        {
            var id = InsertProduct(100, false, "Test Product", 200);

            var name = Db.Open(DB).ExecuteScalar("select Name from Products where id = @id",
                new Param("@id", id, OleDbType.Numeric));

            var price = Db.Open(DB).ExecuteScalar("select price from Products where id = @id",
                new Param("@id", id, OleDbType.Numeric));

            Assert.That(name, Is.EqualTo("Test Product"));
            Assert.That(price, Is.EqualTo(200));
        }

        private static long InsertProduct(int categoryId, bool subCategory, string name, double price)
        {
            var result = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", categoryId, OleDbType.Numeric),
                new Param("@SubCategory", subCategory, OleDbType.Boolean),
                new Param("@Name", name, OleDbType.LongVarWChar),
                new Param("@Price", price, OleDbType.Currency));

            return result.Identity;
        }
    }
}