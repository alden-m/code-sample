namespace BizName.Studio.Data.UnitTests;

public static class TestData
{
    private static readonly Faker _faker = new();

    public class TestEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Priority { get; set; }
    }

    public class RichDataEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NullableString { get; set; }
        public int IntValue { get; set; }
        public int? NullableInt { get; set; }
        public long LongValue { get; set; }
        public bool BoolValue { get; set; }
        public decimal DecimalValue { get; set; }
        public double DoubleValue { get; set; }
        public float FloatValue { get; set; }
        public char CharValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public DateTimeOffset DateTimeOffsetValue { get; set; }
        public TimeSpan TimeSpanValue { get; set; }
        public DayOfWeek EnumValue { get; set; }
        public byte[] ByteArrayValue { get; set; } = Array.Empty<byte>();
        public List<string> StringList { get; set; } = new();
        public Dictionary<string, object> DictionaryValue { get; set; } = new();
        public HashSet<int> HashSetValue { get; set; } = new();
        public NestedObject? NestedValue { get; set; }
        
        public class NestedObject
        {
            public string NestedName { get; set; } = string.Empty;
            public int NestedValue { get; set; }
        }
    }

    public static TestEntity NewTestEntity => new()
    {
        Id = _faker.Random.Guid(),
        Name = _faker.Commerce.ProductName(),
        Description = _faker.Lorem.Paragraph(),
        CreatedAt = _faker.Date.Past(),
        Priority = _faker.Random.Int(1, 10)
    };

    public static RichDataEntity NewRichTestEntity => new()
    {
        Id = _faker.Random.Guid(),
        Name = _faker.Commerce.ProductName(),
        NullableString = _faker.Random.Bool() ? _faker.Lorem.Word() : null,
        IntValue = _faker.Random.Int(1, 100),
        NullableInt = _faker.Random.Bool() ? _faker.Random.Int(1, 100) : null,
        LongValue = _faker.Random.Long(1, 1000000),
        BoolValue = _faker.Random.Bool(),
        DecimalValue = _faker.Random.Decimal(0.01m, 999.99m),
        DoubleValue = _faker.Random.Double(0.01, 999.99),
        FloatValue = _faker.Random.Float(0.01f, 99.99f),
        CharValue = _faker.Random.Char('A', 'Z'),
        DateTimeValue = _faker.Date.Recent(),
        DateTimeOffsetValue = DateTimeOffset.Now.AddDays(-_faker.Random.Int(1, 30)),
        TimeSpanValue = TimeSpan.FromHours(_faker.Random.Int(1, 24)),
        EnumValue = _faker.PickRandom<DayOfWeek>(),
        ByteArrayValue = _faker.Random.Bytes(10),
        StringList = _faker.Make(3, () => _faker.Lorem.Word()).ToList(),
        DictionaryValue = new Dictionary<string, object>
        {
            ["key1"] = _faker.Lorem.Word(),
            ["key2"] = _faker.Random.Int(1, 100),
            ["key3"] = _faker.Random.Bool()
        },
        HashSetValue = new HashSet<int>(_faker.Make(3, () => _faker.Random.Int(1, 100))),
        NestedValue = new RichDataEntity.NestedObject
        {
            NestedName = _faker.Commerce.ProductName(),
            NestedValue = _faker.Random.Int(1, 50)
        }
    };
}
