namespace Shared

open System

type WeatherDataRow =
    { Date : DateTime
      AverageTemp : float option
      MinimumTemp : float option
      MaximumTemp : float option
      Precipitation : float option
      SnowDepth : float option
      WindDirection : float option
      AverageWindSpeed : float option
      WindGustSpeed : float option
      AverageSeaLevelAirPressure : float option
      SunshineDuration : float option
    }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type IWeatherApi =
    { getHighest20TemperatureDays : unit -> Async<WeatherDataRow array> }