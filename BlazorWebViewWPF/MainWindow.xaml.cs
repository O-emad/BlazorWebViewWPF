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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using MudBlazor.Services;
using BlazorApp.Shared;
using BlazorApp.Pages;
using Index = BlazorApp.Pages.Index;
using Microsoft.JSInterop;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Net;
using System.Text.Unicode;
using System.Text.RegularExpressions;

namespace BlazorWebViewWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ControlViewModel ViewModel;
        public MainWindow()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();
            serviceCollection.AddMudServices();
            //Resources.Add("services", serviceCollection.BuildServiceProvider());

            InitializeComponent();
            blazorWebView.Services = serviceCollection.BuildServiceProvider();
            blazorWebView.HostPage = @"wwwroot\index.html";
            blazorWebView.RootComponents.Add(new RootComponent()
            {
                ComponentType = typeof(BlazorApp.App),
                Selector = "#app"
            });
            
            ViewModel = new ControlViewModel();
            ViewModel.FPS = new List<string>() { "20", "30", "40", "50", "60" };
            ViewModel.AspectRatio = new List<KeyValuePair<string, double>>()
            {
                new KeyValuePair<string, double>("4/3",(4.0/3.0)),
                new KeyValuePair<string, double>("5/3",(5.0/3.0)),
                new KeyValuePair<string, double>("16/9",(16.0/9.0))
            };
            ViewModel.Resolutions = new ObservableResolutionList();
            ViewModel.Effects = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("No Effect",""),
                new KeyValuePair<string, string>("Aviators","./effects/aviators.txt"),
                new KeyValuePair<string, string>("Beard","./effects/beard.txt"),
                new KeyValuePair<string, string>("Dalmatian","./effects/dalmatian.txt"),
                new KeyValuePair<string, string>("Flowers","./effects/flowers.txt"),
                new KeyValuePair<string, string>("Koala","./effects/koala.txt"),
                new KeyValuePair<string, string>("Look1","./effects/look1.txt"),
                new KeyValuePair<string, string>("Look2","./effects/look2.txt"),
                new KeyValuePair<string, string>("Lion","./effects/lion.txt"),
                new KeyValuePair<string, string>("Teddycigar","./effects/teddycigar.txt"),
                new KeyValuePair<string, string>("Segmentation","./effects/background_segmentation.txt")
            };
            DataContext = ViewModel;
            
        }

        public void CloseApp()
        {
            //Application.Current.Shutdown();
            if (TopDock.Visibility == Visibility.Visible)
                TopDock.Visibility = Visibility.Collapsed;
            else
                TopDock.Visibility = Visibility.Visible;
        }

        #region Blazor-related Events
        public delegate void ToggleVideoHandler();
        public event ToggleVideoHandler ToggleVideo;
        public delegate void SwitchCameraHandler();
        public event SwitchCameraHandler SwitchCamera;
        public delegate void TakeScreenshotHandler();
        public event TakeScreenshotHandler TakeScreenshot;
        public delegate void ChangeResolutionHandler(int width, int height);
        public event ChangeResolutionHandler ChangeResolution;
        public delegate void ToggleRecordingHandler();
        public event ToggleRecordingHandler ToggleRecording;
        public delegate void ChangeFPSHandler(int fps);
        public event ChangeFPSHandler ChangeFPS;
        public delegate void ClearEffectHandler();
        public event ClearEffectHandler ClearEffect;
        public delegate void ChangeEffectHandler(string effectFilename);
        public event ChangeEffectHandler ChangeEffect;
        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainLayout.onCloseApp += CloseApp;
            ToggleVideo += Index.ToggleVideoHandler;
            SwitchCamera += Index.SwitchCameraHandler;
            TakeScreenshot += Index.TakeScreenshotHandler;
            ChangeResolution += Index.SetResolutionHandler;
            ToggleRecording += Index.ToggleRecordingHandler;
            ChangeFPS += Index.ChangeFPSHandler;
            ClearEffect += Index.ClearEffectHandler;
            ChangeEffect += Index.ChangeEffectHandler;
            AspectRatioComboBox.SelectedIndex = 0;
            FpsComboBox.SelectedIndex = 1;
        }
        private void blazorWebView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(TopDock.Visibility == Visibility.Visible)
                TopDock.Visibility = Visibility.Collapsed;
            else
                TopDock.Visibility = Visibility.Visible;
        }

        private void ToggleVideoBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleVideo();
        }

        private void SwitchCameraBtn_Click(object sender, RoutedEventArgs e)
        {
            SwitchCamera();
        }

        private void AspectRatioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (KeyValuePair<string, double>)AspectRatioComboBox.SelectedItem;
            var aspectRatio= selectedItem.Value;
            var originalResolutionIndex = ResolutionComboBox.SelectedIndex;
            ViewModel.Resolutions.Update(aspectRatio);
            ResolutionComboBox.SelectedIndex = originalResolutionIndex;
            
        }
        private void ResolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ResolutionComboBox.SelectedItem as Resolution;
            if (selectedItem is null) return;
            if (selectedItem.Width == 0) return;
            var width = selectedItem.Width;
            var height = selectedItem.Height;
            ChangeResolution(width, height);

        }
        private void FpsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = FpsComboBox.SelectedItem as string;
            if(string.IsNullOrWhiteSpace(selectedItem)) return;
            var fps = int.Parse(selectedItem);
            ChangeFPS(fps);

        }

        private void EffectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (KeyValuePair<string,string>)EffectComboBox.SelectedItem;
            if(string.IsNullOrWhiteSpace(selectedItem.Value))
            {
                ClearEffect();
                return;
            }
            var effectFilename = selectedItem.Value;
            ChangeEffect(effectFilename);

        }
        private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            TakeScreenshot();
        }
        private void RecordingBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleRecording();
        }
        [JSInvokable]
        public static Task<string> SaveFromDataURL(string dataURL)
        {
            var urlData = dataURL.Split(',');
            var type = Regex.Match(urlData[0], @"\/(.+?)\;").Groups[1].Value;
            byte[] dataBytes = Convert.FromBase64String(urlData[1]);
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = path + @$"/media/{DateTime.Now.Ticks}.{type}";
            File.WriteAllBytes(filename, dataBytes);
            return Task.FromResult<string>(filename);
        }

        private void CloseAppLbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
