namespace WebSharper7

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server
open WebSharper.JQuery

type EndPoint =
    | [<EndPoint "/">] Home

module Templating =
    open WebSharper.UI.Templating

    type HomeTemplate = Template<"Home.html", ClientLoad.FromDocument>

[<JavaScript>]
module Client = 
    let private insertRegion (region : string) = 
        let content = 
            Templating
                .HomeTemplate
                .RegionTemplate()
                .Name(region)
                .Doc()

        Templating
            .HomeTemplate()
            .Region(content)
            .Bind()

    let FillRegionAjax () = 
        let geoUrl = "https://ipinfo.io/json"
        
        JQuery.GetJSON(geoUrl, fun (_, _) ->
            //get by local IP
            insertRegion "Mars-1"
        )|> ignore

    let FillRegionPlain () = 
        insertRegion "Mars-1"

module Site =
    let HomePage ctx =
        Content.Page(
            Templating.HomeTemplate()
                .Title("Home page")
                .Elt(keepUnfilled = true)
                .OnAfterRender(fun _ -> Client.FillRegionAjax ())
        )            
 
    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
        )
