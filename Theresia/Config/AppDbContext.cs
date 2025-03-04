using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresia.Entity;

namespace Theresia.Config
{
    public class AppDbContext : DbContext
    {
        public DbSet<TagEntity> Tag { get; set; } // 创建 Tag 表

        public DbSet<SettingEntity> Setting { get; set; }

        public DbSet<SeriesEntity> Series { get; set; }

        public DbSet<CastCrewEntity> CastCrew { get; set; }

        public DbSet<CastCrewPhotoEntity> CastCrewPhoto { get; set; }

        public DbSet<MovieEntity> Movie { get; set; }

        public DbSet<MovieTagsEntity> MovieTags { get; set; }

        public DbSet<MovieCastEntity> MovieCast { get; set; }

        public DbSet<MediaMetadataEntity> MediaMetadata { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    //.UseLoggerFactory(App.loggerFactory)  // 使用 loggerFactory 配置日志
                    //.EnableSensitiveDataLogging()  // 启用敏感数据日志（如果需要查看参数化查询的值
                    .UseSqlite("Data Source=app.db");// SQLite 数据库文件
            }
        }
    }
}
