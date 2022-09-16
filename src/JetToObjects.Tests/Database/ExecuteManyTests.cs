using System.Data.OleDb;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ekm.oledb.data.Tests.Database
{
    [TestFixture]
    public class ExecuteManyTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
        }

        [Test]
        public void ExecuteMany_WithSelectStatement_ReturnsRecordsInDatabase()
        {
            InsertProduct(1, true, "Product 1", 5.99);
            InsertProduct(2, false, "Product 2", 6.99);
            InsertProduct(3, true, "Product 3", 7.99); 

            var products = Db.Open(DB).ExecuteMany("select * from Products").ToArray();

            Assert.That(products.Length, Is.EqualTo(3));

            AssertProductValues(products[0], 1, true, "Product 1", 5.99);
            AssertProductValues(products[1], 2, false, "Product 2", 6.99);
            AssertProductValues(products[2], 3, true, "Product 3", 7.99);
        }

        [Test]
        public void ExecuteMany_WithSelectStatementAndNoResultsFound_ReturnsEmptyCollection()
        {
            var results = Db.Open(DB).ExecuteMany("select * from Products where id < 0").ToArray();

            Assert.That(results.Length, Is.EqualTo(0));
        }

        private static void InsertProduct(int categoryId, bool subCategory, string name, double price)
        {
            Db.Open(DB).ExecuteNonQuery("insert into Products (CategoryID, SubCategory, Name, Price) values (@CategoryID, @SubCategory, @Name, @Price)",
                new Param("@CategoryID", categoryId, OleDbType.Numeric),
                new Param("@SubCategory", subCategory, OleDbType.Boolean),
                new Param("@Name", name, OleDbType.LongVarWChar),
                new Param("@Price", price, OleDbType.Currency));
        }

        private static void AssertProductValues(dynamic product, int categoryId, bool subCategory, string name, double price)
        {
            Assert.That(product.CategoryID, Is.EqualTo(categoryId));
            Assert.That(product.SubCategory, Is.EqualTo(subCategory));
            Assert.That(product.Name, Is.EqualTo(name));
            Assert.That(product.Price, Is.EqualTo(price));
        }
    }
}