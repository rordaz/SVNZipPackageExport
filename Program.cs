using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;

namespace SVNCodePackageExport
{
    class Program
    {
        // example call
        // cd 
        // FluentCommandLineParser -s "http://YOUR-SVN-REPO-TEST-URL" - d "E:\Test\A\"
        static void Main(string[] args)
        {
            // create a generic parser for the ApplicationArguments type
            var parser = new FluentCommandLineParser<ApplicationArguments>();

            parser.Setup(arg => arg.SVNSource)
            .As('s', "svnSource")
            .SetDefault(@"http://YOUR-SVN-REPO-TEST-URL")
            .WithDescription("SVN Repository URL");

            parser.Setup(arg => arg.DestinationDir)
            .As('d', "destinationDirectory")
            .SetDefault(@"E:\Test")
            .WithDescription("Destination Directory");

            parser.Setup(arg => arg.SubDirectory)
            .As('f', "subDirectory")
            .SetDefault(@"Source_Code_Package")
            .WithDescription("Destination SubDirectory");

            parser.Setup(arg => arg.CleanDirectory)
            .As('c', "cleanDirectory")
            .SetDefault(false)
            .WithDescription("Delete Folders");

            parser.Setup(arg => arg.Zip)
            .As('z', "zip")
            .SetDefault(false)
            .WithDescription("Zip Export Files");

            var result = parser.Parse(args);


            if (result.HasErrors == false)
            {
                try
                {
                    var TimeStamp = DateTime.Now.ToString("yyyyMMdd");
                    //TimeStamp = "20200427";
                    var SVNExporterObj = new SVNExporter();
                    bool zipFiles = parser.Object.Zip;
                    bool CleanDirectory = parser.Object.CleanDirectory;

                    string DestinationDir = parser.Object.DestinationDir +"\\"+ parser.Object.SubDirectory;
                    string ZipFileName = parser.Object.SVNSource.Substring(parser.Object.SVNSource.LastIndexOf(@"/") + 1, parser.Object.SVNSource.Length - 1 - parser.Object.SVNSource.LastIndexOf(@"/"));
                    string SourceZipFile = DestinationDir + "_"+ TimeStamp + "\\" + ZipFileName;

                    if (CleanDirectory)
                    {
                        ZipUtil.CleanDirectory(SourceZipFile);
                    }
                    else {

                        if (SVNExporterObj.SVNExport(parser.Object.SVNSource, DestinationDir + "_" + TimeStamp, ZipFileName))
                        {
                            if (zipFiles)
                            {
                                ZipUtil.ZipFiles(SourceZipFile, SourceZipFile + ".zip", null);
                            }
                            Console.WriteLine("SNV Export: Completed");
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    throw;
                }
            }
            else
            {
                foreach (var item in result.Errors.ToList())
                {
                    Console.WriteLine("Parsing Errors: " + item.Option.ToString());
                    Console.WriteLine("Parsing Errors: " + item.ToString());
                }
                
            }

        }
    }


    public class ApplicationArguments
    {
        public string SVNSource { get; set; }
        public string DestinationDir { get; set; }
        public string SubDirectory { get; set; }
        public Boolean CleanDirectory { get; set; }
        public Boolean Zip { get; set; }
    }
}
