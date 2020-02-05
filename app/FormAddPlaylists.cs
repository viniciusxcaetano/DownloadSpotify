using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace app
{
    public partial class FormAddPlaylists : Form
    {
        Playlist playlist;
        public string Path { get; set; }
        public FirefoxDriver driver { get; set; }
        public ChromeDriver chromeDriver { get; set; }
        public IWebElement WebElement { get; set; }

        public FormAddPlaylists(string Path)
        {
            this.Path = Path;
            InitializeComponent();
        }

        private void btnAddPlaylist_Click(object sender, EventArgs e)
        {
            if (textBoxUrlPlaylist != null)
            {
                string Url = textBoxUrlPlaylist.Text;
                textBoxUrlPlaylist.Clear();
                playlist = new Playlist(Path, Url);
                driver = new FirefoxDriver(BrowserSettings.FirefoxDriverService, BrowserSettings.FirefoxOptions());

                playlist.GetPlaylist(chromeDriver);
                driver.Quit();

                List<Music> MusicsToDownload = new List<Music>();
                DirectoryInfo di = Directory.CreateDirectory(playlist.PathFolder);

                using (StreamWriter sw = File.CreateText(playlist.PathUrlFile))
                {
                    sw.Write(playlist.Url);
                }

                string[] Tracks = Directory.GetFiles(playlist.PathFolder, "*.mp3")
                                                .Select(System.IO.Path.GetFileName)
                                                .ToArray();

                foreach (Music music in playlist.Music)
                {
                    string track = music.Track + ".mp3";
                    bool Contains = Tracks.Any(track.Contains);

                    if (!Contains)
                    {
                        MusicsToDownload.Add(music);
                    }
                }
                if (MusicsToDownload.Count() > 0)
                {
                    playlist.Update();
                }

                using (StreamWriter sw = File.CreateText(playlist.PathUrlFile))
                {
                    sw.Write(playlist.Url);
                }
                MessageBox.Show("Added Successfully");
            }
            else
            {
                MessageBox.Show("The Url playlist can't be empty");
            }
            Close();
        }

        private void textBoxUrlPlaylist_TextChanged(object sender, EventArgs e)
        {
        }
    }
}