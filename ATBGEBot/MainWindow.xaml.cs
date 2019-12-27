using InstagramATBGEBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Threading;
using TweetSharp;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ATBGEBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TwitterBot twtr;
        List<System.Drawing.Image> glblImgs = new List<System.Drawing.Image>();
        public List<string> imageList = new List<string>();
        int imageIndex = -2;
        public bool running = false;
        public DateTime[] dates;
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterBotATBGE");
        public TwitterService service;
        private int successUploads = 0;
        public int timeBetween;
        private int dateCounter = 0;


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
                twtr = new TwitterBot(totalPicTBox.Text);
                twtr.TwitterLogin();
                twittDot.Fill = System.Windows.Media.Brushes.Green;
                twitterLabel.Content = "Logged into Twitter.";
                totalUploads = UploadPic(twtr.results, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
            });                    
            
            Task.Run(() => panelTimeChildAdd(totalUploads));
            //panelTimeChildAdd(totalUploads); // Generating text box's now.

            List<string> urlsFromJson = new List<string>();

            for (int i = 0; i < twtr.results.data.children.Length; i++)
            {
                urlsFromJson.Add(twtr.results.data.children[i].data.url);
            }

            Task.Run(() => SetGlobalImages(urlsFromJson));
            //PostTimerCheck();
        }

        private void SetGlobalImages(List<string> urlsFromJson)
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
                    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot"))
                    {
                        for (int i = 0; i < dir.EnumerateFiles().Count(); i++)
                        {
                            imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                                + "\\ATBGEBot\\" + $"temp{i}.jpg"));
                            if (imageBox.Source != null) // When finally set the source.. end!
                            {
                                return i;
                            }
                        }
                    }
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
                running = true;

                StartTimer();
            }
            Task.Run(AutomationBegin);
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
                UploadPic();
            }
        }

        public DateTime[] UploadDelaySet(int totalPics, DateTime startT, DateTime endT)
        {
            DateTime[] timesToPost = new DateTime[imageList.Count];
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
            if (imageList.Count == 1)
            {
                rate = (hoursBetween / imageList.Count); // how often were going to post a picture.. only one picture so no rate?
            }
            else
            {
                rate = 1.0; //(hoursBetween / totalPics); // how often were going to post a picture.
                //rate += (rate / imageList.Count);
                int notImageCount = totalPics - imageList.Count;
                //int whateverTheFuckThisIs = notImageCount / 
                //double rateToMultipleRateBy = (double)totalPics / (double)imageList.Count;
                //rate = rateToMultipleRateBy * rate;


            }

            int closestValRate = (int)(3600000 * rate);
            timeBetween = closestValRate;

            DateTime postAt;
            for (double i = 0; i < imageList.Count; i++)
            {
                if (i == 0)
                {
                    postAt = startT;
                    timesToPost[(int)i] = postAt;
                }
                else
                {
                    postAt = startT.AddHours(i * rate);
                    timesToPost[(int)i] = postAt;
                }
            }


            return timesToPost;
            //Thread.Sleep(closestVal);
            //await Task.Delay(closestVal); // closestVal
        }

        public int UploadPic(Rootobject pic, DateTime startT, DateTime endT)
        {
            dates = UploadDelaySet(1, startT, endT);
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
                if (pic.data.children[0].data.url.Contains("imgur"))
                {
                    pic.data.children[0].data.url = pic.data.children[0].data.url.Insert(8, "i.");
                    pic.data.children[0].data.url = pic.data.children[0].data.url + ".jpg";
                    WebRequest imgurRequest = WebRequest.Create(pic.data.children[0].data.url);
                }
            }
            catch (Exception)
            {
                errConsoleOutPut += "Imgur link was not an image. Canceling upload. \n";
            }
            System.Drawing.Bitmap photo;
            try
            {
                WebRequest request = WebRequest.Create(pic.data.children[0].data.url);
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

            using (var stream = new FileStream(imageList[0], FileMode.Open))
            {
                try
                {
                    service.SendTweetWithMedia(new SendTweetWithMediaOptions
                    {
                        Status = pic.data.children[0].data.title + "; Uploaded by " + pic.data.children[0].data.author + ". https://www.reddit.com/r/ATBGE/",
                        Images = new Dictionary<string, Stream> { { imageList[0], stream } },
                        PossiblySensitive = pic.data.children[0].data.over_18
                    });
                }
                catch (NullReferenceException e)
                {
                    errConsoleOutPut += ("Null reference error when uploading to reddit.com" + pic.data.children[0].data.permalink) + "\n";
                    errConsoleOutPut += $"Is_video: {pic.data.children[0].data.is_video}; \n";
                    errConsoleOutPut += $"Trying to post this local file: {imageList[0]} \n";
                }
            }
            successUploads++;

            imageIndex++;
            return imageList.Count;
        }
    }
}
