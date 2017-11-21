using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;

namespace TIA_Portal_V14_SP1_Starter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            try
            {
                uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
            catch (Exception exp)
            {

            }

        }
    }

    public partial class MainWindow : Window
    {

        string ProjektName = "";
        string ProjektPfad = "h:\\TiaPortal_V14";
        List<RadioButton> RadioButtonList = new List<RadioButton>();

        public MainWindow()
        {
            InitializeComponent();
            ProjekteLesen();
        }

        public void ProjekteLesen()
        {


            /*
            * Aufbau der Projektnamen (Ordner)
            * TwinCAT_V3_PLC_WEB_FUP_Linearachse
            * 
            * _PLC_ oder  _BUG_    
            * + _NC_
            * + _HMI
            * + _VISU_
            * + _FIO_
            * + _WEB_
            * 
            * _AWL_ oder _AS_ oder _FUP_ oder _KOP_ oder _SCL_ oder _ST_
            * 
            * */


            List<string> ProjektVerzeichnis = new List<string>();
            List<string> Projekte_PLC = new List<string>();
            List<string> Projekte_PLC_HMI = new List<string>();
            List<string> Projekte_PLC_FIO = new List<string>();
            List<string> Projekte_BUG = new List<string>();

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");

            foreach (System.IO.DirectoryInfo d in ParentDirectory.GetDirectories())
                ProjektVerzeichnis.Add(d.Name);

            ProjektVerzeichnis.Sort();

            foreach (string Projekt in ProjektVerzeichnis)
            {
                string Sprache = "";
                int StartBezeichnung = 0;

                if (Projekt.Contains("FUP"))
                {
                    Sprache = " (FUP)";
                    StartBezeichnung = 4 + Projekt.IndexOf("FUP");
                }
                if (Projekt.Contains("KOP"))
                {
                    Sprache = " (KOP)";
                    StartBezeichnung = 4 + Projekt.IndexOf("KOP");
                }
                if (Projekt.Contains("SCL"))
                {
                    Sprache = " (SCL)";
                    StartBezeichnung = 4 + Projekt.IndexOf("SCL");
                }

                RadioButton rdo = new RadioButton();
                rdo.GroupName = "TIA_PORTAL_V14_SP1";
                rdo.VerticalAlignment = VerticalAlignment.Top;
                rdo.Checked += new RoutedEventHandler(radioButton_Checked);
                rdo.FontSize = 14;

                if (Projekt.Contains("PLC"))
                {
                    if (Projekt.Contains("HMI"))
                    {
                        rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                        rdo.Name = Projekt;
                        StackPanel_PLC_HMI.Children.Add(rdo);
                    }
                    else
                    {
                        if (Projekt.Contains("FIO"))
                        {
                            rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                            rdo.Name = Projekt;
                            StackPanel_PLC_FIO.Children.Add(rdo);
                        }
                        else
                        {
                            // nur PLC und sonst nichts
                            rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                            rdo.Name = Projekt;
                            StackPanel_PLC.Children.Add(rdo);
                        }
                    }
                }
                else
                {
                    // Es gibt momentan noch keine Gruppe bei den Bugs
                    rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                    rdo.Name = Projekt;
                    StackPanel_BUG.Children.Add(rdo);
                }

                RadioButtonList.Add(rdo);
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");

            EigenschaftenAendern(ProjektStarten_BUG, ProjektStarten_PLC_FIO, ProjektStarten_PLC, ProjektStarten_PLC_HMI, "Enable", "-");

            ProjektName = rb.Name;

            string DateiName = ParentDirectory.FullName + "\\" + rb.Name + "\\index.html";
            string HtmlSeite = System.IO.File.ReadAllText(DateiName);
            string LeereHtmlSeite = "<!doctype html>   </html >";

            if (rb.Name.Contains("PLC")) Web_PLC.NavigateToString(HtmlSeite);
            else Web_PLC.NavigateToString(LeereHtmlSeite);

            if (rb.Name.Contains("PLC_HMI")) Web_PLC_HMI.NavigateToString(HtmlSeite);
            else Web_PLC_HMI.NavigateToString(LeereHtmlSeite);

            if (rb.Name.Contains("PLC_FIO")) Web_PLC_FIO.NavigateToString(HtmlSeite);
            else Web_PLC_FIO.NavigateToString(LeereHtmlSeite);

            if (rb.Name.Contains("BUG")) Web_BUG.NavigateToString(HtmlSeite);
            else Web_BUG.NavigateToString(LeereHtmlSeite);


        }

        private void ProjektStarten(object sender, RoutedEventArgs e)
        {
            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");
            string sourceDirectory = ParentDirectory.FullName + "\\" + ProjektName;

            EigenschaftenAendern(ProjektStarten_BUG, ProjektStarten_PLC_FIO, ProjektStarten_PLC, ProjektStarten_PLC_HMI, "Start", "Ordner " + ProjektPfad + " löschen");
            if (System.IO.Directory.Exists(ProjektPfad)) System.IO.Directory.Delete(ProjektPfad, true);

            EigenschaftenAendern(ProjektStarten_BUG, ProjektStarten_PLC_FIO, ProjektStarten_PLC, ProjektStarten_PLC_HMI, "Start", "Ordner " + ProjektPfad + " erstellen");
            System.IO.Directory.CreateDirectory(ProjektPfad);

            EigenschaftenAendern(ProjektStarten_BUG, ProjektStarten_PLC_FIO, ProjektStarten_PLC, ProjektStarten_PLC_HMI, "Start", "Alle Dateien kopieren");
            Copy(sourceDirectory, ProjektPfad);

            EigenschaftenAendern(ProjektStarten_BUG, ProjektStarten_PLC_FIO, ProjektStarten_PLC, ProjektStarten_PLC_HMI, "Start", "Projekt starten");
            Process proc = new Process();
            proc.StartInfo.FileName = ProjektPfad + "\\start.cmd";
            proc.StartInfo.WorkingDirectory = ProjektPfad;
            proc.Start();
        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void TabControl_SelectionChanged(object sender, RoutedEventArgs e)
        {

            EigenschaftenAendern(ProjektStarten_BUG, ProjektStarten_PLC_FIO, ProjektStarten_PLC, ProjektStarten_PLC_HMI, "Disable", "-");

            string LeereHtmlSeite = "<!doctype html>   </html >";
            Web_PLC.NavigateToString(LeereHtmlSeite);
            Web_PLC_HMI.NavigateToString(LeereHtmlSeite);
            Web_PLC_FIO.NavigateToString(LeereHtmlSeite);
            Web_BUG.NavigateToString(LeereHtmlSeite);
        }

        private void EigenschaftenAendern(Button Knopf1, Button Knopf2, Button Knopf3, Button Knopf4, String ToDo, string Text)
        {
            switch (ToDo)
            {
                case "Enable":
                    Knopf1.IsEnabled = true;
                    Knopf2.IsEnabled = true;
                    Knopf3.IsEnabled = true;
                    Knopf4.IsEnabled = true;

                    Knopf1.Background = new SolidColorBrush(Colors.Green);
                    Knopf2.Background = new SolidColorBrush(Colors.Green);
                    Knopf3.Background = new SolidColorBrush(Colors.Green);
                    Knopf4.Background = new SolidColorBrush(Colors.Green);

                    Knopf1.Refresh();
                    Knopf2.Refresh();
                    Knopf3.Refresh();
                    Knopf4.Refresh();
                    break;

                case "Disable":

                    foreach (RadioButton R_Button in RadioButtonList)
                    {
                        if (R_Button.IsChecked == true) R_Button.IsChecked = false;
                    }

                    Knopf1.Background = new SolidColorBrush(Colors.Gray);
                    Knopf2.Background = new SolidColorBrush(Colors.Gray);
                    Knopf3.Background = new SolidColorBrush(Colors.Gray);
                    Knopf4.Background = new SolidColorBrush(Colors.Gray);

                    Knopf1.IsEnabled = false;
                    Knopf2.IsEnabled = false;
                    Knopf3.IsEnabled = false;
                    Knopf4.IsEnabled = false;

                    Knopf1.Refresh();
                    Knopf2.Refresh();
                    Knopf3.Refresh();
                    Knopf4.Refresh();
                    break;

                case "Start":
                    Knopf1.Background = new SolidColorBrush(Colors.Yellow);
                    Knopf2.Background = new SolidColorBrush(Colors.Yellow);
                    Knopf3.Background = new SolidColorBrush(Colors.Yellow);
                    Knopf4.Background = new SolidColorBrush(Colors.Yellow);

                    Knopf1.Content = Text;
                    Knopf2.Content = Text;
                    Knopf3.Content = Text;
                    Knopf4.Content = Text;

                    Knopf1.Refresh();
                    Knopf2.Refresh();
                    Knopf3.Refresh();
                    Knopf4.Refresh();
                    break;

                default:
                    break;
            }
        }


    }
}
