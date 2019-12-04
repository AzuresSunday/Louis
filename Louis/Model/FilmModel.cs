using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace Louis.Model
{
    public class FilmModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 导演
        /// </summary>
        public List<string> Directors { get; set; }
        /// <summary>
        /// 演职员
        /// </summary>
        public List<string> Casts { get; set; }
        /// <summary>
        /// 电影名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public string Rate { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 细节地址
        /// </summary>
        public string Url { get; set; }
        public string Star { get; set; }
    }
}