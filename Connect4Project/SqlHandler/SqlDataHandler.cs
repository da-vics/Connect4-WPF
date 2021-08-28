using Connect4Project.SqlHandler.DataModels;
using System;
using Dapper;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Connect4Project.Models;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Connect4Project.SqlHandler
{
    class SqlDataHandler
    {
        string connectionString { get; set; }

        public SqlDataHandler()
        {
            connectionString = ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString; // set Connection String in App.config
            CreateTables();
        }

        #region CreateTables

        private void CreateTables()
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            var sqlConnection = new SqlConnection(connectionString);

            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                    sqlConnection.Open();
            }

            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            string sqlQuery = @"

             	
            IF OBJECT_ID('dbo.UserInfo', 'U') IS NULL CREATE TABLE dbo.UserInfo (
                    id int identity(1,1) PRIMARY KEY,
                    firstname varchar(100) NOT NULL,
                    lastname varchar(100) NOT NULL,
                    email varchar(100) NOT NULL UNIQUE,
                    username varchar(100) NOT NULL UNIQUE,
                    country TEXT NOT NULL
                );


            IF OBJECT_ID('dbo.Highscores', 'U') IS NULL CREATE TABLE  dbo.Highscores (
                    id int identity(1,1) PRIMARY KEY ,
                    username varchar(100) NOT NULL,
                    highscore INT,
                    FOREIGN KEY (username) references userinfo(username) ON DELETE CASCADE
                );

            IF OBJECT_ID('dbo.GameSatistics', 'U') IS NULL CREATE TABLE  dbo.GameSatistics (
                    id int identity(1,1) PRIMARY KEY,
                    username varchar(100) NOT NULL,
                    wins INT,
                    losses INT,
                    FOREIGN KEY (username) references userinfo(username) ON DELETE CASCADE
                );";


            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            Task.Run(async () =>
            {
                try
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }

                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }

                finally
                {
                    sqlConnection.Close();
                }

            });
        }

        #endregion


        #region RegisterUser

        public void RegisterUser(UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = @"
                                    INSERT INTO dbo.userinfo
                                        (firstname,
                                         lastname,
                                         email,
                                         username,
                                         country)
                                    VALUES
                                        (@firstname,@lastname,
                                        @email,@username,@country);
                                    ";
                try
                {
                    connection.Execute(sqlQuery, userInfo);
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

            bool verified = false;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    var result = connection.Query<string>($"select username from dbo.userinfo where username = '{userName}';").ToList<string>();
                    if (result.Count > 0)
                        verified = true;
                }
                catch (Exception err)
                {
                    throw err;
                }
            }

            return verified;
        }

        #endregion


        #region PlayerUPdateStats

        public void UpdatePlayerStats(string playerWon, string playerLost, int winnerpoints)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            Task.Run(async () =>
            {
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        var winnerPoint = await connection.QueryFirstOrDefaultAsync<int>($"select highscore from dbo.highscores where username = '{playerWon}';");
                        winnerPoint += winnerpoints;
                        connection.Execute($"update dbo.highscores set highscore = '{winnerPoint}' where username = '{playerWon}'");

                        var winnerWinCount = await connection.QueryFirstOrDefaultAsync<int>($"select wins from gamesatistics where username = '{playerWon}';");
                        winnerWinCount += 1;
                        connection.Execute($"update dbo.gamesatistics set wins = '{winnerWinCount}' where username = '{playerWon}'");

                        var losserCount = await connection.QueryFirstOrDefaultAsync<int>($"select losses from gamesatistics where username = '{playerLost}';");
                        losserCount += 1;
                        connection.Execute($"update dbo.gamesatistics set losses = '{losserCount}' where username = '{playerLost}'");

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

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    var result = connection.Query<PlayerStats>("select username, highscore from dbo.highscores order by highscore desc;").ToList<PlayerStats>();
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
            if (string.IsNullOrEmpty(connectionString))
                return;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Execute($"delete from dbo.userinfo where username = '{username}';");
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
