using System;
using Microsoft.AnalysisServices;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace CubeGenerator
{
	public class CubeFactory
	{

		private Server   objServer;
		private Database objDatabase;

		public CubeFactory ()
		{
		}

		public CubeFactory (string strDBServerName, string strProviderName,String strDbName)
		{
			ConnectAnalysisServices (strDBServerName,strProviderName);
			DatabaseConnect (strDbName);
		}

		public Server ConnectAnalysisServices(string strDBServerName, string strProviderName)
		{
			string strConnection = "Data Source=" + strDBServerName + ";Provider=" + strProviderName + ";";
			return ConnectAnalysisServices (strConnection);
		}
		public Server ConnectAnalysisServices(string strConnection)
		{
			try
			{
				Console.WriteLine("Connecting to the Analysis Services ...");

				objServer = new Server();

				//Disconnect from current connection if it's currently connected.
				if (objServer.Connected)
					objServer.Disconnect();
				else
					objServer.Connect(strConnection);

				return objServer;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Connecting to the Analysis Services. Error Message -> " + ex.Message);
				return null;
			}

		}
		public Database DatabaseConnect(String strDbName)
		{
			try
			{
				Console.WriteLine("Connecting a Database ...");
				//Add Database to the Analysis Services.
				objDatabase = objServer.Databases.GetByName(strDbName);              
				if(objDatabase == null){
					objDatabase = DatabaseCreate(strDbName);
				}

				return objDatabase;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Creating a Database. Error Message -> " + ex.Message);
				return null;
			}

		}
		public Database DatabaseCreate(String strDbName){
			try
			{
				Console.WriteLine("Creating a Database ...");
				objDatabase = objServer.Databases.Add(strDbName);
				//Save Database to the Analysis Services.
				objDatabase.Update();                

				return objDatabase;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Creating a Database. Error Message -> " + ex.Message);
				return null;
			}

		}
		public bool IsConnected()
		{
			if (objServer != null) {
				bool connected = objServer.Connected;
				return connected;
			}else{
				return false;
			}

		}
	}
}

