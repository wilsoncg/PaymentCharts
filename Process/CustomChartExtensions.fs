module CustomChartExtensions

open System
open System.IO
open XPlot.Plotly
open HtmlAgilityPack

open Html

let CustomSaveHtmlAs pathName (ch:PlotlyChart) =
    let html = ch.GetHtml()
    let customHtml =
        let mutable doc = HtmlDocument() 
        doc.LoadHtml html
        let mutable node = HtmlNode.CreateNode("<meta/>")
        node.Attributes.Add(doc.CreateAttribute("http-equiv", "refresh"))
        node.Attributes.Add(doc.CreateAttribute("content", sprintf "%i" 3600))
        doc.DocumentNode.SelectSingleNode("//head").AppendChild(node) |> ignore
        doc.DocumentNode.InnerHtml
    let file = sprintf "%s.html" pathName
    File.WriteAllText(file, customHtml) |> ignore