using MySql.Data.MySqlClient;

namespace ProteusWeb.Database;

public class DatabaseController
{
    public string? ConnectionString { get; set; }
    
    public DatabaseController(string connectionString)
    {
        ConnectionString = connectionString;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }

    public string GetDatabases()
    {
        var connection = GetConnection();
        connection.Open();

        var adapter = new MySqlDataAdapter();
        adapter.SelectCommand = new MySqlCommand("SELECT * FROM Users", connection);
        return "";
    }
}