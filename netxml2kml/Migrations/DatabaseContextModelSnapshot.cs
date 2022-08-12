﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using netxml2kml.Data;

#nullable disable

namespace netxml2kml.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("netxml2kml.Models.WirelessClient", b =>
                {
                    b.Property<string>("Mac")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FirstSeenDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Mac");

                    b.ToTable("WirelessClients");
                });

            modelBuilder.Entity("netxml2kml.Models.WirelessConnection", b =>
                {
                    b.Property<string>("WirelessNetworkBssid")
                        .HasColumnType("TEXT");

                    b.Property<string>("WirelessClientMac")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FirstSeenDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("TEXT");

                    b.HasKey("WirelessNetworkBssid", "WirelessClientMac");

                    b.HasIndex("WirelessClientMac");

                    b.ToTable("WirelessConnections");
                });

            modelBuilder.Entity("netxml2kml.Models.WirelessNetwork", b =>
                {
                    b.Property<string>("Bssid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Encryption")
                        .HasColumnType("TEXT");

                    b.Property<string>("Essid")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FirstSeenDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("FrequencyMhz")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("MaxAltitude")
                        .HasColumnType("REAL");

                    b.Property<double>("MaxLatitude")
                        .HasColumnType("REAL");

                    b.Property<double>("MaxLongitude")
                        .HasColumnType("REAL");

                    b.Property<int>("MaxSignalDbm")
                        .HasColumnType("INTEGER");

                    b.HasKey("Bssid");

                    b.ToTable("WirelessNetworks");
                });

            modelBuilder.Entity("netxml2kml.Models.WirelessConnection", b =>
                {
                    b.HasOne("netxml2kml.Models.WirelessClient", "WirelessClient")
                        .WithMany("WirelessConnections")
                        .HasForeignKey("WirelessClientMac")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("netxml2kml.Models.WirelessNetwork", "WirelessNetwork")
                        .WithMany("WirelessConnections")
                        .HasForeignKey("WirelessNetworkBssid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WirelessClient");

                    b.Navigation("WirelessNetwork");
                });

            modelBuilder.Entity("netxml2kml.Models.WirelessClient", b =>
                {
                    b.Navigation("WirelessConnections");
                });

            modelBuilder.Entity("netxml2kml.Models.WirelessNetwork", b =>
                {
                    b.Navigation("WirelessConnections");
                });
#pragma warning restore 612, 618
        }
    }
}
