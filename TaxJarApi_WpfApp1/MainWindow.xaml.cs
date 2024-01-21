using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Taxjar;


namespace TaxJarApi_WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        // Method to remove TLS security protocols from api request
        private static void RemoveSecurityProtocols()
        {
            var securityProtocolsToRemove = ConfigurationManager.AppSettings["SecurityProtocols.Remove"]
                ?.Split(',')
                .Select(o => o.Trim())
                .ToArray();

            if (securityProtocolsToRemove == null || securityProtocolsToRemove.Length == 0)
            {
                return;
            }

            foreach (var securityProtocolString in securityProtocolsToRemove)
            {
                SecurityProtocolType securityProtocolEnum;
                if (Enum.TryParse(securityProtocolString, out securityProtocolEnum))
                {
                    // removes security protocol using binary operation
                    ServicePointManager.SecurityProtocol &= ~securityProtocolEnum;
                }
            }
        }

        // Method adds tls security protocols to request
        private static void AddSecurityProtocols()
        {
            var securityProtocolsAdd = ConfigurationManager.AppSettings["SecurityProtocols.Add"]
                ?.Split(',')
                .Select(o => o.Trim())
                .ToArray();

            if (securityProtocolsAdd == null || securityProtocolsAdd.Length == 0)
            {
                return;
            }

            foreach (var securityProtocolString in securityProtocolsAdd)
            {
                SecurityProtocolType securityProtocolEnum;
                if (Enum.TryParse(securityProtocolString, out securityProtocolEnum))
                {
                    // adds security protocol using binary operation
                    ServicePointManager.SecurityProtocol |= securityProtocolEnum;
                }
            }
        }

        // The second button is designed to fail in order to show that lower TLS versions are not acceted by API.
        // When actviating the reverse TLS proxy (not associated with this project) the request should then be accepted by the API
        private void btnRun2_Click(object sender, RoutedEventArgs e)
        {
            var originalSecurityProtocol = ServicePointManager.SecurityProtocol;

            RemoveSecurityProtocols();
           // AddSecurityProtocols();

            var client = new TaxjarApi("");


            try
            {

                Console.WriteLine("Request using SSL3, this should fail. Press any key to start request.");
                var tax = client.TaxForOrder(new
                {
                    from_country = "US",
                    from_zip = "92093",
                    from_state = "CA",
                    from_city = "San Diego",
                    to_country = "US",
                    to_zip = "90002",
                    to_state = "CA",
                    to_city = "Los Angeles",
                    amount = 16.5,
                    shipping = 1.5,
                    line_items = new[] {
                        new {
                          quantity = 1,
                          unit_price = 15,
                          product_tax_code = "31000"
                        }
                      }
                });

                Console.WriteLine(tax.OrderTotalAmount);
                outputTextBox01.Text = tax.OrderTotalAmount.ToString();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        // The first button is the control variable, to demonstrate a working request
        private void btnRun1_Click(object sender, RoutedEventArgs e)
        {
            var originalSecurityProtocol = ServicePointManager.SecurityProtocol;

            var client = new TaxjarApi("");

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                var tax = client.TaxForOrder(new
                {
                    from_country = "US",
                    from_zip = "92093",
                    from_state = "CA",
                    from_city = "San Diego",
                    to_country = "US",
                    to_zip = "90002",
                    to_state = "CA",
                    to_city = "Los Angeles",
                    amount = 16.5,
                    shipping = 1.5,
                    line_items = new[] {
                        new {
                          quantity = 1,
                          unit_price = 15,
                          product_tax_code = "31000"
                        }
                      }
                });

                Console.WriteLine(tax.OrderTotalAmount);
                outputTextBox01.Text = tax.OrderTotalAmount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
