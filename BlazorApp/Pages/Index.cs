using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Pages
{
    public class ToggleButton
    {

        public string ActiveStateText { get; set; }
        public string InactiveStateText { get; set; }
        public string ActiveStateIcon { get; set; }
        public string InactiveStateIcon { get; set; }

        public bool state { get; private set; } = false; //inactive
        public string Text() =>
                 state ? ActiveStateText : InactiveStateText;
        public string Icon() =>
                state ? ActiveStateIcon : InactiveStateIcon;
        public void ToggleState()
        {
            state = !state;
        }
        
    }
    public partial class Index : IAsyncDisposable
    {
        [Inject] public IJSRuntime JS { get; set; }

        private static IJSObjectReference? module;
        public static ToggleButton Play { get; set; } = new ToggleButton
        {
            ActiveStateText = "Stop",
            InactiveStateText = "Start",
            ActiveStateIcon = Icons.Filled.Stop,
            InactiveStateIcon = Icons.Filled.PlayArrow
        };
        public static ToggleButton Record { get; set; } = new ToggleButton
        {
            ActiveStateText = "Stop Recording",
            InactiveStateText = "Start Recording",
            ActiveStateIcon = Icons.Filled.Stop,
            InactiveStateIcon = Icons.Filled.FiberSmartRecord
        };
        private int selectedEffect { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import",
               "./scripts.js");
                await module.InvokeVoidAsync("onReload");
                StateHasChanged();
                
            }
            
        }

        #region WPF-related events

        public static async void ToggleVideoHandler()
        {
            await ToggleVideo();
        }
        public static async void SwitchCameraHandler()
        {
            if (module is not null)
                await module.InvokeVoidAsync("switchCamera");
        }
        public static async void TakeScreenshotHandler()
        {
            await TakeScreenshot();
        }
        public static async void SetResolutionHandler(int width, int height, double aspectRatio)
        {
            if (module is not null)
                await module.InvokeVoidAsync("setResolution",width,height,aspectRatio);
        }
        public static async void ToggleRecordingHandler()
        {
            await VideoRecording();
        }
        public static async void ChangeFPSHandler(int fps)
        {
            if (module is not null)
                await module.InvokeVoidAsync("setFps", fps);
        }
        public static async void ClearEffectHandler()
        {
            if (module is not null)
                await module.InvokeVoidAsync("clearEffect","slot");
        }
        public static async void ChangeEffectHandler(string effectFilename)
        {
            if(module is not null)
                await module.InvokeVoidAsync("switchEffect", effectFilename, 0, "slot");
        }
        #endregion


        public async Task LoadImage()
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync("processPhoto", "./test_photos/camera1.jpg");
            }
        }
        public async Task SetFps()
        {
            if(module is not null)
            {
                await module.InvokeVoidAsync("setFps",5);
            }
        }

        static bool  mirror = true;

        public static async Task ToggleVideo()
        {
            if (module is not null)
            {
                if (Play.state)
                {
                    await module.InvokeVoidAsync("stopVideo");
                }
                else
                {
                    await module.InvokeVoidAsync("startVideo", mirror);
                }
                Play.ToggleState();
            }
                
        }

        private static async Task VideoRecording()
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync("videoRecording");
                Record.ToggleState();
            }
        }

        private static async Task TakeScreenshot()
        {
            if (module is not null)
                await module.InvokeVoidAsync("takeScreenshot");

        }

        public async ValueTask DisposeAsync()
        {

            if (module is not null)
            {
                await module.InvokeVoidAsync("shutdown");
                await module.DisposeAsync();
            }
        }

        [JSInvokable]
        public static void OnConnectionLost()
        {

        }

        [JSInvokable]
        public static async void OnConnectionReturn()
        {
            
        }
    }
}
