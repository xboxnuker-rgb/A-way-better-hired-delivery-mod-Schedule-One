using System.Collections.Generic;

namespace VehicleHandlers.Contracts
{
    internal interface IHandlerAssignmentRegistry
    {
        bool TryGet(HandlerId handler, out HandlerAssignmentSnapshot assignment);

        IReadOnlyList<HandlerAssignmentSnapshot> Snapshot();

        AssignmentWriteResult TryConfigure(
            HandlerAssignmentConfiguration configuration,
            long expectedRevision);

        AssignmentWriteResult TryRestore(HandlerAssignmentSnapshot assignment);

        TripTransitionResult TryTransition(
            HandlerId handler,
            TripId trip,
            HandlerTripState targetState,
            long expectedRevision);

        AssignmentWriteResult TryRemove(HandlerId handler, long expectedRevision);
    }

    internal interface IHandlerAssignmentValidator
    {
        AssignmentValidationResult Validate(
            HandlerAssignmentSnapshot assignment,
            AssignmentValidationOperation operation);
    }

    internal interface IHandlerReservationRegistry
    {
        ReservationAcquireResult TryAcquire(LoadingBayKey bay, ReservationOwner owner);

        bool IsReserved(LoadingBayKey bay);

        bool IsOwnedBy(LoadingBayKey bay, ReservationOwner owner);

        ReservationReleaseResult Release(ReservationLease lease);

        int ReleaseForHandler(HandlerId handler);

        IReadOnlyList<ReservationLease> Snapshot();
    }

    internal interface IHandlerMovementCoordinator
    {
        MovementRequestResult TryStart(HandlerId handler);

        MovementRequestResult TryPlace(HandlerId handler);

        MovementRequestResult TryHideAndRelease(HandlerId handler);

        MovementRequestResult TryRecover(HandlerId handler, HandlerUnavailableReason reason);

        void Tick();
    }

    internal interface IHandlerLifecycleSink
    {
        void HandlerUnavailable(HandlerId handler, HandlerUnavailableReason reason);

        void ModUnloading();
    }

    internal enum HandlerUnavailableReason
    {
        Fired = 0,
        Deleted = 1,
        Despawned = 2,
        MissingAfterLoad = 3,
        InvalidAssignment = 4,
        ModUnloading = 5
    }

    internal enum MovementRequestStatus
    {
        Accepted = 0,
        AlreadyInRequestedState = 1,
        AssignmentNotFound = 2,
        RejectedByValidation = 3,
        IllegalTransition = 4,
        ReservationUnavailable = 5,
        NotServer = 6
    }

    internal readonly struct MovementRequestResult
    {
        public MovementRequestResult(
            MovementRequestStatus status,
            AssignmentValidationResult validation,
            string detail = null)
        {
            Status = status;
            Validation = validation;
            Detail = detail ?? string.Empty;
        }

        public MovementRequestStatus Status { get; }

        public AssignmentValidationResult Validation { get; }

        public string Detail { get; }

        public bool Accepted =>
            Status == MovementRequestStatus.Accepted ||
            Status == MovementRequestStatus.AlreadyInRequestedState;
    }
}
