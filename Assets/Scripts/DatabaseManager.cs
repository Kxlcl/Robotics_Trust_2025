using UnityEngine;
using System.IO;
using SQLite4Unity3d;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection _connection;

    void Awake()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "playerdata.db");

        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _connection.CreateTable<PlayerInfo>();
        _connection.CreateTable<PlayerDecision>();
        Debug.Log("Database initialized at: " + dbPath);
    }

    public void SavePlayerInfo(string playerName, int age)
    {
        _connection.Insert(new PlayerInfo { Name = playerName, Age = age });
    }

    public void SaveDecision(string question, string answer)
    {
        _connection.Insert(new PlayerDecision { Question = question, Answer = answer });
    }

    public class PlayerInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PlayerDecision
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
