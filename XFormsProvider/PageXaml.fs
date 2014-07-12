namespace Moonmile
open System
open System.Xml
open System.Xml.Linq
open System.IO
open System.Reflection
open Xamarin.Forms
open Moonmile.XFormsTypeConv

module XForms =
    /// XElement の拡張メソッド
    type XElement with
        member this.Children 
            with get() =
                seq {
                    let it = ref this.FirstNode
                    while !it <> null do
                        if it.Value.NodeType = XmlNodeType.Element then
                            yield !it :?> XElement
                        it := it.Value.NextNode
                    }
        member this.Attribute(name:string) = 
            if this.Attribute(XName.op_Implicit(name)) <> null then
                this.Attribute(XName.op_Implicit(name)).Value
            else
                ""

    // Xamarin.Forms.Element の拡張メソッド

    // Xamarin製XAMLをパースするクラス
    type ParseXaml() = 

        let getType(propName:string, tagName:string) =
            match propName with 
            | "Title" -> typeof<string> 
            | "Text" -> typeof<string> 
            | "Label" -> typeof<string> 
            | "Placeholder" -> typeof<string> 
            | "Name" -> typeof<string> 
            | "Icon" -> typeof<string> 
            | "BackgroundImage" -> typeof<string> 
            | "Padding" -> typeof<Thickness> 
            | "Font" -> typeof<Font> 
            | "LineBreakMode" -> typeof<LineBreakMode> 
            | "TextColor" | "BackgroundColor" | "Color" -> 
                typeof<Color> 
            | "BorderColor" -> typeof<Color> 
            | "OutlineColor" -> typeof<Color> 
            | "LabelColor" -> typeof<Color> 
            | "Tint" -> typeof<Color> 
            | "BorderRadius" -> typeof<int> 
            | "BorderWidth" -> typeof<int> 
            | "XAlign" -> typeof<TextAlignment> 
            | "YAlign" -> typeof<TextAlignment> 
            | "HasShadow" -> typeof<bool> 
            | "Orientation" -> typeof<StackOrientation> 
            | "Spacing" -> typeof<double> 
            | "Width" -> typeof<GridLength> 
            | "Height" -> typeof<GridLength> 
            | "MaximumDate" -> typeof<DateTime> 
            | "MinimumDate" -> typeof<DateTime> 
            | "Keyboard" -> typeof<Keyboard> 
            | "Aspect" -> typeof<Aspect> 
            | "IsLoading" -> typeof<bool> 
            | "IsOpaque" -> typeof<bool> 
            | "Source" -> 
                match tagName with
                    | "WebView" -> typeof<WebViewSource> 
                    | "Image" -> typeof<ImageSource> 
                    | _ -> null
            | "ImageSource" -> typeof<ImageSource> 
            | "GroupHeaderTemplate" -> typeof<DataTemplate> 
            | "HasUnevenRows" -> typeof<bool> 
            | "IsGroupingEnabled" -> typeof<bool> 
            | "RowHeight" -> typeof<int> 
            | "IsGestureEnabled" -> typeof<bool> 
            | "IsPresented" -> typeof<bool> 
            | "HasRenderLoop" -> typeof<bool> 
            | "SelectedIndex" -> typeof<int> 
            | "Progress" -> typeof<double> 
            | "Maximum" -> typeof<double> 
            | "Minimum" -> typeof<double> 
            | "Value" -> typeof<double> 
            | "Increment" -> typeof<double> 
            | "IsToggled" -> typeof<bool> 
            | "On" -> typeof<bool> 
            | "Intent" -> typeof<TableIntent> 
            | "Root" -> typeof<TableRoot> 
            | "Detail" -> typeof<string> 
            | "DetailColor" -> typeof<Color> 
            | "Format" -> typeof<string> 
            | "Time" -> typeof<TimeSpan> 
            | "Priority" -> typeof<int> 
            | "AnchorX"|"AnchorY" -> typeof<double> 
            | "HeightRequest"|"WidthRequest" -> typeof<double> 
            | "Opacity" -> typeof<double> 
            | "Rotation" | "RotationX" | "RotationY" -> typeof<double> 
            | "Scale" -> typeof<double> 
            | "X" | "Y" -> typeof<double> 
            | "MinimumHeightRequest" -> typeof<double> 
            | "MinimumWidthRequest" -> typeof<double> 
            | "IsBusy" -> typeof<string> 
            | "InputTransparent" -> typeof<bool> 
            | "IsEnabled" -> typeof<bool> 
            | "IsVisible" -> typeof<bool> 
            | "IsClippedToBounds" -> typeof<bool> 
            | "HorizontalOptions"|"VerticalOptions" -> typeof<LayoutOptions> 
            | _ -> null

        member this.SetValue(item:Object, propName:string, s:string, tagName:string ) =
            let t = getType(propName, tagName)
            if t <> null then
                let pi = item.GetType().GetRuntimeProperty(propName)
                if pi <> null then
                    let obj = 
                        match t.Name with
                        | "String" -> s :> System.Object
                        | "Int32" -> s |> int :> System.Object
                        | "Double" -> s |> double :> System.Object
                        | "DateTime" -> Convert.ToDateTime(s) :> System.Object
                        | "TimeSpan" -> TimeSpan.Parse(s) :> System.Object
                        | "Boolean" -> Convert.ToBoolean(s) :> System.Object
                        | "Color"  -> ColorTypeConverter().ConvertFrom(s)
                        | "Font"   -> FontTypeConverter().ConvertFrom(s)
                        | "Thickness" -> ThicknessTypeConverter().ConvertFrom(s)
                        | "GridLength" -> GridLengthTypeConverter().ConvertFrom(s)
                        | "LineBreakMode" -> LineBreakModeTypeConverter().ConvertFrom(s)
                        | "TextAlignment" -> TextAlignmentTypeConverter().ConvertFrom(s)
                        | "StackOrientation" -> StackOrientationTypeConverter().ConvertFrom(s)
                        | "Keyboard" -> KeyboardTypeConverter().ConvertFrom(s)
                        | "WebViewSource" -> WebViewSourceTypeConverter().ConvertFrom(s)
                        | "LayoutOptions" -> LayoutOptionsTypeConverter().ConvertFrom(s)
                        | _ -> null
                    if obj <> null then
                        pi.SetValue( item, obj )

        member this.SetValue(el:XElement, item:Object, propName:string ) =
            let s = el.Attribute(propName)
            if s = "" || s.[0] <> '{' then
                // no binding 
                this.SetValue( item, propName, s, el.Name.LocalName )
            else
                // TODO:binding 
                ()

        member this.SetProperty(el:XElement, item:Object, propName:string ) =
            this.SetValue( el, item, propName )

        member this.SetPropertis( el:XElement, item:Object ) =
            el.Attributes() 
                |> Seq.iter(fun (it) -> this.SetProperty( el, item, it.Name.LocalName ))
            if el.HasAttributes then
                let tag = el.Name.LocalName
                for it in el.Children do
                    if it.Name.LocalName.StartsWith(tag + ".") then
                        let name = it.Name.LocalName.Substring( it.Name.LocalName.IndexOf(".")+1)
                        let value = it.Value
                        this.SetValue( item, name, value, el.Name.LocalName )
                

        member this.SetChildren( el:XElement, item ) =
            let m = item :> Layout<View>
            let mutable it = el.FirstNode
            while it <> null do
                if it.NodeType = XmlNodeType.Element then
                    item.Children.Add(this.LoadView(it :?> XElement))
                it <- it.NextNode

        // Create Page element
        member this.CreatePage(el:XElement, item ) =
            this.SetPropertis( el, item )
            item :> Page
            
        member this.CreateContentPage(el:XElement) =
            let item = new ContentPage()
            this.SetPropertis( el, item )
            // chiled nodes
            for it in el.Children do
                if it.Name.LocalName.IndexOf('.') = -1 then
                    item.Content <- this.LoadView( it )
            item :> Page

        // Create Layout element
        member this.CreateLayout( el:XElement, item:Layout<View> ) =
            this.SetPropertis( el, item )
            this.SetChildren( el, item )
            item :> View
        member this.CreateContentView( el:XElement) =
            let item = new ContentView()
            this.SetPropertis( el, item )
            item.Content <- this.LoadView( el.Children |> Seq.head )
            item :> View
        member this.CreateFrame( el:XElement) =
            let item = new Frame()
            this.SetPropertis( el, item )
            item.Content <- this.LoadView( el.Children |> Seq.head )
            item :> View
        member this.CreateScrollView( el:XElement) =
            let item = new ScrollView()
            this.SetPropertis( el, item )
            item.Content <- this.LoadView( el.Children |> Seq.head )
            item :> View
        member this.CreateGrid( el:XElement ) =
            let item = new Grid()
            this.SetPropertis( el, item )

            for it in el.Children do
                if it.NodeType = XmlNodeType.Element then
                    if it.Name.LocalName.StartsWith("Grid.") then
                        match it.Name.LocalName with
                        | "Grid.RowDefinitions" -> 
                            for ii in it.Children do
                                item.RowDefinitions.Add( this.CreateRowDefinition(ii))
                        | "Grid.ColumnDefinitions" -> 
                            for ii in it.Children do 
                                item.ColumnDefinitions.Add( this.CreateColumnDefinition(ii))
                        | "Grid.BindingContext" -> ()
                        | _ -> ()
                    else
                        let view = this.LoadView(it)
                        let toInt s = if s = "" then 0 else s |> int
                        let toIntOne s = if s = "" then 1 else s |> int
                        let row = toInt(it.Attribute("Grid.Row"))
                        let column = toInt(it.Attribute("Grid.Column"))
                        let rowspan = toIntOne(it.Attribute("Grid.RowSpan"))
                        let colspan = toIntOne(it.Attribute("Grid.ColumnSpan"))
                        item.Children.Add( view, column, column+colspan, row, row+rowspan )
            item :> View

        member this.CreateAbsoluteLayout( el:XElement ) = 
            let item = new AbsoluteLayout()
            this.SetPropertis( el, item )

            for it in el.Children do
                if it.NodeType = XmlNodeType.Element then
                    let view = this.LoadView(it)
                    let bounds = 
                        let v = it.Attribute("AbsoluteLayout.LayoutBounds")
                        if v <> "" then RectangleTypeConverter().ConvertFrom(v) else null
                    let flags = 
                        let v = it.Attribute("AbsoluteLayout.LayoutFlags")
                        if v <> "" then AbsoluteLayoutFlagsTypeConverter().ConvertFrom(v) else null
                    if bounds = null then
                        item.Children.Add( this.LoadView(it))
                    else 
                        item.Children.Add( this.LoadView(it), bounds :?> Rectangle, flags :?> AbsoluteLayoutFlags )
            item :> View

        member this.CreateRowDefinition( el:XElement ) =
            let item = new RowDefinition()
            this.SetPropertis( el,item )
            item
        member this.CreateColumnDefinition( el:XElement ) =
            let item = new ColumnDefinition()
            this.SetPropertis( el, item )
            item 

        // Create Widget element
        member this.CreateWidget( el:XElement, item ) =
            this.SetPropertis( el, item )
            item :> View


        member this.LoadPage(el:XElement) =
            match el.Name.LocalName with
            | "ContentPage" -> this.CreateContentPage(el) 
            | "MasterDetailPage" -> this.CreatePage( el, new MasterDetailPage())
            | "NavigationPage" -> this.CreatePage( el, new NavigationPage())
            | "Page" -> this.CreatePage( el, new Page())
            | "TabbedPage" -> this.CreatePage( el, new TabbedPage())
            | _ -> null

        member this.LoadView(el:XElement) =
            match el.Name.LocalName with
            // layout 
            | "AbsoluteLayout" -> this.CreateAbsoluteLayout(el)
            | "ContentView" -> this.CreateContentView( el )
            | "Frame" -> this.CreateFrame( el )
            | "ScrollView" -> this.CreateScrollView( el )
            | "StackLayout" -> this.CreateLayout( el, new StackLayout())
            | "RelativeLayout" -> this.CreateLayout( el, new RelativeLayout())
            | "Grid" -> this.CreateGrid(el)
            // view 
            | "ActivityIndicator" -> this.CreateWidget( el, new ActivityIndicator())
            | "BoxView" -> this.CreateWidget( el, new BoxView())
            | "Button" -> this.CreateWidget( el, new Button())
            | "DatePicker" -> this.CreateWidget( el, new Xamarin.Forms.DatePicker())
            | "Image" -> this.CreateWidget( el, new Xamarin.Forms.Image())
            | "Editor" -> this.CreateWidget( el, new Xamarin.Forms.Editor())
            | "Entry" -> this.CreateWidget( el, new Xamarin.Forms.Entry())
            | "Label" -> this.CreateWidget( el, new Xamarin.Forms.Label())
            | "ListView" -> this.CreateWidget( el, new Xamarin.Forms.ListView())
            | "OpenGLView" -> this.CreateWidget( el, new Xamarin.Forms.OpenGLView())
            | "Picker" -> this.CreateWidget( el, new Xamarin.Forms.Picker())
            | "ProgressBar" -> this.CreateWidget( el, new Xamarin.Forms.ProgressBar())
            | "SearchBar" -> this.CreateWidget( el, new Xamarin.Forms.SearchBar())
            | "Slider" -> this.CreateWidget(el, new Slider())
            | "Stepper" -> this.CreateWidget( el, new Xamarin.Forms.Stepper())
            | "Switch" -> this.CreateWidget( el, new Xamarin.Forms.Switch())
            | "TableView" -> this.CreateWidget( el, new Xamarin.Forms.TableView())
            | "WebView" -> this.CreateWidget( el, new Xamarin.Forms.WebView())
            | _ -> null
        
        static member LoadXaml(xaml:string) = 
            let doc = XDocument.Load( new StringReader(xaml))
            let px = new ParseXaml()
            px.LoadPage(doc.Root) 

        static member LoadXaml<'T when 'T :> Page >(xaml:string) =
            ParseXaml.LoadXaml( xaml ) :?> 'T



