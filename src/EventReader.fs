namespace FormulaTeamManager

open Microsoft.Extensions.Logging
open StackExchange.Redis
open System.Text.Json

/// <summary>Reads the event log from the stream.</summary>
/// <param name="logger">The standard loggin utility.</param>
/// <param name="stream">The stream to read events from.</param>
type EventReader (logger: ILogger<EventReader>, stream: Stream) = 

    /// <summary>Reads all events from the event stream.</summary>
    /// <returns>An array of events.</returns>
    member __.ReadEvents () =

        let mapToEvent (streamEntry: StreamEntry) =

            match streamEntry.Item(RedisValue("event-type")).ToString() with

            | "car-event" ->
            
                match streamEntry.Item(RedisValue("event-subtype")).ToString() with

                | "added-part-event" ->
                
                    CarEvent(
                        AddedPart(
                            streamEntry.Item(RedisValue("event-detail")).ToString()
                            |> JsonSerializer.Deserialize<AddedPartEvent>))

                | _ -> EventError

            | "team-event" ->

                match streamEntry.Item(RedisValue("event-subtype")).ToString() with

                | "changed-team-name-event" ->

                    TeamEvent(
                        ChangedTeamName(
                            streamEntry.Item(RedisValue("event-detail")).ToString()
                            |> JsonSerializer.Deserialize<ChangedTeamNameEvent>))

                | _ -> EventError

            | _ -> EventError

        stream.ReadAll()
        |> Array.map mapToEvent
