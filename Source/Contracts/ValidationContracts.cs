namespace VehicleHandlers.Contracts
{
    internal enum AssignmentValidationOperation
    {
        Configure = 0,
        StartTrip = 1,
        ResumeTrip = 2,
        PlaceVehicle = 3,
        ManualLoad = 4,
        HideAndRelease = 5,
        RestoreFromSave = 6
    }

    internal enum AssignmentValidationCode
    {
        Valid = 0,
        NotServer = 1,
        AssignmentDisabled = 2,
        ActiveTrip = 3,
        HandlerMissing = 4,
        HandlerWrongRole = 5,
        HandlerUnpaid = 6,
        HandlerFired = 7,
        HandlerUnavailable = 8,
        VehicleMissing = 9,
        VehicleNotOwned = 10,
        VehicleAssignedToAnotherHandler = 11,
        VehicleOccupied = 12,
        VehicleMoving = 13,
        VehicleDestroyed = 14,
        VehicleDeliveryControlled = 15,
        VehicleUnsafe = 16,
        DestinationMissing = 17,
        DestinationNotOwned = 18,
        DestinationUnsupported = 19,
        LoadingBayMissing = 20,
        LoadingBayIdentityMismatch = 21,
        LoadingBayOccupied = 22,
        LoadingBayReservedByAnotherOwner = 23,
        ReservationMissing = 24,
        ReservationNotOwned = 25,
        StaleRevision = 26,
        InvalidSavedState = 27
    }

    internal readonly struct AssignmentValidationResult
    {
        public AssignmentValidationResult(AssignmentValidationCode code, string detail = null)
        {
            Code = code;
            Detail = detail ?? string.Empty;
        }

        public AssignmentValidationCode Code { get; }

        public string Detail { get; }

        public bool IsValid => Code == AssignmentValidationCode.Valid;

        public static AssignmentValidationResult Success() =>
            new AssignmentValidationResult(AssignmentValidationCode.Valid);

        public static AssignmentValidationResult Reject(AssignmentValidationCode code, string detail = null)
        {
            if (code == AssignmentValidationCode.Valid)
            {
                throw new System.ArgumentException("Use Success for a valid result.", nameof(code));
            }

            return new AssignmentValidationResult(code, detail);
        }
    }
}
