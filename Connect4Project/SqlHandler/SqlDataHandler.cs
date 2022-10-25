using Connect4Project.SqlHandler.DataModels;
using System;
using Dapper;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using Connect4Project.Models;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Data.SQLite;

namespace Connect4Project.SqlHandler
{
    class SqlDataHandler
    {
        public string baseDirectory = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%");
        private string connectionString;
        private string dbPath;

        public SqlDataHandler()
        {
            baseDirectory = Path.Combine(baseDirectory, "Connect4");

            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);

            dbPath = Path.Combine(baseDirectory, "access.db");
            connectionString = $"Data Source={dbPath}; Version=3;";
            CreateTables();
        }

        #region CreateTables

        private void CreateTables()
        {
            string sqlQuery = @"
             	
            CREATE TABLE IF NOT EXISTS UserInfo (
                    id INTEGER,
                    username VARCHAR(100) NOT NULL UNIQUE,
                    country TEXT NOT NULL,
                    PRIMARY KEY(id AUTOINCREMENT)
                );


            CREATE TABLE IF NOT EXISTS Highscores (
                    id INTEGER,
                    username VARCHAR(100) NOT NULL,
                    highscore INT,
                    PRIMARY KEY(id AUTOINCREMENT),
                    FOREIGN KEY (username) references UserInfo(username) ON DELETE CASCADE
                );

            CREATE TABLE IF NOT EXISTS GameSatistics (
                    id INTEGER,
                    username VARCHAR(100) NOT NULL,
                    wins INT,
                    losses INT,
                    PRIMARY KEY(id AUTOINCREMENT),
                    FOREIGN KEY (username) references UserInfo(username) ON DELETE CASCADE
                );";

            using (IDbConnection connection = new SQLiteConnection(connectionString))
                connection.Execute(sqlQuery);
        }

        #endregion


        #region RegisterUser

        public void RegisterUser(UserInfo userInfo)
        {
            if (VerifyUser(userInfo.username))
                throw new Exception("User name exist");

            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Execute("INSERT INTO UserInfo (username,country) VALUES(@username,@country)", userInfo);
                    connection.Execute($"insert into Highscores(username,highscore) values('{userInfo.username}',0)");
                    connection.Execute($"insert into GameSatistics(username,wins,losses) values('{userInfo.username}',0,0)");
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
            }
        }

        #endregion


        #region SearchUser

        public bool VerifyUser(string userName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("ConnectionString Invalid");

            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    var result = connection.Query<string>($"select username from UserInfo where username = '{userName}';").ToList<string>();
                    if (result.Count > 0)
                        return true;

                    else
                        return false;
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }

        #endregion

        #region


        #endregion


        #region PlayerUPdateStats

        public void UpdatePlayerStats(string playerWon, string playerLost, int winnerpoints)
        {
            Task.Run(async () =>
          {
              using (IDbConnection connection = new SQLiteConnection(connectionString))
              {
                  try
                  {
                      var winnerPoint = await connection.QueryFirstOrDefaultAsync<int>($"select highscore from Highscores where username = '{playerWon}';");
                      winnerPoint += winnerpoints;
                      connection.Execute($"update Highscores set highscore = '{winnerPoint}' where username = '{playerWon}'");

                      var winnerWinCount = await connection.QueryFirstOrDefaultAsync<int>($"select wins from GameSatistics where username = '{playerWon}';");
                      winnerWinCount += 1;
                      connection.Execute($"update GameSatistics set wins = '{winnerWinCount}' where username = '{playerWon}'");

                      var losserCount = await connection.QueryFirstOrDefaultAsync<int>($"select losses from GameSatistics where username = '{playerLost}';");
                      losserCount += 1;
                      connection.Execute($"update GameSatistics set losses = '{losserCount}' where username = '{playerLost}'");

                  }
                  catch (Exception err)
                  {
                      Console.WriteLine(err.Message);
                  }
              }

          });
        }

        #endregion


        #region LeaderBoard

        public ObservableCollection<PlayerStats> GetLeaderBoard()
        {
            var stats = new ObservableCollection<PlayerStats>();

            if (string.IsNullOrEmpty(connectionString))
                return stats;

            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    var result = connection.Query<PlayerStats>("select username, highscore from Highscores order by highscore desc;").ToList<PlayerStats>();
                    stats = new ObservableCollection<PlayerStats>(result);

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }

            return stats;
        }

        #endregion


        #region DeletePlayer

        public void DeleteUser(string username)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Execute($"delete from UserInfo where username = '{username}';");
                    connection.Execute($"delete from Highscores where username = '{username}';");
                    connection.Execute($"delete from GameSatistics where username = '{username}';");
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }
        #endregion

    }
}
