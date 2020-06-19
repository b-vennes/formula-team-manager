namespace FormulaTeamManager

open StackExchange.Redis
open Microsoft.Extensions.Logging
open System.Text.Json;
open FormulaTeamManager.EventKeys

/// <summary>Logs events to the event stream.</summary>
/// <param name="logger">The standard logging utility.</param>
/// <param name="stream">The stream to log events to.</stream>
type EventLogger(logger : ILogger<EventLogger>, stream: Stream) =
    
    /// <summary>Logs an event to the stream.</summary>
    /// <param name="formulaEvent">The formula team manager event to log.</param>
    member __.Log(formulaEvent: FormulaEvent) = 

        let generateEntries eventType eventSubtype eventDetail =

            [
                NameValueEntry(RedisValue(eventTypeKey), RedisValue(eventType));
                NameValueEntry(RedisValue(eventSubtypeKey), RedisValue(eventSubtype));
                NameValueEntry(RedisValue(eventDetailKey), RedisValue(eventDetail));
                NameValueEntry(RedisValue(timestampKey), RedisValue(JsonSerializer.Serialize formulaEvent.Timestamp));
            ]
            |> List.toArray

        match formulaEvent.Event with

        | CarEvent(carEvent) ->

            match carEvent with

            | AddedPart(addedPartEvent) ->

                generateEntries carEvent.EventName addedPartEvent.EventName (JsonSerializer.Serialize addedPartEvent)
                |> stream.Add
                |> ignore

            | _ -> logger.LogInformation "unloggable event"

        | TeamEvent(teamEvent) ->

            match teamEvent with

            | ChangedTeamName(changedTeamNameEvent) ->

                generateEntries teamEvent.EventName changedTeamNameEvent.EventName (JsonSerializer.Serialize changedTeamNameEvent)
                |> stream.Add
                |> ignore

            | _ -> logger.LogInformation "unloggable event"

        | _ -> logger.LogInformation "unloggable event"
