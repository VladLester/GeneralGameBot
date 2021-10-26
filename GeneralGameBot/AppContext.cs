﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralGameBot.Entities;

namespace GeneralGameBot
{
    class AppContext:DbContext
    {
        public DbSet<Entities.General> Generals { get; set; }
        public AppContext()
        {
            Database.EnsureDeleted();//!!!!! удалить для тестов по загрузке или запуске в паблик !!!!
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=GeneralsDB;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<General>().Property(g => g.HP).HasDefaultValue(100);
            modelBuilder.Entity<General>().Property(g => g.PhotoUrl).HasDefaultValue("https://images-ext-2.discordapp.net/external/B9sqm34yAAxLcvvs6u5ctDWVzKBiCLMZ0o-eWWaqVJ8/https/upload.wikimedia.org/wikipedia/commons/thumb/9/98/Stalin_in_March_1935.jpg/1200px-Stalin_in_March_1935.jpg?width=998&height=657");
            modelBuilder.Entity<General>().Property(g => g.DateOfCreating).HasDefaultValueSql("GETDATE()");
        }
    }
}
