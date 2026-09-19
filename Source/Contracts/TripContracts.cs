using System;

namespace VehicleHandlers.Contracts
{
    internal enum HandlerTripState
    {
        Idle = 0,
        Reserved = 1,
        InTransit = 2,
        WaitingForBay = 3,
        Placing = 4,
        Placed = 5,
        Releasing = 6,
        Recovering = 7,
        Faulted = 8
    }

    internal static class HandlerTripTransitionPolicy
    {
        public static bool CanTransition(HandlerTripState from, HandlerTripState to)
        {
            switch (from)
            {
                case HandlerTripState.Idle:
                    return to == HandlerTripState.Reserved;
                case HandlerTripState.Reserved:
                    return to == HandlerTripState.InTransit ||
                           to == HandlerTripState.Idle ||
                           to == HandlerTripState.Recovering;
                case HandlerTripState.InTransit:
                    return to == HandlerTripState.WaitingForBay ||
                           to == HandlerTripState.Placing ||
                           to == HandlerTripState.Recovering;
                case HandlerTripState.WaitingForBay:
                    return to == HandlerTripState.Placing ||
                           to == HandlerTripState.Recovering;
                case HandlerTripState.Placing:
                    return to == HandlerTripState.Placed ||
                           to == HandlerTripState.WaitingForBay ||
                           to == HandlerTripState.Recovering;
                case HandlerTripState.Placed:
                    return to == HandlerTripState.Releasing ||
                           to == HandlerTripState.Recovering;
                case HandlerTripState.Releasing:
                    return to == HandlerTripState.Idle ||
                           to == HandlerTripState.Recovering;
                case HandlerTripState.Recovering:
                    return to == HandlerTripState.Idle ||
                           to == HandlerTripState.Faulted;
                case HandlerTripState.Faulted:
                    return to == HandlerTripState.Recovering ||
                           to == HandlerTripState.Idle;
                default:
                    return false;
            }
        }

        public static bool RequiresReservation(HandlerTripState state)
        {
            return state >= HandlerTripState.Reserved && state <= HandlerTripState.Recovering;
        }

        public static bool TripMayOwnHiddenVehicle(HandlerTripState state)
        {
            return state == HandlerTripState.InTransit ||
                   state == HandlerTripState.WaitingForBay ||
                   state == HandlerTripState.Placing ||
                   state == HandlerTripState.Recovering;
        }
    }

    internal enum TripTransitionStatus
    {
        Applied = 0,
        NoChange = 1,
        IllegalTransition = 2,
        StaleRevision = 3,
        ReservationNotOwned = 4,
        RejectedByValidation = 5
    }

    internal readonly struct TripTransitionResult
    {
        public TripTransitionResult(
            TripTransitionStatus status,
            HandlerTripState previousState,
            HandlerTripState currentState,
            long revision,
            string detail = null)
        {
            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(revision));
            }

            Status = status;
            PreviousState = previousState;
            CurrentState = currentState;
            Revision = revision;
            Detail = detail ?? string.Empty;
        }

        public TripTransitionStatus Status { get; }

        public HandlerTripState PreviousState { get; }

        public HandlerTripState CurrentState { get; }

        public long Revision { get; }

        public string Detail { get; }

        public bool Applied => Status == TripTransitionStatus.Applied;
    }
}
