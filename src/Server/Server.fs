module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open FSharp.Data

open Shared

type WeatherHistory = CsvProvider<"berlin-weather.csv", CacheRows = false, HasHeaders=false, Schema = "Date (date), AverageTemperature (float option), MinimumTemperature (float option), MaximumTemperature (float option), Precipitation (float option), SnowDepth (float option), WindDirection (float option), AverageWindSpeed (float option), WindGustSpeed (float option), AverageSeaLevelAirPressure (float option), SunshineDuration (float option)">
let weatherHistory = WeatherHistory.Load("berlin-weather.csv")

let weatherDateRowsArray =
    weatherHistory.Rows
    |> Seq.map (fun row ->
        { Date = row.Date
          AverageTemp = row.AverageTemperature
          MinimumTemp = row.MinimumTemperature
          MaximumTemp = row.MaximumTemperature
          Precipitation = row.Precipitation
          SnowDepth = row.SnowDepth
          WindDirection = row.WindDirection
          AverageWindSpeed = row.AverageWindSpeed
          WindGustSpeed = row.WindGustSpeed
          AverageSeaLevelAirPressure = row.AverageSeaLevelAirPressure
          SunshineDuration = row.SunshineDuration
        })
    |> Seq.toArray

let getHighest20TemperatureDays () : Async<WeatherDataRow array> =
    async {
        return
            weatherDateRowsArray
            |> Array.sortByDescending (fun row -> row.MaximumTemp)
            |> Array.take 20
    }

let weatherApi =
    { getHighest20TemperatureDays = getHighest20TemperatureDays }

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue weatherApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
