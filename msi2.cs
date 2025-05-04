using System;
using Microsoft.Deployment.WindowsInstaller;

namespace MsiReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: MsiReader.exe <path to .msi>");
                return;
            }

            string msiPath = args[0];
            try
            {
                // Open in read-only mode
                using (var db = new Database(msiPath, DatabaseOpenMode.ReadOnly))
                {
                    Console.WriteLine($"Properties in {msiPath}:\n");

                    // Query all entries in the Property table
                    using (var view = db.OpenView("SELECT `Property`, `Value` FROM `Property`"))
                    {
                        view.Execute();

                        Record rec;
                        while ((rec = view.Fetch()) != null)
                        {
                            // rec[“Property”] and rec[“Value”] pull columns by name
                            string propName  = rec["Property"].ToString();
                            string propValue = rec["Value"].ToString();
                            Console.WriteLine($"{propName,-20} = {propValue}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading MSI: {ex.Message}");
            }
        }
    }
}
