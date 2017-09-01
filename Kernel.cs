using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Lensinator
{
    class Kernel
    {
        public static  WriteableBitmap bitmap {
            get;
            set;
        }
        public static string Name { get; set; }
        public static string PH { get; set; }
        public static string EM { get; set; }

    }
}
