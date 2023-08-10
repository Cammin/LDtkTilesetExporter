using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Utf8Json;

namespace ExportTilesetDefinition
{
    public class Exporter
    {
        public void Start()
        {
            string dir = Environment.CurrentDirectory;
            Console.WriteLine($"Working from {dir}");

            bool gotOne = false;
            foreach (string projectName in GetOpenAppProjectNames())
            {
                string projectPath = TryProject(dir, projectName);
                if (projectPath != null)
                {
                    gotOne = true;
                    ProcessProject(projectPath);
                }
            }

            if (!gotOne)
            {
                Console.WriteLine("Didn't operate any files. Was this export app launched from the correct working directory?");
            }
        }
        
        public string TryProject(string dir, string projectName)
        {
            string path = Path.Combine(dir, projectName) + ".ldtk";

            if (!File.Exists(path))
            {
                Console.WriteLine($"No file: {path}");
                return null;
            }
            
            Console.WriteLine($"Got file! {path}");
            return path;
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

        public void ProcessProject(string projectPath)
        {
            LdtkJson json = null;
            
            Profiler.RunWithProfiling("Deserialize Project", () =>
            {
                byte[] bytes = File.ReadAllBytes(projectPath);
                json = JsonSerializer.Deserialize<LdtkJson>(bytes);
            });

            LDtkTilesetDefExporter exporter = new LDtkTilesetDefExporter(projectPath);
            exporter.ExportTilesetDefinitions(json);
        }
    }
}