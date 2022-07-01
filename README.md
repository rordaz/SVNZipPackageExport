# SVNCodePackageExport

## Description

CLI to export SVN Repositories to a dated folder, zip the content and remove the folder.

## Libraries used

+ ICSharpCode.SharpZipLib.Zip
+ SharpSvn
+ FluentCommandLineParser

## Usage

```bash
SVNCodePackageExport -s [REPOSITORY_URL] -d [DESTINATION_FOLDER] -f [SUFFIX_FOLDER_NAME] --zip
```

## Parameters

[string]

**-s, -svnSource:** SVN Repository URL

[string]

**-d, -destinationDirectory:** Destination Directory Name

[string]

**-f, -subDirectory:** Destination SubDirectory Name

[boolean]

Default Value: False

**-c, -cleanDirectory:** Delete temporary source folders

[boolean]

Default Value: False

**-z,-zip:** Zip Directories