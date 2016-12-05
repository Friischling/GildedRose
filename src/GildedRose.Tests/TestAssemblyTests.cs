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

            Assert.Equal(9, vest.SellIn); // Basic Item with no special handling
            Assert.Equal(19, vest.Quality);

            Assert.Equal(1, brie.SellIn);
            Assert.Equal(1, brie.Quality);

            Assert.Equal(4, elixir.SellIn);
            Assert.Equal(6, elixir.Quality);

            Assert.Equal(0, sulfuras.SellIn);
            Assert.Equal(80, sulfuras.Quality);

            Assert.Equal(14, backstagepasses.SellIn);
            Assert.Equal(21, backstagepasses.Quality);
            
            Assert.Equal(2, conjuredmanacake.SellIn); // Original code implemented here - no special handling
            Assert.Equal(5, conjuredmanacake.Quality);


        }
    }
}