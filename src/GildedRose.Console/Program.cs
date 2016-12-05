using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

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
            var stockedItems = BuildItemsWithRules(Items);

            var defaultHandling = UpdateQualityForItems(
                                    UpdateSellInForItems(stockedItems
                                        .Where(s => s.Rule == BusinessRule.NoSpecialHandling).Select(s=>s.Item).ToList()));

            var legendaryItems = stockedItems.Where(s => s.Rule == BusinessRule.LegendaryStuff).Select(s=>s.Item);

            var agedGoods = UpdateQualityForAgedGoods(
                                UpdateSellInForAgedGoods(stockedItems.Where(s => s.Rule == BusinessRule.AgedGoods).Select(s=>s.Item).ToList()));

            var backstagePasses = UpdateQualityAndSellInForBackstagePasses(
                                    stockedItems.Where(s=>s.Rule==BusinessRule.BackstagePassesAndSimilar).Select(s=>s.Item).ToList());

            var conjuredItems =
                UpdateQualityForConjuredItems(
                    UpdateSellInForItems(stockedItems.Where(s => s.Rule == BusinessRule.ConjuredItems).Select(s=>s.Item).ToList()));

            Items = defaultHandling.Concat(legendaryItems).Concat(agedGoods).Concat(backstagePasses).Concat(conjuredItems).ToList();

        }

        private List<Item> UpdateQualityForConjuredItems(List<Item> items)
        {
            // Let's reuse standard Quality degradation method here
            // Of course this could be written using some LINQ, but the "Twice as fast as normal items" rule used in documentation is interesting.
            return UpdateQualityForItems(
                UpdateQualityForItems(items));
        }

        private List<Item> UpdateQualityAndSellInForBackstagePasses(List<Item> items)
        {
            // The structure in the original code example, sets the quality before setting the SellIn; doing so, requires me to look at the original SellIn for setting the right quality.
            // Perhaps this is a bug ? 
            return items.Select(i =>
            {
                var newSellIn = i.SellIn-1;
                var qualityAddend = i.SellIn < 11
                    ? (i.SellIn < 6 ? 3 : 2)
                    : 1;
                return new Item
                {
                    Name = i.Name,
                    SellIn = newSellIn,
                    Quality = newSellIn < 0 ? 0 : i.Quality+qualityAddend // Set to 0 if SellIn is exceeded
                };
            }).ToList();
        }

        private List<Item> UpdateQualityForAgedGoods(List<Item> items)
        {
            return items.Select(i =>
                    new Item
                    { 
                        Name =  i.Name,
                        SellIn = i.SellIn,
                        Quality = i.Quality < 50 ? i.Quality + 1 : i.Quality
                    }).ToList();
        }

        private List<Item> UpdateSellInForAgedGoods(List<Item> items)
        {
            return items.Select(i =>
                new Item
                {
                    Name = i.Name,
                    Quality = i.Quality,
                    SellIn = --i.SellIn
                }).ToList();
        }

        private List<StockedItemWithBusinessRules> BuildItemsWithRules(List<Item> stockedItems)
        {
            var l = stockedItems.Select(s =>
            {
                BusinessRule rule;
                switch (s.Name)
                {
                    case "Aged Brie":
                        rule = BusinessRule.AgedGoods;
                        break;
                    case "Backstage passes to a TAFKAL80ETC concert":
                        rule = BusinessRule.BackstagePassesAndSimilar;
                        break;
                    case "Sulfuras, Hand of Ragnaros":
                        rule = BusinessRule.LegendaryStuff;
                        break;
                    case "Conjured Mana Cake":
                        rule = BusinessRule.ConjuredItems;
                        break;
                    default:
                        rule = BusinessRule.NoSpecialHandling;
                        break;
                }
                return new StockedItemWithBusinessRules(s, rule);
            }).ToList();
            return l;
        }
         
        private List<Item> UpdateSellInForItems(List<Item> items)
        {
            return items.Select(i => new Item
            {
                Name = i.Name,
                Quality = i.Quality,
                SellIn = --i.SellIn
            }).ToList();
        }

        private List<Item> UpdateQualityForItems(List<Item> items)
        {
            return items.Select(i =>
            {
                var qualityDegradationFactor = i.SellIn < 0 ? 2 : 1;
                return new Item
                {
                    Name = i.Name,
                    Quality = i.Quality > 0 ? i.Quality - qualityDegradationFactor : i.Quality,
                    SellIn = i.SellIn
                };
            }).ToList();
        }


        public Stock(List<Item> stockedItems)
        {
            Items = stockedItems;
        }

        class StockedItemWithBusinessRules
        {
           public Item Item { get; private set; }
            public BusinessRule Rule { get; private set; }


            public StockedItemWithBusinessRules(Item item, BusinessRule rule)
            {
                Item = item;
                Rule = rule;
            }
        }

        enum BusinessRule
        {
            NoSpecialHandling,
            AgedGoods,
            BackstagePassesAndSimilar,
            LegendaryStuff,
            ConjuredItems
        }

    }

    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }

}
