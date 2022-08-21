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
        //private string twtrUsername = "tytr96";
        //private string twtrPassword = "blargNjelly97";        

        //private int imageId = 0;
        //private int successUploads = 0;
        //private System.Timers.Timer aTimer;
        private TwitterService service;

        private int lastSuccessPhotoCount;
        private int timeBetween;
        private int imageIndex = 0;

        private static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit="; // 5&t=day       
        private List<string> imageList = new List<string>();
        private static List<Bitmap> photos = new List<Bitmap>();
        private Rootobject results;
        private bool twtrLognSuccess = false;

        static List<Post> posts = new List<Post>();

        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");

        public TwitterBot(string picCnt)
        {


        }

    }
}
