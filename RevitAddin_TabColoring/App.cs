using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using UIFramework;
using Xceed.Wpf.AvalonDock.Controls;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace RevitAddin_TabColoring
{
    public class App : IExternalApplication
    {
        // function status
        public static bool isActive = true;

        public Result OnStartup(UIControlledApplication uiApp)
        {
            // create new ribbon tab
            string tabName = "TEST RIBBON";
            uiApp.CreateRibbonTab(tabName);


            // button 1 : coloring on/off

            //create ribbon panel
            RibbonPanel panel = uiApp.CreateRibbonPanel(tabName, "Coloring panel");

            //create button
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData(
                "ChnageColor intern",
                "Tab Coloring ON/OFF",
                assemblyPath,
                "RevitAddin_TabColoring.OnOffCommand"
                );
            PushButton button = panel.AddItem(buttonData) as PushButton;


            // button 2: coloring settings

            //create robbon panel
            RibbonPanel panelSettings = uiApp.CreateRibbonPanel(tabName, "Setting panel");

            // create button
            PushButtonData settingButtonData = new PushButtonData(
                "Settings intern",
                "Coloring Settings",
                assemblyPath,
                "RevitAddin_TabColoring.SettingCommand"
                );
            PushButton settingButton = panelSettings.AddItem(settingButtonData) as PushButton;

            // register an event>> view is opened
            uiApp.ViewActivated += OnViewActivated;

            // regsiter an event >> document is closing
            uiApp.ControlledApplication.DocumentClosing += OnDocumentClosing;

            // result
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication uiApp)
        {
            uiApp.ViewActivated -= OnViewActivated;
            uiApp.ControlledApplication.DocumentClosing += OnDocumentClosing;

            return Result.Succeeded;
        }

        // List of opened documents in order of opening
        public List<Document> documentList = new List<Document>();

        // List of opened document titles (=keys for dictionary)
        public static List<string> documentNames = new List<string>();

        // dictionary to store documentsTitle (key) and corresponding DocumentInfo (value) = color | document type
        public static Dictionary<string, DocumentInfo> documentDictionary = new Dictionary<string, DocumentInfo>();

        // list of colors
        public static List<System.Windows.Media.Color> colorList = new List<System.Windows.Media.Color> { Colors.DeepSkyBlue, Colors.YellowGreen, Colors.Gold, Colors.Coral, Colors.Navy, Colors.Green, Colors.Yellow, Colors.Red, Colors.Orange, Colors.Magenta };

        // create "originalBrush", "originalBorder" and "originalThickness", which will be used to reset tab colors to the revit default values
        public static Brush originalBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        public static Brush originalBorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
        public static Thickness originalThicknes = new Thickness(0, 0, 0, 0);

        // enum for different document types
        public enum ColoringDocumentType
        {
            Project,
            Family,
            Template
        }

        // event handler "OnViewActivated" that executes as soon as event "viewactivated" is triggered
        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            // get the document of the view, that just opened and store it in the document list if it's new to the list
            Document openedDocument = e.Document;
            if (!documentList.Contains(openedDocument))
            {
                documentList.Add(openedDocument);
                documentNames.Add(openedDocument.Title);
                // invoke updateDictionary method
                UpdateDictionary(documentList, documentDictionary, colorList);
            }

            //Ensure TabColoring runs AFTER event finishes >> current view is found by "TabColoring"
            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                TabColoring(documentDictionary);
            });
        }



        // event handler if a document is going to be closed 
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            // get the document, that is going to be closed and remove it from the documentList
            Document closingDocument = e.Document;
            documentList.Remove(closingDocument);
            documentNames.Remove(closingDocument.Title);

            // invoke updateDictionary method
            UpdateDictionary(documentList, documentDictionary, colorList);

            // invoke tabcoloring to apply changes
            TabColoring(documentDictionary);
        }


        // method to update the dictionary
        public static void UpdateDictionary(List<Document> documentList, Dictionary<string, DocumentInfo> documentDictionary, List<System.Windows.Media.Color> colorList)
        {
            //iterate through all currently opened document
            for (int i = 0; i < documentList.Count; i++)
            {
                Document document = documentList[i];

                //get document name
                string docTitle = document.Title;

                //get document type 
                ColoringDocumentType docType;

                if (document.IsFamilyDocument == false)
                {
                    docType = ColoringDocumentType.Project;
                }
                else
                {
                    docType = ColoringDocumentType.Family;
                }

                // create new DocumentInfo instance
                DocumentInfo documentInfo = new DocumentInfo(colorList[i], docType);

                // add documentset elements to the dictionary
                documentDictionary[docTitle] = documentInfo;
            }

            // remove all entries from dictionary, if key is nor part of documentNames
            foreach (string key in documentDictionary.Keys)
            {
                if (!documentNames.Contains(key))
                {
                    documentDictionary.Remove(key);
                }
            }
        }


        // method that uncolors all tabs
        public static void TabUnColoring()
        {
            // get the revit main window and search for every child item of type layoutdocumentpanecontrol
            var docPanes = MainWindow.getMainWnd().FindVisualChildren<LayoutDocumentPaneControl>();

            // ietrate through alle items of docpanes
            foreach (var pane in docPanes)
            {
                // search for every child item of type "TabItem"
                var tabs = pane.FindVisualChildren<TabItem>();

                // iterate through all tabs and color them with the revit originalbrush
                foreach (var tab in tabs)
                {
                    tab.Background = originalBrush;
                    tab.BorderBrush = originalBorderBrush;
                    tab.BorderThickness = originalThicknes;
                }
            }
        }


        //  method that colors tabs according to their documentIdentification and documentType
        public static void TabColoring(Dictionary<string, DocumentInfo> documentDictionary)
        {
            // get the revit main window and search for every child item of type layoutdocumentpanecontrol
            var docPanes = MainWindow.getMainWnd().FindVisualChildren<LayoutDocumentPaneControl>();

            // ietrate through alle items of docpanes
            foreach (var pane in docPanes)
            {
                // searxh for every child item of type "TabItem"
                var tabs = pane.FindVisualChildren<TabItem>();

                // iterate through all tabs and color them according to their name and document-type
                foreach (var tab in tabs)
                {
                    string tooltipName = tab.ToolTip.ToString();

                    // check which document title macthes the tooltipname
                    foreach (string key in documentDictionary.Keys)
                    {
                        // to compare tooltip to documentDictionary member, store docType as a string
                        string docTypeStr = null;
                        if (documentDictionary[key].ColoringDocumentType == ColoringDocumentType.Project)
                        {
                            docTypeStr = ".rvt";
                        }
                        else if (documentDictionary[key].ColoringDocumentType == ColoringDocumentType.Family)
                        {
                            docTypeStr = ".rfa";
                        }

                        // for new created documents >> NO docType
                        if (tooltipName.Contains($"{key} - "))
                        {
                            switch (documentDictionary[key].ColoringDocumentType)
                            {
                                case ColoringDocumentType.Project:
                                    Coloring(SettingWindow.projectSelection, tab, key);
                                    break;
                                case ColoringDocumentType.Family:
                                    Coloring(SettingWindow.familySelection, tab, key);
                                    break;
                                case ColoringDocumentType.Template:
                                    break;
                                default:
                                    break;
                            }
                        }
                        // for documents containing docType
                        else if (tooltipName.Contains($"{key}{docTypeStr} - "))
                        {
                            switch (documentDictionary[key].ColoringDocumentType)
                            {
                                case ColoringDocumentType.Project:
                                    Coloring(SettingWindow.projectSelection, tab, key);
                                    break;
                                case ColoringDocumentType.Family:
                                    Coloring(SettingWindow.familySelection, tab, key);
                                    break;
                                case ColoringDocumentType.Template:
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                }

            }
        }


        // method that colores according to user input (Background, Border, Top Bar)
        public static void Coloring(string selection, TabItem tab, string key)
        {
            //coloring according to users choice
            switch (selection)
            {
                case "Ausgefüllt":
                    tab.Background = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    break;
                case "Umrandet - dünn":
                    tab.BorderBrush = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    tab.BorderThickness = new Thickness(2);
                    break;
                case "Umrandet - mittel":
                    tab.BorderBrush = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    tab.BorderThickness = new Thickness(3);
                    break;
                case "Umrandet - dick":
                    tab.BorderBrush = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    tab.BorderThickness = new Thickness(4);
                    break;
                case "oberer Balken - dünn":
                    tab.BorderBrush = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    tab.BorderThickness = new Thickness(0, 2, 0, 0);
                    break;
                case "oberer Balken - mittel":
                    tab.BorderBrush = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    tab.BorderThickness = new Thickness(0, 3, 0, 0);
                    break;
                case "oberer Balken - dick":
                    tab.BorderBrush = new SolidColorBrush(documentDictionary[key].DocumentColor);
                    tab.BorderThickness = new Thickness(0, 4, 0, 0);
                    break;
            }

        }
    }
}

