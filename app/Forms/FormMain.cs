﻿using app.Services;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace app
{
    public partial class FormMain : Form
    {
        public string defaultPath = "D:\\Music";
        FolderBrowserDialog Dialog;
        List<Playlist> playlists = new List<Playlist>();
        public ChromeDriver chromeDriver;
        private PlaylistService playlistService;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            playlistService = new PlaylistService();
            playlists = ReadLocalPlaylists();
        }
        private List<Playlist> ReadLocalPlaylists()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(defaultPath);
            playlists = new List<Playlist>();

            if (directoryInfo.Exists)
            {
                DirectoryInfo[] directorySpotify = directoryInfo.GetDirectories("*" + "Spotify" + "*.*");


                if (directorySpotify.Any())
                {
                    foreach (var dirSpot in directorySpotify)
                    {
                        string PathFolder = defaultPath + "\\" + dirSpot.Name;
                        string PathUrlFile = PathFolder + "\\url.txt";

                        if (File.Exists(PathUrlFile))
                        {
                            string[] pathUrlFile = File.ReadAllLines(PathUrlFile);

                            if (pathUrlFile.Any())
                            {
                                string Url = pathUrlFile.FirstOrDefault();
                                Playlist playlist = new Playlist(defaultPath, Url)
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
                defaultPath = Dialog.SelectedPath;
                ReadLocalPlaylists();
            }
        }

        private void btnUpdatePlaylists_Click(object sender, EventArgs e)
        {
            chromeDriver = new ChromeDriver(BrowserSettings.ChromeDriverService);
            playlists = playlistService.GetUpdatedPlaylist(chromeDriver, ReadLocalPlaylists());
            playlistService.UpdatePlaylist(chromeDriver, playlists);
            playlistService.CheckIfDownloadedAll();
            playlistService.MoveSongs(playlists);
            chromeDriver.Quit();
            MessageBox.Show("Your playlists are up to date");
        }

        //private void UpdatePlaylist(List<Playlist> playlists)
        //{
        //    foreach (var playlist in playlists)
        //    {
        //        List<Music> MusicsToDownload = new List<Music>();

        //        string[] songsInDirectory = Directory.GetFiles(playlist.PathFolder, "*id=*").Select(Path.GetFileName).ToArray();

        //        var ids = songsInDirectory.Select(d => d.Split(new[] { "id=", ".mp3" }, StringSplitOptions.None)).Select(t => t[1]).ToList();

        //        //songs to delete
        //        foreach (var id in ids)
        //        {
        //            if (!playlist.Music.Any(o => o.Id == id))
        //            {
        //                //delete song with id = id;
        //            }
        //        }
        //        //songs to download
        //        foreach (var music in playlist.Music)
        //        {
        //            if (!ids.Any(o => o == music.Id))
        //            {
        //                playlistService.DownloadSong(music);
        //            }
        //        }
        //    }
        //}

        //try
        //{
        //    driver = new FirefoxDriver(BrowserSettings.FirefoxDriverService, BrowserSettings.FirefoxOptions());
        //}
        //catch (Exception ex)
        //{
        //    if (ex.HResult == -2146233079)
        //    {
        //        MessageBox.Show("Need to install Firefox browser");
        //    }
        //}

        //foreach (Playlist playlist in playlists)
        //{
        //    List<Music> MusicsToDownload = new List<Music>();

        //    string[] downloadedSongs = Directory.GetFiles(playlist.PathFolder, "*.mp3")
        //                                    .Select(Path.GetFileName)
        //                                    .ToArray();

        //        //Songs to download
        //        foreach (Music music in playlist.Music)
        //        {
        //            string track = music.Track + ".mp3";

        //            if (!downloadedSongs.Any(track.Contains))
        //            {
        //                MusicsToDownload.Add(music);
        //            }
        //        }
        //        //if (MusicsToDownload.Any())
        //        //{
        //        //    playlist.MusicToDownload = MusicsToDownload;
        //        //    playlist.Update();
        //        //}

        //        //Songs to delete
        //        foreach (string track in downloadedSongs)
        //        {
        //            if (track.Contains(".mp3"))
        //            {
        //                string[] trackSplit = track.Split(new[] { ".mp3" }, StringSplitOptions.None);
        //                string Track = trackSplit[0];
        //                Music music = playlist.Music.Find(x => (x.Track == Track));

        //                if (music == null)
        //                {
        //                    var test = (playlist.PathFolder + "\\" + Track + ".mp3");
        //                    File.Delete(test);
        //                }
        //            }
        //        }
        //    }

        //    MessageBox.Show("Your playlists are up to date");
        //}
        private void btnAddPlaylists_Click(object sender, EventArgs e)
        {
            Form formAddPlaylists = new FormAddPlaylists(defaultPath);
            formAddPlaylists.Show();
        }
    }
}