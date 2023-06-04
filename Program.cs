using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Oracle.ManagedDataAccess.Client;
using System.Data;
using BookExploreProxy;

/// <summary>
/// 
/// </summary>
public static class Globals
{
    public static OracleConnection conn;
}





// State object for reading client data asynchronously  
public class StateObject
{
    // Client  socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 1024;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
}



public class AsynchronousSocketListener
{
    // Thread signal.  
    public static ManualResetEvent allDone = new ManualResetEvent(false);



    // Main function
    public static void StartListening()
    {
        // Establish the local endpoint for the socket.  
        // The DNS name of the computer  
        // running the listener is "host.contoso.com".  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);



            string oradb = "Data Source=kazu-burrows;User Id=freezerz;Password=Rockstar03;";
            OracleConnection conn = new OracleConnection(oradb);  // C#
            conn.Open();
            Globals.conn = conn;                                                // Make 'conn' a global so I don't have to pass it through every function.

            Console.WriteLine("Connected to Oracle" + conn.ServerVersion);






            /*if (isMongoLive)
            {
                // connected

                Console.WriteLine("Successfully connected to Mongo database.");
            }
            else
            {
                // couldn't connect
                Console.WriteLine("Server was unsuccessful connecting to the database.");
            }*/






            while (true)
            {
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for requests from users.  
                Console.WriteLine("Waiting for requests from users...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }




    /**
     * Take client request from socket or buffer
     * 
     * 
     */
    public static void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.  
        allDone.Set();

        // Get the socket that handles the client request.  
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        // Create the state object.  
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
    }





    /**
     * Read and handle client request
     * 
     * 
     */
    public static void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket.
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            // There  might be more data, so store the data received so far.  
            state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read
            // more data.  
            content = state.sb.ToString();
            if (content.IndexOf("<EOF>") > -1)
            {
                // All the data has been read from the
                // client. Display it on the console.






                                                                    // Handle received message
                string response;

                


                int content_len = content.Length;
                string json_string = content.Substring(0, content_len - 5);

                //Console.WriteLine("My json_string test: " + json_string);

                HandleRequest handle_request = new HandleRequest();
                string[] return_rows = handle_request.handleQuery(Globals.conn, json_string);
                response = JsonConverter.encodeQuery("search response", return_rows);







                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}, {2}",
                    content.Length, content, state.workSocket.RemoteEndPoint);


                //Console.WriteLine("Here is the response for client " + response);
                Send(handler, response);
            }
            else
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }
        }
    }



    /**
     *Prepare and start sending the string data back to the client as bytes
     *
     *
     *
     */
    private static void Send(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }




    /** 
     * Finish sending a response back to the client
     */
    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main()
    {
        StartListening();


        Console.ReadLine();
        return 0;
    }
}



