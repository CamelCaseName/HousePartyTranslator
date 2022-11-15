using Translator.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;

namespace Translator.Managers
{
    internal static class SoftwareVersionManager
    {
        public const string LocalVersion = "0.6.2.0";
        public static string? LatestGithubVersion;
        public static bool UpdatePending = false;
        private static readonly HttpClient client = new();
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
            try
            {
                //get data from github about the packages 
                GithubResponse? response = await JsonSerializer.DeserializeAsync<GithubResponse>(await client.GetStreamAsync(APIUrl));

                //check version and for need of update
                if (!UpdateNeeded(response?.TagName ?? "0.0.0.0")) return;

                //prepare files
                (bool successfull, string newFile, string oldFile) = CreateFiles();
                if (!successfull) return;

                //inform rest of program
                UpdatePending = true;

                if (Msg.InfoYesNoB("A new version is available to download. Do you want to automatically update this installation?\n\n CHANGELOG:\n" + response?.Body, "Update - " + response?.Name))
                {
                    if(response == null || response?.Assets?.Count < 1) throw new NullReferenceException();

                    Download(response?.Assets?[0]?.BrowserDownloadUrl ?? "", newFile);

                    if (!UpdateFile(oldFile, newFile)) return;

                    //inform user
                    _ = Msg.InfoOk("Successfully updated the program! It will close itself now", "Update successful");

                    //exit
                    UIHandler.SignalAppExit();
                }
            }
            catch(Exception e)
            {
                LogManager.Log(e.Message, LogManager.Level.Error);
                LogManager.Log("Self update failed.", LogManager.Level.Warning);
            }
        }

        //returns true if current version is up to date
        private static bool UpdateNeeded(string githubVersion)
        {
            //extract version number
            LatestGithubVersion = $"{githubVersion[0]}.{githubVersion[2]}.0.0";
            if (githubVersion.Length > 3) LatestGithubVersion = $"{githubVersion[0]}.{githubVersion[2]}.{githubVersion[3]}.0";
            if (githubVersion.Length > 4) LatestGithubVersion = $"{githubVersion[0]}.{githubVersion[2]}.{githubVersion[3]}.{githubVersion[4]}";

            //if the version on github has a higher version number
            return LatestGithubVersion[0] > LocalVersion[0]/*major version*/
                || (LatestGithubVersion[0] == LocalVersion[0] && LatestGithubVersion[2] > LocalVersion[2])/*minor version*/
                || (LatestGithubVersion[0] == LocalVersion[0] && LatestGithubVersion[2] == LocalVersion[2] && LatestGithubVersion[4] > LocalVersion[4])/*major release number*/
                || (LatestGithubVersion[0] == LocalVersion[0] && LatestGithubVersion[2] == LocalVersion[2] && LatestGithubVersion[4] == LocalVersion[4] && LatestGithubVersion[6] > LocalVersion[6]);/*minor release number*/
        }

        private static (bool, string, string) CreateFiles()
        {
            try
            {
                //path to file if we need it
                string releaseFile = Path.Combine(Directory.GetCurrentDirectory(), "Release.7z");

                //delete old one if it exists
                string oldFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "prev.exe");
                if (File.Exists(oldFile)) File.Delete(oldFile);
                return (true, releaseFile, oldFile);
            }
            catch (Exception e)
            {
                LogManager.Log(e.Message);
            }
            return (false, "", "");
        }

        private static async void Download(string downloadUrl, string newFile)
        {
            try
            {
                //get stream to the file in web location
                using Stream stream = await client.GetStreamAsync(downloadUrl);
                //stream to the file on dis
                using var fileStream = new FileStream(newFile, FileMode.Create);
                //copy data to file on disk
                await stream.CopyToAsync(fileStream);
            }
            catch (System.UnauthorizedAccessException e)
            {
                LogManager.Log(e.ToString(), LogManager.Level.Error);
                _ = Msg.ErrorOk($"The update failed because the program could not access\n   {newFile}\n or the folder it is in.", "Update failed");
                return;
            }
        }

        private static bool UpdateFile(string oldFile, string newFile)
        {

            //move currently running exe out of the way
            File.Move(Application.ExecutablePath, oldFile);

            //extract file to our current location and replace
            var extractor = new SevenZipExtractor.ArchiveFile(newFile);
            try
            {
                extractor.Extract(Directory.GetCurrentDirectory(), true);
                return true;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                LogManager.Log(e.ToString(), LogManager.Level.Error);
                //move currently running back because something broke
                File.Move(oldFile, Application.ExecutablePath);
                if (File.Exists(oldFile)) File.Delete(oldFile);
                _ = Msg.ErrorOk($"The update failed because the program could not access\n   {Directory.GetCurrentDirectory()}\n or the folder it is in.", "Update failed");
                return false;
            }
        }
    }
}
