using System.Collections.Generic;
using System.Linq;

namespace GildedRose.Console
{
    public class Stock
    {
        public List<Item> Items { get; private set; }

        public void UpdateQuality()
        {
            var stockedItems = BuildItemsWithRules(Items);

            var defaultHandling = UpdateQualityForItems(
                UpdateSellInForItems(stockedItems
                    .Where(s => s.Rule == BusinessRule.NoSpecialHandling).Select(s => s.Item).ToList()));

            var legendaryItems = stockedItems.Where(s => s.Rule == BusinessRule.LegendaryStuff).Select(s => s.Item);

            var agedGoods = UpdateQualityForAgedGoods(
                                UpdateSellInForAgedGoods(stockedItems.Where(s => s.Rule == BusinessRule.AgedGoods).Select(s => s.Item).ToList()));

            var backstagePasses = UpdateQualityAndSellInForBackstagePasses(
                                        stockedItems.Where(s => s.Rule == BusinessRule.BackstagePassesAndSimilar).Select(s => s.Item).ToList());

            var conjuredItems =
                UpdateQualityForConjuredItems(
                    UpdateSellInForItems(stockedItems.Where(s => s.Rule == BusinessRule.ConjuredItems).Select(s => s.Item).ToList()));

            Items = defaultHandling.Concat(legendaryItems).Concat(agedGoods).Concat(backstagePasses).Concat(conjuredItems).ToList();

            //ReferenceImplementation();
        }

        private void ReferenceImplementation()
        {
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
                        Items[i].Quality = Items[i].Quality + 1;

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
                    else
                    {
                        if (Items[i].Quality < 50)
                        {
                            Items[i].Quality = Items[i].Quality + 1;
                        }
                    }
                }
            }
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
            // The code sample increased Quality of Aged Brie by 2, but the business rules says: "Aged Brie" actually increases in Quality the older it gets
            // In this code, I've chosen to use the functionality used by code sample
            return items.Select(i =>
                new Item
                { 
                    Name =  i.Name,
                    SellIn = i.SellIn,
                    Quality = i.SellIn < 0 ? Limit(i.Quality+2, 50) : Limit(i.Quality+1,50)
                }).ToList();
        }

        private int Limit(int i, int limit)
        {
            return i > limit ? limit : i;
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
}