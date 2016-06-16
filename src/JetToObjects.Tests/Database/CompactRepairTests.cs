using System.Data.OleDb;
using System.IO;
using JetToObjects.Database;
using NUnit.Framework;

namespace JetToObjects.Tests.Database
{
    [TestFixture]
    public class CompactRepairTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
        }

        [Test]
        public void CompactRepair_returns_True()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 42, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@Price", 19.99, OleDbType.Currency));
            Assert.That(insertResult.RowsAffected, Is.EqualTo(1));
            insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 10, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@Price", 10.99, OleDbType.Currency));
            Assert.That(insertResult.RowsAffected, Is.EqualTo(1));
            insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 8, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "Product 1", OleDbType.LongVarWChar),
                new Param("@Price", 10.99, OleDbType.Currency));
            Assert.That(insertResult.RowsAffected, Is.EqualTo(1));

            var size = new FileInfo(DB).Length;

            var repair = Db.Open(DB).CompactRepair("repairTest.mdb");
            Assert.True(repair);
            var newSize = new FileInfo(DB).Length;
            Assert.That(newSize, Is.LessThan(size));
        }
    }
}
