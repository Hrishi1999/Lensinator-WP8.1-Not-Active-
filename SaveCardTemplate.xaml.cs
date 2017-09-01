using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Lensinator
{
    public sealed partial class SaveCardTemplate : UserControl
    {
        public string nameofp
        {
            get
            {
                return this.pname.Text;
            }
            set
            {
                this.pname.Text = pname.Text ;
            }
        }
        public string phnop
        {
            get
            {
                return this.pnom .Text;
            }
            set
            {
                this.pnom.Text = pnom .Text ;
            }
        }
        public string Ema
        {
            get
            {
                return this.pem.Text;
            }
            set
            {
                this.pem.Text = pem .Text ;
            }
        }
        public static readonly DependencyProperty BindingTextValueProperty = DependencyProperty.Register(
                                          "BindingTextValue",
                                          typeof(string),
                                          typeof(SaveCardTemplate),
                                          new PropertyMetadata(""));
        public string BindingTextValue
        {
            get
            {
                return GetValue(BindingTextValueProperty) as string;
            }
            set
            {
                SetValue(BindingTextValueProperty, value);
            }
        }
        public SaveCardTemplate()
        {
            this.InitializeComponent();
            gd.DataContext = this;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
