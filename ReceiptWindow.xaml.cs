using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ParkingSystem
{
    public partial class ReceiptWindow : Window
    {
        public ReceiptWindow(
            ParkingRecord rec,
            string duration,
            decimal fee,
            string paymentMethod)
        {
            InitializeComponent();

            FlowDocument doc =
                new FlowDocument();

            doc.FontFamily =
                new FontFamily("Consolas");

            doc.FontSize = 14;

            doc.PagePadding =
                new Thickness(20);


            Paragraph title =
                new Paragraph();

            title.TextAlignment =
                TextAlignment.Center;

            title.FontWeight =
                FontWeights.Bold;

            title.FontSize = 18;

            title.Inlines.Add(
                "PARKING RECEIPT");

            doc.Blocks.Add(title);

            doc.Blocks.Add(
                new Paragraph(
                    new Run("--------------------------------")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Date : {DateTime.Now}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Slot : {rec.SlotId}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Plate : {rec.PlateNumber}")));

         //   doc.Blocks.Add(
          //      new Paragraph(
          //          new Run($"Owner : {rec.OwnerName}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Vehicle : {rec.VehicleType}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Check In : {rec.CheckIn}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Duration : {duration}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Payment : {paymentMethod}")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run($"Fee : {fee:F2}円")));

            doc.Blocks.Add(
                new Paragraph(
                    new Run("--------------------------------")));

            Paragraph thanks =
                new Paragraph();

            thanks.TextAlignment =
                TextAlignment.Center;

            thanks.Inlines.Add(
                "Thank You!");

            doc.Blocks.Add(thanks);

            docViewer.Document = doc;
        }


        private void Print_Click(
            object sender,
            RoutedEventArgs e)
        {
            PrintDialog pd =
                new PrintDialog();

            if (pd.ShowDialog() == true)
            {
                pd.PrintDocument(
                    ((IDocumentPaginatorSource)
                    docViewer.Document)
                    .DocumentPaginator,

                    "Parking Receipt");
            }
        }
    }
}