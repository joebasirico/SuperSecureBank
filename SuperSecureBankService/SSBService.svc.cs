using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace SuperSecureBankService
{
	public class SSBService : ISSBService
	{
		public int CreateUser(string username, string email, string pass)
		{
			try
			{
				string insertUser = @"INSERT INTO Users values ('{0}', '{1}', '{2}'); SELECT SCOPE_IDENTITY();";
				SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString);
				conn.Open();
				insertUser = String.Format(insertUser, username, email, pass);
				SqlCommand command = new SqlCommand(insertUser, conn);
				int userID = Convert.ToInt32(command.ExecuteScalar());
				conn.Close();
				return userID;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public int LookupSession(string sessionValue)
		{
			int userID = 0;
			int sessionID = 0;
			if (int.TryParse(sessionValue, out sessionID))
			{
				string getUserID = "SELECT userID FROM sessions WHERE sessionID = {0}";
				using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString))
				{
					conn.Open();
					getUserID = String.Format(getUserID, sessionValue);
					SqlCommand command = new SqlCommand(getUserID, conn);
					SqlDataReader reader = command.ExecuteReader();

					while (reader.Read())
					{
						userID = reader.GetInt32(0);
					}
				}
			}
			return userID;
		}

		public static string LookupUsername(int userID)
		{
			string userName = "";

			string getUserName = "SELECT userName FROM Users WHERE userID = {0}";
			using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString))
			{
				conn.Open();
				getUserName = String.Format(getUserName, userID);
				SqlCommand command = new SqlCommand(getUserName, conn);
				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					userName = reader.GetString(0);
				}
			}
			return userName;
		}

		public static int CheckUser(string username, string password)
		{
			int userID = 0;

			string getUserID = "SELECT userID FROM Users WHERE userName = '{0}' AND password = '{1}'";
			using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString))
			{
				conn.Open();
				getUserID = String.Format(getUserID, username, password);
				SqlCommand command = new SqlCommand(getUserID, conn);
				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					userID = reader.GetInt32(0);
				}
			}
			return userID;
		}

		public bool UserExists(string username)
		{
			int userID = 0;

			string getUserID = "SELECT userID FROM Users WHERE userName = '{0}'";
			using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString))
			{
				conn.Open();
				getUserID = String.Format(getUserID, username);
				SqlCommand command = new SqlCommand(getUserID, conn);
				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					userID = reader.GetInt32(0);
				}
			}
			return userID != 0;
		}

		public static int CreateSession(int userID)
		{
			int sessionID = SessionIDSingleton.Instance.NextSessionID;
			string insertSession = @"INSERT INTO sessions values ({0}, {1})";
			SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString);
			conn.Open();
			insertSession = String.Format(insertSession, userID, sessionID);
			SqlCommand command = new SqlCommand(insertSession, conn);
			command.ExecuteNonQuery();
			conn.Close();

			return sessionID;
		}

		public void RemoveSession(int sessionID)
		{
			string deleteSession = @"DELETE FROM sessions WHERE sessionID = {0}";
			SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ssbcon"].ConnectionString);
			conn.Open();
			deleteSession = String.Format(deleteSession, sessionID);
			SqlCommand command = new SqlCommand(deleteSession, conn);
			command.ExecuteNonQuery();
			conn.Close();
		}
	}
}
