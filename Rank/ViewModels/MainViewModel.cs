using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Rank.Models;
using Rank.Providers;

namespace Rank.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ApiProvider apiProvider = new ApiProvider();
        private string website;
        private string keyWords;
        private string selectedSearchEngine;
        private string selectedLocation;
        private ObservableCollection<RankInfo> rankInfoList;

        public MainViewModel()
        {
            this.GetAllListSearchEngines();
            this.GetLocations();
            this.SendCommand = new RelayCommand(this.SendExecute);
        }
        #region Properties

        public ICommand SendCommand { get; set; }

        public ObservableCollection<string> SearchEnginesList { get; set; }

        public ObservableCollection<string> LocationsList { get; set; }

        public ObservableCollection<RankInfo> RankInfoList
        {
            get => this.rankInfoList;
            set
            {
                if (this.rankInfoList != value)
                {
                    this.rankInfoList = value;
                    base.RaisePropertyChanged(nameof(this.RankInfoList));
                }
            }
        }

        public string SelectedSearchEngine
        {
            get => this.selectedSearchEngine;
            set
            {
                if (this.selectedSearchEngine != value)
                {
                    this.selectedSearchEngine = value;
                    base.RaisePropertyChanged(nameof(this.SelectedSearchEngine));
                }
            }
        }

        public string SelectedLocation
        {
            get => this.selectedLocation;
            set
            {
                if (this.selectedLocation != value)
                {
                    this.selectedLocation = value;
                    base.RaisePropertyChanged(nameof(this.SelectedLocation));
                }
            }
        }

        public string WebSite
        {
            get => this.website;
            set
            {
                if (this.website != value)
                {
                    this.website = value;
                    base.RaisePropertyChanged(nameof(this.WebSite));

                }
            }
        }

        public string KeyWords
        {
            get => this.keyWords;
            set
            {
                if (this.keyWords != value)
                {
                    this.keyWords = value;
                    base.RaisePropertyChanged(nameof(this.KeyWords));
                }
            }
        }

        #endregion

        private async void GetAllListSearchEngines()
        {
            this.SearchEnginesList = new ObservableCollection<string>();
            var searchEngins = await apiProvider.GetAllListSearchEngines();

            foreach (string item in searchEngins)
            {
                this.SearchEnginesList.Add(item);
            }

            this.SelectedSearchEngine = this.SearchEnginesList.FirstOrDefault();
        }

        private async void GetLocations()
        {
            this.LocationsList = new ObservableCollection<string>();
            var locations = await apiProvider.GetListLocation();

            foreach (string item in locations)
            {
                this.LocationsList.Add(item);
            }

            this.SelectedLocation = this.LocationsList.FirstOrDefault();
        }

        private async void SendExecute()
        {
            this.RankInfoList = new ObservableCollection<RankInfo>();

            var listTemp = this.KeyWords.Split(new char[] { '\r', '\n' });
            var keyWordsList = listTemp.Where(x => !string.IsNullOrEmpty(x));

            foreach (var item in keyWordsList)
            {
                this.RankInfoList.Add(new RankInfo
                {
                    KeyWord = item,
                    SearchEngine = this.SelectedSearchEngine,
                    WebSite = this.WebSite,
                    Status = Status.processing
                });
            }

            var tasksList = await this.apiProvider.RankTaskSettings(this.SelectedSearchEngine, this.SelectedLocation, this.WebSite, keyWordsList);

            foreach (var item in tasksList)
            {
                var rankResult = await this.apiProvider.GetRankResult(item);
                foreach (var itemResult in rankResult)
                {
                    this.RankInfoList.First(x => x.KeyWord == (string)itemResult.post_key).Position = itemResult.result_position;
                    this.RankInfoList.First(x => x.KeyWord == (string)itemResult.post_key).Status = Status.tracked;
                }
            }

            DbProvider dbProvider = new DbProvider();
            foreach (var item in this.rankInfoList)
            {
                await dbProvider.SaveToDb(item);
            }
        }
    }
}
