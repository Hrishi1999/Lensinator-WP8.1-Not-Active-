using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Lensinator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class bif : Page
    {
        public bif()
        {
            this.InitializeComponent();
            DisplayProperties.AutoRotationPreferences = DisplayProperties.CurrentOrientation;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async  override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {

            
            nm.Text = barcodekernel.Name;
           // url.Content = barcodekernel.URL;
            store.Text = barcodekernel.Store;
            prc.Text = barcodekernel.Price + " " +barcodekernel .curr;
            sp.Text = barcodekernel.SaleP;
            iimm.Source = new BitmapImage(new Uri(barcodekernel.pi.ToString(), UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageDialog msgbox = new MessageDialog("An Error Occurred");
                await msgbox.ShowAsync();
            }

        }

        private async void url_Click(object sender, RoutedEventArgs e)
        {
            if  (barcodekernel.URL != "")
                {
                string uriToLaunch = barcodekernel.URL;
                var uri = new Uri(uriToLaunch);
                await Launcher.LaunchUriAsync(new Uri(uri.ToString()));

            }
            else
            {
                MessageDialog msgbox = new MessageDialog("No link to launch");
                await msgbox.ShowAsync();
            }
            }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
         
                string uriToLauncha = "http://google.com/search?q=" + barcodekernel.code;
                var uria = new Uri(uriToLauncha);
                 await Launcher.LaunchUriAsync(new Uri(uria.ToString()));

        }
    }
    }
