using System.Data.OleDb;
using System.IO;
using JetToObjects.Database;
using NUnit.Framework;

namespace JetToObjects.Tests.Database
{
    [TestFixture]
    public class ReturnIdentityTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
        }

        [Test]
        public void ReturnIdentity_WithInsertStatement_ReturnsIncrementingIdentity()
        {
            var id1 = InsertProduct(101, false, "Test Product 1", 100);
            var id2 = InsertProduct(102, true, "Test Product 2", 200);
            var id3 = InsertProduct(103, false, "Test Product 3", 300);

            Assert.That(id3 > id2);
            Assert.That(id2 > id1);
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