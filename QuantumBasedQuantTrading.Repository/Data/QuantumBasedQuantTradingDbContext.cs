using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuantumBasedQuantTrading.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuantumBasedQuantTrading.Repository.Data
{
    public partial class QuantumBasedQuantTradingDbContext : IdentityDbContext
    {
        public virtual DbSet<AllArticles> AllArticlesSet { get; set; }
        public virtual DbSet<FulloutAllArticles> FulloutAllArticlesSet { get; set; }
        public virtual DbSet<OutputSent> OutputSentSet { get; set; }
        public virtual DbSet<AverageSent> AverageSentSet { get; set; }
        public virtual DbSet<Users> SiteUsersSet { get; set; }
        public virtual DbSet<RequestParameters> RequestParametersSet { get; set; }
        public virtual DbSet<MachineLearningModelData> MachineLearningSet { get; set; }

        public QuantumBasedQuantTradingDbContext(DbContextOptions<QuantumBasedQuantTradingDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //output sent articles data
            #region
            //var outputSentList = new List<OutputSent>();
            //string[] csvLines = File.ReadAllLines("C:/Users/Ábel/Desktop/Prog_nyelvek/ProjectWork/Algorithms/Data/OutputSent/output_sentiments_normalized_Berkshire Hathaway_2024-04-06_2024-11-10.csv");

            //for (int i = 1; i < csvLines.Length; i++)
            //{
            //    var columns = csvLines[i].Split(',');

            //    var outputSent = new OutputSent
            //    {
            //        OutputSentID = i,
            //        Published_at = DateTime.ParseExact(columns[0], "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            //        TitleSentiment = double.Parse(columns[1], CultureInfo.InvariantCulture),
            //        ContentSentiment = double.Parse(columns[2], CultureInfo.InvariantCulture),
            //        DescriptionSentiment = double.Parse(columns[3], CultureInfo.InvariantCulture)
            //    };

            //    outputSentList.Add(outputSent);
            //}
            //modelBuilder.Entity<OutputSent>().HasData(outputSentList);
            #endregion

            //avg sent articles data
            #region
            //var avgSentList = new List<AverageSent>();
            //string[] csvavgLines = File.ReadAllLines("C:/Users/Ábel/Desktop/Prog_nyelvek/ProjectWork/Algorithms/Data/AverageSent/average_sentiments_Berkshire Hathaway_2024-04-06_2024-11-10.csv");
            //for (int i = 1; i < csvavgLines.Length; i++)
            //{
            //    var columns = csvavgLines[i].Split(',');

            //    var outputSent = new AverageSent
            //    {
            //        AverageSentID = i,
            //        Published_at = DateTime.ParseExact(columns[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            //        AvgTitleSentiment = double.Parse(columns[1], CultureInfo.InvariantCulture),
            //        AvgContentSentiment = double.Parse(columns[2], CultureInfo.InvariantCulture),
            //        AvgDescriptionSentiment = double.Parse(columns[3], CultureInfo.InvariantCulture)
            //    };

            //    avgSentList.Add(outputSent);
            //}
            //modelBuilder.Entity<AverageSent>().HasData(avgSentList);
            #endregion
            base.OnModelCreating(modelBuilder);
        }

    }
}
