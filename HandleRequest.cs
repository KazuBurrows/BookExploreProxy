using System;
using Oracle.ManagedDataAccess.Client;

using System.Data;
using BookExploreProxy;
using System.Linq;
using System.Collections.Generic;

class HandleRequest
{


	/// <summary>
	/// Main function in Class
	/// </summary>
	/// <param name="conn"></param>
	/// <param name="my_query"></param>
	/// <returns></returns>
	public string[] handleQuery(OracleConnection conn, string my_query)
	{
		//Maybe do some checks?
		// I should have implemented the client sides 'get_query_format()' function in here instead.

		//Console.WriteLine("reached handleQuery function.");
		//Console.WriteLine("my_query: " + my_query);


		SearchQuery search_query = new SearchQuery(my_query);
		string[] words = search_query.getSearchWords().ToArray();



		string startCmdText = "BEGIN ";
		string mainCmdText = "";
		string procedureCmdText = "search_proc(); ";
        string endCmdText = "END;";

		
		string mainText;
        foreach (string word in words) {
			mainText = $"INSERT INTO temp_search_table VALUES ('{word}'); ";

			mainCmdText += mainText;



        }


		var commandText = startCmdText + mainCmdText + procedureCmdText + endCmdText;

		OracleCommand cmd2 = new OracleCommand();
		cmd2.Connection = conn;
		cmd2.CommandText = commandText;
        int feed = cmd2.ExecuteNonQuery();

        Console.WriteLine("FEED: " + feed);


		OracleCommand cmd4 = new OracleCommand();
        cmd4.Connection = conn;
        cmd4.CommandText = "SELECT ID, title FROM TABLE(main_func(" + words.Length + "))";

		OracleDataReader reader = cmd4.ExecuteReader();

		Console.WriteLine("START READER");


        List<string> selected_rows = new List<string>();
		string tuple_book;
        while (reader.Read())
		{
			// output Employee Name and Number
			//Console.WriteLine("ID : " + reader.GetString(0) + " " + "Title : " + reader.GetString(1));
			Console.WriteLine("ID : " + reader.GetString(0) + " " + "Title : " + reader.GetString(1));


			tuple_book =  "" + reader.GetString(0) + ", " + reader.GetString(1) + "";
            selected_rows.Add(tuple_book);

        }
        


        OracleCommand cmd5 = new OracleCommand();
        cmd5.Connection = conn;
        cmd5.CommandText = "TRUNCATE TABLE temp_search_table";
        cmd5.ExecuteNonQuery();

        OracleCommand cmd6 = new OracleCommand();
        cmd6.Connection = conn;
        cmd6.CommandText = "TRUNCATE TABLE temp_results_table";
        cmd6.ExecuteNonQuery();








		// Clean up
		reader.Dispose();




		////string response = "";
		////response = commitDBQuery(conn, my_query);

		//return response;

		//string return_rows = string.Join(",", selected_rows);

		//Tuple<string, string>[] return_rows = selected_rows.Select(i => new Tuple<string, string>(i.Item1, i.Item2)).ToArray();

		//return return_rows;

		
		return selected_rows.ToArray();
		//return words;
	}

	


	
	private string constructQuery(string my_query)
	{
		// NEED TO INSERT QUERY WORDS


		//OracleCommand cmd = new OracleCommand("begin MyPack.in_s_words_pro(:1); end;", con);
		//OracleParameter search_words = cmd.Parameters.Add("1", OracleDbType.Varchar2);

		//search_words.Direction = ParameterDirection.InputOutput;
		//search_words.CollectionType = OracleCollectionType.PLSQLAssociativeArray;						// Specify that we are binding PL/SQL Associative Array

		//search_words.Value = new string[SEARCH_WORDS_COUNT] {
		//	"First Element", "Second Element", "Third Element"
		//};


		//Param1.Size = SEARCH_WORDS_COUNT;																		// Specify the maximum number of elements in the PL/SQL Associative Array
		//Param1.ArrayBindSize = Enumerable.Range(1, SEARCH_WORDS_COUNT).Select(i => new int()).ToArray();		// Setup the ArrayBindSize for Param1

		//cmd.ExecuteNonQuery();



		//"EXECUTE search_proc;"
		//"SELECT * FROM TABLE(main_func(" + SEARCH_WORDS_COUNT + "));"
		//"COMMIT;"

		return "";
	
	}





    /// <summary>
    /// Make a connection with db
    /// https://docs.oracle.com/en/database/oracle/oracle-database/18/odpnt/OracleCommandClass.html#GUID-7C1D5BCC-0268-4F1B-A583-F401F30EB9C0
    /// </summary>
    /// <param name="my_query"></param>
    /// <returns></returns>
    private string commitDBQuery(OracleConnection conn, string my_query)
	{
        //OracleCommand cmd = new OracleCommand(my_query);
        OracleCommand cmd = new OracleCommand();

        cmd.Connection = conn;
        cmd.CommandText = "select id, title from books where title like '%Harry%'";
        cmd.CommandType = CommandType.Text;                     // Let's the ___ know that the SQL query is in string format.
        
        // Execute command, create OracleDataReader object
        OracleDataReader reader = cmd.ExecuteReader();
        
        while (reader.Read())
		{
			// output Employee Name and Number
			Console.WriteLine("ID : " + reader.GetString(0) + " " + "Title : " + reader.GetString(1));
		}

		// Clean up
		reader.Dispose();
		cmd.Dispose();


		return "";
	}










	/////<summary>
	/////
	///// </summary>
	/////<param name=""></param>
	/////<returns></returns>
	//public void constructQuery(string query_type, string search_input)
	//{
			
	//	string query_string = $"SELECT title FROM books WHERE LOWER(title) LIKE '%{search_input}%'";


	//}


	//static void Main(string[] args)
	//{
	//	HandleRequest myObj = new HandleRequest();
	//	myObj.constructQuery("", "Harry Potter");


	//}
	
}
