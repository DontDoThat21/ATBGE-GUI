using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using TweetSharp;
using System.Diagnostics;
using System.Timers;
using System.Windows;

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
        private List<string> imageList = new List<string>();

        public TwitterService service;

        public int lastSuccessPhotoCount;

        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit="; // 5&t=day
        public static List<Bitmap> photos = new List<Bitmap>();
        public Rootobject results;
        public bool twtrLognSuccess = false;


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
            Rootobject objectResponse = JsonConvert.DeserializeObject<Rootobject>(json);
            return objectResponse;
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
                dir.Delete(true);
            }
            dir.Create();

            for (int i = 0; i < resultAmnt; i++)
            {
                WebRequest request = WebRequest.Create(topToday.data.children[i].data.url);                
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                    response.GetResponseStream();
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
                //photo.Save("distortExamie" + i.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
                
                Thread.Sleep(10);

                //UploadPics(topToday);
            }
            lastSuccessPhotoCount = successfullPhotos;

        }

        public void TwitterLogin()
        {
            service = new TwitterService(customerKey, customerSecret, access_token, access_token_secret);
        }
        public void UploadPics(Rootobject pics)
        {
            int picsAmnt = imageList.Count;
            int width = 0;
            int height = 0;

            for (int i = 0; i < imageList.Count; i++)
            {
                try
                {
                    if (pics.data.children[i].data.url.Contains("imgur"))
                    {
                        pics.data.children[i].data.url = pics.data.children[i].data.url.Insert(8, "i.");
                        pics.data.children[i].data.url = pics.data.children[i].data.url + ".jpg";
                        WebRequest imgurRequest = WebRequest.Create(pics.data.children[i].data.url);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Imgur link was not an image. Canceling upload.");
                }
                Bitmap photo;
                try
                {
                    WebRequest request = WebRequest.Create(pics.data.children[i].data.url);
                    WebResponse response = request.GetResponse();
                    System.IO.Stream responseStream =
                    response.GetResponseStream();
                    photo = new Bitmap(responseStream);
                    width = photo.Width;
                    height = photo.Height;
                    photo.Save("temp" + i.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
                }
                catch (Exception)
                {
                    MessageBox.Show("Something went wrong when trying to return the reddit link as a photo into memory. Cancelling this specific upload.");
                }                



                //Bitmap bm = new Bitmap(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                //    + "\\ATBGEBot" + "\\temp" + i.ToString() + ".jpg");
                using (var stream = new FileStream(imageList[i], FileMode.Open))
                {
                    service.SendTweetWithMedia(new SendTweetWithMediaOptions
                    {
                        Status = pics.data.children[i].data.title + "; Uploaded by " + pics.data.children[i].data.author + ". https://www.reddit.com/r/ATBGE/",
                        Images = new Dictionary<string, Stream> { { imageList[i], stream } },
                        PossiblySensitive = pics.data.children[i].data.over_18
                    });
                }
                // Now we wait X hours until posting next picture.
                //await Task.Delay(10000);
                successUploads++;
            }

        }

    }

}
