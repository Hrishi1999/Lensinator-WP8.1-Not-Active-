using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using ZXing.Common;
using Lensinator;
using Windows.Media.Devices;
using System.Diagnostics;
using Windows.Media;
using Windows.Graphics.Display;
using Windows.Media.MediaProperties;
using Windows.Devices.Enumeration;
using Lumia.Imaging;
using VideoEffects;
using Windows.UI;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Lensinator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class bcs : Page
    {
        DisplayRequest m_displayRequest = new DisplayRequest();
        MediaCapture m_capture;
        ContinuousAutoFocus m_autoFocus;
        bool m_initializing;
        BarcodeReader m_reader = new BarcodeReader
        {
            Options = new DecodingOptions
            {
                PossibleFormats = new BarcodeFormat[] { BarcodeFormat.QR_CODE,
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.All_1D,
                    BarcodeFormat.AZTEC,
                    BarcodeFormat.CODABAR,
                    BarcodeFormat.CODE_39,
                    BarcodeFormat.CODE_93 ,
                    BarcodeFormat.DATA_MATRIX ,
                    BarcodeFormat.EAN_13 ,
                    BarcodeFormat.EAN_8  ,
                    BarcodeFormat.ITF  ,
                    BarcodeFormat.MAXICODE  ,
                    BarcodeFormat.MSI ,
                    BarcodeFormat.PDF_417,
                    BarcodeFormat.PLESSEY ,
                    BarcodeFormat.RSS_14,
                    BarcodeFormat.RSS_EXPANDED,
                    BarcodeFormat.UPC_A,
                    BarcodeFormat.UPC_E,
                    BarcodeFormat.UPC_EAN_EXTENSION
                },
                
             TryHarder = true,
             
             
                
            }
        };
        Stopwatch m_time = new Stopwatch();
        volatile bool m_snapRequested;
        public bcs()
        {
            this.InitializeComponent();
            DisplayProperties.AutoRotationPreferences = DisplayProperties.CurrentOrientation;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            m_displayRequest.RequestActive();
            await InitializeCaptureAsync();

             var ignore = InitializeCaptureAsync();
        }
        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
          
            await InitializeCaptureAsync();

            m_displayRequest.RequestRelease();

            await DisposeCaptureAsync();
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
        private VideoRotation VideoRotationLookup(DisplayOrientations displayOrientation, bool counterclockwise)
        {
            switch (displayOrientation)
            {
                case DisplayOrientations.Landscape:
                    return VideoRotation.None;

                case DisplayOrientations.Portrait:
                    return (counterclockwise) ? VideoRotation.Clockwise270Degrees : VideoRotation.Clockwise90Degrees;

                case DisplayOrientations.LandscapeFlipped:
                    return VideoRotation.Clockwise180Degrees;

                case DisplayOrientations.PortraitFlipped:
                    return (counterclockwise) ? VideoRotation.Clockwise90Degrees :
                    VideoRotation.Clockwise270Degrees;

                default:
                    return VideoRotation.None;
            }
        }
        private async Task InitializeCaptureAsync()
        {
            if (m_initializing || (m_capture != null))
            {
                return;
            }
            m_initializing = true;

            try
            {
                var settings = new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = await GetBackOrDefaulCameraIdAsync(),
                    StreamingCaptureMode = StreamingCaptureMode.Video
                    
                    
                };

                var capture = new MediaCapture();
                await capture.InitializeAsync(settings);

                var formats = capture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);
                var format = (VideoEncodingProperties)formats.OrderBy((item) =>
                {
                    var props = (VideoEncodingProperties)item;
                    return Math.Abs(props.Width - this.ActualWidth) + Math.Abs(props.Height - this.ActualHeight);
                }).First();
                await capture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, format);

           
                var definition = new LumiaAnalyzerDefinition(ColorMode.Yuv420Sp, 640, AnalyzeBitmap);
                await capture.AddEffectAsync(MediaStreamType.VideoPreview, definition.ActivatableClassId, definition.Properties);
                
                m_time.Restart();
                capture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                Preview.Source = capture;
                await capture.StartPreviewAsync();

                capture.Failed += capture_Failed;

                m_autoFocus = await ContinuousAutoFocus.StartAsync(capture.VideoDeviceController.FocusControl);
                 
                m_capture = capture;
            }
            catch (Exception e)
            {
                textlog.Text = String.Format("Failed to start the camera: {0}", e.Message);
            }

            m_initializing = false;
        }

        void capture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { });
        }


        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (textlog.Text != "")
            {
                if (textlog.Text.StartsWith("http://"))
                {
                    await Windows.System.Launcher.LaunchUriAsync(new System.Uri(textlog.Text));

                }
                else
                {
                    //if (If System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable Then)
                    if (NetworkInterface.GetIsNetworkAvailable() == true)
                    {
                        if (textlog.Text != null)
                        {
                            TextBox addre = new TextBox();
                            addre.Text = "http://www.searchupc.com/handlers/upcsearch.ashx?request_type=3&access_token=A0C3C284-F6FF-448B-B65B-CDC923752F16&upc=" + textlog.Text;
                            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(addre.Text);
                            var resp = await myReq.GetResponseAsync();
                            StreamReader respStrmReader = new StreamReader(resp.GetResponseStream());
                            var result = await respStrmReader.ReadToEndAsync();
                            TextBox jsonvals = new TextBox();
                            jsonvals.Text = result.ToString();
                            string jsonval = jsonvals.Text;

                            var myJsonString = jsonval;
                            var jo = JObject.Parse(myJsonString);

                            var id = jo["0"]["productname"].ToString();

                            var img = jo["0"]["imageurl"].ToString();

                            var pu = jo["0"]["producturl"].ToString();

                            var pr = jo["0"]["price"].ToString();

                            var cu = jo["0"]["currency"].ToString();

                            var st = jo["0"]["storename"].ToString();

                            var sp = jo["0"]["saleprice"].ToString();


                            barcodekernel.code = textlog.Text;
                            barcodekernel.SaleP = sp;
                            barcodekernel.Name = id;
                            barcodekernel.pi = img;
                            barcodekernel.URL = pu;
                            barcodekernel.Price = pr;
                            barcodekernel.Store = st;
                            barcodekernel.curr = cu;

                            this.Frame.Navigate(typeof(bif));
                        }
                        else
                        {
                            MessageDialog msgbox = new MessageDialog("No barcode scanned!");
                            await msgbox.ShowAsync();
                        }

                    }
                    else
                    {
                        MessageDialog msgbox = new MessageDialog("No network connection found!");
                        await msgbox.ShowAsync();
                    }



                }
            }
            else
            {
                MessageDialog msgbox = new MessageDialog("No data extracted");
                await msgbox.ShowAsync();
            }
            }
        private void AnalyzeBitmap(Bitmap bitmap, TimeSpan time)
        {
            if (m_snapRequested)
            {
                m_snapRequested = false;

                IBuffer jpegBuffer = (new JpegRenderer(new BitmapImageSource(bitmap))).RenderAsync().AsTask().Result;
                var jpegFile = KnownFolders.PicturesLibrary.CreateFileAsync("QrCodeSnap.jpg", CreationCollisionOption.GenerateUniqueName).AsTask().Result;
                FileIO.WriteBufferAsync(jpegFile, jpegBuffer).AsTask().Wait();
            }

 
            Result result = m_reader.Decode(
                bitmap.Buffers[0].Buffer.ToArray(),
                (int)bitmap.Buffers[0].Pitch , 
                (int)bitmap.Dimensions.Height,
                BitmapFormat.Gray8
                );

 
            var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var elapsedTimeInMS = m_time.ElapsedMilliseconds;
                m_time.Restart();

                if (result == null)
                {
                    textlog.Text = String.Format("", elapsedTimeInMS);

                    if (m_autoFocus != null)
                    {
                        m_autoFocus.BarcodeFound = false;
                    }

                    BarcodeOutline.Points.Clear();
                }
                else
                {
                    textlog.Text = String.Format("{1}", elapsedTimeInMS, result.Text);
                    if (textlog.Text != "")
                    {
                        await DisposeCaptureAsync();
                        btnstrt.Visibility = Visibility.Visible;
                        Dispatcher.StopProcessEvents();
                    }
                    if (m_autoFocus != null)
                    {
                        m_autoFocus.BarcodeFound = true;
                    }

                    BarcodeOutline.Points.Clear();

                    for (int n = 0; n < result.ResultPoints.Length; n++)
                    {
                        BarcodeOutline.Points.Add(new Point(
                            result.ResultPoints[n].X * (BarcodeOutline.Width / bitmap.Dimensions.Width),
                            result.ResultPoints[n].Y * (BarcodeOutline.Height / bitmap.Dimensions.Height)
                            ));
                    }
                }
            });
        }

        public static async Task<string> GetBackOrDefaulCameraIdAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            string deviceId = "";

            foreach (var device in devices)
            {
                if ((device.EnclosureLocation != null) &&
                    (device.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back))
                {
                    deviceId = device.Id;
                    break;
                }
            }

            return deviceId;
        }

        // Must be called on the UI thread
        private async Task DisposeCaptureAsync()
        {
            Preview.Source = null;

            if (m_autoFocus != null)
            {
                m_autoFocus.Dispose();
                m_autoFocus = null;
            }

            MediaCapture capture;
            lock (this)
            {
                capture = m_capture;
                m_capture = null;
            }

            if (capture != null)
            {
                capture.Failed -= capture_Failed;

                await capture.StopPreviewAsync();

                capture.Dispose();
            }
        }
        private class ContinuousAutoFocus
        {
            Stopwatch m_timeSinceLastBarcodeFound = Stopwatch.StartNew();
            FocusControl m_control;
            bool m_barcodeFound = false;

            public bool BarcodeFound
            {
                get
                {
                    return m_barcodeFound;
                }

                set
                {
                    lock (this)
                    {
                        m_barcodeFound = value;
                        if (value)
                        {
                            m_timeSinceLastBarcodeFound.Restart();
                        }
                    }
                }
            }

            public static async Task<ContinuousAutoFocus> StartAsync(FocusControl control)
            {
                var autoFocus = new ContinuousAutoFocus(control);

                AutoFocusRange range;
                if (control.SupportedFocusRanges.Contains(AutoFocusRange.FullRange))
                {
                    range = AutoFocusRange.FullRange;
                }
                else if (control.SupportedFocusRanges.Contains(AutoFocusRange.Normal))
                {
                    range = AutoFocusRange.Normal;
                }
                else
                {
                    // Auto-focus disabled
                    return autoFocus;
                }

                FocusMode mode;
                if (control.SupportedFocusModes.Contains(FocusMode.Continuous))
                {
                    mode = FocusMode.Continuous;
                }
                else if (control.SupportedFocusModes.Contains(FocusMode.Single))
                {
                    mode = FocusMode.Single;
                }
                else
                {
                    return autoFocus;
                }

                if (mode == FocusMode.Continuous)
                {
                    var settings = new FocusSettings()
                    {
                        AutoFocusRange = range,
                        Mode = mode,
                        WaitForFocus = false,
                        DisableDriverFallback = false
                    };
                    control.Configure(settings);
                    await control.FocusAsync();
                }
                else
                {
                    var settings = new FocusSettings()
                    {
                        AutoFocusRange = range,
                        Mode = mode,
                        WaitForFocus = true,
                        DisableDriverFallback = false
                    };
                    control.Configure(settings);

                    var ignore = Task.Run(async () => { await autoFocus.DriveAutoFocusAsync(); });
                }
        

                return autoFocus;
            }

            ContinuousAutoFocus(FocusControl control)
            {
                m_control = control;
            }

            public void Dispose()
            {
                lock (this)
                {
                    m_control = null;
                }
            }

            async Task DriveAutoFocusAsync()
            {
                while (true)
                {
                    FocusControl control;
                    bool runFocusSweep;
                    lock (this)
                    {
                        if (m_control == null)
                        {
                            return;
                        }
                        control = m_control;
                        runFocusSweep = m_timeSinceLastBarcodeFound.ElapsedMilliseconds > 1000;
                    }

                    if (runFocusSweep)
                    {
                        try
                        {
                            await control.FocusAsync();
                        }
                        catch
                        {
                        }
                    }

                    await Task.Delay(1000);
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            m_snapRequested = true;

        }

        private async void clc(object sender, RoutedEventArgs e)
        {
            btnstrt.Visibility = Visibility.Collapsed ;
            await InitializeCaptureAsync();
        }
    }
}
