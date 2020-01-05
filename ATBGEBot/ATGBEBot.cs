using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using TweetSharp;
using System.Timers;
using Microsoft.Win32;
using ATBGEBot;

namespace InstagramATBGEBot
{
    public class TwitterBot
    {
        private string twtrUsername = "tytr96";
        private string twtrPassword = "blargNjelly97";        

        private int imageId = 0;
        private int successUploads = 0;
        private System.Timers.Timer aTimer;
        public TwitterService service;

        public int lastSuccessPhotoCount;
        public int timeBetween;
        public int imageIndex = 0;

        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit="; // 5&t=day       
        public List<string> imageList = new List<string>();
        public static List<Bitmap> photos = new List<Bitmap>();
        public Rootobject results;
        public bool twtrLognSuccess = false;

        static List<Post> posts = new List<Post>();

        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterBotATBGE");

        public TwitterBot(string picCnt)
        {


        }

    }
}
