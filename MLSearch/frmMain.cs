using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace MLSearch
{
    public partial class frmMain : Form
    {
        dynamic retorno;
        public frmMain()
        {
            InitializeComponent();
        }

        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string apiUrl = "https://api.mercadolibre.com";
                RestClient client = new RestClient(apiUrl);

                var request = new RestRequest("/sites/MLB/search?q=" + txtPesquisa.Text + "&limit=5000");
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;

                var response = client.Execute(request);

                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

                retorno = serializer.Deserialize(response.Content, typeof(object));

                lvProdutos.Items.Clear();
                for (int i = 0; i < retorno.results.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = retorno.results[i].title;
                    lvProdutos.Items.Add(lvi);
                }
               

            }
        }

        private void lvProdutos_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lvi = (ListView)sender;
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(retorno.results[lvi.FocusedItem.Index].thumbnail);
            MemoryStream ms = new MemoryStream(bytes);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

            pbProduto.Image = img;

            lblPreco.Text = Convert.ToString(retorno.results[lvi.FocusedItem.Index].price);
            lblVendedor.Text = retorno.results[lvi.FocusedItem.Index].seller.power_seller_status;
            lblurl.Text = retorno.results[lvi.FocusedItem.Index].permalink;

        }
    }
}
