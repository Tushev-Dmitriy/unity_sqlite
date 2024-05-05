using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class SqliteConnect : MonoBehaviour
{
    [SerializeField] private int Id = 0;
    [SerializeField] private string Name = string.Empty;

    public GameObject loginField;
    public GameObject passwordField;

    public void SetConnection()
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        Debug.Log("db connected");
        dbConnection.Close();
    }

    public void LoginBtn()
    {
        CheckLogin(loginField.GetComponent<TMP_InputField>().text, passwordField.GetComponent<TMP_InputField>().text);
    }

    public void AddScore()
    {
        int Score = 0;

        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandAddScore = dbConnection.CreateCommand();
        dbCommandAddScore.CommandText = $"SELECT Score FROM Scoreboard WHERE PlayerId = {Id}";
        IDataReader dataReader = dbCommandAddScore.ExecuteReader();
        if (dataReader.Read())
        {
            Score = dataReader.GetInt32(0);
        }
        dbConnection.Close();

        dbConnection = CreateAndOpenDatabase();
        dbCommandAddScore = dbConnection.CreateCommand();
        dbCommandAddScore.CommandText = $"UPDATE Scoreboard SET Score = {Score+1} WHERE PlayerId = {Id}";
        dbCommandAddScore.ExecuteNonQuery();
        dbConnection.Close();
    }

    private void CheckLogin(string login, string password)
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandCheckLogin = dbConnection.CreateCommand();
        dbCommandCheckLogin.CommandText = $"SELECT * FROM Player WHERE Login = '{login}' AND Password = '{password}'";
        IDataReader dataReader = dbCommandCheckLogin.ExecuteReader();

        if (dataReader.Read())
        {
            Id = dataReader.GetInt32(0);
            Name = dataReader.GetString(1);

            Debug.Log("Успешный вход. Id: " + Id + ", Name: " + Name);
        }
        else
        {
            Debug.Log("Вход невозможен. Проверьте логин и пароль.");
        }

        dbConnection.Close();
    }

    private IDbConnection CreateAndOpenDatabase()
    {
        string dbUri = "URI=file:" + Application.dataPath + "/StreamingAssets/db.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        return dbConnection;
    }
}
