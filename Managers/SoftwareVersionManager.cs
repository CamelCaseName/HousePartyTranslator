using HousePartyTranslator.Helpers;
using System.IO;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SevenZipNET;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    static class SoftwareVersionManager
    {
        public static readonly string LocalVersion = "0.3.4.0";
        public static string LatestGithubVersion;
        public static bool UpdatePending = false;
        private static readonly HttpClient client = new HttpClient();
        const string APIUrl = "https://api.github.com/repos/CamelCaseName/HousePartyTranslator/releases/latest";

        public static void ReplaceFileIfNew()
        {
            //modify client signatures
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "House Party Translator update service");

            //offload async context
            DoWork();
        }

        private static async void DoWork()
        {
            //get data from github about the packages 
            GithubResponse response = await JsonSerializer.DeserializeAsync<GithubResponse>(await client.GetStreamAsync(APIUrl));

            //extract version number
            string t = response.TagName;
            LatestGithubVersion = $"{t[0]}.{t[2]}.{t[3]}.0";

            //path to file if we need it
            string releaseFile = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Release.7z");
            string releaseFolder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

            //if the version on github has a higher version number
            if (LatestGithubVersion[0] > LocalVersion[0]
                || LatestGithubVersion[2] > LocalVersion[2]
                || LatestGithubVersion[4] > LocalVersion[4]
                || LatestGithubVersion[6] > LocalVersion[6])
            {
                if (MessageBox.Show("A new version is available to download. Do you want to automatically update this installation?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    UpdatePending = true;

                    //get stream to the file in web location
                    using (Stream stream = await client.GetStreamAsync(response.Assets[0].BrowserDownloadUrl))
                    {
                        //stream to the file on dis
                        using (FileStream fileStream = new FileStream(releaseFile, FileMode.Create))
                        {
                            //copy data to file on disk
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                    //delete old one if it exists
                    string oldExe = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "prev.exe");
                    if (File.Exists(oldExe)) File.Delete(oldExe); ;

                    //move currently running exe out of the way
                    File.Move(Application.ExecutablePath, oldExe);

                    //extract file to our current location and replace
                    SevenZipExtractor extractor = new SevenZipExtractor(releaseFile);
                    extractor.ExtractAll(releaseFolder, true, true);

                    //inform user
                    MessageBox.Show("Successfully updated the program! It will close itself now", "Update successful", MessageBoxButtons.OK);

                    //exit
                    Application.Exit();
                }
            }
        }
    }
}
