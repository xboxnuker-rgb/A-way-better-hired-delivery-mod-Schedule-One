using System;

namespace VehicleHandlers.Contracts
{
    internal readonly struct HandlerId : IEquatable<HandlerId>
    {
        public HandlerId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("A Handler ID cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public Guid Value { get; }

        public bool IsEmpty => Value == Guid.Empty;

        public static bool TryParse(string value, out HandlerId id)
        {
            if (Guid.TryParse(value, out Guid parsed) && parsed != Guid.Empty)
            {
                id = new HandlerId(parsed);
                return true;
            }

            id = default;
            return false;
        }

        public bool Equals(HandlerId other) => Value.Equals(other.Value);

        public override bool Equals(object obj) => obj is HandlerId other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("D");

        public static bool operator ==(HandlerId left, HandlerId right) => left.Equals(right);

        public static bool operator !=(HandlerId left, HandlerId right) => !left.Equals(right);
    }

    internal readonly struct VehicleId : IEquatable<VehicleId>
    {
        public VehicleId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("A vehicle ID cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public Guid Value { get; }

        public bool IsEmpty => Value == Guid.Empty;

        public static bool TryParse(string value, out VehicleId id)
        {
            if (Guid.TryParse(value, out Guid parsed) && parsed != Guid.Empty)
            {
                id = new VehicleId(parsed);
                return true;
            }

            id = default;
            return false;
        }

        public bool Equals(VehicleId other) => Value.Equals(other.Value);

        public override bool Equals(object obj) => obj is VehicleId other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("D");

        public static bool operator ==(VehicleId left, VehicleId right) => left.Equals(right);

        public static bool operator !=(VehicleId left, VehicleId right) => !left.Equals(right);
    }

    internal readonly struct DockId : IEquatable<DockId>
    {
        public DockId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("A loading-dock ID cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public Guid Value { get; }

        public bool IsEmpty => Value == Guid.Empty;

        public static bool TryParse(string value, out DockId id)
        {
            if (Guid.TryParse(value, out Guid parsed) && parsed != Guid.Empty)
            {
                id = new DockId(parsed);
                return true;
            }

            id = default;
            return false;
        }

        public bool Equals(DockId other) => Value.Equals(other.Value);

        public override bool Equals(object obj) => obj is DockId other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("D");

        public static bool operator ==(DockId left, DockId right) => left.Equals(right);

        public static bool operator !=(DockId left, DockId right) => !left.Equals(right);
    }

    internal readonly struct TripId : IEquatable<TripId>
    {
        public TripId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("A trip ID cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public Guid Value { get; }

        public bool IsEmpty => Value == Guid.Empty;

        public static TripId New() => new TripId(Guid.NewGuid());

        public static bool TryParse(string value, out TripId id)
        {
            if (Guid.TryParse(value, out Guid parsed) && parsed != Guid.Empty)
            {
                id = new TripId(parsed);
                return true;
            }

            id = default;
            return false;
        }

        public bool Equals(TripId other) => Value.Equals(other.Value);

        public override bool Equals(object obj) => obj is TripId other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("D");

        public static bool operator ==(TripId left, TripId right) => left.Equals(right);

        public static bool operator !=(TripId left, TripId right) => !left.Equals(right);
    }

    internal readonly struct PropertyKey : IEquatable<PropertyKey>
    {
        public PropertyKey(string propertyCode)
        {
            if (string.IsNullOrWhiteSpace(propertyCode))
            {
                throw new ArgumentException("A property code is required.", nameof(propertyCode));
            }

            PropertyCode = propertyCode.Trim();
        }

        public string PropertyCode { get; }

        public bool IsEmpty => string.IsNullOrEmpty(PropertyCode);

        public bool Equals(PropertyKey other) =>
            StringComparer.OrdinalIgnoreCase.Equals(PropertyCode, other.PropertyCode);

        public override bool Equals(object obj) => obj is PropertyKey other && Equals(other);

        public override int GetHashCode() =>
            PropertyCode == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(PropertyCode);

        public override string ToString() => PropertyCode ?? string.Empty;

        public static bool operator ==(PropertyKey left, PropertyKey right) => left.Equals(right);

        public static bool operator !=(PropertyKey left, PropertyKey right) => !left.Equals(right);
    }

    internal readonly struct LoadingBayKey : IEquatable<LoadingBayKey>
    {
        public LoadingBayKey(PropertyKey property, DockId dock, int dockIndex)
        {
            if (property.IsEmpty)
            {
                throw new ArgumentException("A property is required.", nameof(property));
            }

            if (dock.IsEmpty)
            {
                throw new ArgumentException("A loading dock is required.", nameof(dock));
            }

            if (dockIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dockIndex), "A loading-dock index cannot be negative.");
            }

            Property = property;
            Dock = dock;
            DockIndex = dockIndex;
        }

        public PropertyKey Property { get; }

        public DockId Dock { get; }

        public int DockIndex { get; }

        public bool IsEmpty => Property.IsEmpty || Dock.IsEmpty || DockIndex < 0;

        public bool Equals(LoadingBayKey other) =>
            Property.Equals(other.Property) && Dock.Equals(other.Dock) && DockIndex == other.DockIndex;

        public override bool Equals(object obj) => obj is LoadingBayKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Property, Dock, DockIndex);

        public override string ToString() => $"{Property}/{DockIndex}/{Dock}";

        public static bool operator ==(LoadingBayKey left, LoadingBayKey right) => left.Equals(right);

        public static bool operator !=(LoadingBayKey left, LoadingBayKey right) => !left.Equals(right);
    }
}
