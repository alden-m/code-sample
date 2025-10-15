using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Inventory;

public class Asset : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AssetCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AssetCategory Category { get; set; }
    public decimal Value { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public List<AssetAttribute> Attributes { get; set; } = new();
    public Guid? ParentAssetId { get; set; }
}

public class AssetAttribute
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
}

public enum AssetCategory
{
    Hardware,
    Software,
    Infrastructure,
    Documentation,
    License
}

public enum AssetStatus
{
    Active,
    Maintenance,
    Retired,
    Lost,
    Disposed
}