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

namespace BlazorWebViewWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        }

        public void CloseApp()
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainLayout.onCloseApp += CloseApp;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (blazorWebView.Visibility == Visibility.Visible)
                blazorWebView.Visibility = Visibility.Hidden;
            else
                blazorWebView.Visibility = Visibility.Visible;

        }
    }
}
