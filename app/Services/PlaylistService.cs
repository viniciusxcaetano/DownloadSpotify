using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace app.Services
{
    public class PlaylistService
    {
        public PlaylistService()
        {
        }

        private IWebElement WebElement;
        //public FirefoxDriver driver { get; set; }
        public ChromeDriver chromeDriver { get; set; }
        public string PathDownloadFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"); } }




        public List<Playlist> GetUpdatedPlaylist(ChromeDriver chromeDriver, List<Playlist> playlists)
        {
            //chromeDriver = new ChromeDriver(BrowserSettings.ChromeDriverService);

            foreach (var playlist in playlists)
            {
                int order = 1;
                int nextElementCount = 0;
                var listMusic = new List<Music>();
                playlist.Music = new List<Music>();
                IJavaScriptExecutor js = chromeDriver;

                chromeDriver.Navigate().GoToUrl(playlist.Url);

                WebElement = chromeDriver.FindElement(By.ClassName("mo-info-name"), 10);
                string playlistName = WebElement.Text;

                if (playlistName == "") // for the windown when small
                {
                    WebElement = chromeDriver.FindElement(By.ClassName("TrackListHeader__entity-name"), 10);
                    string[] trackSplit = WebElement.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    playlistName = trackSplit[0];
                }

                playlist.Name = playlistName;
                playlist.PathFolder = playlist.Device + "\\Spotify-" + playlist.Name;
                playlist.PathUrlFile = playlist.PathFolder + "\\url.txt";

                IReadOnlyCollection<IWebElement> tempSongs = chromeDriver.FindElements(By.ClassName("tracklist-name"), 10);
                IReadOnlyCollection<IWebElement> tempArtists = chromeDriver.FindElements(By.ClassName("second-line"), 10);


                foreach (var sa in tempArtists.Zip(tempSongs, Tuple.Create))
                {
                    listMusic.Add(new Music { Artist = sa.Item1.Text, Name = sa.Item2.Text });
                }

                listMusic = FormatMusic(listMusic);

                for (int i = 0; i < 7; i++) // for test
                //for (int i = 0; i < listMusic.Count; i++)
                {
                    nextElementCount = listMusic.Count + 1 <= listMusic.Count ? i + 1 : i;
                    Actions actions = new Actions(chromeDriver);

                    actions.ContextClick(tempSongs.ElementAt(i)).Perform(); // right click
                    var checkIsMusic = chromeDriver.FindElements(By.ClassName("react-contextmenu-item"), 10);

                    if (!checkIsMusic.Any(o => o.Text != "")) { actions.ContextClick(tempSongs.ElementAt(i)).Perform(); } //to fix a bug

                    try
                    {
                        var music = checkIsMusic.Where(o => o.Text == "Copiar link da música" || o.Text == "Copy Song Link").FirstOrDefault();
                        music.Click();
                        var paste = System.Windows.Forms.Clipboard.GetText();
                        string[] trackSplit = paste.Split(new[] { "/track/" }, StringSplitOptions.None);
                        string id = trackSplit[1];

                        //create music add to playlist
                        playlist.Music.Add(new Music
                        {
                            Artist = listMusic[i].Artist,
                            Name = listMusic[i].Name,
                            Id = id,
                            Track = order + ". " + listMusic[i].Artist + "-" + listMusic[i].Name + "    ID=" + id,
                            PlaylistName = playlistName
                        });

                        if (tempSongs.ElementAt(nextElementCount).Location.Y > 200)
                        {
                            js.ExecuteScript($"window.scrollTo({0}, {tempSongs.ElementAt(nextElementCount).Location.Y - 200 })");
                        }
                        order++;
                    }
                    catch
                    {
                        if (tempSongs.ElementAt(nextElementCount).Location.Y > 200)
                        {
                            js.ExecuteScript($"window.scrollTo({0}, {tempSongs.ElementAt(nextElementCount).Location.Y - 200 })");
                        }
                    }
                }
            }
            //chromeDriver.Quit();
            return playlists;
        }

        public void UpdatePlaylist(ChromeDriver chromeDriver, List<Playlist> playlists)
        {
            foreach (var playlist in playlists)
            {
                playlist.MusicsToDownload = new List<Music>();

                string[] songsInDirectory = Directory.GetFiles(playlist.PathFolder, "*id=*").Select(Path.GetFileName).ToArray();

                var ids = songsInDirectory.Select(d => d.Split(new[] { "id=", ".mp3" }, StringSplitOptions.None)).Select(t => t[1]).ToList();

                //songs to delete
                foreach (var id in ids)
                {
                    if (!playlist.Music.Any(o => o.Id == id))
                    {
                        //delete song with id = id;
                    }
                }
                //songs to download
                foreach (var music in playlist.Music)
                {
                    if (!ids.Any(o => o == music.Id))
                    {
                        playlist.MusicsToDownload.Add(music);
                        DownloadSong(chromeDriver, music);
                    }
                }
            }


        }

        public List<Music> FormatMusic(List<Music> listMusic)
        {
            for (int i = 0; i < listMusic.Count; i++)
            {
                if (listMusic[i].Artist.Contains('•'))
                {
                    string[] artist = listMusic[i].Artist.Split('\n');
                    if (artist[0].Contains(','))
                    {
                        artist = artist[0].Split(',');
                        listMusic[i].Artist = artist[0];
                    }
                    else if (!artist[0].Contains("EXPLICIT"))
                    {
                        string[] artistTemp = artist[0].Split('\r');
                        listMusic[i].Artist = artistTemp[0];
                    }
                    else if (artist[1].Contains(','))
                    {
                        artist = artist[1].Split(',');
                        listMusic[i].Artist = artist[0];
                    }
                    else
                    {
                        if (artist[0].Contains("EXPLICIT"))
                        {
                            string[] artistTemp = artist[1].Split('\r');
                            listMusic[i].Artist = artistTemp[0];
                        }
                        else
                        {
                            string[] text1 = artist[0].Split('\r');
                            listMusic[i].Artist = text1[0];
                        }
                    }
                }
                else
                {
                    listMusic[i].Artist = "";
                }
            }
            return listMusic;
        }

        //public void Update()
        //{
        //    foreach (Music music in MusicToDownload)
        //    {
        //        DownloadSong(music);

        //    }
        //    CheckIfDownloadedAll();
        //    CheckIfSongIsEmpty();
        //}

        public void CheckIfDownloadedAll()
        {
            //TODO: set a limit time
            var tracks = Directory.GetFiles(PathDownloadFolder, "*.crdownload")
                                                            .Select(Path.GetFileName)
                                                            .ToList();
            if (tracks.Any())
            {
                Thread.Sleep(2000);
                CheckIfDownloadedAll();
            }
        }

        public void MoveSongs(List<Playlist> playlists)
        {
            //var oldPath = string.Format("{0}\\{1}.mp3", PathDownloadFolder, Utils.FormatTrackName(musicToDownload.Track));

            foreach (var playlist in playlists)
            {
                foreach (var musicDownloaded in playlist.MusicsToDownload)
                {
                    var oldPath = string.Format("{0}\\{1}.mp3", PathDownloadFolder, musicDownloaded.Id);

                    if (File.Exists(oldPath))
                    {
                        FileInfo fileInfo = new FileInfo(oldPath);

                        if (fileInfo.Exists)
                        {
                            var size = fileInfo.Length;

                            if (size == 0)
                            {
                                File.Delete(oldPath);
                                //var splitted = oldPath.Split(new string[] { "\\", ".mp3" }, StringSplitOptions.None);
                                //oldPath = splitted[splitted.Count() - 2];

                                //var music = MusicToDownload.Find(o => Utils.FormatTrackName(o.Track) == oldPath);
                                //if (music != null)
                                //{
                                //    DownloadSong(music);
                                //    CheckIfDownloadedAll();

                                //    fileInfo = new FileInfo(oldPath);
                                //    if (fileInfo.Exists)
                                //        size = fileInfo.Length;

                                //    if (size == 0)
                                //    {
                                //        music.alreadyTried++;
                                //        MoveSongs();
                                //    }
                                //}
                            }

                            // move file to the right folder
                            var newPath = string.Format("{0}\\{1}.mp3", playlist.PathFolder, musicDownloaded.Track);
                            try
                            {
                                File.Move(oldPath, newPath);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public void DownloadSong(ChromeDriver chromeDriver, Music music)
        {
            if (music.alreadyTried < 2)
            {
                string name = RemoveNonAlpha(music.Name).Replace(" ", "+");
                string artist = RemoveNonAlpha(music.Artist).Replace(" ", "+");

                string youtubeUrl = "https://www.youtube.com/results?search_query=" + artist + "+" + name + "+audio";

                chromeDriver.Navigate().GoToUrl(youtubeUrl);

                IReadOnlyCollection<IWebElement> webElements = chromeDriver.FindElements(By.Id("thumbnail"), 10);

                foreach (var we in webElements)
                {
                    youtubeUrl = we.GetAttribute("href");
                    if (!youtubeUrl.Contains("www.googleadservices.com"))
                    {
                        if (CheckDuration(chromeDriver))
                        {
                            var youtUrl = youtubeUrl.Replace("ube", "");

                            chromeDriver.Navigate().GoToUrl(youtUrl);

                            //need this If below, because sometimes cannot download some songs
                            if (chromeDriver.FindElements(By.Name("settings_title"), 20).Count() > 0)
                            {
                                FillTheFieldsAndDownload(chromeDriver, music);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public bool CheckDuration(ChromeDriver chromeDriver)
        {
            //check if music duration is less than 9 minutes 
            var temp = chromeDriver.FindElement(By.XPath("//ytd-video-renderer//ytd-thumbnail-overlay-time-status-renderer"), 50);

            string[] time = temp.Text.Split(new[] { ":" }, StringSplitOptions.None);

            var canContinue = time.Length <= 2 ? true : false;
            if (canContinue)
            {
                canContinue = int.Parse(time[0]) < 9 ? true : false;
            }
            return canContinue;

            //DateTime time = new DateTime();
            //try
            //{
            //    var durationTemp = string.Format("00:0" + chromeDriver.FindElements(By.XPath("//ytd-video-renderer//ytd-thumbnail-overlay-time-status-renderer"), 50).FirstOrDefault().Text);
            //    time = DateTime.ParseExact(durationTemp, "HH:mm:ss", CultureInfo.InvariantCulture);
            //}
            //catch (Exception ex)
            //{
            //    if (!(ex.Message == "Cadeia de caracteres não foi reconhecida como DateTime válido." || ex.Message == "String was not recognized as a valid DateTime."))
            //        GetDurationMusic(chromeDriver);
            //}
        }

        public void FillTheFieldsAndDownload(ChromeDriver chromeDriver, Music music)
        {
            try
            {
                WebElement = chromeDriver.FindElement(By.Name("settings_title"), 10);
                WebElement.SendKeys(Keys.Control + "a");
                WebElement.SendKeys(Keys.Delete);

                WebElement.SendKeys(music.Id);

                WebElement = chromeDriver.FindElement(By.Name("settings_artist"), 10);
                WebElement.SendKeys(Keys.Control + "a");
                WebElement.SendKeys(Keys.Delete);
                WebElement.SendKeys("Spotify-" + music.PlaylistName);

                WebElement = chromeDriver.FindElement(By.ClassName("recorder-action"), 10);
                WebElement.Click();

                CheckIfDownloadHasStarted(chromeDriver, music);
            }
            catch (Exception)
            {
                DownloadSong(chromeDriver, music);
            }
        }
        public void CheckIfDownloadHasStarted(ChromeDriver chromeDriver, Music music)
        {
            if (music.alreadyTried < 2)
            {
                //string track = Utils.FormatTrackName(music.Track);

                if (chromeDriver.FindElements(By.ClassName("btn-warning"), 20).Count() > 0)
                {
                    WebElement = chromeDriver.FindElement(By.ClassName("btn-recorder"), 10);
                    if (WebElement.Text == "Repetir gravação MP3")
                    {
                        //se aparecer "Repetir gravacao" recarregar a pagina e tentar denovo.
                        music.alreadyTried++;
                        DownloadSong(chromeDriver, music);

                    }
                }

                var pathFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                //var Tracks = Directory.GetFiles(pathFolder, "*" + track + "*").
                //           Where(s => s.EndsWith(".mp3") || s.EndsWith(".part")).
                //           Select(System.IO.Path.GetFileName).ToList();
                var Tracks = Directory.GetFiles(pathFolder, "*" + music.Id + "*").
                           Where(s => s.EndsWith(".mp3") || s.EndsWith(".crdownload")).
                           Select(Path.GetFileName).ToList();

                if (!Tracks.Any())
                {
                    Thread.Sleep(1000);
                    CheckIfDownloadHasStarted(chromeDriver, music);
                }
            }
        }

        public static string RemoveNonAlpha(string str)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                str = str.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }

            str = Regex.Replace(str, "[^a-zA-Z0-9 ]", "");
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            str = regex.Replace(str, " ");

            return str;
        }
    }
}