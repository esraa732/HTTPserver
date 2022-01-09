using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 1000);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(endPoint);
        }

        public void StartServer()
        {
            Console.WriteLine("Start Listening");
           
            // TODO: Listen to connections, with large backlog.
             serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket clientsocket = serverSocket.Accept();
                //TODO: accept connections and start thread for each accepted connection.
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientsocket);

            }
        }

        public void HandleConnection(object obj)
        {
            Console.WriteLine("Conection Accepted");

            // TODO: Create client socket 
            Socket clientsocket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientsocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] recievedata = new byte[65536];
                    int receivedLen = clientsocket.Receive(recievedata);
                    string data = Encoding.ASCII.GetString(recievedata);

                    Console.WriteLine(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0) break;

                    // TODO: Create a Request object using received request string
                    Request clientrequest = new Request(data);

                    // TODO: Call HandleRequest Method that returns the response
                    Response serverresponse = HandleRequest(clientrequest);
                    string res = serverresponse.ResponseString;
                    Console.WriteLine(res);

                    byte[] response = Encoding.ASCII.GetBytes(res);


                    // TODO: Send Response back to client
                    clientsocket.Send(response);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientsocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content="";
            string code = "";
            try
            {
                //throw new NotImplementedException();
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    code = "400";
                    content = "<!DOCTYPE html >< html >< body >< h1 > 400 Bad Request</ h1 >< p > 400 Bad Request</ p ></ body > </ html >";
                }


                //TODO: map the relativeURI in request to get the physical path of the resource.
                string[] name = request.relativeURI.Split('/');
                string phy_path = Configuration.RootPath + '\\' + name[1];

                //TODO: check for redirect
                for (int i=0; i<Configuration.RedirectionRules.Count;i++)
                {
                    if ('/'+Configuration.RedirectionRules.Keys.ElementAt(i).ToString()== request.relativeURI)
                    {
                        code = "301";
                        request.relativeURI = "/" + Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        name[1]= Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        phy_path= Configuration.RootPath + '\\' + name[1];
                        content = File.ReadAllText(phy_path);
                        string Location = "http://localhost:1000/" + name[1];
                        Response res = new Response(code, "text/html", content, Location);
                        return res;
                    }
                }
                //TODO: check file exists
                if(!File.Exists(phy_path))
                {
                    phy_path = Configuration.RootPath + '\\' + "NotFound.html";
                    code = "404";
                    content = File.ReadAllText(phy_path);
                }
                //TODO: read the physical file
                else
                {
                    content = File.ReadAllText(phy_path);
                    code = "200";
                }

                // Create OK response
                Response re = new Response(code, "text/html", content, phy_path);
                return re;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class  
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                string physical_path = Configuration.RootPath + '\\' + "InternalError.html";
                code = "500";
                content = File.ReadAllText(physical_path);
                Response re = new Response(code, "text/html", content, physical_path);

                return re;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
            {
                if(relativePath== '/' + Configuration.RedirectionRules.Keys.ElementAt(i).ToString())
                {
                    
                    string redirected_path = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                    string physical_path = Configuration.RootPath + '\\' + redirected_path;
                    return physical_path;
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string content = "";
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            try
            {
                // TODO: check if filepath not exist log exception using Logger class and return empty string
                if (File.Exists(filePath))
                {
                    content = File.ReadAllText(filePath);
                }
            // else read file and return its content
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                FileStream fs = new FileStream(filePath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                // then fill Configuration.RedirectionRules dictionary 
                while(sr.Peek() != -1)
                {
                    String line = sr.ReadLine();
                    String[] data = line.Split(',');
                    if (data[0]=="")
                    {
                        break;
                    }
                    Configuration.RedirectionRules.Add(data[0], data[1]);
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                
                Environment.Exit(1);
            }
        }
    }
}
