using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;
using AnsibleBackgroundWorker;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;

public class SynchronousSocketListener {
    

     // Incoming data from the client.  
    public static string data = null;  
  
    public static void StartListening() {  
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];  

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);  
  
        // Create a TCP/IP socket.  
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp ); 
        
        var rs = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
        rs.Open();
        //Console.WriteLine($"Powershell version: {ps.VersionInfo}");
  
        // Bind the socket to the local endpoint and   
        // listen for incoming connections.  
        try {  
            listener.Bind(localEndPoint);  
            listener.Listen(10);  
  
            // Start listening for connections.  
            while (true) {  
                Console.WriteLine("Waiting for a connection...");  
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();  
                data = null;  
  
                // An incoming connection needs to be processed.  
                while (true) {  
                    int bytesRec = handler.Receive(bytes);  
                    data += Encoding.ASCII.GetString(bytes,0,bytesRec);
                    //Console.WriteLine( "Current string buffer: {0}", data);
                    if (data.IndexOf("\r\n") > -1) {  
                        break;  
                    }  
                }  
  
                // Show the data on the console.    
                // Echo the data back to the client.
                Console.WriteLine( "Text received : {0}", data);
                using (PowerShell ps = PowerShell.Create())
                { 
                    //ps.Runspace = rs;
                    ps.AddScript(data); 
                    var psResult = ps.Invoke();
                    foreach (var psResultItem in psResult)
                    {
                        byte[] msg = Encoding.ASCII.GetBytes(psResultItem.ToString());
                        handler.Send(msg);  
                    } 
                }
                
                  
                handler.Shutdown(SocketShutdown.Both);  
                handler.Close();  
            }  
  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
  
        Console.WriteLine("\nPress ENTER to continue...");  
        Console.Read();  
  
    }  
  
    [STAThread]
    public static int Main(String[] args) {  
        StartListening();  
        return 0;  
    }  
}