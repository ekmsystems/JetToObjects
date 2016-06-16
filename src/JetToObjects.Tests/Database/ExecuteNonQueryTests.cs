using System.Data.OleDb;
using System.IO;
using JetToObjects.Database;
using NUnit.Framework;

namespace JetToObjects.Tests.Database
{
    [TestFixture]
    public class ExecuteNonQueryTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
            Db.Open(DB).ExecuteNonQuery("alter table Products alter column [ID] counter(1,1)");
        }

        [Test]
        public void ExecuteNonQuery_WithInsertStatement_CreatesRecordInDatabase()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 42, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@Price", 19.99, OleDbType.Currency));

            Assert.That(insertResult.RowsAffected, Is.EqualTo(1));
            Assert.That(insertResult.Identity, Is.EqualTo(1));

            var product = Db.Open(DB).ExecuteSingle("select CategoryID, SubCategory, Name, Price from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(product.CategoryID, Is.EqualTo(42));
            Assert.That(product.SubCategory, Is.True);
            Assert.That(product.Name, Is.EqualTo("New Product"));
            Assert.That(product.Price, Is.EqualTo(19.99));
        }

        [Test]
        public void ExecuteNonQuery_WithParametersNotInOrder_CreatesRecordInDatabase()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@Price", 19.99, OleDbType.Currency),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@CategoryID", 42, OleDbType.Numeric));

            Assert.That(insertResult.RowsAffected, Is.EqualTo(1));
            Assert.That(insertResult.Identity, Is.EqualTo(1));

            var product = Db.Open(DB).ExecuteSingle("select CategoryID, SubCategory, Name, Price from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(product.CategoryID, Is.EqualTo(42));
            Assert.That(product.SubCategory, Is.True);
            Assert.That(product.Name, Is.EqualTo("New Product"));
            Assert.That(product.Price, Is.EqualTo(19.99));
        }

        [Test]
        public void ExecuteNonQuery_WithUpdateStatement_UpdatesRecordInDatabase()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 10, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@Price", 10.99, OleDbType.Currency));

            var updateResult = Db.Open(DB).ExecuteNonQuery("update Products set CategoryID = @CategoryID, Name = @Name where id = @id",
                new Param("@CategoryID", 12, OleDbType.Numeric),
                new Param("@Name", "Updated Product", OleDbType.VarWChar),
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            var category = Db.Open(DB).ExecuteSingle("select CategoryID, SubCategory, Name, Price from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(category.SubCategory, Is.True);
            Assert.That(category.CategoryID, Is.EqualTo(12));
            Assert.That(category.Name, Is.EqualTo("Updated Product"));
            Assert.That(category.Price, Is.EqualTo(10.99));
        }

        [Test]
        public void ExecuteNonQuery_WithUpdateStatement_WithSimilarParameterNames_UpdatesRecordInDatabase()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 10, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@Price", 10.99, OleDbType.Currency));

            var updateResult = Db.Open(DB).ExecuteNonQuery("update Products set CategoryID = @CategoryID, Name = @CategoryName, Price = @CategoryPrice where id = @id",
                new Param("@CategoryID", 12, OleDbType.Numeric),
                new Param("@CategoryName", "Updated Product", OleDbType.VarWChar),
                new Param("@CategoryPrice", 12.99, OleDbType.Currency),
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            var category = Db.Open(DB).ExecuteSingle("select CategoryID, SubCategory, Name, Price from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(category.SubCategory, Is.True);
            Assert.That(category.CategoryID, Is.EqualTo(12));
            Assert.That(category.Name, Is.EqualTo("Updated Product"));
            Assert.That(category.Price, Is.EqualTo(12.99));
        }
		
        [Test]
        public void ExecuteNonQuery_WithDeleteStatement_DeletesRecordInDatabase()
        {
            var insertResult = Db.Open(DB).ReturnIdentity().ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", 10, OleDbType.Numeric),
                new Param("@SubCategory", true, OleDbType.Boolean),
                new Param("@Name", "New Product", OleDbType.LongVarWChar),
                new Param("@Price", 10.99, OleDbType.Currency));

            var deleteResult = Db.Open(DB).ExecuteNonQuery("delete Products.* from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(deleteResult.RowsAffected, Is.EqualTo(1));

            var selectResult = Db.Open(DB).ExecuteSingle("select ID from Products where id = @id",
                new Param("@id", insertResult.Identity, OleDbType.Numeric));

            Assert.That(selectResult == null);
        }
    }
}