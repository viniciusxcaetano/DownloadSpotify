using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace app
{
    public partial class FormMain : Form
    {
        public string Path = "D:\\Music";
        FolderBrowserDialog Dialog;
        List<Playlist> Playlist = new List<Playlist>();
        public FirefoxDriver driver;
        public ChromeDriver chromeDriver;
        private IWebElement WebElement;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadPath();
        }
        private void ReadPath()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);

            if (directoryInfo.Exists)
            {
                DirectoryInfo[] directorySpotify = directoryInfo.GetDirectories("*" + "Spotify" + "*.*");


                if (directorySpotify.Any())
                {
                    foreach (var dirSpot in directorySpotify)
                    {
                        string PathFolder = Path + "\\" + dirSpot.Name;
                        string PathUrlFile = PathFolder + "\\url.txt";

                        if (File.Exists(PathUrlFile))
                        {
                            string[] pathUrlFile = File.ReadAllLines(PathUrlFile);

                            if (pathUrlFile.Any())
                            {
                                string Url = pathUrlFile.FirstOrDefault();
                                Playlist playlist = new Playlist(Path, Url)
                                {
                                    Name = dirSpot.Name,
                                    PathUrlFile = PathUrlFile
                                };
                                Playlist.Add(playlist);
                            }
                            else
                            {
                                MessageBox.Show("Url file is Empty");
                            }
                        }
                        else MessageBox.Show("Url file, doesn't exists");
                    }
                    btnUpdatePlaylists.Enabled = true;
                }
                else btnUpdatePlaylists.Enabled = false;
            }
            else btnUpdatePlaylists.Enabled = false;

        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            Dialog = new FolderBrowserDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                Path = Dialog.SelectedPath;
                ReadPath();
            }
        }

        private void btnUpdatePlaylists_Click(object sender, EventArgs e)
        {
            Playlist.Clear();
            ReadPath();

            var firefoxDriverService = FirefoxDriverService.CreateDefaultService();
            firefoxDriverService.HideCommandPromptWindow = true;


            chromeDriver = new ChromeDriver(BrowserSettings.ChromeDriverService);
            foreach (Playlist playlist in Playlist)
            {
                playlist.GetPlaylist(chromeDriver);
            }
            chromeDriver.Quit();

            try
            {
                driver = new FirefoxDriver(BrowserSettings.FirefoxDriverService, BrowserSettings.FirefoxOptions());
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146233079)
                {
                    MessageBox.Show("Need to install Firefox");
                }

            }

            foreach (Playlist playlist in Playlist)
            {
                List<Music> MusicsToDownload = new List<Music>();

                string[] downloadedSongs = Directory.GetFiles(playlist.PathFolder, "*.mp3")
                                                .Select(System.IO.Path.GetFileName)
                                                .ToArray();

                //Songs to download
                foreach (Music music in playlist.Music)
                {
                    string track = music.Track + ".mp3";

                    if (!downloadedSongs.Any(track.Contains))
                    {
                        MusicsToDownload.Add(music);
                    }
                }
                if (MusicsToDownload.Any())
                {
                    playlist.MusicToDownload = MusicsToDownload;
                    playlist.Update();
                }

                //Songs to delete
                foreach (string track in downloadedSongs)
                {
                    if (track.Contains(".mp3"))
                    {
                        string[] trackSplit = track.Split(new[] { ".mp3" }, StringSplitOptions.None);
                        string Track = trackSplit[0];
                        Music music = playlist.Music.Find(x => (x.Track == Track));

                        if (music == null)
                        {
                            var test = (playlist.PathFolder + "\\" + Track + ".mp3");
                            File.Delete(test);
                        }
                    }
                }
            }

            MessageBox.Show("Your playlists are up to date");
        }
        private void btnAddPlaylists_Click(object sender, EventArgs e)
        {
            Form formAddPlaylists = new FormAddPlaylists(Path);
            formAddPlaylists.Show();
        }
    }
}