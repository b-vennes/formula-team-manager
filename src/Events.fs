namespace FormulaTeamManager

open System

/// <summary>When a new part has been added to a car.</summary>
/// <param name="teamName">The name of the team with the part added.</param>
/// <param name="partName">The name of the part added.</param>
type AddedPartEvent(teamName: string, partName: string) =
    let mutable teamName = teamName
    let mutable partName = partName

    /// <summary>Initializes an empty added part event.</summary>
    new() = AddedPartEvent("", "")

    /// <summary>The name of the team with the added part.</summary>
    member __.TeamName
        with get () = teamName
        and set (value) = teamName <- value

    /// <summary>The name of the added part.</summary>
    member __.PartName
        with get () = partName
        and set (value) = partName <- value

    /// <summary>The event name.</summary>
    member __.EventName = "added-part-event"


/// <summary>When the name of a team has changed.</summary>
/// <param name="previousTeamName">The previous name of the team.</param>
/// <param name="newTeamName">The new name of the team.</param>
type ChangedTeamNameEvent(previousTeamName: string, newTeamName: string) =
    let mutable previousTeamName = previousTeamName
    let mutable newTeamName = newTeamName

    /// <summary>Initializes an empty changed team name event.</summary>
    new() = ChangedTeamNameEvent("", "")

    /// <sumamary>The previous team name.</summary>
    member __.PreviousTeamName
        with get () = previousTeamName
        and set (value) = previousTeamName <- value

    /// <summary>The current team name.</summary>
    member __.NewTeamName
        with get () = newTeamName
        and set (value) = newTeamName <- value

    /// <summary>The name of the event.</summary>
    member __.EventName = "changed-team-name-event"

/// <summary>When an event has occurred with a car.</summary>
type CarEvent =
    /// <summary>When an added part event has occurred on the car.</summary>
    | AddedPart of AddedPartEvent

    /// <summary>The event name.</summary>
    member __.EventName = "car-event"

/// <summary>When an event has occurred with driver.</summary>
type DriverEvent =
    /// <summary>When a driver has been hired.</summary>
    | HiredDriver
    /// <summary>When a driver has been fired.</summary>
    | FiredDriver

    /// <summary>The event name.</summary>
    member __.EventName = "driver-event"

/// <summary>When an event has occurred with a team.</summary>
type TeamEvent =
    /// <summary>When a team has changed their name</summary> 
    | ChangedTeamName of ChangedTeamNameEvent

    /// <summary>The name of the event.</summary>
    member __.EventName = "team-event"

/// <summary>The possible events.</summary>
type EventType =
    | CarEvent of CarEvent
    | DriverEvent of DriverEvent
    | TeamEvent of TeamEvent
    | EventError

/// <summary>When an event has occurred in formula team manager.</summary>
type FormulaEvent = { Event: EventType; Timestamp: DateTime }