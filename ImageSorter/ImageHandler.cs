using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageSorter
{
    class ImageHandler : IDisposable
    {
        public string RootDestinationDirectory { get; set; }
        public string InputFile { get; set; }

         

        public ImageHandler(string input, string output)
        {
            InputFile = input;
            RootDestinationDirectory = output;
        }

        public bool Transfer()
        {
            bool result = false;
            if (checkDirectoryExists(InputFile))
            {
                if (!checkIfImageIsDuplicate(InputFile))
                {
                    string fileDestinationPath = createUniqueFilePath(InputFile);
                    File.Copy(InputFile, fileDestinationPath);
                    result = true;
                }
                else result = false;
            }
            else
            {
                string fileDestinationPath = createUniqueFilePath(InputFile);
                Directory.CreateDirectory(Helpers.GenerateDestinationDirectory(new FileInfo(InputFile), RootDestinationDirectory));
                File.Copy(InputFile, fileDestinationPath);
                result = true;
            }

            return result;


        }

        private bool checkIfImageIsDuplicate(string path)
        {
            bool result = false;
            long originalHash = Helpers.GenerateImageHash(path);
            FileInfo info = new FileInfo(path);
            string directory = Helpers.GenerateDestinationDirectory(info, RootDestinationDirectory);
            List<string> files = Directory.EnumerateFiles(directory).ToList();
            List<string> images = new List<string>();
            foreach (string item in files)
            {
                if (Helpers.CheckFileIsImage(item))
                    images.Add(item);
            }

            List<long> hashes = new List<long>();

            foreach (string item in images)
            {
                hashes.Add(Helpers.GenerateImageHash(item));
            }

            if (hashes.Contains(originalHash))
                result = true;

            return result;

        }

       

        private string createUniqueFilePath(string item)
        {
            FileInfo info = new FileInfo(item);
            string originalName = Path.GetFileNameWithoutExtension(item);
            string perspectiveName = originalName;
            string directory = Helpers.GenerateDestinationDirectory(info, RootDestinationDirectory);

            int i = 1;

            while (File.Exists(Path.Combine(directory, perspectiveName) + info.Extension))
            {
                perspectiveName = originalName + " (" + i + ")";
                i++;
            }

            return Path.Combine(directory, perspectiveName) + info.Extension;
        }

        private bool checkDirectoryExists(string path)
        {
            bool result = false;
            FileInfo info = new FileInfo(path);

            if (Directory.Exists(Helpers.GenerateDestinationDirectory(info, RootDestinationDirectory)))
                result = true;

            return result;
        }

        

        

        private string convertMonthNumberToName(int monthNumber)
        {
            string name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber);

            return name;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
