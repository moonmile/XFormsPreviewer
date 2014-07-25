namespace Moonmile.XFormsProvider
open System
open System.Xml
open System.Xml.Linq
open System.IO
open System.Reflection
open Xamarin.Forms
open Moonmile.XFormsTypeConv
open System.Runtime.CompilerServices
open Moonmile.XForms

[<Extension>]
type PageXaml() =
    static member LoadXaml(xaml:string) = 
        Moonmile.XForms.ParseXaml.LoadXaml(xaml)

    static member LoadXaml<'T when 'T :> Page >(xaml:string) =
        Moonmile.XForms.ParseXaml.LoadXaml<'T>(xaml)

    static member FindByName(page:Page, name:string) =
        Moonmile.XForms.FindByName(name, page)

    /// <summary>
    /// Alias FindByName from Xamarin.Forms
    /// </summary>
    /// <param name="name"></param>
    [<Extension>]
    static member FindByName<'T when 'T :> Element >(this, name:string) =
        FindByName(name, this) :?> 'T

