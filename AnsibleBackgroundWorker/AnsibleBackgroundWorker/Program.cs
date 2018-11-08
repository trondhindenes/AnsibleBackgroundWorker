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
                handler.SendBufferSize = 1024;
                handler.ReceiveBufferSize = 10485760;
                data = null;  
  
                // An incoming connection needs to be processed.  

                while (true) {  
                    int bytesRec = handler.Receive(bytes);  
                    data += Encoding.ASCII.GetString(bytes,0,bytesRec);
                    //Console.WriteLine( "Current string buffer: {0}", data);
                    if (data.IndexOf("ANSIBLE_TCP_WINRM_END_END_END") > -1) {  
                        break;  
                    }  
                }

                data = data.Replace("ANSIBLE_TCP_WINRM_END_END_END", "");

/*                int recv;
                while((recv = handler.Receive(bytes)) > 0)
                {
                    // process recv-many bytes
                    // ... stringData = Encoding.ASCII.GetString(data, 0, recv);
                    data = Encoding.ASCII.GetString(bytes,0,recv);
                }*/
  
                // Show the data on the console.    
                // Echo the data back to the client.
                Console.WriteLine( "Text received");
                using (PowerShell ps = PowerShell.Create())
                { 
                    //ps.Runspace = rs;
                    ps.AddScript(data); 
                    var psResult = ps.Invoke();
                    foreach (var psResultItem in psResult)
                    {
                        var strMessage = psResultItem.BaseObject.ToString();
                        //Console.WriteLine(strMessage);
                        byte[] msg = Encoding.ASCII.GetBytes(strMessage);
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
  
    //[STAThread]
    public static int Main(String[] args) {  
        StartListening();  
        return 0;  
    }  
}