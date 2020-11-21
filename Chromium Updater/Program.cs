using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;

namespace Chromium_Updater
{
    class Program
    {
        static string ChromeDirectory = ".";
        static string LastUpdatedFile = "";

        static void Main(string[] args)
        {
            foreach (string s in args) Console.WriteLine(args[0]);
            const string url = "https://download-chromium.appspot.com/dl/Win_x64?type=snapshots";
            if (args.Length > 0)
            {
                try
                {
                    ChromeDirectory = Path.GetFullPath(args[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Supplied Chrome Directory is invalid");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press a key to exit.");
                    Console.ReadKey();
                    return;
                }
            }
            LastUpdatedFile = Path.GetFullPath(Path.Combine(ChromeDirectory, "chrome-win", "lastupdate.txt"));
            string upd = File.Exists(LastUpdatedFile) ? upd = File.ReadAllText(LastUpdatedFile) : "";
            Console.WriteLine(ChromeDirectory);
            Console.WriteLine(LastUpdatedFile);
            Console.ReadLine();

            if (upd == "" || DateTime.Parse(upd).Date < DateTime.Now.Date)
            {
                Console.WriteLine("time to update");
                if (Directory.Exists("chrome-win"))
                {
                    Console.WriteLine("deleting existing chrome");
                    Directory.Delete("chrome-win", true);
                }

                HttpWebRequest hreq = HttpWebRequest.CreateHttp(url);
                hreq.AllowAutoRedirect = true;
                hreq.Host = "download-chromium.appspot.com";
                hreq.KeepAlive = true;
                hreq.Headers.Add("Upgrade-Insecure-Requests", "1");
                hreq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3553.0 Safari/537.36";
                hreq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                hreq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                hreq.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");

                Console.WriteLine("requesting new chrome");

                using (HttpWebResponse hres = (HttpWebResponse)hreq.GetResponse())
                {
                    Console.WriteLine("reading response");
                    using (Stream sr = hres.GetResponseStream())
                    {
                        Console.WriteLine("extracting");
                        using (ZipArchive zip = new ZipArchive(sr))
                        {
                            zip.ExtractToDirectory(ChromeDirectory);
                        }
                    }
                }

                File.WriteAllText(Path.Combine(LastUpdatedFile), DateTime.Now.ToShortDateString());
            }

            Console.WriteLine("launching chrome");

            Process.Start(Path.Combine(ChromeDirectory, "chrome-win", "chrome.exe"));
        }
    }
}
