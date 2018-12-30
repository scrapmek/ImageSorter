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
            ImageInfoList = new List<ImageInfo>();

            //List<string> files = Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories).ToList();
            //List<string> images = Helpers.GetAllImageFiles(files);

            //foreach (string item in images)
            //{
            //    FileInfo info = new FileInfo(item);
            //    long hash = Helpers.GenerateImageHash(info);
            //    ImageInfoList.Add(new ImageInfo(info, hash));
            //}
            
        }

        public void AddNewImageInfo(FileInfo info)
        {
            long hash = Helpers.GenerateImageHash(info);
            ImageInfoList.Add(new ImageInfo(info, hash));

        }

        public bool CheckIfDuplicate(FileInfo file)
        {
            bool result = false;

            long perspectiveHash = Helpers.GenerateImageHash(file); 
            string perspectiveDestinationFolder = Helpers.GetDestinationDirectory(file, directory);

            // Add files from destination tree that havent already been added.

            if (Directory.Exists(perspectiveDestinationFolder)) {
                List<string> bla = Helpers.GetAllImageFiles(Directory.GetFiles(perspectiveDestinationFolder)?.ToList());
                IEnumerable<string> filesToAdd = bla.Where(x => !this.ImageInfoList.Select(y => y.Info.FullName).Contains(x));

                foreach (var item in filesToAdd)
                    AddNewImageInfo(new FileInfo(item));
            }

            
            if (ImageInfoList.Any(x => x.Info.DirectoryName == perspectiveDestinationFolder && x.ImageHash == perspectiveHash))
                    result = true;

            return result;
        }
    }
}
