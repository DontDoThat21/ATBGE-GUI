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
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes.Models;

namespace InstagramATBGEBot
{
    public class InstagramBot
    {
        private string igUsername = "hlsmurf1";
        private string igPassword = "Bluebunny1";

        public int lastSuccessPhotoCount;

        //private static UserSessionData user;
        private static IInstaApi InstaApi;

        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit=5&t=day";
        public static List<Bitmap> photos = new List<Bitmap>();
        public Rootobject results;
        public static bool igLoginSuccess;

        public InstagramBot()
        {

            results = GetJsonForToday();
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

        public void TakeImagesFromResults(Rootobject topToday)
        {
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
                    photo.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + "\\temp" + i.ToString() + ".jpg");
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

        public async void InstagramLogin()
        {
            var userSession = new UserSessionData
            {
                UserName = igUsername,
                Password = igPassword
            };

            InstaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                .Build();
        }
        public async void UploadPics(Rootobject pics)
        {
            int picsAmnt = (int)pics.data.children.GetLongLength(0);
            int width = 0;
            int height = 0;

            UserSessionData user = new UserSessionData();
            user.UserName = igUsername;
            user.Password = igPassword;
            InstagramLogin();

            for (int i = 0; i < picsAmnt; i++)
            {
                WebRequest request = WebRequest.Create(pics.data.children[i].data.url);
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                    response.GetResponseStream();
                Bitmap photo = new Bitmap(responseStream);
                width = photo.Width;
                height = photo.Height;

                photo.Save("temp" + i.ToString() + ".jpg", ImageFormat.Jpeg); // to debug local files saved
                //InstaImage img = new InstaImage(pics.data.children[i].data.url, width, height);
                InstaImageUpload img = new InstaImageUpload(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    + "\\ATBGEBot"
                    + "\\temp" + i.ToString() + ".jpg"
                    ,
                    width, height);

                var result = await InstaApi.MediaProcessor.UploadPhotoAsync(img, "Uploaded by: " + // .UploadImage
                    pics.data.children[i].data.author + "; " +
                    pics.data.children[i].data.title);

            }

        }

    }

}
