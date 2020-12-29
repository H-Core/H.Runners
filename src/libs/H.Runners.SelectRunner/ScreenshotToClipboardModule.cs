//using System.Collections.Generic;
//using System.Windows;
//using System.Windows.Input;
//using HomeCenter.NET.Extensions;

//namespace HomeCenter.NET.Utilities.HookModules
//{
//    public class ScreenshotToClipboardModule : ScreenshotModule
//    {
//        public ScreenshotToClipboardModule() : base(new List<Key> { Key.LeftAlt, Key.RightAlt }, null)
//        {
//            NewImage += (obj, image) =>
//            {
//                Clipboard.SetImage(image.ToBitmapImage());
//            };
//        }
//    }
//}
