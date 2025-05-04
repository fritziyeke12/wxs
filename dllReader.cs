using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MsiReader
{
    class Program
    {
        // 1) P/Invoke declarations
        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        static extern uint MsiOpenPackage(string szPackagePath, out IntPtr hPackage);

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        static extern uint MsiGetProperty(IntPtr hPackage, string szProperty, StringBuilder lpValueBuf, ref uint pcchValueBuf);

        [DllImport("msi.dll")]
        static extern uint MsiCloseHandle(IntPtr hAny);

        static void Main(string[] args)
        {
            // 2) Argument check
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: MsiReader.exe <path to .msi>");
                return;
            }

            string msiPath = args[0];

            // 3) Open the MSI package
            IntPtr hPackage;
            uint result = MsiOpenPackage(msiPath, out hPackage);
            if (result != 0)
            {
                Console.Error.WriteLine($"MsiOpenPackage failed (0x{result:X})");
                return;
            }

            // 4) Which properties to read
            string[] props = { "ProductName", "ProductVersion", "Manufacturer", "ProductCode" };

            // 5) Loop over desired properties
            foreach (var prop in props)
            {
                // a) Set up a buffer for the string
                uint size = 512;
                var sb = new StringBuilder((int)size);

                // b) Call into msi.dll to get the property
                uint res = MsiGetProperty(hPackage, prop, sb, ref size);

                // c) Check the return code
                if (res == 0)
                    Console.WriteLine($"{prop,-20} = {sb}");
                else
                    Console.WriteLine($"{prop,-20} = <error 0x{res:X}>");
            }

            // 6) Clean up
            MsiCloseHandle(hPackage);
        }
    }
}
