﻿using System;
using System.Globalization;
using System.IO;

namespace ImageSorter
{
    class ImageHandler 
    {
        private string RootDestinationDirectory { get; set; }
        public HashChecker Checker { get; set; }

        public ImageHandler(string output)
        {
            RootDestinationDirectory = output;
            Checker = new HashChecker(RootDestinationDirectory);
        }

        public bool Transfer(FileInfo info)
        {
            bool result = false;

            if (checkDestinationDirectoryExists(info))
            {

                if (!Checker.CheckIfDuplicate(info))
                {
                    string fileDestinationPath = createUniqueDestinationPath(info);
                    File.Copy(info.FullName, fileDestinationPath);

                    FileInfo newFileInfo = new FileInfo(fileDestinationPath);
                    Checker.AddNewImageInfo(newFileInfo);
                    Helpers.preserveCreationTime(info, newFileInfo);

                    result = true;
                }
                else result = false;
            }
            else
            {
                string fileDestinationPath = createUniqueDestinationPath(info);
                Directory.CreateDirectory(Helpers.GetDestinationDirectory(info, RootDestinationDirectory));
                File.Copy(info.FullName, fileDestinationPath);


                FileInfo newFileInfo = new FileInfo(fileDestinationPath);
                Checker.AddNewImageInfo(newFileInfo);
                Helpers.preserveCreationTime(info, newFileInfo);

                result = true;
            }

            return result;


        }

        private string createUniqueDestinationPath(FileInfo info)
        {

            string originalName = Path.GetFileNameWithoutExtension(info.FullName);
            string perspectiveName = originalName;
            string directory = Helpers.GetDestinationDirectory(info, RootDestinationDirectory);

            int i = 1;

            while (File.Exists(Path.Combine(directory, perspectiveName) + info.Extension))
            {
                perspectiveName = originalName + " (" + i + ")";
                i++;
            }

            return Path.Combine(directory, perspectiveName) + info.Extension;
        }

        private bool checkDestinationDirectoryExists(FileInfo info)
        {
			return Directory.Exists(Helpers.GetDestinationDirectory(info, RootDestinationDirectory));
        }
    }
}
