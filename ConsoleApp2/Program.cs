using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using Newtonsoft.Json;

namespace odevnet
{
    class Program
    {
        static void Main(string[] args)
        {
            var Ports = new List<PortForJson>();
            using (Process p = new Process())
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.Arguments = "-a -n";
                ps.FileName = "netstat.exe";
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                string exitStatus = p.ExitCode.ToString();

                if (exitStatus != "0")
                {

                }


                string[] rows = Regex.Split(content, "\r\n");
                foreach (var item in rows)
                {
                    if (item == "") continue;
                    if (item.Trim() == "Active Connections") continue;
                    string[] tokens = Regex.Split(item, "\\s+");
                    if (tokens[1].Contains("TCP"))
                    {
                        string yabanciAdres = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        Ports.Add(new PortForJson
                        {
                            ports = Convert.ToInt32(yabanciAdres.Split(':')[1]),
                            foreginAddress = tokens[3].ToString()
                        });
                    }

                }
            
             
            }
            for (int i = 0; i < Ports.Count; i++)
            {
                if (Ports[i].foreginAddress == "0.0.0.0:0" || Ports[i].foreginAddress == "[::]:0" || Ports[i].foreginAddress == "*:*")
                {
                    Ports.RemoveAt(i);
                }
            }
            var jsonData = JsonConvert.SerializeObject(Ports);

            string path = @"D:\Users\JsonDataFile.json"; //Dosya konumunu değiştiriniz.
           
            
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(jsonData);
            }

            Console.WriteLine(jsonData);
            Console.Read();
        }
        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }
    }

    public class Port
    {
        public string name
        {
            get
            {
                return string.Format("{0} ({1} port {2})", this.process_name, this.protocol, this.port_number);
            }
            set { }
        }
        public string port_number { get; set; }
        public string process_name { get; set; }
        public string protocol { get; set; }
    }
    public class PortForJson
    {
        public int ports;
        public string foreginAddress;
    }
}
