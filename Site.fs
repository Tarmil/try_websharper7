namespace WebSharper7

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/">] Home

module Templating =
    open WebSharper.UI.Templating

    type LayoutTemplate = Template<"Layout.html", ClientLoad.FromDocument>
    type HomeTemplate = Template<"Home.html", ClientLoad.FromDocument>   

[<JavaScript>]
module Client = 
    let FillRegion () = 
        //get by local IP
        let region = "Mars-1"
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

module Site =
    let LayoutPage ctx (title: string) (body: Doc) =
        Content.Page(
            Templating
                .LayoutTemplate()
                .Title(title)
                .Body(body)
                .Elt(keepUnfilled = true)
                .OnAfterRender(fun _ -> Client.FillRegion ())
        )

    let HomePage ctx =
        let body = 
            Templating
                .HomeTemplate()
                .Doc(keepUnfilled = true)

        LayoutPage ctx "Home page" body

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
        )
