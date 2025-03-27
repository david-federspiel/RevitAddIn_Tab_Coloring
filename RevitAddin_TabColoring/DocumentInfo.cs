using static RevitAddin_TabColoring.App;

namespace RevitAddin_TabColoring
{
    public class DocumentInfo
    {
        public System.Windows.Media.Color DocumentColor { get; set; }

        public ColoringDocumentType ColoringDocumentType { get; set; }

        public DocumentInfo(System.Windows.Media.Color color, ColoringDocumentType type)
        {
            DocumentColor = color;
            ColoringDocumentType = type;
        }
    }
}

