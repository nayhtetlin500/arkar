using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ParkingSystem
{
    public partial class MainWindow : Window
    {
        private string _vehicleType = "";
        private string _selectedSlot = "";
        private string _accentHex = "#00D4FF";

        // Slot data: key = slotName, value = ParkingRecord or null
        private Dictionary<string, ParkingRecord?> _carSlots = new();
        private Dictionary<string, ParkingRecord?> _bikeSlots = new();
        private Dictionary<string, ParkingRecord?> _motorSlots = new();

        private Dictionary<string, ParkingRecord?> CurrentSlots =>       //dicionary select based on vehicle type
            _vehicleType == "Car" ? _carSlots :
            _vehicleType == "Bicycle" ? _bikeSlots : _motorSlots;

        private decimal _totalRevenue = 0;
        private DispatcherTimer _clock = new();

        public MainWindow()
        {
            InitializeComponent();
            InitSlots();
            StartClock();
            UpdateSidebar();
        }

        void InitSlots()
        {
            for (int i = 1; i <= 10; i++) { _carSlots[$"C{i}"]  = null; }  //initialize 10 slots for each vehicle type with keys C1-C10 for cars, B1-B10 for bikes, M1-M10 for motorbikes, all set to null (free)
            for (int i = 1; i <= 10; i++) { _bikeSlots[$"B{i}"] = null; }
            for (int i = 1; i <= 10; i++) { _motorSlots[$"M{i}"]= null; }
        }

        void StartClock()
        {
            _clock.Interval = TimeSpan.FromSeconds(1);
            _clock.Tick += (s, e) => ClockText.Text = DateTime.Now.ToString("hh:mm:ss tt");
            _clock.Start();
            ClockText.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        // ─── Vehicle Type Selection ───────────────────────────────────────────
        void BtnCar_Click(object sender, RoutedEventArgs e)
        {
            SelectType("Car", "#00D4FF", BtnCar, BtnBike, BtnMotor);

            WelcomePanel.Visibility = Visibility.Visible;
            SlotPanel.Visibility = Visibility.Collapsed;
        }

        void BtnBike_Click(object sender, RoutedEventArgs e)
        {
            SelectType("Bicycle", "#FF6B6B", BtnBike, BtnCar, BtnMotor);

            WelcomePanel.Visibility = Visibility.Visible;
            SlotPanel.Visibility = Visibility.Collapsed;
        }

        void BtnMotor_Click(object sender, RoutedEventArgs e)
        {
            SelectType("Motorbike", "#FFB347", BtnMotor, BtnCar, BtnBike);

            WelcomePanel.Visibility = Visibility.Visible;
            SlotPanel.Visibility = Visibility.Collapsed;
        }
        void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            SlotPanel.Visibility = Visibility.Visible;

            _selectedSlot = "";
            RegisterForm.Visibility = Visibility.Collapsed;

            BuildSlotGrid();
        }


        void SelectType(string type, string hex, Button active, Button b2, Button b3)
        {
            _vehicleType = type;   
            _accentHex   = hex;     //color setting 
            _selectedSlot = "";

            var col = Parse(hex);
        //    PageTitle.Text     = $"{type.ToUpper()} PARKING";
        //    PageTitle.Foreground = new SolidColorBrush(col);
        //    PageSub.Text       = "Click a slot to check in or check out";

            Highlight(active, hex);     //buton highlight
            Reset(b2); Reset(b3);       //button reset

            WelcomePanel.Visibility = Visibility.Collapsed;
            SlotPanel.Visibility    = Visibility.Visible;
            RegisterForm.Visibility = Visibility.Collapsed;
            MsgBar.Visibility       = Visibility.Collapsed;

            BuildSlotGrid();
            UpdateSidebar();
        }

        void Highlight(Button b, string hex)
        {
            var c = Parse(hex);
            b.Background  = new SolidColorBrush(Color.FromArgb(28, c.R, c.G, c.B));
            b.BorderBrush = new SolidColorBrush(c);
        }
        void Reset(Button b)
        {
            b.Background  = new SolidColorBrush(Parse("#1A1F35"));
            b.BorderBrush = new SolidColorBrush(Parse("#2A3060"));
        }

        // ─── Build Slot Buttons ───────────────────────────────────────────────
        void BuildSlotGrid()
        {
            SlotGrid.Children.Clear();
            var accent = Parse(_accentHex);

            foreach (var kv in CurrentSlots)
            {
                bool occupied = kv.Value != null;
                bool selected = kv.Key == _selectedSlot;

                var btn = new Button { Style = (Style)Resources["SlotBtn"], Margin = new Thickness(6) };

                Color bg = selected ? Parse("#1A3A5A") :                            //selected slot
                           occupied ? Parse("#2A0F0F") : Parse("#0F2A1A");          //occupied slot // free slot
                Color border = selected ? accent :
                               occupied ? Parse("#FF4444") : Parse("#00FF88");

                btn.Background  = new SolidColorBrush(bg);
                btn.BorderBrush = new SolidColorBrush(border);
                btn.BorderThickness = new Thickness(selected ? 3 : 2);

                var panel = new StackPanel { Margin = new Thickness(10, 12, 10, 12) };

                // Slot name
                var lbl = new TextBlock
                {
                    Text = kv.Key,
                    FontSize = 22,
                    FontWeight = FontWeights.Black,
                    Foreground = new SolidColorBrush(selected ? accent : occupied ? Parse("#FF4444") : Parse("#00FF88")),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                panel.Children.Add(lbl);

                // Vehicle icon mini
                var icon = new TextBlock
                {
                    Text = occupied ? GetVehicleEmoji() : "—",
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 4),

                    Foreground = occupied
         ? new SolidColorBrush(GetVehicleColor())
         : Brushes.White
                };

                panel.Children.Add(icon);

                // Status or plate
                var status = new TextBlock
                {
                    Text = occupied ? kv.Value!.PlateNumber : "Available",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(occupied ? Parse("#FFAA44") : Parse("#2A5A2A")),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                panel.Children.Add(status);

                btn.Content = panel;
                string slotKey = kv.Key;
                btn.Click += (s, e) => SlotClicked(slotKey);
                SlotGrid.Children.Add(btn);
            }
        }

        string GetVehicleEmoji() => _vehicleType switch
        {
            "Car" => "🚗",
            "Bicycle" => "🚲",
            _ => "🏍"
        }; 
       

        Color GetVehicleColor() => _vehicleType switch
        {
            "Car" => Parse("#00D4FF"),      
            "Bicycle" => Parse("#FF6B6B"),  
            "Motorbike" => Parse("#FFB347"),
            _ => Parse("#FFFFFF")
        };

        // ─── Slot Clicked ─────────────────────────────────────────────────────
        void SlotClicked(string slotKey)
        {
            _selectedSlot = slotKey;
            SelectedSlotLabel.Text = slotKey;

            // Auto fill only when user clicks a slot
            TxtPlate.Text = slotKey;
            TxtPlate.IsReadOnly = false;

            BuildSlotGrid();

            bool occupied = CurrentSlots[slotKey] != null; 
            RegisterForm.Visibility = Visibility.Visible;

            if (occupied)
            {
                var rec = CurrentSlots[slotKey]!;     // non-null because occupied=> checkout button visible

                TxtPlate.Text = rec.PlateNumber;
                TxtPlate.IsReadOnly = true;

                BtnCheckOutForm.Visibility = Visibility.Visible;
            }
            else
            {
                BtnCheckOutForm.Visibility = Visibility.Collapsed;  //frree slot => check in form
            }
        }





        // Update slot label color to accent
        //  SelectedSlotLabel.Background = new SolidColorBrush(Parse(_accentHex));


        // ─── Check In ─────────────────────────────────────────────────────────
        void BtnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedSlot)) { ShowMsg("Please select a slot first!", "#FF4444", "#2A0F0F"); return; }  
            if (CurrentSlots[_selectedSlot] != null) { ShowMsg("This slot is already occupied!", "#FF4444", "#2A0F0F"); return; }     
            if (string.IsNullOrWhiteSpace(TxtPlate.Text)) { ShowMsg("Please enter plate number!", "#FF4444", "#2A0F0F"); return; }

            var rec = new ParkingRecord
            {
                SlotId      = _selectedSlot,
                VehicleType = _vehicleType,
                PlateNumber = TxtPlate.Text.Trim().ToUpper(),
                //  OwnerName   = string.IsNullOrWhiteSpace(TxtOwner.Text) ? "N/A" : TxtOwner.Text.Trim(),
                //   Phone       = string.IsNullOrWhiteSpace(TxtPhone.Text) ? "—" : TxtPhone.Text.Trim(),
                CheckInTime = DateTime.Now,
                CheckIn     = DateTime.Now.ToString("hh:mm tt"),
            };

            CurrentSlots[_selectedSlot] = rec;
            RegisterForm.Visibility      = Visibility.Collapsed;
            // Auto select first free slot
            var firstFree = CurrentSlots.FirstOrDefault(x => x.Value == null);  //find first free slot

            if (!string.IsNullOrEmpty(firstFree.Key))
            {
                _selectedSlot = firstFree.Key;
            }
            else
            {
                _selectedSlot = "";
            }
            BuildSlotGrid();
            UpdateSidebar();
            ShowMsg($"✔  {rec.PlateNumber} checked in to slot {rec.SlotId}", "#00FF88", "#0A2A0A");
        }

        // ─── Check Out → Payment Dialog ───────────────────────────────────────
        void BtnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedSlot) ||
                CurrentSlots[_selectedSlot] == null)
                return;

            var rec = CurrentSlots[_selectedSlot]!;

            TimeSpan dur = DateTime.Now - rec.CheckInTime;

            decimal fee = CalculateFee(
                rec.VehicleType,
                rec.CheckInTime,
                DateTime.Now);

            var payWin = new PaymentWindow(
                rec,
                FormatDur(dur),
                fee,
                _accentHex);

            payWin.Owner = this;

            bool? paid = payWin.ShowDialog();    

            if (paid == true)             
            {
                _totalRevenue += fee;
                CurrentSlots[_selectedSlot] = null;

                RegisterForm.Visibility = Visibility.Collapsed;
                _selectedSlot = "";

                BuildSlotGrid();
                UpdateSidebar();

                ShowMsg(
                    $"✔ Payment complete! ¥{fee:F0}",
                    "#00FF88",
                    "#0A2A0A");
            }
        }
        decimal CalculateFee(
    string vehicleType,
    DateTime checkIn,
    DateTime checkOut)
        {
            bool isDay =
                checkIn.TimeOfDay >= TimeSpan.FromHours(8) &&
                checkIn.TimeOfDay < TimeSpan.FromHours(20);

            if (vehicleType == "Car")
            {
                if (isDay)
                {
                    int blocks =
                        (int)Math.Ceiling(
                            (checkOut - checkIn).TotalMinutes / 30);  //45 min => 45/30 = 1.5 => 2 blocks=> 2*200=400 yen

                    return blocks * 200;
                }

                return 800;
            }

            if (vehicleType == "Bicycle")
            {
                if (isDay)
                {
                    int blocks =
                        (int)Math.Ceiling(
                            (checkOut - checkIn).TotalMinutes / 30); //45 min => 45/30 = 1.5 => 2 blocks=> 2*100=200 yen

                    return blocks * 100;
                }

                return 400;
            }

            // Motorbike
            if (isDay)
            {
                int blocks =
                    (int)Math.Ceiling(
                        (checkOut - checkIn).TotalMinutes / 30);  //45 min => 45/30 = 1.5 => 2 blocks=> 2*150=300 yen

                return blocks * 150;
            }

            return 600;
        }
        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _selectedSlot = "";
            RegisterForm.Visibility = Visibility.Collapsed;
            MsgBar.Visibility = Visibility.Collapsed;
            BuildSlotGrid();
        }

        void UpdateSidebar()
        {
            int carOcc = _carSlots.Values.Count(v => v != null);      //non-null value count = occupied slots
            int bikeOcc = _bikeSlots.Values.Count(v => v != null);
            int motorOcc = _motorSlots.Values.Count(v => v != null);

            CarSlotInfo.Text   = $"{10 - carOcc} Free / 10";        //3 occupied => 10-3 = 7 free
            BikeSlotInfo.Text  = $"{10 - bikeOcc} Free / 10";
            MotorSlotInfo.Text = $"{10 - motorOcc} Free / 10";
            TotalActive.Text   = (carOcc + bikeOcc + motorOcc).ToString();
            TotalRevenue.Text  = $"{_totalRevenue:F2}円";
        }

        void ShowMsg(string msg, string fg, string bg)
        {
            MsgBar.Visibility = Visibility.Visible;
            MsgBar.Background = new SolidColorBrush(Parse(bg));
            MsgText.Text      = msg;
            MsgText.Foreground= new SolidColorBrush(Parse(fg));

            var t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            t.Tick += (s, e) => { MsgBar.Visibility = Visibility.Collapsed; t.Stop(); };
            t.Start();
        }

        string FormatDur(TimeSpan ts) =>
            ts.TotalMinutes < 60 ? $"{(int)ts.TotalMinutes}m" : $"{(int)ts.TotalHours}h {ts.Minutes}m";

        static Color Parse(string hex) => (Color)ColorConverter.ConvertFromString(hex);
    }
}

    // ─── Data Model ──────────────────────────────────────────────────────────
    public class ParkingRecord
    {
        public string SlotId { get; set; } = "";
        public string VehicleType { get; set; } = "";
        public string PlateNumber { get; set; } = "";
   //     public string OwnerName { get; set; } = "";
  //      public string Phone { get; set; } = "";
        public DateTime CheckInTime { get; set; }
        public string CheckIn { get; set; } = "";
    }


