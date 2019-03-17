using System;
using System.Collections.Generic;
using System.Linq;
using EFNested.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nested.LinqExtensions;
using Nested.LinqExtensions.Utils;
using Z.EntityFramework.Plus;

namespace EFNested
{
    using Regions = List<RegionTree>;   
    public class MyDesignTimeContextFactory : IDesignTimeDbContextFactory<MyContext>
    {
        public MyContext CreateDbContext(string[] args)
        {
            return Program.CreateServiceProvider().GetService<MyContext>();
        }
    }

    public class City
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public string Name { get; set; }

        public static implicit operator City(string name)
        {
            return new City() {Name = name};
        }
    }

    public class Role
    {
        public IList<Permission> Permissions { get; set; }
        public int Id { get; set; }
    }

    public class Permission : IHasTreeEntry
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public long TreeEntryId { get; set; }
        public TreeEntry TreeEntry { get; set; }
    }

    class Program
    {
        public static void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(configure => configure.AddConsole(options => options.IncludeScopes = true));
            services.AddDbContext<MyContext>(opt =>
            {
                opt.UseSqlServer(
                    "Data Source=127.0.0.1; Initial Catalog=test_ef; Persist Security Info=True; User ID=sa; Password=3133731337");
            });
        }

        public static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var sp = services.BuildServiceProvider();
            return sp;
        }

        public static Dictionary<string, Region> SetupTree(Regions regionsTree, MyContext context)
        {
            void Visitor(List<EntityTree<Region>> items, Region parent, Action<EntityTree<Region>, Region> visitAction)
            {
                if (items == null) return;

                foreach (var entityTree in items)
                {
                    visitAction(entityTree, parent);
                    Visitor(entityTree.Children, entityTree.Item, visitAction);
                }
            }

            List<Region> regionsFlat = new List<Region>();
            Visitor(regionsTree.Cast<EntityTree<Region>>().ToList(), null, (tree, parent) => 
                context.Regions.AddNextChild(parent, tree.Item));
            Visitor(regionsTree.Cast<EntityTree<Region>>().ToList(), null, (tree, region) => regionsFlat.Add(tree.Item));

            return regionsFlat.ToDictionary(r => r.Name, r => r);
        }

        
        static void Main(string[] args)
        {
            var sp = CreateServiceProvider();
            var context = sp.GetService<MyContext>();
            var logger = sp.GetService<ILoggerProvider>().CreateLogger("MAIN");
            context.Database.Migrate();
            context.Regions.RemoveRange(context.Regions);
            context.ResourcesHierarchies.RemoveRange(context.ResourcesHierarchies);
            context.SaveChanges();

            var regions = SetupTree(new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This notation creates Region with
                //  Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions initialized same way
                (("R2", new[] {"C-R2"}), new Regions { 
                        //Depth 2
                        (("R2-1", new[] {"C-R2-1"}), new Regions { 
                                //Depth 3
                                (("R2-1-1", new[] {"C-R2-1-1"}), new Regions { 
                                        //Depth 4
                                        (("R2-1-1-1", new[] {"C-R2-1-1-1"} ), new Regions { 
                                                //Depth 5
                                                (("R2-1-1-1-1", new[] {"C-R2-1-1-1-1"}), null)
                                            }
                                        ),
                                        (("R2-1-1-2", new[] {"C-R2-1-1-2"} ), null),
                                        (("R2-1-1-3", new[] {"C-R2-1-1-3"}), new Regions {
                                                //Depth 5
                                                (("R2-1-1-3-1", new[] {"C-R2-1-1-3-1"}), null),
                                                (("R2-1-1-3-2", new[] {"C-R2-1-1-3-2"}), null),
                                                (("R2-1-1-3-3", new[] {"C-R2-1-1-3-3"}), null)
                                            }
                                        )
                                    }
                                )
                            }
                        ),
                        (("R2-2", "C-R2-2"), null),
                        ("R2-3", null),
                        ("R2-4", null)
                    }
                )
            }, context);
            context.SaveChanges();

            var srcRoot = regions["R2-1"].TreeEntry;
            var tgtRoot = regions["R2-4"].TreeEntry;
            var depthDiff = tgtRoot.Depth - srcRoot.Depth;
            
            var relocation =
                NestedIntervalMath.BuildSubtreeRelocationMatrix(regions["R2-1"].TreeEntry, regions["R2-4"].TreeEntry);

            var oldPos = NestedIntervalMath.GetIntervalByPosition(regions["R2-1-1"].TreeEntry, 3);
            var r221 = NestedIntervalMath.GetIntervalByPosition(tgtRoot, 1);
            var r2211 = NestedIntervalMath.GetIntervalByPosition(r221, 1);
            var r22111 = NestedIntervalMath.GetIntervalByPosition(r2211, 1);
            var r2212 = NestedIntervalMath.GetIntervalByPosition(r221, 2);
            var r2213 = NestedIntervalMath.GetIntervalByPosition(r221, 3);
            var r22131 = NestedIntervalMath.GetIntervalByPosition(r2213, 1);
            var r22132 = NestedIntervalMath.GetIntervalByPosition(r2213, 2);
            var r22133 = NestedIntervalMath.GetIntervalByPosition(r2213, 3);

            var testPositon = NestedIntervalMath.GetIntervalByPath(new long[] {2, 2, 1, 3, 3});
            var pos2213 = NestedIntervalMath.GetIntervalByPath(new long[] {2, 2, 1, 3});
            var pos2113 = NestedIntervalMath.GetIntervalByPath(new long[]{2, 1, 1, 3});

            var data = context.Regions.DescendantsOf(regions["R2-1"]).Include(i => i.TreeEntry).ToList();

            long relocation00 = relocation[0,0], 
                relocation10 = relocation[1,0], 
                relocation01 = relocation[0, 1], 
                relocation11 = relocation[1, 1];
            var filtered = context.ResourcesHierarchies.Where(NestedIntervalsSpec.DescendantsOf(srcRoot, false, -1)).ToList();

            var updated = context.ResourcesHierarchies.Where(NestedIntervalsSpec.DescendantsOf(srcRoot, false, -1))
                .Update(entry =>
                        new TreeEntry()
                        {
                            Nv =  relocation00 * entry.Nv + relocation01 * entry.Dv,
                            Dv =  relocation10 * entry.Nv + relocation11 * entry.Dv,
                            SNv = relocation00 * entry.SNv + relocation01 * entry.SDv,
                            SDv = relocation10 * entry.SNv + relocation11 * entry.SDv,
                            Depth = entry.Depth + depthDiff
                        }
                );
            context.SaveChanges();
            var r24Child = context.Regions.DescendantsOf(regions["R2-4"]);

            foreach (var region in data)
            {
                //var desiredInterval = NestedIntervalMath.GetIntervalByPosition()
                var updatedInterval = NestedIntervalMath.MatrixToInterval(
                    NestedIntervalMath.MultiplyMatrix(relocation,
                        NestedIntervalMath.IntervalToMatrix(region.TreeEntry)));
                region.TreeEntry.Depth += depthDiff;
                region.TreeEntry.SetFromInterval(updatedInterval);
            }

            context.SaveChanges();

            var upd = context.Regions.DescendantsOf(regions["R2-2"]);
            //-- nvc * dvp >= dvc * nvp && snvc * sdvp <= sdvc * snvp
            var child = regions["R2-1-1-3-1"];
            var anclist = NestedIntervalMath.BuildAncestorsList(child.TreeEntry.Nv, child.TreeEntry.Dv);
            //var ancestors = context.Regions.DescendantsOf(regions["R2-1-1-3"], true);

            var roles = new List<Role>();

            var regionsDescendants = context.Regions.DescendantsOfAny(roles.SelectMany(r => r.Permissions), true);

            var ancestors = context.Cities.DescendantsOfAny(c => c.Region, new[] {regions["R2-1-1-3-1"], regions["R2-1-1-1"],regions["R1"], regions["R2-2"]}, true);

            foreach (var ancestor in ancestors)
            {
                Console.WriteLine(ancestor.Name);
            }
            logger.Log(LogLevel.Debug, "HELLO WORLD");
            Console.ReadKey();
        }
    }
}
