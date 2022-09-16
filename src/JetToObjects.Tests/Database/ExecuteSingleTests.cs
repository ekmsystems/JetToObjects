using System.Data.OleDb;
using System.IO;
using NUnit.Framework;

namespace ekm.oledb.data.Tests.Database
{
    [TestFixture]
    public class ExecuteSingleTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
        }

        [Test]
        public void ExecuteSingle_WithSelectStatement_ReturnsRecordInDatabase()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 8, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "Product 1", OleDbType.LongVarWChar),
                new Param("@Price", 10.99, OleDbType.Currency));

            var product = Db.Open(DB).ExecuteSingle("select CategoryID, SubCategory, Name, Price from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(product.CategoryID, Is.EqualTo(8));
            Assert.That(product.SubCategory, Is.True);
            Assert.That(product.Name, Is.EqualTo("Product 1"));
            Assert.That(product.Price, Is.EqualTo(10.99));
        }

        [Test]
        public void ExecuteSingle_WithSelectStatementAndNoResultsFound_ReturnsNull()
        {
            var product = Db.Open(DB).ExecuteSingle("select * from Products where id = 0");

            Assert.That(product == null);
        }
    }
}