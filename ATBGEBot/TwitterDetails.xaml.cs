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

        }

        private void btnTestCredentials_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Currently work in progress; sorry.");
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
    }
}
