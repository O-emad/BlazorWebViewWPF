using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebViewWPF
{

    public class Resolution
    {
        public Resolution()
        {

        }

        public Resolution(string resolution)
        {
            Value = resolution;
        }
        public Resolution(int width, double aspectRatio)
        {
            Width= width;
            Height = (int)(width / aspectRatio);
            Value = $"{Width}x{Height}";
        }

        public int Width { get; set; }
        public int Height { get; set; }
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
    }
    //public class Resolution :  INotifyPropertyChanged
    //{


    //    public Resolution()
    //    {

    //    }

    //    public Resolution(string resolution)
    //    {
    //        Value = resolution;
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public int Width { get; set; }
    //    public int Height { get; set; }
    //    private string _value;
    //    public string Value { get { return _value; } 
    //        set {
    //         _value = value;
    //            OnPropertyChanged();
    //        } }

    //    protected void OnPropertyChanged([CallerMemberName] string name = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //    }
    //}
}
