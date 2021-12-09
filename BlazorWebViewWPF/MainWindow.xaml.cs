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
            ViewModel.AspectRatio = new List<KeyValuePair<string, double>>()
            {
                new KeyValuePair<string, double>("4/3",(4.0/3.0)),
                new KeyValuePair<string, double>("5/3",(5.0/3.0)),
                new KeyValuePair<string, double>("16/9",(16.0/9.0))
            };
            ViewModel.Resolutions = new ObservableResolutionList();
            this.DataContext = ViewModel;
            
        }

        public void CloseApp()
        {
            //Application.Current.Shutdown();
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
        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainLayout.onCloseApp += CloseApp;
            ToggleVideo += Index.ToggleVideoHandler;
            SwitchCamera += Index.SwitchCameraHandler;
            TakeScreenshot += Index.TakeScreenshotHandler;
            ChangeResolution += Index.SetResolutionHandler;
            AspectRatioComboBox.SelectedIndex = 0;
        }



        private void blazorWebView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
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

        private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            TakeScreenshot();
        }

        [JSInvokable]
        public static Task<string> ReturnScreenshot(string photo)
        {
            var photoData = photo.Split(',');
            byte[] imageBytes = Convert.FromBase64String(photoData[1]);
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = path + @$"/images/{DateTime.Now.Ticks.ToString()}.png";
            File.WriteAllBytes(filename, imageBytes);
            return Task.FromResult<string>(filename);
        }


    }
}
