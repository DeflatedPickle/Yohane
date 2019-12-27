namespace WpfApp
{
    public partial class MainWindow
    {
        class GridItem
        {
            public string Text { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
            public int RowSpan { get; set;  }
            public int ColumnSpan { get; set; }

            public GridItem(string text, int row, int column)
            {
                Text = text;
                Row = row;
                Column = column;
                RowSpan = 1;
                ColumnSpan = 1;
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();

            GridItems.ItemsSource = new GridItem[]
            {
                new GridItem("Beep", 0, 0) { ColumnSpan = 2},
                new GridItem("Boop", 0, 2),
            };
        }
    }
}