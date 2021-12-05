using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Pages
{
    public partial class Counter
    {
        [Inject] public IJSRuntime JS { get; set; }
        public IJSObjectReference? Module { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Module = await JS.InvokeAsync<IJSObjectReference>("import",
               "./video.js");
                StateHasChanged();
            }

        }
    }


}
