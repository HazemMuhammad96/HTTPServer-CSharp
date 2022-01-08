using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
       
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        
        List<string> headerLines = new List<string>();
        public Response(string http , StatusCode code, string contentType, string content, string redirectoinPath)
        {
            
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string statusLine = GetStatusLine(code);
            ;

            //HTTP/1.1
            responseString = http + " " + statusLine + " " + code + "\r\nContent_Type:" + contentType + "\r\nContent_Length:" + content.Length  + "\r\nDate:" + DateTime.Now + "\r\n" + "\r\n";
            if (content != "")
            {
                responseString = responseString + content;
            }

            Console.WriteLine("Code: " + code);
            // TODO: Create the request string
            if (redirectoinPath != "") { responseString = responseString + "Redirected To : " + redirectoinPath; }


        }

        private string GetStatusLine(StatusCode code)
        {
            switch(code)
            {
                case StatusCode.OK: return "200";
                case StatusCode.NotFound: return "404";
                case StatusCode.BadRequest: return "400";
                case StatusCode.Redirect: return "301";
                case StatusCode.InternalServerError: return "500";
                default: return "500";

            }
        }
    }
}
