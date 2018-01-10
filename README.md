# StoreFrontBanner
Custom scrolling notification banner for Storefront

This is completely based on the tool published in the StoreFront Message Customization blog. I wanted just a little more formatting functionality than what was offered. The original tool also came with a PowerShell script to modify the receiver.html which I moved into the executable so everything in managed from a utility.

The solution leverages the WPFToolkit from XCeed. The dll is included in the StoreFrontBanner.zip file, but you will need to add it to the solution in VS using NuGet Package Manager.

PM>Install-Package WPFToolkit -Version 3.5.50211.1

For usage details, see http://www.afinn.net/citrix/Scrolling-Notification-Customization-for-Storefront/ 
