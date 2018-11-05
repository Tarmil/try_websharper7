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
    open WebSharper.UI.Notation

    let private insertRegion (region : View<string>) = 
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

        let region = Var.Create "Loading region..."
        
        JQuery.GetJSON(geoUrl, fun (_, _) ->
            //get by local IP
            region := "Mars-1"
        )|> ignore

        insertRegion region.View

    let FillRegionPlain () = 
        insertRegion (View.Const "Mars-1")

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
