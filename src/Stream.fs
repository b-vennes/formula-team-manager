namespace FormulaTeamManager

open StackExchange.Redis
open System

module Stream =

    /// <summary>Adds an event to a stream.</summary>
    /// <param name="streamKey">The name of the stream.</param>
    /// <oaram name="entries">The event key-value details.</param> 
    /// <param name="redis">The redis connection multiplexer.</param>
    let addToStream streamKey (redis: ConnectionMultiplexer) entries =
        redis.GetDatabase()
            .StreamAdd(RedisKey(streamKey), entries)
        |> Ok

    /// <summary>Reads all events from the stream.</summary>
    /// <param name="streamKey">The name of the stream.</param>
    /// <param name="redis">The redis connection multiplexer.</param>
    let readAllFromStream (redis: ConnectionMultiplexer) (streamKey: string) =
        redis.GetDatabase()
            .StreamRange(RedisKey(streamKey), Nullable(RedisValue("-")), Nullable(RedisValue("+")))
