using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// Need to run this prorgram by administrator mode,
// Or open listener port on your machine.
// 管理者モードで動かすか、指定ポート（例.10150）を許可させてください。
// 大抵の場合、プレビューアから名前解決ができないので IP アドレス経由で
// HTTP 接続すると思われる。

namespace XFormsPreviewHost
{
    public class Program
    {
        static void Main(string[] args)
        {
            new PreviewHost().main(args);
        }
    }

    public class PreviewHost
    {
        public int Port { get; set; }
        public Dictionary<string, string> LoadPaths { get; private set; }

        public PreviewHost()
        {
            this.Port = 10150;
            this.LoadPaths = new Dictionary<string, string>();
            // TODO: change load xaml file
            // ここで、レスポンスを返す XAML ファイルを指定。
            // 作成中のプロジェクトフォルダを指定すればok.
            // 後で WPF あたりで作成してドラッグ＆ドロップ.
            // mac のためにコマンドラインで使えるようにしておく。
            var basefolder = @"..\..\..\Sample\XFormsSampleApp\XFormsSampleApp\";
            this.LoadPaths.Add("0", basefolder + "MainPage.xaml");
        }
        public void main(string[] args)
        {
            string prefix = string.Format( "http://*:{0}/", this.Port );

            /// TODO: ベースフォルダを指定する
            /// フォルダ内の *.xaml ファイルのリストを取得して、LoadPaths に保持する


            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Not Supported!");
                return;
            }
            Console.WriteLine("start");
            HttpListener hl = new HttpListener();
            hl.Prefixes.Add(prefix);
            hl.Start();

            while (true)
            {
                HttpListenerContext context = hl.GetContext();
                HttpListenerRequest req = context.Request;
                HttpListenerResponse res = context.Response;

                res.StatusCode = (int)HttpStatusCode.OK;
                res.ContentType = MediaTypeNames.Text.Xml;
                res.ContentEncoding = Encoding.UTF8;

                var path = req.Url.LocalPath;
                if ( path.StartsWith("/list"))
                {
                    /// TODO: *.xaml ファイルの一覧を返す
                }
                else if ( path.StartsWith("/get"))
                {
                    var num = path.Replace("/get/", "");
                    if ( this.LoadPaths.ContainsKey(num)) {
                        var file = this.LoadPaths[num];
                        Console.WriteLine("get {0} {1}", num, file);
                        using (var st = File.OpenText(file))
                        {
                            var xaml = st.ReadToEnd();
                            StreamWriter sw = new StreamWriter(res.OutputStream);
                            sw.Write(xaml);
                            sw.Flush();
                            res.Close();
                        }
                        
                    }
                }
            }
        }
    }
}
