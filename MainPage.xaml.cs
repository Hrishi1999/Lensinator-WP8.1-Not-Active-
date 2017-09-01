using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Lensinator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DisplayProperties.AutoRotationPreferences = DisplayProperties.CurrentOrientation;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
         
            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(imgc));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(cr));
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void svg3336_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(nots));

        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(bcs));

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(feedback));

        }

        private void AppBarButton4_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(about));

        }

        private void AppBarButton2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AppBarButton3_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(help1));

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(nots));
        }
    }
}
