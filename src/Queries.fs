namespace FormulaTeamManager

open FormulaTeamManager.Eventing
open FormulaTeamManager.Results

module Queries =

    let queryTeams redis =
    
        let foldEventIntoTeams teams event =
            match event with
            | InitializedTeam initializedTeamEvent -> 
                [{ Team.Id = initializedTeamEvent.TeamId; TeamName = initializedTeamEvent.TeamName }]
                |> List.append teams
            | ChangedTeamName changedTeamNameEvent ->
                let replaceNameIfMatchingIds (team: Team) =
                    if team.Id = changedTeamNameEvent.TeamId
                        then { team with TeamName = changedTeamNameEvent.TeamName }
                    else team
                    
                teams |> List.map replaceNameIfMatchingIds
            | _ -> teams

        EventReading.readEvents redis
        |> Array.fold foldEventIntoTeams []
        |> Ok

    let queryTeam redis id =

        let foldEventIntoTeam (teamResult: Result<Team,TeamNotFoundError>) (event: FormulaEvent) =

            let foldInitializedTeam (initializedTeam: InitializedTeamEvent) =
                match teamResult with
                | Ok team -> 
                    if initializedTeam.TeamId = team.Id
                        then Ok({ team with Id = initializedTeam.TeamId })
                    else teamResult
                | Error -> Ok({ Team.Id = initializedTeam.TeamId; TeamName = initializedTeam.TeamName })

            let foldChangedTeamName (changedTeamName: ChangedTeamNameEvent) =
                match teamResult with
                | Ok team -> 
                    if changedTeamName.TeamId = team.Id
                        then Ok({ team with TeamName = changedTeamName.TeamName })
                    else teamResult
                | Error -> teamResult

            match event with
            | InitializedTeam initializedTeamEvent -> foldInitializedTeam initializedTeamEvent
            | ChangedTeamName changedTeamNameEvent -> foldChangedTeamName changedTeamNameEvent
            | _ -> teamResult

        EventReading.readEvents redis
        |> Array.fold foldEventIntoTeam (Error({ TeamNotFoundError.TeamId = id }))

    let queryCar redis id = 

        let foldEventIntoCar (carResult: Result<Car, CarNotFoundError>) event =
            let foldInitializedCarEvent (carResult: Result<Car, CarNotFoundError>) (initializedCarEvent: InitializedCarEvent) =
                match carResult with
                | Ok car -> 
                    if car.Id = initializedCarEvent.CarId
                        then Ok({ Id = initializedCarEvent.CarId; TeamId = initializedCarEvent.TeamId; Parts = initializedCarEvent.PartNames })
                    else carResult
                | Error carNotFoundError ->
                    if carNotFoundError.CarId = initializedCarEvent.CarId
                        then Ok({ Id = initializedCarEvent.CarId; TeamId = initializedCarEvent.TeamId; Parts = initializedCarEvent.PartNames })
                    else carResult

            let foldAddedPartEvent (carResult: Result<Car, CarNotFoundError>) (addedPart: AddedPartEvent) =
                match carResult with
                | Ok car -> 
                    if car.Id = addedPart.CarId
                        then Ok({ car with Parts = (List.append car.Parts [addedPart.PartName]) })
                    else
                        carResult
                | Error -> carResult

            match event with
            | InitializedCar initializedCar -> foldInitializedCarEvent carResult initializedCar
            | AddedPart addedPart -> foldAddedPartEvent carResult addedPart
            | _ -> carResult

        EventReading.readEvents redis
        |> Array.fold foldEventIntoCar (Error({ CarNotFoundError.CarId = id }))

    let queryCars redis teamId =
        let foldEventsIntoCars (carsResult: Result<list<Car>,TeamNotFoundError>) event =
            let foldInitializedCarEvent (initializedCar: InitializedCarEvent) =
                let addToCars cars carId =
                    match (queryCar redis carId) with
                    | Ok car ->
                        List.append cars [car]
                    | Error -> cars

                match carsResult with
                | Ok cars ->
                    if initializedCar.TeamId = teamId
                        then Ok(addToCars cars initializedCar.CarId)
                    else carsResult
                | Error -> carsResult

            let foldInitializedTeamEvent (initializedTeam: InitializedTeamEvent) =
                if (initializedTeam.TeamId = teamId)
                    then Ok([])
                else carsResult

            match event with
            | InitializedCar initializedCar -> foldInitializedCarEvent initializedCar
            | InitializedTeam initializedTeam -> foldInitializedTeamEvent initializedTeam
            | _ -> carsResult

        EventReading.readEvents redis
        |> Array.fold foldEventsIntoCars (Error({ TeamNotFoundError.TeamId = teamId }))
