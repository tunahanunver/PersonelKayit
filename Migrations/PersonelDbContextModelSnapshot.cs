﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonelKayit.Models;

#nullable disable

namespace PersonelKayit.Migrations
{
    [DbContext(typeof(PersonelDbContext))]
    partial class PersonelDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PersonelKayit.Models.Lokasyon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Lokasyonlar");
                });

            modelBuilder.Entity("PersonelKayit.Models.MedyaKutuphanesi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedyaGuid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MedyaKutuphaneleri");
                });

            modelBuilder.Entity("PersonelKayit.Models.Personel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Aciklama")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Cinsiyet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DogumTarihi")
                        .HasColumnType("datetime2");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LokasyonId")
                        .HasColumnType("int");

                    b.Property<int?>("PersonelMedyaId")
                        .HasColumnType("int");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("LokasyonId");

                    b.HasIndex("PersonelMedyaId");

                    b.ToTable("Personeller");
                });

            modelBuilder.Entity("PersonelKayit.Models.PersonelMedya", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PersonelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PersonelId");

                    b.ToTable("PersonelMedyalari");
                });

            modelBuilder.Entity("PersonelKayit.Models.Personel", b =>
                {
                    b.HasOne("PersonelKayit.Models.Lokasyon", "Lokasyon")
                        .WithMany("Personels")
                        .HasForeignKey("LokasyonId");

                    b.HasOne("PersonelKayit.Models.PersonelMedya", null)
                        .WithMany("Personels")
                        .HasForeignKey("PersonelMedyaId");

                    b.Navigation("Lokasyon");
                });

            modelBuilder.Entity("PersonelKayit.Models.PersonelMedya", b =>
                {
                    b.HasOne("PersonelKayit.Models.Personel", "Personel")
                        .WithMany()
                        .HasForeignKey("PersonelId");

                    b.Navigation("Personel");
                });

            modelBuilder.Entity("PersonelKayit.Models.Lokasyon", b =>
                {
                    b.Navigation("Personels");
                });

            modelBuilder.Entity("PersonelKayit.Models.PersonelMedya", b =>
                {
                    b.Navigation("Personels");
                });
#pragma warning restore 612, 618
        }
    }
}
