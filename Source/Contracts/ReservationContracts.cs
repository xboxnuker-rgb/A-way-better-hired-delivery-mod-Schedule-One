using System;

namespace VehicleHandlers.Contracts
{
    internal readonly struct ReservationOwner : IEquatable<ReservationOwner>
    {
        public ReservationOwner(HandlerId handler, TripId trip)
        {
            if (handler.IsEmpty)
            {
                throw new ArgumentException("A Handler is required.", nameof(handler));
            }

            if (trip.IsEmpty)
            {
                throw new ArgumentException("An active trip is required.", nameof(trip));
            }

            Handler = handler;
            Trip = trip;
        }

        public HandlerId Handler { get; }

        public TripId Trip { get; }

        public bool IsEmpty => Handler.IsEmpty || Trip.IsEmpty;

        public bool Equals(ReservationOwner other) => Handler.Equals(other.Handler) && Trip.Equals(other.Trip);

        public override bool Equals(object obj) => obj is ReservationOwner other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Handler, Trip);

        public override string ToString() => $"{Handler}/{Trip}";

        public static bool operator ==(ReservationOwner left, ReservationOwner right) => left.Equals(right);

        public static bool operator !=(ReservationOwner left, ReservationOwner right) => !left.Equals(right);
    }

    internal readonly struct ReservationLease : IEquatable<ReservationLease>
    {
        public ReservationLease(LoadingBayKey bay, ReservationOwner owner, long token)
        {
            if (bay.IsEmpty)
            {
                throw new ArgumentException("A loading bay is required.", nameof(bay));
            }

            if (owner.IsEmpty)
            {
                throw new ArgumentException("A reservation owner is required.", nameof(owner));
            }

            if (token <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(token), "A reservation token must be positive.");
            }

            Bay = bay;
            Owner = owner;
            Token = token;
        }

        public LoadingBayKey Bay { get; }

        public ReservationOwner Owner { get; }

        public long Token { get; }

        public bool IsEmpty => Bay.IsEmpty || Owner.IsEmpty || Token <= 0;

        public bool Equals(ReservationLease other) =>
            Bay.Equals(other.Bay) && Owner.Equals(other.Owner) && Token == other.Token;

        public override bool Equals(object obj) => obj is ReservationLease other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Bay, Owner, Token);
    }

    internal enum ReservationAcquireStatus
    {
        Acquired = 0,
        AlreadyOwned = 1,
        OwnedByAnother = 2,
        BayUnavailable = 3,
        InvalidRequest = 4
    }

    internal readonly struct ReservationAcquireResult
    {
        public ReservationAcquireResult(
            ReservationAcquireStatus status,
            ReservationLease lease,
            string detail = null)
        {
            if ((status == ReservationAcquireStatus.Acquired || status == ReservationAcquireStatus.AlreadyOwned) &&
                lease.IsEmpty)
            {
                throw new ArgumentException("A successful acquisition requires a lease.", nameof(lease));
            }

            Status = status;
            Lease = lease;
            Detail = detail ?? string.Empty;
        }

        public ReservationAcquireStatus Status { get; }

        public ReservationLease Lease { get; }

        public string Detail { get; }

        public bool Acquired =>
            Status == ReservationAcquireStatus.Acquired || Status == ReservationAcquireStatus.AlreadyOwned;
    }

    internal enum ReservationReleaseStatus
    {
        Released = 0,
        NotFound = 1,
        NotOwner = 2,
        StaleLease = 3,
        InvalidRequest = 4
    }

    internal readonly struct ReservationReleaseResult
    {
        public ReservationReleaseResult(ReservationReleaseStatus status, string detail = null)
        {
            Status = status;
            Detail = detail ?? string.Empty;
        }

        public ReservationReleaseStatus Status { get; }

        public string Detail { get; }

        public bool Released => Status == ReservationReleaseStatus.Released;
    }
}
