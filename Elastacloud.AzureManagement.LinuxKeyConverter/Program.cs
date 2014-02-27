using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.LinuxKeyConverter;
using Elastacloud.AzureManagement.ScriptMapper.Linux;

namespace Elastacloud.Fluent.LinuxKeyConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            const string name = @"C:\Projects\cert_export\C3F7DB78D34D5702750BF80239903FFAEE88941E";
            var converter = new SSHKeyConverter(name + ".pem", "password");
            converter.Convert();

            string conversion;
            using (var reader = new StreamReader(name + ".pvk"))
            {
                conversion = reader.ReadToEnd();
            }

            var session = new LinuxSession(args[0], 22, args[1], "password", true, name + ".pvk");
            session.CreateSession();
            string response = session.ExecuteCommand("ls -alt");
            Console.WriteLine("Awesome response: {0}", response);
        }
    }
}
