using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.UI.Input;
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
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using static Lensinator.Kernel;
using Windows.UI.Popups;
using Windows.Graphics.Display;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Lensinator
{
    

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class imgc : Page
    {
        SpeechSynthesizer synth;
        private WriteableBitmap bitmap;
        private OcrEngine ocrEngine = new OcrEngine(OcrLanguage.English);
        public imgc()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            DisplayProperties.AutoRotationPreferences = DisplayProperties.CurrentOrientation;

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

            TextOverlay.Children.Clear();
            ImageProperties imgProp = await file.Properties.GetImagePropertiesAsync();
            reso.Text = imgProp.Height.ToString() + "x" + imgProp.Width.ToString();
            date.Text = imgProp.DateTaken.ToString();
            using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmap = new WriteableBitmap((int)imgProp.Width, (int)imgProp.Height);
                bitmap.SetSource(imgStream);
                img.Source = bitmap;
                imgx.Source = bitmap;
                gg.Source = bitmap;

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
                // Used for text overlay.
                // Prepare scale transform for words since image is not displayed in original format.
                var scaleTrasform = new ScaleTransform
                {
                    CenterX = 0,
                    CenterY = 0,
                    ScaleX = img.ActualWidth / bitmap.PixelWidth,
                    ScaleY = img.ActualHeight / bitmap.PixelHeight,

                };
                var scaleTrasformx = new ScaleTransform
                {
                    CenterX = 0,
                    CenterY = 0,
                    ScaleX = imgx.ActualWidth / bitmap.PixelWidth,
                    ScaleY = imgx.ActualHeight / bitmap.PixelHeight,

                };
                if (ocrResult.TextAngle != null)
                {
                    // If text is detected under some angle then
                    // apply a transform rotate on image around center.
                    img.RenderTransform = new RotateTransform

                    {
                        Angle = (double)ocrResult.TextAngle,
                        CenterX = img.ActualWidth / 2,
                        CenterY = img.ActualHeight / 2
                    };
                    imgx.RenderTransform = new RotateTransform

                    {
                        Angle = (double)ocrResult.TextAngle,
                        CenterX = imgx.ActualWidth / 2,
                        CenterY = imgx.ActualHeight / 2
                    };
                }

              //  string extractedText = "";
                ext.Text = "";

                foreach (var line in ocrResult.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        var originalRect = new Rect(word.Left, word.Top, word.Width, word.Height);
                        var overlayRect = scaleTrasformx.TransformBounds(originalRect);

                        // Define the TextBlock.
                        var wordTextBlock = new TextBlock()
                        {
                            Height = overlayRect.Height,
                            Width = overlayRect.Width,
                            FontSize = overlayRect.Height * 0.8,
                            Text = word.Text,
                        };

                        // Define position, background, etc.
                        var border = new Border()
                        {
                            Margin = new Thickness(overlayRect.Left, overlayRect.Top, 0, 0),
                            Height = overlayRect.Height,
                            Width = overlayRect.Width,
                            Child = wordTextBlock,
                        };

                        // Put the filled textblock in the results grid.
                        TextOverlay.Children.Add(border);
                        ext.Text += word.Text + " ";
                        
                    }
                    ext.Text += '\n';
                   
                }

            }
            else
            {
                ext .Text = "No text found in Image";
                
            }
            
        }
            catch (Exception ex)
            {

                MessageDialog msgbox = new MessageDialog("An Error Occurred");
                await msgbox.ShowAsync();
            }
            }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);
        }
        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Subject";
            request.Data.Properties.Description = ext.Text;
            request.Data.SetText(ext.Text.ToString());

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                            DataRequestedEventArgs>(this.ShareTextHandler);
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
      
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void g3_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
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
        private void Scan(object sender, TappedRoutedEventArgs e)
        {
            {
                var picker = new FileOpenPicker()
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp"},
                };

                picker.PickSingleFileAndContinue();
            }
        }

        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            img256.Visibility  = Visibility.Visible;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            img256.Visibility = Visibility.Collapsed;

        }

        private void olay_Checked(object sender, RoutedEventArgs e)
        {
            TextOverlay.Visibility = olay .IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;

        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            using (var speech = new SpeechSynthesizer())
            {
                var text = ext.Text;
                var voiceStream = await speech.SynthesizeTextToStreamAsync(text);
                md1.SetSource(voiceStream, voiceStream.ContentType);
                md1.Play();
              
            }
        }

    

        private  void buttoan1_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();

        }



        private void srch_TextChanged(object sender, TextChangedEventArgs e)
        {
             
        
        }

    }

}
