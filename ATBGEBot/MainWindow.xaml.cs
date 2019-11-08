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

namespace ATBGEBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TwitterBot twtr;
        public int uithrdImageCount = 0;
        public MainWindow()
        {
           
            InitializeComponent();
        }

        private void Automation_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                twtr = new TwitterBot(totalPicTBox.Text);
                twtr.TwitterLogin();
                twittDot.Fill = Brushes.Green;
                twitterLabel.Content = "Logged into Twitter.";
                int totalUploads = twtr.UploadPics(int.Parse(totalPicTBox.Text), twtr.results, DateTime.Parse(startTCBox.Text), DateTime.Parse(endTCBox.Text));
                panelTimeChildAdd(totalUploads); // Generating text box's now.
            }
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
                lbl.Content = key.GetValue($"UploadDate{i}"); // This should be a registry call.
                lbl.BorderBrush = Brushes.Black;
                lbl.BorderThickness = new Thickness(1, 1, 1, 1);
                lbl.Width = OuterStackPanel.ActualWidth*.8; // Set the text control to 80%.. 
                lbl.Height = 35;
                lbl.Padding = new Thickness(10);
                lbl.Background = Brushes.CadetBlue;
                lbl.HorizontalContentAlignment = HorizontalAlignment.Center;

                spL.VerticalAlignment = VerticalAlignment.Top;
                spL.Margin = new Thickness(1, 0, 0, 0);
                spL.Children.Add(lbl);
                spL.Background = Brushes.White;

                // OuterStackPanel.Background = Brushes.LightGoldenrodYellow; // ???

                // --- Rectangle Section ---
                StackPanel spR = new StackPanel();
                spR.Width = OuterStackPanel.ActualWidth;

                Rectangle rct = new Rectangle();
                rct.Width = OuterStackPanel.ActualWidth * .2;
                rct.Height = 20;
                rct.Stroke = Brushes.Black;
                rct.StrokeThickness = 1;
                rct.Fill = Brushes.Green;

                spR.VerticalAlignment = VerticalAlignment.Center;
                spR.Margin = new Thickness(1, 0, 0, 0);
                spR.Children.Add(rct);
                spR.Background = Brushes.White;

                //OuterStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                //OuterStackPanel.Children.Add(lbl);
                OuterStackPanel.Children.Add(spL);
                OuterStackPanel.Children.Add(spR);

            }

        }
    }
}
