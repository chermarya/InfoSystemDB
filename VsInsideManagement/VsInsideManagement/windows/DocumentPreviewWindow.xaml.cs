using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VsInsideManagement.library;

namespace VsInsideManagement.windows
{
    public partial class DocumentPreviewWindow : Window
    {
        private List<Order> _content;
        private List<Product> _products;

        public DocumentPreviewWindow(List<Order> content)
        {
            _content = content;
            InitializeComponent();

            contentFrame.Width = 1150;
            contentFrame.Height = 550;
            MainWindow.Width = 1200;
            MainWindow.Height = 720;
            SaveBtn.Visibility = Visibility.Collapsed;

            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Name = "flowDocumentViewer";
            viewer.Margin = new Thickness(0);

            FlowDocument document = CreatePrintDocument();
            document.PageHeight = 758;
            document.PageWidth = 1130;
            viewer.Document = document;

            contentFrame.Content = viewer;
        }

        public DocumentPreviewWindow(List<Product> prods)
        {
            _products = prods;
            InitializeComponent();

            contentFrame.Width = 620;
            contentFrame.Height = 570;
            MainWindow.Width = 670;
            MainWindow.Height = 800;
            PrintBtn.Visibility = Visibility.Collapsed;

            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Name = "flowDocumentViewer";
            viewer.Margin = new Thickness(0);

            FlowDocument document = CreateStockDocument();
            document.PageHeight = 842;
            document.PageWidth = 595;
            viewer.Document = document;

            contentFrame.Content = viewer;
        }

        private FlowDocument CreateStockDocument()
        {
            FlowDocument document = new FlowDocument();

            document.PageWidth = 595;
            document.PageHeight = 842;

            document.ColumnWidth = document.PageWidth;

            Table table = new Table();

            table.Columns.Add(new TableColumn() { Width = new GridLength(500) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(50) });
            table.RowGroups.Add(new TableRowGroup());

            // Format the current date and time
            string currentDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            TableRow dateRow = new TableRow(); // Date row
            TableCell dateCell = new TableCell(new Paragraph(new Run("Дата формування документу: " + currentDate)))
            {
                ColumnSpan = 2, // Merge across both columns
                Background = Brushes.LightGray,
                TextAlignment = TextAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            dateRow.Cells.Add(dateCell);
            table.RowGroups[0].Rows.Add(dateRow);

            TableRow headerRow = new TableRow(); // Header row

            TableCell headerCell1 = new TableCell(new Paragraph(new Run("Товар")))
            {
                Background = Brushes.LightGray,
                TextAlignment = TextAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            TableCell headerCell2 = new TableCell(new Paragraph(new Run("Кількість")))
            {
                Background = Brushes.LightGray,
                TextAlignment = TextAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            headerRow.Cells.Add(headerCell1);
            headerRow.Cells.Add(headerCell2);
            table.RowGroups[0].Rows.Add(headerRow);

            foreach (Product i in _products)
            {
                TableRow dataRow = new TableRow();

                TableCell cell1 =
                    new TableCell(
                        new Paragraph(new Run($"{i.ProdType.title} {i.title} {i.Color.title} {i.Size.title}")))
                    {
                        Padding = new Thickness(5),
                        TextAlignment = TextAlignment.Left,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                TableCell cell2 = new TableCell(new Paragraph(new Run(i.quantity.ToString())))
                {
                    Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Left,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };

                dataRow.Cells.Add(cell1);
                dataRow.Cells.Add(cell2);

                if (i.quantity <= 5)
                {
                    dataRow.Background = Brushes.LightPink;
                }

                table.RowGroups[0].Rows.Add(dataRow);
            }

            table.BorderBrush = Brushes.Black;
            table.BorderThickness = new Thickness(1);

            document.Blocks.Add(table);

            return document;
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument document = CreatePrintDocument();

                document.PageWidth = printDialog.PrintableAreaWidth;

                IDocumentPaginatorSource paginatorSource = document;
                if (paginatorSource != null)
                {
                    printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Друк документа");
                }
            }

            MessageBox.Show("Process finished successfully.");
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            SaveToExcel(CreateStockDocument());
        }

        private FlowDocument CreatePrintDocument()
        {
            FlowDocument document = new FlowDocument();

            document.PageWidth = 1130;
            document.PageHeight = 758;

            document.ColumnWidth = document.PageWidth;

            Paragraph dateTimeParagraph =
                new Paragraph(new Run("Дата формування документу: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
            document.Blocks.Add(dateTimeParagraph);

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

            foreach (Order i in _content)
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

        public void SaveToExcel(FlowDocument flowDocument)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "xlsx";
                saveFileDialog.Title = "Save Excel File";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                        int rowIndex = 1;

                        foreach (Block block in flowDocument.Blocks)
                        {
                            if (block is Table)
                            {
                                Table table = (Table)block;

                                foreach (TableRowGroup rowGroup in table.RowGroups)
                                {
                                    foreach (TableRow row in rowGroup.Rows)
                                    {
                                        int columnIndex = 1;

                                        foreach (TableCell cell in row.Cells)
                                        {
                                            string cellValue = GetTableCellValue(cell);

                                            if (rowIndex == 1 && columnIndex == 1)
                                            {
                                                worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1]
                                                    .Merge = true; // Объединяем ячейки
                                            }

                                            worksheet.Cells[rowIndex, columnIndex].Value = cellValue;

                                            if (columnIndex == 2 && rowIndex > 1)
                                            {
                                                int quantity;
                                                if (int.TryParse(cellValue, out quantity))
                                                {
                                                    worksheet.Cells[rowIndex, columnIndex].Value = quantity;
                                                    worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format =
                                                        "#,##0";
                                                }

                                                if (quantity <= 5)
                                                {
                                                    worksheet.Cells[rowIndex, columnIndex - 1].Style.Fill.PatternType =
                                                        ExcelFillStyle.Solid;
                                                    worksheet.Cells[rowIndex, columnIndex - 1].Style.Fill
                                                        .BackgroundColor.Indexed = 29;

                                                    worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType =
                                                        ExcelFillStyle.Solid;
                                                    worksheet.Cells[rowIndex, columnIndex].Style.Fill.BackgroundColor
                                                        .Indexed = 29;
                                                }
                                            }

                                            if (rowIndex == 2)
                                            {
                                                try
                                                {
                                                    worksheet.Cells[rowIndex, columnIndex - 1].Style.Fill.PatternType =
                                                        ExcelFillStyle.Solid;
                                                    worksheet.Cells[rowIndex, columnIndex - 1].Style.Fill
                                                        .BackgroundColor.Indexed = 22;

                                                    worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType =
                                                        ExcelFillStyle.Solid;
                                                    worksheet.Cells[rowIndex, columnIndex].Style.Fill.BackgroundColor
                                                        .Indexed = 22;
                                                }
                                                catch
                                                {
                                                }
                                            }

                                            columnIndex++;
                                        }

                                        rowIndex++;
                                    }
                                }
                            }
                        }

                        excelPackage.SaveAs(new System.IO.FileInfo(filePath));
                        MessageBox.Show($"Excel file saved successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private static string GetTableCellValue(TableCell cell)
        {
            TextRange textRange = new TextRange(cell.Blocks.FirstBlock.ContentStart, cell.Blocks.FirstBlock.ContentEnd);
            return textRange.Text.Trim();
        }
    }
}