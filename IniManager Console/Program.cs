using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Ini;
using System.IO;
using System.Windows.Forms;

namespace IniManager_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string ini = Path.Combine(Application.StartupPath, "test.ini");
            IniManager iniHelper = new IniManager(ini);

            Stopwatch sw = new Stopwatch();

            /* =================================================== */
            sw.Restart();

            for (int i = 0; i < 100; i++)
            {
                iniHelper.Load();
                Console.WriteLine("Loading:".PadRight(28) + " {0} ms", sw.ElapsedMilliseconds);
                sw.Restart();
            }

            sw.Stop();
            /* =================================================== */

            Console.WriteLine("Press any key to test adding.");
            Console.ReadKey();

            /* =================================================== */
            sw.Restart();

            for (int i = 0; i < 25000; i++)
            {
                iniHelper.GetSection("HEADER").Add(IniType.Comment, "Test Comment");
            }

            sw.Stop();

            Console.WriteLine("Inserted 25,000 comments:".PadRight(28) + " {0} ms", sw.ElapsedMilliseconds);
            /* =================================================== */


            /* =================================================== */
            sw.Restart();

            for (int i = 0; i < 25000; i++)
            {
                iniHelper.Put("Testing", "k" + i, "v" + i);
            }

            sw.Stop();

            Console.WriteLine("Inserted 25,000 properties:".PadRight(28) + " {0} ms", sw.ElapsedMilliseconds);
            /* =================================================== */


            /* =================================================== */
            sw.Restart();

            iniHelper.Save();

            sw.Stop();

            Console.WriteLine("Saving:".PadRight(28) + " {0} ms", sw.ElapsedMilliseconds);
            /* =================================================== */

            Console.ReadKey();
        }
    }

    class Test
    {
        public string Section { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public Test(string section, string key, string value)
        {
            this.Section = section;
            this.Key = key;
            this.Value = value;
        }
    }
}