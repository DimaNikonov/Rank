using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rank.Models
{
    public class RankInfo: ViewModelBase
    {
        private string position;
        private Status status;
        private string keyWord;
        private string searchEngine;
        private string webSite;

        public int Id { get; set; }
        public string KeyWord
        {
            get => this.keyWord;
            set
            {
                if (this.keyWord != value)
                {
                    this.keyWord = value;
                    base.RaisePropertyChanged(nameof(this.KeyWord));
                }
            }
        }
        public string SearchEngine
        {
            get => this.searchEngine;
            set
            {
                if (this.searchEngine != value)
                {
                    this.searchEngine = value;
                    base.RaisePropertyChanged(nameof(this.SearchEngine));
                }
            }
        }
        public string WebSite
        {
            get => this.webSite;
            set
            {
                if (this.webSite != value)
                {
                    this.webSite = value;
                    base.RaisePropertyChanged(nameof(this.WebSite));
                }
            }
        }
        public string Position
        {
            get => this.position;
            set
            {
                if (this.position != value)
                {
                    this.position = value;
                    base.RaisePropertyChanged(nameof(this.Position));
                }
            }
        }
        public Status Status
        {
            get => this.status;
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    base.RaisePropertyChanged(nameof(this.Status));
                }
            }
        }
    }
}
