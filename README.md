# JetToObjects

JetToObjects is a basic data access library that simplifies accessing a Jet database and getting results back.

JetToObjects is available via [NuGet](https://www.nuget.org/packages/JetToObjects/):
```PowerShell
Install-Package JetToObjects
```

### Opening A Connection

All queries start with a call to Db.Open() which must be supplied with the path to the database file. Stemming from the .Open() call is a fluent interface that allows the rest of the query to be constructed.


```cs
var result = Db.Open("database.mdb")...
```

## Query API

The following query methods are supported:

#### ExecuteNonQuery

```cs
// Inserts a new row into the Products table and requests that the identity
// of the new row is returned from the call.

var query = "INSERT INTO Products (Name) VALUES (@Name)";
var parameter = new Param("@Name", "New Product", OleDbType.VarWChar);
var id = Db.Open(db).ReturnIdentity().ExecuteNonQuery(query, parameter);
```

#### ExecuteNonQueryWithConnection

**NOTE**: This function does **NOT** automatically dispose of the connection, you must do this yourself.

```cs
// Performs an ExecuteNonQuery but on an existing connection that you pass in as a parameter to the function.

var connection = new OleDbConnection(connectionString);
var query = "INSERT INTO Products (Name) VALUES (@Name)";
var parameter = new Param("@Name", "New Product", OleDbType.VarWChar);
var id = Db.Open(db).ReturnIdentity().ExecuteNonQueryWithConnection(query, connection, parameter);
```

#### ExecuteScalar

```cs
// Selects just the product name from the product whose ID is 42

var query = "SELECT Name FROM Products WHERE ID = @ID";
var parameter = new Param("@ID", 42, OleDbType.BigInt);
var name = Db.Open(db).ExecuteScalar(query, parameter);
```

#### ExecuteScalarWithConnection

**NOTE**: This function does **NOT** automatically dispose of the connection, you must do this yourself.

```cs
// Performs an ExecuteScalar but on an existing connection that you pass in as a parameter to the function.

var connection = new OleDbConnection(connectionString);
var query = "SELECT Name FROM Products WHERE ID = @ID";
var parameter = new Param("@ID", 42, OleDbType.BigInt);
var name = Db.Open(db).ExecuteScalarWithConnection(query, connection, parameter);
```

#### ExecuteSingle

```cs
// Selects the name, description and price of the product whose ID is 42. The result comes
// back as a dynamic object with the respective properties added to it from the row.

var query = "SELECT Name, Description, Price FROM Products WHERE ID = @ID";
var parameter = new Param("@ID", 42, OleDbType.BigInt);
var product = Db.Open(db).ExecuteSingle(query, parameter);
```

#### ExecuteSingleWithConnection

**NOTE**: This function does **NOT** automatically dispose of the connection, you must do this yourself.

```cs
// Performs an Execute Single but on an existing connection that you pass in as a parameter to the function.

var connection = new OleDbConnection(connectionString);
var query = "SELECT Name, Description, Price FROM Products WHERE ID = @ID";
var parameter = new Param("@ID", 42, OleDbType.BigInt);
var product = Db.Open(db).ExecuteSingleWithConnection(query, connection, parameter);
```

#### ExecuteMany

```cs
// Selects every product from the Products table. The result is an IEnumerable<dynamic>,
// with each element holding every column from the row.

var query = "SELECT * FROM Products";
var products = Db.Open(db).ExecuteMany(query);
```

#### ExecuteManyWithConnection

**NOTE**: This function does **NOT** automatically dispose of the connection, you must do this yourself.

```cs
// Performs an Execute Many but on an existing connection that you pass in as a parameter to the function.

var connection = new OleDbConnection(connectionString);
var query = "SELECT * FROM Products";
var products = Db.Open(db).ExecuteManyWithConnection(query, connnection);
```

#### ExecuteMultipleQueries

**NOTE**: QueryId has to be >= 1

```cs
/// <summary>
/// Loops through the IEnumerable of statements passed in, executing each with respective params all whilst the current connection is open.
/// </summary>
/// <param name="queries">A list of MultipleQuery objects, including the query and params for each statement to be run and the QueryType to be executed (ExecuteSingle,ExecuteMany etc. MultipleQuery: int Id: Id, string Query: Query, Parameters Param[] : Param[], /* Enum */ QueryType? queryType: QueryType</param>
/// <returns>A dictionary collection of the results from the queries passed in, with the Id of the query as the Key in the dictionary and a dynamic as the Value. You will have to cast the dynamic value to the result type for each corresponding QueryType to obtain your results from it.</returns>

var queries = new List<MultipleQuery>
{
    new MultipleQuery
    {
        Id = 1,
        Query = "UPDATE Products SET Name = 'Test product name' WHERE ID = 1;",
        Parameters = parameters.ToArray(),
        QueryType = QueryType.ExecuteNonQuery
    },
    new MultipleQuery
    {
        Id = 2,
        Query = "SELECT * FROM Products;",
        Parameters = parameters.ToArray(),
        QueryType = QueryType.ExecuteMany
    }
};

var queryResults = _databaseClient.ExecuteMultipleQueries(_database, queries);
var updateRowsAffected = queryResults[udpateQueryId].RowsAffected;

// Select Results need to be cast to an IEnumerable<dynamic> first as this is the return type from the ExecuteMany function.
var castResults = (IEnumerable<dynamic>) queryResults[selectQueryId];

// If you map your results to a Product class you will now have your results in the correct format.
var selectResults = mapProductResultsExampleFunction(castResults);
```

#### ExecuteMultipleQueriesWithConnection

**NOTE**: This function does **NOT** automatically dispose of the connection, you must do this yourself.

**NOTE**: QueryId has to be >= 1
```cs
/// <summary>
// Performs an Execute Multiple Queries but on an existing connection that you pass in as a parameter to the function.
/// </summary>

var queries = new List<MultipleQuery>
{
    new MultipleQuery
    {
        Id = 1,
        Query = "UPDATE Products SET Name = 'Test product name' WHERE ID = 1;",
        Parameters = parameters.ToArray(),
        QueryType = QueryType.ExecuteNonQuery
    },
    new MultipleQuery
    {
        Id = 2,
        Query = "SELECT * FROM Products;",
        Parameters = parameters.ToArray(),
        QueryType = QueryType.ExecuteMany
    }
};
var connection = new OleDbConnection(connectionString);
var queryResults = _databaseClient.ExecuteMultipleQueriesWithConnection(_database, queries, connection);
var updateRowsAffected = queryResults[udpateQueryId].RowsAffected;

// Select Results need to be cast to an IEnumerable<dynamic> first as this is the return type from the ExecuteMany function.
var castResults = (IEnumerable<dynamic>) queryResults[selectQueryId];

// If you map your results to a Product class you will now have your results in the correct format.
var selectResults = mapProductResultsExampleFunction(castResults);
```

#### CompactRepair

```cs
// Database is compacted to a new database file, then copied over the old one

var product = Db.Open(db).CompactRepair("newDatabasePath"));
```

## Mapping API

To support legacy situations, JetToObjects comes with an object mapper that allows you to convert the dynamic results to other classes by following a mapping rule set.

#### Example
Let's assume you have the following class, and wish to map the results from a query (the dynamic object) to it:

```cs
public class ProductData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
```

The `Mapper.Map()` call will allow you to do just that:

```cs
var query = "SELECT name, desc, cost FROM Products WHERE ID = @ID";
var parameter = new Param("@ID", 42, OleDbType.BigInt);
var product = Db.Open(db).ExecuteSingle(query, parameter);

var productData = Mapper.Map<ProductData, ProductMapping>(product);
```

Notice the use of `ProductMapping` (the second generic argument) which tells `Mapper` how to re-map the fields (name, desc, cost) from the query result to the fields in the `ProductData` class. `ProductMapping` looks like this:

```cs
public class ProductMapping : ObjectMapping
{
    public ProductMapping()
    {
        Map("name", "Name");
        Map("desc", "Description");
        Map("cost", "Price");
    }
}
```
