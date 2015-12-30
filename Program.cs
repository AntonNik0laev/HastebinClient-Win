using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;

namespace HasteClient
{
    static class Program
    {
        class IconApplicationContext : ApplicationContext
        {
            private NotifyIcon icon;
            public NotifyIcon Icon => icon;
            public IconApplicationContext()
            {
                icon = new NotifyIcon
                {
                    Icon = Resources.MainIcon,
                    ContextMenu = new ContextMenu(new[] {new MenuItem("Exit", (sender, args) => Application.Exit()),}),
                    Visible = true
                };
            }            
        }
        class AddDocumentResponse
        {
            public string Key { get; set; }
        }

        [STAThread]
        static void Main()
        {
            var restClient = new RestClient("http://hastebin.com");

            IconApplicationContext ctx = new IconApplicationContext();

            var kbHook = new KeyboardHook();
            var textSelectionReader = new TextSelectionReader();
            kbHook.KeyPressed += (sender, args) =>
            {
                var text = textSelectionReader.TryGetSelectedTextFromActiveControl();
                //Console.WriteLine(text);
                var request = new RestRequest("/documents", Method.POST);
                request.AddParameter("application/json", text, ParameterType.RequestBody);
                var response = restClient.Execute<AddDocumentResponse>(request);
                var link = "http://hastebin.com/" + response.Data.Key;
                Clipboard.SetText(link);
                
                ctx.Icon.ShowBalloonTip(1000, "link copied to clipboard", link, ToolTipIcon.Info);
                Console.WriteLine(link);
            };
            kbHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.S);
            Application.Run(ctx);
        }
    }

}
