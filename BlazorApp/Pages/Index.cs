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
        public IList<string> EffectsThumbs { get; set; } = new List<string>
    {
        "",
        "./thumbs/aviators.png",
        //"./effects/background_segmentation.txt",
        "./thumbs/beard.png",
        "./thumbs/dalmatian.png",
        "./thumbs/flowers.png",
        "./thumbs/koala.png",
        "./thumbs/lion.png",
        "",
        "",
        "./thumbs/teddy_cigar.png"
    };
        public IList<string> Effects { get; set; } = new List<string>
    {
        "",
        "./effects/aviators.txt",
        //"./effects/background_segmentation.txt",
        "./effects/beard.txt",
        "./effects/dalmatian.txt",
        "./effects/flowers.txt",
        "./effects/koala.txt",
        "./effects/lion.txt",
        "./effects/look1.txt",
        "./effects/look2.txt",
        "./effects/teddycigar.txt"
    };

        private IList<string> _source = new List<string>() { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" };
        private MudCarousel<string> _carousel { get; set; }
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
        public static async void SetResolutionHandler(int width, int height)
        {
            if (module is not null)
                await module.InvokeVoidAsync("setResolution",width,height);
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
                //await module.InvokeVoidAsync("switchEffect", Effects[1], face, slot);
               // await module.InvokeVoidAsync("onReload");
                await module.InvokeVoidAsync("processPhoto", "./test_photos/camera1.jpg");
            }
        }
        public async Task LoadEffect()
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync("loadEffect", Effects[selectedEffect]);
            }
        }

        public async Task SetFps()
        {
            if(module is not null)
            {
                await module.InvokeVoidAsync("setFps",5);
            }
        }

        public void EffectChange()
        {
            selectedEffect = _carousel.SelectedIndex;
            StateHasChanged();
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
                //StateHasChanged();
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

        //public async Task StopVideo()
        //{
        //    if (module is not null)
        //        await module.InvokeVoidAsync("stopVideo");
        //}

        int face = 0;
        string slot = "slot";

        private async Task SwitchEffect()
        {
            selectedEffect = _carousel.SelectedIndex;
            if (module is not null)
            {
                if (selectedEffect == 0)
                {
                    await module.InvokeVoidAsync("clearEffect", slot);
                }
                else
                {
                    await module.InvokeVoidAsync("switchEffect", Effects[selectedEffect], face, slot);
                }
                //EffectInUse++;
                //if (EffectInUse >= Effects.Count) EffectInUse = 0;
            }
            StateHasChanged();
        }

        //private async Task ClearEffect()
        //{
        //    //await module.InvokeVoidAsync("setCanvasDimensions", 300, 300);
        //    if (module is not null)
        //        await module.InvokeVoidAsync("clearEffect", slot);
        //}

        //private async Task SetCanvasDimensions()
        //{
        //    await module.InvokeVoidAsync("setCanvasDimensions", 0, 0);
        //}

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
