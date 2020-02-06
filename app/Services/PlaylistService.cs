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
        public PlaylistService() { }

        private IWebElement WebElement;
        public FirefoxDriver driver { get; set; }
        public ChromeDriver chromeDriver { get; set; }



        public void GetPlaylistsData(List<Playlist> playlists)
        {
            chromeDriver = new ChromeDriver(BrowserSettings.ChromeDriverService);
            //Order = 1;
            //var tempMusic = new List<Music>();
            //Music = new List<Music>();
            foreach (var playlist in playlists)
            {
                int order = 1;
                var listMusic = new List<Music>();
                playlist.Music = new List<Music>();
                IJavaScriptExecutor js = chromeDriver;

                chromeDriver.Navigate().GoToUrl(playlist.Url);

                WebElement = chromeDriver.FindElement(By.ClassName("mo-info-name"), 10);
                playlist.Name = WebElement.Text;
                playlist.PathFolder = playlist.Device + "\\Spotify-" + playlist.Name;
                playlist.PathUrlFile = playlist.PathFolder + "\\url.txt";

                IReadOnlyCollection<IWebElement> tempSongs = chromeDriver.FindElements(By.ClassName("tracklist-name"), 10);
                IReadOnlyCollection<IWebElement> tempArtists = chromeDriver.FindElements(By.ClassName("second-line"), 10);


                foreach (var sa in tempArtists.Zip(tempSongs, Tuple.Create))
                {
                    listMusic.Add(new Music { Artist = sa.Item1.Text, Name = sa.Item2.Text });
                }

                listMusic = FormatMusic(listMusic);

                for (int i = 0; i < listMusic.Count; i++)
                {
                    Actions actions = new Actions(chromeDriver);

                    actions.ContextClick(tempSongs.ElementAt(i)).Perform(); // right click
                    var checkIsMusic = chromeDriver.FindElements(By.ClassName("react-contextmenu-item"), 10);

                    if (!checkIsMusic.Any(o => o.Text != "")) { actions.ContextClick(tempSongs.ElementAt(i)).Perform(); }

                    try
                    {
                        var music = checkIsMusic.Where(o => o.Text == "Copiar link da música" || o.Text == "Copy Song Link").FirstOrDefault();
                        music.Click();
                        var paste = System.Windows.Forms.Clipboard.GetText();
                        string[] trackSplit = paste.Split(new[] { "/track/" }, StringSplitOptions.None);
                        string id = trackSplit[1];
                        js.ExecuteScript("window.scrollBy(0, 300)", ""); //scroll down

                        //create music add to playlist
                        playlist.Music.Add(new Music
                        {
                            Artist = listMusic[i].Artist,
                            Name = listMusic[i].Name,
                            Id = id,
                            Track = order + ". " + listMusic[i].Artist + "-" + listMusic[i].Name + "    ID:" + id
                        });
                        order++;
                    }
                    catch (Exception ex)
                    {
                        js.ExecuteScript("window.scrollBy(0, 300)", ""); //scroll down
                    }
                }
            }
            chromeDriver.Quit();
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

        //public void CheckIfDownloadedAll()
        //{
        //    var tracks = Directory.GetFiles(PathDownloadFolder, "*.part")
        //                                                    .Select(Path.GetFileName)
        //                                                    .ToList();
        //    if (tracks.Any())
        //    {
        //        Thread.Sleep(2000);
        //        CheckIfDownloadedAll();
        //    }
        //}

        //public void CheckIfSongIsEmpty()
        //{
        //    foreach (var musicToDownload in MusicToDownload)
        //    {
        //        //var oldPath = string.Format("{0}\\{1}.mp3", PathDownloadFolder, Utils.FormatTrackName(musicToDownload.Track));

        //        if (File.Exists(oldPath))
        //        {
        //            FileInfo fileInfo = new FileInfo(oldPath);

        //            if (fileInfo.Exists)
        //            {
        //                var size = fileInfo.Length;

        //                if (size == 0)
        //                {
        //                    File.Delete(oldPath);
        //                    var splitted = oldPath.Split(new string[] { "\\", ".mp3" }, StringSplitOptions.None);
        //                    oldPath = splitted[splitted.Count() - 2];

        //                    var music = MusicToDownload.Find(o => Utils.FormatTrackName(o.Track) == oldPath);
        //                    if (music != null)
        //                    {
        //                        DownloadSong(music);
        //                        CheckIfDownloadedAll();

        //                        fileInfo = new FileInfo(oldPath);
        //                        if (fileInfo.Exists)
        //                            size = fileInfo.Length;

        //                        if (size == 0)
        //                        {
        //                            music.alreadyTried++;
        //                            CheckIfSongIsEmpty();
        //                        }
        //                    }
        //                }

        //                // move file to the right folder
        //                //var newPath = string.Format("{0}\\{1}.mp3", PathFolder, Utils.FormatTrackName(musicToDownload.Track));
        //                try
        //                {
        //                    File.Move(oldPath, newPath);
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }
        //    }
        //}

        //public void DownloadSong(Music music)
        //{
        //    if (music.alreadyTried < attempts)
        //    {
        //        string name = music.Name.Replace(" ", "+");
        //        string artist = music.Artist.Replace(" ", "+");

        //        string youtubeUrl = "https://www.youtube.com/results?search_query=" + artist + "+" + name + "+audio";

        //        driver.Navigate().GoToUrl(youtubeUrl);

        //        IReadOnlyCollection<IWebElement> webElements = driver.FindElements(By.Id("thumbnail"), 10);

        //        foreach (var we in webElements)
        //        {
        //            youtubeUrl = we.GetAttribute("href");
        //            if (!youtubeUrl.Contains("www.googleadservices.com"))
        //            {
        //                GetDurationMusic(music);
        //                break;
        //            }
        //        }

        //        if (music.Duration.Minute > 0)
        //        {
        //            var youtUrl = youtubeUrl.Replace("ube", "");

        //            driver.Navigate().GoToUrl(youtUrl);

        //            //need this below, because sometimes cannot download some songs
        //            if (driver.FindElements(By.Name("settings_title"), 20).Count() > 0)
        //            {
        //                FillTheFieldsAndDownload(music);
        //            }
        //        }
        //    }
        //}

        //public void GetDurationMusic(Music music)
        //{
        //    try
        //    {
        //        var duration = string.Format("00:0" + driver.FindElements(By.XPath("//ytd-video-renderer//ytd-thumbnail-overlay-time-status-renderer"), 50).FirstOrDefault().Text);
        //        music.Duration = DateTime.ParseExact(duration, "HH:mm:ss", CultureInfo.InvariantCulture);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!(ex.Message == "Cadeia de caracteres não foi reconhecida como DateTime válido." || ex.Message == "String was not recognized as a valid DateTime."))
        //            GetDurationMusic(music);
        //    }
        //}

        //public void FillTheFieldsAndDownload(Music music)
        //{
        //    try
        //    {
        //        WebElement = driver.FindElement(By.Name("settings_title"), 10);
        //        WebElement.SendKeys(Keys.Control + "a");
        //        WebElement.SendKeys(Keys.Delete);

        //        if (music.Artist == "")
        //        {
        //            WebElement.SendKeys(music.Name);
        //            music.Track = music.Name;
        //        }
        //        else
        //        {
        //            WebElement.SendKeys(music.Artist + "-" + music.Name);
        //            music.Track = music.Artist + "-" + music.Name;
        //        }

        //        WebElement = driver.FindElement(By.Name("settings_artist"), 10);
        //        WebElement.SendKeys(Keys.Control + "a");
        //        WebElement.SendKeys(Keys.Delete);
        //        WebElement.SendKeys("Spotify-" + this.Name);

        //        WebElement = driver.FindElement(By.ClassName("recorder-action"), 10);
        //        WebElement.Click();

        //        CheckIfDownloadHasStarted(music);
        //    }
        //    catch (Exception)
        //    {
        //        DownloadSong(music);
        //    }
        //}
        //public void CheckIfDownloadHasStarted(Music music)
        //{
        //    if (music.alreadyTried < attempts)
        //    {
        //        //string track = Utils.FormatTrackName(music.Track);

        //        if (driver.FindElements(By.ClassName("btn-warning"), 20).Count() > 0)
        //        {
        //            WebElement = driver.FindElement(By.ClassName("btn-recorder"), 10);
        //            if (WebElement.Text == "Repetir gravação MP3")
        //            {
        //                //se aparecer "Repetir gravacao" recarregar a pagina e tentar denovo.
        //                music.alreadyTried++;
        //                DownloadSong(music);

        //            }
        //        }

        //        var pathFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        //        var Tracks = Directory.GetFiles(pathFolder, "*" + track + "*").
        //                   Where(s => s.EndsWith(".mp3") || s.EndsWith(".part")).
        //                   Select(System.IO.Path.GetFileName).ToList();

        //        if (!Tracks.Any())
        //        {
        //            Thread.Sleep(1000);
        //            CheckIfDownloadHasStarted(music);
        //        }
        //    }
        //}

        //public static string RemoveNonAlpha(string str)
        //{
        //    string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
        //    string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

        //    for (int i = 0; i < comAcentos.Length; i++)
        //    {
        //        str = str.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
        //    }

        //    str = Regex.Replace(str, "[^a-zA-Z0-9 ]", "");
        //    RegexOptions options = RegexOptions.None;
        //    Regex regex = new Regex("[ ]{2,}", options);
        //    str = regex.Replace(str, " ");

        //    return str;
        //}
    }
}