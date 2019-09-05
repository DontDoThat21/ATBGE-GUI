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
using InstaSharper;
using InstaSharper.Classes;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Logger;
using System;
using System.IO;
using InstaSharper.Classes.Models;
using System.Threading;
using System.Drawing.Imaging;

namespace InstagramATBGEBot
{
    class InstagramBot
    {
        private const string username = "hlsmurf1";
        private const string password = "Bluebunny1";
        private static UserSessionData user;
        private static IInstaApi api;

        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit=5&t=day";
        public static List<Bitmap> photos = new List<Bitmap>();

        public InstagramBot()
        {
            Rootobject results = GetJsonForToday();
            TakeImagesFromResults(results);
        }

        static Rootobject GetJsonForToday()
        {
            Uri reddit = new Uri(redditWorkingUrl);
            HttpClient client = new HttpClient();
            string json = new WebClient().DownloadString(reddit);
            Rootobject objectResponse = JsonConvert.DeserializeObject<Rootobject>(json);
            return objectResponse;
        }

        static void TakeImagesFromResults(Rootobject topToday)
        {
            int resultAmnt = (int)topToday.data.children.GetLongLength(0);
            for (int i = 0; i < resultAmnt; i++)
            {
                WebRequest request = WebRequest.Create(topToday.data.children[i].data.url);
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                    response.GetResponseStream();
                Bitmap photo = new Bitmap(responseStream);
                //photo.Save("distortExamie" + i.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
                photos.Add(photo);

                user = new UserSessionData();
                user.UserName = username;
                user.Password = password;
                Login();
                Thread.Sleep(3000);

                UploadPics(topToday);

                Console.Read();
            }
        }

        public static async void Login()
        {
            api = InstaApiBuilder.CreateBuilder()
                .SetUser(user)
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .Build();

            var loginRequest = await api.LoginAsync();
            if (loginRequest.Succeeded)
                Console.WriteLine("Logged in successfully.");
            else
                Console.WriteLine("Error logging in!" + loginRequest.Info.Message);
        }
        public static async void UploadPics(Rootobject pics)
        {
            int picsAmnt = (int)pics.data.children.GetLongLength(0);
            int width = 0;
            int height = 0;

            for (int i = 0; i < picsAmnt; i++)
            {

                WebRequest request = WebRequest.Create(pics.data.children[i].data.url);
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                    response.GetResponseStream();
                Bitmap photo = new Bitmap(responseStream);
                width = photo.Width;
                height = photo.Height;

                photo.Save("exampleSaved1" + ".jpg", ImageFormat.Jpeg); // to debug local files saved
                //InstaImage img = new InstaImage(pics.data.children[i].data.url, width, height);
                InstaImage img = new InstaImage(@"C:\Users\ttrub\Desktop\InstagramATBGEBot\InstagramATBGEBot\InstagramATBGEBot\bin\Debug\exampleSaved1.jpg",
                    width, height);
                
                var result = await api.UploadPhotoAsync(img, "Uploaded by: " + 
                    pics.data.children[i].data.author + "; " +
                    pics.data.children[i].data.title);

            }

        }

    }

}
