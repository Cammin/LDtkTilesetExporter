using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ExportTilesetDefinition
{
    public class Exporter
    {
        public void Start()
        {
            string dir = Environment.CurrentDirectory;
            Console.WriteLine($"Working from {dir}");
            
            foreach (string projectName in GetOpenAppProjectNames())
            {
                TryProject(dir, projectName);
            }
        }
        
        public void TryProject(string dir, string projectName)
        {
            string path = Path.Combine(dir, projectName) + ".ldtk";

            if (!File.Exists(path))
            {
                Console.WriteLine($"No file: {path}");
                return;
            }
            
            Console.WriteLine($"Got file! {path}");
        }

        public List<string> GetOpenAppProjectNames()
        {
            List<string> projects = new List<string>();
            
            Process[] processes = Process.GetProcessesByName("LDtk");
            foreach (var process in processes)
            {
                string title = process.MainWindowTitle;
                if (string.IsNullOrEmpty(title))
                {
                    continue;
                }
                
                string[] tokens = title.Split(' ');
                projects.Add(tokens[0]);
            }

            if (projects.Count == 0)
            {
                throw new FileNotFoundException("Couldn't find an active LDtk app to get the project names from");
            }

            return projects;
        }

        
    }
}