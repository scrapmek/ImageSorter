using System.IO;

namespace ImageSorter
{
    class ImageInfo
    {
        public FileInfo Info { get; set; }
        public long ImageHash { get; set; }

        public ImageInfo(string file, long hash)
        {
            this.Info = new FileInfo(file);
            this.ImageHash = hash;
        }
    }
}
