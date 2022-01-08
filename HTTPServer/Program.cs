using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();

            // 1) Make server object on port 1000
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "redirectionRules.txt");

            Server server = new Server(1000, path);

            // 2) Start Server
            server.StartServer(100);
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            StreamWriter streamWriter = new StreamWriter(File.Open("redirectionRules.txt", FileMode.Create));
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            streamWriter.Write("aboutus.html,aboutus2.html");

            // means that when making request to aboustus.html,, it redirects me to aboutus2
            streamWriter.Close();
        }
         
    }
}
