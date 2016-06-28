using BiWell.FreedomConnector;
using BiWell.FreedomConnector.Interfaces;
using BiWell.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace BiWell.UI.Forms
{
    public partial class MainForm : Form
    {
        private IConnector _connector = null;
        private List<Order> _orders = null;

        public MainForm()
        {
            InitializeComponent();

            _connector = new VirtualConnector();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _orders = _connector.GetOrders().OrderByDescending(o => o.Date).ToList();

            listViewOrder.BeginUpdate();
            listViewOrder.Items.Clear();

            foreach (var order in _orders)
            {
                ListViewItem lvi = new ListViewItem();
                SetOrderValues(lvi, order);

                listViewOrder.Items.Add(lvi);
            }
            listViewOrder.EndUpdate();
        }

        private void listViewOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            Order selected = SelectedOrder;
            if (selected != null)
            {
                ShowOrderLines(selected);

                deliveryToolStripMenuItem.Enabled = !selected.IsDelivery;
            }
        }

        private void deliveryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = SelectedOrderListViewItem;
            Order order = SelectedOrder;

            if (order != null)
            {
                FrmSetTrackingNumber dlg = new FrmSetTrackingNumber();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Delivery delivery = new Delivery();
                    delivery.TrackNum = dlg.TrackingNumber;
                    order.SetDelivery(delivery);

                    SetOrderValues(lvi, order);
                }
            }
        }

        private Order SelectedOrder
        {
            get
            {
                ListViewItem lvi = SelectedOrderListViewItem;
                if (lvi != null)
                {
                    return lvi.Tag as Order;
                }

                return null;
            }
        }

        private ListViewItem SelectedOrderListViewItem
        {
            get
            {
                if (listViewOrder.SelectedItems.Count > 0)
                {
                    return listViewOrder.SelectedItems[0];
                }

                return null;
            }
        }

        private void ShowOrderLines(Order order)
        {
            listViewOrderLine.BeginUpdate();
            listViewOrderLine.Items.Clear();

            foreach (var orderLine in order.Lines)
            {
                ListViewItem lvi = new ListViewItem(orderLine.LineNum.ToString());
                lvi.SubItems.Add(orderLine.Name);
                lvi.SubItems.Add(orderLine.Price.ToString());
                lvi.SubItems.Add(orderLine.Quantity.ToString());
                lvi.SubItems.Add(orderLine.Amount.ToString());
                lvi.Tag = orderLine;

                listViewOrderLine.Items.Add(lvi);
            }

            listViewOrderLine.EndUpdate();
        }

        private void SetOrderValues(ListViewItem lvi, Order order)
        {
            lvi.SubItems.Clear();

            lvi.Text = order.Number.ToString();
            lvi.SubItems.Add(order.Date.ToShortDateString());
            lvi.SubItems.Add(order.Amount.ToString());
            lvi.SubItems.Add(order.IsDelivery ? "Yes" : "No");
            if (order.IsDelivery)
            {
                lvi.SubItems.Add(order.Delivery.TrackNum);
            }
            lvi.Tag = order;
        }
    }
}
