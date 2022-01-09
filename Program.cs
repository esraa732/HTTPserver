using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            String filepath = @"LOCATIONPATH\HTTPServer\bin\Debug\redirectionRules.txt";


            // 1) Make server object on port 1000
            Server httpserver = new Server(1000, filepath);
            // 2) Start Server
            
            httpserver.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream fs = new FileStream(@"LOCATIONPATH\HTTPServer\bin\Debug\redirectionRules.txt", FileMode.CreateNew);
            StreamWriter fw = new StreamWriter(fs);
            fw.WriteLine(@"aboutus.html,aboutus2.html");
            fw.Close();
        }
         
    }
}
