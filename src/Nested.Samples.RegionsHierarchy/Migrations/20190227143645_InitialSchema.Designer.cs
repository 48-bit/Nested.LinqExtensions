﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nested.Samples.RegionsHierarchy.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20190227143645_InitialSchema")]
    partial class InitialSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NTrees.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("RegionId");

                    b.HasKey("Id");

                    b.HasIndex("RegionId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("NTrees.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RoleId");

                    b.Property<long>("TreeEntryId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("TreeEntryId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("NTrees.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<long>("TreeEntryId");

                    b.HasKey("Id");

                    b.HasIndex("TreeEntryId");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("NTrees.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("NTrees.TreeEntry", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Depth");

                    b.Property<long>("Dv");

                    b.Property<long>("Nv");

                    b.Property<long>("SDv");

                    b.Property<long>("SNv");

                    b.HasKey("Id");

                    b.ToTable("ResourcesHierarchies");
                });

            modelBuilder.Entity("NTrees.City", b =>
                {
                    b.HasOne("NTrees.Region", "Region")
                        .WithMany("Cities")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NTrees.Permission", b =>
                {
                    b.HasOne("NTrees.Role", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NTrees.TreeEntry", "TreeEntry")
                        .WithMany()
                        .HasForeignKey("TreeEntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NTrees.Region", b =>
                {
                    b.HasOne("NTrees.TreeEntry", "TreeEntry")
                        .WithMany()
                        .HasForeignKey("TreeEntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
