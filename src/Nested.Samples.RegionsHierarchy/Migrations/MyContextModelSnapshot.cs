﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Nested.Samples.RegionsHierarchy.Migrations
{
    [DbContext(typeof(MyContext))]
    partial class MyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EFNested.City", b =>
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

            modelBuilder.Entity("EFNested.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RoleId");

                    b.Property<long?>("TreeEntryDv");

                    b.Property<long>("TreeEntryId");

                    b.Property<long?>("TreeEntryNv");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("TreeEntryNv", "TreeEntryDv");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("EFNested.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<long?>("TreeEntryDv");

                    b.Property<long>("TreeEntryId");

                    b.Property<long?>("TreeEntryNv");

                    b.HasKey("Id");

                    b.HasIndex("TreeEntryNv", "TreeEntryDv");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("EFNested.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Nested.LinqExtensions.TreeEntry", b =>
                {
                    b.Property<long>("Nv");

                    b.Property<long>("Dv");

                    b.Property<int>("Depth");

                    b.Property<long>("SDv");

                    b.Property<long>("SNv");

                    b.HasKey("Nv", "Dv");

                    b.ToTable("ResourcesHierarchies");
                });

            modelBuilder.Entity("EFNested.City", b =>
                {
                    b.HasOne("EFNested.Region", "Region")
                        .WithMany("Cities")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EFNested.Permission", b =>
                {
                    b.HasOne("EFNested.Role", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Nested.LinqExtensions.TreeEntry", "TreeEntry")
                        .WithMany()
                        .HasForeignKey("TreeEntryNv", "TreeEntryDv");
                });

            modelBuilder.Entity("EFNested.Region", b =>
                {
                    b.HasOne("Nested.LinqExtensions.TreeEntry", "TreeEntry")
                        .WithMany()
                        .HasForeignKey("TreeEntryNv", "TreeEntryDv");
                });
#pragma warning restore 612, 618
        }
    }
}
