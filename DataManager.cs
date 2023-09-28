
using System.Data;
using System.Data.SqlClient;

public class DatabaseManager
{
    private readonly string connectionString;

    public DatabaseManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public DataTable GetCardData()
    {
        DataTable cardData = new DataTable();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM SM_CardDataAct";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(cardData);
                }
            }
        }
        return cardData;
    }
    public DataTable ExecuteQuery(string sqlQuery)
    {
        DataTable resultData = new DataTable();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(resultData);
                }
            }
        }
        return resultData;
    }


}
