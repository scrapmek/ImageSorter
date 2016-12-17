using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageSorter
{
    class HashChecker
    {
        public List<ImageInfo> ImageInfoList { get; set; }
        private string directory { get; set; }

        public HashChecker(string rootDirectory)
        {
            directory = rootDirectory;

            List<string> files = Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories).ToList();
            List<string> images = Helpers.GetAllImageFiles(files);

            foreach (string item in images)
            {
                long hash = Helpers.GenerateImageHash(item);
                ImageInfoList.Add(new ImageInfo(item, hash));
            }
            
        }

        public void AddNewImageInfo(string path, long hash)
        {
            ImageInfoList.Add(new ImageInfo(path, hash));
        }

        public bool CheckIfDuplicate(string path)
        {
            bool result = false;

            FileInfo perspectiveInfo = new FileInfo(path);
            long perspectiveHash = Helpers.GenerateImageHash(path); 
            string perspectiveDestinationFolder = Helpers.GenerateDestinationDirectory(perspectiveInfo, directory);

            foreach (ImageInfo item in ImageInfoList)
            {
                if (perspectiveDestinationFolder == item.Info.DirectoryName && perspectiveHash == item.ImageHash)
                {
                    result = true;
                    break;
                }

            }

            return result;
        }
    }
}
