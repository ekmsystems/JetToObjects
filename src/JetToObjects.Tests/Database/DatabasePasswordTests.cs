using System.IO;
using System.Linq;
using JetToObjects.Database;
using NUnit.Framework;

namespace JetToObjects.Tests.Database
{
    [TestFixture]
    public class DatabasePasswordTests
    {
        const string DB = "password-test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("password-database.mdb", DB, true);
        }

        [Test]
        public void ExecuteMany_WithPasswordProtectedDB_And_Correct_Password_Can_Connect_To_Database()
        {
            var results = Db.Open(DB, "abcd1234").ExecuteMany("select * from Products");

            Assert.That(results.Count(), Is.EqualTo(0));
        }
    }
}