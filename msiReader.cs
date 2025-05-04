using System;
using System.Runtime.InteropServices;
using WindowsInstaller;

namespace MsiReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string msiPath = @"C:\Path\To\Your\Installer.msi";

            try
            {
                Type installerType = Type.GetTypeFromProgID("WindowsInstaller.Installer");
                Installer installer = (Installer)Activator.CreateInstance(installerType);

                Database database = installer.OpenDatabase(msiPath, MsiOpenDatabaseMode.msiOpenDatabaseModeReadOnly);

                Console.WriteLine("Reading MSI Properties...\n");

                string[] propertyNames = { "ProductName", "ProductVersion", "Manufacturer", "ProductCode" };

                foreach (var property in propertyNames)
                {
                    string value = GetMsiProperty(database, property);
                    Console.WriteLine($"{property}: {value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading MSI: " + ex.Message);
            }
        }

        static string GetMsiProperty(Database db, string property)
        {
            string sql = $"SELECT `Value` FROM `Property` WHERE `Property` = '{property}'";
            View view = db.OpenView(sql);
            view.Execute(null);

            Record record = view.Fetch();
            if (record != null)
            {
                return record.get_StringData(1);
            }

            return "N/A";
        }
    }
}
