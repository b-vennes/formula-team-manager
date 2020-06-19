namespace FormulaTeamManager

open StackExchange.Redis
open System

/// <summary>Handles interactions with the event stream.</summary>
/// <param name="redis">The redis connection to use.<param>
type Stream (redis: ConnectionMultiplexer) =
    let streamKey = RedisKey("formula-team-manager")

    /// <summary>Adds an event to the stream.</summary>
    /// <param name="entries">An array of name-value entries containing event fields.</param>
    member __.Add (entries: NameValueEntry[]) = 
        redis.GetDatabase()
            .StreamAdd(streamKey, entries)

    /// <summary>Reads all events from the stream.</summary>
    /// <returns>An array of stream entries containing the events.</returns>
    member __.ReadAll () =
        redis.GetDatabase()
            .StreamRange(streamKey, Nullable(RedisValue("-")), Nullable(RedisValue("+")))
