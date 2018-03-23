using System;

namespace Geisha.Engine.Core.Assets
{
    public class AssetInfo : IEquatable<AssetInfo>
    {
        public AssetInfo(Type assetType, Guid assetId, string assetFilePath)
        {
            AssetType = assetType;
            AssetId = assetId;
            AssetFilePath = assetFilePath;
        }

        public Type AssetType { get; }
        public Guid AssetId { get; }
        public string AssetFilePath { get; }

        public override string ToString()
        {
            return $"{nameof(AssetType)}: {AssetType}, {nameof(AssetId)}: {AssetId}, {nameof(AssetFilePath)}: {AssetFilePath}";
        }

        public bool Equals(AssetInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AssetType == other.AssetType && AssetId.Equals(other.AssetId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AssetType != null ? AssetType.GetHashCode() : 0) * 397) ^ AssetId.GetHashCode();
            }
        }

        public static bool operator ==(AssetInfo left, AssetInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssetInfo left, AssetInfo right)
        {
            return !Equals(left, right);
        }
    }
}