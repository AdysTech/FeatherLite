using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AdysTech.FeatherLite.Extensions;

namespace AdysTech.FeatherLite.View
{
    public enum DisplayInformationSource
    {
        Hardware,
        LegacyDefault,
        DesignTimeFallback
    }
    //<summary>Gives Information about the physical device screen that the app is running.</summary>
    //<ref>http://blogs.windows.com/buildingapps/2013/11/22/taking-advantage-of-large-screen-windows-phones</ref>
    class DeviceDisplayInfo
    {
        private static readonly string RawDpiValueName = "RawDpiX";
        private static readonly string PhysicalScreenResolutionName = "PhysicalScreenResolution";
        private static Size DesignTimeSizeLumia920 = new Size (2.31523089942387, 3.85871816570645);
        private static Size DesignTimeResolutionLumia920 = new Size (768, 1280);
        public const double BaselineDiagonalInInches15To9HighRes = 4.5; // Lumia 920
        public static readonly double DiagonalToWidthRatio16To9 = 9.0 / Math.Sqrt (Math.Pow (16, 2) + Math.Pow (9, 2));
        public static readonly double DiagonalToWidthRatio15To9 = 9.0 / Math.Sqrt (Math.Pow (15, 2) + Math.Pow (9, 2));
        public static readonly double BaselineWidthInInches = BaselineDiagonalInInches15To9HighRes * DiagonalToWidthRatio15To9;
        internal const int BaselineWidthInViewPixels = 480;

        public double PhysicalDiagonal { get; private set; }
        public Size PhysicalSize { get; private set; }
        public Size PhysicalResolution { get; private set; }
        public Size ViewResolution { get; private set; }
        public double ViewPixelsPerInch { get; private set; }
        public double RawDpi { get; private set; }
        public double AspectRatio { get; private set; }
        public double RawPixelsPerViewPixel { get; private set; }
        public double ViewPixelsPerHostPixel { get; private set; }
        public double HostPixelsPerViewPixel { get; private set; }
        public DisplayInformationSource InformationSource { get; private set; }

        public double GetViewPixelsForPhysicalSize(double inches)
        {
            return inches * ViewPixelsPerInch;
        }


        public static DeviceDisplayInfo Default { get; private set; }
        //<summary>Instance Constructor, will Initialize using static instance</summary>
        public DeviceDisplayInfo()
        {
            PhysicalDiagonal = Default.PhysicalDiagonal;
            PhysicalSize = Default.PhysicalSize;
            PhysicalResolution = Default.PhysicalResolution;
            ViewResolution = Default.ViewResolution;
            ViewPixelsPerInch = Default.ViewPixelsPerInch;
            RawDpi = Default.RawDpi;

            AspectRatio = Default.AspectRatio;
            RawPixelsPerViewPixel = Default.RawPixelsPerViewPixel;

            ViewPixelsPerHostPixel = Default.ViewPixelsPerHostPixel;
            HostPixelsPerViewPixel = Default.HostPixelsPerViewPixel;

            InformationSource = Default.InformationSource;
        }


        static DeviceDisplayInfo()
        {
            if ( !DesignerProperties.IsInDesignTool )
            {
                Default = CreateFromHardwareInfo ();
                return;
            }
            //while in Design mode we will use Lumia 920 as baseline, 4.5" 15:9 ratio
            Default = new DeviceDisplayInfo (DesignTimeSizeLumia920, DesignTimeResolutionLumia920, DisplayInformationSource.DesignTimeFallback);
        }

        private static DeviceDisplayInfo CreateFromHardwareInfo()
        {
            Size screenResolution;
            double rawDpi;
            object temp;
            if ( !DeviceExtendedProperties.TryGetValue (PhysicalScreenResolutionName, out temp) )
                throw new NotSupportedException ("This class needs at least Windows Phone 8.0 GDR3 to operate!!");
            screenResolution = (Size) temp;

            if ( !DeviceExtendedProperties.TryGetValue (RawDpiValueName, out temp) || (double) temp == 0d )
                throw new NotSupportedException ("This class needs at least Windows Phone 8.0 GDR3 to operate!!");

            rawDpi = (double) temp;

            var physicalSize = new Size (screenResolution.Width / rawDpi, screenResolution.Height / rawDpi);

            return new DeviceDisplayInfo (physicalSize, screenResolution, DisplayInformationSource.Hardware);
        }


        private DeviceDisplayInfo(Size physicalSize, Size physicalResolution, DisplayInformationSource displayInformationSource)
        {
            PhysicalSize = physicalSize;
            InformationSource = displayInformationSource;
            PhysicalDiagonal = PhysicalSize.GetHypotenuse ();
            PhysicalResolution = physicalResolution;

            AspectRatio = physicalSize.Height / physicalSize.Width;

            if ( !( AspectRatio.IsCloseEnoughTo (physicalResolution.Height / physicalResolution.Width) ) )
                throw new ArgumentOutOfRangeException ("physicalResolution", "only square pixels supported");

            RawDpi = physicalResolution.Width / physicalSize.Width;
            
            double DiagonalToWidthRatio15To9 = 9.0 / Math.Sqrt (Math.Pow (15, 2) + Math.Pow (9, 2));

            //
            var AbsoluteScaleFactorBeforeNormalizing = PhysicalSize.Width / BaselineWidthInInches;

            // Calculate Scaling factor, Never less than 1 or more view pixels than physical pixels
            var scale = Math.Max (1, AbsoluteScaleFactorBeforeNormalizing);
            var idealViewWidth = Math.Min (BaselineWidthInViewPixels * scale, PhysicalResolution.Width);
            var idealScale = PhysicalResolution.Width / idealViewWidth;

            RawPixelsPerViewPixel = idealScale.NudgeToClosestPoint (1);

            ViewResolution = new Size (PhysicalResolution.Width / RawPixelsPerViewPixel, PhysicalResolution.Height / RawPixelsPerViewPixel);

            ViewPixelsPerInch = RawDpi / RawPixelsPerViewPixel;

            // Adjust for different aspect ratio, if necessary
            ViewPixelsPerHostPixel = Math.Min (ViewResolution.Width / Application.Current.Host.Content.ActualWidth,
              ViewResolution.Height / Application.Current.Host.Content.ActualHeight);

            HostPixelsPerViewPixel = 1 / ViewPixelsPerHostPixel;

        }
    }

}

