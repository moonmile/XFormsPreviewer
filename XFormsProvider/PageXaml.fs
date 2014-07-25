namespace Moonmile
open System
open System.Xml
open System.Xml.Linq
open System.IO
open System.Reflection
open Xamarin.Forms
open Moonmile.XFormsTypeConv
open System.Text.RegularExpressions

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

        member this.GetAttr(name:string) =
            if this.Attribute(XName.Get(name)) = null then "" else this.Attribute(XName.Get(name)).Value

    // エイリアス
    type BaseElement = Xamarin.Forms.BindableObject

    /// <summary>
    /// Pageクラスの情報を保持するクラス
    /// </summary>
    type PageData() = 
        member val ElementNames:list<(string*Element)> = [] with get, set
        member val ClassName = "" with get, set

    let mutable Pages : list<(Page * PageData)> = []

    [<Literal>]
    let XMLNS_WIFX = "http://schemas.microsoft.com/winfx/2009/xaml"

    /// <summary>
    /// 名前解決
    /// </summary>
    /// <param name="name"></param>
    /// <param name="page"></param>
    let FindByName(name:string,page:Page) = 
            let ( _, pdata ) = Pages |> Seq.find( fun(p,d) -> p = page )
            let ( _, el ) =  pdata.ElementNames |> Seq.find( fun(n,el) -> n = name )
            el

    // Xamarin製XAMLをパースするクラス
    type ParseXaml() = 

        let mutable rootPage:Page = null
        let mutable pageData = new PageData()
        let AddPage( page ) = Pages <- ( page :> Page, pageData )::Pages

        /// <summary>
        /// プロパティ名から型を推測する
        /// </summary>
        let getType(propName:XName, tagName:XName) =
            match propName.LocalName with 
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
                match tagName.LocalName with
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

            | "BindingContext" -> typeof<string>
            | "Command" -> typeof<string>
            | "Clicked" -> typeof<string>
            | _ -> null

        /// <summary>
        /// プロパティへ値を設定する
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propName"></param>
        /// <param name="s"></param>
        /// <param name="tagName"></param>
        member this.SetValue(item:BaseElement, propName:XName, s:string, tagName:XName ) =
            let t = getType(propName, tagName)
            if t <> null then
                match propName.NamespaceName with
                | XMLNS_WIFX ->
                    let el = item :?> Element
                    match propName.LocalName with 
                    | "Name" -> 
                        pageData.ElementNames <- (s, item :?> Element)::pageData.ElementNames 
                    | "Class" ->
                        pageData.ClassName <- s
                    | _ -> ()
                | _ -> 
                    match propName.LocalName with
                    | "Clicked" ->
                    (*  TODO: clicked event pending
                        let t = Type.GetType(pageData.ClassName)
                        let mi = t.GetRuntimeMethod(s, [|typeof<obj>; typeof<EventArgs>|])
                        let de = mi.CreateDelegate(t)
                        let ei = item.GetType().GetRuntimeEvent(propName.LocalName)
                        ei.AddEventHandler( item, de )
                    *)  ()
                    | _ ->
                        let pi = item.GetType().GetRuntimeProperty(propName.LocalName) 
                        if pi <> null then
                            let obj =
                                match t.Name with
                                | "String" -> s :> obj
                                | "Int32" -> s |> int :> obj
                                | "Double" -> s |> double :> obj
                                | "DateTime" -> Convert.ToDateTime(s) :> obj
                                | "TimeSpan" -> TimeSpan.Parse(s) :> obj
                                | "Boolean" -> Convert.ToBoolean(s) :> obj
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
        
        member this.SetBinding(item:BaseElement, propName:XName, s:string) =
            if s.StartsWith("{Binding") && s.EndsWith("}") then
                let replace (pattern:string, replacement:string) input:string = 
                    Regex.Replace(input, pattern, replacement)
                let s' = s  |> replace ("^{Binding","")
                            |> replace ("}$", "") 
                let items = s'.Split([|','|])
                let mutable path = ""
                let mutable format = ""
                let mutable mode = BindingMode.Default
                for it in items do
                    let it' = it.Trim()
                    match it' with
                    | _ when it'.StartsWith("Path=") ->
                        path <- it'.Replace("Path=","").Trim()
                    | _ when it'.StartsWith("Mode=") ->
                        match it'.Replace("Mode=","").Trim() with 
                        | "OneWay" -> mode <- BindingMode.OneWay
                        | "OneWayToSource" -> mode <- BindingMode.OneWayToSource
                        | "TwoWay" -> mode <- BindingMode.TwoWay
                        | _ -> mode <- BindingMode.Default
                    | _ when it'.StartsWith("StringFormat=") ->
                        format <- it' |> replace ("^StringFormat='","") |> replace ("'$","" )
                    | _ -> path <- it'.Trim()
                
                let prop = item.GetType().GetRuntimeField(propName.LocalName + "Property")
                if prop <> null then
                    let bp = prop.GetValue( item ) :?> BindableProperty 
                    let bind = new Binding( path, mode, null, format )
                    item.SetBinding( bp, bind )

        member this.SetValue(el:XElement, item:BaseElement, propName:XName ) =
            if el.Attribute(propName) = null then
                ()
            else 
                let s = el.Attribute(propName).Value
                if s = "" || s.[0] <> '{' then
                    // no binding 
                    this.SetValue( item, propName, s, el.Name )
                else
                    // binding 
                    this.SetBinding( item, propName, s )

        member this.SetProperty(el:XElement, item:BaseElement, propName:XName ) =
            this.SetValue( el, item, propName )

        member this.SetPropertis( el:XElement, item:BaseElement ) =
            el.Attributes() 
                |> Seq.iter(fun (it) -> this.SetProperty( el, item, it.Name ))

        member this.SetChildren( el:XElement, item:Element ) =
            let mutable it = el.FirstNode
            while it <> null do
                if it.NodeType = XmlNodeType.Element then
                    let view = this.LoadView(it :?> XElement, item )
                    if view <> null then
                        match item with
                        | :? Layout<View> -> 
                            let it = item :?> Layout<View>
                            it.Children.Add(view)
                        | _ -> ()
                it <- it.NextNode

        // Create Page element
        member this.CreatePage(el:XElement, item ) =
            rootPage <- item
            this.SetPropertis( el, item )
            item :> Page
            
        member this.CreateContentPage(el:XElement) =
            let item = new ContentPage()
            rootPage <- item
            AddPage( item )

            // child nodes
            for it in el.Children do
                let view = this.LoadView( it, item )
                if view <> null then
                    item.Content <- view
            this.SetPropertis( el, item )
            item :> Page

        // Create Layout element
        member this.CreateLayout( el:XElement, item:Layout<View> ) =
            this.SetPropertis( el, item )
            this.SetChildren( el, item )
            item :> View
        member this.CreateContentView( el:XElement) =
            let item = new ContentView()
            item.Content <- this.LoadView( el.Children |> Seq.head, item)
            item.Content.Parent <- item
            this.SetPropertis( el, item )
            item :> View
        member this.CreateFrame( el:XElement) =
            let item = new Frame()
            item.Content <- this.LoadView( el.Children |> Seq.head, item )
            item.Content.Parent <- item
            this.SetPropertis( el, item )
            item :> View
        member this.CreateScrollView( el:XElement) =
            let item = new ScrollView()
            item.Content <- this.LoadView( el.Children |> Seq.head, item)
            item.Content.Parent <- item
            this.SetPropertis( el, item )
            item :> View
        member this.CreateGrid( el:XElement ) =
            let item = new Grid()
            for it in el.Children do
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
                    let view = this.LoadView(it, item)
                    if view <> null then
                        let toInt(s) = if s = "" then 0 else s |> int
                        let toIntOne(s) = if s = "" then 1 else s |> int
                        let row = toInt(it.GetAttr("Grid.Row"))
                        let column = toInt(it.GetAttr("Grid.Column"))
                        let rowspan = toIntOne(it.GetAttr("Grid.RowSpan"))
                        let colspan = toIntOne(it.GetAttr("Grid.ColumnSpan"))
                        item.Children.Add( view, column, column+colspan, row, row+rowspan )
                        view.Parent <- item

            this.SetPropertis( el, item )
            item :> View

        member this.CreateAbsoluteLayout( el:XElement ) = 
            let item = new AbsoluteLayout()
            for it in el.Children do
                let view = this.LoadView(it, item)
                if view <> null then
                    view.Parent <- item
                    let bounds = 
                        let v = it.GetAttr("AbsoluteLayout.LayoutBounds")
                        if v <> "" then RectangleTypeConverter().ConvertFrom(v) else null
                    let flags = 
                        let v = it.GetAttr("AbsoluteLayout.LayoutFlags")
                        if v <> "" then AbsoluteLayoutFlagsTypeConverter().ConvertFrom(v) else null
                    if bounds = null then
                        item.Children.Add( view )
                    else 
                        item.Children.Add( view, bounds :?> Rectangle, flags :?> AbsoluteLayoutFlags )
            this.SetPropertis( el, item )

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
            this.SetChildren( el, item )
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


        member this.LoadView(el:XElement, pa:Element) =
            if el.Name.LocalName.IndexOf(".") < 0 then
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
            else 
                let name = el.Name.LocalName.Substring( el.Name.LocalName.IndexOf(".")+1)
                let value = el.Value
                this.SetValue( pa, XName.Get(name), value, el.Name )

                match name with
                | "BindingContext" ->
                    let elBind = el.Children |> Seq.head
                    let ns = elBind.Name.NamespaceName
                    let asmNs = ns.Split([|';'|]).[0].Split([|':'|]).[1]
                    let asmName = ns.Split([|';'|]).[1].Split([|'='|]).[1]
                    try 
                        let asm = Assembly.Load(AssemblyName(asmName))
                        let t = asm.GetType( asmNs + "." + elBind.Name.LocalName)
                        let obj = System.Activator.CreateInstance(t)
                        let pi = pa.GetType().GetRuntimeProperty("BindingContext")
                        pi.SetValue( pa, obj )
                    with
                        | _ -> ()
                | _ -> ()
                null
        
        static member LoadXaml(xaml:string) = 
            let doc = XDocument.Load( new StringReader(xaml))
            let px = new ParseXaml()
            let page = px.LoadPage(doc.Root) 
            page
        static member LoadXaml<'T when 'T :> Page >(xaml:string) =
            ParseXaml.LoadXaml( xaml ) :?> 'T
