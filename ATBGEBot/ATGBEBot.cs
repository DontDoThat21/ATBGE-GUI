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
        private string customerKey = "NwhE01jkavtSX5I6gfGmD1Qho";
        private string customerSecret = "oUtI7fqxdUf7u2aIGBmxqnPxND2tUrJlKjEG2L9Cc5kb3jii7P";
        private string access_token = "3564689114-x8Axjs1PH2Fp2wemIEKWtq6jkmJK65QDUhk214M";
        private string access_token_secret = "m1wmP3ZTtlLDrkorFDkHbkgVoI7STFGknCgEWWrfysJ82";

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

            results = GetJsonForToday(picCnt);
            TakeImagesFromResults(results);
        }



        static Rootobject GetJsonForToday(string getPicCnt)
        {
            Uri reddit = new Uri(redditWorkingUrl + getPicCnt + "&t=day");
            HttpClient client = new HttpClient();
            string json = new WebClient().DownloadString(reddit);
            Rootobject objectResponse;
            try
            {
                Post redditPost = new Post();
                objectResponse = JsonConvert.DeserializeObject<Rootobject>(json);
                foreach (var item in objectResponse.data.children)
                {
                    Post post = new Post();
                    post.author = item.data.author;
                    post.is_video = item.data.is_video;
                    post.url = item.data.url;
                    posts.Add(post);
                }
                return objectResponse;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}, sorry scrub.");
            }
            return null;
        }

        public void TakeImagesFromResults(Rootobject topToday)
        {
            imageList = new List<string>();
            photos = new List<Bitmap>();

            int resultAmnt = (int)topToday.data.children.GetLongLength(0);
            int successfullPhotos = 0;
            bool exists = System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot");
            DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot");

            if (exists)
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception)
                {
                   MessageBox.Show(@"The temp dir can't at Documents\ATGBEBOT\ can't be deleted. Contact a developer please.");
                }
            }
            dir.Create();

            for (int i = 0; i < resultAmnt; i++)
            {
                WebRequest request = WebRequest.Create(topToday.data.children[i].data.url);                
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream = response.GetResponseStream();
                Bitmap photo;
                try
                {
                    photo = new Bitmap(responseStream);
                    photos.Add(photo);
                    photo.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg");
                    imageList.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg");
                    successfullPhotos++;
                }
                catch (Exception)
                {                   
                }
            }
            lastSuccessPhotoCount = successfullPhotos;

        }

        public void TwitterLogin()
        {
            service = new TwitterService(customerKey, customerSecret, access_token, access_token_secret);
        }
        //public int UploadPics(int totalPics, Rootobject pics, DateTime startT, DateTime endT)
        //{
        //    DateTime[] dates = UploadDelaySet(totalPics, startT, endT);
        //    for(int i = 0; i < dates.Length; i++)
        //    {
        //        key.SetValue($"UploadDate{i}", $"{dates[i]}");
        //    }

        //    int picsAmnt = imageList.Count;
        //    int width = 0;
        //    int height = 0;
        //    string errConsoleOutPut = "";
            

        //    for (int i = 0; i < imageList.Count; i++)
        //    {
        //        if (i > 0)
        //        {
        //            //TimerHelper();
        //        }
        //        try
        //        {
        //            if (pics.data.children[i].data.url.Contains("imgur"))
        //            {
        //                pics.data.children[i].data.url = pics.data.children[i].data.url.Insert(8, "i.");
        //                pics.data.children[i].data.url = pics.data.children[i].data.url + ".jpg";
        //                WebRequest imgurRequest = WebRequest.Create(pics.data.children[i].data.url); 
        //            }
        //        }
        //        catch (Exception)
        //        {
        //             errConsoleOutPut += "Imgur link was not an image. Canceling upload. \n";
        //        }
        //        Bitmap photo;
        //        try
        //        {
        //            WebRequest request = WebRequest.Create(pics.data.children[i].data.url);
        //            WebResponse response = request.GetResponse();
        //            System.IO.Stream responseStream =
        //            response.GetResponseStream();
        //            photo = new Bitmap(responseStream);
        //            width = photo.Width;
        //            height = photo.Height;
        //            photo.Save("jargobargo" + i.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
        //        }
        //        catch (Exception)
        //        {
        //            errConsoleOutPut += "Something went wrong when trying to return the reddit link as a photo into memory. Cancelling this specific upload.";
        //        }                

        //        using (var stream = new FileStream(imageList[i], FileMode.Open))
        //        {
        //            try
        //            {
        //                service.SendTweetWithMedia(new SendTweetWithMediaOptions
        //                {
        //                    Status = pics.data.children[i].data.title + "; Uploaded by " + pics.data.children[i].data.author + ". https://www.reddit.com/r/ATBGE/",
        //                    Images = new Dictionary<string, Stream> { { imageList[i], stream } },
        //                    PossiblySensitive = pics.data.children[i].data.over_18
        //                });
        //            }
        //            catch (NullReferenceException e)
        //            {
        //                errConsoleOutPut += ("Null reference error when uploading to reddit.com" + pics.data.children[i].data.permalink) + "\n";
        //                errConsoleOutPut += $"Is_video: {pics.data.children[i].data.is_video}; \n";
        //                errConsoleOutPut += $"Trying to post this local file: {imageList[i]} \n";
        //            }
        //        }
        //        successUploads++;
        //    }
        //    imageIndex++;
        //    return imageList.Count;
        //}

        //public int UploadPic(Rootobject pic, DateTime startT, DateTime endT)
        //{
        //    dates = UploadDelaySet(1, startT, endT);
        //    for (int i = 0; i < dates.Length; i++)
        //    {
        //        key.SetValue($"UploadDate{i}", $"{dates[i]}");
        //    }

        //    int picsAmnt = imageList.Count;
        //    int width = 0;
        //    int height = 0;
        //    string errConsoleOutPut = "";

        //    try
        //    {
        //        if (pic.data.children[0].data.url.Contains("imgur"))
        //        {
        //            pic.data.children[0].data.url = pic.data.children[0].data.url.Insert(8, "i.");
        //            pic.data.children[0].data.url = pic.data.children[0].data.url + ".jpg";
        //            WebRequest imgurRequest = WebRequest.Create(pic.data.children[0].data.url);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        errConsoleOutPut += "Imgur link was not an image. Canceling upload. \n";
        //    }
        //    Bitmap photo;
        //    try
        //    {
        //        WebRequest request = WebRequest.Create(pic.data.children[0].data.url);
        //        WebResponse response = request.GetResponse();
        //        System.IO.Stream responseStream =
        //        response.GetResponseStream();
        //        photo = new Bitmap(responseStream);
        //        width = photo.Width;
        //        height = photo.Height;
        //        photo.Save("jargobargo" + 0.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
        //    }
        //    catch (Exception)
        //    {
        //        errConsoleOutPut += "Something went wrong when trying to return the reddit link as a photo into memory. Cancelling this specific upload.";
        //    }

        //    using (var stream = new FileStream(imageList[0], FileMode.Open))
        //    {
        //        try
        //        {
        //            service.SendTweetWithMedia(new SendTweetWithMediaOptions
        //            {
        //                Status = pic.data.children[0].data.title + "; Uploaded by " + pic.data.children[0].data.author + ". https://www.reddit.com/r/ATBGE/",
        //                Images = new Dictionary<string, Stream> { { imageList[0], stream } },
        //                PossiblySensitive = pic.data.children[0].data.over_18
        //            });
        //        }
        //        catch (NullReferenceException e)
        //        {
        //            errConsoleOutPut += ("Null reference error when uploading to reddit.com" + pic.data.children[0].data.permalink) + "\n";
        //            errConsoleOutPut += $"Is_video: {pic.data.children[0].data.is_video}; \n";
        //            errConsoleOutPut += $"Trying to post this local file: {imageList[0]} \n";
        //        }
        //    }
        //    successUploads++;

        //    imageIndex++;
        //    return imageList.Count;
        //}


        public void TimerHelper(int imageIndexNum)
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = (timeBetween);//timeBetween;
            //aTimer.Elapsed +=
        }

    }

}
