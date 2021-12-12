using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebViewWPF
{
    public class ObservableResolutionList : ObservableCollection<Resolution>
    {
        private readonly int[] ResolutionWidth = { 3840, 3072, 2880, 2048, 1920, 1280 };
        public ObservableResolutionList()
        {
            foreach (var resolution in ResolutionWidth)
            {
                
                Add(new Resolution(resolution.ToString()));
            }
            //CollectionChanged += HandleChange;
        }
        
        public void Update(double aspectRatio)
        {
            for (int i = 0; i < ResolutionWidth.Length; i++)
            {
                SetItem(i, new Resolution(ResolutionWidth[i], aspectRatio));
            }
        }
        private void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e?.Action == NotifyCollectionChangedAction.Add)
            {
                
            }
        }
    }
}
