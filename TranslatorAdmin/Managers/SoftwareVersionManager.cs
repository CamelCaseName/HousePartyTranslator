using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Helpers;

namespace Translator.Desktop.Managers
{
    [SupportedOSPlatform("Windows")]
    internal static class SoftwareVersionManager
    {
        public const string LocalVersion = "0.7.4.3";
        public static string? LatestGithubVersion = "0.0.0.0";
        public static bool UpdatePending = false;
        private static readonly HttpClient client = new();
#if !DEBUG
        private static bool DownloadDone = false;
        const string APIUrl = "https://api.github.com/repos/CamelCaseName/HousePartyTranslator/releases/latest";
#endif

        /// <summary>
        /// Download and replaces the running application if necessary
        /// </summary>
        public static void ReplaceFileIfNew()
        {
            //modify client signatures
            client.DefaultRequestHeaders.Add("User-Agent", "House Party Translator update service");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");

            //offload async context
            DoWork();
        }

#if DEBUG
#pragma warning disable CS1998
#endif
        private static async void DoWork()
        {
#if DEBUG
            return;
#pragma warning restore CS1998
#else
            if (App.MainForm?.UI == null) return;
            try
            {
                //get data from github about the packages 
                GithubResponse? response = await JsonSerializer.DeserializeAsync<GithubResponse>(await client.GetStreamAsync(APIUrl));

                string oldFile = DeleteOld();//deletes prev.exe if it exists

                //check version and for need of update
                if (!UpdateNeeded(response?.TagName ?? "0.0.0.0"))
                {
                    LogManager.Log("no update needed");
                    return;
                }
                //prepare files
                (bool successfull, string newFile) = CreateFiles();
                if (!successfull) return;

                //inform rest of program
                UpdatePending = true;

                if (Msg.InfoYesNoB("A new version is available to download. Do you want to automatically update this installation?\n\n CHANGELOG:\n" + response?.Body, "Update - " + response?.Name))
                {
                    if (response == null || response?.Assets?.Count < 1) throw new NullReferenceException();

                    LogManager.Log("Self update started");

                    Download(response?.Assets?[0]?.BrowserDownloadUrl ?? string.Empty, oldFile, newFile);
                }
            }
            catch (Exception e)
            {
                LogManager.Log(e.Message, LogManager.Level.Error);
                LogManager.Log("Self update failed.", LogManager.Level.Warning);
                _ = Msg.ErrorOk($"The update failed because {e.Message}", "Update failed");
            }
        }

        //returns true if current version is up to date
        private static bool UpdateNeeded(string githubVersion)
        {
            //extract version number
            //todo, replace by version string with dots so we can just compare directly !ship with 1.0 so it doesnt break anything!
            LatestGithubVersion = $"{githubVersion[0]}.{githubVersion[2]}.0.0";
            if (githubVersion.Length > 3) LatestGithubVersion = $"{githubVersion[0]}.{githubVersion[2]}.{githubVersion[3]}.0";
            if (githubVersion.Length > 4) LatestGithubVersion = $"{githubVersion[0]}.{githubVersion[2]}.{githubVersion[3]}.{githubVersion[4]}";

            //if the version on github has a higher version number
            return Version.Parse(LocalVersion) < Version.Parse(LatestGithubVersion);
        }

        private static (bool, string) CreateFiles()
        {
            try
            {
                //path to file if we need it
                string releaseFile = Path.Combine(Directory.GetCurrentDirectory(), "Release.7z");
                return (true, releaseFile);
            }
            catch (Exception e)
            {
                LogManager.Log(e.Message);
            }
            return (false, string.Empty);
        }

        private static string DeleteOld()
        {
            //delete old one if it exists
            string oldFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "prev.exe");
            if (File.Exists(oldFile)) File.Delete(oldFile);
            return oldFile;
        }

        private static async void Download(string downloadUrl, string oldFile, string newFile)
        {
            try
            {
                LogManager.Log("Download of new Version started");
                //get stream to the file in web location
                using Stream stream = await client.GetStreamAsync(downloadUrl);
                //stream to the file on dis
                using var fileStream = new FileStream(newFile, FileMode.Create);
                //copy data to file on disk
                await stream.CopyToAsync(fileStream);
                fileStream.Close();
                stream.Dispose();
            }
            catch (UnauthorizedAccessException e)
            {
                LogManager.Log(e.ToString(), LogManager.Level.Error);
                _ = Msg.ErrorOk($"The update failed because the program could not access\n   {newFile}\n or the folder it is in.", "Update failed");
                return;
            }
            finally
            {
                LogManager.Log("Download of new Version complete");
                DownloadDone = true;
                UpdateFile(oldFile, newFile);
            }
        }

        private static bool UpdateFile(string oldFile, string newFile)
        {
            //wait for the download to complete
            while (!DownloadDone) ;
            //move currently running exe out of the way
            File.Move(Application.ExecutablePath, oldFile);
            LogManager.Log("Moved old file away");
            LogManager.Log("Extracting new version");
            //extract file to our current location and replace
            var extractor = new SevenZipExtractor.ArchiveFile(newFile);
            try
            {
                extractor.Extract(Directory.GetCurrentDirectory(), true);

                LogManager.Log("New version extracted");

                //inform user
                _ = Msg.InfoOk("Successfully updated the program! Please restart the app now.", "Update successful");
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
#endif
        }
    }
}
