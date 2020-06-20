namespace FormulaTeamManager

open System
open StackExchange.Redis
open FormulaTeamManager.Stream
open FSharp.Json

module Eventing =

    type InitializedCarEvent = { TeamId: string; CarId: string }

    let initializedCarEventKey = "initialized-car"

    /// <summary>When a new part has been added to a car.</summary>
    type AddedPartEvent = { TeamId: string; CarId: string; TeamName: string; PartName: string }

    let addedPartEventKey = "added-part"

    type InitializedTeamEvent = { TeamId: string; TeamName: string; }

    let initializedTeamEventKey = "initialized-team"

    /// <summary>When the name of a team has changed.</summary>
    type ChangedTeamNameEvent = { TeamId: string; NewTeamName: string }

    let changedTeamNameEventKey = "changed-team-name"

    type FormulaEvent =
        | InitializedCar of InitializedCarEvent
        | AddedPart of AddedPartEvent
        | InitializedTeam of InitializedTeamEvent
        | ChangedTeamName of ChangedTeamNameEvent

    /// <summary>When an event has occurred in formula team manager.</summary>
    type LoggedEvent = { Event: FormulaEvent; Timestamp: DateTime; }

    let timestampKey = "timestamp"

    let eventFieldKey = "event"

    let eventDetailsFieldKey = "event-details"

    type StreamType = 
        | Car
        | Team

    let carStreamKey carId = "formula-team-manager-car-" + carId

    let teamStreamKey teamId = "formula-team-manager-team-" + teamId

    let logEvent redis event =
        let generateEntries eventKey serializedEvent =
                [|
                    NameValueEntry(RedisValue(eventFieldKey), RedisValue(eventKey));
                    NameValueEntry(RedisValue(eventDetailsFieldKey), RedisValue(serializedEvent));
                    NameValueEntry(RedisValue(timestampKey), RedisValue(Json.serialize DateTime.Now));
                |]

        match event with
        | InitializedCar initializedCarEvent ->
            initializedCarEvent
            |> Json.serialize
            |> generateEntries initializedCarEventKey
            |> addToStream (carStreamKey initializedCarEvent.CarId) redis
        | AddedPart addedPartEvent ->
            addedPartEvent
            |> Json.serialize
            |> generateEntries addedPartEventKey
            |> addToStream (carStreamKey addedPartEvent.CarId) redis
        | InitializedTeam initializedTeamEvent ->
            initializedTeamEvent
            |> Json.serialize
            |> generateEntries initializedCarEventKey
            |> addToStream (teamStreamKey initializedTeamEvent.TeamId) redis
        | ChangedTeamName changedTeamNameEvent ->
            changedTeamNameEvent
            |> Json.serialize
            |> generateEntries changedTeamNameEventKey
            |> addToStream (teamStreamKey changedTeamNameEvent.TeamId) redis

    let readEvents redis stream =
        let toEvent (entry: StreamEntry) =

            let valueFromKey (entry: StreamEntry) key =
                entry.Item(RedisValue(key)).ToString()

            match (valueFromKey entry eventFieldKey) with 
            | value when value = initializedCarEventKey ->
                valueFromKey entry eventDetailsFieldKey
                |> Json.deserialize<InitializedCarEvent>
                |> InitializedCar
                |> Ok
            | value when value = addedPartEventKey ->
                valueFromKey entry eventDetailsFieldKey
                |> Json.deserialize<AddedPartEvent>
                |> AddedPart
                |> Ok
            | value when value = initializedTeamEventKey ->
                valueFromKey entry eventDetailsFieldKey
                |> Json.deserialize<InitializedTeamEvent>
                |> InitializedTeam
                |> Ok
            | value when value = changedTeamNameEventKey ->
                valueFromKey entry eventDetailsFieldKey
                |> Json.deserialize<ChangedTeamNameEvent>
                |> ChangedTeamName
                |> Ok
            | _ -> Error "Error reading events."

        match stream with
        | Car -> carStreamKey
        | Team -> teamStreamKey
        |> readAllFromStream redis
        |> Array.map toEvent 
            
