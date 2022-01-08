using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        NONE,
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        NONE,
        HTTP10, 
        HTTP11,
        HTTP09
    }
    
    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string fileRelativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();
        public string http;
        public HTTPVersion httpVersion;
        public string requestString;
        string contentLines;
        string contentType;
        int contentLineStartingIndex;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }
        
        public RequestMethod Method
        {
            get { return method;}
        }

        public string ContentLines
        {
            get { return contentLines; }
        }

        public string ContentType
        {
            get { return contentType;}
        }

        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            

            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] stringSeparators = new string[] { "\r\n" };
            requestLines = requestString.Split(stringSeparators, StringSplitOptions.None);


            foreach (var i in requestLines)
            {
                Console.WriteLine(i);
            }

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length >= 3)
            {
                //ValidateIsURI(relativeURI);

                // Parse Request line
                bool isRequestLineParsed = ParseRequestLine();               
                // Validate blank line exists
                // Load header lines into HeaderLines dictionary
                bool isHeaderLinesLoaded = LoadHeaderLines();

                /*if (validateBlankLines())
                {

                }*/

                bool isContentLinesLoaded = LoadContentLines();


                
                return isRequestLineParsed && isHeaderLinesLoaded && isContentLinesLoaded;
            }

            return false;
        }

        private RequestMethod ParseMethod(string methodString)
        {
            switch(methodString)
            {
                case "GET":
                    return RequestMethod.GET;
                    
                case "POST":
                    return RequestMethod.POST;
                    
                case "HEAD":
                    return RequestMethod.HEAD;

                default:
                    return RequestMethod.NONE;
            }
            
        }

        private string parseFileURI(string uri)
        {
            if (ValidateIsURI(uri))
            {
                return uri.Split('/')[1];
            }
            return "";
        }

        private HTTPVersion parseHTTPVersion(string httpString)
        {
            http = httpString.ToUpper();
            switch(httpString.ToLower())
            {
                case "http/0.9":
                    return HTTPVersion.HTTP09;

                case "http/1.0":
                    return HTTPVersion.HTTP10;

                case "http/1.1":
                    return HTTPVersion.HTTP11;

                default:
                    return HTTPVersion.NONE;
            }
        }
        private bool ParseRequestLine()
        {
            try
            {
                string[] parts = requestLines[0].Split(' ');


                method = ParseMethod(parts[0].Trim());
                fileRelativeURI = parseFileURI(parts[1].Trim());

                httpVersion = parseHTTPVersion(parts[2].Trim());

                if (httpVersion == HTTPVersion.NONE && method == RequestMethod.NONE) return false;

                
            }
            catch { return false; }


            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            try
            {
               
                for (int i = 1; i<requestLines.Length; i++)
                {
                    if (requestLines[i] == "")
                    {
                        break;
                    }

                        
                    
                    string[] splitted = requestLines[i].Split(':');
                    if(splitted[0] == "content-type")
                    {
                        contentType = splitted[1];
                    }
                    headerLines.Add(splitted[0], splitted[1]);
                    
                }

            }
            catch 
            {
                return false;
            }

            return true;
        }

        private bool LoadContentLines()
        {
            try
            {
                
                contentLineStartingIndex = requestLines.Length - 1;
                if (requestLines[contentLineStartingIndex] != null)
                {
                   
                    contentLines = requestLines[contentLineStartingIndex].Trim();
                    Console.WriteLine(contentLines);
                }
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }

        private bool validateBlankLines()
        {
            for (int i = 0; i < requestLines.Length; i++)
            {
                if (requestLines[i] == "") { return false; }
            }
            return true;
        }


    }


}
