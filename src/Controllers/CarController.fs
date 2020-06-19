namespace FormulaTeamManager.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FormulaTeamManager

/// <summary>Handles car command and query requests.</summary>
[<ApiController>]
[<Route("[controller]")>]
type CarController (logger : ILogger<CarController>, eventLogger: EventLogger, eventReader: EventReader) =
    inherit ControllerBase()

    /// <summary>Performs a modify part action on a car.</summary>
    /// <param name="parameters">The add part parameters.</param>
    /// <returns>An error message if an error has occurred.</returns>
    [<HttpPost("AddPart")>]
    member __.ModifyPart([<FromBody>] parameters: AddPartParameters) : Option<string> =
        { 
            Event = 
                CarEvent(
                    AddedPart(
                        AddedPartEvent(parameters.TeamName, parameters.PartName)));
            Timestamp = DateTime.Now;
        }
        |> eventLogger.Log

        None

    /// <summary>Queries for car information and history by team name.</summary>
    /// <param name="teamName">The name of the car's team.</param>
    /// <returns>The car information.</returns>
    [<HttpGet("{teamName}")>]
    member __.GetCar(teamName: string) : Car =

        let foldCarEventIntoCar car event =

            match event with

                | AddedPart(addedPartEvent) 
                    when addedPartEvent.TeamName.ToLower() = car.Teams.Head.ToLower() -> 

                        { car with Parts = (List.append car.Parts [addedPartEvent.PartName]) |> List.distinct }

                | _ -> car

        let foldTeamEventIntoCar car event =
        
            match event with

                | ChangedTeamName(changedTeamNameEvent)
                    when changedTeamNameEvent.NewTeamName.ToLower() = car.Teams.Head.ToLower() ->

                        { car with Teams = List.append [changedTeamNameEvent.PreviousTeamName] car.Teams }

                | _ -> car

        let foldEventIntoCar car event =

            match event with
            | CarEvent(carEvent) -> foldCarEventIntoCar car carEvent
            | TeamEvent(teamEvent) -> foldTeamEventIntoCar car teamEvent
            | _ -> car

        eventReader.ReadEvents()
        |> Array.rev
        |> Array.fold foldEventIntoCar { Teams = [teamName]; Parts = [] }
