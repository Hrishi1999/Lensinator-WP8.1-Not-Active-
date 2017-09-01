using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Media.Ocr;
using Windows.Phone.PersonalInformation;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Lensinator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class cr : Page
    {
        private WriteableBitmap bitmap;
        private OcrEngine ocrEngine = new OcrEngine(OcrLanguage.English);
        public cr()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            DisplayProperties.AutoRotationPreferences = DisplayProperties.CurrentOrientation;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>


        private void Scan(object sender, TappedRoutedEventArgs e)
        {
            {
                var picker = new FileOpenPicker()
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp" },
                };

                picker.PickSingleFileAndContinue();
            }
        }
        public async void ContinueFileOpenPicker(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count != 0)
            {
               await LoadImage(args.Files[0]);
            }
        }

        private async Task LoadImage(StorageFile file)
        {
try
            {

            
            await ClearResults();
            fornm.Visibility = Visibility.Collapsed;
           ext.Visibility = Visibility.Collapsed;
            ImageProperties imgProp = await file.Properties.GetImagePropertiesAsync();
         ////   reso.Text = imgProp.Height.ToString() + "x" + imgProp.Width.ToString();
         ////   date.Text = imgProp.DateTaken.ToString();
            using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                bitmap.SetSource(imgStream);
                img.Source = bitmap;
                ho.Source = bitmap;

                }
                if (bitmap.PixelHeight < 40 ||
              bitmap.PixelHeight > 2600 ||
              bitmap.PixelWidth < 40 ||
              bitmap.PixelWidth > 2600)
            {
                ext.Text = "Image size is not supported." +
                                    Environment.NewLine +
                                    "Loaded image size is " + bitmap.PixelWidth + "x" + bitmap.PixelHeight + "." +
                                    Environment.NewLine +
                                    "Supported image dimensions are between 40 and 2600 pixels.";
                return;

            }

            var ocrResult = await ocrEngine.RecognizeAsync((uint)bitmap.PixelHeight, (uint)bitmap.PixelWidth, bitmap.PixelBuffer.ToArray());
           
            if (ocrResult.Lines != null)
            {
             
                var scaleTrasform = new ScaleTransform
                {
                    CenterX = 0,
                    CenterY = 0,
                    ScaleX = img.ActualWidth / bitmap.PixelWidth,
                    ScaleY = img.ActualHeight / bitmap.PixelHeight,
                };

                if (ocrResult.TextAngle != null)
                {
                
                    img.RenderTransform = new RotateTransform
                    {
                        Angle = (double)ocrResult.TextAngle,
                        CenterX = img.ActualWidth / 2,
                        CenterY = img.ActualHeight / 2
                    };
                }
               
                ext.Text = "";

                foreach (var line in ocrResult.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        ext.Text += word.Text + " ";

                    }
                    ext.Text += '\n';

                    string data = ext.Text; 
                                                                
                    Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                        RegexOptions.IgnoreCase);
                  
                    MatchCollection emailMatches = emailRegex.Matches(data);

                    StringBuilder sb = new StringBuilder();

                    foreach (Match emailMatch in emailMatches)
                    {
                        sb.AppendLine(emailMatch.Value);
                        em.Text = emailMatch.Value;
                    }


                    string text = ext.Text; 
                                                                
                    Regex phreg = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}",
                        RegexOptions.IgnoreCase);
                  
                    MatchCollection tt = phreg.Matches(text);

                    StringBuilder str = new StringBuilder();

                    foreach (Match phmatch in tt)
                    {
                        sb.AppendLine(phmatch.Value);
                        ph.Text = phmatch.Value;
                    }


                    string linex5;
                    StringBuilder sb5 = new StringBuilder();
                    using (System.IO.StringReader x5 = new System.IO.StringReader(ext.Text))
                    {
                        while ((linex5 = x5.ReadLine()) != null)
                        {
                            if (linex5.Contains(""))
                            {
                                  sb5.AppendLine(linex5.ToString());
                            }
                        }
                    }
                    string[] exampleStrings = new string[]
                        {
                            sb5.ToString()
                        };

                    foreach (string example in exampleStrings)
                    {
                        string firstWords = FirstWords(example, 3);
                        TextBox t5 = new TextBox();
                        TextBox t6 = new TextBox();
                        t5.Text = example;
                        var output = Regex.Replace(firstWords, @"[\d-]", string.Empty);
                        xm.Text = output;

                      
                    }
                }
            }
            else
            {
                ext.Text = "No data found in card";
                
            }
        }
            catch (Exception ex)
            {
                MessageDialog msgbox = new MessageDialog("An Error Occurred");
                await msgbox.ShowAsync();
            }
            }
        public async Task ClearResults()
        {
            ph.Text = "";
            em.Text = "";
            xm.Text = "";
            img.Source = null;
        }
        public static string FirstWords(string input, int numberWords)
        {
            try
            {
                    int words = numberWords;
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == ' ')
                    {
                        words--;
                    }
                    if (words == 0)
                    {
                        return input.Substring(0, i);
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
     
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void g3_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
        private async void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            ContactStore store = await ContactStore.CreateOrOpenAsync();

            StoredContact contact = new StoredContact(store);


            contact.GivenName = xm.Text ;
            IDictionary<string, object> props = await contact.GetPropertiesAsync();
            props.Add(KnownContactProperties.Email, em.Text);
            props.Add(KnownContactProperties.MobileTelephone , ph.Text);
            props.Add(KnownContactProperties.DisplayName , xm.Text);
            IDictionary<string, object> extprops = await contact.GetExtendedPropertiesAsync();
            extprops.Add("Codename", xm.Text );

            await contact.SaveAsync();


        }

      

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            Kernel.bitmap = bitmap;
            Kernel.EM = em.Text;
            Kernel.PH = ph.Text;
            Kernel.Name = xm.Text;

        }
    }
}
