namespace Moonmile.XFormsProvider
open System
open System.Xml
open System.Xml.Linq
open System.IO
open System.Reflection
open Xamarin.Forms
open Moonmile.XFormsTypeConv

type PageXaml() =
    static member LoadXaml(xaml:string) = 
        Moonmile.XForms.ParseXaml.LoadXaml(xaml)

    static member LoadXaml<'T when 'T :> Page >(xaml:string) =
        Moonmile.XForms.ParseXaml.LoadXaml<'T>(xaml)

    
    



