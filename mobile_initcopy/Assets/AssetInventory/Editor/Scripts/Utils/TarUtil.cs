using System.IO;
using System.Text;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;

namespace AssetInventory
{
    public static class TarUtil
    {
        public static void ExtractGZ(string fileName, string destinationFolder)
        {
            Stream inStream = File.OpenRead(fileName);
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.Default);
            tarArchive.ExtractContents(destinationFolder);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }
    }
}