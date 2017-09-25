using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FreeProxyListLoader.Extensions;
using FreeProxyListLoader.Models;
using FreeProxyListLoader.Models.Loader;
using FreeProxyListLoader.Models.Serialization;
using FreeProxyListLoader.ViewModels.Command;
using Microsoft.Win32;

namespace FreeProxyListLoader.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly LoaderFreeProxy loader;

        private Dictionary<string, Func<FreeProxy, bool>> conditions;

        #region Binding properties

        private bool isWaitingForLoad = true;
        public bool IsWaitingForLoad
        {
            get => isWaitingForLoad;
            set
            {
                isWaitingForLoad = value;
                OnPropertyChanged("IsWaitingForLoad");
            }
        }

        private string mainFilter;
        public string MainFilter
        {
            get => mainFilter;
            set
            {
                if (mainFilter != value)
                {
                    mainFilter = value;
                    OnPropertyChanged("MainFilter");
                    FilterProxy();
                }
            }
        }

        private List<FreeProxy> showedItems;
        public List<FreeProxy> ShowedItems
        {
            get => showedItems;
            set
            {
                showedItems = value;
                OnPropertyChanged("ShowedItems");
            }
        }

        private string ipFilter = "search...";
        public string IpFilter
        {
            get => ipFilter;
            set
            {
                if (ipFilter != value)
                {
                    ipFilter = value;
                    OnPropertyChanged("IpFilter");
                    FilterProxy();
                }
            }
        }

        #region Filter collections

        private IEnumerable<string> ports;
        public IEnumerable<string> Ports
        {
            get => ports;
            set
            {
                ports = value;
                OnPropertyChanged("Ports");
            }
        }

        private IEnumerable<string> codes;
        public IEnumerable<string> Codes
        {
            get => codes;
            set
            {
                codes = value;
                OnPropertyChanged("Codes");
            }
        }

        private IEnumerable<string> countries;
        public IEnumerable<string> Countries
        {
            get => countries;
            set
            {
                countries = value;
                OnPropertyChanged("Countries");
            }
        }

        private IEnumerable<string> anonymity;
        public IEnumerable<string> Anonymity
        {
            get => anonymity;
            set
            {
                anonymity = value;
                OnPropertyChanged("Anonymity");
            }
        }

        private IEnumerable<string> https;
        public IEnumerable<string> Https
        {
            get => https;
            set
            {
                https = value;
                OnPropertyChanged("Https");
            }
        }

        private IEnumerable<string> lastChecked;
        public IEnumerable<string> LastChecked
        {
            get => lastChecked;
            set
            {
                lastChecked = value;
                OnPropertyChanged("LastChecked");
            }
        }

        private string selectedPort;
        public string SelectedPort
        {
            get => selectedPort;
            set
            {
                if (selectedPort != value)
                {
                    selectedPort = value;
                    FilterProxy();
                }
            }
        }

        private string selectedCode;
        public string SelectedCode
        {
            get => selectedCode;
            set
            {
                if (selectedCode != value)
                {
                    selectedCode = value;
                    FilterProxy();
                }
            }
        }

        private string selectedCountry;
        public string SelectedCountry
        {
            get => selectedCountry;
            set
            {
                if (selectedCountry != value)
                {
                    selectedCountry = value;
                    FilterProxy();
                }
            }
        }

        private string selectedAnonymity;
        public string SelectedAnonymity
        {
            get => selectedAnonymity;
            set
            {
                if (selectedAnonymity != value)
                {
                    selectedAnonymity = value;
                    FilterProxy();
                }
            }
        }

        private string selectedHttps;
        public string SelectedHttps
        {
            get => selectedHttps;
            set
            {
                if (selectedHttps != value)
                {
                    selectedHttps = value;
                    FilterProxy();
                }
            }
        }

        private string selectedLastChecked;
        public string SelectedLastChecked
        {
            get => selectedLastChecked;
            set
            {
                if (selectedLastChecked != value)
                {
                    selectedLastChecked = value;
                    FilterProxy();
                }
            }
        }

        #endregion

        public FreeProxy SelectedFreeProxy { get; set; }

        public ICommand UpdateLoadCommand { get; }
        public ICommand GoToWebSiteCommand { get; }
        public ICommand CopyProxyCommand { get; }
        public ICommand SaveFileCommand { get; }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            loader = new LoaderFreeProxy();
            UpdateLoadCommand = new AsyncCommand(UpdateContent, () => isWaitingForLoad);
            GoToWebSiteCommand = new SimpleCommand(() => Process.Start(loader.Url), () => isWaitingForLoad);
            CopyProxyCommand = new SimpleCommand(() => Clipboard.SetText(SelectedFreeProxy.ToString()), () => SelectedFreeProxy != null);
            SaveFileCommand = new SimpleCommand(SaveToFile);
        }

        #endregion

        #region Methods

        private void InitFilterConditions()
        {
            conditions = new Dictionary<string, Func<FreeProxy, bool>>
            {
                { "ip_filter", proxy => ipFilter == "search..." || string.IsNullOrEmpty(ipFilter) || proxy.Ip.Contains(ipFilter) },
                { "main_filter", proxy => string.IsNullOrEmpty(mainFilter) || proxy.Contains(mainFilter) },
                { "port", proxy => SelectedPort == "All" || proxy.Port.ToString() == SelectedPort },
                { "code", proxy => SelectedCode == "All" || proxy.Code == SelectedCode },
                { "country", proxy => SelectedCountry == "All" || proxy.Country == SelectedCountry },
                { "anonymity", proxy => SelectedAnonymity == "All" || proxy.Anonymity.ToString() == SelectedAnonymity },
                { "https", proxy => SelectedHttps == "All" || proxy.IsHttps == (SelectedHttps == "yes") },
                { "last_checked", proxy => SelectedLastChecked == "All" || proxy.LastChecked == SelectedLastChecked }
            };
        }

        private void FilterProxy()
        {
            ShowedItems = loader.LoadedItems
                ?.Where(conditions["ip_filter"])
                .Where(conditions["main_filter"])
                .Where(conditions["port"])
                .Where(conditions["code"])
                .Where(conditions["country"])
                .Where(conditions["anonymity"])
                .Where(conditions["https"])
                .Where(conditions["last_checked"])
                .ToList();
        }

        private void SaveToFile()
        {
            // init and show dialog window
            var sfd = new SaveFileDialog
            {
                Filter = "TXT files (*.txt)|*.txt|XML file (*.xml)|*.xml|JSON file (*.json)|*.json|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != true) return;
            var filepath = sfd.FileName;

            try
            {
                using (var stream = sfd.OpenFile())
                {
                    // file extension which has been chosen by user
                    var mask = Path.GetExtension(filepath).ToLower();

                    // choosing a type of serialization (xml or json)
                    var serializer =
                        mask == ".json"
                            ? new JsonSimpleSerializer()
                            : mask == ".xml"
                                ? new XmlSimpleSerializer() as ISerializer
                                : new TextSimpleSerializer();

                    // saving to file
                    serializer.Serialize(stream, ShowedItems);

                    MessageBox.Show(
                        "Has been saved in " + filepath,
                        "Saving...",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error of saving file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateContent()
        {
            MainFilter = string.Empty;
            IsWaitingForLoad = false;
            LoadProxyList();
            InitTableBody();
            InitFilterConditions();
            IsWaitingForLoad = true;
        }

        private void InitTableBody()
        {
            ShowedItems = loader.LoadedItems.ToList();
            
            // get a filter list
            Ports = loader.LoadedItems.Select(x => x.Port.ToString()).Distinct().InsertFront("All");
            Codes = loader.LoadedItems.Select(x => x.Code).Distinct().InsertFront("All");
            Countries = loader.LoadedItems.Select(x => x.Country).Distinct().InsertFront("All");
            Anonymity = new[] {"All", "Transparent", "Anonymous", "Elite"};
            Https = new[] {"All", "yes", "no"};
            LastChecked = loader.LoadedItems.Select(x => x.LastChecked).Distinct().InsertFront("All");

        }

        private void LoadProxyList()
        {
            ShowedItems = null;

            try
            {
                // trying to load proxy-list
                loader.Load();
            }
            catch (Exception)
            {
                if (MessageBoxResult.Yes == MessageBox.Show(
                    "Do you want to reload proxy-list?",
                    "Server is not responding",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question)
                )
                    // repeat load
                    LoadProxyList();
            }
        }

        #endregion
    }
}
