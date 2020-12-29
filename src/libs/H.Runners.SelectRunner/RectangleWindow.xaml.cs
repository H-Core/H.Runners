using System.Windows;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RectangleWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public RectangleWindow()
        {
            InitializeComponent();

            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
        }
    }
}
