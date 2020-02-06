﻿using app.Services;
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
        List<Playlist> playlists = new List<Playlist>();
        public FirefoxDriver driver;
        public ChromeDriver chromeDriver;
        private IWebElement WebElement;
        private PlaylistService playlistService;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadPath();

            playlistService = new PlaylistService();
        }
        private List<Playlist> ReadPath()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);
            playlists = new List<Playlist>();

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
                                playlists.Add(playlist);
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

            return playlists;
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
            //var firefoxDriverService = FirefoxDriverService.CreateDefaultService();
            //firefoxDriverService.HideCommandPromptWindow = true;

            playlistService.GetPlaylistsData(ReadPath());

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

            foreach (Playlist playlist in playlists)
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
                //if (MusicsToDownload.Any())
                //{
                //    playlist.MusicToDownload = MusicsToDownload;
                //    playlist.Update();
                //}

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