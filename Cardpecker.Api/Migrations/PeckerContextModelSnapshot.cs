﻿// <auto-generated />
using System;
using Cardpecker.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cardpecker.Api.Migrations
{
    [DbContext(typeof(PeckerContext))]
    partial class PeckerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Cardpecker.Api.Core.WorkerServices.WorkerState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset?>("LastRun")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("WorkerStates");
                });

            modelBuilder.Entity("Cardpecker.Api.MagicCardInfo", b =>
                {
                    b.Property<Guid>("ScryfallId")
                        .HasColumnType("uuid");

                    b.HasKey("ScryfallId");

                    b.ToTable("CardInfos", "Magic");
                });

            modelBuilder.Entity("Cardpecker.Api.MagicCardPricingPoint", b =>
                {
                    b.Property<Guid>("ScryfallId")
                        .HasColumnType("uuid");

                    b.Property<string>("PricingProvider")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("PrintingVersion")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<bool>("IsMagicOnline")
                        .HasColumnType("boolean");

                    b.Property<string>("Currency")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<DateOnly>("PriceDate")
                        .HasColumnType("date");

                    b.HasKey("ScryfallId", "PricingProvider", "PrintingVersion", "IsMagicOnline", "Currency");

                    b.HasIndex("ScryfallId");

                    b.ToTable("PricingPoints", "Magic");
                });

            modelBuilder.Entity("Cardpecker.Api.MagicCardPricingPoint", b =>
                {
                    b.HasOne("Cardpecker.Api.MagicCardInfo", null)
                        .WithMany("Pricings")
                        .HasForeignKey("ScryfallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cardpecker.Api.MagicCardInfo", b =>
                {
                    b.Navigation("Pricings");
                });
#pragma warning restore 612, 618
        }
    }
}
