using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using JetToObjects.Database;
using JetToObjects.Models;
using NUnit.Framework;

namespace JetToObjects.Tests.Database
{
    [TestFixture]
    public class ExecuteMultipleQueryTests
    {
        const string DB = "test.mdb";

        [SetUp]
        public void SetUp()
        {
            File.Copy("database.mdb", DB, true);
        }

        [Test]
        public void ExecuteMany_WithMultipleUpdateStatement_ReturnsRecordsInDatabase()
        {
            InsertProduct(1, true, "Product 1", 5.99);
            InsertProduct(2, false, "Product 2", 6.99);
            InsertProduct(3, true, "Product 3", 7.99);

            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Id = 1,
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("@Price", "2.99", OleDbType.Currency),
                        new Param("@Name", "Test", OleDbType.VarWChar),
                        new Param("@CatID", "1", OleDbType.BigInt)
                    },
                    QueryType = QueryType.ExecuteNonQuery
                },
                new MultipleQuery
                {
                    Id = 2,
                    Query = "UPDATE [Products] SET [Price] = @Price WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("@Price", "1.21", OleDbType.Currency),
                        new Param("@CatID", "2", OleDbType.BigInt)
                    },
                    QueryType = QueryType.ExecuteNonQuery
                },
                new MultipleQuery
                {
                    Id = 3,
                    Query = "SELECT * FROM [Products]",
                    Parameters = new Param[0],
                    QueryType = QueryType.ExecuteMany
                }
            };


            var multipleQueryResults = Db.Open(DB).ExecuteMultipleQueries(queries);

            Assert.That(multipleQueryResults.Count, Is.EqualTo(3));

            AssertProductValues(multipleQueryResults[3][0], 1, true, "Test", 2.99);
            AssertProductValues(multipleQueryResults[3][1], 2, false, "Product 2", 1.21);
            AssertProductValues(multipleQueryResults[3][2], 3, true, "Product 3", 7.99);
        }

        #region Single MultipleQuery tests

        [Test]
        public void ExecuteSingleMultipleQuery_PassNoQueryField_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = null
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = string.Empty
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery()
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }
        [Test]
        public void ExecuteSingleMultipleQuery_PassNoId_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 0
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;"
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }
        [Test]
        public void ExecuteSingleMultipleQuery_PassNoParameters_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 0,
                    Parameters = null
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 0,
                    Parameters = new Param[0]
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 0
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }
        [Test]
        public void ExecuteSingleMultipleQuery_PassNoQueryType_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("Price", "2.99", OleDbType.Currency),
                        new Param("Name", "Test", OleDbType.VarWChar),
                        new Param("CatID", "1", OleDbType.BigInt)
                    },
                    Id = 1
                }
            };
            
            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }

        #endregion

        #region Multi MultipleQuery tests

        [Test]
        public void ExecuteMultiMultipleQuery_PassNoQueryField_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;"
                },
                new MultipleQuery
                {
                    Query = null
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;"
                },
                new MultipleQuery
                {
                    Query = string.Empty
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));


            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;"
                },
                new MultipleQuery()
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }
        [Test]
        public void ExecuteMultiMultipleQuery_PassNoId_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 1
                },
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 0
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 1
                },
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;"
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }


        [Test]
        public void ExecuteMultiMultipleQuery_PassNoParameters_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Id = 1,
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("Price", "2.99", OleDbType.Currency),
                        new Param("Name", "Test", OleDbType.VarWChar),
                        new Param("CatID", "1", OleDbType.BigInt)
                    },
                    QueryType = QueryType.ExecuteSingle
                },
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 1,
                    Parameters = null
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Id = 1,
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("Price", "2.99", OleDbType.Currency),
                        new Param("Name", "Test", OleDbType.VarWChar),
                        new Param("CatID", "1", OleDbType.BigInt)
                    },
                    QueryType = QueryType.ExecuteSingle
                },
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 1,
                    Parameters = new Param[0]
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));

            queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Id = 1,
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("Price", "2.99", OleDbType.Currency),
                        new Param("Name", "Test", OleDbType.VarWChar),
                        new Param("CatID", "1", OleDbType.BigInt)
                    },
                    QueryType = QueryType.ExecuteSingle
                },
                new MultipleQuery
                {
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Id = 1
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }
        [Test]
        public void ExecuteMultiMultipleQuery_PassNoQueryType_ShouldThrowMissingFieldException()
        {
            var queries = new List<MultipleQuery>
            {
                new MultipleQuery
                {
                    Id = 1,
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("Price", "2.99", OleDbType.Currency),
                        new Param("Name", "Test", OleDbType.VarWChar),
                        new Param("CatID", "1", OleDbType.BigInt)
                    },
                    QueryType = QueryType.ExecuteSingle
                },
                new MultipleQuery
                {
                    Id = 1,
                    Query = "UPDATE [Products] SET [Price] = @Price, [Name] = @Name WHERE [CategoryID] = @CatID;",
                    Parameters = new[]
                    {
                        new Param("Price", "2.99", OleDbType.Currency),
                        new Param("Name", "Test", OleDbType.VarWChar),
                        new Param("CatID", "1", OleDbType.BigInt)
                    }
                }
            };

            Assert.Throws<MissingFieldException>(() => Db.Open(DB).ExecuteMultipleQueries(queries));
        }

        #endregion

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