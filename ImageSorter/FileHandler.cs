using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Drawing;
using ImageSorter;

public static class FileHandler
{
    

    public static void TransferImage(string imageFilePath, string destinationDirectory)
    {
        string destinationFolder = generateFullDestinationDirectory(imageFilePath, destinationDirectory);
        string fullDestinationPath = generateDestinationPath(destinationFolder, imageFilePath);

        File.Copy(imageFilePath, fullDestinationPath);

        //preserveDateModifiedData(imageFilePath, fullDestinationPath);
    }

    private static void preserveDateModifiedData(string originalImageFilePath, string copiedImageFilePath)
    {
        DateTime modifiedTime = determineLikelyCreationTime(new FileInfo(originalImageFilePath));

        FileInfo newFileInfo = new FileInfo(copiedImageFilePath);
        newFileInfo.IsReadOnly = false;
        newFileInfo.LastWriteTime = modifiedTime;
    }

    public static bool DetermineDuplicate(string imageFilePath, string destinationFolder)
    {
        bool result = false;

        string fullDestinationDirectory = generateFullDestinationDirectory(imageFilePath, destinationFolder);
        Directory.CreateDirectory(fullDestinationDirectory);

        string[] fileList = Directory.GetFiles(fullDestinationDirectory);

        FileInfo info = new FileInfo(imageFilePath);
        string path = Path.Combine(generateFullDestinationDirectory(imageFilePath,destinationFolder), info.Name);

        if (File.Exists(path))
        {
            if (determineIfDuplicateImages(imageFilePath, path))
            {
                result = true;
            }
        }

        if (result == false)
        {
            for (int i = 0; i < fileList.Length; i++)
            {
                if (checkFileIsImage(fileList[i]))
                {
                    if (determineIfDuplicateImages(imageFilePath, fileList[i]))
                    {
                        result = true;
                        break;
                    }
                }
            }
        }

        return result;
    }

    private static bool determineIfDuplicateImages(string image1, string image2)
    {
        bool result = false;

        
        Bitmap a = new Bitmap(image1);
        Bitmap b = new Bitmap(image2);

        if (ImageFingerprint.CheckForIdenticalImageFingerprint(a, b, 10))
        {
            if (ImageFingerprint.CheckForIdenticalImageFingerprint(a, b, 75))
            {
                result = true;

            }
        }

        a.Dispose();
        a = null;
        b.Dispose();
        b = null;

        return result;
    }

    

    /*
    public static bool DetermineDuplicate(string imageFilePath, string destinationFolder)
    {
        bool result = false;

        

        string[] fileList = Directory.GetFiles(fullDestinationDirectory);

        DateTime fileCreationTime = determineLikelyCreationTime(new FileInfo(imageFilePath));
        List<string> checkFurtherForDuplicatesList = new List<string>();


        if (fileList.Length > 0)
        {
            for (int i = 0; i < fileList.Length; i++)
            {
                FileInfo file = new FileInfo(fileList[i]);
                if (fileCreationTime == determineLikelyCreationTime(file))
                {
                    checkFurtherForDuplicatesList.Add(fileList[i]);
                } 

            }
        }

        if (checkFurtherForDuplicatesList.Count > 0)
        {
            result = checkFurtherForDuplicates(imageFilePath, checkFurtherForDuplicatesList);
        }
        
        checkFurtherForDuplicatesList = null;

        return result;
    }

    private static bool checkFurtherForDuplicates(string imageFilePath, List<string> checkFurtherForDuplicatesList)
    {
        bool result = false;

        foreach (string item in checkFurtherForDuplicatesList)
        {
            Image image = new Bitmap(imageFilePath);
            Image listedImage = new Bitmap(item);
            if (ImageFingerprint.CheckForIdenticalImageFingerprint(image, listedImage))
            {
                result = true;
                image.Dispose();
                image = null;
                listedImage.Dispose();
                listedImage = null;
                break;
            }
            image.Dispose();
            image = null;
            listedImage.Dispose();
            listedImage = null;
        }
       
        
        return result;
    }
    */
    public static List<string> PopulateImageList(string sourceFolderPath)
    {
        List<string> pendingPathList = findAllFiles(sourceFolderPath);
        List<string> imagePathList = new List<string>();
                
                
        foreach (string item in pendingPathList)
        {
            if (checkFileIsImage(item)) imagePathList.Add(item);
        }

        return imagePathList;
    }

    private static List<string> findAllFiles(string sourceFolderPath)
    {
        List<string> filePathList = new List<string>();

        foreach (string file in Directory.EnumerateFiles(sourceFolderPath,"*.*",SearchOption.AllDirectories))
        {
            filePathList.Add(file);
        }

        return filePathList;
    }

    private static List<string> convertStringArrayToList(string[] v)
    {
        List<string> list = new List<string>();

        for (int i = 0; i < v.Length; i++)
        {
            list.Add(v[i]);
        }

        return list;
    }

    private static bool checkFileIsImage(string filePath)
    {
        bool result = false;
        string extension = Path.GetExtension(filePath).ToUpper();
        List<string> filetypes = new List<string> {
            ".JPG",
            ".JPEG",
            ".JIF",
            ".JFIF",
            ".PNG",
            ".BMP",
            ".TIFF",
            ".TIF",
            ".GIF"};
        

        if (filetypes.Contains(extension))
        {
            result = true;
        }
        
        return result;
    }

    private static string generateFullDestinationDirectory(string imagePath, string destinationDirectory)
    {
        string directory = destinationDirectory;
        FileInfo currentFileInfo = new FileInfo(imagePath);
        DateTime creationTime = determineLikelyCreationTime(currentFileInfo);

        directory += "/" + creationTime.Year.ToString();
        directory += "/" + convertMonthNumberToName(creationTime.Month);

        return directory;
    }

    private static DateTime determineLikelyCreationTime(FileInfo fileInfo)
    {
        DateTime time = fileInfo.CreationTime;
        if (time > fileInfo.LastAccessTime)
        {
            time = fileInfo.LastAccessTime;
        }
        if (time > fileInfo.LastWriteTime)
        {
            time = fileInfo.LastWriteTime;
        }

        return time;
    }

    private static string generateDestinationPath(string destinationFolder, string imagePath)
    {
        FileInfo info = new FileInfo(imagePath);
        string originalName = Path.GetFileNameWithoutExtension(imagePath);
        string candidateName = originalName;
        int i = 1;

        while (File.Exists(Path.Combine(destinationFolder, candidateName) + info.Extension))
        {
            candidateName = originalName + " (" + i + ")";
            i++;
        }

        return Path.Combine(destinationFolder, candidateName) + info.Extension;
    }

    private static bool checkForFilenameClash(string destinationFolder, string filename)
    {
        string[] file = Directory.GetFiles(destinationFolder, filename);

        if (file == null) return false;
        else return true;
    }

    private static string convertMonthNumberToName(int monthNumber)
    {
        string name = Enum.GetName(typeof (MonthName), monthNumber);

        return name;
    }

    enum MonthName
    {
        January = 1,
        February,
        March,
        April, 
        May,
        June,
        July, 
        August, 
        September,
        October,
        November,
        December
    }

    enum FileExtension
    {
        JPG,
        JPEG,
        JIF,
        JFIF,
        PNG,
        BMP,
        TIFF,
        TIF,
        GIF,
    }
}
