using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Curacao.Mvvm.Abstractions.Navigation;
using Curacao.Mvvm.Abstractions.Services;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;

namespace Curacao.Phone.Toolkit.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly PhoneApplicationFrame _rootFrame;
        private readonly ISystemDispatcher _dispatcher;

        public NavigationService([NotNull] PhoneApplicationFrame rootFrame, [NotNull] ISystemDispatcher dispatcher)
        {
            if (rootFrame == null) throw new ArgumentNullException("rootFrame");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _rootFrame = rootFrame;
            _dispatcher = dispatcher;
        }

        public void GoToPage([NotNull] string pageName, [CanBeNull] IEnumerable<NavigationParameter> parameters = null)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");

            var sb = new StringBuilder();
            sb.Append(pageName);
            if (parameters != null)
            {
                sb.Append("?");
                foreach (var navigationParameter in parameters)
                {
                    sb.Append(navigationParameter.Parameter);
                    sb.Append("=");
                    sb.Append(Uri.EscapeDataString(navigationParameter.Value));
                    sb.Append("&");
                }
            }
            _dispatcher.InvokeOnUIifNeeded(() => _rootFrame.Navigate(new Uri(sb.ToString(), UriKind.Relative)));
        }

        public void GoToPage(string page, IDictionary<string, string> parameters)
        {
            GoToPage(page, parameters
                .Select(p => new NavigationParameter
                {
                    Parameter = p.Key,
                    Value = p.Value
                }
                ));
        }

        public void CleanNavigationStack()
        {
            while (_rootFrame.BackStack.Any())
            {
                _rootFrame.RemoveBackEntry();
            }
        }

        public void GoToPage(string page, IEnumerable<NavigationParameter> parameters, int numberOfItemsToRemove)
        {
            GoToPage(page, parameters);
            _dispatcher.InvokeOnUIifNeeded(() => RemoveEntries(numberOfItemsToRemove));
        }

        private void RemoveEntries(int numberOfItemsToRemove)
        {
            for (var counter = 0; counter < numberOfItemsToRemove; counter++)
            {
                if (CanGoBack())
                {
                    _rootFrame.RemoveBackEntry();
                }
                else
                {
                    return;
                }
            }
        }

        public void GoBack()
        {
            _rootFrame.GoBack();
        }

        public bool CanGoBack()
        {
            return _rootFrame.CanGoBack;
        }
    }
}