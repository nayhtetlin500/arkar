using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ParkingSystem
{
    public partial class PaymentWindow : Window
    {
        private string _selectedMethod = "";
        private readonly decimal _fee;
        private readonly string _accentHex;
        private readonly ParkingRecord _record;
        private readonly string _duration;

        public PaymentWindow(
            ParkingRecord rec,
            string duration,
            decimal fee,
            string accentHex)
        {
            InitializeComponent();

            _record = rec;
            _duration = duration;
            _fee = fee;
            _accentHex = accentHex;

            SlotLabel.Text = $"Slot: {rec.SlotId}";
            PlateLabel.Text = $"Plate: {rec.PlateNumber}";
          //  OwnerLabel.Text = $"Owner: {rec.OwnerName}";
            TypeLabel.Text = $"Type: {rec.VehicleType}";
            CheckInLabel.Text = $"In: {rec.CheckIn}";
            DurLabel.Text = $"Duration: {duration}";
            FeeLabel.Text = $"{fee:F2}円";
        }

        void SelectCash(object sender, RoutedEventArgs e)
            => SelectMethod("Cash", BtnCash, "#00FF88");

        void SelectCard(object sender, RoutedEventArgs e)
            => SelectMethod("Card", BtnCard, "#8B5CF6");

        void SelectQR(object sender, RoutedEventArgs e)
            => SelectMethod("QR", BtnQR, "#00D4FF");


        void SelectMethod(
            string method,
            Button active,
            string hex)
        {
            _selectedMethod = method;

            var col = (Color)ColorConverter.ConvertFromString(hex);
            var dim = (Color)ColorConverter.ConvertFromString("#2A3060");

            foreach (var btn in new[]
            {
                BtnCash,
                BtnCard,
                BtnQR
            })
            {
                btn.Background =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#0D1228"));

                btn.BorderBrush =
                    new SolidColorBrush(dim);

                btn.BorderThickness =
                    new Thickness(2);
            }

            active.Background =
                new SolidColorBrush(
                    Color.FromArgb(
                        25,
                        col.R,
                        col.G,
                        col.B));

            active.BorderBrush =
                new SolidColorBrush(col);

            active.BorderThickness =
                new Thickness(3);

            BtnConfirm.Background =
                new SolidColorBrush(col);
        }


        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMethod))
            {
                MessageBox.Show(
                    "Please select a payment method!",
                    "Payment",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                "Payment successful!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Open receipt window
            ReceiptWindow receipt =
                new ReceiptWindow(
                    _record,
                    _duration,
                    _fee,
                    _selectedMethod);

            receipt.ShowDialog();

            DialogResult = true;
            Close();
        }

        void BtnCancelPay_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}