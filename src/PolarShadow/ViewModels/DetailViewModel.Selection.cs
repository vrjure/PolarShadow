using Avalonia.Controls.Selection;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public partial class DetailViewModel
    {
        private async void AnalysisSelection_SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count == 0 || SelectionModel.Count == 0)
            {
                return;
            }

            var resource = SelectionModel.SelectedItem as ResourceTreeNode;
            if (string.IsNullOrEmpty(resource.Src) && SourceSelection.Count > 0)
            {
                resource = SourceSelection.SelectedItem as ResourceTreeNode;
            }

            var site = e.SelectedItems[0] as ISite;

            await LinkClick(new ResourceTreeNode
            {
                Name = resource.Name,
                Src = resource.Src,
                SrcType = LinkType.HtmlSource,
                Site = site.Name
            });
        }

        private async void SourceSelection_SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count == 0)
            {
                return;
            }
            var resource = e.SelectedItems[0] as ResourceTreeNode;
            if (resource.SrcType == LinkType.WebAnalysisSource) return;
            await LinkClick(resource);
        }

        protected override async void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                var resource = e.SelectedItems[0] as ResourceTreeNode;
                if (string.IsNullOrEmpty(resource.Src))
                {
                    if (resource.Children?.Count > 1)
                    {
                        SourceOptions = resource.Children.ToList();
                    }
                    else if (resource.Children?.Count == 1)
                    {
                        var first = resource.Children.First();
                        if (first.SrcType == LinkType.HtmlSource)
                        {
                            await LinkClick(first);
                        }
                        else if (first.SrcType == LinkType.WebAnalysisSource && AnalysisSelection?.SelectedIndex >= 0)
                        {
                            var site = AnalysisSelection.SelectedItem as ISite;
                            await LinkClick(new ResourceTreeNode
                            {
                                Name = first.Name,
                                Src = first.Src,
                                Site = site.Name,
                                SrcType = LinkType.HtmlSource
                            });
                        }
                    }
                    else
                    {
                        SourceOptions = null;
                    }
                }
                else
                {
                    await LinkClick(resource);
                }
            }
        }
    }
}
