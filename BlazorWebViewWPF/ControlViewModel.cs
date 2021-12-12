using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebViewWPF
{
    public class ControlViewModel
    {
        public List<string> FPS { get; set; }
        public List<KeyValuePair<string,double>> AspectRatio { get; set; }
        public ObservableResolutionList Resolutions { get; set; }
        public List<KeyValuePair<string,string>> Effects { get; set; }
    }
}
