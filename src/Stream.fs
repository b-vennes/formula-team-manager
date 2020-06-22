namespace FormulaTeamManager

open StackExchange.Redis
open System

module Stream =

    let addToStream streamKey (redis: ConnectionMultiplexer) entries =
        redis.GetDatabase()
            .StreamAdd(RedisKey(streamKey), entries)
        |> Ok

    let readFromStream (redis: ConnectionMultiplexer) (streamKey: string) =
        redis.GetDatabase()
            .StreamRange(RedisKey(streamKey), Nullable(RedisValue("-")), Nullable(RedisValue("+")))
