using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace StoreFrontBanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cmbBgColor.ItemsSource = typeof(Colors).GetProperties();
            cmbFontColor.ItemsSource = typeof(Colors).GetProperties();
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\Publish.txt"))
            {
                MessageBox.Show("Unable to locate " + Directory.GetCurrentDirectory() + "\\Publish.txt.\r\n\r\nPushing changes to all Storefront servers will not be available.\r\nChanges will only be reflected on the current server.",
                    "Missing Publish.txt file");
                lblPublishStatus.Visibility = Visibility.Visible;
            }
        }

        public static string cssstyle = String.Empty;

        private void cmbBgColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Color SelectedBgColor = (Color)(cmbBgColor.SelectedItem as PropertyInfo).GetValue(null, null);
            lblBannerContent.Background = new SolidColorBrush(SelectedBgColor);
        }

        private void cmbFontColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Color SelectedFontColor = (Color)(cmbFontColor.SelectedItem as PropertyInfo).GetValue(null, null);
            lblBannerContent.Foreground = new SolidColorBrush(SelectedFontColor);
        }

        private void chkFontBold_Checked(object sender, RoutedEventArgs e)
        {
            HandleFontBold(sender as CheckBox);
        }
        private void chkFontBold_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleFontBold(sender as CheckBox);
        }

        void HandleFontBold (CheckBox checkBox)
        {
            if (checkBox.IsChecked.Value)
            {
                lblBannerContent.FontWeight = FontWeights.Bold;
            }
            else
            {
                lblBannerContent.FontWeight = FontWeights.Normal;
            }
        }

        private void chkFontItalic_Checked(object sender, RoutedEventArgs e)
        {
            HandleFontItalic(sender as CheckBox);
        }

        private void chkFontItalic_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleFontItalic(sender as CheckBox);
        }

        void HandleFontItalic(CheckBox checkBox)
        {
            if (checkBox.IsChecked.Value)
            {
                lblBannerContent.FontStyle = FontStyles.Italic;
            }
            else
            {
                lblBannerContent.FontStyle = FontStyles.Normal;
            }
        }

        private void xctkFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            double em = Convert.ToDouble(xctkFontSize.Text);
            double pt = em * 12;
            lblBannerContent.FontSize = pt;
        }

        private void XctkBannerSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            lblBannerContent.Height = Convert.ToDouble(xctkBannerSize.Text);
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\Publish.txt"))
                btnPublish.IsEnabled = true;
            ModifyStyleCSS(cssstyle);
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            var targets = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Publish.txt");
            var currenthost = System.Net.Dns.GetHostEntry("localhost").HostName.ToLower();
            foreach (string targetpath in targets)
            {
                if (!targetpath.ToLower().Contains(currenthost))
                {
                    try
                    {
                        File.Copy(cssstyle, targetpath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error copying style.css: " + ex.Message, "File Copy Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSelectStyleCSS_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            cssstyle = ofd.FileName;
        }

        private static string FindColorName(Color color)
        {
            Type colors = typeof(System.Windows.Media.Colors);
            foreach (var prop in colors.GetProperties())
            {
                if (((System.Windows.Media.Color)prop.GetValue(null, null)) == color)
                    return prop.Name;
            }
            return null;
        }

        private void cmbBannerState_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> BannerState = new List<string>();
            BannerState.Add("Select");
            BannerState.Add("Disabled");
            BannerState.Add("Enabled");

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = BannerState;
            comboBox.SelectedIndex = 0;

            Style comboBoxItemStyle = new Style(typeof(ComboBoxItem));
            comboBoxItemStyle.Setters.Add(new Setter(TextElement.FontSizeProperty, 11));
            comboBoxItemStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(4, 0, 0, 0)));
        }

        private void btnModifyReceiverHtml_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please select the 'receiver.html' file usually located in the" + Environment.NewLine + "\"C:\\inetpub\\wwwroot\\Citrix\\<StoreName>Web\" folder.", "Select receiver.html");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string receiverhtml = ofd.FileName;
            string currenthtml = "<div id=\"pluginTop\"><div id=\"customTop\"></div></div>";
            string updatedhtml = "<div id=\"pluginTop\"><div id=\"customTop\"><div class=\"StoreMarquee\"><span></span></div></div></div>";
            var contents = File.ReadAllText(receiverhtml);
            if (contents.Contains(updatedhtml))
            {
                MessageBox.Show("The receiver.html file has already been updated.", "Update Status");
                return;
            }
            if (contents.Contains(currenthtml))
            {
                try
                {
                    File.Copy(receiverhtml, receiverhtml.Replace("receiver", "ReceiverORIG"),true);
                }
                catch (Exception excopy)
                {
                    MessageBoxResult result = MessageBox.Show("Error creating backup copy of receiver.html./r/nSelect OK to continue or Cancel to stop to allow for a manual copy./r/n/r/n"+
                        excopy.Message, "Copy Error", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Cancel)
                        return;
                }

                try
                {
                    contents = contents.Replace(currenthtml, updatedhtml);
                    File.WriteAllText(receiverhtml, contents);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message +
                        "A backup copy named receiverORIG.html was made in the same directory/r/n which can be used for restoring if necessary.", "Error updating receiver.html", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ModifyStyleCSS(string csspath)
        {
            string cssappend = BuildCSS();

            if (csspath == String.Empty)
            {
                MessageBox.Show("Please select the 'style.css' file usually located in the" + Environment.NewLine + "\"C:\\inetpub\\wwwroot\\Citrix\\<StoreName>Web\\custom\" folder.","Missing style.css");
                return;
            }
            
            var stylecsscontents = File.ReadAllText(csspath);
            List<string> stylecsslines = File.ReadAllLines(csspath).ToList();
            if (!stylecsscontents.Contains("/* StoreFront messaging begin */"))
            {
                try
                {
                    File.Copy(csspath, csspath.Replace("style", "style_backup"), true);
                    File.AppendAllText(csspath, cssappend);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message + "/r/nStackTrace: " + ex.StackTrace, "Error modifying style.css",MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {                
                int start = stylecsslines.FindIndex(s => s.Contains("/* StoreFront messaging begin */"));
                int end = stylecsslines.FindIndex(s => s.Contains("/* StoreFront messaging end */"));
                for (int i=end; i>=start-1;i--)
                {
                    stylecsslines.RemoveAt(i);
                }
                try
                {
                    File.Delete(csspath);
                    File.WriteAllLines(csspath, stylecsslines);
                    File.AppendAllText(csspath, cssappend);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message + "/r/nStackTrace: " + ex.StackTrace, "Error modifying style.css", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }            
        }

        // String interpolation: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interpolated-strings
        private string BuildCSS()
        {
            var bannersize = xctkBannerSize.Text;
            var bgColor = "White";
            var textColor = "Black";
            var message = txtMessage.Text.Replace("/r/n"," ");
            var statusblock = String.Empty;
            if (cmbBgColor.SelectedItem != null)
            {
                bgColor = cmbBgColor.GetValue(ComboBox.SelectedValueProperty).ToString().Split(' ')[1].ToString();
            }
            if (cmbFontColor.SelectedItem != null)
            {
                textColor = cmbFontColor.GetValue(ComboBox.SelectedValueProperty).ToString().Split(' ')[1].ToString();
            }
            string fontweight;
            fontweight = (chkFontBold.IsChecked == true) ? "bold" : "normal";
            string fontstyle;
            fontstyle = (chkFontItalic.IsChecked == true) ? "italic" : "normal";
            string fontsize = xctkFontSize.Text;
            if (cmbBannerState.SelectedItem.ToString() == "Enabled")
            {
                statusblock = "/* StoreFront messaging begin */";
            }
            if (cmbBannerState.SelectedItem.ToString() == "Disabled")
            {
                statusblock = "/* StoreFront messaging begin *//*";
            }

            var customCSS = $@"
{statusblock}

#customTop {{
height:{bannersize}px;
background:{bgColor};
}}


.StoreMarquee {{
    width: 0 auto;
	margin: 0 auto;
	white-space: nowrap;
	overflow: hidden;
	box-sizing: border-box;
}}


.StoreMarquee span {{
    font-family:'Arial Black', sans-serif;
    font-style:{fontstyle};
    font-size:{fontsize}em;
	font-weight:{fontweight};
    color:{textColor};
	display: inline-block;
	padding-left: 100%; 
	animation: StoreMarquee 30s linear infinite;
	-moz-animation: StoreMarquee 30s linear infinite;
	-webkit-animation: StoreMarquee 30s linear infinite;
}}


@keyframes StoreMarquee {{
    0 %   {{ transform: translate(0, 0); }}
	100% {{ transform: translate(-100%, 0); }}
}}

@-moz-keyframes StoreMarquee {{
    0 %   {{ -moz-transform: translate(0, 0); }}
	100% {{ -moz-transform: translate(-100%, 0); }}
}}

@-webkit-keyframes StoreMarquee {{
    0 %   {{ -webkit-transform: translate(0, 0); }}
	100% {{ -webkit-transform: translate(-100%, 0); }}
}}

.StoreMarquee span:after {{
Content: '{message}'; 
display: inline;
}}

/* StoreFront messaging end */
";
            return customCSS;
        }        
    }
}
