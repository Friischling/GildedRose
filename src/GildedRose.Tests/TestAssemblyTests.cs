using System.Collections.Generic;
using System.Linq;
using GildedRose.Console;
using Xunit;

namespace GildedRose.Tests
{
    public class TestAssemblyTests
    {
        private Stock sut;
        public TestAssemblyTests()
        {
            sut = new Stock(new List<Item>
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
        }

        [Fact]
        public void TestBasicAssumptions()
        {
            // Initial state
            var vest = sut.Items.Single(n => n.Name.Equals("+5 Dexterity Vest")); // Fails if no such item is present or multiple instances
            Assert.Equal(10, vest.SellIn); // Copying values instead of referencing back to original list gives better readability
            Assert.Equal(20, vest.Quality);

            var brie = sut.Items.Single(n => n.Name.Equals("Aged Brie")); 
            Assert.Equal(2, brie.SellIn);
            Assert.Equal(0, brie.Quality);

            var elixir = sut.Items.Single(n => n.Name.Equals("Elixir of the Mongoose"));
            Assert.Equal(5, elixir.SellIn);
            Assert.Equal(7, elixir.Quality);

            var sulfuras = sut.Items.Single(n => n.Name.Equals("Sulfuras, Hand of Ragnaros"));
            Assert.Equal(0, sulfuras.SellIn);
            Assert.Equal(80, sulfuras.Quality);

            var backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(15, backstagepasses.SellIn);
            Assert.Equal(20, backstagepasses.Quality);

            var conjuredmanacake = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(3, conjuredmanacake.SellIn);
            Assert.Equal(6, conjuredmanacake.Quality);

            // Act
            sut.UpdateQuality(); // At the end of each day our system lowers both values for every item

            vest = sut.Items.Single(n => n.Name.Equals("+5 Dexterity Vest")); // Fails if no such item is present or multiple instances
            Assert.Equal(9, vest.SellIn); // Basic Item with no special handling
            Assert.Equal(19, vest.Quality);

            brie = sut.Items.Single(n => n.Name.Equals("Aged Brie"));
            Assert.Equal(1, brie.SellIn);
            Assert.Equal(1, brie.Quality);

            elixir = sut.Items.Single(n => n.Name.Equals("Elixir of the Mongoose"));
            Assert.Equal(4, elixir.SellIn);
            Assert.Equal(6, elixir.Quality);

            sulfuras = sut.Items.Single(n => n.Name.Equals("Sulfuras, Hand of Ragnaros"));
            Assert.Equal(0, sulfuras.SellIn);
            Assert.Equal(80, sulfuras.Quality);

            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(14, backstagepasses.SellIn);
            Assert.Equal(21, backstagepasses.Quality);

            conjuredmanacake = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(2, conjuredmanacake.SellIn); // Original code implemented here - no special handling
            Assert.Equal(4, conjuredmanacake.Quality);


        }


        [Fact]
        public void TestThatQualityDegradesTwiceAsFastOnceSellInIsPassed()
        {
            // Initial state
            var vest = sut.Items.Single(n => n.Name.Equals("+5 Dexterity Vest")); // Fails if no such item is present or multiple instances
            Assert.Equal(10, vest.SellIn); // Copying values instead of referencing back to original list gives better readability
            Assert.Equal(20, vest.Quality);
          
            // Act
            Enumerable.Range(0,10).ForEach(i=>sut.UpdateQuality()); // At the end of each day our system lowers both values for every item
            vest = sut.Items.Single(n => n.Name.Equals("+5 Dexterity Vest")); // Fails if no such item is present or multiple instances
            Assert.Equal(0, vest.SellIn); // Basic Item with no special handling
            Assert.Equal(10, vest.Quality);

            sut.UpdateQuality();
            vest = sut.Items.Single(n => n.Name.Equals("+5 Dexterity Vest")); // Fails if no such item is present or multiple instances
            Assert.Equal(-1, vest.SellIn); 
            Assert.Equal(8, vest.Quality); // Twice as fast, 2 per day

            sut.UpdateQuality();
            vest = sut.Items.Single(n => n.Name.Equals("+5 Dexterity Vest")); // Fails if no such item is present or multiple instances
            Assert.Equal(-2, vest.SellIn);
            Assert.Equal(6, vest.Quality); // Twice as fast, 2 per day

        }

        [Fact]
        public void TestThatTheQualityOfAnItemIsNeverNegative()
        {
            // Get initial state
            var elixir = sut.Items.Single(n => n.Name.Equals("Elixir of the Mongoose"));
            Assert.Equal(5, elixir.SellIn);
            Assert.Equal(7, elixir.Quality);

            //Act -  Excercise system 
            Enumerable.Range(0,50).ForEach(i=>sut.UpdateQuality());
            elixir = sut.Items.Single(n => n.Name.Equals("Elixir of the Mongoose"));
            Assert.Equal(0,elixir.Quality);


        }
        [Fact]
        public void TestThatTheQualityOfAnItemIsNeverAbove50()
        {
            // Get initial state
            var brie = sut.Items.Single(n => n.Name.Equals("Aged Brie"));
            Assert.Equal(2, brie.SellIn);
            Assert.Equal(0, brie.Quality);

            //Act -  Excercise system 
            Enumerable.Range(0, 100).ForEach(i => sut.UpdateQuality());
            brie = sut.Items.Single(n => n.Name.Equals("Aged Brie"));
            Assert.Equal(50, brie.Quality);
        }


        [Fact]
        public void TestThatAgedBrieActuallyIncreasesInQualityTheOlderItGets()
        {
            // Get initial state
            var brie = sut.Items.Single(n => n.Name.Equals("Aged Brie"));
            Assert.Equal(2, brie.SellIn);
            Assert.Equal(0, brie.Quality);

            //Act -  Excercise system 
            Enumerable.Range(0, 10).ForEach(i => sut.UpdateQuality());  // Is there a bug in the code ? 
            brie = sut.Items.Single(n => n.Name.Equals("Aged Brie"));
            Assert.Equal(10, brie.Quality);

            Enumerable.Range(0, 10).ForEach(i => sut.UpdateQuality());
            brie = sut.Items.Single(n => n.Name.Equals("Aged Brie"));
            Assert.Equal(20, brie.Quality);
        }

        [Fact]
        public void TestThatSulfurasBeingALegendaryItemNeverHasToBeSoldOrDecreasesInQuality()
        {
            var sulfuras = sut.Items.Single(n => n.Name.Equals("Sulfuras, Hand of Ragnaros"));
            Assert.Equal(0, sulfuras.SellIn);
            Assert.Equal(80, sulfuras.Quality);

            Enumerable.Range(0,34).ForEach(i=>sut.UpdateQuality());
            sulfuras = sut.Items.Single(n => n.Name.Equals("Sulfuras, Hand of Ragnaros"));
            Assert.Equal(0, sulfuras.SellIn);
            Assert.Equal(80, sulfuras.Quality);


        }
        [Fact]
        public void TestThatBackstagePassesIncreaseInQualityTheCloseWeGetToSellInButDropsTo0AfterSellIn()
        {
            var backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(15, backstagepasses.SellIn);
            Assert.Equal(20, backstagepasses.Quality);

            // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; Quality increases by 2 when there are 10 days or less and by 3 when there are 5 days or less but Quality drops to 0 after the concert
            Enumerable.Range(0,5).ForEach(i=> sut.UpdateQuality());
            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(10, backstagepasses.SellIn);
            Assert.Equal(25, backstagepasses.Quality);

            sut.UpdateQuality();
            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(9, backstagepasses.SellIn);
            Assert.Equal(27, backstagepasses.Quality);

            Enumerable.Range(0,4).ForEach(i=>sut.UpdateQuality());
            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(5, backstagepasses.SellIn);
            Assert.Equal(35, backstagepasses.Quality);

            sut.UpdateQuality();
            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(4, backstagepasses.SellIn);
            Assert.Equal(38, backstagepasses.Quality);

            Enumerable.Range(0, 4).ForEach(i => sut.UpdateQuality());
            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(0, backstagepasses.SellIn);
            Assert.Equal(50, backstagepasses.Quality);

            sut.UpdateQuality();
            backstagepasses = sut.Items.Single(n => n.Name.Equals("Backstage passes to a TAFKAL80ETC concert"));
            Assert.Equal(-1, backstagepasses.SellIn);
            Assert.Equal(0, backstagepasses.Quality);


        }


        [Fact]
        public void TestThatConjuredItemsAreDegradingTwiceAsFast() // "Conjured" items degrade in Quality twice as fast as normal items
        {
            var conjured = sut.Items.Single(n=>n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(3, conjured.SellIn);
            Assert.Equal(6,conjured.Quality);

            sut.UpdateQuality();
            conjured = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(2, conjured.SellIn);
            Assert.Equal(4, conjured.Quality);

            sut.UpdateQuality();
            conjured = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(1, conjured.SellIn);
            Assert.Equal(2, conjured.Quality);

            sut.UpdateQuality();
            conjured = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(0, conjured.SellIn);
            Assert.Equal(0, conjured.Quality);

            sut.UpdateQuality();
            conjured = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(-1, conjured.SellIn);
            Assert.Equal(0, conjured.Quality);

            sut.UpdateQuality();
            conjured = sut.Items.Single(n => n.Name.Equals("Conjured Mana Cake"));
            Assert.Equal(-2, conjured.SellIn);
            Assert.Equal(0, conjured.Quality);

        }


        
    }
}