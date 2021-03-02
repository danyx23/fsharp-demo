module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model =
    { WeatherData: WeatherDataRow array }

type Msg =
    | GotWeatherData of WeatherDataRow array
    | RequestHighestTemperatures

let weatherApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IWeatherApi>

let init(): Model * Cmd<Msg> =
    let model =
        { WeatherData = [||] }

    model, Cmd.none

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GotWeatherData weatherRows ->
        Fable.Core.JS.console.log("got weather data")
        { model with WeatherData = weatherRows }, Cmd.none
    | RequestHighestTemperatures ->
        let cmd = Cmd.OfAsync.perform weatherApi.getHighest20TemperatureDays () GotWeatherData
        model, cmd

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]

let containerBox (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [

        Content.content [ ] [
            h2 [ ] [ str "Hottest days in Berlin"]
            Table.table [ ] [
                thead [ ] [
                    tr [] [
                        th [ Style [ TextAlign TextAlignOptions.Right ] ] [ str "Date"]
                        th [ Style [ TextAlign TextAlignOptions.Right ] ] [ str "Temperature"]
                    ]]

                tbody [ ] [

                    for weatherDataRow in model.WeatherData do
                        let temperatureDisplayString =
                            match weatherDataRow.MaximumTemp with
                            | None -> "-"
                            | Some f -> sprintf "%.1f °C" f
                        yield
                            tr [ ] [
                                td [ Style [ TextAlign TextAlignOptions.Right ] ] [str (sprintf "%s" (weatherDataRow.Date.ToShortDateString())) ]
                                td [ Style [ TextAlign TextAlignOptions.Right ] ] [str temperatureDisplayString ]
                                ]
                ]

            ]
            Field.div [ Field.IsGrouped ] [


            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.OnClick (fun _ -> dispatch RequestHighestTemperatures)
                ] [
                    str "Get highest temperatures"
                ]
            ]
        ]
        ]

    ]

let view (model : Model) (dispatch : Msg -> unit) =
    Hero.hero [
        Hero.Color IsPrimary
        Hero.IsFullHeight
        Hero.Props [
            Style [
                Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                BackgroundSize "cover"
            ]
        ]
    ] [
        Hero.head [ ] [
            Navbar.navbar [ ] [
                Container.container [ ] [ navBrand ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    Heading.p [ Heading.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [ str "BFP Meetup Demo" ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
