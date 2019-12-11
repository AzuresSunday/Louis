using System;
using GalaSoft.MvvmLight;
using Louis.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Louis.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Field

        private bool _initialization = false;
        private string BaseUrl = "https://movie.douban.com/j/new_search_subjects?sort={0}&range=0,10&tags={1}&start={2}";
        private int start = 0;
        private const int FACTORSTART = 20;

        private readonly Dictionary<string, string> Genres = new Dictionary<string, string>()
        {
            {"剧情", "剧情"}, {"喜剧", "喜剧"}, {"动作", "动作"}, {"爱情", "爱情"}, {"科幻", "科幻"}, {"动画", "动画"}, {"悬疑", "悬疑"},
            {"惊悚", "惊悚"}, {"恐怖", "恐怖"}, {"犯罪", "犯罪"}, {"同性", "同性"}, {"音乐", "音乐"},
            {"歌舞", "歌舞"}, {"传记", "传记"}, {"历史", "历史"}, {"战争", "战争"}, {"西部", "西部"}, {"奇幻", "奇幻"}, {"冒险", "冒险"},
            {"灾难", "灾难"}, {"武侠", "武侠"}, {"情色", "情色"}
        };
        private readonly Dictionary<string, string> Regional = new Dictionary<string, string>()
        {
            {"中国大陆", "中国大陆"}, {"中国香港", "中国香港"}, {"中国台湾", "中国台湾"}, {"美国", "美国"}, {"日本", "日本"}, {"韩国", "韩国"},
            {"法国", "法国"}, {"德国", "德国"}, {"意大利", "意大利"},
            {"西班牙", "西班牙"}, {"印度", "印度"}, {"泰国", "泰国"}, {"俄罗斯", "俄罗斯"}, {"伊朗", "伊朗"}, {"加拿大", "加拿大"}, {"爱尔兰", "爱尔兰"},
            {"瑞典", "瑞典"}, {"巴西", "巴西"}, {"丹麦", "丹麦"}
        };
        private readonly Dictionary<string, string> Age = new Dictionary<string, string>()
        {
            {"2019", "2019"}, {"2018", "2018"}, {"2010年代", "2010,2019"}, {"2000年代", "2000,2009"}, {"90年代", "1990,1999"},
            {"80年代", "1980,1989"}, {"70年代", "1970,1979"}, {"60年代", "1960,1969"}, {"更早", "1,1959"}
        };

        private readonly WebClient _webClient;
        private bool _asking = false;

        #endregion

        #region Property

        public const string FilmCollectionPropertyName = "FilmCollection";
        public const string FilmSelectedPropertyName = "FilmSelected";
        public const string SortCollectionPropertyName = "SortCollection";
        public const string SortSelectedPropertyName = "SortSelected";
        public const string SearchWordPropertyName = "SearchWord";

        private ObservableCollection<FilmModel> _filmCollction = null;
        private FilmModel _filmSelected = null;
        private ObservableCollection<KeyValuePair<string, string>> _sortCollection = new ObservableCollection<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("近期热门", "U"), new KeyValuePair<string, string>("标记最多", "T"),
                new KeyValuePair<string, string>("评分最高", "S"), new KeyValuePair<string, string>("最新上映", "R")
            };
        private KeyValuePair<string, string> _sortSelected = new KeyValuePair<string, string>("", "");
        private string _searchWord = string.Empty;

        public ObservableCollection<FilmModel> FilmCollection
        {
            get => _filmCollction;
            set => Set(FilmCollectionPropertyName, ref _filmCollction, value);
        }

        public FilmModel FilmSelected
        {
            get => _filmSelected;
            set => Set(FilmSelectedPropertyName, ref _filmSelected, value);
        }

        public ObservableCollection<KeyValuePair<string,string>> SortCollection
        {
            get => _sortCollection;
            set => Set(SortCollectionPropertyName, ref _sortCollection, value);
        }

        public KeyValuePair<string,string> SortSelected
        {
            get => _sortSelected;
            set
            {
                Set(SortSelectedPropertyName, ref _sortSelected, value);

                start = 0;
                if (_initialization)
                    RequestData();
            }
        }

        public string SearchWord
        {
            get => _searchWord;
            set => Set(SearchWordPropertyName, ref _searchWord, value);
        }

        #region Tags
        
        public const string GenresCollectionPropertyName = "GenresCollection";
        public const string RegionalCollectionPropertyName = "RegionalCollection";
        public const string AgeCollectionPropertyName = "AgeCollection";
        public const string GenresSelectedPropertyName = "GenresSelected";
        public const string RegionalSelectedPropertyName = "RegionalSelected";
        public const string AgeSelectedPropertyName = "AgeSelected";

        private ObservableCollection<Tag> _genresCollecion = null;
        private ObservableCollection<Tag> _regionalCollection = null;
        private ObservableCollection<Tag> _agecollection = null;
        private Tag _genresSelected = null;
        private Tag _regionalSelected = null;
        private Tag _ageSelected = null;

        public ObservableCollection<Tag> GenresCollection
        {
            get => _genresCollecion;
            set => Set(GenresCollectionPropertyName, ref _genresCollecion, value);
        }
        
        public ObservableCollection<Tag> RegionalCollection
        {
            get => _regionalCollection;
            set => Set(RegionalCollectionPropertyName, ref _regionalCollection, value);
        }

        public ObservableCollection<Tag> AgeCollection
        {
            get => _agecollection;
            set => Set(AgeCollectionPropertyName, ref _agecollection, value);
        }
        
        public Tag GenresSelected
        {
            get => _genresSelected;
            set
            {
                Set(GenresSelectedPropertyName, ref  _genresSelected, value);

                start = 0;
                if (_initialization)
                    RequestData();
            }
        }
        
        public Tag RegionalSelected
        {
            get => _regionalSelected;
            set
            {
                Set(RegionalSelectedPropertyName, ref _regionalSelected, value);

                start = 0;
                if (_initialization)
                    RequestData();
            }
        }

        public Tag AgeSelected
        {
            get => _ageSelected;
            set
            {
                Set(AgeSelectedPropertyName, ref _ageSelected, value);

                start = 0;
                if (_initialization)
                    RequestData();
            }
        }

        #endregion

        #endregion

        #region Command

        private RelayCommand _focusSearchCommand;
        private RelayCommand _srarchCommand;

        /// <summary>
        /// Gets the FocusSearchCommand.
        /// </summary>
        public RelayCommand FocusSearchCommand
        {
            get
            {
                return _focusSearchCommand
                    ?? (_focusSearchCommand = new RelayCommand(
                        () => { Messenger.Default.Send(true, "FocusOnSearch"); }));
            }
        }

        /// <summary>
        /// Gets the SearchCommand.
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                return _srarchCommand
                    ?? (_srarchCommand = new RelayCommand(
                    async () =>
                    {
                        var tags = string.Join(",", "电影", SearchWord);
                        var url = string.Format(BaseUrl, SortSelected.Value, tags);
                        var data = await _webClient.DownloadStringTaskAsync(url);
                        var result = JsonConvert.DeserializeObject<FilmResult>(data);

                        FilmCollection.Clear();
                        FilmCollection = new ObservableCollection<FilmModel>(
                            result.data.Select(r => new FilmModel()
                            {
                                Id = r.id,
                                Directors = r.directors,
                                Casts = r.casts,
                                Title = r.title,
                                Rate = r.rate,
                                Cover = r.cover,
                                Url = r.url,
                                Star = r.star
                            }));
                    }));
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Messenger.Default.Register<bool>(this, "RequestData", p => RequestData());

            _webClient = new WebClient();

            FilmCollection = new ObservableCollection<FilmModel>();
            GenresCollection = new ObservableCollection<Tag>();
            RegionalCollection = new ObservableCollection<Tag>();
            AgeCollection = new ObservableCollection<Tag>();
            Initialization();
            RequestData();

            _initialization = true;
        }

        #region Method

        private void Initialization()
        {
            _webClient.Encoding = Encoding.UTF8;

            var i = 0;
            foreach (var genre in Genres)
                GenresCollection.Add(new Tag(i++, genre.Key, genre.Value));

            i = 0;
            foreach (var reg in Regional)
                RegionalCollection.Add(new Tag(i++, reg.Key, reg.Value));

            i = 0;
            foreach (var age in Age)
                AgeCollection.Add(new Tag(i++, age.Key, age.Value));

            GenresSelected = new Tag(-1, "", "");
            RegionalSelected = new Tag(-1, "", "");
            AgeSelected = new Tag(-1, "", "");
            SortSelected = SortCollection.FirstOrDefault();
        }

        private async void RequestData()
        {
            if (_asking) return;

            var tags = string.Join(",", "电影", GenresSelected.Value, RegionalSelected.Value, AgeSelected.Value);
            var url = string.Format(BaseUrl, SortSelected.Value, tags, start);
            _asking = true;
            var data = await _webClient.DownloadStringTaskAsync(url);
            var result = JsonConvert.DeserializeObject<FilmResult>(data);

            var list = result.data.Select(r => new FilmModel()
            {
                Id = r.id,
                Directors = r.directors,
                Casts = r.casts,
                Title = r.title,
                Rate = r.rate,
                Cover = r.cover,
                Url = r.url,
                Star = r.star
            });

            foreach (var filmModel in list)
            {
                FilmCollection.Add(filmModel);
            }

            start += FACTORSTART;

            _asking = false;
        }

        #endregion
    }
}