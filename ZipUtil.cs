using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;

namespace SVNCodePackageExport
{
    public static class ZipUtil
    {
        public static void ZipFiles(string inputFolderPath, string outputPathAndFile, string password = null, Boolean cleanDir = false)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            // find number of chars to remove     // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            //string outPath = inputFolderPath + @"\" + outputPathAndFile;
            string outPath = outputPathAndFile;

            Console.WriteLine("SNV Export: Ziping Files");
            // create zip stream
            using (ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)))
            {
                if (password != null && password != String.Empty)
                    oZipStream.Password = password;
                oZipStream.SetLevel(7); // maximum compression
                ZipEntry oZipEntry;
                foreach (string Fil in ar) // for each file, generate a zipentry
                {
                    oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                    oZipStream.PutNextEntry(oZipEntry);

                    if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                    {
                        ostream = File.OpenRead(Fil);
                        obuffer = new byte[ostream.Length];
                        ostream.Read(obuffer, 0, obuffer.Length);
                        oZipStream.Write(obuffer, 0, obuffer.Length);
                    }
                }
                oZipStream.Finish();
                oZipStream.Close();
                Console.WriteLine("SNV Export: Files successfully compressed in Zip Files");
            }
            if (cleanDir)
            {
                ZipUtil.ClearAttributes(inputFolderPath);
                ZipUtil.CleanDirectory(inputFolderPath);
            }
        }

        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            bool Empty = true;
            foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                if (Directory.GetDirectories(Dir).Length == 0)
                // if directory is completely empty, add it
                {
                    fils.Add(Dir + @"/");
                }
            }

            foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    fils.Add(obj);
                }
            }
            return fils; // return file list
        }

        public static void UnZipFiles(string zipPathAndFile, string outputFolder, string password, bool deleteZipFile)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipPathAndFile));
            if (password != null && password != String.Empty)
                s.Password = password;
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = outputFolder;
                string fileName = Path.GetFileName(theEntry.Name);
                // create directory 
                if (directoryName != "")
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (fileName != String.Empty)
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace("\\ ", "\\");
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                        FileStream streamWriter = File.Create(fullPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();
            if (deleteZipFile)
                File.Delete(zipPathAndFile);
        }

        public static void CleanDirectory(string DirectoryPath)
        {
  
            if (Directory.Exists(DirectoryPath))
            {
                Console.WriteLine("SNV Export: Removing Directories");
                try
                {
                    Directory.Delete(DirectoryPath, true);
                }
                catch (Exception e)
                {
                        Console.WriteLine("SNV Export: " + e.Message);
                        //throw;
                }
                Console.WriteLine("SNV Export: Removing Directories Successfully");
            }
        }

        public static void CleanDirectory(string DirectoryPath, bool Retry)
        {
            if (Retry)
            {
                int retryMax = 5;
                const int DelayOnRetry = 1000;
                for (int i = 1; i <= retryMax; ++i)
                {
                    try
                    {
                        // Do stuff with file
                        Directory.Delete(DirectoryPath, true);
                        break; // When done we can break loop
                    }
                    catch (IOException e) when (i <= retryMax)
                    {
                        // You may check error code to filter some exceptions, not every error
                        // can be recovered.
                        Thread.Sleep(DelayOnRetry);
                    }
                }
            }
            else {
                CleanDirectory(DirectoryPath);
            }
            
            
            
        }

        public static void ClearAttributes(string DirectoryPath)
        {
            if (Directory.Exists(DirectoryPath))
            {
                File.SetAttributes(DirectoryPath, FileAttributes.Normal);

                string[] subDirs = Directory.GetDirectories(DirectoryPath);
                foreach (string dir in subDirs)
                {
                    ClearAttributes(dir);
                }

                string[] files = files = Directory.GetFiles(DirectoryPath);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
            }
        }
    }
}
