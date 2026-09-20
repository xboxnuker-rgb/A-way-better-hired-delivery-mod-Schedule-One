using System;
using VehicleHandlers.Contracts;

namespace VehicleHandlers.Contracts.Tests
{
    internal static class Program
    {
        private static int assertionCount;

        private static int Main()
        {
            try
            {
                VerifyIdentifiers();
                VerifyAssignmentInvariants();
                VerifyTripStateGraph();
                VerifyReservationOwnership();
                VerifyValidationResults();

                Console.WriteLine($"Vehicle Handlers contract verification passed ({assertionCount} assertions).");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Contract verification failed: {exception.Message}");
                return 1;
            }
        }

        private static void VerifyIdentifiers()
        {
            const string guidText = "67f5f75d-c6db-4aa3-9ceb-2f17874b1202";

            Assert(ContractVersion.Current == 1, "Contract version should remain pinned to 1.");
            Assert(HandlerId.TryParse(guidText, out HandlerId handler), "Handler GUID should parse.");
            Assert(!handler.IsEmpty, "Parsed Handler ID should not be empty.");
            Assert(handler.ToString() == guidText, "Handler ID should use canonical D formatting.");
            Assert(!HandlerId.TryParse(Guid.Empty.ToString(), out _), "Empty Handler GUID should be rejected.");
            Assert(!VehicleId.TryParse("not-a-guid", out _), "Invalid vehicle GUID should be rejected.");

            PropertyKey upper = new PropertyKey("BARN");
            PropertyKey lower = new PropertyKey("barn");
            Assert(upper == lower, "Property codes should compare case-insensitively.");

            DockId dock = new DockId(Guid.Parse("05ae4e62-da7b-4c73-a080-a0866e5e8c73"));
            LoadingBayKey first = new LoadingBayKey(upper, dock, 0);
            LoadingBayKey same = new LoadingBayKey(lower, dock, 0);
            LoadingBayKey otherIndex = new LoadingBayKey(lower, dock, 1);
            Assert(first == same, "Equivalent loading-bay identities should compare equal.");
            Assert(first != otherIndex, "The loading-dock index must participate in bay identity.");
        }

        private static void VerifyAssignmentInvariants()
        {
            HandlerId handler = Handler();
            VehicleId vehicle = Vehicle();
            LoadingBayKey bay = Bay();
            HandlerAssignmentConfiguration configuration = new HandlerAssignmentConfiguration(
                handler,
                vehicle,
                bay,
                true,
                false);

            Assert(configuration.Handler == handler && configuration.Vehicle == vehicle,
                "Configuration should preserve its strongly typed identities.");
            AssertThrows<ArgumentException>(() => new HandlerAssignmentConfiguration(
                default,
                vehicle,
                bay,
                true,
                false), "Configuration without a Handler should be rejected.");

            HandlerAssignmentSnapshot idle = new HandlerAssignmentSnapshot(
                handler,
                vehicle,
                bay,
                true,
                false,
                default,
                HandlerTripState.Idle,
                0);

            Assert(!idle.TryGetReservationOwner(out _), "An idle assignment must not expose a reservation owner.");

            TripId trip = TripId.New();
            HandlerAssignmentSnapshot active = new HandlerAssignmentSnapshot(
                handler,
                vehicle,
                bay,
                true,
                false,
                trip,
                HandlerTripState.Reserved,
                1);

            Assert(active.TryGetReservationOwner(out ReservationOwner owner), "An active trip should expose its owner.");
            Assert(owner == new ReservationOwner(handler, trip), "Reservation owner should bind Handler and trip.");

            AssertThrows<ArgumentException>(() => new HandlerAssignmentSnapshot(
                handler,
                vehicle,
                bay,
                true,
                false,
                trip,
                HandlerTripState.Idle,
                0), "Idle assignment with a trip ID should be rejected.");

            AssertThrows<ArgumentException>(() => new HandlerAssignmentSnapshot(
                handler,
                vehicle,
                bay,
                true,
                false,
                default,
                HandlerTripState.InTransit,
                0), "Active state without a trip ID should be rejected.");
        }

