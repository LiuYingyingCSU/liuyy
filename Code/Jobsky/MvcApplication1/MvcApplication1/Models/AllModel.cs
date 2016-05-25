using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class AllModel
    {
        public Admin admin;
        public News news;
        public ArticleType articleType;
        public Article article;
        public List<DemandInfo> demandInfo;
        public TempArticle tempArticle;
        public List<TempDemandInfo> tempDemandInfo;
        public PlaceListFirst placeListFirst;
        public PlaceListSecond placeListSecond;
        public Employer employer;

        public AllModel(Admin admin, News news, ArticleType articleType, Article article, List<DemandInfo> demandInfo, TempArticle tempArticle, List<TempDemandInfo> tempDemandInfo, PlaceListFirst placeListFirst, PlaceListSecond placeListSecond, Employer employer)
        {
            this.admin = admin;
            this.news = news;
            this.articleType = articleType;
            this.article = article;
            this.demandInfo = demandInfo;
            this.tempArticle = tempArticle;
            this.tempDemandInfo = tempDemandInfo;
            this.placeListFirst = placeListFirst;
            this.placeListSecond = placeListSecond;
            this.employer = employer;
        }

    }
}