using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace SVNCodePackageExport
{
    class SVNExporter
    {
        public SVNExporter()
        {

        }

        // SVN Command to Export the Source Code
        public Boolean SVNExport(string SVNSource, string DestinationDir, string ZipFileName) 
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("--------------");

                SvnClient svnClient = new SvnClient();
                SvnUriTarget target = new SvnUriTarget(SVNSource);
                SvnExportArgs exportArgs = new SvnExportArgs();
                exportArgs.Overwrite = true;
                SvnUpdateResult svnUpdateResult;

                string FullDestination = DestinationDir + "\\" + ZipFileName;

                //Console.WriteLine("SVNSource: " + SVNSource);
                //Console.WriteLine("DestinationDir: " + DestinationDir);
                Console.WriteLine("SNV Export: Export in progress for " + ZipFileName);


                if (svnClient.Export(target, FullDestination, exportArgs, out svnUpdateResult))
                {
                    Console.WriteLine("SNV Export: Successful Revision: " + svnUpdateResult.Revision);
                    //Console.ReadKey();
                    return true;
                }
                else {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SNV Export: Exported Failed " + e.Message);
                //Console.ReadKey();
                return false;
            }
        }

        public void Notifier(string message)
        { 
        
        }

    }
}