        private static void VerifyTripStateGraph()
        {
            HandlerTripState[] states = (HandlerTripState[])Enum.GetValues(typeof(HandlerTripState));
            bool[,] expected = new bool[states.Length, states.Length];
            Allow(expected, HandlerTripState.Idle, HandlerTripState.Reserved);
            Allow(expected, HandlerTripState.Reserved,
                HandlerTripState.InTransit, HandlerTripState.Idle, HandlerTripState.Recovering);
            Allow(expected, HandlerTripState.InTransit,
                HandlerTripState.WaitingForBay, HandlerTripState.Placing, HandlerTripState.Recovering);
            Allow(expected, HandlerTripState.WaitingForBay,
                HandlerTripState.Placing, HandlerTripState.Recovering);
            Allow(expected, HandlerTripState.Placing,
                HandlerTripState.Placed, HandlerTripState.WaitingForBay, HandlerTripState.Recovering);
            Allow(expected, HandlerTripState.Placed,
                HandlerTripState.Releasing, HandlerTripState.Recovering);
            Allow(expected, HandlerTripState.Releasing,
                HandlerTripState.Idle, HandlerTripState.Recovering);
            Allow(expected, HandlerTripState.Recovering,
                HandlerTripState.Idle, HandlerTripState.Faulted);
            Allow(expected, HandlerTripState.Faulted,
                HandlerTripState.Recovering, HandlerTripState.Idle);

            foreach (HandlerTripState from in states)
            {
                foreach (HandlerTripState to in states)
                {
                    bool actual = HandlerTripTransitionPolicy.CanTransition(from, to);
                    Assert(actual == expected[(int)from, (int)to],
                        $"Transition policy mismatch for {from} -> {to}.");
                }
            }

            Assert(HandlerTripTransitionPolicy.RequiresReservation(HandlerTripState.Reserved),
                "Reserved state should require a lease.");
            Assert(HandlerTripTransitionPolicy.RequiresReservation(HandlerTripState.Recovering),
                "Recovery should retain its lease until cleanup completes.");
            Assert(!HandlerTripTransitionPolicy.RequiresReservation(HandlerTripState.Faulted),
                "Faulted state must not retain a reservation.");
            Assert(HandlerTripTransitionPolicy.TripMayOwnHiddenVehicle(HandlerTripState.InTransit),
                "In-transit vehicles may be hidden.");
            Assert(!HandlerTripTransitionPolicy.TripMayOwnHiddenVehicle(HandlerTripState.Placed),
                "Placed vehicles must be visible.");
        }

        private static void VerifyReservationOwnership()
        {
            ReservationOwner owner = new ReservationOwner(Handler(), TripId.New());
            ReservationLease lease = new ReservationLease(Bay(), owner, 1);
            ReservationAcquireResult acquired = new ReservationAcquireResult(
                ReservationAcquireStatus.Acquired,
                lease);

            Assert(acquired.Acquired, "Acquired result should report success.");
            Assert(acquired.Lease.Equals(lease), "Acquired result should preserve its lease.");
            AssertThrows<ArgumentException>(() => new ReservationAcquireResult(
                ReservationAcquireStatus.Acquired,
                default), "Successful acquisition without a lease should be rejected.");
            AssertThrows<ArgumentOutOfRangeException>(() => new ReservationLease(Bay(), owner, 0),
                "Reservation tokens must be positive.");
        }

        private static void VerifyValidationResults()
        {
            AssignmentValidationResult valid = AssignmentValidationResult.Success();
            AssignmentValidationResult rejected = AssignmentValidationResult.Reject(
                AssignmentValidationCode.VehicleOccupied,
                "Vehicle has a player occupant.");

            Assert(valid.IsValid, "Success should be valid.");
            Assert(!rejected.IsValid, "A rejection should be invalid.");
            Assert(rejected.Code == AssignmentValidationCode.VehicleOccupied,
                "A rejection should retain its stable reason code.");
            AssertThrows<ArgumentException>(() => AssignmentValidationResult.Reject(AssignmentValidationCode.Valid),
                "Reject must not accept the valid code.");
        }

        private static HandlerId Handler() =>
            new HandlerId(Guid.Parse("67f5f75d-c6db-4aa3-9ceb-2f17874b1202"));

        private static VehicleId Vehicle() =>
            new VehicleId(Guid.Parse("9136bb7a-58da-4e74-8edf-e21806844d05"));

        private static LoadingBayKey Bay() =>
            new LoadingBayKey(
                new PropertyKey("BARN"),
                new DockId(Guid.Parse("05ae4e62-da7b-4c73-a080-a0866e5e8c73")),
                0);

        private static void Allow(bool[,] transitions, HandlerTripState from, params HandlerTripState[] destinations)
        {
            foreach (HandlerTripState destination in destinations)
            {
                transitions[(int)from, (int)destination] = true;
            }
        }

        private static void Assert(bool condition, string message)
        {
            assertionCount++;
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void AssertThrows<TException>(Action action, string message)
            where TException : Exception
        {
            assertionCount++;
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            throw new InvalidOperationException(message);
        }
    }
}
