namespace FormulaTeamManager

open System
open StackExchange.Redis
open FormulaTeamManager.Stream
open FSharp.Json

module Eventing =

    type InitializedCarEvent = { TeamId: string; CarId: string; PartNames: list<string> }

    type AddedPartEvent = { CarId: string; PartName: string }

    type InitializedTeamEvent = { TeamId: string; TeamName: string; }

    type ChangedTeamNameEvent = { TeamId: string; TeamName: string }

    type FormulaEvent =
        | InitializedCar of InitializedCarEvent
        | AddedPart of AddedPartEvent
        | InitializedTeam of InitializedTeamEvent
        | ChangedTeamName of ChangedTeamNameEvent

    let timestampKey = "timestamp"

    let eventDetailsFieldKey = "event-details"
    
    let streamKey = "formula-team-manager"

    module EventLogging =

        let logEvent redis event =
            let generateEntries serializedEvent =
                    [|
                        NameValueEntry(RedisValue(eventDetailsFieldKey), RedisValue(serializedEvent));
                        NameValueEntry(RedisValue(timestampKey), RedisValue(DateTime.Now.ToString()));
                    |]

            generateEntries (Json.serialize event)
            |> addToStream streamKey redis 

    module EventReading = 

        let readEvents redis =
            let toEvent (entry: StreamEntry) =
                let valueFromKey (entry: StreamEntry) key =
                    entry.Item(RedisValue(key)).ToString()

                valueFromKey entry eventDetailsFieldKey
                |> Json.deserialize<FormulaEvent>

            readFromStream redis streamKey
            |> Array.map toEvent
