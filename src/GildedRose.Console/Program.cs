using System.Collections.Generic;
using System.Linq;

namespace GildedRose.Console
{
    class Program
    {
        IList<Item> Items;
        static void Main(string[] args)
        {
            System.Console.WriteLine("OMGHAI!");
            
            var stock = new Stock(
                new List<Item>
                {
                    new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                    new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                    new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                    new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                    new Item
                    {
                        Name = "Backstage passes to a TAFKAL80ETC concert",
                        SellIn = 15,
                        Quality = 20
                    },
                    new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                });
            

            stock.UpdateQuality();

            System.Console.ReadKey();

        }
    }

    public class Stock
    {
        public List<Item> Items { get; private set; }

        public void UpdateQuality()
        {
            //Items.ForEach(i =>
            //{

            //        if (i.Name != "Aged Brie" && i.Name != "Backstage passes to a TAFKAL80ETC concert")
            //            // Items differenct from Brie and Backstage passes
            //        {
            //            if (i.Quality > 0) // Never negative quality
            //            {
            //                if (i.Name != "Sulfuras, Hand of Ragnaros")
            //                    // Special handling of item never degrading
            //                {
            //                    i.Quality = i.Quality - 1; // Decrease quality by 1
            //                }
            //            }
            //        }
            //        else // Age Brie and backstage passes, but not Sulfuras as it never degrades/increases in quality
            //        {
            //            if (i.Quality < 50) // 50 is max
            //            {
            //                i.Quality = i.Quality + 1; // Add to quality - 

            //                if (i.Name == "Backstage passes to a TAFKAL80ETC concert")
            //                    // Special handling of backstage passes
            //                {
            //                    if (i.SellIn < 11)
            //                    {
            //                        if (i.Quality < 50)
            //                        {
            //                            i.Quality = i.Quality + 1;
            //                        }
            //                    }

            //                    if (i.SellIn < 6)
            //                    {
            //                        if (i.Quality < 50)
            //                        {
            //                            i.Quality = i.Quality + 1;
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //    if (i.Name != "Sulfuras, Hand of Ragnaros")
            //    {
            //       i.SellIn = i.SellIn - 1;
            //    }


            //    if (i.SellIn < 0) // Once the sell by date has passed, Quality degrades twice as fast
            //        {
            //            if (i.Name != "Aged Brie")
            //            {
            //                if (i.Name != "Backstage passes to a TAFKAL80ETC concert")
            //                {
            //                    if (i.Quality > 0)
            //                    {
            //                        if (i.Name != "Sulfuras, Hand of Ragnaros")
            //                        {
            //                            i.Quality = i.Quality - 1;
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    i.Quality = i.Quality - i.Quality;
            //                }
            //            }
            //            else
            //            {
            //                if (i.Quality < 50)
            //                {
            //                    i.Quality = i.Quality + 1;
            //                }
            //            }
            //        }

            //});
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name != "Aged Brie" && Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                {
                    if (Items[i].Quality > 0)
                    {
                        if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                        {
                            Items[i].Quality = Items[i].Quality - 1;
                        }
                    }
                }
                else
                {
                    if (Items[i].Quality < 50)
                    {
                        Items[i].Quality = Items[i].Quality + 1; // Aged Brie  - second place Quality is added 

                        if (Items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (Items[i].SellIn < 11)
                            {
                                if (Items[i].Quality < 50)
                                {
                                    Items[i].Quality = Items[i].Quality + 1;
                                }
                            }

                            if (Items[i].SellIn < 6)
                            {
                                if (Items[i].Quality < 50)
                                {
                                    Items[i].Quality = Items[i].Quality + 1;
                                }
                            }
                        }
                    }
                }

                if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                {
                    Items[i].SellIn = Items[i].SellIn - 1;
                }

                if (Items[i].SellIn < 0)
                {
                    if (Items[i].Name != "Aged Brie")
                    {
                        if (Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                        {
                            if (Items[i].Quality > 0)
                            {
                                if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                                {
                                    Items[i].Quality = Items[i].Quality - 1;
                                }
                            }
                        }
                        else
                        {
                            Items[i].Quality = Items[i].Quality - Items[i].Quality;
                        }
                    }
                    else  // Aged Brie
                    {
                        if (Items[i].Quality < 50)
                        {
                            Items[i].Quality = Items[i].Quality + 1;
                        }
                    }
                }
            }



        }

        private Item UpdateSellIn(Item arg)
        {
            return arg.Name != "Sulfuras, Hand of Ragnaros"
                ? new Item{
                    Name = arg.Name,
                    Quality = arg.Quality,
                    SellIn = --arg.SellIn
                }
                : arg;
          

        }

        public Stock(List<Item> stockedItems)
        {
            Items = stockedItems;
        }

    }

    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }

}
