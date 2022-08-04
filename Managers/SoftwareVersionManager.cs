using HousePartyTranslator.Helpers;
using SevenZipNET;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    static class SoftwareVersionManager
    {
        public static readonly string LocalVersion = "0.5.3.1";
        public static string LatestGithubVersion;
        public static bool UpdatePending = false;
        private static readonly HttpClient client = new HttpClient();
        const string APIUrl = "https://api.github.com/repos/CamelCaseName/HousePartyTranslator/releases/latest";

        /// <summary>
        /// Download and replaces the running application if necessary
        /// </summary>
        public static void ReplaceFileIfNew()
        {
            //modify client signatures
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
            if(t.Length > 4) LatestGithubVersion = $"{t[0]}.{t[2]}.{t[3]}.{t[4]}";

            //path to file if we need it
            string releaseFile = Path.Combine(Directory.GetCurrentDirectory(), "Release.7z");
            string releaseFolder = Directory.GetCurrentDirectory();

            //delete old one if it exists
            string oldExe = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "prev.exe");
            if (File.Exists(oldExe)) File.Delete(oldExe);

            //if the version on github has a higher version number
            UpdatePending = LatestGithubVersion[0] > LocalVersion[0]/*major version*/
                || (LatestGithubVersion[0] == LocalVersion[0] && LatestGithubVersion[2] > LocalVersion[2])/*minor version*/
                || (LatestGithubVersion[0] == LocalVersion[0] && LatestGithubVersion[2] == LocalVersion[2] && LatestGithubVersion[4] > LocalVersion[4])/*major release number*/
                || (LatestGithubVersion[0] == LocalVersion[0] && LatestGithubVersion[2] == LocalVersion[2] && LatestGithubVersion[4] == LocalVersion[4] && LatestGithubVersion[6] > LocalVersion[6]);/*minor release number*/
            if (UpdatePending)
            {
                if (MessageBox.Show("A new version is available to download. Do you want to automatically update this installation?\n\n CHANGELOG:\n" + response.Body, "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    try
                    {
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
                    }
                    catch (System.UnauthorizedAccessException e)
                    {
                        LogManager.LogEvent(e.ToString());
                        MessageBox.Show($"The update failed because the program could not access\n   {oldExe}\n or the folder it is in.", "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //move currently running exe out of the way
                    File.Move(Application.ExecutablePath, oldExe);

                    //extract file to our current location and replace
                    SevenZipExtractor extractor = new SevenZipExtractor(releaseFile);
                    try {
                        extractor.ExtractAll(releaseFolder, true, true); 
                    }
                    catch (System.ComponentModel.Win32Exception e)
                    {
                        LogManager.LogEvent(e.ToString());
                        //move currently running back because something broke
                        File.Move(oldExe, Application.ExecutablePath);
                        if (File.Exists(oldExe)) File.Delete(oldExe);
                        MessageBox.Show($"The update failed because the program could not access\n   {releaseFolder}\n or the folder it is in.", "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //inform user
                    MessageBox.Show("Successfully updated the program! It will close itself now", "Update successful", MessageBoxButtons.OK);

                    //exit
                    Application.Exit();
                }
            }
        }
    }
}
