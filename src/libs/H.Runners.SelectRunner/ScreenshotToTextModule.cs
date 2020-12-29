//using System.Collections.Generic;
////using System.Threading.Tasks;
////using System.Windows;
//using System.Windows.Input;
////using IronOcr;
////using IronOcr.Languages;

//namespace HomeCenter.NET.Utilities.HookModules
//{
//    public class ScreenshotToTextModule : ScreenshotModule
//    {
//        //private AdvancedOcr? Ocr { get; set; }

//        public ScreenshotToTextModule() : base(new List<Key> { Key.LeftAlt, Key.M }, null)
//        {
//            AutoDisposeImage = false;

//            /*
//            NewImage += async (obj, image) => await Task.Run(() =>
//            {
//                Ocr ??= new AdvancedOcr
//                {
//                    //CleanBackgroundNoise = true,
//                    //EnhanceContrast = true,
//                    //EnhanceResolution = true,
//                    Language = new MultiLanguage(English.OcrLanguagePack, Russian.OcrLanguagePack),
//                    Strategy = AdvancedOcr.OcrStrategy.Advanced,
//                    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
//                    DetectWhiteTextOnDarkBackgrounds = true,
//                    InputImageType = AdvancedOcr.InputTypes.AutoDetect,
//                    //RotateAndStraighten = true,
//                    //ReadBarCodes = true,
//                    //ColorDepth = 4,
//                };
//                var result = Ocr.Read(image);

//                Application.Current.Dispatcher?.Invoke(() => Clipboard.SetText(result.Text));
//            });*/
//        }
//    }
//}
