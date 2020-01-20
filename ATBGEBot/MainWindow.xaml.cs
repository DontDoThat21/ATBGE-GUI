using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Deployment;
using System.Deployment.Internal;
using TweetSharp;

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
        public bool missingApiKeys;
        public DateTime[] dates;
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
        public static string redditWorkingUrl = "https://www.reddit.com/r/ATBGE/top/.json?limit="; // 5&t=day       
        public TwitterService service;
        public Rootobject results;
        public int timeBetween;
        public int totalRequestedUploads;
        private int dateCounter = 0;
        public int whatUploadWereOn = 0;
        public int lastSuccessPhotoCount;
        public int videoOffset = 0;
        //public string customerKey = "NwhE01jkavtSX5I6gfGmD1Qho";
        //public string customerSecret = "oUtI7fqxdUf7u2aIGBmxqnPxND2tUrJlKjEG2L9Cc5kb3jii7P";
        //public string access_token = "3564689114-x8Axjs1PH2Fp2wemIEKWtq6jkmJK65QDUhk214M";
        //public string access_token_secret = "m1wmP3ZTtlLDrkorFDkHbkgVoI7STFGknCgEWWrfysJ82";
        
        public string apiKey = "v1tlVp3XwmZhjKrmc04qRBYbc";
        public string apiSecret = "IPbslteDjQoGNx5tJS6QNmBvoCoq65Nh2E9TPQBGDLpor1jtUr";
        public string accessToken = "947618558347022336-OXuymBoyTX3tIwziX8g51tSMtXasV7S";
        public string accessTokenSecret = "2yojE4I2niAzqdM94vae6MOMIK17XDsXChFcw45iDWRe3";

        public DateTime dayAt;
        public DateTime dayFirstRun;

        public MainWindow()
        {           
            InitializeComponent();
            AuditThisLogin();
            imageIndex = CheckForImage("BOOT");
            //startTCBox.SelectedIndex = 0;
            //endTCBox.SelectedIndex = endTCBox.Items.Count-5;
            StartTimer();
            int lnchCnt = int.Parse(key.GetValue("AppLaunchCount", 0).ToString());
            if (lnchCnt == 0)
            {
                key.SetValue("AppLaunchCount", lnchCnt + 1);
                MessageBox.Show("Welcome to your automated Twitter Poster! Please set the the api keys (instructions included) at the top right before continuing. The app needs these to continue.", "Welcome!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                key.SetValue("AppLaunchCount", lnchCnt + 1);
            }
        }

        private void AuditThisLogin()
        {

            TwitterPoster.AppAudit.GPJ_AppAudit appAudit = new TwitterPoster.AppAudit.GPJ_AppAudit();

            string[] environ = new string[]
            {
                "ENVIRONMENT=PROD",
                "USERID=TWTRPSTR",
                "FULLNAME=TWITTER POSTER"
            };
            string[] vars = GetAuditParams();

            int major = Environment.OSVersion.Version.Major;
            int minor = Environment.OSVersion.Version.Minor;
            bool bitBool = Environment.Is64BitOperatingSystem; int osBit = -1;

            if (bitBool)
            {
                osBit = 64;
            }
            else
            {
                osBit = 32;
            }

            var result = appAudit.SaveUserData(vars[0], vars[1], vars[2], vars[3], vars[4], major, minor, osBit, vars[5], vars[6], vars[7], environ);
        }

        private string[] GetAuditParams()
        {
            bool bitBool = Environment.Is64BitOperatingSystem; int bit = -1;

            if (bitBool)
            {
                bit = 64;
            }
            else
            {
                bit = 32;
            }

            string userLogon = Environment.MachineName.Substring(0, 8);
            string machineName = "DET" + Environment.MachineName.Substring(3, 5);
            if (userLogon.ToUpper().Contains("TTRUB")) // Literally since we're using GPJ WS, lets see if the dev is opening app.. if so? Edit to app_logon name always!
            {
                userLogon = "TTRUB";
                machineName = "DETTTRUB902055";
            }
            string[] auditVars = new string[8]
            {
                userLogon,
                "GPJ",
                machineName,
                Environment.OSVersion.ToString(),
                Environment.OSVersion.Platform.ToString(),
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                TimeZone.CurrentTimeZone.StandardName.ToString(),
            };

            return auditVars;            
        }

        private async void AutomationBegin()
        {
            int totalUploads = 0;
            this.Dispatcher.Invoke(() =>
            {
                missingApiKeys = TwitterLogin();
                if (!missingApiKeys)
                {
                    twittDot.Fill = System.Windows.Media.Brushes.Green;
                    twitterLabel.Content = "Logged into Twitter.";
                    if (dates is null)
                    {
                        dates = UploadDelaySet(imageList.Count, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
                    }

                    totalUploads = imageList.Count;
                }
            });

            if (!missingApiKeys)
            {
                if (imageList.Count < int.Parse(totalPicTBox.Text))
                {
                    MessageBox.Show("How the hell did we not download enough images? The bot somehow isn't downloading enough from reddit; please contact a developer. The app will continute to run, but very buggily. This wasn't accounted for, dammit!");
                }
                else
                {
                    PopulateScheduledUploadsDraws();
                    StackPanel stackPanel = (StackPanel)OuterStackPanel.Children[0];
                    Label child = (Label)stackPanel.Children[0];
                    lblNextUploadTime.Content = child.Content;
                }
            }
        }

        public void PopulateScheduledUploadsDraws()
        {
                panelTimeChildAdd(totalRequestedUploads);
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
           // panelTimeChildAdd(4);  // add one for now..
        }

        private void panelTimeChildAdd(int uploadCount) // Upload count lets us know a ton of useful info, like how many registry vals to search/add.
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            var brsh = new SolidColorBrush(Color.FromArgb(20, 78, 155, 241));

            for (int i = 0; i < uploadCount; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    // --- Label Gen Section --- 
                    StackPanel spL = new StackPanel();
                    spL.Name = "datesCollection";
                    spL.Width = OuterStackPanel.ActualWidth;

                    Label lbl = new Label();
                    lbl.Name = $"dateVal{i}";
                    string cast = (string)key.GetValue($"UploadDate{i}");
                    DateTime dVal = DateTime.Parse(cast);
                    lbl.Content = dVal.Month + "/" + dVal.Day + " " + dVal.TimeOfDay; // This should be a registry call.
                    lbl.BorderBrush = System.Windows.Media.Brushes.Black;
                    lbl.BorderThickness = new Thickness(1, 1, 1, 1);
                    lbl.Width = OuterStackPanel.ActualWidth * .8; // Set the text control to 80%.. 
                    lbl.FontSize = 9;
                    lbl.Height = 35;
                    lbl.Padding = new Thickness(10);
                    lbl.Background = brsh;
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
                    //MessageBox.Show("\"Fixed\" the issue that errored when starting was temporarily fixed, but not images won't open on start.");
                    return 0;
                    //if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot"))
                    //{
                    //    string[] photoFileNames = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot");
                    //    string pattern = @"\d";
                    //    StringBuilder sb = new StringBuilder();
                    //    int[] photoIndexNums = new int[photoFileNames.Length];

                    //    for (int i = 0; i < photoFileNames.Length; i++)
                    //    {
                    //        foreach (Match m in Regex.Matches(photoFileNames[i], pattern))
                    //        {
                    //            sb.Append(m);
                    //        }
                    //        photoIndexNums[i] = int.Parse(sb.ToString());
                    //        sb = new StringBuilder();
                    //    }

                    //    int lowest = photoIndexNums.Min();

                    //    imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    //        + "\\ATBGEBot\\" + $"temp{lowest}.jpg"));
                    //}
                    //return 0;
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
            //OuterStackPanel = new StackPanel();
            imageBox.Source = null;
            totalRequestedUploads = int.Parse(totalPicTBox.Text);
            dates = UploadDelaySet(imageList.Count, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
            dayAt = DateTime.Now;
            dayFirstRun = DateTime.Now;
            lastRunLabel.Content = "Turned on at: " + dayFirstRun.ToShortTimeString() + " " + dayFirstRun.Month.ToString() + "/" + dayFirstRun.Day.ToString();

            if (running == true)
            {
                running = false;
                btnBegin.Content = "Begin";                
            }
            else if (running == false)
            {
                PopulateScheduledUploadsDraws();
                running = true;
                results = GetJsonForToday(totalPicTBox.Text);
                TwitterLogin();
                GetImagesFromResults(results);
                AutomationBegin();
                btnBegin.Content = "Stop";
                dateCounter = 0;
                //StartTimer();
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
            lblTimer.Content = "Now: " + DateTime.Now.ToShortTimeString();
            if (DateTime.Now.DayOfWeek.ToString().Equals(dayAt.DayOfWeek.ToString()) // It's the same say as when we last started the cycle.
                && dateCounter <= int.Parse(totalPicTBox.Text)-1
                && running == true) // The total pictures the user enters, say 5, isn't == to the index for the date array associated for each upload since 0 based.
            {
                try
                {
                    if (DateTime.Now > dates[dateCounter]) // was not -1
                    {
                        if (dateCounter >= totalRequestedUploads) // was +1
                        {
                            // i dont believe anything should exec.
                        }
                        else
                        {
                            UploadPic(posts, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text), imageIndex);
                            if (dateCounter == totalRequestedUploads)
                            {
                            }
                            else
                            {
                                dateCounter++;
                            }
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    dates = UploadDelaySet(imageList.Count, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
                }
            }
            else if (DateTime.Now.DayOfWeek.ToString().Equals(dayAt.DayOfWeek.ToString()) == false) // If the day we clicked the begin is not the same day as the current tick of the timer.. we must have reset. It's probably midnight. Lets reget our photos for the day, AND set up our timers given last params.
            {
                imageIndex = 0; // Probably reset everything, yes?
                dayAt = DateTime.Now;
                results = GetJsonForToday(totalPicTBox.Text); // This should be re-get/setting the root object or photos object at midnight.
                // With all of this being reset.. Can't we just call upload pic? Well, we wouldn't want to call it here since after this SINGLE midnight tick being entered, 
                // it will make the next tick a second later begin the cycle! 
                // Dank.

                // One thing definitely not doing that should be done.. that stack panel should be cleared first no?
                OuterStackPanel = new StackPanel(); // This should re-init the panel and remove hopefully previous day drawings.
            }
            else
            {
                // When we've uploaded all the pics for the day, and the next day still hasn't begun.
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

        public void UploadPic(List<Post> posts, DateTime startT, DateTime endT, int imageOfResultsIndex)
        {            
            for (int i = 0; i < dates.Length; i++)
            {
                key.SetValue($"UploadDate{i}", $"{dates[i]}");
            }

            string errConsoleOutPut = "";

            try
            {
                if (posts[imageOfResultsIndex].Url.Contains("imgur"))
                {
                    posts[imageOfResultsIndex].Url = posts[imageOfResultsIndex].Url.Insert(8, "i.");
                    posts[imageOfResultsIndex].Url = posts[imageOfResultsIndex].Url + ".jpg"; // Tried, but never seen an imgur link fail to open with jpg. So, hard coding.
                }
            }
            catch (Exception)
            {
                errConsoleOutPut += "Imgur link was not an image. Canceling upload. \n";
            }
            System.Drawing.Bitmap photo;

            bool failedAtFirstUp = false;
            if (imageOfResultsIndex - videoOffset < 0)
            {
                imageOfResultsIndex++;
                failedAtFirstUp = true;
            }
            using (var stream = new FileStream(imageList[imageOfResultsIndex - videoOffset], FileMode.Open))
            {
                if (failedAtFirstUp)
                {
                    imageOfResultsIndex--;
                }
                try
                {
                    if (posts[imageOfResultsIndex].isVideo == true)
                    {
                        imageOfResultsIndex++; // Skip videos for now.
                    }
                    else
                    {
                        service.SendTweetWithMedia(new SendTweetWithMediaOptions
                        {
                            Status = posts[imageOfResultsIndex].title + "; Uploaded by /u/" + posts[imageOfResultsIndex].author + ". https://www.reddit.com/r/ATBGE/",
                            Images = new Dictionary<string, Stream> { { imageList[imageOfResultsIndex - videoOffset], stream } },
                            PossiblySensitive = posts[imageOfResultsIndex].over18
                        });
                        consoleTBox.AppendText($"\nUploaded {posts[imageOfResultsIndex].title} at {DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString()}.");
                        StackPanel stackPanel = new StackPanel();
                        try
                        {
                            stackPanel = (StackPanel)OuterStackPanel.Children[imageOfResultsIndex];
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            PopulateScheduledUploadsDraws();
                            stackPanel = (StackPanel)OuterStackPanel.Children[imageOfResultsIndex];
                        }
                        Rectangle child = (Rectangle)stackPanel.Children[1];
                        child.Fill = Brushes.Green;
                        //item.Stroke = Brushes.Green;
                        // do some magic and bullshit with the panel generated.
                        uploadCountLabel.Content = $"Photos uploaded: {imageIndex + 1}";
                        uploadRemainLabel.Content = $"Remaining: {totalRequestedUploads - (imageIndex + 1)}";
                    }                    
                }
                catch (NullReferenceException vidErr)
                {
                    errConsoleOutPut += ("Null reference error when uploading to reddit.com" + posts[imageOfResultsIndex].Url) + "\n";
                    errConsoleOutPut += $"Is_video: {posts[imageOfResultsIndex].isVideo}; \n";
                    errConsoleOutPut += $"Trying to post this local file: {imageList[imageOfResultsIndex]} \n";
                }
            }

            imageIndex++;
            imageOfResultsIndex++;

            if (string.IsNullOrEmpty(errConsoleOutPut))
            {
                consoleTBox.AppendText("\n" + errConsoleOutPut);
            }
        }

        public bool TwitterLogin()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            apiKey = key.GetValue("ApiKey", "EmptyAPIK!").ToString(); // Gets the key, if it's exist. If it doesn't? Returns "Empty!" as a string.
            apiSecret = key.GetValue("ApiSecret", "EmptyAPIS!").ToString();
            accessToken = key.GetValue("AccessToken", "EmptyACCT!").ToString();
            accessTokenSecret = key.GetValue("AccessTokenSecret", "EmptyACCTS!").ToString();
            bool failedKeys = false;

            string[] keys = new string[]
            {
                apiKey,
                apiSecret,
                accessToken,
                accessTokenSecret
            };

            for (int i = 0; i < keys.Length; i++) // lets loop through the params and make sure alls
            {
                if (keys[i] == "EmptyAPIK!" || keys[i] == "EmptyAPIS!" || keys[i] == "EmptyACCT!" || keys[i] == "EmptyACCTS!")
                {
                    if (keys[i] == "EmptyAPIK!")
                    {
                        MessageBox.Show(@"Missing Api key. Did you set up all four keys in the top right login clickable form? Look at registry if certain you're correct: Computer\HKEY_CURRENT_USER\Software\TwitterPoster", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (keys[i] == "EmptyAPIS!")
                    {
                        MessageBox.Show(@"Missing Api secret. Did you set up all four keys in the top right login clickable form? Look at registry if certain you're correct: Computer\HKEY_CURRENT_USER\Software\TwitterPoster", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (keys[i] == "EmptyACCT!")
                    {
                        MessageBox.Show(@"Missing Api key. Did you set up all four keys in the top right login clickable form? Look at registry if certain you're correct: Computer\HKEY_CURRENT_USER\Software\TwitterPoster", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (keys[i] == "EmptyACCTS!")
                    {
                        MessageBox.Show(@"Missing Api key. Did you set up all four keys in the top right login clickable form? Look at registry if certain you're correct: Computer\HKEY_CURRENT_USER\Software\TwitterPoster", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    failedKeys = true;
                }
            }

            if (failedKeys)
            {
                MessageBox.Show("Since we have missing keys, we can't auth. with twitter. Cancelling uploads.", "Stopping.", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                service = new TwitterService(apiKey, apiSecret, accessToken, accessTokenSecret);
                return true;
            }
        }

        Rootobject GetJsonForToday(string imgCount)
        {
            int paddedResultCount = int.Parse(imgCount) + 10;

            Uri reddit = new Uri(redditWorkingUrl + paddedResultCount + "&t=day");
            HttpClient client = new HttpClient();
            string json = new WebClient().DownloadString(reddit);
            Rootobject objectResponse;
            try
            {
                objectResponse = JsonConvert.DeserializeObject<Rootobject>(json);
                foreach (var item in objectResponse.data.children)
                {
                    Post post = new Post();
                    post.title = item.data.title;
                    post.author = item.data.author;
                    post.Url = item.data.url;
                    post.isVideo = item.data.is_video;
                    if (post.isVideo)
                    {
                        videoOffset++;
                    }
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
            bool exists = System.IO.Directory.Exists(
                
                
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot");
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
                    if (topToday.data.children[i].data.is_video == true)
                    {
                        continue;
                        // skip cus its a video? videos are for normies. maybe well code that later.
                    }
                    else
                    {
                        request = WebRequest.Create(topToday.data.children[i].data.url + ".jpg");
                    }
                }
                else
                {
                    if (topToday.data.children[i].data.is_video == true)
                    {
                        continue;
                    }
                    else
                    {
                        request = WebRequest.Create(topToday.data.children[i].data.url);
                    }
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

        private void imageBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ATBGEBot\\" + "temp" + imageIndex.ToString() + ".jpg");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Somehow failed to open the image at {Environment.SpecialFolder.MyDocuments + "\\ATBGEBot\\" + "temp" + imageIndex.ToString() + ".jpg"} Please contact a developer. May be because file is not a JPG and is another format? {ex.Message}.", "Error!");
                consoleTBox.AppendText($"Failed opening image {Environment.SpecialFolder.MyDocuments + "\\ATBGEBot\\" + "temp" + imageIndex.ToString() + ".jpg"} at {DateTime.Now.ToShortTimeString() + DateTime.Now.ToShortDateString()}");
            }
        }

        private void mysteryMenu_OpenHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExtrasWindow tomWindow = new ExtrasWindow();
            tomWindow.Show();
        }

        private void editTwitterLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TwitterDetails twtrDets = new TwitterDetails();
            twtrDets.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Twitter Poster 1.0.2.4"; // Should be assembly version... publish version...
        }

        //private void GetImagesFromUrls(List<string> urlsFromJson)
        //{
        //    System.Drawing.Image[] images = DownloadImagesFromUrls(urlsFromJson); // new Image[totalUploads]; // image array returned.
        //    if (images != null)
        //    {
        //        glblImgs = new List<System.Drawing.Image>();
        //        foreach (System.Drawing.Image img in images)
        //            glblImgs.Add(img);

        //        for (int i = 0; i < glblImgs.Count; i++)
        //        {
        //            try
        //            {
        //                this.Dispatcher.Invoke(() =>
        //                {
        //                    imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
        //                "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg"));
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error setting image box source. Did we find a video? {ex.Message}");
        //            }
        //        }
        //    }
        //}

        //private System.Drawing.Image[] DownloadImagesFromUrls(List<string> urls)
        //{
        //    System.Drawing.Image[] imagesReturned = new System.Drawing.Image[urls.Count];

        //    for (int i = 0; i < urls.Count; i++)
        //    {
        //        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(urls[0]);
        //        webRequest.AllowWriteStreamBuffering = true;
        //        webRequest.Timeout = 45000;

        //        WebResponse webResponse = webRequest.GetResponse();

        //        Stream stream = webResponse.GetResponseStream();

        //        imagesReturned[i] = System.Drawing.Image.FromStream(stream);
        //    }            


        //    return imagesReturned;
        //}
    }
}
