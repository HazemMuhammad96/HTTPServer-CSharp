using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        int portNumber;


        public Server(int portNumber, string redirectionMatrixPath)
        {

            this.portNumber = portNumber;

            //LoadRedirectionRules: loads the redirection rules
            LoadRedirectionRules(redirectionMatrixPath);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(iPEndPoint);

        }

        public void StartServer(int backLog)
        {
            serverSocket.Listen(backLog);

            Console.WriteLine("Server is running on port: " + portNumber + ".");

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                Console.WriteLine("New client accepted: " + clientSocket.RemoteEndPoint);


                Thread connectionThread = new Thread(
                    new ParameterizedThreadStart(HandleConnection)
                );
                connectionThread.Start(clientSocket);
            }

        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket) obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0;

            byte[] buffer;

            // TODO: receive requests in while true until remote client closes the socket.
            int length;
            while (true)
            {
                try
                {
                    //Bad Request 
                    // TODO: Receive request
                    buffer = new byte[1024];
                    length = clientSock.Receive(buffer);


                    string requestString = Encoding.ASCII.GetString(buffer);

                    // TODO: break the while loop if receivedLen==0
                    if (length == 0)
                    {
                        Console.WriteLine(
                            "Client: Connection with client {0} has been ended",                       clientSock.RemoteEndPoint
                            );
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(requestString);

                    if (!request.ParseRequest())
                    {
                        request.fileRelativeURI = Configuration.InternalErrorDefaultPageName;
                    }

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = null;


                    Console.WriteLine("Method: " + request.Method);
                    switch(request.Method)
                    {
                  
                        case RequestMethod.POST:
                            response = HandlePostRequest(request);
                            break;
                        case RequestMethod.GET:
                            response = HandleGetRequest(request);
                            break;
                        case RequestMethod.HEAD:
                            response = HandleHeadRequest(request);
                            break;
                    }
                    // TODO: Send Response back to client
                    if(response != null)
                    {
                        clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString), 0, response.ResponseString.Length, SocketFlags.None);
                    }

                }
                catch (Exception e)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(e);
                }
                break;

            }

            // TODO: close client socket
            clientSock.Close();

        }

        private Response HandleHeadRequest(Request request)
        {
            StatusCode code = StatusCode.OK;

            if (!request.fileRelativeURI.Contains(".html"))
            {
                code = StatusCode.BadRequest;
            }

            String redirectedPhysicalPath = GetRedirectionPagePathIFExist(request.fileRelativeURI);
            if (redirectedPhysicalPath != string.Empty)
            {
                code = StatusCode.Redirect;
            }

            //TODO: map the relativeURI in request to get the physical path of the resource.
            string physicalPath = Configuration.RootPath + "\\" + request.fileRelativeURI;


            //TODO: check file exists          
            if (!File.Exists(physicalPath))
            {
                code = StatusCode.NotFound;
            }

            return new Response(request.http, code, "text/html", "", null);
        }

        private Response HandlePostRequest(Request request)
        {
            
            string requestBody  = request.ContentLines.Trim();
            StatusCode code = StatusCode.OK;
            string content = requestBody;

            return new Response(request.http, code, request.ContentType, content, "");

        }

        Response HandleGetRequest(Request request)
        {

            string content;
            StatusCode code = StatusCode.OK;


            // Server Error
            

            if (!request.fileRelativeURI.Contains(".html"))
            {
                code = StatusCode.BadRequest;
                request.fileRelativeURI = Configuration.BadRequestDefaultPageName;
            }

            //TODO: check for redirect
            String redirectedPhysicalPath = GetRedirectionPagePathIFExist(request.fileRelativeURI);
            if(redirectedPhysicalPath != string.Empty)
            {
                code = StatusCode.Redirect;
                request.fileRelativeURI = redirectedPhysicalPath;
            }

            //TODO: map the relativeURI in request to get the physical path of the resource.
            string physicalPath = Configuration.RootPath + "\\" + request.fileRelativeURI;


            //TODO: check file exists          
            if (!File.Exists(physicalPath)) {

                code = StatusCode.NotFound;
                request.fileRelativeURI = Configuration.NotFoundDefaultPageName;
                physicalPath = Configuration.RootPath + "\\" + request.fileRelativeURI;
            }


            Console.WriteLine("Physical path: " + physicalPath);

            byte[] fileData = new byte[1000];
            fileData = File.ReadAllBytes(physicalPath);
            content = Encoding.ASCII.GetString(fileData).Trim();
            
            return new Response(request.http, code, "text/html", content, redirectedPhysicalPath);
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            try
            {
                return Configuration.RedirectionRules[relativePath];
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return string.Empty;
            }


        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {

                // TODO: using the filepath paramter read the redirection rules from file 
                string[] fileData = File.ReadAllLines(filePath);

                foreach (string elem in fileData)
                {

                    string[] redrectString = elem.Split(',');
                    Configuration.RedirectionRules = new Dictionary<string, string>
                    {
                        { redrectString[0], redrectString[1] }
                    };
                }

            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
