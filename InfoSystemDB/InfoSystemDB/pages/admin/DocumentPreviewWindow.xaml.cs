using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace InfoSystemDB
{
    public partial class DocumentPreviewWindow : Window
    {
        private List<Order> content;

        public DocumentPreviewWindow(List<Order> content)
        {
            this.content = content;
            InitializeComponent();

            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Name = "flowDocumentViewer";
            viewer.Margin = new Thickness(10);

            FlowDocument document = CreateFlowDocument();
            document.PageHeight = 842; 
            document.PageWidth = 595;
            viewer.Document = document;

            contentFrame.Content = viewer;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocumentScrollViewer viewer = contentFrame.Content as FlowDocumentScrollViewer;

                FlowDocument document = XamlReader.Parse(XamlWriter.Save(viewer.Document)) as FlowDocument;
                
                IDocumentPaginatorSource paginatorSource = document;
                if (paginatorSource != null)
                {
                    printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Друк документа");
                }
            }
        }

        private FlowDocument CreateFlowDocument()
        {
            FlowDocument document = new FlowDocument();
            
            foreach (Order i in content)
            {
                Paragraph productParagraph = new Paragraph(new Run($"{i.ID}: {i.Products}"));
                document.Blocks.Add(productParagraph);
            }

            return document;
        }
    }
}