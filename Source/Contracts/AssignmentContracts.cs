using System;

namespace VehicleHandlers.Contracts
{
    internal sealed class HandlerAssignmentConfiguration
    {
        public HandlerAssignmentConfiguration(
            HandlerId handler,
            VehicleId vehicle,
            LoadingBayKey destination,
            bool enabled,
            bool manuallyHidden)
        {
            if (handler.IsEmpty)
            {
                throw new ArgumentException("A Handler is required.", nameof(handler));
            }

            if (vehicle.IsEmpty)
            {
                throw new ArgumentException("A vehicle is required.", nameof(vehicle));
            }

            if (destination.IsEmpty)
            {
                throw new ArgumentException("A destination is required.", nameof(destination));
            }

            Handler = handler;
            Vehicle = vehicle;
            Destination = destination;
            Enabled = enabled;
            ManuallyHidden = manuallyHidden;
        }

        public HandlerId Handler { get; }

        public VehicleId Vehicle { get; }

        public LoadingBayKey Destination { get; }

        public bool Enabled { get; }

        public bool ManuallyHidden { get; }
    }

    internal sealed class HandlerAssignmentSnapshot
    {
        public HandlerAssignmentSnapshot(
            HandlerId handler,
            VehicleId vehicle,
            LoadingBayKey destination,
            bool enabled,
            bool manuallyHidden,
            TripId activeTrip,
            HandlerTripState tripState,
            long revision)
        {
            if (handler.IsEmpty)
            {
                throw new ArgumentException("A Handler is required.", nameof(handler));
            }

            if (vehicle.IsEmpty)
            {
                throw new ArgumentException("A vehicle is required.", nameof(vehicle));
            }

            if (destination.IsEmpty)
            {
                throw new ArgumentException("A destination is required.", nameof(destination));
            }

            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(revision));
            }

            bool hasTrip = !activeTrip.IsEmpty;
            if (tripState == HandlerTripState.Idle && hasTrip)
            {
                throw new ArgumentException("An idle assignment cannot have a trip ID.", nameof(activeTrip));
            }

            if (tripState != HandlerTripState.Idle && !hasTrip)
            {
                throw new ArgumentException("An active trip state requires a trip ID.", nameof(activeTrip));
            }

            Handler = handler;
            Vehicle = vehicle;
            Destination = destination;
            Enabled = enabled;
            ManuallyHidden = manuallyHidden;
            ActiveTrip = activeTrip;
            TripState = tripState;
            Revision = revision;
        }

        public HandlerId Handler { get; }

        public VehicleId Vehicle { get; }

        public LoadingBayKey Destination { get; }

        public bool Enabled { get; }

        public bool ManuallyHidden { get; }

        public TripId ActiveTrip { get; }

        public HandlerTripState TripState { get; }

        public long Revision { get; }

        public bool HasActiveTrip => !ActiveTrip.IsEmpty;

        public bool TryGetReservationOwner(out ReservationOwner owner)
        {
            if (ActiveTrip.IsEmpty)
            {
                owner = default;
                return false;
            }

            owner = new ReservationOwner(Handler, ActiveTrip);
            return true;
        }
    }

    internal enum AssignmentWriteStatus
    {
        Applied = 0,
        NotFound = 1,
        StaleRevision = 2,
        ActiveTrip = 3,
        DuplicateVehicle = 4,
        RejectedByValidation = 5,
        RestoreConflict = 6
    }

    internal readonly struct AssignmentWriteResult
    {
        public AssignmentWriteResult(AssignmentWriteStatus status, long revision, string detail = null)
        {
            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(revision));
            }

            Status = status;
            Revision = revision;
            Detail = detail ?? string.Empty;
        }

        public AssignmentWriteStatus Status { get; }

        public long Revision { get; }

        public string Detail { get; }

        public bool Applied => Status == AssignmentWriteStatus.Applied;
    }
}
