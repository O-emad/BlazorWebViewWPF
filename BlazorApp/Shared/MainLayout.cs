using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared
{
    public partial class MainLayout
    {

        [Inject] public  NavigationManager NavigationManager { get; set; }
       

        public static readonly MudTheme MyCustomTheme = new MudTheme()
        {
            Palette = new Palette()
            {

                //Success = Colors.LightGreen.Accent4,
                //Primary = Colors.Blue.Default,
                //Secondary = Colors.Green.Accent4,
                //AppbarBackground = Colors.Blue.Default,'
                Black = "#27272f",
                Primary = Colors.Orange.Accent3,
                Secondary = Colors.Orange.Accent1,
                PrimaryDarken = Colors.LightGreen.Accent4,
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#27272f",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                Divider = "rgba(255,255,255, 0.12)",
                DividerLight = "rgba(255,255,255, 0.06)",
                TableLines = "rgba(255,255,255, 0.12)",
                LinesDefault = "rgba(255,255,255, 0.12)",
                LinesInputs = "rgba(255,255,255, 0.3)",
                TextDisabled = "rgba(255,255,255, 0.2)"
            },

            LayoutProperties = new LayoutProperties()
            {
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "300px",
                
            }
        };
        public delegate void CloseAppHandler(); 
        public static event CloseAppHandler onCloseApp;
        private void CloseApp(MouseEventArgs args)
        {
            onCloseApp();
            
        }
    }
}
