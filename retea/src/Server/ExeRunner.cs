using System;
using System.Diagnostics;
using System.IO;

namespace Server
{
    public sealed class ExeRunner
    {
        private static readonly Lazy<ExeRunner> instance = new Lazy<ExeRunner>(() => new ExeRunner());

        private ExeRunner()
        {
            string[] relativePaths = new string[]
            {
                @"..\..\..\..\..\..\..\..\DUNE\IP-2A3-2024-API\serialization_deserialization_testing\bin\Debug\net8.0\testing.exe",
                @"..\..\..\..\..\src\GUIClient\bin\Debug\net8.0\GuiClient.exe"
            };

            foreach (string relativePath in relativePaths)
            {
                string absolutePath = Path.GetFullPath(relativePath);
                if (File.Exists(absolutePath))
                {
                    try
                    {
                        var process = new Process();
                        process.StartInfo.FileName = absolutePath;
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("The .exe file doesn't exist");
                }
            }
        }

        public static ExeRunner Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}
