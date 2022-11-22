using TestCustomSettingsProvider.Properties;

namespace TestCustomSettingsProvider
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.WindowLocation;
            Size = Settings.Default.WindowSize;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowLocation = Location;
            Settings.Default.WindowSize = WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;
            Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Branch> branches = new()
            {
                new("041")
                {
                    Name = "EUROPART Danmark A/S",
                    StreetName = "Kokmose",
                    BuildingNumber = "14"
                },
                new("045")
                {
                    Name = "EUROPART Aalborg A/S",
                    StreetName = "Mineralvej",
                    BuildingNumber = "31"
                },
                new("043")
                {
                    Name = "EUROPART Brabrand A/S",
                    StreetName = "Dalgårdsvej",
                    BuildingNumber = "4"
                },
                new("156")
                {
                    Name = "EUROPART Herning A/S",
                    StreetName = "Taksvej",
                    BuildingNumber = "34"
                },
                new("042")
                {
                    Name = "EUROPART Padborg A/S",
                    StreetName = "Industrivej",
                    BuildingNumber = "9"
                },
                new("044")
                {
                    Name = "EUROPART Greve A/S",
                    StreetName = "Ventrupvej",
                    BuildingNumber = "26-28"
                }
            };
            foreach (Branch branch in branches)
            {
                Console.WriteLine("Number: " + branch.ID + " Name: " + branch.Name + " Address: " + branch.StreetName + " " + branch.BuildingNumber);
            }
            branches.Sort((x, y) => x.ID!.CompareTo(y.ID));
            foreach (Branch branch in branches)
            {
                Console.WriteLine("Number: " + branch.ID + " Name: " + branch.Name + " Address: " + branch.StreetName + " " + branch.BuildingNumber);
            }
            List<Branch> tmp = Settings.Default.DKBranches;
            if (tmp == null)
            {
                Console.WriteLine("Branch settings null");
            }
            else
            {
                Console.Write("Branch settings is not null");
            }
            Settings.Default.DKBranches = branches;
            Settings.Default.Save();
        }
    }
}