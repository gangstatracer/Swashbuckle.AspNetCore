using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class Spike
    {
        [Fact]
        public void Test()
        {
            var value = new Dictionary<string, int>()
            {
                ["foo"] = 1
            };

            var jsonString = JsonSerializer.Serialize(value);
            throw new Exception(jsonString);
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StringEnum
    {
        Foo = 1,
        Bar = 2
    }
}
