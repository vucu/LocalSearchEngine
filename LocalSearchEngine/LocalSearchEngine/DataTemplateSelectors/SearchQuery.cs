using System;
using System.Collections.Generic;
using System.Text;
using LocalSearchEngine.Core;
using LocalSearchEngine.ViewModels.Components;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace LocalSearchEngine.DataTemplateSelectors
{
    public class SearchQuery : DataTemplateSelector
    {
        public DataTemplate TermTemplate { get; set; }
        public DataTemplate OperatorTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var vm = (SearchQueryItemViewModel)item;

            switch (vm.Type)
            {
                case SearchQueryItemType.Term:
                    return TermTemplate;
                case SearchQueryItemType.Operator:
                    return OperatorTemplate;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
