using System;
using System.Collections.Generic;
using System.Windows.Media;
using Wpf.Windows.Mvvm;

namespace Wpf.Windows
{
    /// <summary>
    /// Simplified MVVM pattern. Usually in combination with Prism or other MVVM frameworks
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {

        private int activeLedItem1 = 1;
        /// <summary>
        /// Index of the active LED
        /// </summary>
        public int ActiveLedItem1
        {
            get { return activeLedItem1; }
            set { activeLedItem1 = value; RaisePropertyChangedEvent("ActiveLedItem1"); }
        }


        private int activeLedItem2 = 2;
        /// <summary>
        /// Index of the active LED
        /// </summary>
        public int ActiveLedItem2
        {
            get { return activeLedItem2; }
            set { activeLedItem2 = value; RaisePropertyChangedEvent("ActiveLedItem2"); }
        }


        private List<Color> colors = new List<Color>
        {
            System.Windows.Media.Colors.DodgerBlue,
            System.Windows.Media.Colors.BlueViolet,
            System.Windows.Media.Colors.DarkSlateBlue,
            System.Windows.Media.Colors.Chocolate
        };
        /// <summary>
        /// Custom color of the LEDs
        /// </summary>
        public List<Color> Colors
        {
            get { return colors; }
            set { colors = value; RaisePropertyChangedEvent("Colors"); }
        }


        /// <summary>
        /// Test delegate
        /// </summary>
        public DelegateCommand TestCommand { get; set; }
        

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            TestCommand = new DelegateCommand(TestMethod);
        }


        /// <summary>
        /// Test method to increase the counter of the LEDs
        /// </summary>
        private void TestMethod()
        {
            if (ActiveLedItem1++ == 3)
            {
                ActiveLedItem1 = 0;
            }

            if (ActiveLedItem2++ == 3)
            {
                ActiveLedItem2 = 0;
            }
        }
    }
}
