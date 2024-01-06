using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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
            viewer.Margin = new Thickness(0);

            FlowDocument document = CreateFlowDocument();
            document.PageHeight = 758;
            document.PageWidth = 1130;
            viewer.Document = document;

            contentFrame.Content = viewer;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument document = CreateFlowDocument();

                document.PageWidth = printDialog.PrintableAreaWidth;

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

            document.PageWidth = 1130;
            document.PageHeight = 758;

            document.ColumnWidth = document.PageWidth;

            Table table = new Table();

            table.Columns.Add(new TableColumn() { Width = new GridLength(70) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(95) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(290) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(140) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(70) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(120) });

            table.RowGroups.Add(new TableRowGroup());

            TableRow headerRow = new TableRow();

            foreach (TableColumn column in table.Columns)
            {
                TableCell cell = new TableCell(new Paragraph(new Run(" ")))
                {
                    Background = Brushes.LightGray,
                    TextAlignment = TextAlignment.Center
                };

                cell.BorderBrush = Brushes.Black;
                cell.BorderThickness = new Thickness(1);

                headerRow.Cells.Add(cell);
            }

            headerRow.Cells[0].Blocks.Clear();
            headerRow.Cells[0].Blocks.Add(new Paragraph(new Run("Номер")));
            headerRow.Cells[1].Blocks.Clear();
            headerRow.Cells[1].Blocks.Add(new Paragraph(new Run("Дата")));
            headerRow.Cells[2].Blocks.Clear();
            headerRow.Cells[2].Blocks.Add(new Paragraph(new Run("Покупець")));
            headerRow.Cells[3].Blocks.Clear();
            headerRow.Cells[3].Blocks.Add(new Paragraph(new Run("Телефон")));
            headerRow.Cells[4].Blocks.Clear();
            headerRow.Cells[4].Blocks.Add(new Paragraph(new Run("Вироби")));
            headerRow.Cells[5].Blocks.Clear();
            headerRow.Cells[5].Blocks.Add(new Paragraph(new Run("Адреса")));
            headerRow.Cells[6].Blocks.Clear();
            headerRow.Cells[6].Blocks.Add(new Paragraph(new Run("Сума замов.")));
            headerRow.Cells[7].Blocks.Clear();
            headerRow.Cells[7].Blocks.Add(new Paragraph(new Run("Накладна")));
            headerRow.Cells[8].Blocks.Clear();
            headerRow.Cells[8].Blocks.Add(new Paragraph(new Run("Примітка")));

            table.RowGroups[0].Rows.Add(headerRow);

            foreach (Order i in content)
            {
                TableRow dataRow = new TableRow();

                TableCell cell;

                foreach (TableColumn column in table.Columns)
                {
                    cell = new TableCell(new Paragraph(new Run(" ")))
                    {
                        Padding = new Thickness(5),
                        TextAlignment = TextAlignment.Left
                    };

                    cell.BorderBrush = Brushes.Black;
                    cell.BorderThickness = new Thickness(1);

                    dataRow.Cells.Add(cell);
                }

                dataRow.Cells[0].Blocks.Clear();
                dataRow.Cells[0].Blocks.Add(new Paragraph(new Run(i.ID.ToString())));
                dataRow.Cells[1].Blocks.Clear();
                dataRow.Cells[1].Blocks.Add(new Paragraph(new Run(i.Date.ToString())));
                dataRow.Cells[2].Blocks.Clear();
                dataRow.Cells[2].Blocks.Add(new Paragraph(new Run(i.Name)));
                dataRow.Cells[3].Blocks.Clear();
                dataRow.Cells[3].Blocks.Add(new Paragraph(new Run(i.Phone)));
                dataRow.Cells[4].Blocks.Clear();
                dataRow.Cells[4].Blocks.Add(new Paragraph(new Run(i.Products)));
                dataRow.Cells[5].Blocks.Clear();
                dataRow.Cells[5].Blocks.Add(new Paragraph(new Run(i.Address)));
                dataRow.Cells[6].Blocks.Clear();
                dataRow.Cells[6].Blocks.Add(new Paragraph(new Run(i.Sum.ToString())));
                dataRow.Cells[7].Blocks.Clear();
                dataRow.Cells[7].Blocks.Add(new Paragraph(new Run(i.Invoice)));
                dataRow.Cells[8].Blocks.Clear();
                dataRow.Cells[8].Blocks.Add(new Paragraph(new Run(i.Note)));

                table.RowGroups[0].Rows.Add(dataRow);
            }

            table.BorderBrush = Brushes.Black;
            table.BorderThickness = new Thickness(1);

            document.Blocks.Add(table);

            return document;
        }
    }
}