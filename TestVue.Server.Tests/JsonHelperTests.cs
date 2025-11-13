using System.Text.Json;
using TestVue.Server.Helper;
using Xunit;

namespace TestVue.Server.Tests
{
    public class JsonHelperTests
    {
        private static JsonElement Parse(string json)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        [Fact]
        public void ConvertJsonElementToDictionary_Throws_For_NonObject()
        {
            var element = Parse("[1,2,3]");
            Assert.Throws<ArgumentException>(() => JsonHelper.ConvertJsonElementToDictionary(element));
        }

        private class TestModel
        {
            public string? Str { get; set; }
            public int IntVal { get; set; }
            public long LongVal { get; set; }
            public bool BoolTrue { get; set; }
            public bool BoolFalse { get; set; }
            public string? NullVal { get; set; }
            public double DoubleVal { get; set; }
            public TestModel[]? Children { get; set; }
        }

        [Fact]
        public void Converts_Types_Correctly()
        {
            // Build JSON from a strongly-typed model using configured serializer options
            var model = new TestModel
            {
                Str = "hello",
                IntVal = 42,
                LongVal = 2147483648L,
                BoolTrue = true,
                BoolFalse = false,
                NullVal = null,
                DoubleVal = 3.14,
                Children = new[]
                {
                    new TestModel { Str = "child1", IntVal = 1, DoubleVal = 1.1 },
                    new TestModel { Str = "child2", IntVal = 2, DoubleVal = 2.2 }
                }
            };

            var json = JsonSerializer.Serialize(model, Configuration.JsonConfiguration.DefaultOptions);
            var dict = JsonHelper.ConvertJsonElementToDictionary(Parse(json));

            Assert.Equal(model.Str, dict[nameof(model.Str)]);

            Assert.IsType<int>(dict[nameof(model.IntVal)]);
            Assert.Equal(model.IntVal, dict[nameof(model.IntVal)]);

            Assert.IsType<long>(dict[nameof(model.LongVal)]);
            Assert.Equal(model.LongVal, dict[nameof(model.LongVal)]);

            Assert.IsType<bool>(dict[nameof(model.BoolTrue)]);
            Assert.Equal(model.BoolTrue, dict[nameof(model.BoolTrue)]);

            Assert.IsType<bool>(dict[nameof(model.BoolFalse)]);
            Assert.Equal(model.BoolFalse, dict[nameof(model.BoolFalse)]);

            // Null property should be omitted due to DefaultIgnoreCondition.WhenWritingNull
            Assert.False(dict.ContainsKey(nameof(model.NullVal)));

            // Double may be parsed as decimal or double depending on JSON number handling
            Assert.True(dict[nameof(model.DoubleVal)] is double or decimal);
            var dbl = Convert.ToDouble(dict[nameof(model.DoubleVal)]);
            Assert.Equal(model.DoubleVal, dbl, precision: 3);

            // Children should be converted to a list of dictionaries
            var children = Assert.IsType<List<object?>>(dict[nameof(model.Children)]);
            Assert.Equal(model.Children.Length, children.Count);
            var child0 = Assert.IsType<Dictionary<string, object?>>(children[0]);
            var child1 = Assert.IsType<Dictionary<string, object?>>(children[1]);
            Assert.Equal(model.Children[0].Str, child0[nameof(model.Str)]);
            Assert.Equal(model.Children[0].IntVal, child0[nameof(model.IntVal)]);
            Assert.True(child0[nameof(model.DoubleVal)] is double or decimal);

            Assert.Equal(model.Children[1].Str, child1[nameof(model.Str)]);
            Assert.Equal(model.Children[1].IntVal, child1[nameof(model.IntVal)]);
            Assert.True(child1[nameof(model.DoubleVal)] is double or decimal);
        }

        private class PriceModel { public decimal Price { get; set; } }

        [Fact]
        public void Converts_Decimal_Number()
        {
            var model = new PriceModel { Price = 123.456m };
            var json = JsonSerializer.Serialize(model, Configuration.JsonConfiguration.DefaultOptions);
            var dict = JsonHelper.ConvertJsonElementToDictionary(Parse(json));
            Assert.IsType<decimal>(dict[nameof(model.Price)]);
            Assert.Equal(model.Price, (decimal)dict[nameof(model.Price)]);
        }

        private class InnerModel { public int Value { get; set; } }
        private class OuterModel { public InnerModel Inner { get; set; } = new InnerModel(); }
        private class WrapperModel { public OuterModel Outer { get; set; } = new OuterModel(); }

        [Fact]
        public void Converts_Nested_Object()
        {
            var model = new WrapperModel { Outer = new OuterModel { Inner = new InnerModel { Value = 10 } } };
            var json = JsonSerializer.Serialize(model, Configuration.JsonConfiguration.DefaultOptions);
            var dict = JsonHelper.ConvertJsonElementToDictionary(Parse(json));
            var outerDict = Assert.IsType<Dictionary<string, object?>>(dict[nameof(model.Outer)]);
            var innerDict = Assert.IsType<Dictionary<string, object?>>(outerDict[nameof(model.Outer.Inner)]);
            Assert.Equal(model.Outer.Inner.Value, innerDict[nameof(model.Outer.Inner.Value)]);
        }
    }
}
