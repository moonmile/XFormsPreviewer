namespace Moonmile
open System
open System.Xml
open System.Xml.Linq
open System.IO
open System.Reflection
open Xamarin.Forms

module XFormsTypeConv =

    type EnumTypeConverter<'T>() =
        inherit Xamarin.Forms.TypeConverter()

        override this.CanConvertFrom(sourceType:Type)  =
            if sourceType = typeof<string> then true else false

        override this.ConvertFrom(culture, value:System.Object) =
            let s = value :?> string
            let mutable obj = null
            for v in Enum.GetValues( typeof<'T>) do
                if Enum.GetName( typeof<'T>, v ) = s then
                    obj <- v
            obj

    type LineBreakModeTypeConverter() = inherit EnumTypeConverter<LineBreakMode>()
    type TextAlignmentTypeConverter() = inherit EnumTypeConverter<TextAlignment>()
    type StackOrientationTypeConverter() = inherit EnumTypeConverter<StackOrientation>()
    type LayoutAlignmentTypeConverter() = inherit EnumTypeConverter<LayoutAlignment>()
    type AbsoluteLayoutFlagsTypeConverter() = inherit EnumTypeConverter<AbsoluteLayoutFlags>()

    type LayoutOptionsTypeConverter() =
        inherit TypeConverter()

        override this.CanConvertFrom(sourceType:Type)  =
            sourceType = typeof<string>

        override this.ConvertFrom(culture, value:System.Object) =
            let s = value :?> string
            match s with
            | "Center" -> LayoutOptions.Center 
            | "CenterAndExpand" -> LayoutOptions.CenterAndExpand
            | "End" -> LayoutOptions.End 
            | "EndAndExpand" -> LayoutOptions.EndAndExpand
            | "Fill" -> LayoutOptions.Fill 
            | "FillAndExpand" -> LayoutOptions.FillAndExpand
            | "Start" -> LayoutOptions.Start 
            | "StartAndExpand" -> LayoutOptions.StartAndExpand
            | _ -> LayoutOptions.Start
            :> System.Object


type DateTimeConverter() =
    interface Xamarin.Forms.IValueConverter with
        member x.Convert(value: obj, targetType: Type, parameter: obj, culture: Globalization.CultureInfo): obj = 
            let date = value :?> DateTime
            date.ToString() :> obj     
        member x.ConvertBack(value: obj, targetType: Type, parameter: obj, culture: Globalization.CultureInfo): obj = 
            let strValue = value :?> string
            let mutable resultDateTime:DateTime = DateTime()
            if DateTime.TryParse(strValue, ref resultDateTime) then
                resultDateTime :> obj
            else
                DateTime() :> obj
