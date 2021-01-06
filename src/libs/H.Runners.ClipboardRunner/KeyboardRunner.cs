//using System.Windows.Forms;
//using H.Core.Runners;

//namespace HomeCenter.NET.Runners
//{
//    public class KeyboardRunner : Runner
//    {
//        #region Constructors

//        public KeyboardRunner()
//        {
//            //AddAction("keyboard", KeyboardCommand, "CONTROL+V");
//        }

//        #endregion

//        #region Private methods

//        private static void KeyboardCommand(string? command)
//        {
//            if (string.IsNullOrWhiteSpace(command))
//            {
//                return;
//            }

//            SendKeys.Send(command);
//        }

//        #endregion
//    }
//}
