﻿using InstagramATBGEBot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Threading;
using TweetSharp;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;

namespace ATBGEBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<System.Drawing.Image> glblImgs = new List<System.Drawing.Image>();
        public static List<System.Drawing.Bitmap> photos = new List<System.Drawing.Bitmap>();
        static List<Post> posts = new List<Post>();
        public List<string> imageList = new List<string>();
        int imageIndex = -2;
        public bool running = false;
        public DateTime[] dates;
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterBotATBGE");
        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit="; // 5&t=day       
        public TwitterService service;
        public Rootobject results;
        public int timeBetween;
        private int dateCounter = 0;
        public int whatUploadWereOn = 0;
        public int lastSuccessPhotoCount;
        public string customerKey = "NwhE01jkavtSX5I6gfGmD1Qho";
        public string customerSecret = "oUtI7fqxdUf7u2aIGBmxqnPxND2tUrJlKjEG2L9Cc5kb3jii7P";
        public string access_token = "3564689114-x8Axjs1PH2Fp2wemIEKWtq6jkmJK65QDUhk214M";
        public string access_token_secret = "m1wmP3ZTtlLDrkorFDkHbkgVoI7STFGknCgEWWrfysJ82";

        public MainWindow()
        {           
            InitializeComponent();
            imageIndex = CheckForImage("BOOT");
        }

        private void AutomationBegin()
        {
            int totalUploads = 0;
            this.Dispatcher.Invoke(() =>
            {
                TwitterLogin();
                twittDot.Fill = System.Windows.Media.Brushes.Green;
                twitterLabel.Content = "Logged into Twitter.";
                if (dates is null)
                {
                    dates = UploadDelaySet(imageList.Count, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
                }

                totalUploads = UploadPic(results, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text), imageIndex);
            });                    
            
            Task.Run(() => panelTimeChildAdd(totalUploads));
        }

        private void GetImagesFromUrls(List<string> urlsFromJson)
        {
            System.Drawing.Image[] images = DownloadImagesFromUrls(urlsFromJson); // new Image[totalUploads]; // image array returned.
            if (images != null)
            {
                glblImgs = new List<System.Drawing.Image>();
                foreach (System.Drawing.Image img in images)
                    glblImgs.Add(img);

                for (int i = 0; i < glblImgs.Count; i++)
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg"));
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error setting image box source. Did we find a video? {ex.Message}");
                    }
                }
            }
        }

        private System.Drawing.Image[] DownloadImagesFromUrls(List<string> urls)
        {
            System.Drawing.Image[] imagesReturned = new System.Drawing.Image[urls.Count];

            for (int i = 0; i < urls.Count; i++)
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(urls[0]);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 45000;
                
                WebResponse webResponse = webRequest.GetResponse();
                
                Stream stream = webResponse.GetResponseStream();
                
                imagesReturned[i] = System.Drawing.Image.FromStream(stream);
            }            

            
            return imagesReturned;
        }

        private void TotalPicTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if last character was a letter or 0. If so, undo.
            var txtBox = (TextBox)sender;
            string newText = txtBox.Text.ToString();
            try
            {
                if (newText.Substring(newText.Length - 1, 1) == "0" 
                    || newText.Substring(newText.Length-1, 1) == "1"
                    || newText.Substring(newText.Length - 1, 1) == "2" 
                    || newText.Substring(newText.Length - 1, 1) == "3" 
                    || newText.Substring(newText.Length - 1, 1) == "4" 
                    || newText.Substring(newText.Length - 1, 1) == "5" 
                    || newText.Substring(newText.Length - 1, 1) == "6" 
                    || newText.Substring(newText.Length - 1, 1) == "7" 
                    || newText.Substring(newText.Length - 1, 1) == "8" 
                    || newText.Substring(newText.Length - 1, 1) == "9")
                {
                    // Verify its still a number.
                    int worked;
                    int.TryParse(newText, out worked);                    
                    if (worked == 0)
                    {
                        // 0 means it isnt a number.
                        totalPicTBox.Text = "";
                    }
                    if (newText.Length > 2)
                    {
                        totalPicTBox.Text = newText.Substring(0, 2);
                    }
                }
                else
                {
                    totalPicTBox.Text = txtBox.Text.Substring(0, txtBox.Text.Length - 1);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                totalPicTBox.Text = "";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            panelTimeChildAdd(4);  // add one for now..
        }

        private void panelTimeChildAdd(int uploadCount) // Upload count lets us know a ton of useful info, like how many registry vals to search/add.
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterBotATBGE");

            for (int i = 0; i < uploadCount; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    // --- Label Gen Section --- 
                    StackPanel spL = new StackPanel();
                    spL.Width = OuterStackPanel.ActualWidth;

                    Label lbl = new Label();
                    string cast = (string)key.GetValue($"UploadDate{i}");
                    DateTime dVal = DateTime.Parse(cast);
                    lbl.Content = dVal.Month + "/" + dVal.Day + " " + dVal.TimeOfDay; // This should be a registry call.
                    lbl.BorderBrush = System.Windows.Media.Brushes.Black;
                    lbl.BorderThickness = new Thickness(1, 1, 1, 1);
                    lbl.Width = OuterStackPanel.ActualWidth * .8; // Set the text control to 80%.. 
                    lbl.FontSize = 9;
                    lbl.Height = 35;
                    lbl.Padding = new Thickness(10);
                    lbl.Background = System.Windows.Media.Brushes.CadetBlue;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Center;

                    Rectangle rct = new Rectangle();
                    rct.Width = (int)(OuterStackPanel.ActualWidth * .15);
                    rct.Height = 20;
                    rct.Stroke = System.Windows.Media.Brushes.Black;
                    rct.StrokeThickness = 1;
                    rct.Fill = Brushes.Red;

                    spL.VerticalAlignment = VerticalAlignment.Center;
                    spL.Margin = new Thickness(2, 0, 5, 0);
                    spL.Background = Brushes.White;
                    spL.Orientation = Orientation.Horizontal;
                    spL.Children.Add(lbl);
                    spL.Children.Add(rct);

                    //OuterStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    //OuterStackPanel.Children.Add(lbl);
                    OuterStackPanel.Children.Add(spL);
                });
            }

        }
        
        private int CheckForImage(string passedEnvironnment)
        {
            DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot");

            try
            {
                if (passedEnvironnment == "BOOT") // We're opening the first image possible if found in the local dir created by the app.
                {
                    //if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot"))
                    //{
                    //    for (int i = 0; i < dir.EnumerateFiles().Count(); i++)
                    //    {
                    //        imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    //            + "\\ATBGEBot\\" + $"temp{i}.jpg"));
                    //        if (imageBox.Source != null) // When finally set the source.. end!
                    //        {
                    //            return i;
                    //        }
                    //    }
                    //}
                    return 0;
                }
                else if (passedEnvironnment == "NEXT")
                {
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + $"temp{imageIndex+1}.jpg"))
                    {
                        imageIndex++;
                        imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                            + "\\ATBGEBot\\" + $"temp{imageIndex}.jpg"));
                        if (imageBox.Source != null) // When finally set the source.. end!
                        {
                            return imageIndex;
                        }
                    }
                }
                else if (passedEnvironnment == "PREV")
                {
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + $"temp{imageIndex - 1}.jpg"))
                    {
                        imageIndex--;
                        imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                            + "\\ATBGEBot\\" + $"temp{imageIndex}.jpg"));
                        if (imageBox.Source != null) // When finally set the source.. end!
                        {
                            return imageIndex;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error, probably tried setting the image box's source to a video. Continue as normal. {ex.Message}", "YEET!", MessageBoxButton.OK);
                return -1;
            }
        }

        private void btnImgIndexClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btnImgNext")
            {
                CheckForImage("NEXT");
            }
            else if(btn.Name == "btnImgPrev")
            {
                CheckForImage("PREV");
            }            
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            if (running == true)
            {
                running = false;
            }
            else if(running == false)
            {
                results = GetJsonForToday(totalPicTBox.Text);
                TwitterLogin();
                GetImagesFromResults(results);
                AutomationBegin();
                running = true;
                dates = UploadDelaySet(1, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
                StartTimer();
            }            
        }

        private void StartTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            lblTimer.Content = DateTime.Now.ToShortTimeString();
            if (DateTime.Now > dates[dateCounter])
            {
                UploadPic(results, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text), imageIndex);
                dateCounter++;
            }
        }

        public DateTime[] UploadDelaySet(int totalPics, DateTime startT, DateTime endT)
        {
            DateTime[] timesToPost = new DateTime[int.Parse(totalPicTBox.Text)];
            if (startT > DateTime.Now)
            {
                startT = DateTime.Now;
            }
            TimeSpan total = (endT - startT);
            Double hoursBetween = total.TotalHours; // hours between start and end time of selected values.
            if (hoursBetween < 0 && hoursBetween != 0) // if they selected stupid before and after times.. inverse the negative number.
            {
                hoursBetween = (hoursBetween * -1); // make the neg a pos.
            }
            double rate = 0;
            if (int.Parse(totalPicTBox.Text) == 1)
            {
                rate = 1.0; // how often were going to post a picture.. only one picture so no rate?
            }
            else
            {
                rate = (hoursBetween / int.Parse(totalPicTBox.Text)); //(hoursBetween / totalPics); // how often were going to post a picture.
                //rate += (rate / imageList.Count);
                int notImageCount = totalPics - int.Parse(totalPicTBox.Text);
                //double rateToMultipleRateBy = (double)totalPics / (double)imageList.Count;
                //rate = rateToMultipleRateBy * rate;
            }

            int closestValRate = (int)(3600000 * rate);
            timeBetween = closestValRate;

            DateTime postAt;
            for (double i = 0; i < int.Parse(totalPicTBox.Text); i++)
            {
                if (i == 0)
                {

                    postAt = startT;
                    timesToPost[(int)i] = postAt.AddHours(rate); // The first date should be affected by the rate. Otherwise, the last date is early!
                }
                else
                {
                    postAt = startT.AddHours((i+1) * rate);
                    timesToPost[(int)i] = postAt;
                }
            }

            return timesToPost;
            //Thread.Sleep(closestVal);
            //await Task.Delay(closestVal); // closestVal
        }

        public int UploadPic(Rootobject pic, DateTime startT, DateTime endT, int imageOfResultsIndex)
        {            
            for (int i = 0; i < dates.Length; i++)
            {
                key.SetValue($"UploadDate{i}", $"{dates[i]}");
            }

            int picsAmnt = imageList.Count;
            int width = 0;
            int height = 0;
            string errConsoleOutPut = "";

            try
            {
                if (pic.data.children[imageOfResultsIndex].data.url.Contains("imgur"))
                {
                    pic.data.children[imageOfResultsIndex].data.url = pic.data.children[imageOfResultsIndex].data.url.Insert(8, "i.");
                    pic.data.children[imageOfResultsIndex].data.url = pic.data.children[imageOfResultsIndex].data.url + ".jpg";
                }
            }
            catch (Exception)
            {
                errConsoleOutPut += "Imgur link was not an image. Canceling upload. \n";
            }
            System.Drawing.Bitmap photo;
            try
            {
                WebRequest request = WebRequest.Create(pic.data.children[imageOfResultsIndex].data.url);
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream =
                response.GetResponseStream();
                photo = new System.Drawing.Bitmap(responseStream);
                width = photo.Width;
                height = photo.Height;
                photo.Save("jargobargo" + 0.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg); // to debug local files saved
            }
            catch (Exception)
            {
                errConsoleOutPut += "Something went wrong when trying to return the reddit link as a photo into memory. Cancelling this specific upload.";
            }

            using (var stream = new FileStream(imageList[imageOfResultsIndex], FileMode.Open))
            {
                try
                {
                    service.SendTweetWithMedia(new SendTweetWithMediaOptions
                    {
                        Status = pic.data.children[imageOfResultsIndex].data.title + "; Uploaded by " + pic.data.children[imageOfResultsIndex].data.author + ". https://www.reddit.com/r/ATBGE/",
                        Images = new Dictionary<string, Stream> { { imageList[imageOfResultsIndex], stream } },
                        PossiblySensitive = pic.data.children[0].data.over_18
                    });
                }
                catch (NullReferenceException e)
                {
                    errConsoleOutPut += ("Null reference error when uploading to reddit.com" + pic.data.children[imageOfResultsIndex].data.permalink) + "\n";
                    errConsoleOutPut += $"Is_video: {pic.data.children[imageOfResultsIndex].data.is_video}; \n";
                    errConsoleOutPut += $"Trying to post this local file: {imageList[imageOfResultsIndex]} \n";
                }
            }

            imageIndex++;
            imageOfResultsIndex++;
            return imageList.Count;
        }

        public void TwitterLogin()
        {
            service = new TwitterService(customerKey, customerSecret, access_token, access_token_secret);
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

        public List<string> GetImagesFromResults(Rootobject topToday)
        {
            imageList = new List<string>();
            photos = new List<System.Drawing.Bitmap>();

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
                catch (Exception ex)
                {
                    MessageBox.Show($@"The temp dir can't at Documents\ATGBEBOT\ can't be deleted. Contact a developer please. {ex.Message}");
                }
            }
            dir.Create();

            for (int i = 0; i < resultAmnt; i++)
            {
                WebRequest request;
                if (topToday.data.children[i].data.url.Contains("imgur"))
                {
                    request = WebRequest.Create(topToday.data.children[i].data.url + ".jpg");
                }
                else
                {
                    request = WebRequest.Create(topToday.data.children[i].data.url);
                }
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream = response.GetResponseStream();
                System.Drawing.Bitmap photo;
                try
                {
                    photo = new System.Drawing.Bitmap(responseStream);
                    photos.Add(photo);
                    photo.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg");
                    imageList.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg");
                    successfullPhotos++;
                }
                catch (Exception ex)
                {

                }
            }
            lastSuccessPhotoCount = successfullPhotos;

            return imageList;
        }

    }
}
