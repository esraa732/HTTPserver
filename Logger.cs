using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        //static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            FileStream FS = new FileStream(@"LOCATIONPATH\HTTPServer\bin\Debug\log.txt", FileMode.OpenOrCreate);
            //Datetime:
            //message:
            StreamWriter FW = new StreamWriter(FS);
            FW.WriteLine("Time : " + DateTime.Now.ToString());
            FW.WriteLine("Message :" + ex.Message);
            FS.Close();
            // for each exception write its details associated with datetime 
        }
    }
}
