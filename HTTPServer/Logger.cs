using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
            
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            string date = DateTime.Now.ToString();
            //message:
            string details = ex.Message;
            // for each exception write its details associated with datetime 
            sr.WriteLine( "The erorr is at time  " + date+"With details "  +  details);
            Debug.WriteLine("The erorr is at time  " + date + "With details " + details);

        }
    }
}
