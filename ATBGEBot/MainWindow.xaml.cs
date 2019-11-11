using InstagramATBGEBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ATBGEBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TwitterBot twtr;
        List<System.Drawing.Image> glblImgs = new List<System.Drawing.Image>();
        int imageIndex = -2;

        public MainWindow()
        {           
            InitializeComponent();
            imageIndex = CheckForImage("BOOT");
        }

        private void AutomationBegin(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            twtr = new TwitterBot(totalPicTBox.Text);
            twtr.TwitterLogin();
            twittDot.Fill = Brushes.Green;
            twitterLabel.Content = "Logged into Twitter.";
            int totalUploads = twtr.UploadPics(int.Parse(totalPicTBox.Text), twtr.results, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
            panelTimeChildAdd(totalUploads); // Generating text box's now.

            List<string> urlsFromJson = new List<string>();

            for (int i = 0; i < twtr.results.data.children.Length; i++)
            {
                urlsFromJson.Add(twtr.results.data.children[i].data.url);
            }

            SetGlobalImages(urlsFromJson);

            


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
                        imageBox.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        "\\ATBGEBot\\" + "temp" + i.ToString() + ".jpg"));
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
                // --- Label Gen Section --- 
                StackPanel spL = new StackPanel();
                spL.Width = OuterStackPanel.ActualWidth;

                Label lbl = new Label();
                string cast = (string)key.GetValue($"UploadDate{i}");
                DateTime dVal = DateTime.Parse(cast);
                lbl.Content = dVal.Month + "/" + dVal.Day + " " + dVal.TimeOfDay; // This should be a registry call.
                lbl.BorderBrush = Brushes.Black;
                lbl.BorderThickness = new Thickness(1, 1, 1, 1);
                lbl.Width = OuterStackPanel.ActualWidth*.8; // Set the text control to 80%.. 
                lbl.FontSize = 9;
                lbl.Height = 35;
                lbl.Padding = new Thickness(10);
                lbl.Background = Brushes.CadetBlue;
                lbl.HorizontalContentAlignment = HorizontalAlignment.Center;

                Rectangle rct = new Rectangle();
                rct.Width = OuterStackPanel.ActualWidth * .15;
                rct.Height = 20;
                rct.Stroke = Brushes.Black;
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


        private void PostTimerCheck()
        {
            DateTime dt = DateTime.Parse(endTCBox.Text);
            if (DateTime.Now >= dt)
            {
                // End..

            }
            else
            {
                consoleTBox.AppendText(Environment.NewLine + "Finished at:" + DateTime.Now);
                PostTimerCheck();
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
    }
}
