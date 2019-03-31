using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nested.LinqExtensions;
using Nested.Samples.RegionsHierarchy;
using Nested.Samples.RegionsHierarchy.Utils;
using NUnit.Framework;

namespace Nested.LinqExtensions.Tests
{
    using Regions = List<RegionTree>;   
    public class Nested
    {
        private MyContext context;

        [SetUp]
        public void Init()
        {
            var opts = new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("RegionsHierarchy").Options;
            var context = new MyContext(opts);
            this.context = context;
        }
        public Dictionary<string, Region> SetupTree(MyContext context, Regions regionsTree)
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
            Visitor(regionsTree.Cast<EntityTree<Region>>().ToList(), null, (tree, parent) => context.Regions.AddNextChild(parent, tree.Item));
            Visitor(regionsTree.Cast<EntityTree<Region>>().ToList(), null, (tree, region) => regionsFlat.Add(tree.Item));

            context.SaveChanges();

            return regionsFlat.ToDictionary(r => r.Name, r => r);
        }

        [Test]
        public void AncestorsTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                ("R1", null), 
                //This creates Region with
                //Name = "R2" and list of child regions with same initialization
                ("R2", new Regions { 
                        //Depth 2
                        ("R2-1", new Regions { 
                                //Depth 3
                                ("R2-1-1", new Regions { 
                                        //Depth 4
                                        ("R2-1-1-1", new Regions { 
                                                //Depth 5
                                                ("R2-1-1-1-1", null)
                                            }
                                        ),
                                        ("R2-1-1-2", null),
                                        ("R2-1-1-3", new Regions {
                                                //Depth 5
                                                ("R2-1-1-3-1", null),
                                                ("R2-1-1-3-2", null),
                                                ("R2-1-1-3-3", null)
                                            }
                                        )
                                    }
                                )
                            }
                        ),
                        ("R2-2", null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.AncestorsOf(regions["R2-1-1-3"],includeSelf: true).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEqual(new[] {"R2", "R2-1", "R2-1-1", "R2-1-1-3"}, collection);
        }

        [Test]
        public void AncestorsTestWithNavigate()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var citiesCollection = context.Cities.AncestorsOf(c => c.Region, regions["R2-1-1-3"], includeSelf: true)
                .Select(c => c.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"C-R2", "C-R2-1", "C-R2-1-1", "C-R2-1-1-3"}, citiesCollection);
        }

        [Test]
        public void AncestorsTestWithoutSelf()
        {

            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.AncestorsOf(regions["R2-1-1-3"], includeSelf: false).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEqual(new[] {"R2", "R2-1", "R2-1-1"}, collection);
        }

        [Test]
        public void DescendantsTest()
        {

            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.DescendantsOf(regions["R2-1-1"], includeSelf: true).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1", "R2-1-1-1", "R2-1-1-2", "R2-1-1-3", "R2-1-1-1-1", "R2-1-1-3-1", "R2-1-1-3-2", "R2-1-1-3-3"},collection);

        }

        [Test]
        public void DescendantsTestWithoutSelf()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.DescendantsOf(regions["R2-1-1"], includeSelf: false).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-1", "R2-1-1-2", "R2-1-1-3", "R2-1-1-1-1", "R2-1-1-3-1", "R2-1-1-3-2", "R2-1-1-3-3"},collection);

        }

        [Test]
        public void DescendantsOfMultipleTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.DescendantsOfAny(new[] {regions["R2-1-1-1"], regions["R2-1-1-3"]}, includeSelf: true).Select(r => r.Name).ToList();
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-1", "R2-1-1-3", "R2-1-1-1-1", "R2-1-1-3-1", "R2-1-1-3-2", "R2-1-1-3-3"}, collection);
        }

        [Test]
        public void DescendantsOfMultipleWithoutSelfTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.DescendantsOfAny(new[] {regions["R2-1-1-1"], regions["R2-1-1-3"]}).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-1-1", "R2-1-1-3-1", "R2-1-1-3-2", "R2-1-1-3-3"}, collection);
        }

        [Test]
        public void DescendantsWithLimitedDepthTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var collection = context.Regions.DescendantsOf(regions["R2-1"], includeSelf: false, depth: 2).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1", "R2-1-1-1", "R2-1-1-2", "R2-1-1-3"}, collection);
        }

        [Test]
        public void ParentTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();
            
            //ACT
            var item = context.Regions.ParentOf(regions["R2-1-1-3"]).Name;

            //ASSERT
            Assert.AreEqual("R2-1-1", item);
        }

        [Test]
        public void ChildrenTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var children = context.Regions.ChildrenOf(regions["R2-1-1"]).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-1", "R2-1-1-2", "R2-1-1-3"}, children);
        }

        [Test]
        public void SiblingsTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var siblings = context.Regions.SiblingsOf(regions["R2-1-1-3-2"], true).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-3-1", "R2-1-1-3-2", "R2-1-1-3-3"}, siblings);
        }

        [Test]
        public void SiblignsWithoutSelfTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var siblings = context.Regions.SiblingsOf(regions["R2-1-1-3-2"]).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-3-1", "R2-1-1-3-3"}, siblings);
        }
        
        [Test]
        public void SiblingsBeforeTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var siblings = context.Regions.SiblingsBefore(regions["R2-1-1-3-2"], true).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-3-1", "R2-1-1-3-2"}, siblings);
        }

        [Test]
        public void SiglignsBeforeWithoutSelfTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var siblings = context.Regions.SiblingsBefore(regions["R2-1-1-3-2"]).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-3-1"}, siblings);
        }

        [Test]
        public void SiblignsAfterTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var siblings = context.Regions.SiblingsAfter(regions["R2-1-1-3-2"], true).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-3-2", "R2-1-1-3-3"}, siblings);
        }

        [Test]
        public void SiblingsAfterWithoutSelfTest()
        {
            //ARRANGE
            var regions  = SetupTree(context, new Regions { 
                //Depth 1
                (("R1", new[] {"C-R1"}), null), 
                //This creates Region with
                //Name = "R2", list of Cities with 1 City named "C-R2" and list of child regions with same initialization
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
                        (("R2-2", "C-R2-2"), null)
                    }
                )
            });
            context.SaveChanges();

            //ACT
            var siblings = context.Regions.SiblingsAfter(regions["R2-1-1-3-2"], false).Select(r => r.Name).ToList();

            //ASSERT
            CollectionAssert.AreEquivalent(new[] {"R2-1-1-3-3"}, siblings);
        }
    }
}
