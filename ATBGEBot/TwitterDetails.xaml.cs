using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace ATBGEBot
{
    /// <summary>
    /// Interaction logic for TwitterDetails.xaml
    /// </summary>
    public partial class TwitterDetails : Window
    {
        public TwitterDetails()
        {
            InitializeComponent();
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            tboxApiKey.Text = key.GetValue("ApiKey", "").ToString();
            tboxApiSecret.Text = key.GetValue("ApiSecret", "").ToString();
            tboxAccessToken.Text = key.GetValue("AccessToken", "").ToString();
            tboxAccessTokenSecret.Text = key.GetValue("AccessTokenSecret", "").ToString();
            //tboxRedditLink.Text = key.GetValue("RedditLink", "https://www.reddit.com/r/").ToString();
        }

        private void btnTestCredentials_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Currently a work in progress; sorry.");
        }

        private void tboxApiKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            key.SetValue("ApiKey", tboxApiKey.Text);
        }

        private void tboxApiSecret_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            key.SetValue("ApiSecret", tboxApiSecret.Text);
        }

        private void tboxAccessToken_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            key.SetValue("AccessToken", tboxAccessToken.Text);
        }

        private void tboxAccessTokenSecret_TextChanged(object sender, TextChangedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            key.SetValue("AccessTokenSecret", tboxAccessTokenSecret.Text);
        }

        private void tboxRedditLink_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Back) || e.Key.Equals(Key.Delete))
            {
                MessageBox.Show("Nope!");
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TwitterPoster");
            }
        }

        private void SaveToReg(string key, string value)
        {
            const string keyName = "HKEY_CURRENT_USER" + "\\" + "RegistrySetValueExample";

            Registry.SetValue(keyName, key, value);
        }

        private void SaveRedditLinks()
        {
            List<string> redditLinks = new List<string>();
            foreach (var item in redditLinks)
            {
                redditLinks.Add("");
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(tboxApiKey.Text))
            {
                SaveToReg("TWITTER_API_KEY", tboxApiKey.Text);
            }
            if (!string.IsNullOrEmpty(tboxApiSecret.Text))
            {
                SaveToReg("TWITTER_API_SECRET", tboxApiSecret.Text);
            }
            if (!string.IsNullOrEmpty(tboxAccessToken.Text))
            {
                SaveToReg("TWITTER_ACCESS_TOKEN", tboxAccessToken.Text);
            }
            if (!string.IsNullOrEmpty(tboxAccessTokenSecret.Text))
            {
                SaveToReg("TWITTER_ACCESS_TOKEN_SECRET", tboxAccessTokenSecret.Text);
            }


            if (true)
            {
                SaveRedditLinks();
            }
        }

        private void tboxRedditLink_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgTwitter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgFacebook_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgInstagram_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lboxInstaLinks_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
 