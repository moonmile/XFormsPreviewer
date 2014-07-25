XFormsPreviewer
===============
Dynamic loading XAML file of Xamarin.Forms

It is a tool that you can preview by dynamically loading the XAML file of Xamarin.Froms format.
Preview itself is implemented by the various emulator (iOS / Android / Winwodws Phone).
Without having to build it is incorporated into Xamarin.Forms a XAML file, 
you can see the layout of the screen.

Code of these modules are written in F#. 
However, since it does not depend on FSharp.Core.dll, you can use in the same way from the C#.

# Implements 

- Easy preview tool
- Easy preview host (easy http server)
- NuGet
- Use x:Name by FindByName<T>("name")
- Use Data Binding ex. Text="{Binding propName}"
- Use Command event ex. Command="{Binding ClickCommand}"
- Auto loading BindingContext ex. <local:MyViewModelData />

# NuGet

Xamarin.Forms.XamlProvider
https://www.nuget.org/packages/Xamarin.Forms.XamlProvider/

# Sample

XamarinXamlPreview

# License

The MIT License (MIT)

# Future

- Static Resource の参照
- 外部UIモジュールの参照（特に Xamarin.Forms.Labs）
- デザイン時の Data Binding の実装

# History

- ver.0.1.6 2014/07/25 FindByName bugfix and Xamarin.Forms version up
- ver.0.1.5 2014/07/24 standalone compile switch and modified FileByName<> method

# 日本語の解説

Xamarin.Forms 用のプレビューアをアルファ版で公開 
http://www.moonmile.net/blog/archives/6030
