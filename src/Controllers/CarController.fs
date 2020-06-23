namespace FormulaTeamManager.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FormulaTeamManager.Actions
open FormulaTeamManager.Queries
open StackExchange.Redis
open FormulaTeamManager.Results

[<ApiController>]
[<Route("[controller]")>]
type CarController (logger: ILogger<CarController>, redis: ConnectionMultiplexer) =
    inherit ControllerBase()

    [<HttpGet("QueryById/{carId}")>]
    member this.QueryById(carId: string) =
        let handleResult result =
            match result with
            | Ok car -> car |> this.Ok :> IActionResult
            | Error -> this.NotFound() :> IActionResult

        queryCar redis carId |> handleResult
        

    [<HttpGet("QueryByTeamId/{teamId}")>]
    member this.QueryByTeamId(teamId: string) =
        let handleResult result =
            match result with
            | Ok cars -> cars |> this.Ok :> IActionResult
            | Error -> this.NotFound() :> IActionResult

        queryCars redis teamId |> handleResult

    [<HttpPost("Initialize")>]
    member this.Initialize([<FromBody>] parameters: InitializeCarParameters) =
        let handleResult result =
            match result with
            | Ok -> this.Ok() :> IActionResult
            | Error e -> this.StatusCode(500, e) :> IActionResult

        handleAction redis (InitializeCar(parameters)) |> handleResult

    [<HttpPost("AddPart")>]
    member this.AddPart([<FromBody>] parameters: AddPartParameters) = 
        let handleResult result =
            match result with
            | Ok -> this.Ok() :> IActionResult
            | Error e -> this.StatusCode(500, e) :> IActionResult

        handleAction redis (AddPart(parameters)) |> handleResult
